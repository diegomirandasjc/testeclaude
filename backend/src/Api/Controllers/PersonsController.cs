using Application.UseCases.Persons.CreatePerson;
using Application.UseCases.Persons.GetPerson;
using Application.UseCases.Persons.SearchPersons;
using Application.UseCases.Persons.UpdatePerson;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<SearchPersonsResponse>> Search([FromQuery] SearchPersonsQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetPersonResponse>> Get(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new GetPersonQuery { Id = id });
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreatePersonResponse>> Create(CreatePersonCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(errors);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdatePersonResponse>> Update(Guid id, UpdatePersonCommand command)
    {
        try
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(errors);
        }
    }
}
