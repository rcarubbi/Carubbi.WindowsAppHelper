﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Carubbi.WindowsAppHelper
{
    public static class CustomComboBoxRenderer
    {

        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer _visualStyleRenderer = null;

         
       
        private static readonly VisualStyleElement TextBoxElement = VisualStyleElement.TextBox.TextEdit.Normal;

        //cannot instantiate

        public static bool IsSupported => VisualStyleRenderer.IsSupported;

        private static void DrawBackground(Graphics g, Rectangle bounds, ComboBoxState state)
        {
            _visualStyleRenderer.DrawBackground(g, bounds);
            //for disabled comboboxes, comctl does not use the window backcolor, so
            // we don't refill here in that case. 
            if (state == ComboBoxState.Disabled) return;

            var windowColor = _visualStyleRenderer.GetColor(ColorProperty.FillColor);

            if (windowColor == SystemColors.Window) return;

            var fillRect = _visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
            fillRect.Inflate(-2, -2);
            //then we need to re-fill the background.
            g.FillRectangle(SystemBrushes.Window, fillRect);
        }
   
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally]
        public static void DrawTextBox(Graphics g, Rectangle bounds, ComboBoxState state)
        {
            if (_visualStyleRenderer == null)
            {
                _visualStyleRenderer = new VisualStyleRenderer(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }
            else
            {
                _visualStyleRenderer.SetParameters(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }

            DrawBackground(g, bounds, state);
        }

       
        public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, ComboBoxState state)
        {
            DrawTextBox(g, bounds, comboBoxText, font, TextFormatFlags.TextBoxControl, state);
        }

       
        public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, Rectangle textBounds, ComboBoxState state)
        {
            DrawTextBox(g, bounds, comboBoxText, font, textBounds, TextFormatFlags.TextBoxControl, state);
        }
 
        public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, TextFormatFlags flags, ComboBoxState state)
        {
            if (_visualStyleRenderer == null)
            {
                _visualStyleRenderer = new VisualStyleRenderer(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }
            else
            {
                _visualStyleRenderer.SetParameters(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }

            var textBounds = _visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
            textBounds.Inflate(-2, -2);
            DrawTextBox(g, bounds, comboBoxText, font, textBounds, flags, state);
        }

        
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")] // Using Graphics instead of IDeviceContext intentionally
        public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, Rectangle textBounds, TextFormatFlags flags, ComboBoxState state)
        {
            if (_visualStyleRenderer == null)
            {
                _visualStyleRenderer = new VisualStyleRenderer(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }
            else
            {
                _visualStyleRenderer.SetParameters(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }

            DrawBackground(g, bounds, state);
            var textColor = _visualStyleRenderer.GetColor(ColorProperty.TextColor);
            TextRenderer.DrawText(g, comboBoxText, font, textBounds, textColor, flags);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]// Using Graphics instead of IDeviceContext intentionally
        public static void DrawDropDownButton(Graphics g, Rectangle controlBounds, Rectangle buttonBounds, ComboBoxState state, Color buttonColor, Color triangleColor)
        {
            var dropButtonBrush = new SolidBrush(buttonColor);
            g.FillRectangle(dropButtonBrush, buttonBounds);

            var topLeft = new PointF(controlBounds.Width - 20, (controlBounds.Height - 7) / 2);
            var topRight = new PointF(controlBounds.Width - 10, (controlBounds.Height - 7) / 2);
            var bottom = new PointF(controlBounds.Width - 15, (controlBounds.Height + 3) / 2);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            var arrowBrush = new SolidBrush(triangleColor);

            g.FillRectangle(dropButtonBrush, buttonBounds);
            g.FillPolygon(arrowBrush, new[] { topLeft, topRight, bottom });
            
        }
    }
} 

