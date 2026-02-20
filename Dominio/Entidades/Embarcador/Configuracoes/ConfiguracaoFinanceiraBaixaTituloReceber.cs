using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_BAIXA_TITULO_RECEBER", EntityName = "ConfiguracaoFinanceiraBaixaTituloReceber", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber", NameType = typeof(ConfiguracaoFinanceiraBaixaTituloReceber))]
    public class ConfiguracaoFinanceiraBaixaTituloReceber : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CBT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CBT_GERAR_MOVIMENTO_AUTOMATICO_DIFERENCA_COTACAO_MOEDA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarMovimentoAutomaticoDiferencaCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracoesMoedas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_FINANCEIRA_BAIXA_TITULO_RECEBER_MOEDA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CBT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoFinanceiraBaixaTituloReceberMoeda", Column = "CBM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda> ConfiguracoesMoedas { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
