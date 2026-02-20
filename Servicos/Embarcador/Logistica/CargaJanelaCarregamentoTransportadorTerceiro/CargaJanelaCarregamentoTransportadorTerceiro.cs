using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoTransportadorTerceiro
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoTransportadorTerceiro(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CargaJanelaCarregamentoTransportadorTerceiro(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void DefinirDataDisponibilizacaoTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.DataDisponibilizacaoTransportadores.HasValue)
                return;

            cargaJanelaCarregamento.DataDisponibilizacaoTransportadores = DateTime.Now;

            new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork).Atualizar(cargaJanelaCarregamento);
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

        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao> ObterListaTempoEsperaPorPontuacao()
        {
            Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao repositorioTempoEsperaPorPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao(_unitOfWork);

            return repositorioTempoEsperaPorPontuacao.BuscarTodos();
        }

        public void SalvarHistoricoAlteracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, string mensagem)
        {
            SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, mensagem, usuario: null);
        }

        public void SalvarHistoricoAlteracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, string mensagem, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repositorioHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico historico = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico()
            {
                CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador,
                Data = DateTime.Now,
                Descricao = mensagem,
                Tipo = TipoCargaJanelaCarregamentoTransportadorHistorico.RegistroAlteracao,
                Usuario = usuario
            };

            repositorioHistorico.Inserir(historico);
        }

        private void ValidarCargasInteressadas(Dominio.Entidades.Cliente terceiro, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos, int numeroPallet)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaInteresse = repositorioCargaJanelaCarregamentoTransportador.BuscarCargaComInteressePorCargaTransportardorTerceiro(terceiro?.CPF_CNPJ ?? 0, numeroPallet, modelosVeicularesPermitidos);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaInteresse)
            {
                try
                {
                    ValidarPermissaoMarcarInteresseCarga(terceiro, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga);
                }
                catch (ServicoException)
                {
                    cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Disponivel;
                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Removido o interesse na carga");
                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento);
                }
            }
        }

        private void InformarCargaAtualizadaEmbarcador(int codigoCarga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, string adminStringConexao)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
            try
            {
                if (cliente == null)
                    return;

                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repConfig = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repConfig.BuscarPorClienteETipo(cliente.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                if (clienteURLAcesso == null)
                    return;

                string urlJanelaCarregamento = "http://" + clienteURLAcesso.URLAcesso;

                if (clienteURLAcesso.URLAcesso.Contains("192.168.0.125"))
                    urlJanelaCarregamento += "/Embarcador";

                urlJanelaCarregamento += "/JanelaCarregamento/InformarCargaAtualizada?Carga=" + codigoCarga;

                WebRequest wRequest = WebRequest.Create(urlJanelaCarregamento);
                wRequest.Method = "GET";

                WebResponse response = wRequest.GetResponse();

                response.Close();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        private void RejeitarCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Cargas.Carga carga, string motivoRejeicaoCarga, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCarga(carga.Codigo);

            cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga = null;
            cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Rejeitada;
            cargaJanelaCarregamentoTransportador.NumeroRejeicoesManuais += 1;

            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento?.ExigirTransportadorInformarMotivoAoRejeitarCarga ?? false)
            {
                cargaJanelaCarregamentoTransportador.MotivoRejeicaoCarga = motivoRejeicaoCarga;
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaBase, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouCargaPeloMotivo, cargaJanelaCarregamentoTransportador.Transportador.NomeFantasia, cargaJanelaCarregamentoTransportador.MotivoRejeicaoCarga)), _unitOfWork);
            }

            carga.DataAtualizacaoCarga = DateTime.Now;
            carga.Terceiro = null;
            carga.RejeitadaPeloTransportador = true;

            if (agendamento != null && agendamento.Transportador != null)
            {
                string nomeTransportador = $"({agendamento.Transportador.CNPJ_Formatado}) {agendamento.Transportador.Descricao}";
                agendamento.Transportador = null;
                agendamento.EtapaAgendamentoColeta = (configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta || (carga.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.RemoverEtapaAgendamentoDoAgendamentoColeta ?? false)) ? EtapaAgendamentoColeta.NFe : EtapaAgendamentoColeta.DadosTransporte;
                repAgendamentoColeta.Atualizar(agendamento);

                //if (agendamento.Remetente != null && configuracaoEmbarcador.NotificarCargaAgConfirmacaoTransportador)
                    //NotificarCargaRejeitadaPorTransportador(agendamento, nomeTransportador);

                Servicos.Auditoria.Auditoria.Auditar(auditado, agendamento, null, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouaCarga, nomeTransportador)), _unitOfWork);
            }

            repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
            SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouCarga, usuario);
            repositorioCarga.Atualizar(carga);
        }

        #endregion

        #region Métodos Públicos

        public void DisponibilizarAutomaticamenteParaTransportadoresTerceiros(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento? tipoTransportador = null)
        {
            if (!(cargaJanelaCarregamento.CentroCarregamento?.LiberarCargaAutomaticamenteParaTransportadorasTerceiros ?? false))
                return;

            if ((cargaJanelaCarregamento.Carga.ModeloVeicularCarga == null) || (cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador != null))
                return;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento tipoTransportadorTerceiro = cargaJanelaCarregamento.CentroCarregamento.TipoTransportadorTerceiro;
            if (tipoTransportador != null)
                tipoTransportadorTerceiro = tipoTransportador.Value;

            DisponibilizarParaTransportadoresTerceiros(cargaJanelaCarregamento, tipoTransportadorTerceiro, tipoServicoMultisoftware);
        }

        public void DisponibilizarParaTransportadoresTerceiros(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TipoTransportadorCentroCarregamento tipoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool bloquearLiberacaoPorTipoCondicaoPagamento = cargaJanelaCarregamento.Carga.TipoCondicaoPagamento.HasValue && (cargaJanelaCarregamento.Carga.TipoCondicaoPagamento.Value == TipoCondicaoPagamento.FOB) && !(cargaJanelaCarregamento.Carga.TipoOperacao?.ConfiguracaoJanelaCarregamento?.PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores ?? false);
            bool bloquearLiberacaoPorTipoCarga = (cargaJanelaCarregamento.Carga.TipoDeCarga?.BloquearLiberacaoParaTransportadores ?? false);

            if (bloquearLiberacaoPorTipoCondicaoPagamento || bloquearLiberacaoPorTipoCarga)
                return;

            cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemValorFrete;

            DisponibilizarParaTransportadoresTerceirosPorTipo(cargaJanelaCarregamento, tipoTransportador, tipoServicoMultisoftware);
        }

        public void DisponibilizarParaTransportadoresTerceirosPorTipo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TipoTransportadorCentroCarregamento tipoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaJanelaCarregamento.Carga == null || cargaJanelaCarregamento.Carga.ModeloVeicularCarga == null || cargaJanelaCarregamento.Carga.TipoDeCarga == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
            decimal numeroPaletesPedidos = repositorioCargaPedido.BuscarNumeroPaletesPorCarga(cargaJanelaCarregamento.Carga?.Codigo ?? 0);
            int numeroPallet = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.NumeroPaletes ?? (int)numeroPaletesPedidos;
            List<double> codigosTransportadoresTerceiros = new List<double>();
            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repositorioTipoCargaModeloVeicular.ConsultarPorTipoCarga(cargaJanelaCarregamento.Carga.TipoDeCarga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = (from o in tipoCargaModeloVeicular select o.ModeloVeicularCarga).ToList();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracao, configuracaoJanelaCarregamento);

            switch (tipoTransportador)
            {
                case TipoTransportadorCentroCarregamento.Todos:
                    codigosTransportadoresTerceiros = repositorioCliente.BuscarCodigosTransportadoresTerceiros();
                    break;

                case TipoTransportadorCentroCarregamento.TodosCentroCarregamento:
                    if (cargaJanelaCarregamento.CentroCarregamento == null)
                        return;

                    codigosTransportadoresTerceiros = cargaJanelaCarregamento.CentroCarregamento.TransportadoresTerceiros.Select(o => o.Transportador.CPF_CNPJ).ToList();
                    break;

                case TipoTransportadorCentroCarregamento.TodosCentroCarregamentoComTipoVeiculoCarga:
                    if (cargaJanelaCarregamento.CentroCarregamento == null || cargaJanelaCarregamento.Carga.ModeloVeicularCarga == null)
                        return;

                    if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                        codigosTransportadoresTerceiros = repositorioVeiculo.BuscarCodigosTransportadoresTerceirosPorTipoVeiculoProprietario((from o in cargaJanelaCarregamento.CentroCarregamento.TransportadoresTerceiros select o.Transportador.CPF_CNPJ).ToList(), numeroPallet, modelosVeicularesPermitidos);
                    else
                        codigosTransportadoresTerceiros = repositorioVeiculo.BuscarCodigosTransportadoresTerceirosPorTipoVeiculo(cargaJanelaCarregamento.CentroCarregamento.Codigo, numeroPallet, modelosVeicularesPermitidos);
                    break;

                case TipoTransportadorCentroCarregamento.TodosComTipoVeiculoCarga:
                    codigosTransportadoresTerceiros = repositorioVeiculo.BuscarCodigosTransportadoresTerceirosPorTipoVeiculo(numeroPallet, modelosVeicularesPermitidos);
                    break;
                case TipoTransportadorCentroCarregamento.PrioridadePorFilaCarregamento:
                    codigosTransportadoresTerceiros = repositorioVeiculo.BuscarCodigosTransportadoresTerceirosPorFilaCarregamento(numeroPallet, modelosVeicularesPermitidos);
                    break;
            }

            List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao> listaTempoEsperaPorPontuacao = ObterListaTempoEsperaPorPontuacao();
            List<Dominio.Entidades.Cliente> transportadoresTerceiros = new List<Dominio.Entidades.Cliente>();

            if (codigosTransportadoresTerceiros.Count < 2000)
                transportadoresTerceiros = repositorioCliente.BuscarPorCodigos(codigosTransportadoresTerceiros);
            else
            {
                try
                {
                    decimal decimalBlocos = Math.Ceiling(((decimal)codigosTransportadoresTerceiros.Count) / 1000);
                    int blocos = (int)Math.Truncate(decimalBlocos);

                    for (int i = 0; i < blocos; i++)
                    {
                        Log.TratarErro($"blocos {codigosTransportadoresTerceiros.Count} indice {i}");
                        transportadoresTerceiros.AddRange(repositorioCliente.BuscarPorCodigos(codigosTransportadoresTerceiros.Skip(i * 1000).Take(1000).ToList()));
                    }
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);
                }
            }

            List<double> janelaTransportadores = repositorioCargaJanelaCarregamentoTransportador.BuscarTransportadoresTerceirosPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargajanelaCarregamentoTrasnportadorInserir = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            int tempoAguardarInteresse = 0;
            TipoCargaJanelaCarregamentoTransportador tipoTransportadorJanelaCarregamento = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga;

            if (cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.TipoTransportadorTerceiroSecundario.HasValue
                && (cargaJanelaCarregamento.CentroCarregamento.TipoTransportadorTerceiroSecundario.Value != tipoTransportador)
                && cargaJanelaCarregamento.CentroCarregamento.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente > 0)
            {
                tempoAguardarInteresse = cargaJanelaCarregamento.CentroCarregamento.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente;
                tipoTransportadorJanelaCarregamento = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorTerceiroCargaSecundario;
            }

            foreach (Dominio.Entidades.Cliente transportadorTerceiroAdicionarJanela in transportadoresTerceiros)
            {
                if (janelaTransportadores.Contains(transportadorTerceiroAdicionarJanela.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador
                {
                    CargaJanelaCarregamento = cargaJanelaCarregamento,
                    HorarioLiberacao = DateTime.Now,
                    Terceiro = transportadorTerceiroAdicionarJanela,
                    Situacao = SituacaoCargaJanelaCarregamentoTransportador.Disponivel,
                    PendenteCalcularFrete = false,
                    Tipo = tipoTransportadorJanelaCarregamento,
                    HorarioLimiteConfirmarCarga = tempoAguardarInteresse > 0 ? DateTime.Now.AddMinutes(tempoAguardarInteresse) : null
                };

                listaCargajanelaCarregamentoTrasnportadorInserir.Add(cargaJanelaCarregamentoTransportador);
            }

            if (listaCargajanelaCarregamentoTrasnportadorInserir.Count > 0)
            {
                repositorioCargaJanelaCarregamentoTransportador.InsertSQLListaCargajanelaCarregamentoTrasnportador(listaCargajanelaCarregamentoTrasnportadorInserir);
                repositorioCargaJanelaCarregamentoTransportador.InsertSQLListaCargaJanelaCarregamentoTransportadoHistorico(cargaJanelaCarregamento.Codigo);
            }

            DefinirDataDisponibilizacaoTransportadores(cargaJanelaCarregamento);
        }

        public void ValidarPermissaoMarcarInteresseCarga(Dominio.Entidades.Cliente terceiro, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (configuracaoEmbarcador.NaoExigeInformarDisponibilidadeDeVeiculo)
                return;

            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);
            Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repositorioVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repositorioTipoCargaModeloVeicular.ConsultarPorTipoCarga(carga.TipoDeCarga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = (from o in tipoCargaModeloVeicular select o.ModeloVeicularCarga).ToList();
            int numeroVeiculosDisponiveisSemModelo = repositorioVeiculoDisponivelCarregamento.ContarNumeroVeiculosDisponiveisSemModeloTransportadorTerceiro(terceiro?.CPF_CNPJ ?? 0);
            int numeroPallet = carga.ModeloVeicularCarga.NumeroPaletes.HasValue ? carga.ModeloVeicularCarga.NumeroPaletes.Value : carga.Pedidos.Sum(o => o.Pedido.NumeroPaletes);

            if (carga.TipoOperacao?.ExigeQueVeiculoIgualModeloVeicularDaCarga ?? false)
            {
                numeroVeiculosDisponiveisSemModelo = 0;

                if (carga.ModeloVeicularCarga != null)
                    modelosVeicularesPermitidos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>() { carga.ModeloVeicularCarga };
            }

            int numeroVeiculosDisponiveisParaEssaCarga = repositorioVeiculoDisponivelCarregamento.ContarNumeroVeiculosDisponiveisPodemFazerCargaTransportadorTerceiro(numeroPallet, modelosVeicularesPermitidos, terceiro?.CPF_CNPJ ?? 0);
            int totalVeiculos = numeroVeiculosDisponiveisParaEssaCarga + numeroVeiculosDisponiveisSemModelo;

            if (totalVeiculos <= 0)
                throw new ServicoException($"Você não possui veículos disponíveis que possam ser utilizados para fazer esta carga ({carga.CodigoCargaEmbarcador}).");

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            int numeroCargasAguardandoConfirmacao = repositorioCargaJanelaCarregamentoTransportador.ContarNumeroCargasAguardandAceiteOuConfirmacaoPorTipoCargaEPalletsTransportadorTerceiro(numeroPallet, terceiro?.CPF_CNPJ ?? 0, modelosVeicularesPermitidos);

            if (numeroVeiculosDisponiveisParaEssaCarga > numeroCargasAguardandoConfirmacao)
                return;

            if (numeroVeiculosDisponiveisSemModelo > 0)
            {
                int totalCargaAguardandoAceiteOuConfirmacao = repositorioCargaJanelaCarregamentoTransportador.ContarPorSituacoesTransportadorTerceiro(new List<SituacaoCargaJanelaCarregamentoTransportador> { SituacaoCargaJanelaCarregamentoTransportador.AgAceite, SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao }, terceiro?.CPF_CNPJ ?? 0);

                if (totalVeiculos > totalCargaAguardandoAceiteOuConfirmacao)
                    return;
            }

            throw new ServicoException($"Não é possível marcar interesse nesta carga ({carga.CodigoCargaEmbarcador}), pois, existem cargas aguardando a confirmação que irão utilizar todos os possiveis veículos disponíveis para essa carga.");
        }

        public void ValidarCargasInteressadas(Dominio.Entidades.Cliente terceiro, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            int numeroPallet = modeloVeicular?.NumeroPaletes != null ? modeloVeicular.NumeroPaletes.Value : 0;

            ValidarCargasInteressadas(terceiro, modelosVeicularesPermitidos, numeroPallet);
        }

        public void ValidarCargasInteressadas(Dominio.Entidades.Cliente terceiro, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repositorioTipoCargaModeloVeicular.ConsultarPorTipoCarga(carga.TipoDeCarga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = (from o in tipoCargaModeloVeicular select o.ModeloVeicularCarga).ToList();
            int numeroPallet = carga.ModeloVeicularCarga?.NumeroPaletes != null ? carga.ModeloVeicularCarga.NumeroPaletes.Value : carga.Pedidos.Sum(obj => obj.Pedido.NumeroPaletes);

            ValidarCargasInteressadas(terceiro, modelosVeicularesPermitidos, numeroPallet);
        }

        public void DefinirTransportadorComValorFreteInformado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(_unitOfWork, configuracaoEmbarcador);
            Servicos.Embarcador.Carga.CargaOperador servicoCargaOperador = new Servicos.Embarcador.Carga.CargaOperador(_unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete servicoComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(_unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);

            cargaJanelaCarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;
            cargaJanelaCarregamento.Carga.Terceiro = cargaJanelaCarregamentoTransportador.Terceiro;
            cargaJanelaCarregamento.Carga.ValorFrete = cargaJanelaCarregamentoTransportador.ValorFreteTransportador;
            cargaJanelaCarregamento.Carga.ValorFreteOperador = cargaJanelaCarregamentoTransportador.ValorFreteTransportador;
            cargaJanelaCarregamento.Carga.TipoFreteEscolhido = configuracaoJanelaCarregamento.NaoPermitirRecalcularValorFreteInformadoPeloTransportador ? TipoFreteEscolhido.Embarcador : TipoFreteEscolhido.Operador;
            cargaJanelaCarregamento.Carga.OrigemFretePelaJanelaTransportador = true;

            if (usuario != null)
            {
                cargaJanelaCarregamento.Carga.OperadorContratouCarga = usuario;
                servicoCargaOperador.Atualizar(cargaJanelaCarregamento.Carga, usuario, tipoServicoMultisoftware);
            }

            if (cargaJanelaCarregamento.CentroCarregamento?.ManterComponentesTabelaFrete ?? false)
            {
                decimal teto = cargaJanelaCarregamento.CentroCarregamento?.PercentualMaximoDiferencaValorCotacao ?? 0m;
                decimal valorLimiteNaoSolicitarAprovacao = cargaJanelaCarregamento.Carga.ValorFreteTabelaFrete * (teto / 100.0m);

                if (cargaJanelaCarregamento.Carga.ValorFrete > valorLimiteNaoSolicitarAprovacao)
                    servicoCargaAprovacaoFrete.CriarAprovacao(cargaJanelaCarregamento.Carga, TipoRegraAutorizacaoCarga.InformadoManualmente, tipoServicoMultisoftware);
                else
                    servicoCargaAprovacaoFrete.RemoverAprovacao(cargaJanelaCarregamento.Carga);
            }
            else
            {
                cargaJanelaCarregamento.Carga.ValorFreteTabelaFrete = 0;
                cargaJanelaCarregamento.Carga.TabelaFrete = null;

                repositorioCargaComponentesFrete.DeletarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                if (cargaJanelaCarregamento.Carga.CargaAgrupada)
                    repositorioCargaComponentesFrete.DeletarPorCargaAgrupamento(cargaJanelaCarregamento.Carga.Codigo, false);

                servicoCargaAprovacaoFrete.RemoverAprovacao(cargaJanelaCarregamento.Carga);
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> componentesFrete = repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.BuscarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete componenteFrete in componentesFrete)
                servicoComponetesFrete.AdicionarComponenteFreteCarga(cargaJanelaCarregamento.Carga, componenteFrete.ComponenteFrete, componenteFrete.ValorComponente, componenteFrete.Percentual, false, componenteFrete.TipoValor, componenteFrete.ComponenteFrete.TipoComponenteFrete, null, true, false, componenteFrete.ModeloDocumentoFiscal, tipoServicoMultisoftware, usuario, _unitOfWork, false, TipoCargaComponenteFrete.Manual, false, false, null, false, componenteFrete.Moeda, componenteFrete.ValorCotacaoMoeda, componenteFrete.ValorTotalMoeda);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

            repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
            servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento.Carga, tipoServicoMultisoftware);
            servicoRateioFrete.RatearValorDoFrenteEntrePedidos(cargaJanelaCarregamento.Carga, cargaPedidos, configuracaoEmbarcador, false, _unitOfWork, tipoServicoMultisoftware, new Dominio.ObjetosDeValor.Embarcador.Carga.ConfiguracaoRateioValorFreteEntrePedidos() { ValorFreteInformadoPeloTransportador = true });
        }

        public void DefinirTransportadorSemValorFreteInformado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Servicos.Embarcador.Carga.CargaOperador servicoCargaOperador = new Servicos.Embarcador.Carga.CargaOperador(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);

            cargaJanelaCarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;
            cargaJanelaCarregamento.Carga.Terceiro = cargaJanelaCarregamentoTransportador.Terceiro;
            cargaJanelaCarregamento.Carga.OrigemFretePelaJanelaTransportador = true;

            if (usuario != null)
            {
                cargaJanelaCarregamento.Carga.OperadorContratouCarga = usuario;
                servicoCargaOperador.Atualizar(cargaJanelaCarregamento.Carga, usuario, tipoServicoMultisoftware);
            }

            if (!cargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete)
            {
                cargaJanelaCarregamento.Carga.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                cargaJanelaCarregamento.Carga.PossuiPendencia = false;
                cargaJanelaCarregamento.Carga.CalcularFreteSemEstornarComplemento = true;
                cargaJanelaCarregamento.Carga.MotivoPendencia = "";
                cargaJanelaCarregamento.Carga.DataInicioCalculoFrete = DateTime.Now;
                cargaJanelaCarregamento.Carga.CalculandoFrete = true;
            }

            repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
            servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        public void InformaCargaAtualizadaEmbarcador(int codigoCarga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, string adminStringConexao)
        {
            InformarCargaAtualizadaEmbarcador(codigoCarga, cliente, adminStringConexao);
        }

        public void RejeitarCarga(int codigoCarga, string motivoRejeicaoCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga select o).FirstOrDefault();

            if (cargaJanelaCarregamentoTransportadorReferencia == null)
                throw new ServicoException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoVinculadaAoTransportador);

            if ((cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgAceite) && (cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao))
                throw new ServicoException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ASituacaoDaCargaNaoPermiteRejeicao);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga;

            if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware))
                throw new ServicoException((string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AAtualSituacaoDaCargaNaoPermiteRejeicao, carga.DescricaoSituacaoCarga)));

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);

            List<Dominio.Entidades.Usuario> usuariosNotificacao = new List<Dominio.Entidades.Usuario>();
            List<string> emailsNotificacao = new List<string>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
            {
                RejeitarCarga(cargaJanelaCarregamentoTransportador, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga, motivoRejeicaoCarga, usuario, auditado);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento;
                bool shouldNotificarPorEmail = centroCarregamento?.EnviarNotificacoesPorEmail ?? false;
                bool notificarSomenteCargasRejeitadas = centroCarregamento?.EnviarNotificacoesCargasRejeitadasPorEmail ?? false;

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail> emailsCentroCarregamento = (shouldNotificarPorEmail || notificarSomenteCargasRejeitadas) ? centroCarregamento?.Emails.ToList() : null;
                List<Dominio.Entidades.Usuario> usuariosNotificacaoCarregamento = centroCarregamento?.UsuariosNotificacao.ToList() ?? new List<Dominio.Entidades.Usuario>();

                if (usuariosNotificacaoCarregamento.Count > 0)
                    usuariosNotificacao.AddRange(usuariosNotificacaoCarregamento);
                else if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador != null)
                    usuariosNotificacao.Add(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador);

                if (emailsCentroCarregamento != null && emailsCentroCarregamento.Count > 0)
                    emailsNotificacao.AddRange(emailsCentroCarregamento.Select(o => o.Email));

                if (!(centroCarregamento?.NaoEnviarNotificacaoCargaRejeitadaParaTransportador ?? false))
                    emailsNotificacao.Add(cargaJanelaCarregamentoTransportador.Transportador.Email ?? string.Empty);
            }

            if (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.RetornarJanelaCarregamentoParaAgLiberacaoParaTransportadoresAposRejeicaoDoTransportador ?? false)
            {
                servicoCargaJanelaCarregamento.AlterarSituacao(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores);
            }

            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaJanelaCarregamentoTransportadorReferencia, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.RejeitouCarga, _unitOfWork);

            System.Text.StringBuilder mensagemNotificacao = new System.Text.StringBuilder();

            string numeroCarga = servicoCarga.ObterNumeroCarga(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga, configuracaoEmbarcador);
            string dataCarregamento = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.DataCarregamentoProgramada != null ? cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.DataCarregamentoProgramada.ToDateTimeString() : string.Empty;
            string dataRejeicao = carga.DataAtualizacaoCarga != null ? carga.DataAtualizacaoCarga.Value.ToDateTimeString() : string.Empty;
            string rota = carga.Rota?.Descricao != null ? carga.Rota.Descricao : string.Empty;
            string pedido = string.Join(", ", cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga?.Pedidos.Select(o => o.Pedido.NumeroPedidoEmbarcador).ToList());

            mensagemNotificacao.Append($"{usuario.Empresa.RazaoSocial} rejeitou a carga {numeroCarga} de pedido número {pedido}.").AppendLine();
            mensagemNotificacao.Append($"Rota: {rota}.").AppendLine();
            mensagemNotificacao.Append($"Data Carregamento: {dataCarregamento}.").AppendLine();
            mensagemNotificacao.Append($"Data Rejeição: {dataRejeicao}.").AppendLine().AppendLine();
            mensagemNotificacao.Append($"Rota ofertada para outra transportadora!").AppendLine();
            mensagemNotificacao.Append($"Importante: Conforme contrato vigente, se o valor do frete da nova transportadora for maior poderá haver cobrança da diferença.");

            //NotificarUsuarios(cargaJanelaCarregamentoTransportadorReferencia, usuariosNotificacao, mensagemNotificacao.ToString(), cliente, tipoServicoMultisoftware, _unitOfWork, adminStringConexao);
            //NotificarEmails(cargaJanelaCarregamentoTransportadorReferencia, emailsNotificacao, mensagemNotificacao.ToString(), _unitOfWork);

            _unitOfWork.CommitChanges();

            cargaJanelaCarregamentoTransportadorReferencia = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(cargaJanelaCarregamentoTransportadorReferencia.Codigo);

            InformarCargaAtualizadaEmbarcador(carga.Codigo, cliente, adminStringConexao);
        }

        public void RejeitarJanelasCarregamentoTransportadorTerceiroPorTempoEncerrado(SituacaoCargaJanelaCarregamentoTransportador situacaoAlvo, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            List<int> listaCodigoCargaJanelaCarregamentoTransportador = situacaoAlvo switch
            {
                SituacaoCargaJanelaCarregamentoTransportador.Disponivel => repositorioCargaJanelaCarregamentoTransportador.BuscarCodigosPorTempoInteresseEncerradoTransportadorTerceiro(limiteRegistros: 5),
                _ => new List<int>()
            };

            if (listaCodigoCargaJanelaCarregamentoTransportador.Count == 0)
                return;

            if (!configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                return;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            CargaJanelaCarregamento servicoCargaJanelaCarregamento = new CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);
            CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new CargaJanelaCarregamentoTransportadorConsulta(_unitOfWork);
            Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Hubs.JanelaCarregamento();

            foreach (int codigoCargaJanelaCarregamentoTransportador in listaCodigoCargaJanelaCarregamentoTransportador)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(codigoCargaJanelaCarregamentoTransportador);

                    if (cargaJanelaCarregamentoTransportadorReferencia.Situacao != situacaoAlvo)
                        continue;

                    _unitOfWork.Start();

                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.Codigo, cargaJanelaCarregamentoTransportadorReferencia.Terceiro, retornarCargasOriginais: true);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                    {
                        RejeitarJanelaCarregamentoTransportadorTerceiroPorTempoConfirmacaoEncerrado(cargaJanelaCarregamentoTransportador, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga);
                        servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, tipoServicoMultisoftware);
                    }

                    Auditoria.Auditoria.Auditar(auditoria, cargaJanelaCarregamentoTransportadorReferencia, null, "Carga rejeitada por tempo de confirmação encerrado.", _unitOfWork);
                   
                    CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new CargaJanelaCarregamentoTransportadorTerceiro(_unitOfWork, configuracaoEmbarcador);

                    if (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.TipoTransportadorTerceiroSecundario.HasValue ?? false)
                    {
                        servicoCargaJanelaCarregamentoTransportadorTerceiro.DisponibilizarAutomaticamenteParaTransportadoresTerceiros(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, tipoServicoMultisoftware, cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.TipoTransportadorTerceiroSecundario);

                        servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, tipoServicoMultisoftware);

                    }

                    _unitOfWork.CommitChanges();

                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento);
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Log.TratarErro(excecao);
                }
            }
        }

        #endregion

        #region Métodos privados
        private void RejeitarJanelaCarregamentoTransportadorTerceiroPorTempoConfirmacaoEncerrado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);

            cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga = null;
            cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Rejeitada;

            carga.DataAtualizacaoCarga = DateTime.Now;
            carga.Empresa = null;
            carga.RejeitadaPeloTransportador = true;

            repositorioCarga.Atualizar(carga);
            repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
            SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga rejeitada para o transportador por tempo de confirmação encerrado");
        }
        #endregion
    }

}
