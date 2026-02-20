namespace Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SUGESTAO_PROGRAMACAO_CARGA_DESTINO", EntityName = "SugestaoProgramacaoCargaDestino", Name = "Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino", NameType = typeof(SugestaoProgramacaoCargaDestino))]
    public class SugestaoProgramacaoCargaDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SugestaoProgramacaoCarga", Column = "SPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SugestaoProgramacaoCarga SugestaoProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }
    }
}
