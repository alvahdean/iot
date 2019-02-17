using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Models
{
    public class PinNameModel
    {
        public PinNameModel() : this(PinNamingScheme.Unspecified)
        {
        }

        public PinNameModel(PinNamingScheme scheme, string name=null)
        {
            Scheme = scheme;
            Name = name;
        }

        public static implicit operator string(PinNameModel model) => model.Name;

        public PinNamingScheme Scheme { get; set; }
        public string Name { get; set; }
    }
}
