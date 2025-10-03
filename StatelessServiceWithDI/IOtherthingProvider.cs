using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatelessServiceWithDI
{
    public interface IOtherthingProvider
    {
        string GetOtherthing();
    }

    public class OtherthingProvider : IOtherthingProvider
    {
        private ISomethingProvider somethingProvider;


        public OtherthingProvider(ISomethingProvider somethingProvider)
        {
            this.somethingProvider = somethingProvider;
            // Constructor logic here
        }

        public string GetOtherthing()
        {
            return "Hello from OtherthingProvider!";
        }
    }
}
