using System;

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

        public void InjectOnExit_MoreExits(int value, Action<string> action)
        {
            if (value >= 0)
            {
                action("if");
            }
            else
            {
                action("else");
                action("exit 2");
                return;
            }

            action("exit 1");
        }

        public void InjectOnExit_WithJumpToEnd()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    goto END;
                }
            }

            END:
            return;
        }
    }
}