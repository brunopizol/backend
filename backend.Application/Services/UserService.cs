using backend.Application.DTOs;
using backend.Domain.Entities;
using backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IAuthService _authService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserSecurityRepository _securityRepo;
        private readonly IAuditService _audit;

        public UserService(
            IUserRepository repository,
            IPasswordHasher passwordHasher,
            IAuthService authService,
            IUserSecurityRepository securityRepo,
            IAuditService audit)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _authService = authService;
            _securityRepo = securityRepo;
            _audit = audit;
        }


        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
        {
            if (await _repository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email already exists");

            var hash = _passwordHasher.Hash(dto.Password);

            var user = new User(dto.Name, dto.Email, hash, "User");

            await _repository.AddAsync(user);

            var security = new UserSecurity
            {
                UserId = user.Id,
                FailedLoginCount = 0,
                LockoutEnd = null,
                MfaEnabled = false,
                TOTPSecret = null
            };

            await _securityRepo.AddAsync(security);

            await _audit.LogAsync(new AuditLog
            {
                UserId = user.Id,
                Event = "user.created",
                Ip = "system",
                DeviceId = "system"
            });

            return MapToDto(user);
        }




        public async Task<UserResponseDto> GetByIdAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            return MapToDto(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var existingUser = await _repository.GetByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.Id != id)
                throw new InvalidOperationException("Email already in use");

            user.Update(dto.Name, dto.Email);
            await _repository.UpdateAsync(user);

            return MapToDto(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await _repository.ExistsAsync(id))
                throw new KeyNotFoundException("User not found");

            await _repository.DeleteAsync(id);
        }

        private UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Role = user.Role,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive
            };
        }
    }
}
