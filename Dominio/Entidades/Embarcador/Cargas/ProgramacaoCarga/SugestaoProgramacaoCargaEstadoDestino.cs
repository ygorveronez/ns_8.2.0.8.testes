namespace Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SUGESTAO_PROGRAMACAO_CARGA_ESTADO_DESTINO", EntityName = "SugestaoProgramacaoCargaEstadoDestino", Name = "Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino", NameType = typeof(SugestaoProgramacaoCargaEstadoDestino))]
    public class SugestaoProgramacaoCargaEstadoDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SugestaoProgramacaoCarga", Column = "SPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SugestaoProgramacaoCarga SugestaoProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }
    }
}
