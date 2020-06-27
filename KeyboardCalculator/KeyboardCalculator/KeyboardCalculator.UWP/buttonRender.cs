using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

namespace KeyboardCalculator.UWP
{
    class ButtonRender : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                //hover
                Control.PointerEntered += (s, ex) => {
                    Control.BackgroundColor = new SolidColorBrush(Colors.Red);
                };
                Control.PointerMoved += (s, ex) => {
                    Control.BackgroundColor = new SolidColorBrush(Colors.Red);
                };
                Control.PointerExited += (s, ex) =>
                {
                    Control.BackgroundColor = new SolidColorBrush(Colors.Red);
                };

                //click
                Control.Click += (s, ex) => {
                    Control.BackgroundColor = new SolidColorBrush(Colors.Red);
                };
            }
        }
    }
}
