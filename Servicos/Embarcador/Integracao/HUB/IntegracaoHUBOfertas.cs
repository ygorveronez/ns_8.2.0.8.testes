using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Servicos.Embarcador.Integracao.HUB.Demanda;
using Servicos.Embarcador.Integracao.HUB.Infra;
using Servicos.Embarcador.Integracao.HUB.Tranportador;
using System;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.HUB
{
    public class IntegracaoHUBOfertas
    {

        #region Propriedades Protegidas
        protected readonly Repositorio.UnitOfWork _unitOfWork;
        protected readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        protected readonly Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas _repositorioCargaIntegracaoHUB;
        #endregion

        #region Construtores
        public IntegracaoHUBOfertas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) 
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _repositorioCargaIntegracaoHUB = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(_unitOfWork);
        }
        #endregion

        #region Métodos Públicos
        public async Task<bool> AdicionarIntegracaoHUB(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            if (tipoIntegracao == null)
                return false;
            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas integracaoExistente = await _repositorioCargaIntegracaoHUB.ConsultarIntegracaoCargaEnviadaHUB(carga.Codigo);

                if (integracaoExistente == null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas()
                    {
                        Carga = carga,
                        TipoIntegracao = tipoIntegracao,
                        TipoEnvioHUBOfertas = TipoEnvioHUBOfertas.EnvioDemandaOferta,
                        ProblemaIntegracao = "",
                        DataIntegracao = DateTime.Now,
                        SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                    };

                    await _repositorioCargaIntegracaoHUB.InserirAsync(cargaIntegracao);
                }
                else
                {
                    integracaoExistente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracaoExistente.DataIntegracao = DateTime.Now;
                    integracaoExistente.ProblemaIntegracao = "";
                    integracaoExistente.NumeroTentativas = 0;
                    await _repositorioCargaIntegracaoHUB.AtualizarAsync(integracaoExistente);

                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro($"Erro integração Transportador: {excecao.Message}", "HUBOfertas");
                return false;
            }

            return true;
        }

        public async Task<bool> GerarIntegracoes()
        {

            IntegracaoTransportadorHUBOferta integracaoTransportadorHUBOferta = new IntegracaoTransportadorHUBOferta(_unitOfWork, _tipoServicoMultisoftware);
            IntegracaoDemandaHUBOfertas integracaoDemandaHUBOfertas = new IntegracaoDemandaHUBOfertas(_unitOfWork, _tipoServicoMultisoftware);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracaoHUB in await _repositorioCargaIntegracaoHUB.BuscarIntegracoesHubPendente())
            {
                HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta();
                cargaIntegracaoHUB.NumeroTentativas++;
                cargaIntegracaoHUB.DataIntegracao = DateTime.Now;

                try
                {
                    switch (cargaIntegracaoHUB.TipoEnvioHUBOfertas)
                    {
                        case TipoEnvioHUBOfertas.EnvioTransportador:
                            httpRequisicaoResposta = await integracaoTransportadorHUBOferta.GerarIntegracaoTransportador(cargaIntegracaoHUB);
                            break;
                        case TipoEnvioHUBOfertas.EnvioDemandaOferta:
                        case TipoEnvioHUBOfertas.FinalizacaoDemandaOferta:
                        case TipoEnvioHUBOfertas.CancelamentoDemandaOferta:
                            httpRequisicaoResposta = await integracaoDemandaHUBOfertas.GerarIntegracaoHUBDemanda(cargaIntegracaoHUB);
                            break;
                        default:
                            throw new ServicoException("Tipo de integração inválida para HUB de ofertas.");
                    }

                    cargaIntegracaoHUB.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    cargaIntegracaoHUB.ProblemaIntegracao = "Aguardando Validação TNS Match.";
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
                    Servicos.Log.TratarErro($"Erro Processar Integracao HUB : {excecao.Message}", "HUBOfertas");
                }
                finally
                {
                    await _repositorioCargaIntegracaoHUB.AtualizarAsync(cargaIntegracaoHUB);
                }
            }

            return true;
        }
        #endregion

    }
}
