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
        private readonly ILogger<ApplicationEvaluator> logger;

        public ApplicationEvaluator(ILogger<ApplicationEvaluator> logger)
        {
            this.logger = logger;
        }

        public EvaluationResult Evaluate(string firstName, string lastName, IFormFile bankFile)
        {
            logger.LogDebug($"Evaluating bankfile for {firstName} {lastName}");
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
            } catch(Exception ex)
            {
                //sometimes it can be handy to give support a breadcrumb
                var reference = Guid.NewGuid().ToString().Substring(0, 5);
                logger.LogError($"bankFile content: {xDoc.InnerText}\r\nerror: {ex.Message}\r\nref: {reference}", ex);
                return new EvaluationResult(EvaluationStatus.Rejected, $"Oh Snap! Something went wrong. Please call us on 55555555. reference: {reference}");
            }
        }

        private XmlDocument ParseBankFile(IFormFile bankFile)
        {
            //copied from our internal intranet code base
            var xDoc = new XmlDocument();
            xDoc.XmlResolver = new XmlUrlResolver();

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
                catch(Exception ex)
                {
                    //the boss said need to know when hackers are trying to get us
                    logger.LogError(ex, ex.Message);
                }
            }
            return xDoc;
        }
    }
}