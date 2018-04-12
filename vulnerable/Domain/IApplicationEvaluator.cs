using System.IO;
using Microsoft.AspNetCore.Http;

namespace vulnerable.Domain
{
    public interface IApplicationEvaluator {
        EvaluationResult Evaluate(string firstName, string lastName, IFormFile bankFile);
    }
}