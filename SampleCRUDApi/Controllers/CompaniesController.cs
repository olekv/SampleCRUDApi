using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleCRUDApi.Data;

namespace SampleCRUDApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        
        public CompaniesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<CompanyEntity>> Get([FromQuery] GetBindModel bindModel)
        {
            var query = _dbContext.Companies.AsQueryable();

            if (bindModel.OrderByAsc)
            {
                switch (bindModel.OrderBy)
                {
                    case GetBindModel.OrderByEnum.Id:
                        query = query.OrderBy(x => x.Id);
                        break;
                    case GetBindModel.OrderByEnum.Name:
                        query = query.OrderBy(x => x.Name);
                        break;
                }
            }
            else
            {
                switch (bindModel.OrderBy)
                {
                    case GetBindModel.OrderByEnum.Id:
                        query = query.OrderByDescending(x => x.Id);
                        break;
                    case GetBindModel.OrderByEnum.Name:
                        query = query.OrderByDescending(x => x.Name);
                        break;
                }
            }

            return await query.Skip(bindModel.Skip).Take(bindModel.Take).ToListAsync();
        }
        
        [HttpPost]
        public async Task<ActionResult> Post([Required, FromBody] PostBindModel bindModel)
        {
            var entity = new CompanyEntity
            {
                TenantId = (int)bindModel.TenantId,
                Industry = bindModel.Industry,
                Name = bindModel.Name,
                CompanyIconUri = bindModel.CompanyIconUri,
                Website = bindModel.Website,
                CompanyInfo = bindModel.CompanyInfo,
                Notes = bindModel.Notes,
                Progress = bindModel.Progress,
                OriginalEstimate = bindModel.OriginalEstimate,
                RemainingEstimate = bindModel.RemainingEstimate,
                CreatedBy = 1 // Let's pretend we take this from User's claims
            };
            
            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync();
            
            return new JsonResult(entity);
        }

        [HttpPut("id")]
        public async Task<ActionResult> Put(int id, [Required, FromBody] PutBindModel bindModel)
        {
            var entity = await _dbContext.Companies.FindAsync(id);

            if (entity == null) return NotFound();
            // Let's pretend we take this from User's claims
            if (entity.CreatedBy != 1) return Forbid();

            entity.TenantId = (int)bindModel.TenantId;
            entity.Industry = bindModel.Industry;
            entity.Name = bindModel.Name;
            entity.CompanyIconUri = bindModel.CompanyIconUri;
            entity.Website = bindModel.Website;
            entity.CompanyInfo = bindModel.CompanyInfo;
            entity.Notes = bindModel.Notes;
            entity.Progress = bindModel.Progress;
            entity.OriginalEstimate = bindModel.OriginalEstimate;
            entity.RemainingEstimate = bindModel.RemainingEstimate;
            entity.UpdatedBy = 1;
            
            await _dbContext.SaveChangesAsync();

            return new JsonResult(entity);
        }

        [HttpDelete("id")]
        public async Task<ActionResult> Delete(int id)
        {
            var entity = await _dbContext.Companies.FindAsync(id);

            if (entity == null) return NotFound();
            // Let's pretend we take this from User's claims
            if (entity.CreatedBy != 1 && entity.UpdatedBy != 1) return Forbid();

            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
            
            return NoContent();
        }
        
        public class GetBindModel
        {
            public enum OrderByEnum
            {
                Id,
                Name
            }

            public OrderByEnum OrderBy { get; set; }
            public bool OrderByAsc { get; set; }
            public int Skip { get; set; }
            public int Take { get; set; } = 10;
        }
        
        public class PostBindModel
        {
            [Required] public int? TenantId { get; set; }
            [StringLength(255)] public string Industry { get; set; }
            [Required, StringLength(255)] public string Name { get; set; }
            [StringLength(255)] public string CompanyIconUri { get; set; }
            [StringLength(255)] public string Website { get; set; }
            public string CompanyInfo { get; set; }
            public string Notes { get; set; }
        
            public float Progress { get; set; }
            public int OriginalEstimate { get; set; }
            public int RemainingEstimate { get; set; }
        }
        
        public class PutBindModel
        {
            [Required] public int? TenantId { get; set; }
            [StringLength(255)] public string Industry { get; set; }
            [Required, StringLength(255)] public string Name { get; set; }
            [StringLength(255)] public string CompanyIconUri { get; set; }
            [StringLength(255)] public string Website { get; set; }
            public string CompanyInfo { get; set; }
            public string Notes { get; set; }
        
            public float Progress { get; set; }
            public int OriginalEstimate { get; set; }
            public int RemainingEstimate { get; set; }
        }
    }
}