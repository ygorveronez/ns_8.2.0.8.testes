namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PONTUACAO_POR_QUANTIDADE_CARGA_GANHA_COTACAO", EntityName = "PontuacaoPorQuantidadeCargaGanhaCotacao", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao", NameType = typeof(PontuacaoPorQuantidadeCargaGanhaCotacao))]
    public class PontuacaoPorQuantidadeCargaGanhaCotacao : PontuacaoBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PQG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeInicio", Column = "PQG_QUANTIDADE_INICIO", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeFim", Column = "PQG_QUANTIDADE_FIM", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "PQG_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public override int Pontuacao { get; set; }

        public override string Descricao
        {
            get { return $"Quantidade de cargas ganhas em cotação de {this.QuantidadeInicio} a {this.QuantidadeFim}"; }
        }
    }
}
