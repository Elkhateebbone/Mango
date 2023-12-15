using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IjwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,IjwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager; 
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user  = _db.ApplicationUsers.FirstOrDefault(u=>u.Email.ToLower() == email.ToLower());
            if (user != null) {
            if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
            await _userManager.AddToRoleAsync(user,roleName);
                return true;
            }
            return
                false;
        }
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {

            var user = _db.ApplicationUsers.FirstOrDefault(u=>u.UserName.ToLower()==loginRequestDTO.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (user == null || isValid == false)
            {
                return new LoginResponseDTO()
                {
                    User = null,
                    Token = ""
                };
            }
                //if user was found ,Generate JWT Token
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtTokenGenerator.GenerateToken(user,roles);


            ;  UserDTO userDTO = new UserDTO()
                {
                    Email = user.Email,
                    ID = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                };
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                User = userDTO,
                Token = token
            };

            return loginResponseDTO;

        

        }

        public async Task<string> Register(RegestirationrequestDTO regestirationrequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = regestirationrequestDTO.Email,
                Email = regestirationrequestDTO.Email,
                NormalizedEmail = regestirationrequestDTO.Email.ToUpper(),
                PhoneNumber = regestirationrequestDTO.PhoneNumber,


            };
            try
            {
                var result =await _userManager.CreateAsync(user, regestirationrequestDTO.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == regestirationrequestDTO.Email);
                    UserDTO userDTO = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber,
                    };
                    return "";
                    
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                        
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Error Encountered";

        }
    }
}
