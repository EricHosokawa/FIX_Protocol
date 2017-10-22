using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix;

namespace ConsoleFIX_Acceptor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=============");
            Console.WriteLine("This is only an example program.");
            Console.WriteLine("It's a simple server (e.g. Acceptor) app that will let clients (e.g. Initiators)");
            Console.WriteLine("connect to it.  It will accept and display any application-level messages that it receives.");
            Console.WriteLine("Connecting clients should set TargetCompID to 'SIMPLE' and SenderCompID to 'CLIENT1' or 'CLIENT2'.");
            Console.WriteLine("Port is 5001.");
            Console.WriteLine("(see simpleacc.cfg for configuration details)");
            Console.WriteLine("=============");

            if (args.Length != 1)
            {
                Console.WriteLine("usage: SimpleAcceptor CONFIG_FILENAME");
            }

            try
            {
                var configPath = "..\\Debug\\sample_acceptor.cfg";

                SessionSettings settings = new SessionSettings(configPath);
                IApplication app = new SimpleAcceptorApp();
                IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
                ILogFactory logFactory = new FileLogFactory(settings);
                IAcceptor acceptor = new ThreadedSocketAcceptor(app, storeFactory, settings, logFactory);

                acceptor.Start();
                Console.WriteLine("press <enter> to quit");
                Console.Read();

                acceptor.Stop();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("==FATAL ERROR==");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
