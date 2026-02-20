namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_BAIXA_TITULO_RECEBER_MOEDA", EntityName = "ConfiguracaoFinanceiraBaixaTituloReceberMoeda", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda", NameType = typeof(ConfiguracaoFinanceiraBaixaTituloReceberMoeda))]
    public class ConfiguracaoFinanceiraBaixaTituloReceberMoeda: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CBM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoFinanceiraBaixaTituloReceber", Column = "CBT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber ConfiguracaoFinanceiraBaixaTituloReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CBM_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_ACRESCIMO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_DESCONTO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaDesconto { get; set; }
    }
}
