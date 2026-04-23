using Application.DTOs;
using Application.Interfaces;
using Domain_layer.Entities;
using Domain_layer.Enums;
using Infrastructure.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        public Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = MockDatabase.Users.FirstOrDefault(u => u.Email == loginDto.Email && u.PasswordHash == loginDto.Password);
            if (user == null) return Task.FromResult<AuthResponseDto>(null!);

            return Task.FromResult(new AuthResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = "mock-jwt-token-for-phase-2"
            });
        }

        public Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            UserRole role = registerDto.Role == "Worker" ? UserRole.Worker : UserRole.Customer;
            User newUser;

            int newId = MockDatabase.Users.Any() ? MockDatabase.Users.Max(u => u.Id) + 1 : 1;

            if (role == UserRole.Worker)
            {
                newUser = new Worker { Id = newId, FullName = registerDto.FullName, Email = registerDto.Email, PasswordHash = registerDto.Password, PhoneNumber = registerDto.PhoneNumber, Role = role, IsVerified = false, CategoryId = 1 };
                MockDatabase.Workers.Add((Worker)newUser);
            }
            else
            {
                newUser = new Customer { Id = newId, FullName = registerDto.FullName, Email = registerDto.Email, PasswordHash = registerDto.Password, PhoneNumber = registerDto.PhoneNumber, Role = role };
                MockDatabase.Customers.Add((Customer)newUser);
            }

            MockDatabase.Users.Add(newUser);

            return Task.FromResult(new AuthResponseDto
            {
                Id = newUser.Id,
                FullName = newUser.FullName,
                Email = newUser.Email,
                Role = newUser.Role.ToString(),
                Token = "mock-jwt-token-for-phase-2"
            });
        }
    }
}
