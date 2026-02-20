namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_CARREGAMENTO", EntityName = "TipoCarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.TipoCarregamento", NameType = typeof(TipoCarregamento))]
    public class TipoCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TCA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TCA_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TCA_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "TCA_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPadraoAgrupamentoCarga", Column = "TCA_TIPO_PADRAO_AGRUPAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoPadraoAgrupamentoCarga { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return this.Situacao ? "Ativo" : "Inativo"; }
        }
    }
}
