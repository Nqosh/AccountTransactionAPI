using AccountTransaction.DTOs;
using AccountTransaction.Helpers;
using AccountTransaction.Model;
using AutoMapper;
using Customer_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Customer_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthService repo, IConfiguration config, IMapper mapper)
        {
            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExist(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createdUser = await _repo.Regiser(userToCreate, userForRegisterDto.Password);
            var UserToReturn = _mapper.Map<User>(createdUser);

            return Ok(UserToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLogInDto userForLogInDto)
        {

            //try
            //{
            var userfromRepo = await _repo.Login(userForLogInDto.UserName.ToLower(), userForLogInDto.Password);

            if (userfromRepo == null)
                return Unauthorized();

            Claims.ClaimsArr = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userfromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userfromRepo.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.
                GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims.ClaimsArr),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user = _mapper.Map<UserForListDto>(userfromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
            //}
            //catch
            //{
            //    return StatusCode(500, "Computer really says no!");
            //}
        }

    }
}
