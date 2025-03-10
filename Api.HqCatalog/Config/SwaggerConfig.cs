using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace HqCatalog.Api.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "📚 HqCatalog API",
                    Version = "v1",
                    Description = "API para gerenciar HQs. Permite adicionar, atualizar, remover HQs e enviar imagens.",
                    Contact = new OpenApiContact
                    {
                        Name = "Kauan",
                        Email = "kauan.cerqueira198@outlook.com",
                        Url = new Uri("https://github.com/KauanCerqueira")
                    }
                });

                // 🔹 Adicionar suporte para autenticação JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Digite 'Bearer {seu_token}' abaixo para autenticar nos endpoints",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
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

                // 🔹 Habilita anotações de resumo e descrição nos endpoints
                c.EnableAnnotations();
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "📚 HqCatalog API v1");
                c.DocumentTitle = "📚 HqCatalog API - Documentação";
                c.DisplayRequestDuration(); // Exibe tempo de resposta da API

                // 🔹 Permite que o usuário digite um token manualmente
                c.DefaultModelsExpandDepth(-1); // Oculta a seção "Schemas"
            });

            return app;
        }
    }
}
