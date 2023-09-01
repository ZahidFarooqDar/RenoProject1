using ProjectHero.ServiceModals;

namespace ProjectHero.Repository
{
    public interface IHeroRepository
    {
        List<HeroSM> GetAllHeroes();
        HeroSM? GetHeroById(int id);
        HeroSM? AddHero(HeroSM hero);
        bool UpdateHero(int id, HeroSM updatedHero);
        bool DeleteHero(int id);
    }
}