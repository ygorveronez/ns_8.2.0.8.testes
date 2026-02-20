using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoDemandaTransporte;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao.HUB.Base;
using System;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.HUB.Mensageria.Transportador
{
    public class RetornoTransportadorHUBOfertas
    {
        #region Propriedades Privadas
        protected readonly Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas _repositorioCargaIntegracaoHUB;
        #endregion

        #region Construtores
        public RetornoTransportadorHUBOfertas(Repositorio.UnitOfWork unitOfWork)
        {
            _repositorioCargaIntegracaoHUB = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);
        }
        #endregion

        #region Métodos Públicos
        public async Task<bool> ProcessarRetornoAsync(string jsonRetorno)
        {
            MensagemRetorno retornoDemanda = JsonConvert.DeserializeObject<MensagemRetorno>(jsonRetorno);
            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracaoHUB = await _repositorioCargaIntegracaoHUB.BuscarPorProtocolo(retornoDemanda.IdMensagemOrigem);

            if (cargaIntegracaoHUB == null)
                throw new ArgumentNullException("Mensagem não possui vinculo a nenhum transportador desse embarcador.");

            try
            {
                if (!retornoDemanda.Erros.IsNullOrEmpty() && retornoDemanda.Erros.Count > 0 && string.IsNullOrEmpty(retornoDemanda.Id))
                    throw new ServicoException(string.Join(",", retornoDemanda.Erros));

                if (cargaIntegracaoHUB != null)
                {
                    cargaIntegracaoHUB.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracaoHUB.IdVinculoDemanda = retornoDemanda.Id;
                    cargaIntegracaoHUB.ProblemaIntegracao = "Transportador Integrado com Sucesso!!!";
                }
            }
            catch (ServicoException excecao)
            {
                cargaIntegracaoHUB.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoHUB.ProblemaIntegracao = excecao.Message;

            }
            catch (Exception excecao)
            {
                cargaIntegracaoHUB.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoHUB.ProblemaIntegracao = "Problema ao tentar integrar com HUB de ofertas.";
                Log.TratarErro($"Erro no ProcessarRetornoDemandaAsync: {excecao.Message}", "HUBOfertas");
            }
            finally
            {
                await _repositorioCargaIntegracaoHUB.AtualizarAsync(cargaIntegracaoHUB);
            }

            return true;
        }

        #endregion

    }
}
