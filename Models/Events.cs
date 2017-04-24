using System;

namespace Reader.Models
{
    /// <summary>
    /// Delegate for the SpeechCompleteEvent a Speaker can raise for the viewmodel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void SpeechCompleteEvent(object sender, SpeechCompleteEventArgs args);

    /// <summary>
    /// Obligated custom EventArgs for a custom Event, just an empty object
    /// </summary>
    public class SpeechCompleteEventArgs : EventArgs
    {
        public bool Cancelled;
    }
}
