using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Models
{
    [Flags]
    public enum PinCapabilities
    {
        None=0,
        CanWrite=1,
        CanRead=2,
        CanPwm=4,
        CanSoftPwm=8,
    }
}
