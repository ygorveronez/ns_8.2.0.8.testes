using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TAXA_DESCARGA_AJUDANTES", EntityName = "ConfiguracaoTaxaDescargaAjudantes", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescargaAjudantes", NameType = typeof(ConfiguracaoTaxaDescargaAjudantes))]
    public class ConfiguracaoTaxaDescargaAjudantes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CTA_TIPO", TypeType = typeof(ConfiguracaoTaxaDescargaTipo), NotNull = false)]
        public virtual ConfiguracaoTaxaDescargaTipo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_QUANTIDADE_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_QUANTIDADE_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_QUANTIDADE_AJUDANTES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeAjudantes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoTaxaDescarga", Column = "CTD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga ConfiguracaoTaxaDescarga { get; set; }

      
    }
}
