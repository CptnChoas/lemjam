using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LemJam
{
    public partial class MainWindow : Form
    {

        List<MediaFolder> mediaFolder;


        public MainWindow()
        {
            InitializeComponent();
            refreshListViewPathItem();

            Program.Logger.NewLogMessage += Logger_NewLogMessage;
        }

        private void Logger_NewLogMessage(string message)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                lbConsole.BeginUpdate();
                lbConsole.Items.Add(message);
                lbConsole.EndUpdate();
            }));
        }

        private void neuesVerzeichnisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch(folderBrowserDialog.ShowDialog())
            {
                case DialogResult.OK:

                    MediaFolder item = new MediaFolder(folderBrowserDialog.SelectedPath, Path.GetDirectoryName(folderBrowserDialog.SelectedPath), MediaFolderType.Slave, false);
                    item.Save();

                    refreshListViewPathItem();

                    break;
                default:
                    return;
            }
        }

        private void listViewPathItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPathItems.SelectedItems.Count == 0)
                return;

            MediaFolder pi = (MediaFolder)listViewPathItems.SelectedItems[0].Tag;


        }

        private void refreshListViewPathItem()
        {
            mediaFolder = Program.db.GetPathItems();

            listViewPathItems.BeginUpdate();
            listViewPathItems.Items.Clear();

            foreach (MediaFolder item in mediaFolder)
            {
                ListViewItem lvi = new ListViewItem(new string[] { item.DisplayName, item.Path, item.Type.ToString(), item.WorkerActive ? "Aktiv" : "Nicht aktiv" });

                lvi.Tag = item;

                listViewPathItems.Items.Add(lvi);

            }

            listViewPathItems.EndUpdate();
        }

        private void schalteMasterSlaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MediaFolder pi = (MediaFolder)listViewPathItems.SelectedItems[0].Tag;

            if (pi.Type == MediaFolderType.Master)
                pi.Type = MediaFolderType.Slave;
            else
                pi.Type = MediaFolderType.Master;

            refreshListViewPathItem();
        }

        private void löschenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MediaFolder pi = (MediaFolder)listViewPathItems.SelectedItems[0].Tag;

            pi.Delete();
        }

        private void PathItemContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (listViewPathItems.SelectedItems.Count == 0)
                return;

            if (((MediaFolder)listViewPathItems.SelectedItems[0].Tag).WorkerActive)
                PathItemContextMenu.Items[3].Text = "Deaktiviere Aktualisierung";
            else
                PathItemContextMenu.Items[3].Text = "Aktiviere Aktualisierung";

            if (listViewPathItems.SelectedItems.Count == 0)
            {
                PathItemContextMenu.Items[1].Enabled = false;
                PathItemContextMenu.Items[2].Enabled = false;
                PathItemContextMenu.Items[3].Enabled = false;
            }
            else if (listViewPathItems.SelectedItems.Count == 1)
            {
                PathItemContextMenu.Items[1].Enabled = true;
                PathItemContextMenu.Items[2].Enabled = true;
                PathItemContextMenu.Items[3].Enabled = true;
            }
            else
                MessageBox.Show("Now Syncing");
        }

        private void deaktiviereAktualisierungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MediaFolder folder = (MediaFolder)listViewPathItems.SelectedItems[0].Tag;
            folder.WorkerActive = !folder.WorkerActive;

            refreshListViewPathItem();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (MediaFolder mf in mediaFolder)
                mf.ShutdownWorker();
        }
    }
}
