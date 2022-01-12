using Microsoft.Xna.Framework.Audio;

namespace ReFoSl.Models
{
    class SoundEffectInstanceExtendedModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private SoundEffectInstance _soundEffectInstanceProperty;
        public SoundEffectInstance SoundEffectInstanceProperty
        {
            get { return _soundEffectInstanceProperty; }
            set { _soundEffectInstanceProperty = value; }
        }

        private double _volume;
        public double Volume
        {
            get { return _volume; }
            set { _volume = value; _soundEffectInstanceProperty.Volume = (float)value; }
        }


        public SoundEffectInstanceExtendedModel(string name, SoundEffectInstance soundEffectInstance)
        {
            _name = name;
            _soundEffectInstanceProperty = soundEffectInstance;
        }

        public void Play(double volume = 0.5f)
        {
            Volume = volume;
            _soundEffectInstanceProperty.IsLooped = true;
            _soundEffectInstanceProperty.Play();
        }

        public void Stop()
        {
            _soundEffectInstanceProperty.Stop();
        }

    }
}
