using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infra.Shared.Exceptions;
using Infra.Shared.Extensions;
using Infra.Validation.Interfaces;
using Infra.Validation.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Validation.Validator
{
    public class Validator : IValidator
    {
        private readonly IServiceProvider _serviceProvider;

        public Validator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<BadRequestException.ValidationError> Validate(
            FieldValidation fieldValidation,
            bool shouldCheckIsRequired = false)
        {
            if (fieldValidation.Rules.IsNullOrEmpty())
                return new List<BadRequestException.ValidationError>();

            // first check if value has been passed
            var errorList = new ConcurrentBag<BadRequestException.ValidationError>();

            if (fieldValidation.Value == null)
            {
                if (shouldCheckIsRequired)
                {
                    errorList.Add(new BadRequestException.ValidationError
                    {
                        Errors = new List<BadRequestException.ValidationErrorDetail>
                        {
                            new BadRequestException.ValidationErrorDetail
                            {
                                DisplayErrorMessage = $"No value was specified"
                            }
                        },
                        Target = fieldValidation.Name
                    });
                }

                return errorList.ToList();
            }

            Parallel.ForEach(fieldValidation.Rules, validationRule =>
            {
                var isValid = true;

                switch (validationRule.Type)
                {
                    case ViewValidationType.Email:
                        var emailStrategy = _serviceProvider.GetService<IEmailFieldValidator>();
                        isValid = emailStrategy.Apply(fieldValidation.Value, validationRule);
                        break;
                    case ViewValidationType.Required:
                        var requiredStrategy = _serviceProvider.GetService<IRequiredFieldValidator>();
                        isValid = !shouldCheckIsRequired || requiredStrategy.Apply(fieldValidation.Value, validationRule);
                        break;
                    case ViewValidationType.Date:
                        var dateStrategy = _serviceProvider.GetService<IDateFieldValidator>();
                        isValid = dateStrategy.Apply(fieldValidation.Value, validationRule);
                        break;
                    case ViewValidationType.DateTime:
                        var datetimeStrategy = _serviceProvider.GetService<IDateTimeFieldValidator>();
                        isValid = datetimeStrategy.Apply(fieldValidation.Value, validationRule);
                        break;
                    case ViewValidationType.Length:
                        var lengthStrategy = _serviceProvider.GetService<ILengthFieldValidator>();
                        isValid = lengthStrategy.Apply(fieldValidation.Value, validationRule);
                        break;
                    default:
                        errorList.Add(new BadRequestException.ValidationError
                        {
                            Errors = new List<BadRequestException.ValidationErrorDetail>
                            {
                                new BadRequestException.ValidationErrorDetail
                                {
                                    DisplayErrorMessage = $"Unsupported type [{validationRule.Type}] specified"
                                }
                            },
                            Target = fieldValidation.Name
                        });
                        break;
                }

                if (!isValid)
                {
                    errorList.Add(new BadRequestException.ValidationError
                    {
                        Errors = new List<BadRequestException.ValidationErrorDetail>
                        {
                            new BadRequestException.ValidationErrorDetail
                            {
                                DisplayErrorMessage = validationRule.ErrorMessage
                            }
                        },
                        Target = fieldValidation.Name
                    });
                }
            });

            return errorList.ToList();
        }

        public void Validate(List<FieldValidation> fieldValidations, bool shouldCheckIsRequired = false)
        {
            var validationErrors = new ConcurrentBag<BadRequestException.ValidationError>();

            Parallel.ForEach(fieldValidations, fieldValidation =>
            {
                var errorList = Validate(fieldValidation, shouldCheckIsRequired);
                if (errorList.Any())
                {
                    validationErrors.AddRange(errorList);
                }
            });

            var errors = validationErrors.ToList();

            if(errors.Any())
                throw new BadRequestException(
                    "An error occured trying to validate the view",
                    errors);
        }
      
    }
}