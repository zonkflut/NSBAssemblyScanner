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

            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetExportedTypes()
                   .Where(t => t.GetInterfaces().Any(i => i.FullName.StartsWith("NServiceBus.IHandleMessages") || i.FullName.StartsWith("NServiceBus.IAmStartedByMessages")))
                   .ToList();

                foreach (var handlerType in handlerTypes)
                {
                    results.Add(new Handler
                    {
                        AssemblyName = assembly.FullName,
                        Name = handlerType.FullName,
                        Messages = GetMessageTypes(handlerType)
                            .Select(m => new Message { Name = m.FullName })
                            .ToList()
                    });
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

            foreach (var assemblyDefinition in assemblyDefinitions)
            {
                foreach (var typeDefinition in assemblyDefinition.MainModule.Types)
                {
                    var instructions = typeDefinition.Methods
                        .Where(m => m.Body != null)
                        .SelectMany(m => m.Body.Instructions)
                        .Where(i => i.Operand != null)
                        .Where(i => i.Operand.GetType() == typeof(MethodReference))
                        .Where(m => ((MethodReference)m.Operand).DeclaringType.FullName == "NServiceBus.IBus")
                        .ToList();

                    if (!instructions.Any()) continue;

                    var result = new Sender
                    {
                        AssemblyName = assemblyDefinition.FullName,
                        Name = typeDefinition.FullName,
                    };
                    
                    results.Add(result);

                    foreach (var instruction in instructions)
                    {
                        var parameter = GetParameterType(instruction);
                        result.Messages.Add(new Message { Name = parameter.DeclaringType.FullName, Operation = ((MethodReference)instruction.Operand).Name });
                    }
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

    public class Sender : IInstance
    {
        public Sender()
        {
            Messages = new List<Message>();
        }

        public string AssemblyName { get; set; }

        public string Name { get; set; }

        public IList<Message> Messages { get; set; }
    }

    public interface IInstance
    {
        string AssemblyName { get; set; }

        string Name { get; set; }

        IList<Message> Messages { get; set; }
    }

    public class Message
    {
        public string Name { get; set; }

        public string Operation { get; set; }
    }

    public class Handler : IInstance
    {
        public Handler()
        {
            Messages = new List<Message>();
        }

        public string AssemblyName { get; set; }

        public string Name { get; set; }

        public IList<Message> Messages { get; set; }
    }
}
