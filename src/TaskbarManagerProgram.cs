using System;

namespace TaskBarManager {
    static class Program {
        static void Main() {
            Taskbar mainTaskbar = new Taskbar(true, null, TaskbarAccent.AccentState.ACCENT_DISABLED);
            mainTaskbar.FixTaskbar();
            mainTaskbar.RegisterEvents();

            var secondaryTaskbarPosition = new TaskbarPosition.TaskbarRect(2560, 1391, 1913, 40);
            Taskbar secondaryTaskbar = new Taskbar(false, secondaryTaskbarPosition, TaskbarAccent.AccentState.ACCENT_ENABLE_TRANSPARANT);
            secondaryTaskbar.FixTaskbar();
            secondaryTaskbar.RegisterEvents();

            Console.WriteLine("Starting Message Pump...");
            EventManager.StartMessagePump();
        }
    }
}