using Microsoft.EntityFrameworkCore;

namespace work_01_MasterDerails_API.Models
{
    public class CandidateDbContext:DbContext
    {
        public CandidateDbContext(DbContextOptions<CandidateDbContext> options):base(options)
        {
            
        }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData
            (
                new Skill { SkillId=1,SkillName="C#"},
                new Skill { SkillId=2,SkillName="Java"},
                new Skill { SkillId=3,SkillName="HTML"},
                new Skill { SkillId=4,SkillName="PHP"},
                new Skill { SkillId=5,SkillName="JavaScript"}
            );
        }
    }
}
