using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
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

        //Manejar los mandos Segundo feo
        private readonly object myLock = new object();
        private List<Gamepad> myGamepads = new List<Gamepad>();
        private Gamepad mainGamepad = null;
        private GamepadReading reading, prereading;
        private GamepadVibration vibration;

        //Manejar el timer
        DispatcherTimer gameTimer;
        public MandoPage()
        {
            this.InitializeComponent();

            //Chekeamos si al cambiar de pagina, se ha añadido un mando o se ha removido alguno
            checkGamepadAdded();

            checkGamepadRemoved();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Configuramos los mandos
            GameTimerSetUp();
        }

        #region Back BUtton
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }

        private void BackBut_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.OriginalKey == Windows.System.VirtualKey.GamepadB)
            {

            }
            else if (e.Key == Windows.System.VirtualKey.Escape || e.Key == Windows.System.VirtualKey.GamepadMenu)
            {
                Back_Click(sender, e);
            }
        }
        #endregion


        #region MouseEvents
        private void C_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            name = (sender as ContentControl).Name;

            ptrPt = e.GetCurrentPoint(Canvas);
            if (ptrPt.Properties.IsLeftButtonPressed) BotIzq = true;
            //Establecer Cursor
            if (ptrPt.Properties.IsRightButtonPressed) BotDer = true;
        }
        private void C_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint NewptrPt = e.GetCurrentPoint(Canvas);

            if (BotIzq)
            {
                if (name == "C1")
                {
                    Canvas.SetLeft(C1, (int)NewptrPt.Position.X - (C1.Width / 2));
                    Canvas.SetTop(C1, (int)NewptrPt.Position.Y - (C1.Height / 2));
                }
                else if (name == "C2")
                {
                    Canvas.SetLeft(C2, (int)NewptrPt.Position.X - (C2.Width / 2));
                    Canvas.SetTop(C2, (int)NewptrPt.Position.Y - (C2.Height / 2));
                }
            }
        }

        private void C_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!ptrPt.Properties.IsLeftButtonPressed) BotIzq = false;
            if (!ptrPt.Properties.IsRightButtonPressed) BotDer = false;

            name = "";
        }
        #endregion


        #region KeyBoardEvents
        private void C_KeyDown(object sender, KeyRoutedEventArgs e){
            bool move = false;
            int X = 0, Y = 0;
            name = (sender as ContentControl).Name;
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

        #endregion

        #region GamePadEvents
        private void checkGamepadAdded(){
            Gamepad.GamepadAdded += (object sender, Gamepad e) =>
            {
                lock (myLock)
                {
                    bool gamepadInList = myGamepads.Contains(e);
                    //Mira se programar 
                    if (!gamepadInList) myGamepads.Add(e);
                }
            };
        }

        private void checkGamepadRemoved(){
            Gamepad.GamepadRemoved += (object sender, Gamepad e) => {
                lock (myLock)
                {
                    //Buscamos el indice del GamePad que se ha removido
                    int indexRemoved = myGamepads.IndexOf(e);
                    // Si existe en la lista
                    if (indexRemoved > -1)
                    {
                        //Verificamos si es el actual /princiapl
                        if (mainGamepad == myGamepads[indexRemoved])
                            mainGamepad = null;
                        //Se remueve de la lista sea el principal o no
                        myGamepads.RemoveAt(indexRemoved);
                    }
                }
            };
        }

        /// <summary>
        /// Esto establecer el mando para que empiece a funcinar :)
        /// </summary>
        public void GameTimerSetUp()
        {
            gameTimer = new DispatcherTimer();
            //Llamamos a un callback 
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = new TimeSpan(1000); //1 segundo subnormal
            gameTimer.Start();

        }


        void GameTimer_Tick(object sender, object e){
            //Leemos los mandos
            LeeMando();
            //Ni idea loco
            DeadZoneMando();
            //
            AplMando();
            //Dar FeedBack de vibracion
            //FeedBack();
        }

        private void LeeMando(){
            //No hay GamePads en la lista, no añade ninguno
            if (myGamepads.Count != 0){
                //Selecciona el principal como el primero de la lista
                mainGamepad = myGamepads[0];
                prereading = reading;
                //Prerading es el reading anterior. Al principio es NULL
                reading = mainGamepad.GetCurrentReading();
            }
        }

        private void DeadZoneMando(){
            //Entiendo que si alguno de los ejes se mueve a mas de 0.1u/sec, lo limita a 0.1 para que sea constante
            checkDeadZoneMando(ref reading.RightThumbstickX);
            checkDeadZoneMando(ref reading.RightThumbstickY);
        }
        private void checkDeadZoneMando(ref double gamePadRead)
        {
            //Entiendo que 
            if (gamePadRead < 0.1) gamePadRead += 0.1;
            else if (gamePadRead > 0.1) gamePadRead -= 0.1;
            //Si ya se mueve a 0.1, lo pone a 0... wtf
            else gamePadRead = 0;
        }

        private void FeedBack()
        {
            if (mainGamepad != null)
            {
                //get the first gamepad
                mainGamepad = myGamepads[0];

                float limit = 0.5f;
                //Set Vibration to your wife

                if ((Math.Abs(reading.RightThumbstickX) > limit) | (Math.Abs(reading.RightThumbstickY) > limit))
                {
                    double X = reading.RightThumbstickX * reading.RightThumbstickX;
                    double Y = reading.RightThumbstickY * reading.RightThumbstickY;

                    if (X > Y) vibration.RightMotor = X;
                    else vibration.RightMotor = Y;
                }
                else vibration.RightMotor = 0;

                if ((Math.Abs(reading.LeftTrigger) > limit) | (Math.Abs(reading.RightTrigger) > limit))
                {
                    if (reading.LeftTrigger > reading.RightTrigger)
                        vibration.LeftMotor = reading.LeftTrigger;
                    else vibration.RightMotor = reading.RightTrigger;
                }
                else vibration.LeftMotor = 0;

                //copy vibration to mainGamePad
                mainGamepad.Vibration = vibration;
            }
        }

        private void AplMando(){
            if ((mainGamepad != null))
                //Si el elemento C2, no esta seleccionado, no lo movemos
                if(C2.FocusState != FocusState.Unfocused) move(C2);
        }

        //Movemos un elemento del Canvas con el mando
        private void move(UIElement C4Samir){
            int X = (int)Canvas.GetLeft(C4Samir);
            int Y = (int)Canvas.GetTop(C4Samir);

            //Movemos la imagen en funcion de la posicion del JoyStick
            X = (int)(X + 10 * reading.RightThumbstickX);
            Y = (int)(Y - 10 * reading.RightThumbstickY);

            //Aplicamos Pos
            Canvas.SetLeft(C4Samir, X);
            Canvas.SetTop(C4Samir, Y);
        }
        #endregion
    }
}
