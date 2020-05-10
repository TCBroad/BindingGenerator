namespace BindingGenerator.Console
{
    using System;

    internal class Program
    {
        internal static void Main(string[] args)
        {
            Console.WriteLine("Binding Generator - because binding redirects can suck a dick\n");

            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("BindingGenerator <bin dir> <output> [exclude pattern]");

                return;
            }

            var generator = new Generator(args[0], args[1]);
            if (args.Length > 2)
            {
                generator.ExcludePattern = args[2];
            }

            var assemblies = generator.GetBindings();

            Console.WriteLine("Found {0} assemblies", assemblies.Count);

            var xml = generator.GetXml(assemblies);

            generator.SaveXml(xml);

            Console.WriteLine("\nDone!");
        }
    }
}
