using System;
using System.Collections.Generic;
using System.Drawing;
using ReceiptPrinterEmulator.Emulator.Abstraction;
using ReceiptPrinterEmulator.Emulator.Enums;

namespace ReceiptPrinterEmulator.Emulator.Printables;

public class ReceiptTextLine : IReceiptPrintable
{
    private readonly PaperConfiguration.FontConfiguration _font;
    private readonly int _printWidth;
    private readonly int _charHeight;
    private readonly TextJustification _justification;
    private readonly bool _bold;
    private readonly bool _italic;
    private readonly UnderlineMode _underline;

    private int _totalWidth;
    private readonly List<(string text, PrintMode mode)> _strings = new();

    public bool IsEmpty => _strings.Count == 0;
    
    public ReceiptTextLine(PaperConfiguration paperConfiguration, PrintMode mode)
    {
        _font = paperConfiguration.GetFont(mode.Font);
        _printWidth = paperConfiguration.GetPrintWidthInPixels();
        _charHeight = _font.CharacterHeight * mode.CharHeightScale;
        _justification = mode.Justification;
        _bold = mode.Emphasize;
        _italic = mode.Italic;
        _underline = mode.Underline;

        _totalWidth = 0;
    }

    public bool TryWriteChar(char c, PrintMode mode)
    {
        int charWidth = (_font.CharacterWidth * mode.CharWidthScale);
        if ((_totalWidth + charWidth) >= _printWidth)
            return false;

        if (_strings.Count > 0 && mode.Equals(_strings[^1].mode))
        {
            // Append to last run
            var (text, lastMode) = _strings[^1];
            _strings[^1] = (text + c, lastMode);
        }
        else
        {
            // Start new run
            _strings.Add((c.ToString(), mode.Clone()));
        }
        _totalWidth += charWidth;
        return true;
    }

    public int GetPrintHeight()
    {
        // Use the tallest run's height for correct line spacing
        int maxCharHeight = 0;
        foreach (var (_, mode) in _strings)
        {
            int charHeight = (_font.CharacterHeight / 2) * mode.CharHeightScale;
            if (charHeight > maxCharHeight)
                maxCharHeight = charHeight;
        }
        // Add a small extra space for visual separation (like real printers)
        return maxCharHeight + (_font.CharacterHeight / 4);
    }

    public void Render(Bitmap bitmap, Graphics g, int offsetX, int offsetY)
    {
        // 1. Measure total line width for justification
        float totalWidth = 0;
        var runWidths = new List<float>();
        foreach (var (text, mode) in _strings)
        {
            int baseCharHeight = _font.CharacterHeight / 2;
            var fontStyle = FontStyle.Regular;
            if (mode.Emphasize) fontStyle |= FontStyle.Bold;
            if (mode.Italic) fontStyle |= FontStyle.Italic;
            using var font = new Font(_font.RenderFont, baseCharHeight, fontStyle);
            SizeF baseSize = g.MeasureString(text, font, int.MaxValue, StringFormat.GenericTypographic);
            float scaledWidth = baseSize.Width * mode.CharWidthScale;
            runWidths.Add(scaledWidth);
            totalWidth += scaledWidth;
        }

        // 2. Use the justification of the first run (ESC/POS line property)
        TextJustification justification = _strings.Count > 0 ? _strings[0].mode.Justification : TextJustification.Left;
        int x = offsetX;
        if (justification == TextJustification.Center)
            x += (int)((_printWidth - totalWidth) / 2);
        else if (justification == TextJustification.Right)
            x += (int)(_printWidth - totalWidth);
       

        // Find the tallest run in this line for baseline alignment
        int maxCharHeight = 0;
        float maxAscent = 0;
        foreach (var (_, mode) in _strings)
        {
            int baseCharHeight = _font.CharacterHeight / 2;
            int charHeight = baseCharHeight * mode.CharHeightScale;
            using var font = new Font(_font.RenderFont, baseCharHeight, FontStyle.Regular);
            var ascent = font.FontFamily.GetCellAscent(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style) * mode.CharHeightScale;
            if (charHeight > maxCharHeight)
                maxCharHeight = charHeight;
            if (ascent > maxAscent)
                maxAscent = ascent;
        }

        foreach (var (text, mode) in _strings)
        {
            int baseCharWidth = _font.CharacterWidth / 2;
            int baseCharHeight = _font.CharacterHeight / 2;
            int charHeight = baseCharHeight * mode.CharHeightScale;

            var fontStyle = FontStyle.Regular;
            if (mode.Emphasize) fontStyle |= FontStyle.Bold;
            if (mode.Italic) fontStyle |= FontStyle.Italic;

            using var font = new Font(_font.RenderFont, baseCharHeight, fontStyle);

            // Font metrics for baseline alignment
            float ascent = font.FontFamily.GetCellAscent(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style) * mode.CharHeightScale;
            float baselineOffset = (float)(maxAscent - ascent + (maxCharHeight - charHeight));

            // Measure the string width in base font, then scale
            SizeF baseSize = g.MeasureString(text, font, int.MaxValue, StringFormat.GenericTypographic);
            float scaledWidth = baseSize.Width * mode.CharWidthScale;

            var state = g.Save();

            // Align baseline of run to line baseline
            g.TranslateTransform(x, offsetY + baselineOffset);
            g.ScaleTransform(mode.CharWidthScale, mode.CharHeightScale);

            g.DrawString(text, font, Brushes.Black, 0, 0, StringFormat.GenericTypographic);

            // Underline (draw in scaled context)
            if (mode.Underline is UnderlineMode.OnOneDot or UnderlineMode.OnTwoDots)
            {
                var dotHeight = (mode.Underline is UnderlineMode.OnTwoDots ? 2 : 1);
                g.DrawLine(new Pen(Color.Black, dotHeight), 0, baseCharHeight*2, baseSize.Width, baseCharHeight*2);
            }

            g.Restore(state);

            x += (int)Math.Ceiling(scaledWidth);
        }
    }
}