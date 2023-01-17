using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Core.Models;
using Marcus.Umbraco.Model;

namespace Marcus.Umbraco.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly UmbracoHelper _umbracoHelper;

        public EmployeeController(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        [HttpGet]
        public ActionResult<List<Employee>> getEmployees()
        {
            var employees = _umbracoHelper.ContentAtXPath("//employee").Where(x => x.IsVisible());
            List<Employee> responseData = new List<Employee>();

            foreach(var employee in employees)
            {
                responseData.Add(new Employee
                {
                    fullName = employee.Value<string>("fullName"),
                    email = employee.Value<string>("email"),
                    dateOfBirth = employee.Value<DateTime>("dateOfBirth"),
                    age = employee.Value<int>("age"),
                }); ;
            }

            return Ok(responseData);
        }

        [HttpGet]
        [Route("{name}")]
        public ActionResult<Employee> getEmployeeById(string name)
        {
            var employees = _umbracoHelper.ContentAtXPath("//employee")
                                         .Where(x => x.IsVisible() && x.Name == name)
                                         .ToList();
            if(employees.Count > 0)
            {
                var employee = employees.First();
                var responseData = new Employee
                {
                    fullName = employee.Value<string>("fullName"),
                    email = employee.Value<string>("email"),
                    dateOfBirth = employee.Value<DateTime>("dateOfBirth"),
                    age = employee.Value<int>("age"),
                };
                return Ok(responseData);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
