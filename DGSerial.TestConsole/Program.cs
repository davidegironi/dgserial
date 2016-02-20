#region License
// Copyright (c) 2015 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

using System;

namespace DG.Serial.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Type testclass = typeof(DGSerialTest);
            NUnit.ConsoleRunner.Runner.Main(new string[] {
                testclass.Assembly.Location, "/basepath="+"../../../" + testclass.Assembly.ManifestModule.Name.Substring(0, testclass.Assembly.ManifestModule.Name.Length - 4) + "/bin/Debug"
            });
            Console.ReadKey();
        }
    }
}
