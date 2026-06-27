using ReceiptPrinterEmulator.Emulator;

namespace ReceiptPrinterEmulator.EscPos.Commands.ESC;

/// <summary>
/// 2024.02.18 Leo
/// Paper dot movement command
/// At 203 DPI, every dot is nearly 0.125 mm
/// https://escpos.readthedocs.io/en/latest/paper_movement.html#print-and-feed-paper-1b-4a-phx
/// </summary>
public class PaperPrintFeed : BaseCommand
{
	public override string Prefix => EscPosInterpreter.ESC + "J";
	public override bool HasArgs => true;

	private byte _n;

	public override void Reset()
	{
		_n = 0;
	}
	
	public override bool InterpretNextChar(char c)
	{
		_n = (byte)c;
		
		return false;
	}

	public override void Execute(ReceiptPrinter printer, string? args)
	{
		if (printer.CurrentReceipt.GetSkipLineFeed())
		{
			printer.CurrentReceipt.ResetSkipLineFeed();
			return;
		}

		if (_n > 0) printer.CurrentReceipt.FeedDotLines(_n);
		_n = 0;
	}
}
