using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarAuto.EFCore.BaseEntity;

[Serializable]
public class BaseEntity
{
    public static readonly Guid USERUNSPECIFIEDID = Guid.Empty;

    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime ModifiedAt { get; set; }

    [ConcurrencyCheck]
    [Column("xmin", TypeName = "xid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public uint Xmin { get; set; }
}