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

            Button nextButton = new Button { Style = (Style)Resources["brightButton"], Content = "Next",
                Width = 100, HorizontalAlignment = HorizontalAlignment.Right };
            nextButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ROOM));

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
            roomsListBox.MouseDoubleClick += new MouseButtonEventHandler((sender, args) => HandleButtonClick(Windows.ROOM));
            for (int i = 0; i < 10; i++)
            {
                roomsListBox.Items.Add("Room " + i.ToString());
            }

            Button backButton = new Button { Style = (Style)Resources["brightButton"], Content = "Back", Margin = new Thickness(0, 35, 0, 0) };
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);
            head.Children.Add(roomsListBox);
            head.Children.Add(backButton);

            MainGrid.Children.Add(head);
        }

        private void SetRoomWindow()
        {
            MainGrid.Children.Clear();
            Height = 550;
            Width = 600;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Margin = new Thickness(0, 0, 0, 10),
                Text = "Room Name: " + ", Room Admin: " + ", \nTime Per Qst: " + ", Members Amount: " + ", Qst Amount: ",
                FontSize = 16, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.Lavender) , Height = 60, Width = 500};


            TextBlock playersBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Room members are: ",
                FontSize = 16, VerticalAlignment = VerticalAlignment.Stretch, Foreground = new SolidColorBrush(Colors.Lavender),
                 Margin = new Thickness(0, 0, 0, 10) };

            ListBox playersListBox = new ListBox { Style = (Style)Resources["roomList"] , Height = 200};
            for(int i = 0; i < 6; i++)
            {
                playersListBox.Items.Add("player" + i.ToString());
            }

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);
            head.Children.Add(playersBlock);
            head.Children.Add(playersListBox);

            MainGrid.Children.Add(head);

            Button startButton, closeButton, leaveButton;
   
            if ("user0" == "user0") // if admin
            {
                startButton = new Button { Style = (Style)Resources["brightButton"], Content = "Start Game", Margin = new Thickness(0, 0, 40, 30),
                    HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom};
                startButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

                closeButton = new Button { Style = (Style)Resources["brightButton"], Content = "Close Room", Margin = new Thickness(40, 0, 0, 30),
                    HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom };
                closeButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

                MainGrid.Children.Add(startButton);
                MainGrid.Children.Add(closeButton);
            }
            else // if not admin
            {
                leaveButton = new Button { Style = (Style)Resources["brightButton"], Content = "Leave Room", Margin = new Thickness(0, 30, 0, 0) };
                leaveButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));
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

        private void SetMyStatisticsWindow()
        {
            MainGrid.Children.Clear();
            Height = 515;
            Width = 450;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock totalAnswersBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Total Answers: " };
            totalAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            
            TextBlock correctAnswersBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Correct Answers: " };
            correctAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            
            TextBlock incorrectAnswersBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Incorrect Answers: " };
            incorrectAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            
            TextBlock avgTimeBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Avarage Time Per Answer: " };
            avgTimeBlock.Foreground = new SolidColorBrush(Colors.Lavender);
           
            TextBlock totalGamesBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Text = "Total Answers: " };
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

        private void SetHighScoresWindow()
        {
            MainGrid.Children.Clear();
            Height = 370;
            Width = 450;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image { Style = (Style)Resources["brightLogo"] };

            TextBlock firstScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0), Text = "1. " };
            
            TextBlock secondScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0), Text = "2. " };
            
            TextBlock thirdScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0), Text = "3. " };
            
            TextBlock fourthScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0), Text = "4. " };
            
            TextBlock fifthScoreBlock = new TextBlock { Style = (Style)Resources["myTextBlock"], Foreground = new SolidColorBrush(Colors.Lavender),
                TextAlignment = TextAlignment.Left, Margin = new Thickness(40, 0, 0, 0), Text = "5. " };

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

        public void HandleButtonClick(Windows nextWindow, List<TextBox> textBoxes = null, PasswordBox passwordBox = null)
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
                    _currWindow = Windows.ROOM;
                    SetRoomWindow();
                    break;

                case Windows.STATISTICS:                 // STATISTICS V
                    _currWindow = Windows.STATISTICS;
                    SetStatisticsWindow();
                    break;

                case Windows.USER_STATISTICS:
                    _currWindow = Windows.USER_STATISTICS;
                    SetMyStatisticsWindow();
                    break;

                case Windows.HIGH_SCORES:
                    _currWindow = Windows.HIGH_SCORES;
                    SetHighScoresWindow();
                    break;

                default:
                    break;
            }
        }

        private string _username;
        private Windows _currWindow;
        private Communicator _communicator;
    }
}