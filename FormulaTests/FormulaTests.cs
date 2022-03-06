using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;
/// <summary>
/// Author - Wenlin Li
/// </summary>
namespace FormulaTestes
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void Test1()
        {
            Formula f = new Formula("a1 + b1 ", s => s, s => true);
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test2()
        {
            Formula f = new Formula("2a + 2b", normalizeTester, isValidTester);
        }

        [TestMethod]
        public void Test4()
        {
            Formula test = new Formula("a1+b2", s => s, s => true);
            Assert.IsTrue(test.Equals(new Formula("a1+b2")));
        }

        [TestMethod]
        public void Test5()
        {
            Formula test = new Formula("a1+b2");
            Assert.IsFalse(test.Equals(new Formula("b2+a1")));
        }

        [TestMethod]
        public void Test6()
        {
            Formula test = new Formula("5.0 + a1");
            Assert.IsTrue(test.Equals(new Formula("5.0000000 + a1")));
        }

        [TestMethod]
        public void Test7()
        {
            Formula f = new Formula("X1+11", normalizeTester, s => true);
            Assert.IsTrue(f == new Formula("X1+11"));
        }

        [TestMethod]
        public void Test8()
        {
            Formula f = new Formula("a4 + 33");
            Assert.IsFalse(f == new Formula("2.000 + a7"));
        }


        [TestMethod]
        public void Test9()
        {
            string[] knownVariables = new string[3];
            knownVariables[0] = "X";
            knownVariables[1] = "Y";
            knownVariables[2] = "Z";

            List<string> variables = new List<string>();
            foreach (string s in new Formula("x+y*z", normalizeTester, s => true).GetVariables())
            {
                variables.Add(s);
            }

            bool exists = true;
            foreach (string s in knownVariables)
            {
                if (!variables.Contains(s))
                {
                    exists = false;
                }

            }

            Assert.IsTrue(exists);
            Assert.IsTrue(variables.Count == 3);
        }


        [TestMethod]
        public void Test10()
        {
            Formula f = new Formula("a + b", normalizeTester, s => true);
            String temp = "A+B";

            Assert.IsTrue(f.ToString().Equals(temp));
        }

        [TestMethod]
        public void Test11()
        {
            Formula f1 = new Formula("a1+b2", s => s, isValidTester);
            Formula f2 = new Formula("a1+b2");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        [TestMethod]
        public void Test12()
        {
            Formula f1 = new Formula("a1+b2", normalizeTester, isValidTester);
            Formula f2 = new Formula("A1+B2");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        [TestMethod()]
        public void Test13()
        {
            Formula f = new Formula("7");
            Assert.AreEqual(7.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test14()
        {
            Formula f = new Formula("a5");
            Assert.AreEqual(100.0, f.Evaluate(s => 100));
        }

        [TestMethod()]
        public void Test15()
        {
            Formula f = new Formula("11+7");
            Assert.AreEqual(18.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test16()
        {
            Formula f = new Formula("10-10");
            Assert.AreEqual(0.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test17()
        {
            Formula f = new Formula("4*4");
            Assert.AreEqual(16.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test18()
        {
            Formula f = new Formula("8/2");
            Assert.AreEqual(4.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test19()
        {
            Formula f = new Formula("3+X1");
            Assert.AreEqual(10.0, f.Evaluate(s => 7));
        }

        [TestMethod()]
        public void Test20()
        {
            Formula f = new Formula("3*7+4");
            Assert.AreEqual(25.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test21()
        {
            Formula f = new Formula("2+14*2");
            Assert.AreEqual(30.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test22()
        {
            Formula f = new Formula("(5+5)*3");
            Assert.AreEqual(30.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test23()
        {
            Formula f = new Formula("2*(5+10)");
            Assert.AreEqual(30.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test24()
        {
            Formula f = new Formula("2+(9+11)");
            Assert.AreEqual(22.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test25()
        {
            Formula f = new Formula("2+(3+11*0)");
            Assert.AreEqual(5.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test26()
        {
            Formula f = new Formula("2+1*(5+5)");
            Assert.AreEqual(12.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test27()
        {
            Formula f = new Formula("4+5*2+(8+2*1)*10+2");
            Assert.AreEqual(116.0, f.Evaluate(s => 0));
        }

        [TestMethod()]
        public void Test28()
        {
            Formula f = new Formula("10/0");
            Assert.IsTrue(f.Evaluate(s => 0) is FormulaError);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test29()
        {
            Formula f1 = new Formula("+");
            Formula f2 = new Formula("-");
            Formula f3 = new Formula("*");
            Formula f4 = new Formula("/");
            Formula f5 = new Formula("(");
            Formula f6 = new Formula(")");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test30()
        {
            Formula f = new Formula("10+100+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test31()
        {
            Formula f = new Formula("15+20*8)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test32()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod()]
        public void Test33()
        {
            Formula f = new Formula("10+aa");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test34()
        {
            Formula f = new Formula("10+(55)8");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test35()
        {
            Formula f = new Formula("");
        }

        [TestMethod()]
        public void Test36()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.142857142, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod()]
        public void Test37()
        {
            Formula f = new Formula("a1+(a2+(a3+(a4+(a5+a6))))");
            Assert.AreEqual(60.0, f.Evaluate(s => 10));
        }

        [TestMethod()]
        public void Test38()
        {
            Formula f = new Formula("((((a1+a2)+a3)+a4)+a5)+a6");
            Assert.AreEqual(60.0, f.Evaluate(s => 10));
        }

        [TestMethod()]
        public void Test39()
        {
            Formula f = new Formula("a1-a1*a1/a1");
            Assert.AreEqual(0.0, f.Evaluate(s => 500));
        }


        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test40()
        {
            Formula f = new Formula("100+50(80()722");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test41()
        {
            Formula f = new Formula("553223+55467^5478X%100()8546");
        }

        [TestMethod()]
        public void Test42()
        {
            Formula f = new Formula("46554-578*50/a1");
            Assert.IsTrue(f.Evaluate(s => 0) is FormulaError);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test43()
        {
            Formula f = new Formula("5456+-4567(5456()8456");
        }

        [TestMethod()]
        public void Test44()
        {
            Formula f1 = new Formula("4");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1.ToString().Equals("4"));
            Assert.IsTrue(f2.ToString().Equals("5"));
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test45()
        {
            Formula f = new Formula("5456+a2%@");
        }

        [TestMethod()]
        public void Test46()
        {
            Formula f = new Formula("100+a1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test47()
        {
            Formula f = new Formula("100/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test48()
        {
            Formula f = new Formula("(100574 + T1) / (T1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test49()
        {
            Formula f1 = new Formula("W1+W2");
            Formula f2 = new Formula(" W1  +  W2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test50()
        {
            Formula f1 = new Formula("1e-2 + X7 + 20.00 * 80 ");
            Formula f2 = new Formula("   0.0100  +     X7+ 20 * 80.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test51()
        {
            Formula f = new Formula("100");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }

        [TestMethod()]
        public void Test52()
        {
            Formula f1 = new Formula("100");
            Formula f2 = new Formula("100");
            Assert.IsFalse(null == f1);
        }


        [TestMethod()]
        public void Test53()
        {
            Formula f1 = new Formula("100");
            Formula f2 = new Formula("100");
            Assert.IsFalse(f1 == null);
        }

        [TestMethod()]
        public void Test54()
        {
            Formula f1 = new Formula("100");
            Formula f2 = new Formula("100");
            Assert.IsTrue(f1 == f2);
        }

        // Tests of != operator
        [TestMethod()]
        public void Test55()
        {
            Formula f1 = new Formula("888");
            Formula f2 = new Formula("888");
            Assert.IsFalse(f1 != f2);
        }

        // Test of ToString method
        [TestMethod()]
        public void Test56()
        {
            Formula f = new Formula("100*888");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }

        // Tests of GetHashCode method
        [TestMethod()]
        public void Test57()
        {
            Formula f1 = new Formula("555*888");
            Formula f2 = new Formula("555*888");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        [TestMethod()]
        public void Test58()
        {
            Formula f1 = new Formula("100*555");
            Formula f2 = new Formula("888/8*567+(876521)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }

        // Tests of GetVariables method
        [TestMethod()]
        public void Test59()
        {
            Formula f = new Formula("111*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test60()
        {
            Formula f = new Formula("(((((8+55*X57)/(89e-587+X26-X487))*X875+.0005e+9782)-8.278)*3.14159) * ((x287+3.1758)-.00000000008)");
        }

        [TestMethod()]
        public void TestHashCodeComplex()
        {
            Formula f1 = new Formula("555 * 4565 + 4778.00 - _a");
            Formula f2 = new Formula("2564*546+4456-_a");

            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }



        public static string normalizeTester(string formula)
        {
            return formula.ToUpper();
        }

        public static bool isValidTester(string formula)
        {
            if (formula[0].Equals("_") || Char.IsLetter(formula[0]))
            {
                if (formula.Length == 1)
                {
                    return true;
                }
                for (int i = 1; i < formula.Length; i++)
                {
                    if (Char.IsLetter(formula[i]) || Char.IsNumber(formula[i]) || formula[i].Equals("_"))
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


    }
}