using Microsoft.Win32;
using System.IO;

namespace LogAnalyzer.Services
{
    public class FileService
    {
        public string OpenLogFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Log files (*.log;*.txt)|*.log;*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                return File.ReadAllText(dialog.FileName);
            }

            return null;
        }
    }
}
