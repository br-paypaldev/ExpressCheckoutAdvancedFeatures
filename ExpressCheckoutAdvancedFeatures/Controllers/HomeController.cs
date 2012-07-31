using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Specialized;

namespace ExpressCheckoutAdvancedFeatures.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ServicePointManager.ServerCertificateValidationCallback = Validator;

            NameValueCollection nvp = new NameValueCollection();

            nvp.Set("USER", "usuario");
            nvp.Set("PWD", "senha");
            nvp.Set("SIGNATURE", "assinatura");
            nvp.Set("METHOD", "SetExpressCheckout");
            nvp.Set("RETURNURL", "http://127.0.0.1/return");
            nvp.Set("CANCELURL", "http://127.0.0.1/cancel");
            nvp.Set("LOCALECODE", "pt_BR");
            nvp.Set("PAYMENTREQUEST_0_AMT", "1442.80");
            nvp.Set("PAYMENTREQUEST_0_ITEMAMT", "1442.80");
            nvp.Set("PAYMENTREQUEST_0_SELLERPAYPALACCOUNTID", "empresa@turismo.com");
            nvp.Set("PAYMENTREQUEST_0_PAYMENTACTION", "Sale");
            nvp.Set("PAYMENTREQUEST_0_PAYMENTREQUESTID", "CVC123");
            nvp.Set("PAYMENTREQUEST_0_DESC", "Disney 8 dias");
            nvp.Set("L_PAYMENTREQUEST_0_NAME0", "Transporte aéreo para Orlando");
            nvp.Set("L_PAYMENTREQUEST_0_DESC0", "Transporte aéreo ida e volta");
            nvp.Set("L_PAYMENTREQUEST_0_QTY0", "1");
            nvp.Set("L_PAYMENTREQUEST_0_AMT0", "1342.8");
            nvp.Set("L_PAYMENTREQUEST_0_NAME1", "Traslado");
            nvp.Set("L_PAYMENTREQUEST_0_DESC1", "Traslado: de chegada e saída em Orlando.");
            nvp.Set("L_PAYMENTREQUEST_0_QTY1", "1");
            nvp.Set("L_PAYMENTREQUEST_0_AMT1", "100");
            nvp.Set("PAYMENTREQUEST_1_AMT", "480.93");
            nvp.Set("PAYMENTREQUEST_1_ITEMAMT", "480.93");
            nvp.Set("PAYMENTREQUEST_1_SELLERPAYPALACCOUNTID", "reserva@hotel.com");
            nvp.Set("PAYMENTREQUEST_1_PAYMENTACTION", "Sale");
            nvp.Set("PAYMENTREQUEST_1_PAYMENTREQUESTID", "CVC456");
            nvp.Set("PAYMENTREQUEST_1_DESC", "Hospedagem");
            nvp.Set("L_PAYMENTREQUEST_1_NAME0", "Hospedagem");
            nvp.Set("L_PAYMENTREQUEST_1_DESC0", "7 noites de hospedagem em Resort Disney com Disney Dollar para o café da manhã e 1 MUG");
            nvp.Set("L_PAYMENTREQUEST_1_QTY0", "1");
            nvp.Set("L_PAYMENTREQUEST_1_AMT0", "480.93");

            StringBuilder sb = new StringBuilder();

            for (int i = 0, t = nvp.Count; i < t; ++i)
            {
                string key = nvp.GetKey(i);

                sb.Append(key + "=");
                sb.Append(HttpUtility.UrlEncode(nvp.Get(key)) + "&");
            }

            sb.Append("VERSION=84.0");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api-3t.sandbox.paypal.com/nvp");

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (Stream stream = request.GetRequestStream())
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(sb.ToString());

                stream.Write(bytes, 0, bytes.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            NameValueCollection nvc;

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string result = reader.ReadToEnd();

                    nvc = HttpUtility.ParseQueryString(result);
                }
            }

            return Redirect("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + nvc.Get("TOKEN"));
        }

        bool Validator(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}