using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KeyboardCalculator
{
    class CalculatorDisplay : INotifyPropertyChanged
    {
        private string _display = ""; 
        public event PropertyChangedEventHandler PropertyChanged;
        private static CalculatorDisplay _instance = null;
        

        public static CalculatorDisplay Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CalculatorDisplay();
                }
                return _instance;
            }
        }

        public string display
        {
            get
            {
                return _display;
            }
            set
            {
                _display = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("display"));
            }
        }

        public bool decimalPlaceLock = false;
    }
}
