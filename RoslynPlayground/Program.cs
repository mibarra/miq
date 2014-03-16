using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            ISolution solution = Solution.Load(@"C:\Users\Miguel\Desktop\Tfs\Miq\MiqLibrary\Dev\Miq.sln");
            var sieve = solution
                            .Projects.First(project => project.Name == "Miq")
                            .Documents.First(document => document.Name == "Sieve.cs");
            var syntax = (SyntaxNode)sieve.GetSyntaxRoot();
            var y = new x();
            y.Visit(syntax);
            Console.ReadLine();
        }
    }

    public class x : SyntaxWalker
    {
        public override void Visit(SyntaxNode node)
        {
            int level = node.Ancestors().Count();
            var typeName = node.GetType().ToString().Replace("Roslyn.Compilers.CSharp.", "");
            if (typeName.EndsWith("Syntax"))
            {
                typeName = typeName.Substring(0, typeName.Length - 6);
            }
            string line = new String(' ', level) + typeName;
            Console.WriteLine(line);
            base.Visit(node);
        }
    }
}
