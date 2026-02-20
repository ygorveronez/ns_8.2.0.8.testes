namespace Dominio.Entidades.Embarcador.Simulacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_SIMULACAO_TRANSPORTADOR", EntityName = "RegrasSimulacaoTransportador", Name = "Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador", NameType = typeof(RegrasSimulacaoTransportador))]
    public class RegrasSimulacaoTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RST_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoSimulacao", Column = "RAS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoSimulacao RegrasAutorizacaoSimulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RST_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RST_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoSimulacao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoSimulacao Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RST_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoSimulacao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoSimulacao Juncao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

    }

}


