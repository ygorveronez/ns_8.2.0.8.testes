using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections;

namespace Servicos.Embarcador.Integracao.KMM
{
    partial class IntegracaoKMM
    {
        public void IntegrarLiberacaoPagamentoContratoFrete(EnumTipoPagamentoAutorizacaoPagamento tipoPagamento, Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao pagamentoContratoIntegracao)
        {
            Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao repositorioPagamentoContrato = new Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            try
            {
                pagamentoContratoIntegracao.DataIntegracao = DateTime.Now;
                pagamentoContratoIntegracao.NumeroTentativas++;

                Hashtable parameters = new Hashtable {
                    { "num_formulario", pagamentoContratoIntegracao.ContratoFrete.NumeroContrato.ToString()},
                    { "tipo_pagamento", this.ObterCodigoTipoPagamentoKMM(pagamentoContratoIntegracao.ContratoFrete.ConfiguracaoCIOT)},
                    { "tipo_saldo_adto", tipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento ? "0" : "1"},
                    { "data_emissao", pagamentoContratoIntegracao.ContratoFrete.DataEmissaoContrato.ToString("dd/MM/yyyy HH:mm:ss")},
                    { "cnpj_unidade_negocio", pagamentoContratoIntegracao.ContratoFrete.Carga?.Empresa?.CNPJ},
                    { "valor", tipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento ? pagamentoContratoIntegracao.ContratoFrete.ValorAdiantamento : pagamentoContratoIntegracao.ContratoFrete.SaldoAReceber},
                };

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "incluirLiberacao" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                pagamentoContratoIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                pagamentoContratoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pagamentoContratoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                pagamentoContratoIntegracao.ProblemaIntegracao = message;
            }
            repositorioPagamentoContrato.Atualizar(pagamentoContratoIntegracao);

        }
    }
}
