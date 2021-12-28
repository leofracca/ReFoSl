using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ReFoSl
{
    public partial class ButtonAndSlider : UserControl
    {
        /// <summary>
        /// The ToolTip text of each button
        /// </summary>
        public string ToolTipText
        {
            get { return (string)GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }

        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register("ToolTipText", typeof(string), typeof(ButtonAndSlider), new UIPropertyMetadata(""));


        /// <summary>
        /// The name of the sound that each button needs to play
        /// </summary>
        public string SoundName
        {
            get { return (string)GetValue(SoundNameProperty); }
            set { SetValue(SoundNameProperty, value); }
        }

        public static readonly DependencyProperty SoundNameProperty =
            DependencyProperty.Register("SoundName", typeof(string), typeof(ButtonAndSlider), new UIPropertyMetadata(""));


        /// <summary>
        /// The path of the image of each button
        /// </summary>
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(ButtonAndSlider), new UIPropertyMetadata(""));


        public ButtonAndSlider()
        {
            InitializeComponent();
        }
    }
}
