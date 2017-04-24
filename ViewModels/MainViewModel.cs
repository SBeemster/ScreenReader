using Reader.Models;
using System;
using System.Windows;

namespace Reader.ViewModels
{
    class MainViewModel : ObservableObject
    {
        // Commands
        public DelegateCommand SpeakCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }

        // Models
        private static Speaker speaker = new Speaker();

        // Properties
        private string _textBoxText;

        /// <summary>
        /// Keep track of the text the user wants to be read aloud
        /// </summary>
        public string TextBoxText
        {
            get { return _textBoxText; }
            set { SetProperty(ref _textBoxText, value); }
        }

        private int _rate;

        /// <summary>
        /// To keep track of the desired reading speed
        /// </summary>
        public int Rate
        {
            get { return _rate; }
            set { SetProperty(ref _rate, value); }
        }

        private bool _autoClipboard;

        /// <summary>
        /// Boolean to enable and disable the automatic reading of text from the clipboard
        /// </summary>
        public bool AutoClipboard
        {
            get { return _autoClipboard; }
            set
            {
                SetProperty(ref _autoClipboard, value);
            }
        }

        private bool pausing = false;

        private string _pauseText;

        /// <summary>
        /// Get and set the text on the pause button
        /// </summary>
        public string PauseText
        {
            get { return _pauseText; }
            set { SetProperty(ref _pauseText, value); }
        }

        private bool _speaking;

        /// <summary>
        /// Boolean to keep track of any active speeches
        /// </summary>
        public bool Speaking
        {
            get { return _speaking; }
            set
            {
                SetProperty(ref _speaking, value);
                PauseCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
                if (value == true)
                {
                    PauseText = "Pause";
                    pausing = false;
                }
            }
        }

        /// <summary>
        /// Initialize the viewmodel
        /// </summary>
        public MainViewModel()
        {
            // Initialize commands
            SpeakCommand = new DelegateCommand(SpeakCommandExecute);
            PauseCommand = new DelegateCommand(PauseCommandExecute, () => Speaking);
            StopCommand = new DelegateCommand(StopCommandExecute, () => Speaking);

            // Initialize properties
            TextBoxText = "Karen! I can read this text aloud. Motherfucker!";
            PauseText = "Pause";
            Rate = 3;
            Speaking = false;
            AutoClipboard = true;

            // Event subscription
            speaker.SpeechComplete += Completed;

            // Initialize ClipboardManager
            var windowClipboardManager = new ClipboardManager(App.Current.MainWindow);
            windowClipboardManager.ClipboardChanged += ClipboardChanged;
        }


        /// <summary>
        /// On ClipboardChanged, if it contains new text, read the contents aloud.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardChanged(object sender, EventArgs e)
        {
            // Handle your clipboard update here, debug logging example:
            if (Clipboard.ContainsText() && AutoClipboard)
            {
                TextBoxText = Clipboard.GetText();

                speaker.Stop();
                speaker.Speak(TextBoxText, Rate);
                Speaking = true;
            }
        }

        // Execute Commands

        /// <summary>
        /// Execute the SpeakCommand -> creat a new speaker, pass it the current TextBoxText, and subscribe to its SpeechComplete event
        /// </summary>
        private void SpeakCommandExecute()
        {
            speaker.Stop();
            speaker.Speak(TextBoxText, Rate);
            Speaking = true;
        }

        /// <summary>
        /// Execute the PauseCommand
        /// </summary>
        private void PauseCommandExecute()
        {
            speaker.Pause();
            pausing = !pausing;
            if (pausing) PauseText = "Continue";
            else PauseText = "Pause";
        }

        /// <summary>
        /// Execute the StopCommand
        /// </summary>
        private void StopCommandExecute()
        {
            Speaking = false;
            speaker.Stop();
            PauseText = "Pause";
        }

        /// <summary>
        /// Handle the completion notifications from the speaker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Completed(object sender, SpeechCompleteEventArgs e)
        {
            if (!e.Cancelled)
            {
                Speaking = false;
            }
        }
    }
}
