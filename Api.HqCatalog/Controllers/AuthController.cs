using HqCatalog.Api.Config;
using Microsoft.AspNetCore.Mvc;

public class AuthController : ControllerBase
{
    private readonly JwtSettings _jwtSettings;

    public AuthController(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
    }

    // Métodos de autenticação...
}
