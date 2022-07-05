using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Xna.Framework.Audio;
using ReFoSl.Models;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;

namespace ReFoSl.ViewModels
{
    public class ShellViewModel : Screen
    {
        private const string SOUNDS_FOLDER = "Sounds/";
        private const string WAV_EXTENSION = ".wav";

        // The list that contains all the playing sounds
        private BindableCollection<SoundEffectInstanceExtendedModel> _players = new BindableCollection<SoundEffectInstanceExtendedModel>();
        
        // To take the count of how many sounds are playing
        // It is bound to _players.Count
        // We need this variable to fire the CanAddMix method to disable/enable the button to add a new mix
        private int count = 0;
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                NotifyOfPropertyChange(() => Count);
                NotifyOfPropertyChange(() => CanAddMix);
            }
        }

        // The list that contains all the sounds paused with the "Pause all playing sounds"
        private List<SoundEffectInstanceExtendedModel> _pausedSounds = new List<SoundEffectInstanceExtendedModel>();

        // The general volume (it affects all the volumes)
        private double _masterVolumeMult = 1f;

        // A dictionary that contains the mixes
        // The key is the name of the mix
        // The value is a list of sounds bound to that mix
        public static Dictionary<string, BindableCollection<string>> mixNameWithSoundName;

        // The names of the mixes
        // Show in a combobox
        private BindableCollection<string> mixNames;
        public BindableCollection<string> MixNames
        {
            get { return mixNames; }
            set { mixNames = value; }
        }

        private string _selectedMixName;
        public string SelectedMixName
        {
            get { return _selectedMixName; }
            set
            {
                _selectedMixName = value;
                NotifyOfPropertyChange(() => SelectedMixName);

                if (_selectedMixName != null)
                {
                    // Stop all playing sounds
                    var _playersCopy = new List<SoundEffectInstanceExtendedModel>(_players);
                    foreach (var sound in _playersCopy)
                        CheckUncheckButton(Application.Current.MainWindow, sound.Name, false);

                    // Find the name of the mix in the dictionary and start the sounds
                    foreach (string sound in mixNameWithSoundName[_selectedMixName])
                        CheckUncheckButton(Application.Current.MainWindow, sound, true);
                }
            }
        }

        public ShellViewModel()
        {
            MixNames = new BindableCollection<string>();
            LoadSettings();
        }

        /// <summary>
        /// Load the mixes
        /// </summary>
        private void LoadSettings()
        {
            mixNameWithSoundName = JsonConvert.DeserializeObject<Dictionary<string, BindableCollection<string>>>(Properties.Settings.Default.MixList);

            if (mixNameWithSoundName == null)
                mixNameWithSoundName = new Dictionary<string, BindableCollection<string>>();
            foreach (string s in mixNameWithSoundName.Keys)
                MixNames.Add(s);
        }

        /// <summary>
        /// Save the mixes
        /// </summary>
        private void SaveSettings()
        {
            Properties.Settings.Default.MixList = JsonConvert.SerializeObject(mixNameWithSoundName);
            Properties.Settings.Default.Save();
        }

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
            Count++;

            // If the "Pause all sounds" is checked, and the user start a new sound
            // delete all the paused sounds, uncheck the button and continue normally
            _pausedSounds.Clear();
            var pauseAll = (ToggleButton)window.FindName("PauseAll");
            pauseAll.IsChecked = false;

            ClearMixNameComboBox();
        }

        /// <summary>
        /// Stop the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        public void StopSound(string soundName)
        {
            // Loop through the _players to find the one that is playing the selected sound
            foreach (SoundEffectInstanceExtendedModel player in _players)
            {
                if (player.Name.Equals(soundName))
                {
                    player.Stop();
                    _players.Remove(player);
                    break;
                }
            }

            Count--;

            ClearMixNameComboBox();
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
            var _playersCopy = new List<SoundEffectInstanceExtendedModel>(_players);
            foreach (SoundEffectInstanceExtendedModel player in _playersCopy)
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
        /// Enable/Disable the button to add a new mix
        /// It is enable if and only if there is at least one sound playing
        /// </summary>
        public bool CanAddMix
        {
            get
            {
                return Count != 0;
            }
        }

        public void AddMix() {}

        /// <summary>
        /// Open a new dialog to save the mix
        /// </summary>
        /// <param name="accepted">True if the user pressed on the button to create the mix, false otherwise</param>
        /// <param name="mixName">The name that the user writes for the mix</param>
        public void AddNewMix(bool accepted, string mixName)
        {
            // TODO: handle the case of empty mixName
            if (accepted)
            {
                MixNames.Add(mixName);

                BindableCollection<string> sounds = new BindableCollection<string>();
                foreach (var s in _players)
                    sounds.Add(s.Name);
                mixNameWithSoundName.Add(mixName, sounds);

                SaveSettings();
            }
        }

        /// <summary>
        /// Check or uncheck the button with the same name as the sound name (soundName)
        /// This trigger the sound to play (check) or to stop (uncheck)
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

            ClearMixNameComboBox();
        }

        /// <summary>
        /// Clear the mixes combobox when a mix is playing and a sound is stopped or played
        /// </summary>
        private void ClearMixNameComboBox()
        {
            if (SelectedMixName != null)
            {
                var mixCB = (ComboBox)Application.Current.MainWindow.FindName("MixNames");
                mixCB.SelectedValue = null;
            }
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
