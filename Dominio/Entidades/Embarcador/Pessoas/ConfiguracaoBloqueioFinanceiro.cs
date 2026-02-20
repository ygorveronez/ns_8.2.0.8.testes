namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_BLOQUEIO_FINANCEIRO", EntityName = "ConfiguracaoBloqueioFinanceiro", Name = "Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro", NameType = typeof(ConfiguracaoBloqueioFinanceiro))]
    public class ConfiguracaoBloqueioFinanceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CBF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CBF_DESCRICAO", TypeType = typeof(string), NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CBF_HABILITAR_REGRA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool HabilitarRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CBF_QUANTIDADE_DIAS_ATRASO_PAGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasAtrasoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CBF_QUANTIDADE_DIAS_NOVO_BLOQUEIO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasNovoBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }


    }
}
