namespace Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_PROGRAMACAO_CARGA_REGIAO_DESTINO", EntityName = "ConfiguracaoProgramacaoCargaRegiaoDestino", Name = "Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino", NameType = typeof(ConfiguracaoProgramacaoCargaRegiaoDestino))]
    public class ConfiguracaoProgramacaoCargaRegiaoDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoProgramacaoCarga", Column = "CPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoProgramacaoCarga ConfiguracaoProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidades.Regiao Regiao { get; set; }
    }
}
