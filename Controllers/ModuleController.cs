using EduFlow.DTOs.Module;
using EduFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [Authorize(Roles = "Professor")]
        [HttpPost("{courseId}")]
        public async Task<IActionResult> CreateModule(int courseId,ModuleCreateDto _dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _moduleService.CreateModuleAsync(courseId,_dto.Title, _dto.Description, userId);

            return Ok("Module created successfully");
        }

        [Authorize(Roles = "Student")]
        [HttpGet("{courseId}")]

        public async Task<IActionResult> GetMyModules(int courseId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var modules = await _moduleService.GetMyModulesAsync(courseId, userId);

            var response = modules
                .Select(m => new ModuleResponseDto
                {
                    Id = m.Id,
                    CourseId = courseId,
                    Description = m.Description,
                    Title = m.Title,
                    CreatedAt = m.CreatedAt
                });

            return Ok(response);
        }
    }
}
