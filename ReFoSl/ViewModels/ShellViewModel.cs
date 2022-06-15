using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Xna.Framework.Audio;
using ReFoSl.Models;
using System.Windows.Controls.Primitives;

namespace ReFoSl.ViewModels
{
    public class ShellViewModel : Screen
    {
        private const string SOUNDS_FOLDER = "Sounds/";
        private const string WAV_EXTENSION = ".wav";

        // The list that contains all the playing sounds
        private List<SoundEffectInstanceExtendedModel> _players = new List<SoundEffectInstanceExtendedModel>();
        // The list that contains all the sounds paused with the "Pause all playing sounds"
        private List<SoundEffectInstanceExtendedModel> _pausedSounds = new List<SoundEffectInstanceExtendedModel>();

        private double _masterVolumeMult = 1f;

        /// <summary>
        /// Play the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        /// <param name="volume">The volume of the selected sound</param>
        public void PlaySound(string soundName, double volume)
        {
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            SoundEffectInstanceExtendedModel player = new SoundEffectInstanceExtendedModel(soundName,
                SoundEffect.FromStream(Application.GetContentStream(new Uri(sound, UriKind.Relative)).Stream).CreateInstance());
            player.Play(volume, _masterVolumeMult);

            _players.Add(player);
        }

        /// <summary>
        /// Stop the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        public void StopSound(string soundName)
        {
            // Loop through the players to find the one that is playing the selected sound
            foreach (SoundEffectInstanceExtendedModel player in _players)
            {
                if (player.Name.Equals(soundName))
                {
                    player.Stop();
                    _players.Remove(player);
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
            foreach (SoundEffectInstanceExtendedModel player in _players)
            {
                if (player.Name.Equals(soundName))
                {
                    player.Volume = slider.Value * _masterVolumeMult;
                    player.RelativeVolume = slider.Value;
                    break;
                }
            }
        }

        /// <summary>
        /// Set the master volume
        /// </summary>
        /// <param name="slider">The slider that represents the master volume</param>
        public void SetMasterVolume(Slider slider)
        {
            // List that contains the relative volume of each sound
            // (i.e. the volume of the slider below the button)
            List<double> relativeVolumes = new List<double>();

            // Save the value of the master volume
            _masterVolumeMult = slider.Value;

            // Set the volume for each playing sound
            for (int i = 0; i < _players.Count; i++)
            {
                if (!relativeVolumes.Contains(_players[i].RelativeVolume))
                    relativeVolumes.Add(_players[i].RelativeVolume);

                _players[i].Volume = relativeVolumes[i] * _masterVolumeMult;
            }
        }

        /// <summary>
        /// Pause all the playing sounds
        /// </summary>
        public void PauseAllPlayingSounds(Window window)
        {
            var playersCopy = new List<SoundEffectInstanceExtendedModel>(_players);
            foreach (SoundEffectInstanceExtendedModel player in playersCopy)
            {
                _pausedSounds.Add(player);

                // Find the right button and uncheck it (doing this will stop the sound)
                var b = (ButtonAndSlider)window.FindName(player.Name);
                var tb = (ToggleButton)b.FindName("button");
                tb.IsChecked = false;
            }
            
        }

        /// <summary>
        /// Restart all the paused sounds
        /// </summary>
        /// <param name="window"></param>
        public void PlayAllPausedSounds(Window window)
        {
            var pausedCopy = new List<SoundEffectInstanceExtendedModel>(_pausedSounds);
            foreach (SoundEffectInstanceExtendedModel player in pausedCopy)
            {
                _pausedSounds.Remove(player);

                // Find the right button and check it (doing this will restart the sound)
                var b = (ButtonAndSlider)window.FindName(player.Name);
                var tb = (ToggleButton)b.FindName("button");
                tb.IsChecked = true;
            }
        }
    }
    
}
