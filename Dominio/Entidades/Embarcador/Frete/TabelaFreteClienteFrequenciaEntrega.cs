namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_FREQUENCIA_ENTREGA", EntityName = "TabelaFreteClienteFrequenciaEntrega", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao", NameType = typeof(TabelaFreteClienteFrequenciaEntrega))]
    public class TabelaFreteClienteFrequenciaEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFE_DIA_SEMANA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana DiaSemana { get; set; }

    }
}
