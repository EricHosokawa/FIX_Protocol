using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix;
using QuickFix.Fields;

namespace ConsoleFIX_Acceptor
{
    public class SimpleAcceptorApp : MessageCracker, IApplication
    {
        #region QuickFix.IApplication

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

        }

        public void OnLogon(SessionID sessionID)
        {

        }

        public void OnLogout(SessionID sessionID)
        {

        }

        #endregion

        #region MessageCracker

        public void OnMessage(QuickFix.FIX44.QuoteRequest rfq, SessionID s)
        {
            try
            {
                Console.WriteLine("Received Quote Request Message: ");

                QuickFix.FIX44.Quote quote = new QuickFix.FIX44.Quote();

                quote.SetField(new QuoteID(new Guid().ToString()));
                quote.SetField(new QuoteReqID(rfq.QuoteReqID.getValue()));
                quote.SetField(new BidPx(0));
                quote.SetField(new OfferPx(Convert.ToDecimal(1.18)));
                quote.SetField(new OrderQty(Convert.ToDecimal(rfq.GetField(Tags.OrderQty))));
                quote.SetField(new Side(Convert.ToChar(rfq.GetField(Tags.Side))));
                quote.SetField(new SecurityType(rfq.GetField(Tags.SecurityType)));
                quote.SetField(new CFICode(rfq.GetField(Tags.CFICode)));
                quote.SetField(new TransactTime(DateTime.Now.Date));
                quote.SetField(new SettlDate(rfq.GetField(Tags.SettlDate)));
                quote.SetField(new Account(rfq.GetField(Tags.Account)));
                quote.SetField(new Symbol(rfq.GetField(Tags.Symbol)));
                quote.SetField(new Currency(rfq.GetField(Tags.Currency)));

                Session.SendToTarget(quote, s);
                Console.WriteLine(quote.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void OnMessage(QuickFix.FIX44.ExecutionReport exec, SessionID s)
        {
            Console.WriteLine("Received Execution Report Message.");
        }

        #endregion
    }
}