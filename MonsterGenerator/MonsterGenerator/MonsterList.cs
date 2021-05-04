using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterGenerator
{

    public class MonsterList
    {
        public int count { get; set; }
        public Result[] results { get; set; }
    }

    public class Result
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

}
