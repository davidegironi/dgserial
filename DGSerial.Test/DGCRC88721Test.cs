#region License
// Copyright (c) 2015 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

using System;
using System.Linq;
using System.Configuration;
using System.Threading;
using DG.Serial.Helper;
using NUnit.Framework;
using System.IO.Ports;

namespace DG.Serial.Test
{
    [TestFixture]
    public class DGCRC88721Test
    {
        [Test]
        public void DGCRC88721()
        {
            Assert.That(DGCRC8721.CRC8721(0xAA), Is.EqualTo(0xF6));

            Assert.That(DGCRC8721.CRC8721(new byte[] { 0xAA, 0x01, 0xBB }), Is.EqualTo(0x97));
        }

    }
}
