using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using LAB4_FOX.Models;

namespace LAB4_FOX.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _variableCount = 3;
        private int _functionNumber = 11;
        private string _formula = "(x1 | x2) -> x3";
        private string _formula1 = "x1 & !x2 | x3";
        private string _formula2 = "x1 & !x2 | x3";
        private string _truthTableText = "";
        private string _dnfText = "";
        private string _knfText = "";
        private string _comparisonResult = "";
        private string _dnfCost = "";
        private string _knfCost = "";

        public int VariableCount
        {
            get => _variableCount;
            set { _variableCount = value; OnPropertyChanged(); }
        }

        public int FunctionNumber
        {
            get => _functionNumber;
            set { _functionNumber = value; OnPropertyChanged(); }
        }

        public string Formula
        {
            get => _formula;
            set { _formula = value; OnPropertyChanged(); }
        }

        public string Formula1
        {
            get => _formula1;
            set { _formula1 = value; OnPropertyChanged(); }
        }

        public string Formula2
        {
            get => _formula2;
            set { _formula2 = value; OnPropertyChanged(); }
        }

        public string TruthTableText
        {
            get => _truthTableText;
            set { _truthTableText = value; OnPropertyChanged(); }
        }

        public string DnfText
        {
            get => _dnfText;
            set { _dnfText = value; OnPropertyChanged(); }
        }

        public string KnfText
        {
            get => _knfText;
            set { _knfText = value; OnPropertyChanged(); }
        }

        public string ComparisonResult
        {
            get => _comparisonResult;
            set { _comparisonResult = value; OnPropertyChanged(); }
        }

        public string DnfCost
        {
            get => _dnfCost;
            set { _dnfCost = value; OnPropertyChanged(); }
        }

        public string KnfCost
        {
            get => _knfCost;
            set { _knfCost = value; OnPropertyChanged(); }
        }

        public ICommand GenerateFromNumberCommand { get; }
        public ICommand GenerateFromFormulaCommand { get; }
        public ICommand CompareFunctionsCommand { get; }
        public ICommand CopyDNFCommand { get; }
        public ICommand CopyKNFCommand { get; }

        public MainViewModel()
        {
            GenerateFromNumberCommand = new RelayCommand(GenerateFromNumber);
            GenerateFromFormulaCommand = new RelayCommand(GenerateFromFormula);
            CompareFunctionsCommand = new RelayCommand(CompareFunctions);
            CopyDNFCommand = new RelayCommand(CopyDNF);
            CopyKNFCommand = new RelayCommand(CopyKNF);
        }

        private void GenerateFromNumber()
        {
            try
            {
                var function = new BooleanFunction(VariableCount, FunctionNumber);
                DisplayFunctionResults(function);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void GenerateFromFormula()
        {
            try
            {
                var function = new BooleanFunction(Formula);
                DisplayFunctionResults(function);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка парсинга формулы: {ex.Message}");
            }
        }

        private void CompareFunctions()
        {
            try
            {
                var func1 = new BooleanFunction(Formula1);
                var func2 = new BooleanFunction(Formula2);

                bool areEquivalent = AreFunctionsEquivalent(func1, func2);

                if (areEquivalent)
                {
                    ComparisonResult = "Функции ЭКВИВАЛЕНТНЫ";
                }
                else
                {
                    var counterExample = FindCounterExample(func1, func2);
                    ComparisonResult = $"Функции НЕ ЭКВИВАЛЕНТНЫ\nКонтрпример: {counterExample}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сравнения: {ex.Message}");
            }
        }

        private void CopyDNF()
        {
            Clipboard.SetText(DnfText);
        }

        private void CopyKNF()
        {
            Clipboard.SetText(KnfText);
        }

        private void DisplayFunctionResults(BooleanFunction function)
        {
            // Таблица истинности
            var table = new TruthTable(function);
            TruthTableText = BuildTableText(table);

            // DNF
            DnfText = function.GetDNF();
            var dnfCostCalc = new DNFBuilder().CalculateCost(DnfText);
            DnfCost = $"Литералы: {dnfCostCalc.Literals}, Конъюнкции: {dnfCostCalc.Conjunctions}, Дизъюнкции: {dnfCostCalc.Disjunctions}";

            // KNF
            KnfText = function.GetKNF();
            var knfCostCalc = new KNFBuilder().CalculateCost(KnfText);
            KnfCost = $"Литералы: {knfCostCalc.Literals}, Конъюнкции: {knfCostCalc.Conjunctions}, Дизъюнкции: {knfCostCalc.Disjunctions}";
        }

        private string BuildTableText(TruthTable table)
        {
            var sb = new StringBuilder();

            // Заголовок
            foreach (var varName in table.VariableNames)
            {
                sb.Append($"{varName}\t");
            }
            sb.AppendLine("f()");
            sb.AppendLine(new string('-', (table.VariableCount + 1) * 8));

            // Данные
            foreach (var row in table.Rows)
            {
                foreach (var input in row.Inputs)
                {
                    sb.Append($"{(input ? "1" : "0")}\t");
                }
                sb.AppendLine($"{(row.Output ? "1" : "0")}");
            }

            return sb.ToString();
        }

        private bool AreFunctionsEquivalent(BooleanFunction func1, BooleanFunction func2)
        {
            if (func1.VariableCount != func2.VariableCount)
                return false;

            for (int i = 0; i < func1.TruthTable.Length; i++)
            {
                if (func1.TruthTable[i] != func2.TruthTable[i])
                    return false;
            }

            return true;
        }

        private string FindCounterExample(BooleanFunction func1, BooleanFunction func2)
        {
            int varCount = Math.Max(func1.VariableCount, func2.VariableCount);

            for (int i = 0; i < (1 << varCount); i++)
            {
                bool val1 = GetValueForInput(func1, i, varCount);
                bool val2 = GetValueForInput(func2, i, varCount);

                if (val1 != val2)
                {
                    return FormatInput(i, varCount);
                }
            }

            return "не найден";
        }

        private bool GetValueForInput(BooleanFunction func, int input, int maxVarCount)
        {
            if (func.VariableCount == maxVarCount)
            {
                return func.TruthTable[input];
            }
            else
            {
                // Для функций с меньшим числом переменных
                int adjustedInput = input >> (maxVarCount - func.VariableCount);
                return func.TruthTable[adjustedInput];
            }
        }

        private string FormatInput(int input, int varCount)
        {
            var values = new List<string>();
            for (int j = 0; j < varCount; j++)
            {
                bool value = ((input >> (varCount - 1 - j)) & 1) == 1;
                values.Add($"x{j + 1} = {(value ? "1" : "0")}");
            }
            return string.Join(", ", values);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}