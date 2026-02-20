namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PONTUACAO_POR_PESSOA_CLASSIFICACAO", EntityName = "PontuacaoPorPessoaClassificacao", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao", NameType = typeof(PontuacaoPorPessoaClassificacao))]
    public class PontuacaoPorPessoaClassificacao : PontuacaoBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PessoaClassificacao", Column = "PCL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.PessoaClassificacao PessoaClassificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "PPC_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public override int Pontuacao { get; set; }

        public override string Descricao
        {
            get { return $"Classificação de cliente {this.PessoaClassificacao.Descricao}"; }
        }
    }
}
