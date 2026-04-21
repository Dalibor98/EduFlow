using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.DTOs.Module;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public ModuleController(IModuleRepository moduleRepository, ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
        {
            _moduleRepository = moduleRepository;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
        }


        [Authorize(Roles = "Professor")]
        [HttpPost("{courseId}")]
        public async Task<IActionResult> CreateModule(int courseId,ModuleCreateDto _dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var course = await _courseRepository.GetByIdAndProfessorAsync(courseId, userId);

            if (course == null)
            {
                return BadRequest("Course not found or acess denied.");
            }

            var module = new Module
            {
                Title = _dto.Title,
                Description = _dto.Description,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow,
            };

            await _moduleRepository.AddAsync(module);
            await _moduleRepository.SaveChangesAsync();
            return Ok("Module created successfully.");
        }

        [Authorize(Roles = "Student")]
        [HttpGet("{courseId}")]

        public async Task<IActionResult> GetMyModules(int courseId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var course = await _courseRepository.GetByIdAsync(courseId);

            if (course == null)
            {
                return BadRequest("Course not found.");
            }
           
            var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(userId, courseId);
            if (enrollment == null)
            {
                return BadRequest("Permission denied.");
            }

            
            var modules = await _moduleRepository.GetModulesByCourseIdAsync(courseId);

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
