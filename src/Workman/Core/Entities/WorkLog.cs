using System.ComponentModel.DataAnnotations.Schema;

namespace Workman.Core.Entities
{
    [Table("wm_work_logs")]
    internal class WorkLog
    {
        [Column("f_id")]
        public long Id { get; set; }
        [Column("f_project_id")]
        public long ProjectId { get; set; }
        [Column("f_date")]
        public DateTime Date { get; set; }
        [Column("f_content")]
        public string Content { get; set; } = string.Empty;
        [Column("f_elapsed_time")]
        public float ElapsedTime { get; set; }
    }
}
