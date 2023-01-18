using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common;
using Marcus.Umbraco.Model;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;

namespace Marcus.Umbraco.Controller
{
    [Route("api/[controller]")]
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
        public ActionResult<List<Employee>> Get()
        {
            var employees = _umbracoHelper.ContentAtXPath("//employee").Where(x => x.IsVisible());
            List<Employee> responseData = new List<Employee>();

            foreach(var employee in employees)
            {
                responseData.Add(new Employee
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
        public ActionResult<Employee> GetByName(string name)
        {
            var employees = _umbracoHelper.ContentAtXPath("//employee")
                                          .Where(x => x.IsVisible() && x.Name == name)
                                          .ToList();
            if(employees.Count > 0)
            { 
                var employee = employees.First();
                var responseData = new Employee
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
        public ActionResult<Employee> Post([FromBody] Employee employee)
        {
            try
            {
                //Validate

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

                    return Ok(new Employee
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
    }
}
