using backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Domain.Interfaces
{
    public interface IUserSecurityRepository
    {
        Task<UserSecurity> GetAsync(Guid userId);
        Task UpdateAsync(UserSecurity sec);
        Task AddAsync(UserSecurity sec);

    }
}
