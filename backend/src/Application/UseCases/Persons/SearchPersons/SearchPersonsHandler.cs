using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Persons.SearchPersons;

public class SearchPersonsHandler : IRequestHandler<SearchPersonsQuery, SearchPersonsResponse>
{
    private readonly IPersonRepository _personRepository;

    public SearchPersonsHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<SearchPersonsResponse> Handle(SearchPersonsQuery request, CancellationToken cancellationToken)
    {
        var result = await _personRepository.SearchAsync(
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new SearchPersonsResponse
        {
            Items = result.Items.Select(p => new PersonDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                CreatedAt = p.CreatedAt
            }).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }
}
