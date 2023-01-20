using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common;
using Marcus.Umbraco.Model;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;
using Marcus.Service;

namespace Marcus.Umbraco.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IContentService _contentService;

        public EmployeeController(UmbracoHelper umbracoHelper, IContentService contentService)
        {
            _umbracoHelper = umbracoHelper;
            _contentService = contentService;
        }

        [HttpGet]
        public ActionResult<List<EmployeeModel>> Get()
        {
            var employees = _umbracoHelper.ContentAtXPath("//employee").Where(x => x.IsVisible());
            List<EmployeeModel> responseData = new List<EmployeeModel>();

            foreach(var employee in employees)
            {
                responseData.Add(new EmployeeModel
                {
                    ContentName = employee.Name,
                    Id = employee.Id,
                    FullName = employee.Value<string>("fullName"),
                    Email = employee.Value<string>("email"),
                    DateOfBirth = employee.Value<DateTime>("dateOfBirth"),
                    Age = employee.Value<int>("age"),
                }); ;
            }

            return Ok(responseData);
        }

        [HttpGet]
        [Route("{name}")]
        public ActionResult<EmployeeModel> GetByName(string name)
        {
            var employees = _umbracoHelper.ContentAtXPath("//employee")
                                          .Where(x => x.IsVisible() && x.Name == name)
                                          .ToList();
            if(employees.Count > 0)
            { 
                var employee = employees.First();
                var responseData = new EmployeeModel
                {
                    ContentName = employee.Name,
                    Id = employee.Id,
                    FullName = employee.Value<string>("fullName"),
                    Email = employee.Value<string>("email"),
                    DateOfBirth = employee.Value<DateTime>("dateOfBirth"),
                    Age = employee.Value<int>("age"),
                };
                return Ok(responseData);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult<EmployeeModel> Post([FromBody] EmployeeModel employee)
        {
            try
            {
                //Validate
                var errors = Validate(employee);
                if(errors.Count > 0)
                {
                    return BadRequest(errors);
                }

                var employeesNode = _umbracoHelper.ContentSingleAtXPath("//schema");
                if (employeesNode == null)
                {
                    return NotFound("Parent Document Type was not found");
                }else
                {
                    IContent newEmployee = _contentService.Create(employee.FullName, employeesNode.Id, "employee", -1);
                    newEmployee.SetValue("fullName", employee.FullName);
                    newEmployee.SetValue("email", employee.Email);
                    newEmployee.SetValue("dateOfBirth", employee.DateOfBirth);
                    newEmployee.SetValue("age", employee.Age);
                    _contentService.SaveAndPublish(newEmployee);

                    return Ok(new EmployeeModel
                    {
                        Id = newEmployee.Id,
                        ContentName = newEmployee.Name,
                        FullName = newEmployee.GetValue<string>("fullName"),
                        Email = newEmployee.GetValue<string>("email"),
                        DateOfBirth = newEmployee.GetValue<DateTime>("dateOfBirth"),
                        Age = newEmployee.GetValue<int>("age"),
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult<EmployeeModel> Delete(int id)
        {
            try
            {
                var content = _contentService.GetById(id);
                if (content == null)
                {
                    return NotFound("Could not delete. Employee is not exist.");
                }
                else
                {
                    _contentService.Delete(content);
                    return Ok(content.Id);
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public ActionResult Update([FromBody] EmployeeModel newVersion)
        {
            try
            {
                //Validate
                var errors = Validate(newVersion);
                if(errors.Count > 0)
                {
                    return BadRequest(errors);
                }

                var content = _contentService.GetById(newVersion.Id);
                if(content == null)
                {
                    return NotFound("Could not update.Employee is not exist.");
                }
                else
                {
                    content.SetValue("fullName", newVersion.FullName);
                    content.SetValue("email", newVersion.Email);
                    content.SetValue("dateOfBirth", newVersion.DateOfBirth);
                    content.SetValue("age", newVersion.Age);
                    _contentService.SaveAndPublish(content);
                    return Ok();
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private List<string> Validate(EmployeeModel employee)
        {
            var errors = new List<string>();
            if(String.IsNullOrEmpty(employee.Email) || String.IsNullOrWhiteSpace(employee.Email))
            {
                errors.Add("Email is empty");
            }
            else if (!Validator.IsValidEmail(employee.Email))
            {
                errors.Add("Email is invalid");
            }
            if (!Validator.IsValidName(employee.FullName))
            {
                errors.Add("Name is empty");
            }
            if (!Validator.IsValidAge(employee.Age))
            {
                errors.Add("Age must greater than 0");
            }
            return errors;
        }
    }
}
