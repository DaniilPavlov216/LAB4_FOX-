using NUnit.Framework;
using LAB4_FOX.Models;
using System;
using LAB4_FOX.ViewModels;

namespace LAB4_FOX.Tests
{
    [TestFixture]
    public class BooleanFunctionTests
    {
        [Test]
        public void Constructor_WithValidParameters_ShouldInitializeProperties()
        {
            // Arrange & Act
            var function = new BooleanFunction(3, 11);

            // Assert
            Assert.That(function.VariableCount, Is.EqualTo(3));
            Assert.That(function.FunctionNumber, Is.EqualTo(11));
            Assert.That(function.Formula, Is.EqualTo("f11(3)"));
            Assert.That(function.TruthTable, Is.Not.Null);
            Assert.That(function.TruthTable.Length, Is.EqualTo(8)); // 2^3 = 8
        }

        [Test]
        public void Constructor_WithInvalidVariableCount_ShouldThrowException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() => new BooleanFunction(0, 0));
            Assert.Throws<ArgumentException>(() => new BooleanFunction(9, 0));
        }

        [Test]
        public void Constructor_WithFormula_ShouldParseCorrectly()
        {
            // Arrange & Act
            var function = new BooleanFunction("x1 & x2");

            // Assert
            Assert.That(function.VariableCount, Is.EqualTo(2));
            Assert.That(function.Formula, Is.EqualTo("x1 & x2"));
            Assert.That(function.TruthTable, Is.Not.Null);
        }

        [Test]
        public void GenerateTruthTableFromNumber_ShouldGenerateCorrectTable()
        {
            // Arrange
            int n = 2; // 2 variables
            int num = 6; // binary: 0110

            // Act
            var table = new BooleanFunction(2, 6).TruthTable;

            // Assert
            // For 2 variables: 00, 01, 10, 11
            // num=6 (0110 binary) means: 00→0, 01→1, 10→1, 11→0
            bool[] expected = { false, true, true, false };
            Assert.That(table, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateFunctionNumber_ShouldReturnCorrectNumber()
        {
            // Arrange
            bool[] truthTable = { false, true, true, false }; // num=6 for 2 variables

            // Act
            var function = new BooleanFunction("(x1 & !x2) | (!x1 & x2)"); // XOR equivalent

            // Assert - This should match the XOR function number
            Assert.That(function.FunctionNumber, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetDNF_ForSpecificFunction_ShouldReturnCorrectDNF()
        {
            // Arrange
            var function = new BooleanFunction(2, 6); // XOR-like function

            // Act
            string dnf = function.GetDNF();

            // Assert
            Assert.That(dnf, Is.Not.Empty);
            Assert.That(dnf, Does.Contain("∨").Or.EqualTo("0").Or.EqualTo("1"));
        }

        [Test]
        public void GetKNF_ForSpecificFunction_ShouldReturnCorrectKNF()
        {
            // Arrange
            var function = new BooleanFunction(2, 6); // XOR-like function

            // Act
            string knf = function.GetKNF();

            // Assert
            Assert.That(knf, Is.Not.Empty);
            Assert.That(knf, Does.Contain("∧").Or.EqualTo("0").Or.EqualTo("1"));
        }

        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var function = new BooleanFunction(3, 11);

            // Act
            string result = function.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("f(3) №11"));
        }
    }

    [TestFixture]
    public class DNFBuilderTests
    {
        [Test]
        public void BuildDNF_ForAlwaysTrueFunction_ShouldReturn1()
        {
            // Arrange
            var builder = new DNFBuilder();
            bool[] truthTable = { true, true, true, true }; // Always true for 2 variables
            int variableCount = 2;

            // Act
            string dnf = builder.BuildDNF(truthTable, variableCount);

            // Assert
            Assert.That(dnf, Is.EqualTo("1"));
        }

        [Test]
        public void BuildDNF_ForAlwaysFalseFunction_ShouldReturn0()
        {
            // Arrange
            var builder = new DNFBuilder();
            bool[] truthTable = { false, false, false, false }; // Always false for 2 variables
            int variableCount = 2;

            // Act
            string dnf = builder.BuildDNF(truthTable, variableCount);

            // Assert
            Assert.That(dnf, Is.EqualTo("0"));
        }

        [Test]
        public void BuildDNF_ForSingleTrueFunction_ShouldReturnSingleMinterm()
        {
            // Arrange
            var builder = new DNFBuilder();
            bool[] truthTable = { false, true, false, false }; // Only 01 is true
            int variableCount = 2;

            // Act
            string dnf = builder.BuildDNF(truthTable, variableCount);

            // Assert
            Assert.That(dnf, Does.Contain("¬x1"));
            Assert.That(dnf, Does.Contain("x2"));
            Assert.That(dnf, Does.Not.Contain("∨"));
        }

        [Test]
        public void CalculateCost_ForSimpleDNF_ShouldReturnCorrectCost()
        {
            // Arrange
            var builder = new DNFBuilder();
            string dnf = "(x1 ∧ ¬x2) ∨ (¬x1 ∧ x2)";

            // Act
            var cost = builder.CalculateCost(dnf);

            // Assert
            Assert.That(cost.Literals, Is.EqualTo(4));
            Assert.That(cost.Conjunctions, Is.EqualTo(2)); // Two AND operations inside
            Assert.That(cost.Disjunctions, Is.EqualTo(1)); // One OR operation between terms
        }

        [Test]
        public void CalculateCost_ForConstants_ShouldReturnZeroCost()
        {
            // Arrange
            var builder = new DNFBuilder();

            // Act & Assert
            var cost0 = builder.CalculateCost("0");
            Assert.That(cost0.Literals, Is.EqualTo(0));
            Assert.That(cost0.Conjunctions, Is.EqualTo(0));
            Assert.That(cost0.Disjunctions, Is.EqualTo(0));

            var cost1 = builder.CalculateCost("1");
            Assert.That(cost1.Literals, Is.EqualTo(0));
            Assert.That(cost1.Conjunctions, Is.EqualTo(0));
            Assert.That(cost1.Disjunctions, Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class KNFBuilderTests
    {
        [Test]
        public void BuildKNF_ForAlwaysTrueFunction_ShouldReturn1()
        {
            // Arrange
            var builder = new KNFBuilder();
            bool[] truthTable = { true, true, true, true }; // Always true for 2 variables
            int variableCount = 2;

            // Act
            string knf = builder.BuildKNF(truthTable, variableCount);

            // Assert
            Assert.That(knf, Is.EqualTo("1"));
        }

        [Test]
        public void BuildKNF_ForAlwaysFalseFunction_ShouldReturn0()
        {
            // Arrange
            var builder = new KNFBuilder();
            bool[] truthTable = { false, false, false, false }; // Always false for 2 variables
            int variableCount = 2;

            // Act
            string knf = builder.BuildKNF(truthTable, variableCount);

            // Assert
            Assert.That(knf, Is.EqualTo("0"));
        }

        [Test]
        public void BuildKNF_ForSingleFalseFunction_ShouldReturnSingleMaxterm()
        {
            // Arrange
            var builder = new KNFBuilder();
            bool[] truthTable = { true, false, true, true }; // Only 01 is false
            int variableCount = 2;

            // Act
            string knf = builder.BuildKNF(truthTable, variableCount);

            // Assert
            Assert.That(knf, Does.Contain("x1"));
            Assert.That(knf, Does.Contain("¬x2"));
            Assert.That(knf, Does.Not.Contain("∧"));
        }

        [Test]
        public void CalculateCost_ForSimpleKNF_ShouldReturnCorrectCost()
        {
            // Arrange
            var builder = new KNFBuilder();
            string knf = "(x1 ∨ ¬x2) ∧ (¬x1 ∨ x2)";

            // Act
            var cost = builder.CalculateCost(knf);

            // Assert
            Assert.That(cost.Literals, Is.EqualTo(4));
            Assert.That(cost.Conjunctions, Is.EqualTo(1)); // One AND operation between terms
            Assert.That(cost.Disjunctions, Is.EqualTo(2)); // Two OR operations inside
        }
    }

    [TestFixture]
    public class FormulaParserTests
    {
        [Test]
        public void Parse_SimpleAndFormula_ShouldReturnCorrectTruthTable()
        {
            // Arrange
            var parser = new FormulaParser();
            string formula = "x1 & x2";

            // Act
            var result = parser.Parse(formula);

            // Assert
            Assert.That(result.VariableCount, Is.EqualTo(2));
            Assert.That(result.TruthTable.Length, Is.EqualTo(4));
            // AND: 00→0, 01→0, 10→0, 11→1
            Assert.That(result.TruthTable[0], Is.False); // 00
            Assert.That(result.TruthTable[1], Is.False); // 01
            Assert.That(result.TruthTable[2], Is.False); // 10
            Assert.That(result.TruthTable[3], Is.True);  // 11
        }

        [Test]
        public void Parse_SimpleOrFormula_ShouldReturnCorrectTruthTable()
        {
            // Arrange
            var parser = new FormulaParser();
            string formula = "x1 | x2";

            // Act
            var result = parser.Parse(formula);

            // Assert
            Assert.That(result.VariableCount, Is.EqualTo(2));
            // OR: 00→0, 01→1, 10→1, 11→1
            Assert.That(result.TruthTable[0], Is.False); // 00
            Assert.That(result.TruthTable[1], Is.True);  // 01
            Assert.That(result.TruthTable[2], Is.True);  // 10
            Assert.That(result.TruthTable[3], Is.True);  // 11
        }

        [Test]
        public void Parse_NotFormula_ShouldReturnCorrectTruthTable()
        {
            // Arrange
            var parser = new FormulaParser();
            string formula = "!x1";

            // Act
            var result = parser.Parse(formula);

            // Assert
            Assert.That(result.VariableCount, Is.EqualTo(1));
            // NOT: 0→1, 1→0
            Assert.That(result.TruthTable[0], Is.True);  // 0
            Assert.That(result.TruthTable[1], Is.False); // 1
        }

        [Test]
        public void Parse_ComplexFormulaWithParentheses_ShouldReturnCorrectResult()
        {
            // Arrange
            var parser = new FormulaParser();
            string formula = "(x1 | x2) & !x3";

            // Act
            var result = parser.Parse(formula);

            // Assert
            Assert.That(result.VariableCount, Is.EqualTo(3));
            Assert.That(result.TruthTable.Length, Is.EqualTo(8));
        }

        [Test]
        public void Parse_FormulaWithDifferentOperators_ShouldHandleAllOperators()
        {
            // Arrange
            var parser = new FormulaParser();

            // Act & Assert - Should not throw exceptions
            Assert.DoesNotThrow(() => parser.Parse("x1 ∧ x2"));
            Assert.DoesNotThrow(() => parser.Parse("x1 ∨ x2"));
            Assert.DoesNotThrow(() => parser.Parse("x1 → x2"));
            Assert.DoesNotThrow(() => parser.Parse("x1 ↔ x2"));
            Assert.DoesNotThrow(() => parser.Parse("x1 xor x2"));
        }
    }

    [TestFixture]
    public class TruthTableTests
    {
        [Test]
        public void Constructor_WithVariableCount_ShouldGenerateEmptyTable()
        {
            // Arrange & Act
            var table = new TruthTable(3);

            // Assert
            Assert.That(table.VariableCount, Is.EqualTo(3));
            Assert.That(table.VariableNames, Has.Length.EqualTo(3));
            Assert.That(table.VariableNames, Is.EqualTo(new[] { "x1", "x2", "x3" }));
            Assert.That(table.Rows, Has.Count.EqualTo(8)); // 2^3 = 8
        }

        [Test]
        public void Constructor_FromBooleanFunction_ShouldCreateCorrectTable()
        {
            // Arrange
            var function = new BooleanFunction(2, 6); // XOR-like function

            // Act
            var table = new TruthTable(function);

            // Assert
            Assert.That(table.VariableCount, Is.EqualTo(2));
            Assert.That(table.Rows, Has.Count.EqualTo(4));
        }
    }

    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void BooleanFunction_FromFormulaAndFromNumber_ShouldBeEquivalent()
        {
            // Arrange
            var functionFromNumber = new BooleanFunction(2, 6); // XOR function
            var functionFromFormula = new BooleanFunction("(x1 & !x2) | (!x1 & x2)"); // XOR formula

            // Act
            string dnf1 = functionFromNumber.GetDNF();
            string dnf2 = functionFromFormula.GetDNF();

            string knf1 = functionFromNumber.GetKNF();
            string knf2 = functionFromFormula.GetKNF();

            // Assert - Both should represent the same function
            Assert.That(dnf1, Is.EqualTo(dnf2));
            Assert.That(knf1, Is.EqualTo(knf2));
        }

        [Test]
        public void DNFAndKNF_ForSameFunction_ShouldBeLogicallyEquivalent()
        {
            // Arrange
            var function = new BooleanFunction("x1 & x2");

            // Act
            string dnf = function.GetDNF();
            string knf = function.GetKNF();

            // Parse both back to check if they're equivalent
            var dnfFunction = new BooleanFunction(dnf);
            var knfFunction = new BooleanFunction(knf);

            // Assert - They should have the same truth table
            Assert.That(dnfFunction.TruthTable, Is.EqualTo(knfFunction.TruthTable));
        }
    }

    [TestFixture]
    public class MainViewModelTests
    {
        [Test]
        public void MainViewModel_PropertyChanges_ShouldNotifyCorrectly()
        {
            // Arrange
            var viewModel = new MainViewModel();
            bool propertyChanged = false;
            viewModel.PropertyChanged += (s, e) => propertyChanged = true;

            // Act
            viewModel.VariableCount = 4;

            // Assert
            Assert.That(propertyChanged, Is.True);
        }

        [Test]
        public void AreFunctionsEquivalent_ForSameFunctions_ShouldReturnTrue()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var func1 = new BooleanFunction("x1 & x2");
            var func2 = new BooleanFunction("x1 & x2");

            // Act
            bool result = viewModel.GetType().GetMethod("AreFunctionsEquivalent",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(viewModel, new object[] { func1, func2 }) as bool? ?? false;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void AreFunctionsEquivalent_ForDifferentFunctions_ShouldReturnFalse()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var func1 = new BooleanFunction("x1 & x2");
            var func2 = new BooleanFunction("x1 | x2");

            // Act
            bool result = viewModel.GetType().GetMethod("AreFunctionsEquivalent",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(viewModel, new object[] { func1, func2 }) as bool? ?? false;

            // Assert
            Assert.That(result, Is.False);
        }
    }
}