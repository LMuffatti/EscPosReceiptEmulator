# ESC/POS Receipt Printer Emulator
🖨️ **This app emulates a networked receipt printer to test your ESC/POS commands against.**

### About
- Windows application (WPF + .NET 6)
- Binds to a TCP/IP interface and listens for ESC/POS commands
- Logs commands and visually represents the resulting receipt(s)
- With italics text cutting bug corrected

👷 **This is a working in progress.** Use at your own risk and keep your expectations low. :)

### Supported commands

⚠️ Support is currently limited to only a subset of ESC/POS. Even the commands listed here may only be partially implemented.

- Raw Text
- FF: Form feed
- HT: Horizontal tabs
- LF: Line feed
- CR: Carriage return
- ESC Commands:
  - Initialize printer (`ESC @`)
  - Toggle italic (`ESC 4` / `ESC 5`)
  - Select font (`ESC M`)
  - Select charset (`ESC R`)
  - Select character table (`ESC t`)
  - Select justification (`ESC a`)
  - Select line spacing (`ESC 2` / `ESC 3`)
  - Toggle emphasis (`ESC E`)
  - Toggle underline (`ESC -`)
  - Set print text mode (`ESC !`)
  - Paper full cut (`ESC m`)
  - Paper partial cut (`ESC i`)
  - Paper feed N lines (`ESC d`)
  - Paper feed (`ESC J`)
- FS Commands:
  - Print stored logo (`FS p n m`)
  - Paper auto cut (`FS } 0x60 n`)
- GS Commands:
  - Select character size 
  - Select cut mode and cut paper
  - Paper eject (`GS e n [m t]`)
  - Print raster image (`GS v 0 [m xL xH yL yH ...pixels]`)

### Example

![image info](Docs/Example1.png)
<br>
and also
<br>
![image info](Docs/Example2.png)

### Emulated printer

This program emulates a printer with the following specifications:

 - 80mm paper width
 - 76mm printing width
 - 203x203dpi
 - ASCII Font A: 12x24 pixels
 - ASCII Font B: 9x24 pixels
 - ASCII Font C: 24x48 pixels
 - ASCII Font D: 16x24 pixels
 - Automatic line feed

### References

https://aures-support.com/DATA/drivers/Imprimantes/Commande%20ESCPOS.pdf<br>
https://www.starmicronics.com/support/Mannualfolder/escpos_cm_en.pdf<br>
https://www.printercontroller.com/kcfinder/upload/files/esc_pos_commands_1_0_7.pdf<br>
https://www.electronicaembajadores.com/Datos/pdf1/i5/i5tm/i5tm004.pdf<br>
https://download4.epson.biz/sec_pubs/pos/reference_en/escpos/<br>
https://escpos.readthedocs.io/en/latest/commands.html<br>
https://tabshop.smartlab.at/help-topics/help-esc-pos-codes.html<br>
