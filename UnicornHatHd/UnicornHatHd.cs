using System;
using System.Buffers.Binary;
using System.Device.Spi;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Iot.Device.UnicornHatHd
{
    public class UnicornHatHd : IDisposable
    {
        /// <summary>
        /// Total number of pixels
        /// </summary>
        public const int NumberOfPixels = 256;

        /// <summary>
        /// Number of pixels per row
        /// </summary>
        public const int Rows = 16;

        /// <summary>
        /// Number of pixels per column
        /// </summary>
        public const int Columns = 16;

        private const int PixelLength = 3;

        private const int FrameBufferLength = PixelLength * Rows * Columns;

        private byte[] frameBuffer = new byte[FrameBufferLength];

        private Bitmap image = null;
        public Graphics Graph {get; set;}

        private float[,] brightnessLevels = new float[Rows, Columns];

        private const byte SOF = 0x72; // Start of frame
        private SpiDevice spiDevice;

        public UnicornHatHd()
        {
            image = new Bitmap(Rows, Columns);
            Graph = Graphics.FromImage(image);

            SpiConnectionSettings settings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 9_000_000,
                Mode = SpiMode.Mode0
            };

            spiDevice = SpiDevice.Create(settings);
        }


        public void SetPixel(int x, int y, Color color, float brightness = 1)
        {
            if (x < 0 || x >= Rows)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0 || y >= Columns)
                throw new ArgumentOutOfRangeException(nameof(y));

            image.SetPixel(x, y, color);
            brightnessLevels[x, y] = brightness;
        }

        public void Fill(Color color, float brightness = 1)
        {
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                    SetPixel(x, y, color, brightness);
        }

        public void RandomFill()
        {
            var r = new Random();
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                {
                    var c = Color.FromArgb(0xff, r.Next(255), r.Next(255), r.Next(255));
                    SetPixel(x, y, c, (float)r.NextDouble());
                }
        }

        public void Clear()
        {
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                    SetPixel(x, y, Color.Empty);
        }


        public void DrawText(int x, int y, string text)
        {
            var image = new Bitmap(16, 16);

            var graph = Graphics.FromImage(image);

            //graph.DrawString("Hello", );
        }

        public void Show()
        {
            int i = 0;

            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Columns; y++)
                {
                    var pixel = image.GetPixel(x, y);
                    var brightness = brightnessLevels[x, y];
                    frameBuffer[i++] = (byte)(pixel.R * brightness);
                    frameBuffer[i++] = (byte)(pixel.G * brightness);
                    frameBuffer[i++] = (byte)(pixel.B * brightness);
                }

            spiDevice.WriteByte(SOF);
            spiDevice.Write(frameBuffer);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    image?.Dispose();
                    image = null;

                    Graph?.Dispose();
                    Graph = null;

                    spiDevice?.Dispose();
                    spiDevice = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnicornHatHd()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
