using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iot.Device.Common;
using Iot.Device.Models;

namespace Iot.Device
{
    // todo: Create contract and concrete RaspberryPi implementation
    public class PinMeta
    {
        private readonly List<PinIndexModel> _pinIds = new List<PinIndexModel>();
        private readonly List<PinNameModel> _pinNames = new List<PinNameModel>();
        private PinCapabilities _pinCapabilities = PinCapabilities.None;

        public PinMeta(int physicalId, string defaultName)
        {
        }

        public int? this[PinIndexScheme scheme]
        {
            get => _pinIds.FirstOrDefault(t => t.Scheme == scheme);
            set
            {
                if (scheme == PinIndexScheme.Unspecified)
                {
                    throw new InvalidOperationException($"Can't set {scheme} pin Id");
                }

                var model = _pinIds.FirstOrDefault(t => t.Scheme == scheme);
                if (!value.HasValue)
                {
                    if (model != null)
                    {
                        _pinIds.Remove(model);
                    }
                }
                else if (model != null)
                {
                    model.Id = value.Value;
                }
                else
                {
                    _pinIds.Add(new PinIndexModel(scheme, value.Value));
                }
            }
        }

        public string this[PinNamingScheme scheme]
        {
            get => _pinNames.FirstOrDefault(t => t.Scheme == scheme);
            set
            {
                if (scheme == PinNamingScheme.Unspecified)
                {
                    throw new InvalidOperationException($"Can't set {scheme} pin name");
                }

                var model = _pinNames.FirstOrDefault(t => t.Scheme == scheme);
                if (model != null)
                {
                    model.Name = value;
                }
                else
                {
                    _pinNames.Add(new PinNameModel(scheme, value));
                }
            }
        }

        public bool HasIndex(PinIndexScheme scheme) => _pinIds.Any(t => t.Scheme == scheme);

        public bool HasName(PinNamingScheme scheme) => _pinNames.Any(t => t.Scheme == scheme);

        public PinCapabilities Capabilities => _pinCapabilities;

        public bool HasCapabilities(PinCapabilities capabilities) =>
            (capabilities != PinCapabilities.None) && Capabilities.HasFlag(capabilities);

        public bool HasAnyCapability(PinCapabilities capabilities)
        {
            int mask = (int) capabilities;
            return mask != 0 && (mask & (int) Capabilities) != 0;
        }

        #region Shortcuts
        public int PhysicalId
        {
            get => this[PinIndexScheme.Physical].Value;
            set => this[PinIndexScheme.Physical] = value;
        }

        public int? BcmId
        {
            get => this[PinIndexScheme.Bcm];
            set => this[PinIndexScheme.Bcm] = value;
        }

        public int? WiringPiId
        {
            get => this[PinIndexScheme.WiringPi];
            set => this[PinIndexScheme.WiringPi] = value;
        }

        public string Name
        {
            get => this[PinNamingScheme.Custom] ?? this[PinNamingScheme.Default];
            set => this[PinNamingScheme.Custom] = value;
        }

        public bool CanRead => HasCapabilities(PinCapabilities.CanRead);
        public bool CanWrite => HasCapabilities(PinCapabilities.CanWrite);
        public bool CanPwm => HasCapabilities(PinCapabilities.CanPwm);
        public bool CanSoftPwm => HasCapabilities(PinCapabilities.CanSoftPwm);
        #endregion
    }
}
