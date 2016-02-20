#region License
// Copyright (c) 2015 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

using System;
using System.Linq;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace DG.Serial
{
    public class DGSerial
    {
        /// <summary>
        /// Serial port
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        /// Default Handshake value
        /// </summary>
        private const Handshake DefaultHandshake = Handshake.None;
        /// <summary>
        /// Default StopBits value
        /// </summary>
        private const StopBits DefaultStopBits = StopBits.One;

        /// <summary>
        /// Default DataBits value
        /// </summary>
        private const int DefaultDataBits = 8;

        /// <summary>
        /// Default Parity value
        /// </summary>
        private const Parity DefaultParity = Parity.None;

        /// <summary>
        /// Default ReadTimeout
        /// </summary>
        private const int DefaultReadTimeout = 1000;

        /// <summary>
        /// Default WriteTimeout
        /// </summary>
        private const int DefaultWriteTimeout = 1000;

        /// <summary>
        /// Loaded ReadTimeout
        /// </summary>
        private readonly int _loadedReadTimeout = DefaultReadTimeout;

        /// <summary>
        /// Loaded WriteTimeout
        /// </summary>
        private readonly int _loadedWriteTimeout = DefaultWriteTimeout;

        /// <summary>
        /// Initialize the port connection
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="handshake"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public DGSerial(
            string portName,
            int baudRate,
            Parity parity,
            int dataBits,
            StopBits stopBits,
            Handshake handshake,
            int readTimeout,
            int writeTimeout)
        {
            _loadedReadTimeout = readTimeout;
            _loadedWriteTimeout = writeTimeout;

            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

            _serialPort.Handshake = handshake;

            _serialPort.ReadTimeout = _loadedReadTimeout;
            _serialPort.WriteTimeout = _loadedWriteTimeout;
        }
        
        /// <summary>
        /// Initialize the port connection
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public DGSerial(
            string portName,
            int baudRate,
            Parity parity,
            int dataBits,
            StopBits stopBits,
            int readTimeout,
            int writeTimeout)
            : this(portName, baudRate, parity, dataBits, stopBits, DefaultHandshake, readTimeout, writeTimeout)
        { }

        /// <summary>
        /// Initialize the port connection
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public DGSerial(
            string portName,
            int baudRate,
            Parity parity,
            int dataBits,
            int readTimeout,
            int writeTimeout)
            : this(portName, baudRate, parity, dataBits, DefaultStopBits, DefaultHandshake, readTimeout, writeTimeout)
        { }

        /// <summary>
        /// Initialize the port connection
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public DGSerial(
            string portName,
            int baudRate,
            Parity parity,
            int readTimeout,
            int writeTimeout)
            : this(portName, baudRate, parity, DefaultDataBits, DefaultStopBits, DefaultHandshake, readTimeout, writeTimeout)
        { }

        /// <summary>
        /// Initialize the port connection
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public DGSerial(
            string portName,
            int baudRate,
            int readTimeout,
            int writeTimeout)
            : this(portName, baudRate, DefaultParity, DefaultDataBits, DefaultStopBits, DefaultHandshake, readTimeout, writeTimeout)
        { }

        /// <summary>
        /// Initialize the port connection
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        public DGSerial(
            string portName,
            int baudRate)
            : this(portName, baudRate, DefaultParity, DefaultDataBits, DefaultStopBits, DefaultHandshake, DefaultReadTimeout, DefaultWriteTimeout)
        { }

        /// <summary>
        /// Return the current serial port instance
        /// </summary>
        /// <returns></returns>
        public SerialPort Get()
        {
            return _serialPort;
        }

        /// <summary>
        /// Check if the port connection is Open
        /// </summary>
        public bool IsOpen()
        {
            return _serialPort.IsOpen;
        }

        /// <summary>
        /// Open the port connection
        /// </summary>
        public bool Open()
        {
            if (!_serialPort.IsOpen)
                try
                {
                    _serialPort.Open();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.DiscardInBuffer();
                }
                catch { }

            return _serialPort.IsOpen;
        }

        /// <summary>
        /// Close the port connection
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (_serialPort.IsOpen)
                try
                {
                    _serialPort.Close();
                }
                catch { }
            else
                return false;

            return !_serialPort.IsOpen;
        }

        /// <summary>
        /// Discard data from the input buffer
        /// </summary>
        public void DiscardInBuffer()
        {
            if (!_serialPort.IsOpen)
                return;

            _serialPort.DiscardInBuffer();
        }
        
        /// <summary>
        /// Discard data from the output buffer
        /// </summary>
        public void DiscardOutBuffer()
        {
            if (!_serialPort.IsOpen)
                return;

            _serialPort.DiscardOutBuffer();
        }
        
        /// <summary>
        /// Get number of bytes to read
        /// </summary>
        /// <returns></returns>
        public int BytesToRead()
        {
            if (!_serialPort.IsOpen)
                return -1;

            return _serialPort.BytesToRead;
        }

        /// <summary>
        /// Get number of bytes to write
        /// </summary>
        /// <returns></returns>
        public int BytesToWrite()
        {
            if (!_serialPort.IsOpen)
                return -1;

            return _serialPort.BytesToWrite;
        }

        /// <summary>
        /// Attach a serial data reciver event handler
        /// </summary>
        /// <param name="DataReceivedEventHandler"></param>
        public void AttachDataReceived(SerialDataReceivedEventHandler DataReceivedEventHandler)
        {
            if (!_serialPort.IsOpen)
                return;

            _serialPort.DataReceived += DataReceivedEventHandler;
        }
        
        /// <summary>
        /// Read bytes from port connection
        /// </summary>
        /// <param name="count"></param>
        /// <param name="readTimeout"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int count, int readTimeout)
        {
            byte[] ret = new byte[] { };

            if (!_serialPort.IsOpen)
                return ret;

            if (readTimeout != _loadedReadTimeout)
                _serialPort.ReadTimeout = readTimeout;

            if (_serialPort.ReadTimeout == SerialPort.InfiniteTimeout)
                while (_serialPort.BytesToRead <= 0) ;

            try
            {
                if (count == -1 && _serialPort.ReadTimeout != SerialPort.InfiniteTimeout)
                {
                    TimeSpan maxDuration = TimeSpan.FromMilliseconds(readTimeout);
                    Stopwatch sw = Stopwatch.StartNew();
                    bool read = false;
                    int tries = 0;
                    int triesMstime = 5;
                    int maxTries = 10; //min out timeout = triesMstime*maxtries
                    while (sw.Elapsed < maxDuration && !read)
                    {
                        if (_serialPort.BytesToRead > 0)
                        {
                            byte[] readbytes = new byte[_serialPort.BytesToRead];
                            _serialPort.Read(readbytes, 0, readbytes.Length);
                            ret = ret.Concat(readbytes).ToArray();
                        }
                        else
                        {
                            tries++;
                            if (tries > maxTries)
                                read = true;
                        }
                        Thread.Sleep(triesMstime);
                    }
                }
                else
                {
                    ret = new byte[_serialPort.BytesToRead];
                    if (ret.Length > 0)
                        _serialPort.Read(ret, 0, ret.Length);
                    if (ret.Length != count)
                        ret = null;
                }
            }
            catch (TimeoutException)
            {
                ret = null;
            }

            if (_serialPort.ReadTimeout != _loadedReadTimeout)
                _serialPort.ReadTimeout = _loadedReadTimeout;

            return ret;
        }

        /// <summary>
        /// Read bytes from port connection
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            return ReadBytes(count, _loadedReadTimeout);
        }

        /// <summary>
        /// Read until byte to using the escape byte from port connection
        /// </summary>
        /// <param name="to"></param>
        /// <param name="escape"></param>
        /// <param name="readTimeout"></param>
        /// <returns></returns>
        public byte[] ReadBytesTo(byte to, byte escape, int readTimeout)
        {
            byte[] ret = null;

            if (!_serialPort.IsOpen)
                return ret;

            if (readTimeout != _loadedReadTimeout)
                _serialPort.ReadTimeout = readTimeout;

            if (_serialPort.ReadTimeout == SerialPort.InfiniteTimeout)
                while (_serialPort.BytesToRead <= 0) ;

            try
            {
                ret = new byte[] { };
                byte read = 0x00;
                byte previousbyte = 0x00;
                while(true)
                {
                    read = (byte)_serialPort.ReadByte();
                    if (read == to && previousbyte != escape)
                        break;
                    if (read != escape)
                    {
                        if (read != to || (read == to && previousbyte == escape))
                            ret = ret.Concat(new byte[] { read }).ToArray();
                    }
                    else
                    {
                        if(previousbyte == escape)
                            ret = ret.Concat(new byte[] { read }).ToArray();
                    }
                    previousbyte = read;
                };
            }
            catch (TimeoutException) { }

            if (_serialPort.ReadTimeout != _loadedReadTimeout)
                _serialPort.ReadTimeout = _loadedReadTimeout;

            return ret;
        }

        /// <summary>
        /// Read bytes from port connection
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] ReadBytesBlocking(int count)
        {
            return ReadBytes(count, SerialPort.InfiniteTimeout);
        }

        /// <summary>
        /// Read all bytes from port connection
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes()
        {
            return ReadBytes(-1, _loadedReadTimeout);
        }

        /// <summary>
        /// Read all bytes from port connection
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytesBlocking()
        {
            return ReadBytes(-1, SerialPort.InfiniteTimeout);
        }

        /// <summary>
        /// Write bytes to port connection
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="writeTimeout"></param>
        /// <returns></returns>
        public bool WriteBytes(byte[] buffer, int writeTimeout)
        {
            bool ret = false;

            if (!_serialPort.IsOpen)
                return ret;

            if (writeTimeout != _loadedWriteTimeout)
                _serialPort.WriteTimeout = writeTimeout;

            try
            {
                _serialPort.Write(buffer, 0, buffer.Length);
                ret = true;
            }
            catch (TimeoutException)
            { }

            if (_serialPort.WriteTimeout != _loadedWriteTimeout)
                _serialPort.WriteTimeout = _loadedWriteTimeout;

            return ret;
        }

        /// <summary>
        /// Write bytes to port connection
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool WriteBytes(byte[] buffer)
        {
            return WriteBytes(buffer, _loadedWriteTimeout);
        }

        /// <summary>
        /// Write bytes to port connection
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool WriteBytesBlocking(byte[] buffer)
        {
            return WriteBytes(buffer, SerialPort.InfiniteTimeout);
        }
    }
}
