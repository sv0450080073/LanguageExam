using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLanguage.Dtos
{
    public class InMemoryFileDto
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
