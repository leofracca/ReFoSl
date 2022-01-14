using Microsoft.Xna.Framework.Audio;

namespace ReFoSl.Models
{
    class SoundEffectInstanceExtendedModel
    {
        /// <summary>
        /// The name of the sound
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The SoundEffectInstance: used to play the sound
        /// </summary>
        private SoundEffectInstance _soundEffectInstanceProperty;
        public SoundEffectInstance SoundEffectInstanceProperty
        {
            get { return _soundEffectInstanceProperty; }
            set { _soundEffectInstanceProperty = value; }
        }

        /// <summary>
        /// The general volume of the sound (calculated considering the master volume)
        /// </summary>
        private double _volume;
        public double Volume
        {
            get { return _volume; }
            set { _volume = value; _soundEffectInstanceProperty.Volume = (float)value; }
        }

        /// <summary>
        /// The relative volume of the sound (calculated without considering the master volume)
        /// </summary>
        private double _relativeVolume;
        public double RelativeVolume
        {
            get { return _relativeVolume; }
            set { _relativeVolume = value; }
        }


        public SoundEffectInstanceExtendedModel(string name, SoundEffectInstance soundEffectInstance)
        {
            _name = name;
            _soundEffectInstanceProperty = soundEffectInstance;
        }

        public void Play(double volume = 0.5f, double masterVolume = 1f)
        {
            Volume = volume * masterVolume;
            RelativeVolume = volume;
            _soundEffectInstanceProperty.IsLooped = true;
            _soundEffectInstanceProperty.Play();
        }

        public void Stop()
        {
            _soundEffectInstanceProperty.Stop();
        }
    }
}
