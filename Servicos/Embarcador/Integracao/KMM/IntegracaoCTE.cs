using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Métodos Públicos

        public void IntegrarCTE(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repIntegracaoCte = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas++;

            try
            {
                Hashtable body = new Hashtable
                {
                    { "chave", integracao.CTe.Chave },
                };

                Hashtable request = new Hashtable
                {
                    { "module", "M325.INTEGRACAO" },
                    { "operation", "operationConsultaCteChave" },
                    { "parameters", body }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                integracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                integracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da KMM";
            }

            servicoArquivoTransacao.Adicionar(integracao, jsonRequisicao, jsonRetorno, "json");

            repIntegracaoCte.Atualizar(integracao);
        }
        #endregion

        #region Métodos Privados
        #endregion
    }
}