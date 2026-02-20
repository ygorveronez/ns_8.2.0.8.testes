namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PONTUACAO_POR_TEMPO_ATIVIDADE", EntityName = "PontuacaoPorTempoAtividade", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade", NameType = typeof(PontuacaoPorTempoAtividade))]
    public class PontuacaoPorTempoAtividade : PontuacaoBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnoInicio", Column = "PTA_ANO_INICIO", TypeType = typeof(int), NotNull = true)]
        public virtual int AnoInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnoFim", Column = "PTA_ANO_FIM", TypeType = typeof(int), NotNull = true)]
        public virtual int AnoFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "PTA_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public override int Pontuacao { get; set; }

        public override string Descricao
        {
            get { return $"Tempo de atividade de {this.AnoInicio} a {this.AnoFim} ano{(this.AnoFim > 1 ? "s" : "")}"; }
        }
    }
}
