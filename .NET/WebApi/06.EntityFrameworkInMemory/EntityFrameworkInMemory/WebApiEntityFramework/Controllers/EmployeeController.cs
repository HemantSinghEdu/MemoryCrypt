using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using WebApiEntityFramework.Contracts.Repositories;
using WebApiEntityFramework.DatabaseContext;
using WebApiEntityFramework.Implementations.Repositories;
using WebApiEntityFramework.Models;

namespace WebApiEntityFramework.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Returns a list of all employees
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeResponseDto), 200)]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeRepository.GetAllAsync();
            var response = employees.Select(emp => (EmployeeResponseDto)emp).ToList();
            return Ok(response);
        }


        /// <summary>
        /// Returns details of a single employee based on input employeeId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
            EmployeeResponseDto employee = await _employeeRepository.GetById(id);
            if (employee == null)
            {
                _logger.LogInformation("Unable to find Employee with requested employee id", id);
                return NotFound();
            }

            return Ok(employee);
        }

        /// <summary>
        /// Adds a new employee to employee table
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddEmployee(EmployeeRequestDto employeeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestErrorMessages();
            }

            Employee employee = employeeRequest;
            employee.EmployeeId = Guid.NewGuid().ToString();
            employee.Addresses.ToList().ForEach(addr =>
            {
                addr.EmployeeId = employee.EmployeeId;
                addr.AddressId = Guid.NewGuid().ToString();
            });
            var isUnique = await IsRecordUnique(employee);
            if (isUnique)
            {
                await _employeeRepository.CreateAsync(employee);

                EmployeeResponseDto employeeResponse = employee;
                _logger.LogInformation("New Employee added with employee id", employee.EmployeeId);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employeeResponse.EmployeeId }, employeeResponse);
            }
            else
            {
                return Conflict("Employee with same details already exists");
            }
        }

        /// <summary>
        /// Update an employee's details based on employee id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmployee(string id, EmployeeRequestDto employeeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestErrorMessages();
            }

            var employeeToUpdate = await _employeeRepository.GetById(id);

            if (employeeToUpdate == null)
            {
                _logger.LogInformation("UpdateEmployee: Unable to find Employee with employee id", id);
                return NotFound();
            }

            var isUnique = await IsRecordUnique(employeeRequest, id);

            if (isUnique)
            {

                employeeToUpdate.FirstName = employeeRequest.FirstName;
                employeeToUpdate.LastName = employeeRequest.LastName;
                employeeToUpdate.Age = employeeRequest.Age;
                employeeToUpdate.EmailAddress = employeeRequest.EmailAddress;
                employeeToUpdate.Addresses = employeeRequest.Addresses.Select(addr => (Address)addr).ToList();

                employeeToUpdate.Addresses.ToList().ForEach(addr =>
                {
                    addr.EmployeeId = id;
                    addr.AddressId = Guid.NewGuid().ToString();
                });

                await _employeeRepository.UpdateAsync(employeeToUpdate);
            }
            else
            {
                return Conflict("Employee with same details already exists");
            }

            return NoContent();
        }

        /// <summary>
        /// Delete an employee by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Deleteemployee(string id)
        {
            var employeeToDelete = await _employeeRepository.GetById(id);

            if (employeeToDelete == null)
            {
                _logger.LogInformation("DeleteEmployee: Unable to find Employee with employee id", id);
                return NotFound();
            }

            await _employeeRepository.DeleteAsync(employeeToDelete);

            return NoContent();
        }

        private IActionResult BadRequestErrorMessages()
        {
            var errMsgs = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            _logger.LogInformation("Bad request received", errMsgs);
            return BadRequest(errMsgs);
        }

        /// <summary>
        /// This is a workaround function to check for uniqueness
        /// This is being used because adding constraints in Entity Framework 
        /// doesn't work for In-Memory Database, which is what we are using here
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<bool> IsRecordUnique(Employee employee, string id = "")
        {
            var existingEmployees = await _employeeRepository.GetAllAsync();
            var isAlreadyPresent = existingEmployees.Any(emp => emp.FirstName == employee.FirstName
                                    && emp.LastName == employee.LastName
                                    && emp.EmailAddress == employee.EmailAddress && emp.EmployeeId != id);
            return !isAlreadyPresent;
        }

    }
}