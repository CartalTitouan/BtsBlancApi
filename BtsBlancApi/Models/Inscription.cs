namespace BtsBlancApi.Models;

public class Inscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int EvenementId { get; set; }
    public Evenement Evenement { get; set; } = null!;
    public DateTime InscritLe { get; set; } = DateTime.UtcNow;
}
