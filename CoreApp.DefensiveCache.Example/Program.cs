using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace CoreApp.DefensiveCache.Example
{
    class Program
    {
        static void Main(string[] args)
        {

            Assembly myAssembly = Assembly.GetExecutingAssembly();
            foreach (var item in myAssembly.GetTypes())
            {
                Console.WriteLine(item.FullName);
            }
            
        }
    }
}
