using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StreamCommandBooper.Helpers
{
    /// <summary>
    /// Interaction logic for MessageBox2.xaml
    /// </summary>
    public partial class MessageBox2 : Window, INotifyPropertyChanged
    {
        #region Binding Helper
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }
        #endregion
        /// <summary>
        /// The message title
        /// </summary>
        public string? MessageType { get { return _MessageType; } set { _MessageType = value; OnPropertyChanged(nameof(MessageType)); } }
        private string? _MessageType;

        /// <summary>
        /// The message
        /// </summary>
        public string? Message { get { return _Message; } set { _Message = value; OnPropertyChanged(nameof(Message)); } }
        private string? _Message;

        public MessageBox2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show the exception
        /// </summary>
        /// <param name="exception">The exception to show</param>
        public static void ShowDialog(Exception exception, string location = "")
        {
            MessageBox2 dialog = new MessageBox2();
            dialog.Title = StreamCommandBooper.Resources.Localisation.UI.Exception;
            dialog.MessageType = StreamCommandBooper.Resources.Localisation.UI.Exception_Message; 
            dialog.Message = exception.Message;
            if (!string.IsNullOrWhiteSpace(location)) { dialog.Message += $"\n\n{StreamCommandBooper.Resources.Localisation.UI.Location}: {location}"; }
            dialog.DataContext = dialog;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Show the message
        /// </summary>
        /// <param name="MessageType">The message type, and title</param>
        /// <paramref name="Message"/>The message</param>
        public static void ShowDialog(string MessageTitle, string MessageType, string Message)
        {
            MessageBox2 dialog = new MessageBox2();
            dialog.Title = MessageTitle;
            dialog.MessageType = MessageType;
            dialog.Message = Message; 
            dialog.ShowDialog();
        }

        private void btnOK_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
