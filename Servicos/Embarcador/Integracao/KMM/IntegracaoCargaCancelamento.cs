using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        public void IntegrarCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente)
        {
            this.IntegrarCargaCancelamentoContratoFrete(integracaoPendente);
            this.IntegrarCargaCancelamentoCarga(integracaoPendente);
        }

        private void IntegrarCargaCancelamentoContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteTerceiro = repContratoFreteTerceiro.BuscarPorCarga(integracaoPendente.CargaCancelamento.Carga.Codigo);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            var configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            integracaoPendente.DataIntegracao = DateTime.Now;
            integracaoPendente.NumeroTentativas++;

            try
            {
                string codigoExternoLancamentoID = repIntegracaoCodigoExterno.BuscarPorContratoFreteETipo(contratoFreteTerceiro.Codigo, TipoCodigoExternoIntegracao.ContratoFrete, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                Hashtable body = new Hashtable
                {
                    { "tipo_pagamento", this.ObterCodigoTipoPagamentoKMM(contratoFreteTerceiro.ConfiguracaoCIOT) }, // 1 = empresa
                    { "cnpj_filial", integracaoPendente.CargaCancelamento.Carga.Filial.CNPJ },
                    { "serie", null },
                    { "num_formulario", contratoFreteTerceiro.NumeroContrato.ToString() },
                    { "data_emissao", contratoFreteTerceiro.DataEmissaoContrato.ToString("yyyy-MM-dd") },
                    { "data_cancelamento", integracaoPendente.CargaCancelamento.DataCancelamento?.ToString("yyyy-MM-dd") },
                    { "motivo_cancelamento", integracaoPendente.CargaCancelamento.JustificativaCancelamentoCarga.MotivoCancelamento },
                    { "lancto_id", codigoExternoLancamentoID }
                };

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "cancelarCT" },
                    { "parameters", body }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                integracaoPendente.SituacaoIntegracao = retWS.SituacaoIntegracao;
                integracaoPendente.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da KMM";
            }

            servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");

            repCargaIntegracao.Atualizar(integracaoPendente);
        }

        private void IntegrarCargaCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            var configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            integracaoPendente.DataIntegracao = DateTime.Now;
            integracaoPendente.NumeroTentativas++;

            try
            {
                Hashtable body = new Hashtable
                {
                    { "codigo_carga_embarcador", servicoCarga.ObterNumeroCarga(integracaoPendente.CargaCancelamento.Carga, configuracaoEmbarcador) }
                };

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "cancelarCarga" },
                    { "parameters", body }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                integracaoPendente.SituacaoIntegracao = retWS.SituacaoIntegracao;
                integracaoPendente.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da KMM";
            }

            servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");

            repCargaIntegracao.Atualizar(integracaoPendente);
        }
    }
}
