// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;

Console.WriteLine(@"Console application that prevents Windows from going to sleep until application is closed.

                __          __                                     _       __  
   ____  ____  / /_   _____/ /__  ___  ____     ____ _   _      __(_)___  / /__
  / __ \/ __ \/ __/  / ___/ / _ \/ _ \/ __ \   / __ `/  | | /| / / / __ \/ //_/
 / / / / /_/ / /_   (__  ) /  __/  __/ /_/ /  / /_/ /   | |/ |/ / / / / / ,<   
/_/ /_/\____/\__/  /____/_/\___/\___/ .___/   \__,_/    |__/|__/_/_/ /_/_/|_|  
                                   /_/                                         

https://github.com/drweb86/windows-sleep-prevention

Usages:
'WindowsSleepPrevention.exe'
'WindowsSleepPrevention.exe 5' - Prevents sleep for 5 hours.

> Application has started prevention of Windows from sleepping.
Press [Enter] to stop preventing from sleep and quit the application.

");

NativeMethods.PreventSleep();

if (args.Length == 1 && int.TryParse(args[0], out var hoursToSleep))
{
    Console.WriteLine($"Sleeping is prevented for {hoursToSleep} hours. After that application will terminate.");

    // Convert hours to milliseconds
    int timeoutMilliseconds = (int)(hoursToSleep * 3600 * 1000);

    // Set up a timer to exit the application
    Timer timer = new Timer(_ =>
    {
        Console.WriteLine($"Time limit of {hoursToSleep} hours reached. Exiting...");
        Environment.Exit(0);
    }, null, timeoutMilliseconds, Timeout.Infinite);

}

Console.ReadLine();

static class NativeMethods
{
    [DllImport("kernel32.dll")]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern uint SetThreadExecutionState(uint esFlags);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private const uint _eS_CONTINUOUS = 0x80000000;
    private const uint _eS_SYSTEM_REQUIRED = 0x00000001;

    public static void PreventSleep()
    {
#pragma warning disable CA1806 // Do not ignore method results
        SetThreadExecutionState(_eS_CONTINUOUS | _eS_SYSTEM_REQUIRED);
#pragma warning restore CA1806 // Do not ignore method results
    }
}