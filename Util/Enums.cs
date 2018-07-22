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
        Connecting = 0b0000_1001,

        Connected = 0b0001_0000,
        Recording = 0b0011_0000,
        WorkingError = 0b0101_0000,
        WorkingTimeout = 0b1101_0000,
    }
}
