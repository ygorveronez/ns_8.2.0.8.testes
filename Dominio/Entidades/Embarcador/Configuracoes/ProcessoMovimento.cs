using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROCESSO_MOVIMENTO", EntityName = "ProcessoMovimento", Name = "Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento", NameType = typeof(ProcessoMovimento))]
    public class ProcessoMovimento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Configuracoes.ProcessoMovimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PRM_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoProcessoMovimento", Column = "PRM_PROCESSO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento TipoProcessoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PRM_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PRM_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

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

        public virtual string DescricaoTipoProcessoMovimento
        {
            get
            {
                switch (this.TipoProcessoMovimento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.EmissaoCTENFSe:
                        return "Emissão CTE/NFSe";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoCTENFSe:
                        return "Cancela CTE/NFSe";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.AnulacaoCTENFSe:
                        return "Anulação CTE/NFSe";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.ValePedagioCTENFSe:
                        return "Vale Pedágio CTE/NFSe";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoValePedagioCTENFSe:
                        return "Cancela Vale Pedágio CTE/NFSe";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.EmissaodeSubContratacao:
                        return "Emissão SubContratação";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentodeSubContratacao:
                        return "Cancela SubContratação";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.PagamentoContratoSubContratacao:
                        return "Pagamento Contrato SubContratação";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.AdiantamentoTerceiros:
                        return "Adiantamento Terceiro";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentodaBaixadeAdiantamentoTerceiro:
                        return "Cancela Adiantamento Terceiro";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.Ocorrencias:
                        return "Ocorrencia";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentodeOcorrencias:
                        return "Cancela Ocorrencia";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.Ocorrencias999:
                        return "Ocorrencia 999";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoOcorrencias999:
                        return "Cancela Ocorrencia 999";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.GeracaodeFatura:
                        return "Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoFatura:
                        return "Cancela Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.DescontosFatura:
                        return "Desconto na Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoDescontosFatura:
                        return "Cancela Desconto na Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.AcrescimosFatura:
                        return "Acréscimo na Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoAcrescimoFatura:
                        return "Cancela Acréscimo na Fatura";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.BaixadeTitulosaReceber:
                        return "Baixa de Título";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.DescontoBaixadeTitulusaReceber:
                        return "Desconto na Baixa";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.AcrescimoBaixadeTitulosaReceber:
                        return "Acréscimo na Baixa";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoBaixadeTitulosaReceber:
                        return "Cancela Baixa de Título";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoAcrescimoBaixadeTitulosaReceber:
                        return "Cancela Acréscimo na Baixa";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoDescontoBaixadeTitulusaReceber:
                        return "Cancela Desconto na Baixa";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.GeracaodeParcelasdeNegociacao:
                        return "Geração de Novas Parcelas de Negociação";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.ValePedagioRecebidodeClienteAcerto:
                        return "Pedágio Recebido no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoPedagioRecebidodeClienteAcerto:
                        return "Cancela Pedágio Recebido no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.AbastecimentoPagoMotoristaAcerto:
                        return "Abastecimento do Motorista no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoAbastecimentoPagoMotoristaAcerto:
                        return "Cancela Abastecimento do Motorista no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.PagamentoPedagiopeloMotoristaAcerto:
                        return "Pagamento Pedágio do Motorista no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoPagamentoPedagioPeloMotoristaAcerto:
                        return "Cancela Pagamento Pedágio do Motorista no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.OutrasDespesasAcerto:
                        return "Outas Despesas no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoOutrasDespesasAcerto:
                        return "Cancela Outras Despesas no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.DescontodoAcerto:
                        return "Desconto no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelaDescontodoAcerto:
                        return "Cancela Desconto no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.BonificacaoMotoristaAcerto:
                        return "Bonificação do Motorista no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentodaBonificacaodoMotoristaAcerto:
                        return "Cancela Bonificação do Motorista no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.ComissaoAcerto:
                        return "Comissão no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelaComissaoAcerto:
                        return "Cancela Comissão no Acerto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.CancelamentoGeracaodeParcelasdeNegociacao:
                        return "Cancela de Percelas de Negociação";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.EmissaoDocumentoEntrada:
                        return "Emissão de Documento de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoProcessoMovimento.ExtornoDocumentoEntrada:
                        return "Extorno de Documento de Entrada";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(ProcessoMovimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
