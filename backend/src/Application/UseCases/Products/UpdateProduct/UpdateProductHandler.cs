using Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Products.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<UpdateProductCommand> _validator;

    public UpdateProductHandler(IProductRepository productRepository, IValidator<UpdateProductCommand> validator)
    {
        _productRepository = productRepository;
        _validator = validator;
    }

    public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException("Produto não encontrado.");
        }

        var existingProduct = await _productRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingProduct != null && existingProduct.Id != request.Id)
        {
            throw new ValidationException("Já existe um produto com este nome.");
        }

        product.Name = request.Name;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product, cancellationToken);

        return new UpdateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
} 