using System;
using System.Collections.Generic;
using System.Linq;

namespace LAB4_FOX.Models
{//
    public class BooleanFunction
    {
        public int VariableCount { get; private set; }
        public bool[] TruthTable { get; private set; }
        public int FunctionNumber { get; private set; }
        public string Formula { get; private set; }

        public BooleanFunction(int variableCount, int functionNumber)
        {
            if (variableCount <= 0 || variableCount > 8)
                throw new ArgumentException("Количество переменных должно быть от 1 до 8");

            VariableCount = variableCount;
            FunctionNumber = functionNumber;
            TruthTable = GenerateTruthTableFromNumber(variableCount, functionNumber);
            Formula = $"f{functionNumber}({variableCount})";
        }

        public BooleanFunction(string formula)
        {
            Formula = formula;
            var parser = new FormulaParser();
            var result = parser.Parse(formula);
            TruthTable = result.TruthTable;
            VariableCount = result.VariableCount;
            FunctionNumber = CalculateFunctionNumber(TruthTable);
        }

        private bool[] GenerateTruthTableFromNumber(int n, int num)
        {
            int tableSize = 1 << n;
            var table = new bool[tableSize];

            for (int i = 0; i < tableSize; i++)
            {
                table[i] = ((num >> i) & 1) == 1;
            }

            return table;
        }

        private int CalculateFunctionNumber(bool[] truthTable)
        {
            int number = 0;
            for (int i = 0; i < truthTable.Length; i++)
            {
                if (truthTable[i])
                    number |= (1 << i);
            }
            return number;
        }

        public string GetDNF()
        {
            var dnfBuilder = new DNFBuilder();
            return dnfBuilder.BuildDNF(TruthTable, VariableCount);
        }

        public string GetKNF()
        {
            var knfBuilder = new KNFBuilder();
            return knfBuilder.BuildKNF(TruthTable, VariableCount);
        }

        public override string ToString()
        {
            return $"f({VariableCount}) №{FunctionNumber}";
        }
    }
}