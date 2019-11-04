using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogerProgram
{
    class RogerReporter
    {
        private static string RogerLogPath { get; set; }    //Adres ścieżki z której będą czytane logi Rogera
        private static string AttendanceRaportPath { get; set; }    //Adres ścieżki, gdzie będzie produkowany/tworzony raport obecności
        private static int TargetHour { get; set; }    //Godzina terminu
        private static int TargetMinute { get; set; }   //Minuta terminu   

        static RogerReporter()
        {
            RogerLogPath = "C:\\Users\\rafal\\Desktop\\test.txt";
            AttendanceRaportPath = "C:\\Users\\rafal\\Desktop\\Obecnosc.csv";
            TargetHour = 12;
            TargetMinute = 0;
        }
    }
}
