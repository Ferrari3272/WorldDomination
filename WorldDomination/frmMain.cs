using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldDomination.Controllers;
using WorldDomination.Models;

namespace WorldDomination
{
    public partial class frmMain : Form
    {
        public delegate void PopulateTreeDelegate(Tree tree);
        public PopulateTreeDelegate populateTreeDelegate;
        public delegate void EnableControlsDelegate();
        public EnableControlsDelegate enableControlsDelegate;
        public frmMain()
        {
            InitializeComponent();
            populateTreeDelegate = new PopulateTreeDelegate(PopulateTree);
            enableControlsDelegate = new EnableControlsDelegate(EnableControls);
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            if (CheckURLValid(txtUrl.Text) == true)
            {
                cmdSearch.Enabled = false;
                cmdExport.Enabled = false;
                trvTree.Enabled = false;
                txtUrl.Enabled = false;
                pbLoading.Visible = true;

                trvTree.Nodes.Clear();

                Thread thread = new Thread(delegate ()
                {
                    GetTree(txtUrl.Text, 2);
                });
                thread.Start();
            }
            else
            {
                MessageBox.Show("Please enter a valid URL", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmdExport_Click(object sender, EventArgs e)
        {
            sfdSaveFile.Title = "Export to CSV file";
            sfdSaveFile.AddExtension = true;
            sfdSaveFile.DefaultExt = "csv";
            DialogResult result = sfdSaveFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                ExportToCSV(sfdSaveFile.FileName);
            }
        }

        private void GetTree(string url, int depth)
        {
            var crawlerController = new CrawlerController();

            var tree = crawlerController.GetTree(url, depth);
            Invoke(populateTreeDelegate, new object[] { tree });
            Invoke(enableControlsDelegate);
        }

        private void PopulateTree(Tree tree)
        {
            foreach (var node in tree.Nodes)
            {
                if (node.Parent != null)
                {
                    trvTree.Nodes.Find(node.Parent.Url, true).FirstOrDefault().Nodes.Add(node.Url, node.Url);
                }
                else
                {
                    trvTree.Nodes.Add(node.Url, node.Url);
                }
            }
        }

        private void EnableControls()
        {
            cmdSearch.Enabled = true;
            cmdExport.Enabled = true;
            trvTree.Enabled = true;
            txtUrl.Enabled = true;
            pbLoading.Visible = false;
        }

        private void ExportToCSV(string path)
        {
            string csvData = "";

            BuildCSV(trvTree.Nodes, ref csvData, 0);

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(csvData);
            }
        }

        private void BuildCSV(TreeNodeCollection nodes, ref string csvData, int depth)
        {
            foreach (TreeNode node in nodes)
            {
                csvData = csvData + new String(',', depth) + node.Text + "\n";

                if (node.Nodes.Count > 0)
                    BuildCSV(node.Nodes, ref csvData, depth + 1);
            }
        }

        private static bool CheckURLValid(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }
    }
}
