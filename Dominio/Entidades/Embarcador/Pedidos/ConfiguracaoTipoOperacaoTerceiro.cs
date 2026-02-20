namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_TERCEIRO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoTerceiro", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTerceiro", NameType = typeof(ConfiguracaoTipoOperacaoTerceiro))]
    public class ConfiguracaoTipoOperacaoTerceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_SOMAR_VALOR_PEDAGIO_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoSomarValorPedagioContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoSubtrairValePedagioDoContrato { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro", Column = "CTT_NAO_UTILIZAR_OPERADORA_CONFIGURADA_TRANSPORTADOR_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ADICIONAR_VALOR_CONTRATO_FRETE_COMO_ACRESCIMO_DESCONTO_PROXIMO_CONTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_ADICIONAR_VALOR_CONTRATO_FRETE_COMO_ACRESCIMO_DESCONTO_PROXIMO_CONTRATO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato { get; set; }
        public virtual string Descricao
        {
            get { return "Configurações do Terceiro"; }
        }
    }
}
