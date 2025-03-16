using Application.UseCases.Products.CreateProduct;
using Application.UseCases.Products.GetProduct;
using Application.UseCases.Products.SearchProducts;
using Application.UseCases.Products.UpdateProduct;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<SearchProductsResponse>> Search([FromQuery] SearchProductsQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductResponse>> Get(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new GetProductQuery { Id = id });
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> Create(CreateProductCommand command)
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
    public async Task<ActionResult<UpdateProductResponse>> Update(Guid id, UpdateProductCommand command)
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