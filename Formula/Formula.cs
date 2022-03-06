// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private String formulaStr;
        HashSet<string> variableSet;
        Boolean error;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {

        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// Author -Wenlin Li
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            StringBuilder formulaStrBuilder = new StringBuilder();
            variableSet = new HashSet<string>();
            int leftParen = 0;
            int rightParen = 0;
            string[] tokens = new string[GetTokens(formula).Count()];
            var tEnum = GetTokens(formula).GetEnumerator();

            for (int i = 0; i < GetTokens(formula).Count(); i++)
            {
                tEnum.MoveNext();
                tokens[i] = normalize(tEnum.Current);
            }

            for (int i = 0; i < GetTokens(formula).Count(); i++)
            {
                if (i == 0)
                {
                    if (tokens[i].Equals("("))
                    {
                        leftParen++;
                        continue;
                    }
                    if (Double.TryParse(tokens[i], out double num1))
                    {
                        continue;
                    }
                    if (variableChecker(tokens[i]))
                    {
                        continue;
                    }
                    else
                    {
                        throw new FormulaFormatException("The first token of an expression must be a number, a variable, or an opening parenthesis.");
                    }

                }

                else if (i == tokens.Count() - 1)
                {
                    if (Double.TryParse(tokens[i], out double last) || variableChecker(tokens[i]) || tokens[i].Equals(")"))
                    {
                        if (tokens[i].Equals(")"))
                        {
                            rightParen++;
                        }
                        continue;
                    }
                    else
                    {
                        throw new FormulaFormatException("The last token of an expression must be a number, a variable, or a closing parenthesis.");
                    }
                }

                else if (Double.TryParse(tokens[i], out double num3))
                {
                    if (i != tokens.Count() && !(oChecker(tokens[i + 1])))
                    {
                        throw new FormulaFormatException("The token following by must be an operator or closing parenthesis.");
                    }
                }

                else if (variableChecker(tokens[i]))
                {
                    string newNormalize = normalize(tokens[i]);
                    if (!variableChecker(newNormalize))
                    {
                        throw new FormulaFormatException("It is not a variable");
                    }
                    if (!isValid(newNormalize))
                    {
                        throw new FormulaFormatException("It is not a variable");
                    }

                    // - Any token that immediately follows a number, a variable, or a closing parenthesis must be either 
                    //   an operator or a closing parenthesis.
                    if (i > tokens.Count() && !oChecker(tokens[i + 1]) && tokens.Length > 1)
                    {
                        throw new FormulaFormatException("The token following by must be an operator or closing parenthesis.");
                    }

                }
                else if (tokens[i].Equals("+") || tokens[i].Equals("-") || tokens[i].Equals("*") || tokens[i].Equals("/"))
                {
                    // - Any token that immediately follows an opening parenthesis or an operator must be either a number, 
                    //   a variable, or an opening parenthesis.
                    if (i != tokens.Count())
                    {
                        if (tokens[i + 1].Equals("(") || Double.TryParse(tokens[i + 1], out double num2) || variableChecker(tokens[i + 1]))
                            continue;
                        else
                        {
                            throw new FormulaFormatException("The token following by must be a number, a variable, or an opening parenthesis");
                        }
                    }
                }
                else if (tokens[i].Equals("("))
                {
                    leftParen++;
                    if (i != tokens.Count() && tokens.Length > 1)
                    {
                        if (tokens[i + 1].Equals("("))
                        {
                            continue;
                        }
                        if (Double.TryParse(tokens[i + 1], out double num2))
                        {
                            continue;
                        }
                        if (variableChecker(tokens[i + 1]))
                        {
                            continue;
                        }
                        throw new FormulaFormatException("The token following by must be a number, a variable, or an opening parenthesis");
                    }
                }

                else if (tokens[i].Equals(")"))
                {
                    rightParen++;
                    if (i != tokens.Count() && tokens.Length > 1)
                    {
                        if (!oChecker(tokens[i + 1]))
                            throw new FormulaFormatException("The token following by must be an operator or closing parenthesis.");
                    }
                }
            }

            if (leftParen > rightParen)
            {
                throw new FormulaFormatException("It is illegal that the closing parentheses is more than the opening parentheses .");
            }

            if (leftParen != rightParen)
            {
                throw new FormulaFormatException("The total number of opening parentheses must equal the total number of closing parentheses.");
            }

            if (GetTokens(formula).Count() < 1)
            {
                throw new FormulaFormatException("There must be at least one token");
            }

            for (int index = 0; index < tokens.Length; index++)
            {
                formulaStrBuilder.Append(tokens[index]);
                if (variableChecker(tokens[index]))
                {
                    variableSet.Add(tokens[index]);
                }
            }
            formulaStr = formulaStrBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token">
        /// 
        /// </param>
        /// <returns>
        /// boolean
        /// </returns>
        private bool variableChecker(String token)
        {
            if (token[0].Equals("_") || Char.IsLetter(token[0]))
            {
                char[] tokenArr = token.ToCharArray();
                if (tokenArr.Length == 1)
                {
                    return true;
                }
                foreach (char t in tokenArr)
                {
                    if (Char.IsLetter(t) || Char.IsNumber(t) || t.Equals("_"))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool oChecker(String token)
        {
            if (token.Equals("+") || token.Equals("-") || token.Equals("*") || token.Equals("/") || token.Equals(")"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            ///Create two new stacks 
            Stack<Double> value = new Stack<Double>();
            Stack<String> theOperator = new Stack<String>();
            /// Create a int variable to store the number from the Lookup method.
            double n = 0;
            var token = GetTokens(formulaStr).GetEnumerator();
            string[] substrings = new string[GetTokens(formulaStr).Count()];

            for (int i = 0; i < substrings.Length; i++)
            {
                token.MoveNext();
                substrings[i] = token.Current;

            }

            foreach (String s in substrings)
            {

                if (Double.TryParse(s, out n))
                {
                    value.Push(n);
                    CheckOnTop(s, true, value, theOperator);
                    if (error == true)
                    {
                        return new FormulaError("Can't divide by zero");
                    }
                }

                else if (s == " " || s == "")
                {
                    continue;
                }

                ///If it is a variable, proceed as above.
                else if (s.Length >= 2 && !isNumberic(s))
                {
                    char[] array = s.ToCharArray();
                    ///If it is not a variable, then throw a exception.
                    if (!(Char.IsLetter(array[0]) == true && Char.IsNumber(array[array.Length - 1]) == true))
                    {
                        return new FormulaError("Does not exist.");
                    }
                    else
                    {
                        /// Look up the number by putting the variable into the delegate method.
                        try
                        {
                            double variable = lookup(s);
                            value.Push(variable);
                            CheckOnTop(s, true, value, theOperator);
                            if (error == true)
                            {
                                return new FormulaError("Can't divide by zero");
                            }
                        }
                        catch (Exception)
                        {
                            return new FormulaError("Lookup of variable does not exist.");
                        }
                    }
                }
                /// If the operator is "-" or "+", check the top of operator stack and process them.
                else if (s.Equals("-") || s.Equals("+"))
                {
                    CheckOnTop(s, false, value, theOperator);
                    if (error == true)
                    {
                        return new FormulaError("Can't divide by zero");
                    }
                }
                /// If the operator is "*" or "/", push the arithnetic symbol onto top of the stack.
                else if (s.Equals("*") || s.Equals("/"))
                {
                    theOperator.Push(s);
                }
                /// If the operator is "(", push the arithnetic symbol onto top of the stack.
                else if (s.Equals("("))
                {
                    theOperator.Push(s);
                }
                /// If the operator is ")", process them if the operator have elements.
                else if (s.Equals(")"))
                {
                    ///If the stack is empty, throw the exception.
                    if (theOperator.Count == 0)
                    {
                        /*return new FormulaError("Can't divide by zero");*/
                    }
                    if (theOperator.Peek() == "+")
                    {
                        Add(value, theOperator);
                        ///Make sure there is at least one operator or the next operand should be a '('.
                        if (theOperator.Count == 0 || theOperator.Pop() != "(")
                        {
                            /* return new FormulaError("Can't divide by zero");*/
                        }
                    }
                    else if (theOperator.Peek() == "-")
                    {
                        Subtract(value, theOperator);
                        ///Make sure there is at least one operator or the next operand should be a '('.
                        if (theOperator.Count == 0 || theOperator.Pop() != "(")
                        {
                            /*return new FormulaError("Can't divide by zero");*/
                        }
                    }
                    else if (theOperator.Peek() == "(")
                    {
                        theOperator.Pop();
                    }
                    /// We need to do the multiply and divide first if the operator stack still have element.
                    if (theOperator.Count > 0)
                    {
                        if (theOperator.Peek() == "/")
                        {
                            Divide(value, theOperator);
                            if (error == true)
                            {
                                return new FormulaError("Can't divide by zero");
                            }
                        }
                        else if (theOperator.Peek() == "*")
                        {
                            Multiply(value, theOperator);
                        }
                    }
                }
            }
            /// Processing the remaning value and operator if the operator stack is not empty.
            while (theOperator.Count != 0)
            {
                if (theOperator.Peek() == "+")
                {
                    Add(value, theOperator);
                }
                else if (theOperator.Peek() == "-")
                {
                    Subtract(value, theOperator);
                }
                else if (theOperator.Peek() == "/")
                {
                    Divide(value, theOperator);
                    if (error == true)
                    {
                        return new FormulaError("Can't divide by zero");
                    }
                }
                else if (theOperator.Peek() == "*")
                {
                    Multiply(value, theOperator);
                }
                ///If the operator still have "(", it should throw a exception.
                else if (theOperator.Peek() == "(")
                {

                }
            }

            /// Return the final answer.
            return value.Pop();
        }

        /// <summary>
        /// A static helper method to check the element on the top of the operator stack. 
        /// And process the calculation depends on what kind of the element it is.
        /// </summary>
        /// <param name="s">The input string that need to determine the operator on the top of the stack.</param>
        /// <param name="integer">The boolean variable to determine if it is a integer.</param>
        private void CheckOnTop(String s, bool integer, Stack<Double> value, Stack<String> theOperator)
        {
            ///If it is a value, it should process the multiplication and divison.
            if (integer == true)
            {
                if (theOperator.Count != 0)
                {
                    if (theOperator.Peek() == "/")
                    {
                        Divide(value, theOperator);
                    }
                    else if (theOperator.Peek() == "*")
                    {
                        Multiply(value, theOperator);
                    }
                }
            }
            /// If not, it process the remaning the value and operator.
            else
            {
                if (theOperator.Count != 0)
                {
                    if (theOperator.Peek() == "+")
                    {
                        Add(value, theOperator);
                    }
                    else if (theOperator.Peek() == "-")
                    {
                        Subtract(value, theOperator);
                    }
                    else if (theOperator.Peek() == "/")
                    {
                        Divide(value, theOperator);

                    }
                    else if (theOperator.Peek() == "*")
                    {
                        Multiply(value, theOperator);
                    }
                    theOperator.Push(s);
                }

                else if (theOperator.Count == 0)
                {
                    theOperator.Push(s);

                }
                else if (value.Count == 0)
                {
                    new FormulaError("Can't divide by zero");
                }
            }
        }

        /// <summary>
        /// A static helper method for processing the add operation. Add two numbers and push the result onto the value stack.
        /// </summary>
        private void Add(Stack<Double> value, Stack<String> theOperator)
        {
            if (value.Count < 2)
            {
                new FormulaError("Can't divide by zero");
            }
            /// Pop the integer 
            double number1 = (double)value.Pop();
            double number2 = (double)value.Pop();
            /// Get the division sign "+"
            theOperator.Pop();
            /// Add them together
            double result = number1 + number2;
            /// Push the sum onto value stack
            value.Push(result);
        }

        /// <summary>
        /// A static helper method for processing the multiplication operation. Multiply two numbers and push the result onto the value stack.
        /// </summary>
        private void Multiply(Stack<Double> value, Stack<String> theOperator)
        {
            if (value.Count() < 2)
            {
                new FormulaError("Can't divide by zero");
            }
            /// Pop the integer 
            double number1 = value.Pop();
            double number2 = value.Pop();
            /// Get the division sign "*"
            theOperator.Pop();
            /// Multiply them together
            double result = number1 * number2;
            value.Push(result);

        }

        /// <summary>
        /// A static helper method for processing the subtraction operation. Subtract two numbers and push the result onto the value stack.
        /// </summary>
        private void Subtract(Stack<Double> value, Stack<String> theOperator)
        {
            if (value.Count < 2)
            {
                new FormulaError("Can't divide by zero");
            }
            /// Pop the integer 
            double number1 = value.Pop();
            double number2 = value.Pop();
            /// Get the division sign "-"
            theOperator.Pop();
            /// Multiply them together
            double result = number2 - number1;
            value.Push(result);
        }

        /// <summary>
        /// A static helper method for processing the division operation. Divide two numbers and push the result onto the value stack. 
        /// </summary>
        private void Divide(Stack<Double> value, Stack<String> theOperator)
        {
            if (value.Count < 2)
            {
                new FormulaError("Can't divide by zero");
            }
            /// Pop the integer 
            double number1 = value.Pop();
            double number2 = value.Pop();
            /// Get the division sign "/"
            if (theOperator.Count == 0)
            {
                new FormulaError("Can't divide by zero");
            }
            theOperator.Pop();
            if (number1 == 0)
            {
                error = true;
            }
            /// Multiply them together
            else
            {
                double result = number2 / number1;
                value.Push(result);
            }
        }

        /// <summary>
        /// A static helper method to determine if it is a variable.
        /// </summary>
        /// <param name="oText"></param>
        /// <returns></returns> Return true if the input is a number.
        private bool isNumberic(string oText)
        {
            try
            {
                double var1 = Convert.ToDouble(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variableSet;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return formulaStr;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            //
            if (obj == null || !(obj is Formula))
            {
                return false;
            }

            // return false if the they are different objects
            if (obj.GetType() != typeof(Formula))
            {
                return false;
            }


            string thisString = this.ToString();
            string formulaString = ((Formula)obj).ToString();

            // 
            string[] thisEnumArr = new string[GetTokens(thisString).Count()];
            var thisEnum = GetTokens(thisString).GetEnumerator();
            for (int i = 0; i < thisEnumArr.Length; i++)
            {
                thisEnum.MoveNext();
                thisEnumArr[i] = thisEnum.Current;
            }

            // 
            string[] formulaEnumArr = new string[GetTokens(formulaString).Count()];
            var formulaEnum = GetTokens(formulaString).GetEnumerator();
            for (int i = 0; i < GetTokens(formulaString).Count(); i++)
            {
                formulaEnum.MoveNext();
                formulaEnumArr[i] = formulaEnum.Current;
            }

            if (formulaEnumArr.Count() != thisEnumArr.Count())
            {
                return false;
            }

            for (int i = 0; i < formulaEnumArr.Count(); i++)
            {

                // 
                if (Double.TryParse(thisEnumArr[i], out double thisNum))
                {
                    if (Double.TryParse(formulaEnumArr[i], out double formulaNum))
                    {
                        if (thisNum.CompareTo(formulaNum) == 0)
                            continue;
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                // 
                else if (variableChecker(thisEnumArr[i]) || variableChecker(formulaEnumArr[i]))
                {
                    if (thisEnumArr[i].Equals(formulaEnumArr[i]))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                // 
                else if (oChecker(thisEnumArr[i]) || thisEnumArr[i].Equals(")"))
                {
                    if (thisEnumArr[i].Equals(formulaEnumArr[i]))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {

            if (f1 is null && f2 is null)
            {
                return true;
            }

            else if (f1 is null && !(f2 is null) || !(f1 is null) && f2 is null)
            {
                return false;
            }

            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null)
            {
                return false;
            }

            else if (f1 is null && !(f2 is null) || !(f1 is null) && f2 is null)
            {
                return true;
            }

            if (!(f1.Equals(f2)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return formulaStr.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}