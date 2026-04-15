using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.User;

namespace StudentParliamentSystem.UseCases.Departments.Retrieve.Members;

public class RetrieveDepartmentMembersHandler
{
    private readonly IUserRepository _userRepository;

    public RetrieveDepartmentMembersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserPreview>> HandleAsync(RetrieveDepartmentMembers query)
    {
        return await _userRepository.RetrieveByDepartmentAsync(query.DepartmentId, query.PageNumber, query.PageSize, query.Query);
    }
}