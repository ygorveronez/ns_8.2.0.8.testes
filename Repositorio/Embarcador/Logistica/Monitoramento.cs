using Dominio.Entidades.Embarcador.Cargas;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using NHibernate;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class Monitoramento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.Monitoramento>
    {

        #region Atributos Públicos
        public Monitoramento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Monitoramento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #endregion

        #region Atributos Privados
        private readonly DateTime _dataAtual = DateTime.Now;
        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss";
        #endregion

        #region Métodos Públicos


        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => codigos.Contains(ent.Codigo));
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarPorCodigosLista(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = new List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            int take = 600;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
                IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> filter = from obj in query
                                                                                          where tmp.Contains(obj.Codigo)
                                                                                          select obj;

                result.AddRange(filter.Fetch(x => x.Carga).ThenFetch(o => o.TipoOperacao).Fetch(x => x.Veiculo).ToList());

                start += take;
            }

            return result;
        }

        public Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);
            return result.Fetch(x => x.Carga).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarUltimoPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga);
            return result.OrderByDescending(ent => ent.DataCriacao).FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarUltimoPorCargaAsync(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga);
            return await result.OrderByDescending(ent => ent.DataCriacao).FirstOrDefaultAsync();
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosDetalhesCarga>> BuscarDadosMonitoramentoPorCodigosCargasAsync(List<int> codigosCargas)
        {
            List<string> codigosCargasString = new List<string>();

            foreach (int codigoCarga in codigosCargas)
                codigosCargasString.Add(codigoCarga.ToString());

            string sql = $@"
                            SELECT Carga.CAR_CODIGO as CodigoCarga,
                            Monitoramento.MON_CRITICO as MonitoramentoCritico,
                            Posicao.POS_DATA As DataPosicao,
                            Monitoramento.MON_STATUS as StatusMonitoramento
                            FROM T_MONITORAMENTO Monitoramento
                            LEFT JOIN T_CARGA Carga on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO
                            LEFT JOIN T_POSICAO Posicao on Posicao.POS_CODIGO = Monitoramento.POS_ULTIMA_POSICAO
							WHERE Carga.CAR_CODIGO IN ({string.Join(", ", codigosCargasString)})
                            order by Monitoramento.MON_DATA_CRIACAO desc";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosDetalhesCarga)));

            var result = await consulta.ListAsync<Dominio.ObjetosDeValor.Embarcador.Monitoramento.MonitoramentoDadosDetalhesCarga>();

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarPrimeiroPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga);
            return result.OrderBy(ent => ent.DataCriacao).FirstOrDefault();
        }

        public int BuscarCodigoUltimoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga);
            return query.OrderByDescending(o => o.DataCriacao).Select(o => o.Codigo).Skip(0).Take(1).FirstOrDefault();
        }

        public List<string> BuscarDadosPolilinhasPlanejadasPorMonitoramentos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> consultaMonitoramento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaMonitoramento
                .Select(o => o.PolilinhaPrevista)
                .ToList();
        }

        public List<string> BuscarDadosPolilinhasRealizadasPorMonitoramentos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> consultaMonitoramento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaMonitoramento
                .Select(o => o.PolilinhaRealizada)
                .ToList();
        }

        public string BuscarPolilinhaPlanejada(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(o => o.Codigo == codigo);
            return query.Select(o => o.PolilinhaPrevista).FirstOrDefault();
        }

        public string BuscarPolilinhaRealizada(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(o => o.Codigo == codigo);
            return query.Select(o => o.PolilinhaRealizada).FirstOrDefault();
        }

        public bool VerificarSeNaoValidaStatusViagemPorMonitoramento(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(o => o.Codigo == codigo && o.StatusViagem.NaoUtilizarStatusParaCalculoTemperaturaDentroFaixa);

            return query.Any();
        }
        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarUltimoPorVeiculoPeriodo(List<int> codigosVeiculo, DateTime dataInicial, DateTime dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent =>
                codigosVeiculo.Contains(ent.Veiculo.Codigo) &&
                (
                    (
                        ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && ent.DataInicio <= dataFinal
                    )
                    ||
                    (
                        ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado &&
                        (
                            (dataInicial >= ent.DataInicio && dataInicial <= ent.DataFim)
                            ||
                            (dataFinal >= ent.DataInicio && dataFinal <= ent.DataFim)
                            ||
                            (dataInicial <= ent.DataInicio && dataFinal >= ent.DataFim)
                            ||
                            (dataInicial >= ent.DataInicio && dataFinal <= ent.DataFim)
                        )
                    )
                )
            );
            return result.Fetch(x => x.Carga).ToList();
        }
        public Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarUltimoPorVeiculoPeriodo(List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal)
        {
            return monitoramentos.Where(ent =>
                 ent.Veiculo.Codigo == codigoVeiculo &&
                 (
                     (
                         ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && ent.DataInicio <= dataFinal
                     )
                     ||
                     (
                         ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado &&
                         (
                             (dataInicial >= ent.DataInicio && dataInicial <= ent.DataFim)
                             ||
                             (dataFinal >= ent.DataInicio && dataFinal <= ent.DataFim)
                             ||
                             (dataInicial <= ent.DataInicio && dataFinal >= ent.DataFim)
                             ||
                             (dataInicial >= ent.DataInicio && dataFinal <= ent.DataFim)
                         )
                     )
                 )
             ).OrderByDescending(ent => ent.DataCriacao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarTodosPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga);
            return result.OrderBy(ent => ent.DataCriacao).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarPorCargaAguardando(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga && ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando);
            return result.OrderByDescending(ent => ent.DataCriacao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarPorCargaNaoFinalizado(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga && ent.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado && ent.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado);
            return result.OrderByDescending(ent => ent.DataCriacao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarAtivoPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga && ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado);
            return result.OrderByDescending(ent => ent.DataCriacao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarPorCargas(List<int> codigosCarga)
        {
            return BuscarPorCargasAsync(codigosCarga, null).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>> BuscarPorCargasAsync(IList<int> codigosCarga, IList<string> configuracaoFetchs)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> consultaMonitoramento = ObterQueryableBuscarPorCargas(codigosCarga, configuracaoFetchs);

            return consultaMonitoramento.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoNaoFinalizadoEStatus()
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado && obj.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado &&
            (obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
            || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
            || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento
            ));

            return query.Fetch(obj => obj.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosRetornoVazioAguardandoInicioPorVeiculo(Dominio.Entidades.Veiculo veiculo, DateTime dataInicial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj =>
                obj.Veiculo.Codigo == veiculo.Codigo &&
                obj.DataCriacao >= dataInicial &&
                obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando &&
                obj.Carga.TipoOperacao.RetornoVazio == true
            ).OrderBy(obj => obj.DataCriacao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoNaoFinalizado()
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado && obj.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado);

            return query.Fetch(obj => obj.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoIniciado()
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.DataCriacao > DateTime.Now.AddDays(-4));

            return query.Fetch(obj => obj.Carga).ToList();

        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSemSinalAntesDoCarregamento> BuscarMonitoramentoIniciadoParaCargasComDataCarregamentoCargaDefinido(int maiorTempoEvento)
        {
            IQueryable<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSemSinalAntesDoCarregamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>()
                        .Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                                      obj.Carga.DataCarregamentoCarga.HasValue &&
                                      DateTime.Now >= obj.Carga.DataCarregamentoCarga.Value.AddMinutes(-maiorTempoEvento) &&
                                      DateTime.Now <= obj.Carga.DataCarregamentoCarga.Value)
                        .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSemSinalAntesDoCarregamento
                        {
                            CodigoMonitoramento = obj.Codigo,
                            CodigoCarga = obj.Carga.Codigo,
                            CodigoVeiculo = obj.Veiculo.Codigo,
                            DataCarregamentoCarga = obj.Carga.DataCarregamentoCarga,
                            DataInicio = obj.DataInicio
                        });
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoFinalizadoPorDataFinalMaior(DateTime Data)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado && obj.DataFim >= Data);

            return query.Fetch(obj => obj.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosFinalizadoPorVeiculoEDataFinalMaior(List<int> veiculos, DateTime Data)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => veiculos.Contains(obj.Veiculo.Codigo) &&
                                        obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado && obj.DataFim >= Data);

            return query.Fetch(obj => obj.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosAgendadosPorVeiculoDataCarregamentoCarga(int codigoVeiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(obj => obj.Veiculo.Codigo == codigoVeiculo && obj.Carga.DataCarregamentoCarga != null && obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando);
            return query.Fetch(obj => obj.Carga).OrderBy(obj => obj.Carga.DataCarregamentoCarga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosAgendadosPorPlacaVeiculoDataCarregamentoCarga(string placa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(obj => obj.Veiculo.Placa == placa && obj.Carga.DataCarregamentoCarga != null && obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando);
            return query.Fetch(obj => obj.Carga).OrderBy(obj => obj.Carga.DataCarregamentoCarga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosAgendadosPorVeiculo(int codigoVeiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(obj => obj.Veiculo.Codigo == codigoVeiculo && obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando);
            return query.Fetch(obj => obj.Carga).OrderBy(obj => obj.DataCriacao).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosAgendadosPorPlacaVeiculo(string placa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(obj => obj.Veiculo.Placa == placa && obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando);
            return query.Fetch(obj => obj.Carga).OrderBy(obj => obj.DataCriacao).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoEmAbertoPorVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.Veiculo.Codigo == veiculo.Codigo);

            return query.OrderBy(obj => obj.DataCriacao).Fetch(obj => obj.Carga).ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>> BuscarMonitoramentoEmAbertoPorVeiculoAsync(Dominio.Entidades.Veiculo veiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.Veiculo.Codigo == veiculo.Codigo);

            return await query.OrderBy(obj => obj.DataCriacao).Fetch(obj => obj.Carga).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoInciadoPorVeiculoEMotorista(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Usuario motorista)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.Veiculo.Codigo == veiculo.Codigo &&
                                obj.Carga.Motoristas.Contains(motorista));

            return query.OrderBy(obj => obj.DataCriacao).Fetch(obj => obj.Carga).ToList();
        }

        public bool ExisteMonitoramentoInciadoPorVeiculoEMotorista(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Usuario motorista)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.Veiculo.Codigo == veiculo.Codigo &&
                                obj.Carga.Motoristas.Contains(motorista));

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoEmAbertoPorVeiculoPlaca(string placa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.Veiculo.Placa == placa);
            return query.Fetch(obj => obj.Carga).ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>> BuscarMonitoramentoEmAbertoPorVeiculoPlacaAsync(string placa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.Veiculo.Placa == placa);
            return await query.Fetch(obj => obj.Carga).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoEmAbertoPorVeiculoPlacas(List<string> placas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && placas.Contains(obj.Veiculo.Placa));
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoEmAbertoPorVeiculo(Dominio.Entidades.Veiculo veiculo, DateTime data)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Veiculo.Codigo == veiculo.Codigo && (
                (obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.DataInicio <= data) ||
                (obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado && data >= obj.DataInicio && data <= obj.DataFim)
            ));

            return query.Fetch(obj => obj.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoEmAbertoNoPeriodo(List<int> codigosVeiculos, DateTime dataInicial, DateTime dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj =>
                obj.Veiculo.Codigo > 0 &&
                codigosVeiculos.Contains(obj.Veiculo.Codigo) &&
                (
                    (
                        obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && obj.DataInicio <= dataFinal
                    )
                    ||
                    (
                        obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado &&
                        (
                            (dataInicial >= obj.DataInicio && dataInicial <= obj.DataFim)
                            ||
                            (dataFinal >= obj.DataInicio && dataFinal <= obj.DataFim)
                            ||
                            (dataInicial <= obj.DataInicio && dataFinal >= obj.DataFim)
                        )
                    )
                )
            );

            return query.Fetch(obj => obj.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoEmAberto(Dominio.Entidades.Veiculo veiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado);

            return query.ToList();
        }

        public dynamic ConsultarOnTime(List<int> codigosCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            string sql = GetSQLSelect(configuracao) + GetSQLFromMonitoramento() + GetSQLJoins();
            sql += $" WHERE Carga.CAR_CODIGO IN ({string.Join(",", codigosCarga.ToArray())})";

            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento)));

            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> lista = consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();

            return lista;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> ConsultarMonitoramentoRefatorado(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            var sql = MontarSQLMonitoramentoRefatorado(filtrosPesquisa, parametrosConsulta, configuracao, configuracaoMonitoramento);

            ISQLQuery consulta = sql.CriarSQLQuery(this.SessionNHiBernate);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento)));
            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();
        }

        private SQLDinamico MontarSQLMonitoramentoRefatorado(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            var sqlBuilder = new StringBuilder();
            var parametros = new List<ParametroSQL>();

            sqlBuilder.AppendLine(MonitoramentoConstantesConsulta.ObterConsultaCompletaMonitoramento(filtrosPesquisa.SomenteUltimoPorCarga, false));

            string filtro = GetFiltroMonitoramento(filtrosPesquisa, configuracao, configuracaoMonitoramento, parametros);

            sqlBuilder.AppendLine(" WHERE 1=1 ");
            sqlBuilder.AppendLine(filtro);

            if (parametrosConsulta != null)
            {
                sqlBuilder.AppendLine();
                sqlBuilder.Append("ORDER BY ");
                switch (parametrosConsulta.PropriedadeOrdenar)
                {
                    case "Rastreador":
                    case "Status":
                    case "Coletas":
                    case "Entregas":
                        sqlBuilder.Append("1");
                        break;
                    case "DataCarregamento":
                        switch (configuracao.DataBaseCalculoPrevisaoControleEntrega)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                                sqlBuilder.Append(" DataPrevisaoTerminoCarga");
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                                sqlBuilder.Append(" DataInicioViagemPrevista");
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                                sqlBuilder.Append(" DataCarregamentoCarga");
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                                sqlBuilder.Append(" DataInicioCarregamentoJanela");
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                            default:
                                sqlBuilder.Append(" DataCriacaoCarga");
                                break;
                        }
                        break;
                    default:
                        sqlBuilder.Append(" " + parametrosConsulta.PropriedadeOrdenar);
                        break;
                }
                sqlBuilder.Append(" " + parametrosConsulta.DirecaoOrdenar);

                if ((parametrosConsulta.InicioRegistros > 0 || parametrosConsulta.LimiteRegistros > 0) && !filtrosPesquisa.VeiculosEmLocaisTracking)
                {
                    sqlBuilder.AppendLine();
                    sqlBuilder.AppendFormat("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
                }
            }

            return new SQLDinamico(sqlBuilder.ToString(), parametros);
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            var parametros = new List<ParametroSQL>();

            string filtro = GetFiltroMonitoramento(filtrosPesquisa, configuracao, configuracaoMonitoramento, parametros);

            string sql = GetSQLSelect(configuracao, filtrosPesquisa) + GetSQLFromMonitoramento() + GetSQLJoins();
            sql += !string.IsNullOrWhiteSpace(filtro) ? " WHERE 1 = 1 " + filtro : "";

            if (filtrosPesquisa.SomenteUltimoPorCarga)
                sql += ") AS Result WHERE RowNum = 1 ";

            if (parametrosConsulta != null)
            {
                sql = sql + $" order by";
                switch (parametrosConsulta.PropriedadeOrdenar)
                {
                    case "Rastreador":
                    case "Status":
                    case "Coletas":
                    case "Entregas":
                        sql = sql + " 1";
                        break;
                    case "DataCarregamento":
                        switch (configuracao.DataBaseCalculoPrevisaoControleEntrega)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                                sql = sql + $" DataPrevisaoTerminoCarga";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                                sql = sql + $" DataInicioViagemPrevista";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                                sql = sql + $" DataCarregamentoCarga";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                                sql = sql + $" DataInicioCarregamentoJanela";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                            default:
                                sql = sql + $" DataCriacaoCarga";
                                break;
                        }
                        break;
                    default:
                        sql = sql + $" {parametrosConsulta.PropriedadeOrdenar}";
                        break;
                }

                sql += $" {parametrosConsulta.DirecaoOrdenar}";
                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0) && !filtrosPesquisa.VeiculosEmLocaisTracking) //se esta filtrando veiculos em locais de tracking nao vamos paginar.
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";

            }

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            var parametros = new List<ParametroSQL>();

            string filtro = GetFiltroMonitoramento(filtrosPesquisa, configuracao, configuracaoMonitoramento, parametros);

            string sqlContar = filtrosPesquisa.SomenteUltimoPorCarga ? "SELECT count(1) FROM ( SELECT  ROW_NUMBER() OVER (PARTITION BY Monitoramento.CAR_CODIGO ORDER BY Carga.CAR_DATA_CRIACAO DESC) as RowNum " + GetSQLFromMonitoramento() + GetSQLJoins() : $"SELECT COUNT(*) CONTADOR " + GetSQLFromMonitoramento() + GetSQLJoins(); // SQL-INJECTION-SAFE
            sqlContar += !string.IsNullOrWhiteSpace(filtro) ? " WHERE 1 = 1 " + filtro : "";

            sqlContar += filtrosPesquisa.SomenteUltimoPorCarga ? ") AS Result WHERE  RowNum = 1; " : "";

            var sqlDinamico = new SQLDinamico(sqlContar, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaRefatorada(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            var parametros = new List<ParametroSQL>();

            string filtro = GetFiltroMonitoramento(filtrosPesquisa, configuracao, configuracaoMonitoramento, parametros);

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(MonitoramentoConstantesConsulta.ObterConsultaCompletaMonitoramento(filtrosPesquisa.SomenteUltimoPorCarga, isCount: true));

            sqlBuilder.AppendLine(" WHERE 1=1 ");

            sqlBuilder.AppendLine(filtro);

            var sqlContar = sqlBuilder.ToString();

            var sqlDinamico = new SQLDinamico(sqlContar, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento> ConsultarTorre(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            var parametros = new List<ParametroSQL>();

            string filtro = GetFiltroMonitoramentoTorre(filtrosPesquisa, configuracao, parametros);

            string sql = GetSQLSelectTorreMonitoramento(configuracao) + GetSQLFromMonitoramento() + GetSQLJoinsTorre();
            sql += !string.IsNullOrWhiteSpace(filtro) ? " WHERE 1 = 1 " + filtro : "";

            if (parametrosConsulta != null)
            {
                sql = sql + $" order by";
                switch (parametrosConsulta.PropriedadeOrdenar)
                {
                    case "Rastreador":
                    case "Status":
                    case "Coletas":
                    case "Entregas":
                        sql = sql + " 1";
                        break;
                    case "DataCarregamento":
                        switch (configuracao.DataBaseCalculoPrevisaoControleEntrega)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                                sql = sql + $" DataPrevisaoTerminoCarga";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                                sql = sql + $" DataInicioViagemPrevista";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                                sql = sql + $" DataCarregamentoCarga";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                                sql = sql + $" DataInicioCarregamentoJanela";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                            default:
                                sql = sql + $" DataCriacaoCarga";
                                break;
                        }
                        break;
                    default:
                        sql = sql + $" {parametrosConsulta.PropriedadeOrdenar}";
                        break;
                }

                sql = sql + $" {parametrosConsulta.DirecaoOrdenar}";
                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql = sql + $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento>();
        }

        public int ContarConsultaTorre(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            var parametros = new List<ParametroSQL>();

            string filtro = GetFiltroMonitoramentoTorre(filtrosPesquisa, configuracao, parametros);

            string sqlContar = $"SELECT COUNT(*) CONTADOR " + GetSQLFromMonitoramento() + GetSQLJoinsTorre(); // SQL-INJECTION-SAFE
            sqlContar += !string.IsNullOrWhiteSpace(filtro) ? " WHERE 1 = 1 " + filtro : "";

            var sqlDinamico = new SQLDinamico(sqlContar, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCargaResumo> ConsultarTotaisAlertasTorre(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            var parametros = new List<ParametroSQL>();

            string select = $@"
                SELECT 
                    CargaEvento.ALC_TIPO,
                    CargaEvento.ALC_STATUS,
                    CargaEvento.CAR_CODIGO";

            string from = $@"
                    from T_CARGA_EVENTO as CargaEvento
                    left join T_MONITORAMENTO Monitoramento on Monitoramento.CAR_CODIGO = CargaEvento.CAR_CODIGO" + GetSQLJoins();

            string where = " WHERE 1 = 1 ";
            string filtro = GetFiltroMonitoramentoTorre(filtrosPesquisa, configuracao, parametros);
            string group = "GROUP BY CargaEvento.ALC_TIPO, CargaEvento.ALC_STATUS, CargaEvento.CAR_CODIGO";

            string sqlBase = select + from + where + filtro + group;

            string sql = $@"
                SELECT 
                    ALC_TIPO as TipoAlerta,
                    SUM(CASE WHEN ALC_STATUS = 0 THEN 1 ELSE 0 END) Pendentes,
                    COUNT(CAR_CODIGO) Total,
                    COUNT(DISTINCT CAR_CODIGO) Cargas
                FROM (
                    {sqlBase}
                ) AS Alertas
                GROUP BY 
                    ALC_TIPO";

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCargaResumo)));
            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCargaResumo>();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertasResumo> ConsultarTotaisAlertas(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            var parametros = new List<ParametroSQL>();

            string select = GetSQLSelectAlertaMonitor();
            string from = GetSQLFromAlertaMonitor() + GetSQLJoins();
            string where = " WHERE AlertaMonitor.ALE_STATUS = 0 ";
            string filtro = GetFiltroMonitoramento(filtrosPesquisa, configuracao, configuracaoMonitoramento, parametros);
            string group = "GROUP BY AlertaMonitor.ALE_TIPO, AlertaMonitor.MEV_CODIGO, AlertaMonitor.CAR_CODIGO";

            string sqlBase = select + from + where + filtro + group;

            string sql = $@"
                SELECT 
                    ALE_TIPO as TipoAlerta,
                    MEV_CODIGO as MonitoramentoEvento,
                    COUNT(CAR_CODIGO) Total
                FROM (
                    {sqlBase}
                ) AS Alertas
                GROUP BY 
                    ALE_TIPO, MEV_CODIGO";

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertasResumo)));
            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertasResumo>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.ResumoCargaStatus> ObterResumoCargaStatus(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            var parametros = new List<ParametroSQL>();

            string filtro = GetFiltroMonitoramento(filtrosPesquisa, configuracao, configuracaoMonitoramento, parametros);

            string sql = GetSQLSelectResumoCargasSituacao(configuracao) + GetSQLFromMonitoramento() + GetSQLJoins();
            sql += !string.IsNullOrWhiteSpace(filtro) ? " WHERE 1 = 1 " + filtro : "";
            sql += " order by MonitoramentoStatus.MSV_ORDEM";

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.ResumoCargaStatus)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.ResumoCargaStatus>();
        }



        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarNivelServico(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return ObterLista(ConsultaNivelServico(), parametrosConsulta);
        }

        public int ContarNivelServico()
        {

            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = ConsultaNivelServico();

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaVeiculo> BuscarUltimaCargaPorVeiculos(List<int> codigosVeiculos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> listaSituacoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
            };

            IQueryable<Dominio.ObjetosDeValor.Embarcador.Carga.CargaVeiculo> queryGrupo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(o => codigosVeiculos.Contains(o.Veiculo.Codigo) && listaSituacoes.Contains(o.SituacaoCarga))
                .GroupBy(o => o.Veiculo.Codigo)
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaVeiculo { CodigoCarga = o.Count(), CodigoVeiculo = o.Key });

            return queryGrupo.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.TotalMonitoramentosVeiculo> BuscarMonitoramentosFinalizadosPorVeiculos(List<int> veiculos)
        {

            IQueryable<Dominio.ObjetosDeValor.Embarcador.Logistica.TotalMonitoramentosVeiculo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>()
                .Where(obj => veiculos.Contains(obj.Veiculo.Codigo) && (obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado || obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado))
                .GroupBy(obj => obj.Veiculo.Codigo)
                .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Logistica.TotalMonitoramentosVeiculo { TotalMonitoramentos = obj.Count(), CodigoVeiculo = obj.Key });
            return query.ToList();

        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> BuscarProcessarComLimite(int quantidade = 100)
        {
            string sql = $@"
               select 
	            monitorame0_.MON_CODIGO CodigoMonitoramento,
	            monitorame0_.MON_DATA_CRIACAO DataCriacaoMonitoramento,
	            monitorame0_.VEI_CODIGO CodigoVeiculo,
	            monitorame0_.MON_DATA_INICIO DataInicioMonitoramento,
	            monitorame0_.MON_DATA_FIM DataFimMonitoramento,
	            monitorame0_.CAR_CODIGO CodigoCarga,
	            carga1_.CAR_DATA_INICIO_VIAGEM DataInicioViagem,
	            carga1_.CAR_DATA_CARREGAMENTO DataCarregamentoCarga
            from
                T_MONITORAMENTO monitorame0_ 
            left outer join
                T_CARGA carga1_ 
                    on monitorame0_.CAR_CODIGO=carga1_.CAR_CODIGO 
            where
                monitorame0_.MON_PROCESSAR in (
                    0 , 1
                )  
                and mon_status in (0, 1)
                and MON_DATA_INICIO >= '{DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")}'
            order by
                monitorame0_.POS_ULTIMA_POSICAO asc OFFSET 0 ROWS FETCH FIRST :limite ROWS ONLY";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("limite", quantidade);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> BuscarProcessarStatusViagem()
        {
            string sql = $@"
               select 
	            monitorame0_.MON_CODIGO CodigoMonitoramento,
	            monitorame0_.MON_DATA_CRIACAO DataCriacaoMonitoramento,
	            monitorame0_.VEI_CODIGO CodigoVeiculo,
	            monitorame0_.MON_DATA_INICIO DataInicioMonitoramento,
	            monitorame0_.MON_DATA_FIM DataFimMonitoramento,
	            monitorame0_.CAR_CODIGO CodigoCarga,
	            carga1_.CAR_DATA_INICIO_VIAGEM DataInicioViagem,
	            carga1_.CAR_DATA_CARREGAMENTO DataCarregamentoCarga
            from
                T_MONITORAMENTO monitorame0_ 
            left outer join
                T_CARGA carga1_ 
                    on monitorame0_.CAR_CODIGO=carga1_.CAR_CODIGO 
            where
                monitorame0_.MON_PROCESSAR in (
                    0 , 1
                )  
                and mon_status in (0, 1)
                and MON_DATA_INICIO >= '{DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")}'
            order by
                monitorame0_.POS_ULTIMA_POSICAO asc";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar>();
        }


        public List<int> BuscarCodigosMonitoramentosPendentesProcessamento()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao> listaSituacoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao>
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Pendente,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processando
            };
            IQueryable<int> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>()
                .Where(o => listaSituacoes.Contains(o.Processar) && o.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado)
                .Select(o => o.Codigo);
            return query.Distinct().ToList();
        }

        public void ExcluirTodosPorCargas(List<int> codigosCarga)
        {
            UnitOfWork.Sessao.CreateQuery("delete from MonitoramentoVeiculoPosicao where MonitoramentoVeiculo.Codigo in (select Codigo from MonitoramentoVeiculo where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo in (:codigosCarga)))")
                    .SetParameterList("codigosCarga", codigosCarga)
                    .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("DELETE FROM MonitoramentoHistoricoStatusViagemPermanenciaCliente WHERE Codigo IN (SELECT c.Codigo FROM MonitoramentoHistoricoStatusViagemPermanenciaCliente c WHERE c.PermanenciaCliente.CargaEntrega.Carga.Codigo in (:codigosCarga))")
                .SetParameterList("codigosCarga", codigosCarga)
                .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("DELETE FROM MonitoramentoHistoricoStatusViagemPermanenciaSubArea WHERE Codigo IN (SELECT c.Codigo FROM MonitoramentoHistoricoStatusViagemPermanenciaSubArea c WHERE c.PermanenciaSubarea.CargaEntrega.Carga.Codigo in (:codigosCarga))")
                .SetParameterList("codigosCarga", codigosCarga)
                .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("delete from MonitoramentoHistoricoStatusViagem where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo in (:codigosCarga))")
                .SetParameterList("codigosCarga", codigosCarga)
                .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("delete from PerdaSinalMonitoramento where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo in (:codigosCarga))")
                  .SetParameterList("codigosCarga", codigosCarga)
                  .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("delete from MonitoramentoVeiculo where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo in (:codigosCarga))")
                 .SetParameterList("codigosCarga", codigosCarga)
                 .ExecuteUpdate();

            UnitOfWork.Sessao.CreateSQLQuery(@"DELETE FROM T_MONITORAMENTO_DADOS_SUMARIZADOS_POSICAO
                                    WHERE MDS_CODIGO IN (
                                        SELECT MDS_CODIGO 
                                        FROM T_MONITORAMENTO_DADOS_SUMARIZADOS
                                        WHERE MON_CODIGO IN (
                                            SELECT MON_CODIGO 
                                            FROM T_MONITORAMENTO
                                            WHERE CAR_CODIGO in (:codigosCarga)))")
                  .SetParameterList("codigosCarga", codigosCarga)
                  .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("delete from MonitoramentoDadosSumarizados where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo in (:codigosCarga))")
                .SetParameterList("codigosCarga", codigosCarga)
                .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("delete from Monitoramento where Carga.Codigo in (:codigosCarga)")
                .SetParameterList("codigosCarga", codigosCarga)
                .ExecuteUpdate();
        }

        public void ExcluirTodosPorCarga(int codigoCarga)
        {
            UnitOfWork.Sessao.CreateQuery("delete from MonitoramentoVeiculoPosicao where MonitoramentoVeiculo.Codigo in (select Codigo from MonitoramentoVeiculo where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo = :codigoCarga))")
                             .SetInt32("codigoCarga", codigoCarga)
                             .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("DELETE FROM MonitoramentoHistoricoStatusViagemPermanenciaCliente WHERE Codigo IN (SELECT c.Codigo FROM MonitoramentoHistoricoStatusViagemPermanenciaCliente c WHERE c.PermanenciaCliente.CargaEntrega.Carga.Codigo = :codigoCarga)")
                            .SetInt32("codigoCarga", codigoCarga)
                            .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("DELETE FROM MonitoramentoHistoricoStatusViagemPermanenciaSubArea WHERE Codigo IN (SELECT c.Codigo FROM MonitoramentoHistoricoStatusViagemPermanenciaSubArea c WHERE c.PermanenciaSubarea.CargaEntrega.Carga.Codigo = :codigoCarga)")
                            .SetInt32("codigoCarga", codigoCarga)
                            .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("delete from MonitoramentoHistoricoStatusViagem where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo = :codigoCarga)")
                             .SetInt32("codigoCarga", codigoCarga)
                             .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("delete from PerdaSinalMonitoramento where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo = :codigoCarga)")
                        .SetInt32("codigoCarga", codigoCarga)
                        .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery("delete from MonitoramentoVeiculo where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo = :codigoCarga)")
                             .SetInt32("codigoCarga", codigoCarga)
                             .ExecuteUpdate();

            UnitOfWork.Sessao.CreateSQLQuery(@"DELETE FROM T_MONITORAMENTO_DADOS_SUMARIZADOS_POSICAO
                                    WHERE MDS_CODIGO IN (
                                        SELECT MDS_CODIGO 
                                        FROM T_MONITORAMENTO_DADOS_SUMARIZADOS
                                        WHERE MON_CODIGO IN (
                                            SELECT MON_CODIGO 
                                            FROM T_MONITORAMENTO
                                            WHERE CAR_CODIGO = :codigoCarga))")
                  .SetInt32("codigoCarga", codigoCarga)
                  .ExecuteUpdate();

            UnitOfWork.Sessao.CreateQuery(@"delete from MonitoramentoDadosSumarizados where Monitoramento.Codigo in (select Codigo from Monitoramento where Carga.Codigo = :codigoCarga)")
                            .SetInt32("codigoCarga", codigoCarga)
                            .ExecuteUpdate();


            UnitOfWork.Sessao.CreateQuery("delete from Monitoramento where Carga.Codigo = :codigoCarga")
                             .SetInt32("codigoCarga", codigoCarga)
                             .ExecuteUpdate();
        }

        public void AtualizarRotaRealizada(int codigo, string polilinhaRealizada, decimal distanciaRealizada, string polilinhaAteOrigem, decimal distanciaAteOrigem, string polilinhaAteDestino, decimal distanciaAteDestino, int codigoStatusViagem, decimal percentual, bool processado, DbConnection connection, DbTransaction transaction)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = 300;
            command.Transaction = transaction;

            command.CommandText = "UPDATE t_monitoramento SET mon_polilinha_ate_origem = @mon_polilinha_ate_origem ";

            DbParameter parPolilinhaAteorigem = command.CreateParameter();
            parPolilinhaAteorigem.ParameterName = "@mon_polilinha_ate_origem";
            parPolilinhaAteorigem.Value = (!string.IsNullOrWhiteSpace(polilinhaAteOrigem)) ? polilinhaAteOrigem : SqlString.Null;
            command.Parameters.Add(parPolilinhaAteorigem);

            if (distanciaRealizada > 0)
            {
                command.CommandText += " , mon_distancia_realizada = @mon_distancia_realizada";
                DbParameter parDistancia = command.CreateParameter();
                parDistancia.ParameterName = "@mon_distancia_realizada";
                parDistancia.Value = distanciaRealizada;
                command.Parameters.Add(parDistancia);
            }


            if (!string.IsNullOrWhiteSpace(polilinhaRealizada))
            {
                command.CommandText += ", mon_polinha_realizada = @mon_polilinha_realizada";
                DbParameter parPolilinhaRealizada = command.CreateParameter();
                parPolilinhaRealizada.ParameterName = "@mon_polilinha_realizada";
                parPolilinhaRealizada.Value = polilinhaRealizada;
                command.Parameters.Add(parPolilinhaRealizada);
            }


            if (distanciaAteOrigem > 0)
            {
                command.CommandText += ", mon_distancia_ate_origem = @mon_distancia_ate_origem";
                DbParameter parDistanciaAteOrigem = command.CreateParameter();
                parDistanciaAteOrigem.ParameterName = "@mon_distancia_ate_origem";
                parDistanciaAteOrigem.Value = (distanciaAteOrigem > 0) ? distanciaAteOrigem : SqlDecimal.Null;
                command.Parameters.Add(parDistanciaAteOrigem);
            }

            if (!string.IsNullOrWhiteSpace(polilinhaAteDestino))
            {
                command.CommandText += ", mon_polilinha_ate_destino = @mon_polilinha_ate_destino";
                DbParameter parPolilinhaAteDestino = command.CreateParameter();
                parPolilinhaAteDestino.ParameterName = "@mon_polilinha_ate_destino";
                parPolilinhaAteDestino.Value = (!string.IsNullOrWhiteSpace(polilinhaAteDestino)) ? polilinhaAteDestino : SqlString.Null;
                command.Parameters.Add(parPolilinhaAteDestino);
            }

            if (distanciaAteDestino > 0)
            {
                command.CommandText += ", mon_distancia_ate_destino = @mon_distancia_ate_destino";
                DbParameter parDistanciaAteDestino = command.CreateParameter();
                parDistanciaAteDestino.ParameterName = "@mon_distancia_ate_destino";
                parDistanciaAteDestino.Value = (distanciaAteDestino > 0) ? distanciaAteDestino : SqlDecimal.Null;
                command.Parameters.Add(parDistanciaAteDestino);
            }

            if (codigoStatusViagem > 0)
            {
                command.CommandText += ", msv_codigo = @msv_codigo";
                DbParameter parStatusViagem = command.CreateParameter();
                parStatusViagem.ParameterName = "@msv_codigo";
                parStatusViagem.Value = (codigoStatusViagem > 0) ? codigoStatusViagem : SqlInt32.Null;
                command.Parameters.Add(parStatusViagem);
            }

            if (percentual > 0)
            {
                command.CommandText += ", mon_percentual_viagem = @mon_percentual_viagem";
                DbParameter parPercentual = command.CreateParameter();
                parPercentual.ParameterName = "@mon_percentual_viagem";
                parPercentual.Value = percentual;
                command.Parameters.Add(parPercentual);
            }

            if (processado)
            {
                command.CommandText += ", mon_processar = @mon_processar";
                DbParameter parProcessar = command.CreateParameter();
                parProcessar.ParameterName = "@mon_processar";
                parProcessar.Value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado;
                command.Parameters.Add(parProcessar);
            }

            DbParameter parCodigo = command.CreateParameter();
            parCodigo.ParameterName = "@codigo";
            parCodigo.Value = codigo;
            command.Parameters.Add(parCodigo);
            command.CommandText += " WHERE mon_codigo = @codigo";

            command.ExecuteNonQuery();

        }

        public void AtualizarProcessarRota(List<int> codigosMonitoramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao processar, DbConnection connection, DbTransaction transaction)
        {
            AtualizarProcessarRota(codigosMonitoramento, 0, null, false, processar, connection, transaction, false);
        }

        public void AtualizarProcessarRota(List<int> codigosMonitoramento, long codigoUltimaPosicao, decimal? UltimaTemperatura, bool naFaixa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao processar, DbConnection connection, DbTransaction transaction, bool contabilizarTemperaturaFaixa)
        {
            if (codigosMonitoramento.Count > 0)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandTimeout = 300;
                command.Transaction = transaction;

                command.CommandText = "UPDATE t_monitoramento SET ";
                if (codigoUltimaPosicao > 0)
                {
                    command.CommandText += "pos_ultima_posicao = @pos_ultima_posicao, ";

                    DbParameter posUltimaPosicao = command.CreateParameter();
                    posUltimaPosicao.ParameterName = "@pos_ultima_posicao";
                    posUltimaPosicao.Value = codigoUltimaPosicao;
                    command.Parameters.Add(posUltimaPosicao);

                }

                if (UltimaTemperatura != null)
                {
                    command.CommandText += "mon_ultima_temperatura = @mon_ultima_temperatura, ";

                    DbParameter posUltimaTemperatura = command.CreateParameter();
                    posUltimaTemperatura.ParameterName = "@mon_ultima_temperatura";
                    posUltimaTemperatura.Value = UltimaTemperatura;
                    command.Parameters.Add(posUltimaTemperatura);

                    if (contabilizarTemperaturaFaixa)
                    {
                        if (naFaixa)
                            command.CommandText += "MON_TEMPERATURA_NA_FAIXA += 1, ";


                        command.CommandText += "MON_NUMERO_TEMPERATURA_RECEBIDA += 1, ";
                    }
                }

                command.CommandText += "mon_processar = @mon_processar ";
                command.CommandText += "WHERE mon_codigo in (" + String.Join(",", codigosMonitoramento) + ")";

                DbParameter monProcessar = command.CreateParameter();
                monProcessar.ParameterName = "@mon_processar";
                monProcessar.Value = (int)processar;
                command.Parameters.Add(monProcessar);

                command.ExecuteNonQuery();
            }
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosEmAbertoNoPeriodo(List<int> codigosVeiculos, DateTime dataInicio, DateTime dataFim)
        {
            if (codigosVeiculos?.Count > 0)
            {
                string sql = $@"
                    select 
	                    mon_codigo Codigo,
	                    mon_status Status,
	                    mon_data_inicio DataInicioMonitoramento,
	                    mon_data_fim DataFimMonitoramento,
	                    vei_codigo Veiculo,
	                    car_codigo Carga
                    from
	                    t_monitoramento
                    where
	                    vei_codigo in ({string.Join(",", codigosVeiculos)})
	                    and (
			                    (mon_status = :status_iniciado and mon_data_inicio <= :data_fim)
		                    or
		                    (
			                    mon_status = :status_finalizado and
                                (
                                    (:data_inicio >= mon_data_inicio and :data_inicio <= mon_data_fim)
                                    or
                                    (:data_fim >= mon_data_inicio and :data_fim <= mon_data_fim)
                                    or
                                    (:data_inicio <= mon_data_inicio and :data_fim >= mon_data_fim)
                                )
		                    )
	                    )";

                ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
                query.SetParameter("status_iniciado", Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado);
                query.SetParameter("status_finalizado", Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado);
                query.SetParameter("data_inicio", dataInicio);
                query.SetParameter("data_fim", dataFim);

                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento)));
                return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();
            }
            return new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();
        }

        public IList<int> BuscarVeiculosComMonitoramento(DateTime dataInicio, DateTime dataFim)
        {
            string sql = $@"
                select 
                    VEI_CODIGO
                from 
                    T_MONITORAMENTO
                where
                    VEI_CODIGO is not null and 
                    (
	                    (MON_STATUS = :mon_status_iniciado and MON_DATA_INICIO <= :data_inicio) or
	                    (
		                    MON_STATUS = :mon_status_finalizado and 
		                    (
			                    (:data_inicio >= MON_DATA_INICIO and :data_inicio <= MON_DATA_FIM) or
			                    (:data_fim >= MON_DATA_INICIO and :data_fim <= MON_DATA_FIM)
		                    )
	                    )
                    )";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("mon_status_iniciado", Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado);
            query.SetParameter("mon_status_finalizado", Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado);
            query.SetParameter("data_inicio", dataInicio);
            query.SetParameter("data_fim", dataFim);
            IList<int> codigosVeiculos = query.List<int>();
            return codigosVeiculos;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PedidosCargaMonitoramento> ConsultarPedidos(int codigoCarga, double? codigoCliente = 0)
        {
            string sql = $@"
                SELECT 
                    CargaPedido.CAR_CODIGO Carga,
                    Pedido.PED_NUMERO_PEDIDO_EMBARCADOR Pedido, 
                    NotaFiscal.NF_NUMERO NotaFiscal, 
                    NotaFiscal.NF_SERIE Serie, 
                    NotaFiscal.NF_CHAVE Chave, 
                    NotaFiscal.NF_VALOR Valor,
                    Cliente.CLI_NOME NomeCliente,
					Cliente.CLI_CGCCPF CodigoCliente
                FROM 
                    t_carga_pedido CargaPedido
                JOIN 
                    t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                JOIN
                    t_cliente Cliente on Cliente.CLI_CGCCPF = Pedido.CLI_CODIGO
                LEFT JOIN 
                    t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                LEFT JOIN 
                    t_xml_nota_fiscal NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO AND NotaFiscal.NF_ATIVA = 1
                WHERE 
                    CargaPedido.CAR_CODIGO = {codigoCarga}";

            if (codigoCliente > 0)
            {
                sql += $@"
                    and Cliente.CLI_CGCCPF = {codigoCliente}";
            }

            sql += $@"
                ORDER BY 
                    Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, Pedido.CLI_CODIGO";

            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PedidosCargaMonitoramento)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Logistica.PedidosCargaMonitoramento>();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoStatusStatusViagem BuscarStatusMonitoramento(DbConnection connection, DbTransaction transaction, int codigo)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoStatusStatusViagem monitoramentoStatusStatusViagem = null;
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = 300;
            command.Transaction = transaction;
            command.CommandType = CommandType.Text;
            command.Connection = connection;

            command.CommandText = $@"
                select 
                    MON_STATUS Status,
                    MSV_CODIGO CodigoStatusViagem
                from 
                    T_MONITORAMENTO
                where
                    MON_CODIGO = @mon_codigo";

            DbParameter parCodigo = command.CreateParameter();
            parCodigo.ParameterName = "@mon_codigo";
            parCodigo.Value = codigo;
            command.Parameters.Add(parCodigo);

            DbDataReader reader = command.ExecuteReader();
            if (reader.HasRows && reader.Read())
            {
                monitoramentoStatusStatusViagem = new Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoStatusStatusViagem
                {
                    Codigo = codigo,
                    Status = (reader["Status"] != DBNull.Value) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus)Enum.Parse(typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus), reader["Status"].ToString()) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Todos,
                    CodigoStatusViagem = (reader["CodigoStatusViagem"] != DBNull.Value) ? int.Parse(reader["CodigoStatusViagem"].ToString()) : 0
                };
            }
            if (!reader.IsClosed) reader.Close();

            return monitoramentoStatusStatusViagem;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentoEmAbertoPorMotorista(Dominio.Entidades.Usuario motorista)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                                obj.Carga.Motoristas.Contains(motorista));

            return query.ToList();
        }

        public bool validarExisteMonitoramentoFinalizadoPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga && ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado);
            return result.Any();
        }
        public async Task<bool> validarExisteMonitoramentoFinalizadoPorCargaAsync(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga && ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado);
            return await result.AnyAsync();
        }

        public bool CargaEstaEmMonitoramento(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query select obj;
            result = result.Where(ent => ent.Carga.Codigo == carga && ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado);
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Logistica.Monitoramento BuscarPorCodigoCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }


        public int ContarMonitoramentosPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado);

            return query.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> BuscarVeiculosMonitorados(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            string sql = $@"
                SELECT 
                    max(monitorame0_.VEI_CODIGO) as Codigo,
                    veiculo1_.VEI_PLACA as Placa,
                    veiculo1_.VEI_NUMERO_EQUIPAMENTO_RASTREADOR as NumeroEquipamentoRastreador 
                    from
                        T_MONITORAMENTO monitorame0_ 
                    left outer join
                        T_VEICULO veiculo1_ 
                            on monitorame0_.VEI_CODIGO=veiculo1_.VEI_CODIGO";

            if (tipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Rastrear || tipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskPosicoesPlaca)
                sql += $" Where monitorame0_.MON_STATUS = 1 and monitorame0_.MON_DATA_INICIO >= '{DateTime.Now.AddDays(-25).ToString("yyyy-MM-dd")}' group by veiculo1_.VEI_PLACA, veiculo1_.VEI_NUMERO_EQUIPAMENTO_RASTREADOR";
            else
                sql += $" Where monitorame0_.MON_STATUS in (0,1) and (monitorame0_.MON_DATA_INICIO >= '{DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")}' or monitorame0_.MON_DATA_INICIO is null) group by veiculo1_.VEI_PLACA, veiculo1_.VEI_NUMERO_EQUIPAMENTO_RASTREADOR";

            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> BuscarFaixasTemperatura(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> query;

            if (!String.IsNullOrWhiteSpace(descricao))
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>().Where(t => t.Descricao.Contains(descricao));
            else
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> result = from obj in query select obj;

            return result.ToList();
        }

        public int ContarConsulta(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> query;

            if (!String.IsNullOrWhiteSpace(descricao))
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>().Where(t => t.Descricao.Contains(descricao));
            else
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> result = from obj in query select obj;

            return result.Count();
        }

        public FaixaTemperatura BuscarFaixaTemperaturaPorCodigo(int codigoFaixaTemperatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>()
                .Where(o => o.Codigo == codigoFaixaTemperatura);
            IQueryable<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> result = from obj in query select obj;
            return result.FirstOrDefault();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoVeiculoAlvo> ConsultarRelatorioVeiculoAlvo(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoAlvo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaMonitoramentoVeiculoAlvo = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoVeiculoAlvo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMonitoramentoVeiculoAlvo.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoVeiculoAlvo)));

            return consultaMonitoramentoVeiculoAlvo.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoVeiculoAlvo>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoVeiculoPosicao> ConsultarRelatorioVeiculoPosicao(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaMonitoramentoVeiculoAlvo = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoVeiculoPosicao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMonitoramentoVeiculoAlvo.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoVeiculoPosicao)));

            return consultaMonitoramentoVeiculoAlvo.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoVeiculoPosicao>();
        }
        public int ContarConsultaRelatorioVeiculoAlvo(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoAlvo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            ISQLQuery consultaMonitoramentoVeiculoAlvo = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoVeiculoAlvo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMonitoramentoVeiculoAlvo.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaRelatorioVeiculoPosicao(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            ISQLQuery consultaMonitoramentoVeiculoPosicao = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoVeiculoPosicao().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMonitoramentoVeiculoPosicao.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoDaFrota> ConsultarRelatorioPosicaoDaFrota(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaMonitoramentoPosicaoDaFrota = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoPosicaoDaFrota().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMonitoramentoPosicaoDaFrota.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoDaFrota)));

            return consultaMonitoramentoPosicaoDaFrota.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoDaFrota>();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> ConsultarRelatorioPosicaoFrotaRastreamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoPosicaoFrotaRastreamento repConsulta = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoPosicaoFrotaRastreamento();
            return repConsulta.ExtrairMonitoramentoPosicaoFrotaRastreamento(base.UnitOfWork, filtrosPesquisa, propriedades, parametrosConsulta, configuracao);
        }

        public int ContarRelatorioPosicaoFrotaRastreamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoPosicaoFrotaRastreamento repConsulta = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoPosicaoFrotaRastreamento();
            return repConsulta.ContarMonitoramentoPosicaoFrotaRastreamento(base.UnitOfWork, filtrosPesquisa, propriedades);
        }

        public int ContarConsultaRelatorioPosicaoDaFrota(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            ISQLQuery consultaMonitoramentoPosicaoDaFrota = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoPosicaoDaFrota().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMonitoramentoPosicaoDaFrota.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoNivelServico> ConsultarRelatorioNivelServico(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaMonitoramentoNivelServico = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoNivelServico().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMonitoramentoNivelServico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoNivelServico)));

            return consultaMonitoramentoNivelServico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoNivelServico>();
        }

        public int ContarConsultaRelatorioNivelServico(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            ISQLQuery consultaMonitoramentoNivelServico = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoNivelServico().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMonitoramentoNivelServico.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta> ConsultarRelatorioAlerta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoAlerta repConsultaMonitoramentoAlerta = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoAlerta();

            ISQLQuery consultaMonitoramentoAlerta = repConsultaMonitoramentoAlerta.ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMonitoramentoAlerta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta)));

            return consultaMonitoramentoAlerta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta>();
        }

        public int ContarRelatorioAlerta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoAlerta repConsultaMonitoramentoAlerta = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoAlerta();

            ISQLQuery consultaMonitoramentoAlerta = repConsultaMonitoramentoAlerta.ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMonitoramentoAlerta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTratativaAlerta> ConsultarRelatorioTratativaAlerta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTratativaAlerta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaMonitoramentoNivelServico = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoTratativaAlerta().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMonitoramentoNivelServico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTratativaAlerta)));

            return consultaMonitoramentoNivelServico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTratativaAlerta>();
        }

        public int ContarConsultaRelatorioTratativaAlerta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTratativaAlerta filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            ISQLQuery consultaMonitoramentoNivelServico = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoTratativaAlerta().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMonitoramentoNivelServico.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoHistoricoTemperatura> ConsultarRelatorioHistoricoTemperatura(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaMonitoramentoNivelServico = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoHistoricoTemperatura().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMonitoramentoNivelServico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoHistoricoTemperatura)));

            return consultaMonitoramentoNivelServico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoHistoricoTemperatura>();
        }

        public int ContarConsultaRelatorioHistoricoTemperatura(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            ISQLQuery consultaMonitoramentoNivelServico = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoHistoricoTemperatura().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMonitoramentoNivelServico.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTempoVeiculo> ConsultarRelatorioTempoVeiculo(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaMonitoramentoTempoVeiculo = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoTempoVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMonitoramentoTempoVeiculo.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTempoVeiculo)));

            return consultaMonitoramentoTempoVeiculo.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTempoVeiculo>();
        }

        public int ContarConsultaRelatorioTempoVeiculo(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            ISQLQuery consultaMonitoramentoTempoVeiculo = new Repositorio.Embarcador.Logistica.Consulta.ConsultaMonitoramentoTempoVeiculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaMonitoramentoTempoVeiculo.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoControleEntregas> ConsultarRelatorioMonitoramentoControleEntrega(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoControleEntrega filtroPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sql = GetSQLSelectConsultaMonitoramentoEntrega(filtroPesquisa, false);

            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoControleEntregas)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoControleEntregas>();

        }

        public int ContarConsultarRelatorioMonitoramentoControleEntrega(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoControleEntrega filtroPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sqlContar = GetSQLSelectConsultaMonitoramentoEntrega(filtroPesquisa, true);
            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sqlContar);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> ConsultarParadasVeiculos(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consulta = new ConsultaMonitoramentoVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo>();
        }

        public IList<int> BuscarCodigosMonitoramentosFinalizarDataInicio(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, int limit)
        {
            List<int> listaCodigos = new List<int>();
            var sql = $@"
                SELECT distinct monitoramento.MON_CODIGO
                from t_monitoramento monitoramento 
                where monitoramento.MON_STATUS = 1 and MON_DATA_FIM is null AND DATEADD(DAY,{configuracaoMonitoramento.DiasFinalizarAutomaticamenteMonitoramentoEmAndamento}, MON_DATA_INICIO) <= GETDATE();";

            var consultaMonitoramentosEmAndamento = this.SessionNHiBernate.CreateSQLQuery(sql);
            listaCodigos.AddRange(consultaMonitoramentosEmAndamento.List<int>());

            return listaCodigos;
        }

        public List<int> BuscarCodigosMonitoramentosFinalizarDataPrevisao(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, int limit)
        {
            List<int> listaCodigos = new List<int>();

            var sql = $@"
                SELECT 
                distinct monitoramento.MON_CODIGO
            FROM 
                t_monitoramento monitoramento
            INNER JOIN 
                t_carga carga ON carga.CAR_CODIGO = monitoramento.CAR_CODIGO
            INNER JOIN 
                T_CARGA_ENTREGA entrega ON entrega.CAR_CODIGO = carga.CAR_CODIGO
            WHERE 
                monitoramento.MON_STATUS = 1
                AND monitoramento.MON_DATA_FIM IS NULL
            GROUP BY 
                monitoramento.MON_CODIGO
            HAVING 
               DATEADD(DAY,{configuracaoMonitoramento.DiasFinalizarMonitoramentoPrevisaoUltimaEntrega}, MAX(entrega.CEN_DATA_ENTREGA_PREVISTA)) <= GETDATE();";

            var consultaMonitoramentosDataPrevisao = this.SessionNHiBernate.CreateSQLQuery(sql);
            listaCodigos.AddRange(consultaMonitoramentosDataPrevisao.List<int>());
            return listaCodigos;
        }

        #endregion

        #region View

        //drop tab temp
        private string MontarDropTableTemporaria(Guid identificadorTabela)
        {
            return $@"DROP TABLE #TempMonitoramento_{identificadorTabela.ToString().Replace("-", "")}";
        }

        private string MontarTabelaTemporariaMonitoramento(Guid identificadorTabela)
        {
            return $@" SELECT Monitoramento.MON_CODIGO, Carga.CAR_CODIGO INTO #TempMonitoramento_{identificadorTabela.ToString().Replace("-", "")}";
        }

        private string MontarWhereSomenteUltimaCarga()
        {
            return @" AND ViewMonitoramento.DataCriacaoCarga = ( SELECT MAX(VM1.DataCriacaoCarga) 
                      FROM VIEW_MONITORAMENTO AS VM1
                      WHERE VM1.Carga = ViewMonitoramento.Carga ) ";
        }

        private string MontarJoinTabelaTemporaria(string whereUltimoPorCarga, Guid identificador)
        {
            return $@" WHERE ViewMonitoramento.Codigo in (SELECT Temp.MON_CODIGO FROM #TempMonitoramento_{identificador.ToString().Replace("-", "")} AS Temp) 
                {whereUltimoPorCarga}";
        }

        private string MontarSelectMonitoramento()
        {
            return @" SELECT 
                        [Codigo],
                        [Data],
                        [DataInicioMonitoramento],
                        [DataFimMonitoramento],
                        [DistanciaPrevista],
                        [DistanciaRealizada],
                        [DistanciaAteOrigem],
                        [DistanciaAteDestino],
                        [DistanciaTotal],
                        [Status],
                        [TotalTemperaturasRecebidas],
                        [TotalTemperaturasDentroFaixa],
                        [StatusViagem],
                        [CorStatusViagem],
                        [TiporRegraViagem],
                        [DataInicioStatusAtual],
                        [DataFimStatusAtual],
                        [TipoOperacao],
                        [GrupoTipoOperacao],
                        [GrupoTipoOperacaoCor],
                        [GrupoStatusViagemCor],
                        [GrupoStatusViagemCodigo],
                        [GrupoStatusViagemDescricao],
                        [PercentualViagem],
                        [Carga],
                        [CargaEmbarcador],
                        [DataCriacaoCarga],
                        [DataPrevisaoTerminoCarga],
                        [DataInicioViagem],
                        [DataInicioViagemPrevista],
                        [DataReagendamento],
                        [DataCarregamentoCarga],
                        [PesoTotalCarga],
                        [Expedidor],
                        [Recebedor],
                        [DataInicioCarregamentoJanela],
                        [DataPrevisaoChegada],
                        [DataPrevisaoChegadaPlanta],
                        [Veiculo],
                        [RazaoSocialTransportador],
                        [NomeFantasiaTransportador],
                        [Posicao],
                        [Latitude],
                        [Longitude],
                        [Velocidade],
                        [Ignicao],
                        [IDEquipamento],
                        [Temperatura],
                        [TecnologiaRastreador],
                        [Gerenciadora],
                        [TemperaturaMonitoramento],
                        [NivelGPS],
                        [DescricaoFaixaTemperatura],
                        [TemperaturaFaixaInicial],
                        [TemperaturaFaixaFinal],
                        [CodigoFilial],
                        [Filial],
                        [DataPosicaoAtual],
                        [Tracao],
                        [Observacao],
                        [TipoTrecho],
                        [Reboques],
                        [Motoristas],
                        [CPFMotoristas],
                        [Destinos],
                        [DestinosPontoPassagem],
                        [CidadeDestino],
                        [CategoriasAlvos],
                        [TotalColetas],
                        [ColetaAtual],
                        [TotalEntregas],
                        [TotalEntregasEntregues],
                        [TotalEntregasRejeitadas],
                        [TotalEntregasAderencia],
                        [TotalEntregasNoRaio],
                        [EntregaAtual],
                        [DataEntregaReprogramadaProximaEntrega],
                        [CodigoProximaEntrega],
                        [TendenciaProximaParada],
                        [TendenciaColeta],
                        [TendenciaEntrega],
                        [ProximoDestino],
                        [CodigoIntegracaoDestino],
                        [DataEntregaPlanejadaProximaEntrega],
                        [Pedidos],
                        [ValorTotalNFe],
                        [DataPrevisaoEntregaPedido],
                        [DataProgramadaColeta],
                        [DataPrevisaoDescargaJanela],
                        [NumeroEXP],
                        [PossuiContratoFrete],
                        [Critico],
                        [ClienteOrigem],
                        [CidadeOrigem],
                        [CentroResultado],
                        [FronteiraRotaFrete],
                        [TotalAlertas],
                        [TotalAlertasTratados],
                        [TotalAlertaTratativaEspecifica],
                        [Alertas],
                        [Ordens],
                        [DataSaidaOrigem],
                        [DataChegadaDestino],
                        [NumeroEquipamentoRastreador],
                        [NomeResponsavelVeiculo],
                        [CPFResponsavelVeiculo],
                        [TipoIntegracaoTecnologiaRastreador],
                        [NotasFiscais],
                        [NumeroProtocoloIntegracaoCarga],
                        [VersaoAppMotorista],
                        [NumeroPedidoEmbarcadorSumarizado],
                        [NumeroContainer],
                        [PrevisaoFimViagem],
                        [NumeroFrota],
                        [PrevisaoTerminoViagem],
                        [PrevisaoStopTranking],
                        [PrevisaoSaidaDestino],
                        [DataPrimeiraPosicao],
                        [DadosAlerta],
                        [SituacaoCarga],
                        [DataAgendamentoPedido],
                        [DataCarregamentoPedido],
                        [NumeroPedidoCliente],
                        [EscritorioVendasComplementar],
                        [MatrizComplementar],
                        [TipoCarga],
                        [DiasUteisPrazoTransportador],
                        [RetornoIntegracaoSM],
                        [SituacaoIntegracaoSM],
                        [TipoModalTransporte],
                        [CanalVenda],
                        [DataBaseCalculoPrevisaoControleEntrega],
                        [UsarGrupoDeTipoDeOperacaoNoMonitoramento],
                        [TempoSemPosicaoParaVeiculoPerderSinal],
                        [DistanciaMaximaRotaCurta],
                        [Parqueada],
                        [UltimaColetaRealizadaNoPrazo],
                        [TempoPermitidoPermanenciaEmCarregamento],
                        [TempoPermitidoPermanenciaNoCliente],
                        [UltimaEntregaRealizadaNoPrazo],
                        [DataAgendamentoParada],
                        [Mesoregiao],
                        [Regiao],
                        [DataChegadaColeta],
                        [DataSaidaColeta]
            ";
        }

        private string MontarSelectSimplificadoMapa()
        {
            return @"select
                     [Codigo]
                    ,[Data]
                    ,[DataInicioMonitoramento]
                    ,[DataFimMonitoramento]
                    ,[DistanciaPrevista]
                    ,[DistanciaRealizada]
                    ,[DistanciaAteOrigem]
                    ,[DistanciaAteDestino]
                    ,[DistanciaTotal]
                    ,[Status]
                    ,[TotalTemperaturasRecebidas]
                    ,[TotalTemperaturasDentroFaixa]
                    ,[StatusViagem]
                    ,[CorStatusViagem]
                    ,[TiporRegraViagem]
                    ,[DataInicioStatusAtual]
                    ,[DataFimStatusAtual]
                    ,[TipoOperacao]
                    ,[GrupoTipoOperacao]
                    ,[GrupoTipoOperacaoCor]
                    ,[GrupoStatusViagemCor]
                    ,[GrupoStatusViagemCodigo]
	                ,[GrupoStatusViagemDescricao]
                    ,[Carga]
                    ,[CargaEmbarcador]
                    ,[DataCriacaoCarga]
                    ,[DataPrevisaoTerminoCarga]
                    ,[DataInicioViagem]
                    ,[DataInicioViagemPrevista]
                    ,[DataReagendamento]
                    ,[DataCarregamentoCarga]
                    ,[PesoTotalCarga]
                    ,[Expedidor]
                    ,[Recebedor]
                    ,[DataInicioCarregamentoJanela]
                    ,[DataPrevisaoChegada]
                    ,[DataPrevisaoChegadaPlanta]
                    ,[Veiculo]
                    ,[RazaoSocialTransportador]
                    ,[NomeFantasiaTransportador]
                    ,[Posicao]
                    ,[Latitude]
                    ,[Longitude]
                    ,[Velocidade]
                    ,[Ignicao]
                    ,[IDEquipamento]
                    ,[Temperatura]
                    ,[TecnologiaRastreador]
                    ,[Gerenciadora]
                    ,[TemperaturaMonitoramento]
                    ,[NivelGPS]
                    ,[DescricaoFaixaTemperatura]
                    ,[TemperaturaFaixaInicial]
                    ,[TemperaturaFaixaFinal]
                    ,[CodigoFilial]
                    ,[Filial]
                    ,[DataPosicaoAtual]
                    ,[Tracao]
                    ,[Observacao]
                    ,[Reboques]
                    ,[Motoristas]
                    ,[CPFMotoristas]
                    ,[Destinos]
                    ,[DestinosPontoPassagem]
                    ,[CidadeDestino]
                    ,[ProximoDestino]
                    ,[DataEntregaPlanejadaProximaEntrega]
                    ,[DataProgramadaColeta]
                    ,[Critico]
                    ,[ClienteOrigem]
                    ,[CidadeOrigem]
                    ,[TotalAlertas]
                    ,[TotalAlertasTratados]
                    ,[TotalAlertaTratativaEspecifica]
                    ,[DataChegadaDestino]
                    ,[NumeroEquipamentoRastreador]
                    ,[TipoIntegracaoTecnologiaRastreador]
                    ,[PrevisaoFimViagem]
                    ,[NumeroFrota]
                    ,[DadosAlerta]
                    ,[SituacaoCarga]
                    ,[DataAgendamentoPedido]
                    ,[DataCarregamentoPedido]
                    ,[NumeroPedidoCliente]
                    ,[EscritorioVendasComplementar]
                    ,[MatrizComplementar]
                    ,[DiasUteisPrazoTransportador]
                    ,[TendenciaEntrega]
                    ,[TendenciaProximaParada]
                    ,[DataAgendamentoParada]
                    ,[TempoPermitidoPermanenciaEmCarregamento]
                    ,[TempoPermitidoPermanenciaNoCliente]
                    ,[PercentualViagem]
                    ,[TempoSemPosicaoParaVeiculoPerderSinal]
                    ";
        }

        private string MontarSelectSimplificadoFiltroCarrossel()
        {
            return @"select
                     [Codigo]
                    ,[Status]
                    ,[StatusViagem]
                    ,[CorStatusViagem]
                    ,[TiporRegraViagem]
                    ,[TipoOperacao]
                    ,ISNULL([GrupoTipoOperacao],'Nenhum') AS GrupoTipoOperacao
                    ,[CodigoGrupoTipoOperacao]
                    ,[GrupoTipoOperacaoCor]
                    ,[GrupoStatusViagemCor]
                    ,[GrupoStatusViagemCodigo]
	                ,[GrupoStatusViagemDescricao]
                    ,[Carga]
                    ,[SituacaoCarga]
                    ,[TendenciaEntrega]
                    ,[TendenciaProximaParada]
                    ,[PossuiAlertaEmAberto]
                    ,[DataPosicaoAtual]
                    ,[TempoSemPosicaoParaVeiculoPerderSinal]
                    ";
        }

        public int ContarConsultaView(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa)
        {
            Guid identiicadorTemp = Guid.NewGuid();

            var parametros = new List<ParametroSQL>();

            string where = GetSQLFiltrosView(filtrosPesquisa, parametros);
            string from = GetSQLFromView();

            string tabelaTemporaria = $"{MontarTabelaTemporariaMonitoramento(identiicadorTemp)} {where};";

            string whereUltimaCarga = filtrosPesquisa?.SomenteUltimoPorCarga ?? false
                ? MontarWhereSomenteUltimaCarga()
                : "";

            string sqlContar = "SELECT COUNT(1) AS Contador";
            string sql = $"{tabelaTemporaria}\n{sqlContar} {from} {MontarJoinTabelaTemporaria(whereUltimaCarga, identiicadorTemp)}\n{MontarDropTableTemporaria(identiicadorTemp)}";

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);
            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> ConsultarMonitoramento(
           Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa,
           Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Guid identificadorTabela = Guid.NewGuid();

            var parametros = new List<ParametroSQL>();

            string where = GetSQLFiltrosView(filtrosPesquisa, parametros);
            string from = GetSQLFromView();
            string tabelaTemporaria = $"{MontarTabelaTemporariaMonitoramento(identificadorTabela)} {where};\n";

            string whereUltimaCarga = filtrosPesquisa?.SomenteUltimoPorCarga ?? false
            ? MontarWhereSomenteUltimaCarga()
            : "";

            string sql = $"{tabelaTemporaria}\n{MontarSelectMonitoramento()} {from} {MontarJoinTabelaTemporaria(whereUltimaCarga, identificadorTabela)} {GetSQLParametrosView(filtrosPesquisa, parametrosConsulta)}\n{MontarDropTableTemporaria(identificadorTabela)}";

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento)));
            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> ConsultarMonitoramentoSimplificadoMapa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Guid identificadorTabela = Guid.NewGuid();

            var parametros = new List<ParametroSQL>();

            string select = MontarSelectSimplificadoMapa();
            string from = GetSQLFromView();
            string where = GetSQLFiltrosView(filtrosPesquisa, parametros);

            string whereUltimaCarga = filtrosPesquisa?.SomenteUltimoPorCarga ?? false
                    ? MontarWhereSomenteUltimaCarga()
                    : " ";

            string tabelaTemporaria = $"{MontarTabelaTemporariaMonitoramento(identificadorTabela)} {where};";

            string sql = $"{tabelaTemporaria}\n{select} {from} {MontarJoinTabelaTemporaria(whereUltimaCarga, identificadorTabela)} {GetSQLParametrosView(filtrosPesquisa, parametrosConsulta)}\n{MontarDropTableTemporaria(identificadorTabela)}";

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento)));

            return query.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> ConsultarContadorFiltroCarrosselSimplificado(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Guid identificadorTabela = Guid.NewGuid();

            var parametros = new List<ParametroSQL>();

            string select = MontarSelectSimplificadoFiltroCarrossel();
            string from = GetSQLFromView();
            string where = GetSQLFiltrosView(filtrosPesquisa, parametros);

            string whereUltimaCarga = filtrosPesquisa?.SomenteUltimoPorCarga ?? false
                    ? MontarWhereSomenteUltimaCarga()
                    : " ";

            string tabelaTemporaria = $"{MontarTabelaTemporariaMonitoramento(identificadorTabela)} {where};";
            
            string sql = $"{tabelaTemporaria}\n{select} {from} {MontarJoinTabelaTemporaria(whereUltimaCarga, identificadorTabela)} {GetSQLParametrosView(filtrosPesquisa, parametrosConsulta)}\n{MontarDropTableTemporaria(identificadorTabela)}";

            var sqlDinamico = new SQLDinamico(sql, parametros);

            ISQLQuery query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento)));

            return query.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> ObterQueryableBuscarPorCargas(IList<int> codigosCarga, IList<string> configuracaoFetchs)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> consultaMonitoramento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo));

            if (configuracaoFetchs?.Count > 0)
            {
                for (int i = 0; i < configuracaoFetchs.Count; i++)
                {
                    switch (configuracaoFetchs[i])
                    {
                        case nameof(Dominio.Entidades.Embarcador.Logistica.Monitoramento.StatusViagem):
                            consultaMonitoramento = consultaMonitoramento.Fetch(o => o.StatusViagem);
                            break;
                        case nameof(Dominio.Entidades.Embarcador.Logistica.Monitoramento.Veiculo):
                            consultaMonitoramento = consultaMonitoramento.Fetch(o => o.Veiculo);
                            break;
                        default:
                            break;
                    }
                }
            }

            return consultaMonitoramento.OrderByDescending(o => o.DataCriacao);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> ConsultaNivelServico()
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.Monitoramento> result = from obj in query
                                                                                      where obj.Carga.Pedidos.Any(ped => ped.DataChegada != null && ped.DataSaida != null)
                                                                                      select obj;

            return result;
        }

        private string GetSQLSelectResumoCargasSituacao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            return $@"  
            SELECT
                 MonitoramentoStatus.MSV_DESCRICAO StatusViagem,
                 MonitoramentoStatus.MSV_CODIGO CodigoStatusViagem,
                (
                SELECT
                    count(1)               
                    FROM
                        t_alerta_monitor Alerta               
                    WHERE
                        Alerta.CAR_CODIGO = Carga.CAR_CODIGO       
                ) TotalAlertas,
		        SUBSTRING((SELECT
                    ', '+ MonitoramentoEvento.MEV_DESCRICAO  AS [text()]                              
                FROM
                    t_alerta_monitor Alerta                              
                JOIN
                    T_MONITORAMENTO_EVENTO MonitoramentoEvento 
                        ON MonitoramentoEvento.MEV_CODIGO = Alerta.MEV_CODIGO                              
                WHERE
                    Alerta.CAR_CODIGO = Carga.CAR_CODIGO 
			    group by MonitoramentoEvento.MEV_DESCRICAO
                ORDER BY
                    MonitoramentoEvento.MEV_DESCRICAO FOR XML PATH ('')), 3, 1000) Alertas   
                    ";
        }

        private string GetSQLSelectConsultaMonitoramentoEntrega(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoControleEntrega filtroPesquisa, bool contar)
        {
            string sqlSelect = @"ret.Pedido
                             ,ret.Carga
                             ,ret.CargaCodigoEmbarcador
                             ,ret.Cliente
                             ,ret.CodCliente
                             ,ret.DataMonitoramento
                             ,ret.CEN_COLETA Coleta
                             ,ret.CEN_CODIGO CodEntrega
                             ,ret.NotasFiscais
                             ,ret.Transp
                             ,ret.NomeTransportador
                             ,ret.NomeFantasiaTransportador
                             ,ret.LocalizacaoCliente
                             ,ret.DataEntradaRaio
                             ,ret.DataSaidaRaio
                             ,ret.DataCarregamento
                             ,ret.DataEntregaPrevista
                             ,ret.TempoRaio
                             ,Mon.MON_CODIGO CodMonitoramento
                             ,ret.kmOrigemDestino KmOrigemDestino
                             ,ret.CodVeiculo
                             ,ret.PlacaVeiculo
                             ,ret.TempoViagem
                             ,ret.DataPosicaoAtual
                             ,ret.DataFimEventoSemSinal
                             ,ret.DataInicioEventoSemSinal
                             ,ret.TotalTemperaturasRecebidas
                             ,ret.TotalTemperaturasDentroFaixa
                             ,tipo.TOP_DESCRICAO TipoOperacao";


            string sql = $@"select {(contar ? " count(1) " : sqlSelect)}
                            from (
                            select distinct               
                                    monativo.MON_DATA_CRIACAO as DataMonitoramento
                                   ,monativo.MON_NUMERO_TEMPERATURA_RECEBIDA TotalTemperaturasRecebidas
                                   ,monativo.MON_TEMPERATURA_NA_FAIXA TotalTemperaturasDentroFaixa
                                   ,alerta.ALE_DATA DataInicioEventoSemSinal
                                   ,alerta.ALE_DATA_CADASTRO DataFimEventoSemSinal
                                   ,Posicao.POS_DATA_VEICULO DataPosicaoAtual
		                           ,ce.CEN_COLETA
	                               ,ce.CEN_CODIGO
	                               ,ce.CLI_CODIGO_ENTREGA CodCliente
                                   ,Carga.CAR_CODIGO Carga
                                   ,carga.CAR_CODIGO_CARGA_EMBARCADOR CargaCodigoEmbarcador
	                               ,Carga.EMP_CODIGO Transp
	                               ,emp.EMP_FANTASIA NomeFantasiaTransportador
                                   ,SUBSTRING((SELECT emp.EMP_RAZAO  + ' (' + loctrans.LOC_DESCRICAO + '/' + loctrans.UF_SIGLA + ')' AS [text()]             
		                            FOR XML PATH , TYPE).value(N'.[1]', N'nvarchar(max)'), 0, 2000) NomeTransportador
                                   ,veiculo.VEI_CODIGO CodVeiculo
	                               ,veiculo.VEI_PLACA PlacaVeiculo
                                   ,SUBSTRING((SELECT cli.CLI_NOME  + ' (' + loc.LOC_DESCRICAO + '/' + loc.UF_SIGLA + ')' AS [text()]             
		                            FOR XML PATH , TYPE).value(N'.[1]', N'nvarchar(max)'), 0, 2000) Cliente
                                  ,SUBSTRING((select distinct ', ' + cast(pedido.PED_NUMERO_PEDIDO_EMBARCADOR as varchar(30)) FROM T_CARGA_ENTREGA_PEDIDO cep
                                                                         inner join T_CARGA_PEDIDO cargapedido on cargapedido.CPE_CODIGO = cep.CPE_CODIGO 
                                                                         inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
      									                                 WHERE cep.CEN_CODIGO  = ce.CEN_CODIGO
                                   for xml path('')),3,1000) Pedido
								   ,SUBSTRING((select distinct ', ' + cast(nf.NF_NUMERO as varchar(30)) FROM T_PEDIDO_XML_NOTA_FISCAL pnf  
									                             inner join T_XML_NOTA_FISCAL nf on nf.NFX_CODIGO = pnf.NFX_CODIGO
								                             WHERE  pnf.CPE_CODIGO = cargapedido.CPE_CODIGO
									for xml path('')),3,1000) NotasFiscais
                                   ,cli.LOC_CODIGO LocalizacaoCliente
                                   ,ce.CEN_DATA_ENTRADA_RAIO DataEntradaRaio
                                   ,ce.CEN_DATA_SAIDA_RAIO DataSaidaRaio
	                               ,jan.CJC_INICIO_CARREGAMENTO DataCarregamento
	                               ,ce.CEN_DATA_ENTREGA_PREVISTA DataEntregaPrevista
	                               ,COALESCE(CAST(ce.CEN_DISTANCIA_ATE_DESTINO as DECIMAL(9,2)), 0) kmOrigemDestino
                                   ,case when carga.CAR_DATA_INICIO_VIAGEM is not null 
	                                     then DATEDIFF(MINUTE, carga.CAR_DATA_INICIO_VIAGEM , case when carga.CAR_DATA_FIM_VIAGEM is not null then carga.CAR_DATA_FIM_VIAGEM else cast(getdate() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time' as datetime) 
		                                 end) 
	                                end TempoViagem
	                               ,DATEDIFF(MINUTE, ce.CEN_DATA_ENTRADA_RAIO, ce.CEN_DATA_SAIDA_RAIO) TempoRaio
	                               ,max(monativo.MON_CODIGO) CodMonitoramento
                                        from T_CARGA Carga  
         	                            inner join T_CARGA_ENTREGA ce                 on ce.CAR_CODIGO       = Carga.CAR_CODIGO
			                            inner join T_EMPRESA emp                      on emp.EMP_CODIGO      = Carga.EMP_CODIGO
			                            left  join T_CARGA_JANELA_CARREGAMENTO jan    on jan.car_codigo      = Carga.CAR_CODIGO
         	                            inner join T_MONITORAMENTO monativo           on monativo.CAR_CODIGO = Carga.CAR_CODIGO
			                            inner join T_VEICULO veiculo                  on monativo.VEI_CODIGO = veiculo.VEI_CODIGO	
                                        left  join T_CLIENTE cli                      on cli.CLI_CGCCPF = ce.CLI_CODIGO_ENTREGA
                                        left  join T_ALERTA_MONITOR alerta            on alerta.CAR_CODIGO = Carga.CAR_CODIGO and alerta.ALE_TIPO = 6
			                            left  join T_LOCALIDADES loc				  on cli.LOC_CODIGO  = loc.LOC_CODIGO	
			                            left  join T_LOCALIDADES loctrans			  on emp.LOC_CODIGO  = loctrans.LOC_CODIGO	
                                        left  join T_POSICAO Posicao                  on Posicao.POS_CODIGO = monativo.POS_ULTIMA_POSICAO
         	                            inner join T_CARGA_PEDIDO cargapedido         on cargapedido.CAR_CODIGO = Carga.CAR_CODIGO
                                        where              
         	                            carga.CAR_SITUACAO <> 13 and Carga.CAR_SITUACAO <> 18 {(!string.IsNullOrEmpty(filtroPesquisa.CodigoCargaEmbarcador) ? filtroPesquisa.FiltrarCargasPorParteDoNumero ? $" and Carga.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtroPesquisa.CodigoCargaEmbarcador}%'" : " and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '" + filtroPesquisa.CodigoCargaEmbarcador + "'" : "")}
                                        {(filtroPesquisa.Filiais.Any(codigo => codigo == -1) ? $@" and (carga.FIL_CODIGO in ({string.Join(",", filtroPesquisa.Filiais)}) OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtroPesquisa.Recebedores)})))" : filtroPesquisa.CodigoFilial > 0 ? " and carga.FIL_CODIGO = " + filtroPesquisa.CodigoFilial : "")}
                                        {(filtroPesquisa.NumeroNotaFiscal > 0 ? "and '" + filtroPesquisa.NumeroNotaFiscal + "' in (select nf.NF_NUMERO FROM T_CARGA_ENTREGA_PEDIDO cep inner join T_CARGA_PEDIDO cargapedido on cargapedido.CPE_CODIGO = cep.CPE_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL pnf on pnf.CPE_CODIGO = cargapedido.CPE_CODIGO inner join T_XML_NOTA_FISCAL nf on nf.NFX_CODIGO = pnf.NFX_CODIGO WHERE  cep.CEN_CODIGO  = ce.CEN_CODIGO)" : "")}
                                        {(!string.IsNullOrEmpty(filtroPesquisa.NumeroPedido) ? "and '" + filtroPesquisa.NumeroPedido + "' in (select pedido.PED_NUMERO_PEDIDO_EMBARCADOR FROM T_CARGA_ENTREGA_PEDIDO cep inner join T_CARGA_PEDIDO cargapedido on cargapedido.CPE_CODIGO = cep.CPE_CODIGO inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO WHERE cep.CEN_CODIGO  = ce.CEN_CODIGO)" : "")}
		
                            group by
                                        monativo.MON_DATA_CRIACAO 
                                       ,monativo.MON_NUMERO_TEMPERATURA_RECEBIDA
                                       ,monativo.MON_TEMPERATURA_NA_FAIXA
                                       ,alerta.ALE_DATA 
                                       ,alerta.ALE_DATA_CADASTRO
                                       ,Posicao.POS_DATA_VEICULO
                                       ,ce.CEN_CODIGO
			                           ,ce.CEN_COLETA
		                               ,monativo.MON_DATA_CRIACAO
	                                   ,ce.cen_ordem
                                       ,Carga.CAR_CODIGO
                                       ,carga.CAR_CODIGO_CARGA_EMBARCADOR 
                                       ,concat(carga.CAR_CODIGO_CARGA_EMBARCADOR , ce.cen_ordem)    
	                                   ,ce.CLI_CODIGO_ENTREGA 
		                               ,cli.CLI_NOME
		                               ,emp.EMP_RAZAO
		                               ,emp.EMP_FANTASIA
                                       ,carga.CAR_DATA_INICIO_VIAGEM
		                               ,carga.CAR_DATA_FIM_VIAGEM 
                                       ,veiculo.VEI_CODIGO
		                               ,veiculo.VEI_PLACA
		                               ,Carga.EMP_CODIGO
                                       ,cli.LOC_CODIGO 
		                               ,loc.UF_SIGLA
		                               ,loc.LOC_DESCRICAO
		                               ,loctrans.UF_SIGLA
		                               ,loctrans.LOC_DESCRICAO
                                       ,ce.CEN_DATA_ENTRADA_RAIO
		                               ,jan.CJC_INICIO_CARREGAMENTO 
		                               ,ce.CEN_DATA_ENTREGA_PREVISTA 
                                       ,ce.CEN_DATA_SAIDA_RAIO   
      	                               ,ce.CEN_SITUACAO
		                               ,ce.cEN_DISTANCIA_ATE_DESTINO
                                       ,CargaPedido.CPE_CODIGO

                            ) ret
                              left  join T_MONITORAMENTO mon                on mon.CAR_CODIGO = ret.Carga and mon.MON_CODIGO = ret.CodMonitoramento				 
                              left  join T_POSICAO upos                     on upos.POS_CODIGO = mon.POS_ULTIMA_POSICAO
                              left  join T_CARGA carga                      on carga.CAR_CODIGO = mon.CAR_CODIGO
                              left  join T_TIPO_OPERACAO tipo               on tipo.TOP_CODIGO = carga.TOP_CODIGO
                              left  join T_CONFIGURACAO_EMBARCADOR cfg      on 1=1 where 1=1
                              {(filtroPesquisa.CodigoVeiculo > 0 ? " and mon.VEI_CODIGO = " + filtroPesquisa.CodigoVeiculo : "")}
                              {(filtroPesquisa.DataMonitoramentoInicial != DateTime.MinValue ? " and mon.MON_DATA_CRIACAO >= '" + filtroPesquisa.DataMonitoramentoInicial.ToString("yyyy-MM-dd") + "' " : "")}
                              {(filtroPesquisa.DataMonitoramentoFinal != DateTime.MinValue ? " and mon.MON_DATA_CRIACAO <= '" + filtroPesquisa.DataMonitoramentoFinal.ToString("yyyy-MM-dd") + "'" : "")}
                              {(filtroPesquisa.CodigoTipoOperacao > 0 ? " and tipo.TOP_CODIGO = " + filtroPesquisa.CodigoTipoOperacao : "")}
                              ";

            return sql;
        }

        private string GetSQLSelect(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = null)
        {
            string sql = "";
            if (filtrosPesquisa != null && filtrosPesquisa.SomenteUltimoPorCarga)
            {
                sql += "SELECT * FROM ( ";
            }

            sql += $@"
                SELECT 
                    Monitoramento.MON_CODIGO Codigo,
                    Monitoramento.MON_DATA_CRIACAO Data,
                    isNull(Monitoramento.MON_DATA_INICIO, Monitoramento.MON_DATA_CRIACAO) DataInicioMonitoramento,
                    Monitoramento.MON_DATA_FIM DataFimMonitoramento,
                    Monitoramento.MON_DISTANCIA_PREVISTA DistanciaPrevista,
                    Monitoramento.MON_DISTANCIA_REALIZADA DistanciaRealizada,
                    Monitoramento.MON_DISTANCIA_ATE_ORIGEM DistanciaAteOrigem,
                    Monitoramento.MON_DISTANCIA_ATE_DESTINO DistanciaAteDestino,
                    Monitoramento.MON_DISTANCIA_PREVISTA DistanciaTotal,
                    Monitoramento.MON_STATUS Status,
                    Monitoramento.MON_NUMERO_TEMPERATURA_RECEBIDA TotalTemperaturasRecebidas,
                    Monitoramento.MON_TEMPERATURA_NA_FAIXA TotalTemperaturasDentroFaixa,
                    MonitoramentoStatus.MSV_DESCRICAO StatusViagem,
		            MonitoramentoStatus.MSV_TIPO_REGRA TiporRegraViagem,
		            (SELECT
                        top  1 MonitoramentoHistorico.MHS_DATA_INICIO 
                    FROM
                        T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM  MonitoramentoHistorico 
                    WHERE
                        MonitoramentoHistorico.MON_CODIGO = Monitoramento.MON_CODIGO 
                        and MonitoramentoHistorico.MSV_CODIGO = MonitoramentoStatus.MSV_CODIGO) DataInicioStatusAtual,
                    (SELECT
                        top  1 MonitoramentoHistorico.MHS_DATA_FIM 
                    FROM
                        T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM  MonitoramentoHistorico 
                    WHERE
                        MonitoramentoHistorico.MON_CODIGO = Monitoramento.MON_CODIGO 
                        and MonitoramentoHistorico.MSV_CODIGO = MonitoramentoStatus.MSV_CODIGO) DataFimStatusAtual,
                    TipoOperacao.TOP_DESCRICAO TipoOperacao,
                    GrupoTipoOperacao.GTO_DESCRICAO GrupoTipoOperacao,
                    Monitoramento.MON_PERCENTUAL_VIAGEM PercentualViagem,
                    Carga.CAR_CODIGO Carga,
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR CargaEmbarcador,
                    Carga.CAR_DATA_CRIACAO DataCriacaoCarga,
                    Carga.CAR_DATA_TERMINO_CARGA DataPrevisaoTerminoCarga,
                    Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem,
                    Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA DataInicioViagemPrevista,
                    Carga.CAR_DATA_REAGENDAMENTO DataReagendamento,
                    Carga.CAR_DATA_CARREGAMENTO DataCarregamentoCarga,
                    CargaDadosSumarizados.CDS_PESO_TOTAL PesoTotalCarga,
                    CASE WHEN CargaDadosSumarizados.CDS_EXPEDIDORES <> '' THEN CargaDadosSumarizados.CDS_EXPEDIDORES+'-'+CDS_ORIGENS ELSE '' END Expedidor,
                    CASE WHEN CargaDadosSumarizados.CDS_RECEBEDORES <> '' THEN CargaDadosSumarizados.CDS_RECEBEDORES+'-'+CDS_DESTINOS ELSE '' END Recebedor,
                    (SELECT top 1 JanelaCarregamento.CJC_INICIO_CARREGAMENTO FROM T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento WHERE JanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO and JanelaCarregamento.CEC_CODIGO is not null) DataInicioCarregamentoJanela,
                    Carga.CAR_DATA_FIM_VIAGEM_PREVISTA DataPrevisaoChegada,
                    Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM DataPrevisaoChegadaPlanta,
                    Veiculo.VEI_CODIGO Veiculo,
                    Empresa.EMP_RAZAO RazaoSocialTransportador,
                    Empresa.EMP_FANTASIA NomeFantasiaTransportador,
                    Posicao.POS_DESCRICAO Posicao,
                    Posicao.POS_LATITUDE Latitude,
                    Posicao.POS_LONGITUDE Longitude,
                    Posicao.POS_VELOCIDADE Velocidade,
                    Posicao.POS_IGNICAO Ignicao,
                    Posicao.POS_ID_EQUIPAMENTO IDEquipamento,
                    Posicao.POS_TEMPERATURA Temperatura,
                    Posicao.POS_RASTREADOR TecnologiaRastreador,
                    Posicao.POS_GERENCIADORA Gerenciadora,
                    Monitoramento.MON_ULTIMA_TEMPERATURA TemperaturaMonitoramento,
		            Posicao.POS_NIVEL_SINAL_GPS NivelGPS,
                    FaixaTemperatura.FTE_DESCRICAO DescricaoFaixaTemperatura,
                    FaixaTemperatura.FTE_FAIXA_INICIAL TemperaturaFaixaInicial,
                    FaixaTemperatura.FTE_FAIXA_FINAL TemperaturaFaixaFinal,
                    Filial.FIL_CODIGO_FILIAL_EMBARCADOR CodigoFilial,
                    Filial.FIL_DESCRICAO Filial,
                    Posicao.POS_DATA_VEICULO DataPosicaoAtual,
                    Veiculo.VEI_PLACA as Tracao,
                    Monitoramento.MON_OBSERVACAO as Observacao,
                    TipoTrecho.TTR_DESCRICAO as TipoTrecho,
                    Terceiro.CLI_NOME as Subcontratado,
                    SUBSTRING((SELECT ', ' + Veiculo1.vei_placa AS [text()]  
		                        FROM T_CARGA_VEICULOS_VINCULADOS VeiculosVinculados
		                        JOIN T_VEICULO VEICULO1 on Veiculo1.VEI_CODIGO = VeiculosVinculados.VEI_CODIGO
		                        WHERE VeiculosVinculados.CAR_CODIGO = Carga.CAR_CODIGO
		                        FOR XML PATH ('')), 3, 1000) Reboques,
                    SUBSTRING((SELECT ', '+ Motorista.FUN_NOME  AS [text()]
                                FROM T_CARGA_MOTORISTA CargaMotorista
                                JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
                                ORDER BY Motorista.FUN_NOME
                                FOR XML PATH ('')), 3, 1000) Motoristas, 
                    SUBSTRING((SELECT ', '+ Motorista.FUN_CPF  AS [text()]
                                FROM T_CARGA_MOTORISTA CargaMotorista
                                JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
                                ORDER BY Motorista.FUN_NOME
                                FOR XML PATH ('')), 3, 1000) CPFMotoristas, 
                    SUBSTRING((SELECT ', ' {((configuracao.ApresentarCodigoIntegracaoComNomeFantasiaCliente) ? "+ CASE isnull(ClienteEntrega.CLI_CODIGO_INTEGRACAO, '') WHEN '' THEN '' ELSE ClienteEntrega.CLI_CODIGO_INTEGRACAO + '-' END " : "")}
                                + CASE isnull(ClienteEntrega.CLI_NOMEFANTASIA, '') WHEN '' THEN ClienteEntrega.CLI_NOME ELSE ClienteEntrega.CLI_NOMEFANTASIA END 
                                + ' (' + Localidade.LOC_DESCRICAO + '/' + Localidade.UF_SIGLA + ')' AS [text()]             
		                        FROM T_CARGA_ENTREGA CargaEntrega
		                        JOIN t_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
		                        JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
		                        WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_COLETA = 0
                                ORDER BY CargaEntrega.CEN_ORDEM
		                        FOR XML PATH , TYPE).value(N'.[1]', N'nvarchar(max)'), 3, 2000) Destinos,
                    SUBSTRING((SELECT ', ' + Localidade.LOC_DESCRICAO + '/' + Localidade.UF_SIGLA AS [text()]             
		                        FROM T_CARGA_ENTREGA CargaEntrega
		                        JOIN t_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
		                        JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
		                        WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_COLETA = 0
                                ORDER BY CargaEntrega.CEN_ORDEM
		                        FOR XML PATH , TYPE).value(N'.[1]', N'nvarchar(max)'), 3, 2000) CidadeDestino,
                    SUBSTRING((SELECT', ' + CategoriaCliente.CTP_DESCRICAO AS [text()]                                                        
                                FROM T_POSICAO_ALVO PosicaoAlvo
                                JOIN t_CLIENTE ClienteAlvo  on ClienteAlvo.CLI_CGCCPF = PosicaoAlvo.CLI_CGCCPF                                           
                                left join T_CATEGORIA_PESSOA as CategoriaCliente on CategoriaCliente.CTP_CODIGO = ClienteAlvo.CTP_CODIGO                                              
                                WHERE PosicaoAlvo.POS_CODIGO = Posicao.POS_CODIGO                                                 
                                ORDER BY CategoriaCliente.CTP_DESCRICAO
                                FOR XML PATH ('')), 3,2000) CategoriasAlvos,
                    (select count(1) from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_COLETA = 1) TotalColetas,
                    (select count(1)+1 from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO = 2 and CargaEntrega.CEN_COLETA = 1) ColetaAtual,
                    (select count(1) from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_COLETA = 0) TotalEntregas,
             	    (select count(1) from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO = 2) TotalEntregasEntregues,
                    (select count(1) from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO = 3) TotalEntregasRejeitadas,
	                (select sum(CASE WHEN CargaEntrega.CEN_ORDEM = CargaEntrega.CEN_ORDEM_REALIZADA THEN 1 ELSE 0 END) from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO = 2) TotalEntregasAderencia,
                    (select sum(CAST(CargaEntrega.CEN_ENTREGA_NO_RAIO AS INT)) from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO = 2) TotalEntregasNoRaio,
					(select count(1)+1 from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO = 2 and CargaEntrega.CEN_COLETA = 0) EntregaAtual,
					(select isnull(min(CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA), min(CargaEntrega.CEN_DATA_ENTREGA_PREVISTA)) CEN_DATA_ENTREGA_REPROGRAMADA from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO != 2 and CargaEntrega.CEN_COLETA = 0 and CargaEntrega.CEN_POSTO_FISCAL = 0) DataEntregaReprogramadaProximaEntrega,
                    (select min(CargaEntrega.CEN_CODIGO) CEN_CODIGO from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO != 2 and CargaEntrega.CEN_COLETA = 0) CodigoProximaEntrega,
                    (SELECT TOP 1 CargaEntrega.CEN_TENDENCIA
					    FROM t_carga_entrega CargaEntrega 
					    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
					    AND CargaEntrega.CEN_SITUACAO != 2
					    ORDER BY CEN_ORDEM) TendenciaEntrega,
					(SELECT TOP 1 CASE isnull(ClienteEntrega.CLI_NOMEFANTASIA, '') WHEN '' THEN ClienteEntrega.CLI_NOME ELSE ClienteEntrega.CLI_NOMEFANTASIA END
					    FROM t_carga_entrega CargaEntrega 
					    join t_cliente ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
					    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
					    AND CargaEntrega.CEN_SITUACAO != 2
					    ORDER BY CEN_ORDEM) ProximoDestino,
                    CASE WHEN CargaDadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS <> '' THEN CargaDadosSumarizados.CDS_CODIGO_INTEGRACAO_DESTINATARIOS+' - '+CargaDadosSumarizados.CDS_DESTINOS ELSE '' END CodigoIntegracaoDestino,
                    (select min(CargaEntrega.CEN_DATA_ENTREGA_PREVISTA) from t_carga_entrega CargaEntrega where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_SITUACAO != 2 and CargaEntrega.CEN_COLETA = 0) DataEntregaPlanejadaProximaEntrega,
                    SUBSTRING((SELECT', ' + SUBSTRING(trim(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR), CHARINDEX('_',trim(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR)) + 1, LEN(trim(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR))) AS [text()]      
			                                FROM t_carga_pedido CargaPedido 
			                                JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
			                                WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO
			                                ORDER BY Pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                                            FOR XML PATH ('')), 3,2000) Pedidos,
		                (select 
		                 sum(NotaFiscal.NF_VALOR)
		                 FROM
                             t_carga_pedido CargaPedido
			                 JOIN
                            t_pedido_xml_nota_fiscal PedidoNotaFiscal 
                                ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO 
			                JOIN 
			                t_xml_nota_fiscal NotaFiscal 
			                    ON PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO
	                      WHERE
                            CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO) as ValorTotalNFe,
                    (select max(Pedido.PED_PREVISAO_ENTREGA)
							FROM t_carga_pedido CargaPedido 
							JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
							LEFT JOIN (
								SELECT TOP 1 CLI_CODIGO_ENTREGA AS CLI_CODIGO
								FROM t_carga_entrega CargaEntrega 
								WHERE CargaEntrega.CAR_CODIGO = Monitoramento.CAR_CODIGO
								AND CargaEntrega.CEN_SITUACAO != 2 
								AND CargaEntrega.CEN_COLETA = 0
								ORDER BY CEN_ORDEM
							) ClienteEntrega on ClienteEntrega.CLI_CODIGO = Pedido.CLI_CODIGO
							WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO) DataPrevisaoEntregaPedido,
                    (select max(Pedido.PED_DATA_INICIAL_COLETA)
							FROM t_carga_pedido CargaPedido 
							JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
							JOIN (
								SELECT TOP 1 CLI_CODIGO_ENTREGA AS CLI_CODIGO
								FROM t_carga_entrega CargaEntrega 
								WHERE CargaEntrega.CAR_CODIGO = Monitoramento.CAR_CODIGO
								AND CargaEntrega.CEN_SITUACAO != 2 
								AND CargaEntrega.CEN_COLETA = 0
								ORDER BY CEN_ORDEM
							) ClienteEntrega on ClienteEntrega.CLI_CODIGO = Pedido.CLI_CODIGO
							WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO) DataProgramadaColeta,
                    (select min(JanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO)
					        from T_CARGA_JANELA_DESCARREGAMENTO JanelaDescarregamento
					        join T_CENTRO_DESCARREGAMENTO CentroDescarregamento on CentroDescarregamento.CED_CODIGO = JanelaDescarregamento.CED_CODIGO
					        join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = JanelaDescarregamento.CAR_CODIGO and CentroDescarregamento.CLI_CGCCPF_DESTINATARIO = CargaEntrega.CLI_CODIGO_ENTREGA
					        where JanelaDescarregamento.CAR_CODIGO = Carga.CAR_CODIGO
                            and isnull(JanelaDescarregamento.CJD_CANCELADA, 0) = 0
					        and CargaEntrega.CEN_DATA_ENTREGA is null) DataPrevisaoDescargaJanela,
                    SUBSTRING((SELECT', ' + Pedido.PED_NUMERO_EXP AS [text()] 
			                                FROM t_carga_pedido CargaPedido 
			                                JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
			                                WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO
                                            FOR XML PATH ('')), 3,2000) NumeroEXP,
                    (SELECT DISTINCT 1
                            FROM T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador
                            JOIN T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoFreteTransportadorVeiculo on ContratoFreteTransportadorVeiculo.CFT_CODIGO = ContratoFreteTransportador.CFT_CODIGO
                            WHERE ContratoFreteTransportador.CFT_ATIVO = 1
	                        AND ContratoFreteTransportador.CFT_SITUACAO in (1,2)
	                        AND CURRENT_TIMESTAMP between ContratoFreteTransportador.CFT_DATA_INICIAL and ContratoFreteTransportador.CFT_DATA_FINAL
	                        AND ContratoFreteTransportadorVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO
                    ) PossuiContratoFrete,
                    cast (Monitoramento.MON_CRITICO as bit) Critico,
                    (SELECT TOP 1 
						    CASE 
							    WHEN CargaPedido.CLI_CODIGO_EXPEDIDOR IS NOT NULL THEN 
								    {((configuracao.ApresentarCodigoIntegracaoComNomeFantasiaCliente) ? "+ CASE isnull(Cliente1.CLI_CODIGO_INTEGRACAO, '') WHEN '' THEN '' ELSE Cliente1.CLI_CODIGO_INTEGRACAO + '-' END " : "")}
                                    + CASE isnull(Cliente1.CLI_NOMEFANTASIA, '') WHEN '' THEN Cliente1.CLI_NOME ELSE Cliente1.CLI_NOMEFANTASIA END
							    ELSE 
                                    {((configuracao.ApresentarCodigoIntegracaoComNomeFantasiaCliente) ? "+ CASE isnull(Cliente2.CLI_CODIGO_INTEGRACAO, '') WHEN '' THEN '' ELSE Cliente2.CLI_CODIGO_INTEGRACAO + '-' END " : "")}
                                    + CASE isnull(Cliente2.CLI_NOMEFANTASIA, '') WHEN '' THEN Cliente2.CLI_NOME ELSE Cliente2.CLI_NOMEFANTASIA END
						    END ClienteOrigem
						    FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
						    LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
			                WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO
                    ) ClienteOrigem,
					(SELECT TOP 1 
						    CASE 
							    WHEN CargaPedido.CLI_CODIGO_EXPEDIDOR IS NOT NULL THEN Localidade1.LOC_DESCRICAO + '/' +  Localidade1.UF_SIGLA
							    ELSE Localidade2.LOC_DESCRICAO + '/' +  Localidade2.UF_SIGLA
						    END CidadeOrigem
						    FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
						    LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
			                WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO
                    ) CidadeOrigem,
					SUBSTRING((SELECT', ' + CentroResultado.CRE_DESCRICAO
						                    FROM T_CARGA_PEDIDO CargaPedido 
						                    JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
						                    JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = Pedido.CRE_CODIGO
			                                WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO
                                            FOR XML PATH ('')), 3,2000) CentroResultado,
					SUBSTRING((SELECT', ' + ClienteFronteira.CLI_NOME 
						                    FROM T_CARGA_FRONTEIRA CargaFronteira 
                                            left join T_CLIENTE ClienteFronteira on ClienteFronteira.CLI_CGCCPF = CargaFronteira.CLI_CGCCPF
			                                WHERE CargaFronteira.CAR_CODIGO = Monitoramento.CAR_CODIGO
                                            FOR XML PATH ('')), 3,2000) FronteiraRotaFrete,
					(SELECT count(1)
					        FROM t_alerta_monitor Alerta
                            inner join T_MONITORAMENTO_EVENTO MonitoramentoEvento ON MonitoramentoEvento.MEV_TIPO_ALERTA = Alerta.ALE_TIPO
					        WHERE MonitoramentoEvento.MEV_CONSIDERAR_SEMAFORO = 1 and Alerta.CAR_CODIGO = Carga.CAR_CODIGO
					) TotalAlertas,
		            (SELECT	
			            COUNT(1)  
			        FROM
                        t_alerta_monitor Alerta 
                    inner join T_MONITORAMENTO_EVENTO MonitoramentoEvento ON MonitoramentoEvento.MEV_TIPO_ALERTA = Alerta.ALE_TIPO
		            inner join T_ALERTA_TRATATIVA tratativa on Alerta.ALE_CODIGO = tratativa.ALE_CODIGO
                    WHERE
                    MonitoramentoEvento.MEV_CONSIDERAR_SEMAFORO = 1 and Alerta.CAR_CODIGO = Carga.CAR_CODIGO ) TotalAlertasTratados,
			        (
		            SELECT	
			            COUNT(1)  
			        FROM
                        t_alerta_monitor Alerta
                    inner join T_MONITORAMENTO_EVENTO MonitoramentoEvento ON MonitoramentoEvento.MEV_TIPO_ALERTA = Alerta.ALE_TIPO
		            inner join T_ALERTA_TRATATIVA tratativa on Alerta.ALE_CODIGO = tratativa.ALE_CODIGO 
			        inner join T_ALERTA_TRATATIVA_ACAO acao on tratativa.ATC_CODIGO = acao.ATC_CODIGO
                    WHERE
                    Alerta.CAR_CODIGO = Carga.CAR_CODIGO and acao.ATC_ACAO_MONITORADA = 1 and MonitoramentoEvento.MEV_CONSIDERAR_SEMAFORO = 1) TotalAlertaTratativaEspecifica,
                    SUBSTRING((SELECT ', '+ MonitoramentoEvento.MEV_DESCRICAO AS [text()], + ' (' + CAST(count(1) as varchar(10)) + ') '
                            FROM t_alerta_monitor Alerta
                            INNER JOIN T_MONITORAMENTO_EVENTO MonitoramentoEvento ON MonitoramentoEvento.MEV_CODIGO = Alerta.MEV_CODIGO and MonitoramentoEvento.MEV_ATIVO = 1
                            WHERE Alerta.CAR_CODIGO = Carga.CAR_CODIGO
                            GROUP BY MonitoramentoEvento.MEV_DESCRICAO ORDER BY MonitoramentoEvento.MEV_DESCRICAO
                            FOR XML PATH ('')), 3, 1000) Alertas,
                    SUBSTRING((SELECT ', ' + rtrim(ltrim(Pedido.PED_ORDEM)) AS [text()]                                           
                            FROM t_carga_pedido CargaPedido                                      
                            JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO                                     
                            WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO                                     
                            ORDER BY Pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                            FOR XML PATH ('')), 3, 2000) Ordens,
                    (
                        SELECT min(CargaEntrega.CEN_DATA_SAIDA_RAIO) 
                        FROM T_CARGA_ENTREGA CargaEntrega 
                        WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO 
                        AND CargaEntrega.CEN_DATA_SAIDA_RAIO is not null 
                        AND CargaEntrega.CEN_COLETA = 1
                    ) DataSaidaOrigem,
		            (
                        SELECT min(CargaEntrega.CEN_DATA_ENTRADA_RAIO) 
                        FROM T_CARGA_ENTREGA CargaEntrega 
                        WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO 
                        AND CargaEntrega.CEN_DATA_ENTRADA_RAIO is not null 
                        AND CargaEntrega.CEN_COLETA = 0
                     ) DataChegadaDestino,
                    Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR NumeroEquipamentoRastreador,
                    FuncionarioResponsavel.FUN_NOME NomeResponsavelVeiculo,
                    FuncionarioResponsavel.FUN_CPF CPFResponsavelVeiculo,
                    TecnologiaRastreador.TRA_TIPO_INTEGRACAO TipoIntegracaoTecnologiaRastreador,
                    SUBSTRING((
                        SELECT ', ' + 
                            CAST(NotaFiscal.NF_NUMERO AS VARCHAR(20))
                        FROM T_CARGA_PEDIDO CargaPedido
                            LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                            LEFT JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO
                        WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                        FOR XML PATH ('')
                    ), 3, 1000) NotasFiscais,
                    COALESCE(
                        SUBSTRING((SELECT ', ' + CargaDadosTransporteIntegracao.CDI_PROTOCOLO
                            FROM T_CARGA_DADOS_TRANSPORTE_INTEGRACAO CargaDadosTransporteIntegracao
                            LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaDadosTransporteIntegracao.CAR_CODIGO
                            WHERE Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO AND CargaDadosTransporteIntegracao.CDI_PROTOCOLO IS NOT NULL AND CargaDadosTransporteIntegracao.CDI_PROTOCOLO <> ''
                            FOR XML PATH ('')
                        ), 3, 1000),
                        SUBSTRING((SELECT ', ' + CargaCargaIntegracao.CCA_PROTOCOLO
                            FROM T_CARGA_CARGA_INTEGRACAO CargaCargaIntegracao
                            LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCargaIntegracao.CAR_CODIGO
                            WHERE Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO AND CargaCargaIntegracao.CCA_PROTOCOLO IS NOT NULL AND CargaCargaIntegracao.CCA_PROTOCOLO <> ''
                            FOR XML PATH ('')
                        ), 3, 1000), ''
                    ) NumeroProtocoloIntegracaoCarga,
                    (select 
		                 TOP 1 CargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL
		                 FROM
                             t_carga_pedido CargaPedido
	                      WHERE
                            CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO) as TipoModalTransporte,
                    SUBSTRING((SELECT ', ' + Motorista.FUN_VERSAO_APP
                                FROM T_CARGA_MOTORISTA CargaMotorista
                                JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
                                ORDER BY Motorista.FUN_NOME
                                FOR XML PATH ('')), 3, 1000) VersaoAppMotorista,
                    ISNULL(CargaDadosSumarizados.CDS_NUMERO_PEDIDO_EMBARCADOR, ' ') NumeroPedidoEmbarcadorSumarizado,
                    SUBSTRING((SELECT DISTINCT ', ' + Container.CTR_NUMERO 
                                FROM T_COLETA_CONTAINER ColetaContainer 
                                LEFT JOIN T_CONTAINER Container ON Container.CTR_CODIGO = ColetaContainer.CTR_CODIGO 
                                WHERE ColetaContainer.CAR_CODIGO_ATUAL = Carga.CAR_CODIGO AND ColetaContainer.CTR_CODIGO IS NOT NULL
                                FOR XML PATH ('')), 3, 2000) NumeroContainer, 
                    (SELECT Max(isnull(CE.CEN_DATA_ENTREGA_REPROGRAMADA, CE.CEN_DATA_ENTREGA_PREVISTA)) DataMáximaFinal
                    FROM T_CARGA_ENTREGA AS CE
                    WHERE CE.CAR_CODIGO = Carga.CAR_CODIGO) PrevisaoFimViagem,
                    Veiculo.VEI_NUMERO_FROTA NumeroFrota,
                    Carga.CAR_DATA_PREVISAO_TERMINO_VIAGEM PrevisaoTerminoViagem,
                    Carga.CAR_DATA_PREVISAO_STOP_TRACKING PrevisaoStopTranking,
                    (select min(_pedido.PEP_DATA_PREVISAO_SAIDA_DESTINATARIO) from t_carga_entrega as _cargaEntrega
                    join T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido on _cargaEntregaPedido.CEN_CODIGO = _cargaEntrega.CEN_CODIGO
                    join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.CPE_CODIGO = _cargaEntregaPedido.CPE_CODIGO
                    join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                    where _cargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and _cargaEntrega.CEN_SITUACAO != 2) PrevisaoSaidaDestino
                    ,Ocorrencia.UltimaOcorrencia,

                    _ConfiguracaoTMS.TempoSemPosicaoParaVeiculoPerderSinal TempoSemPosicaoParaVeiculoPerderSinal,
                    _ConfiguracaoTMS.DataBaseCalculoPrevisaoControleEntrega DataBaseCalculoPrevisaoControleEntrega
                    ";

            if (filtrosPesquisa?.SomenteUltimoPorCarga ?? false)
                sql += ", ROW_NUMBER() OVER (PARTITION BY Monitoramento.CAR_CODIGO ORDER BY Carga.CAR_DATA_CRIACAO DESC) as RowNum";

            return sql;
        }

        private string GetSQLFromMonitoramento()
        {
            return $@"
                    from T_MONITORAMENTO as Monitoramento";
        }

        private string GetSQLSelectAlertaMonitor()
        {
            return $@"
                SELECT 
                    AlertaMonitor.ALE_TIPO,
                    AlertaMonitor.MEV_CODIGO,
                    AlertaMonitor.CAR_CODIGO ";
        }

        private string GetSQLFromAlertaMonitor()
        {
            return $@"
                    from T_ALERTA_MONITOR as AlertaMonitor
                    left join T_MONITORAMENTO Monitoramento on Monitoramento.CAR_CODIGO = AlertaMonitor.CAR_CODIGO";
        }

        private string GetFiltroMonitoramento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, List<ParametroSQL> parametros)
        {
            string filtro = "and carga.CAR_CARGA_FECHADA = 1 ";

            switch (configuracao.TelaMonitoramentoApresentarCargasQuando)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarMonitoramento:
                    filtro += $" and Monitoramento.MON_STATUS != 0 ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarViagem:
                    filtro += $" and Carga.CAR_CARGA_DE_PRE_CARGA = 0 ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoCriarMonitoramento:
                default:
                    break;
            }

            if (filtrosPesquisa.CodigosVeiculos?.Count > 0)
            {
                string codigosVeiculos = string.Join(", ", filtrosPesquisa.CodigosVeiculos);
                filtro += $@" 
                    AND (
                        Veiculo.VEI_CODIGO in ({codigosVeiculos}) 
                        OR exists (
                            SELECT 1
                            FROM T_CARGA_VEICULOS_VINCULADOS VeiculosVinculados9
                            JOIN T_VEICULO Veiculo9 on Veiculo9.VEI_CODIGO = VeiculosVinculados9.VEI_CODIGO
                            WHERE VeiculosVinculados9.CAR_CODIGO = Carga.CAR_CODIGO AND Veiculo9.VEI_CODIGO in ({codigosVeiculos})
			            )) ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    filtro += $"AND Carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE '%{filtrosPesquisa.CodigoCargaEmbarcador}%'";
                else
                    filtro += $" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'";
            }

            if (filtrosPesquisa.CodigosCarga?.Count > 0)
                filtro += $"AND Carga.CAR_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosCarga.ToArray())})";

            if (filtrosPesquisa.CodigoCargaEmbarcadorMulti != null && filtrosPesquisa.CodigoCargaEmbarcadorMulti.Count > 0)
                filtro += $" and Carga.CAR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoCargaEmbarcadorMulti)})";

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                filtro += $" AND Carga.GRP_CODIGO = '{filtrosPesquisa.CodigoGrupoPessoa}'";

            if (filtrosPesquisa.TipoAlerta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta)
            {
                string filtroCodigoEvento = filtrosPesquisa.CodigoEvento.HasValue ? $" and TMO.MEV_CODIGO = {filtrosPesquisa.CodigoEvento.Value} " : " ";
                filtro += $" AND EXISTS (SELECT 1 FROM T_ALERTA_MONITOR TMO WHERE TMO.CAR_CODIGO = Monitoramento.CAR_CODIGO AND TMO.ALE_STATUS = 0 AND TMO.ALE_TIPO = {Convert.ToInt32(filtrosPesquisa.TipoAlerta)} {filtroCodigoEvento} )"; // SQL-INJECTION-SAFE
            }


            if (filtrosPesquisa.Status?.Count > 0)
                filtro += $" AND Monitoramento.MON_STATUS in ({string.Join(", ", (from obj in filtrosPesquisa.Status select (int)obj).ToList())}) ";

            if (filtrosPesquisa.CodigosStatusViagem != null && filtrosPesquisa.CodigosStatusViagem.Count > 0)
            {
                filtro += " and (";
                if (filtrosPesquisa.CodigosStatusViagem.Contains(-1))
                    filtro += $" Monitoramento.MSV_CODIGO is null or ";
                filtro += $" Monitoramento.MSV_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosStatusViagem)}) )";
            }

            if (filtrosPesquisa.CodigosGrupoTipoOperacao != null && filtrosPesquisa.CodigosGrupoTipoOperacao.Count > 0)
                filtro += $" and ( GrupoTipoOperacao.GTO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoTipoOperacao)}) )";

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                filtro += $" and Empresa.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)}) ";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CRIACAO >= convert(datetime, '{filtrosPesquisa.DataInicial.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CRIACAO <= convert(datetime, '{filtrosPesquisa.DataFinal.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.SomenteRastreados)
                filtro += $" AND Posicao.POS_DATA_VEICULO > convert(datetime, '{_dataAtual.AddMinutes(-configuracao.TempoSemPosicaoParaVeiculoPerderSinal).ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.CodigosResponsavelVeiculo?.Count > 0)
                filtro += $" AND Veiculo.FUN_CODIGO_RESPONSAVEL in ({string.Join(", ", filtrosPesquisa.CodigosResponsavelVeiculo)}) ";

            if (filtrosPesquisa.DataInicioCarregamento != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CARREGAMENTO >= '{filtrosPesquisa.DataInicioCarregamento.ToString(_dateFormat)}'";

            if (filtrosPesquisa.DataFimCarregamento != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CARREGAMENTO <= '{filtrosPesquisa.DataFimCarregamento.ToString(_dateFormat)}'";

            if (filtrosPesquisa.InicioViagemPrevistaInicial != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA >= '{filtrosPesquisa.InicioViagemPrevistaInicial.ToString(_dateFormat)}'";

            if (filtrosPesquisa.InicioViagemPrevistaFinal != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA <= '{filtrosPesquisa.InicioViagemPrevistaFinal.ToString(_dateFormat)}'";

            if (filtrosPesquisa.CodigosFronteiraRotaFrete?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                    SELECT 1
					FROM T_CARGA_FRONTEIRA CargaFronteira 
                    join T_CLIENTE ClienteFronteira on ClienteFronteira.CLI_CGCCPF = CargaFronteira.CLI_CGCCPF
			        WHERE CargaFronteira.CAR_CODIGO = Monitoramento.CAR_CODIGO and CargaFronteira.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.CodigosFronteiraRotaFrete)})) ";
            }

            if (filtrosPesquisa.ModalTransporte > 0)
            {
                filtro += $@" AND EXISTS (
                     SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and CargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = {Convert.ToInt32(filtrosPesquisa.ModalTransporte)} ) ";
            }

            switch (filtrosPesquisa.FiltroCliente)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.EmAlvo:
                    filtro += $" AND Posicao.POS_EM_ALVO = 1 ";
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1 FROM T_POSICAO_ALVO PosicaoAlvo1
                            JOIN t_CLIENTE ClienteAlvo1 ON ClienteAlvo1.CLI_CGCCPF = PosicaoAlvo1.CLI_CGCCPF
                            WHERE PosicaoAlvo1.POS_CODIGO = Posicao.POS_CODIGO AND ClienteAlvo1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
		                ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_POSICAO_ALVO PosicaoAlvo2
                            WHERE PosicaoAlvo2.POS_CODIGO = Posicao.POS_CODIGO AND PosicaoAlvo2.CLI_CGCCPF = {filtrosPesquisa.Cliente}
                        ) ";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComColeta:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_COLETA = 1 AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_COLETA = 1 AND CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComEntrega:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND (CargaEntrega.CEN_COLETA is null OR CargaEntrega.CEN_COLETA = 0) AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND (CargaEntrega.CEN_COLETA is null OR CargaEntrega.CEN_COLETA = 0) AND CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComColetaOuEntrega:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND (EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) OR EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido
                            LEFT JOIN T_CLIENTE Cliente2 on Cliente2.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND Cliente2.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ))";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";

                    if (filtrosPesquisa.CodigoClienteDestino?.Count > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CLI_CODIGO_ENTREGA in ({string.Join(", ", filtrosPesquisa.CodigoClienteDestino)})
                        ) ";
                    break;
            }



            if (filtrosPesquisa.CodigosFilial != null && filtrosPesquisa.CodigosFilial.Count > 0)
            {
                if (filtrosPesquisa.CodigosFilial.Any(o => o == -1))
                {
                    filtro += $@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))";
                }
                else if (configuracaoMonitoramento?.TelaMonitoramentoFiltroFilialDaCarga ?? false)
                    filtro += $" and (Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))";
                else
                    filtro += $" and (Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) or Carga.FIL_CODIGO_DESTINO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))";
            }


            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido) || !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP) ||
                    filtrosPesquisa.NumeroNotaFiscal > 0 || filtrosPesquisa.CodigoFuncionarioVendedor > 0 || filtrosPesquisa.PossuiExpedidor.HasValue || filtrosPesquisa.PossuiRecebedor.HasValue ||
                    (filtrosPesquisa.CodigosExpedidores?.Count ?? 0) > 0 || (filtrosPesquisa.Recebedores?.Count ?? 0) > 0 || (filtrosPesquisa.Destinatario?.Count ?? 0) > 0 ||
                    filtrosPesquisa.DataEntregaPedidoInicio != DateTime.MinValue || filtrosPesquisa.DataEntregaPedidoFinal != DateTime.MinValue ||
                    filtrosPesquisa.CodigosFilialVenda?.Count > 0 || (filtrosPesquisa.Remetente?.Count ?? 0) > 0 ||
                    filtrosPesquisa.DataEmissaoNFeFim != DateTime.MinValue || filtrosPesquisa.DataEmissaoNFeInicio != DateTime.MinValue)
            {
                filtro += @" AND EXISTS (
                    SELECT 1
                    FROM t_carga_pedido CargaPedido
                    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                    LEFT JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                    LEFT JOIN t_xml_nota_fiscal NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO AND NotaFiscal.NF_ATIVA = 1
                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO";

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                {
                    filtro += $" and Pedido.PED_NUMERO_PEDIDO_EMBARCADOR like :PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR ";
                    parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR", $"%{filtrosPesquisa.NumeroPedido}%"));
                }

                if (filtrosPesquisa.NumeroNotaFiscal > 0)
                    filtro += $" and NotaFiscal.nf_numero = {filtrosPesquisa.NumeroNotaFiscal} ";

                if (filtrosPesquisa.CodigoFuncionarioVendedor > 0)
                    filtro += $" and Pedido.FUN_CODIGO_VENDEDOR = {filtrosPesquisa.CodigoFuncionarioVendedor} ";

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                {
                    filtro += $" and Pedido.PED_NUMERO_EXP like :PEDIDO_PED_NUMERO_EXP ";
                    parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_EXP", $"%{filtrosPesquisa.NumeroEXP}%"));
                }

                if (filtrosPesquisa.CodigosExpedidores.Count > 0)
                    filtro += $" and CargaPedido.CLI_CODIGO_EXPEDIDOR in ({string.Join(", ", filtrosPesquisa.CodigosExpedidores)}) ";

                if (filtrosPesquisa.Recebedores != null && filtrosPesquisa.Recebedores.Count > 0)
                    filtro += $" and CargaPedido.CLI_CODIGO_RECEBEDOR in ({string.Join(", ", filtrosPesquisa.Recebedores)}) ";

                if (filtrosPesquisa.DataEntregaPedidoInicio != DateTime.MinValue)
                    filtro += $" AND Pedido.PED_PREVISAO_ENTREGA >= convert(datetime, '{filtrosPesquisa.DataEntregaPedidoInicio.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataEntregaPedidoFinal != DateTime.MinValue)
                    filtro += $" AND Pedido.PED_PREVISAO_ENTREGA <= convert(datetime, '{filtrosPesquisa.DataEntregaPedidoFinal.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    filtro += $" and Pedido.FIL_CODIGO_VENDA in ({string.Join(", ", filtrosPesquisa.CodigosFilialVenda)}) ";

                if (filtrosPesquisa.Destinatario != null && filtrosPesquisa.Destinatario.Count > 0)
                    filtro += $" and Pedido.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.Destinatario)})";

                if (filtrosPesquisa.DataEmissaoNFeInicio != DateTime.MinValue)
                    filtro += $" AND NotaFiscal.NF_DATA_EMISSAO >= convert(datetime, '{filtrosPesquisa.DataEmissaoNFeInicio.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataEmissaoNFeFim != DateTime.MinValue)
                    filtro += $" AND NotaFiscal.NF_DATA_EMISSAO <= convert(datetime, '{filtrosPesquisa.DataEmissaoNFeFim.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataInicioCarregamento != DateTime.MinValue)
                    filtro += $" AND Carga.CAR_DATA_CARREGAMENTO >= convert(datetime, '{filtrosPesquisa.DataInicioCarregamento.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataFimCarregamento != DateTime.MinValue)
                    filtro += $" AND Carga.CAR_DATA_CARREGAMENTO <= convert(datetime, '{filtrosPesquisa.DataFimCarregamento.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.Remetente != null && filtrosPesquisa.Remetente.Count > 0)
                    filtro += $" AND Pedido.CLI_CODIGO_REMETENTE in ({string.Join(", ", filtrosPesquisa.Remetente)})";

                filtro += ")";
            }

            if (filtrosPesquisa.PossuiExpedidor.HasValue)
                if (filtrosPesquisa.PossuiExpedidor.Value)
                {
                    filtro += " and CargaDadosSumarizados.CDS_EXPEDIDORES <> '' ";
                }
                else
                {
                    filtro += " and CargaDadosSumarizados.CDS_EXPEDIDORES = '' ";
                }

            if (filtrosPesquisa.PossuiRecebedor.HasValue)
                if (filtrosPesquisa.PossuiRecebedor.Value)
                {
                    filtro += " and CargaDadosSumarizados.CDS_RECEBEDORES <> '' ";
                }
                else
                {
                    filtro += " and CargaDadosSumarizados.CDS_RECEBEDORES = '' ";
                }

            if (filtrosPesquisa.PrevisaoEntregaInicio != DateTime.MinValue || filtrosPesquisa.PrevisaoEntregaFinal != DateTime.MinValue)
            {
                filtro += @" AND EXISTS (
                    SELECT 1
                    FROM t_carga_entrega CargaEntrega
                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO";

                if (filtrosPesquisa.PrevisaoEntregaInicio != DateTime.MinValue)
                    filtro += $" AND CargaEntrega.CEN_COLETA = 0 AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= convert(datetime, '{filtrosPesquisa.PrevisaoEntregaInicio.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.PrevisaoEntregaFinal != DateTime.MinValue)
                    filtro += $" AND CargaEntrega.CEN_COLETA = 0 AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= convert(datetime, '{filtrosPesquisa.PrevisaoEntregaFinal.ToString(_dateFormat)}', 102) ";

                filtro += ")";
            }

            if (filtrosPesquisa.CodigosDestinos?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND ClienteEntrega.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosDestinos)})) ";
            }

            //if (filtrosPesquisa.EstadosDestino?.Count > 0)
            //{
            //    filtro += $@" AND EXISTS (
            //                SELECT 1
            //                FROM T_CARGA_ENTREGA CargaEntrega
            //                JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
            //                JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
            //                WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND Localidade.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosDestino)}')) ";
            //}

            if (filtrosPesquisa.EstadosDestino?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido 
						    JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND Localidade.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosDestino)}')) ";
            }

            if (filtrosPesquisa.CodigosPaisDestino?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND Localidade.PAI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosPaisDestino)})) ";
            }

            //COLOCAR PRA CIMA. EM APENAS UM EXISTS COM O CARGA_PEDIDO..
            if (filtrosPesquisa.CodigosOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND ((CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND Localidade1.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)})) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Localidade2.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)}))))";
            }

            if (filtrosPesquisa.EstadosOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND ((CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND Localidade1.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosOrigem)}')) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Localidade2.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosOrigem)}'))))";
            }

            if (filtrosPesquisa.CodigosPaisOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND ((CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND Localidade1.PAI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosPaisOrigem)})) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Localidade2.PAI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosPaisOrigem)}))))";
            }

            //ATE AQUI


            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                filtro += $" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ";

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                filtro += $" and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ";

            if (filtrosPesquisa.VeiculosComContratoDeFrete)
                filtro += $@" and exists (
                    select 
	                    1
                    from
	                    T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador
                    JOIN
	                    T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoFreteTransportadorVeiculo on ContratoFreteTransportadorVeiculo.CFT_CODIGO = ContratoFreteTransportador.CFT_CODIGO
                    where
	                    ContratoFreteTransportador.CFT_ATIVO = 1
	                    and ContratoFreteTransportador.CFT_SITUACAO in (1,2)
	                    and CURRENT_TIMESTAMP between ContratoFreteTransportador.CFT_DATA_INICIAL and ContratoFreteTransportador.CFT_DATA_FINAL
	                    and ContratoFreteTransportadorVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO
                    )";

            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido
                            JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = Pedido.CRE_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO AND Pedido.CRE_CODIGO in ('{string.Join("', '", filtrosPesquisa.CodigosCentroResultado)}'))";
            }

            if (filtrosPesquisa.ApenasMonitoramentosCriticos)
                filtro += " AND Monitoramento.MON_CRITICO = 1 ";

            if (filtrosPesquisa.CodigosTiposTrecho?.Count > 0)
                filtro += $" and TipoTrecho.TTR_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTiposTrecho)}) ";

            if (filtrosPesquisa.CodigoMotorista > 0)
                filtro += $" AND EXISTS (SELECT 0 FROM T_CARGA_MOTORISTA WHERE T_CARGA_MOTORISTA.CAR_CODIGO = Monitoramento.CAR_CODIGO AND CAR_MOTORISTA = {filtrosPesquisa.CodigoMotorista} ) "; // SQL-INJECTION-SAFE

            return filtro;
        }

        private string GetSQLJoins()
        {
            //nao faça vinculos com carga pedido ou pedido aqui. (tem q ser subselect )
            return $@"
                    join T_VEICULO as Veiculo on Monitoramento.VEI_CODIGO =  Veiculo.VEI_CODIGO
                    left join T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatus on MonitoramentoStatus.MSV_CODIGO = Monitoramento.MSV_CODIGO
                    left join T_CARGA as Carga on Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO
                    left join T_CARGA_DADOS_SUMARIZADOS as CargaDadosSumarizados on Carga.CDS_CODIGO  = CargaDadosSumarizados.CDS_CODIGO
                    left join T_TIPO_DE_CARGA as TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO
                    left join T_FAIXA_TEMPERATURA as FaixaTemperatura on FaixaTemperatura.FTE_CODIGO = COALESCE(Carga.FTE_CODIGO, TipoCarga.FTE_CODIGO)
                    left join T_POSICAO Posicao on Posicao.POS_CODIGO = Monitoramento.POS_ULTIMA_POSICAO
                    left join T_FILIAL as Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO
                    left join T_EMPRESA as Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO
                    left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                    left join T_GRUPO_TIPO_OPERACAO GrupoTipoOperacao on GrupoTipoOperacao.GTO_CODIGO = TipoOperacao.GTO_CODIGO 
                    left join T_FUNCIONARIO FuncionarioResponsavel on FuncionarioResponsavel.FUN_CODIGO = Veiculo.FUN_CODIGO_RESPONSAVEL  
                    left join T_RASTREADOR_TECNOLOGIA TecnologiaRastreador ON TecnologiaRastreador.TRA_CODIGO = Veiculo.TRA_CODIGO
                    left join T_TIPO_TRECHO TipoTrecho ON TipoTrecho.TTR_CODIGO = Carga.TTR_CODIGO
                    left join T_CLIENTE as Terceiro ON Terceiro.CLI_CGCCPF = Carga.CLI_CGCCPF_TERCEIRO
                    cross join (select top 1
					CEM_TEMPO_SEM_POSICAO_PARA_VEICULO_PERDER_SINAL TempoSemPosicaoParaVeiculoPerderSinal, 
					CEM_DATA_BASE_PARA_CALCULO_PREVISAO_CONTROLE_ENTREGA DataBaseCalculoPrevisaoControleEntrega
				    from T_CONFIGURACAO_EMBARCADOR) _ConfiguracaoTMS   
	                OUTER APPLY (SELECT TOP 1 subOCO.OCO_DESCRICAO + ' - (' + subOCO.OCO_DESCRICAO_PORTAL + ')' AS UltimaOcorrencia
				                 FROM T_CARGA_ENTREGA subCEN
				                 INNER JOIN T_OCORRENCIA_COLETA_ENTREGA subOCE ON subCEN.CEN_CODIGO = subOCE.CEN_CODIGO
				                 INNER JOIN T_OCORRENCIA subOCO ON subOCE.OCO_CODIGO = subOCO.OCO_CODIGO
				                 WHERE subCEN.CAR_CODIGO = Carga.CAR_CODIGO
				                 ORDER BY subOCE.OCE_DATA_OCORRENCIA DESC

		                ) AS Ocorrencia ";
        }

        private string GetFiltroMonitoramentoTorre(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<ParametroSQL> parametros)
        {
            string filtro = "";

            switch (configuracao.TelaMonitoramentoApresentarCargasQuando)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarMonitoramento:
                    filtro += $" and Monitoramento.MON_STATUS != 0 ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarViagem:
                    filtro += $" and Carga.CAR_CARGA_DE_PRE_CARGA = 0 ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoCriarMonitoramento:
                default:
                    break;
            }

            if (filtrosPesquisa.CodigosVeiculos?.Count > 0)
            {
                string codigosVeiculos = string.Join(", ", filtrosPesquisa.CodigosVeiculos);
                filtro += $@" 
                    AND (
                        Veiculo.VEI_CODIGO in ({codigosVeiculos}) 
                        OR exists (
                            SELECT 1
                            FROM T_CARGA_VEICULOS_VINCULADOS VeiculosVinculados9
                            JOIN T_VEICULO Veiculo9 on Veiculo9.VEI_CODIGO = VeiculosVinculados9.VEI_CODIGO
                            WHERE VeiculosVinculados9.CAR_CODIGO = Carga.CAR_CODIGO AND Veiculo9.VEI_CODIGO in ({codigosVeiculos})
			            )) ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    filtro += $"AND Carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE '%{filtrosPesquisa.CodigoCargaEmbarcador}%'";
                else
                    filtro += $" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'";
            }

            if (filtrosPesquisa.CodigosCarga?.Count > 0)
                filtro += $"AND Carga.CAR_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosCarga.ToArray())})";

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                filtro += $" AND Carga.GRP_CODIGO = '{filtrosPesquisa.CodigoGrupoPessoa}'";

            if (filtrosPesquisa.TipoAlerta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta)
                filtro += $" AND EXISTS (SELECT 1 FROM T_ALERTA_MONITOR TMO WHERE TMO.CAR_CODIGO = Monitoramento.CAR_CODIGO AND TMO.ALE_STATUS = 0 AND TMO.ALE_TIPO = {Convert.ToInt32(filtrosPesquisa.TipoAlerta)} )"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TipoAlertaCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.SemAlerta)
                filtro += $" AND EXISTS (SELECT 1 FROM T_CARGA_EVENTO TCE WHERE TCE.CAR_CODIGO = Monitoramento.CAR_CODIGO AND TCE.ALC_STATUS = 0 AND TCE.ALC_TIPO = {Convert.ToInt32(filtrosPesquisa.TipoAlertaCarga)} )"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Status != null && filtrosPesquisa.Status.Count > 0)
            {
                string Status = string.Join(", ", from status in filtrosPesquisa.Status select status.ToString("D"));

                filtro += $" AND Monitoramento.MON_STATUS in ({Status})";
            }

            if (filtrosPesquisa.CodigosStatusViagem != null && filtrosPesquisa.CodigosStatusViagem.Count > 0)
            {
                filtro += " and (";
                if (filtrosPesquisa.CodigosStatusViagem.Contains(-1))
                    filtro += $" Monitoramento.MSV_CODIGO is null or ";
                filtro += $" Monitoramento.MSV_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosStatusViagem)}) )";
            }

            if (filtrosPesquisa.CodigosGrupoTipoOperacao != null && filtrosPesquisa.CodigosGrupoTipoOperacao.Count > 0)
                filtro += $" and ( GrupoTipoOperacao.GTO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoTipoOperacao)}) )";

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                filtro += $" and Empresa.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)}) ";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CRIACAO >= convert(datetime, '{filtrosPesquisa.DataInicial.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CRIACAO <= convert(datetime, '{filtrosPesquisa.DataFinal.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.DataInicioCarregamento != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CARREGAMENTO >= convert(datetime, '{filtrosPesquisa.DataInicioCarregamento.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.DataFimCarregamento != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CARREGAMENTO <= convert(datetime, '{filtrosPesquisa.DataFimCarregamento.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.SomenteRastreados)
                filtro += $" AND Posicao.POS_DATA_VEICULO > convert(datetime, '{_dataAtual.AddMinutes(-configuracao.TempoSemPosicaoParaVeiculoPerderSinal).ToString(_dateFormat)}', 102) ";

            switch (filtrosPesquisa.FiltroCliente)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.EmAlvo:
                    filtro += $" AND Posicao.POS_EM_ALVO = 1 ";
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1 FROM T_POSICAO_ALVO PosicaoAlvo1
                            JOIN t_CLIENTE ClienteAlvo1 ON ClienteAlvo1.CLI_CGCCPF = PosicaoAlvo1.CLI_CGCCPF
                            WHERE PosicaoAlvo1.POS_CODIGO = Posicao.POS_CODIGO AND ClienteAlvo1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
		                ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_POSICAO_ALVO PosicaoAlvo2
                            WHERE PosicaoAlvo2.POS_CODIGO = Posicao.POS_CODIGO AND PosicaoAlvo2.CLI_CGCCPF = {filtrosPesquisa.Cliente}
                        ) ";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComColeta:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_COLETA = 1 AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_COLETA = 1 AND CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComEntrega:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND (CargaEntrega.CEN_COLETA is null OR CargaEntrega.CEN_COLETA = 0) AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND (CargaEntrega.CEN_COLETA is null OR CargaEntrega.CEN_COLETA = 0) AND CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComColetaOuEntrega:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND  CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";
                    break;
            }

            if (filtrosPesquisa.CodigosFilial != null && filtrosPesquisa.CodigosFilial.Count > 0)
                filtro += $" and (Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) or Carga.FIL_CODIGO_DESTINO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))";
            
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido) || !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP) || 
                filtrosPesquisa.NumeroNotaFiscal > 0 || filtrosPesquisa.CodigoFuncionarioVendedor > 0 || filtrosPesquisa.CodigosExpedidores?.Count > 0 ||
                filtrosPesquisa.DataEntregaPedidoInicio != DateTime.MinValue || filtrosPesquisa.DataEntregaPedidoFinal != DateTime.MinValue ||
                filtrosPesquisa.CodigosFilialVenda?.Count > 0)
            {
                filtro += @" AND EXISTS (
                    SELECT 1
                    FROM t_carga_pedido CargaPedido
                    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                    LEFT JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                    LEFT JOIN t_xml_nota_fiscal NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO AND NotaFiscal.NF_ATIVA = 1
                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO";

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                {
                    filtro += $" and Pedido.PED_NUMERO_PEDIDO_EMBARCADOR like :PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR ";
                    parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR", $"%{filtrosPesquisa.NumeroPedido}%"));
                }

                if (filtrosPesquisa.NumeroNotaFiscal > 0)
                    filtro += $" and NotaFiscal.nf_numero = {filtrosPesquisa.NumeroNotaFiscal} ";

                if (filtrosPesquisa.CodigoFuncionarioVendedor > 0)
                    filtro += $" and Pedido.FUN_CODIGO_VENDEDOR = {filtrosPesquisa.CodigoFuncionarioVendedor} ";

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                {
                    filtro += $" and Pedido.PED_NUMERO_EXP like :PEDIDO_PED_NUMERO_EXP ";
                    parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_EXP", $"%{filtrosPesquisa.NumeroEXP}%"));

                }

                if (filtrosPesquisa.CodigosExpedidores.Count > 0)
                    filtro += $" and Pedido.CLI_CODIGO_EXPEDIDOR in ({string.Join(", ", filtrosPesquisa.CodigosExpedidores)}) ";

                if (filtrosPesquisa.DataEntregaPedidoInicio != DateTime.MinValue)
                    filtro += $" AND Pedido.PED_PREVISAO_ENTREGA >= convert(datetime, '{filtrosPesquisa.DataEntregaPedidoInicio.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataEntregaPedidoFinal != DateTime.MinValue)
                    filtro += $" AND Pedido.PED_PREVISAO_ENTREGA <= convert(datetime, '{filtrosPesquisa.DataEntregaPedidoFinal.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    filtro += $" and Pedido.FIL_CODIGO_VENDA in ({string.Join(", ", filtrosPesquisa.CodigosFilialVenda)}) ";

                filtro += ")";
            }

            if (filtrosPesquisa.PrevisaoEntregaInicio != DateTime.MinValue || filtrosPesquisa.PrevisaoEntregaFinal != DateTime.MinValue)
            {
                filtro += @" AND EXISTS (
                    SELECT 1
                    FROM t_carga_entrega CargaEntrega
                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO";

                if (filtrosPesquisa.PrevisaoEntregaInicio != DateTime.MinValue)
                    filtro += $" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= convert(datetime, '{filtrosPesquisa.PrevisaoEntregaInicio.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.PrevisaoEntregaFinal != DateTime.MinValue)
                    filtro += $" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= convert(datetime, '{filtrosPesquisa.PrevisaoEntregaFinal.ToString(_dateFormat)}', 102) ";

                filtro += ")";
            }

            if (filtrosPesquisa.CodigoClienteDestino?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido
                            join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (1, 4) AND CargaPedido.CLI_CODIGO_RECEBEDOR in ({string.Join(", ", filtrosPesquisa.CodigoClienteDestino)})) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (1, 4) AND Pedido.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoClienteDestino)})))";
            }

            if (filtrosPesquisa.CodigosDestinos?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND ClienteEntrega.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosDestinos)})) ";
            }

            if (filtrosPesquisa.EstadosDestino?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND Localidade.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosDestino)}')) ";
            }

            if (filtrosPesquisa.CodigoClienteOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido
                            join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND CargaPedido.CLI_CODIGO_EXPEDIDOR in ({string.Join(", ", filtrosPesquisa.CodigoClienteOrigem)})) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Pedido.CLI_CODIGO_REMETENTE in ({string.Join(", ", filtrosPesquisa.CodigoClienteOrigem)})))";
            }

            if (filtrosPesquisa.CodigosOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND Localidade1.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)})) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Localidade2.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)})))";
            }

            if (filtrosPesquisa.EstadosOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND Localidade1.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosOrigem)}')) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Localidade2.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosOrigem)}')))";
            }

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                filtro += $" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ";

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                filtro += $" and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ";

            if (filtrosPesquisa.VeiculosComContratoDeFrete)
                filtro += $@" and exists (
                    select 
	                    1
                    from
	                    T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador
                    JOIN
	                    T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoFreteTransportadorVeiculo on ContratoFreteTransportadorVeiculo.CFT_CODIGO = ContratoFreteTransportador.CFT_CODIGO
                    where
	                    ContratoFreteTransportador.CFT_ATIVO = 1
	                    and ContratoFreteTransportador.CFT_SITUACAO in (1,2)
	                    and CURRENT_TIMESTAMP between ContratoFreteTransportador.CFT_DATA_INICIAL and ContratoFreteTransportador.CFT_DATA_FINAL
	                    and ContratoFreteTransportadorVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO
                    )";

            return filtro;
        }

        private string GetSQLSelectTorreMonitoramento(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoesAprovadasouEmAprovacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaHelper.ObterSituacoesAprovadasOuEmAprovacao();
            string joinSituacaoOcorrencias = string.Join(",", from o in situacoesAprovadasouEmAprovacao select (int)o);


            return $@"
                SELECT 
                    Monitoramento.MON_CODIGO Codigo,
                    isNull(Monitoramento.MON_DATA_INICIO, Monitoramento.MON_DATA_CRIACAO) DataInicioMonitoramento,
                    Monitoramento.MON_DATA_FIM DataFimMonitoramento,
                    Monitoramento.MON_STATUS Status,
                    Carga.CAR_CODIGO Carga,
                    Veiculo.VEI_CODIGO Veiculo,
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR CargaEmbarcador,
                    Carga.CAR_DATA_CRIACAO DataCriacaoCarga,
                    Carga.CAR_DATA_TERMINO_CARGA DataPrevisaoTerminoCarga,
                    Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem,
                    Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA DataInicioViagemPrevista,
                    Carga.CAR_DATA_REAGENDAMENTO DataReagendamento,
                    Monitoramento.MON_DISTANCIA_ATE_DESTINO DistanciaAteDestino,
                    Monitoramento.MON_DISTANCIA_ATE_ORIGEM DistanciaAteOrigem,
                    Carga.CAR_DATA_CARREGAMENTO DataCarregamentoCarga,
                    Carga.CAR_VALOR_FRETE ValorFrete,
                    (SELECT top 1 JanelaCarregamento.CJC_INICIO_CARREGAMENTO FROM T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento WHERE JanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO and JanelaCarregamento.CEC_CODIGO is not null) DataInicioCarregamentoJanela,
                    Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM DataPrevisaoChegadaPlanta,
                    Empresa.EMP_RAZAO Transportador,
                    Posicao.POS_DESCRICAO Posicao,
                    Posicao.POS_ID_EQUIPAMENTO IDEquipamento,
                    Filial.FIL_DESCRICAO Filial,
                    PosicaO.POS_DATA_VEICULO DataPosicaoAtual,
                    Veiculo.VEI_PLACA as Tracao,
                    TipoCarga.TCG_DESCRICAO as TipoCarga,
                    (SELECT count(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO) FROM T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento
                            left join T_CENTRO_CARREGAMENTO_LIMITE_CARREGAMENTO LimiteCarregamento on LimiteCarregamento.CEC_CODIGO = CargaJanelaCarregamento.CEC_CODIGO and DATEPART(WEEKDAY, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO) = LimiteCarregamento.CLC_DIA and Carga.TCG_CODIGO = LimiteCarregamento.TCG_CODIGO 
                            WHERE CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO and CargaJanelaCarregamento.CEC_CODIGO is not null and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO is not null and Carga.CAR_DATA_CRIACAO < CAST(DATEADD(dd, - LimiteCarregamento.CLC_DIAS_ANTECEDENCIA, CONVERT(date, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO)) AS DATETIME) + CAST(LimiteCarregamento.CLC_HORA_LIMITE AS DATETIME)
                     ) AntecedenciaGrade,
                    (select sum(cargaOcorrencia.COC_VALOR_OCORRENCIA) from t_carga_ocorrencia cargaOcorrencia 
                        inner join T_OCORRENCIA Ocorrencia on cargaOcorrencia.OCO_CODIGO = Ocorrencia.OCO_CODIGO where cargaOcorrencia.CAR_CODIGO = Carga.CAR_CODIGO and cargaOcorrencia.COC_SITUACAO_OCORRENCIA in ({joinSituacaoOcorrencias}) and Ocorrencia.OCO_TIPO_CLASSIFICACAO_OCORRENCIA = 1) OcorrenciasDeslocamento,
                    (select sum(cargaOcorrencia.COC_VALOR_OCORRENCIA) from t_carga_ocorrencia cargaOcorrencia 
                        inner join T_OCORRENCIA Ocorrencia on cargaOcorrencia.OCO_CODIGO = Ocorrencia.OCO_CODIGO where cargaOcorrencia.CAR_CODIGO = Carga.CAR_CODIGO and cargaOcorrencia.COC_SITUACAO_OCORRENCIA in ({joinSituacaoOcorrencias}) and Ocorrencia.OCO_TIPO_CLASSIFICACAO_OCORRENCIA = 2) OcorrenciasFreteNegociado,
                    (select sum(cargaOcorrencia.COC_VALOR_OCORRENCIA) from t_carga_ocorrencia cargaOcorrencia 
                        inner join T_OCORRENCIA Ocorrencia on cargaOcorrencia.OCO_CODIGO = Ocorrencia.OCO_CODIGO where cargaOcorrencia.CAR_CODIGO = Carga.CAR_CODIGO and cargaOcorrencia.COC_SITUACAO_OCORRENCIA in ({joinSituacaoOcorrencias}) and Ocorrencia.OCO_TIPO_CLASSIFICACAO_OCORRENCIA IS NULL) OutrasOcorrencias,
                    (select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 13 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoDescarga,
                    SUBSTRING((SELECT ', ' + Veiculo1.vei_placa AS [text()]  
		                        FROM T_CARGA_VEICULOS_VINCULADOS VeiculosVinculados
		                        JOIN T_VEICULO VEICULO1 on Veiculo1.VEI_CODIGO = VeiculosVinculados.VEI_CODIGO
		                        WHERE VeiculosVinculados.CAR_CODIGO = Carga.CAR_CODIGO
		                        FOR XML PATH ('')), 3, 1000) Reboques,
                    SUBSTRING((SELECT ', ' {((configuracao.ApresentarCodigoIntegracaoComNomeFantasiaCliente) ? "+ CASE isnull(ClienteEntrega.CLI_CODIGO_INTEGRACAO, '') WHEN '' THEN '' ELSE ClienteEntrega.CLI_CODIGO_INTEGRACAO + '-' END " : "")}
                                + CASE isnull(ClienteEntrega.CLI_NOMEFANTASIA, '') WHEN '' THEN ClienteEntrega.CLI_NOME ELSE ClienteEntrega.CLI_NOMEFANTASIA END 
                                + ' (' + Localidade.LOC_DESCRICAO + '/' + Localidade.UF_SIGLA + ')' AS [text()]             
		                        FROM T_CARGA_ENTREGA CargaEntrega
		                        JOIN t_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
		                        JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
		                        WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
                                ORDER BY CargaEntrega.CEN_ORDEM
		                        FOR XML PATH , TYPE).value(N'.[1]', N'nvarchar(max)'), 3, 2000) Destinos,
                    SUBSTRING((SELECT ', ' + Localidade.LOC_DESCRICAO + '/' + Localidade.UF_SIGLA AS [text()]             
		                        FROM T_CARGA_ENTREGA CargaEntrega
		                        JOIN t_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
		                        JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
		                        WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and CargaEntrega.CEN_COLETA = 0
                                ORDER BY CargaEntrega.CEN_ORDEM
		                        FOR XML PATH , TYPE).value(N'.[1]', N'nvarchar(max)'), 3, 2000) CidadeDestino,
                    SUBSTRING((SELECT', ' + rtrim(ltrim(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR)) AS [text()]      
			                                FROM t_carga_pedido CargaPedido 
			                                JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
			                                WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO
			                                ORDER BY Pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                                            FOR XML PATH ('')), 3,2000) Pedidos,
                    SUBSTRING((SELECT ', ' + rtrim(ltrim(Ordem.OEM_NUMERO)) AS [text()]                                           
                            FROM T_ORDEM_EMBARQUE Ordem                                       
                            JOIN t_carga Carga ON Carga.CAR_CODIGO = Ordem.CAR_CODIGO                                       
                            WHERE Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO                                     
                            ORDER BY Ordem.OEM_NUMERO
                            FOR XML PATH ('')), 3, 2000) Ordens,
                    (
                        SELECT min(CargaEntrega.CEN_DATA_SAIDA_RAIO) 
                        FROM T_CARGA_ENTREGA CargaEntrega 
                        WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO 
                        AND CargaEntrega.CEN_DATA_SAIDA_RAIO is not null 
                        AND CargaEntrega.CEN_COLETA = 1
                    ) DataSaidaOrigem,
                    (
                        SELECT min(CargaEntrega.CEN_DATA_ENTRADA_RAIO) 
                        FROM T_CARGA_ENTREGA CargaEntrega 
                        WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO 
                        AND CargaEntrega.CEN_DATA_SAIDA_RAIO is not null 
                        AND CargaEntrega.CEN_COLETA = 1
                    ) DataEntradaOrigem";
        }

        private string GetSQLJoinsTorre()
        {
            return $@"
                    join T_VEICULO as Veiculo on Monitoramento.VEI_CODIGO =  Veiculo.VEI_CODIGO
                    left join T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatus on MonitoramentoStatus.MSV_CODIGO = Monitoramento.MSV_CODIGO
                    left join T_CARGA as Carga on Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO
                    left join T_CARGA_DADOS_SUMARIZADOS as CargaDadosSumarizados on Carga.CDS_CODIGO  = CargaDadosSumarizados.CDS_CODIGO
                    left join T_TIPO_DE_CARGA as TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO
                    left join T_POSICAO Posicao on Posicao.POS_CODIGO = Monitoramento.POS_ULTIMA_POSICAO
                    left join T_FILIAL as Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO
                    left join T_EMPRESA as Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO
                    left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                    left join T_GRUPO_TIPO_OPERACAO GrupoTipoOperacao on GrupoTipoOperacao.GTO_CODIGO = TipoOperacao.GTO_CODIGO";

        }

        #endregion

        #region Métodos Privados - View
        private string GetSQLFromView()
        {
            return $@"
                    from VIEW_MONITORAMENTO as ViewMonitoramento
                    ";
        }

        private string GetSQLFiltrosView(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, List<ParametroSQL> parametros)
        {
            string filtroInterno = GetFiltroInternoView(filtrosPesquisa, parametros);

            string filtro = !string.IsNullOrWhiteSpace(filtroInterno) ? $@" 
            from T_MONITORAMENTO Monitoramento {filtroInterno} " : "";

            return filtro;
        }

        private string GetFiltroInternoView(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, List<ParametroSQL> parametros)
        {
            string filtro = @"where Carga.CAR_CARGA_FECHADA = 1 
                              ";
            string joins = "";

            AdicionarJoinInternoCarga(ref joins);

            switch (filtrosPesquisa.TelaMonitoramentoApresentarCargasQuando)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarMonitoramento:
                    filtro += $@" and Monitoramento.MON_STATUS != 0 
                                ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarViagem:
                    filtro += $@" and Carga.CAR_CARGA_DE_PRE_CARGA = 0 
                                ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoCriarMonitoramento:
                default:
                    break;
            }

            if (filtrosPesquisa.CodigosVeiculos?.Count > 0)
            {
                AdicionarJoinInternoVeiculo(ref joins);
                string codigosVeiculos = string.Join(", ", filtrosPesquisa.CodigosVeiculos);
                filtro += $@" 
                    AND (
                        Veiculo.VEI_CODIGO in ({codigosVeiculos}) 
                        OR exists (
                            SELECT 1
                            FROM T_CARGA_VEICULOS_VINCULADOS VeiculosVinculados9
                            JOIN T_VEICULO Veiculo9 on Veiculo9.VEI_CODIGO = VeiculosVinculados9.VEI_CODIGO
                            WHERE VeiculosVinculados9.CAR_CODIGO = Carga.CAR_CODIGO AND Veiculo9.VEI_CODIGO in ({codigosVeiculos})
			            )) 
                    ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    filtro += $"AND Carga.CAR_CODIGO_CARGA_EMBARCADOR LIKE '%{filtrosPesquisa.CodigoCargaEmbarcador}%'";
                else
                    filtro += $" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'";
            }

            if (filtrosPesquisa.Parqueada.HasValue)
            {
                if (filtrosPesquisa.Parqueada.Value)
                {
                    filtro += $"AND Carga.CAR_PARQUEADA = 1";
                }
                else
                {
                    filtro += "AND Carga.CAR_PARQUEADA = 0";
                }
            }

            if (filtrosPesquisa.CodigosCarga?.Count > 0)
                filtro += $"AND Carga.CAR_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosCarga.ToArray())})";

            if (filtrosPesquisa.SituacaoCarga != 0)
                filtro += $"AND Carga.CAR_SITUACAO = {filtrosPesquisa.SituacaoCarga}";

            if (filtrosPesquisa.CodigoCargaEmbarcadorMulti != null && filtrosPesquisa.CodigoCargaEmbarcadorMulti.Count > 0)
                filtro += $" and Carga.CAR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoCargaEmbarcadorMulti)})";

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                filtro += $" AND Carga.GRP_CODIGO = '{filtrosPesquisa.CodigoGrupoPessoa}'";

            if (filtrosPesquisa.TipoAlerta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta)
                filtro += $" AND EXISTS (SELECT 1 FROM T_ALERTA_MONITOR TMO WHERE TMO.CAR_CODIGO = Monitoramento.CAR_CODIGO AND TMO.ALE_STATUS = 0 AND TMO.ALE_TIPO = {Convert.ToInt32(filtrosPesquisa.TipoAlerta)} )"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Status?.Count > 0)
                filtro += $" AND Monitoramento.MON_STATUS in ({string.Join(", ", (from obj in filtrosPesquisa.Status select (int)obj).ToList())}) ";


            if (filtrosPesquisa.RastreadorOnlineOffline != null)
            {
                AdicionarJoinInternoPosicao(ref joins);
                filtro += $@" AND (CASE 
                                        WHEN Posicao.POS_DATA_VEICULO IS NOT NULL 
                                                AND DATEDIFF(MINUTE, Posicao.POS_DATA_VEICULO, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}') <= {filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal}
                                        THEN 1
                                 ELSE 0 END) = {filtrosPesquisa.RastreadorOnlineOffline.GetHashCode()}";
            }


            if (filtrosPesquisa.CodigosStatusViagem != null && filtrosPesquisa.CodigosStatusViagem.Count > 0)
            {
                filtro += " and (";
                if (filtrosPesquisa.CodigosStatusViagem.Contains(-1))
                    filtro += $" Monitoramento.MSV_CODIGO is null or ";
                filtro += $" Monitoramento.MSV_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosStatusViagem)}) )";
            }

            if (filtrosPesquisa.MonitoramentoStatusViagemTipoRegra != 0)
            {

                if (filtrosPesquisa.MonitoramentoStatusViagemTipoRegra == -1)
                {
                    filtro += " AND Monitoramento.MSV_CODIGO IS NULL";
                }
                else
                {
                    filtro += $@" AND EXISTS (
                    SELECT 1
                    FROM T_MONITORAMENTO_STATUS_VIAGEM StatusViagemMonitoramento
                    WHERE
                        StatusViagemMonitoramento.MSV_TIPO_REGRA = {filtrosPesquisa.MonitoramentoStatusViagemTipoRegra}
                        AND StatusViagemMonitoramento.MSV_CODIGO = Monitoramento.MSV_CODIGO)";
                }
            }

            if (filtrosPesquisa.GrupoStatusViagem > 0)
            {
                filtro += $@" AND EXISTS (
                                  SELECT 1
                                  FROM T_MONITORAMENTO_STATUS_VIAGEM StatusViagemMonitoramento
                                  JOIN T_MONITORAMENTO_GRUPO_STATUS_VIAGEM GrupoStatusViagemMonitoramento
                                      ON GrupoStatusViagemMonitoramento.MGV_CODIGO = StatusViagemMonitoramento.MGV_CODIGO
                                  WHERE
                                      GrupoStatusViagemMonitoramento.MGV_CODIGO = {filtrosPesquisa.GrupoStatusViagem}
                                      AND StatusViagemMonitoramento.MSV_CODIGO = Monitoramento.MSV_CODIGO)";
            }

            if (filtrosPesquisa.CodigosGrupoTipoOperacao != null && filtrosPesquisa.CodigosGrupoTipoOperacao.Count > 0)
            {
                if (filtrosPesquisa.CodigosGrupoTipoOperacao.Contains(-1))
                {
                    AdicionarJoinInternoGrupoTipoOperacao(ref joins);
                    filtro += $"  and ((GrupoTipoOperacao.GTO_CODIGO IS NULL) OR ( GrupoTipoOperacao.GTO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoTipoOperacao)})))";
                }
                else
                {
                    AdicionarJoinInternoGrupoTipoOperacao(ref joins);
                    filtro += $" and ( GrupoTipoOperacao.GTO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoTipoOperacao)}) )";
                }
            }

            if (filtrosPesquisa.GrupoTipoOperacaoIndicador != null)
            {
                if (filtrosPesquisa?.GrupoTipoOperacaoIndicador > 0)
                {
                    AdicionarJoinInternoGrupoTipoOperacao(ref joins);
                    filtro += $" and ( GrupoTipoOperacao.GTO_CODIGO = {filtrosPesquisa.GrupoTipoOperacaoIndicador})";
                }
                else
                {
                    AdicionarJoinInternoGrupoTipoOperacao(ref joins);
                    filtro += $" and ( GrupoTipoOperacao.GTO_CODIGO IS NULL)";
                }
            }


            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
            {
                AdicionarJoinInternoEmpresa(ref joins);
                filtro += $" and Empresa.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)}) ";
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CRIACAO >= convert(datetime, '{filtrosPesquisa.DataInicial.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CRIACAO <= convert(datetime, '{filtrosPesquisa.DataFinal.ToString(_dateFormat)}', 102) ";

            if (filtrosPesquisa.SomenteRastreados)
            {
                AdicionarJoinInternoPosicao(ref joins);
                filtro += $" AND Posicao.POS_DATA_VEICULO > convert(datetime, '{_dataAtual.AddMinutes(-filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal).ToString(_dateFormat)}', 102) ";
            }

            if (filtrosPesquisa.CodigosResponsavelVeiculo?.Count > 0)
            {
                AdicionarJoinInternoPosicao(ref joins);
                AdicionarJoinInternoVeiculo(ref joins);

                filtro += $" AND Veiculo.FUN_CODIGO_RESPONSAVEL in ({string.Join(", ", filtrosPesquisa.CodigosResponsavelVeiculo)}) ";
            }

            if (filtrosPesquisa.DataInicioCarregamento != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CARREGAMENTO >= '{filtrosPesquisa.DataInicioCarregamento.ToString(_dateFormat)}'";

            if (filtrosPesquisa.DataFimCarregamento != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_CARREGAMENTO <= '{filtrosPesquisa.DataFimCarregamento.ToString(_dateFormat)}'";

            if (filtrosPesquisa.InicioViagemPrevistaInicial != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA >= '{filtrosPesquisa.InicioViagemPrevistaInicial.ToString(_dateFormat)}'";

            if (filtrosPesquisa.InicioViagemPrevistaFinal != DateTime.MinValue)
                filtro += $" AND Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA <= '{filtrosPesquisa.InicioViagemPrevistaFinal.ToString(_dateFormat)}'";

            if (filtrosPesquisa.DataInicioMonitoramento != DateTime.MinValue)
                filtro += $" AND Monitoramento.MON_DATA_CRIACAO >= '{filtrosPesquisa.DataInicioMonitoramento.ToString(_dateFormat)}'";

            if (filtrosPesquisa.DataFimMonitoramento != DateTime.MinValue)
                filtro += $" AND Monitoramento.MON_DATA_CRIACAO <= '{filtrosPesquisa.DataFimMonitoramento.ToString(_dateFormat)}'";

            if (filtrosPesquisa.CodigosFronteiraRotaFrete?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                    SELECT 1
					FROM T_CARGA_FRONTEIRA CargaFronteira 
                    join T_CLIENTE ClienteFronteira on ClienteFronteira.CLI_CGCCPF = CargaFronteira.CLI_CGCCPF
			        WHERE CargaFronteira.CAR_CODIGO = Monitoramento.CAR_CODIGO and CargaFronteira.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.CodigosFronteiraRotaFrete)})) ";
            }

            switch (filtrosPesquisa.FiltroCliente)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.EmAlvo:
                    AdicionarJoinInternoPosicao(ref joins);
                    filtro += $" AND Posicao.POS_EM_ALVO = 1 ";
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1 FROM T_POSICAO_ALVO PosicaoAlvo1
                            JOIN t_CLIENTE ClienteAlvo1 ON ClienteAlvo1.CLI_CGCCPF = PosicaoAlvo1.CLI_CGCCPF
                            WHERE PosicaoAlvo1.POS_CODIGO = Posicao.POS_CODIGO AND ClienteAlvo1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
		                ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_POSICAO_ALVO PosicaoAlvo2
                            WHERE PosicaoAlvo2.POS_CODIGO = Posicao.POS_CODIGO AND PosicaoAlvo2.CLI_CGCCPF = {filtrosPesquisa.Cliente}
                        ) ";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComColeta:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_COLETA = 1 AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_COLETA = 1 AND CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComEntrega:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND (CargaEntrega.CEN_COLETA is null OR CargaEntrega.CEN_COLETA = 0) AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) ";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND (CargaEntrega.CEN_COLETA is null OR CargaEntrega.CEN_COLETA = 0) AND CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComColetaOuEntrega:
                    if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                        filtro += $@" AND (EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN t_CLIENTE Cliente1 ON Cliente1.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND Cliente1.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ) OR EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido
                            LEFT JOIN T_CLIENTE Cliente2 on Cliente2.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND Cliente2.CTP_CODIGO = {filtrosPesquisa.CodigoCategoriaPessoa}
                        ))";
                    if (filtrosPesquisa.Cliente > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.Cliente}
                        ) ";

                    if (filtrosPesquisa.CodigoClienteDestino?.Count > 0)
                        filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_entrega CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CLI_CODIGO_ENTREGA in ({string.Join(", ", filtrosPesquisa.CodigoClienteDestino)})
                        ) ";
                    break;
            }



            if (filtrosPesquisa.CodigosFilial != null && filtrosPesquisa.CodigosFilial.Count > 0)
            {
                AdicionarJoinInternoFilial(ref joins);
                if (filtrosPesquisa.CodigosFilial.Any(o => o == -1))
                {
                    filtro += $@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))";
                }
                else if (filtrosPesquisa?.TelaMonitoramentoFiltroFilialDaCarga ?? false)
                    filtro += $" and (Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))";
                else
                    filtro += $" and (Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) or Carga.FIL_CODIGO_DESTINO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))";
            }

            
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido) || !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP) ||
                    filtrosPesquisa.NumeroNotaFiscal > 0 || filtrosPesquisa.Vendedor.Count > 0 || filtrosPesquisa.Supervisor.Count > 0 || filtrosPesquisa.PossuiExpedidor.HasValue || filtrosPesquisa.PossuiRecebedor.HasValue ||
                    (filtrosPesquisa.CodigosExpedidores?.Count ?? 0) > 0 || (filtrosPesquisa.Recebedores?.Count ?? 0) > 0 || (filtrosPesquisa.Destinatario?.Count ?? 0) > 0 ||
                    filtrosPesquisa.DataEntregaPedidoInicio != DateTime.MinValue || filtrosPesquisa.DataEntregaPedidoFinal != DateTime.MinValue ||
                    filtrosPesquisa.CodigosFilialVenda?.Count > 0 || (filtrosPesquisa.Remetente?.Count ?? 0) > 0 ||
                    filtrosPesquisa.DataEmissaoNFeFim != DateTime.MinValue || filtrosPesquisa.DataEmissaoNFeInicio != DateTime.MinValue ||
                    filtrosPesquisa.DataAgendamentoPedidoInicial != DateTime.MinValue || filtrosPesquisa.DataAgendamentoPedidoFinal != DateTime.MinValue ||
                    filtrosPesquisa.DataColetaPedidoInicial != DateTime.MinValue || filtrosPesquisa.DataColetaPedidoFinal != DateTime.MinValue ||
                    !string.IsNullOrWhiteSpace(filtrosPesquisa.EscritorioVenda) ||
                    !string.IsNullOrWhiteSpace(filtrosPesquisa.Matriz) ||
                    !string.IsNullOrWhiteSpace(filtrosPesquisa.EquipeVendas) ||
                    !string.IsNullOrWhiteSpace(filtrosPesquisa.TipoMercadoria) ||
                    !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente) || filtrosPesquisa.CodigosClienteComplementar?.Count > 0 || filtrosPesquisa.CanalVenda > 0)
            {
                filtro += @" AND EXISTS (
                    SELECT 1
                    FROM t_carga_pedido CargaPedido
                    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                    LEFT JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                    LEFT JOIN t_xml_nota_fiscal NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO AND NotaFiscal.NF_ATIVA = 1
                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO";

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                {
                    filtro += $" and Pedido.PED_NUMERO_PEDIDO_EMBARCADOR like :PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR ";
                    parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR", $"%{filtrosPesquisa.NumeroPedido}%"));
                }

                if (filtrosPesquisa.NumeroNotaFiscal > 0)
                    filtro += $" and NotaFiscal.nf_numero = {filtrosPesquisa.NumeroNotaFiscal} ";

                if (filtrosPesquisa.CodigoFuncionarioVendedor > 0)
                    filtro += $" and Pedido.FUN_CODIGO_VENDEDOR = {filtrosPesquisa.CodigoFuncionarioVendedor} ";

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                {
                    filtro += $" and Pedido.PED_NUMERO_EXP like PEDIDO_PED_NUMERO_EXP ";
                    parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_EXP", $"%{filtrosPesquisa.NumeroEXP}%"));

                }

                if (filtrosPesquisa.CodigosExpedidores.Count > 0)
                    filtro += $" and CargaPedido.CLI_CODIGO_EXPEDIDOR in ({string.Join(", ", filtrosPesquisa.CodigosExpedidores)}) ";

                if (filtrosPesquisa.Recebedores != null && filtrosPesquisa.Recebedores.Count > 0)
                    filtro += $" and CargaPedido.CLI_CODIGO_RECEBEDOR in ({string.Join(", ", filtrosPesquisa.Recebedores)}) ";

                if (filtrosPesquisa.DataEntregaPedidoInicio != DateTime.MinValue)
                    filtro += $" AND Pedido.PED_PREVISAO_ENTREGA >= convert(datetime, '{filtrosPesquisa.DataEntregaPedidoInicio.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataEntregaPedidoFinal != DateTime.MinValue)
                    filtro += $" AND Pedido.PED_PREVISAO_ENTREGA <= convert(datetime, '{filtrosPesquisa.DataEntregaPedidoFinal.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    filtro += $" and Pedido.FIL_CODIGO_VENDA in ({string.Join(", ", filtrosPesquisa.CodigosFilialVenda)}) ";

                if (filtrosPesquisa.Destinatario != null && filtrosPesquisa.Destinatario.Count > 0)
                    filtro += $" and Pedido.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.Destinatario)})";

                if (filtrosPesquisa.DataEmissaoNFeInicio != DateTime.MinValue)
                    filtro += $" AND NotaFiscal.NF_DATA_EMISSAO >= convert(datetime, '{filtrosPesquisa.DataEmissaoNFeInicio.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataEmissaoNFeFim != DateTime.MinValue)
                    filtro += $" AND NotaFiscal.NF_DATA_EMISSAO <= convert(datetime, '{filtrosPesquisa.DataEmissaoNFeFim.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataInicioCarregamento != DateTime.MinValue)
                    filtro += $" AND Carga.CAR_DATA_CARREGAMENTO >= convert(datetime, '{filtrosPesquisa.DataInicioCarregamento.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.DataFimCarregamento != DateTime.MinValue)
                    filtro += $" AND Carga.CAR_DATA_CARREGAMENTO <= convert(datetime, '{filtrosPesquisa.DataFimCarregamento.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.Remetente != null && filtrosPesquisa.Remetente.Count > 0)
                    filtro += $" AND Pedido.CLI_CODIGO_REMETENTE in ({string.Join(", ", filtrosPesquisa.Remetente)})";

                if (filtrosPesquisa.DataAgendamentoPedidoInicial != DateTime.MinValue)
                    filtro += $@" AND EXISTS (
                     SELECT 1
                     FROM t_carga_entrega CargaEntrega
                     WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_DATA_AGENDAMENTO >= '{filtrosPesquisa.DataAgendamentoPedidoInicial.ToString(_dateFormat)}' AND CargaEntrega.CEN_COLETA = 0) ";

                if (filtrosPesquisa.DataAgendamentoPedidoFinal != DateTime.MinValue)
                    filtro += $@" AND EXISTS (
                     SELECT 1
                     FROM t_carga_entrega CargaEntrega
                     WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_DATA_AGENDAMENTO <= '{filtrosPesquisa.DataAgendamentoPedidoFinal.ToString(_dateFormat)}' AND CargaEntrega.CEN_COLETA = 0) ";


                if (filtrosPesquisa.DataColetaPedidoInicial != DateTime.MinValue)
                    filtro += $@" AND EXISTS (
                                    SELECT 1
                                    FROM t_carga_entrega CargaEntrega
                                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_DATA_AGENDAMENTO >= '{filtrosPesquisa.DataColetaPedidoInicial.ToString(_dateFormat)}' AND CargaEntrega.CEN_COLETA = 1) ";

                if (filtrosPesquisa.DataColetaPedidoFinal != DateTime.MinValue)
                    filtro += $@" AND EXISTS (
                                    SELECT 1
                                    FROM t_carga_entrega CargaEntrega
                                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_DATA_AGENDAMENTO <= '{filtrosPesquisa.DataColetaPedidoFinal.ToString(_dateFormat)}' AND CargaEntrega.CEN_COLETA = 1) ";


                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                    filtro += $" AND Pedido.PED_CODIGO_PEDIDO_CLIENTE = '{filtrosPesquisa.NumeroPedidoCliente}' ";

                if (filtrosPesquisa.CodigosClienteComplementar?.Count > 0)
                    filtro += @$" AND EXISTS (
	                            SELECT 1 FROM T_CARGA_PEDIDO CargaPedido
		                            JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                            JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
	                            WHERE Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO AND DestinatarioPedido.CLI_CGCCPF IN ({string.Join(", ", filtrosPesquisa.CodigosClienteComplementar)})
                            ) "; // SQL-INJECTION-SAFE

                if (filtrosPesquisa.CanalVenda > 0)
                    filtro += $" AND Pedido.CNV_CODIGO = '{filtrosPesquisa.CanalVenda}' ";

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EscritorioVenda))
                {
                    List<string> splitEscritorioVenda = filtrosPesquisa.EscritorioVenda.Split(',').Select(m => m.Trim()).ToList();

                    filtro += @$" AND EXISTS (
                                   SELECT 1 FROM T_CARGA_PEDIDO CargaPedido
                                   INNER JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                   INNER JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
                                   INNER JOIN T_CLIENTE_COMPLEMENTAR ClienteComplementar ON ClienteComplementar.CLI_CODIGO = DestinatarioPedido.CLI_CGCCPF
                                   WHERE Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                   AND (ClienteComplementar.CLC_ESCRITORIO_VENDAS LIKE :CLIENTECOMPLEMENTAR_CLC_ESCRITORIO_VENDAS 
                                        OR ClienteComplementar.CLC_ESCRITORIO_VENDAS IN ('{string.Join("','", splitEscritorioVenda)}')))";
                    parametros.Add(new ParametroSQL("CLIENTECOMPLEMENTAR_CLC_ESCRITORIO_VENDAS", $"%{filtrosPesquisa.Matriz}%"));

                }


                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Matriz)) 
                {
                    List<string> splitMatriz = filtrosPesquisa.Matriz.Split(',').Select(m => m.Trim()).ToList();

                    filtro += @$" AND EXISTS (
                                     SELECT 1 FROM T_CARGA_PEDIDO CargaPedido
                                     INNER JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                     INNER JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
                                     INNER JOIN T_CLIENTE_COMPLEMENTAR ClienteComplementar ON ClienteComplementar.CLI_CODIGO = DestinatarioPedido.CLI_CGCCPF
                                     WHERE Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                     AND (ClienteComplementar.CLC_MATRIZ LIKE :CLIENTECOMPLEMENTAR_CLC_MATRIZ  
                                          OR ClienteComplementar.CLC_MATRIZ IN ('{string.Join("','", splitMatriz)}')))"; 
                    parametros.Add(new ParametroSQL("CLIENTECOMPLEMENTAR_CLC_MATRIZ", $"%{filtrosPesquisa.Matriz}%"));
                }



                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EquipeVendas))
                {
                    filtro += $" AND Pedido.PED_EQUIPE_VENDAS like :PEDIDO_PED_EQUIPE_VENDAS ";
                    parametros.Add(new ParametroSQL("PEDIDO_PED_EQUIPE_VENDAS", $"%{filtrosPesquisa.EquipeVendas}%"));
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoMercadoria)) {
                    filtro += $" AND Pedido.PED_TIPO_MERCADORIA like :PEDIDO_PED_TIPO_MERCADORIA ";
                    parametros.Add(new ParametroSQL("PEDIDO_PED_TIPO_MERCADORIA", $"%{filtrosPesquisa.TipoMercadoria}%"));
                }

                if (filtrosPesquisa.Vendedor?.Count > 0)
                {
                    filtro += $" AND Pedido.FUN_CODIGO_VENDEDOR IN ({string.Join(", ", filtrosPesquisa.Vendedor)})";
                    




                }

                if (filtrosPesquisa.Supervisor?.Count > 0)
                {
                    filtro += $" AND Pedido.FUN_CODIGO_SUPERVISOR IN ({string.Join(", ", filtrosPesquisa.Supervisor)})";
                    




                }

                filtro += ")";
            }

            if (filtrosPesquisa.PossuiExpedidor.HasValue)
            {
                AdicionarJoinInternoCargaDadosSumarizados(ref joins);
                if (filtrosPesquisa.PossuiExpedidor.Value)
                {
                    filtro += " and CargaDadosSumarizados.CDS_EXPEDIDORES <> '' ";
                }
                else
                {
                    filtro += " and CargaDadosSumarizados.CDS_EXPEDIDORES = '' ";
                }
            }

            if (filtrosPesquisa.PossuiRecebedor.HasValue)
            {
                AdicionarJoinInternoCargaDadosSumarizados(ref joins);
                if (filtrosPesquisa.PossuiRecebedor.Value)
                {
                    filtro += " and CargaDadosSumarizados.CDS_RECEBEDORES <> '' ";
                }
                else
                {
                    filtro += " and CargaDadosSumarizados.CDS_RECEBEDORES = '' ";
                }
            }

            if (filtrosPesquisa.PrevisaoEntregaInicio != DateTime.MinValue || filtrosPesquisa.PrevisaoEntregaFinal != DateTime.MinValue)
            {
                filtro += @" AND EXISTS (
                    SELECT 1
                    FROM t_carga_entrega CargaEntrega
                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO";

                if (filtrosPesquisa.PrevisaoEntregaInicio != DateTime.MinValue)
                    filtro += $" AND CargaEntrega.CEN_COLETA = 0 AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= convert(datetime, '{filtrosPesquisa.PrevisaoEntregaInicio.ToString(_dateFormat)}', 102) ";

                if (filtrosPesquisa.PrevisaoEntregaFinal != DateTime.MinValue)
                    filtro += $" AND CargaEntrega.CEN_COLETA = 0 AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= convert(datetime, '{filtrosPesquisa.PrevisaoEntregaFinal.ToString(_dateFormat)}', 102) ";

                filtro += ")";
            }

            if (filtrosPesquisa.CodigosDestinos?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND ClienteEntrega.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosDestinos)})) ";
            }

            if (filtrosPesquisa.EstadosDestino?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido 
						    JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND Localidade.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosDestino)}')) ";
            }

            if (filtrosPesquisa.CodigosPaisDestino?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            JOIN T_CLIENTE ClienteEntrega on ClienteEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                            JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = ClienteEntrega.LOC_CODIGO
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND Localidade.PAI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosPaisDestino)})) ";
            }

            //COLOCAR PRA CIMA. EM APENAS UM EXISTS COM O CARGA_PEDIDO..
            if (filtrosPesquisa.CodigosOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND ((CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND Localidade1.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)})) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Localidade2.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)}))))";
            }

            if (filtrosPesquisa.EstadosOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND ((CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND Localidade1.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosOrigem)}')) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Localidade2.UF_SIGLA in ('{string.Join("', '", filtrosPesquisa.EstadosOrigem)}'))))";
            }

            if (filtrosPesquisa.CodigosPaisOrigem?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM t_carga_pedido CargaPedido 
						    JOIN t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            LEFT JOIN t_cliente Cliente1 ON Cliente1.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR
						    LEFT JOIN t_localidades Localidade1 ON Localidade1.LOC_CODIGO = Cliente1.LOC_CODIGO
						    LEFT JOIN t_cliente Cliente2 ON Cliente2.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
						    LEFT JOIN t_localidades Localidade2 ON Localidade2.LOC_CODIGO = Cliente2.LOC_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                            AND ((CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (2, 4) AND Localidade1.PAI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosPaisOrigem)})) 
                            OR (CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES not in (2, 4) AND Localidade2.PAI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosPaisOrigem)}))))";
            }

            //ATE AQUI


            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                filtro += $" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ";

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                filtro += $" and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ";

            if (filtrosPesquisa.VeiculosComContratoDeFrete)
            {
                AdicionarJoinInternoVeiculo(ref joins);
                filtro += $@" and exists (
                    select 
	                    1
                    from
	                    T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador
                    JOIN
	                    T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoFreteTransportadorVeiculo on ContratoFreteTransportadorVeiculo.CFT_CODIGO = ContratoFreteTransportador.CFT_CODIGO
                    where
	                    ContratoFreteTransportador.CFT_ATIVO = 1
	                    and ContratoFreteTransportador.CFT_SITUACAO in (1,2)
	                    and CURRENT_TIMESTAMP between ContratoFreteTransportador.CFT_DATA_INICIAL and ContratoFreteTransportador.CFT_DATA_FINAL
	                    and ContratoFreteTransportadorVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO
                    )";
            }


            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido
                            JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = Pedido.CRE_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Monitoramento.CAR_CODIGO AND Pedido.CRE_CODIGO in ('{string.Join("', '", filtrosPesquisa.CodigosCentroResultado)}'))";
            }

            if (filtrosPesquisa.ApenasMonitoramentosCriticos)
                filtro += " AND Monitoramento.MON_CRITICO = 1 ";

            if (filtrosPesquisa.CodigosTiposTrecho?.Count > 0)
            {
                AdicionarJoinInternoTipoTrecho(ref joins);
                filtro += $" and TipoTrecho.TTR_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTiposTrecho)}) ";
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
                filtro += $" AND EXISTS (SELECT 0 FROM T_CARGA_MOTORISTA WHERE T_CARGA_MOTORISTA.CAR_CODIGO = Monitoramento.CAR_CODIGO AND CAR_MOTORISTA = {filtrosPesquisa.CodigoMotorista} ) "; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.VeiculoNoRaio)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_ENTREGA CargaEntrega
                            WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO AND CargaEntrega.CEN_SITUACAO = 1)";
            }

            if (filtrosPesquisa.ModalTransporte > 0)
            {
                filtro += $@" AND EXISTS (
                            SELECT 1
                            FROM T_CARGA_PEDIDO CargaPedido
                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND CargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = {Convert.ToInt32(filtrosPesquisa.ModalTransporte)} ) ";
            }

            if (filtrosPesquisa.SituacaoIntegracaoSM != null)
            {
                filtro += $@" AND EXISTS (
                SELECT 1
                FROM T_CARGA_CARGA_INTEGRACAO CargaIntegracao
                LEFT JOIN T_TIPO_INTEGRACAO TipoIntegracao 
                ON TipoIntegracao.TPI_CODIGO = CargaIntegracao.TPI_CODIGO
                WHERE CargaIntegracao.CAR_CODIGO = Carga.CAR_CODIGO 
                AND TipoIntegracao.TPI_TIPO IN (168, 8, 11, 12, 183)
                AND CargaIntegracao.INT_SITUACAO_INTEGRACAO = {(int)filtrosPesquisa.SituacaoIntegracaoSM}
            )";
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.RotaFrete))
            {
                filtro += $@" AND EXISTS(
                SELECT 1
                FROM T_CARGA C
                LEFT JOIN T_ROTA_FRETE RotaFrete ON RotaFrete.ROF_CODIGO = C.ROF_CODIGO
                WHERE RotaFrete.ROF_DESCRICAO = :ROTAFRETE_ROF_DESCRICAO AND C.CAR_CODIGO = Carga.CAR_CODIGO)";

                parametros.Add(new ParametroSQL("ROTAFRETE_ROF_DESCRICAO", filtrosPesquisa.RotaFrete));
            }

            if (filtrosPesquisa.Mesoregiao?.Count > 0)
                filtro += @$" AND EXISTS (
	                            SELECT 1 FROM T_CARGA_PEDIDO CargaPedido
		                            JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                            JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
                                    JOIN T_MESO_REGIAO MesoRegiao ON MesoRegiao.MRE_CODIGO = DestinatarioPedido.MRE_CODIGO
	                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND MesoRegiao.MRE_CODIGO IN ({string.Join(", ", filtrosPesquisa.Mesoregiao)})
                            ) "; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Regiao?.Count > 0)
                filtro += @$" AND EXISTS (
	                            SELECT 1 FROM T_CARGA_PEDIDO CargaPedido
		                            JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                            JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
                                    JOIN T_REGIAO Regiao ON Regiao.REG_CODIGO = DestinatarioPedido.REG_CODIGO
	                            WHER E CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND Regiao.REG_CODIGO IN ({string.Join(", ", filtrosPesquisa.Regiao)})
                            ) "; // SQL-INJECTION-SAFE


            if (filtrosPesquisa.ColetaNoPrazo.HasValue)
            {
                if (filtrosPesquisa.ColetaNoPrazo.Value)
                    filtro += @$" AND (SELECT TOP 1 CargaEntrega.CEN_REALIZADA_NO_PRAZO
                                    FROM t_carga_entrega CargaEntrega 
                                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
                                      AND CargaEntrega.CEN_SITUACAO = 2
                                      AND CargaEntrega.CEN_COLETA = 1
                                    ORDER BY CEN_ORDEM_REALIZADA DESC) = 1 ";
                else
                    filtro += @$" AND (SELECT TOP 1 CargaEntrega.CEN_REALIZADA_NO_PRAZO
                                    FROM t_carga_entrega CargaEntrega 
                                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
                                      AND CargaEntrega.CEN_SITUACAO = 2
                                      AND CargaEntrega.CEN_COLETA = 1
                                    ORDER BY CEN_ORDEM_REALIZADA DESC) = 0 ";
            }

            if (filtrosPesquisa.EntregaNoPrazo.HasValue)
            {
                if (filtrosPesquisa.EntregaNoPrazo.Value)
                    filtro += @$" AND (SELECT TOP 1 CargaEntrega.CEN_REALIZADA_NO_PRAZO
                                    FROM t_carga_entrega CargaEntrega 
                                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
                                      AND CargaEntrega.CEN_SITUACAO = 2
                                      AND CargaEntrega.CEN_COLETA = 0
                                    ORDER BY CEN_ORDEM_REALIZADA DESC) = 1 ";
                else
                    filtro += @$" AND (SELECT TOP 1 CargaEntrega.CEN_REALIZADA_NO_PRAZO
                                    FROM t_carga_entrega CargaEntrega 
                                    WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
                                      AND CargaEntrega.CEN_SITUACAO = 2
                                      AND CargaEntrega.CEN_COLETA = 0
                                    ORDER BY CEN_ORDEM_REALIZADA DESC) = 0 ";
            }

            if (filtrosPesquisa.TendenciaProximaColeta.Count > 0)
            {
                filtrosPesquisa.TendenciaProximaColeta.RemoveAll(tendencia => tendencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Nenhum);
                if (filtrosPesquisa.TendenciaProximaColeta.Count > 0)
                    filtro += @$" AND ISNULL(
                                    (SELECT TOP 1 CargaEntrega.CEN_TENDENCIA
                                     FROM t_carga_entrega CargaEntrega 
                                     WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
                                       AND CargaEntrega.CEN_SITUACAO != 2
                                       AND CargaEntrega.CEN_COLETA = 1
                                     ORDER BY CEN_ORDEM),

                                    (SELECT TOP 1 CargaEntrega.CEN_TENDENCIA
                                     FROM t_carga_entrega CargaEntrega 
                                     WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
                                       AND CargaEntrega.CEN_SITUACAO = 2
                                       AND CargaEntrega.CEN_COLETA = 1
                                     ORDER BY CEN_ORDEM DESC)) in ({string.Join(",", filtrosPesquisa.TendenciaProximaColeta.Select(tendencia => ((int)tendencia).ToString()).ToList())}) "; // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.TendenciaProximaEntrega.Count > 0)
            {
                filtrosPesquisa.TendenciaProximaEntrega.RemoveAll(tendencia => tendencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Nenhum);
                if (filtrosPesquisa.TendenciaProximaEntrega.Count > 0)
                    filtro += @$" AND isnull( (SELECT TOP 1 CargaEntrega.CEN_TENDENCIA
											FROM t_carga_entrega CargaEntrega 
											WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
												AND CargaEntrega.CEN_SITUACAO != 2
												AND CargaEntrega.CEN_COLETA = 0
											ORDER BY CEN_ORDEM),
											(SELECT TOP 1 CargaEntrega.CEN_TENDENCIA
											FROM t_carga_entrega CargaEntrega 
											WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
												AND CargaEntrega.CEN_SITUACAO = 2
												AND CargaEntrega.CEN_COLETA = 0
											ORDER BY CEN_ORDEM DESC)) in ({string.Join(",", filtrosPesquisa.TendenciaProximaEntrega.Select(tendencia => ((int)tendencia).ToString()).ToList())}) "; // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.TendenciaEntrega != 0)
            {
                filtro += $@" and (SELECT TOP 1 CargaEntrega.CEN_TENDENCIA
									FROM t_carga_entrega CargaEntrega 
									WHERE CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
									AND CargaEntrega.CEN_SITUACAO != 2
									ORDER BY CEN_ORDEM) = {(int)filtrosPesquisa.TendenciaEntrega}";
            }

            if (filtrosPesquisa.ComAlerta != null)
            {
                if (filtrosPesquisa.ComAlerta == true)
                {
                    filtro += @$" AND EXISTS (
                                    SELECT 1 
                                    FROM T_ALERTA_MONITOR AlertaMonitoramento
                                    WHERE AlertaMonitoramento.CAR_CODIGO = Carga.CAR_CODIGO
                                    AND AlertaMonitoramento.ALE_STATUS = 0)";
                }
                else if (filtrosPesquisa.ComAlerta == false)
                {
                    filtro += @$" AND NOT EXISTS (
                                    SELECT 1 
                                    FROM T_ALERTA_MONITOR AlertaMonitoramento
                                    WHERE AlertaMonitoramento.CAR_CODIGO = Carga.CAR_CODIGO
                                    AND AlertaMonitoramento.ALE_STATUS = 0)";
                }
            }

            if (filtrosPesquisa.TipoAlertaEvento.Count > 0)
            {
                AdicionarJoinInternoTipoAlertaEvento(ref joins);

                filtro += @$" AND MonitoramentoEvento.MEV_CODIGO IN ({string.Join(", ", filtrosPesquisa.TipoAlertaEvento)})";
            }

            return joins + filtro;
        }

        private string GetSQLParametrosView(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sql = "";
            if (parametrosConsulta != null)
            {
                sql = $" order by";
                switch (parametrosConsulta.PropriedadeOrdenar)
                {
                    case "Rastreador":
                    case "Status":
                    case "Coletas":
                    case "Entregas":
                    case "AlertasAbertos":
                        sql = sql + " 1";
                        break;
                    case "DataCarregamento":
                    case "DataCarregamentoFormatada":
                        switch (filtrosPesquisa.DataBaseCalculoPrevisaoControleEntrega)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                                sql = sql + $" DataPrevisaoTerminoCarga";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                                sql = sql + $" DataInicioViagemPrevista";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                                sql = sql + $" DataCarregamentoCarga";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                                sql = sql + $" DataInicioCarregamentoJanela";
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                            default:
                                sql = sql + $" DataCriacaoCarga";
                                break;
                        }
                        break;
                    default:
                        sql = sql + $" {parametrosConsulta.PropriedadeOrdenar}";
                        break;
                }

                sql = sql + $" {parametrosConsulta.DirecaoOrdenar}";
                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0) && !filtrosPesquisa.VeiculosEmLocaisTracking) //se esta filtrando veiculos em locais de tracking nao vamos paginar.
                    sql = sql + $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }
            return sql;
        }

        private void AdicionarJoinInternoTipoAlertaMonitor(ref string joins)
        {
            if (!joins.Contains(" AlertaMonitor "))
                joins += @"left join T_ALERTA_MONITOR AlertaMonitor on AlertaMonitor.CAR_CODIGO = Carga.CAR_CODIGO
                          ";
        }

        private void AdicionarJoinInternoTipoAlertaEvento(ref string joins)
        {
            AdicionarJoinInternoTipoAlertaMonitor(ref joins);
            if (!joins.Contains(" MonitoramentoEvento "))
                joins += @"left join T_MONITORAMENTO_EVENTO MonitoramentoEvento on MonitoramentoEvento.MEV_CODIGO = AlertaMonitor.MEV_CODIGO
                          ";
        }

        private void AdicionarJoinInternoTipoTrecho(ref string joins)
        {
            AdicionarJoinInternoCarga(ref joins);
            if (!joins.Contains(" TipoTrecho "))
                joins += @" left join T_TIPO_TRECHO TipoTrecho ON TipoTrecho.TTR_CODIGO = Carga.TTR_CODIGO
                         ";
        }

        private void AdicionarJoinInternoCarga(ref string joins)
        {
            if (!joins.Contains(" Carga "))
                joins += @" left join T_CARGA Carga on Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO
                         ";
        }

        private void AdicionarJoinInternoCargaDadosSumarizados(ref string joins)
        {
            AdicionarJoinInternoCarga(ref joins);
            if (!joins.Contains(" CargaDadosSumarizados "))
                joins += @" left join T_CARGA_DADOS_SUMARIZADOS as CargaDadosSumarizados on Carga.CDS_CODIGO  = CargaDadosSumarizados.CDS_CODIGO
                         ";
        }

        private void AdicionarJoinInternoPosicao(ref string joins)
        {
            if (!joins.Contains(" Posicao "))
                joins += @" left join T_POSICAO Posicao on Posicao.POS_CODIGO = Monitoramento.POS_ULTIMA_POSICAO
                         ";
        }

        private void AdicionarJoinInternoFilial(ref string joins)
        {
            AdicionarJoinInternoCarga(ref joins);
            if (!joins.Contains(" Filial "))
                joins += @" left join T_FILIAL as Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO
                         ";
        }

        private void AdicionarJoinInternoVeiculo(ref string joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins += @" join T_VEICULO as Veiculo on Monitoramento.VEI_CODIGO = Veiculo.VEI_CODIGO
                         ";
        }

        private void AdicionarJoinInternoTipoOperacao(ref string joins)
        {
            AdicionarJoinInternoCarga(ref joins);

            if (!joins.Contains(" TipoOperacao "))
                joins += @"left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                          ";
        }

        private void AdicionarJoinInternoGrupoTipoOperacao(ref string joins)
        {
            AdicionarJoinInternoTipoOperacao(ref joins);

            if (!joins.Contains(" GrupoTipoOperacao "))
                joins += @"left join T_GRUPO_TIPO_OPERACAO GrupoTipoOperacao on GrupoTipoOperacao.GTO_CODIGO = TipoOperacao.GTO_CODIGO
                          ";
        }

        private void AdicionarJoinInternoEmpresa(ref string joins)
        {
            AdicionarJoinInternoCarga(ref joins);

            if (!joins.Contains(" Empresa "))
                joins += @"left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO
                          ";
        }

        #endregion
    }
}
