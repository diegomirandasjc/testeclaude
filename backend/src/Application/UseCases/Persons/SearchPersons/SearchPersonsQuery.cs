using MediatR;

namespace Application.UseCases.Persons.SearchPersons;

public class SearchPersonsQuery : IRequest<SearchPersonsResponse>
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
