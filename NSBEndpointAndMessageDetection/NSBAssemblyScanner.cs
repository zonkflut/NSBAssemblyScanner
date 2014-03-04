using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NSBEndpointAndMessageDetection
{
    public class NsbAssemblyScanner
    {
        private readonly string[] ignoredAssemblyNames = { "NServiceBus.Core", "NServiceBus" };

        public IList<IInstance> Scan(string assemblyDirectory)
        {
            var assemblies = LoadAllAssembliesInDirectory(assemblyDirectory);
            var assemblyNames = assemblies.Select(a => a.GetName().Name).ToList();
            var results = new List<IInstance>();
            results.AddRange(GetHandlers(assemblies));
            results.AddRange(GetSenders(assemblyDirectory, assemblyNames));
            return results;
        }

        private static IList<Assembly> LoadAllAssembliesInDirectory(string assemblyDirectory)
        {
            var assemblies = new List<Assembly>();
            foreach (var file in Directory.GetFiles(assemblyDirectory))
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(file));
                }
                catch
                {
                    // NOOP 
                }
            }

            return assemblies;
        }

        private List<Handler> GetHandlers(IList<Assembly> assemblies)
        {
            var results = new List<Handler>();

            foreach (var assembly in assemblies.Where(a => !ignoredAssemblyNames.Contains(a.GetName().Name)))
            {
                try
                {
                    var handlerTypes = assembly.GetExportedTypes();

                    foreach (var handlerType in handlerTypes)
                    {
                        var messageTypes = GetMessageTypes(handlerType);
                        if (!messageTypes.Any()) continue;

                        results.Add(new Handler
                        {
                            AssemblyName = assembly.FullName,
                            Name = handlerType.FullName,
                            Messages = messageTypes
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error occurred during GetHandlers for assembly: {0}\r\n{1}", assembly.FullName, e);
                }
            }

            return results;
        }

        private IList<Sender> GetSenders(string assemblyDirectory, IList<string> assemblyNames)
        {
            var results = new List<Sender>();

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(assemblyDirectory);

            var assemblyDefinitions = assemblyNames.Select(resolver.Resolve).ToList();

            foreach (var assemblyDefinition in assemblyDefinitions.Where(a => !ignoredAssemblyNames.Contains(a.Name.Name)))
            {
                try
                {
                    foreach (var typeDefinition in assemblyDefinition.MainModule.Types)
                    {
                        var hasInstructions = typeDefinition.Methods
                            .Where(m => m.Body != null)
                            .SelectMany(m => m.Body.Instructions)
                            .Where(i => i.Operand != null)
                            .Where(i => i.Operand.GetType() == typeof(MethodReference) || i.Operand.GetType() == typeof(GenericInstanceMethod))
                            .Where(m => (((MethodReference)m.Operand).DeclaringType.FullName == "NServiceBus.IBus") ||
                                        (((MethodReference)m.Operand).DeclaringType.FullName.StartsWith("NServiceBus.Saga.Saga") && ((MethodReference)m.Operand).GetElementMethod().Name == "RequestTimeout"))
                            .Any();


                        if (!hasInstructions) continue;

                        var result = new Sender
                        {
                            AssemblyName = assemblyDefinition.FullName,
                            Name = typeDefinition.FullName,
                        };

                        results.Add(result);

                        foreach (var methodDefinition in typeDefinition.Methods.Where(m => m.Body != null))
                        {
                            try
                            {
                                var instructions = methodDefinition.Body.Instructions
                                    .Where(i => i.Operand != null)
                                    .Where(i => i.Operand.GetType() == typeof(MethodReference) || i.Operand.GetType() == typeof(GenericInstanceMethod))
                                    .Where(m => (((MethodReference)m.Operand).DeclaringType.FullName == "NServiceBus.IBus") || 
                                                (((MethodReference)m.Operand).DeclaringType.FullName.StartsWith("NServiceBus.Saga.Saga") && ((MethodReference)m.Operand).GetElementMethod().Name == "RequestTimeout"))
                                    .ToList();

                                foreach (var instruction in instructions)
                                {
                                    var parameter = GetParameterType(methodDefinition, instruction);
                                    result.Messages.Add(new Message
                                    {
                                        Name = parameter ?? "Unknown Message Type",
                                        Operation = ((MethodReference)instruction.Operand).Name
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine("Error Occurred during GetSenders for Type Definition: {0}\r\n{1}", typeDefinition.FullName, e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error Occurred during GetSenders for Assembly Definition: {0}\r\n{1}", assemblyDefinition.FullName, e);
                }
            }

            return results;
        }

        private IList<Message> GetMessageTypes(Type handlerType)
        {
            return handlerType.GetInterfaces()
                .Where(i => i.FullName.StartsWith("NServiceBus.IAmStartedByMessages") || 
                            i.FullName.StartsWith("NServiceBus.IHandleMessages") || 
                            i.FullName.StartsWith("NServiceBus.Saga.IHandleTimeouts"))
                .Select(handlingInterface => new Message
                {
                    Name = handlingInterface.GetGenericArguments()[0].FullName, 
                    Operation = handlingInterface.Name.Replace("`1", string.Empty)
                })
                .ToList();
        }
        
        private string GetParameterType(MethodDefinition methodDefinition, Instruction instruction)
        {
            if (instruction.Operand as GenericInstanceMethod != null) 
                return (instruction.Operand as GenericInstanceMethod).GenericArguments[0].FullName;

            var operationCode = instruction.Previous.OpCode.Name;
            if (instruction.Previous != null && operationCode == "newobj")
            {
                return instruction.Previous != null && (instruction.Previous.Operand as MemberReference) != null
                    ? ((MemberReference)instruction.Previous.Operand).DeclaringType.FullName
                    : null;
            }

            if (instruction.Previous != null && operationCode.StartsWith("ldloc."))
            {
                var corrInst = GetCorrespondingInstruction(instruction.Previous, "stloc." + operationCode.Replace("ldloc.", string.Empty));

                if (corrInst.Previous != null && corrInst.Previous.OpCode.Name == "newobj")
                {
                    return corrInst.Previous != null && (corrInst.Previous.Operand as MemberReference) != null
                        ? ((MemberReference)corrInst.Previous.Operand).DeclaringType.FullName
                        : null;
                }
                else if (corrInst.Previous != null && corrInst.Previous.OpCode.Name == "call")
                {
                    return ((MethodReference)corrInst.Previous.Operand).ReturnType.FullName;
                }
                else
                    return null;
            }

            if (instruction.Previous != null && operationCode.StartsWith("ldarg."))
            {
                var parameterIndex = int.Parse(operationCode.Replace("ldarg.", string.Empty)) - 1;
                var parameter = methodDefinition.Parameters[parameterIndex];
                return parameter.ParameterType.FullName;
            }

            return null;
        }

        private Instruction GetCorrespondingInstruction(Instruction instruction, string operationCodeName)
        {
            return instruction == null || instruction.OpCode.Name == operationCodeName
                ? instruction
                : GetCorrespondingInstruction(instruction.Previous, operationCodeName);
        }
    }
}