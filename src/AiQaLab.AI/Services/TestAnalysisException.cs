using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiQaLab.AI.Services
{
    public class TestAnalysisException : Exception
    {
        public TestAnalysisException(string message) : base(message) { }

        public TestAnalysisException(string message, Exception innerException) : base(message, innerException) { }
    }
}
