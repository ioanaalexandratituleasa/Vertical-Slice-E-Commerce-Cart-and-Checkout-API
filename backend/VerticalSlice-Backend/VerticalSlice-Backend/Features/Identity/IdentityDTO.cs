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

    public record LoginDTO(

        string Email = "",
        string Password = ""
    );

    public record LoginResponseDTO(
         string Toke = "",
         int UserID = 0,
         string FName = "",
         string Email = ""
    );
}
