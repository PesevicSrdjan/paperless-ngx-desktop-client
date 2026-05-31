using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    /// <summary>
    /// ISpeechService predstavlja interfejs koji implementira SpeechService.
    /// Odvojena logika, kako bi DashboardWindow koristio samo ISpeechService. 
    /// DashBoardWindow ne zna ništa o dodatnim implementacijama SpeechService.
    /// </summary>
    public interface ISpeechService
    {
        // Metoda koju implementira SpeechService, a koja će biti korištena za pokretanje SpeechRecognition -a.
        void Start();

        // Metoda koju implementira SpeechService, a koja će biti korištena za zaustavljanje SpeechRecognition -a.
        void Stop();

        // Boolean koji govori da li SpeechRecognition sluša.
        bool IsListening { get; }

        // Event koji će se dogoditi i na osnovu koga ćemo dalje graditi našu logiku.
        event Action<string> CommandRecognized;
    }
}
