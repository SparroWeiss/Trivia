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
            STATISTICS,
            USER_STATISTICS,
            HIGH_SCORES
        }


        public MainWindow()
        {
            InitializeComponent();

            /*connect:

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
            */
            SetEntryWindow();
        }
        // Entry                                          // V
        // Login / Signup                                 // V
        // Menu (CreateRoom ; JoinRomm ; Statistics)      // V
        // CreateRoom                                     // 
        // JoinRoom                                       // V
        // Statistics (MyStatistics ; HighScores)         // V

        private void SetEntryWindow()
        {
            MainGrid.Children.Clear();
            Height = 250;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Linen, Colors.PaleTurquoise, 90);

            Image logo = new Image();
            logo.Style = (Style)Resources["darkLogo"];
            logo.Width = 300;

            Button loginButton = new Button();
            loginButton.Content = "Login";
            loginButton.Style = (Style)Resources["darkButton"];
            loginButton.Height = 50;
            loginButton.Width = 170;
            loginButton.Margin = new Thickness(15, 0, 0, 20);
            loginButton.HorizontalAlignment = HorizontalAlignment.Left;
            loginButton.VerticalAlignment = VerticalAlignment.Bottom;
            loginButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.LOGIN));

            Button signupButton = new Button();
            signupButton.Content = "Signup";
            signupButton.Style = (Style)Resources["darkButton"];
            signupButton.Height = 50;
            signupButton.Width = 170;
            signupButton.Margin = new Thickness(0, 0, 15, 20);
            signupButton.HorizontalAlignment = HorizontalAlignment.Right;
            signupButton.VerticalAlignment = VerticalAlignment.Bottom;
            signupButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.SIGNUP));

            TextBlock messageBlock = new TextBlock
            {
                Style = (Style)Resources["myTextBlock"],
                Text = "Do you want to login or sign up? :)",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.DarkBlue)
            };

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

            Image logo = new Image();
            logo.Style = (Style)Resources["darkLogo"];

            TextBlock usernameBlock = new TextBlock();
            usernameBlock.Style = (Style)Resources["myTextBlock"];
            usernameBlock.Text = "Username";
            TextBlock passwordBlock = new TextBlock();
            passwordBlock.Style = (Style)Resources["myTextBlock"];
            passwordBlock.Text = "Password";

            TextBox usernameBox = new TextBox();
            usernameBox.Style = (Style)Resources["myTextBox"];
            usernameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(usernameBlock, usernameBox));
            PasswordBox passwordBox = new PasswordBox();
            passwordBox.Style = (Style)Resources["myPasswordBox"];
            passwordBox.PasswordChanged += new RoutedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, passwordBox));

            Button nextButton = new Button();
            nextButton.Style = (Style)Resources["darkButton"];
            nextButton.Content = "Next";
            nextButton.HorizontalAlignment = HorizontalAlignment.Right;
            nextButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));
            Button backButton = new Button();
            backButton.Style = (Style)Resources["darkButton"];
            backButton.Content = "Back";
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

            StackPanel boxes = new StackPanel();
            boxes.Orientation = Orientation.Vertical;
            boxes.VerticalAlignment = VerticalAlignment.Bottom;
            boxes.Children.Add(usernameBox);
            boxes.Children.Add(passwordBox);
            boxes.Children.Add(nextButton);

            StackPanel blocks = new StackPanel();
            blocks.Orientation = Orientation.Vertical;
            blocks.VerticalAlignment = VerticalAlignment.Bottom;
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

            Image logo = new Image();
            logo.Style = (Style)Resources["darkLogo"];

            TextBlock messageBlock = new TextBlock
            {
                Style = (Style)Resources["myTextBlock"],
                Text = "Enter your details :)",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.DarkBlue)
            };

            TextBlock usernameBlock = new TextBlock();
            usernameBlock.Style = (Style)Resources["myTextBlock"];
            usernameBlock.Text = "Username";
            TextBlock passwordBlock = new TextBlock();
            passwordBlock.Style = (Style)Resources["myTextBlock"];
            passwordBlock.Text = "Password";
            TextBlock emailBlock = new TextBlock();
            emailBlock.Style = (Style)Resources["myTextBlock"];
            emailBlock.Text = "Email";
            TextBlock addressBlock = new TextBlock();
            addressBlock.Style = (Style)Resources["myTextBlock"];
            addressBlock.Text = "Address (Street, Apt, City)";
            TextBlock phoneBlock = new TextBlock();
            phoneBlock.Style = (Style)Resources["myTextBlock"];
            phoneBlock.Text = "Phone";
            TextBlock birthdateBlock = new TextBlock();
            birthdateBlock.Style = (Style)Resources["myTextBlock"];
            birthdateBlock.Text = "Birthdate (DD/MM/YYYY)";


            TextBox usernameBox = new TextBox();
            usernameBox.Style = (Style)Resources["myTextBox"];
            usernameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(usernameBlock, usernameBox));
            PasswordBox passwordBox = new PasswordBox();
            passwordBox.Style = (Style)Resources["myPasswordBox"];
            passwordBox.PasswordChanged += new RoutedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, passwordBox));
            TextBox emailBox = new TextBox();
            emailBox.Style = (Style)Resources["myTextBox"];
            emailBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(emailBlock, emailBox));
            TextBox addressBox = new TextBox();
            addressBox.Style = (Style)Resources["myTextBox"];
            addressBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(addressBlock, addressBox));
            TextBox phoneBox = new TextBox();
            phoneBox.Style = (Style)Resources["myTextBox"];
            phoneBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(phoneBlock, phoneBox));
            TextBox birthdateBox = new TextBox();
            birthdateBox.Style = (Style)Resources["myTextBox"];
            birthdateBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(birthdateBlock, birthdateBox));

            Button nextButton = new Button();
            nextButton.Style = (Style)Resources["darkButton"];
            nextButton.Content = "Next";
            nextButton.HorizontalAlignment = HorizontalAlignment.Right;
            nextButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));
            Button backButton = new Button();
            backButton.Style = (Style)Resources["darkButton"];
            backButton.Content = "Back";
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

            StackPanel boxes = new StackPanel();
            boxes.Orientation = Orientation.Vertical;
            boxes.VerticalAlignment = VerticalAlignment.Bottom;
            boxes.Children.Add(usernameBox);
            boxes.Children.Add(passwordBox);
            boxes.Children.Add(emailBox);
            boxes.Children.Add(addressBox);
            boxes.Children.Add(phoneBox);
            boxes.Children.Add(birthdateBox);
            boxes.Children.Add(nextButton);

            StackPanel blocks = new StackPanel();
            blocks.Orientation = Orientation.Vertical;
            blocks.VerticalAlignment = VerticalAlignment.Bottom;
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

            Image logo = new Image();
            logo.Style = (Style)Resources["brightLogo"];

            TextBlock messageBlock = new TextBlock
            {
                Style = (Style)Resources["myTextBlock"],
                Text = "Choose an option :)",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.Lavender)
            };

            Button createRoomButton = new Button();
            createRoomButton.Style = (Style)Resources["brightButton"];
            createRoomButton.Content = "Create Room";
            createRoomButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.CREATE_ROOM));
            Button joinRoomButton = new Button();
            joinRoomButton.Style = (Style)Resources["brightButton"];
            joinRoomButton.Content = "Join Room";
            joinRoomButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.JOIN_ROOM));
            Button statisticsButton = new Button();
            statisticsButton.Style = (Style)Resources["brightButton"];
            statisticsButton.Content = "Statistics";
            statisticsButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.STATISTICS));
            Button signoutButton = new Button();
            signoutButton.Style = (Style)Resources["brightButton"];
            signoutButton.Content = "Signout";
            signoutButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.ENTRY));

            StackPanel buttons = new StackPanel();
            buttons.Orientation = Orientation.Vertical;
            buttons.VerticalAlignment = VerticalAlignment.Bottom;
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

        //private void SetCreateRoomWindow()

        private void SetJoinRoomWindow()
        {
            MainGrid.Children.Clear();
            Height = 550;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image();
            logo.Style = (Style)Resources["brightLogo"];

            TextBlock messageBlock = new TextBlock
            {
                Style = (Style)Resources["myTextBlock"],
                Text = "Choose a room to play in :)",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.Lavender)
            };

            ListBox roomsListBox = new ListBox();
            roomsListBox.Style = (Style)Resources["roomList"];
            roomsListBox.MouseDoubleClick += new MouseButtonEventHandler((sender, args) => HandleButtonClick(Windows.MENU));
            for (int i = 0; i < 10; i++)
            {
                roomsListBox.Items.Add("Room " + i.ToString());
            }

            Button backButton = new Button();
            backButton.Style = (Style)Resources["brightButton"];
            backButton.Content = "Back";
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));
            backButton.Margin = new Thickness(0, 35, 0, 0);

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(messageBlock);
            head.Children.Add(roomsListBox);
            head.Children.Add(backButton);

            MainGrid.Children.Add(head);
        }

        private void SetStatisticsWindow()
        {
            MainGrid.Children.Clear();
            Height = 350;
            Width = 400;
            MainGrid.Background = new LinearGradientBrush(Colors.Tomato, Colors.DarkRed, 90);

            Image logo = new Image();
            logo.Style = (Style)Resources["brightLogo"];

            TextBlock messageBlock = new TextBlock
            {
                Style = (Style)Resources["myTextBlock"],
                Text = "Which statistics? :)",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Stretch,
                Foreground = new SolidColorBrush(Colors.Lavender)
            };

            Button personalButton = new Button();
            personalButton.Style = (Style)Resources["brightButton"];
            personalButton.Content = "My Statistics";
            personalButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.USER_STATISTICS));
            Button highScoresButton = new Button();
            highScoresButton.Style = (Style)Resources["brightButton"];
            highScoresButton.Content = "High Scores";
            highScoresButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.HIGH_SCORES));
            Button backButton = new Button();
            backButton.Style = (Style)Resources["brightButton"];
            backButton.Content = "Back";
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.MENU));

            StackPanel buttons = new StackPanel();
            buttons.Orientation = Orientation.Vertical;
            buttons.VerticalAlignment = VerticalAlignment.Bottom;
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

            Image logo = new Image();
            logo.Style = (Style)Resources["brightLogo"];

            TextBlock totalAnswersBlock = new TextBlock();
            totalAnswersBlock.Style = (Style)Resources["myTextBlock"];
            totalAnswersBlock.Text = "Total Answers: ";
            totalAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            TextBlock correctAnswersBlock = new TextBlock();
            correctAnswersBlock.Style = (Style)Resources["myTextBlock"];
            correctAnswersBlock.Text = "Correct Answers: ";
            correctAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            TextBlock incorrectAnswersBlock = new TextBlock();
            incorrectAnswersBlock.Style = (Style)Resources["myTextBlock"];
            incorrectAnswersBlock.Text = "Incorrect Answers: ";
            incorrectAnswersBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            TextBlock avgTimeBlock = new TextBlock();
            avgTimeBlock.Style = (Style)Resources["myTextBlock"];
            avgTimeBlock.Text = "Avarage Time Per Answer: ";
            avgTimeBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            TextBlock totalGamesBlock = new TextBlock();
            totalGamesBlock.Style = (Style)Resources["myTextBlock"];
            totalGamesBlock.Text = "Total Answers: ";
            totalGamesBlock.Foreground = new SolidColorBrush(Colors.Lavender);

            Button backButton = new Button();
            backButton.Style = (Style)Resources["brightButton"];
            backButton.Content = "Back";
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.STATISTICS));

            StackPanel blocks = new StackPanel();
            blocks.Orientation = Orientation.Vertical;
            blocks.VerticalAlignment = VerticalAlignment.Bottom;
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

            Image logo = new Image();
            logo.Style = (Style)Resources["brightLogo"];

            TextBlock firstScoreBlock = new TextBlock();
            firstScoreBlock.Style = (Style)Resources["myTextBlock"];
            firstScoreBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            firstScoreBlock.TextAlignment = TextAlignment.Left;
            firstScoreBlock.Margin = new Thickness(40, 0, 0, 0);
            firstScoreBlock.Text = "1. ";
            TextBlock secondScoreBlock = new TextBlock();
            secondScoreBlock.Style = (Style)Resources["myTextBlock"];
            secondScoreBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            secondScoreBlock.TextAlignment = TextAlignment.Left;
            secondScoreBlock.Margin = new Thickness(40, 0, 0, 0);
            secondScoreBlock.Text = "2. ";
            TextBlock thirdScoreBlock = new TextBlock();
            thirdScoreBlock.Style = (Style)Resources["myTextBlock"];
            thirdScoreBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            thirdScoreBlock.TextAlignment = TextAlignment.Left;
            thirdScoreBlock.Margin = new Thickness(40, 0, 0, 0);
            thirdScoreBlock.Text = "3. ";
            TextBlock fourthScoreBlock = new TextBlock();
            fourthScoreBlock.Style = (Style)Resources["myTextBlock"];
            fourthScoreBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            fourthScoreBlock.TextAlignment = TextAlignment.Left;
            fourthScoreBlock.Margin = new Thickness(40, 0, 0, 0);
            fourthScoreBlock.Text = "4. ";
            TextBlock fifthScoreBlock = new TextBlock();
            fifthScoreBlock.Style = (Style)Resources["myTextBlock"];
            fifthScoreBlock.Foreground = new SolidColorBrush(Colors.Lavender);
            fifthScoreBlock.TextAlignment = TextAlignment.Left;
            fifthScoreBlock.Margin = new Thickness(40, 0, 0, 0);
            fifthScoreBlock.Text = "5. ";

            Button backButton = new Button();
            backButton.Style = (Style)Resources["brightButton"];
            backButton.Content = "Back";
            backButton.VerticalAlignment = VerticalAlignment.Bottom;
            backButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick(Windows.STATISTICS));

            StackPanel blocks = new StackPanel();
            blocks.HorizontalAlignment = HorizontalAlignment.Left;
            blocks.VerticalAlignment = VerticalAlignment.Center;
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

        public void HandleButtonClick(Windows nextWindow)
        {
            switch (nextWindow)
            {
                case Windows.ENTRY:
                    SetEntryWindow();
                    break;

                case Windows.LOGIN:
                    SetLoginWindow();
                    break;

                case Windows.SIGNUP:
                    SetSignupWindow();
                    break;
                case Windows.MENU:
                    SetMenuWindow();
                    break;
                case Windows.JOIN_ROOM:
                    SetJoinRoomWindow();
                    break;
                case Windows.STATISTICS:
                    SetStatisticsWindow();
                    break;
                case Windows.USER_STATISTICS:
                    SetMyStatisticsWindow();
                    break;
                case Windows.HIGH_SCORES:
                    SetHighScoresWindow();
                    break;
                default:
                    break;
            }
        }


        private Communicator _communicator;
    }
}