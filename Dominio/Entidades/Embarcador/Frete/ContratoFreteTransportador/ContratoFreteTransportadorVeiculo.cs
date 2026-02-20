namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO", EntityName = "ContratoFreteTransportadorVeiculo", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorClienteVeiculo", NameType = typeof(ContratoFreteTransportadorVeiculo))]
    public class ContratoFreteTransportadorVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFV_TIPO_PAGAMENTO_CONTRATO_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete TipoPagamentoContratoFrete { get; set; }

        public virtual string DescricaoTipoPagamentoContratoFrete
        {
            get
            {
                switch (TipoPagamentoContratoFrete)
                {
                    case ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Diaria:
                        return "Diário";
                    case ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Quinzena:
                        return "Quinzenal";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return Veiculo.Descricao;
            }
        }
    }
}
