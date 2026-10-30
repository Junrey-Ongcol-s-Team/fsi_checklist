// =====================================================================
// Ang AppDbContext: ang 'tulay' tali sa C# ug sa SQLite database (EF Core).
// Matag DbSet = usa ka table nga magamit sa C#. Ang ngalan sa column gi-convert ngadto sa snake_case (pananglitan DisplayName -> display_name).
// =====================================================================
using System.Text;
using fsi_checklist.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace fsi_checklist.Data
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<FormTemplate> FormTemplates => Set<FormTemplate>();
        public DbSet<FormSection> FormSections => Set<FormSection>();
        public DbSet<FormColumn> FormColumns => Set<FormColumn>();
        public DbSet<FormItem> FormItems => Set<FormItem>();
        public DbSet<FormChoice> FormChoices => Set<FormChoice>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Inspection> Inspections => Set<Inspection>();
        public DbSet<InspectionAnswer> InspectionAnswers => Set<InspectionAnswer>();
        public DbSet<AppSetting> AppSettings => Set<AppSetting>();
        public DbSet<SignatoryRole> SignatoryRoles => Set<SignatoryRole>();
        public DbSet<FormTemplateSignatory> FormTemplateSignatories => Set<FormTemplateSignatory>();
        public DbSet<InspectionSignatory> InspectionSignatories => Set<InspectionSignatory>();
        public DbSet<Attachment> Attachments => Set<Attachment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inspection>(e =>
            {
                e.HasOne(i => i.Subject).WithMany().HasForeignKey(i => i.SubjectId);
                e.HasOne(i => i.Template).WithMany().HasForeignKey(i => i.TemplateId);
            });

            modelBuilder.Entity<FormTemplateSignatory>()
                .HasKey(x => new { x.TemplateId, x.RoleCode });

            // schema.sql uses snake_case column names.
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
            }
        }

        private static string ToSnakeCase(string name)
        {
            var sb = new StringBuilder(name.Length + 4);
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0) sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
