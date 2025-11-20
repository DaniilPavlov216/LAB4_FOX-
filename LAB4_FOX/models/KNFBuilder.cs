using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LAB4_FOX.Models
{
    public class KNFBuilder
    {
        public string BuildKNF(bool[] truthTable, int variableCount)
        {
            var maxterms = new List<string>();
            string[] variables = Enumerable.Range(0, variableCount)
                .Select(i => $"x{i + 1}")
                .ToArray();

            for (int i = 0; i < truthTable.Length; i++)
            {
                if (!truthTable[i])
                {
                    maxterms.Add(BuildMaxterm(i, variableCount, variables));
                }
            }

            if (maxterms.Count == 0)
                return "1";

            if (maxterms.Count == truthTable.Length)
                return "0";

            return string.Join(" ∧ ", maxterms);
        }

        private string BuildMaxterm(int index, int variableCount, string[] variables)
        {
            var literals = new List<string>();

            for (int j = 0; j < variableCount; j++)
            {
                bool value = ((index >> (variableCount - 1 - j)) & 1) == 1;
                string literal = value ? $"¬{variables[j]}" : variables[j];
                literals.Add(literal);
            }

            return $"({string.Join(" ∨ ", literals)})";
        }

        public (int Literals, int Conjunctions, int Disjunctions) CalculateCost(string knf)
        {
            if (knf == "0" || knf == "1")
                return (0, 0, 0);

            var disjunctions = knf.Split('∧').Select(s => s.Trim()).ToArray();
            int conjunctions = disjunctions.Length - 1;

            int totalLiterals = 0;
            int totalDisjunctions = 0;

            foreach (var disj in disjunctions)
            {
                var cleanDisj = disj.Trim('(', ')');
                var literals = cleanDisj.Split('∨').Select(s => s.Trim()).ToArray();
                totalDisjunctions += literals.Length - 1;
                totalLiterals += literals.Length;
            }

            return (totalLiterals, conjunctions, totalDisjunctions);
        }
    }
}