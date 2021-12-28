using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Animation;

namespace ReFoSl.ViewModels
{
    public class ShellViewModel : Screen
    {
        private const string SOUNDS_FOLDER = "sounds/";
        private const string WAV_EXTENSION = ".wav";

        private List<MediaElement> _players = new List<MediaElement>();

        /// <summary>
        /// Play the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        /// <param name="volume">The volume of the selected sound</param>
        public void PlaySound(string soundName, double volume)
        {
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            //Check if the sound has already being played
            foreach (MediaElement p in _players)
            {
                if (p.Source.ToString().Equals(sound))
                {
                    // If it is, then play it
                    p.Play();
                    p.BeginAnimation(MediaElement.VolumeProperty, new DoubleAnimation(0, volume, TimeSpan.FromSeconds(1)));
                    return;
                }
            }

            // Otherwise this is the first time for the sound to be played
            // So, instantiate a new MediaElement, play it and add it to the list
            MediaElement player = new MediaElement();
            player.LoadedBehavior = MediaState.Manual;
            player.UnloadedBehavior = MediaState.Manual;
            player.Source = new Uri(sound, UriKind.Relative);

            player.Play();

            // Fade in animation
            player.BeginAnimation(MediaElement.VolumeProperty, new DoubleAnimation(0, volume, TimeSpan.FromSeconds(1)));
            player.MediaEnded += new RoutedEventHandler(MediaElement_MediaEnded);

            _players.Add(player);
        }

        /// <summary>
        /// Stop the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        public void StopSound(string soundName)
        {
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            // Loop through the players to find the one that is playing the selected sound
            foreach (MediaElement player in _players)
            {
                if (player.Source.ToString().Equals(sound))
                {
                    // Fade out animation
                    DoubleAnimation fadeOut = new DoubleAnimation(player.Volume, 0, TimeSpan.FromSeconds(1));
                    fadeOut.Completed += (sender, e) => DoubleAnimationFadeOut_Completed(sender, e, player);
                    player.BeginAnimation(MediaElement.VolumeProperty, fadeOut);

                    break;
                }
            }
        }

        /// <summary>
        /// Change the volume of the selected sound
        /// </summary>
        /// <param name="slider">The slider that represents the volume of the selected sound</param>
        /// <param name="soundName">The name of the selected sound</param>
        public void ChangeVolume(Slider slider, string soundName)
        {
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            foreach (MediaElement player in _players)
            {
                if (player.Source.ToString().Equals(sound))
                {
                    player.BeginAnimation(MediaElement.VolumeProperty, new DoubleAnimation(player.Volume, slider.Value, TimeSpan.FromSeconds(1)));
                    break;
                }
            }
        }

        private void MediaElement_MediaEnded(object sender, EventArgs e)
        {
            ((MediaElement)sender).Position = TimeSpan.Zero;
            ((MediaElement)sender).Play();
        }

        private void DoubleAnimationFadeOut_Completed(object sender, EventArgs e, MediaElement player)
        {
            player.Stop();
        }

    }
}
