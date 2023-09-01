using System.ComponentModel.DataAnnotations;
namespace ProjectHero.DomainModal
{
    public class HeroDM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Power { get; set; }

    }
}
