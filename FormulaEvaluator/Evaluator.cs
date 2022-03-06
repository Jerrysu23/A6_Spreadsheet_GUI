using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{   /// <summary>
    /// Author:Wenlin Li
    /// uid: u1327012
    /// 
    /// This class it to write a method that evaluates arithmetic expressions using standard infix notation.
    /// 
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(String v);
        /// <summary>
        /// This method is to split the math problem into number, variable and operator. And the process should respect the usual precedence rules.
        /// If the math problem does not meet the law of arithmetic it would throw exception by any chance.
        /// </summary>
        /// <param name="exp">The input mathematical expression.</param>
        /// <param name="variableEvaluator">The input delegate method use to find the number by variable.</param>
        /// <returns></returns> Return the fianl answer calculated by following code
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
             exp = exp.Replace(" ", "");
            ///Create two new stacks 
            Stack<int> value = new Stack<int>();
            Stack<string>  theOperator = new Stack<string>();
            /// Create a int variable to store the number from the Lookup method.
            int n = 0;
            ///Need a string splitter
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            foreach (String s in substrings)
            {

                if (Int32.TryParse(s, out n))
                {
                    value.Push(n);
                    CheckOnTop(s, true, value, theOperator);
                }

                ///If it is a variable, proceed as above.
                else if (s.Length >= 2 && !isNumberic(s))
                {
                    char[] array = s.ToCharArray();
                    ///If it is not a variable, then throw a exception.
                    if (!(Char.IsLetter(array[0]) == true && Char.IsNumber(array[array.Length - 1]) == true))
                    {
                        throw new ArgumentException("It is not a variable");
                    }
                    else
                    {   /// Look up the number by putting the variable into the delegate method.
                        int variable = variableEvaluator(s);
                        value.Push(variable);
                        CheckOnTop(s, true, value, theOperator);
                    }
                }
                /// If the operator is "-" or "+", check the top of operator stack and process them.
                else if (s.Equals("-") || s.Equals("+"))
                {
                    CheckOnTop(s, false, value, theOperator);
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
                        throw new ArgumentException("Error arithmetic problem.");
                    }
                    if (theOperator.Peek() == "+")
                    {
                        Add(value, theOperator);
                        ///Make sure there is at least one operator or the next operand should be a '('.
                        if (theOperator.Count == 0 || theOperator.Pop() != "(")
                        {
                            throw new ArgumentException("Error arithmetic problem");
                        }
                    }
                    else if (theOperator.Peek() == "-")
                    {
                        Subtract(value, theOperator);
                        ///Make sure there is at least one operator or the next operand should be a '('.
                        if (theOperator.Count == 0 || theOperator.Pop() != "(")
                        {
                            throw new ArgumentException("Error arithmetic problem");
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
                }
                else if (theOperator.Peek() == "*")
                {
                    Multiply(value, theOperator);
                }
                ///If the operator still have "(", it should throw a exception.
                else if (theOperator.Peek() == "(")
                {
                    throw new ArgumentException("Error arithmetic problem");
                }
            }
            ///If the operator stack is empty and the value stack still have two elements, it should throw a exception. 
            ///It means it is not vaild math problem.
            if (theOperator.Count == 0 && value.Count >= 2)
            {
                throw new ArgumentException("There is no operator for two and more numbers to operate");
            }
            
            if (theOperator.Count == 0 && value.Count == 0)
            {
                throw new ArgumentException("There should only be one value left but there are " + value.Count + " left.");
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
        private static void CheckOnTop(String s, bool integer , Stack<int> value, Stack<String> theOperator)
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
                    throw new ArgumentException("There is not value in stack");
                }
            }
        }

        /// <summary>
        /// A static helper method for processing the add operation. Add two numbers and push the result onto the value stack.
        /// </summary>
        private static void Add(Stack<int> value, Stack<String> theOperator)
        {
            if (value.Count < 2)
            {
                throw new ArgumentException("Need more than two numbers to operate");
            }
            /// Pop the integer 
            int number1 = value.Pop();
            int number2 = value.Pop();
            /// Get the division sign "+"
            theOperator.Pop();
            /// Add them together
            int result = number1 + number2;
            /// Push the sum onto value stack
            value.Push(result);
        }

        /// <summary>
        /// A static helper method for processing the multiplication operation. Multiply two numbers and push the result onto the value stack.
        /// </summary>
        private static void Multiply(Stack<int> value, Stack<String> theOperator)
        {
            if (value.Count() < 2)
            {
                throw new ArgumentException("Only one or none number to operate");
            }
            /// Pop the integer 
            int number1 = value.Pop();
            int number2 = value.Pop();
            /// Get the division sign "*"
            theOperator.Pop();
            /// Multiply them together
            int result = number1 * number2;
            value.Push(result);

        }

        /// <summary>
        /// A static helper method for processing the subtraction operation. Subtract two numbers and push the result onto the value stack.
        /// </summary>
        private static void Subtract(Stack<int> value, Stack<String> theOperator)
        {
            if (value.Count < 2)
            {
                throw new ArgumentException("Only one or none number to operate");
            }
            /// Pop the integer 
            int number1 = value.Pop();
            int number2 = value.Pop();
            /// Get the division sign "-"
            theOperator.Pop();
            /// Multiply them together
            int result = number2 - number1;
            value.Push(result);
        }

        /// <summary>
        /// A static helper method for processing the division operation. Divide two numbers and push the result onto the value stack. 
        /// </summary>
        private static void Divide(Stack<int> value, Stack<String> theOperator)
        {
            if (value.Count < 2)
            {
                throw new ArgumentException("Need more than two numbers to operate");
            }
            /// Pop the integer 
            int number1 = value.Pop();
            int number2 = value.Pop();
            /// Get the division sign "/"
            if (theOperator.Count == 0)
            {
                throw new ArgumentException("The stack is empty");
            }
            theOperator.Pop();
            if (number1 == 0)
            {
                throw new ArgumentException("The denominator cannot be 0");
            }
            /// Multiply them together
            int result = number2 / number1;
            value.Push(result);
        }

        /// <summary>
        /// A static helper method to determine if it is a variable.
        /// </summary>
        /// <param name="oText"></param>
        /// <returns></returns> Return true if the input is a number.
        private static bool isNumberic(string oText)
        {
            try
            {
                int var1 = Convert.ToInt32(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
