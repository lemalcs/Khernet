using Khernet.UI.IoC;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Khernet.UI.Managers
{
    public class PresentationApplicationContext : IApplicationContext
    {
        public byte[] ConvertStringToDocument(string value)
        {
            FlowDocument fw = new FlowDocument();

            fw.Blocks.Add(new Paragraph(new Run(value)));

            TextRange range = new TextRange(fw.ContentStart, fw.ContentEnd);
            byte[] messageContent = new byte[0];
            using (MemoryStream mem = new MemoryStream())
            {
                range.Save(mem, DataFormats.XamlPackage);
                messageContent = mem.ToArray();
            }
            return messageContent;
        }

        public void Execute(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                action();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        public bool IsInDesignTime()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
    }
}
