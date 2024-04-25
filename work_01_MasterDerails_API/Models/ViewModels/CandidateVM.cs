using System.ComponentModel.DataAnnotations.Schema;

namespace work_01_MasterDerails_API.Models.ViewModels
{
    public class CandidateVM
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = default!;
        [Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }
        public string PhoneNo { get; set; } = default!;
        public string? Picture { get; set; } = default!;
        public IFormFile? PictureFile { get; set; } = default!;
        public string? SkillStringify { get; set; } = default!;
        public bool Fresher { get; set; }
        public Skill[]? SkillList { get; set; } = default!;

    }
}
