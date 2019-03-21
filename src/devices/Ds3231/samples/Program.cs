﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Device.I2c;
using System.Device.I2c.Drivers;

namespace Iot.Device.Ds3231.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            I2cConnectionSettings settings = new I2cConnectionSettings(1, Ds3231.DefaultI2cAddress);
            // get I2cDevice (in Linux)
            UnixI2cDevice device = new UnixI2cDevice(settings);
            // get I2cDevice (in Win10)
            //Windows10I2cDevice device = new Windows10I2cDevice(settings);

            using (Ds3231 rtc = new Ds3231(device))
            {
                // set DS3231 time
                rtc.DateTime = DateTime.Now;

                // loop
                while (true)
                {
                    // read temperature
                    double temp = rtc.Temperature.Celsius;
                    // read time
                    DateTime dt = rtc.DateTime;

                    Console.WriteLine($"Time: {dt.ToString("yyyy/MM/dd HH:mm:ss")}");
                    Console.WriteLine($"Temperature: {temp} ℃");
                    Console.WriteLine();

                    // wait for a second
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
