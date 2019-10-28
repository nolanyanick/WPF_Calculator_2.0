using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;
namespace WpfCalculator.ViewModels
{
    class CalculatorViewModel : ObservableObject
    {

        //operators
        private enum Operation
        {
            None,
            Add, 
            Subtract,
            Multiply,
            Divide,
            Percent,
            Sin,
            Cos,
            Tan,
            Sqaure,
            SquareRoot,
            Equal
        }

        //dictionary of binary operators
        private Dictionary<string, Operation> Binary = new Dictionary<string, Operation>()
        {
            { "+", Operation.Add},
            { "-", Operation.Subtract},
            { "*", Operation.Multiply},
            { "/", Operation.Divide},
            { "%", Operation.Percent},
            { "=", Operation.Equal}
        };

        //dictionary of unary operators
        private Dictionary<string, Operation> Unary = new Dictionary<string, Operation>()
        {
            { "Sin", Operation.Sin},
            { "Cos", Operation.Cos},
            { "Tan", Operation.Tan},
            { "Sqr", Operation.Sqaure},
            { "SqrRt", Operation.SquareRoot}       
        };

        private static string _operandString;
        private static double _operand1;
        private static double _operand2;
        private static Operation _binaryOperator;
        private static string _operandError;

        //ICommand used for getting numbers
        public ICommand ButtonNumberCommand { get; set; }

        //ICommand for getting operators
        public ICommand ButtonOperationCommand { get; set; }

        private Operation CurrentOperation { get; set; }

        //string used to give feedback to user
        private string _displayContent;
                
        public string DisplayContent
        {
            get { return _displayContent; }
            set
            {
                _displayContent = value;
                OnPropertyChanged("DisplayContent");
            }
        }

        //method to update _operandString
        private void UpdateOperandString(object obj)
        {
            if (obj.ToString() != "CE")
            {
                _operandString += obj.ToString();
                _operandError += obj.ToString();
            }
            else
            {
                _operandString = "";
                _operandError = "";
            }
            DisplayContent = _operandString;
        }

        //method to get current operator
        private Operation CurrentOperator(string operationString)
        {
            if (Binary.ContainsKey(operationString))
            {
                return Binary[operationString];
            }
            else if (Unary.ContainsKey(operationString))
            {
                return Unary[operationString];
            }

            return Operation.None;
        }

        //method used to set up entire operation
        private void SetOperation(object obj)
        {
            Operation operation = CurrentOperator(obj.ToString());

            if (double.TryParse(_operandString, out double result))
            {
                if (Binary.ContainsValue(operation))
                {
                    if (operation == Operation.Equal)
                    {
                        _operand2 = result;

                        if (ProcessBinaryOperation(_binaryOperator) == 9999999999999999999)
                        {
                            _operandError += obj;
                            DisplayContent = "Invalid Syntax: " + _operandError;
                            _binaryOperator = Operation.None;
                            _operandError = "";
                            _operandString = "";
                        }
                        else
                        {
                            DisplayContent = ProcessBinaryOperation(_binaryOperator).ToString();
                            _binaryOperator = Operation.None;
                            _operandError = "";
                        }                    
                    }
                    else
                    {
                        _operand1 = result;
                        _binaryOperator = operation;
                        _operandString = "";
                        DisplayContent = _operandString;
                        _operandError += obj;
                    }
                }
                else if (Unary.ContainsValue(operation))
                {
                    _operand1 = result;
                    DisplayContent = ProcessUnaryOperation(operation).ToString();
                }
                else
                {
                    DisplayContent = "Syntax Error: " + _operandError;
                }

                _operandString = "";
            }
            else
            {
                DisplayContent = "Invalid number entered, please try again";
            }
        }

        //initialize viewModel
        private void InitializeViewModel()
        {
            _displayContent = "Enter a Number";
            ButtonNumberCommand = new RelayCommand(new Action<object>(UpdateOperandString));
            ButtonOperationCommand = new RelayCommand(new Action<object>(SetOperation));
        }

        //perform binary calculations
        private double ProcessBinaryOperation(Operation operation)
        {
            switch (operation)
            {                
                case Operation.Add:
                    return _operand1 + _operand2;
                    
                case Operation.Subtract:
                    return _operand1 - _operand2;

                case Operation.Multiply:
                    return _operand1 * _operand2;

                case Operation.Divide:
                    return _operand1 / _operand2;

                case Operation.Percent:
                    return _operand1 * (_operand2 / 100);

                default:
                    DisplayContent = "Syntax Error";
                    return 9999999999999999999;
            }
        }

        //perform unary calculations
        private double ProcessUnaryOperation(Operation operation)
        {
            switch (operation)
            {
                case Operation.Sin:
                    return Math.Sin(_operand1);

                case Operation.Cos:
                    return Math.Cos(_operand1);

                case Operation.Tan:
                    return Math.Tan(_operand1);

                case Operation.Sqaure:
                    return Math.Pow(_operand1, 2);

                case Operation.SquareRoot:
                    return Math.Sqrt(_operand1);

                default:
                    DisplayContent = "Invalid Syntax";
                    return 0;
            }
        }

        //constructor
        public CalculatorViewModel()
        {
            InitializeViewModel();
        }
    }
}