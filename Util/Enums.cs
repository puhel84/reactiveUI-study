using System;

namespace Util
{
    public enum Category
    {
        Broker,
        Station,
        Simulation
    }

    [Flags]
    public enum ConnectionState
    {
        Disconnected = 0b0000_0001,
        ConnectionError = 0b0000_0011,
        ConnectionFail = 0b0000_0111,
        ConnectionTimeout = 0b0000_1011,

        Connecting = 0b0001_0010,
        Connected = 0b0001_0000,
        Running = 0b0011_0000,
        Recording = 0b0111_0000,
        WorkingError = 0b1001_0000,
        WorkingTimeout = 0b1001_1000,
    }
}
