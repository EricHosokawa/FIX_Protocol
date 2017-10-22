using System;
using QuickFix;
using QuickFix.Fields;
using System.Collections.Generic;

namespace ConsoleFIX_Initiator
{
    public class TradeClientApp : MessageCracker, IApplication
    {
        #region Properties

        public static Session _session = null;

        public static int contadorQuote;

        public IInitiator MyInitiator = null;

        #endregion

        public void Run()
        {
            while (true)
            {
                try
                {
                    char action = QueryAction();

                    switch (action)
                    {
                        case '1':
                            QueryQuoteRequest();
                            break;
                        case 'Q':
                            {
                                if (this.MyInitiator.IsStopped)
                                    Console.WriteLine("Already stopped.");
                                else
                                {
                                    Console.WriteLine("Stopping initiator...");
                                    this.MyInitiator.Stop();
                                }

                                Console.WriteLine("Program shutdown.");
                            }
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Message Not Sent: " + e.Message);
                    Console.WriteLine("StackTrace: " + e.StackTrace);
                }
            }
        }

        private char QueryAction()
        {
            // Commands 'g' and 'x' are intentionally hidden.
            Console.Write("\n"
                + "1) Enter RFQ\n"
                + "2) Enter ExecutionReport\n"
                + "Q) Quit\n"
                + "Action: "
            );

            HashSet<string> validActions = new HashSet<string>("1,2,Q".Split(','));

            string cmd = Console.ReadLine().Trim();

            if (cmd.Length != 1 || validActions.Contains(cmd) == false)
                throw new System.Exception("Invalid action");

            return cmd.ToCharArray()[0];
        }

        #region IApplication Methods

        public void FromAdmin(Message message, SessionID sessionID)
        {
            Console.WriteLine("Admin IN:  " + message.ToString());
        }

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("App IN:  " + message.ToString());

            try
            {
                Crack(message, sessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==Cracker exception==");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void ToAdmin(Message message, SessionID sessionID)
        {
            Console.WriteLine("Admin OUT: " + message.ToString());
        }

        public void ToApp(Message message, SessionID sessionId)
        {
            Console.WriteLine("App OUT: " + message.ToString());
        }

        public void OnCreate(SessionID sessionID)
        {
            _session = Session.LookupSession(sessionID);
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine("Logon - " + sessionID.ToString());
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine("Logout - " + sessionID.ToString());
        }

        #endregion

        #region MessageCracker

        public void OnMessage(QuickFix.FIX44.Quote quote, SessionID s)
        {
            try
            {
                Console.WriteLine("Received Quote Message.");

                QuickFix.FIX44.ExecutionReport exec = new QuickFix.FIX44.ExecutionReport();

                exec.SetField(new OrderID(quote.QuoteID.getValue()));
                exec.SetField(new ClOrdID(quote.QuoteReqID.getValue()));
                exec.SetField(new Side(quote.Side.getValue()));
                exec.SetField(new Symbol(quote.Symbol.getValue()));
                exec.SetField(new Currency(quote.Currency.getValue()));
                exec.SetField(new SecurityType(quote.SecurityType.getValue()));
                exec.SetField(new CFICode(quote.CFICode.getValue()));

                if (quote.BidPx.getValue() > 0)
                    exec.SetField(new Price(quote.OfferPx.getValue()));
                else
                    exec.SetField(new Price(quote.BidPx.getValue()));

                exec.SetField(new OrderQty(quote.OrderQty.getValue()));
                exec.SetField(new TransactTime(DateTime.Now.Date));
                exec.SetField(new SettlDate(quote.SettlDate.getValue()));
                exec.SetField(new Account(quote.Account.getValue()));

                _session.Send(exec);

                Console.WriteLine(exec.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        private void QueryQuoteRequest()
        {
            try
            {
                Console.WriteLine("Sending a QuoteRequest.");

                QuickFix.FIX44.QuoteRequest rfq = new QuickFix.FIX44.QuoteRequest();

                rfq.SetField(new QuoteReqID(new Guid().ToString()));
                rfq.SetField(new Side('1'));
                rfq.SetField(new Symbol("EUR/USD"));
                rfq.SetField(new Currency("USD"));
                rfq.SetField(new SecurityType("FOR"));
                rfq.SetField(new CFICode("SPOT"));
                rfq.SetField(new OrderQty(2000000));
                rfq.SetField(new TransactTime(DateTime.Now.Date));
                rfq.SetField(new SettlDate(DateTime.Now.Date.AddDays(2).ToString("yyyy-MM-dd")));
                rfq.SetField(new Account("client123"));

                _session.Send(rfq);

                Console.WriteLine(rfq.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}