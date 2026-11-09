using LSLibraryApp.Server.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mapster;
using LSLibraryApp.Server.Models.Entity;
using LSLibraryApp.Server.Services.Interface;

namespace LSLibraryApp.Server.Controllers
{
    [ApiController]
    [Route("topics")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TopicDTO>>> GetAllTopics()
        {
            var topics = await _topicService.GetAllTopics();
            var topicDtos = topics.Adapt<List<TopicDTO>>();
            return Ok(topicDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDTO>> GetTopicById(int id)
        {
            var topic = await _topicService.GetTopicById(id);
            if (topic is null)
            {
                return NotFound();
            }

            return Ok(topic.Adapt<TopicDTO>());
        }

        [HttpPost]
        public async Task<ActionResult<TopicDTO>> CreateTopic(TopicDTO topicDto)
        {
            try
            {
                var topic = topicDto.Adapt<Topic>();
                var createdTopic = await _topicService.CreateTopic(topic);
                return CreatedAtAction(nameof(GetTopicById), new { id = createdTopic.Id }, createdTopic.Adapt<TopicDTO>());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TopicDTO>> UpdateTopic(int id, TopicDTO topicDto)
        {
            if (topicDto.Id != id) // just make sure they're the same—why would they be different!?
            {
                return BadRequest(new { error = "ID mismatch" });
            }

            try
            {
                var topic = topicDto.Adapt<Topic>();
                var updatedTopic = await _topicService.UpdateTopic(topic);
                return Ok(updatedTopic.Adapt<TopicDTO>());
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { error = "The topic's version has since changed. Please try again with the latest copy of the data." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/usage")]
        public async Task<ActionResult<TopicUsageDTO>> GetTopicUsage(int id)
        {
            var usage = await _topicService.GetTopicUsageInformation(id);

            return Ok(usage.Adapt<TopicUsageDTO>());
        }
    }
}

