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

        // The general volume (it affects all the volumes)
        private double _masterVolumeMult = 1f;

        /// <summary>
        /// Play the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        /// <param name="volume">The volume of the selected sound</param>
        /// <param name="window">The window</param>
        public void PlaySound(string soundName, double volume, Window window)
        {
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            SoundEffectInstanceExtendedModel player = new SoundEffectInstanceExtendedModel(soundName,
                SoundEffect.FromStream(Application.GetContentStream(new Uri(sound, UriKind.Relative)).Stream).CreateInstance());
            player.Play(volume, _masterVolumeMult);

            _players.Add(player);

            // If the "Pause all sounds" is checked, and the user start a new sound
            // delete all the paused sounds, uncheck the button and continue normally
            _pausedSounds.Clear();
            var pauseAll = (ToggleButton)window.FindName("PauseAll");
            pauseAll.IsChecked = false;
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
        /// <param name="window">The window</param>
        public void PauseAllPlayingSounds(Window window)
        {
            var playersCopy = new List<SoundEffectInstanceExtendedModel>(_players);
            foreach (SoundEffectInstanceExtendedModel player in playersCopy)
            {
                _pausedSounds.Add(player);

                CheckUncheckButton(window, player.Name, false);
            }
            
        }

        /// <summary>
        /// Restart all the paused sounds
        /// </summary>
        /// <param name="window">The window</param>
        public void PlayAllPausedSounds(Window window)
        {
            var pausedCopy = new List<SoundEffectInstanceExtendedModel>(_pausedSounds);
            foreach (SoundEffectInstanceExtendedModel player in pausedCopy)
            {
                _pausedSounds.Remove(player);

                CheckUncheckButton(window, player.Name, true);
            }
        }

        /// <summary>
        /// Throw a random number between 2 and 4 (n) and play n random sounds
        /// </summary>
        /// <param name="window">The window</param>
        public void PlayRandomSounds(Window window)
        {
            string[] soundNames = new string[] {"footsteps_on_grass", "birds", "campfire", "water_flowing",
                                                "forest", "rain", "thunderstorm", "waves",
                                                "happy_puppy_barks", "kids_playing", "restaurant", "stadium",
                                                "eating", "keyboard", "writing_on_blackboard", "clock"};

            // Stop all the sounds
            PauseAllPlayingSounds(window);
            // But clear the list (we don't want to remember the sounds in this case)
            _pausedSounds.Clear();

            // Choose a random number
            Random rnd = new Random();
            int n = rnd.Next(2, 5);

            // Start random sounds
            for (int i = 0; i < n; i++)
            {
                // Choose a random sound and start it
                var randomSound = soundNames[rnd.Next(soundNames.Length)];
                CheckUncheckButton(window, randomSound, true);

                // Then set the volume to a random value
                SetSlider(window, randomSound, rnd.NextDouble());
            }
        }

        /// <summary>
        /// Check or uncheck the button with the same name as the sound name (soundName)
        /// </summary>
        /// <param name="window">The window</param>
        /// <param name="soundName">The name of the sound (i.e. the name of the button)</param>
        /// <param name="check">True if we need to check the button, false if we need to uncheck the button</param>
        private void CheckUncheckButton(Window window, string soundName, bool check)
        {
            // Find the right button and uncheck it (doing this will stop the sound)
            var b = (ButtonAndSlider)window.FindName(soundName);
            var tb = (ToggleButton)b.FindName("button");

            // Avoid to check the button if it is already checked (a new sound would start...)
            if (tb.IsChecked != check)
                tb.IsChecked = check;
        }

        /// <summary>
        /// Set the value of the slider corresponding to the button with the name of the sound (soundName)
        /// and change the volume
        /// </summary>
        /// <param name="window">The window</param>
        /// <param name="soundName">The name of the sound (i.e. the name of the button)</param>
        /// <param name="volume">The value to set the slider (and to set the volume)</param>
        private void SetSlider(Window window, string soundName, double volume)
        {
            // Find the right slider and adjust the value
            var b = (ButtonAndSlider)window.FindName(soundName);
            var s = (Slider)b.FindName("slider");
            s.Value = volume;

            ChangeVolume(s, soundName);
        }
    }
    
}
