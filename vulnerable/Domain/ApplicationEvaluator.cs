using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace vulnerable.Domain
{
    public class ApplicationEvaluator : IApplicationEvaluator {
        private const string XPATH_TOTAL_AMOUNT = @"/bankFile/account/currentAmount";
        private const string XPATH_ACC_NAME = @"/bankFile/account/name";

        public EvaluationResult Evaluate(string firstName, string lastName, IFormFile bankFile)
        {
            if (bankFile == null || bankFile.Length == 0)
            {
                throw new ArgumentNullException(nameof(bankFile));
            }

            XmlDocument xDoc = ParseBankFile(bankFile);
            try 
            {
                var amount = xDoc.SelectSingleNode(XPATH_TOTAL_AMOUNT)?.InnerText;
                var accountName = xDoc.SelectSingleNode(XPATH_ACC_NAME)?.InnerText;
                decimal amountNumber = 0;

                if(decimal.TryParse(amount, out amountNumber)) 
                {
                    if(amountNumber > 2000) {
                        return new EvaluationResult(EvaluationStatus.Accepted);
                    } else {
                        return new EvaluationResult(EvaluationStatus.Rejected, $"You've only got ${amount} in account {accountName}");
                    }   
                } 

                return new EvaluationResult(EvaluationStatus.Rejected, "Please check your bank statement!");
            } catch(Exception)
            {
                // GULP!
                return new EvaluationResult(EvaluationStatus.Rejected, "Oh Snap! Something went wrong. Please call our IT administrator on 55555555.");
            }
        }

        private static XmlDocument ParseBankFile(IFormFile bankFile)
        {
            var xDoc = new XmlDocument();
            var settings = new XmlReaderSettings() 
            {
                MaxCharactersFromEntities = 0,
                MaxCharactersInDocument = 0,
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = new XmlUrlResolver()
            };

            using (var reader = XmlReader.Create(bankFile.OpenReadStream(), settings))
            {
                try 
                {
                    xDoc.Load(reader);
                }
                catch(Exception)
                {
                    //the boss said we have to write bug free code
                }
            }
            return xDoc;
        }
    }
}