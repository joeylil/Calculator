using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace KeyboardCalculator
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        static public string messagingCenterHasToggled = "messagingCenterHasToggled";
        

        public MainPage()
        {
            InitializeComponent();
            BindingContext = CalculatorDisplay.Instance;
            Keyboard.buttonPressed = CalculateDisplay;
            MessagingCenter.Subscribe<KeyboardLayout, char>(this, KeyboardLayout.calculatorButtonClicked, (sender, arg) =>
            {
                
                if (arg == '=')
                {
                    char lastCharacter = CalculatorDisplay.Instance.display[CalculatorDisplay.Instance.display.Length - 1];

                    if (!char.IsDigit(lastCharacter))
                    {
                        return;
                    }
                    //The following code works but i wanted to try do this myself as i was unsure how to and wanted the challenge
                    //CalculatorDisplay.Instance.display = new DataTable().Compute(CalculatorDisplay.Instance.display, null).ToString();

                    char[] operators = { '*', '/', '+', '-' };
                    foreach (char operation in operators)
                    {
                        CalculatorDisplay.Instance.display = getIndexOfOperator(CalculatorDisplay.Instance.display, operation);
                    }
                    double total = 0;
                    double.TryParse(CalculatorDisplay.Instance.display, out total);
                    if (total % 1 == 0) // if total is an interger
                    {
                        try
                        {
                            CalculatorDisplay.Instance.display = Convert.ToInt32(total).ToString();
                        }
                        catch (Exception OverflowException) // if total is too small or too big for int 32
                        {
                            try
                            {
                                CalculatorDisplay.Instance.display = Convert.ToInt64(total).ToString();
                            }
                            catch // if the total is too small or too big for int 64
                            {
                                CalculatorDisplay.Instance.display = total.ToString();
                            }
                        }
                    }                  

                }
                else if (arg == '←')
                {
                    char lastCharacter = CalculatorDisplay.Instance.display[CalculatorDisplay.Instance.display.Length - 2];                    
                    if (CalculatorDisplay.Instance.display.Length > 0)
                    {
                        if (lastCharacter == ' ')
                        {
                            CalculatorDisplay.Instance.display = CalculatorDisplay.Instance.display.Remove(CalculatorDisplay.Instance.display.Length - 2);
                        }
                        else
                        {
                            CalculatorDisplay.Instance.display = CalculatorDisplay.Instance.display.Remove(CalculatorDisplay.Instance.display.Length - 1);
                        }
                    }
                }
                else if(arg == 'C')
                {
                    CalculatorDisplay.Instance.display = "";
                }
                else //if the input is a digit, decimal place or operator
                {
                    if (CalculatorDisplay.Instance.display.Length > 0)
                    {
                        long i = 0;
                        if (CalculatorDisplay.Instance.display.Length > 10) 
                        {
                            // calculate if the last 10 characters are all digits or the input is not a digit to prevent digits longer than 10 characters long
                            if (!long.TryParse(CalculatorDisplay.Instance.display.Substring(CalculatorDisplay.Instance.display.Length - 10, 10), out i) || !char.IsDigit(arg)) 
                            {
                                AddToDisplay(arg);
                            }
                        }
                        else
                        {
                            AddToDisplay(arg);
                        }                                              
                    }
                    else
                    {
                        if(char.IsDigit(arg) || arg == '-') // first input needs to be digit or "-"
                        {
                            CalculatorDisplay.Instance.display += arg;
                        }                       
                    }
                }
            });
        }

        private void AddToDisplay(char arg)
        {
            char lastCharacter = CalculatorDisplay.Instance.display[CalculatorDisplay.Instance.display.Length - 1];
            //calcualte whether input will be added to display, and whether a space is required before input.
            if (((char.IsDigit(lastCharacter) || lastCharacter == '.') && char.IsDigit(arg)) || arg == '.' && char.IsDigit(lastCharacter))
            {
                if (!char.IsDigit(arg) && arg != '.')
                {
                    CalculatorDisplay.Instance.decimalPlaceLock = false;
                    CalculatorDisplay.Instance.display += arg;
                }                                  
                else if (!CalculatorDisplay.Instance.decimalPlaceLock && arg == '.')
                {
                    CalculatorDisplay.Instance.display += arg;
                }
                else if (arg != '.')
                {
                    CalculatorDisplay.Instance.display += arg;
                }
            }
            else if (char.IsDigit(lastCharacter) && !char.IsDigit(arg) || !char.IsDigit(lastCharacter) && char.IsDigit(arg))
            {
                if (!char.IsDigit(arg) && arg != '.')
                {
                    CalculatorDisplay.Instance.decimalPlaceLock = false;
                    CalculatorDisplay.Instance.display += " " + arg;
                }
                else if (!CalculatorDisplay.Instance.decimalPlaceLock && arg == '.')
                {
                    CalculatorDisplay.Instance.display += " " + arg;
                }
                else if (arg != '.')
                {
                    CalculatorDisplay.Instance.display += " " + arg;
                }
            }
        }

        private string getIndexOfOperator(string source, char searchChar)
        {
            int numberOfOperatorRemaining = source.Split(searchChar).Length - 1;
            while (numberOfOperatorRemaining > 0)
            {
                string[] splitDisplayString = source.Split(' ');
                int operatorIndex = 0;
                for (int i = 0; i < splitDisplayString.Length; i++)
                {
                    if (splitDisplayString[i] == searchChar.ToString())
                    {
                        operatorIndex = i;
                    }
                }
                double firstOperand = 0;
                try
                {
                    double.TryParse(splitDisplayString[operatorIndex - 1], out firstOperand);
                }
                catch (Exception IndexOutOfRangeException)
                {
                    return splitDisplayString[0];
                }
                double secondOperand = 0;
                double.TryParse(splitDisplayString[operatorIndex + 1], out secondOperand);
                double subtotal = operatorLogic(firstOperand, splitDisplayString[operatorIndex], secondOperand);

                //convert array to List to enable removing
                var listDisplayString = new List<string>(splitDisplayString);
                listDisplayString.Insert(operatorIndex + 2, subtotal.ToString());
                listDisplayString.RemoveRange(operatorIndex - 1, 3);
                source = String.Join(" ", listDisplayString);
                numberOfOperatorRemaining--;
            }
            return source;
                
        }

        private double operatorLogic(double firstOperand, string operatorString, double secondOperand)
        {
            switch (operatorString)
            {
                case "*": return firstOperand * secondOperand;
                case "/": return firstOperand / secondOperand;
                case "+": return firstOperand + secondOperand;
                case "-": return firstOperand - secondOperand;
                default: throw new Exception("Invalid logic");
            }
        }

        private void CalculateDisplay(char textofButtonPushed)
        {
            if (textofButtonPushed == '=')
            {
                char lastCharacter = CalculatorDisplay.Instance.display[CalculatorDisplay.Instance.display.Length - 1];

                if (!char.IsDigit(lastCharacter))
                {
                    return;
                }
                //The following code works but i wanted to try do this myself as i was unsure how to and wanted the challenge
                //CalculatorDisplay.Instance.display = new DataTable().Compute(CalculatorDisplay.Instance.display, null).ToString();

                char[] operators = { '*', '/', '+', '-' };
                foreach (char operation in operators)
                {
                    CalculatorDisplay.Instance.display = getIndexOfOperator(CalculatorDisplay.Instance.display, operation);
                }
                double total = 0;
                double.TryParse(CalculatorDisplay.Instance.display, out total);
                if (total % 1 == 0) // if total is an interger
                {
                    try
                    {
                        CalculatorDisplay.Instance.display = Convert.ToInt32(total).ToString();
                    }
                    catch (Exception OverflowException) // if total is too small or too big for int 32
                    {
                        try
                        {
                            CalculatorDisplay.Instance.display = Convert.ToInt64(total).ToString();
                        }
                        catch // if the total is too small or too big for int 64
                        {
                            CalculatorDisplay.Instance.display = total.ToString();
                        }
                    }
                }

            }
            else if (textofButtonPushed == '←')
            {
                char lastCharacter = CalculatorDisplay.Instance.display[CalculatorDisplay.Instance.display.Length - 2];
                if (CalculatorDisplay.Instance.display.Length > 0)
                {
                    if (lastCharacter == ' ')
                    {
                        CalculatorDisplay.Instance.display = CalculatorDisplay.Instance.display.Remove(CalculatorDisplay.Instance.display.Length - 2);
                    }
                    else
                    {
                        CalculatorDisplay.Instance.display = CalculatorDisplay.Instance.display.Remove(CalculatorDisplay.Instance.display.Length - 1);
                    }
                }
            }
            else if (textofButtonPushed == 'C')
            {
                CalculatorDisplay.Instance.display = "";
            }
            else //if the input is a digit, decimal place or operator
            {
                if (CalculatorDisplay.Instance.display.Length > 0)
                {
                    long i = 0;
                    if (CalculatorDisplay.Instance.display.Length > 10)
                    {
                        // calculate if the last 10 characters are all digits or the input is not a digit to prevent digits longer than 10 characters long
                        if (!long.TryParse(CalculatorDisplay.Instance.display.Substring(CalculatorDisplay.Instance.display.Length - 10, 10), out i) || !char.IsDigit(textofButtonPushed))
                        {
                            AddToDisplay(textofButtonPushed);
                        }
                    }
                    else
                    {
                        AddToDisplay(textofButtonPushed);
                    }
                }
                else
                {
                    if (char.IsDigit(textofButtonPushed) || textofButtonPushed == '-') // first input needs to be digit or "-"
                    {
                        CalculatorDisplay.Instance.display += textofButtonPushed;
                    }
                }
            }
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            MessagingCenter.Send<MainPage>(this, messagingCenterHasToggled);
        }
       
    }
}
