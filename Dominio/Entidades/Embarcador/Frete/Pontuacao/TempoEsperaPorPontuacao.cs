namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TEMPO_ESPERA_POR_PONTUACAO", EntityName = "TempoEsperaPorPontuacao", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao", NameType = typeof(TempoEsperaPorPontuacao))]
    public class TempoEsperaPorPontuacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontuacaoInicial", Column = "TEP_PONTUACAO_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int PontuacaoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontuacaoFinal", Column = "TEP_PONTUACAO_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int PontuacaoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoEsperaEmMinutos", Column = "TEP_TEMPO_ESPERA_EM_MINUTOS", TypeType = typeof(int), NotNull = true)]
        public virtual int TempoEsperaEmMinutos { get; set; }

        public virtual string Descricao
        {
            get { return $"Tempo de espera (pontuação de {this.PontuacaoInicial} a {this.PontuacaoFinal})"; }
        }
    }
}
