using back_end.DTOs;
using back_end.Records;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Configurations
{
    public static class ApiBehaviorConfiguration
    {
        public static IServiceCollection AddApiBehaviorConfiguration(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x =>
                            string.IsNullOrWhiteSpace(x.ErrorMessage)
                                ? "Invalid value."
                                : x.ErrorMessage)
                        .ToList();

                    var response = ApiResponse<List<string>>.ErrorResponse(
                        ErrorRecord.RequestInvalid,
                        errors
                    );

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}