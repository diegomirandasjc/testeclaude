using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Products.SearchProducts;

public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, SearchProductsResponse>
{
    private readonly IProductRepository _productRepository;

    public SearchProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<SearchProductsResponse> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var result = await _productRepository.SearchAsync(request.SearchTerm, request.Page, request.PageSize, cancellationToken);

        return new SearchProductsResponse
        {
            Items = result.Items.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                CreatedAt = p.CreatedAt
            }).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalCount,
            TotalPages = result.TotalPages
        };
    }
} 