using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaDescarregamento
    {
        #region Atributos

        private Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaDescarregamentoComposicaoHorario _composicaoHorarioAtual;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento _configuracoesDescarregamento;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaJanelaDescarregamento(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, auditado: null, configuracoesDescarregamento: null) { }

        public CargaJanelaDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, configuracaoEmbarcador: null, auditado, configuracoesDescarregamento: null) { }

        public CargaJanelaDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, auditado: null, configuracoesDescarregamento: null) { }

        public CargaJanelaDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, configuracaoEmbarcador, auditado, configuracoesDescarregamento: null) { }

        public CargaJanelaDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento configuracoesDescarregamento) : this(unitOfWork, configuracaoEmbarcador, auditado: null, configuracoesDescarregamento) { }

        public CargaJanelaDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento configuracoesDescarregamento)
        {
            _auditado = auditado;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
            _configuracoesDescarregamento = configuracoesDescarregamento ?? new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento();
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento Adicionar(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataDescarregamento, SituacaoCargaJanelaDescarregamento situacaoInicial, bool redefinirHorarioDescarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, bool adicionarManualmente)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCargaECentroDescarregamento(carga.Codigo, centroDescarregamento.Codigo);

            if (cargaJanelaDescarregamento != null)
            {
                if (cargaJanelaDescarregamento.Cancelada)
                {
                    if (!adicionarManualmente)
                        return null;

                    cargaJanelaDescarregamento.Cancelada = false;
                }
                else
                {
                    bool atualizarDefinicaoHorarioDescarregamentoCargaJanelaCarregamentoExistente = !carga.CargaAgrupada && !redefinirHorarioDescarregamento;

                    if (atualizarDefinicaoHorarioDescarregamentoCargaJanelaCarregamentoExistente)
                    {
                        AtualizarDefinicaoHorarioDescarregamento(cargaJanelaDescarregamento);
                        repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento);

                        return cargaJanelaDescarregamento;
                    }
                }
            }

            if (cargaJanelaDescarregamento == null)
                cargaJanelaDescarregamento = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento()
                {
                    Carga = carga,
                    CentroDescarregamento = centroDescarregamento,
                    Situacao = situacaoInicial
                };

            DateTime inicioDescarregamentoAnterior = cargaJanelaDescarregamento.InicioDescarregamento;

            DefinirHorarioDescarregamento(cargaJanelaDescarregamento, dataDescarregamento, periodo);

            if (cargaJanelaDescarregamento.Codigo > 0)
            {
                repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento);

                if ((cargaJanelaDescarregamento.InicioDescarregamento != inicioDescarregamentoAnterior) && (_auditado != null))
                    Auditoria.Auditoria.Auditar(_auditado, cargaJanelaDescarregamento, $"Data de descarregamento alterada de {inicioDescarregamentoAnterior.ToDateTimeString()} para {cargaJanelaDescarregamento.InicioDescarregamento.ToDateTimeString()}{(redefinirHorarioDescarregamento ? " devido a mudança na data de carregamento" : "")}", _unitOfWork);
            }
            else
            {
                repositorioCargaJanelaDescarregamento.Inserir(cargaJanelaDescarregamento);
                DefinirSituacao(cargaJanelaDescarregamento);
            }

            AdicionarComposicaoHorario(cargaJanelaDescarregamento);
            ConfirmarAgendamentoAutomaticamente(cargaJanelaDescarregamento);

            return cargaJanelaDescarregamento;
        }

        private void AdicionarAgendamentoEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            if ((carga.ModeloVeicularCarga == null) || (carga.TipoDeCarga == null) || (cargaJanelaDescarregamento == null))
                return;

            Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega = repositorioAgendamentoEntrega.BuscarPorCarga(carga.Codigo);

            if (agendamentoEntrega != null)
                return;

            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
            List<Dominio.Entidades.Cliente> destinatarios = servicoCargaDadosSumarizados.ObterDestinatarios(carga.Codigo, _unitOfWork);
            List<Dominio.Entidades.Cliente> remetentes = servicoCargaDadosSumarizados.ObterRemetentes(carga.Codigo, _unitOfWork);
            Dominio.Entidades.Cliente destinatario = destinatarios?.FirstOrDefault();
            Dominio.Entidades.Cliente remetente = remetentes?.FirstOrDefault();

            agendamentoEntrega = new Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega()
            {
                Carga = carga,
                DataAgendamento = cargaJanelaDescarregamento.InicioDescarregamento,
                Destinatario = destinatario,
                ModeloVeicularCarga = carga.ModeloVeicularCarga,
                Motorista = carga.Motoristas?.FirstOrDefault()?.Nome ?? "",
                Placa = carga.Veiculo?.Placa ?? "",
                Remetente = remetente,
                Senha = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper(),
                SenhaAgendamento = repositorioAgendamentoEntrega.ObterProximaSenhaAgendamento(),
                Situacao = SituacaoAgendamentoEntrega.Agendado,
                TipoDeCarga = carga.TipoDeCarga,
                Transportador = carga.Empresa?.Descricao ?? ""
            };

            repositorioAgendamentoEntrega.Inserir(agendamentoEntrega);
        }

        private void AdicionarComposicaoHorario(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            if (_composicaoHorarioAtual == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario repositorioComposicaoHorario = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe repositorioComposicaoHorarioDetalhe = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario composicaoHorario = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario()
            {
                CargaJanelaDescarregamento = cargaJanelaDescarregamento
            };

            repositorioComposicaoHorario.Inserir(composicaoHorario);

            for (int i = 0; i < _composicaoHorarioAtual.Detalhes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe composicaoHorarioDetalhe = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe()
                {
                    ComposicaoHorario = composicaoHorario,
                    Descricao = _composicaoHorarioAtual.Detalhes[i],
                    Ordem = i + 1
                };

                repositorioComposicaoHorarioDetalhe.Inserir(composicaoHorarioDetalhe);
            }
        }

        private void AdicionarDetalheComposicaoHorario(string detalhe)
        {
            if (_composicaoHorarioAtual == null)
                return;

            _composicaoHorarioAtual.Detalhes.Add(detalhe);
        }

        private void AdicionarDetalhesComposicaoHorario(List<string> detalhes)
        {
            if (_composicaoHorarioAtual == null)
                return;

            _composicaoHorarioAtual.Detalhes.AddRange(detalhes);
        }

        private void AdicionarFluxoGestaoPatio(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AdicionarFluxoGestaoPatio(carga, tipoServicoMultisoftware, cargaJanelaCarregamento: null);
        }

        private void AdicionarFluxoGestaoPatio(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            servicoFluxoGestaoPatio.Adicionar(carga, tipoServicoMultisoftware, cargaJanelaCarregamento);
        }

        private void AdicionarOuAtualizar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool atualizar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            if (!IsPermitirAdicionar(carga))
                return;

            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = ObterCentrosDescarregamento(carga);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = ObterPrevisoesCargaEntrega(carga, centrosDescarregamento);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamentoAdicionada = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>();

            Log.TratarErro($"Encontrados {centrosDescarregamento.Count} centros de descarregamento", "GeracaoJanelaDescarga");
            Log.TratarErro($"Encontradas {previsoesCargaEntrega.Count} previsões de carga entrega", "GeracaoJanelaDescarga");

            if (previsoesCargaEntrega.Count <= 0)
            {
                foreach (Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento in centrosDescarregamento)
                {
                    _composicaoHorarioAtual = new Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaDescarregamentoComposicaoHorario();

                    SituacaoCargaJanelaDescarregamento situacaoInicialDescarregamento = ObterSituacaoInicial(centroDescarregamento);

                    DateTime inicioDescarregamento = ObterInicioDescarregamento(carga, centroDescarregamento, cargaJanelaCarregamento);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = Adicionar(centroDescarregamento, carga, inicioDescarregamento, situacaoInicialDescarregamento, redefinirHorarioDescarregamento: atualizar, periodo: null, adicionarManualmente: false);

                    Log.TratarErro($"Janela de descarga {(cargaJanelaDescarregamento == null ? " não gerada" : "gerada")}", "GeracaoJanelaDescarga");

                    if (cargaJanelaDescarregamento != null)
                        listaCargaJanelaDescarregamentoAdicionada.Add(cargaJanelaDescarregamento);

                    _composicaoHorarioAtual = null;
                }
            }
            else
            {
                double diferencaTempo = 0d;

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega previsaoCarregamento in previsoesCargaEntrega)
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = (from obj in centrosDescarregamento where obj.Destinatario?.CPF_CNPJ == previsaoCarregamento.Cliente?.CPF_CNPJ select obj).FirstOrDefault();

                    Log.TratarErro($"Centro de descarregamento {(centroDescarregamento == null ? " não encontrado" : "encontrado")} para o cliente {previsaoCarregamento.Cliente?.CPF_CNPJ}", "GeracaoJanelaDescarga");

                    if (centroDescarregamento == null)
                        continue;

                    _composicaoHorarioAtual = new Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaDescarregamentoComposicaoHorario();

                    DateTime inicioDescarregamento = previsaoCarregamento.DataInicioEntregaPrevista.Value.AddMinutes(diferencaTempo);

                    AdicionarDetalhesComposicaoHorario(previsaoCarregamento.ComposicaoPrevisao.DetalhesDataInicioEntregaPrevista);
                    AdicionarDetalheComposicaoHorario($"Data de início do descarregamento: {previsaoCarregamento.DataInicioEntregaPrevista.Value.ToDateTimeString()} [Data de início de trânsito]");

                    if (diferencaTempo != 0d)
                    {
                        AdicionarDetalheComposicaoHorario($"Diferença de tempo na alocação do horário de descarregamento anterior: {diferencaTempo} minutos");
                        AdicionarDetalheComposicaoHorario($"Data de início do descarregamento: {inicioDescarregamento.ToDateTimeString()} [Data de início do descarregamento + Diferença de tempo na alocação do horário de descarregamento anterior]");
                    }

                    SituacaoCargaJanelaDescarregamento situacaoInicialDescarregamento = ObterSituacaoInicial(centroDescarregamento);

                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = Adicionar(centroDescarregamento, carga, inicioDescarregamento, situacaoInicialDescarregamento, redefinirHorarioDescarregamento: atualizar, periodo: null, adicionarManualmente: false);

                    Log.TratarErro($"Janela de descarga {(cargaJanelaDescarregamento == null ? " não gerada" : "gerada")}", "GeracaoJanelaDescarga");

                    if (cargaJanelaDescarregamento != null)
                    {
                        listaCargaJanelaDescarregamentoAdicionada.Add(cargaJanelaDescarregamento);

                        diferencaTempo += (inicioDescarregamento - cargaJanelaDescarregamento.InicioDescarregamento).TotalMinutes.RoundUp(0);
                    }

                    _composicaoHorarioAtual = null;
                }
            }

            RemoverTodasForaDaListaPorCarga(carga, listaCargaJanelaDescarregamentoAdicionada);

            if ((centrosDescarregamento.Count > 0) && !atualizar)
            {
                AdicionarFluxoGestaoPatio(carga, tipoServicoMultisoftware.Value, cargaJanelaCarregamento); // Gambiada autorizada pelo guigo para atender a tarefa #10701
                AdicionarAgendamentoEntrega(carga, listaCargaJanelaDescarregamentoAdicionada.FirstOrDefault());
            }
        }

        private void AtualizarDefinicaoHorarioDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoDisponibilidadeDescarregamento = new Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(_unitOfWork);

            servicoDisponibilidadeDescarregamento.AtualizarDefinicaoHorarioDescarregamento(cargaJanelaDescarregamento);
        }

        private void ConfirmarAgendamentoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            AgendamentoColeta servicoAgendamentoColeta = new AgendamentoColeta(_unitOfWork);

            if (!servicoAgendamentoColeta.ValidarConfirmarAgendamentoAutomaticamente(cargaJanelaDescarregamento))
                return;

            Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega = repositorioAgendamentoEntrega.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);
            Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet = repositorioAgendamentoPallet.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

            ConfirmarAgendamento(cargaJanelaDescarregamento, agendamentoColeta, agendamentoEntrega, agendamentoPallet, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
        }

        private void DefinirHorarioDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime dataDescarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo)
        {
            cargaJanelaDescarregamento.TempoDescarregamento = ObterTempoDescarregamento(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);
            cargaJanelaDescarregamento.InicioDescarregamento = (periodo == null) ? dataDescarregamento : dataDescarregamento.Date.Add(periodo.HoraInicio);
            cargaJanelaDescarregamento.TerminoDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);
            cargaJanelaDescarregamento.DataDescarregamentoProgramada = cargaJanelaDescarregamento.InicioDescarregamento;

            if (cargaJanelaDescarregamento.TempoDescarregamento == 0)
            {
                if (cargaJanelaDescarregamento.CentroDescarregamento.BloquearJanelaDescarregamentoExcedente)
                    throw new ServicoException($"Não existe uma configuração de tempo de descarregamento no centro {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para as configurações desta carga.");

                cargaJanelaDescarregamento.Excedente = true;
                return;
            }

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento configuracaoDisponibilidadeDescarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento()
            {
                PermitirHorarioDescarregamentoComLimiteAtingido = _configuracoesDescarregamento.PermitirHorarioDescarregamentoComLimiteAtingido,
                PermitirHorarioDescarregamentoInferiorAoAtual = _configuracoesDescarregamento.PermitirHorarioDescarregamentoInferiorAoAtual,
                NaoPermitirBuscarOutroPeriodo = _configuracoesDescarregamento.NaoPermitirBuscarOutroPeriodo
            };

            CargaJanelaDescarregamentoDisponibilidade servicoDisponibilidadeDescarregamento = new CargaJanelaDescarregamentoDisponibilidade(_unitOfWork, configuracaoDisponibilidadeDescarregamento);

            if (periodo != null)
            {
                servicoDisponibilidadeDescarregamento.DefinirHorarioDescarregamentoPorPeriodo(cargaJanelaDescarregamento, periodo, dataDescarregamento);
                return;
            }

            try
            {
                servicoDisponibilidadeDescarregamento.DefinirHorarioDescarregamento(cargaJanelaDescarregamento);
            }
            catch (ServicoException excecao) when (
                (excecao.ErrorCode == CodigoExcecao.HorarioDescarregamentoIndisponivel) ||
                (excecao.ErrorCode == CodigoExcecao.HorarioDescarregamentoInferiorAtual) ||
                (excecao.ErrorCode == CodigoExcecao.HorarioLimiteDescarregamentoAtingido) ||
                (excecao.ErrorCode == CodigoExcecao.PeriodosIndisponiveisFaixaHorarioSelecionada) ||
                (excecao.ErrorCode == CodigoExcecao.DataBloqueada)
            )
            {
                if (cargaJanelaDescarregamento.CentroDescarregamento.PermitirBuscarAteFimDaJanela)
                    servicoDisponibilidadeDescarregamento.DefinirHorarioDescarregamentoAteDataLimite(cargaJanelaDescarregamento);
                else
                    servicoDisponibilidadeDescarregamento.DefinirHorarioDescarregamentoAteLimiteTentativas(cargaJanelaDescarregamento);
            }

            if (!cargaJanelaDescarregamento.Excedente)
                AdicionarDetalheComposicaoHorario($"Data de início do descarregamento: {cargaJanelaDescarregamento.InicioDescarregamento.ToDateTimeString()} [Primeira data disponível na janela]");
        }

        private void DefinirSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            SituacaoCargaJanelaDescarregamentoCadastrada situacao = SituacaoCargaJanelaDescarregamentoCadastrada.PendenteColeta;

            if (new Carga.Carga(_unitOfWork).IsDadosTransporteinformados(cargaJanelaDescarregamento.Carga, _unitOfWork))
                situacao = SituacaoCargaJanelaDescarregamentoCadastrada.Programado;

            AtualizarSituacao(cargaJanelaDescarregamento, situacao);
        }

        private bool IsCargaJanelaCarregamentoComHorarioAlocado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            return ((cargaJanelaCarregamento != null) && (cargaJanelaCarregamento.CentroCarregamento != null) && !cargaJanelaCarregamento.Excedente);
        }

        private bool IsPermitirAdicionarPorCentroDescarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            try
            {
                ValidarPermissaoAdicionarPorCentroDescarregamento(carga, centroDescarregamento);
                return true;
            }
            catch (ServicoException)
            {
                return false;
            }
        }

        private List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> ObterCentrosDescarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatariosDaCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamentoRetornar = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();
            List<Dominio.Entidades.Cliente> destinatarios = centrosDescarregamento.Select(centro => centro.Destinatario).DistinctBy(centro => centro.CPF_CNPJ).ToList();

            foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
            {
                bool destinatarioPossuiCentroDescarregamento = false;
                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamentoPorDestinatario = centrosDescarregamento
                    .Where(centro => centro.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ)
                    .OrderBy(centro => centro.Filial == null)
                    .ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento in centrosDescarregamentoPorDestinatario)
                {
                    if ((centroDescarregamento.Filial != null) && (centroDescarregamento.Filial.Codigo != carga.Filial?.Codigo))
                        continue;

                    if (configuracaoJanelaCarregamento.UtilizarCentroDescarregamentoPorTipoCarga && !(centroDescarregamento.TiposCarga?.Any(tipoCarga => tipoCarga.Codigo == carga.TipoDeCarga?.Codigo) ?? false))
                        continue;

                    destinatarioPossuiCentroDescarregamento = true;

                    if (IsPermitirAdicionarPorCentroDescarregamento(carga, centroDescarregamento))
                        centrosDescarregamentoRetornar.Add(centroDescarregamento);

                    break;
                }

                if (!destinatarioPossuiCentroDescarregamento && destinatario.ClienteDescargas.Any(clienteDescarga => clienteDescarga.AgendamentoDescargaObrigatorio))
                    throw new ServicoException($"O destinatário {destinatario.Descricao} não possui um centro de descarregamento compatível com as configurações da carga.");
            }

            return centrosDescarregamentoRetornar;
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

        private SituacaoCargaJanelaDescarregamento ObterSituacaoInicial(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            if (centroDescarregamento.ExigeAprovacaoCargaParaDescarregamento)
                return SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento;

            return SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento;
        }

        private DateTime ObterInicioDescarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            bool cargaJanelaCarregamentoComHorarioAlocado = IsCargaJanelaCarregamentoComHorarioAlocado(cargaJanelaCarregamento);

            if (!cargaJanelaCarregamentoComHorarioAlocado)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                DateTime? dataPrevisaoChegadaDestinatario = repositorioCargaPedido.BuscarDataPrevisaoChegadaDestinatario(carga.Codigo, centroDescarregamento.Destinatario.CPF_CNPJ);

                if (dataPrevisaoChegadaDestinatario.HasValue)
                {
                    AdicionarDetalheComposicaoHorario($"Data de início do descarregamento: {dataPrevisaoChegadaDestinatario.Value.ToDateTimeString()} [Data de previsão de chegada no destinatário]");
                    return dataPrevisaoChegadaDestinatario.Value;
                }
            }

            DateTime inicioCarregamento;
            DateTime terminoCarregamento;
            int tempoCarregamento;
            string campoDataUtilizado;

            if (cargaJanelaCarregamentoComHorarioAlocado)
            {
                inicioCarregamento = cargaJanelaCarregamento.InicioCarregamento;
                terminoCarregamento = cargaJanelaCarregamento.TerminoCarregamento;
                tempoCarregamento = cargaJanelaCarregamento.TempoCarregamento;
                campoDataUtilizado = "Data de início do carregamento";
            }
            else
            {
                if (carga.DataCarregamentoCarga.HasValue)
                {
                    inicioCarregamento = carga.DataCarregamentoCarga.Value;
                    campoDataUtilizado = "Data de carregamento da carga";
                }
                else if (configuracaoEmbarcador.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento > 0 || (cargaJanelaCarregamento != null && cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento > 0))
                {
                    inicioCarregamento = carga.DataCriacaoCarga;
                    campoDataUtilizado = "Data de integração da carga";
                }
                else
                {
                    inicioCarregamento = DateTime.Now.Date;
                    campoDataUtilizado = "Data atual";
                }

                if (carga.Rota?.HoraInicioCarregamento != null)
                {
                    inicioCarregamento = inicioCarregamento.Date.AddMinutes(carga.Rota.HoraInicioCarregamento.Value.TotalMinutes);

                    if (DateTime.Now > cargaJanelaCarregamento.InicioCarregamento)
                        inicioCarregamento = DateTime.Now.Date.AddDays(1).AddMinutes(carga.Rota.HoraInicioCarregamento.Value.TotalMinutes);
                }

                tempoCarregamento = new CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoEmbarcador).ObterTempoCarregamento(carga, inicioCarregamento.TimeOfDay);
                terminoCarregamento = inicioCarregamento.AddMinutes(tempoCarregamento);
            }

            int tempoDeViagemEmMinutos = carga.Rota?.ObterTempoViagemEmMinutos() ?? 0;
            DateTime inicioDescarregamento = terminoCarregamento.AddMinutes(tempoDeViagemEmMinutos);

            AdicionarDetalheComposicaoHorario($"{campoDataUtilizado}: {inicioCarregamento.ToDateTimeString()}");
            AdicionarDetalheComposicaoHorario($"Tempo de carregamento: {tempoCarregamento} minutos ({Math.Round(tempoCarregamento / 60d, digits: 2)} horas)");
            AdicionarDetalheComposicaoHorario($"Tempo de viagem: {tempoDeViagemEmMinutos} minutos ({Math.Round(tempoDeViagemEmMinutos / 60d, digits: 2)} horas)");
            AdicionarDetalheComposicaoHorario($"Data de início do descarregamento: {inicioDescarregamento.ToDateTimeString()} [{campoDataUtilizado} + Tempo de carregamento + Tempo de viagem]");

            return inicioDescarregamento;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> ObterPrevisoesCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ParametrosConfiguracaoCalculoPrevisaoEntrega parametrosConfiguracaoCalculo = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ParametrosConfiguracaoCalculoPrevisaoEntrega();
            parametrosConfiguracaoCalculo.ArmazenarComposicoesPrevisoes = true;

            Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntrega = new Carga.ControleEntrega.PrevisaoControleEntrega(_unitOfWork, configuracaoEmbarcador, parametrosConfiguracaoCalculo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = servicoPrevisaoControleEntrega.CalcularPrevisoesEntregas(carga, centrosDescarregamento);

            return previsoesCargaEntrega.Where(previsaoEntrega => !previsaoEntrega.Coleta).ToList();
        }

        private void RemoverTodasForaDaListaPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamentoPorCarga = repositorioCargaJanelaDescarregamento.BuscarTodasPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in listaCargaJanelaDescarregamentoPorCarga)
            {
                if (!listaCargaJanelaDescarregamento.Any(o => o.Codigo == cargaJanelaDescarregamento.Codigo))
                    repositorioCargaJanelaDescarregamento.DeletarPorCodigo(cargaJanelaDescarregamento.Codigo);
            }
        }

        private void ValidarPermissaoAdicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if ((carga?.TipoOperacao == null) || !carga.TipoOperacao.PermitirAdicionarNaJanelaDescarregamento)
                throw new ServicoException("O tipo de operação da carga não permite adicionar na janela de descarrregamento.");

            if (carga.BloquearAlteracaoJanelaDescarregamento)
                throw new ServicoException("A janela de descarrregamento está bloqueada para alterações.");
        }

        private void ValidarPermissaoAdicionarPorCentroDescarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            if (carga.Redespacho != null)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

                if (configuracaoJanelaCarregamento.BloquearGeracaoJanelaParaCargaRedespacho && !centroDescarregamento.PermitirGeracaoJanelaParaCargaRedespacho)
                    throw new ServicoException("Não é possível adicionar carga de redespacho na janela de descarrregamento.");
            }
        }

        private void AdicionarCargaIntegracaoRasterAtualizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(_unitOfWork);

            if (repCargaIntegracao.ExistePorTipoIntegracao(carga.Codigo, tipoIntegracao.Codigo))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracao()
            {
                Carga = carga,
                TipoIntegracao = tipoIntegracao
            };
            repCargaIntegracao.Inserir(cargaIntegracao);
        }

        private void AdicionarCargaCargaIntegracaoRasterAtualizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoRasterAtualizacao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao.Codigo);

            if (cargaIntegracaoRasterAtualizacao != null)
            {
                cargaIntegracaoRasterAtualizacao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                cargaIntegracaoRasterAtualizacao.ProblemaIntegracao = "Reenviado e aguardando integração";
                repCargaCargaIntegracao.Atualizar(cargaIntegracaoRasterAtualizacao);
            }
            else
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao novaCargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao();
                novaCargaIntegracao.Carga = carga;
                novaCargaIntegracao.DataIntegracao = DateTime.Now;
                novaCargaIntegracao.NumeroTentativas = 0;
                novaCargaIntegracao.ProblemaIntegracao = "";
                novaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                novaCargaIntegracao.TipoIntegracao = tipoIntegracao;
                novaCargaIntegracao.IntegracaoColeta = false;
                novaCargaIntegracao.RealizarIntegracaoCompleta = false;
                novaCargaIntegracao.IntegracaoFilialEmissora = false;
                novaCargaIntegracao.FinalizarCargaAnterior = false;
                repCargaCargaIntegracao.Inserir(novaCargaIntegracao);
            }
        }

        #endregion

        #region Métodos Públicos

        public void ConfirmarAgendamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega, Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(_unitOfWork);
            Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware repositorioClienteMultisoftware = new Repositorio.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware clienteMultisoftware = repositorioClienteMultisoftware.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();
            AgendamentoColeta servicoAgendamentoColeta = new AgendamentoColeta(_unitOfWork, tipoServicoMultisoftware);
            GestaoPallet.AgendamentoPallet servicoAgendamentoPallet = new GestaoPallet.AgendamentoPallet(_unitOfWork);
            GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(_unitOfWork, _auditado, null);

            string mensagemAuditoria = usuario == null ? "Agendamento confirmado automaticamente." : "Confirmou o agendamento.";
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = _auditado;
            bool erroAoBuscarSenhaAgendamento = false;

            if (auditado == null)
                auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    Usuario = usuario,
                    Empresa = usuario?.Empresa
                };

            if (agendamentoColeta != null)
            {
                agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Agendado;

                if (agendamentoColeta.EtapaAgendamentoColeta == EtapaAgendamentoColeta.AguardandoAceite)
                    agendamentoColeta.EtapaAgendamentoColeta = (configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta || (cargaJanelaDescarregamento.Carga.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.RemoverEtapaAgendamentoDoAgendamentoColeta ?? false)) ? EtapaAgendamentoColeta.NFe : EtapaAgendamentoColeta.DadosTransporte;

                agendamentoColeta.ResponsavelConfirmacao = usuario;
                agendamentoColeta.Senha = servicoAgendamentoColeta.ObterSenhaAgendamentoColeta(cargaJanelaDescarregamento, agendamentoColeta, configuracaoAgendamentoColeta);

                if (agendamentoColeta.Carga?.OcultarNoPatio ?? false)
                {
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                    agendamentoColeta.Carga.OcultarNoPatio = false;
                    repositorioCarga.Atualizar(agendamentoColeta.Carga);
                }

                if (string.IsNullOrWhiteSpace(agendamentoColeta.Senha) && (cargaJanelaDescarregamento.CentroDescarregamento?.BuscarSenhaViaIntegracao ?? false))
                {
                    agendamentoColeta.Situacao = SituacaoAgendamentoColeta.AguardandoGeracaoSenha;
                    erroAoBuscarSenhaAgendamento = true;
                    mensagemAuditoria = "Agendamento Aguardando Geração de Senha";
                }

                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
                EnviarEmailConfirmacaoAgendamentoColeta(agendamentoColeta, _unitOfWork, clienteMultisoftware);
            }

            if (agendamentoEntrega != null)
            {
                agendamentoEntrega.Situacao = SituacaoAgendamentoEntrega.Agendado;
                repositorioAgendamentoEntrega.Atualizar(agendamentoEntrega);

                EnviarEmailConfirmacaoAgendamentoEntrega(agendamentoEntrega, _unitOfWork, clienteMultisoftware);
            }

            if (agendamentoPallet != null)
            {
                agendamentoPallet.Situacao = SituacaoAgendamentoPallet.Acompanhamento;
                agendamentoPallet.EtapaAgendamentoPallet = EtapaAgendamentoPallet.Acompanhamento;
                agendamentoPallet.ResponsavelConfirmacao = usuario;
                agendamentoPallet.Senha = servicoAgendamentoPallet.ObterSenhaAgendamentoPallet(agendamentoPallet);

                repositorioAgendamentoPallet.Atualizar(agendamentoPallet);

                Servicos.Auditoria.Auditoria.Auditar(auditado, agendamentoPallet, null, $"Confirmou o agendamento pallet.", _unitOfWork);
            }

            if (cargaJanelaDescarregamento.Codigo > 0)
            {
                AtualizarSituacao(cargaJanelaDescarregamento, (erroAoBuscarSenhaAgendamento ? SituacaoCargaJanelaDescarregamento.AguardandoGeracaoSenha : SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento));
                Auditoria.Auditoria.Auditar(auditado, cargaJanelaDescarregamento, null, mensagemAuditoria, _unitOfWork);
                _unitOfWork.Flush();
            }

            servicoFluxoGestaoPatio.Adicionar(cargaJanelaDescarregamento.Carga, tipoServicoMultisoftware);
            servicoGestaoDevolucao.MovimentarEtapaCargaDevolucao(cargaJanelaDescarregamento.Carga, true, "Confirmaçao de Agendamento.");

            if (agendamentoColeta != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, agendamentoColeta, null, mensagemAuditoria, _unitOfWork);

            if (agendamentoEntrega != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, agendamentoEntrega, null, mensagemAuditoria, _unitOfWork);
        }

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime DataAgendamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = null)
        {
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = ObterCentrosDescarregamento(carga);

            if (centrosDescarregamento.Count == 0)
                throw new ServicoException("Não existe centro de descarregamento para as configurações desta carga.");

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento in centrosDescarregamento)
                Adicionar(centroDescarregamento, carga, DataAgendamento, SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento, redefinirHorarioDescarregamento: false, periodo, adicionarManualmente: false);

            AdicionarFluxoGestaoPatio(carga, tipoServicoMultisoftware); // Gambiada autorizada pelo guigo para atender a tarefa #10701
        }

        public void Adicionar(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = ObterCentrosDescarregamento(agendamentoColeta.Carga);

            if (centrosDescarregamento.Count <= 0 && configuracaoEmbarcador.ControlarAgendamentoSKU)
                throw new ServicoException("Não foi encontrado um centro de descarregamento.");

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento in centrosDescarregamento)
            {
                if (configuracaoEmbarcador.ControlarAgendamentoSKU)
                    Adicionar(centroDescarregamento, agendamentoColeta.Carga, (DateTime)agendamentoColeta.DataAgendamento, SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento, redefinirHorarioDescarregamento: false, periodo: null, adicionarManualmente: false);
                else
                    Adicionar(centroDescarregamento, agendamentoColeta.Carga, (DateTime)agendamentoColeta.DataEntrega, SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento, redefinirHorarioDescarregamento: false, periodo: null, adicionarManualmente: false);
            }

            if (centrosDescarregamento.Count > 0)
                AdicionarFluxoGestaoPatio(agendamentoColeta.Carga, tipoServicoMultisoftware);
        }

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AdicionarOuAtualizar(carga, cargaJanelaCarregamento, atualizar: false, tipoServicoMultisoftware: tipoServicoMultisoftware);
        }

        public void AdicionarManualmente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, DateTime dataDescarregamento)
        {
            AdicionarManualmente(carga, centroDescarregamento, dataDescarregamento, periodo: null);
        }

        public void AdicionarManualmente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, DateTime dataDescarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo)
        {
            if (carga.BloquearAlteracaoJanelaDescarregamento)
            {
                carga.BloquearAlteracaoJanelaDescarregamento = false;
                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(carga);
            }

            ValidarPermissaoAdicionar(carga);
            ValidarPermissaoAdicionarPorCentroDescarregamento(carga, centroDescarregamento);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            if (!repositorioCargaPedido.ExistePorCargaEDestinatario(carga.Codigo, centroDescarregamento.Destinatario.CPF_CNPJ))
                throw new ServicoException("A carga não possui nenhum pedido destinado ao centro de descarregamento informado.");

            SituacaoCargaJanelaDescarregamento situacaoInicialDescarregamento = ObterSituacaoInicial(centroDescarregamento);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = Adicionar(centroDescarregamento, carga, dataDescarregamento, situacaoInicialDescarregamento, redefinirHorarioDescarregamento: false, periodo: periodo, adicionarManualmente: true);

            AdicionarAgendamentoEntrega(carga, cargaJanelaDescarregamento);
        }

        public void Atualizar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            AdicionarOuAtualizar(carga, cargaJanelaCarregamento, atualizar: true, tipoServicoMultisoftware: null);
        }

        public void AtualizarSituacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, EtapaFluxoGestaoPatio etapaFluxoPatio)
        {
            switch (etapaFluxoPatio)
            {
                case EtapaFluxoGestaoPatio.ChegadaVeiculo:
                    AtualizarSituacao(carga, SituacaoCargaJanelaDescarregamentoCadastrada.AguardandoDescarga);
                    break;

                case EtapaFluxoGestaoPatio.Guarita:
                    AtualizarSituacao(carga, SituacaoCargaJanelaDescarregamentoCadastrada.EmDescarga);
                    break;

                case EtapaFluxoGestaoPatio.InformarDoca:
                    AtualizarSituacao(carga, SituacaoCargaJanelaDescarregamentoCadastrada.AguardandoVeiculoEncostar);
                    break;

                case EtapaFluxoGestaoPatio.FimDescarregamento:
                    AtualizarSituacao(carga, SituacaoCargaJanelaDescarregamentoCadastrada.DescarregamentoFinalizado);
                    break;
            }
        }

        public void AtualizarSituacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, SituacaoCargaJanelaDescarregamentoCadastrada novaSituacao)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarTodasPorCarga(carga.Codigo, retornarCanceladas: false);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in listaCargaJanelaDescarregamento)
                AtualizarSituacao(cargaJanelaDescarregamento, novaSituacao);
        }

        public void AtualizarSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamentoCadastrada novaSituacao)
        {
            Repositorio.Embarcador.Cargas.JanelaDescarregamentoSituacao repositorioJanelaDescarregamentoSituacao = new Repositorio.Embarcador.Cargas.JanelaDescarregamentoSituacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.JanelaDescarregamentoSituacao janelaDescarregamentoSituacao = repositorioJanelaDescarregamentoSituacao.BuscarPorSituacao(novaSituacao);

            if (janelaDescarregamentoSituacao == null)
                return;

            if (cargaJanelaDescarregamento.JanelaDescarregamentoSituacao?.Codigo == janelaDescarregamentoSituacao.Codigo)
                return;

            cargaJanelaDescarregamento.JanelaDescarregamentoSituacao = janelaDescarregamentoSituacao;

            new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork).Atualizar(cargaJanelaDescarregamento);

            if (_auditado == null)
                Auditoria.Auditoria.AuditarSemDadosUsuario(cargaJanelaDescarregamento, $"Alterada a situação para {janelaDescarregamentoSituacao.Descricao}", _unitOfWork);
            else
                Auditoria.Auditoria.Auditar(_auditado, cargaJanelaDescarregamento, $"Alterada a situação para {janelaDescarregamentoSituacao.Descricao}", _unitOfWork);
        }

        public void AtualizarSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento novaSituacao)
        {
            if (cargaJanelaDescarregamento.Situacao == novaSituacao)
                return;


            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);


            InserirHistoricoCargaJanelaDescarregamento(cargaJanelaDescarregamento, novaSituacao);
            cargaJanelaDescarregamento.Situacao = novaSituacao;

            if (cargaJanelaDescarregamento.Situacao == SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento || cargaJanelaDescarregamento.Situacao == SituacaoCargaJanelaDescarregamento.ChegadaConfirmada)
                cargaJanelaDescarregamento.DataConfirmacao = DateTime.Now;

            if (cargaJanelaDescarregamento.Situacao == SituacaoCargaJanelaDescarregamento.NaoComparecimento)
                cargaJanelaDescarregamento.QuantidadeNaoComparecimento = cargaJanelaDescarregamento.QuantidadeNaoComparecimento + 1;


            AdicionarIntegracaoComAtualizacao(cargaJanelaDescarregamento.Carga, _unitOfWork);
            repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento);
        }

        public void InserirHistoricoCargaJanelaDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento situacaoNova)
        {

            if (cargaJanelaDescarregamento.Situacao != situacaoNova)
            {

                Repositorio.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento repHistoricoSituacaoJanelaDescarregamento = new Repositorio.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento(_unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento historico = new Dominio.Entidades.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento()
                {
                    CargaJanelaDescarregamento = cargaJanelaDescarregamento,
                    DataAlteracao = DateTime.Now,
                    SituacaoAnterior = cargaJanelaDescarregamento.Situacao,
                    SituacaoNova = situacaoNova,
                };

                repHistoricoSituacaoJanelaDescarregamento.Inserir(historico);
            }
        }

        public async Task InserirHistoricoCargaJanelaDescarregamentoAsync(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento situacaoNova)
        {

            if (cargaJanelaDescarregamento.Situacao != situacaoNova)
            {

                Repositorio.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento repHistoricoSituacaoJanelaDescarregamento = new Repositorio.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento(_unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento historico = new Dominio.Entidades.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento()
                {
                    CargaJanelaDescarregamento = cargaJanelaDescarregamento,
                    DataAlteracao = DateTime.Now,
                    SituacaoAnterior = cargaJanelaDescarregamento.Situacao,
                    SituacaoNova = situacaoNova,
                };

                await repHistoricoSituacaoJanelaDescarregamento.InserirAsync(historico);
            }
        }



        public bool IsPermitirAdicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            try
            {
                ValidarPermissaoAdicionar(carga);
                return true;
            }
            catch (ServicoException)
            {
                return false;
            }
        }

        public int ObterTempoDescarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            if ((centroDescarregamento.TemposDescarregamento == null) || (centroDescarregamento.TemposDescarregamento.Count == 0))
                return 0;

            Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoDisponibilidadeDescarregamento = new Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(_unitOfWork);
            int quantidadeSKU = servicoDisponibilidadeDescarregamento.ObterSku(carga, centroDescarregamento.Destinatario);
            int tempoDescarregamento = (
                from o in centroDescarregamento.TemposDescarregamento
                where
                (
                    o.ModeloVeicular.Codigo == carga.ModeloVeicularCarga?.Codigo && o.TipoCarga.Codigo == carga.TipoDeCarga?.Codigo &&
                    ((!o.SkuDe.HasValue || o?.SkuDe <= quantidadeSKU) && (!o.SkuAte.HasValue || o?.SkuAte >= quantidadeSKU) || (!o.SkuDe.HasValue && !o.SkuAte.HasValue))
                )
                select o.Tempo
            ).FirstOrDefault();

            return tempoDescarregamento;
        }

        public void RemoverPorCargaEmAgrupamento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarParaRemover(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in listaCargaJanelaDescarregamento)
            {
                cargaJanelaDescarregamento.CentroDescarregamento = null;
                repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarTodasPorCarga(cargaAtual.Codigo, retornarCanceladas: true);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in listaCargaJanelaDescarregamento)
            {
                cargaJanelaDescarregamento.Carga = cargaNova;
                repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento);
            }
        }

        public void DefinirHorarioPorAgendamentoColeta(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento = repCargaJanelaDescarregamento.BuscarTodasPorCarga(agendamentoColeta.Carga.Codigo, retornarCanceladas: false);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in listaCargaJanelaDescarregamento)
            {
                try
                {
                    DefinirHorarioDescarregamento(cargaJanelaDescarregamento, agendamentoColeta.DataEntrega.Value, null);
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaJanelaDescarregamento, null, $"Alterou o horário de descarregamento para {cargaJanelaDescarregamento.InicioDescarregamento:dd/MM/yyyy HH:mm} após emissão do CTE.", _unitOfWork);
                }
                catch (Exception ex)
                {
                    Log.TratarErro(ex, "DescarregamentoFinalizacaoCarga");
                }
            }
        }

        public void AdicionarIntegracaoComAtualizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoRaster = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoRasterAtualizacao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.RasterAtualizacao);

            if (tipoIntegracaoRaster == null || tipoIntegracaoRasterAtualizacao == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoRaster = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracaoRaster.Codigo);
            
            if (cargaIntegracaoRaster != null && string.IsNullOrEmpty(cargaIntegracaoRaster.PreProtocolo))
                return;

            AdicionarCargaIntegracaoRasterAtualizacao(carga, tipoIntegracaoRasterAtualizacao);
            AdicionarCargaCargaIntegracaoRasterAtualizacao(carga, tipoIntegracaoRasterAtualizacao);
        }

        private void EnviarEmailConfirmacaoAgendamentoColeta(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware clienteMultisoftware)
        {
            //alterar consulta para buscar apenas os emails do remetente.. (criar metodo unico no servico)
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailConfirmacaoAgendamentoColeta(agendamentoColeta, emails, clienteMultisoftware);
        }

        private void EnviarEmailConfirmacaoAgendamentoEntrega(Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware clienteMultisoftware)
        {
            //alterar consulta para buscar apenas os emails do remetente.. (criar metodo unico no servico)
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorCarga(agendamentoEntrega.Carga.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailConfirmacaoAgendamentoEntrega(agendamentoEntrega, emails, clienteMultisoftware);
        }

        #endregion
    }
}
