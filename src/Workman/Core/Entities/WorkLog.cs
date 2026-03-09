using System.ComponentModel.DataAnnotations.Schema;

namespace Workman.Core.Entities
{
    [Table("wm_work_logs")]
    public class WorkLog
    {
        [Column("f_id")]
        public int Id { get; set; }

        [Column("f_task_id")]
        public int TaskId { get; set; }

        [Column("f_content")]
        public string Content { get; set; } = string.Empty;

        [Column("f_date")]
        public DateTime Date { get; set; }

        [Column("f_elapsed_time")]
        public float ElapsedTime { get; set; }
    }
}
