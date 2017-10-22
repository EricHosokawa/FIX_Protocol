using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFIX_Initiator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=============");
            Console.WriteLine("This is only an example program, meant to run against the Executor or SimpleAcceptor example programs.");
            Console.WriteLine();
            Console.WriteLine("                                                    ! ! !");
            Console.WriteLine("              DO NOT USE THIS ON A COMMERCIAL FIX INTERFACE!  It won't work and it's a bad idea!");
            Console.WriteLine("                                                    ! ! !");
            Console.WriteLine();
            Console.WriteLine("=============");

            if (args.Length != 1)
            {
                System.Console.WriteLine("usage: TradeClient.exe CONFIG_FILENAME");
            }

            string file = "..\\Debug\\sample_initiator.cfg";

            try
            {
                QuickFix.SessionSettings settings = new QuickFix.SessionSettings(file);
                TradeClientApp application = new TradeClientApp();
                QuickFix.IMessageStoreFactory storeFactory = new QuickFix.FileStoreFactory(settings);
                QuickFix.ILogFactory logFactory = new QuickFix.FileLogFactory(settings);
                QuickFix.Transport.SocketInitiator initiator = new QuickFix.Transport.SocketInitiator(application, storeFactory, settings, logFactory);

                application.MyInitiator = initiator;

                initiator.Start();
                application.Run();

                initiator.Stop();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Environment.Exit(1);
        }
    }
}
