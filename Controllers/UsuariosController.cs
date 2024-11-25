using System;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public UsuariosController(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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



    private string HashPass(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8));
    }

    private string GenerateToken(Usuario usuario, int duracion)
    {
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.ASCII.GetBytes(_configuration["TokenAuthentication:SecretKey"]));
        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim("Id", usuario.Id.ToString()),
            new Claim("FullName", usuario.Nombre + " " + usuario.Apellido)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["TokenAuthentication:Issuer"],
            audience: _configuration["TokenAuthentication:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(duracion),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}