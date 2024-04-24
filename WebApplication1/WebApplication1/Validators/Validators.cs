using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1.Validators
{
    public static class Validators
    {
        public static void RegisterValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<AnimalCreateRequestValidator>>();
            services.AddSingleton<IValidator<AnimalReplaceRequestValidator>>();
        }
    }
}