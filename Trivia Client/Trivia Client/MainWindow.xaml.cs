using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Ink;
using System.Diagnostics;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace Trivia_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        Constructor - Initializes _communicator and background workers.
        */
        public MainWindow()
        {
            InitializeComponent();
            // Initializing ToolTip message time
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            
            // Setting fields
            _available_rooms_worker.WorkerSupportsCancellation = true;
            _available_rooms_worker.WorkerReportsProgress = true;
            _available_rooms_worker.DoWork += getAvailableRooms;
            _available_rooms_worker.ProgressChanged += update_rooms_list;
            
            _room_state_worker.WorkerSupportsCancellation = true;
            _room_state_worker.WorkerReportsProgress = true;
            _room_state_worker.DoWork += checkRoomState;
            _room_state_worker.ProgressChanged += update_room_window;

            _game_results_worker.WorkerSupportsCancellation = true;
            _game_results_worker.WorkerReportsProgress = true;
            _game_results_worker.DoWork += checkForGameResults;
            _game_results_worker.ProgressChanged += update_game_results_window;

            _using_communicator = new Mutex();

            // Creating connection with server
        connect:
                try
                {
                    _communicator = new Communicator();
                }
                catch (Exception e)
                {
                    MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                        MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes);

                    if (result.Equals(MessageBoxResult.Yes))
                    {
                        goto connect; // Try again to connect
                    }
                    else
                    {
                        this.Close(); // Exit
                    }
                }
            _currWindow = Windows.ENTRY;
            SetEntryWindow();
        }

        /*
        This function sets the window's height, width, and background.
        Input: height, width, isDark 
        Output: none
        */
        private void SetWindow(int height, int width, bool isDark)
        {
            MainGrid.Children.Clear();
            Height = height;
            Width = width;

            if (isDark)
            {
                MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);
            }
            else
            {
                MainGrid.Background = new LinearGradientBrush(Colors.Linen, Colors.PaleTurquoise, 90);
            }
        }

        /*
        This function set the 'entry' window.
        Input: none
        Output: none
        */
        private void SetEntryWindow()
        {
            SetWindow(250, 400, false);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["darkLogo"], Width = 300 };

            Button loginButton = new Button { Content = "Login", Style = (Style)Resources["darkButton"],
                Height = 50, Width = 170, Margin = new Thickness(15, 0, 0, 20), 
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom };
            loginButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.LOGIN));

            Button signupButton = new Button { Content = "Signup", Style = (Style)Resources["darkButton"],
                Height = 50, Width = 170, Margin = new Thickness(0, 0, 15, 20),
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom };
            signupButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.SIGNUP));

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["darkTitle"], Text = "Do you want to login or sign up? :)"};

            StackPanel stack = new StackPanel();
            stack.Children.Add(logo);
            stack.Children.Add(messageBlock);

            // Adding controls to grid
            MainGrid.Children.Add(stack);
            MainGrid.Children.Add(loginButton);
            MainGrid.Children.Add(signupButton);
        }

        /*
        This function set the 'login' window.
        Input: none
        Output: none
        */
        private void SetLoginWindow()
        {
            SetWindow(350, 420, false);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["darkLogo"] };

            TextBlock usernameBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Username", Margin = new Thickness(0, 0, 0, 10) };
           
            TextBlock passwordBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Password", Margin = new Thickness(0, 0, 0, 20) };

            TextBlock passwordMessageBlock = new TextBlock { Style = (Style)Resources["messageBlock"],
                Text = "Your password is incorrect" };

            TextBlock usernameMessageBlock = new TextBlock { Style = (Style)Resources["messageBlock"], 
                Text = "The user name doesn't exists in our lists." };

            ToolTip username = new ToolTip();
            username.Style = (Style)Resources["ToolTip"];
            username.Content = "Please enter your user name.";
            TextBox usernameBox = new TextBox { Style = (Style)Resources["darkTextBox"], ToolTip = username, Margin = new Thickness(0, 0, 0, 10) };
            usernameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(usernameBlock, usernameBox, usernameMessageBlock));

            ToolTip password = new ToolTip();
            password.Style = (Style)Resources["ToolTip"];
            password.Content = "Don't forget that the password must contain:\n" +
                "only 8 characters,\n" +
                "at least 1 upper case letter, \n" +
                "at least 1 lower case letter,\n" +
                "at least 1 number\n" +
                "and at least 1 special character:\n" +
                "'!', '@', '#', '$', '%', '^', '&', '*'";
            PasswordBox hiddenPasswordBox = new PasswordBox { Style = (Style)Resources["darkPasswordBox"], ToolTip = password, Margin = new Thickness(0, 25, 0, 20) };

            TextBox visiblePasswordBox = new TextBox
            {
                Style = (Style)Resources["darkTextBox"],
                Margin = new Thickness(0, 116, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Hidden,
                MaxLength = 8,
                ToolTip = password
            };

            Button passwordEye = new Button
            {
                Style = (Style)Resources["passwordEyeButton"],
                Background = new ImageBrush(new BitmapImage(new Uri("..\\..\\Resources\\HidePassword.png", UriKind.Relative)))
            };
            visiblePasswordBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, visiblePasswordBox, passwordMessageBlock));
            hiddenPasswordBox.PasswordChanged += new RoutedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, hiddenPasswordBox, passwordMessageBlock, visiblePasswordBox));

            passwordEye.Click += (sender, args) =>
            {
                if (hiddenPasswordBox.Password.Length != 0)
                {
                    if (hiddenPasswordBox.Visibility == Visibility.Visible)
                    {
                        if (hiddenPasswordBox.Password != visiblePasswordBox.Text)
                        {
                            visiblePasswordBox.Text = hiddenPasswordBox.Password;
                        }
                        hiddenPasswordBox.Visibility = Visibility.Hidden;
                        visiblePasswordBox.Visibility = Visibility.Visible;
                        passwordEye.Background = new ImageBrush(new BitmapImage(new Uri("..\\..\\Resources\\ShowPassword.png", UriKind.Relative)));
                    }
                    else
                    {
                        if (hiddenPasswordBox.Password != visiblePasswordBox.Text)
                        {
                            hiddenPasswordBox.Password = visiblePasswordBox.Text;
                        }
                        visiblePasswordBox.Visibility = Visibility.Hidden;
                        hiddenPasswordBox.Visibility = Visibility.Visible;
                        passwordEye.Background = new ImageBrush(new BitmapImage(new Uri("..\\..\\Resources\\HidePassword.png", UriKind.Relative)));
                    }
                }
            };

            List<TextBox> textBoxes = new List<TextBox> { usernameBox, visiblePasswordBox };
            List<TextBlock> textBlocks = new List<TextBlock> { passwordMessageBlock, usernameMessageBlock };
            Button nextButton = new Button { Style = (Style)Resources["darkButton"], Content = "Next",
               HorizontalAlignment = HorizontalAlignment.Right };
            nextButton.Click += (sender, args) =>
            {
                if (hiddenPasswordBox.Visibility == Visibility.Hidden)
                {
                    hiddenPasswordBox.Password = visiblePasswordBox.Text;
                }

                HandleButtonClick(Windows.MENU, textBoxes, hiddenPasswordBox, textBlocks: textBlocks);
            };
            
            Button backButton = new Button { Style = (Style)Resources["darkButton"], Content = "Back",
                HorizontalAlignment = HorizontalAlignment.Left };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

            StackPanel boxes = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom};
            boxes.Children.Add(usernameBox);
            boxes.Children.Add(hiddenPasswordBox);
            boxes.Children.Add(nextButton);

            StackPanel blocks = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            blocks.Children.Add(usernameMessageBlock);
            blocks.Children.Add(usernameBlock);
            blocks.Children.Add(passwordMessageBlock);
            blocks.Children.Add(passwordBlock);
            blocks.Children.Add(backButton);

            // Adding controls to grid
            MainGrid.Children.Add(logo);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(boxes);
            MainGrid.Children.Add(passwordEye);
            MainGrid.Children.Add(visiblePasswordBox);
        }

        /*
        This function set the 'signup' window.
        Input: none
        Output: none
        */
        private void SetSignupWindow()
        {
            SetWindow(600, 400, false);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["darkLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["darkTitle"], Text = "Enter your details :)", Margin = new Thickness(0, 0, 0, 5) };

            TextBlock usernameBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Username", Margin = new Thickness(0, 0, 0, 5) };

            TextBlock passwordBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Password", Margin = new Thickness(0, 0, 0, 5) };

            TextBlock emailBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Email", Margin = new Thickness(0, 0, 0, 5) };

            TextBlock streetBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Street", Margin = new Thickness(40, 0, 0, 5), HorizontalAlignment = HorizontalAlignment.Left, Width = 140 };
            TextBlock aptBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Apt", Margin = new Thickness(25, 0, 0, 32), HorizontalAlignment = HorizontalAlignment.Center, Width = 40 };
            TextBlock cityBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "City", Margin = new Thickness(0, 0, 45, 202), HorizontalAlignment = HorizontalAlignment.Right, Width = 110 };

            TextBlock phoneBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Phone", Margin = new Thickness(0, 0, 40, 140), Width = 240, HorizontalAlignment = HorizontalAlignment.Right };

            TextBlock dayBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Day", Margin = new Thickness(40, 0, 20, 82), HorizontalAlignment = HorizontalAlignment.Left, Width = 90 };
            TextBlock monthBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Month", Margin = new Thickness(0, 0, 0, 82), HorizontalAlignment = HorizontalAlignment.Center, Width = 80 };
            TextBlock yearBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Year", Margin = new Thickness(25, 0, 40, 82), HorizontalAlignment = HorizontalAlignment.Right, Width = 90 };

            TextBlock usernameMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "The user name is taken!"
            };

            ToolTip username = new ToolTip();
            username.Style = (Style)Resources["ToolTip"];
            username.Content = "Choose your own unique user name!\n" +
                "can contain only letters and numbers.";
            TextBox usernameBox = new TextBox { Style = (Style)Resources["darkTextBox"], ToolTip = username, Margin = new Thickness(0, 25, 0, 5) };
            usernameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(usernameBlock, usernameBox, usernameMessageBlock));

            TextBlock passwordMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "Your password is invalid."
            };

            ToolTip password = new ToolTip();
            password.Style = (Style)Resources["ToolTip"];
            password.Content = "Password must contain:\n" +
                "only 8 characters,\n" +
                "at least 1 upper case letter, \n" +
                "at least 1 lower case letter,\n" +
                "at least 1 number\n" +
                "and at least 1 special character:\n" +
                "'!', '@', '#', '$', '%', '^', '&', '*'";
            PasswordBox hiddenPasswordBox = new PasswordBox { Style = (Style)Resources["darkPasswordBox"], ToolTip = password, Margin = new Thickness(0, 25, 0, 5) };

            TextBox visiblePasswordBox = new TextBox
            {
                Style = (Style)Resources["darkTextBox"],
                Margin = new Thickness(0, 0, 0, 113),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Hidden,
                MaxLength = 8,
                ToolTip = password
            };

            visiblePasswordBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, visiblePasswordBox, passwordMessageBlock));
            hiddenPasswordBox.PasswordChanged += new RoutedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, hiddenPasswordBox, passwordMessageBlock, visiblePasswordBox));

            Button passwordEye = new Button
            {
                Style = (Style)Resources["passwordEyeButton"],
                Margin = new Thickness(0, 0, 0, 113),
                Background = new ImageBrush(new BitmapImage(new Uri("..\\..\\Resources\\HidePassword.png", UriKind.Relative)))
            };
            passwordEye.Click += (sender, args) =>
            {
                if (hiddenPasswordBox.Password.Length != 0)
                {
                    if (hiddenPasswordBox.Visibility == Visibility.Visible)
                    {
                        if (hiddenPasswordBox.Password != visiblePasswordBox.Text)
                        {
                            visiblePasswordBox.Text = hiddenPasswordBox.Password;
                        }
                        hiddenPasswordBox.Visibility = Visibility.Hidden;
                        visiblePasswordBox.Visibility = Visibility.Visible;
                        passwordEye.Background = new ImageBrush(new BitmapImage(new Uri("..\\..\\Resources\\ShowPassword.png", UriKind.Relative)));
                    }
                    else
                    {
                        if (hiddenPasswordBox.Password != visiblePasswordBox.Text)
                        {
                            hiddenPasswordBox.Password = visiblePasswordBox.Text;
                        }
                        visiblePasswordBox.Visibility = Visibility.Hidden;
                        hiddenPasswordBox.Visibility = Visibility.Visible;
                        passwordEye.Background = new ImageBrush(new BitmapImage(new Uri("..\\..\\Resources\\HidePassword.png", UriKind.Relative)));
                    }
                }
            };

            TextBlock emailMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "Your email is invalid."
            };

            ToolTip email = new ToolTip();
            email.Style = (Style)Resources["ToolTip"];
            email.Content = "Must be a valid email!\n" +
                "with '@' in the middle\n" +
                "and some web domain at the end\n" +
                "('gmail.com', etc...)";
            TextBox emailBox = new TextBox{ Style = (Style)Resources["darkTextBox"], ToolTip = email, Margin = new Thickness(0, 25, 0, 262) };
            emailBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(emailBlock, emailBox, emailMessageBlock));

            TextBlock addressMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "Your address is invalid."
            };

            ToolTip apt = new ToolTip();
            apt.Style = (Style)Resources["ToolTip"];
            apt.Content = "Enter your Apartment\n" +
                "must only contain numbers";
            TextBox aptBox = new TextBox { Style = (Style)Resources["darkTextBox"], ToolTip = apt, Margin = new Thickness(5, 25, 0, 202), Width = 40, MaxLength = 3, HorizontalAlignment = HorizontalAlignment.Center};

            ToolTip street = new ToolTip();
            street.Style = (Style)Resources["ToolTip"];
            street.Content = "Enter your Street\n" +
                "must only contain letters";
            TextBox streetBox = new TextBox { Style = (Style)Resources["darkTextBox"], ToolTip = street, Margin = new Thickness(40, 25, 0, 202), Width = 140, HorizontalAlignment = HorizontalAlignment.Left };

            ToolTip city = new ToolTip();
            city.Style = (Style)Resources["ToolTip"];
            city.Content = "Enter your City\n" +
                "must only contain letters";
            TextBox cityBox = new TextBox { Style = (Style)Resources["darkTextBox"], ToolTip = city, Margin = new Thickness(5, 25, 40, 202), Width = 110, HorizontalAlignment = HorizontalAlignment.Right };

            List<TextBox> address = new List<TextBox> { streetBox, cityBox, aptBox };
            aptBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(aptBlock, aptBox, addressMessageBlock, address));
            streetBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(streetBlock, streetBox, addressMessageBlock, address));
            cityBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(cityBlock, cityBox, addressMessageBlock, address));

            TextBlock phoneMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "Your phone is invalid."
            };

            ToolTip prefix = new ToolTip();
            prefix.Style = (Style)Resources["ToolTip"];
            prefix.Content = "Phone prefix";
            ComboBox prefixBox = new ComboBox { Style = (Style)Resources["comboStyle"],
                Margin = new Thickness(40, 0, 0, 115),
                HorizontalAlignment = HorizontalAlignment.Left, Height = 30,
                ToolTip = prefix,
                Foreground = Brushes.DarkBlue, Width = 60 };
            /*0[2-48-9])|(05\d)*/

            prefixBox.Items.Add("02");
            prefixBox.Items.Add("03");
            prefixBox.Items.Add("04");
            prefixBox.Items.Add("08");
            prefixBox.Items.Add("09");
            prefixBox.Items.Add("050");
            prefixBox.Items.Add("051");
            prefixBox.Items.Add("052");
            prefixBox.Items.Add("053");
            prefixBox.Items.Add("054");
            prefixBox.Items.Add("055");
            prefixBox.Items.Add("056");
            prefixBox.Items.Add("057");
            prefixBox.Items.Add("058");
            prefixBox.Items.Add("059");

            ToolTip phone = new ToolTip();
            phone.Style = (Style)Resources["ToolTip"];
            phone.Content = "Phone Number\n" +
                "must contain 7 numbers, no less, no more";
            TextBox phoneBox = new TextBox { Style = (Style)Resources["darkTextBox"], ToolTip = phone,
                Margin = new Thickness(10, 25, 0, 140), Width = 230, HorizontalAlignment = HorizontalAlignment.Left, MaxLength = 7 };
            phoneBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(phoneBlock, phoneBox, phoneMessageBlock));

            TextBlock birthdateMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "Your birth date is invalid.", Margin = new Thickness(0, 35, 0, 50)
            };

            ToolTip day = new ToolTip();
            day.Style = (Style)Resources["ToolTip"];
            day.Content = "Enter the day in the month of your birthday";
            TextBox dayBox = new TextBox
            {
                Style = (Style)Resources["darkTextBox"],
                ToolTip = day,
                Margin = new Thickness(40, 25, 15, 82),
                Width = 90,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxLength = 2
            };

            ToolTip month = new ToolTip();
            month.Style = (Style)Resources["ToolTip"];
            month.Content = "Enter the month of your birthday";
            TextBox monthBox = new TextBox
            {
                Style = (Style)Resources["darkTextBox"],
                ToolTip = month,
                Margin = new Thickness(0, 25, 0, 82),
                Width = 90,
                HorizontalAlignment = HorizontalAlignment.Center,
                MaxLength = 2
            };

            ToolTip year = new ToolTip();
            year.Style = (Style)Resources["ToolTip"];
            year.Content = "Enter the year you were born";
            TextBox yearBox = new TextBox
            {
                Style = (Style)Resources["darkTextBox"],
                ToolTip = year,
                Margin = new Thickness(15, 25, 40, 82),
                Width = 90,
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxLength = 4
            };

            List<TextBox> birthdate = new List<TextBox> { dayBox, monthBox, yearBox };
            dayBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(dayBlock, dayBox, birthdateMessageBlock, birthdate));
            monthBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(monthBlock, monthBox, birthdateMessageBlock, birthdate));
            yearBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(yearBlock, yearBox, birthdateMessageBlock, birthdate));

            List<TextBox> textBoxes = new List<TextBox>{ usernameBox, emailBox, streetBox, aptBox, cityBox, phoneBox, dayBox, monthBox, yearBox, visiblePasswordBox };
            List<TextBlock> textBlocks = new List<TextBlock> { passwordMessageBlock, emailMessageBlock, addressMessageBlock, phoneMessageBlock, birthdateMessageBlock, usernameMessageBlock };
            List<ComboBox> comboBoxes = new List<ComboBox> { prefixBox };

            Button nextButton = new Button { Style = (Style)Resources["darkButton"], Content = "Next",
                HorizontalAlignment = HorizontalAlignment.Right };
            nextButton.Click += (sender, args) =>
            {
                if (hiddenPasswordBox.Visibility == Visibility.Hidden)
                {
                    hiddenPasswordBox.Password = visiblePasswordBox.Text;
                }

                HandleButtonClick(Windows.MENU, textBoxes, hiddenPasswordBox, textBlocks: textBlocks, comboBoxes: comboBoxes);
            };

            Button backButton = new Button { Style = (Style)Resources["darkButton"], Content = "Back",
                HorizontalAlignment = HorizontalAlignment.Left};
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

            StackPanel boxes = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            boxes.Children.Add(usernameBox);
            boxes.Children.Add(hiddenPasswordBox);
            boxes.Children.Add(emailBox);

            StackPanel blocks = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            blocks.Children.Add(usernameMessageBlock);
            blocks.Children.Add(usernameBlock);
            blocks.Children.Add(passwordMessageBlock);
            blocks.Children.Add(passwordBlock);
            blocks.Children.Add(emailMessageBlock);
            blocks.Children.Add(emailBlock);
            blocks.Children.Add(addressMessageBlock);
            blocks.Children.Add(streetBlock);
            blocks.Children.Add(phoneMessageBlock);
            blocks.Children.Add(birthdateMessageBlock);
            blocks.Children.Add(backButton);

            StackPanel phoneText = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            phoneText.Children.Add(aptBlock);
            phoneText.Children.Add(phoneBlock);

            StackPanel addressBoxes = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Bottom };
            addressBoxes.Children.Add(streetBox);
            addressBoxes.Children.Add(aptBox);
            addressBoxes.Children.Add(cityBox);

            StackPanel phoneBoxes = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Bottom };
            phoneBoxes.Children.Add(prefixBox);
            phoneBoxes.Children.Add(phoneBox);

            StackPanel birthdateBoxes = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Bottom };
            birthdateBoxes.Children.Add(dayBox);
            birthdateBoxes.Children.Add(monthBox);
            birthdateBoxes.Children.Add(yearBox);

            StackPanel birthdateBlocks = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Bottom };
            birthdateBlocks.Children.Add(dayBlock);
            birthdateBlocks.Children.Add(monthBlock);
            birthdateBlocks.Children.Add(yearBlock);

            StackPanel cityBlocks = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            cityBlocks.Children.Add(cityBlock);

            StackPanel nextBox = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            nextBox.Children.Add(nextButton);

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);

            // Adding controls to grid
            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(cityBlocks);
            MainGrid.Children.Add(birthdateBlocks);
            MainGrid.Children.Add(phoneText);
            MainGrid.Children.Add(boxes);
            MainGrid.Children.Add(addressBoxes);
            MainGrid.Children.Add(phoneBoxes);
            MainGrid.Children.Add(birthdateBoxes);
            MainGrid.Children.Add(nextBox);
            MainGrid.Children.Add(visiblePasswordBox);
            MainGrid.Children.Add(passwordEye);
        }

        /*
        This function set the 'menu' window.
        Input: none
        Output: none
        */
        private void SetMenuWindow()
        {
            SetWindow(420, 400, true);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["brightTitle"], Text = "Choose an option :)" };

            Button createRoomButton = new Button { Style = (Style)Resources["brightButton"], Content = "Create Room" };
            createRoomButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.CREATE_ROOM));
            
            Button joinRoomButton = new Button { Style = (Style)Resources["brightButton"], Content = "Join Room" };
            joinRoomButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.JOIN_ROOM));
           
            Button statisticsButton = new Button { Style = (Style)Resources["brightButton"], Content = "Statistics" };
            statisticsButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.STATISTICS));
            
            Button signoutButton = new Button { Style = (Style)Resources["brightButton"], Content = "Signout" };
            signoutButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

            StackPanel buttons = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            buttons.Children.Add(createRoomButton);
            buttons.Children.Add(joinRoomButton);
            buttons.Children.Add(statisticsButton);
            buttons.Children.Add(signoutButton);

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);

            // Adding controls to grid
            MainGrid.Children.Add(head);
            MainGrid.Children.Add(buttons);
        }

        /*
        This function set the 'create room' window.
        Input: none
        Output: none
        */
        private void SetCreateRoomWindow() 
        {
            SetWindow(480, 400, true);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["brightTitle"], Text = "Enter room details :)" };

            TextBlock roomNameBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Room name", Margin = new Thickness(0, 0, 0, 200) };


            TextBlock roomNameMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "The room name is taken!",
                Foreground = Brushes.White
            };
            ToolTip roomName = new ToolTip();
            roomName.Style = (Style)Resources["ToolTip"];
            roomName.Background = new LinearGradientBrush(Colors.Red, Colors.DarkRed, 90);
            roomName.Content = "Choose your own unique room name!\n" +
                "can contain only letters and numbers.";
            TextBox roomNameBox = new TextBox { Style = (Style)Resources["brightTextBox"], Margin = new Thickness(0, 0, 0, 5), ToolTip = roomName };
            roomNameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(roomNameBlock, roomNameBox));

            TextBlock questionTimeMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "Please choose an option.",
                Foreground = Brushes.White
            };
            ComboBox questionTimeBox = new ComboBox { Style = (Style)Resources["comboStyle"], SelectedIndex = 0, Margin = new Thickness(0, 0, 0, 20) };
            questionTimeBox.Items.Add("-- Time Per Questions --");
            for (int i = 10; i <= 50; i += 10)
            {
                questionTimeBox.Items.Add(i.ToString());
            }
            TextBlock questionNumMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "Please choose an option.",
                Foreground = Brushes.White
            };
            ComboBox questionNumBox = new ComboBox { Style = (Style)Resources["comboStyle"], SelectedIndex = 0, Margin = new Thickness(0, 0, 0, 5) };
            questionNumBox.Items.Add("-- Number Of Questions --");
            for (int i = 5; i <= 20; i += 5)
            {
                questionNumBox.Items.Add(i.ToString());
            }

            TextBlock userMaxMessageBlock = new TextBlock
            {
                Style = (Style)Resources["messageBlock"],
                Text = "Please choose an option.",
                Foreground = Brushes.White
            };
            ComboBox userNumBox = new ComboBox { Style = (Style)Resources["comboStyle"], SelectedIndex = 0, Margin = new Thickness(0, 0, 0, 5) };
            userNumBox.Items.Add("-- Maximum Amount Of Users --");
            for (int i = 2; i <= 10; i++)
            {
                userNumBox.Items.Add(i.ToString());
            }

            List<TextBox> textBoxes = new List<TextBox> { roomNameBox };
            List<ComboBox> comboBoxes = new List<ComboBox> { userNumBox, questionNumBox, questionTimeBox };
            List<TextBlock> textBlocks = new List<TextBlock> { roomNameMessageBlock, userMaxMessageBlock, questionNumMessageBlock, questionTimeMessageBlock };

            Button nextButton = new Button
            {
                Style = (Style)Resources["brightButton"],
                Content = "Next",
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            nextButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ROOM, textBoxes, comboBoxes: comboBoxes, textBlocks: textBlocks));

            Button backButton = new Button
            {
                Style = (Style)Resources["brightButton"],
                Content = "Back",
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            StackPanel boxes = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            boxes.Children.Add(roomNameMessageBlock);
            boxes.Children.Add(roomNameBox);
            boxes.Children.Add(userMaxMessageBlock);
            boxes.Children.Add(userNumBox);
            boxes.Children.Add(questionNumMessageBlock);
            boxes.Children.Add(questionNumBox);
            boxes.Children.Add(questionTimeMessageBlock);
            boxes.Children.Add(questionTimeBox);
            boxes.Children.Add(nextButton);

            StackPanel blocks = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            blocks.Children.Add(roomNameBlock);
            blocks.Children.Add(backButton);


            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);

            // Adding controls to grid
            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(boxes);
        }

        /*
        This function set the 'join room' window.
        Input: none
        Output: none
        */
        private void SetJoinRoomWindow()
        {
            SetWindow(550, 400, true);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["brightTitle"], Text = "Choose a room to play in :)" };

            ListBox roomsListBox = new ListBox { Style = (Style)Resources["brightListBox"] };
            roomsListBox.MouseDoubleClick += (sender, args) =>
            {
                if (roomsListBox.SelectedItem != null) // Only if a room is selected
                {
                    HandleButtonClick(Windows.ROOM, roomName: roomsListBox.SelectedItem.ToString());
                }
            };

            Button backButton = new Button { Style = (Style)Resources["brightButton"], Content = "Back", Margin = new Thickness(0, 35, 0, 0) };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);
            head.Children.Add(roomsListBox);
            head.Children.Add(backButton);

            // Adding controls to grid
            MainGrid.Children.Add(head);

            // Activating background worker
            _available_rooms_worker.RunWorkerAsync(argument: roomsListBox);
        }

        /*
        This function set the 'room' window.
        Input: room's data
        Output: none
        */
        private void SetRoomWindow(RoomData roomData)
        {
            SetWindow(550, 600, true);

            // Creating controls for window
            string roomAdmin = "";
            try
            {
                _using_communicator.WaitOne();
                roomAdmin = _communicator.getRoomAdmin();
            }
            catch (Exception e)
            {
                this.Close();
                MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
            }
            finally
            {
                _using_communicator.ReleaseMutex();
            }

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["brightTitle"], Margin = new Thickness(0, 0, 0, 10),
                Text = "Room Name: " + roomData.name +  ", Room Admin: " + roomAdmin +  ", \nTime Per Qst: " + roomData.timePerQuestion +
                ", Members Amount: "+ roomData.maxPlayers + ", Qst Amount: " + roomData.questionCount,
                FontSize = 16, Height = 60, Width = 500};

            TextBlock playersBlock = new TextBlock { Style = (Style)Resources["brightTitle"], Text = "Room members are: ",
                FontSize = 16, Margin = new Thickness(0, 0, 0, 10) };

            ListBox playersListBox = new ListBox { Style = (Style)Resources["brightListBox"] , Height = 200};

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);
            head.Children.Add(playersBlock);
            head.Children.Add(playersListBox);

            // Adding control to grid
            MainGrid.Children.Add(head);

            Button startButton, closeButton, leaveButton;
            leaveButton = new Button { Style = (Style)Resources["brightButton"], Content = "Leave Room", Margin = new Thickness(0, 30, 0, 0) };
            leaveButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU, closeRoom: false));

            // Activating background worker
            _room_state_worker.RunWorkerAsync(argument: new Tuple<ListBox, Button, bool>(playersListBox, leaveButton, _username == roomAdmin));

            if (_username == roomAdmin) // if admin
            {
                startButton = new Button { Style = (Style)Resources["brightButton"], Content = "Start Game", Margin = new Thickness(0, 0, 40, 30),
                    HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom};
                startButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.GAME));

                closeButton = new Button { Style = (Style)Resources["brightButton"], Content = "Close Room", Margin = new Thickness(40, 0, 0, 30),
                    HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom };
                closeButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU, closeRoom:true));

                MainGrid.Children.Add(startButton);
                MainGrid.Children.Add(closeButton);
            }
            else // if not admin
            {
                head.Children.Add(leaveButton);
            }
        }

        /*
        This function set the 'statistics' window.
        Input: none
        Output: none
        */
        private void SetStatisticsWindow()
        {
            SetWindow(350, 400, true);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["brightTitle"], Text = "Which statistics? :)" };

            Button personalButton = new Button { Style = (Style)Resources["brightButton"], Content = "My Statistics" };
            personalButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.USER_STATISTICS));
            
            Button highScoresButton = new Button { Style = (Style)Resources["brightButton"], Content = "High Scores" };
            highScoresButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.HIGH_SCORES));
           
            Button backButton = new Button { Style = (Style)Resources["brightButton"], Content = "Back" };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            StackPanel buttons = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            buttons.Children.Add(personalButton);               
            buttons.Children.Add(highScoresButton);             
            buttons.Children.Add(backButton);                   

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);

            // Adding controls to grid
            MainGrid.Children.Add(head);
            MainGrid.Children.Add(buttons);
        }

        /*
        This function set the 'my statistics' window.
        Input: statistics
        Output: none
        */
        private void SetMyStatisticsWindow(List<string> statistics)
        {
            SetWindow(515, 450, true);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock totalAnswersBlock = new TextBlock { Style = (Style)Resources["brightText"], Text = "Total Answers: " + statistics[0]};
            
            TextBlock correctAnswersBlock = new TextBlock { Style = (Style)Resources["brightText"], Text = "Correct Answers: " + statistics[1] };
            
            TextBlock incorrectAnswersBlock = new TextBlock { Style = (Style)Resources["brightText"], Text = "Incorrect Answers: " + statistics[2] };
            
            TextBlock avgTimeBlock = new TextBlock { Style = (Style)Resources["brightText"], Text = "Avg Time Per Answer: " + statistics[3] };
           
            TextBlock totalGamesBlock = new TextBlock { Style = (Style)Resources["brightText"], Text = "Total Games: " + statistics[4] };

            Button backButton = new Button { Style = (Style)Resources["brightButton"], Content = "Back" };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.STATISTICS));

            StackPanel blocks = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            blocks.Children.Add(totalAnswersBlock);
            blocks.Children.Add(correctAnswersBlock);
            blocks.Children.Add(incorrectAnswersBlock);
            blocks.Children.Add(avgTimeBlock);
            blocks.Children.Add(totalGamesBlock);
            blocks.Children.Add(backButton);

            StackPanel head = new StackPanel();
            head.Children.Add(logo);

            // Adding controls to grid
            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
        }

        /*
        This function set the 'high scores' window.
        Input: Dictionary of top users and their scores
        Output: none
        */
        private void SetHighScoresWindow(Dictionary<string, string> highScores)
        {
            SetWindow(390, 450, true);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock firstScoreBlock = new TextBlock { Style = (Style)Resources["leftBrightText"]};
            
            TextBlock secondScoreBlock = new TextBlock { Style = (Style)Resources["leftBrightText"]};
            
            TextBlock thirdScoreBlock = new TextBlock { Style = (Style)Resources["leftBrightText"]};
            
            TextBlock fourthScoreBlock = new TextBlock { Style = (Style)Resources["leftBrightText"]};
            
            TextBlock fifthScoreBlock = new TextBlock { Style = (Style)Resources["leftBrightText"]};

            TextBlock[] textBlocks = new TextBlock[5]{ firstScoreBlock, secondScoreBlock, thirdScoreBlock, fourthScoreBlock, fifthScoreBlock};

            for (int i = 0; i < highScores.Count(); i++)
            {
                textBlocks[i].Text = (i+1).ToString() + ". " + highScores.ElementAt(i).Key + " --> " + highScores.ElementAt(i).Value;
            }

            Button backButton = new Button { Style = (Style)Resources["brightButton"], Content = "Back",
                VerticalAlignment = VerticalAlignment.Bottom };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.STATISTICS));

            StackPanel blocks = new StackPanel { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
            blocks.Children.Add(firstScoreBlock);
            blocks.Children.Add(secondScoreBlock);
            blocks.Children.Add(thirdScoreBlock);
            blocks.Children.Add(fourthScoreBlock);
            blocks.Children.Add(fifthScoreBlock);

            StackPanel head = new StackPanel();
            head.Children.Add(logo);

            // Adding controls to grid
            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(backButton);
        }

        /*
        This function set the 'game' window.
        Input: question counter, question amount, correct answers counter, time for question, the question, list of the answers and their id
        Output: none
        */
        private void SetGameWindow(uint currQuestionNum, uint questionAmount, uint correctAnswers, uint timeForQue, string question, Dictionary<uint, string> answers, bool useHelp)
        {
            SetWindow(600, 900, true);

            System.Media.SoundPlayer player = new System.Media.SoundPlayer("..\\..\\Resources\\"+timeForQue.ToString()+"_sec.wav");

            bool help = false;
            uint timePerQuestion = timeForQue;
            DispatcherTimer timer = new DispatcherTimer();
            uint selectedId = 0, currTime = timePerQuestion;
            StackPanel head = new StackPanel();
            StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center};

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };
            TextBlock gameProgressBlock = new TextBlock { Style = (Style)Resources["gameProgressText"],
                Text = "Game Progress: " + currQuestionNum.ToString() + "/" + questionAmount.ToString() + "\nCorrect Answers: " + correctAnswers.ToString() };

            TextBlock timeBlock = new TextBlock { Style = (Style)Resources["gameTimerText"], Text = (++timeForQue - 1).ToString() };

            TextBlock questionBlock = new TextBlock { Style = (Style)Resources["gameQuestionText"], Text = question };

            int[] indexs = { 0, 1, 2, 3 };
            Random r = new Random();
            indexs = indexs.OrderBy(x => r.Next()).ToArray();

            ListBox answersListBox = new ListBox { Style = (Style)Resources["brightListBox"], Width = 810, Height = 300, HorizontalAlignment = HorizontalAlignment.Center };
            answersListBox.MouseUp += (sender, args) =>
                {
                    if (timeForQue > 1 && answersListBox.SelectedItem != null)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (((TextBlock)answersListBox.SelectedItem).Text == answers.ElementAt(i).Value)
                            {
                                selectedId = answers.ElementAt(i).Key;
                                break;
                            }
                        }
                        currTime -= (timeForQue-1);
                        timeForQue = 1;
                    }
                };

            answersListBox.Items.Add(new TextBlock { Style = (Style)Resources["brightText"], Height = 60, Text = answers.ElementAt(indexs[0]).Value, Width = 790, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 0) });
            answersListBox.Items.Add(new TextBlock { Style = (Style)Resources["brightText"], Height = 60, Text = answers.ElementAt(indexs[1]).Value, Width = 790, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 0) });
            answersListBox.Items.Add(new TextBlock { Style = (Style)Resources["brightText"], Height = 60, Text = answers.ElementAt(indexs[2]).Value, Width = 790, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 0) });
            answersListBox.Items.Add(new TextBlock { Style = (Style)Resources["brightText"], Height = 60, Text = answers.ElementAt(indexs[3]).Value, Width = 790, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 0) });

            Button leaveButton = new Button { Style = (Style)Resources["brightButton"], Content = "Leave Game", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            leaveButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, args) =>
                    {
                        if(_currWindow != Windows.GAME)
                        {
                            player.Stop();
                        }
                        timeBlock.Text = (timeForQue-1).ToString();
                        if (timeForQue == 1)
                        {
                            timeBlock.Text = "Timeout!";

                            uint correctAnswerId = 0;
                            if (help)
                            {
                                if (_currWindow == Windows.GAME)
                                {
                                    _using_communicator.WaitOne();
                                    correctAnswerId = _communicator.submitAnswer(selectedId, currTime + 5);
                                    _using_communicator.ReleaseMutex();
                                }
                            }
                            else
                            {
                                if (_currWindow == Windows.GAME)
                                {
                                    _using_communicator.WaitOne();
                                    correctAnswerId = _communicator.submitAnswer(selectedId, currTime);
                                    _using_communicator.ReleaseMutex();
                                }
                            }
                           
                            for(int i = 0; i < 4; i++)
                            {
                                if(((TextBlock)answersListBox.Items[i]).Text == answers.ElementAt(0).Value)
                                {
                                    ((TextBlock)answersListBox.Items[i]).Background = new SolidColorBrush(Colors.LightGreen);
                                    break;
                                }
                            }
                            if(selectedId != 0)
                            {
                                timeBlock.Text = (timePerQuestion - currTime).ToString();
                                timeForQue--;

                                if (correctAnswerId == selectedId)
                                {
                                    correctAnswers++;
                                }
                            }
                        }
                        else if (timeForQue == 0)
                        {
                            player.Stop();
                            timer.Stop();
                            if (currQuestionNum < questionAmount && _currWindow == Windows.GAME)
                            {
                                _using_communicator.WaitOne();
                                GetQuestionRes nextQuestion = _communicator.getQuestion();
                                _using_communicator.ReleaseMutex();
                                SetGameWindow(currQuestionNum + 1, questionAmount, correctAnswers, timePerQuestion,  nextQuestion.question, nextQuestion.answers, useHelp);
                            }
                            else if (_currWindow == Windows.GAME)
                            {
                                SetGameResultsWindow();
                            }
                        }
                        if(timeForQue != 0 && selectedId == 0)
                        {
                            timeForQue--;
                        }
                    };
            timer.Start(); // Starting timer for question
            Thread.Sleep(1500);
            player.Play();

            head.Children.Add(logo);
            head.Children.Add(questionBlock);
            head.Children.Add(answersListBox);

            Button helpButton = new Button();
            if (!useHelp)
            {
                helpButton = new Button { Style = (Style)Resources["Button"], Content = "50/50", 
                    HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top, };
                helpButton.Click += (sender, args) =>
                {
                    uint counter = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (((TextBlock)answersListBox.Items[i]).Text != answers.ElementAt(0).Value)
                        {
                            answersListBox.Items.Remove(answersListBox.Items[i]);
                            if (++counter == 2)
                            {
                                break;
                            }
                        }
                    }

                    leaveButton.HorizontalAlignment = HorizontalAlignment.Center;
                    head.Children.Remove(buttons);
                    buttons.Children.Clear();

                    head.Children.Add(leaveButton);
                    useHelp = true;
                    help = true;
                };

                buttons.Children.Add(leaveButton);
                buttons.Children.Add(helpButton);
                head.Children.Add(buttons);
            }
            else
            {
                leaveButton.HorizontalAlignment = HorizontalAlignment.Center;
                head.Children.Add(leaveButton);
            }

            // Adding controls to grid
            MainGrid.Children.Add(timeBlock);
            MainGrid.Children.Add(gameProgressBlock);
            MainGrid.Children.Add(head);
        }

        /*
        This function set the 'game results' window.
        Input: none
        Output: none
        */
        private void SetGameResultsWindow()
        {
            SetWindow(600, 500, true);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };
            TextBlock waitingBlock = new TextBlock { Style = (Style)Resources["bigBrightText"], Text = "Waiting for other players to finish... :)" };
            ListBox resultsListBox = new ListBox { Style = (Style)Resources["brightListBox"], Width = 450 };

            Button backButton = new Button { Style = (Style)Resources["brightButton"], Content = "Back to Menu" };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(waitingBlock);
            head.Children.Add(resultsListBox);
            head.Children.Add(backButton);

            // Adding controls to grid
            MainGrid.Children.Add(head);

            // Activating background worker
            _game_results_worker.RunWorkerAsync(argument: new Tuple<StackPanel, TextBlock, ListBox>(head, waitingBlock, resultsListBox));
        }

        /*
        This function handles TextBlocks' visibility,
         that are in use to help the user with inserting input.
        Input: the TextBlock to handle, the TextBox to determine visibility.
        Output: none
        */
        public void HandleBlockOutput(TextBlock textBlock, TextBox textBox, TextBlock textBlock2 = null, List<TextBox> textBoxes = null)
        {
            if (textBox.BorderBrush == Brushes.Red)
            { // for wrong input that were corrected
                textBlock2.Visibility = Visibility.Hidden;
                if (textBoxes != null)
                {
                    foreach (TextBox box in textBoxes)
                    {
                        box.BorderBrush = Brushes.DarkBlue;
                    }
                }
                else
                {
                    textBox.BorderBrush = Brushes.DarkBlue;
                }
            }
            if (textBox.Text != "") // If the user inserts input
            {
                textBlock.Visibility = Visibility.Hidden;
            }
            else // If the input TextBox is empty
            {
                textBlock.Visibility = Visibility.Visible;
            }
     }
        
        /*
        This function handles TextBlocks' visibility,
         that are in use to help the user with inserting password input.
        Input: the TextBlock to handle, the PasswordBox to determine visibility.
        Output: none
        */
        public void HandleBlockOutput(TextBlock textBlock, PasswordBox passwordBox, TextBlock textBlock2 = null, TextBox hidden = null)
        {
            if (passwordBox.BorderBrush == Brushes.Red)
            { // for wrong input that were corrected
                if (hidden != null)
                {
                    hidden.BorderBrush = Brushes.DarkBlue;
                }
                passwordBox.BorderBrush = Brushes.DarkBlue;
                textBlock2.Visibility = Visibility.Hidden;
            }
            if (passwordBox.Password != "") // If the user inserts password input
            {
                textBlock.Visibility = Visibility.Hidden;
            }
            else // If the input PasswordBox is empty
            {
                textBlock.Visibility = Visibility.Visible;
            }
        }

        /*
        This function handles all button clicks, and is the main function to manage ui,
         and connect between the ui input and the code behind.
        Input: nextWindow - the next window to set, if the current act will be performed successfully
               Optionals: textBoxes - list of TextBox to get input values
                          passwordBox - PasswordBox to get password input values
                          roomName - string that contains the current room's name
                          closeRoom - boolean to determine if a request is close/leave room
        Output: none
        */
        public void HandleButtonClick(Windows nextWindow, List<TextBox> textBoxes = null, PasswordBox passwordBox = null, 
            string roomName = null, bool closeRoom = true, List<TextBlock> textBlocks = null, List<ComboBox> comboBoxes = null)
        {
            try
            {
                switch (nextWindow)
                {
                    case Windows.ENTRY:
                        if (_currWindow == Windows.MENU)
                        {
                            _using_communicator.WaitOne();
                            _communicator.logout();
                            _using_communicator.ReleaseMutex();
                        }
                        _currWindow = Windows.ENTRY;
                        SetEntryWindow();
                        break;

                    case Windows.LOGIN:
                        _currWindow = Windows.LOGIN;
                        SetLoginWindow();
                        break;

                    case Windows.SIGNUP:
                        _currWindow = Windows.SIGNUP;
                        SetSignupWindow();
                        break;

                    case Windows.MENU:
                        if (_currWindow == Windows.LOGIN)
                        {
                            if (textBoxes[0].Text.Length == 0)
                            {
                                textBoxes[0].BorderBrush = Brushes.Red;
                                textBlocks[1].Text = "Please enter an user name.";
                                textBlocks[1].Visibility = Visibility.Visible;
                            }
                            else if (checkUsername(textBoxes[0].Text, textBlocks[1]))
                            { // name doesn't exists
                                textBoxes[0].BorderBrush = Brushes.Red;
                                textBlocks[1].Text = "The user name doesn't exists in our lists.";
                                textBlocks[1].Visibility = Visibility.Visible;
                            }
                            else
                            {
                                _using_communicator.WaitOne();
                                LoginStatus login = _communicator.login(textBoxes[0].Text, passwordBox.Password);
                                _using_communicator.ReleaseMutex();

                                switch (login)
                                {
                                    case LoginStatus.SUCCESS:
                                        _username = textBoxes[0].Text;
                                        _currWindow = Windows.MENU;
                                        SetMenuWindow();
                                        break;
                                    case LoginStatus.ALREADYINGAME:
                                        textBoxes[0].BorderBrush = Brushes.Red;
                                        textBlocks[1].Text = "This account already logged in.";
                                        textBlocks[1].Visibility = Visibility.Visible;
                                        break;
                                    case LoginStatus.WRONGPASSWORD:
                                        textBoxes[1].BorderBrush = Brushes.Red;
                                        passwordBox.BorderBrush = Brushes.Red;
                                        textBlocks[0].Visibility = Visibility.Visible;
                                        break;
                                    case LoginStatus.WRONGUSERNAME:
                                        textBoxes[0].BorderBrush = Brushes.Red;
                                        textBlocks[1].Text = "The user name doesn't exists in our lists.";
                                        textBlocks[1].Visibility = Visibility.Visible;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else if (_currWindow == Windows.SIGNUP)
                        {
                            string day = "";
                            string month = "";
                            string year = "";
                            bool problem = false;
                            if (!checkDate(textBoxes[6].Text, textBoxes[7].Text, textBoxes[8].Text))
                            { // birthdate
                                textBlocks[4].Visibility = Visibility.Visible;
                                textBoxes[6].BorderBrush = Brushes.Red; // day
                                textBoxes[7].BorderBrush = Brushes.Red; // month
                                textBoxes[8].BorderBrush = Brushes.Red; // year
                                problem = true;
                            }
                            else
                            {
                                day = textBoxes[6].Text.Length == 1 ? "0" + textBoxes[6].Text : textBoxes[6].Text;
                                month = textBoxes[7].Text.Length == 1 ? "0" + textBoxes[7].Text : textBoxes[7].Text;
                                for (int i = 0; i < textBoxes[6].Text.Length; i++)
                                {
                                    year += "0";
                                }
                                year += textBoxes[8].Text;
                            }
                            if (!checkUsername(textBoxes[0].Text, textBlocks[5]))
                            { // name
                                textBlocks[5].Visibility = Visibility.Visible;
                                textBoxes[0].BorderBrush = Brushes.Red;
                                problem = true;
                            }
                            if (!checkPassword(passwordBox.Password))
                            { // password
                                textBlocks[0].Visibility = Visibility.Visible;
                                passwordBox.BorderBrush = Brushes.Red;
                                textBoxes[9].BorderBrush = Brushes.Red;
                                problem = true;
                            }
                            if (!checkEmail(textBoxes[1].Text))
                            { // email
                                textBlocks[1].Visibility = Visibility.Visible;
                                textBoxes[1].BorderBrush = Brushes.Red;
                                problem = true;
                            }
                            if (!checkAddress(textBoxes[2].Text, textBoxes[3].Text, textBoxes[4].Text))
                            { // address
                                textBlocks[2].Visibility = Visibility.Visible;
                                textBoxes[2].BorderBrush = Brushes.Red; // street
                                textBoxes[3].BorderBrush = Brushes.Red; // apartment
                                textBoxes[4].BorderBrush = Brushes.Red; // city
                                problem = true;
                            }
                            if (comboBoxes[0].Text.Length == 0)
                            {
                                textBlocks[3].Visibility = Visibility.Visible;
                                textBlocks[3].Text = "Enter a prefix.";
                                problem = true;
                            }
                            else
                            {
                                textBlocks[3].Visibility = Visibility.Hidden;
                            }
                            if (!checkPhone(textBoxes[5].Text))
                            { // phone
                                textBlocks[3].Visibility = Visibility.Visible;
                                textBoxes[5].BorderBrush = Brushes.Red;
                                textBlocks[3].Text = "Your phone is invalid.";
                                problem = true;
                            }

                            if (!problem)
                            {
                                _using_communicator.WaitOne();
                                int signup = _communicator.signup(textBoxes[0].Text, passwordBox.Password, textBoxes[1].Text,
                                    string.Format("{0}, {1}, {2}", textBoxes[2].Text, textBoxes[3].Text, textBoxes[4].Text),
                                    string.Format("{0}-{1}", comboBoxes[0].Text, textBoxes[5].Text),
                                    string.Format("{0}.{1}.{2}", day, month, year));
                                _using_communicator.ReleaseMutex();

                                if (signup == 1)
                                {
                                    _username = textBoxes[0].Text;
                                    _currWindow = Windows.MENU;
                                    SetMenuWindow();
                                }
                            }
                            
                        }
                        else if (_currWindow == Windows.ROOM)
                        {
                            if (closeRoom)
                            {
                                _using_communicator.WaitOne();
                                bool isRoomClosed = _communicator.closeRoom();
                                _using_communicator.ReleaseMutex();

                                if (!isRoomClosed)
                                {
                                    MessageBoxResult result = MessageBox.Show("Failed to close room", "Trivia",
                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                    break;
                                }
                            }
                            else
                            {
                                _using_communicator.WaitOne();
                                bool leave_room = _communicator.leaveRoom();
                                _using_communicator.ReleaseMutex();

                                if (!leave_room)
                                {
                                    MessageBoxResult result = MessageBox.Show("Failed to leave room", "Trivia",
                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                    break;
                                }
                            }
                            _currWindow = Windows.MENU;
                            SetMenuWindow();
                        }
                        else if (_currWindow == Windows.GAME)
                        {
                            _using_communicator.WaitOne();
                            bool isLeftGame = _communicator.leaveGame();
                            _using_communicator.ReleaseMutex();

                            if (!isLeftGame)
                            {
                                MessageBoxResult result = MessageBox.Show("Failed to leave game", "Trivia",
                                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                break;
                            }
                            _currWindow = Windows.MENU;
                            SetMenuWindow();
                        }
                        else
                        {
                            _currWindow = Windows.MENU;
                            SetMenuWindow();
                        }
                        break;

                    case Windows.CREATE_ROOM:
                        _currWindow = Windows.CREATE_ROOM;
                        SetCreateRoomWindow();
                        break;

                    case Windows.JOIN_ROOM:
                        _currWindow = Windows.JOIN_ROOM;
                        SetJoinRoomWindow();
                        break;

                    case Windows.ROOM:
                        if (_currWindow == Windows.CREATE_ROOM)
                        {
                            for (int i = 0; i < textBlocks.Count; i++)
                            {
                                if (textBlocks[i].Visibility == Visibility.Visible)
                                {
                                    textBlocks[i].Visibility = Visibility.Hidden;
                                }
                            }
                            bool success = true;
                            if (comboBoxes[0].Text[0] == '-')
                            { // check the user max box
                                textBlocks[1].Visibility = Visibility.Visible;
                                success = false;
                            }
                            if (comboBoxes[1].Text[0] == '-')
                            { // check the number of question box
                                textBlocks[2].Visibility = Visibility.Visible;
                                success = false;
                            }
                            if (comboBoxes[2].Text[0] == '-')
                            { // check the time per question box
                                textBlocks[3].Visibility = Visibility.Visible;
                                success = false;
                            }
                            if (textBoxes[0].Text.Length == 0)
                            { // check the room name box
                                textBlocks[0].Text = "Please enter a name";
                                textBlocks[0].Visibility = Visibility.Visible;
                                success = false;
                            }
                            foreach (char ch in textBoxes[0].Text)
                            {
                                if (!isDigit(ch) && !isLower(ch) && !isUpper(ch))
                                {
                                    textBlocks[0].Text = "Invalid room name.";
                                    textBlocks[0].Visibility = Visibility.Visible;
                                    success = false;
                                    break;
                                }
                            }
                            _using_communicator.WaitOne();
                            List<RoomData> rooms = _communicator.getAvailableRooms();
                            _using_communicator.ReleaseMutex();
                            foreach (RoomData room in rooms)
                            {
                                if (room.name == textBoxes[0].Text)
                                {
                                    textBlocks[0].Text = "Your room name is taken.";
                                    textBlocks[0].Visibility = Visibility.Visible;
                                    success = false;
                                    break;
                                }
                            }
                            if (success)
                            {
                                _using_communicator.WaitOne();
                                bool create_room = _communicator.createRoom(textBoxes[0].Text, comboBoxes[0].Text, comboBoxes[1].Text, comboBoxes[2].Text);
                                _using_communicator.ReleaseMutex();

                                if (create_room)
                                {
                                    RoomData temp;
                                    temp.id = 0;
                                    temp.name = textBoxes[0].Text;
                                    temp.maxPlayers = UInt32.Parse(comboBoxes[0].Text);
                                    temp.questionCount = UInt32.Parse(comboBoxes[1].Text);
                                    temp.timePerQuestion = UInt32.Parse(comboBoxes[2].Text);
                                    temp.isActive = (uint)ActiveMode.WAITING;

                                    _currWindow = Windows.ROOM;
                                    SetRoomWindow(temp);
                                }
                                else
                                {
                                    textBlocks[0].Text = "The room name is taken";
                                    textBlocks[0].Visibility = Visibility.Visible;
                                }
                            }
                        }
                        else if (_currWindow == Windows.JOIN_ROOM)
                        {
                            _using_communicator.WaitOne();
                            RoomData r = _communicator.getRoomData(roomName.Split(" >".ToCharArray())[0]);
                            _using_communicator.ReleaseMutex();

                            if (r.name != "")
                            {
                                _using_communicator.WaitOne();
                                bool join_room = _communicator.joinRoom(roomName);
                                _using_communicator.ReleaseMutex();

                                if (join_room)
                                {
                                    _currWindow = Windows.ROOM;
                                    SetRoomWindow(r);
                                }
                                else
                                {
                                    MessageBoxResult result = MessageBox.Show("Faild to join room. \nTry again :)", "Trivia",
                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                }
                            }
                            else
                            {
                                MessageBoxResult result = MessageBox.Show("Faild to join room. \nTry again :)", "Trivia",
                                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                            }
                        }
                        break;

                    case Windows.STATISTICS:
                        _currWindow = Windows.STATISTICS;
                        SetStatisticsWindow();
                        break;

                    case Windows.USER_STATISTICS:
                        _using_communicator.WaitOne();
                        List<string> userStatistics = _communicator.getUserStatistics();
                        _using_communicator.ReleaseMutex();

                        if (userStatistics == null)
                        {
                            MessageBoxResult result = MessageBox.Show("There were some errors ): \nGoing back to menu.", "Trivia",
                                MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                            _currWindow = Windows.MENU;
                            SetMenuWindow();
                        }
                        else
                        {
                            _currWindow = Windows.USER_STATISTICS;
                            SetMyStatisticsWindow(userStatistics);
                        }
                        break;

                    case Windows.HIGH_SCORES:
                        _using_communicator.WaitOne();
                        Dictionary<string, string> highScores = _communicator.getHighScores();
                        _using_communicator.ReleaseMutex();

                        if (highScores == null)
                        {
                            MessageBoxResult result = MessageBox.Show("There were some errors ): \nGoing back to menu.", "Trivia",
                                MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                            _currWindow = Windows.MENU;
                            SetMenuWindow();
                        }
                        else
                        {
                            _currWindow = Windows.HIGH_SCORES;
                            SetHighScoresWindow(highScores);
                        }
                        break;

                    case Windows.GAME:
                        _currWindow = Windows.GAME;
                        _using_communicator.WaitOne();
                        GetRoomStateRes roomState = _communicator.getRoomState();
                        bool started = _communicator.startGame();
                        _using_communicator.ReleaseMutex();

                        if(!started)
                        {
                            _currWindow = Windows.ROOM;
                            MessageBoxResult result = MessageBox.Show("Faild to start game :( \nTry again.", "Trivia",
                                MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            break;
                        }
                        _using_communicator.WaitOne();
                        GetQuestionRes firstQuestion = _communicator.getQuestion();
                        _using_communicator.ReleaseMutex();
                        SetGameWindow(1, roomState.questionCount, 0, roomState.answerTimeout, firstQuestion.question, firstQuestion.answers, false);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                _using_communicator.ReleaseMutex();
                this.Close();
                MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
            }
        }

        /*
        This function is the 'DoWork' function of '_available_rooms_worker',
         and it keeps checking for new rooms, by communicating the server.
        Input: sender, e - the event's args - including ui elements sent from sender
        Output: none
        */
        private void getAvailableRooms(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    List<string> roomNames = new List<string>();
                    _using_communicator.WaitOne();
                    List<RoomData> currRooms = _communicator.getAvailableRooms();
                    _using_communicator.ReleaseMutex();

                    foreach (RoomData room in currRooms)
                    {
                        _using_communicator.WaitOne();
                        int loggedPlayersAmount = _communicator.getPlayersInRoom(room.id);
                        _using_communicator.ReleaseMutex();

                        roomNames.Add(room.name + " >>> (" + loggedPlayersAmount.ToString() + "/ " + room.maxPlayers.ToString() + ")");
                    }

                    _available_rooms_worker.ReportProgress(0, new Tuple<List<string>, ListBox>(roomNames, (ListBox)e.Argument));

                    System.Threading.Thread.Sleep(1000); // Checks every second

                    if (_currWindow != Windows.JOIN_ROOM) // Works only if the rooms list is displayed
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _using_communicator.ReleaseMutex();
                //MessageBoxResult result = MessageBox.Show(ex.Message, "Trivia",
                  //  MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
            }
        }

        /*
        This function is the 'ProgressChanged' function of '_available_rooms_worker',
         and it updates the ui, by updating the rooms list.
        Input: sender, e - the event's args - including ui elements sent from sender
        Output: none
        */
        private void update_rooms_list(object sender, ProgressChangedEventArgs e)
        {
            Tuple<List<string>, ListBox> rooms = (Tuple<List<string>, ListBox>)e.UserState;
            rooms.Item2.Items.Clear(); // clears rooms list

            foreach (string room in rooms.Item1) // inserting current rooms
            {
                rooms.Item2.Items.Add(room);
            }
        }

        /*
        This function is the 'DoWork' function of '_room_state_worker',
         and it keeps checking for the current room's state, by communicating the server.
        Input: sender, e - the event's args - including ui elements sent from sender
        Output: none
        */
        private void checkRoomState(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    _using_communicator.WaitOne();
                    GetRoomStateRes state = _communicator.getRoomState();
                    _using_communicator.ReleaseMutex();

                    Tuple<ListBox, Button, bool> tmp = (Tuple<ListBox, Button, bool>)e.Argument;
                    Tuple<ListBox, Button, bool, GetRoomStateRes> args = new Tuple<ListBox, Button, bool, GetRoomStateRes>(tmp.Item1, tmp.Item2, tmp.Item3, state);

                    _room_state_worker.ReportProgress(0, args);
                    System.Threading.Thread.Sleep(1000); // Checks every second

                    if (_currWindow != Windows.ROOM) // Works only if the user is in a room
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _using_communicator.ReleaseMutex();
            }
        }

        /*
        This function is the 'ProgressChanged' function of '_room_state_worker',
         and it updates the ui, by updating the players list and the window settings.
        Input: sender, e - the event's args - including ui elements sent from sender
        Output: none
        */
        private void update_room_window(object sender, ProgressChangedEventArgs e)
        {
            Tuple<ListBox, Button, bool, GetRoomStateRes> args = (Tuple<ListBox, Button, bool, GetRoomStateRes>)e.UserState;

            if (args.Item4.status == (uint)ActiveMode.DONE && !args.Item3) // room was closed by admin
            {
                args.Item2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); // Clicks the 'leaveButton' button and performs leave request
                MessageBoxResult result = MessageBox.Show("Admin closed the room :(", "Trivia",
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                return;
            }
            else if(args.Item4.status == (uint)ActiveMode.START_PLAYING && !args.Item3)
            {   
                _using_communicator.WaitOne();
                _communicator.leaveRoom();
                GetQuestionRes firstQuestion = _communicator.getQuestion();
                _using_communicator.ReleaseMutex();
                _currWindow = Windows.GAME;
                SetGameWindow(1, args.Item4.questionCount, 0, args.Item4.answerTimeout, firstQuestion.question, firstQuestion.answers, false);
                return;
            }

            args.Item1.Items.Clear(); // clears players list

            foreach (string player in args.Item4.players) // inserting current players
            {
                args.Item1.Items.Add(player);
            }
        }

        /*
        This function is the 'DoWork' function of '_game_results_worker',
         and it keeps checking for the current game's results, by communicating the server.
        Input: sender, e - the event's args - including ui elements sent from sender
        Output: none
        */
        private void checkForGameResults(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<PlayerResults> gameResults;

                Tuple<StackPanel, TextBlock, ListBox> tmp = (Tuple<StackPanel, TextBlock, ListBox>)e.Argument;

                while (true)
                {
                    _using_communicator.WaitOne();
                    gameResults = _communicator.getGameResults();
                    _using_communicator.ReleaseMutex();

                    if (gameResults.Count() > 0)
                    {
                        Tuple<StackPanel, TextBlock, ListBox, List<PlayerResults>> args =
                            new Tuple<StackPanel, TextBlock, ListBox, List<PlayerResults>>(tmp.Item1, tmp.Item2, tmp.Item3, gameResults);

                        _game_results_worker.ReportProgress(0, args);
                        break;
                    }
                    
                    System.Threading.Thread.Sleep(1000); // Checks every second

                    if (_currWindow != Windows.GAME) // Works only if the user is in a game
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _using_communicator.ReleaseMutex();
                MessageBoxResult result = MessageBox.Show(ex.Message, "Trivia",
                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
            }
        }

        /*
        This function is the 'ProgressChanged' function of '_game_results_worker',
         and it updates the ui, by updating the players list.
        Input: sender, e - the event's args - including ui elements sent from sender
        Output: none
        */
        private void update_game_results_window(object sender, ProgressChangedEventArgs e)
        {
            Tuple<StackPanel, TextBlock, ListBox, List<PlayerResults>> args = (Tuple<StackPanel, TextBlock, ListBox, List<PlayerResults>>)e.UserState;

            args.Item1.Children.Remove(args.Item2);
            foreach (PlayerResults p in args.Item4)
            {
                args.Item3.Items.Add(p.username + ": " + (1 / p.averageAnswerTime * p.correctAnswerCount / (p.correctAnswerCount + p.wrongAnswerCount) * 100).ToString());
            }
        }

        /*
        This function checks if a char is a lower case letter
        Input: character
        Output: true - char is lower letter, false - else
        */
        private bool isLower(char ch)
        {
            return ch >= 'a' && ch <= 'z';
        }
        /*
        This function checks if a char is a upper case letter
        Input: character
        Output: true - char is upper letter, false - else
        */
        private bool isUpper(char ch)
        {
            return ch >= 'A' && ch <= 'Z';
        }
        /*
        This function checks if a char is a digit
        Input: character
        Output: true - char is digit, false - else
        */
        private bool isDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        /*
        This function checks if user name is valid
        Input: username, his message text block
        Output: true - name is valid, false - name is invalid or taken
        */
        private bool checkUsername(string username, TextBlock textBlock)
        {
            textBlock.Text = "Your user name is invalid.";

            string pattern = @"^\w+$";
            Regex rgx = new Regex(pattern, RegexOptions.None);

            if (rgx.IsMatch(username) == false)
            {
                return false;
            }

            _using_communicator.WaitOne();
            LoginStatus login = _communicator.login(username, "");
            _using_communicator.ReleaseMutex();

            if (login == LoginStatus.WRONGUSERNAME)
            {
                return true;
            }
            textBlock.Text = "Your user name is taken.";
            return false;
        }

        /*
        This function checks if password is valid
        Input: password
        Output: true - password is valid, false - password is invalid or taken
        */
        private bool checkPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8}$";
            Regex rgx = new Regex(pattern, RegexOptions.None);

            return rgx.IsMatch(password);
        }

        /*
        This function checks if email is valid
        Input: email
        Output: true - email is valid, false - email is invalid or taken
        */
        private bool checkEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9]+@[a-zA-Z]+(\.[a-zA-Z]{2,})+$";
            Regex rgx = new Regex(pattern, RegexOptions.None);

            if (rgx.IsMatch(email) == false)
            {
                return false;
            }

            string domain = email.Split('@')[1];
            try
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry(domain);
            }
            catch (Exception e)
            { // the domain is invalid
                return false;
            }
            return true;
        }

        /*
        This function checks if address is valid
        Input: street, apt, city
        Output: true - address is valid, false - address is invalid or taken
        */
        private bool checkAddress(string street, string apt, string city)
        {
            string pattern = @"^[a-zA-Z]{2,},\ \d+,\ [a-zA-Z\ ]{2,}$";
            Regex rgx = new Regex(pattern, RegexOptions.None);

            return rgx.IsMatch(street + ", " + apt + ", " + city);
        }

        /*
        This function checks if phone is valid
        Input: phone number
        Output: true - phone is valid, false - phone is invalid or taken
        */
        private bool checkPhone(string phone)
        {
            string pattern = @"^\d{7}$";
            Regex rgx = new Regex(pattern, RegexOptions.None);

            return rgx.IsMatch(phone);
        }

        /*
        This function checks if date is valid
        Input: day, month, year
        Output: true - date is valid, false - date is invalid or taken
        */
        private bool checkDate(string day, string month, string year)
        {
            string date = day + "." + month + "." + year;
            DateTime temp;
            string pattern = @"^(((3[0-3]|[0-2]\d)\/(0\d|1[0-2])\/(\d{4}))|((3[0-3]|[0-2]\d)\.(0\d|1[0-2])\.(\d{4})))$";
            Regex rgx = new Regex(pattern, RegexOptions.None);

            if (!rgx.IsMatch(date) || !DateTime.TryParse(date, out temp) || temp > DateTime.Now)
            {
                return false;
            }
            return true;
        }

        private string _username;
        private Windows _currWindow;
        private Communicator _communicator;
        private BackgroundWorker _available_rooms_worker = new BackgroundWorker();
        private BackgroundWorker _room_state_worker = new BackgroundWorker();
        private BackgroundWorker _game_results_worker = new BackgroundWorker();
        Mutex _using_communicator;
    }
}