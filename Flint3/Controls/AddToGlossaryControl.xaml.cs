using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Flint3.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Controls
{
    public sealed partial class AddToGlossaryControl : UserControl
    {
        public MainViewModel ViewModel { get; set; } = null;

        public AddToGlossaryControl()
        {
            this.InitializeComponent();

            ViewModel = MainViewModel.Instance;
        }

        private void OnClickFilterColor(object sender, RoutedEventArgs e)
        {

        }
    }
}
