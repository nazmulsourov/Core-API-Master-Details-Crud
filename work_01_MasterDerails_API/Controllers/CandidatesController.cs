using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using work_01_MasterDerails_API.Models;
using work_01_MasterDerails_API.Models.ViewModels;

namespace work_01_MasterDerails_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly CandidateDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CandidatesController(CandidateDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        [Route("GetSkills")]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            return await _context.Skills.ToListAsync();
        }
        [HttpGet]
        [Route("GetCandidates")]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
        {
            return await _context.Candidates.ToListAsync();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidateVM>>> GetCandidateSkills()
        {
            List<CandidateVM> candidateSkills=new List<CandidateVM>();
            var allCandidates = _context.Candidates.ToList();
            foreach (var candidate in allCandidates)
            {
                var skillList = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).Select(x => new Skill
                {
                    SkillId = x.SkillId,
                    SkillName=x.Skill.SkillName
                }).ToList();
                candidateSkills.Add(new CandidateVM
                {
                    CandidateId=candidate.CandidateId,
                    CandidateName=candidate.CandidateName,
                    BirthDate=candidate.BirthDate,
                    PhoneNo=candidate.PhoneNo,
                    Fresher=candidate.Fresher,
                    Picture=candidate.Picture,
                    SkillList=skillList.ToArray()
                });
            }
            return candidateSkills;
        }
        [HttpPost]
        public async Task<ActionResult<CandidateSkill>> PostCandidateSkill([FromForm] CandidateVM VM)
        {
            var skillItems = JsonConvert.DeserializeObject<Skill[]>(VM.SkillStringify);
            Candidate candidate = new Candidate
            {
                CandidateName=VM.CandidateName,
                BirthDate=VM.BirthDate,
                PhoneNo=VM.PhoneNo,
                Fresher=VM.Fresher
            };
            //image
            if(VM.PictureFile!=null)
            {
                var webroot = _env.WebRootPath;
                var fileName = DateTime.Now.Ticks.ToString() + Path.GetFileName(VM.PictureFile.FileName);
                var filePath = Path.Combine(webroot, "Images", fileName);

                FileStream fileStream=new FileStream(filePath,FileMode.Create);
                await VM.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                candidate.Picture = fileName;
            }
            foreach (var item in skillItems)
            {
                var candidateSkill = new CandidateSkill
                {
                    Candidate=candidate,
                    CandidateId=candidate.CandidateId,
                    SkillId=item.SkillId
                };
                _context.Add(candidateSkill);
            }
            await _context.SaveChangesAsync();
            return Ok(candidate);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateVM>> GetCandidates(int id)
        {
            Candidate candidate = await _context.Candidates.FindAsync(id);
            Skill[] skillList = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).Select(x => new Skill { SkillId = x.SkillId,SkillName=x.Skill.SkillName }).ToArray();

            CandidateVM candidateVM = new CandidateVM()
            {
                CandidateId=candidate.CandidateId,
                CandidateName=candidate.CandidateName,
                BirthDate=candidate.BirthDate,
                PhoneNo=candidate.PhoneNo,
                Fresher=candidate.Fresher,
                Picture=candidate.Picture,
                SkillList=skillList
            };
            return candidateVM;
        }
        [Route("Update")]
        [HttpPost]
        public async Task<ActionResult<CandidateSkill>> UpdateCandidateSkill([FromForm] CandidateVM vm)
        {
            var skillItems = JsonConvert.DeserializeObject<Skill[]>(vm.SkillStringify);
            Candidate candidate = _context.Candidates.Find(vm.CandidateId);
            candidate.CandidateName = vm.CandidateName;
            candidate.BirthDate = vm.BirthDate;
            candidate.PhoneNo = vm.PhoneNo;
            candidate.Fresher = vm.Fresher;

            //Image
            if(vm.PictureFile!= null)
            {
                var webroot = _env.WebRootPath;
                var fileName = DateTime.Now.Ticks.ToString() + Path.GetFileName(vm.PictureFile.FileName);
                var filePath = Path.Combine(webroot, "Images", fileName);

                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                await vm.PictureFile.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                candidate.Picture = fileName;
            }

            //Delete existing Skills
            var existingSkills = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
            foreach (var item in existingSkills)
            {
                _context.CandidateSkills.Remove(item);
            }

            //Add newly added skills
            foreach (var item in skillItems)
            {
                var candidateSkill = new CandidateSkill
                {
                    CandidateId = candidate.CandidateId,
                    SkillId = item.SkillId
                };
                _context.Add(candidateSkill);
            }

            _context.Entry(candidate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(candidate);
        }

        [Route("Delete/{id}")]
        [HttpPost]
        public async Task<ActionResult<CandidateSkill>> DeleteCandidateSkill(int id)
        {
            Candidate candidate = _context.Candidates.Find(id);
            var existingSkills = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
            foreach (var item in existingSkills)
            {
                _context.CandidateSkills.Remove(item);
            }
            _context.Entry(candidate).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return Ok(candidate);
        }

    }
}
