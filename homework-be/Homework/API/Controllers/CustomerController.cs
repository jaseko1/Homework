using Homework.Gateway.API.Models;
using Homework.Gateway.API.Requests;
using Homework.Gateway.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Homework.Gateway.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerDto>> Get([FromQuery] GetCustomersRequest request)
        {
            try
            {
                var customerDto = await _customerService.Get(request);
                return Ok(customerDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetOne(string id)
        {
            try
            {
                var customerDto = await _customerService.GetOne(id);
                return Ok(customerDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateCustomerRequest customer)
        {
            try
            {
                await _customerService.Create(customer);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(UpdateCustomerRequest request)
        {
            try
            {
                await _customerService.Update(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{customerId}")]
        public async Task<ActionResult> Delete(string customerId)
        {
            try
            {
                await _customerService.Delete(customerId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
