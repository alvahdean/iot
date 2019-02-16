﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Iot.Device.Ssd1306.Command;
using Xunit;

namespace Iot.Device.Mcp23xxx.Tests
{
    public class SetComOutputScanDirectionTests
    {
        [Fact]
        public void Get_Bytes_With_Default_Values()
        {
            SetComOutputScanDirection setComOutputScanDirection = new SetComOutputScanDirection();
            byte[] actualBytes = setComOutputScanDirection.GetBytes();
            Assert.Equal(new byte[] { 0xC0 }, actualBytes);
        }

        [Theory]
        [InlineData(false, new byte[] { 0xC8 })]
        [InlineData(true, new byte[] { 0xC0 })]
        public void Get_Bytes(bool normalMode, byte[] expectedBytes)
        {
            SetComOutputScanDirection setComOutputScanDirection = new SetComOutputScanDirection(normalMode);
            byte[] actualBytes = setComOutputScanDirection.GetBytes();
            Assert.Equal(expectedBytes, actualBytes);
        }
    }
}
