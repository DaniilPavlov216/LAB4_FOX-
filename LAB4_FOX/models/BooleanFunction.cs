
using System;

namespace LogicTool.Models
{
    public class BooleanFunction
    {
        public int VariableCount { get; private set; }
        public bool[] TruthTable { get; private set; }
        public int FunctionNumber { get; private set; }

        public BooleanFunction(int variableCount, int functionNumber)
        {
            if (variableCount <= 0 || variableCount > 8)
                throw new ArgumentException("Количество переменных должно быть от 1 до 8");

            VariableCount = variableCount;
            FunctionNumber = functionNumber;
            TruthTable = GenerateTruthTableFromNumber(variableCount, functionNumber);
        }
        //
        private bool[] GenerateTruthTableFromNumber(int n, int num)
        {
            int tableSize = 1 << n; // 2^n
            var table = new bool[tableSize];

            for (int i = 0; i < tableSize; i++)
            {
                table[i] = ((num >> i) & 1) == 1;
            }

            return table;
        }

        public override string ToString()
        {
            return $"f({VariableCount}) №{FunctionNumber}";
        }
    }
}