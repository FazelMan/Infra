using System.Collections.Generic;
using System.Threading.Tasks;
using Infra.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Infra.Shared.Http.Filters
{
    public class ModelValidationTransformation : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {

                var errors = new List<BadRequestException.ValidationError>();

                CheckForError(context.ModelState.Root, ref errors);

                throw new BadRequestException(
                    "Model state validation failed",
                    errors);

            }
            await next();
        }

        private void CheckForError(ModelStateEntry entry, ref List<BadRequestException.ValidationError> list)
        {

            if (entry.IsContainerNode)
            {
                foreach (var child in entry.Children)
                {
                    CheckForError(child, ref list);
                }
            }

            if (entry.ValidationState == ModelValidationState.Invalid)
            {
                var modelStateError =
                    JsonConvert.DeserializeObject<ModelStateError>(JsonConvert.SerializeObject(entry));

                list.Add(new BadRequestException.ValidationError
                {
                    Target = modelStateError.Key,
                    Errors = modelStateError.Errors
                });
            }
        }

        private class ModelStateError
        {
            public List<BadRequestException.ValidationErrorDetail> Errors { get; set; }
            public string Key { get; set; }
        }
    }
}