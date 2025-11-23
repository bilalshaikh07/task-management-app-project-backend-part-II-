namespace TaskMgmAPI.DTOfile
{
    public class AuthResponseDto
    {
        public int id { get; set; }

        public string Token { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? Designation { get; set; }
    }
}
