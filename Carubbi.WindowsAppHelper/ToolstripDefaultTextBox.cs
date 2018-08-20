using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Carubbi.WindowsAppHelper
{
    /// <summary>
    /// Textbox padrão customizado para ser colocado na Toolstrip
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public class ToolstripDefaultTextBox: ToolStripControlHost
    {
        private readonly TextBox _textBoxControl;

        public ToolstripDefaultTextBox()
            : base(new TextBox())
        {
            _textBoxControl = Control as TextBox;
        }

        /// <summary>
        /// Caractere para senhas
        /// </summary>
        public char PasswordChar
        {
            get => _textBoxControl.PasswordChar;
            set => _textBoxControl.PasswordChar = value;
        }
    }
}
