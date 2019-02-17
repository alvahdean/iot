using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Models
{
    public enum PinNamingScheme
    {
        Unspecified = 0,
        Default,            // The most common name/use of the pin
        Alt0,               // pinout.xyz Alt0 name
        Alt1,               // pinout.xyz Alt1 name
        Alt2,               // pinout.xyz Alt2 name
        Alt3,               // pinout.xyz Alt3 name
        Alt4,               // pinout.xyz Alt4 name
        Alt5,               // pinout.xyz Alt5 name
        Custom              // A settable custom name, specific to the project 
    }
}
