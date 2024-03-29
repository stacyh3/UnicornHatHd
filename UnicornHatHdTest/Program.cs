﻿using Iot.Device.UnicornHatHd;
using System;
using System.Drawing;

namespace UnicornHatHdTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using var uhat = new UnicornHatHd();

            uhat.RandomFill();
            uhat.Show();

            Console.WriteLine("Press Enter to test Fill");
            Console.ReadLine();

            uhat.Fill(Color.Azure);
            uhat.Show();

            Console.WriteLine("Press Enter to test Clear");
            Console.ReadLine();

            uhat.Clear();
            uhat.Show();

            var pen = new Pen(Color.Blue);
            uhat.Graph.DrawEllipse(pen, 0, 0, 16, 16);
            uhat.Show();

            Console.WriteLine("Press Enter to for fun");
            Console.ReadLine();


            Console.WriteLine("Press q to quit");

            uhat.Clear();
            uhat.Show();

            ConsoleKeyInfo keyInfo = default;
            while (keyInfo.Key != ConsoleKey.Q)
            {
                uhat.RandomFill();
                uhat.Show();
                System.Threading.Thread.Sleep(125);
                if(Console.KeyAvailable)
                    keyInfo = Console.ReadKey();
            }

            uhat.Clear();
            uhat.Show();
        }
    }
}
