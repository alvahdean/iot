// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.I2c;
using System.Numerics;
using System.Threading;

namespace Iot.Device.Bno055
{
    public class Bno055 : IDisposable
    {
        public const byte BNO055_ADDRESS_A = 0x28;
        public const byte BNO055_ADDRESS_B = 0x29;
        private const byte BNO055_ID = 0xA0;
        private const int NUM_BNO055_OFFSET_REGISTERS = 22;

        private OperationMode _mode;
        private readonly I2cDevice _i2cDevice;

        public Bno055(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        public bool begin(OperationMode mode = OperationMode.OPERATION_MODE_NDOF)
        {
            // BNO055 clock stretches for 500us or more!
            // todo: investigate applicability to Raspberry Pi
            // Wire.setClockStretchLimit(1000); // Allow for 1000us of clock stretching

            /* Make sure we have the right device */
            byte id = read8(Register.BNO055_CHIP_ID_ADDR);
            if (id != BNO055_ID)
            {
                Thread.Sleep(1000); // hold on for boot
                id = read8(Register.BNO055_CHIP_ID_ADDR);
                if (id != BNO055_ID)
                {
                    return false; // still not? ok bail
                }
            }

            /* Switch to config mode (just in case since this is the default) */
            setMode(OperationMode.OPERATION_MODE_CONFIG);

            /* Reset */
            write8(Register.BNO055_SYS_TRIGGER_ADDR, 0x20);
            while (read8(Register.BNO055_CHIP_ID_ADDR) != BNO055_ID)
            {
                Thread.Sleep(10);
            }

            Thread.Sleep(50);

            /* Set to normal power mode */
            write8(Register.BNO055_PWR_MODE_ADDR, (byte) PowerMode.POWER_MODE_NORMAL);
            Thread.Sleep(10);

            write8(Register.BNO055_PAGE_ID_ADDR, 0);

            /* Set the output units */
            //byte unitsel = (0 << 7) | // Orientation = Android
            //                  (0 << 4) | // Temperature = Celsius
            //                  (0 << 2) | // Euler = Degrees
            //                  (1 << 1) | // Gyro = Rads
            //                  (0 << 0);  // Accelerometer = m/s^2
            //write8(Register.BNO055_UNIT_SEL_ADDR, unitsel);

            /* Configure axis mapping (see section 3.4) */
            //write8(Register.BNO055_AXIS_MAP_CONFIG_ADDR, (byte)AxisRemapConfig.REMAP_CONFIG_P2); // P0-P7, Default is P1
            //Thread.Sleep(10);
            //write8(Register.BNO055_AXIS_MAP_SIGN_ADDR, (byte)AxisRemapSign.REMAP_SIGN_P2); // P0-P7, Default is P1
            //Thread.Sleep(10);

            write8(Register.BNO055_SYS_TRIGGER_ADDR, 0x0);
            Thread.Sleep(10);
            /* Set the requested operating mode (see section 3.3) */
            setMode(mode);
            Thread.Sleep(20);

            return true;
        }

        public void setMode(OperationMode mode)
        {
            _mode = mode;
            write8(Register.BNO055_OPR_MODE_ADDR, (byte) _mode);
            Thread.Sleep(30);
        }

        public RevInfo getRevInfo()
        {
            byte a, b;
            RevInfo info = new RevInfo();

            /* Check the accelerometer revision */
            info.accel_rev = read8(Register.BNO055_ACCEL_REV_ID_ADDR);

            /* Check the magnetometer revision */
            info.mag_rev = read8(Register.BNO055_MAG_REV_ID_ADDR);

            /* Check the gyroscope revision */
            info.gyro_rev = read8(Register.BNO055_GYRO_REV_ID_ADDR);

            /* Check the SW revision */
            info.bl_rev = read8(Register.BNO055_BL_REV_ID_ADDR);

            a = read8(Register.BNO055_SW_REV_ID_LSB_ADDR);
            b = read8(Register.BNO055_SW_REV_ID_MSB_ADDR);
            info.sw_rev = (UInt16) ((b << 8) | a);

            return info;
        }

        public void setExtCrystalUse(bool usextal)
        {
            OperationMode lastMode = _mode;

            /* Switch to config mode (just in case since this is the default) */
            setMode(OperationMode.OPERATION_MODE_CONFIG);
            Thread.Sleep(25);

            write8(Register.BNO055_PAGE_ID_ADDR, 0);
            if (usextal)
            {
                write8(Register.BNO055_SYS_TRIGGER_ADDR, 0x80);
            }
            else
            {
                write8(Register.BNO055_SYS_TRIGGER_ADDR, 0x00);
            }

            Thread.Sleep(10);
            /* Set the requested operating mode (see section 3.3) */
            setMode(lastMode);
            Thread.Sleep(20);
        }

        //// public void getSystemStatus(uint8_t* system_status,uint8_t* self_test_result,uint8_t* system_error)
        public SystemStatus getSystemStatus()
        {
            SystemStatus result = new SystemStatus();
            write8(Register.BNO055_PAGE_ID_ADDR, 0);

            /* System Status (see section 4.3.58)
               ---------------------------------
               0 = Idle
               1 = System Error
               2 = Initializing Peripherals
               3 = System Iniitalization
               4 = Executing Self-Test
               5 = Sensor fusio algorithm running
               6 = System running without fusion algorithms */

            result.system_status = read8(Register.BNO055_SYS_STAT_ADDR);

            /* Self Test Results (see section )
               --------------------------------
               1 = test passed, 0 = test failed

               Bit 0 = Accelerometer self test
               Bit 1 = Magnetometer self test
               Bit 2 = Gyroscope self test
               Bit 3 = MCU self test

               0x0F = all good! */

            result.self_test_result = read8(Register.BNO055_SELFTEST_RESULT_ADDR);

            /* System Error (see section 4.3.59)
               ---------------------------------
               0 = No error
               1 = Peripheral initialization error
               2 = System initialization error
               3 = Self test result failed
               4 = Register map value out of range
               5 = Register map address out of range
               6 = Register map write error
               7 = BNO low power mode not available for selected operat ion mode
               8 = Accelerometer power mode not available
               9 = Fusion algorithm configuration error
               A = Sensor configuration error */

            result.system_error = read8(Register.BNO055_SYS_ERR_ADDR);

            Thread.Sleep(200);

            return result;
        }

        public Calibration getCalibration()
        {
            Calibration calibration = new Calibration();

            byte data = read8(Register.BNO055_CALIB_STAT_ADDR);

            calibration.system = (byte) ((data >> 6) & 0x03);
            calibration.gyro = (byte) ((data >> 4) & 0x03);
            calibration.accel = (byte) ((data >> 2) & 0x03);
            calibration.mag = (byte) (data & 0x03);

            return calibration;
        }

        public Vector3 getVector(VectorType vector_type)
        {
            var xyz = new Vector3();

            /* Read vector data (6 bytes) */
            var buffer = readLen((Register) vector_type, 6);

            Int16 x = (Int16) (buffer[0] | buffer[1] << 8);
            Int16 y = (Int16) (buffer[2] | buffer[3] << 8);
            Int16 z = (Int16) (buffer[4] | buffer[5] << 8);

            /* Convert the value to an appropriate range (section 3.6.4) */
            /* and assign the value to the Vector type */
            switch (vector_type)
            {
                case VectorType.VECTOR_MAGNETOMETER: /* 1uT = 16 LSB */
                case VectorType.VECTOR_GYROSCOPE: /* 1dps = 16 LSB */
                case VectorType.VECTOR_EULER: /* 1 degree = 16 LSB */
                    xyz = new Vector3(x / 16.0f, y / 16.0f, z / 16.0f);
                    break;
                case VectorType.VECTOR_ACCELEROMETER:
                case VectorType.VECTOR_LINEARACCEL:
                case VectorType.VECTOR_GRAVITY:
                    /* 1m/s^2 = 100 LSB */
                    xyz = new Vector3(x / 100.0f, y / 100.0f, z / 100.0f);
                    break;
            }

            return xyz;
        }

        public Quaternion getQuat()
        {
            /* Read quat data (8 bytes) */
            var buffer = readLen(Register.BNO055_QUATERNION_DATA_W_LSB_ADDR, 8);
            Int16 w = (Int16) ((buffer[1] << 8) | buffer[0]);
            Int16 x = (Int16) ((buffer[3] << 8) | buffer[2]);
            Int16 y = (Int16) ((buffer[5] << 8) | buffer[4]);
            Int16 z = (Int16) ((buffer[7] << 8) | buffer[6]);

            /* Assign to Quaternion */
            /* See http://ae-bst.resource.bosch.com/media/products/dokumente/bno055/BST_BNO055_DS000_12~1.pdf
               3.6.5.5 Orientation (Quaternion)  */
            float scale = (float) (1.0 / (1 << 14));
            Quaternion quat = new Quaternion(scale * w, scale * x, scale * y, scale * z);

            return quat;
        }

        public sbyte getTemp()
        {
            var tempC = (sbyte) (read8(Register.BNO055_TEMP_ADDR));

            return tempC;
        }

        /* Functions to deal with raw calibration data */
        public byte[] getSensorOffsetBytes()
        {
            if (!isFullyCalibrated())
                return null;

            OperationMode lastMode = _mode;
            setMode(OperationMode.OPERATION_MODE_CONFIG);
            Thread.Sleep(25);

            var calibData = readLen(Register.ACCEL_OFFSET_X_LSB_ADDR, NUM_BNO055_OFFSET_REGISTERS);

            setMode(lastMode);

            return calibData;
        }

        ////public bool getSensorOffsets(Offsets &offsets_type)
        public Offsets getSensorOffsets()
        {
            if (!isFullyCalibrated())
                return null;

            OperationMode lastMode = _mode;
            setMode(OperationMode.OPERATION_MODE_CONFIG);
            Thread.Sleep(25);
            Offsets offsets_type = new Offsets();

            offsets_type.accel_offset_x = (ushort) ((read8(Register.ACCEL_OFFSET_X_MSB_ADDR) << 8) |
                                                    (read8(Register.ACCEL_OFFSET_X_LSB_ADDR)));
            offsets_type.accel_offset_y = (ushort) ((read8(Register.ACCEL_OFFSET_Y_MSB_ADDR) << 8) |
                                                    (read8(Register.ACCEL_OFFSET_Y_LSB_ADDR)));
            offsets_type.accel_offset_z = (ushort) ((read8(Register.ACCEL_OFFSET_Z_MSB_ADDR) << 8) |
                                                    (read8(Register.ACCEL_OFFSET_Z_LSB_ADDR)));

            offsets_type.gyro_offset_x =
                (ushort) ((read8(Register.GYRO_OFFSET_X_MSB_ADDR) << 8) | (read8(Register.GYRO_OFFSET_X_LSB_ADDR)));
            offsets_type.gyro_offset_y =
                (ushort) ((read8(Register.GYRO_OFFSET_Y_MSB_ADDR) << 8) | (read8(Register.GYRO_OFFSET_Y_LSB_ADDR)));
            offsets_type.gyro_offset_z =
                (ushort) ((read8(Register.GYRO_OFFSET_Z_MSB_ADDR) << 8) | (read8(Register.GYRO_OFFSET_Z_LSB_ADDR)));

            offsets_type.mag_offset_x =
                (ushort) ((read8(Register.MAG_OFFSET_X_MSB_ADDR) << 8) | (read8(Register.MAG_OFFSET_X_LSB_ADDR)));
            offsets_type.mag_offset_y =
                (ushort) ((read8(Register.MAG_OFFSET_Y_MSB_ADDR) << 8) | (read8(Register.MAG_OFFSET_Y_LSB_ADDR)));
            offsets_type.mag_offset_z =
                (ushort) ((read8(Register.MAG_OFFSET_Z_MSB_ADDR) << 8) | (read8(Register.MAG_OFFSET_Z_LSB_ADDR)));

            offsets_type.accel_radius =
                (ushort) ((read8(Register.ACCEL_RADIUS_MSB_ADDR) << 8) | (read8(Register.ACCEL_RADIUS_LSB_ADDR)));
            offsets_type.mag_radius =
                (ushort) ((read8(Register.MAG_RADIUS_MSB_ADDR) << 8) | (read8(Register.MAG_RADIUS_LSB_ADDR)));

            setMode(lastMode);

            return offsets_type;
        }

        public void setSensorOffsets(byte[] calibData)
        {
            OperationMode lastMode = _mode;
            setMode(OperationMode.OPERATION_MODE_CONFIG);
            Thread.Sleep(25);

            // todo: Can write an array/span to sequential registers in single op?
            write8(Register.ACCEL_OFFSET_X_LSB_ADDR, calibData[0]);
            write8(Register.ACCEL_OFFSET_X_MSB_ADDR, calibData[1]);
            write8(Register.ACCEL_OFFSET_Y_LSB_ADDR, calibData[2]);
            write8(Register.ACCEL_OFFSET_Y_MSB_ADDR, calibData[3]);
            write8(Register.ACCEL_OFFSET_Z_LSB_ADDR, calibData[4]);
            write8(Register.ACCEL_OFFSET_Z_MSB_ADDR, calibData[5]);

            write8(Register.GYRO_OFFSET_X_LSB_ADDR, calibData[6]);
            write8(Register.GYRO_OFFSET_X_MSB_ADDR, calibData[7]);
            write8(Register.GYRO_OFFSET_Y_LSB_ADDR, calibData[8]);
            write8(Register.GYRO_OFFSET_Y_MSB_ADDR, calibData[9]);
            write8(Register.GYRO_OFFSET_Z_LSB_ADDR, calibData[10]);
            write8(Register.GYRO_OFFSET_Z_MSB_ADDR, calibData[11]);

            write8(Register.MAG_OFFSET_X_LSB_ADDR, calibData[12]);
            write8(Register.MAG_OFFSET_X_MSB_ADDR, calibData[13]);
            write8(Register.MAG_OFFSET_Y_LSB_ADDR, calibData[14]);
            write8(Register.MAG_OFFSET_Y_MSB_ADDR, calibData[15]);
            write8(Register.MAG_OFFSET_Z_LSB_ADDR, calibData[16]);
            write8(Register.MAG_OFFSET_Z_MSB_ADDR, calibData[17]);

            write8(Register.ACCEL_RADIUS_LSB_ADDR, calibData[18]);
            write8(Register.ACCEL_RADIUS_MSB_ADDR, calibData[19]);

            write8(Register.MAG_RADIUS_LSB_ADDR, calibData[20]);
            write8(Register.MAG_RADIUS_MSB_ADDR, calibData[21]);
            setMode(lastMode);
        }

        public void setSensorOffsets(Offsets offsets_type)
        {
            OperationMode lastMode = _mode;
            setMode(OperationMode.OPERATION_MODE_CONFIG);
            Thread.Sleep(25);

            write8(Register.ACCEL_OFFSET_X_LSB_ADDR, (byte) ((offsets_type.accel_offset_x) & 0x0FF));
            write8(Register.ACCEL_OFFSET_X_MSB_ADDR, (byte) ((offsets_type.accel_offset_x >> 8) & 0x0FF));
            write8(Register.ACCEL_OFFSET_Y_LSB_ADDR, (byte) ((offsets_type.accel_offset_y) & 0x0FF));
            write8(Register.ACCEL_OFFSET_Y_MSB_ADDR, (byte) ((offsets_type.accel_offset_y >> 8) & 0x0FF));
            write8(Register.ACCEL_OFFSET_Z_LSB_ADDR, (byte) ((offsets_type.accel_offset_z) & 0x0FF));
            write8(Register.ACCEL_OFFSET_Z_MSB_ADDR, (byte) ((offsets_type.accel_offset_z >> 8) & 0x0FF));

            write8(Register.GYRO_OFFSET_X_LSB_ADDR, (byte) ((offsets_type.gyro_offset_x) & 0x0FF));
            write8(Register.GYRO_OFFSET_X_MSB_ADDR, (byte) ((offsets_type.gyro_offset_x >> 8) & 0x0FF));
            write8(Register.GYRO_OFFSET_Y_LSB_ADDR, (byte) ((offsets_type.gyro_offset_y) & 0x0FF));
            write8(Register.GYRO_OFFSET_Y_MSB_ADDR, (byte) ((offsets_type.gyro_offset_y >> 8) & 0x0FF));
            write8(Register.GYRO_OFFSET_Z_LSB_ADDR, (byte) ((offsets_type.gyro_offset_z) & 0x0FF));
            write8(Register.GYRO_OFFSET_Z_MSB_ADDR, (byte) ((offsets_type.gyro_offset_z >> 8) & 0x0FF));

            write8(Register.MAG_OFFSET_X_LSB_ADDR, (byte) ((offsets_type.mag_offset_x) & 0x0FF));
            write8(Register.MAG_OFFSET_X_MSB_ADDR, (byte) ((offsets_type.mag_offset_x >> 8) & 0x0FF));
            write8(Register.MAG_OFFSET_Y_LSB_ADDR, (byte) ((offsets_type.mag_offset_y) & 0x0FF));
            write8(Register.MAG_OFFSET_Y_MSB_ADDR, (byte) ((offsets_type.mag_offset_y >> 8) & 0x0FF));
            write8(Register.MAG_OFFSET_Z_LSB_ADDR, (byte) ((offsets_type.mag_offset_z) & 0x0FF));
            write8(Register.MAG_OFFSET_Z_MSB_ADDR, (byte) ((offsets_type.mag_offset_z >> 8) & 0x0FF));

            write8(Register.ACCEL_RADIUS_LSB_ADDR, (byte) ((offsets_type.accel_radius) & 0x0FF));
            write8(Register.ACCEL_RADIUS_MSB_ADDR, (byte) ((offsets_type.accel_radius >> 8) & 0x0FF));

            write8(Register.MAG_RADIUS_LSB_ADDR, (byte) ((offsets_type.mag_radius) & 0x0FF));
            write8(Register.MAG_RADIUS_MSB_ADDR, (byte) ((offsets_type.mag_radius >> 8) & 0x0FF));

            setMode(lastMode);
        }

        public bool isFullyCalibrated()
        {
            Calibration calibration = getCalibration();

            return calibration.FullyCalibrated;
        }

        private byte read8(Register register)
        {
            _i2cDevice.WriteByte((byte) register);

            return _i2cDevice.ReadByte();
        }

        private byte[] readLen(Register register, byte len)
        {
            var data = new Span<byte>(new byte[len]);

            _i2cDevice.WriteByte((byte) register);
            _i2cDevice.Read(data);

            return data.ToArray();
        }

        // todo: Just throw exception
        private bool write8(Register register, byte value)
        {
            try
            {
                _i2cDevice.WriteByte((byte) register);
                _i2cDevice.WriteByte(value);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
        }
    }
}
