using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Test.Data
{
    public class Book
    {
        public int Id { get; set; }

        public string BookName { get; set; }

        public int? BookPages { get; set; }

        public int? BookCategory { get; set; }
    }
}
