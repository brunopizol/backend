using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Infrastructure.Security
{
    public class PasswordOptions
    {
        public int DegreeOfParallelism { get; set; } = 8;
        public int Iterations { get; set; } = 4;
        public int MemorySizeKB { get; set; } = 65536;
        public int SaltSize { get; set; } = 16;
        public int HashSize { get; set; } = 32;
        public string Pepper { get; set; } = "";
    }

}
