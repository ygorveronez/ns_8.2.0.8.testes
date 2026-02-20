namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_REJEICAO_AUDITORIA", EntityName = "MotivoRejeicaoAuditoria", Name = "Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria", NameType = typeof(MotivoRejeicaoAuditoria))]
    public class MotivoRejeicaoAuditoria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRA_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRA_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get {  return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
