using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace TestExamen2
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MandoPage : Page
    {

        PointerPoint ptrPt;
        string name = "";
        bool BotDer = false, BotIzq = false;
        public MandoPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void Back_Click(object sender, RoutedEventArgs e){
            On_BackRequested();
        }

        private bool On_BackRequested(){
            if(this.Frame.CanGoBack){
                this.Frame.GoBack();
                return true;
            }
            return false;
        }

        private void BackBut_KeyDown(object sender, KeyRoutedEventArgs e){
            if (e.OriginalKey == Windows.System.VirtualKey.GamepadB)
            {

            }
            else if (e.Key == Windows.System.VirtualKey.Escape || e.Key == Windows.System.VirtualKey.GamepadMenu)
            {
                Back_Click(sender, e);
            }
        }

        private void C_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            name = (sender as TextBlock).Name;

            ptrPt = e.GetCurrentPoint(Canvas);
            if (ptrPt.Properties.IsLeftButtonPressed) BotIzq = true;
            //Establecer Cursor
            if (ptrPt.Properties.IsRightButtonPressed) BotDer = true;
        }

        private void C_PointerReleased(object sender, PointerRoutedEventArgs e){
            if (!ptrPt.Properties.IsLeftButtonPressed) BotIzq = false;
            if (!ptrPt.Properties.IsRightButtonPressed) BotDer = false;

            name = "";
        }

        private void C_PointerMoved(object sender, PointerRoutedEventArgs e){
            PointerPoint NewptrPt = e.GetCurrentPoint(Canvas);

            if(BotIzq){
                if (name == "C1"){
                    Canvas.SetLeft(C1, (int)NewptrPt.Position.X - (C1.Width / 2));
                    Canvas.SetTop(C1, (int)NewptrPt.Position.Y - (C1.Height / 2));
                }
                else if (name == "C2"){
                    Canvas.SetLeft(C2, (int)NewptrPt.Position.X - (C2.Width / 2));
                    Canvas.SetTop(C2, (int)NewptrPt.Position.Y - (C2.Height / 2));
                }
            }
        }
    }
}
