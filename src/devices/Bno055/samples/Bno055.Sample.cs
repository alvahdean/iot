using System;
using System.Numerics;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using System.Threading;
using Iot.Device.Bno055;

namespace Iot.Device.Bno055.Samples
{
    class Program
    {
        private const int BNO055_SAMPLERATE_DELAY_MS = 250;

        static void Main(string[] args)
        {
            Bno055 bno = Initialize(Bno055.BNO055_ADDRESS_A);
            if (bno == null)
            {
                Console.WriteLine("BNO055 device initialization failed, exiting");
                return;
            }

            /* Display the current temperature */
            sbyte temp = bno.getTemp();
            Console.WriteLine($"Current Temperature: {temp}°C");
            Console.WriteLine();

            bno.setExtCrystalUse(true);

            Console.WriteLine("Calibration status values: 0=uncalibrated, 3=fully calibrated");

            while (true)
            {
                // Possible vector values can be:
                // - VECTOR_ACCELEROMETER - m/s^2
                // - VECTOR_MAGNETOMETER  - uT
                // - VECTOR_GYROSCOPE     - rad/s
                // - VECTOR_EULER         - degrees
                // - VECTOR_LINEARACCEL   - m/s^2
                // - VECTOR_GRAVITY       - m/s^2
                Vector3 euler = bno.getVector(VectorType.VECTOR_EULER);

                /* Display the floating point data */
                Console.WriteLine($"EULER: X: {euler.X:f2}, Y: {euler.Y:f2}, Z: {euler.Z:f2}");

                
                // Quaternion data
                Quaternion quat = bno.getQuat();
                Console.WriteLine($"QUATERNION: X: {quat.X:f2}, Y: {quat.Y:f2}, Z: {quat.Z:f2}, Z: {quat.W:f2}");                

                /* Display calibration status for each sensor. */
                var calibration = bno.getCalibration();
                Console.WriteLine(
                    $"CALIBRATION: Sys: {calibration.system}, Gyro: {calibration.gyro}, Accel: {calibration.accel}, Mag: {calibration.mag}");

                Console.WriteLine();

                Thread.Sleep(BNO055_SAMPLERATE_DELAY_MS);
            }
        }

        private static Bno055 Initialize(byte i2cAddress)
        {
            var i2cConnectionSettings = new I2cConnectionSettings(1, i2cAddress);
            var i2cDevice = new UnixI2cDevice(i2cConnectionSettings);
            Bno055 bno055 = new Bno055(i2cDevice);

            /* Initialise the sensor */
            if (!bno055.begin())
            {

                /* There was a problem detecting the BNO055 ... check your connections */
                Console.Error.WriteLine("Ooops, no BNO055 detected ... Check your wiring or I2C ADDR!");
                return null;
            }

            Thread.Sleep(1000);
            return bno055;
        }
    }
}
