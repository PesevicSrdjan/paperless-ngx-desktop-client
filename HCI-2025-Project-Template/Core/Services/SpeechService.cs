using HCI_2025_Project_Template.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Services
{
    public class SpeechService : ISpeechService
    {
        private SpeechRecognitionEngine? _recognizer;
        public bool IsListening { get; private set; }

        public event Action<string>? CommandRecognized;

        public SpeechService()
        {
            Initialize();
        }
        private void Initialize()
        {
            var recognizerInfo = SpeechRecognitionEngine.InstalledRecognizers()
                .FirstOrDefault(r => r.Culture.Name.StartsWith("en"));

            if (recognizerInfo == null)
                return;

            _recognizer = new SpeechRecognitionEngine(recognizerInfo);

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

            var grammar = new Grammar(new GrammarBuilder(commands)
            {
                Culture = recognizerInfo.Culture
            });

            _recognizer.LoadGrammar(grammar);

            var searchBuilder = new GrammarBuilder();
            searchBuilder.Culture = recognizerInfo.Culture;
            searchBuilder.Append("search");
            searchBuilder.AppendDictation();

            _recognizer.LoadGrammar(new Grammar(searchBuilder));

            _recognizer.SpeechRecognized += OnSpeechRecognized;
            _recognizer.SetInputToDefaultAudioDevice();
        }

        private void OnSpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.4)
                return;

            var text = e.Result.Text.ToLower();

            CommandRecognized?.Invoke(text);
        }

        public void Start()
        {
            if (_recognizer == null || IsListening) return;

            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            IsListening = true;
        }

        public void Stop()
        {
            if (_recognizer == null || !IsListening) return;

            _recognizer.RecognizeAsyncStop();
            IsListening = false;
        }
    }
}
