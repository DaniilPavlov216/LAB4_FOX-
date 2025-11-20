using System.Collections.Generic;
using System.Linq;

namespace LAB4_FOX.Models
{
    public class TruthTable
    {
        public List<TruthTableRow> Rows { get; } = new List<TruthTableRow>();
        public int VariableCount { get; }
        public string[] VariableNames { get; }

        public TruthTable(int variableCount)
        {
            VariableCount = variableCount;
            VariableNames = Enumerable.Range(0, variableCount)
                .Select(i => $"x{i + 1}")
                .ToArray();
            GenerateTable();
        }

        public TruthTable(BooleanFunction function)
        {
            VariableCount = function.VariableCount;
            VariableNames = Enumerable.Range(0, VariableCount)
                .Select(i => $"x{i + 1}")
                .ToArray();

            int rowCount = 1 << VariableCount;
            for (int i = 0; i < rowCount; i++)
            {
                var inputs = new bool[VariableCount];
                for (int j = 0; j < VariableCount; j++)
                {
                    inputs[j] = ((i >> (VariableCount - 1 - j)) & 1) == 1;
                }
                Rows.Add(new TruthTableRow(inputs, function.TruthTable[i]));
            }
        }

        private void GenerateTable()
        {
            int rowCount = 1 << VariableCount;

            for (int i = 0; i < rowCount; i++)
            {
                var inputs = new bool[VariableCount];
                for (int j = 0; j < VariableCount; j++)
                {
                    inputs[j] = ((i >> (VariableCount - 1 - j)) & 1) == 1;
                }
                Rows.Add(new TruthTableRow(inputs, false));
            }
        }
    }

    public class TruthTableRow
    {
        public bool[] Inputs { get; }
        public bool Output { get; set; }

        public TruthTableRow(bool[] inputs, bool output)
        {
            Inputs = inputs;
            Output = output;
        }
    }
}