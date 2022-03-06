using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// This spreadsheetGUI class is to implement a GUI front end for the Spreadsheet class that you have been developing all semester. 
    /// build an effective GUI for user interaction with your spreadsheet. This includes: entering data (i.e., strings, numbers, and formulas) into 
    /// the spreadsheet and updating/displaying the results in a clear manner, and saving/loading files.
    /// 
    /// @Authors Wenlin Li & Robert Li
    /// @Team: LplChampion
    /// Version 1.0-Implemented all method
    /// Version 1.1-Add extra feature
    /// Version 1.2-Found some problems and fixed them
    /// </summary>


    ///The delegate holding value
    public delegate void BoxesChanged(string cellName);

    public partial class Form1 : Form
    {
        /// <summary>
        /// Controller object for holding the delegate
        /// </summary>
        private Controller spreadsheetController;

        /// <summary>
        /// Dictinary object to hold coordinate
        /// </summary>
        private Dictionary<string, int[]> cellsDictionary;

        /// <summary>
        /// The bool object holding our feature value besides the spreadsheet changes.
        /// </summary>
        private bool Changed;

        /// <summary>
        /// The default constructor 
        /// </summary>
        public Form1()
        {
            spreadsheetController = new Controller();
            cellsDictionary = new Dictionary<string, int[]>();
            Changed = false;

            InitializeComponent();
        }

        /// <summary>
        /// The Form Loading process, we initialed the value and basic events as the form open. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //Holding basic selecting box events, and update the box make sure every boxes and their contents match
            spreadsheetPanel.SelectionChanged += spreadsheetController.BoxesSelected;
            spreadsheetController.panels += GetBoxElements;

            //holding input botton events
            inputBotton.Click += SetBoxElements;
            
            //Holding Enter pressed 
            Button enter = new Button();
            AcceptButton = enter;
            enter.Click += SetBoxElements;

            //holding the closing event
            this.FormClosing += ClosingWindow;

            //starts from a1
            GetBoxElements("A1");
            spreadsheetPanel.Focus();

        }

        /// <summary>
        /// This method get the content and update value from matched namebox
        /// </summary>
        /// <param name="s">name of the cell we are cuurently on.</param>
        private void GetBoxElements(string cellName)
        {
            
            nameBox.Text = cellName;
            //make sure those boxes are cleared before putting new one
            valueBox.Clear();
            contentsBox.Clear();
            //setting up
            valueBox.Text = spreadsheetController.GetValue(cellName).ToString();
            contentsBox.Text = spreadsheetController.GetContents(cellName).ToString();

        }

        /// <summary>
        /// This method is setting up the value to spreadsheet from content box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetBoxElements(object sender, EventArgs e)
        {
            try
            {
                spreadsheetController.SetContents(contentsBox.Text);
                spreadsheetPanel.GetSelection(out int col, out int row);

                // If no cell is found, add a new one from dictionary
                if (!cellsDictionary.ContainsKey(nameBox.Text))
                    cellsDictionary.Add(nameBox.Text, new int[] { col, row });
                // setting up the cells
                if (spreadsheetController.GetValue(nameBox.Text) is FormulaError == false)
                {
                    valueBox.Text = spreadsheetController.GetValue(nameBox.Text).ToString();
                    spreadsheetPanel.SetValue(col, row, valueBox.Text);

                }
                // setting up formula error
                else
                {       
                    spreadsheetPanel.SetValue(col, row, "#VALUE!").ToString();
                }
                //refocus on the spreadsheet after we done setting
                spreadsheetPanel.Focus();
            }

            // Exception catches
            catch (FormulaFormatException)
            {
                MessageBox.Show("It Causes Invalid Formula Because of Cell: " + nameBox.Text);
            }
            // Exception catches
            catch (CircularException)
            {
                MessageBox.Show("It Causes Circular Exception Because of Cell: " + nameBox.Text);
            }
        }

        /// <summary>
        /// This is a method able 
        /// 1. space key to forcus on contentsBox
        /// 2. F1 key to make picture visible
        /// 3. F2 key to make picture not visible
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {

            if (keyData == Keys.Space)
            {
                contentsBox.Focus();
            }
            if (keyData == Keys.F1)
            {
                pictureBox1.BringToFront();
            }
            if (keyData == Keys.F2)
            {
                pictureBox1.SendToBack();
            }
   
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Create a brand new spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            SpreadsheetFormContext.GetFormContext().RunForm(new Form1());
        }


        /// <summary>
        /// Save the spreadsheet file to computer
        /// referrence: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.filedialog.filterindex?view=windowsdesktop-5.0
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "All files (*.*)|*.*|Spreadsheet Files| *.sprd";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                    spreadsheetController.SaveSpreadsheet(saveFileDialog.FileName);
            }
        }


        /// <summary>
        /// open a spreadsheet from current directory.
        /// referrence: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.filedialog.filterindex?view=windowsdesktop-5.0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*|Spreadsheet Files| *.sprd";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            // making sure everything element is correct when writing new file.
            try
            {
                writeFile(filePath);
            }

            catch (SpreadsheetReadWriteException excption)
            {
                MessageBox.Show("It Causes An Error: " + excption);
            }

            // when open the new file, reset to defult.
            reset();
        }

        /// <summary>
        /// Resetting to our default cell
        /// </summary>
        private void reset()
        {
            nameBox.Text = "A1";
            valueBox.Text = spreadsheetController.GetValue("A1").ToString();
            contentsBox.Text = spreadsheetController.GetContents("A1").ToString();
            spreadsheetPanel.SetSelection(0, 0);
        }

        /// <summary>
        /// This method write the file
        /// </summary>
        /// <param name="filePath"></param>
        private void writeFile(object filePath)
        {
            // get the file path and  spreadsheet.
            Spreadsheet s = spreadsheetController.LoadSpreadsheet(filePath.ToString());

            spreadsheetPanel.Clear();
            cellsDictionary.Clear();

            // Get new cellDictionary and setting new cell up.
            foreach (string cell in s.GetNamesOfAllNonemptyCells())
            {
                WriteCell(out int col, out int row, cell);
                cellsDictionary.Add(cell, new int[] { col, row });
                String value = spreadsheetController.GetValue(cell).ToString();
                spreadsheetPanel.SetValue(cellsDictionary[cell][0], cellsDictionary[cell][1], value);
            }
        }

        /// <summary>
        ///  This method wrire new cells for new spreadsheet
        /// </summary>
        /// <param name="col">collumn of the cell</param>
        /// <param name="row">row of the cell</param>
        /// <param name="name">name of the cell</param>
        private void WriteCell(out int col, out int row, string name)
        {
            col = name[0] - 'A';
            row = int.Parse(name.Substring(1, name.Length - 1)) - 1;
        }


        /// <summary>
        /// Closes the current spreadsheet, and we have the safety feature on it, to make sure user
        /// save the content if they want to before closed.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (spreadsheetController.SpreadsheetHasChanged() || Changed == true)
            {
                string message = "Do you want to save unsaved changes?";
                DialogResult res = MessageBox.Show(message, "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (res == DialogResult.Yes)
                {
                    saveToolStripMenuItem1_Click(sender, e);
                } 
                
                else
                {
                    Close();
                }
                    
            }

            Close();
        }


        /// <summary>
        /// Helper method for closing window using standard windows close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosingWindow(object sender, FormClosingEventArgs e)
        {
            if (spreadsheetController.SpreadsheetHasChanged() || Changed == true)
            {
                string message = "Do you want to save unsaved changes?";
                DialogResult res = MessageBox.Show(message, "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (res== DialogResult.Yes)
                {
                    saveToolStripMenuItem1_Click(sender, e);
                }

                else
                    e.Cancel = false;
            }
        }


        /// <summary>
        /// Help menum get README file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Directory.GetCurrentDirectory();
            string target = @"..\..\..\README.txt";
           
            if (!Directory.Exists(target))
            {
                File.ReadAllText(Path.GetFullPath(@"..\..\..\temp.txt"));
            }

            Form form = new Form();
            form.Size = new Size(500, 500);

            RichTextBox textbox = new RichTextBox();
            textbox.Size = new Size(100000, 100000);

            textbox.Text = File.ReadAllText(Path.GetFullPath(target));

            form.Controls.Add(textbox);
            form.ShowDialog();
        }

        /// <summary>
        /// Extra feature for picture box. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Load(openFileDialog.FileName);
                    pictureBox1.BringToFront();
                    Changed = true;
                }
            }
        }

        /// <summary>
        /// picturebox feature, for deleting the pics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox1.SendToBack();
        }

        /// <summary>
        /// click on picture box and can delete it with esc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            Button esc = new Button();
            CancelButton = esc;
            esc.Click += imageToolStripMenuItem1_Click;
        }


        /// <summary>
        /// The controller class to hold the panel's inputs and events
        /// 
        /// </summary>
        public class Controller
        {
            /// <summary>
            /// private spreadsheet object
            /// </summary>
            private Spreadsheet s;

            /// <summary>
            /// private int object to hold col and row values.
            /// </summary>
            private int col, row;

            /// <summary>
            /// update the panel in the spreadsheets
            /// </summary>
            public event BoxesChanged panels;

            /// <summary>
            /// The constructor of controller
            /// </summary>
            public Controller()
            {
                s = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[1-9]{1}[0-9]?$"), s => s.ToUpper(), "ps6");
            }

            /// <summary>
            /// get the selected boxes
            /// </summary>
            /// <param name="spreadsheetPanel"></param>
            public void BoxesSelected(SpreadsheetPanel spreadsheetPanel)
            {
                spreadsheetPanel.GetSelection(out int col, out int row);
                panels(GetName(col, row));
                this.col = col;
                this.row = row;
            }

            /// <summary>
            /// get the cell's value
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public object GetValue(string name)
            {
                return s.GetCellValue(name);
            }

            /// <summary>
            /// get the content's elements
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public object GetContents(string name)
            {
                return s.GetCellContents(name);
            }


            /// <summary>
            /// Thhe setter for contents
            /// </summary>
            /// <param name="contents"></param>
            /// <returns></returns>
            public IEnumerable<string> SetContents(string contents)
            {
                return s.SetContentsOfCell(GetName(col, row), contents);
            }

            /// <summary>
            /// Saving the file
            /// </summary>
            /// <param name="fileName">File to save</param>
            public void SaveSpreadsheet(string fileName)
            {
                s.Save(fileName);
            }


            /// <summary>
            /// This method load the new spreadsheet
            /// </summary>
            /// <param name="filePath">Path to the file</param>
            public Spreadsheet LoadSpreadsheet(string filePath)
            {
                return s = new Spreadsheet(filePath, s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[1-9]{1}[0-9]?$"), s => s.ToUpper(), "ps6");
            }

            /// <summary>
            /// determine if spreadsheet has changed
            /// </summary>
            /// <returns>true if changed, false if not</returns>
            public bool SpreadsheetHasChanged()
            {
                return s.Changed;
            }

            

            /// <summary>
            ///  This method get the name of the cell
            /// </summary>
            /// <param name="col">Column of current cell</param>
            /// <param name="row">Row of current cell</param>
            /// <returns>cell name</returns>
            public string GetName(int col, int row)
            {
                if (col > 26 || row > 99)
                {
                    throw new ArgumentOutOfRangeException("");
                }

                return ((char)(col + 'A')).ToString() + (row + 1);
            }


        }

    }
}