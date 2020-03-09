using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPARobot.Models
{
    public class Cycle
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public Cycle(int key, string name)
        {
            Key = key;
            Name = name;
        }

        public static Cycle Disposable = new Cycle(0, "一次性");
        public static Cycle Daily = new Cycle(1, "每天");
        public static Cycle Weekly = new Cycle(2, "每周");
        public static Cycle Monthly = new Cycle(3, "每月");

        public static List<Cycle> Cycles = new List<Cycle> {
            Disposable,
            Daily,
            Weekly,
            Monthly
        };
    }
}
