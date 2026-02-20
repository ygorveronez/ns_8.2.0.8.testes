namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_JUSTIFICATIVA_AUTORIZACAO_CARGA", EntityName = "JustificativaAutorizacaoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga", NameType = typeof(JustificativaAutorizacaoCarga))]
    public class JustificativaAutorizacaoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JAC_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "JAC_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "JAC_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "JAC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        public virtual bool Equals(JustificativaAutorizacaoCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
