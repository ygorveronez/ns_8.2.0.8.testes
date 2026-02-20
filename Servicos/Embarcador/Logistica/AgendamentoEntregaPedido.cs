using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica
{
    public class AgendamentoEntregaPedido
    {

        #region Atributos Privados ReadOnly

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.Embarcador.Pedidos.Pedido _repositorio;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Dominio.Entidades.Usuario _usuario;

        #endregion

        #region Atributos Privados

        Dominio.Entidades.ConfiguracaoEmail _configuracaoEmail;

        #endregion

        #region Construtores
        public AgendamentoEntregaPedido(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _repositorio = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
        }

        public AgendamentoEntregaPedido(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario)
        {
            _unitOfWork = unitOfWork;
            _configuracaoEmbarcador = configuracao;
            _repositorio = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
            _usuario = usuario;
        }

        #endregion

        #region Métodos Públicos

        public void AguardandoRetornoCliente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.SituacaoAgendamentoEntregaPedido != SituacaoAgendamentoEntregaPedido.ReagendamentoSolicitado && pedido.SituacaoAgendamentoEntregaPedido != SituacaoAgendamentoEntregaPedido.AguardandoAgendamento)
                throw new ServicoException("A situação atual do agendamento não permite a alteração.");

            pedido.DataSolicitacaoAgendamento = DateTime.Now;
            pedido.SituacaoAgendamentoEntregaPedido = SituacaoAgendamentoEntregaPedido.AguardandoRetornoCliente;
            _repositorio.Atualizar(pedido);
        }

        public void AjustarStatusCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            if (cargaJanelaCarregamento?.Carga?.Pedidos == null)
                return;

            List<Dominio.Entidades.Cliente> destinatarios = cargaJanelaCarregamento.Carga.Pedidos.Select(obj => obj.ObterDestinatario()).Distinct().ToList();

            bool possuiDatasDivergentes = false;

            foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
            {
                if (cargaJanelaCarregamento.Carga.Pedidos.Where(obj => obj.Pedido.DataAgendamento.HasValue && obj.ObterDestinatario() == destinatario).Select(obj => obj.Pedido.DataAgendamento.Value).Distinct().ToList().Count > 1)
                    possuiDatasDivergentes = true;
            }

            cargaJanelaCarregamento.DatasAgendadasDivergentes = possuiDatasDivergentes;

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        public void AlterarDataPrevisaoEntrega(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (!pedido.DataTerminoCarregamento.HasValue)
                return;

            DateTime dataPrevisaoEntregaInicial = pedido.DataTerminoCarregamento.Value;
            bool somenteEmDiasUteis = (pedido.TipoOperacao?.ConsiderarApenasDiasUteisNaPrevisaoDeEntrega ?? false);
            int quantidadeDiasAdicionar = (pedido.RotaFrete.TempoDeViagemEmMinutos / 1440);

            if (dataPrevisaoEntregaInicial.TimeOfDay > pedido.RotaFrete.HoraLimiteSaidaCD)
                quantidadeDiasAdicionar++;

            if (somenteEmDiasUteis && (dataPrevisaoEntregaInicial.DayOfWeek == DayOfWeek.Saturday))
                quantidadeDiasAdicionar++;

            DateTime dataPrevisaoEntrega;
            List<DateTime> datasComFeriado;
            int diasAdicionar = 0;
            int diasAdicionados = 0;

            if (somenteEmDiasUteis)
            {
                Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(_unitOfWork);
                int diasBuscarFeriado = Math.Max(quantidadeDiasAdicionar * 2, 30);

                datasComFeriado = servicoFeriado.ObterDatasComFeriado(dataPrevisaoEntregaInicial, dataPrevisaoEntregaInicial.AddDays(diasBuscarFeriado));
            }
            else
                datasComFeriado = new List<DateTime>();

            while (true)
            {
                dataPrevisaoEntrega = dataPrevisaoEntregaInicial.AddDays(diasAdicionar++);

                if (datasComFeriado.Contains(dataPrevisaoEntrega))
                    continue;

                if (somenteEmDiasUteis && ((dataPrevisaoEntrega.DayOfWeek == DayOfWeek.Saturday) || (dataPrevisaoEntrega.DayOfWeek == DayOfWeek.Sunday)))
                    continue;

                if (diasAdicionados == quantidadeDiasAdicionar)
                    break;

                diasAdicionados++;
            }

            if (datasComFeriado.Contains(dataPrevisaoEntrega))
                dataPrevisaoEntrega = dataPrevisaoEntrega.AddDays(1);

            pedido.PrevisaoEntrega = dataPrevisaoEntrega;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidosAgendamento(List<int> codigos, List<int> codigosEntregas)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new();

            if (codigosEntregas?.Count > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
                pedidos = repCargaEntregaPedido.BuscarPedidosPorEntregas(codigosEntregas);
            }
            else if (codigos.Count > 0)
                pedidos = _repositorio.ObterAgendamentoEntregaPedidos(codigos);

            return pedidos;
        }

        public List<PeriodosAgendamentoEntregaPedido> ObterPeriodosDescargaDestinatario(double cpfCnpj)
        {
            List<PeriodosAgendamentoEntregaPedido> listaRetorno = new List<PeriodosAgendamentoEntregaPedido>();

            if (cpfCnpj == 0)
                return listaRetorno;

            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodo = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(_unitOfWork);
            List<(DiaSemana Dia, TimeSpan Inicio, TimeSpan Fim)> periodos = repositorioPeriodo.BuscarPorDestinatario(cpfCnpj);

            foreach ((DiaSemana Dia, TimeSpan Inicio, TimeSpan Fim) periodo in periodos)
            {
                if (!listaRetorno.Any(obj => obj.Dia == periodo.Dia))
                    listaRetorno.Add(new PeriodosAgendamentoEntregaPedido()
                    {
                        Dia = periodo.Dia,
                        Periodos = new List<PeriodoAgendamentoEntregaHorario>()
                    });

                PeriodosAgendamentoEntregaPedido elementoDia = listaRetorno.Where(obj => obj.Dia == periodo.Dia).Select(obj => obj).FirstOrDefault();
                elementoDia.Periodos.Add(new PeriodoAgendamentoEntregaHorario
                {
                    Inicio = periodo.Inicio,
                    Fim = periodo.Fim
                });
            }

            return listaRetorno;
        }

        public void SalvarDataSugestaoEntrega(DateTime data, int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true);

            if (carga == null)
                throw new ServicoException("A carga não foi encontrada.");

            carga.DataSugestaoEntrega = data;
            carga.DataAlteracaoSugestaoEntrega = DateTime.Now;
            repositorioCarga.Atualizar(carga);

            Auditoria.Auditoria.Auditar(_auditado, carga, carga.GetChanges(), "Alterou data de sugestão de entrega", _unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork).BuscarPorCarga(codigoCarga);
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                SituacaoAgendamentoEntregaPedido? oldSituacaoAgendamentoEntregaPedido = pedido.SituacaoAgendamentoEntregaPedido;
                Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, $"Alterado a Data Sugestão de Entrega para {data.ToString("dd/MM/yyyy HH:mm")}", _unitOfWork);
                SalvarHistoricoAgendamentoEntregaPedido(pedido, carga.DataSugestaoEntrega, $"Alterado a Data Sugestão de Entrega para {data.ToString("dd/MM/yyyy HH:mm")}", null, TipoHistoricoAgendamento.SugestaoDeAgendamento, _usuario, null, oldSituacaoAgendamentoEntregaPedido);
            }
        }

        public void SalvarHorarioAgendamento(InformacoesAgendamentoEntregaPedido informacoesAgendamentoEntrega, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega repositorioConfiguracaoAgendamentoEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega configuracaoAgendamentoEntrega = repositorioConfiguracaoAgendamentoEntrega.BuscarPrimeiroRegistro();

            bool permiteReagendamentoRetroativo = configuracaoAgendamentoEntrega.PermiteInformarDataDeAgendamentoEReagendamentoRetroativamente;

            if (informacoesAgendamentoEntrega.Pedidos?.Count <= 0 || !informacoesAgendamentoEntrega.DataAgendamento.HasValue)
                return;

            if (informacoesAgendamentoEntrega.Pedidos.Any(obj => obj.SituacaoAgendamentoEntregaPedido == SituacaoAgendamentoEntregaPedido.Finalizado))
                throw new ServicoException("Não é possível agendar descarga para agendamentos já finalizados.");

            if (informacoesAgendamentoEntrega.DataAgendamento < DateTime.Now && !permiteReagendamentoRetroativo)
            {
                throw new ServicoException("A data de agendamento/reagendamento não pode ser retroativa.");
            }

            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Logistica.MotivoReagendamento repositorioMotivoReagendamento = new Repositorio.Embarcador.Logistica.MotivoReagendamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> adicionalPedidos = repositorioPedidoAdicional.BuscarPorPedidos(informacoesAgendamentoEntrega.Pedidos.Select(x => x.Codigo).ToList());
            Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento motivoReagendamento = informacoesAgendamentoEntrega.CodigoMotivoReagendamento > 0 ? repositorioMotivoReagendamento.BuscarPorCodigo(informacoesAgendamentoEntrega.CodigoMotivoReagendamento, false) : null;

            string mensagemAuditoria = "Atualizou data de entrega.";

            if (informacoesAgendamentoEntrega.DataAgendamento < DateTime.Now)
            {
                mensagemAuditoria += " (Data retroativa)";
            }

            DateTime dataSolicitacao = DateTime.Now;
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in informacoesAgendamentoEntrega.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = (from pa in adicionalPedidos where pa.Pedido.Codigo == pedido.Codigo select pa).FirstOrDefault();

                if ((pedido.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.ExigirNumeroIsisReturnParaAgendarEntrega ?? false) && (pedidoAdicional?.ISISReturn == 0 || pedidoAdicional == null))
                    throw new ServicoException($"O Pedido {pedido.NumeroPedidoEmbarcador} não possui número ISIS return.");

                pedido.Initialize();

                if (!pedido.DataPrimeiroAgendamento.HasValue)
                    pedido.DataPrimeiroAgendamento = informacoesAgendamentoEntrega.DataAgendamento;

                pedido.ObservacaoSolicitacaoReagendamento = informacoesAgendamentoEntrega.Observacoes;
                pedido.DataAgendamento = informacoesAgendamentoEntrega.DataAgendamento;
                pedido.SenhaAgendamento = informacoesAgendamentoEntrega.SenhaEntregaAgendamento;
                pedido.ObservacaoReagendamento = informacoesAgendamentoEntrega.ObservacaoReagendamento;
                pedido.ResponsavelMotivoReagendamento = informacoesAgendamentoEntrega.ResponsavelMotivoReagendamentoPedidos;
                pedido.DataSolicitacaoAgendamentoDescarga = dataSolicitacao;
                SituacaoAgendamentoEntregaPedido? oldSituacaoAgendamentoEntregaPedido = pedido.SituacaoAgendamentoEntregaPedido;
                pedido.SituacaoAgendamentoEntregaPedido = informacoesAgendamentoEntrega.Reagendamento ? SituacaoAgendamentoEntregaPedido.Reagendado : SituacaoAgendamentoEntregaPedido.Agendado;
                pedido.MotivoReagendamento = motivoReagendamento;
                pedido.Usuario = _usuario;
                _repositorio.Atualizar(pedido);

                if (informacoesAgendamentoEntrega.Reagendamento)
                    mensagemAuditoria += $" Motivo do Reagendamento: {motivoReagendamento?.Descricao ?? string.Empty}.{(!string.IsNullOrWhiteSpace(informacoesAgendamentoEntrega.ObservacaoReagendamento) ? $" Observação: {informacoesAgendamentoEntrega.ObservacaoReagendamento}" : string.Empty)}";

                Auditoria.Auditoria.Auditar(_auditado, pedido, pedido.GetChanges(), mensagemAuditoria, _unitOfWork);
                SalvarHistoricoAgendamentoEntregaPedido(pedido, pedido.DataAgendamento, informacoesAgendamentoEntrega.Reagendamento ? pedido.ObservacaoReagendamento : pedido.ObservacaoSolicitacaoReagendamento, pedido.MotivoReagendamento, informacoesAgendamentoEntrega.Reagendamento ? TipoHistoricoAgendamento.Reagendamento : TipoHistoricoAgendamento.Agendamento, _usuario, pedido.ResponsavelMotivoReagendamento, oldSituacaoAgendamentoEntregaPedido);
            }

            GerarIntegracaoDriveIn(_unitOfWork, informacoesAgendamentoEntrega.Pedidos, cliente, motivoReagendamento == null ? GatilhoIntegracaoMondelezDrivin.AgendamentoEntrega : GatilhoIntegracaoMondelezDrivin.ReagendamentoEntrega);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarPorPedidos(informacoesAgendamentoEntrega.Pedidos.Select(obj => obj.Codigo).ToList());
            Carga.ControleEntrega.ControleEntrega.AtualizarDataAgendamentoPorPedido(cargasEntrega, informacoesAgendamentoEntrega.Pedidos, _unitOfWork, informacoesAgendamentoEntrega.SenhaEntregaAgendamento);

            AlterarDataJanelaCarregamento(informacoesAgendamentoEntrega.Pedidos, informacoesAgendamentoEntrega.SalvarComDataRetroativa);
        }

        public void SetarAguardandoReagendamento(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            if (pedidos?.Count <= 0)
                return;

            if (pedidos.Any(obj => obj.SituacaoAgendamentoEntregaPedido == SituacaoAgendamentoEntregaPedido.Finalizado))
                throw new ServicoException("Não é possível reaagendar descargas para agendamentos já finalizados.");

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                pedido.Initialize();

                pedido.DataAgendamento = null;

                pedido.SituacaoAgendamentoEntregaPedido = SituacaoAgendamentoEntregaPedido.AguardandoReagendamento;

                _repositorio.Atualizar(pedido);

                Auditoria.Auditoria.Auditar(_auditado, pedido, pedido.GetChanges(), "Setou como aguardando reagendamento", _unitOfWork);
            }
        }

        public void SetarRotaPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido == null)
                throw new ServicoException("O pedido não foi encontrado.");

            pedido.Initialize();

            Dominio.Entidades.RotaFrete rotaFrete = BuscarRota(pedido);

            if (rotaFrete == null)
                throw new ServicoException("Não foram encontradas rotas para as configurações do pedido.");

            pedido.RotaFrete = rotaFrete;

            AlterarDataPrevisaoEntrega(pedido);

            _repositorio.Atualizar(pedido);

            Auditoria.Auditoria.Auditar(_auditado, pedido, pedido.GetChanges(), "Alterou a rota", _unitOfWork);
        }

        public void SolicitarAgendamento(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            DateTime dataSolicitacao = DateTime.Now;
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                pedido.SituacaoAgendamentoEntregaPedido = SituacaoAgendamentoEntregaPedido.AguardandoRetornoCliente;
                pedido.DataSolicitacaoAgendamento = dataSolicitacao;
                repositorioPedido.Atualizar(pedido);
            }

            EnviarEmailAgendamentoSolicitado(pedidos);

            repositorioPedido.AdicionarInformacaoContatoCliente(pedidos.Select(obj => obj.Codigo).ToList(), _usuario.Nome, "Agendamento Solicitado");
        }

        public void SolicitarReagendamento(string observacao, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                pedido.SituacaoAgendamentoEntregaPedido = SituacaoAgendamentoEntregaPedido.ReagendamentoSolicitado;
                pedido.ObservacaoSolicitacaoReagendamento = observacao;
                _repositorio.Atualizar(pedido);
            }
        }

        public void SugerirDataReagendamento(InformacoesAgendamentoEntregaPedido informacoesAgendamentoEntrega)
        {
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in informacoesAgendamentoEntrega.Pedidos)
            {
                SituacaoAgendamentoEntregaPedido? oldSituacaoAgendamentoEntregaPedido = pedido.SituacaoAgendamentoEntregaPedido;
                pedido.DataSugestaoReagendamentoDescarga = informacoesAgendamentoEntrega.DataAgendamento;
                _repositorio.Atualizar(pedido);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, $"Alterado a Data Sugestão de Reagendamento para {pedido.DataSugestaoReagendamentoDescarga?.ToString("dd/MM/yyyy HH:mm")}", _unitOfWork);
                SalvarHistoricoAgendamentoEntregaPedido(pedido,
                    pedido.DataSugestaoReagendamentoDescarga,
                    informacoesAgendamentoEntrega.ObservacaoReagendamento,
                    new Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento() { Codigo = informacoesAgendamentoEntrega.CodigoMotivoReagendamento },
                    TipoHistoricoAgendamento.SugestaoDeReagendamento,
                    _usuario,
                    informacoesAgendamentoEntrega.ResponsavelMotivoReagendamentoPedidos,
                    oldSituacaoAgendamentoEntregaPedido);
            }
        }

        public void SalvarSituacaoNotaFiscal(int codigoCargaPedido, DateTime data, int codigoNotaFiscalSituacao)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao repositorioNotaFiscalSituacao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXmlNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedido(codigoCargaPedido);

            if (listaPedidoXmlNotaFiscal.Count == 0)
                throw new ServicoException("Nenhuma nota fiscal foi encontrada para o pedido.");

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao = repositorioNotaFiscalSituacao.BuscarPorCodigo(codigoNotaFiscalSituacao, false);

            if (notaFiscalSituacao == null)
                throw new ServicoException("O registro da situação da nota fiscal não foi encontrado.");

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in listaPedidoXmlNotaFiscal)
            {
                SalvarSituacaoEDataXMLNotaFiscal(pedidoXMLNotaFiscal, data, notaFiscalSituacao);
            }

            if (notaFiscalSituacao.FinalizarAgendamentoEntregaPedido)
            {
                FinalizarAgendamentoPorSituacaoNotaFiscal(listaPedidoXmlNotaFiscal.FirstOrDefault().CargaPedido.Pedido);
            }
        }

        public void AssumirAgendamento(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Usuario usuario, DateTime data)
        {
            pedido.UsuarioAssumiuAgendamento = usuario;
            pedido.DataAssumiuAgendamento = data;

            _repositorio.Atualizar(pedido, _auditado);
        }

        public void InformarDadosAgendamento(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, string nome, string senha, DateTime data, string protocolo)
        {
            pedido.NomeResponsavelAgendamentoCliente = nome;
            pedido.SenhaAgendamento = senha;
            pedido.DataAgendamento = data;
            pedido.ProtocoloAgendamento = protocolo;
            if (pedido.DataPrimeiroAgendamento == null)
                pedido.DataPrimeiroAgendamento = data;

            _repositorio.Atualizar(pedido, _auditado);
        }

        public void GerarIntegracaoDriveIn(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, GatilhoIntegracaoMondelezDrivin gatilho, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = null)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (!repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Mondelez))
                return;

            Servicos.Embarcador.Integracao.Mondelez.IntegracaoMondelez servicoIntegracaoMondelez = new Servicos.Embarcador.Integracao.Mondelez.IntegracaoMondelez(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                Dominio.ObjetosDeValor.Embarcador.Mondelez.ParametrosGeracaoIntegracaoDrivin parametrosGeracaoIntegracaoDrivin = new Dominio.ObjetosDeValor.Embarcador.Mondelez.ParametrosGeracaoIntegracaoDrivin()
                {
                    Gatilho = gatilho,
                    Pedido = pedido,
                    ClienteMultisoftware = cliente,
                    TipoOcorrencia = tipoOcorrencia
                };

                servicoIntegracaoMondelez.GerarRegistroIntegracaoDrivin(parametrosGeracaoIntegracaoDrivin);
            }
        }

        public void InformarTipoAgendamentoEntrega(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, TipoAgendamentoEntrega tipoAgendamentoEntrega)
        {
            pedido.TipoAgendamentoEntrega = tipoAgendamentoEntrega;
            _repositorio.Atualizar(pedido, _auditado);

        }

        public void GravarDataEnvioEmailNotificacaoTransportador(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, DateTime dataEnvioEmail, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido ped in pedidos)
            {
                ped.DataeHoraEnvioEmailNotificacaoAgendamentoTransportador = dataEnvioEmail;
                repositorioPedido.Atualizar(ped);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ped, $"Envio de email de notificação ao Transportador", unitOfWork);
            }
        }

        public string BuscarCorpoEmailPorModelo(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork, int codigoCarga = 0, int codigoPedido = 0, double codigoCliente = 0)
        {
            if (modeloEmail.TipoModeloEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloEmail.Padrao) return PreencherCorpoEmailAgendamentoPadrao(cargas?[0], pedidos?[0], unitOfWork);

            #region Definição das Tags

            List<Dominio.ObjetosDeValor.Email.TagValorAgendamento> TagValor = new List<Dominio.ObjetosDeValor.Email.TagValorAgendamento>();
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagNumeroPedido", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagNumeroPedidoCliente", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagNumeroNotaFiscal", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCarga", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagDataAgendamento", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagRazaoSocialRemetente", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCNPJRemetente", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagEnderecoRemetente", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagComplementoRemetente", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagBairroRemetente", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCidadeUFRemetente", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagTelefoneRemetente", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagRazaoSocialDestinatario", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCNPJDestinatario", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagEnderecoDestinatario", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagComplementoDestinatario", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagBairroDestinatario", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCidadeUFDestinatario", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagTelefoneDestinatario", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagRazaoSocialTransportador", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCNPJTransportador", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagEnderecoTransportador", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagComplementoTransportador", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagBairroTransportador", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCidadeUFTransportador", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagTelefoneTransportador", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagDataSugestaoEntrega", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCodigoIntegracaoFilial", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagTipoCarga", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCodigoIntegracaoDestinatarioPedido", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagCanalClientes", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagQtdVolumesCarga", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagSenhaEntregaAgendamento", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagDataHoraAgendamentoColeta", Valor = "" });
            TagValor.Add(new Dominio.ObjetosDeValor.Email.TagValorAgendamento() { Tag = "#TagSenhaAgendamentoColeta", Valor = "" });

            #endregion

            return MontarEmail(TagValor, pedidos, cargas, modeloEmail, unitOfWork);
        }

        public string MontarEmail(List<Dominio.ObjetosDeValor.Email.TagValorAgendamento> TagValor, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail, Repositorio.UnitOfWork unitOfWork)
        {
            ObterDadosTags(pedidos, TagValor, cargas, unitOfWork);
            return TrocarTagsPorValor(modeloEmail.Corpo + "<br><br>" + modeloEmail.RodaPe, TagValor);
        }

        public void EnviarEmailAgendamento(Dominio.ObjetosDeValor.Email.Mensagem mensagem)
        {
            Dominio.Entidades.ConfiguracaoEmail configuracaoEmail = new Repositorio.ConfiguracaoEmail(_unitOfWork).BuscarConfiguracao();
            Servicos.Email serEmail = new Servicos.Email(_unitOfWork);
            if (!serEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, "", "", "", mensagem.Assunto, mensagem.Corpo, configuracaoEmail.Smtp, null, "", configuracaoEmail.RequerAutenticacaoSmtp, string.Empty, configuracaoEmail.PortaSmtp, _unitOfWork, 0, false, mensagem.Destinatarios))
                throw new ServicoException("Ocorreu uma falha ao enviar email");
        }

        #endregion

        #region Métodos Privados

        private void AlterarDataJanelaCarregamento(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, bool salvarComDataRetroativa)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            List<int> codigosCargas = repositorioCarga.BuscarCodigosPorPedidos(pedidos.Select(p => p.Codigo).ToList());

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarJanelasNaoConfirmadasPeloTransportadorPorCargas(codigosCargas);

            if (cargasJanelaCarregamento.Count == 0)
                return;

            if (pedidos.Any(obj => obj.TipoOperacao?.PermitirAgendarEntregaSomenteAposInicioViagemCarga ?? false))
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosValidar = pedidos.Where(obj => obj.TipoOperacao?.PermitirAgendarEntregaSomenteAposInicioViagemCarga ?? false).ToList();

                if (pedidosValidar.Any(obj => !obj.DataTerminoCarregamento.HasValue))
                    throw new ServicoException($"O pedido {pedidosValidar.FirstOrDefault().NumeroPedidoEmbarcador} exige que o carregamento tenha sido finalizado para que o agendamento seja realizado.");
            }

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
            {
                PermitirHorarioCarregamentoComLimiteAtingido = true
            };
            CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new CargaJanelaCarregamentoDisponibilidade(_unitOfWork, _configuracaoEmbarcador, configuracaoDisponibilidadeCarregamento);

            List<(int CodigoJanelaCarregamento, DateTime HorarioAntigo)> codigosJanelasHorarios = new List<(int CodigoJanelaCarregamento, DateTime HorarioAntigo)>();
            List<string> cargasComDataRetroativa = new List<string>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento in cargasJanelaCarregamento)
            {
                if (janelaCarregamento.Carga.TipoOperacao != null && janelaCarregamento.Carga.TipoOperacao.BloquearLiberacaoCargasComPedidosDatasAgendadasDivergentes)
                    AjustarStatusCargaJanelaCarregamento(janelaCarregamento);

                if (!(janelaCarregamento.Carga.TipoOperacao?.AlterarDataJanelaCarregamentoAoAtualizarDataAgendamentoEntregaPedido ?? false))
                    continue;

                if (janelaCarregamento.Carga.TipoOperacao?.AlterarDataJanelaCarregamentoAoAtualizarDataAgendamentoEntregaPedido ?? false)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = (from obj in janelaCarregamento.Carga.Pedidos where obj.Pedido.DataAgendamento.HasValue select obj.Pedido).OrderBy(obj => obj.DataAgendamento).FirstOrDefault();

                    if (pedidoBase != null)
                    {
                        DateTime novoHorarioInicio = pedidoBase.DataAgendamento.Value.AddMinutes(-pedidoBase.RotaFrete?.ObterTempoViagemEmMinutos() ?? 0);

                        if (novoHorarioInicio.DayOfWeek == DayOfWeek.Sunday)
                            novoHorarioInicio = novoHorarioInicio.AddDays(-1);

                        if (!salvarComDataRetroativa && novoHorarioInicio <= DateTime.Now && servicoDisponibilidadeCarregamento.IsDataRetroativaDentroDaToleranciaValida(novoHorarioInicio, janelaCarregamento.CentroCarregamento))
                        {
                            cargasComDataRetroativa.Add(janelaCarregamento.Carga.CodigoCargaEmbarcador);
                            continue;
                        }

                        DateTime antigoHorarioInicio = janelaCarregamento.InicioCarregamento;

                        janelaCarregamento.HorarioEncaixado = false;

                        servicoDisponibilidadeCarregamento.AlterarHorarioCarregamento(janelaCarregamento, novoHorarioInicio, _tipoServicoMultisoftware);

                        codigosJanelasHorarios.Add((janelaCarregamento.Codigo, antigoHorarioInicio));
                    }
                }
            }

            if (cargasComDataRetroativa.Count > 0)
                throw new ServicoException($"A(s) carga(s) {string.Join(", ", cargasComDataRetroativa)} terão data de início de carregamento retroativas.", errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.DataCarregamentoRetroativa);

            Task.Factory.StartNew(() => EnviarEmailsDescarregamento(pedidos.Select(obj => obj.Codigo).ToList(), codigosJanelasHorarios, _unitOfWork.StringConexao));
        }

        private Dominio.Entidades.RotaFrete BuscarRota(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);

            return repositorioRotaFrete.BuscarPorOrigemDestinoTipoOperacaoTransportador(pedido.Origem?.Codigo ?? 0, pedido.Destino?.Codigo ?? 0, pedido.TipoOperacao?.Codigo ?? 0, pedido.Empresa?.Codigo ?? 0);
        }

        private void EnviarEmailAgendamentoSolicitado(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            _configuracaoEmail = ObterConfiguracaoEmail(_unitOfWork);

            string mensagemErro;
            string message;

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                if (string.IsNullOrWhiteSpace(pedido.Destinatario?.Email))
                    throw new ServicoException($"O Cliente {pedido.Destinatario?.Descricao} não possui e-mail cadastrado.");

                message = ObterBodyEmailNotificacaoClienteSolicitacaoAgendamento(pedidos);
                Servicos.Email.EnviarEmail(_configuracaoEmail.Email, _configuracaoEmail.Email, _configuracaoEmail.Senha, pedido.Destinatario.Email, null, null, $"Solicitação de Agendamento de Entrega", message, _configuracaoEmail.Smtp, out mensagemErro, "", null, "", false, "", _configuracaoEmail.PortaSmtp, _unitOfWork);
            }
        }

        private void EnviarEmailsDescarregamento(List<int> codigosPedidos, List<(int CodigoJanelaCarregamento, DateTime HorarioAntigo)> codigosJanelasHorarios, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.AtualizarAtual);

            try
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork).BuscarPorCodigos(codigosPedidos);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork).BuscarPorCodigos(codigosJanelasHorarios.Select(obj => obj.CodigoJanelaCarregamento).ToList());
                List<(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento JanelaCarregamento, DateTime HorarioAntigo)> janelasHorarios = new List<(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento JanelaCarregamento, DateTime HorarioAntigo)>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamento)
                    janelasHorarios.Add((cargaJanelaCarregamento, (from o in codigosJanelasHorarios where o.CodigoJanelaCarregamento == cargaJanelaCarregamento.Codigo select o.HorarioAntigo).FirstOrDefault()));

                EnviarNotificacaoEmailTransportadorAlteracaoDataAgendamento(pedidos, unitOfWork);
                EnviarNotificacaoEmailTransportadorAlteracaoDataJanelaCarregamento(janelasHorarios, unitOfWork);
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Erro ao enviar e-mails alteração data descarga agendamento entrega pedido {excecao.Message}.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void EnviarNotificacaoEmailTransportadorAlteracaoDataAgendamento(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorPedidos(pedidos.Select(p => p.Codigo).ToList());

            if (notasFiscais.Count == 0)
                return;

            _configuracaoEmail = ObterConfiguracaoEmail(unitOfWork);

            string mensagemErro;
            string message;
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal notaFiscal;

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in (from o in pedidos where o.TipoOperacao != null && o.TipoOperacao.NotificarTransportadorAoAgendarEntrega && notasFiscais.Any(nf => nf.CargaPedido.Pedido.Codigo == o.Codigo) select o))
            {
                notaFiscal = notasFiscais.Where(nf => nf.CargaPedido.Pedido.Codigo == pedido.Codigo).FirstOrDefault();
                message = ObterBodyEmailNotificacaoTransportadorAlteracaoDataAgendamento(notaFiscal);
                Servicos.Email.EnviarEmail(_configuracaoEmail.Email, _configuracaoEmail.Email, _configuracaoEmail.Senha, notaFiscal.CargaPedido.Carga.Empresa.Email, null, null, $"Entrega Agendada (Nota: {notaFiscal.XMLNotaFiscal.Numero}, Destinatário: {notaFiscal.CargaPedido.Pedido.Destinatario?.Nome})", message, _configuracaoEmail.Smtp, out mensagemErro, "", null, "", false, "", _configuracaoEmail.PortaSmtp, _unitOfWork);
            }
        }

        private void EnviarNotificacaoEmailTransportadorAlteracaoDataJanelaCarregamento(List<(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento JanelaCarregamento, DateTime HorarioAntigo)> janelasHorarios, Repositorio.UnitOfWork unitOfWork)
        {
            foreach ((Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento JanelaCarregamento, DateTime HorarioAntigo) janelaHorario in janelasHorarios)
            {
                if (janelaHorario.JanelaCarregamento.CargaBase.Empresa == null)
                    continue;

                _configuracaoEmail = ObterConfiguracaoEmail(unitOfWork);

                string message = ObterBodyEmailNotificacaoTransportadorAlteracaoDataCarregamento(janelaHorario.JanelaCarregamento, janelaHorario.HorarioAntigo);
                string mensagemErro;
                Servicos.Email.EnviarEmail(_configuracaoEmail.Email, _configuracaoEmail.Email, _configuracaoEmail.Senha, janelaHorario.JanelaCarregamento.CargaBase.Empresa.Email, null, null, $"Alteração da data de horário de carregamento da carga {janelaHorario.JanelaCarregamento.Carga.CodigoCargaEmbarcador}", message, _configuracaoEmail.Smtp, out mensagemErro, "", null, "", false, "", _configuracaoEmail.PortaSmtp, _unitOfWork);
            }
        }

        private string ObterBodyEmailNotificacaoTransportadorAlteracaoDataCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime horarioAntigoCarregamento)
        {
            return $@"Olá {cargaJanelaCarregamento.CargaBase.Empresa.NomeFantasia}, {System.Environment.NewLine}
                            O horário de carregamento da carga {cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador} 
                            foi alterado de {horarioAntigoCarregamento.ToString("dd/MM/yyyy HH:mm")} 
                            para {cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm")}.";
        }

        private string ObterBodyEmailNotificacaoTransportadorAlteracaoDataAgendamento(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlNotaFiscal)
        {
            return $"Prezado transportador {xmlNotaFiscal.CargaPedido.Carga.Empresa?.NomeFantasia}, o pedido {xmlNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador} contendo a nota fiscal {xmlNotaFiscal.XMLNotaFiscal.Numero} teve seu agendamento confirmado para {xmlNotaFiscal.CargaPedido.Pedido.DataAgendamento?.ToString("dd/MM/yyyy HH:mm")} no cliente {xmlNotaFiscal.CargaPedido.Pedido.Destinatario?.Nome}. {System.Environment.NewLine} Observação: {xmlNotaFiscal.CargaPedido.Pedido.ObservacaoAdicional}.";
        }

        private string ObterBodyEmailNotificacaoClienteSolicitacaoAgendamento(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> xmlNotasFiscais = repositorioPedidoXmlNotaFiscal.BuscarPorPedidos(pedidos.Select(p => p.Codigo).ToList());
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorPedido(pedidos.FirstOrDefault().Codigo);

            string numeroCarga = carga.CodigoCargaEmbarcador;
            string notasFiscais = string.Join(", ", (from obj in xmlNotasFiscais select obj.XMLNotaFiscal.Numero).ToList());
            string quantidadeVolumes = xmlNotasFiscais.Sum(obj => obj.XMLNotaFiscal.Volumes).ToString();
            string transportadora = carga.Empresa?.Descricao;
            string dataSugerida = carga.DataSugestaoEntrega?.ToString("dd/MM/yyyy");

            return $@"Olá,{Environment.NewLine}Espero que esteja bem.{Environment.NewLine}
                      Prezado cliente, segue a solicitação de agendamento de entrega das mercadorias da Mattel, referente as notas fiscais abaixo:{Environment.NewLine}
                      Carga: {numeroCarga}
                      N° Nota Fiscal: {notasFiscais}
                      Quantidade de Volumes: {quantidadeVolumes}
                      Transportadora: {transportadora}
                      Data Sugerida: {dataSugerida}{Environment.NewLine}{Environment.NewLine}
                      Aguardamos confirmação da data e hora do agendamento.";
        }

        private Dominio.Entidades.ConfiguracaoEmail ObterConfiguracaoEmail(Repositorio.UnitOfWork unitOfWork)
        {
            if (_configuracaoEmail == null)
                return new Repositorio.ConfiguracaoEmail(unitOfWork).BuscarConfiguracao();

            return _configuracaoEmail;
        }

        private void SalvarSituacaoEDataXMLNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, DateTime data, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            pedidoXMLNotaFiscal.XMLNotaFiscal.Initialize();
            pedidoXMLNotaFiscal.CargaPedido.Pedido.Initialize();

            pedidoXMLNotaFiscal.XMLNotaFiscal.DataNotaFiscalSituacao = data;
            pedidoXMLNotaFiscal.XMLNotaFiscal.NotaFiscalSituacao = notaFiscalSituacao;

            pedidoXMLNotaFiscal.CargaPedido.Pedido.SetExternalChange(pedidoXMLNotaFiscal.XMLNotaFiscal.GetChangeByPropertyName("NotaFiscalSituacao"));

            repositorioXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal.XMLNotaFiscal);
            Auditoria.Auditoria.Auditar(_auditado, pedidoXMLNotaFiscal.CargaPedido.Pedido, pedidoXMLNotaFiscal.CargaPedido.Pedido.GetChanges(), "Alterou a situação da nota fiscal", _unitOfWork);
        }

        private void FinalizarAgendamentoPorSituacaoNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            pedido.SituacaoAgendamentoEntregaPedido = SituacaoAgendamentoEntregaPedido.Finalizado;
            repositorioPedido.Atualizar(pedido);

            Auditoria.Auditoria.Auditar(_auditado, pedido, "Finalizou agendamento automaticamente ao alterar a situação da nota fiscal", _unitOfWork);
        }

        private string PreencherCorpoEmailAgendamentoPadrao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            decimal qtdPallets = 0;
            decimal cubico = 0;
            decimal peso = 0;

            Dominio.Entidades.Usuario usuario = new Repositorio.Usuario(unitOfWork).BuscarPorCodigo(pedido.UsuarioAssumiuAgendamento?.Codigo ?? 0);
            Dominio.Entidades.Usuario usuarioVendedor = new Repositorio.Usuario(unitOfWork).BuscarPorCodigo(pedido.FuncionarioVendedor?.Codigo ?? 0);

            string dataPrevisao = string.Empty;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork).BuscarPorCargaEPedido(carga.Codigo, pedido.Codigo);
            if (cargaPedido != null)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork).BuscarPorCargaPedido(cargaPedido.Codigo);
                dataPrevisao = cargaEntregaPedido?.CargaEntrega?.DataPrevista?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty;
            }

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork).BuscarNotasFiscaisPorPedido(pedido.Codigo);

            StringBuilder corpoEmail = new StringBuilder();
            corpoEmail.AppendLine($"<span style=\"width: 100%; display: inline-block; text-align: center; color: red;\"><span>Mensagem eletrônica para solicitação de agendamento, </span><span style=\"font-weight: bold; text-decoration: underline;\">não retorne esse Email.</span></span>");
            corpoEmail.AppendLine($"<br/><span style=\"width: 100%; display: inline-block; text-align: center; color: red;\">O retorno do agendamento deve ser enviado para:</span>");
            corpoEmail.AppendLine($"<br/><span style=\"width: 100%; display: inline-block; text-align: center; font-weight: bold;\">{usuario?.Nome ?? string.Empty}, Tel: {usuario?.Telefone_Formatado ?? string.Empty}, email: {usuario?.Email ?? string.Empty}</span>");

            corpoEmail.AppendLine($"<br/>");

            corpoEmail.AppendLine($"<span style=\"width: 100%; display: inline-block;\"/>");
            corpoEmail.AppendLine($"<span style=\"align: center;\">");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Nº Pedido: </span><span>{pedido.NumeroPedidoEmbarcador}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Nº Pedido Cliente: </span><span>{pedido.CodigoPedidoCliente ?? string.Empty}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Nº NF-e: </span><span>{(notas != null && notas.Count > 0 ? notas.Select(o => o.Numero.ToString()).Aggregate((i, j) => i + ", " + j) : string.Empty)}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Nº Carga: </span><span>{carga.CodigoCargaEmbarcador ?? string.Empty}</span>");
            corpoEmail.AppendLine($"</span>");

            corpoEmail.AppendLine($"<br/>");


            corpoEmail.AppendLine("<table style=\"width: 1000px; display: inline-block;\">");

            corpoEmail.AppendLine("<tr>");

            corpoEmail.AppendLine("<td style=\"width: 33%;\">");
            corpoEmail.AppendLine($"<span style=\"display: inline-block; padding-right: 20px;\">");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Remetente:</span>");
            corpoEmail.AppendLine($"<br/><span>{pedido.Remetente.Nome}</span>");
            if (pedido.UsarOutroEnderecoOrigem)
                corpoEmail.AppendLine($"<br/><span>{pedido.EnderecoOrigem?.Localidade?.DescricaoCidadeEstado ?? string.Empty}, {pedido.EnderecoOrigem?.Localidade?.Pais?.Nome ?? string.Empty} CEP: {pedido.EnderecoOrigem?.CEP ?? string.Empty}</span>");
            else
                corpoEmail.AppendLine($"<br/><span>{pedido.Remetente?.Localidade?.DescricaoCidadeEstado ?? string.Empty}, {pedido.Remetente?.Localidade?.Pais?.Nome ?? string.Empty} CEP: {pedido.Remetente?.CEP ?? string.Empty}</span>");
            corpoEmail.AppendLine($"<br/><span>Telefone: {pedido.Remetente?.Telefone1?.ObterTelefoneFormatado() ?? string.Empty}</span>");
            corpoEmail.AppendLine($"</span>");
            corpoEmail.AppendLine("</td>");

            corpoEmail.AppendLine("<td style=\"width: 33%;\">");
            corpoEmail.AppendLine($"<span style=\"display: inline-block; padding-right: 20px;\">");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Destinatário:</span>");
            corpoEmail.AppendLine($"<br/><span>{pedido.Destinatario?.Nome ?? string.Empty}</span>");
            if (pedido.UsarOutroEnderecoDestino)
                corpoEmail.AppendLine($"<br/><span>{pedido.EnderecoDestino?.Localidade?.DescricaoCidadeEstado ?? string.Empty}, {pedido.EnderecoDestino?.Localidade?.Pais?.Nome ?? string.Empty} CEP: {pedido.EnderecoDestino?.CEP ?? string.Empty}</span>");
            else
                corpoEmail.AppendLine($"<br/><span>{pedido.Destinatario?.Localidade?.DescricaoCidadeEstado ?? string.Empty}, {pedido.Destinatario.Localidade.Pais.Nome ?? string.Empty} CEP: {pedido.Destinatario.CEP ?? string.Empty}</span>");
            corpoEmail.AppendLine($"<br/><span>Telefone: {pedido.Destinatario?.Telefone1?.ObterTelefoneFormatado() ?? string.Empty}</span>");
            corpoEmail.AppendLine($"</span>");
            corpoEmail.AppendLine("</td>");

            corpoEmail.AppendLine("<td style=\"width: 33%;\">");
            corpoEmail.AppendLine($"<span style=\"display: inline-block\">");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Transportadora:</span>");
            corpoEmail.AppendLine($"<br/><span>{carga.Empresa?.RazaoSocial ?? string.Empty}</span>");
            corpoEmail.AppendLine($"<br/><span>{carga.Empresa?.Localidade?.DescricaoCidadeEstado ?? string.Empty}, {carga.Empresa?.Localidade?.Pais.Nome ?? string.Empty} CEP: {carga.Empresa?.CEP ?? string.Empty}</span>");
            corpoEmail.AppendLine($"<br/><span>Telefone: {carga.Empresa?.Telefone ?? string.Empty}</span>");
            corpoEmail.AppendLine($"</span>");

            corpoEmail.AppendLine("</td>");
            corpoEmail.AppendLine("</tr>");

            corpoEmail.AppendLine("</table>");

            corpoEmail.AppendLine($"<br/>");

            corpoEmail.AppendLine($"<br/><span style=\"width: 100%; display: inline-block; font-weight: bold;\">Representante: {usuarioVendedor?.Nome ?? string.Empty}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"width: 100%; display: inline-block; font-weight: bold;\">Data previsão de entrega: {dataPrevisao}</span>");

            corpoEmail.AppendLine($"<br/>");

            corpoEmail.AppendLine("<table style =\"margin: 30px 0 30px 0; border: 1px solid #b9b5b5; border-collapse: collapse; border-collapse: collapse;\">");
            corpoEmail.AppendLine("<thead style=\"background-color: #d9e1f2; color: black;\">");

            corpoEmail.AppendLine("<tr>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Código Integração </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Descrição  </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Quantidade </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Peso </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Peso Total </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Qtd. Pallet </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Pallet Fechado </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> M³ </th>");
            corpoEmail.AppendLine("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Preço Unitário </th>");
            corpoEmail.AppendLine("</tr>");

            corpoEmail.AppendLine("</thead>");
            corpoEmail.AppendLine("<tbody>");

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in pedido.Produtos)
            {
                corpoEmail.AppendLine("<tr>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{produto.Produto.CodigoProdutoEmbarcador}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{produto.Produto.Descricao}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{produto.Quantidade.ToString("N3")}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{produto.Produto.PesoUnitario.ToString("N3")}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{produto.PesoProduto.ToString("N3")}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{produto.QuantidadePalet.ToString("N3")}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{(produto.PalletFechado ? "Sim" : "Não")}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{produto.MetroCubico.ToString("N4")}</td>");
                corpoEmail.AppendLine($"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{produto.PrecoUnitario.ToString("N3")}</td>");
                corpoEmail.AppendLine("</tr>");

                qtdPallets += produto.QuantidadePalet;
                cubico += produto.MetroCubico;
                peso += produto.PesoProduto;
            }

            corpoEmail.AppendLine("</tbody>");
            corpoEmail.AppendLine("</table>");

            corpoEmail.AppendLine($"<br/><span style=\"width: 100%; display: inline-block; font-weight: bold;font-size: 20px\">Totais:</span>");
            corpoEmail.AppendLine($"<br/>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Palete: </span><span>{qtdPallets.ToString("N3")}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Mix de itens: </span><span>{pedido.Produtos.Select(o => o.Produto.CodigoProdutoEmbarcador).Distinct().Count()}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">M³: </span><span>{cubico.ToString("N4")}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Peso: </span><span>{peso.ToString("N3")}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Valor NF-e: </span><span>{(notas != null && notas.Count > 0 ? notas.Sum(o => o.Valor).ToString("N3") : string.Empty)}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Nº NF-e: </span><span>{(notas != null && notas.Count > 0 ? notas.Select(o => o.Numero.ToString()).Aggregate((i, j) => i + ", " + j) : string.Empty)}</span>");
            corpoEmail.AppendLine($"<br/><span style=\"font-weight: bold;\">Chave NF-e: </span><span>{(notas != null && notas.Count > 0 && notas.Select(o => o.Chave).Where(o => !string.IsNullOrEmpty(o)).Count() > 0 ? notas.Select(o => o.Chave).Aggregate((i, j) => i + ", " + j) : string.Empty)}</span>");

            return corpoEmail.ToString();
        }

        private string TrocarTagsPorValor(string texto, List<Dominio.ObjetosDeValor.Email.TagValorAgendamento> listaTagValor)
        {
            foreach (Dominio.ObjetosDeValor.Email.TagValorAgendamento tagValor in listaTagValor.Where(t => texto.Contains(t.Tag)))
                texto = texto.Replace(tagValor.Tag, tagValor.Valor);

            return texto;
        }

        private void SalvarHistoricoAgendamentoEntregaPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, DateTime? dataAgenda, string observacao, Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento motivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoHistoricoAgendamento tipoHistoricoAgendamento, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega responsavel, SituacaoAgendamentoEntregaPedido? oldSituacaoAgendamentoEntregaPedido)
        {
            Repositorio.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido repositorioHistoricoAgendamentoEntregaPedido = new Repositorio.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido historioAgendamento = new Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido();
            historioAgendamento.Tipo = tipoHistoricoAgendamento;
            historioAgendamento.DataHoraAgenda = dataAgenda;
            historioAgendamento.Pedido = pedido;
            historioAgendamento.Observacao = observacao;
            historioAgendamento.MotivoReagendamento = motivo;
            historioAgendamento.ResponsavelMotivoReagendamento = responsavel;
            historioAgendamento.Usuario = usuario;
            historioAgendamento.SituacaoAgendamentoEntregaPedido = oldSituacaoAgendamentoEntregaPedido;

            repositorioHistoricoAgendamentoEntregaPedido.Inserir(historioAgendamento);
        }

        private List<Dominio.ObjetosDeValor.Email.TagValorAgendamento> ObterDadosTags(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, List<Dominio.ObjetosDeValor.Email.TagValorAgendamento> listaTagValor, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaBase = cargas[0];
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(cargaBase.Codigo);
            Dominio.Entidades.Empresa transportador = cargaBase.Empresa;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = pedidos[0];
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork).BuscarNotasFiscaisPorPedidos(pedidos.Select(p => p.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork).BuscarPorPedidos(pedidos.Select(p => p.Codigo).ToList());
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCarga(cargaBase.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCargaEPedido(cargaBase.Codigo, pedidoBase.Codigo);

            string valor = string.Empty;
            foreach (Dominio.ObjetosDeValor.Email.TagValorAgendamento tagValor in listaTagValor)
            {
                try
                {
                    valor = string.Empty;

                    switch (tagValor.Tag)
                    {
                        case "#TagNumeroPedido":
                            valor = string.Join(", ", pedidos.Select(p => p.NumeroPedidoEmbarcador));
                            break;

                        case "#TagNumeroPedidoCliente":
                            valor = string.Join(", ", pedidos.Select(p => p.CodigoPedidoCliente));
                            break;

                        case "#TagNumeroNotaFiscal":
                            valor = string.Join(", ", notas.Select(nota => nota.Numero));
                            break;

                        case "#TagCarga":
                            valor = string.Join(", ", pedidos.Select(p => p.CodigoCargaEmbarcador));
                            break;

                        case "#TagDataAgendamento":
                            valor = string.Join(", ", pedidos.Select(p => p.DataAgendamento?.ToString("dd/MM/yyyy HH:mm:ss") ?? ""));
                            break;

                        case "#TagRazaoSocialRemetente":
                            valor = pedidoBase.Remetente?.Nome ?? string.Empty;
                            break;

                        case "#TagCNPJRemetente":
                            valor = pedidoBase.Remetente?.CPF_CNPJ_Formatado ?? string.Empty;
                            break;

                        case "#TagEnderecoRemetente":
                            if (pedidoBase.UsarOutroEnderecoOrigem)
                                valor = pedidoBase.EnderecoOrigem?.Endereco ?? string.Empty;
                            else
                                valor = pedidoBase.Remetente?.Endereco ?? string.Empty;
                            break;

                        case "#TagComplementoRemetente":
                            if (pedidoBase.UsarOutroEnderecoOrigem)
                                valor = pedidoBase.EnderecoOrigem?.Complemento ?? string.Empty;
                            else
                                valor = pedidoBase.Remetente?.Complemento ?? string.Empty;
                            break;

                        case "#TagBairroRemetente":
                            if (pedidoBase.UsarOutroEnderecoOrigem)
                                valor = pedidoBase.EnderecoOrigem?.Bairro ?? string.Empty;
                            else
                                valor = pedidoBase.Remetente?.Bairro ?? string.Empty;
                            break;

                        case "#TagCidadeUFRemetente":
                            if (pedidoBase.UsarOutroEnderecoOrigem)
                                valor = pedidoBase.EnderecoOrigem?.Localidade?.DescricaoCidadeEstado ?? string.Empty;
                            else
                                valor = pedidoBase.Remetente?.Localidade?.DescricaoCidadeEstado ?? string.Empty;
                            break;

                        case "#TagTelefoneRemetente":
                            valor = pedidoBase.Remetente?.Telefone1?.ObterTelefoneFormatado() ?? string.Empty;
                            break;

                        case "#TagRazaoSocialDestinatario":
                            valor = pedidoBase.Destinatario?.Nome ?? string.Empty;
                            break;

                        case "#TagCNPJDestinatario":
                            valor = pedidoBase.Destinatario?.CPF_CNPJ_Formatado ?? string.Empty;
                            break;

                        case "#TagEnderecoDestinatario":
                            if (pedidoBase.UsarOutroEnderecoDestino)
                                valor = pedidoBase.EnderecoDestino?.Endereco ?? string.Empty;
                            else
                                valor = pedidoBase.Destinatario?.Endereco ?? string.Empty;
                            break;

                        case "#TagComplementoDestinatario":
                            if (pedidoBase.UsarOutroEnderecoDestino)
                                valor = pedidoBase.EnderecoDestino?.Complemento ?? string.Empty;
                            else
                                valor = pedidoBase.Destinatario?.Complemento ?? string.Empty;
                            break;

                        case "#TagBairroDestinatario":
                            if (pedidoBase.UsarOutroEnderecoDestino)
                                valor = pedidoBase.EnderecoDestino?.Bairro ?? string.Empty;
                            else
                                valor = pedidoBase.Destinatario?.Bairro ?? string.Empty;
                            break;

                        case "#TagCidadeUFDestinatario":
                            if (pedidoBase.UsarOutroEnderecoDestino)
                                valor = pedidoBase.EnderecoDestino?.Localidade?.DescricaoCidadeEstado ?? string.Empty;
                            else
                                valor = pedidoBase.Destinatario?.Localidade?.DescricaoCidadeEstado ?? string.Empty;
                            break;

                        case "#TagTelefoneDestinatario":
                            valor = pedidoBase.Destinatario?.Telefone1?.ObterTelefoneFormatado() ?? string.Empty;
                            break;

                        case "#TagRazaoSocialTransportador":
                            valor = transportador?.RazaoSocial ?? string.Empty;
                            break;

                        case "#TagCNPJTransportador":
                            valor = transportador?.CNPJ_Formatado ?? string.Empty;
                            break;

                        case "#TagEnderecoTransportador":
                            valor = transportador?.Endereco ?? string.Empty;
                            break;

                        case "#TagComplementoTransportador":
                            valor = transportador?.Complemento ?? string.Empty;
                            break;

                        case "#TagBairroTransportador":
                            valor = transportador?.Bairro ?? string.Empty;
                            break;

                        case "#TagCidadeUFTransportador":
                            valor = transportador?.Localidade?.DescricaoCidadeEstado ?? string.Empty;
                            break;

                        case "#TagTelefoneTransportador":
                            valor = transportador?.Telefone ?? string.Empty;
                            break;

                        case "#TagDataSugestaoEntrega":
                            valor = string.Join(", ", cargas.Distinct().Select(c => $"Carga: {c.CodigoCargaEmbarcador} => Data: {(c.DataSugestaoEntrega.Value.ToString() ?? string.Empty)}"));
                            break;

                        case "#TagCodigoIntegracaoFilial":
                            valor = "Cargas sem filial definida";
                            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasComFilial = cargas?.Where(c => c.Filial?.OutrosCodigosIntegracao != null).Distinct().ToList() ?? new();
                            if (cargasComFilial.Count > 0)
                                valor = string.Join(", ", cargasComFilial.Select(c => $"Carga: {c.CodigoCargaEmbarcador} => Filial: {string.Join(", ", c.Filial.OutrosCodigosIntegracao)}"));
                            break;

                        case "#TagTipoCarga":
                            valor = string.Join(", ", cargas?.Distinct().Select(c => $"Carga: {c.CodigoCargaEmbarcador} => Tipo: {c.TipoDeCarga?.Descricao ?? string.Empty}"));
                            break;

                        case "#TagCodigoIntegracaoDestinatarioPedido":
                            valor = string.Join(", ", pedidos.Select(p => $"Pedido: {p.NumeroPedidoEmbarcador} => Destinatário: {p.Destinatario?.CodigoIntegracao ?? string.Empty}"));
                            break;

                        case "#TagCanalClientes":
                            List<string> descricoes = pedidos.Select(p =>
                            {
                                string descricaoCompleta = p.Destinatario?.MesoRegiao?.Descricao ?? string.Empty;

                                // Verifica se a descrição contém o pipe '|'
                                string descricaoFiltrada = descricaoCompleta.Contains('|')
                                    ? descricaoCompleta.Split('|').FirstOrDefault() ?? string.Empty
                                    : descricaoCompleta;

                                return "Pedido: " + p.NumeroPedidoEmbarcador + " => Canal: " + descricaoFiltrada;
                            }).ToList();

                            if (descricoes != null && descricoes.Any())
                            {
                                valor = string.Join(", ", descricoes);
                            }
                            break;

                        case "#TagQtdVolumesCarga":
                            valor = pedidos.Sum(x => x.QtVolumes).ToString();
                            break;

                        case "#TagStatus":
                            valor = cargaJanelaDescarregamento?.Situacao.ObterDescricao() ?? SituacaCargaJanelaDescarregamentoHelper.ObterDescricao(SituacaoCargaJanelaDescarregamento.Cancelado);
                            break;

                        case "#TagDataColeta":
                            valor = pedidoBase.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty;
                            break;

                        case "#TagRazaoSocialRecebedor":
                            valor = cargaPedido.Recebedor?.Nome ?? pedidoBase.Recebedor?.Nome;
                            break;

                        case "#TagCNJPRecebedor":
                            valor = cargaPedido.Recebedor?.CPF_CNPJ_Formatado ?? pedidoBase.Recebedor?.CPF_CNPJ_Formatado;
                            break;

                        case "#TagEnderecoRecebedor":
                            valor = cargaPedido.Recebedor?.Endereco ?? pedidoBase.Recebedor?.Endereco;
                            break;

                        case "#TagSenhaEntregaAgendamento":
                            if (cargaEntregas.Count > 0)
                                valor = string.Join(", ", cargaEntregas.Select(x => x.SenhaEntrega).Distinct());
                            break;

                        case "#TagDataHoraAgendamentoColeta":
                            if (cargaJanelaDescarregamento != null)
                                valor = cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty;
                            break;

                        case "#TagSenhaAgendamentoColeta":
                            if (agendamento != null)
                                valor = agendamento.Senha ?? string.Empty;
                            break;


                        default: break;
                    }

                    if (!string.IsNullOrEmpty(valor))
                        tagValor.Valor = valor;
                }
                catch (Exception ex) 
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar tag no agendamento de entrega - continuando processamento: {ex.ToString()}", "CatchNoAction");
                } //Enterra para seguir nas demais tags.
            }

            return listaTagValor;
        }

        #endregion
    }
}

