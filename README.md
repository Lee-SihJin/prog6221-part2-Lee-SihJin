# Cybersecurity Awareness Bot - Part 2

A WPF desktop application that educates South African citizens about cybersecurity through an interactive chatbot. The bot provides information about password safety, phishing protection, safe browsing, and privacy tips.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [How to Use](#how-to-use)

## Features

### Part 2 Implemented Features:

- **GUI Interface** - Clean Windows Presentation Foundation (WPF) design with chat bubbles
- **Voice Greeting** - Plays a welcome message when the application starts
- **ASCII Art Logo** - Displays a cybersecurity-themed logo
- **Keyword Recognition** - Recognizes topics like password, phishing, browsing, privacy
- **Random Responses** - Gives varied answers so conversations feel natural
- **Memory & Recall** - Remembers your name and topics you're interested in
- **Sentiment Detection** - Detects if you're worried, confused, curious, or happy
- **Follow-up Questions** - Handles "tell me more" and "another tip" requests
- **Conversation Flow** - Maintains natural conversation without restarting
- **Error Handling** - Handles invalid inputs gracefully

### Example Questions You Can Ask:

| Question | Bot Response Topic |
|----------|-------------------|
| "How do I create a strong password?" | Password Safety |
| "What is phishing?" | Phishing Protection |
| "Is public Wi-Fi safe?" | Safe Browsing |
| "How can I protect my privacy?" | Privacy Tips |
| "Tell me another tip" | Follow-up Response |
| "I'm worried about scams" | Sentiment-based Response |

## Technologies Used

| Technology | Purpose |
|------------|---------|
| C# .NET | Main programming language |
| WPF | Desktop GUI framework |
| XAML | User interface design |
| System.Media | Audio playback for voice greeting |
| .NET Collections | Dictionaries, Lists for storing responses |

## Project Structure

```
CybersecurityChatbotWPF/
├── MainWindow.xaml           # GUI design (colors, layout, chat bubbles)
├── MainWindow.xaml.cs        # Main logic (message handling, sentiment)
├── ChatbotService.cs         # Chatbot brain (keywords, responses)
├── AudioService.cs           # Audio playback for voice greeting
├── Data/
│   └── greeting.wav          # Voice greeting audio file
└── README.md                 # This file
```

### File Descriptions:

| File | What it does |
|------|--------------|
| **MainWindow.xaml** | Creates the visual interface - colors, chat bubbles, input box |
| **MainWindow.xaml.cs** | Controls everything - sends messages, shows responses, detects feelings |
| **ChatbotService.cs** | Stores all knowledge - keywords, responses, random answers |
| **AudioService.cs** | Plays the voice greeting when app starts |

## Setup Instructions

### Prerequisites

- Visual Studio 2022 or later
- .NET 6.0 or later
- Windows OS

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/Lee-SihJin/prog6221-part2-Lee-SihJin.git
   ```

2. **Open the project**
   - Open Visual Studio
   - Click "Open a project or solution"
   - Navigate to the folder and select the `.csproj` file

3. **Build the project**
   - Press `Ctrl + Shift + B` or click Build → Build Solution

4. **Run the application**
   - Press `F5` or click Debug → Start Debugging

### Audio Setup (Important!)

For the voice greeting to work:

1. Place your `greeting.wav` file in the `Data` folder
2. In Solution Explorer, right-click the file → Properties
3. Set:
   - **Build Action**: `Content`
   - **Copy to Output Directory**: `Copy if newer`

If no audio file is found, the app will still work fine - just without the voice greeting.

## How to Use

### Starting the Chatbot

1. Run the application
2. The ASCII logo and welcome message will appear
3. Enter your name when prompted
4. Start chatting about cybersecurity!

### Sample Conversation

```
Bot: Hello! Welcome to the Cybersecurity Awareness Bot!
Bot: What's your name?

User: John

Bot: Nice to meet you, John! I'm your Cybersecurity Awareness Assistant.
Bot: I can help you with password safety, phishing protection, safe browsing, and privacy tips.

User: How do I create a strong password?

Bot: [Password] Create strong passwords with at least 12 characters, mixing uppercase, 
     lowercase, numbers, and symbols.

User: Tell me another tip

Bot: [Password] Use a password manager like Bitwarden or LastPass to generate and 
     store unique passwords.

User: I'm worried about online scams

Bot: [Phishing] It's completely understandable to feel that way. Never click on 
     suspicious links in emails. Always hover over links to see the actual URL.

User: bye

Bot: Goodbye, John! Stay safe online!
```

### Commands You Can Use

| Command | What it does |
|---------|--------------|
| `help` or `menu` | Shows available topics |
| `bye` or `exit` | Closes the application |
| `tell me more` | Gets more information about current topic |
| `another tip` | Gets a different tip on same topic |
| `I'm interested in [topic]` | Bot remembers your interest |

### Releases

| Release | Tag | Description |
|---------|-----|-------------|
| Part 1 | v1.0 | Console application version |
| Part 2 | v2.0 | WPF GUI version with all features |



## Date

April 2026


