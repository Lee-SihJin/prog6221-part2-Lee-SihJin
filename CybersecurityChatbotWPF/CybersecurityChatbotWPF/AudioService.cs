using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbotWPF
{
    public class AudioService
    {
        private string _audioFilePath;

        public AudioService()
        {
            // Look for audio file in multiple possible locations
            string[] possiblePaths = {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "greeting.wav"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav"),
                Path.Combine(Directory.GetCurrentDirectory(), "Data", "greeting.wav"),
                Path.Combine(Directory.GetCurrentDirectory(), "greeting.wav"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data", "greeting.wav")
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _audioFilePath = path;
                    break;
                }
            }
        }

        public void PlayGreeting()
        {
            try
            {
                if (!string.IsNullOrEmpty(_audioFilePath) && File.Exists(_audioFilePath))
                {
                    using (SoundPlayer player = new SoundPlayer(_audioFilePath))
                    {
                        player.Play(); // Async play for WPF
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[Audio] Greeting file not found. Continuing without audio...");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Audio] Error playing greeting: {ex.Message}");
            }
        }
    }
}