using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Models
{
    [Flags]
    public enum ButtonEvent
    {
        Unspecified=0,
        Pressed =1,
        Released = 2,
        Click = 4,
        DoubleClick = 8,
    }
}
