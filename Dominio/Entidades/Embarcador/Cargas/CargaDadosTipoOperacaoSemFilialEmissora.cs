namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_DADOS_TIPO_OPERACAO_SEM_FILIAL_EMISSORA", EntityName = "CargaDadosTipoOperacaoSemFilialEmissora", Name = "Dominio.Entidades.Embarcador.Cargas.CargaDadosTipoOperacaoSemFilialEmissora", NameType = typeof(CargaDadosTipoOperacaoSemFilialEmissora))]
    public class CargaDadosTipoOperacaoSemFilialEmissora : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContratacaoCarga", Column = "CDF_CONTRATACAO_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga TipoContratacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContratacaoCargaSubContratacaoFilialEmissora", Column = "CDF_CONTRATACAO_CARGA_SUB_CONTRATACAO_FILIAL_EMISSORA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga TipoContratacaoCargaSubContratacaoFilialEmissora { get; set; }
    }
}
