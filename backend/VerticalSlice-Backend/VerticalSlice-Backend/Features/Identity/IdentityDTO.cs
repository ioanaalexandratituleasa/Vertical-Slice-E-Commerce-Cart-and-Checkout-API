namespace VerticalSlice_Backend.Features.Identity
{
    public record RegisterDTO(
        string FName = "",
        string LName = "",
        string Email ="",
        string Password = ""
    );

    public record RegisterResponseDTO(
        int UserID = 0,
        string FName = "",
        string LName = "",
        string Email = ""
    );
}
