namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_FATURA_MODELO_TIPO_MOVIMENTO", EntityName = "ConfiguracaoFinanceiraFaturaModeloTipoMovimento", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento", NameType = typeof(ConfiguracaoFinanceiraFaturaModeloTipoMovimento))]
    public class ConfiguracaoFinanceiraFaturaModeloTipoMovimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Financeira Fatura";
            }
        }
    }
}
