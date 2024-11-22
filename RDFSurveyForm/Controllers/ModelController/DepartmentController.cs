using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.DepartmentDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Services;
using System.Linq;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.AddDepartment.AddDepartmentHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.GetDepartment.GetDepartment;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.InActiveDepartment.DepartmentActiveHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.UpdateDepartment.UpdateDepartmentHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.UserActive.UserActiveHandler;

namespace RDFSurveyForm.Controllers.ModelController
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;
        private readonly IMediator _mediator;
        public DepartmentController(IUnitOfWork unitOfWork, StoreContext context, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mediator = mediator;

        }
        [HttpPost]
        [Route("AddNewDepartment")]
        public async Task<IActionResult> AddDepartment(AddDepartmentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("UpdateDepartment/{id:int}")]

        public async Task<IActionResult> UpdateDepartment([FromBody] UpdateDepartmentCommand command, [FromRoute] int id)
        {
            command.Id = id;
            try
            {
                var result = await _mediator.Send(command);

                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        

        [HttpPatch("SetIsActive/{id:int}")]
        public async Task<IActionResult> SetIsActive([FromRoute] int id)
        {
            try
            {
                var command = new DepartmentActiveCommand
                {
                    Id = id
                };

                var result = await _mediator.Send(command);

                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpGet]
        [Route("DepartmentListPagination")]
        public async Task<IActionResult> GetDepartment([FromQuery] GetDepartmentQuery query)
        {
            try
            {
                var users = await _mediator.Send(query);

                Response.AddPaginationHeader(

                   users.CurrentPage,
                   users.PageSize,
                   users.TotalCount,
                   users.TotalPages,
                   users.HasNextPage,
                   users.HasPreviousPage

                    );

                var results = new
                {
                    users,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasNextPage,
                    users.HasPreviousPage
                };

                var successResult = Result.Success(results);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("SyncDepartment")]
        public async Task<IActionResult> SyncDepartment([FromBody] AddDepartmentDto[] department)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong!") { StatusCode = 500 };
            }

            var duplicateList = new List<AddDepartmentDto>();
            var availableImport = new List<AddDepartmentDto>();
            var availableUpdate = new List<AddDepartmentDto>();
            var departmentNameEmpty = new List<AddDepartmentDto>();
            var deleteDepartment = new List<AddDepartmentDto>();

            
            foreach (var items in department)
            {
                if (department.Count(x => x.DepartmentName == items.DepartmentName && x.Id == items.Id) > 1)
                {
                    duplicateList.Add(items);
                }
                if (items.DepartmentName == string.Empty || items.DepartmentName == null)
                {
                    departmentNameEmpty.Add(items);
                    continue;
                }
                


                else
                {
                    var existingDepartment = await _unitOfWork.Department.GetByDepartmentNo(items.DepartmentNo);
                    if (existingDepartment != null)
                    {
                        bool hasChanged = false;

                        if (existingDepartment.DepartmentName != items.DepartmentName)
                        {
                            existingDepartment.DepartmentName = items.DepartmentName;
                            hasChanged = true;
                        }

                        if (hasChanged)
                        {
                            existingDepartment.IsActive = items.IsActive;
                            existingDepartment.EditedBy = User.Identity.Name;
                            existingDepartment.EditedAt = DateTime.Now;
                            existingDepartment.StatusSync = "New Update";
                            existingDepartment.SyncDate = DateTime.Now;

                            availableUpdate.Add(items);
                        }

                        if (!hasChanged)
                        {
                            existingDepartment.SyncDate = DateTime.Now;
                            existingDepartment.StatusSync = "No new update";
                        }
                    }
                    else
                    {
                        items.StatusSync = "New Added";
                        availableImport.Add(items);
                        await _unitOfWork.Department.AddDepartment(items);
                    }

                    deleteDepartment.Add(items);
                }

 

            }

            //var alldepartment = deleteDepartment.Where(x => x.DepartmentNo != null).ToList();

            var departmentlist = deleteDepartment.Select(x => x.DepartmentNo);

            var alldepartmentdelete = await _context.Department.ToListAsync();

            var differnce = alldepartmentdelete.Where(x => !departmentlist.Contains(x.DepartmentNo)).ToList();

            foreach (var dept in differnce)
            {
                _context.Remove(dept);
            }

            //var syncDelete = await _unitOfWork.Department.SyncDeleteCheck(deleteDepartment);

            var resultList = new
            {
                AvailableImport = availableImport,
                AvailableUpdate = availableUpdate,
                DuplicateList = duplicateList,
                DepartmentNameEmpty = departmentNameEmpty,
            };

            if (duplicateList.Count == 0 && departmentNameEmpty.Count == 0 )
            {
                await _unitOfWork.CompleteAsync();
                return Ok("Successfully updated and added!");
            }
            else
            {
                return BadRequest(resultList);
            }
        }
    }
}
