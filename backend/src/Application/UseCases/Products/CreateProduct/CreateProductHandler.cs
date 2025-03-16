using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateProductCommand> _validator;

    public CreateProductHandler(IProductRepository productRepository, IValidator<CreateProductCommand> validator)
    {
        _productRepository = productRepository;
        _validator = validator;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (await _productRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            throw new ValidationException("Já existe um produto com este nome.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.CreateAsync(product, cancellationToken);

        return new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            CreatedAt = product.CreatedAt
        };
    }
} 