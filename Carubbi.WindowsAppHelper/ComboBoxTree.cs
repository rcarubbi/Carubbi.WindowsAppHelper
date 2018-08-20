using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Carubbi.WindowsAppHelper
{
    public delegate void NodeSelectEventHandler();
    /// <summary>
    /// ComboBoxTree control is a treeview that drops down much like a combobox
    /// </summary>
    public class ComboBoxTree : UserControl
    {
        #region Private Fields
        private readonly Panel _pnlBack;
        private readonly Panel _pnlTree;
        private readonly TextBox _tbSelectedValue;
        private readonly ButtonEx _btnSelect;
        private readonly TreeView _tvTreeView;
        private readonly LabelEx _lblSizingGrip;
        private readonly Form _frmTreeView;

        private string _branchSeparator = "\\";
        private Point _dragOffset;
        #endregion

        #region Public Properties
        [Browsable(true), Description("Gets the TreeView Nodes collection"), Category("TreeView"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor(typeof(TreeNodeCollection), typeof(TreeNodeCollection))]
        public TreeNodeCollection Nodes => _tvTreeView.Nodes;

        [Browsable(true), Description("Gets or sets the TreeView's Selected Node"), Category("TreeView")]
        public TreeNode SelectedNode
        {
            set => _tvTreeView.SelectedNode = value;
            get => _tvTreeView.SelectedNode;
        }

        [Browsable(true), Description("Gets or sets the TreeView's Selected Node"), Category("TreeView")]
        public ImageList Imagelist
        {
            get => _tvTreeView.ImageList;
            set => _tvTreeView.ImageList = value;
        }
        [Browsable(true), Description("The text in the ComboBoxTree control"), Category("Appearance")]
        public override string Text
        {
            get => _tbSelectedValue.Text;
            set => _tbSelectedValue.Text = value;
        }
        [Browsable(true), Description("Gets or sets the separator for the selected node's value"), Category("Appearance")]
        public string BranchSeparator
        {
            get => _branchSeparator;
            set
            {
                if (value.Length > 0)
                    _branchSeparator = value.Substring(0, 1);
            }
        }
        [Browsable(true), Description("Gets or sets the separator for the selected node's value"), Category("Behavior")]
        public bool AbsoluteChildrenSelectableOnly { get; set; }

        #endregion

        public ComboBoxTree()
        {
            InitializeComponent();
            FontChanged += ComboBoxTree_FontChanged;
            // Initializing Controls
            _pnlBack = new Panel
            {
                BorderStyle = BorderStyle.Fixed3D,
                BackColor = Color.White,
                AutoScroll = false
            };

            _tbSelectedValue = new TextBox {BorderStyle = BorderStyle.None};


            _btnSelect = new ButtonEx();
            _btnSelect.Click += ToggleTreeView;
            _btnSelect.FlatStyle = FlatStyle.Flat;


            _lblSizingGrip = new LabelEx
            {
                Size = new Size(9, 9),
                BackColor = Color.Transparent,
                Cursor = Cursors.SizeNWSE
            };
            _lblSizingGrip.MouseMove += SizingGripMouseMove;
            _lblSizingGrip.MouseDown += SizingGripMouseDown;

            _tvTreeView = new TreeView {BorderStyle = BorderStyle.None};
            _tvTreeView.DoubleClick += TreeViewNodeSelect;
            _tvTreeView.Location = new Point(0, 0);
            _tvTreeView.LostFocus += TreeViewLostFocus;


            //this.tvTreeView.Scrollable = false;

            _frmTreeView = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                BackColor = SystemColors.Control
            };

            _pnlTree = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            // Adding Controls to UserControl
            _pnlTree.Controls.Add(_lblSizingGrip);
            _pnlTree.Controls.Add(_tvTreeView);
            _frmTreeView.Controls.Add(_pnlTree);
            _pnlBack.Controls.AddRange(new Control[] { _btnSelect, _tbSelectedValue });
            Controls.Add(_pnlBack);


        }

        void ComboBoxTree_FontChanged(object sender, EventArgs e)
        {
            _tvTreeView.Font = Font;
        }

        private void RelocateGrip()
        {
            _lblSizingGrip.Top = _frmTreeView.Height - _lblSizingGrip.Height - 1;
            _lblSizingGrip.Left = _frmTreeView.Width - _lblSizingGrip.Width - 1;
        }


        [Browsable(true), Description("Fit the size of the container to the height of all nodes"), Category("TreeView")]
        public bool FitToAllNodes
        {
            get;
            set;
        }

        private void ToggleTreeView(object sender, EventArgs e)
        {
            if (!_frmTreeView.Visible)
            {
                var cbRect = this.RectangleToScreen(ClientRectangle);
                _frmTreeView.Location = new Point(cbRect.X, cbRect.Y + _pnlBack.Height);

                if (FitToAllNodes)
                {
                    ExpandAllNodes();
                    using (Graphics cg = CreateGraphics())
                    {
                        var size = cg.MeasureString(_tvTreeView.Nodes[0].Text, _tvTreeView.Font);

                        _frmTreeView.Height =
                        _pnlTree.Height =
                        _tvTreeView.Height = Convert.ToInt32(size.Width > 0? size.Width : 20) * GetTotalNodes();
                    }
                }

                _frmTreeView.Show();
                _frmTreeView.BringToFront();

                RelocateGrip();
                //this.tbSelectedValue.Text = "";
            }
            else
            {
                _frmTreeView.Hide();
            }
        }

        private void ExpandAllNodes()
        {
            foreach (TreeNode node in _tvTreeView.Nodes)
            {
                node.ExpandAll();
            }
        }

        private int GetTotalNodes()
        {
            var total = 0;
            foreach (TreeNode item in _tvTreeView.Nodes)
            {
                total += item.GetNodeCount(true);
            }

            return total;
        }

        public bool ValidateText()
        {
            var validatorText = Text;
            var tnc = _tvTreeView.Nodes;

            for (var i = 0; i < validatorText.Split(_branchSeparator.ToCharArray()[0]).Length; i++)
            {
                var nodeFound = false;
                var nodeToFind = validatorText.Split(_branchSeparator.ToCharArray()[0])[i];
                for (var j = 0; j < tnc.Count; j++)
                {
                    if (tnc[j].Text != nodeToFind) continue;
                    nodeFound = true;
                    tnc = tnc[j].Nodes;
                    break;
                }

                if (!nodeFound)
                    return false;
            }

            return true;
        }

        #region Events
        private void SizingGripMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var tvWidth = Cursor.Position.X - _frmTreeView.Location.X;
            tvWidth = tvWidth + _dragOffset.X;
            var tvHeight = Cursor.Position.Y - _frmTreeView.Location.Y;
            tvHeight = tvHeight + _dragOffset.Y;

            if (tvWidth < 50)
                tvWidth = 50;
            if (tvHeight < 50)
                tvHeight = 50;

            _frmTreeView.Size = new Size(tvWidth, tvHeight);
            _pnlTree.Size = _frmTreeView.Size;
            _tvTreeView.Size = new Size(_frmTreeView.Size.Width - _lblSizingGrip.Width, _frmTreeView.Size.Height - _lblSizingGrip.Width); ;
            RelocateGrip();
        }

        private void SizingGripMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var OffsetX = Math.Abs(Cursor.Position.X - _frmTreeView.RectangleToScreen(_frmTreeView.ClientRectangle).Right);
            var OffsetY = Math.Abs(Cursor.Position.Y - _frmTreeView.RectangleToScreen(_frmTreeView.ClientRectangle).Bottom);

            _dragOffset = new Point(OffsetX, OffsetY);
        }


        private void TreeViewLostFocus(object sender, EventArgs e)
        {
            if (!_btnSelect.RectangleToScreen(_btnSelect.ClientRectangle).Contains(Cursor.Position))
                _frmTreeView.Hide();
        }

        private void TreeViewNodeSelect(object sender, EventArgs e)
        {
            if (AbsoluteChildrenSelectableOnly)
            {
                if (_tvTreeView.SelectedNode.Nodes.Count != 0) return;
                _tbSelectedValue.Text = _tvTreeView.SelectedNode.FullPath.Replace(@"\", _branchSeparator);
                ToggleTreeView(sender, null);
            }
            else
            {
                _tbSelectedValue.Text = _tvTreeView.SelectedNode.FullPath.Replace(@"\", _branchSeparator);
                ToggleTreeView(sender, null);
            }
        }

        private void InitializeComponent()
        {
            // 
            // ComboBoxTree
            // 
            Name = "ComboBoxTree";
            AbsoluteChildrenSelectableOnly = true;
            Layout += ComboBoxTree_Layout;

        }

        private void ComboBoxTree_Layout(object sender, LayoutEventArgs e)
        {
            Height = _tbSelectedValue.Height + 8;
            _pnlBack.Size = new Size(Width, Height - 2);

            _btnSelect.Size = new Size(16, Height - 6);
            _btnSelect.Location = new Point(Width - _btnSelect.Width - 4, 0);

            _tbSelectedValue.Location = new Point(2, 2);
            _tbSelectedValue.Width = Width - _btnSelect.Width - 4;

            _frmTreeView.Size = new Size(Width, _tvTreeView.Height);
            _pnlTree.Size = _frmTreeView.Size;
            _tvTreeView.Width = _frmTreeView.Width - _lblSizingGrip.Width;
            _tvTreeView.Height = _frmTreeView.Height - _lblSizingGrip.Width;
            RelocateGrip();
        }

        #endregion

        #region LabelEx
        private class LabelEx : Label
        {
            /// <summary>
            /// 
            /// </summary>
            public LabelEx()
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.DoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                ControlPaint.DrawSizeGrip(e.Graphics, Color.Black, 1, 0, Size.Width, Size.Height);
            }
        }
        #endregion
        #region ButtonEx
        private class ButtonEx : Button
        {
            private ButtonState _state;

            /// <summary>
            /// 
            /// </summary>
            public ButtonEx()
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.DoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseDown(MouseEventArgs e)
            {
                _state = ButtonState.Pushed;
                base.OnMouseDown(e);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseUp(MouseEventArgs e)
            {
                _state = ButtonState.Normal;
                base.OnMouseUp(e);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                ControlPaint.DrawComboButton(e.Graphics, 0, 0, Width, Height, _state);
            }
        }
        #endregion
    }
}
