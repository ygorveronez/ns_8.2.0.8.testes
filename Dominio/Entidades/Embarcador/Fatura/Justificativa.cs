using System;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_JUSTIFICATIVA", EntityName = "Justificativa", Name = "Dominio.Entidades.Embarcador.Fatura.Justificativa", NameType = typeof(Justificativa))]
    public class Justificativa : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.Justificativa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JUS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JUS_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoJustificativa", Column = "JUS_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa TipoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalidadeJustificativa", Column = "JUS_FINALIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa FinalidadeJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AplicacaoValorContratoFrete", Column = "JUS_APLICACAO_VALOR_CONTRATO_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete? AplicacaoValorContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoAutomatico", Column = "JUS_GERAR_MOVIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO_JUSTIFICATIVA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoUsoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_USO_JUSTIFICATIVA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoUsoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "JUS_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAvaria", Column = "MAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Avarias.MotivoAvaria MotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarDataAutorizacaoParaMovimentoAcrescimoDesconto", Column = "JUS_USAR_DATA_AUTORIZACA_PARA_MOVIMENTO_ACRESCIMO_DESCONTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarDataAutorizacaoParaMovimentoAcrescimoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JUS_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        #region Repom

        [NHibernate.Mapping.Attributes.Property(0, Column = "JUS_CODIGO_INTEGRACAO_REPOM", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CodigoIntegracaoRepom { get; set; }

        #endregion

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string DescricaoAplicacaoValorContratoFrete
        {
            get
            {
                switch (AplicacaoValorContratoFrete)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoAdiantamento:
                        return "No Adiantamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal:
                        return "No Total";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipoJustificativa
        {
            get
            {
                switch (this.TipoJustificativa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto:
                        return "Desconto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo:
                        return "Acréscimo";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoFinalidadeJustificativa
        {
            get
            {
                switch (FinalidadeJustificativa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.Todas:
                        return "Todas";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.Fatura:
                        return "Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.TitulosPagar:
                        return "Títulos a Pagar";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.TitulosReceber:
                        return "Títulos a Receber";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.ContratoFrete:
                        return "Contrato de Frete";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.AcertoViagemMotorista:
                        return "Acerto de Viagem - Justificativas do Motorista";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.AcertoViagemEmbarcador:
                        return "Acerto de Viagem - Justificativas do Embarcador";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.AcertoViagemOutrasDespesas:
                        return "Acerto de Viagem - Outras Despesas";
                    case ObjetosDeValor.Embarcador.Enumeradores.FinalidadeJustificativa.PendenciaMotorista:
                        return "Pendência Motorista";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual bool Equals(Justificativa other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
