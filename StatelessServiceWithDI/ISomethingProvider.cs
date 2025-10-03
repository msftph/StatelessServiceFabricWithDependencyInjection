using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatelessServiceWithDI
{
    public interface ISomethingProvider
    {
        string GetSomething();
    }

    public class SomethingProvider : ISomethingProvider
    {
        public string GetSomething()
        {
            return "Hello from SomethingProvider!";
        }
    }
}
