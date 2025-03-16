using Application.UseCases.Cities.CreateCity;
using Application.UseCases.Cities.GetCity;
using Application.UseCases.Cities.SearchCities;
using Application.UseCases.Cities.UpdateCity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<SearchCitiesResponse>> Search([FromQuery] SearchCitiesQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCityResponse>> Get(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new GetCityQuery { Id = id });
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreateCityResponse>> Create(CreateCityCommand command)
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
    public async Task<ActionResult<UpdateCityResponse>> Update(Guid id, UpdateCityCommand command)
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