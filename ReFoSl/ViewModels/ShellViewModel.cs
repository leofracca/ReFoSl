using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Xna.Framework.Audio;
using ReFoSl.Models;

namespace ReFoSl.ViewModels
{
    public class ShellViewModel : Screen
    {
        private const string SOUNDS_FOLDER = "Sounds/";
        private const string WAV_EXTENSION = ".wav";

        private List<SoundEffectInstanceExtendedModel> _players = new List<SoundEffectInstanceExtendedModel>();

        private double _masterVolumeMult = 1f;

        /// <summary>
        /// Play the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        /// <param name="volume">The volume of the selected sound</param>
        public void PlaySound(string soundName, double volume)
        {
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            SoundEffectInstanceExtendedModel player = new SoundEffectInstanceExtendedModel(sound,
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
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            // Loop through the players to find the one that is playing the selected sound
            foreach (SoundEffectInstanceExtendedModel player in _players)
            {
                if (player.Name.Equals(sound))
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
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            foreach (SoundEffectInstanceExtendedModel player in _players)
            {
                if (player.Name.Equals(sound))
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
    }
    
}
