using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager_API;
using TaskManager_API.Models;
using Microsoft.AspNetCore.Identity;
using TaskManager_API.Dto;

namespace TaskManager_API.Controllers
{


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
    {
        return await _context.Users
            .Include(u => u.Tasks)
            .ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Tasks)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        return user;
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<Users>> CreateUser(Users user)
    {
        var passwordHasher = new PasswordHasher<Users>();
        user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginRequestDto loginRequest)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);
        if (user == null)
            return Unauthorized("Niepoprawna nazwa użytkownika lub hasło1");

        var passwordHasher = new PasswordHasher<Users>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Niepoprawna nazwa użytkownika lub hasło2");

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username
        };

        return Ok(userDto);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
    {
        if (string.IsNullOrWhiteSpace(registerRequest.Username) || string.IsNullOrWhiteSpace(registerRequest.Password))
        {
            return BadRequest("Login i hasło wymagane!");
        }

        if (_context.Users.Any(u => u.Username == registerRequest.Username))
        {
            return Conflict("Taki użytkownik już istnieje.");
        }

        var user = new Users
        {
            Username = registerRequest.Username
        };

        var passwordHasher = new PasswordHasher<Users>();
        user.PasswordHash = passwordHasher.HashPassword(user, registerRequest.Password);


        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.Username });
    }
    

    // PUT: api/Users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, Users user)
    {
        if (id != user.Id)
            return BadRequest();

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id) =>
        _context.Users.Any(e => e.Id == id);
    }
}

