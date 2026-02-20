using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoHistoricoStatusViagem : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>
    {
        public MonitoramentoHistoricoStatusViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MonitoramentoHistoricoStatusViagem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> Consultar(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();

            if (monitoramento != null)
                query = query.Where(obj => obj.Monitoramento == monitoramento);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();

            if (monitoramento != null)
                query = query.Where(obj => obj.Monitoramento == monitoramento);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> BuscarPorMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();
            query = query = query.Where(obj => obj.Monitoramento == monitoramento);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>> BuscarPorMonitoramentoAsync(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();
            query = query = query.Where(obj => obj.Monitoramento == monitoramento);
            return query.OrderBy(obj => obj.DataInicio).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> BuscarPorMonitoramentos(List<int> CodMonitoramentos)
        {
            return BuscarPorMonitoramentosAsync(CodMonitoramentos).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>> BuscarPorMonitoramentosAsync(IList<int> codigosMonitoramentos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> query = ObterQueryableBuscarPorMonitoramentos(codigosMonitoramentos);

            return query.ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem BuscarHistoricoAnteriorDoMonitoramento(Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem monitororamentoHistoricoStatusViagem, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();
            query = query = query.Where(obj => obj.Codigo != monitororamentoHistoricoStatusViagem.Codigo && obj.DataInicio <= monitororamentoHistoricoStatusViagem.DataInicio && obj.Monitoramento == monitoramento);
            return query.OrderByDescending(obj => obj.DataInicio).Take(1).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem BuscarUltimoHistoricoDoMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();
            query = query = query.Where(obj => obj.Monitoramento == monitoramento);
            return query.OrderByDescending(obj => obj.DataInicio).Take(1).FirstOrDefault();
        }


        public Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoStatusStatusViagem BuscarUltimoStatusViagemDoHistoricoMonitoramentoDBComponents(DbConnection connection, DbTransaction transaction, int codigo)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoStatusStatusViagem monitoramentoStatusStatusViagem = null;
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = 300;
            command.Transaction = transaction;
            command.CommandType = CommandType.Text;
            command.Connection = connection;

            command.CommandText = $@"
                select 
                   top 1 MSV_CODIGO CodigoStatusViagem
                from 
                    T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM
                where
                    MON_CODIGO = @mon_codigo 
                order by
                   MHS_DATA_INICIO asc";

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
                    CodigoStatusViagem = (reader["CodigoStatusViagem"] != DBNull.Value) ? int.Parse(reader["CodigoStatusViagem"].ToString()) : 0
                };
            }
            if (!reader.IsClosed) reader.Close();

            return monitoramentoStatusStatusViagem;
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem BuscarUltimoHistoricoStatusViagem(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();
            query = query.Where(obj => obj.Monitoramento.Codigo == codigo);
            var resultado = query.OrderByDescending(obj => obj.DataInicio).FirstOrDefault();
            return resultado;
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem BuscarAbertoPorMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();
            query = query = query.Where(obj => obj.Monitoramento == monitoramento && obj.DataFim == null);
            return query.FirstOrDefault();
        }

        public void ExcluirTodosPorMonitoramento(int codigoMonitoramento)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE MonitoramentoHistoricoStatusViagem obj WHERE obj.Monitoramento = :Monitoramento")
                             .SetInt32("Monitoramento", codigoMonitoramento)
                             .ExecuteUpdate();
        }

        public void ExcluirTodosPorMonitoramentos(List<int> codigosMonitoramento)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE MonitoramentoHistoricoStatusViagem obj WHERE obj.Monitoramento in (:Monitoramentos)")
                             .SetParameterList("Monitoramentos", codigosMonitoramento)
                             .ExecuteUpdate();
        }

        /**
         * Inserção de um novo regitro via DbConnection em detrimento ao NHibernate para obter alta performace
         */
        public void Inserir(int codigoMonitoramento, int codigoStatus, double latitude, double longitude, int codigoVeiculo, DateTime dataInicio, double codigoCliente, int codigoSubarea, DateTime? dataFim, DbConnection connection, DbTransaction transaction)
        {
            DbCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            string sqlColunas = "mhs_latitude, mhs_longitude, mon_codigo, msv_codigo, vei_codigo, mhs_data_inicio";
            string sqlValues = "@mhs_latitude, @mhs_longitude, @mon_codigo, @msv_codigo, @vei_codigo, @mhs_data_inicio";

            DbParameter parLatitude = command.CreateParameter();
            parLatitude.ParameterName = "@mhs_latitude";
            parLatitude.Value = latitude;
            command.Parameters.Add(parLatitude);

            DbParameter parLongitude = command.CreateParameter();
            parLongitude.ParameterName = "@mhs_longitude";
            parLongitude.Value = longitude;
            command.Parameters.Add(parLongitude);

            DbParameter parMonCodigo = command.CreateParameter();
            parMonCodigo.ParameterName = "@mon_codigo";
            parMonCodigo.Value = codigoMonitoramento;
            command.Parameters.Add(parMonCodigo);

            DbParameter parMsvCodigo = command.CreateParameter();
            parMsvCodigo.ParameterName = "@msv_codigo";
            parMsvCodigo.Value = codigoStatus;
            command.Parameters.Add(parMsvCodigo);

            DbParameter parVeiCodigo = command.CreateParameter();
            parVeiCodigo.ParameterName = "@vei_codigo";
            parVeiCodigo.Value = codigoVeiculo;
            command.Parameters.Add(parVeiCodigo);

            DbParameter parDataInicio = command.CreateParameter();
            parDataInicio.ParameterName = "@mhs_data_inicio";
            parDataInicio.Value = dataInicio;
            command.Parameters.Add(parDataInicio);

            if (codigoCliente > 0)
            {
                sqlColunas += ", cli_cgccpf";
                sqlValues += ", @cli_cgccpf";

                DbParameter parCodigoCliente = command.CreateParameter();
                parCodigoCliente.ParameterName = "@cli_cgccpf";
                parCodigoCliente.Value = codigoCliente;
                command.Parameters.Add(parCodigoCliente);
            }

            if (codigoSubarea > 0)
            {
                sqlColunas += ", sac_codigo";
                sqlValues += ", @sac_codigo";

                DbParameter parCodigoSubarea = command.CreateParameter();
                parCodigoSubarea.ParameterName = "@sac_codigo";
                parCodigoSubarea.Value = codigoSubarea;
                command.Parameters.Add(parCodigoSubarea);
            }

            if (dataFim != null)
            {

                sqlColunas += ", mhs_data_fim, mhs_tempo_segundos";
                sqlValues += ", @mhs_data_fim, @mhs_tempo_segundos";

                DbParameter parDataFim = command.CreateParameter();
                parDataFim.ParameterName = "@mhs_data_fim";
                parDataFim.Value = dataFim;
                command.Parameters.Add(parDataFim);

                DbParameter parTempoSegundos = command.CreateParameter();
                parTempoSegundos.ParameterName = "@mhs_tempo_segundos";
                int tempo = (dataFim.Value > dataInicio) ? (int)(dataFim.Value - dataInicio).TotalSeconds : 0;
                parTempoSegundos.Value = tempo;
                command.Parameters.Add(parTempoSegundos);

            }

            command.CommandText = $"INSERT INTO t_monitoramento_historico_status_viagem ({sqlColunas}) VALUES ({sqlValues})"; // SQL-INJECTION-SAFE

            command.ExecuteNonQuery();
        }

        /**
         * Encerra um de um novo regitro via DbConnection em detrimento ao NHibernate para obter alta performace
         */
        public void DefinirDataFinal(int codigoMonitoramento, DateTime dataFim, DbConnection connection, DbTransaction transaction)
        {
            DbCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = "UPDATE t_monitoramento_historico_status_viagem  " +
                "SET mhs_data_fim = (case when @mhs_data_fim < mhs_data_inicio then mhs_data_inicio else @mhs_data_fim end), mhs_tempo_segundos = datediff(second, mhs_data_inicio, (case when @mhs_data_fim < mhs_data_inicio then mhs_data_inicio else @mhs_data_fim end)) " +
                "WHERE mon_codigo = @mon_codigo and mhs_data_fim is null ";

            DbParameter parDataFim = command.CreateParameter();
            parDataFim.ParameterName = "@mhs_data_fim";
            parDataFim.Value = dataFim;
            command.Parameters.Add(parDataFim);

            DbParameter parMonCodigo = command.CreateParameter();
            parMonCodigo.ParameterName = "@mon_codigo";
            parMonCodigo.Value = codigoMonitoramento;
            command.Parameters.Add(parMonCodigo);

            command.ExecuteNonQuery();
        }

        /**
         * Inserção de um novo regitro via DbConnection em detrimento ao NHibernate para obter alta performace
         */
        public void InserirEncerrarAnterior(int codigoMonitoramento, int codigoStatus, double latitude, double longitude, int codigoVeiculo, DateTime dataInicio, double codigoCliente, int codigoSubarea, DateTime? dataFim, DbConnection connection, DbTransaction transaction)
        {
            DefinirDataFinal(codigoMonitoramento, dataInicio, connection, transaction);
            Inserir(codigoMonitoramento, codigoStatus, latitude, longitude, codigoVeiculo, dataInicio, codigoCliente, codigoSubarea, dataFim, connection, transaction);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> BuscarObjetoDeValorPorMonitoramento(int codigoMonitoramento)
        {

            string sql = @" Select
                                MonitoramentoHistoricoStatusViagem.MON_CODIGO CodigoMonitoramento,
                                MonitoramentoHistoricoStatusViagem.MHS_LATITUDE Latitude,
                                MonitoramentoHistoricoStatusViagem.MHS_LONGITUDE Longitude,
                                MonitoramentoHistoricoStatusViagem.MHS_DATA Data,
                                MonitoramentoStatusViagem.MSV_TIPO_REGRA TipoRegra,
                                MonitoramentoHistoricoStatusViagem.MHS_DATA_INICIO DataInicio,
                                MonitoramentoHistoricoStatusViagem.MHS_DATA_FIM DataFim,
                                MonitoramentoHistoricoStatusViagem.MHS_TEMPO_SEGUNDOS TempoSegundos,
                                MonitoramentoHistoricoStatusViagem.CLI_CGCCPF CodigoCliente
                            From
                                T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM MonitoramentoHistoricoStatusViagem
                                    Join T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatusViagem On MonitoramentoStatusViagem.MSV_CODIGO = MonitoramentoHistoricoStatusViagem.MSV_CODIGO
                            Where
                                MonitoramentoHistoricoStatusViagem.MON_CODIGO = :codigoMonitoramento
                            Order By
                                MonitoramentoHistoricoStatusViagem.MHS_DATA_INICIO,
                                MonitoramentoHistoricoStatusViagem.MHS_CODIGO";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("codigoMonitoramento", codigoMonitoramento);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> ObterQueryableBuscarPorMonitoramentos(IList<int> codigosMonitoramentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem>();

            query = query = query.Where(obj => codigosMonitoramentos.Contains(obj.Monitoramento.Codigo)).OrderBy(obj => obj.DataInicio);

            return query;
        }

        #endregion Métodos Privados

    }

}