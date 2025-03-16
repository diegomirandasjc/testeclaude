using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Products.GetProduct;

public class GetProductHandler : IRequestHandler<GetProductQuery, GetProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException("Produto não encontrado.");
        }

        return new GetProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            CreatedAt = product.CreatedAt
        };
    }
} 