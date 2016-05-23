using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace SplitCSV
{
   

    public partial class SplitCSV : Form
    {
        public SplitCSV()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.csv_text;
            progressBar1.Visible = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
           
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            txtSource.Text = openFileDialog1.FileName;
            if (!txtSource.Text.Contains(".csv"))
            {
                MessageBox.Show("Please select csv file");
                txtSource.Text = string.Empty;
            }
        }

        private void txtNumberOfLineCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;

        }

        private void btnSplit_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtSource.Text))
            {
                MessageBox.Show("Please select csv file");
                return;
            }
            if (string.IsNullOrEmpty(txtNumberOfLineCount.Text) || Convert.ToInt32(txtNumberOfLineCount.Text)==0)
            {
                MessageBox.Show("Please sepcify the split count");
                return;

            }
            progressBar1.Visible = true;
            string sourceFileName = txtSource.Text;
            string [] arrayFiles = txtSource.Text.Split('\\');
            StringBuilder sb = new StringBuilder();
            int linesPerFile =Convert.ToInt32(txtNumberOfLineCount.Text);
            string fileName = string.Empty;
            foreach (var item in arrayFiles)
            {
                if (!item.Contains(".csv"))
                {
                    sb.Append(item + "\\");

                }
                else {
                    fileName = item;
                
                }


            }
            fileName = fileName.ToLower();
            fileName = fileName.Replace(".csv", "");
            string destinationFileName = sb + fileName +"-{0}.csv ";

            using (var sourceFile = new StreamReader(sourceFileName))
            {

                var fileCounter = 0;
                var destinationFile = new StreamWriter(
                    string.Format(destinationFileName, fileCounter + 1));

                try
                {
                    var lineCounter = 0;

                    string line;
                    string headerline=string.Empty;
                    progressBar1.Minimum = fileCounter;
                    progressBar1.Maximum = linesPerFile;
                    while ((line = sourceFile.ReadLine()) != null)
                    {
                        if (lineCounter == 0)
                        {
                            headerline = line;
                        }
                        // Did we reach the maximum number of lines for the file?
                        if (lineCounter >= linesPerFile)
                        {
                            
                            // Yep... Time to close this one and 
                            // switch to another file
                            lineCounter = 0;
                            fileCounter++;

                            destinationFile.Dispose();
                            destinationFile = new StreamWriter(
                                string.Format(destinationFileName, fileCounter + 1));
                            if (!string.IsNullOrEmpty(headerline) && chkHeader.Checked)
                            {
                                destinationFile.WriteLine(headerline);
                            }
                            
                        }
                        progressBar1.Value = fileCounter;    
                        destinationFile.WriteLine(line);
                        lineCounter++;
                    }

                    progressBar1.Value = progressBar1.Maximum;
                    MessageBox.Show("Split Completed");
                    progressBar1.Visible = false;
                    txtNumberOfLineCount.Text = string.Empty;
                    txtSource.Text = string.Empty;
                    chkHeader.Checked = false;

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    destinationFile.Dispose();
                }
            }

        }
    }
}
