using HCI_2025_Project_Template.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Services
{
    /// <summary>
    /// SpeechService klasa - predstavlja 'jezgro' SpeechRecognition - a.
    /// </summary>
    public class SpeechService : ISpeechService
    {
        /// <summary>
        /// Koristimo SpeechRecognitionEngine (depracated).
        /// </summary>
        private SpeechRecognitionEngine? _recognizer;
        /// <summary>
        /// Boolean vrijednost - da li SpeechRecognitionEngine 'sluša'?
        /// </summary>
        public bool IsListening { get; private set; }

        /// <summary>
        /// Događaj koji će se 'Invoke', na osnovu onoga što izgovorimo.
        /// </summary>
        public event Action<string>? CommandRecognized;

        /// <summary>
        /// Konstruktor SpeechService.
        /// </summary>
        public SpeechService()
        {
            Initialize();
        }

        /// <summary>
        /// Privatna metoda - baš iz ovog razloga koristimo interfejs ISpeechService kako DashBoardWindow, ne bi znao ništa o Initialize.
        /// </summary>
        private void Initialize()
        {
            // Postavljamo jezik SpeechRecognitionEngine- a.
            var recognizerInfo = SpeechRecognitionEngine.InstalledRecognizers()
                .FirstOrDefault(r => r.Culture.Name.StartsWith("en"));

            // Zaštita da ako je jezik null, onda neće ni raditi.
            if (recognizerInfo == null)
                return;

            // Kreiranje instance SpeechRecognitionEngine - a.
            _recognizer = new SpeechRecognitionEngine(recognizerInfo);

            // Navodimo komande koje će biti korištene na izgovor.
            var commands = new Choices
            (
                "dashboard",
                "documents",
                "tags",
                "settings",
                "correspondents",
                "types",
                "upload",
                "refresh",
                "logout"
            );

            // Gramatika samog SpeechRecognitionEngine - a, to su riječi i fraze koje SpeechRecognition može da prepozna.
            var grammar = new Grammar(new GrammarBuilder(commands)
            {
                Culture = recognizerInfo.Culture
            });

            _recognizer.LoadGrammar(grammar);

            // Ovo se odnosi na search jer je on malo drugačije napravljen od klasičnih komandi.

            // Naime na izgovoreno 'search' SpeechRecognitionEngine čeka na sljedeću izgovorenu riječ.
            // Izgovorenu riječ 'append' na searchBuilder.
            var searchBuilder = new GrammarBuilder();
            searchBuilder.Culture = recognizerInfo.Culture;
            searchBuilder.Append("search");
            searchBuilder.AppendDictation();

            _recognizer.LoadGrammar(new Grammar(searchBuilder));

            // Na osnovu prepoznatog govora poziva se metoda OnSpeechRecognized.
            _recognizer.SpeechRecognized += OnSpeechRecognized;
            _recognizer.SetInputToDefaultAudioDevice();
        }


        /// <summary>
        /// Metoda koja na osnovu prepoznatog govora, izgovorenu riječ pretvara u string i invoke događaj.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            // Odnosi se na tačnost prepoznavanja, iako je diskutabilnor, jer je SpeechRecognitionEngine 'depracated'.
            if (e.Result.Confidence < 0.4)
                return;

            var text = e.Result.Text.ToLower();

            // Invoka se događaj sa tom riječi.
            CommandRecognized?.Invoke(text);
        }

        // Metoda koja se koristi za pokretanje SpeechRecognition - a, koristi se u DashBoardWindow.
        public void Start()
        {
            if (_recognizer == null || IsListening) return;

            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            IsListening = true;
        }

        // Metoda koja se koristi za zaustavljanje SpeechRecognition - a, koristi se u DashBoardWindow.
        public void Stop()
        {
            if (_recognizer == null || !IsListening) return;

            _recognizer.RecognizeAsyncStop();
            IsListening = false;
        }
    }
}
