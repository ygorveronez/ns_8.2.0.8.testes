using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.WebService.ModeloDados
{
    public class ModeloDados
    {
        #region Propiedades Privadas

        readonly private Repositorio.UnitOfWork _unitOfWork;
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Constructores

        public ModeloDados(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Metodos Publicos

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Atendimento>> BuscarDadosAtendimentos(DateTime dataInicioCriacao, DateTime dataFimCriacao, int numeroAtendimento)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Atendimento> modeloDadosAtendimentos = repositorioChamado.ObterModeloDadosAtendimentos(dataInicioCriacao, dataFimCriacao, numeroAtendimento);

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Atendimento>>.CriarRetornoSucesso(modeloDadosAtendimentos);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Carga>> BuscarDadosCargas(DateTime dataAtualizacaoInicial, DateTime dataAtualizacaoFinal, string numeroCarga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Carga> modeloDadosCargas = repositorioCarga.ObterModeloDadosCargas(dataAtualizacaoInicial, dataAtualizacaoFinal, numeroCarga);

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Carga>>.CriarRetornoSucesso(modeloDadosCargas);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CargaEntrega>> BuscarDadosEntregas(DateTime dataAtualizacaoInicial, DateTime dataAtualizacaoFinal, string numeroCarga, string numeroPedido)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CargaEntrega> modeloDadosEntregas = repositorioCargaEntrega.ObterModeloDadosEntregas(dataAtualizacaoInicial, dataAtualizacaoFinal, numeroCarga, numeroPedido);

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CargaEntrega>>.CriarRetornoSucesso(modeloDadosEntregas);
        }

        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento>>> BuscarDadosMonitoramentoAsync(DateTime? dataCriacaoCargaInicial, DateTime? dataCriacaoCargaFinal, string numeroCarga, CancellationToken cancellationToken)
        {
            bool temNumeroCarga = !string.IsNullOrWhiteSpace(numeroCarga);

            if (!temNumeroCarga && (!dataCriacaoCargaInicial.HasValue || !dataCriacaoCargaFinal.HasValue))
            {
                throw new WebServiceException("Os campos dataCriacaoCargaInicial e dataCriacaoCargaFinal são obrigatórios quando o número da carga não é informado.");
            }

            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(_unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = temNumeroCarga
                ? await repositorioCarga.BuscarPorCodigoCargaEmbarcadorAsync(numeroCarga)
                : await repositorioCarga.BuscarPorPeriodoCriacaoAsync(dataCriacaoCargaInicial!.Value, dataCriacaoCargaFinal!.Value, 100);

            if (cargas == null || cargas.Count == 0)
                throw new WebServiceException("Nenhuma carga foi encontrada");

            List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos =
                await repositorioMonitoramento.BuscarPorCargasAsync(cargas.Select(c => c.Codigo).ToArray(), new string[] { nameof(Dominio.Entidades.Embarcador.Logistica.Monitoramento.StatusViagem) });

            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicos =
                (monitoramentos != null && monitoramentos.Count > 0)
                    ? await repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramentosAsync(monitoramentos.Select(m => m.Codigo).ToArray())
                    : new List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();

            Dictionary<int, List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>> monitoramentosPorCarga = monitoramentos
                .GroupBy(m => m.Carga.Codigo)
                .ToDictionary(g => g.Key, g => g.ToList());

            Dictionary<int, List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Historico>> historicosPorMonitoramento = historicos
                .Where(h => h.Monitoramento != null)
                .GroupBy(h => h.Monitoramento.Codigo)
                .ToDictionary(g => g.Key, g => g.Select(h => new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Historico
                {
                    Status = h.StatusViagem?.Descricao,
                    Tempo = h.TempoSegundos.HasValue ? h.Tempo.ToString() : null
                }).ToList());

            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento> listaDadosMonitoramento = new List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento>();
            foreach (var carga in cargas)
            {
                List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Monitoramento> listaMonitoramentos = new List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Monitoramento>();

                if (!monitoramentosPorCarga.TryGetValue(carga.Codigo, out List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosDaCarga))
                    monitoramentosDaCarga = new List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

                foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento in monitoramentosDaCarga)
                {
                    historicosPorMonitoramento.TryGetValue(monitoramento.Codigo, out List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Historico> historicoDoMonitoramento);

                    listaMonitoramentos.Add(new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Monitoramento
                    {
                        Codigo = monitoramento.Codigo,
                        DataCriacao = monitoramento.DataCriacao,
                        DataInicio = monitoramento.DataInicio,
                        Critico = monitoramento.Critico,
                        Situacao = monitoramento.Carga.SituacaoCarga,
                        NomeMotorista = carga.NomePrimeiroMotorista,
                        PlacaTracao = carga.Veiculo?.Placa_Formatada,
                        PlacaReboque = carga.VeiculosVinculados?.FirstOrDefault()?.Placa_Formatada,
                        DataPosicaoAtual = monitoramento.UltimaPosicao?.Data,
                        LatitudePosicaoAtual = monitoramento.UltimaPosicao?.Latitude,
                        LongitudePosicaoAtual = monitoramento.UltimaPosicao?.Longitude,
                        PercentualViagem = monitoramento.PercentualViagem,
                        KmTotal = monitoramento.DistanciaPrevista,
                        TipoOperacao = monitoramento.Carga.TipoOperacao.Descricao,
                        Distancias = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Distancia
                        {
                            Prevista = monitoramento.DistanciaPrevista,
                            Realizada = monitoramento.DistanciaRealizada,
                            AteOrigem = monitoramento.DistanciaAteOrigem,
                            AteDestino = monitoramento.DistanciaAteDestino
                        },
                        Historicos = historicoDoMonitoramento
                    });
                }

                listaDadosMonitoramento.Add(new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento
                {
                    ProtocoloCarga = carga.Codigo,
                    NumeroCarga = carga.CodigoCargaEmbarcador,
                    Monitoramentos = listaMonitoramentos
                });
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento>>.CriarRetornoSucesso(listaDadosMonitoramento);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Ocorrencia>> BuscarDadosOcorrencias(DateTime dataInicioCriacao, DateTime dataFimCriacao, int numeroOcorrencia)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Ocorrencia> modeloDadosOcorrencias = repositorioCargaOcorrencia.ObterModeloDadosOcorrencias(dataInicioCriacao, dataFimCriacao, numeroOcorrencia);

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Ocorrencia>>.CriarRetornoSucesso(modeloDadosOcorrencias);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Pedido>> BuscarDadosPedidos(DateTime dataInicioCriacao, DateTime dataFimCriacao, string numeroPedido)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Pedido> modeloDadosPedidos = repositorioPedido.ObterModeloDadosPedidos(dataInicioCriacao, dataFimCriacao, numeroPedido);

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Pedido>>.CriarRetornoSucesso(modeloDadosPedidos);
        }

        #endregion Metodos Publicos
    }
}
