using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Carubbi.WindowsAppHelper
{
    /// <summary>
    /// Checkbox customizado para ser colocado na Toolstrip
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public class ToolstripCheckbox : ToolStripControlHost
    {
        private CheckBox _checkboxControl;

        public ToolstripCheckbox()
            : base(new CheckBox())
        {
            _checkboxControl = Control as CheckBox;
        }
    }
}
