using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace EthnoBot.Models
{
    public class PaypalLogger
    {
      
        public static void Log(String Messages)
        {
            try {
             //   StreamWriter strw = new StreamWriter( "Logs/PaypalError.log",true);
           //     strw.WriteLine("{0}--->{1}",DateTime.Now.ToString("MM/dd//yyyy HH:mm:ss") + "-->" +Messages);
            } catch(Exception ) { throw; }

        }
    }
}