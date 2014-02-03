using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NSBEndpointAndMessageDetection
{
    public class Program
    {
        private static readonly IList<Assembly> Assemblies = new List<Assembly>();

        public static void Main(string[] args)
        {
            var scanner = new NSBAssemblyScanner();

            var results = scanner.Scan(args[0]);
                
            if (results.Any())
            {
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            else
            {
                Console.WriteLine("No NServiceBus Directives found");
            }
        }
    }

    public class NSBAssemblyScanner
    {
        public IList<string> Scan(string assemblyDirectory)
        {
            var assemblies = LoadAllAssembliesInDirectory(assemblyDirectory);
            var results = GetHandlers(assemblies);
            var assemblyNames = assemblies.Select(a => a.GetName().Name).ToList();
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

        private List<string> GetHandlers(IList<Assembly> assemblies)
        {
            var results = new List<string>();

            var handlerTypes = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t.GetInterfaces().Any(i => i.FullName.StartsWith("NServiceBus.IHandleMessages") || i.FullName.StartsWith("NServiceBus.IAmStartedByMessages")))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                results.Add(string.Format("Type: {0} Handles Messages.", handlerType.FullName));
                results.AddRange(GetMessageTypes(handlerType).Select(m => string.Format("Message: {0} Is handled by Type: {1}", m.FullName, handlerType.FullName)));
            }

            return results;
        }

        private IList<string> GetSenders(string assemblyDirectory, IList<string> assemblyNames)
        {
            var results = new List<string>();

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(assemblyDirectory);

            var assemblyDefinitions = assemblyNames.Select(resolver.Resolve).ToList();

            foreach (var typeDefinition in assemblyDefinitions.SelectMany(d => d.MainModule.Types))
            {
                var instructions = typeDefinition.Methods
                    .Where(m => m.Body != null)
                    .SelectMany(m => m.Body.Instructions)
                    .Where(i => i.Operand != null)
                    .Where(i => i.Operand.GetType() == typeof(MethodReference))
                    .Where(m => ((MethodReference)m.Operand).DeclaringType.FullName == "NServiceBus.IBus")
                    .ToList();

                if (!instructions.Any()) continue;

                results.Add(string.Format("Type: \"{0}\" Sends Messages", typeDefinition.FullName));
                foreach (var instruction in instructions)
                {
                    var parameter = GetParameterType(instruction);
                    results.Add(string.Format("Message: \"{0}\" was \"{1}\"", parameter.DeclaringType.FullName, ((MethodReference)instruction.Operand).Name));
                }
            }

            return results;
        }

        private IList<Type> GetMessageTypes(Type handlerType)
        {
            return handlerType.GetInterfaces()
                .Where(i => i.FullName.StartsWith("NServiceBus.IHandleMessages") || i.FullName.StartsWith("NServiceBus.IAmStartedByMessages"))
                .Select(handlingInterfaces => handlingInterfaces.GetGenericArguments()[0])
                .ToList();
        }
        
        private MemberReference GetParameterType(Instruction instruction)
        {
            return instruction.Previous.OpCode.Name != "newobj"
                ? GetParameterType(instruction.Previous)
                : instruction.Previous.Operand as MemberReference;
        }
    }
}
