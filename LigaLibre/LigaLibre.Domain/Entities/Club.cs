namespace LigaLibre.Domain.Entities
{
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int NumberOfPartners { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string StadiumName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Navegacion
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
