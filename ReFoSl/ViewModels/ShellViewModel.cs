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

        private List<SoundEffectInstanceExtended> _players = new List<SoundEffectInstanceExtended>();

        /// <summary>
        /// Play the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        /// <param name="volume">The volume of the selected sound</param>
        public void PlaySound(string soundName, double volume)
        {
            string sound = SOUNDS_FOLDER + soundName + WAV_EXTENSION;

            SoundEffectInstanceExtended player = new SoundEffectInstanceExtended(sound,
                SoundEffect.FromStream(Application.GetContentStream(new Uri(sound, UriKind.Relative)).Stream).CreateInstance());
            player.Play(volume);

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
            foreach (SoundEffectInstanceExtended player in _players)
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

            foreach (SoundEffectInstanceExtended player in _players)
            {
                if (player.Name.Equals(sound))
                {
                    player.Volume = slider.Value;
                    break;
                }
            }
        }
    }
}
