using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface ISpeechService
    {
        void Start();
        void Stop();
        bool IsListening { get; }

        event Action<string> CommandRecognized;
    }
}
