﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Iot.Device.Ssd1306.Command;
using Xunit;

namespace Iot.Device.Mcp23xxx.Tests
{
    public class NoOperationTests
    {
        [Fact]
        public void Get_Bytes()
        {
            NoOperation noOperation = new NoOperation();
            byte[] actualBytes = noOperation.GetBytes();
            Assert.Equal(new byte[] { 0xE3 }, actualBytes);
        }
    }
}
