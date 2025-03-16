using MediatR;

namespace Application.UseCases.Products.CreateProduct;

public class CreateProductCommand : IRequest<CreateProductResponse>
{
    public string Name { get; set; } = string.Empty;
}

public class CreateProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 