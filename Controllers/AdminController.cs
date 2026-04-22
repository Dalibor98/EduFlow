using EduFlow.DTOs.Auth;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService) 
        {
            _adminService = adminService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-professor")]
        public async Task<IActionResult> RegisterProfessor(RegisterDto dto)
        {
            await _adminService.RegisterProfessorAsync(dto.FullName,dto.Email,dto.Password);
            return Ok("Registration successful.");
        }

    }
}
