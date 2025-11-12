using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackBird.Api.src.Bird.Modules.Users.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Commands.Login
{
    public class LoginHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public LoginHandler(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> Handle(LoginCommand command)
        {
            // 1. Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(command.Email);
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("Email o contraseña incorrectos");
            }

            // 2. Verificar contraseña con BCrypt
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash);
            
            if (!isValidPassword)
            {
                throw new UnauthorizedAccessException("Email o contraseña incorrectos");
            }

            // 3. Generar JWT Token
            var token = GenerateJwtToken(user.Id.ToString(), user.Email, user.Role.ToString());

            // 4. Retornar respuesta con token
            return new LoginResponse
            {
                Token = token,
                UserId = user.Id.ToString(),
                Email = user.Email,
                Name = user.Name,
                Role = user.Role.ToString().ToLower() // admin, experto, usuario
            };
        }

        private string GenerateJwtToken(string userId, string email, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24), // Token válido por 24 horas
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
