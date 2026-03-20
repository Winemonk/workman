using System.Windows;
using System.Windows.Input;

namespace Workman.Apps.Views
{
    /// <summary>
    /// DialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWindow : Window, IDialogWindow
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Result = new DialogResult(ButtonResult.Cancel);
                Close();
                e.Handled = true;
            }
            base.OnPreviewKeyUp(e);
        }

        public IDialogResult Result { get; set; }
    }
}
