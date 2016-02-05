using System;
using System.Diagnostics;
using System.IO;

namespace Tester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter PDF file path");

            var fileName = Console.ReadLine();

            var buffer = File.ReadAllBytes(fileName);

            using (var file = new Engine.FileParsers.Pdf(buffer))
            {
                Console.WriteLine("Content:");
                Console.WriteLine(file.Content().Result);

                Console.ReadLine();

                var thumbnail = file.Thumbnail(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png")).Result;

                var p = Process.Start(thumbnail);
                //p.WaitForExit();

                File.Delete(thumbnail);
            }

            Console.ReadLine();
        }
    }
}