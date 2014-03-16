using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace Miq.Tests.Nursery
{
    public class x : SyntaxWalker
    {
        public override void VisitIfStatement(IfStatementSyntax node)
        {
            cyclo++;
            base.VisitIfStatement(node);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            cyclo++;
            base.VisitWhileStatement(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            cyclo++;
            base.VisitForEachStatement(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            cyclo++;
            base.VisitForStatement(node);
        }

        public override void VisitSwitchLabel(SwitchLabelSyntax node)
        {
            cyclo++;
            base.VisitSwitchLabel(node);
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            cyclo++;
            base.VisitCatchClause(node);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            SyntaxKind[] tokens = new SyntaxKind[] { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression, SyntaxKind.QuestionQuestionToken };
            if (tokens.Contains(node.OperatorToken.Kind))
            {
                cyclo++;

            }
            base.VisitBinaryExpression(node);
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            cyclo++;
            base.VisitConditionalExpression(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            cyclo = 1;
            base.VisitMethodDeclaration(node);
            Console.WriteLine(node.Identifier.ValueText, " ", cyclo);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            cyclo = 1;
            base.VisitConstructorDeclaration(node);
            Console.WriteLine(node.Identifier.ValueText, " ", cyclo);
        }

        private int cyclo;
    }

    [TestClass]
    public class RoslynTests
    {
        [TestMethod]
        [Ignore]
        public void TestMethod1()
        {
            ISolution solution = Solution.Load(@"C:\Users\Miguel\Desktop\Tfs\Miq\MiqLibrary\Dev\Miq.sln");
            var sieve = solution
                            .Projects.First(project => project.Name == "Miq")
                            .Documents.First(document => document.Name == "Sieve.cs");
            var syntax = (CompilationUnitSyntax)sieve.GetSyntaxRoot();
            var y = new x();
            y.Visit(syntax);
        }
    }
}
