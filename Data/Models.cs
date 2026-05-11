using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SMRS.Web.Data
{
    public class SmrsDbContext : DbContext
    {
        public SmrsDbContext(DbContextOptions<SmrsDbContext> options) : base(options) { }

        public DbSet<JobRole> JobRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionListing> PositionListings { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<AppPersonalInfo> AppPersonalInfos { get; set; }
        public DbSet<AppEducation> AppEducations { get; set; }
        public DbSet<AppWorkExperience> AppWorkExperiences { get; set; }
        public DbSet<AppChild> AppChildren { get; set; }
        public DbSet<AppEdTechLiteracy> AppEdTechLiteracies { get; set; }
        public DbSet<AppReference> AppReferences { get; set; }
        public DbSet<AppSocialActivity> AppSocialActivities { get; set; }
        public DbSet<Token> Tokens { get; set; }
        
        // Missing System Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionPosition> QuestionPositions { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<InboxEvent> InboxEvents { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        // Missing Application Child Tables
        public DbSet<AppPositionInterest> AppPositionInterests { get; set; }
        public DbSet<AppPositionSelection> AppPositionSelections { get; set; }
        public DbSet<AppInternship> AppInternships { get; set; }
        public DbSet<AppWorkshop> AppWorkshops { get; set; }
        public DbSet<ApplicationAnswer> ApplicationAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Map relationships
            modelBuilder.Entity<Position>()
                .HasOne(p => p.Department)
                .WithMany(d => d.Positions)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Position>()
                .HasOne(p => p.JobRole)
                .WithMany(r => r.Positions)
                .HasForeignKey(p => p.JobRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.PositionListing)
                .WithMany(l => l.Applications)
                .HasForeignKey(a => a.ListingId)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<AppPersonalInfo>()
                .HasOne(p => p.Application)
                .WithOne(a => a.PersonalInfo)
                .HasForeignKey<AppPersonalInfo>(p => p.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    [Table("job_roles")]
    public class JobRole
    {
        [Key, Column("job_role_id")]
        public int JobRoleId { get; set; }
        
        [Column("job_role_name")]
        [StringLength(100)]
        public string JobRoleName { get; set; } = "";
        
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public List<Position> Positions { get; set; } = new();
    }

    [Table("departments")]
    public class Department
    {
        [Key, Column("department_id")]
        public int DepartmentId { get; set; }

        [Column("department_name")]
        [StringLength(150)]
        public string DepartmentName { get; set; } = "";

        [Column("job_role_id")]
        public int? JobRoleId { get; set; }
        public JobRole? JobRole { get; set; }

        public List<Position> Positions { get; set; } = new();
    }

    [Table("positions")]
    public class Position
    {
        [Key, Column("position_id")]
        public int PositionId { get; set; }

        [Column("position_name")]
        [StringLength(200)]
        public string PositionName { get; set; } = "";

        [Column("department_id")]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        [Column("job_role_id")]
        public int? JobRoleId { get; set; }
        public JobRole? JobRole { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public List<PositionListing> Listings { get; set; } = new();
    }

    [Table("position_listings")]
    public class PositionListing
    {
        [Key, Column("listing_id")]
        public int ListingId { get; set; }

        [Column("position_id")]
        public int PositionId { get; set; }
        public Position? Position { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Open";

        [Column("posted_at")]
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        [Column("closed_at")]
        public DateTime? ClosedAt { get; set; }

        [Column("notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        public List<Application> Applications { get; set; } = new();
    }

    [Table("applications")]
    public class Application
    {
        [Key, Column("application_id")]
        public int ApplicationId { get; set; }

        [Column("applicant_email")]
        [StringLength(255)]
        public string ApplicantEmail { get; set; } = "";

        [Column("applicant_name")]
        [StringLength(200)]
        public string ApplicantName { get; set; } = "";

        [Column("listing_id")]
        public int? ListingId { get; set; }
        public PositionListing? PositionListing { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Opened";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("submitted_at")]
        public DateTime? SubmittedAt { get; set; }

        [Column("is_locked")]
        public bool IsLocked { get; set; } = false;

        public AppPersonalInfo? PersonalInfo { get; set; }
        public AppPositionInterest? PositionInterest { get; set; }
        public List<AppPositionSelection> PositionSelections { get; set; } = new();
        public List<AppEducation> Educations { get; set; } = new();
        public List<AppWorkExperience> WorkExperiences { get; set; } = new();
        public List<AppInternship> Internships { get; set; } = new();
        public List<AppWorkshop> Workshops { get; set; } = new();
        public List<AppChild> Children { get; set; } = new();
        public AppEdTechLiteracy? EdTechLiteracy { get; set; }
        public List<AppReference> References { get; set; } = new();
        public List<AppSocialActivity> SocialActivities { get; set; } = new();
    }

    [Table("app_personal_info")]
    public class AppPersonalInfo
    {
        [Key, Column("pi_id")]
        public int PiId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Column("title"), StringLength(10)]
        public string Title { get; set; } = "";

        [Column("first_name"), StringLength(100)]
        public string FirstName { get; set; } = "";

        [Column("family_name"), StringLength(100)]
        public string FamilyName { get; set; } = "";

        [Column("father_name"), StringLength(100)]
        public string FatherName { get; set; } = "";

        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }

        [Column("nationality"), StringLength(100)]
        public string Nationality { get; set; } = "";

        [Column("address"), StringLength(500)]
        public string Address { get; set; } = "";

        [Column("mobile_phone"), StringLength(30)]
        public string MobilePhone { get; set; } = "";

        [Column("email"), StringLength(255)]
        public string Email { get; set; } = "";

        [Column("marital_status"), StringLength(20)]
        public string MaritalStatus { get; set; } = "";

        [Column("maiden_name"), StringLength(100)]
        public string? MaidenName { get; set; }

        [Column("spouse_occupation"), StringLength(200)]
        public string? SpouseOccupation { get; set; }

        [Column("spouse_work_country"), StringLength(100)]
        public string? SpouseWorkCountry { get; set; }

        [Column("spouse_work_city"), StringLength(100)]
        public string? SpouseWorkCity { get; set; }

        [Column("number_of_children")]
        public int? NumberOfChildren { get; set; }

        [Column("self_description"), StringLength(1000)]
        public string? SelfDescription { get; set; }

        [Column("passport_photocopy"), StringLength(500)]
        public string? PassportPhotocopy { get; set; }

        [Column("lebanese_id_card"), StringLength(500)]
        public string? LebaneseIdCard { get; set; }

        [Column("passport_photo"), StringLength(500)]
        public string? PassportPhoto { get; set; }
    }

    [Table("app_education")]
    public class AppEducation
    {
        [Key, Column("edu_id")]
        public int EduId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Column("university_name")]
        [StringLength(200)]
        public string UniversityName { get; set; } = "";

        [Column("degree")]
        [StringLength(200)]
        public string Degree { get; set; } = "";

        [Column("major")]
        [StringLength(200)]
        public string Major { get; set; } = "";
    }

    [Table("app_work_experience")]
    public class AppWorkExperience
    {
        [Key, Column("exp_id")]
        public int ExpId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Column("institution")]
        [StringLength(200)]
        public string Institution { get; set; } = "";

        [Column("position_held")]
        [StringLength(200)]
        public string PositionHeld { get; set; } = "";

        [Column("reason_change")]
        [StringLength(500)]
        public string? ReasonChange { get; set; }
    }

    [Table("app_children")]
    public class AppChild
    {
        [Key, Column("child_id")]
        public int ChildId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Column("child_name")]
        [StringLength(100)]
        public string ChildName { get; set; } = "";

        [Column("child_age")]
        public int ChildAge { get; set; }

        [Column("child_school")]
        [StringLength(200)]
        public string? ChildSchool { get; set; }
    }

    [Table("app_edtech_literacy")]
    public class AppEdTechLiteracy
    {
        [Key, Column("lit_id")]
        public int LitId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Column("edtech_literacy")]
        public string? EdTechLiteracy { get; set; }

        [Column("computer_literacy")]
        public string? ComputerLiteracy { get; set; }
    }

    [Table("app_references")]
    public class AppReference
    {
        [Key, Column("ref_id")]
        public int RefId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Column("first_name")]
        [StringLength(100)]
        public string FirstName { get; set; } = "";

        [Column("last_name")]
        [StringLength(100)]
        public string LastName { get; set; } = "";

        [Column("mobile")]
        [StringLength(30)]
        public string Mobile { get; set; } = "";

        [Column("ref_email")]
        [StringLength(255)]
        public string RefEmail { get; set; } = "";

        [Column("position")]
        [StringLength(200)]
        public string Position { get; set; } = "";
    }

    [Table("app_social_activities")]
    public class AppSocialActivity
    {
        [Key, Column("act_id")]
        public int ActId { get; set; }

        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        [Column("activity_name")]
        [StringLength(200)]
        public string ActivityName { get; set; } = "";

        [Column("organization")]
        [StringLength(200)]
        public string Organization { get; set; } = "";

        [Column("role")]
        [StringLength(200)]
        public string Role { get; set; } = "";
    }

    [Table("tokens")]
    public class Token
    {
        [Key, Column("token_id")]
        public int TokenId { get; set; }

        [Column("token_value")]
        [StringLength(500)]
        public string TokenValue { get; set; } = "";

        [Column("applicant_email")]
        [StringLength(255)]
        public string ApplicantEmail { get; set; } = "";

        [Column("applicant_name")]
        [StringLength(200)]
        public string ApplicantName { get; set; } = "";

        [Column("listing_id")]
        public int? ListingId { get; set; }
        public PositionListing? PositionListing { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(1);

        [Column("is_used")]
        public bool IsUsed { get; set; } = false;
    }

    [Table("roles")]
    public class Role
    {
        [Key, Column("role_id")]
        public int RoleId { get; set; }
        [Column("role_name"), StringLength(50)]
        public string RoleName { get; set; } = "";
        [Column("description"), StringLength(200)]
        public string? Description { get; set; }
        
        public ICollection<User> Users { get; set; } = new List<User>();
    }

    [Table("users")]
    public class User
    {
        [Key, Column("user_id")]
        public int UserId { get; set; }
        [Column("username"), StringLength(100)]
        public string Username { get; set; } = "";
        [Column("password_hash"), StringLength(255)]
        public string PasswordHash { get; set; } = "";
        [Column("full_name"), StringLength(200)]
        public string FullName { get; set; } = "";
        [Column("email"), StringLength(255)]
        public string Email { get; set; } = "";
        [Column("role_id")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("levels")]
    public class Level
    {
        [Key, Column("level_id")]
        public int LevelId { get; set; }
        [Column("level_name"), StringLength(100)]
        public string LevelName { get; set; } = "";
        [Column("job_role_id")]
        public int JobRoleId { get; set; }
        public JobRole? JobRole { get; set; }
    }

    [Table("subjects")]
    public class Subject
    {
        [Key, Column("subject_id")]
        public int SubjectId { get; set; }
        [Column("subject_name"), StringLength(100)]
        public string SubjectName { get; set; } = "";
    }

    [Table("questions")]
    public class Question
    {
        [Key, Column("question_id")]
        public int QuestionId { get; set; }
        [Column("question_text"), StringLength(1000)]
        public string QuestionText { get; set; } = "";
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        [Column("created_by")]
        public int CreatedBy { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<QuestionPosition> QuestionPositions { get; set; } = new List<QuestionPosition>();
    }

    [Table("question_positions")]
    public class QuestionPosition
    {
        [Key, Column("qp_id")]
        public int QpId { get; set; }
        [Column("question_id")]
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        [Column("position_id")]
        public int PositionId { get; set; }
        public Position? Position { get; set; }
    }

    [Table("settings")]
    public class Setting
    {
        [Key, Column("setting_id")]
        public int SettingId { get; set; }
        [Column("setting_key"), StringLength(100)]
        public string SettingKey { get; set; } = "";
        [Column("setting_value"), StringLength(200)]
        public string SettingValue { get; set; } = "";
        [Column("description"), StringLength(300)]
        public string? Description { get; set; }
        [Column("updated_by")]
        public int UpdatedBy { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("audit_log")]
    public class AuditLog
    {
        [Key, Column("log_id")]
        public int LogId { get; set; }
        [Column("application_id")]
        public int? ApplicationId { get; set; }
        [Column("action"), StringLength(100)]
        public string Action { get; set; } = "";
        [Column("performed_by")]
        public int PerformedBy { get; set; }
        [Column("performed_at")]
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
        [Column("notes"), StringLength(500)]
        public string? Notes { get; set; }
    }

    [Table("inbox_events")]
    public class InboxEvent
    {
        [Key, Column("event_id")]
        public int EventId { get; set; }
        [Column("application_id")]
        public int ApplicationId { get; set; }
        [Column("event_type"), StringLength(50)]
        public string EventType { get; set; } = "";
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("acknowledged_by")]
        public int? AcknowledgedBy { get; set; }
    }

    [Table("email_templates")]
    public class EmailTemplate
    {
        [Key, Column("template_id")]
        public int TemplateId { get; set; }
        [Column("trigger_event"), StringLength(50)]
        public string TriggerEvent { get; set; } = "";
        [Column("recipient_type"), StringLength(20)]
        public string RecipientType { get; set; } = "";
        [Column("subject"), StringLength(300)]
        public string Subject { get; set; } = "";
        [Column("body_html")]
        public string BodyHtml { get; set; } = "";
        [Column("meeting_type"), StringLength(20)]
        public string? MeetingType { get; set; }
        [Column("updated_by")]
        public int? UpdatedBy { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("app_position_interest")]
    public class AppPositionInterest
    {
        [Key, Column("pi_id")]
        public int PiId { get; set; }
        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }
        [Column("applying_for"), StringLength(100)]
        public string ApplyingFor { get; set; } = "";
        
        [Column("educational_philosophy"), StringLength(500)]
        public string? EducationalPhilosophy { get; set; }
        
        [Column("applied_other"), StringLength(3)]
        public string AppliedOther { get; set; } = "No";
        
        [Column("other_school_details"), StringLength(300)]
        public string? OtherSchoolDetails { get; set; }

        [Column("employment_type"), StringLength(50)]
        public string EmploymentType { get; set; } = "";
        [Column("expected_salary"), StringLength(50)]
        public string ExpectedSalary { get; set; } = "";
        [Column("availability_date")]
        public DateTime? AvailabilityDate { get; set; }
        [Column("reason_school"), StringLength(1000)]
        public string? ReasonSchool { get; set; }
        [Column("commitment"), StringLength(1000)]
        public string? Commitment { get; set; }
    }

    [Table("app_position_selections")]
    public class AppPositionSelection
    {
        [Key, Column("sel_id")]
        public int SelId { get; set; }
        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }
        [Column("job_role_id")]
        public int? JobRoleId { get; set; }
        [Column("level_id")]
        public int? LevelId { get; set; }
        [Column("subject_id")]
        public int? SubjectId { get; set; }
        [Column("department_id")]
        public int? DepartmentId { get; set; }
        [Column("position_id")]
        public int? PositionId { get; set; }
    }

    [Table("app_internships")]
    public class AppInternship
    {
        [Key, Column("int_id")]
        public int IntId { get; set; }
        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }
        [Column("has_internships")]
        public bool HasInternships { get; set; } = false;
        [Column("internship_hours")]
        public int? InternshipHours { get; set; }
        [Column("internship_letter"), StringLength(500)]
        public string? InternshipLetter { get; set; }
    }

    [Table("app_workshops")]
    public class AppWorkshop
    {
        [Key, Column("ws_id")]
        public int WsId { get; set; }
        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }
        [Column("has_workshops")]
        public bool HasWorkshops { get; set; } = false;
        [Column("certificate"), StringLength(500)]
        public string? Certificate { get; set; }
    }

    [Table("application_answers")]
    public class ApplicationAnswer
    {
        [Key, Column("answer_id")]
        public int AnswerId { get; set; }
        [Column("application_id")]
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }
        [Column("question_id")]
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        [Column("answer_text"), StringLength(2000)]
        public string AnswerText { get; set; } = "";
    }
}
