﻿using Caliburn.Micro;
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
        
        // Variable to count how many sounds are playing
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

        // The list that contains all the sounds paused with the "Pause all playing sounds" button
        private List<SoundEffectInstanceExtendedModel> _pausedSounds = new List<SoundEffectInstanceExtendedModel>();

        // The general volume (it affects all the volumes)
        private double _masterVolume;
        public double MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                _masterVolume = value;
                NotifyOfPropertyChange(() => MasterVolume);
            }
        }

        // A dictionary that contains the mixes
        // The key is the name of the mix
        // The value is another dictionary with the sounds of that mix and the corresponding volumes
        public static Dictionary<string, Dictionary<string, double>> mixNameWithSoundName;

        // The names of the mixes
        private BindableCollection<string> mixNames;
        public BindableCollection<string> MixNames
        {
            get { return mixNames; }
            set { mixNames = value; }
        }

        // The selected mix by the user from the combobox
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
                    StopAllPlayingSounds();

                    // Find the name of the mix in the dictionary and start the sounds
                    foreach (var sound in mixNameWithSoundName[_selectedMixName])
                    {
                        CheckUncheckButton(sound.Key, true);
                        SetSlider(sound.Key, sound.Value);
                    }
                        
                }
            }
        }

        /// <summary>
        /// Constructor
        /// It initializes the mix names collection and loads the saved settings
        /// </summary>
        public ShellViewModel()
        {
            MixNames = new BindableCollection<string>();
            LoadSettings();
        }

        /// <summary>
        /// Load the saved settings
        /// </summary>
        private void LoadSettings()
        {
            // Load the mixes
            mixNameWithSoundName = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(Properties.Settings.Default.MixList);

            if (mixNameWithSoundName == null)
                mixNameWithSoundName = new Dictionary<string, Dictionary<string, double>>();
            foreach (string s in mixNameWithSoundName.Keys)
                MixNames.Add(s);

            // Load the value of the master volume
            MasterVolume = Properties.Settings.Default.MasterVolume;
        }

        /// <summary>
        /// Save the current settings
        /// </summary>
        private void SaveSettings()
        {
            // Save the mixes
            Properties.Settings.Default.MixList = JsonConvert.SerializeObject(mixNameWithSoundName);
            // Save the value of the master volume
            Properties.Settings.Default.MasterVolume = MasterVolume;

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Called when the user closes the window
        /// </summary>
        public void OnClose()
        {
            SaveSettings();
        }

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
            player.Play(volume, MasterVolume);

            _players.Add(player);
            Count++;

            // If the "Pause all sounds" is checked, and the user start a new sound,
            // delete all the paused sounds, uncheck the button and continue normally
            _pausedSounds.Clear();
            var pauseAll = (ToggleButton)Application.Current.MainWindow.FindName("PauseAll");
            pauseAll.IsChecked = false;

            ClearMixNameComboBox();
        }

        /// <summary>
        /// Stop the selected sound
        /// </summary>
        /// <param name="soundName">The name of the selected sound</param>
        public void StopSound(string soundName)
        {
            // Loop through all the playing sounds to find the one that has the same name of the selected sound
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
            // Loop through all the playing sounds to find the one that has the same name of the selected sound
            foreach (SoundEffectInstanceExtendedModel player in _players)
            {
                if (player.Name.Equals(soundName))
                {
                    player.Volume = slider.Value * MasterVolume;
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
            // Save the value of the master volume
            MasterVolume = slider.Value;

            // Set the volume for each playing sound
            for (int i = 0; i < _players.Count; i++)
                _players[i].Volume = _players[i].RelativeVolume * MasterVolume;
        }

        /// <summary>
        /// Pause all the playing sounds
        /// </summary>
        public void PauseAllPlayingSounds()
        {
            var _playersCopy = new List<SoundEffectInstanceExtendedModel>(_players);
            foreach (SoundEffectInstanceExtendedModel player in _playersCopy)
            {
                _pausedSounds.Add(player);

                CheckUncheckButton(player.Name, false);
            }
        }

        /// <summary>
        /// Restart all the paused sounds
        /// </summary>
        public void PlayAllPausedSounds()
        {
            var pausedCopy = new List<SoundEffectInstanceExtendedModel>(_pausedSounds);
            foreach (SoundEffectInstanceExtendedModel player in pausedCopy)
            {
                _pausedSounds.Remove(player);

                CheckUncheckButton(player.Name, true);
            }
        }

        /// <summary>
        /// Stop all the playing sounds
        /// </summary>
        private void StopAllPlayingSounds()
        {
            // Pause all the sounds
            PauseAllPlayingSounds();
            // But clear the list of the paused sounds (we don't want to remember the sounds in this case)
            _pausedSounds.Clear();
        }

        /// <summary>
        /// Throw a random number (n) and play n random sounds
        /// </summary>
        public void PlayRandomSounds()
        {
            // TODO: find a new way to get all the name of the sounds without listing them manually
            // (look inside the Sounds folder and get the names?)
            string[] soundNames = new string[] {"footsteps_on_grass", "birds", "campfire", "water_flowing",
                                                "forest", "rain", "thunderstorm", "waves",
                                                "happy_puppy_barks", "kids_playing", "restaurant", "stadium",
                                                "eating", "keyboard", "writing_on_blackboard", "clock",
                                                "cicadas", "crickets", "wind", "footsteps_on_snow"};

            StopAllPlayingSounds();

            // Choose a random number
            Random rnd = new Random();
            int n = rnd.Next(2, 5);

            // Start random sounds
            for (int i = 0; i < n; i++)
            {
                // Choose a random sound and start it
                var randomSound = soundNames[rnd.Next(soundNames.Length)];
                CheckUncheckButton(randomSound, true);

                // Then set the volume to a random value
                SetSlider(randomSound, rnd.NextDouble());
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
        /// Open a new dialog to save a new mix
        /// </summary>
        /// <param name="accepted">True if the user pressed on the button to create the mix, false otherwise</param>
        /// <param name="mixName">The name that the user writes for the mix</param>
        public void AddNewMix(bool accepted, string mixName)
        {
            // TODO: handle the case of empty mixName

            if (accepted)
            {
                MixNames.Add(mixName);

                var sounds = new Dictionary<string, double>();
                foreach (var s in _players)
                    sounds.Add(s.Name, s.RelativeVolume);
                mixNameWithSoundName.Add(mixName, sounds);
            }
        }

        /// <summary>
        /// Check or uncheck the button with the same name as the sound name (soundName)
        /// Doing this will trigger the sound to play (check the button) or to stop (uncheck the button)
        /// </summary>
        /// <param name="soundName">The name of the sound (i.e. the name of the button)</param>
        /// <param name="check">True if we need to check the button, false if we need to uncheck the button</param>
        private void CheckUncheckButton(string soundName, bool check)
        {
            // Find the right button and uncheck it (doing this will stop the sound)
            var b = (ButtonAndSlider)Application.Current.MainWindow.FindName(soundName);
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
        /// <param name="soundName">The name of the sound (i.e. the name of the button)</param>
        /// <param name="volume">The value to set the slider (and to set the volume)</param>
        private void SetSlider(string soundName, double volume)
        {
            // Find the right slider and adjust the value
            var b = (ButtonAndSlider)Application.Current.MainWindow.FindName(soundName);
            var s = (Slider)b.FindName("slider");
            s.Value = volume;

            ChangeVolume(s, soundName);
        }
    }
}
