using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Iot.Device.Models
{
    public class IotBoardModel
    {
        private List<GpioHeaderModel> _headers=new List<GpioHeaderModel>();

        public GpioHeaderModel this[int index] => _headers[index];

        public IEnumerable<GpioHeaderModel> Headers => _headers;

        public int HeaderCount => _headers.Count;

        public int AddHeader(GpioHeaderModel header)
        {
            _headers.Add(header);

            return _headers.IndexOf(header);
        }
    }
}
