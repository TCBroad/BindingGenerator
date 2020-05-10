namespace BindingGenerator.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    internal class Generator
    {
        private readonly string inputPath;
        private readonly string outputPath;

        public string ExcludePattern { get; set; }

        public Generator(string inputPath, string outputPath)
        {
            this.inputPath = inputPath;
            this.outputPath = outputPath;
        }

        public List<AssemblyDefinition> GetBindings()
        {
            Console.WriteLine("Reading directory {0}", this.inputPath);
            var files = Directory.GetFiles(this.inputPath, "*.dll", SearchOption.TopDirectoryOnly);

            var assemblies = new List<AssemblyDefinition>();
            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFile(file);

                    var name = assembly.GetName();

                    var bytes = name.GetPublicKeyToken();
                    if (bytes == null || bytes.Length == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Could not read public key token for dll {0}", file);
                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;
                    }

                    var publicKeyToken = string.Empty;
                    for (var i = 0; i < bytes.GetLength(0); i++)
                    {
                        publicKeyToken += $"{bytes[i]:x2}";
                    }

                    var definition = new AssemblyDefinition
                    {
                        Culture = string.IsNullOrWhiteSpace(name.CultureName) ? "neutral" : name.CultureName,
                        Name = name.Name,
                        Version = name.Version?.ToString(),
                        PublicKeyToken = publicKeyToken
                    };

                    assemblies.Add(definition);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Could not load dll {0}", file);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            return assemblies;
        }

        public string GetXml(List<AssemblyDefinition> assemblies)
        {
            Console.WriteLine("Generating binding definitions...");

            if (!string.IsNullOrEmpty(this.ExcludePattern))
            {
                assemblies.RemoveAll(x => x.Name.Contains(this.ExcludePattern));
            }

            var bindingSection = new XElement("assemblyBinding", assemblies
                .Select(x => new XElement("dependentAssembly",
                    new XElement("assemblyIdentity", new XAttribute("name", x.Name), new XAttribute("publicKeyToken", x.PublicKeyToken), new XAttribute("culture", x.Culture)),
                    new XElement("bindingRedirect", new XAttribute("oldVersion", "0.0.0.0-" + x.Version), new XAttribute("newVersion", x.Version)))));

            var xml = bindingSection.ToString();

            // cba fucking about with namespaces
            return xml.Replace("<assemblyBinding>", "<assemblyBinding xmlns=\"urn:schemas-microsoft-com:asm.v1\">");
        }

        public void SaveXml(string xml)
        {
            Console.WriteLine("Writing to {0}", this.outputPath);

            File.WriteAllText(this.outputPath, xml);
        }
    }

    internal class AssemblyDefinition
    {
        public string Name { get; set; }

        public string Culture { get; set; }

        public string Version { get; set; }

        public string PublicKeyToken { get; set; }
    }
}