using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp;
internal class TestService : ITestService
{
    public void PrintToConsole()
    {
        Console.WriteLine("Test");
    }
}
