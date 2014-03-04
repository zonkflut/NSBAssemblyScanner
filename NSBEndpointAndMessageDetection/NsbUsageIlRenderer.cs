using System.IO;
using System.Text;
using Mono.Cecil;

namespace NSBEndpointAndMessageDetection
{
    public class NsbUsageIlRenderer
    {
        public string RenderUsages(string assemblyPath)
        {
            var render = new StringBuilder();
            var assemblyFile = new FileInfo(assemblyPath);
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(assemblyFile.Directory.FullName);
            var assemblyDefinitions = resolver.Resolve(assemblyFile.Name.Substring(0, assemblyFile.Name.Length - assemblyFile.Extension.Length));
            foreach (var typeDefinition in assemblyDefinitions.MainModule.Types)
            {
                render.AppendFormat("Type: {0}\r\n", typeDefinition.FullName);
                render.AppendLine("============================================");
                foreach (var methodDefinition in typeDefinition.Methods)
                {
                    render.AppendFormat("Method: {0}\r\n", methodDefinition.FullName);
                    render.AppendLine("---------------------");
                    foreach (var instruction in methodDefinition.Body.Instructions)
                    {
                        render.AppendLine(string.Format("{0} {1} {2}", instruction.OpCode.Name, instruction.Operand, instruction.Offset));
                    }

                    render.AppendLine("---------------------");
                }

                render.AppendLine("===========================================");
            }

            return render.ToString();
        }
    }
}