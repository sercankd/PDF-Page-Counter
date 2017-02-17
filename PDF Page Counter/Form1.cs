using System;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Linq;

namespace PDF_Page_Counter
{
    public partial class Form1 : Form
    {
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public Form1()
        {
            Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.AllowDrop = true;
            listView1.DragDrop += new DragEventHandler(listView1_DragDrop);
            listView1.DragEnter += new DragEventHandler(listView1_DragEnter);
        }
        void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] handles = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string s in handles)
            {
                if (File.Exists(s))
                {
                    if (string.Compare(Path.GetExtension(s), ".pdf", true) == 0)
                    {
                        AddFileToListview(s);
                    }
                }
                else if (Directory.Exists(s))
                {
                    DirectoryInfo di = new DirectoryInfo(s);
                    FileInfo[] files = di.GetFiles("*.pdf", SearchOption.AllDirectories);
                    foreach (FileInfo file in files)
                        AddFileToListview(file.FullName);
                }
            }

        }
        private void AddFileToListview(string fullFilePath)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!File.Exists(fullFilePath))
                return;
            string fileName = Path.GetFileName(fullFilePath);
            string dirName = Path.GetDirectoryName(fullFilePath);
            if (dirName.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)))
                dirName = dirName.Substring(0, dirName.Length - 1);
            ListViewItem itm = listView1.Items.Add(fileName);
            FileInfo fFile = new FileInfo(fileName);
            long length = new System.IO.FileInfo(fullFilePath).Length;
            itm.SubItems.Add(SizeSuffix(length)); //size column
            PdfReader pdfReader = new PdfReader(fullFilePath);
            int numberOfPages = pdfReader.NumberOfPages;
            itm.SubItems.Add(numberOfPages.ToString()); //pages count
            itm.SubItems.Add(dirName); //path

            //calculate items count with linq
            int countItems = listView1.Items.Cast<ListViewItem>().Count();
            toolStripStatusLabel3.Text = countItems.ToString();

            //calculate items count with linq
            int countTotalPages = listView1.Items.Cast<ListViewItem>().Sum(item => int.Parse(item.SubItems[2].Text));
            toolStripStatusLabel4.Text = countTotalPages.ToString();
            Cursor.Current = Cursors.Default;

        }
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        private void toolStripStatusLabel5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/sercankd/PDF-Page-Counter/");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = listView1.Items.Cast<ListViewItem>().Count();
            toolStripStatusLabel4.Text = count.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            toolStripStatusLabel3.Text = "0";
            toolStripStatusLabel4.Text = "0";

        }
    }
}
