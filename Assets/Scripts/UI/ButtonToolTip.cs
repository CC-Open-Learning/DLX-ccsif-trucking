using System;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    //this class handles abstraction between Buttons and ToolTips
    //This class is used by other UI Classes with Buttons
    public class ButtonToolTip
    {
        private TooltipUI tooltip;
        private Button button;
        private TooltipType direction;
        private String message;

        public ButtonToolTip(Button btn, TooltipUI tip, String msg, TooltipType dir = TooltipType.Left)
        {
            button = btn;
            tooltip = tip;
            message = msg;
            direction = dir;
            RegisterEvents();
        }

        //function for showing the tooltip
        private void ShowToolTip(PointerEnterEvent e, ButtonToolTip btnTip)
        {
            btnTip.tooltip.HandleDisplayUI(btnTip.button, btnTip.direction, message);
        }

        //function for hiding the tooltip
        private void HideToolTip(PointerLeaveEvent e, ButtonToolTip btnTip)
        {
            btnTip.tooltip.CloseTooltip();
        }

        public void SetMessage(string newMsg)
        {
            message = newMsg;
        }

        //function to register events for button entering and leaving
        private void RegisterEvents()
        {
            this.button.RegisterCallback<PointerEnterEvent, ButtonToolTip>(ShowToolTip, this);
            this.button.RegisterCallback<PointerLeaveEvent, ButtonToolTip>(HideToolTip, this);
        }
    }
}
