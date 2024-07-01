using System;
using System.Linq;
using System.Threading;

namespace DesktopManager {
    static class Program {
        static void Main(string[] args) {
            Config.LoadConfig();

            if (args.Contains("--print-monitors")) {
                Console.WriteLine(string.Join('\n', MonitorPosition.GetMonitors()));
            }

            Taskbar.FixAllTaskbars();

            MonitorPosition.SetAllMonitorPositions();

            Keybinds.Handlers.AddRange(new Keybinds.IKeybindHandler[] {
                new WindowManagement(),
                new VoicemeeterEq()
            });
            Keybinds.RegisterKeybinds();

            Thread trayIconThread = new(TrayIcon.Create);
            trayIconThread.Start();

            WindowManager.FixAllWindows();

            Console.WriteLine("Starting Message Pump...");
            EventManager.StartMessagePump();
        }
    }
}
