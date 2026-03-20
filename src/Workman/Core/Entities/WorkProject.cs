using System.ComponentModel.DataAnnotations.Schema;

namespace Workman.Core.Entities
{
    [Table("wm_work_projects")]
    public class WorkProject
    {
        [Column("f_id")]
        public int Id { get; set; }

        [Column("f_name")]
        public string Name { get; set; } = string.Empty;

        [Column("f_is_archived")]
        public bool IsArchived { get; set; }

        [Column("f_created_time")]
        public DateTime CreatedTime { get; set; }

        [Column("f_archived_time")]
        public DateTime? ArchivedTime { get; set; }
    }
}
