using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetGUI;
using SS;

namespace SS
{
    /// <summary>
    /// Keeps track of how many top-level forms are running
    /// @Team: LplChampion
    /// Author: Wenlin Li && Robert Li
    /// </summary>
    class SpreadsheetFormContext : ApplicationContext
    {
        /// <summary>
        /// Number of open forms
        /// </summary>
        private int formCount = 0;


        /// <summary>
        /// Singleton ApplicationContext
        /// </summary>
        private static SpreadsheetFormContext FormContext;


        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadsheetFormContext()
        {
        }

        /// <summary>
        /// Returns the one SpreadsheetFormContext.
        /// </summary>
        public static SpreadsheetFormContext GetFormContext()
        {
            if (FormContext == null)
            {
                FormContext = new SpreadsheetFormContext();
            }
            return FormContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            formCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }


        
    }


    /// <summary>
    /// Main Program.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            SpreadsheetFormContext appContext = SpreadsheetFormContext.GetFormContext();
            appContext.RunForm(new Form1());
            Application.Run(appContext);
        }
    }
}