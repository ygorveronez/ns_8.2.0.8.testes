namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_PONTUACAO_TRANSPORTADOR_REGRA", EntityName = "FechamentoPontuacaoTransportadorRegra", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra", NameType = typeof(FechamentoPontuacaoTransportadorRegra))]
    public class FechamentoPontuacaoTransportadorRegra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FTR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "FPT_PONTUACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Pontuacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPontuacaoTransportador", Column = "FPT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPontuacaoTransportador FechamentoPontuacaoTransportador { get; set; }
    }
}
