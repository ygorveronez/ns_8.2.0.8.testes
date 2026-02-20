namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PONTUACAO_POR_QUANTIDADE_CARGA", EntityName = "PontuacaoPorQuantidadeCarga", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga", NameType = typeof(PontuacaoPorQuantidadeCarga))]
    public class PontuacaoPorQuantidadeCarga : PontuacaoBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PQC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeInicio", Column = "PQC_QUANTIDADE_INICIO", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeFim", Column = "PQC_QUANTIDADE_FIM", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "PQC_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public override int Pontuacao { get; set; }

        public override string Descricao
        {
            get { return $"Quantidade de cargas de {this.QuantidadeInicio} a {this.QuantidadeFim}"; }
        }
    }
}
