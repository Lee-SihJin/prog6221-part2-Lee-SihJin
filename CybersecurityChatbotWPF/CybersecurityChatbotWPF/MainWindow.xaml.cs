using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbotWPF
{
    public partial class MainWindow : Window
    {
        private ChatbotService _chatbotService;
        private AudioService _audioService;
        private Dictionary<string, List<string>> _conversationMemory;
        private string _currentTopic;
        private int _currentTipIndex;
        private List<string> _currentTips;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            PlayVoiceGreeting();
            DisplayAsciiArt();
            ShowWelcomeMessage();
        }

        private void InitializeServices()
        {
            _chatbotService = new ChatbotService();
            _audioService = new AudioService();
            _conversationMemory = new Dictionary<string, List<string>>();
            _currentTips = new List<string>();
            _currentTipIndex = 0;
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                _audioService.PlayGreeting();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
            }
        }

        private void DisplayAsciiArt()
        {
            // ASCII art is already in XAML, just ensure it's visible
            AsciiArtBlock.Visibility = Visibility.Visible;
        }

        private async void ShowWelcomeMessage()
        {
            await Task.Delay(500);
            AddBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot! I'm here to help you stay safe online.", "Greeting");
            await Task.Delay(500);

            // Ask for user name
            AddBotMessage("What's your name?", "Greeting");
        }

        private void AddUserMessage(string message)
        {
            var messageBorder = new Border
            {
                Style = (Style)FindResource("ChatBubbleUser"),
                Margin = new Thickness(10, 5, 50, 5)
            };

            var stackPanel = new StackPanel();

            var nameText = new TextBlock
            {
                Text = _chatbotService.GetUserName() ?? "You",
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 150, 200)),
                Margin = new Thickness(0, 0, 0, 3)
            };

            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 13,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            var timeText = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                Margin = new Thickness(0, 3, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            stackPanel.Children.Add(nameText);
            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(timeText);
            messageBorder.Child = stackPanel;

            ChatStackPanel.Children.Add(messageBorder);
            ScrollToBottom();
        }

        private void AddBotMessage(string message, string topic)
        {
            var messageBorder = new Border
            {
                Style = (Style)FindResource("ChatBubbleBot"),
                Margin = new Thickness(50, 5, 10, 5)
            };

            var stackPanel = new StackPanel();

            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            var botIcon = new TextBlock
            {
                Text = "Bot",
                FontSize = 12,
                Margin = new Thickness(0, 0, 5, 0)
            };
            var topicText = new TextBlock
            {
                Text = $"[{topic}]",
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 100)),
                Margin = new Thickness(0, 0, 0, 3)
            };
            headerPanel.Children.Add(botIcon);
            headerPanel.Children.Add(topicText);

            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 13,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            var timeText = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 9,
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                Margin = new Thickness(0, 3, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stackPanel.Children.Add(headerPanel);
            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(timeText);
            messageBorder.Child = stackPanel;

            ChatStackPanel.Children.Add(messageBorder);
            ScrollToBottom();
        }

        private void AddSystemMessage(string message)
        {
            var messageBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(30, 5, 30, 5),
                Padding = new Thickness(10, 5, 10, 5)
            };

            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                TextAlignment = TextAlignment.Center,
                FontStyle = FontStyles.Italic
            };

            messageBorder.Child = messageText;
            ChatStackPanel.Children.Add(messageBorder);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToBottom();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                await ProcessUserInput();
            }
        }

        private async Task ProcessUserInput()
        {
            string userInput = MessageTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput))
                return;

            // Clear input box
            MessageTextBox.Text = "";

            // Add user message to chat
            AddUserMessage(userInput);

            // Show loading
            LoadingOverlay.Visibility = Visibility.Visible;

            // Simulate thinking delay
            await Task.Delay(500);

            // Get bot response
            var response = await Task.Run(() => _chatbotService.GetResponse(userInput));

            // Handle sentiment detection
            string sentiment = DetectSentiment(userInput);
            string finalResponse = AdjustResponseForSentiment(response.Message, sentiment);

            // Handle follow-up questions
            if (IsFollowUpQuestion(userInput))
            {
                finalResponse = HandleFollowUp(userInput, finalResponse);
            }

            // Store in memory
            StoreInMemory(userInput, response.Topic);

            // Check for recall requests
            if (userInput.ToLower().Contains("remember") || userInput.ToLower().Contains("recall"))
            {
                finalResponse = RecallInformation(userInput);
            }

            // Add bot message
            AddBotMessage(finalResponse, response.Topic);

            // Store conversation history
            StoreConversationHistory(userInput, finalResponse);

            // Hide loading
            LoadingOverlay.Visibility = Visibility.Collapsed;

            // Handle exit command
            if (response.Topic == "Exit")
            {
                await Task.Delay(1000);
                AddBotMessage("Thank you for using the Cybersecurity Awareness Bot! Stay safe online! 🛡️", "Farewell");
            }
        }

        private string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("worried") || lowerInput.Contains("scared") ||
                lowerInput.Contains("anxious") || lowerInput.Contains("nervous"))
            {
                return "worried";
            }
            else if (lowerInput.Contains("confused") || lowerInput.Contains("lost") ||
                     lowerInput.Contains("dont understand"))
            {
                return "confused";
            }
            else if (lowerInput.Contains("frustrated") || lowerInput.Contains("annoyed") ||
                     lowerInput.Contains("angry"))
            {
                return "frustrated";
            }
            else if (lowerInput.Contains("curious") || lowerInput.Contains("interested") ||
                     lowerInput.Contains("tell me"))
            {
                return "curious";
            }

            return "neutral";
        }

        private string AdjustResponseForSentiment(string response, string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "It's completely understandable to feel that way. " + response +
                           " Remember, staying informed is the first step to staying safe! 💙";
                case "confused":
                    return "I understand this can be confusing. Let me explain simply: " + response +
                           " Would you like me to explain more about this topic?";
                case "frustrated":
                    return "I hear your frustration. Cybersecurity can be challenging, but you're doing great! " +
                           response + " Take a deep breath and remember you're building good habits. 💪";
                case "curious":
                    return "Great question! I love your curiosity about cybersecurity. " + response;
                default:
                    return response;
            }
        }

        private bool IsFollowUpQuestion(string input)
        {
            string lowerInput = input.ToLower();
            return lowerInput.Contains("another") ||
                   lowerInput.Contains("more") ||
                   lowerInput.Contains("explain") ||
                   lowerInput.Contains("tell me more") ||
                   lowerInput.Contains("elaborate");
        }

        private string HandleFollowUp(string input, string currentResponse)
        {
            string lowerInput = input.ToLower();

            if ((lowerInput.Contains("another") || lowerInput.Contains("more")) &&
                !string.IsNullOrEmpty(_currentTopic))
            {
                var newResponse = _chatbotService.GetRandomResponseForTopic(_currentTopic);
                if (!string.IsNullOrEmpty(newResponse))
                {
                    return $"Sure! Here's another tip about {_currentTopic}: {newResponse}";
                }
            }
            else if (lowerInput.Contains("explain") || lowerInput.Contains("tell me more"))
            {
                return $"Of course! Let me provide more detail. {currentResponse} Would you like to know more about this topic?";
            }

            return currentResponse;
        }

        private void StoreInMemory(string userInput, string topic)
        {
            if (!string.IsNullOrEmpty(topic) && topic != "General" && topic != "Invalid Input")
            {
                _currentTopic = topic;

                if (!_conversationMemory.ContainsKey(topic))
                {
                    _conversationMemory[topic] = new List<string>();
                }

                _conversationMemory[topic].Add(userInput);
            }
        }

        private string RecallInformation(string userInput)
        {
            string lowerInput = userInput.ToLower();

            foreach (var topic in _conversationMemory.Keys)
            {
                if (lowerInput.Contains(topic.ToLower()))
                {
                    return $"I recall you were asking about {topic} earlier. " +
                           $"Would you like me to share more {topic} tips with you?";
                }
            }

            if (_conversationMemory.Count > 0 && !string.IsNullOrEmpty(_currentTopic))
            {
                return $"We were discussing {_currentTopic} before. Would you like me to continue with that topic?";
            }

            return null;
        }

        private void StoreConversationHistory(string userInput, string botResponse)
        {
            // Store for future recall (max 10 items per topic)
            if (!string.IsNullOrEmpty(_currentTopic))
            {
                if (!_conversationMemory.ContainsKey(_currentTopic))
                {
                    _conversationMemory[_currentTopic] = new List<string>();
                }

                _conversationMemory[_currentTopic].Add($"User: {userInput}");
                _conversationMemory[_currentTopic].Add($"Bot: {botResponse}");

                // Keep only last 20 entries per topic
                if (_conversationMemory[_currentTopic].Count > 20)
                {
                    _conversationMemory[_currentTopic].RemoveRange(0, 10);
                }
            }
        }
    }
}