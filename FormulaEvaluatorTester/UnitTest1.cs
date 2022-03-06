// These tests are for private use only
// Redistributing this file is strictly against SoC policy.

using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PS1GradingTests
{


    /// <summary>
    ///This is a test class for EvaluatorTest and is intended
    ///to contain all EvaluatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EvaluatorTest
    {

        [TestMethod(), Timeout(5000)]
        [TestCategory("1")]
        public void TestSingleNumber()
        {
            Assert.AreEqual(5, Evaluator.Evaluate("5", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("2")]
        public void TestSingleVariable()
        {
            Assert.AreEqual(13, Evaluator.Evaluate("X5", s => 13));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("3")]
        public void TestAddition()
        {
            Assert.AreEqual(8, Evaluator.Evaluate("5+3", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("4")]
        public void TestSubtraction()
        {
            Assert.AreEqual(8, Evaluator.Evaluate("18-10", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("5")]
        public void TestMultiplication()
        {
            Assert.AreEqual(8, Evaluator.Evaluate("2*4", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("6")]
        public void TestDivision()
        {
            Assert.AreEqual(8, Evaluator.Evaluate("16/2", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("7")]
        public void TestArithmeticWithVariable()
        {
            Assert.AreEqual(6, Evaluator.Evaluate("2+X1", s => 4));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("8")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUnknownVariable()
        {
            Evaluator.Evaluate("2+X1", s => { throw new ArgumentException("Unknown variable"); });
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("9")]
        public void TestLeftToRight()
        {
            Assert.AreEqual(15, Evaluator.Evaluate("2*6+3", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("10")]
        public void TestOrderOperations()
        {
            Assert.AreEqual(20, Evaluator.Evaluate("2+6*3", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("11")]
        public void TestParenthesesTimes()
        {
            Assert.AreEqual(24, Evaluator.Evaluate("(2+6)*3", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("12")]
        public void TestTimesParentheses()
        {
            Assert.AreEqual(16, Evaluator.Evaluate("2*(3+5)", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("13")]
        public void TestPlusParentheses()
        {
            Assert.AreEqual(10, Evaluator.Evaluate("2+(3+5)", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("14")]
        public void TestPlusComplex()
        {
            Assert.AreEqual(50, Evaluator.Evaluate("2+(3+5*9)", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("15")]
        public void TestOperatorAfterParens()
        {
            Assert.AreEqual(0, Evaluator.Evaluate("(1*1)-2/2", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("16")]
        public void TestComplexTimesParentheses()
        {
            Assert.AreEqual(26, Evaluator.Evaluate("2+3*(3+5)", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("17")]
        public void TestComplexAndParentheses()
        {
            Assert.AreEqual(194, Evaluator.Evaluate("2+3*5+(3+4*8)*5+2", s => 0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("18")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDivideByZero()
        {
            Evaluator.Evaluate("5/0", s => 0);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("19")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSingleOperator()
        {
            Evaluator.Evaluate("+", s => 0);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("20")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestExtraOperator()
        {
            Evaluator.Evaluate("2+5+", s => 0);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("21")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestExtraParentheses()
        {
            Evaluator.Evaluate("2+5*7)", s => 0);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("22")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidVariable()
        {
            Evaluator.Evaluate("xx", s => 0);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("23")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPlusInvalidVariable()
        {
            Evaluator.Evaluate("5+xx", s => 0);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("24")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParensNoOperator()
        {
            Evaluator.Evaluate("5+7+(5)8", s => 0);
        }


        [TestMethod(), Timeout(5000)]
        [TestCategory("25")]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEmpty()
        {
            Evaluator.Evaluate("", s => 0);
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("26")]
        public void TestComplexMultiVar()
        {
            Assert.AreEqual(6, Evaluator.Evaluate("y1*3-8/2+4*(8-9*2)/14*x7", s => (s == "x7") ? 1 : 4));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("27")]
        public void TestComplexNestedParensRight()
        {
            Assert.AreEqual(6, Evaluator.Evaluate("x1+(x2+(x3+(x4+(x5+x6))))", s => 1));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("28")]
        public void TestComplexNestedParensLeft()
        {
            Assert.AreEqual(12, Evaluator.Evaluate("((((x1+x2)+x3)+x4)+x5)+x6", s => 2));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("29")]
        public void TestRepeatedVar()
        {
            Assert.AreEqual(0, Evaluator.Evaluate("a4-a4*a4/a4", s => 3));
        }

    }
}
