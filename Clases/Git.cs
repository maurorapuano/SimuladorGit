using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorGit.Clases
{
    internal class Git
    {
        public List<string> files;
        public List<string> filesCommited;
        public Git() 
        {
            files = new List<string>();
            filesCommited = new List<string>();
        }

        public void menu(string linea)
        {
            bool error = false;
            string[] comandos = linea.Split(" ");

            switch (comandos[0].ToLower())
            {
                case "add":
                    if (comandos.Length < 2)
                    {
                        printError("Please, type at least one file to add...");
                        break;
                    }
                    /* Agrego cada archivo que haya sido tipeado luego del comando. Al salir, muestro el area de trabajo */
                    for (int i = 1; i < comandos.Length; i++)
                    {
                        error = add(comandos[i]);
                        if (error) printError("Adding file " + i.ToString() + " failed. Try again."); else printSuccess(comandos[i]);
                    }
                    break;
                case "commit":
                    /* Solo permito entradas tipo "commit <mensaje>" */
                    if(comandos.Length != 2)
                    {
                        printError("Please, only type a description message for your commit...");
                        break;
                    }
                    if (commit(comandos[1])) printSuccess("Commited succesfully!"); else printError("Commit failed. Try again."); 
                    break;
                case "push":
                    if (comandos.Length != 1)
                    {
                        printError("No params needed...");
                        break;
                    }
                    if (push()) printSuccess("Local commits succesfully uploaded!"); else printError("Push failed. Try again."); 
                    break;
                case "status":
                    showWorkingArea();
                    break;
                case "log":
                    if (comandos.Length != 1)
                    {
                        printError("No params needed...");
                        break;
                    }
                    log();
                    break;
                case "help":
                    help();
                    break;
                default:
                    break;
            }
        }

        private bool add(string file)
        {
            files.Add(file);
            return false;
        }

        private bool commit(string message)
        {
            /* Para lograr la persistencia de commits, simulo una BBDD con un csv */
            bool success = false;

            try
            {
                string rutaBase = Directory.GetCurrentDirectory();
                string filePath = Path.Combine(rutaBase, "Commits");
                Directory.CreateDirectory(filePath);
                filePath = Path.Combine(filePath, "commits.csv");

                if (!File.Exists(filePath))
                    if (!createCommitFile(filePath)) printError("Failed creating commits file.");

                using (StreamWriter writer = new StreamWriter(filePath,true))
                {
                    foreach (string file in files)
                    {
                        writer.WriteLine(file + "," + message + "," + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        filesCommited.Add(file);
                    }
                }
                success = true;

                /* Luego del commit, limpio los archivos en el area de trabajo y los dejo commiteados */
                files.Clear();
            }
            catch(Exception ex)
            {
                success = false;
            }

            return success;
        }

        private bool push()
        {
            /* Limpio lista de archivos commiteados */
            try
            {
                filesCommited.Clear();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        private void log()
        {
            try
            {
                string rutaBase = Directory.GetCurrentDirectory();
                string filePath = Path.Combine(rutaBase, "Commits");
                filePath = Path.Combine(filePath, "commits.csv");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("---------------------");
                    return;
                }

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string linea;
                    while ((linea = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(linea);
                    }
                    Console.WriteLine("---------------------");
                }
            }
            catch(Exception ex)
            {
                printError("Failed reading commits file...");
            }
        }

        public void help()
        {
            Console.WriteLine("Git console commands list:");
            Console.WriteLine("  add <filename> : Add files to your working area");
            Console.WriteLine("  commit <message> : Make new commit");
            Console.WriteLine("  push : Send your commit to a remote server");
            Console.WriteLine("  status : Show your full working area");
            Console.WriteLine("  log : Get a full log of all commits");
            Console.WriteLine("---------------------");
        }
        private void showWorkingArea()
        {
            Console.WriteLine("<Your working area>");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("<Not commited>");
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("<Commited>");
            foreach (string file in filesCommited)
            {
                Console.WriteLine(file);
            }
            Console.WriteLine("---------------------");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void printError(string msj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR - " + msj);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void printSuccess(string msj)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msj);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private bool createCommitFile(string filePath)
        {
            /* En primera pasada, creo archivo para indicarle cabeceras */
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("File,Message,Date");
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
