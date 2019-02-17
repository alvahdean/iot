using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Iot.Device.Models
{
    public class GpioHeaderModel
    {
        private readonly List<PinMeta> _pinInfo;

        protected GpioHeaderModel(int pinCount)
        {
            _pinInfo=new List<PinMeta>(pinCount);
            for (int i = 1; i <= pinCount; i++)
            {
                _pinInfo.Add(new PinMeta(i,$"PIN[{i}]"));
            }
        }

        public PinIndexScheme DefaultIndexScheme { get; set; } = PinIndexScheme.Bcm;
        public PinMeta this[int id] => this[DefaultIndexScheme, id];
        public PinMeta this[PinIndexScheme scheme, int id] => _pinInfo.FirstOrDefault(t => t[scheme] == id);
        public PinMeta this[string name] => _pinInfo.FirstOrDefault(t => t.Name == name);
        public PinMeta this[PinNamingScheme scheme, string name] => _pinInfo.FirstOrDefault(t => t[scheme] == name);
        public int PinCount => _pinInfo.Count;
        public bool IsValidPhysicalPin(int id) => id > 0 && id <= PinCount;

        protected void LoadPinMap(PinIndexScheme scheme, Dictionary<int, int> map)
        {
            foreach (var item in map)
            {
                this[PinIndexScheme.Physical, item.Key][scheme] = item.Value;
            }
        }

        protected void AddPin(PinMeta pin, bool replace = false)
        {
            if (!IsValidPhysicalPin(pin?.PhysicalId ?? 0))
            {
                throw new InvalidOperationException($"Invalid physical pin [{pin?.PhysicalId}]");
            }

            var existingPin = this[PinIndexScheme.Physical, pin.PhysicalId];
            if (existingPin != null)
            {
                if (replace)
                {
                    _pinInfo.Remove(existingPin);
                }
                else
                {
                    throw new DuplicateNameException($"Pin {existingPin.PhysicalId} already exists");
                }
            }
            _pinInfo.Add(pin);
        }
    }
}
