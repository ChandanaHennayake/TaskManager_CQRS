
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Security;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Users.Queries.Login;
using TaskManager.Domain.Entities;
using Xunit;

namespace TaskManager.Tests.Features.Users.Queries;

public class LoginQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    private readonly LoginQueryHandler _handler;

    public LoginQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _userRepositoryMock = new Mock<IUserRepository>();

        _passwordHasherMock = new Mock<IPasswordHasher>();

        _unitOfWorkMock
            .Setup(x => x.Users)
            .Returns(_userRepositoryMock.Object);

        _handler = new LoginQueryHandler(
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@test.com",
            PasswordHash = "hashed-password"
        };

        var query = new LoginQuery(
            "admin",
            "Admin@123");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync(
                query.Username,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                query.Password,
                user.PasswordHash))
            .Returns(true);

        // Act

        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert

        result.Should().NotBeNull();

        result.Id.Should().Be(user.Id);

        result.Username.Should().Be(user.Username);

        result.Email.Should().Be(user.Email);

        _userRepositoryMock.Verify(x =>
            x.GetByUsernameAsync(
                query.Username,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _passwordHasherMock.Verify(x =>
            x.VerifyPassword(
                query.Password,
                user.PasswordHash),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange

        var query = new LoginQuery(
            "unknown",
            "password");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync(
                query.Username,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act

        Func<Task> action = async () =>
            await _handler.Handle(
                query,
                CancellationToken.None);

        // Assert

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("User 'unknown' not found");

        _passwordHasherMock.Verify(x =>
            x.VerifyPassword(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@test.com",
            PasswordHash = "hashed-password"
        };

        var query = new LoginQuery(
            "admin",
            "WrongPassword");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameAsync(
                query.Username,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                query.Password,
                user.PasswordHash))
            .Returns(false);

        // Act

        Func<Task> action = async () =>
            await _handler.Handle(
                query,
                CancellationToken.None);

        // Assert

        await action.Should()
            .ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid credentials");

        _passwordHasherMock.Verify(x =>
            x.VerifyPassword(
                query.Password,
                user.PasswordHash),
            Times.Once);
    }
}

