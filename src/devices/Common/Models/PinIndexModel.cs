using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Models
{
    public class PinIndexModel
    {
        public PinIndexModel() : this(PinIndexScheme.Unspecified)
        {
        }

        public PinIndexModel(PinIndexScheme scheme, int id =-1)
        {
            Scheme = scheme;
            Id = id;
        }

        public static implicit operator int(PinIndexModel model) => model.Id;

        public PinIndexScheme Scheme { get; set; }
        public int Id { get; set; }
    }
}
