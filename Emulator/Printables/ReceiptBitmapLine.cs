using ReceiptPrinterEmulator.Emulator.Abstraction;
using ReceiptPrinterEmulator.Logging;
using System;
using System.Drawing;

namespace ReceiptPrinterEmulator.Emulator.Printables;

public class ReceiptBitmapLine : IReceiptPrintable
{
    private readonly int _printWidth;
    private readonly Bitmap _image;
  
    public ReceiptBitmapLine(PaperConfiguration paperConfiguration, Bitmap image)
    {
        _printWidth = paperConfiguration.GetPrintWidthInPixels();
        _image = image;
    }
    
    public int GetPrintHeight()
    {
        if (_image.Width <= _printWidth)
            return _image.Height;

        return (int)Math.Ceiling(_image.Height * (float)_printWidth / _image.Width);
    }

    public void Render(Bitmap bitmap, Graphics g, int offsetX, int offsetY)
    {
        Logger.Info($"Rendering bitmap line at offset ({offsetX}, {offsetY}) with size ({_image.Width}, {_image.Height})");

        if (_image.Width <= _printWidth)
        {
            // Center the image horizontally if it fits within the print width
            offsetX += (_printWidth - _image.Width) / 2;
            g.DrawImageUnscaled(_image, offsetX, offsetY, _image.Width, _image.Height);
        }
        else
        {
            g.DrawImage(_image, offsetX, offsetY, _printWidth, _image.Height * (float)_printWidth / _image.Width);
        }
    }
}
