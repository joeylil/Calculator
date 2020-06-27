using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyboardCalculator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KeyboardLayout : StackLayout
    {
        bool messagingCenter = true;        
        static public string calculatorButtonClicked = "Calculator Button Clicked";
        public delegate void ButtonPressed(char operation);
        public ButtonPressed buttonPressed;


        public KeyboardLayout()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<MainPage>(this, MainPage.messagingCenterHasToggled, sender =>
            {
                messagingCenter = !messagingCenter;
            });
        }

        private void KeyboardButton_Clicked(object sender, EventArgs e)
        {            
            Button tappedButton = sender as Button;
            char inputCharacter = tappedButton.Text[0];
            if (messagingCenter)
            {               
                MessagingCenter.Send<KeyboardLayout, char>(this, calculatorButtonClicked, inputCharacter);
            }
            else
            {
                buttonPressed(inputCharacter);              
            }
        }


        private static string GetAllIndex(string source, char searchChar)
        {
            char operation = searchChar;
            int count = source.Split(searchChar).Length - 1;
            while(count > 0)
            {
                int index = source.IndexOf(searchChar);            
                int previousNumberStartIndex = index;
                int integer = 0;
                while (int.TryParse(source[previousNumberStartIndex - 1].ToString(), out integer))
                {
                    if(previousNumberStartIndex == 1)
                    {
                        previousNumberStartIndex = 0;
                        break;
                    }
                    previousNumberStartIndex--;
                }
                int nextNumberIndexEndIndex = index;
                while (int.TryParse(source[nextNumberIndexEndIndex + 1].ToString(), out integer))
                {
                    if(nextNumberIndexEndIndex == source.Length-2)
                    {
                        nextNumberIndexEndIndex = source.Length-1;
                        break;
                    }
                    nextNumberIndexEndIndex++;
                }
                int previousNumber = 0;
                if (index - 1 - previousNumberStartIndex > 0)
                {
                    int.TryParse(source.Substring(previousNumberStartIndex, index - previousNumberStartIndex), out previousNumber);
                }
                else
                {
                    int.TryParse(source[previousNumberStartIndex].ToString(), out previousNumber);
                }
                int nextNumber = 0;
                if (nextNumberIndexEndIndex - (index + 1) > 0)
                {
                    int.TryParse(source.Substring(index + 1, nextNumberIndexEndIndex - index), out nextNumber);
                    
                }
                else
                {
                    int.TryParse(source[nextNumberIndexEndIndex].ToString(), out nextNumber);
                }
                int subtotal = OperatorLogic(previousNumber, operation, nextNumber);
                source = source.Remove(previousNumberStartIndex, nextNumberIndexEndIndex - previousNumberStartIndex + 1);
                source = source.Insert(previousNumberStartIndex, subtotal.ToString());
                count--;
            }           
            return source;
        }
        private static int OperatorLogic(int firstOperand, char operatorString, int secondOperand)
        {
            switch (operatorString)
            {
                case '*': return firstOperand * secondOperand;
                case '/': return firstOperand / secondOperand;
                case '+': return firstOperand + secondOperand;
                case '-': return firstOperand - secondOperand;
                default: throw new Exception("Invalid logic");
            }
        }
    }
}