using MediatR;

namespace Application.UseCases.Products.UpdateProduct;

public class UpdateProductCommand : IRequest<UpdateProductResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UpdateProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 