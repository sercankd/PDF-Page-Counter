using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using iTextSharp.text.pdf;

namespace PDF_Page_Counter
{
    public partial class Form1 : Form
    {
        private static readonly string[] SizeSuffixes = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};
        public Form1()
        {
            Application.VisualStyleState = VisualStyleState.NonClientAreaEnabled;
            InitializeComponent();
        }
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            var handles = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
            foreach (var s in handles)
                if (File.Exists(s))
                {
                    if (string.Compare(Path.GetExtension(s), ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
                        AddFileToListview(s);
                }
                else if (Directory.Exists(s))
                {
                    var di = new DirectoryInfo(s);
                    var files = di.GetFiles("*.pdf", checkBox1.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                        AddFileToListview(file.FullName);
                }
        }

        private void AddFileToListview(string fullFilePath)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!File.Exists(fullFilePath))
                return;
            var fileName = Path.GetFileName(fullFilePath);
            var dirName = Path.GetDirectoryName(fullFilePath);
            if (dirName != null && dirName.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)))
                dirName = dirName.Substring(0, dirName.Length - 1);
            var itm = listView1.Items.Add(fileName);
            if (fileName != null)
            {
                var fFile = new FileInfo(fileName);
            }
            var length = new FileInfo(fullFilePath).Length;
            itm.SubItems.Add(SizeSuffix(length)); //size column
            var pdfReader = new PdfReader(fullFilePath);
            var numberOfPages = pdfReader.NumberOfPages;
            itm.SubItems.Add(numberOfPages.ToString()); //pages count
            itm.SubItems.Add(dirName); //path

            //calculate items count with linq
            var countItems = listView1.Items.Cast<ListViewItem>().Count();
            toolStripStatusLabel3.Text = countItems.ToString();

            //calculate total pages count with linq
            var countTotalPages = listView1.Items.Cast<ListViewItem>().Sum(item => int.Parse(item.SubItems[2].Text));
            toolStripStatusLabel4.Text = countTotalPages.ToString();
            Cursor.Current = Cursors.Default;
        }

        private static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (value < 0) return "-" + SizeSuffix(-value);
            if (value == 0) return "0.0 bytes";

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            var mag = (int) Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            var adjustedSize = (decimal) value / (1L << (mag * 10));

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
            Process.Start("https://github.com/sercankd/PDF-Page-Counter/");
        }

        // Clears listview items sets counters to zero
        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            toolStripStatusLabel3.Text = @"0";
            toolStripStatusLabel4.Text = @"0";
        }
    }
}