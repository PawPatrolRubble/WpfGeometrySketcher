using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lan.ImageViewer
{
    public sealed class BorderedTextBlock : Border
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(BorderedTextBlock), new PropertyMetadata(default(string), OnTextChanges));

        private static void OnTextChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BorderedTextBlock borderedTextBlock)
            {
                borderedTextBlock._textBlock.Text = e.NewValue as string;
            }
        }

        private readonly TextBlock _textBlock;

        public BorderedTextBlock()
        {
            _textBlock = new TextBlock();
            Child = _textBlock;
            _textBlock.FontSize = 15;
            _textBlock.Foreground = Brushes.Lime;
            _textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }



    }
}
