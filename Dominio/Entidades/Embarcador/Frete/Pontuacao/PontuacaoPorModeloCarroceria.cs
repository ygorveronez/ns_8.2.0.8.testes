namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PONTUACAO_POR_MODELO_CARROCERIA", EntityName = "PontuacaoPorModeloCarroceria", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria", NameType = typeof(PontuacaoPorModeloCarroceria))]
    public class PontuacaoPorModeloCarroceria : PontuacaoBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloCarroceria", Column = "MCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculos.ModeloCarroceria ModeloCarroceria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "PMC_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public override int Pontuacao { get; set; }

        public override string Descricao
        {
            get { return $"Modelo de carroceria {this.ModeloCarroceria.Descricao}"; }
        }
    }
}
