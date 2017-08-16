using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zipper
{
    public partial class formMain : Form
    {
        public formMain()
        {
            InitializeComponent();
        }

        public ZipFile file;
        public List<ZipEntry> entries;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                Filter = "Zip compressed folders (*.zip)|*.zip"
            };

            if (open.ShowDialog() == DialogResult.OK)
            {
                file = new ZipFile(open.FileName);
                entries = file.Cast<ZipEntry>().ToList();

                ReloadEntries("/");
            }
        }

        public ICollection<ZipEntry> GetEntries(string path)
        {
            List<ZipEntry> pathEntries = new List<ZipEntry>();
            string correctPath = path.TrimStart('/');

            foreach (var entry in entries)
            {
                if (entry.Name.Contains(correctPath) &&
                    entry.Name.TrimEnd('/').LastIndexOf('/') == correctPath.LastIndexOf('/'))
                    pathEntries.Add(entry);
            }

            return pathEntries;
        }

        public void ReloadEntries(string path)
        {
            lsvFiles.Items.Clear();

            if (path != "/")
            {
                var root = lsvFiles.Items.Add((".."));
                root.ImageIndex = root.StateImageIndex = 1;

                root.Tag = new EntryMetadata
                {
                    IsFolder = true,
                    Path = path.Remove(path.TrimEnd('/').LastIndexOf('/') + 1)
                };
            }

            foreach (var entry in GetEntries(path))
            {
                string filename = entry.Name.Trim('/');
                filename = filename.Remove(0, filename.LastIndexOf('/') + 1);

                var item = lsvFiles.Items.Add(filename);

                if (entry.IsFile)
                    item.SubItems.Add(Utility.GetBytesReadable(entry.Size).ToString());

                if (!entry.IsFile)
                    item.ImageIndex = item.StateImageIndex = 1;
                else if (entry.Name.EndsWith(".wav")
                    || entry.Name.EndsWith(".mp3")
                    || entry.Name.EndsWith(".flac")
                    || entry.Name.EndsWith(".aiff")
                    || entry.Name.EndsWith(".wma"))
                    item.ImageIndex = item.StateImageIndex = 2;
                else if (entry.Name.EndsWith(".bmp")
                    || entry.Name.EndsWith(".jpg")
                    || entry.Name.EndsWith(".jpeg")
                    || entry.Name.EndsWith(".png")
                    || entry.Name.EndsWith(".gif")
                    || entry.Name.EndsWith(".tiff"))
                    item.ImageIndex = item.StateImageIndex = 3;
                else if (entry.Name.EndsWith(".mp4")
                    || entry.Name.EndsWith(".mkv")
                    || entry.Name.EndsWith(".webm"))
                    item.ImageIndex = item.StateImageIndex = 4;
                else if (entry.Name.EndsWith(".zip")
                    || entry.Name.EndsWith(".7z")
                    || entry.Name.EndsWith(".gz")
                    || entry.Name.EndsWith(".rar"))
                    item.ImageIndex = item.StateImageIndex = 5;
                else
                    item.ImageIndex = item.StateImageIndex = 0;

                item.Tag = new EntryMetadata
                {
                    IsFolder = !entry.IsFile,
                    Path = "/" + entry.Name
                };
            }
        }

        private void lsvFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lsvFiles.SelectedIndices.Count > 0)
            {
                EntryMetadata meta = lsvFiles.SelectedItems[0].Tag as EntryMetadata;

                if (meta.IsFolder)
                    ReloadEntries(meta.Path);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            lsvFiles.Items.Clear();
        }
    }

    public class EntryMetadata
    {
        public bool IsFolder;
        public string Path;
    }
}
