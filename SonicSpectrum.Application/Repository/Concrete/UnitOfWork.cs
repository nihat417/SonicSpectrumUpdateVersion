using SonicSpectrum.Application.Repository.Abstract;
using SonicSpectrum.Persistence.Data;
using IdentityManagerServerApi.Services;
using SonicSpectrum.Application.Models;
using Microsoft.AspNetCore.Identity;
using SonicSpectrum.Domain.Entities;

namespace SonicSpectrum.Application.Repository.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly EmailConfiguration _emailConfiguration;

        private IAuthService? _authService;
        private IEmailService? _emailService;
        private IMusicSettingService? _musicSettingService;
        private IMessageService? _messageService;
        private IAccountService? _accountService;
        private IFollowService? _followService;

        public UnitOfWork(
            AppDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            JwtTokenService jwtTokenService,
            EmailConfiguration emailConfiguration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
            _emailConfiguration = emailConfiguration;
        }

        public IAuthService AuthService => _authService ??= new AuthService(_userManager, _roleManager, _jwtTokenService);
        public IEmailService EmailService => _emailService ??= new EmailService(_emailConfiguration);
        public IMusicSettingService MusicSettingService => _musicSettingService ??= new MusicSettingService(_context);
        public IMessageService MessageService => _messageService ?? (_messageService = new MessageService(_context));
        public IFollowService FollowService => _followService ??= new FollowService(_context);
        public IAccountService AccountService => _accountService ??= new AccountService(_context,_userManager);
        public UserManager<User> UserManager => _userManager;
        public RoleManager<IdentityRole> RoleManager => _roleManager;


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
