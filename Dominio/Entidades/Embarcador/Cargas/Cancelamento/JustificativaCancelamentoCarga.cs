namespace Dominio.Entidades.Embarcador.Cargas.Cancelamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CANCELAMENTO_JUSTIFICATIVA", EntityName = "JustificativaCancelamentoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga", NameType = typeof(JustificativaCancelamentoCarga))]
    public class JustificativaCancelamentoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TCJ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCJ_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCJ_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCJ_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCJ_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "TCJ_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual bool Equals(JustificativaCancelamentoCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
