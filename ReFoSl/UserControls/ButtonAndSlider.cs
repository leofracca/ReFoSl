using System.Windows;
using System.Windows.Controls;

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
