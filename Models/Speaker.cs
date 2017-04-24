using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace Reader.Models
{
    class Speaker
    {
        private bool speaking;
        private string textToSpeak;
        private SpeechSynthesizer speaker;

        /// <summary>
        /// Instantiate a new instance of the speaker
        /// </summary>
        public Speaker()
        {
            // Initiate and subscribe to the SpeechSynthesizer.SpeakCompleted event to know when speaking has concluded
            speaker = new SpeechSynthesizer();
            speaker.SpeakCompleted += Completed;
            speaking = false;
        }

        /// <summary>
        /// Start speaking, text to speak and rate supplied by user
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rate"></param>
        public void Speak(string text, int rate)
        {
            speaker.Resume();
            textToSpeak = CleanText(text); // Probably do not use CleanText
            speaker.Rate = rate;
            speaker.SpeakAsync(textToSpeak);
            speaking = true;
        }

        /// <summary>
        /// Regex text cleaner, very specific to current usage with OneNote OCR
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string CleanText(string text)
        {
            text = Regex.Replace(text, @"(?m)^([A-Z\s]+)$", @"$0.");
            text = Regex.Replace(text, @"\s+", " ");
            text = Regex.Replace(text, @"—|-", ". ");
            text = Regex.Replace(text, @"T ", "T");
            return text;
        }

        /// <summary>
        /// Pause speaking the text for later continuation
        /// </summary>
        public void Pause()
        {
            if (speaking)
            {
                speaker.Pause();
                speaking = false;
            }
            else
            {
                speaker.Resume();
                speaking = true;
            }
        }

        /// <summary>
        /// Cancel speaking of the text
        /// </summary>
        public void Stop()
        {
            speaking = false;
            speaker.SpeakAsyncCancelAll();
        }

        /// <summary>
        /// Upon completion or cancelation of the speaking, raise a SpeechCompleteEvent for the viewmodel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Completed(object sender, SpeakCompletedEventArgs e)
        {
            speaker.Resume();
            textToSpeak = null;

            SpeechCompleteEventArgs args = new SpeechCompleteEventArgs();
            args.Cancelled = e.Cancelled;
            SpeechComplete(this, args);
        }

        /// <summary>
        /// SpeechCompleteEvent for the viewmodel
        /// </summary>
        public event SpeechCompleteEvent SpeechComplete;
    }
}
