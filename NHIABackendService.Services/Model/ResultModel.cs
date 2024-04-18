using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace NHIABackendService.Services.Model
{
    public class ResultModel<T>
    {
        public ResultModel()
        {
        }

        public ResultModel(T data, string message)
        {
            Data = data;
            Message = message;
        }

        public ResultModel(string errorMessage)
        {
            AddError(errorMessage);
        }

        public ResultModel(List<string> errorMessage)
        {
            errorMessage.ForEach(x => AddError(x));
        }

        public List<ValidationResult> ValidationErrors { get; set; } = new List<ValidationResult>();

        public string Message { get; set; }

        public T Data { get; set; }

        public string this[string columnName]
        {
            get
            {
                var validatioResult =
                    ValidationErrors.FirstOrDefault(r => r.MemberNames.FirstOrDefault() == columnName);
                return validatioResult == null ? string.Empty : validatioResult.ErrorMessage;
            }
        }

        public bool HasError
        {
            get
            {
                if (ValidationErrors.Count > 0) return true;

                return false;
            }
        }

        public List<string> GetErrorMessages()
        {
            return ValidationErrors.Select(c => c.ErrorMessage).ToList();
        }

        public void AddError(string error)
        {
            ValidationErrors.Add(new ValidationResult(error));
        }

        public void AddError(ValidationResult validationResult)
        {
            ValidationErrors.Add(validationResult);
        }

        public void AddError(IEnumerable<ValidationResult> validationResults)
        {
            ValidationErrors.AddRange(validationResults);
        }
    }
}
