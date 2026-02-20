using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections;

namespace Servicos.Embarcador.Integracao.KMM
{
    partial class IntegracaoKMM
    {

        //integração errada
        //public void IntegrarPagamentoContratoFrete(EnumTipoPagamentoAutorizacaoPagamento tipoPagamento, Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao pagamentoContratoIntegracao)
        //{
        //    Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao repositorioPagamentoContrato = new Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao(_unitOfWork);
        //    Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
        //    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

        //    try
        //    {
        //        pagamentoContratoIntegracao.DataIntegracao = DateTime.Now;
        //        pagamentoContratoIntegracao.NumeroTentativas++;

        //        Hashtable parameters = new Hashtable {
        //            { "cnpj_filial", pagamentoContratoIntegracao.ContratoFrete.Carga?.Empresa?.CNPJ },
        //            { "num_formulario", pagamentoContratoIntegracao.ContratoFrete.NumeroContrato.ToString() },
        //            { "data_emissao", pagamentoContratoIntegracao.ContratoFrete.DataEmissaoContrato.ToString("yyyy-MM-dd") },
        //            { "data_amortizacao", pagamentoContratoIntegracao.ContratoFrete.DataAutorizacaoPagamento?.ToString("yyyy-MM-dd") },
        //            { "valor", tipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento ? pagamentoContratoIntegracao.ContratoFrete.ValorAdiantamento.ToString() : pagamentoContratoIntegracao.ContratoFrete.ValorSaldo.ToString() },
        //            { "tipo_pagamento", tipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento ? "1" : "2" },
        //            { "observacao", pagamentoContratoIntegracao.ContratoFrete.Observacao },
        //            { "autenticado", 1 },
        //        };

        //        Hashtable request = new Hashtable
        //        {
        //            { "module", "M217-IMPORTACAO" },
        //            { "operation", "inserirAmortizacao" },
        //            { "parameters", parameters }
        //        };

        //        var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

        //        pagamentoContratoIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
        //        pagamentoContratoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
        //    }
        //    catch (Exception excecao)
        //    {
        //        Log.TratarErro(excecao);

        //        pagamentoContratoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
        //        String message = excecao.Message;
        //        if (message.Length > 300)
        //        {
        //            message = message.Substring(0, 300);
        //        }
        //        pagamentoContratoIntegracao.ProblemaIntegracao = message;
        //    }
        //    repositorioPagamentoContrato.Atualizar(pagamentoContratoIntegracao);

        //}
    }
}
