using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;

namespace Flint3.Helpers
{
    public static class TextToSpeechHelper
    {
        private static MediaPlayer _mediaPlayer;

        public static List<string> GetAllVoices() => SpeechSynthesizer.AllVoices.Select(v => v.DisplayName).ToList();

        public static async Task SpeakTextAsync(string text, double volume, string voice = "")
        {
            using var synthesizer = new SpeechSynthesizer();

            if (!string.IsNullOrWhiteSpace(voice))
            {
                var allVoices = SpeechSynthesizer.AllVoices;
                var selectedVoice = allVoices.FirstOrDefault(v => v.DisplayName.Equals(voice, StringComparison.OrdinalIgnoreCase));

                if (selectedVoice != null)
                {
                    synthesizer.Voice = selectedVoice;
                }
            }

            using SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(text);

            _mediaPlayer ??= new MediaPlayer();
            _mediaPlayer.Pause();
            _mediaPlayer.Source = null;
            _mediaPlayer.Volume = volume;
            _mediaPlayer.Source = MediaSource.CreateFromStream(stream, stream.ContentType);
            _mediaPlayer.Play();
        }
    }
}
