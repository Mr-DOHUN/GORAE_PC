using System;
using System.IO.Ports;
using NModbus.IO;

public class SerialPortAdapter : IStreamResource
{
    private readonly SerialPort _serialPort;

    public event EventHandler PortOpened;

    public SerialPortAdapter(SerialPort serialPort)
    {
        _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
    }

    public int InfiniteTimeout => SerialPort.InfiniteTimeout;

    public int ReadTimeout
    {
        get => _serialPort.ReadTimeout;
        set => _serialPort.ReadTimeout = value;
    }

    public int WriteTimeout
    {
        get => _serialPort.WriteTimeout;
        set => _serialPort.WriteTimeout = value;
    }

    public void DiscardInBuffer()
    {
        _serialPort.DiscardInBuffer();
    }

    public void DiscardOutBuffer()
    {
        _serialPort.DiscardOutBuffer();
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        return _serialPort.Read(buffer, offset, count);
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        _serialPort.Write(buffer, offset, count);
    }

    public void Open()
    {
        _serialPort.Open();
        OnPortOpened(EventArgs.Empty);
    }

    protected virtual void OnPortOpened(EventArgs e)
    {
        PortOpened?.Invoke(this, e);
    }

    public void Dispose()
    {
        _serialPort.Dispose();
    }
}