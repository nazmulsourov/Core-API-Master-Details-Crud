using System.ComponentModel.DataAnnotations.Schema;

namespace work_01_MasterDerails_API.Models
{
    public class CandidateSkill
    {
        public int CandidateSkillId { get; set; }
        [ForeignKey("Candidate")]
        public int CandidateId { get; set; }
        [ForeignKey("Skill")]
        public int SkillId { get; set; }
        //nev
        public virtual Candidate Candidate { get; set; } = default!;
        public virtual Skill Skill { get; set; } = default!;

    }
}
