using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoCotacao
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoCotacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, auditado: null) { }

        public CargaJanelaCarregamentoCotacao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, configuracaoEmbarcador: null, auditado) { }

        public CargaJanelaCarregamentoCotacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, auditado: null) { }

        public CargaJanelaCarregamentoCotacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        public CargaJanelaCarregamentoCotacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Privados

        private void FinalizarCotacaoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            cargaJanelaCarregamento.ProcessoCotacaoFinalizada = true;
            cargaJanelaCarregamento.CargaLiberadaCotacao = false;
            cargaJanelaCarregamento.DataTerminoCotacao = null;

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            SalvarHistoricoAlteracao(cargaJanelaCarregamento, $"Cotação encerrada {((cargaJanelaCarregamento.Carga?.Empresa != null) ? "com" : "sem")} transportador escolhido manualmente");

            if (cargaJanelaCarregamento.Carga?.Empresa == null)
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaSemTransportadorEscolhido(cargaJanelaCarregamento);
        }

        private void FinalizarCotacaoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Empresa transportadorEscolhido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorEscolhida = null;

            if (transportadorEscolhido != null)
            {
                cargaJanelaCarregamentoTransportadorEscolhida = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaJanelaCarregamentoETransportador(cargaJanelaCarregamento.Codigo, transportadorEscolhido.Codigo);

                if (cargaJanelaCarregamentoTransportadorEscolhida == null)
                    return;
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(_unitOfWork, configuracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);
            string numeroCarga = servicoCarga.ObterNumeroCarga(cargaJanelaCarregamento.Carga, configuracaoEmbarcador);

            if (transportadorEscolhido != null)
            {
                cargaJanelaCarregamento.CargaCotacaoGanhaAutomaticamente = true;

                servicoCargaJanelaCarregamentoTransportador.DefinirTransportadorComValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportadorEscolhida, tipoServicoMultisoftware);
                SalvarHistoricoAlteracao(cargaJanelaCarregamento, $"Cotação encerrada. Transportador escolhido: {transportadorEscolhido.Descricao}");
                servicoCargaJanelaCarregamentoNotificacao.NotificarAlteracaoCotacao(cargaJanelaCarregamento, $"Cotação da carga {numeroCarga} encerrada. Transportador escolhido: {transportadorEscolhido.Descricao}", tipoServicoMultisoftware);
            }
            else
            {
                SalvarHistoricoAlteracao(cargaJanelaCarregamento, $"Cotação encerrada sem transportador escolhido");
                servicoCargaJanelaCarregamentoNotificacao.NotificarAlteracaoCotacao(cargaJanelaCarregamento, $"Cotação da carga {numeroCarga} encerrada sem transportador escolhido", tipoServicoMultisoftware);
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaSemTransportadorEscolhido(cargaJanelaCarregamento);
            }

            cargaJanelaCarregamento.ProcessoCotacaoFinalizada = true;
            cargaJanelaCarregamento.CargaLiberadaCotacao = false;
            cargaJanelaCarregamento.DataTerminoCotacao = null;

            if (transportadorEscolhido == null && (cargaJanelaCarregamento.CentroCarregamento?.LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente ?? false))
            {
                servicoCargaJanelaCarregamentoTransportador.LimparTransportadoresRejeitadas(cargaJanelaCarregamento);
                servicoCargaJanelaCarregamentoTransportador.DisponibilizarAutomaticamenteParaTransportadores(cargaJanelaCarregamento, tipoServicoMultisoftware);
            }

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        private void LiberarParaCotacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);
            CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);
            string numeroCarga = servicoCarga.ObterNumeroCarga(cargaJanelaCarregamento.Carga, configuracaoEmbarcador);

            cargaJanelaCarregamento.ProcessoCotacaoFinalizada = false;
            cargaJanelaCarregamento.CargaLiberadaCotacao = true;

            if (cargaJanelaCarregamento.CentroCarregamento.TempoMinutosEscolhaAutomaticaCotacao > 0)
                cargaJanelaCarregamento.DataTerminoCotacao = DateTime.Now.AddMinutes(cargaJanelaCarregamento.CentroCarregamento.TempoMinutosEscolhaAutomaticaCotacao);
            else
                cargaJanelaCarregamento.DataTerminoCotacao = null;

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            SalvarHistoricoAlteracao(cargaJanelaCarregamento, "Carga liberada para cotação");
            servicoCargaJanelaCarregamentoNotificacao.NotificarAlteracaoCotacao(cargaJanelaCarregamento, string.Format(Localization.Resources.Logistica.CentroCarregamento.CargaLiberadaCotacao, numeroCarga), tipoServicoMultisoftware);

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, cargaJanelaCarregamento, null, "Liberou a carga para cotação.", _unitOfWork);

            if (cargaJanelaCarregamento.Carga != null)
                new Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private void ReordenarTransportadoresCotacaoPorValorDosLances(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaTransportadores = repositorioCargaJanelaCarregamentoTransportador.BuscarPorInteressadosComValorInformado(cargaJanelaCarregamento.Codigo);
            List<decimal> valoresLancesOrdenados = listaTransportadores.OrderByDescending(x => x.ValorFreteTransportador).Select(x => x.ValorFreteTransportador).Distinct().ToList();
            List<int> prioridadesOrdenadas = listaTransportadores.OrderByDescending(x => x.DadosTransporte?.ModeloCarroceria?.Prioridade ?? 0).Select(x => x.DadosTransporte?.ModeloCarroceria?.Prioridade ?? 0).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaTransportadoresOrdenada = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            foreach (decimal valor in valoresLancesOrdenados)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> transportadoresParaObterPontuacao = listaTransportadores.Where(obj => obj.ValorFreteTransportador == valor).ToList();

                foreach (int prioridade in prioridadesOrdenadas)
                {
                    transportadoresParaObterPontuacao = transportadoresParaObterPontuacao.Where(obj => obj.DadosTransporte?.ModeloCarroceria?.Prioridade == prioridade).ToList();

                    if (transportadoresParaObterPontuacao.Count > 0)
                        listaTransportadoresOrdenada.AddRange(servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportadorOrdenadasPorPontuacao(transportadoresParaObterPontuacao));
                }
            }

            for (int i = 0; i < listaTransportadoresOrdenada.Count; i++)
            {
                listaTransportadoresOrdenada[i].Ordem = (i + 1);
                repositorioCargaJanelaCarregamentoTransportador.Atualizar(listaTransportadoresOrdenada[i]);
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void BloquearParaCotacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);
            CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);
            string numeroCarga = servicoCarga.ObterNumeroCarga(cargaJanelaCarregamento.Carga, configuracaoEmbarcador);

            cargaJanelaCarregamento.ProcessoCotacaoFinalizada = false;
            cargaJanelaCarregamento.CargaLiberadaCotacao = false;
            cargaJanelaCarregamento.DataTerminoCotacao = null;

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            SalvarHistoricoAlteracao(cargaJanelaCarregamento, "Carga bloqueada para cotação");
            servicoCargaJanelaCarregamentoNotificacao.NotificarAlteracaoCotacao(cargaJanelaCarregamento, string.Format(Localization.Resources.Logistica.CentroCarregamento.CargaBloqueadaCotacao, numeroCarga), tipoServicoMultisoftware);

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, cargaJanelaCarregamento, null, "Bloqueou a carga para cotação.", _unitOfWork);

            if (cargaJanelaCarregamento.Carga != null)
                new Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
        }

        public void CancelarCargaLiberadaParaTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador)
                throw new ServicoException("A situação da janela de carregamento não permite que seja cancelada.");

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            CargaJanelaCarregamento servicoCargaJanelaCarregamento = new CargaJanelaCarregamento(_unitOfWork);
            CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new CargaJanelaCarregamentoCotacao(_unitOfWork, configuracaoEmbarcador);

            servicoCargaJanelaCarregamento.AlterarSituacao(cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores);
            servicoCargaJanelaCarregamentoCotacao.BloquearParaCotacao(cargaJanelaCarregamento, _tipoServicoMultisoftware);
            repositorioCargaJanelaCarregamentoTransportador.DeletarPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);
        }

        public void DescartarCotacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.ProntaParaCarregamento) && (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador))
                throw new ServicoException("A situação da janela de carregamento não permite que seja efetuado o descarte.");

            Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if ((cargaJanelaCarregamento.Carga != null) && !servicoCarga.VerificarSeCargaEstaNaLogistica(cargaJanelaCarregamento.Carga, tipoServicoMultisoftware))
                throw new ServicoException("A situação da carga não permite que seja efetuado o descarte.");

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            CargaJanelaCarregamento servicoCargaJanelaCarregamento = new CargaJanelaCarregamento(_unitOfWork);
            CargaJanelaCarregamentoCotacaoAprovacao servicoCargaJanelaCarregamentoCotacaoAprovacao = new CargaJanelaCarregamentoCotacaoAprovacao(_unitOfWork);

            if (cargaJanelaCarregamento.Carga?.Empresa != null)
            {

                Auditoria.Auditoria.Auditar(_auditado, cargaJanelaCarregamento.Carga, $"Removido o transportador {cargaJanelaCarregamento.Carga.Empresa.Descricao} ao descargar a cotação", _unitOfWork);
                cargaJanelaCarregamento.Carga.Empresa = null;

                //#Unilvere pediu para voltra o transportador original ao descartar o leião #70981 
                if (new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork).ExistePorTipo(TipoIntegracao.Unilever))
                    cargaJanelaCarregamento.Carga.Empresa = cargaJanelaCarregamento.TransportadorOriginal;

            }

            cargaJanelaCarregamento.ProcessoCotacaoFinalizada = false;
            cargaJanelaCarregamento.CargaLiberadaCotacao = false;
            cargaJanelaCarregamento.DataTerminoCotacao = null;
            cargaJanelaCarregamento.NaoComparecido = TipoNaoComparecimento.NaoCompareceu;
            cargaJanelaCarregamento.SituacaoCotacao = SituacaoCargaJanelaCarregamentoCotacao.NaoDefinida;
            cargaJanelaCarregamento.Carga.TipoFreteEscolhido = TipoFreteEscolhido.Operador;
            repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);

            servicoCargaJanelaCarregamento.AlterarSituacao(cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores);
            servicoCargaJanelaCarregamentoCotacaoAprovacao.RemoverAprovacao(cargaJanelaCarregamento);
            repositorioCargaJanelaCarregamentoTransportador.DeletarPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);
            SalvarHistoricoAlteracao(cargaJanelaCarregamento, "Cotação da carga descartada");

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, cargaJanelaCarregamento, null, "Descartou a cotação da carga.", _unitOfWork);

            if (cargaJanelaCarregamento.Carga != null)
                new Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
        }

        public void LiberarParaCotacaoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            cargaJanelaCarregamento.CargaLiberadaCotacaoAutomaticamente = true;

            LiberarParaCotacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        public void LiberarParaCotacaoManualmente(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            cargaJanelaCarregamento.CargaLiberadaCotacaoAutomaticamente = false;

            LiberarParaCotacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        public void SalvarHistoricoAlteracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, string mensagem)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao repositorioHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao historico = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao()
            {
                CargaJanelaCarregamento = cargaJanelaCarregamento,
                Data = DateTime.Now,
                Descricao = mensagem
            };

            repositorioHistorico.Inserir(historico);
        }

        public void ValidarPermissaoBloquearParaCotacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador)
                throw new ServicoException("A situação da janela de carregamento não permite bloquear para cotação");

            if (!cargaJanelaCarregamento.CargaLiberadaCotacao)
                throw new ServicoException("A carga já está bloqueada para cotação");
        }

        public void ValidarPermissaoLiberarParaCotacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador)
                throw new ServicoException("A situação da janela de carregamento não permite liberar para cotação");

            if (cargaJanelaCarregamento.CargaLiberadaCotacao)
                throw new ServicoException("A carga já está liberada para cotação");
        }

        public void VerificarCotacaoComTempoEsgotado(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<int> codigosCargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarCodigosPorCotacaoEsgotada(DateTime.Now, limiteRegistros: 5);

            if (codigosCargaJanelaCarregamento.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.CargaVinculada repositorioCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoViagem servicoCargaJanelaCarregamentoViagem = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoViagem(_unitOfWork, tipoServicoMultisoftware);
            bool liberarCargaCotacaoAutomaticamente = ObterConfiguracaoJanelaCarregamento().LiberarCargaParaCotacaoAoLiberarParaTransportadores;

            foreach (int codigoCargaJanelaCarregamento in codigosCargaJanelaCarregamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoCargaJanelaCarregamento);

                if (!cargaJanelaCarregamento.CargaLiberadaCotacao)
                    continue;

                _unitOfWork.Start();

                List<int> codigosCargasVinculadas = repositorioCargaVinculada.BuscarCodigosCargasPorCarga(cargaJanelaCarregamento.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamentoVinculadas = repositorioCargaJanelaCarregamento.BuscarPorCargas(codigosCargasVinculadas);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorEscolhida = null;

                if ((cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.SemTransportador) && (!liberarCargaCotacaoAutomaticamente || cargaJanelaCarregamento.CargaLiberadaCotacaoAutomaticamente))
                {
                    if ((cargaJanelaCarregamento.CentroCarregamento?.RepassarCargaCasoNaoExistaVeiculoDisponivel ?? false) && (cargaJanelaCarregamento.CentroCarregamento?.TempoMinutosEscolhaAutomaticaCotacao > 0))
                        ReordenarTransportadoresCotacaoPorValorDosLances(cargaJanelaCarregamento);

                    Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete transportadorComMenorLance = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargaJanelaCarregamentoTransportadorComMenorLance(cargaJanelaCarregamento, considerarCargasVinculadas: true);

                    if (transportadorComMenorLance != null)
                    {
                        decimal valorLimiteParaCotacao = cargaJanelaCarregamento.Carga.CustoAtualIntegracaoLeilao + cargasJanelaCarregamentoVinculadas.Sum(janelaVinculada => janelaVinculada.Carga.CustoAtualIntegracaoLeilao);

                        if ((valorLimiteParaCotacao <= 0m) || (transportadorComMenorLance.ValorTotalFrete <= valorLimiteParaCotacao))
                            cargaJanelaCarregamentoTransportadorEscolhida = transportadorComMenorLance.CargaJanelaCarregamentoTransportador;
                    }

                    FinalizarCotacaoAutomaticamente(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportadorEscolhida?.Transportador, tipoServicoMultisoftware);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoVinculada in cargasJanelaCarregamentoVinculadas)
                        FinalizarCotacaoAutomaticamente(cargaJanelaCarregamentoVinculada, cargaJanelaCarregamentoTransportadorEscolhida?.Transportador, tipoServicoMultisoftware);

                    servicoCargaJanelaCarregamentoViagem.GerarIntegracaoVencedorLeilao(cargaJanelaCarregamentoTransportadorEscolhida);
                }
                else
                {
                    FinalizarCotacaoAutomaticamente(cargaJanelaCarregamento);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoVinculada in cargasJanelaCarregamentoVinculadas)
                        FinalizarCotacaoAutomaticamente(cargaJanelaCarregamentoVinculada);
                }

                _unitOfWork.CommitChanges();

                if (cargaJanelaCarregamentoTransportadorEscolhida != null)
                    servicoCargaJanelaCarregamentoNotificacao.EnviarEmailParaTransportadorEscolhido(cargaJanelaCarregamentoTransportadorEscolhida);
            }
        }

        #endregion Métodos Públicos
    }
}
