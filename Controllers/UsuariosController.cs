using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Razor;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWebHostEnvironment _environment;

    public UsuariosController(DataContext context, IConfiguration configuration, IRazorViewEngine razorViewEngine, IServiceProvider serviceProvider, IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        _razorViewEngine = razorViewEngine;
        _serviceProvider = serviceProvider;
        _environment = environment;
    }

    [AllowAnonymous]
    [HttpPost("login")] // Listo
    public async Task<IActionResult> Login([FromForm] LoginView loginView)
    {
        try
        {
            string hashed = HashPass(loginView.Clave);

            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == loginView.Email && x.Clave == hashed);

            if (u == null)
                return BadRequest("Email o clave incorrectos.");

            string token = GenerateToken(u, 60);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("registrar")] // Listo
    public async Task<IActionResult> Registrar([FromForm] RegistrarView registrarView)
    {
        try
        {
            if (registrarView.Email != registrarView.ConfirmarEmail)
                return BadRequest("Los emails no coinciden.");

            if (registrarView.Clave != registrarView.ConfirmarClave)
                return BadRequest("Las claves no coinciden.");

            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == registrarView.Email);

            if (u != null)
                return BadRequest("El email ya está en uso.");

            string passHashed = HashPass(registrarView.Clave);

            await _context.Usuarios.AddAsync(new Usuario
            {
                Nombre = registrarView.Nombre,
                Apellido = registrarView.Apellido,
                Email = registrarView.Email,
                Clave = passHashed,
                Avatar = "default.jpg"
            });
            
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("perfil")] // Listo
    public async Task<IActionResult> Perfil()
    {
        try
        {
            string id = User.Claims.First(c => c.Type == "Id").Value;
            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (u == null)
                return NotFound("No se encontró el usuario.");

            u.Clave = "";
            return Ok(u);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("perfil")] // Listo
    public async Task<IActionResult> Perfil([FromForm] Usuario usuario)
    {
        try
        {
            string id = User.Claims.First(c => c.Type == "Id").Value;
            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (u == null)
                return NotFound("No se encontró el usuario.");

            var emailExiste = await _context.Usuarios
                .AnyAsync(x => x.Email == usuario.Email && x.Id != u.Id);

            if (emailExiste)
                return BadRequest("El email ya está en uso");

            u.Nombre = usuario.Nombre;
            u.Apellido = usuario.Apellido;
            u.Email = usuario.Email;
            await _context.SaveChangesAsync();

            return Ok("Perfil actualizado.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("avatar")] // Listo
    public async Task<IActionResult> Avatar([FromForm] IFormFile? avatar)
    {
        try
        {
            string id = User.Claims.First(c => c.Type == "Id").Value;
            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (u == null)
                return NotFound("No se encontró el usuario.");

            if (avatar == null)
                return BadRequest("No se seleccionó un archivo.");

            if (!string.IsNullOrEmpty(u.Avatar))
            {
                var pathOld = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "av", u.Avatar);
                if (System.IO.File.Exists(pathOld))
                    System.IO.File.Delete(pathOld);
            }

            var guid = Guid.NewGuid().ToString();
            var fileName = $"{guid}{Path.GetExtension(avatar.FileName)}";
            var pathNew = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "av", fileName);

            using (var stream = new FileStream(pathNew, FileMode.Create))
                await avatar.CopyToAsync(stream);

            u.Avatar = fileName;
            await _context.SaveChangesAsync();
            return Ok("Avatar actualizado.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("clave")] // Listo
    public async Task<IActionResult> Clave([FromForm] ClaveView claveView)
    {
        try
        {
            string id = User.Claims.First(c => c.Type == "Id").Value;
            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (u == null)
                return NotFound("No se encontró el usuario.");

            string hashedActual = HashPass(claveView.Actual);

            if (hashedActual != u.Clave)
                return BadRequest("La clave actual es incorrecta.");

            if (claveView.Nueva != claveView.Repetida)
                return BadRequest("Las claves no coinciden.");
            
            string hashedNueva = HashPass(claveView.Nueva);

            u.Clave = hashedNueva;
            await _context.SaveChangesAsync();

            return Ok("Clave actualizada.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("email")]
    public async Task<IActionResult> Email([FromForm] string email)
    {
        try
        {
            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == email);

            if (u == null)
                return NotFound("El email no está registrado.");

            string token = GenerateToken(u, 10);
            string url = this.GenerateUrl("Token", "Usuarios", _environment);

            var datos = new EmailView { Enlace = url, Nombre = u.Nombre, Token = token };
            string htmlView = await this.RenderView("Views/Emails/RecuperarClave.cshtml", datos, _razorViewEngine, _serviceProvider);
            SendMail("soporte@mailtrap.com", u.Email, "Restablecer contraseña", htmlView);

            return Ok("Email enviado.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("token")]
    public async Task<IActionResult> Token([FromQuery] string access_token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var expiration = handler.ReadJwtToken(access_token).ValidTo;

            if (expiration < DateTime.UtcNow)
            {
                string tokenVencidoView = await this.RenderView
                (
                    "Views/Emails/Informacion.cshtml", 
                    new { Mensaje = "El link ya expiró." }, 
                    _razorViewEngine, _serviceProvider
                );
                return Content(tokenVencidoView, "text/html");
            }

            string id = User.Claims.First(c => c.Type == "Id").Value;
            var u = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == int.Parse(id));

            if (u == null)
                return NotFound("No se encontró el usuario.");

            string newPass = GeneratePass(4);
            u.Clave = HashPass(newPass);
            await _context.SaveChangesAsync();

            var datos = new EmailView { Nombre = u.Nombre, Clave = newPass };
            string htmlView = await this.RenderView("Views/Emails/NuevaClave.cshtml", datos, _razorViewEngine, _serviceProvider);
            SendMail("soporte@mailtrap.com", u.Email, "Tu nueva contraseña", htmlView);

            string claveEnviadaView = await this.RenderView
            (
                "Views/Emails/Informacion.cshtml", 
                new { Mensaje = "Enviamos tu nueva clave por correo." }, 
                _razorViewEngine, _serviceProvider
            );
            return Content(claveEnviadaView, "text/html");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private string HashPass(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8));
    }

    private string GenerateToken(Usuario user, int duration)
    {
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.ASCII.GetBytes(_configuration["TokenAuthentication:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim("Id", user.Id.ToString()),
            new Claim("FullName", user.Nombre + " " + user.Apellido)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["TokenAuthentication:Issuer"],
            audience: _configuration["TokenAuthentication:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(duration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void SendMail(string sender, string receiver, string subject, string body)
    {
        var client = new SmtpClient(_configuration["SMTP:Host"], _configuration.GetValue<int>("SMTP:Port"))
        {
            Credentials = new NetworkCredential(_configuration["SMTP:User"], _configuration["SMTP:Password"]),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(sender),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        message.To.Add(new MailAddress(receiver));
        client.SendMailAsync(message);
    }

    private string GeneratePass(int length)
    {
        Random rand = new Random(Environment.TickCount);
        string randomChars = "abcdefghijklmnpqrstuvwxyz123456789";
        string newPass = "";
        for (int i = 0; i < length; i++)
            newPass += randomChars[rand.Next(0, randomChars.Length)];
        return newPass;
    }

}