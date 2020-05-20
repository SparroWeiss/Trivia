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

namespace Trivia_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum Windows
        {
            ENTRY,
            LOGIN,
            SIGNUP,
            MENU,
            CREATE_ROOM,
            JOIN_ROOM,
            ROOM,
            STATISTICS,
            USER_STATISTICS,
            HIGH_SCORES
        }

        public MainWindow()
        {
            InitializeComponent();
            _available_rooms_worker.WorkerSupportsCancellation = true;
            _available_rooms_worker.WorkerReportsProgress = true;
            _available_rooms_worker.DoWork += getAvailableRooms;
            _available_rooms_worker.ProgressChanged += update_rooms_list;
            
            _room_state_worker.WorkerSupportsCancellation = true;
            _room_state_worker.WorkerReportsProgress = true;
            _room_state_worker.DoWork += checkRoomState;
            _room_state_worker.ProgressChanged += update_room_window;

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

        private void SetEntryWindow()
        {
            MainGrid.Children.Clear();
            Height = 250;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Linen, Colors.PaleTurquoise, 90);

            Image logo = new Image { Style = (Style)Resources["darkLogo"], Width = 300 };

            Button loginButton = new Button { Content = "Login", Style = (Style)Resources["darkButton"],
                Height = 50, Width = 170, Margin = new Thickness(15, 0, 0, 20), 
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom };
            loginButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.LOGIN));

            Button signupButton = new Button { Content = "Signup", Style = (Style)Resources["darkButton"],
                Height = 50, Width = 170, Margin = new Thickness(0, 0, 15, 20),
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom };
            signupButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.SIGNUP));

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], 
                Text = "Do you want to login or sign up? :)", FontSize = 14,
                VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.DarkBlue) };

            StackPanel stack = new StackPanel();
            stack.Children.Add(logo);
            stack.Children.Add(messageBlock);

            MainGrid.Children.Add(stack);
            MainGrid.Children.Add(loginButton);
            MainGrid.Children.Add(signupButton);
        }

        private void SetLoginWindow()
        {
            MainGrid.Children.Clear();
            Height = 350;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Linen, Colors.PaleTurquoise, 90);

            Image logo = new Image { Style = (Style)Resources["darkLogo"] };

            TextBlock usernameBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Username" };

            TextBlock passwordBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Password" };

            TextBox usernameBox = new TextBox { Style = (Style)Resources["myTextBox"] };
            usernameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(usernameBlock, usernameBox));
            
            PasswordBox passwordBox = new PasswordBox { Style = (Style)Resources["myPasswordBox"] };
            passwordBox.PasswordChanged += new RoutedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, passwordBox));

            List<TextBox> textBoxes = new List<TextBox> { usernameBox };

            Button nextButton = new Button { Style = (Style)Resources["darkButton"], Content = "Next",
               HorizontalAlignment = HorizontalAlignment.Right };
            nextButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU, textBoxes, passwordBox));
            
            Button backButton = new Button { Style = (Style)Resources["darkButton"], Content = "Back",
                HorizontalAlignment = HorizontalAlignment.Left };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

            StackPanel boxes = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom};
            boxes.Children.Add(usernameBox);
            boxes.Children.Add(passwordBox);
            boxes.Children.Add(nextButton);

            StackPanel blocks = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            blocks.Children.Add(usernameBlock);
            blocks.Children.Add(passwordBlock);
            blocks.Children.Add(backButton);

            MainGrid.Children.Add(logo);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(boxes);
        }

        private void SetSignupWindow()
        {
            MainGrid.Children.Clear();
            Height = 600;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Linen, Colors.PaleTurquoise, 90);

            Image logo = new Image { Style = (Style)Resources["darkLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Enter your details :)",
                FontSize = 14, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.DarkBlue) };

            TextBlock usernameBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Username" };

            TextBlock passwordBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Password" };

            TextBlock emailBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Email" };

            TextBlock addressBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Address (Street, Apt, City)" };

            TextBlock phoneBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Phone" };

            TextBlock birthdateBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Birthdate (DD/MM/YYYY)" };

            TextBox usernameBox = new TextBox { Style = (Style)Resources["myTextBox"] };
            usernameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(usernameBlock, usernameBox));

            PasswordBox passwordBox = new PasswordBox { Style = (Style)Resources["myPasswordBox"] };
            passwordBox.PasswordChanged += new RoutedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, passwordBox));

            TextBox emailBox = new TextBox{ Style = (Style)Resources["myTextBox"] };
            emailBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(emailBlock, emailBox));
            
            TextBox addressBox = new TextBox { Style = (Style)Resources["myTextBox"] };
            addressBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(addressBlock, addressBox));
            
            TextBox phoneBox = new TextBox { Style = (Style)Resources["myTextBox"] };
            phoneBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(phoneBlock, phoneBox));
            
            TextBox birthdateBox = new TextBox { Style = (Style)Resources["myTextBox"] };
            birthdateBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(birthdateBlock, birthdateBox));

            List<TextBox> textBoxes = new List<TextBox>{ usernameBox, emailBox, addressBox, phoneBox, birthdateBox };

            Button nextButton = new Button { Style = (Style)Resources["darkButton"], Content = "Next",
                HorizontalAlignment = HorizontalAlignment.Right };
            nextButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU, textBoxes, passwordBox));
            
            Button backButton = new Button { Style = (Style)Resources["darkButton"], Content = "Back",
                HorizontalAlignment = HorizontalAlignment.Left};
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

            StackPanel boxes = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            boxes.Children.Add(usernameBox);
            boxes.Children.Add(passwordBox);
            boxes.Children.Add(emailBox);
            boxes.Children.Add(addressBox);
            boxes.Children.Add(phoneBox);
            boxes.Children.Add(birthdateBox);
            boxes.Children.Add(nextButton);

            StackPanel blocks = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            blocks.Children.Add(usernameBlock);
            blocks.Children.Add(passwordBlock);
            blocks.Children.Add(emailBlock);
            blocks.Children.Add(addressBlock);
            blocks.Children.Add(phoneBlock);
            blocks.Children.Add(birthdateBlock);
            blocks.Children.Add(backButton);

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);

            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(boxes);
        }

        private void SetMenuWindow()
        {
            MainGrid.Children.Clear();
            Height = 420;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Choose an option :)",
                FontSize = 14, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.Lavender) };

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

            MainGrid.Children.Add(head);
            MainGrid.Children.Add(buttons);
        }

        private void SetCreateRoomWindow() 
        {
            MainGrid.Children.Clear();
            Height = 480;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Enter room details :)",
                FontSize = 14, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.Lavender) };

            TextBlock roomNameBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Room name" };

            TextBlock userNumBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Number of users" };

            TextBlock questionNumBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Number of questions" };

            TextBlock questionTimeBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Time for question (sec)" };

            TextBox roomNameBox = new TextBox { Style = (Style)Resources["myTextBox"], BorderBrush = new SolidColorBrush(Colors.Lavender),
                Foreground = new SolidColorBrush(Colors.Lavender) };
            roomNameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(roomNameBlock, roomNameBox));

            TextBox userNumBox = new TextBox { Style = (Style)Resources["myTextBox"], BorderBrush = new SolidColorBrush(Colors.Lavender),
                Foreground = new SolidColorBrush(Colors.Lavender) };
            userNumBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(userNumBlock, userNumBox));

            TextBox questionNumBox = new TextBox { Style = (Style)Resources["myTextBox"], BorderBrush = new SolidColorBrush(Colors.Lavender),
                Foreground = new SolidColorBrush(Colors.Lavender) };
            questionNumBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(questionNumBlock, questionNumBox));
            
            TextBox questionTimeBox = new TextBox { Style = (Style)Resources["myTextBox"], BorderBrush = new SolidColorBrush(Colors.Lavender),
                Foreground = new SolidColorBrush(Colors.Lavender),  };
            questionTimeBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(questionTimeBlock, questionTimeBox));

            List<TextBox> textBoxes = new List<TextBox> { roomNameBox, userNumBox, questionNumBox, questionTimeBox };

            Button nextButton = new Button { Style = (Style)Resources["brightButton"], Content = "Next",
                Width = 100, HorizontalAlignment = HorizontalAlignment.Right };
            nextButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ROOM, textBoxes));

            Button backButton = new Button { Style = (Style)Resources["brightButton"], Content = "Back",
                Width = 100, HorizontalAlignment = HorizontalAlignment.Left };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            StackPanel boxes = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            boxes.Children.Add(roomNameBox);
            boxes.Children.Add(userNumBox);
            boxes.Children.Add(questionNumBox);
            boxes.Children.Add(questionTimeBox);
            boxes.Children.Add(nextButton);

            StackPanel blocks = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Bottom };
            blocks.Children.Add(roomNameBlock);
            blocks.Children.Add(userNumBlock);
            blocks.Children.Add(questionNumBlock);
            blocks.Children.Add(questionTimeBlock);
            blocks.Children.Add(backButton);

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);

            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(boxes);
        }

        private void SetJoinRoomWindow()
        {
            MainGrid.Children.Clear();
            Height = 550;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Choose a room to play in :)",
                FontSize = 14, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.Lavender) };

            ListBox roomsListBox = new ListBox { Style = (Style)Resources["roomList"] };
            roomsListBox.MouseDoubleClick += new MouseButtonEventHandler((sender, args) => HandleButtonClick(Windows.ROOM, roomName: roomsListBox.SelectedItem.ToString()));

            Button backButton = new Button { Style = (Style)Resources["brightButton"], Content = "Back", Margin = new Thickness(0, 35, 0, 0) };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);
            head.Children.Add(roomsListBox);
            head.Children.Add(backButton);

            MainGrid.Children.Add(head);

            _available_rooms_worker.RunWorkerAsync(argument: roomsListBox);
        }

        private void SetRoomWindow(RoomData roomData)
        {
            MainGrid.Children.Clear();
            Height = 550;
            Width = 600;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            string roomAdmin = _communicator.getRoomAdmin();

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Margin = new Thickness(0, 0, 0, 10),
                Text = "Room Name: " + roomData.name +  ", Room Admin: " + roomAdmin +  ", \nTime Per Qst: " + roomData.timePerQuestion +
                ", Members Amount: "+ roomData.maxPlayers + ", Qst Amount: " + roomData.questionCount,
                FontSize = 16, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.Lavender) , Height = 60, Width = 500};


            TextBlock playersBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Room members are: ",
                FontSize = 16, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.Lavender),
                 Margin = new Thickness(0, 0, 0, 10) };

            ListBox playersListBox = new ListBox { Style = (Style)Resources["roomList"] , Height = 200};

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);
            head.Children.Add(playersBlock);
            head.Children.Add(playersListBox);

            MainGrid.Children.Add(head);

            Button startButton, closeButton, leaveButton;
            leaveButton = new Button { Style = (Style)Resources["brightButton"], Content = "Leave Room", Margin = new Thickness(0, 30, 0, 0) };
            leaveButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU, close_room: false));

            _room_state_worker.RunWorkerAsync(argument: new Tuple<ListBox, Button, bool>(playersListBox, leaveButton, _username == roomAdmin));

            if (_username == roomAdmin) // if admin
            {
                startButton = new Button { Style = (Style)Resources["brightButton"], Content = "Start Game", Margin = new Thickness(0, 0, 40, 30),
                    HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom};
                //startButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick()); // for now no start game option

                closeButton = new Button { Style = (Style)Resources["brightButton"], Content = "Close Room", Margin = new Thickness(40, 0, 0, 30),
                    HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom };
                closeButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU, close_room:true));

                MainGrid.Children.Add(startButton);
                MainGrid.Children.Add(closeButton);
            }
            else // if not admin
            {
                head.Children.Add(leaveButton);
            }
        }

        private void SetStatisticsWindow()
        {
            MainGrid.Children.Clear();
            Height = 350;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Which statistics? :)",
                FontSize = 14, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.Lavender) };

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

            MainGrid.Children.Add(head);
            MainGrid.Children.Add(buttons);
        }

        private void SetMyStatisticsWindow(List<string> statistics)
        {
            MainGrid.Children.Clear();
            Height = 515;
            Width = 450;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock totalAnswersBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Total Answers: " + statistics[0]};
            totalAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            
            TextBlock correctAnswersBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Correct Answers: " + statistics[1] };
            correctAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            
            TextBlock incorrectAnswersBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Incorrect Answers: " + statistics[2] };
            incorrectAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            
            TextBlock avgTimeBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Avg Time Per Answer: " + statistics[3] };
            avgTimeBlock.Foreground = new SolidColorBrush(Colors.Lavender);
           
            TextBlock totalGamesBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Total Games: " + statistics[4] };
            totalGamesBlock.Foreground = new SolidColorBrush(Colors.Lavender);

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

            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
        }

        private void SetHighScoresWindow(Dictionary<string, string> highScores)
        {
            MainGrid.Children.Clear();
            Height = 390;
            Width = 450;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock firstScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0) };
            
            TextBlock secondScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0) };
            
            TextBlock thirdScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0) };
            
            TextBlock fourthScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0) };
            
            TextBlock fifthScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0) };

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

            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(backButton);
        }

        public void HandleBlockOutput(TextBlock textBlock, TextBox textBox)
        {
            if (textBox.Text != "")
            {
                textBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                textBlock.Visibility = Visibility.Visible;
            }
     }

        public void HandleBlockOutput(TextBlock textBlock, PasswordBox passwordBox)
        {
            if (passwordBox.Password != "")
            {
                textBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                textBlock.Visibility = Visibility.Visible;
            }
        }

        public void HandleButtonClick(Windows nextWindow, List<TextBox> textBoxes = null, PasswordBox passwordBox = null, string roomName = null, bool close_room = true)
        {
            switch (nextWindow)
            {
                case Windows.ENTRY:                      // ENTRY V
                    if (_currWindow == Windows.MENU)
                    {
                        try
                        {
                            _communicator.logout();
                            this.Close();
                        }
                        catch (Exception e)
                        {
                            this.Close();
                            MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                                MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
                        }
                    }
                    _currWindow = Windows.ENTRY;
                    SetEntryWindow();
                    break;

                case Windows.LOGIN:                      // LOGIN V
                    _currWindow = Windows.LOGIN;
                    SetLoginWindow();
                    break;

                case Windows.SIGNUP:                     // SIGNUP V
                    _currWindow = Windows.SIGNUP;
                    SetSignupWindow();
                    break;

                case Windows.MENU:
                    switch (_currWindow)
                    {
                        case Windows.LOGIN:
                            try
                            {
                                if (_communicator.login(textBoxes[0].Text, passwordBox.Password))
                                {
                                    _username = textBoxes[0].Text;
                                    _currWindow = Windows.MENU;
                                    SetMenuWindow();
                                }
                                else
                                {
                                    MessageBoxResult result = MessageBox.Show("Some details were invalid. \nTry again :)", "Trivia",
                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                }
                            }
                            catch (Exception e)
                            {
                                this.Close();
                                MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
                            }
                            break;

                        case Windows.SIGNUP:
                            try
                            {
                                if (_communicator.signup(textBoxes[0].Text, passwordBox.Password, textBoxes[1].Text,
                                    textBoxes[2].Text, textBoxes[3].Text, textBoxes[4].Text))
                                {
                                    _username = textBoxes[0].Text;
                                    _currWindow = Windows.MENU;
                                    SetMenuWindow();
                                }
                                else
                                {
                                    MessageBoxResult result = MessageBox.Show("Some details were invalid. \nTry again :)", "Trivia",
                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                }
                            }
                            catch (Exception e)
                            {
                                this.Close();
                                MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
                            }
                            break;

                        case Windows.ROOM:
                            if (close_room)
                            {
                                if (!_communicator.closeRoom())
                                {
                                    MessageBoxResult result = MessageBox.Show("Failed to close room", "Trivia",
                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                    break;
                                }
                            }
                            else
                            {
                                if (!_communicator.leaveRoom())
                                {
                                    MessageBoxResult result = MessageBox.Show("Failed to leave room", "Trivia",
                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                    break;
                                }
                            }
                            _currWindow = Windows.MENU;
                            SetMenuWindow();
                            break;

                        default:
                            _currWindow = Windows.MENU;
                            SetMenuWindow();
                            break;
                    }
                    break;

                case Windows.CREATE_ROOM:                // CREATE ROOM V
                    _currWindow = Windows.CREATE_ROOM;
                    SetCreateRoomWindow();
                    break;

                case Windows.JOIN_ROOM:
                    _currWindow = Windows.JOIN_ROOM;
                    SetJoinRoomWindow();
                    break;

                case Windows.ROOM:
                    switch(_currWindow)
                    {
                        case Windows.CREATE_ROOM:
                            try
                            {
                                if (_communicator.createRoom(textBoxes[0].Text, textBoxes[1].Text, textBoxes[2].Text, textBoxes[3].Text))
                                {
                                    RoomData temp;
                                    temp.id = 0;
                                    temp.name = textBoxes[0].Text;
                                    temp.maxPlayers = UInt32.Parse(textBoxes[1].Text);
                                    temp.questionCount = UInt32.Parse(textBoxes[2].Text);
                                    temp.timePerQuestion = UInt32.Parse(textBoxes[3].Text);
                                    temp.isActive = (uint)ActiveMode.WAITING;

                                    _currWindow = Windows.ROOM;
                                    SetRoomWindow(temp);
                                }
                                else
                                {
                                    MessageBoxResult result = MessageBox.Show("Some details were invalid. \nTry again :)", "Trivia",
                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                                }
                            }
                            catch (Exception e)
                            {
                                this.Close();
                                MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
                            }
                            break;
                        case Windows.JOIN_ROOM:
                            try
                            {

                                RoomData r = _communicator.getRoomData(roomName);
                                if (r.name != "")
                                {
                                    if (_communicator.joinRoom(roomName))
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
                            catch (Exception e)
                            {
                                this.Close();
                                MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
                            }
                            break;
                        default:
                            break;
                    }
                    break;

                case Windows.STATISTICS:                 // STATISTICS V
                    _currWindow = Windows.STATISTICS;
                    SetStatisticsWindow();
                    break;

                case Windows.USER_STATISTICS:
                    List<string> userStatistics = null;

                    try
                    {
                        userStatistics = _communicator.getUserStatistics();
                    }
                    catch (Exception e)
                    {
                        this.Close();
                        MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                            MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
                        break;
                    }

                    if (userStatistics == null)
                    {
                        MessageBoxResult result = MessageBox.Show("There were some errors ): \nGoing back to menu.", "Trivia",
                            MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                        _currWindow = Windows.MENU;
                        SetMenuWindow();
                        break;
                    }
                    _currWindow = Windows.USER_STATISTICS;
                    SetMyStatisticsWindow(userStatistics);
                    break;

                case Windows.HIGH_SCORES:
                    Dictionary<string, string> highScores = null;

                    try
                    {
                        highScores = _communicator.getHighScores();
                    }
                    catch (Exception e)
                    {
                        this.Close();
                        MessageBoxResult result = MessageBox.Show(e.Message, "Trivia",
                            MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
                        break;
                    }

                    if (highScores == null)
                    {
                        MessageBoxResult result = MessageBox.Show("There were some errors ): \nGoing back to menu.", "Trivia",
                            MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                        _currWindow = Windows.MENU;
                        SetMenuWindow();
                        break;
                    }

                    _currWindow = Windows.HIGH_SCORES;
                    SetHighScoresWindow(highScores);
                    break;

                default:
                    break;
            }
        }

        private void getAvailableRooms(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                List<string> roomNames = new List<string>();
                List<RoomData> currRooms = _communicator.getAvailableRooms();

                foreach (RoomData room in currRooms)
                {
                    roomNames.Add(room.name);
                }

                _available_rooms_worker.ReportProgress(0, new Tuple<List<string>, ListBox>(roomNames, (ListBox)e.Argument));

                System.Threading.Thread.Sleep(1000);

                if (_currWindow != Windows.JOIN_ROOM)
                {
                    break;
                }
            }
        }

        private void update_rooms_list(object sender, ProgressChangedEventArgs e)
        {
            Tuple<List<string>, ListBox> rooms = (Tuple<List<string>, ListBox>)e.UserState;
            rooms.Item2.Items.Clear();

            foreach (string room in rooms.Item1)
            {
                rooms.Item2.Items.Add(room);
            }
        }

        private void checkRoomState(object sender, DoWorkEventArgs e)
        {
            while (_currWindow == Windows.ROOM)
            {
                GetRoomStateRes state = _communicator.getRoomState();
                Tuple<ListBox, Button, bool> tmp = (Tuple<ListBox, Button, bool>)e.Argument;
                Tuple<ListBox, Button, bool, GetRoomStateRes> args = new Tuple<ListBox, Button, bool, GetRoomStateRes>(tmp.Item1, tmp.Item2, tmp.Item3, state);

                _room_state_worker.ReportProgress(0, args);
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void update_room_window(object sender, ProgressChangedEventArgs e)
        {
            Tuple<ListBox, Button, bool, GetRoomStateRes> args = (Tuple<ListBox, Button, bool, GetRoomStateRes>)e.UserState;

            if (args.Item4.status == (uint)ActiveMode.DONE && !args.Item3) // room mode is done and user is not admin
            {
                args.Item2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                MessageBoxResult result = MessageBox.Show("Admin closed the room :(", "Trivia",
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                return;
            }

            args.Item1.Items.Clear();

            foreach (string player in args.Item4.players)
            {
                args.Item1.Items.Add(player);
            }
        }

        private string _username;
        private Windows _currWindow;
        private Communicator _communicator;
        private BackgroundWorker _available_rooms_worker = new BackgroundWorker();
        private BackgroundWorker _room_state_worker = new BackgroundWorker();
    }
}