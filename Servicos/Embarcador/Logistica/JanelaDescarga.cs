using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public class JanelaDescarga
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.Entidades.Usuario _usuario;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoTMS;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;

        #endregion Atributos

        #region Construtores

        public JanelaDescarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _usuario = usuario;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _configuracaoTMS = configuracaoTMS;
        }

        #endregion Construtores

        #region Métodos Públicos

        public string NaoComparecimentoCargaDevolvida(List<int> codigosJanelaDescarregamento, TipoGatilhoNotificacao tipoGatilhoNotificacao, bool naoComparecimento = false, bool ignorarSituacao = false)
        {
            _unitOfWork.Start();

            if (codigosJanelaDescarregamento.Count == 0)
                throw new ServicoException("Nenhum registro selecionado.");

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargasJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCodigos(codigosJanelaDescarregamento);

            if (cargasJanelaDescarregamento.Count == 0)
                throw new ServicoException("Não foi possível encontrar o(s) registro(s).");

            if (!ignorarSituacao)
            {
                if (cargasJanelaDescarregamento.Exists(obj => obj.Situacao == SituacaoCargaJanelaDescarregamento.DescarregamentoFinalizado))
                    throw new ServicoException("Um ou mais agendamentos selecionados já foram finalizados.");

                if (naoComparecimento && cargasJanelaDescarregamento.Exists(obj => obj.Situacao != SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento))
                    throw new ServicoException("Um ou mais agendamentos selecionados ainda não foram confirmados.");

                if (!naoComparecimento && cargasJanelaDescarregamento.Exists(obj => obj.Situacao != SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento) && cargasJanelaDescarregamento.Exists(obj => obj.Situacao != SituacaoCargaJanelaDescarregamento.ChegadaConfirmada))
                    throw new ServicoException("Um ou mais agendamentos selecionados ainda não foram confirmados.");

                if (cargasJanelaDescarregamento.Exists(obj => obj.Situacao == SituacaoCargaJanelaDescarregamento.NaoComparecimento))
                    throw new ServicoException("Um ou mais agendamentos selecionados já foram ajustados como não comparecido.");

                if (cargasJanelaDescarregamento.Exists(obj => obj.Situacao == SituacaoCargaJanelaDescarregamento.CargaDevolvida))
                    throw new ServicoException("Um ou mais agendamentos selecionados já foram ajustados como carga devolvida.");
            }

            List<int> codigosCargas = cargasJanelaDescarregamento.Select(obj => obj.Carga.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentosColeta = repositorioAgendamentoColeta.BuscarPorCargas(codigosCargas);
            List<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet> agendamentosPallet = repositorioAgendamentoPallet.BuscarPorCargas(codigosCargas);

            if (agendamentosColeta.Exists(obj => obj.Cancelado) || agendamentosPallet.Exists(o => o.Situacao == SituacaoAgendamentoPallet.Cancelado))
                throw new ServicoException("Um ou mais agendamentos selecionados já foram cancelados.");

            Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(_unitOfWork, _configuracaoTMS, _auditado);
            string mensagemCancelamento = string.Empty;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            bool possuiIntegracaoSAD = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.SAD);
            List<(string URL, int CodigoCentroDescarregamento)> urlsSad = new List<(string URL, int CodigoCentroDescarregamento)>();

            if (possuiIntegracaoSAD)
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoSAD repositorioSAD = new Repositorio.Embarcador.Configuracoes.IntegracaoSAD(_unitOfWork);
                List<int> codigosCentrosDescarregamento = cargasJanelaDescarregamento.Select(obj => obj.CentroDescarregamento.Codigo).Distinct().ToList();
                urlsSad = repositorioSAD.BuscarURLsCancelarAgendaPorCentrosDescarregamento(codigosCentrosDescarregamento);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = agendamentosColeta.Find(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo);
                Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet = agendamentosPallet.Find(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo);

                bool permiteCancelarCarga = !naoComparecimento || PermiteCancelarCarga(cargaJanelaDescarregamento.InicioDescarregamento);

                if (permiteCancelarCarga)
                {
                    bool gerarIntegracoes = PermitirGerarIntegracoes(cargaJanelaDescarregamento, urlsSad, possuiIntegracaoSAD, agendamentoColeta != null);

                    mensagemCancelamento = SolicitarCancelamentoCarga(cargaJanelaDescarregamento, agendamentoColeta, agendamentoPallet, gerarIntegracoes, mensagemCancelamento);
                }

                (string mensagemAuditoriaPadrao, SituacaoCargaJanelaDescarregamento situacaoCargaJanelaDescarregamento, SituacaoAgendamentoColeta situacaoAgendamentoColeta) = ObterSituacoesEMotivo(naoComparecimento, permiteCancelarCarga);

                AlterarSituacaoAgendamento(cargaJanelaDescarregamento, agendamentoColeta, agendamentoPallet, situacaoAgendamentoColeta, mensagemAuditoriaPadrao);

                servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, situacaoCargaJanelaDescarregamento);

            }

            _unitOfWork.CommitChanges();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = agendamentosColeta.Find(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo);

                System.Threading.Tasks.Task.Factory.StartNew(() => new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_unitOfWork.StringConexao, _usuario, _auditado).EnviarEmails(agendamentoColeta, tipoGatilhoNotificacao));
            }

            return mensagemCancelamento;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CancelarAgendamento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool gerarIntegracoes, string motivoCancelamento)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            if (agendamentoColeta != null && agendamentoColeta.EtapaAgendamentoColeta == EtapaAgendamentoColeta.Emissao)
                throw new ServicoException("Não é possível cancelar o agendamento na etapa atual.");

            if ((agendamentoColeta == null) || !agendamentoColeta.ApenasGerarPedido)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                {
                    Carga = carga,
                    GerarIntegracoes = gerarIntegracoes,
                    MotivoCancelamento = !string.IsNullOrWhiteSpace(motivoCancelamento) ? motivoCancelamento : "Cancelamento por Agendamento de Coleta",
                    TipoServicoMultisoftware = _tipoServicoMultisoftware,
                    Usuario = _usuario
                };

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, _configuracaoTMS, _unitOfWork);
                Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);

                return cargaCancelamento;
            }

            if (repositorioCargaPedido.BuscarCargaAtualPorPedido(agendamentoColeta.Pedido.Codigo) != null)
                throw new ServicoException("Já existe uma carga com esse pedido. Não é possível cancelar o pedido");

            agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.NFeCancelada;
            agendamentoColeta.Pedido.SituacaoPedido = SituacaoPedido.Cancelado;
            agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Cancelado;

            repositorioPedido.Atualizar(agendamentoColeta.Pedido);
            repositorioAgendamentoColeta.Atualizar(agendamentoColeta);

            if (agendamentoPallet != null)
            {
                agendamentoPallet.Situacao = SituacaoAgendamentoPallet.Cancelado;
                agendamentoPallet.DataCancelamento = DateTime.Now;

                repositorioAgendamentoPallet.Atualizar(agendamentoPallet);
            }

            return null;
        }

        public void SolicitarCancelamentoPorTempoLimiteNaoComparecimento()
        {
            if (!ObterConfiguracaoJanelaCarregamento().NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento && (ObterConfiguracaoJanelaCarregamento().TempoPermitirReagendamentoHoras != 0))
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargasJanelaDescarregamentoCancelar = repositorioCargaJanelaDescarregamento.BuscarNaoComparecimentosParaCancelamento(ObterConfiguracaoJanelaCarregamento().TempoPermitirReagendamentoHoras, limiteRegistros: 5);

            if (cargasJanelaDescarregamentoCancelar.Count == 0)
                return;

            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(_unitOfWork);

            List<int> codigosCargas = cargasJanelaDescarregamentoCancelar.Select(o => o.Carga.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentosColeta = repositorioAgendamentoColeta.BuscarPorCargas(codigosCargas);
            List<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet> agendamentosPallet = repositorioAgendamentoPallet.BuscarPorCargas(codigosCargas);

            for (int i = 0; i < cargasJanelaDescarregamentoCancelar.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamentoCancelar = cargasJanelaDescarregamentoCancelar[i];
                try
                {
                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = agendamentosColeta.Find(obj => obj.Carga.Codigo == cargaJanelaDescarregamentoCancelar.Carga.Codigo);
                    Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet = agendamentosPallet.Find(obj => obj.Carga.Codigo == cargaJanelaDescarregamentoCancelar.Carga.Codigo);

                    string mensagemPadrao = "Cancelado por tempo limite em situação Não comparecimento";

                    CancelarAgendamento(agendamentoColeta, agendamentoPallet, cargaJanelaDescarregamentoCancelar.Carga, true, mensagemPadrao);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro($"CargaJanelaDescarregamento: {cargaJanelaDescarregamentoCancelar.Codigo};" + excecao.Message, "SolicitarCancelamentoPorTempoLimiteNaoComparecimento");
                }
            }
        }

        #endregion

        #region Métodos Privados        

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private bool PermiteCancelarCarga(DateTime dataReferencia)
        {
            bool permiteCancelar = !ObterConfiguracaoJanelaCarregamento().NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento;

            if (!permiteCancelar)
            {
                int tempoPermitirReagendamentoHoras = ObterConfiguracaoJanelaCarregamento().TempoPermitirReagendamentoHoras;

                permiteCancelar = tempoPermitirReagendamentoHoras != 0 && dataReferencia.AddHours(tempoPermitirReagendamentoHoras) <= DateTime.Now;
            }

            return permiteCancelar;
        }

        private bool PermitirGerarIntegracoes(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, List<(string URL, int CodigoCentroDescarregamento)> urlsSad, bool possuiIntegracaoSAD, bool existeAgendamentoColeta)
        {
            bool gerarIntegracoes = !possuiIntegracaoSAD;
            string urlSADCentroDescarregamento = (from obj in urlsSad where obj.CodigoCentroDescarregamento == cargaJanelaDescarregamento.CentroDescarregamento.Codigo select obj.URL).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(urlSADCentroDescarregamento))
                urlSADCentroDescarregamento = (from obj in urlsSad where obj.CodigoCentroDescarregamento == 0 select obj.URL).FirstOrDefault();

            if (existeAgendamentoColeta && !string.IsNullOrWhiteSpace(urlSADCentroDescarregamento))
                gerarIntegracoes = true;

            return gerarIntegracoes;
        }

        private string SolicitarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet, bool gerarIntegracoes, string mensagemCancelamento)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = CancelarAgendamento(agendamentoColeta, agendamentoPallet, cargaJanelaDescarregamento.Carga, gerarIntegracoes, string.Empty);

            if (cargaCancelamento != null && cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                mensagemCancelamento += $"{cargaCancelamento.MensagemRejeicaoCancelamento}";

            return mensagemCancelamento;
        }

        private void AlterarSituacaoAgendamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet, SituacaoAgendamentoColeta situacaoAgendamentoColeta, string mensagemAuditoriaPadrao)
        {
            if (agendamentoColeta != null)
            {
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, agendamentoColeta, $"Agendamento Coleta {mensagemAuditoriaPadrao}.", _unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaJanelaDescarregamento, $"Agendamento Coleta {mensagemAuditoriaPadrao}.", _unitOfWork);
                agendamentoColeta.Situacao = situacaoAgendamentoColeta;
                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
            }

            if (agendamentoPallet != null)
            {
                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(_unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, agendamentoPallet, $"Agendamento Pallet {mensagemAuditoriaPadrao}.", _unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaJanelaDescarregamento, $"Agendamento Pallet {mensagemAuditoriaPadrao}.", _unitOfWork);

                agendamentoPallet.EtapaAgendamentoPallet = EtapaAgendamentoPallet.Acompanhamento;
                agendamentoPallet.Situacao = SituacaoAgendamentoPallet.Cancelado;

                agendamentoPallet.DataCancelamento = DateTime.Now;

                repositorioAgendamentoPallet.Atualizar(agendamentoPallet);
            }
        }

        private (string, SituacaoCargaJanelaDescarregamento, SituacaoAgendamentoColeta) ObterSituacoesEMotivo(bool naoComparecimento, bool permiteCancelarCarga)
        {
            string mensagemAuditoriaPadrao = "Cancelado por devolução de carga";
            SituacaoCargaJanelaDescarregamento situacaoCargaJanelaDescarregamento = SituacaoCargaJanelaDescarregamento.CargaDevolvida;
            SituacaoAgendamentoColeta situacaoAgendamentoColeta = SituacaoAgendamentoColeta.CargaDevolvida;

            if (naoComparecimento)
            {
                mensagemAuditoriaPadrao = "Cancelado por não comparecimento";
                situacaoCargaJanelaDescarregamento = SituacaoCargaJanelaDescarregamento.NaoComparecimento;
                situacaoAgendamentoColeta = SituacaoAgendamentoColeta.NaoComparecimento;

                if (!permiteCancelarCarga)
                    mensagemAuditoriaPadrao = "com não comparecimento informado";
            }

            return ValueTuple.Create(mensagemAuditoriaPadrao, situacaoCargaJanelaDescarregamento, situacaoAgendamentoColeta);
        }

        #endregion
    }
}
