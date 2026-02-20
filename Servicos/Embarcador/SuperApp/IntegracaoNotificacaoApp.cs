using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.SuperApp
{
    public class IntegracaoNotificacaoApp
    {
        private protected Repositorio.UnitOfWork _unitOfWork;

        public IntegracaoNotificacaoApp(Repositorio.UnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }

        #region Métodos Públicos

        public void Iniciar()
        {
            Processar();
        }

        public void GerarIntegracaoNotificacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoApp tipoNotificacaoApp)
        {
            Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repositorioMonitoramentoNotificacoesApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(_unitOfWork);
            Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp integracaoMonitoramentoNotificacoesApp = repositorioMonitoramentoNotificacoesApp.BuscarPorCargaETipo(carga.Codigo, tipoNotificacaoApp);

            if (integracaoMonitoramentoNotificacoesApp != null && integracaoMonitoramentoNotificacoesApp.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                return;

            Dominio.ObjetosDeValor.Embarcador.TorreControle.IntegracaoNotificacao parametrosIntegracaoNotificacao = new Dominio.ObjetosDeValor.Embarcador.TorreControle.IntegracaoNotificacao()
            {
                Carga = carga,
                TipoNotificacaoApp = tipoNotificacaoApp,
                IntegracaoMonitoramentoNotificacoesApp = integracaoMonitoramentoNotificacoesApp
            };

            GerarIntegracaoNotificacao(parametrosIntegracaoNotificacao);
        }

        public void GerarIntegracaoNotificacao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoApp tipoNotificacaoApp)
        {
            Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repositorioMonitoramentoNotificacoesApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(_unitOfWork);
            Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp integracaoMonitoramentoNotificacoesApp = repositorioMonitoramentoNotificacoesApp.BuscarPorChamadoETipo(chamado.Codigo, tipoNotificacaoApp);

            if (integracaoMonitoramentoNotificacoesApp != null && integracaoMonitoramentoNotificacoesApp.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                return;

            Dominio.ObjetosDeValor.Embarcador.TorreControle.IntegracaoNotificacao parametrosIntegracaoNotificacao = new Dominio.ObjetosDeValor.Embarcador.TorreControle.IntegracaoNotificacao()
            {
                IntegracaoMonitoramentoNotificacoesApp = integracaoMonitoramentoNotificacoesApp,
                TipoNotificacaoApp = tipoNotificacaoApp,
                Carga = chamado.Carga,
                Chamado = chamado
            };

            GerarIntegracaoNotificacao(parametrosIntegracaoNotificacao);
        }

        public void GerarIntegracaoNotificacao(Dominio.ObjetosDeValor.Embarcador.TorreControle.IntegracaoNotificacao parametrosIntegracaoNotificacao)
        {
            Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repositorioMonitoramentoNotificacoesApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoNotificacaoApp repositorioTipoOperacaoNotificacaoApp = new Repositorio.Embarcador.Pedidos.TipoOperacaoNotificacaoApp(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.Trizy);
            if (tipoIntegracao == null) return;

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp> listaTipoOperacaoNotificacaoApp = repositorioTipoOperacaoNotificacaoApp.BuscarPorTipoOperacao(parametrosIntegracaoNotificacao.Carga.TipoOperacao?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp notificacaoApp = listaTipoOperacaoNotificacaoApp.Where(x => x.NotificacaoApp.Tipo == parametrosIntegracaoNotificacao.TipoNotificacaoApp).Select(x => x.NotificacaoApp).FirstOrDefault();
            if (notificacaoApp == null) return;

            if (parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp == null)
                parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp = new Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp();

            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.DataIntegracao = DateTime.Now;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.NumeroTentativas = 0;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.ProblemaIntegracao = string.Empty;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.TipoIntegracao = tipoIntegracao;

            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.Carga = parametrosIntegracaoNotificacao.Carga;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.Motorista = repositorioCargaMotorista.BuscarPrimeiroMotoristaPorCarga(parametrosIntegracaoNotificacao.Carga.Codigo);
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.NotificacaoApp = notificacaoApp;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.TipoNotificacaoApp = parametrosIntegracaoNotificacao.TipoNotificacaoApp;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.GestaoDadosColetaDadosNFe = parametrosIntegracaoNotificacao.GestaoDadosColetaDadosNFe;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.Motivo = parametrosIntegracaoNotificacao.MotivoRejeicao;
            parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.Chamado = parametrosIntegracaoNotificacao.Chamado;

            if (parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp.Codigo == 0)
                repositorioMonitoramentoNotificacoesApp.Inserir(parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp);
            else
                repositorioMonitoramentoNotificacoesApp.Atualizar(parametrosIntegracaoNotificacao.IntegracaoMonitoramentoNotificacoesApp);
        }

        public void GerarIntegracaoNotificacaoCustom(int codigoMotorista, int codigoNotificacao, int codigoCarga = 0)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repositorioMonitoramentoNotificacoesApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.Trizy);
            if (tipoIntegracao == null) throw new ServicoException("Tipo de Integração Trizy não cadastrado.");

            Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp integracaoMonitoramentoNotificacoesApp = new Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp();

            integracaoMonitoramentoNotificacoesApp.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            integracaoMonitoramentoNotificacoesApp.DataIntegracao = DateTime.Now;
            integracaoMonitoramentoNotificacoesApp.NumeroTentativas = 0;
            integracaoMonitoramentoNotificacoesApp.ProblemaIntegracao = string.Empty;
            integracaoMonitoramentoNotificacoesApp.TipoIntegracao = tipoIntegracao;

            if (codigoCarga > 0)
                integracaoMonitoramentoNotificacoesApp.Carga = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = codigoCarga };
            integracaoMonitoramentoNotificacoesApp.Motorista = new Dominio.Entidades.Usuario() { Codigo = codigoMotorista };
            integracaoMonitoramentoNotificacoesApp.NotificacaoApp = new Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp() { Codigo = codigoNotificacao };
            integracaoMonitoramentoNotificacoesApp.TipoNotificacaoApp = TipoNotificacaoApp.Custom;

            repositorioMonitoramentoNotificacoesApp.Inserir(integracaoMonitoramentoNotificacoesApp);
        }
        #endregion

        #region Métodos Privados
        private void Processar()
        {
            Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy servicoIntegracaoTrzy = new Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy(_unitOfWork);
            Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repositorioIntegraaoNotificacaoApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            List<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp> notificacoesPendentes = repositorioIntegraaoNotificacaoApp.BuscarIntegracoesPendentes();

            foreach (Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp notificacao in notificacoesPendentes)
            {
                string mensagemProcessamento = string.Empty;

                notificacao.NumeroTentativas++;
                repositorioIntegraaoNotificacaoApp.Atualizar(notificacao);

                _unitOfWork.Start();
                try
                {
                    Dominio.Entidades.Usuario motorista = notificacao.Motorista ?? repositorioCargaMotorista.BuscarPrimeiroMotoristaPorCarga(notificacao.Carga?.Codigo ?? 0) ?? throw new ServicoException("Motorista não encontrado na base.");
                    if (string.IsNullOrEmpty(notificacao.NotificacaoApp.Titulo)) throw new ServicoException("Notificação sem Título.");
                    if (string.IsNullOrEmpty(notificacao.NotificacaoApp.Mensagem)) throw new ServicoException("Notificação sem Mensagem.");

                    //Integração Notificao AppTrizy.
                    mensagemProcessamento = servicoIntegracaoTrzy.EnviarNotificacaoSuperApp(notificacao, motorista.CPF, _unitOfWork);

                    if (mensagemProcessamento.Length > 200)
                        mensagemProcessamento = mensagemProcessamento.Substring(0, 200);
                    notificacao.SituacaoIntegracao = string.IsNullOrEmpty(mensagemProcessamento) ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                    notificacao.ProblemaIntegracao = string.IsNullOrEmpty(mensagemProcessamento) ? string.Empty : mensagemProcessamento;
                    repositorioIntegraaoNotificacaoApp.Atualizar(notificacao);
                    if (_unitOfWork.IsActiveTransaction())
                        _unitOfWork.CommitChanges();
                }
                catch (ServicoException ex)
                {
                    _unitOfWork.Rollback();
                    mensagemProcessamento = ex.Message;
                    Servicos.Log.TratarErro(ex);

                    notificacao.ProblemaIntegracao = mensagemProcessamento.Length > 200 ? mensagemProcessamento.Substring(0, 200) : mensagemProcessamento;
                    notificacao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    repositorioIntegraaoNotificacaoApp.Atualizar(notificacao);
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);

                    notificacao.ProblemaIntegracao = "Ocorreu uma falha genérica ao processar o evento.";
                    notificacao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    repositorioIntegraaoNotificacaoApp.Atualizar(notificacao);
                }
            }
        }
        #endregion
    }
    public class TagValor
    {
        public string Tag { get; set; }
        public string Valor { get; set; }
    }
}