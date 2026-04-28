using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbotWPF
{
    public class ChatResponse
    {
        public string Message { get; set; }
        public string Topic { get; set; }

        public ChatResponse(string message, string topic)
        {
            Message = message;
            Topic = topic;
        }
    }

    public class ChatbotService
    {
        private Dictionary<string, string[]> _intentKeywords;
        private Dictionary<string, string[]> _responses;
        private Dictionary<string, List<string>> _userProfile;
        private string _userName;
        private Random _random;

        public ChatbotService()
        {
            _random = new Random();
            _userProfile = new Dictionary<string, List<string>>();
            InitializeIntentKeywords();
            InitializeResponses();
        }

        private void InitializeIntentKeywords()
        {
            _intentKeywords = new Dictionary<string, string[]>
            {
                { "Password", new[] { "password", "pass", "strong password", "password manager", "2fa", "two factor", "authentication", "login", "credential" } },
                { "Phishing", new[] { "phish", "phishing", "scam", "fraud", "suspicious email", "fake email", "email scam", "smishing" } },
                { "Browsing", new[] { "browse", "browsing", "https", "secure site", "public wifi", "wifi", "browser", "extension", "adblock" } },
                { "Privacy", new[] { "privacy", "data", "personal info", "tracking", "cookie", "private" } },
                { "Greeting", new[] { "how are you", "how are you doing", "how's it going", "what's up", "hello", "hi", "hey" } },
                { "Purpose", new[] { "what is your purpose", "what do you do", "who are you", "what can you do", "about you" } },
                { "Help", new[] { "help", "menu", "what can i ask", "commands", "options", "capabilities" } },
                { "Goodbye", new[] { "bye", "goodbye", "exit", "quit", "see you", "later" } }
            };
        }

        private void InitializeResponses()
        {
            _responses = new Dictionary<string, string[]>
            {
                { "Password", new[]
                    {
                        "Use a password manager like Bitwarden or LastPass to generate and store unique passwords.",
                        "Create strong passwords with at least 12 characters, mixing uppercase, lowercase, numbers, and symbols.",
                        "Never reuse passwords across different accounts. Each account needs its own unique password!",
                        "Enable Two-Factor Authentication (2FA) whenever possible - it's like a second lock on your door.",
                        "Consider using passphrases - a sequence of random words like 'correct-horse-battery-staple' - they're long but easy to remember!",
                        "Avoid using personal information like birthdays, pet names, or family names in your passwords."
                    }
                },
                { "Phishing", new[]
                    {
                        "Never click on suspicious links! Always hover over links first to see the actual URL.",
                        "Check the sender's email address carefully - scammers use addresses that look similar to legitimate ones.",
                        "Legitimate companies will NEVER ask for your password or personal information via email.",
                        "Red flags include: urgent language, spelling errors, requests for personal info, and unexpected attachments.",
                        "When in doubt, go directly to the company's website instead of clicking email links.",
                        "Be cautious of SMS phishing (smishing) too! Scammers use text messages as well."
                    }
                },
                { "Browsing", new[]
                    {
                        "Always look for 'https://' and the padlock icon in your browser's address bar before entering sensitive info.",
                        "Avoid using public Wi-Fi for banking or shopping. If you must, use a VPN (Virtual Private Network).",
                        "Keep your browser and extensions updated - updates often include important security fixes.",
                        "Use ad-blockers and privacy extensions to reduce tracking and block malicious ads.",
                        "Regularly clear your browser cache, cookies, and history to protect your privacy.",
                        "Consider using privacy-focused browsers like Firefox with privacy settings enabled."
                    }
                },
                { "Privacy", new[]
                    {
                        "Review privacy settings on social media regularly - limit who can see your posts and personal info.",
                        "Be careful what you share online - once something is posted, it's difficult to completely remove.",
                        "Use different email addresses for different purposes (personal, shopping, newsletters).",
                        "Check app permissions on your phone - many apps request access they don't actually need.",
                        "When signing up for services, read the privacy policy (or at least check what data they collect)."
                    }
                },
                { "Greeting", new[]
                    {
                        "I'm doing great, thanks for asking! I'm here to help you stay safe online. What can I teach you today?",
                        "All systems secure! Ready to help you learn about cybersecurity. What would you like to know?",
                        "I'm functioning perfectly! Cybersecurity is my specialty. Want tips on passwords, phishing, or safe browsing?",
                        "Hello! I'm your friendly Cybersecurity Bot! Let me help you protect your digital life."
                    }
                },
                { "Purpose", new[]
                    {
                        "I'm your Cybersecurity Awareness Bot! I teach online safety including password security, phishing protection, safe browsing, and privacy tips.",
                        "My purpose is to help South Africans stay safe online. I provide practical cybersecurity tips that you can use every day!",
                        "I'm designed to raise cybersecurity awareness. Ask me anything about staying safe online - from passwords to privacy settings!"
                    }
                },
                { "Help", new[]
                    {
                        "I can help you with:\n• Password safety tips\n• Phishing protection\n• Safe browsing practices\n• Privacy protection\n• General cybersecurity questions\n\nTry asking: 'How do I create strong passwords?' or 'What is phishing?'",
                        "Here's what I can do: answer questions about cybersecurity topics, provide tips and advice, and help you understand online safety. Type 'help' anytime to see this menu!"
                    }
                }
            };
        }

        public void SetUserName(string name)
        {
            _userName = name;
            if (!string.IsNullOrEmpty(name))
            {
                if (!_userProfile.ContainsKey("user_info"))
                    _userProfile["user_info"] = new List<string>();
                _userProfile["user_info"].Add($"Name: {name}");
            }
        }

        public string GetUserName()
        {
            return _userName;
        }

        public void StoreUserInterest(string interest)
        {
            if (!_userProfile.ContainsKey("interests"))
                _userProfile["interests"] = new List<string>();

            if (!_userProfile["interests"].Contains(interest))
                _userProfile["interests"].Add(interest);
        }

        public string GetUserInterests()
        {
            if (_userProfile.ContainsKey("interests") && _userProfile["interests"].Count > 0)
            {
                return string.Join(", ", _userProfile["interests"]);
            }
            return null;
        }

        public ChatResponse GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return new ChatResponse("I didn't catch that. Could you please say something?", "Invalid Input");
            }

            string lowerInput = userInput.ToLower().Trim();

            // Check for stored interests and personalize
            string interests = GetUserInterests();
            if (!string.IsNullOrEmpty(interests) && (lowerInput.Contains("for me") || lowerInput.Contains("my interest")))
            {
                return new ChatResponse($"Based on your interest in {interests}, would you like me to share more tips about these topics?", "Personalized");
            }

            // Check for name storage
            if (!string.IsNullOrEmpty(_userName) && !_userProfile.ContainsKey("user_info"))
            {
                SetUserName(_userName);
            }

            // Find matching intent
            string intent = "General";
            foreach (var kvp in _intentKeywords)
            {
                if (kvp.Value.Any(keyword => lowerInput.Contains(keyword)))
                {
                    intent = kvp.Key;
                    break;
                }
            }

            // Handle goodbye
            if (intent == "Goodbye")
            {
                return new ChatResponse($"Goodbye, {_userName ?? "friend"}! Remember: Stay vigilant, stay secure! Come back anytime you need cybersecurity tips.", "Exit");
            }

            // Get response based on intent
            string responseMessage;
            if (_responses.ContainsKey(intent))
            {
                var possibleResponses = _responses[intent];
                responseMessage = possibleResponses[_random.Next(possibleResponses.Length)];
            }
            else
            {
                responseMessage = GetDefaultResponse();
            }

            // Personalize with user name if available and appropriate
            if (!string.IsNullOrEmpty(_userName) && intent != "Greeting" && !responseMessage.Contains(_userName))
            {
                // Only personalize some responses to keep it natural
                if (_random.Next(3) == 0)
                {
                    responseMessage = $"{_userName}, {responseMessage.ToLower()}";
                }
            }

            // Store conversation topic as interest if relevant
            if (intent != "General" && intent != "Invalid Input" && intent != "Greeting")
            {
                StoreUserInterest(intent.ToLower());
            }

            return new ChatResponse(responseMessage, intent);
        }

        public string GetRandomResponseForTopic(string topic)
        {
            if (!string.IsNullOrEmpty(topic) && _responses.ContainsKey(topic))
            {
                var responses = _responses[topic];
                return responses[_random.Next(responses.Length)];
            }
            return null;
        }

        private string GetDefaultResponse()
        {
            string[] defaultResponses = {
                "I'm not sure about that. Could you ask me about passwords, phishing, safe browsing, or privacy?",
                "Interesting question! I specialize in cybersecurity topics like password safety, phishing protection, and secure browsing. Try asking about one of those!",
                "I didn't quite understand that. Try asking: 'How to create strong passwords?' or 'What is phishing?'",
                $"I'm here to help with cybersecurity! Would you like tips on passwords, phishing, safe browsing, or privacy?"
            };

            return defaultResponses[_random.Next(defaultResponses.Length)];
        }
    }
}