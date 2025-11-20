using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LAB4_FOX.Models
{
    public class DNFBuilder
    {
        public string BuildDNF(bool[] truthTable, int variableCount)
        {
            var minterms = new List<string>();
            string[] variables = Enumerable.Range(0, variableCount)
                .Select(i => $"x{i + 1}")
                .ToArray();

            for (int i = 0; i < truthTable.Length; i++)
            {
                if (truthTable[i])
                {
                    minterms.Add(BuildMinterm(i, variableCount, variables));
                }
            }

            if (minterms.Count == 0)
                return "0";

            if (minterms.Count == truthTable.Length)
                return "1";

            return string.Join(" ∨ ", minterms);
        }

        private string BuildMinterm(int index, int variableCount, string[] variables)
        {
            var literals = new List<string>();

            for (int j = 0; j < variableCount; j++)
            {
                bool value = ((index >> (variableCount - 1 - j)) & 1) == 1;
                string literal = value ? variables[j] : $"¬{variables[j]}";
                literals.Add(literal);
            }

            return $"({string.Join(" ∧ ", literals)})";
        }

        public (int Literals, int Conjunctions, int Disjunctions) CalculateCost(string dnf)
        {
            if (dnf == "0" || dnf == "1")
                return (0, 0, 0);

            var conjunctions = dnf.Split('∨').Select(s => s.Trim()).ToArray();
            int disjunctions = conjunctions.Length - 1;

            int totalLiterals = 0;
            int totalConjunctions = 0;

            foreach (var conj in conjunctions)
            {
                var cleanConj = conj.Trim('(', ')');
                var literals = cleanConj.Split('∧').Select(s => s.Trim()).ToArray();
                totalConjunctions += literals.Length - 1;
                totalLiterals += literals.Length;
            }

            return (totalLiterals, totalConjunctions, disjunctions);
        }
    }
}