using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Models
{
    public class PiHeaderModel : GpioHeaderModel
    {
        public PiHeaderModel() : base(40)
        {
            initHeader();
        }

        private void initHeader()
        {
            var bcmMapH0 = new Dictionary<int, int>()
            {
                {3, 2}, {5, 3}, {7, 4}, {8, 14}, {10, 15}, {11, 17},
                {12, 18}, {13, 27}, {15, 22}, {16, 23}, {18, 24}, {19, 10},
                {21, 9}, {22, 25}, {23, 11}, {24, 8}, {26, 7}, {27, 0},
                {28, 1}, {29, 5}, {31, 6}, {32, 12}, {33, 13}, {35, 19},
                {36, 16}, {37, 26}, {38, 20}, {40, 21}
            };

            var wiringPiMapH0 = new Dictionary<int, int>()
            {
                {3, 8}, {5, 9}, {7, 7}, {8, 15}, {10, 16}, {11, 0},
                {12, 1}, {13, 2}, {15, 3}, {16, 4}, {18, 24}, {19, 12},
                {21, 13}, {22, 6}, {23, 14}, {24, 10}, {26, 11}, {27, 30},
                {28, 31}, {29, 21}, {31, 22}, {32, 26}, {33, 23}, {35, 24},
                {36, 27}, {37, 25}, {38, 28}, {40, 29}
            };

            LoadPinMap(PinIndexScheme.Bcm, bcmMapH0);
            LoadPinMap(PinIndexScheme.WiringPi, wiringPiMapH0);

            AddPin(new PinMeta(1,"3v3"));
            AddPin(new PinMeta(2, "5v"));
            AddPin(new PinMeta(4, "5v"));
            AddPin(new PinMeta(6, "GND"));
            AddPin(new PinMeta(9, "GND"));
            AddPin(new PinMeta(14, "GND"));
            AddPin(new PinMeta(17, "3v3"));
            AddPin(new PinMeta(20, "GND"));
            AddPin(new PinMeta(25, "GND"));
            AddPin(new PinMeta(30, "GND"));
            AddPin(new PinMeta(34, "GND"));
            AddPin(new PinMeta(39, "GND"));
        }
    }
}
