using Microsoft.OpenApi.Models;

namespace API_UsePrevention.Extensions
{
    public static class SwaggerServiceExtension
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Use Prevention Trading API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http, 
                    Scheme = "bearer",              
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập token (không cần chữ Bearer ở đầu)"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });

            return services;
        }
    }
}
