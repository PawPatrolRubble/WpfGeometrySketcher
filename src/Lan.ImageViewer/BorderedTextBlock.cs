using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lan.ImageViewer
{
    public sealed class BorderedTextBlock : Border
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(BorderedTextBlock),
            new PropertyMetadata(default(string), OnTextChanges));

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(BorderedTextBlock), new PropertyMetadata(default(double),OnFontSizeChanges));

        private static void OnFontSizeChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BorderedTextBlock borderedTextBlock)
            {
                borderedTextBlock._textBlock.FontSize = (double)e.NewValue;
            }
        }

        public static readonly DependencyProperty TextBlockStyleProperty = DependencyProperty.Register(
            "TextBlockStyle", typeof(Style), typeof(BorderedTextBlock), new PropertyMetadata(default(Style), OnStyleChanges) );

        private static void OnStyleChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BorderedTextBlock borderedTextBlock && e.NewValue is Style style) {
                borderedTextBlock._textBlock.Style = style;
            }
        }

        public Style TextBlockStyle
        {
            get { return (Style)GetValue(TextBlockStyleProperty); }
            set { SetValue(TextBlockStyleProperty, value); }
        }
        

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        #region fields

        private readonly TextBlock _textBlock;

        #endregion

        #region constructors

        public BorderedTextBlock()
        {
            _textBlock = new TextBlock();
            Child = _textBlock;
            //_textBlock.FontSize = 15;
            //_textBlock.Foreground = Brushes.Lime;
            //_textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            //_textBlock.VerticalAlignment = VerticalAlignment.Center;
        }

        #endregion

        #region properties

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region all other members

        private static void OnTextChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BorderedTextBlock borderedTextBlock) borderedTextBlock._textBlock.Text = e.NewValue as string;
        }

        #endregion
    }
}