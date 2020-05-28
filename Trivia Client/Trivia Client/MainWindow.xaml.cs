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
            /*
            // Setting fields
            _available_rooms_worker.WorkerSupportsCancellation = true;
            _available_rooms_worker.WorkerReportsProgress = true;
            _available_rooms_worker.DoWork += getAvailableRooms;
            _available_rooms_worker.ProgressChanged += update_rooms_list;
            
            _room_state_worker.WorkerSupportsCancellation = true;
            _room_state_worker.WorkerReportsProgress = true;
            _room_state_worker.DoWork += checkRoomState;
            _room_state_worker.ProgressChanged += update_room_window;

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
            SetEntryWindow();*/
            SetGameWindow(1, new Question());
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
            SetWindow(350, 400, false);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["darkLogo"] };

            TextBlock usernameBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Username" };

            TextBlock passwordBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Password" };

            TextBox usernameBox = new TextBox { Style = (Style)Resources["darkTextBox"] };
            usernameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(usernameBlock, usernameBox));
            
            PasswordBox passwordBox = new PasswordBox { Style = (Style)Resources["darkPasswordBox"] };
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

            // Adding controls to grid
            MainGrid.Children.Add(logo);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(boxes);
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

            TextBlock messageBlock = new TextBlock { Style = (Style)Resources["darkTitle"], Text = "Enter your details :)" };

            TextBlock usernameBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Username" };

            TextBlock passwordBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Password" };

            TextBlock emailBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Email" };

            TextBlock addressBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Address (Street, Apt, City)" };

            TextBlock phoneBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Phone" };

            TextBlock birthdateBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Birthdate (DD/MM/YYYY)" };

            TextBox usernameBox = new TextBox { Style = (Style)Resources["darkTextBox"] };
            usernameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(usernameBlock, usernameBox));

            PasswordBox passwordBox = new PasswordBox { Style = (Style)Resources["darkPasswordBox"] };
            passwordBox.PasswordChanged += new RoutedEventHandler((sender, args) => HandleBlockOutput(passwordBlock, passwordBox));

            TextBox emailBox = new TextBox{ Style = (Style)Resources["darkTextBox"] };
            emailBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(emailBlock, emailBox));
            
            TextBox addressBox = new TextBox { Style = (Style)Resources["darkTextBox"] };
            addressBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(addressBlock, addressBox));
            
            TextBox phoneBox = new TextBox { Style = (Style)Resources["darkTextBox"] };
            phoneBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(phoneBlock, phoneBox));
            
            TextBox birthdateBox = new TextBox { Style = (Style)Resources["darkTextBox"] };
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

            // Adding controls to grid
            MainGrid.Children.Add(head);
            MainGrid.Children.Add(blocks);
            MainGrid.Children.Add(boxes);
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

            TextBlock roomNameBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Room name" };

            TextBlock userNumBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Number of users" };

            TextBlock questionNumBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Number of questions" };

            TextBlock questionTimeBlock = new TextBlock { Style = (Style)Resources["grayText"], Text = "Time for question (sec)" };

            TextBox roomNameBox = new TextBox { Style = (Style)Resources["brightTextBox"] };
            roomNameBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(roomNameBlock, roomNameBox));

            TextBox userNumBox = new TextBox { Style = (Style)Resources["brightTextBox"] };
            userNumBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(userNumBlock, userNumBox));

            TextBox questionNumBox = new TextBox { Style = (Style)Resources["brightTextBox"] };
            questionNumBox.TextChanged += new TextChangedEventHandler((sender, args) => HandleBlockOutput(questionNumBlock, questionNumBox));
            
            TextBox questionTimeBox = new TextBox { Style = (Style)Resources["brightTextBox"] };
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
            roomsListBox.MouseDoubleClick += new MouseButtonEventHandler((sender, args) => HandleButtonClick(Windows.ROOM, roomName: roomsListBox.SelectedItem.ToString()));

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
                //startButton.Click += new RoutedEventHandler((sender, args) => HandleButtonClick()); // for now no start game option

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

        private void SetGameWindow(int i, Question currQuestion)
        {
            SetWindow(600, 900, true);

            // Creating controls for window
            Image logo = new Image { Style = (Style)Resources["brightLogo"] };
            TextBlock gameProgressBlock = new TextBlock
            {
                Style = (Style)Resources["brightText"],
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 30, 0, 0),
                FontSize = 40,
                Height = 60,
                Text = i.ToString() + "/" + 5.ToString()
        };

            TextBlock timeBlock = new TextBlock
            {
                Style = (Style)Resources["brightText"],
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 30, 0, 0),
                FontSize = 40,
                Height = 60
            };

            TextBlock questionBlock = new TextBlock
            {
                Style = (Style)Resources["brightText"],
                Height = 60,
                Text = "This is some question?",
                Margin = new Thickness(0, 10, 0, 0),
                TextWrapping = TextWrapping.Wrap
            };

            ListBox answersListBox = new ListBox { Style = (Style)Resources["brightListBox"], Width = 810, Height = 300, HorizontalAlignment = HorizontalAlignment.Center };
            //answersListBox.MouseDoubleClick += new MouseButtonEventHandler((sender, args) => SubmitAnswer(question, ((TextBlock)answersListBox.SelectedItem).Text));

            answersListBox.Items.Add(new TextBlock { Style = (Style)Resources["brightText"], Height = 60, Text = "Answer 1" + i.ToString(), Width = 790, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 0) });
            answersListBox.Items.Add(new TextBlock { Style = (Style)Resources["brightText"], Height = 60, Text = "Answer 2" + i.ToString(), Width = 790, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 0) });
            answersListBox.Items.Add(new TextBlock { Style = (Style)Resources["brightText"], Height = 60, Text = "Answer 3" + i.ToString(), Width = 790, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 0) });
            answersListBox.Items.Add(new TextBlock { Style = (Style)Resources["brightText"], Height = 60, Text = "Answer 4" + i.ToString(), Width = 790, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 0) });

            Button leaveButton;
            leaveButton = new Button { Style = (Style)Resources["brightButton"], Content = "Leave Room", VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 0, 0, 0) };
            
            int timeForQue = 10 + 1;
            timeBlock.Text = (timeForQue - 1).ToString();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, args) =>
                    {
                        timeBlock.Text = (timeForQue-1).ToString();
                        if (timeForQue == 1)
                        {
                            timeBlock.Text = "Timeout!";
                        }
                        else if (timeForQue == 0)
                        {
                            timer.Stop();
                            if (i < 5)
                            {
                                SetGameWindow(i + 1, new Question());
                            }
                        }
                        timeForQue--;
                    };
            timer.Start(); // Starting timer for question
            Thread.Sleep(1500);

            StackPanel head = new StackPanel();
            head.Children.Add(logo);
            head.Children.Add(questionBlock);
            head.Children.Add(answersListBox);
            head.Children.Add(leaveButton);

            // Adding controls to grid
            MainGrid.Children.Add(timeBlock);
            MainGrid.Children.Add(gameProgressBlock);
            MainGrid.Children.Add(head);
        }

        /*
        This function handles TextBlocks' visibility,
         that are in use to help the user with inserting input.
        Input: the TextBlock to handle, the TextBox to determine visibility.
        Output: none
        */
        public void HandleBlockOutput(TextBlock textBlock, TextBox textBox)
        {
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
        public void HandleBlockOutput(TextBlock textBlock, PasswordBox passwordBox)
        {
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
            string roomName = null, bool closeRoom = true)
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
                            this.Close();
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
                            _using_communicator.WaitOne();
                            bool login = _communicator.login(textBoxes[0].Text, passwordBox.Password);
                            _using_communicator.ReleaseMutex();

                            if (login)
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
                        else if (_currWindow == Windows.SIGNUP)
                        {
                            _using_communicator.WaitOne();
                            bool signup = _communicator.signup(textBoxes[0].Text, passwordBox.Password, textBoxes[1].Text,
                                textBoxes[2].Text, textBoxes[3].Text, textBoxes[4].Text);
                            _using_communicator.ReleaseMutex();

                            if (signup)
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
                            _using_communicator.WaitOne();
                            bool create_room = _communicator.createRoom(textBoxes[0].Text, textBoxes[1].Text, textBoxes[2].Text, textBoxes[3].Text);
                            _using_communicator.ReleaseMutex();
                            
                            if (create_room)
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
                        else if (_currWindow == Windows.JOIN_ROOM)
                        {
                            _using_communicator.WaitOne();
                            RoomData r = _communicator.getRoomData(roomName);
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
                        roomNames.Add(room.name);
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
                MessageBoxResult result = MessageBox.Show(ex.Message, "Trivia",
                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
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
                MessageBoxResult result = MessageBox.Show(ex.Message, "Trivia",
                    MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
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

            args.Item1.Items.Clear(); // clears players list

            foreach (string player in args.Item4.players) // inserting current players
            {
                args.Item1.Items.Add(player);
            }
        }

        private string _username;
        private Windows _currWindow;
        private Communicator _communicator;
        private BackgroundWorker _available_rooms_worker = new BackgroundWorker();
        private BackgroundWorker _room_state_worker = new BackgroundWorker();
        Mutex _using_communicator;
    }
}