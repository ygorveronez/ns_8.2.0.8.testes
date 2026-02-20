namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_PONTUACAO_TRANSPORTADOR", EntityName = "FechamentoPontuacaoTransportador", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador", NameType = typeof(FechamentoPontuacaoTransportador))]
    public class FechamentoPontuacaoTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "FPT_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Pontuacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPontuacao", Column = "FPN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPontuacao FechamentoPontuacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        public virtual string Descricao
        {
            get { return $"{FechamentoPontuacao.Descricao} - {Transportador.Descricao}"; }
        }
    }
}
