using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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

        private void C_KeyDown(object sender, KeyRoutedEventArgs e){
            bool move = false;
            int X = 0, Y = 0;
            name = (sender as TextBlock).Name;
            if(name == "C1"){
                X = (int)Canvas.GetLeft(C1);
                Y = (int)Canvas.GetTop(C1);

            }
            else if (name == "C2"){
                X = (int)Canvas.GetLeft(C2);
                Y = (int)Canvas.GetTop(C2);
            }

            switch (e.Key){
                //Izquierda
                case VirtualKey.A:
                case VirtualKey.GamepadRightThumbstickLeft:
                    X -= 10;
                    move = true;
                    break;
                //Derecha
                case VirtualKey.D:
                case VirtualKey.GamepadRightThumbstickRight:
                    X += 10;
                    move = true;
                    break;
                //Arriba
                case VirtualKey.W:
                case VirtualKey.GamepadRightThumbstickUp:
                    Y -= 10;
                    move = true;
                    break;
                case VirtualKey.S:
                case VirtualKey.GamepadRightThumbstickDown:
                    Y += 10;
                    move = true;
                    break;
            }

            if(name == "C1"){
                Canvas.SetLeft(C1, (int)X);
                Canvas.SetTop(C1, (int)Y);
            }
            else if(name == "C2")
            {
                Canvas.SetLeft(C2, (int)X);
                Canvas.SetTop(C2, (int)Y);
            }
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
