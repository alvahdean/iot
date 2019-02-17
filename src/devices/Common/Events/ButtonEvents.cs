// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using System.Device.Spi;
using System.Device.Spi.Drivers;
using Iot.Device.Models;

namespace Iot.Device.Common
{
    public class ButtonStateChangedArgs
    {
        public ButtonStateChangedArgs(PinMeta pin, ButtonEvent buttonEvent)
        {
            Pin = pin;
            ButtonEvent = buttonEvent;
        }

        public PinMeta Pin { get; }
        public ButtonEvent ButtonEvent { get; }
    }

    public delegate void ButtonStateChangedHandler(object sender, ButtonStateChangedArgs args);
}

