namespace BtsBlancApi.Models;

public enum TypeEvenement
{
    SalonProfessionnel,
    Conference,
    JourneesThematiques
}

public class Evenement
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Lieu { get; set; } = string.Empty;
    public TypeEvenement Type { get; set; }
}
