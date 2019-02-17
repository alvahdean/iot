using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Models
{
    public class RaspberryPiModel : IotBoardModel
    {
        private RaspberryPiModel()
        {
            initHeader();
        }

        private void initHeader()
        {
            var piHeader=new PiHeaderModel();
            AddHeader(piHeader);
        }
    }
}
