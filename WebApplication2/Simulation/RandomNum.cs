using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using WebApplication2.Models;


namespace WebApplication2.Simulation
{
    public class RandomNum
    {
         public static class Util
    {
        private static Random rnd = new Random();
        public static int GetRandom()
        {
            return rnd.Next();
        }
    }
    }
}