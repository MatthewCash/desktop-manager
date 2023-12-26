using System;
using System.Drawing;
using System.Windows.Forms;

static class TrayIcon {
    private static NotifyIcon trayIcon;

    public static void Create() {
        trayIcon = new() {
            Icon = SystemIcons.Application, // TODO: get an actual icon?
            Text = "Desktop Manager",
            Visible = true
        };

        ContextMenuStrip contextMenu = new();

        trayIcon.ContextMenuStrip = contextMenu;

        contextMenu.Items.Add(new ToolStripMenuItem("Reload Config", null, ReloadConfig));
        contextMenu.Items.Add(new ToolStripMenuItem("Print Monitors", null, PrintMonitors));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(new ToolStripMenuItem("Reposition Monitors", null, RepositionMonitors));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, Exit));

        Application.Run();
    }

    private static void ReloadConfig(object sender, EventArgs e) {
        Config.LoadConfig();
    }

    private static void PrintMonitors(object sender, EventArgs e) {
        MessageBox.Show(
            string.Join('\n', MonitorPosition.GetMonitors()),
            "Monitors"
        );
    }

    private static void RepositionMonitors(object sender, EventArgs e) {
        MonitorPosition.SetAllMonitorPositions();
    }

    private static void Exit(object sender, EventArgs e) {
        trayIcon.Visible = false;

        Application.Exit();
    }
}
