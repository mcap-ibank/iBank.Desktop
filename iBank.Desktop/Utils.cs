using System;

using Xceed.Wpf.Toolkit;

namespace iBank.Desktop
{
    public static class Utils
    {
        public static void ShowException(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ошибка!");
            //MessageBox.Show(ex.Message, "Ошибка!");
        }
    }
}