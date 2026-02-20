using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections;

namespace Servicos.Embarcador.Integracao.KMM
{
    partial class IntegracaoKMM
    {
        #region Métodos Globais

        public bool EstornarPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            var configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
            pagamentoRetorno.Data = DateTime.Now;
            pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
            pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;

            bool sucesso = false;
            string codigoRetorno = string.Empty;
            string jsonPost = string.Empty;
            string jsonResult = string.Empty;
            string mensagemErro = string.Empty;

            try
            {
                TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;

                string codigoExternoLancamentoID = repIntegracaoCodigoExterno.BuscarPorPagamentoMotoristaTMSETipo(pagamentoMotorista.Codigo, TipoCodigoExternoIntegracao.PagamentoMotoristaTMS, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                Hashtable body = new Hashtable
                {
                    { "tipo_pagamento", this.ObterCodigoTipoPagamentoKMM(tipoIntegracaoPagamentoMotorista) },
                    { "cnpj_filial", pagamentoMotorista.Carga.Empresa.CNPJ },
                    { "serie", "P" },
                    { "num_formulario", pagamentoMotorista.Numero.ToString() },
                    { "data_emissao", pagamentoMotorista.Data.ToString("yyyy-MM-dd") },
                    { "data_cancelamento", System.DateTime.Now.ToString("yyyy-MM-dd") },
                    { "motivo_cancelamento", "Cancelamento pagamento" },
                    { "lancto_id", codigoExternoLancamentoID }
                };

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "cancelarCT" },
                    { "parameters", body }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                sucesso = retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado ? true : false;
                mensagemErro = retWS.ProblemaIntegracao;
                jsonPost = retWS.jsonRequisicao;
                jsonResult = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                sucesso = false;
                mensagemErro = "Ocorreu uma falha ao realizar a integração da KMM";
            }

            pagamentoRetorno.CodigoRetorno = codigoRetorno;
            pagamentoRetorno.DescricaoRetorno = mensagemErro;
            pagamentoRetorno.Data = DateTime.Now;
            pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
            pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
            pagamentoRetorno.ArquivoRetorno = jsonResult;

            pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonPost, "json", unitOfWork);
            pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork);

            repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);

            return sucesso;
        }

        #endregion
    }
}
