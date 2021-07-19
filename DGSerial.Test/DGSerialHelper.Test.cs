#region License
// Copyright (c) 2015 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

using DG.Serial.Helper;
using NUnit.Framework;
using System.Linq;

namespace DG.Serial.Test
{
    [TestFixture]
    public class DGSerialHelperTest
    {
        [Test]
        public void StringToBytes()
        {
            Assert.That(DGSerialHelper.StringToBytes("Test"), Is.EqualTo(new byte[] { 84, 101, 115, 116 }));
        }

        [Test]
        public void BytesToString()
        {
            Assert.That(DGSerialHelper.BytesToString(new byte[] { 84, 101, 115, 116 }), Is.EqualTo("Test"));
        }

        [Test]
        public void ListPorts()
        {
            Assert.That(DGSerialHelper.ListPorts().Count(), Is.AtLeast(1));
        }

    }
}
