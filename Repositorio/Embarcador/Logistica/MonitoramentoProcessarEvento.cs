using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoProcessarEvento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoProcessarEvento>
    {
        public MonitoramentoProcessarEvento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public new List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoProcessarEvento> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoProcessarEvento>();
            return query.OrderBy(obj => obj.Data).ToList();
        }

        //public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoProcessarEvento> BuscarTodos(int quantidadeRegistros)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoProcessarEvento>();
        //    query = query.OrderBy(obj => obj.Posicao.DataVeiculo).ThenBy(obj => obj.Data).ThenBy(obj => obj.Codigo);
        //    query = query.Fetch(obj => obj.Posicao);
        //    return query.Take(quantidadeRegistros).ToList();
        //}

		public Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento BuscarProcessarEventosPendentes(long codigoPosicao, int codigoMonitoramento)
		{
			string sql = $@"
                select
					Posicao.POS_CODIGO CodigoPosicao,
					Posicao.VEI_CODIGO CodigoVeiculo,
					Posicao.POS_DATA_VEICULO DataVeiculoPosicao,
					Posicao.POS_LATITUDE LatitudePosicao,
					Posicao.POS_LONGITUDE LongitudePosicao,
					Posicao.POS_IGNICAO IgnicaoPosicao,
					Posicao.POS_VELOCIDADE VelocidadePosicao,
                    Posicao.POS_TEMPERATURA TemperaturaPosicao,
                    Posicao.POS_SENSOR_TEMPERATURA SensorTemperaturaPosicao,
					Posicao.POS_EM_ALVO EmAlvoPosicao,
					case
						when Posicao.POS_EM_ALVO = 1 then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							WHERE PosicaoAlvo.POS_CODIGO = Posicao.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosClientesAlvoPosicao,
					Monitoramento.MON_CODIGO CodigoMonitoramento,
					Monitoramento.MON_DATA_CRIACAO DataCriacaoMonitoramento,
					Monitoramento.MON_DATA_INICIO DataInicioMonitoramento,
					Monitoramento.MON_DATA_FIM DataFimMonitoramento,
					Monitoramento.CAR_CODIGO CodigoCarga,
					Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem,
					Carga.CAR_DATA_CARREGAMENTO DataCarregamentoCarga
				from
					t_posicao Posicao
				left join
					t_monitoramento Monitoramento on Monitoramento.MON_CODIGO = {codigoMonitoramento}
				left join
					t_carga Carga on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO
				where
					Posicao.POS_CODIGO = {codigoPosicao}";

			var query = this.SessionNHiBernate.CreateSQLQuery(sql);
			query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento)));
			Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento = query.UniqueResult<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento>();
			return monitoramentoProcessarEvento;
		}

		public void ExcluiPorCodigos(List<int> codigos, DbConnection connection, DbTransaction transaction)
		{
			if (codigos != null && codigos.Count > 0)
			{
				DbCommand command = connection.CreateCommand();
				command.CommandTimeout = 300;
				command.Transaction = transaction;

				command.CommandText = "DELETE t_monitoramento_processar_evento WHERE mpe_codigo in (" + string.Join(",", codigos) + ")"; // SQL-INJECTION-SAFE
                command.ExecuteNonQuery();
			}
		}

	}

}
