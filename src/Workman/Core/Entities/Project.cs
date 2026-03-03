using System.ComponentModel.DataAnnotations.Schema;

namespace Workman.Core.Entities
{
    [Table("wm_projects")]
    internal class Project
    {
        [Column("f_id")]
        public long Id { get; set; }

        [Column("f_name")]
        public string Name { get; set; } = string.Empty;
    }
}
