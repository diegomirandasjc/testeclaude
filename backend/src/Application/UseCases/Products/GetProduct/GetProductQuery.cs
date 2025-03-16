using MediatR;

namespace Application.UseCases.Products.GetProduct;

public class GetProductQuery : IRequest<GetProductResponse>
{
    public Guid Id { get; set; }
}

public class GetProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 