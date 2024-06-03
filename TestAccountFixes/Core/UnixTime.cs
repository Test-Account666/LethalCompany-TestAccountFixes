using System;

namespace TestAccountFixes.Core;

public static class UnixTime {
    public static long GetCurrentTime() => (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
}