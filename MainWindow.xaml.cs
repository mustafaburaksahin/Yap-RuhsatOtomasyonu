using System;
using System.Windows;

namespace YapıRuhsatOtomasyonu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Eski IE emülasyon kayıt defteri ayarları tamamen temizlendi.
            // PDF ve web önizlemeleri projedeki modern 'WebView2' (Edge Chromium) bileşeniyle çözüldüğü için
            // bu pencerenin başlangıcında ekstra bir yük oluşturmaya gerek yoktur.

            InitializeComponent();
        }
    }
}