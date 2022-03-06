// Written by Wenlin Li for CS 3500, October 2021
// Version 1.1
// Revision history:
//   Version 1.0                     PS4 Spreadsheet.
// Branched from PS4 Spreadsheet
//   Version 1.1                    PS5 Spreadsheet.
//           Fill in the methods in Spreadsheet by reference AbstractSpreadsheet.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using SpreadsheetUtilities;

namespace SS

/// <summary>
/// This Class represents the ps5.
/// There are a number of additions and modifications to AbstractSpreadsheet.cs.
/// This ps5 is based on the operation rules of dependent connections and formulas.    
/// @Author Wenlin Li u1327012 
/// 10/3/21
///  
///
/// </summary>
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Represents all of the cell's dependencies 
        /// </summary>
        private DependencyGraph myGraph;

        /// <summary>
        /// Represents all cell which contain values.
        /// </summary>
        private Dictionary<string, Cell> cells;

        /// <summary>
        /// True if this spreadsheet has been modified,                  
        /// false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {

            myGraph = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        ///It should be added a three-argument constructor to the Spreadsheet class. 
        ///Just like the zero-argument constructor, it should create an empty spreadsheet. 
        ///However, it should allow the user to provide a validity delegate (first parameter), 
        ///a normalization delegate (second parameter), and a version (third parameter).
        /// 
        /// Validity delegate:  determine if the variable is valid.
        /// Normalize delegate: normalize variable names.
        /// Version:            The spreadsheet's version.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            myGraph = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// It should be added a four-argument constructor to the Spreadsheet class. 
        /// It should allow the user to provide a string representing a path to a file (first parameter), 
        /// a validity delegate (second parameter), a normalization delegate (third parameter), and a version (fourth parameter). 
        /// It should read a saved spreadsheet from the file (see the Save method) and use it to construct a new spreadsheet.
        /// The new spreadsheet should use the provided validity delegate, normalization delegate, and version. 
        /// Do not try to implement loading from file until after we have discussed XML in class. 
        /// See the Examples repository for an example of reading and writing XML files.
        /// 
        /// filename:           String representing filename that need to input.
        /// Validity delegate:  determine if the variable is valid.
        /// Normalize delegate: normalize variable names.
        /// Version:           The spreadsheet's version.
        /// 
        ///
        /// </summary>
        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {

            myGraph = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
            Version = version;
            try
            {
                string cellName = "";
                // Create an XmlReader 
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":

                                    if (reader["version"] != version)
                                        throw new SpreadsheetReadWriteException("Wrong version");
                                    break; // no more direct info to read for Spreadsheet

                                case "cell":
                                    break; // no more direct info to read on Cell

                                case "name":
                                    reader.Read();
                                    cellName = reader.Value; // Grab cell name
                                    break;

                                case "contents":
                                    reader.Read(); // reader value is now cell contents.
                                    SetContentsOfCell(cellName, reader.Value);
                                    try
                                    {
                                        SetContentsOfCell(cellName, reader.Value);
                                    }
                                    catch (CircularException)
                                    {
                                        throw new SpreadsheetReadWriteException("A circular exception was thrown");
                                    }
                                    catch (InvalidNameException)
                                    {
                                        throw new SpreadsheetReadWriteException("Invaild name");
                                    }
                                    break;
                            }
                        }
                    }
                }
                Changed = false;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error loading");
            }

        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (name is null)
            {

                throw new InvalidNameException();
            }
            // Normalize the name
            name = Normalize(name);

            if (cells.ContainsKey(name))
            {
                return cells[name].value;
            }

            return "";
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException();
            try
            {
                // Create an XmlReader inside this block, and automatically Dispose() it at the end.
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    // This is an example of reading an attribute on an element
                                    return reader.GetAttribute(0);
                            }
                        }
                    }
                }
            }
            catch
            {   // if input the file name is not correct or not exist then throw exception.
                throw new SpreadsheetReadWriteException("unable to find the file");
            }
            throw new SpreadsheetReadWriteException("unable to find the attribute of this file");
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                // We want some non-default settings for our XML writer.
                // Specifically, use indentation to make it more readable.
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";

                // Create an XmlWriter inside this block, and automatically Dispose() it at the end.
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {

                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    // This adds an attribute to the element
                    writer.WriteAttributeString("version", this.Version);
                    // write the cells themselves
                    foreach (string cell in cells.Keys)
                    {
                        // Convert cell contents to string
                        Cell wrtittenCell = cells[cell];
                        string content = "";
                        if (wrtittenCell.contents is string)
                        {
                            content = (string)wrtittenCell.contents;
                        }
                        if (wrtittenCell.contents is double)
                        {
                            content = wrtittenCell.contents.ToString();
                        }
                        if (wrtittenCell.contents is Formula)
                        {
                            content = "=" + wrtittenCell.contents.ToString();
                        }

                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell);
                        writer.WriteElementString("contents", content);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndDocument();  // Ends the block
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Invaild File");
            }
            Changed = false;
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            // Normalize the cell
            name = Normalize(name);
            if (content is null)
            {
                throw new ArgumentNullException();
            }
            //Check the name
            if (!IsValid(name) && Cell.IsValidName(name))
            {
                throw new InvalidNameException();
            }

            IList<string> returnDependencies = new List<string>();
            // If content parses as double then set the content to double.
            if (Double.TryParse(content, out double contentDouble))
            {
                returnDependencies = SetCellContents(name, contentDouble);
            }

            else if (content.Length != 0)
            {
                //if content begins with the character '=', an attempt is made
                /// to parse the remainder of content into a Formula f using the Formula
                /// constructor. 
                if (content.StartsWith("="))
                {

                    try
                    {
                        string remainder = content.Substring(1, content.Length - 1);
                        // Normalize the formula
                        remainder = Normalize(remainder);
                        Formula f = new Formula(remainder, Normalize, IsValid);
                        returnDependencies = SetCellContents(name, f);
                    }
                    catch (FormulaFormatException e)
                    {
                        Cell c = new Cell("");
                        if (!cells.TryGetValue(name, out Cell value))
                            returnDependencies = SetContentsOfCell(name, c.contents.ToString());
                        throw e;
                    }
                }
                //If the cell contents is a string
                else
                    returnDependencies = SetCellContents(name, content);
            }
            //If the cell contents is an empty string
            else
                returnDependencies = SetCellContents(name, content);

            //re-evaluated the cells 
            foreach (string cell in GetCellsToRecalculate(name))
            {
                if (cells.ContainsKey(cell))
                    cells[cell].Recalculate(LookupValue);
            }

            // Mark as changed
            Changed = true;

            return returnDependencies;

        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return myGraph.GetDependents(name);
        }


        /// <summary>
        /// The contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            // determine if cell contains the name
            if (!cells.ContainsKey(name))
            {
                // If contain then add.
                cells.Add(name, new Cell(number));

            }
            //If not, replace all the value.
            else
            {
                myGraph.ReplaceDependees(name, new List<string>());
                cells[name] = new Cell(number);

            }

            return new List<String>(GetCellsToRecalculate(name).ToList());

        }

        // MODIFIED PROTECTION FOR PS5
        /// <summary>
        /// The contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            // determine if cell contains the name
            if (!cells.ContainsKey(name))
            {
                // If contain then add.
                cells.Add(name, new Cell(text));

            }
            //If not, replace all the value.
            else
            {
                myGraph.ReplaceDependees(name, new List<string>());
                cells[name] = new Cell(text);
            }


            return new List<String>(GetCellsToRecalculate(name).ToList());
        }

        /// <summary>
        /// If changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula. The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // Check if every variable is valid. 
            foreach (string var in formula.GetVariables())
            {

                if (!IsValid(var))
                    throw new FormulaFormatException("Invalid variable");

            }

            string oldValue = "";
            // store the old value in case throw a exception  
            if (cells.TryGetValue(name, out Cell c))
                oldValue = c.contents.ToString();


            try
            {
                // determine if cell contains the name
                List<string> returnList = new List<string>();
                myGraph.ReplaceDependees(name, formula.GetVariables());
                IEnumerable<String> dependees = myGraph.GetDependees(name);
                if (!cells.ContainsKey(name))
                {
                    cells.Add(name, new Cell(formula, LookupValue));

                }
                //If not, then make a new formula.
                else
                {
                    cells[name] = new Cell(formula, LookupValue);
                }

                // get the dependents of the name, add to the list
                foreach (string dp in dependees)
                {
                    returnList.Add(dp);
                }

                // return the list
                return returnList;
            }
            // circular dependency will throws a exception
            catch (CircularException)
            {
                SetContentsOfCell(name, oldValue);
                throw new CircularException();

            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            name = Normalize(name);
            //If name is null or invalid, throws an InvalidNameException.
            if (name is null || !Cell.IsValidName(name))
            {
                throw new InvalidNameException();
            }

            //If name is in the cells then return the cell content.
            if (cells.TryGetValue(name, out Cell returnCell))
            {
                return returnCell.contents;
            }

            return "";

        }
        /// <summary>
        /// Helper method for delegate function of loopup .
        /// 
        /// </summary>
        private double LookupValue(string name)
        {
            name = Normalize(name);
            if (double.TryParse(GetCellValue(name).ToString(), out double result))
                return result;
            else
                throw new ArgumentException();
        }
        /// <summary>
        /// Private class to represents how the cell is structured
        /// </summary>
        private class Cell
        {
            /// <summary> 
            /// contents of the cell 
            /// </summary>
            public Object contents
            {
                get;
                set;
            }
            /// <summary> 
            /// value of cell 
            /// </summary>
            public object value
            {
                get;
                private set;
            }


            /// <summary>
            /// Constructor that has a string input
            /// </summary>
            /// <param cellString="cellString">a string text</param>
            public Cell(String cellString)
            {
                contents = cellString;
                value = cellString;
            }

            /// <summary>
            /// Constructor that has a double input
            /// </summary>
            /// <param cellDouble="cellDouble">a floating point number</param>
            public Cell(double cellDouble)
            {
                contents = cellDouble;
                value = cellDouble;
            }

            /// <summary>
            /// Constructor that has a formula input
            /// </summary>
            /// <param name="cellFormula"></param>
            /// <param name="lookup"></param>
            public Cell(Formula cellFormula, Func<string, double> lookup)
            {
                contents = cellFormula;
                value = cellFormula.Evaluate(lookup);
            }
            /// <summary>
            /// The cell need to recalculate and upadta it.
            /// </summary>
            /// <param name="lookup"></param>
            public void Recalculate(Func<string, double> lookup)
            {
                if (contents is Formula formula)
                    value = formula.Evaluate(lookup);
            }
            /// <summary>
            /// Helper method to determine if it is a vaild name
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static bool IsValidName(String name)
            {
                if (name.Length == 0)
                    throw new InvalidNameException();

                return Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline);
            }
        }



    }
}
