using SimuladorGit.Clases;
using System;

namespace MiAppConsola
{
    class Program
    {
        static void Main(string[] args)
        {
            string linea = "";
            Git _git = new Git();
            Console.WriteLine("------- Git Console - v1.0 -------");
            _git.help();

            while (true)
            {
                linea = Console.ReadLine();

                _git.menu(linea);
            }
        }
    }
}