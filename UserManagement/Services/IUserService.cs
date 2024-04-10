namespace UserManagement.Services
{
    using System.Collections.Generic;
    using UserManagement.Models;

    public interface IUserService
    {
        // AuthenticateResponse Authenticate(AuthenticateRequest model);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> Create(User user);
        Task<User> Update(User user);
        Task Delete(int id);
    }
}
