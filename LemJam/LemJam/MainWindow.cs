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

        List<PathItem> pathItems;
        //List<FileItem> fileItems;
        Database db;


        public MainWindow()
        {
            InitializeComponent();

            db = new Database();
            refreshListViewPathItem();
            //fileItems = db.GetFileItems();
        }

        private void neuesVerzeichnisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch(folderBrowserDialog.ShowDialog())
            {
                case DialogResult.OK:

                    PathItem item = new PathItem(folderBrowserDialog.SelectedPath, Path.GetDirectoryName(folderBrowserDialog.SelectedPath), PathItemType.Slave);
                    item.Save(db);

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

            PathItem pi = (PathItem)listViewPathItems.SelectedItems[0].Tag;


        }

        private void refreshListViewPathItem()
        {
            pathItems = db.GetPathItems();

            listViewPathItems.BeginUpdate();
            listViewPathItems.Items.Clear();

            foreach (PathItem item in pathItems)
            {
                ListViewItem lvi = new ListViewItem(new string[] { item.DisplayName, item.Path, item.Type.ToString() });

                lvi.Tag = item;

                listViewPathItems.Items.Add(lvi);

            }

            listViewPathItems.EndUpdate();
        }

        private void schalteMasterSlaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PathItem pi = (PathItem)listViewPathItems.SelectedItems[0].Tag;

            if (pi.Type == PathItemType.Master)
                pi.Type = PathItemType.Slave;
            else
                pi.Type = PathItemType.Master;

            pi.Update(db);

            refreshListViewPathItem();
        }

        private void löschenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PathItem pi = (PathItem)listViewPathItems.SelectedItems[0].Tag;

            pi.Delete(db);
        }

        private void PathItemContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if(listViewPathItems.SelectedItems.Count == 0)
            {
                PathItemContextMenu.Items[1].Enabled = false;
                PathItemContextMenu.Items[2].Enabled = false;
            } 
            else
            {
                PathItemContextMenu.Items[1].Enabled = true;
                PathItemContextMenu.Items[2].Enabled = true;
            }
        }
    }
}
