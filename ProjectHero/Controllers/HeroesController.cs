using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectHero.Repository;
using ProjectHero.ServiceModals;

namespace ProjectHero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HeroesController : ControllerBase
    {
        private readonly IHeroRepository _heroRepository;

        public HeroesController(IHeroRepository heroRepository)
        {
            _heroRepository = heroRepository;
        }
        //Get All Heroes
        [HttpGet]
        public IActionResult GetAllHeroes()
        {
            var heroes = _heroRepository.GetAllHeroes();

            if (heroes.Count == 0)
            {
                return BadRequest("Heroes Not available...");
            }

            return Ok(heroes);
        }

        // Get Hero By Id
        [HttpGet("id/")]
        public IActionResult getHeroById([FromQuery]int id)
        {
            var hero = _heroRepository.GetHeroById(id);

            if (hero == null)
            {
                return NotFound("Hero with ID: " + id + " is Not Available....Try Another Id");
            }

            return Ok(hero);
        }

        //Post a Hero

        [HttpPost]
        public IActionResult PostHero([FromBody] HeroSM hero)
        {
             
            if (hero == null)
            {
                return BadRequest("Invalid Details...");
            }
            _heroRepository.AddHero(hero);

            return Created("Created Sucessfully", hero);
            //return LocalRedirect("/api/Heroes");

        }
        //Update Hero
        [HttpPut]
        public IActionResult UpdateHero([FromQuery] int id, [FromBody] HeroSM hero)
        {
            var isUpdated = _heroRepository.UpdateHero(id, hero);

            if (isUpdated)
            {
                return Ok("Hero With ID: " + id + " Updated Successfully");
            }
            else
            {
                return NotFound("Hero not found or update failed");
            }
        }
        [HttpDelete]
        public IActionResult DeleteHero([FromQuery] int id)
        {
            var isDeleted = _heroRepository.DeleteHero(id);
            if (isDeleted)
            {
                return Ok("Hero With ID: " + id + " Deleted Successfully");
            }
            return NotFound("Hero with ID: "+id+ " not found or Deletion failed...");
        }
    }
}
