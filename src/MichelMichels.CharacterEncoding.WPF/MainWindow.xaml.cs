using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MichelMichels.CharacterEncoding.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush errorForegroundBrush = new SolidColorBrush(Colors.White);
        private readonly SolidColorBrush errorBackgroundBrush = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush defaultForegroundBrush = new SolidColorBrush(Colors.Black);
        private readonly SolidColorBrush defaultBackgroundBrush = new SolidColorBrush(Colors.White);
        private static List<Encoding> encodings = new();
        private readonly IEncodingConverter encodingConverter;

        public enum InputMode
        {
            Bytes,
            String,
        }

        private Encoding currentInputEncoding = Encoding.UTF8;
        private Encoding currentOutputEncoding = Encoding.UTF8;
        private InputMode currentInputMode = InputMode.Bytes;
        private InputMode currentOutputMode = InputMode.String;

        public MainWindow()
        {
            InitializeComponent();

            encodingConverter = new EncodingConverter();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeEncodings();
            InitializeInput();
            InitializeOutput();
        }

        private void InitializeInput()
        {
            var flowDocument = new FlowDocument();
            tbInput.Document = flowDocument;

            cbInputMode.ItemsSource = Enum.GetValues(typeof(InputMode)).Cast<InputMode>();
            cbInputMode.SelectedItem = InputMode.Bytes;

            currentOutputEncoding = encodings.FirstOrDefault() ?? Encoding.UTF8;
            cbOutputEncoding.ItemsSource = encodings;
            cbOutputEncoding.SelectedItem = currentOutputEncoding;
            cbOutputEncoding.DisplayMemberPath = "EncodingName";
        }
        private void InitializeOutput()
        {           
            cbOutputMode.ItemsSource = Enum.GetValues(typeof(InputMode)).Cast<InputMode>();
            cbOutputMode.SelectedItem = InputMode.Bytes;

            currentInputEncoding = encodings.FirstOrDefault() ?? Encoding.UTF8;
            cbInputEncoding.ItemsSource = encodings;
            cbInputEncoding.SelectedItem = currentOutputEncoding;
            cbInputEncoding.DisplayMemberPath = "EncodingName";

        }
        private void InitializeEncodings()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            encodings = new List<Encoding>
            {
                Encoding.UTF8,
                Encoding.UTF32,
                Encoding.BigEndianUnicode,
                Encoding.Unicode,
                Encoding.ASCII,
                Encoding.GetEncoding(1252),
            };
        }

        private void tbInput_TextChanged(object sender, TextChangedEventArgs e) => UpdateOutput();
        private void UpdateOutput()
        {
            if (currentOutputEncoding == null || currentInputEncoding == null)
            {
                tbOutput.Text = "Select a text encoding above.";
                return;
            }

            var inputStrings = currentInputMode switch
            {
                InputMode.Bytes => GetTextFromRichTextBox(tbInput).Replace(" ", "").Split("\r\n"),
                InputMode.String => GetTextFromRichTextBox(tbInput).Split("\r\n"),
                _ => throw new NotSupportedException(),
            };

            var result = new StringBuilder();
            for (int i = 0; i < inputStrings.Length; i++)
            {
                var line = inputStrings[i];

                try
                {
                    byte[] bytes = ConvertStringToBytes(line);
                    string outputString = ReencodeBytesToString(bytes);

                    result.AppendLine(outputString);
                    SetDefaultStyle(tbInput, i);
                }
                catch (FormatException)
                {
                    SetErrorStyle(tbInput, i);
                }
            }

            tbOutput.Text = result.ToString();
        }

        private string ReencodeBytesToString(byte[] bytes)
        {
            return currentOutputMode switch
            {
                InputMode.Bytes => ByteArrayToString(encodingConverter.ConvertBytes(bytes, currentInputEncoding, currentOutputEncoding)),
                InputMode.String => currentOutputEncoding.GetString(bytes),
                _ => throw new NotSupportedException()
            };
        }

        private byte[] ConvertStringToBytes(string line)
        {
            return currentInputMode switch
            {
                InputMode.Bytes => Convert.FromHexString(line),
                InputMode.String => encodingConverter.ConvertString(line, currentInputEncoding),
                _ => throw new NotSupportedException(),
            };
        }

        private string GetTextFromRichTextBox(RichTextBox rtb) => GetTextFromRichTextBox(rtb.Document.ContentStart, rtb.Document.ContentEnd);
        private string GetTextFromRichTextBox(TextPointer start, TextPointer end)
        {
            return new TextRange(start, end).Text;
        }

        private void SetErrorStyle(RichTextBox rtb, int lineIndex) => SetLineStyle(rtb, lineIndex, errorForegroundBrush, errorBackgroundBrush);
        private void SetDefaultStyle(RichTextBox rtb, int lineIndex) => SetLineStyle(rtb, lineIndex, defaultForegroundBrush, defaultBackgroundBrush);
        private void SetLineStyle(RichTextBox rtb, int lineIndex, SolidColorBrush foreground, SolidColorBrush background)
        {
            var paragraph = GetParagraph(rtb, lineIndex);
            SetParagraphColors(paragraph, foreground, background);
        }
        private Paragraph? GetParagraph(RichTextBox rtb, int lineIndex)
        {
            return rtb.Document.Blocks.OfType<Paragraph>().ElementAtOrDefault(lineIndex);
        }
        private void SetParagraphColors(Paragraph? paragraph, SolidColorBrush foreground, SolidColorBrush background)
        {
            if (paragraph == null)
            {
                return;
            }

            paragraph.Foreground = foreground;
            paragraph.Background = background;
        }

        private void OutputModeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbOutputMode.SelectedItem is InputMode textMode)
            {
                currentOutputMode = textMode;
                UpdateOutput();
            }
        }
        private void OutputEncodingChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbOutputEncoding.SelectedItem is Encoding selectedEncoding)
            {
                currentOutputEncoding = selectedEncoding;
                UpdateOutput();
            }
        }
        private void InputModeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbInputMode.SelectedItem is InputMode selectedMode)
            {
                currentInputMode = selectedMode;
                UpdateOutput();
            }
        }
        private void InputEncodingChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbInputEncoding.SelectedItem is Encoding selectedEncoding)
            {
                currentInputEncoding = selectedEncoding;
                UpdateOutput();
            }
        }

        private string ByteArrayToString(byte[] bytes)
        {
            return string.Join(" ", bytes.Select(x => x.ToString("X2")));
        }
    }
}
