using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter.Tests.MethodCodeInjectingProcessor
{
    public class Injection_TestClass1
    {
        public void InjectAtBegining_WithoutParam()
        {
        }

        public void InjectOnExit_WithoutParam()
        {
        }

        public void InjectAtBegining_WithParam(int value)
        {
        }

        public void InjectOnExit_WithParam(int value)
        {
        }

        public void InjectAtBegining_WithChangingParam(int value)
        {
            value *= 2;
        }

        public void InjectOnExit_WithChangingParam(int value)
        {
            value *= 2;
        }

        public void InjectAtBegining_Method6(int value, Action<string> action)
        {
            if (value >= 0)
            {
                action("if");
            }
            else
            {
                action("else");
                action("exit 1");
                return;
            }

            action("exit 2");
        }
    }
}