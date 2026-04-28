using System;
using System.IO;
using System.Media;
using System.Windows;

namespace CybersecurityChatbotWPF
{
    public class AudioService
    {
        private string _audioFilePath;

        public AudioService()
        {
            FindAudioFile();
        }

        private void FindAudioFile()
        {
            // Get the directory where the EXE is running
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;


            string projectDirectory = Path.GetFullPath(Path.Combine(exeDirectory, @"..\..\..\"));

            string[] possiblePaths = {
                // Direct path 
                @"C:\Users\lab_services_student\Desktop\prog6221-part2-Lee-SihJin\CybersecurityChatbotWPF\CybersecurityChatbotWPF\Data\greeting.wav",
                
                // Relative to project root
                Path.Combine(projectDirectory, "Data", "greeting.wav"),
                Path.Combine(projectDirectory, "CybersecurityChatbotWPF", "Data", "greeting.wav"),
                
                // Relative to EXE (build output)
                Path.Combine(exeDirectory, "Data", "greeting.wav"),
                Path.Combine(exeDirectory, "greeting.wav"),
                
                // Going up one level from EXE
                Path.Combine(Path.GetDirectoryName(exeDirectory), "Data", "greeting.wav"),
                Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(exeDirectory)), "Data", "greeting.wav")
            };

            foreach (string path in possiblePaths)
            {
                try
                {
                    string fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        _audioFilePath = fullPath;
                        System.Diagnostics.Debug.WriteLine($"[Audio] Found greeting file at: {_audioFilePath}");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Audio] Error checking path {path}: {ex.Message}");
                }
            }

            // File not found - log for debugging
            System.Diagnostics.Debug.WriteLine("[Audio] Greeting file NOT FOUND in any location");
            System.Diagnostics.Debug.WriteLine($"Current EXE directory: {exeDirectory}");
            System.Diagnostics.Debug.WriteLine($"Project directory: {projectDirectory}");
        }

        public void PlayGreeting()
        {
            try
            {
                if (!string.IsNullOrEmpty(_audioFilePath) && File.Exists(_audioFilePath))
                {
                    using (SoundPlayer player = new SoundPlayer(_audioFilePath))
                    {
                        player.Play(); // Use Play() instead of PlaySync() to not block UI
                    }
                    System.Diagnostics.Debug.WriteLine("[Audio] Greeting playing...");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[Audio] No valid greeting file found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Audio] Error playing greeting: {ex.Message}");
            }
        }
    }
}