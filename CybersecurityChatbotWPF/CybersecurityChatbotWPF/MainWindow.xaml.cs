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
        private bool _waitingForName = true;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            PlayVoiceGreeting();
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

        private async void ShowWelcomeMessage()
        {
            await Task.Delay(500);
            AddBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot! I'm here to help you stay safe online. 🔒", "Greeting");
            await Task.Delay(500);

            // Ask for user name
            AddBotMessage("What's your name?", "Greeting");
            _waitingForName = true;
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
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 400
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
                Text = "🤖",
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
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 400
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

            // Handle name input if waiting for name
            if (_waitingForName)
            {
                _chatbotService.SetUserName(userInput);
                _waitingForName = false;

                // Personalized welcome
                await Task.Delay(500);
                AddBotMessage($"Nice to meet you, {userInput}! I'm your Cybersecurity Awareness Assistant. 🔐", "Greeting");
                await Task.Delay(500);
                AddBotMessage("I can help you with password safety, phishing protection, safe browsing, and privacy tips. What would you like to learn about today?", "Help");
                return;
            }

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
            if (userInput.ToLower().Contains("remember") || userInput.ToLower().Contains("recall") || userInput.ToLower().Contains("what did i"))
            {
                string recalledInfo = RecallInformation(userInput);
                if (!string.IsNullOrEmpty(recalledInfo))
                {
                    finalResponse = recalledInfo;
                }
            }

            // Check for interest storage
            if (userInput.ToLower().Contains("interested in") || userInput.ToLower().Contains("i like"))
            {
                StoreUserInterest(userInput);
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
                lowerInput.Contains("anxious") || lowerInput.Contains("nervous") ||
                lowerInput.Contains("fear") || lowerInput.Contains("afraid"))
            {
                return "worried";
            }
            else if (lowerInput.Contains("confused") || lowerInput.Contains("lost") ||
                     lowerInput.Contains("dont understand") || lowerInput.Contains("don't understand"))
            {
                return "confused";
            }
            else if (lowerInput.Contains("frustrated") || lowerInput.Contains("annoyed") ||
                     lowerInput.Contains("angry") || lowerInput.Contains("fed up"))
            {
                return "frustrated";
            }
            else if (lowerInput.Contains("curious") || lowerInput.Contains("interested") ||
                     lowerInput.Contains("tell me") || lowerInput.Contains("want to know"))
            {
                return "curious";
            }
            else if (lowerInput.Contains("happy") || lowerInput.Contains("great") ||
                     lowerInput.Contains("good") || lowerInput.Contains("excited"))
            {
                return "happy";
            }

            return "neutral";
        }

        private string AdjustResponseForSentiment(string response, string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "💙 It's completely understandable to feel that way. " + response +
                           "\n\nRemember, staying informed is the first step to staying safe! You're doing the right thing by learning about this.";
                case "confused":
                    return "🤔 I understand this can be confusing. Let me explain simply: " + response +
                           "\n\nWould you like me to explain more about this topic in a different way?";
                case "frustrated":
                    return "💪 I hear your frustration. Cybersecurity can be challenging, but you're doing great! " + response +
                           "\n\nTake a deep breath - you're building important skills that will protect you online!";
                case "curious":
                    return "🌟 Great question! I love your curiosity about cybersecurity. " + response +
                           "\n\nKeep asking questions - that's how we all learn to stay safe!";
                case "happy":
                    return "😊 That's wonderful to hear! " + response;
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
                   lowerInput.Contains("elaborate") ||
                   lowerInput.Contains("continue");
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
                return $"Of course! Let me provide more detail. {currentResponse}\n\nWould you like to know more about this topic?";
            }

            return currentResponse;
        }

        private void StoreInMemory(string userInput, string topic)
        {
            if (!string.IsNullOrEmpty(topic) && topic != "General" && topic != "Invalid Input" && topic != "Exit")
            {
                _currentTopic = topic;

                if (!_conversationMemory.ContainsKey(topic))
                {
                    _conversationMemory[topic] = new List<string>();
                }

                _conversationMemory[topic].Add(userInput);

                // Keep only last 10 entries per topic to manage memory
                if (_conversationMemory[topic].Count > 10)
                {
                    _conversationMemory[topic].RemoveAt(0);
                }
            }
        }

        private void StoreUserInterest(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("password"))
            {
                _chatbotService.StoreUserInterest("password safety");
                AddBotMessage("Great! I'll remember that you're interested in password safety. It's a crucial part of staying safe online! 🔐", "Memory");
            }
            else if (lowerInput.Contains("phish") || lowerInput.Contains("scam"))
            {
                _chatbotService.StoreUserInterest("phishing protection");
                AddBotMessage("Excellent! I'll remember you want to learn about phishing protection. Scammers are getting smarter, but so are you! 🎣", "Memory");
            }
            else if (lowerInput.Contains("brows") || lowerInput.Contains("wifi"))
            {
                _chatbotService.StoreUserInterest("safe browsing");
                AddBotMessage("Good choice! I'll note that you're interested in safe browsing habits. The web can be safer with the right knowledge! 🌐", "Memory");
            }
            else if (lowerInput.Contains("privacy"))
            {
                _chatbotService.StoreUserInterest("privacy");
                AddBotMessage("Privacy is so important! I'll remember you want to learn about protecting your personal information online. 🔏", "Memory");
            }
        }

        private string RecallInformation(string userInput)
        {
            string lowerInput = userInput.ToLower();

            // Recall user interests
            string interests = _chatbotService.GetUserInterests();
            if (!string.IsNullOrEmpty(interests) && (lowerInput.Contains("interest") || lowerInput.Contains("like")))
            {
                return $"I remember you're interested in {interests}. Would you like me to share more tips about these topics? 📚";
            }

            // Recall conversation topics
            foreach (var topic in _conversationMemory.Keys)
            {
                if (lowerInput.Contains(topic.ToLower()))
                {
                    return $"I recall you were asking about {topic} earlier. Would you like me to share more {topic} tips with you? 💡";
                }
            }

            // General recall
            if (_conversationMemory.Count > 0 && !string.IsNullOrEmpty(_currentTopic))
            {
                string userName = _chatbotService.GetUserName();
                if (!string.IsNullOrEmpty(userName))
                {
                    return $"{userName}, we were discussing {_currentTopic} before. Would you like me to continue with that topic? 🤖";
                }
                return $"We were discussing {_currentTopic} earlier. Would you like more information about this? 💭";
            }

            return null;
        }

        private void StoreConversationHistory(string userInput, string botResponse)
        {
            // Store for future recall (max 10 items per topic)
            if (!string.IsNullOrEmpty(_currentTopic) && _currentTopic != "General" && _currentTopic != "Invalid Input")
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