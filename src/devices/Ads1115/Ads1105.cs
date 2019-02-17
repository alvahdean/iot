// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.I2c;

namespace Iot.Device
{
    /// <summary>
    /// Analog-to-Digital Converter ADS1115
    /// </summary>
    public class Ads1015 : Ads1x15
    {
        private I2cDevice _sensor = null;

        private readonly byte _inputMultiplexer;
        private readonly byte _measuringRange;
        private readonly byte _dataRate;

        /// <summary>
        /// Initialize a new Ads1115 device connected through I2C
        /// </summary>
        /// <param name="sensor">I2C Device, like UnixI2cDevice or Windows10I2cDevice</param>
        /// <param name="inputMultiplexer">Input Multiplexer</param>
        /// <param name="measuringRange">Programmable Gain Amplifier</param>
        /// <param name="dataRate">Data Rate</param>
        public Ads1015(I2cDevice sensor, InputMultiplexer inputMultiplexer = InputMultiplexer.AIN0, MeasuringRange measuringRange = MeasuringRange.FS4096, DataRate dataRate = DataRate.SPS128)
            :base(sensor,inputMultiplexer,measuringRange,dataRate)
        {
            if (_measuringRange > (byte)MeasuringRange.FS4096)
            {
                throw new ArgumentOutOfRangeException($"Maximum resolution for ADS1015 is 12 bits");
            }

            Initialize();
        }
    }
}
