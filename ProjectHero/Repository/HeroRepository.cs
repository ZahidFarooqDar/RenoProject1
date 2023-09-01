using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectHero.Data;
using ProjectHero.DomainModal;
using ProjectHero.ServiceModals;

namespace ProjectHero.Repository
{
    public class HeroRepository : IHeroRepository  //It is Implementing Interface
    {
        private readonly ProjectHeroContext _context;
        private readonly IMapper _mapper;

        public HeroRepository(IMapper mapper ,ProjectHeroContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        /*---------------------------------------AutoMapperUsages------------------------------------
         -----------------------------------_mapper.Map<List<HeroSM>>(_herosDM)----------------------------------
        ----------------------------------------------------------------------------------------------------------------
        -------------_mapper.Map<What we need what actually return type is>(what we have available);-------------
        -----------------------------------------So we need _mapper----------------------------------- 
         */
        public List<HeroSM> GetAllHeroes()
        {
            var _herosDM = _context.Hero.ToList();  //We need to return data as HeroSM format but we have data in HeroDM format 
                                                    //We need to have our data in HeroSM Format so we use _mapper
            //return _herosDM; //Only when we are returning HeroDM
            var _herosSM = _mapper.Map<List<HeroSM>>(_herosDM); //Here _mapper maps data of HeroDM with HeroSM
            return _herosSM;                                    //Data here is in required format (heroSM)
           /* var heroes = _context.Hero.ToList();
            return heroes;*/

            // return _mapper.Map<List<HeroSM>>(_context.Hero.ToList());
        }
        public HeroSM GetHeroById(int id)
        {
            var _heroDM = _context.Hero.Find(id);
            if (_heroDM == null)
            {
                return null;
            }
            var _heroSM = _mapper.Map<HeroSM>(_heroDM); //Mapper here also Maps Data of heroDM and Changes into HeroSM the required format
            return _heroSM;
        }
        public HeroSM? AddHero(HeroSM hero)
        {
            var _heroDM = _mapper.Map<HeroDM>(hero); //Here we fetch data in HeroDM and Add it into database
                                                     //We need to return HeroSM not HeroDM
                                                     //So _mapper comes into place
            _context.Hero.Add(_heroDM);
            if(_context.SaveChanges() > 0)
            {
                return _mapper.Map<HeroSM>(_heroDM); //We here return HeroSM by mapping it with HeroDM
            }
            return null;
        }
        public bool UpdateHero(int id ,HeroSM updatedHero)
        {
            var existingHero = _context.Hero.Find(id);
            if (existingHero == null)
            {
                return false;
            }
            //---------------------- Null-coalescing operator (??)
            /*
             * update properties of an existing Hero object with values from an updated Hero object. 
             * The null-coalescing operator is used to provide a default value when an expression evaluates to null.
             * {
                 "id": 0,
                 "firstName": null,
                 "lastName": null,
                 "email": null,
                 "bestMovie": null
                } 
            Using Null Values will not effect the previous data, so will not get updated
             */
            /*existingHero.FirstName = updatedHero.FirstName ?? existingHero.FirstName;
            existingHero.FirstName = existingHero.FirstName ?? updatedHero.LastName;
            existingHero.City = updatedHero.City ?? existingHero.City;
            existingHero.Gender = updatedHero.Gender ?? existingHero.Gender;
            existingHero.DOB = updatedHero.DOB ?? existingHero.DOB;
            existingHero.Image = updatedHero.Image ?? existingHero.Image;
            existingHero.Power = updatedHero.Power ?? existingHero.Power;*/
            //-------------------  Using AUTOMAPPER  ------------------------
            //var _heroDM = _mapper.Map<Hero>(updatedHero); // creates a new Hero object with properties from updatedHero.

            //-------------------------------_mapper.Map<HeroSM, HeroDM>(updatedHero, existingHero)---------------------------------------
            /*The Map method from AutoMapper is used to perform the mapping operation.
             * The first generic parameter(HeroSM) specifies the source type.
             * The second generic parameter(HeroDM) specifies the destination type.
             * The method takes two arguments:
             * updatedHero: This is the source object that holds the data you want to map.
             * existingHero: This is the destination object that you want to map the data to.*/

                     var _heroDM = _mapper.Map<HeroSM, HeroDM>(updatedHero, existingHero);
                    //creates a new HeroDM object, mapping from HeroSM to HeroDM based on a specific configuration.
                    //var _heroDM = _mapper.Map(updatedHero, existingHero); //updates existingHero with non-null properties from updatedHero.
            try
            {
                _heroDM.Id = id;
                _context.Hero.Update(_heroDM);
                if (_context.SaveChanges() > 0)
                    return true;

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeroExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return false;
        }
        public bool DeleteHero(int id)
        {
            var hero = _context.Hero.Find(id);
            if (hero == null)
            {
                return false;
            }

            _context.Hero.Remove(hero);
            _context.SaveChanges();

            return true;
        }
        private bool HeroExists(int id)
        {
            return (_context.Hero?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}

