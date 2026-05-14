namespace VerticalSlice_Backend.Features.Identity
{
    public interface IIdentityRepository
    {
        Task<RegisterResponseDTO> RegisterAsync(RegisterDTO data);
        Task<LoginResponseDTO> LoginAsync(LoginDTO data);
    }
}
