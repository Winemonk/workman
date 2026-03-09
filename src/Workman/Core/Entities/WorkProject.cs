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
    }
}
