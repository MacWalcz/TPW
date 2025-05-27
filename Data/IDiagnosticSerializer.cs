using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data.Diagnostics
{
    internal interface IDiagnosticSerializer<T>
    {
        string Serialize(T data);
    }
}

