using Application.UseCases.Customers.CreateCustomer;
using Application.UseCases.Customers.GetCustomer;
using Application.UseCases.Customers.SearchCustomers;
using Application.UseCases.Customers.UpdateCustomer;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<SearchCustomersResponse>> Search([FromQuery] SearchCustomersQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCustomerResponse>> Get(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new GetCustomerQuery { Id = id });
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreateCustomerResponse>> Create(CreateCustomerCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return BadRequest(errors);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateCustomerResponse>> Update(Guid id, UpdateCustomerCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("O ID na URL não corresponde ao ID no corpo da requisição.");
        }

        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return BadRequest(errors);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 