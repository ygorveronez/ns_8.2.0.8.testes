using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class Posicao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.Posicao>
    {
        public Posicao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Posicao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPorCodigoMaior(Int64 codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo > codigo);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarPosicaoAnteriorVeiculo(Int64 codigoPosicao, int Veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo < codigoPosicao && ent.Veiculo.Codigo == Veiculo);

            return result.OrderByDescending(o => o.DataVeiculo).Fetch(obj => obj.Veiculo).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarUltimaPosicao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query
                         orderby obj.Codigo descending
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarUltimaPosicaoDataVeiculo(int Veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query
                         where obj.Veiculo.Codigo == Veiculo && obj.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado
                         orderby obj.DataVeiculo descending
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarUltimaPosicaoDataVeiculo(string numeroEquipamentoRastreador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query
                         where obj.IDEquipamento == numeroEquipamentoRastreador && obj.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado
                         orderby obj.DataVeiculo descending
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarUltimaPosicaoDataVeiculoPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query
                         where obj.Veiculo.Placa == placa
                         orderby obj.DataVeiculo descending
                         select obj;

            return result.FirstOrDefault();
        }
        public long BuscarCodigoUltimaPosicaoVeiculo(int Veiculo, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from pos in query
                         where
                            pos.Veiculo.Codigo == Veiculo &&
                            pos.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado &&
                            pos.DataVeiculo >= dataInicial &&
                            pos.DataVeiculo <= dataFinal
                         orderby pos.DataVeiculo descending
                         select pos.Codigo;

            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarUltimaPosicaoVeiculo(int Veiculo, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            var result = from obj in query select obj;
            result = result.Where(ent => (
                ent.Veiculo.Codigo == Veiculo &&
                ent.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado &&
                ent.DataVeiculo >= dataInicial &&
                ent.DataVeiculo <= dataFinal
            ));
            result = result.OrderByDescending(ent => ent.DataVeiculo);
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarUltimaPosicaoVeiculo(int Veiculo, DateTime dataInicial, DateTime dataFinal, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes)
        {
            return posicoes.Where(ent => (
                ent.Veiculo.Codigo == Veiculo &&
                ent.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado &&
                ent.DataVeiculo >= dataInicial &&
                ent.DataVeiculo <= dataFinal
            )).OrderByDescending(ent => ent.DataVeiculo).FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPosicaoVeiculos(List<int> codigosVeiculos, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            var result = from obj in query select obj;
            result = result.Where(ent => (
                codigosVeiculos.Contains(ent.Veiculo.Codigo) &&
                ent.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado &&
                ent.DataVeiculo >= dataInicial &&
                ent.DataVeiculo <= dataFinal
            ));
            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarPosicaoPorVeiculoData(int codigoVeiculo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>()
                .Where(o => o.Veiculo.Codigo == codigoVeiculo && o.DataVeiculo == data);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPorCodigosLista(List<Int64> codigos)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Posicao> result = new List<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            int take = 600;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<Int64> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();
                var filter = from obj in query
                             where tmp.Contains(obj.Codigo)
                             select obj;

                result.AddRange(filter.Select(x => x).ToList());

                start += take;
            }

            return result;
        }


        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPorVeiculoDataInicialeFinal(int Veiculo, DateTime dataInicial, DateTime dataFinal, string direcaoOrdenar = "", List<int> codigosVeiculo = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            var result = from obj in query select obj;

            if (codigosVeiculo == null)
                result = result.Where(ent => (ent.DataVeiculo > dataInicial && ent.DataVeiculo < dataFinal && ent.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado && ent.Veiculo.Codigo == Veiculo));
            else
                result = result.Where(ent => (ent.DataVeiculo > dataInicial && ent.DataVeiculo < dataFinal && ent.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado && codigosVeiculo.Contains(ent.Veiculo.Codigo)));

            if (direcaoOrdenar == "desc")
            {
                result = result.OrderByDescending(ent => ent.DataVeiculo);
            }
            else
            {
                result = result.OrderBy(ent => ent.DataVeiculo);
            }
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPorVeiculosDataInicialeFinal(List<int> codigosVeiculos, DateTime dataInicial, DateTime dataFinal, string direcaoOrdenar = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.DataVeiculo >= dataInicial && ent.DataVeiculo <= dataFinal && ent.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado && codigosVeiculos.Contains(ent.Veiculo.Codigo));
            result = result.Fetch(ent => ent.Veiculo);
            if (direcaoOrdenar == "desc")
            {
                result = result.OrderByDescending(ent => ent.DataVeiculo);
            }
            else
            {
                result = result.OrderBy(ent => ent.DataVeiculo);
            }
            return result.WithOptions(options => options.SetTimeout(120)).ToList();

        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarWaypointsPorVeiculoDataInicialeFinal(int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, string direcaoOrdenar = "")
        {
            List<int> codigosVeiculos = new List<int>();
            codigosVeiculos.Add(codigoVeiculo);
            return BuscarWaypointsPorVeiculosDataInicialeFinal(codigosVeiculos, dataInicial, dataFinal, direcaoOrdenar);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarWaypointsPorVeiculosDataInicialeFinal(List<int> codigosVeiculos, DateTime dataInicial, DateTime dataFinal, string direcaoOrdenar = "")
        {
            string sql = $@"
                select
                    Posicao.POS_CODIGO ID,
					Posicao.POS_DATA Data,
					Posicao.POS_DATA_CADASTRO DataCadastro,
                    Posicao.POS_DATA_VEICULO DataVeiculo,
					Posicao.VEI_CODIGO CodigoVeiculo,
					Posicao.POS_ID_EQUIPAMENTO IDEquipamento,
					Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude,
					Posicao.POS_DESCRICAO Descricao,
					Posicao.POS_IGNICAO Ignicao,
					Posicao.POS_VELOCIDADE Velocidade,
					Posicao.POS_TEMPERATURA Temperatura,
					Posicao.POS_SENSOR_TEMPERATURA SensorTemperatura,
                    Posicao.POS_NIVEL_BATERIA NivelBateria,
                    Posicao.POS_NIVEL_SINAL_GPS NivelSinalGPS,
					Posicao.POS_EM_ALVO EmAlvo,
                    case
						when Posicao.POS_EM_ALVO = 1 then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							WHERE PosicaoAlvo.POS_CODIGO = Posicao.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosClientesAlvo
                from
	                T_POSICAO Posicao
                where
	                Posicao.VEI_CODIGO in ({String.Join(",", codigosVeiculos)})
                    and Posicao.POS_PROCESSAR = :pos_processar
	                and Posicao.POS_DATA_VEICULO >= :data_inicio
	                and Posicao.POS_DATA_VEICULO <= :data_fim
                order by
	                Posicao.POS_DATA_VEICULO ";

            if (direcaoOrdenar == "desc") sql += "desc";
            else sql += "asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetParameter("pos_processar", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
            query.SetParameter("data_inicio", dataInicial);
            query.SetParameter("data_fim", dataFinal);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarWaypointsPorMonitoramentoVeiculo(int codigoMonitoramento, int? codigoVeiculo, DateTime dataInicial)
        {
            string sql = $@"
                select
                    Posicao.POS_CODIGO ID,
					Posicao.POS_DATA_VEICULO DataVeiculo,
					Posicao.VEI_CODIGO CodigoVeiculo,
					Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude
                from
                    T_MONITORAMENTO_VEICULO MonitoramentoVeiculo 
                join
	                T_MONITORAMENTO_VEICULO_POSICAO MonitoramentoVeiculoPosicao 
                        on MonitoramentoVeiculo.MOV_CODIGO = MonitoramentoVeiculoPosicao.MOV_CODIGO
				join
	                T_POSICAO Posicao
                        on MonitoramentoVeiculoPosicao.POS_CODIGO = Posicao.POS_CODIGO 
                where
	                MonitoramentoVeiculo.MON_CODIGO = :mon_codigo
                    and Posicao.POS_PROCESSAR = :pos_processar
					and Posicao.POS_DATA_VEICULO >= :data_inicio";

            if (codigoVeiculo > 0) sql += @"
                    and Posicao.VEI_CODIGO = :vei_codigo and MonitoramentoVeiculo.VEI_CODIGO = :vei_codigo";

            sql += " order by Posicao.POS_DATA_VEICULO asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("mon_codigo", codigoMonitoramento);
            if (codigoVeiculo > 0)
                query.SetParameter("vei_codigo", codigoVeiculo);
            query.SetParameter("pos_processar", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
            query.SetParameter("data_inicio", dataInicial);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarWaypointsPorMonitoramentoVeiculoTuplas(List<Tuple<int, int, DateTime>> monitoramentosVeiculoDataInicial)
        {
            string sql = $@"
                select
                    Posicao.POS_CODIGO ID,
					Posicao.POS_DATA_VEICULO DataVeiculo,
					Posicao.VEI_CODIGO CodigoVeiculo,
					Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude,
                    MonitoramentoVeiculo.MON_CODIGO CodigoMonitoramento
                from
                    T_MONITORAMENTO_VEICULO MonitoramentoVeiculo 
                join
	                T_MONITORAMENTO_VEICULO_POSICAO MonitoramentoVeiculoPosicao 
                        on MonitoramentoVeiculo.MOV_CODIGO = MonitoramentoVeiculoPosicao.MOV_CODIGO
				join
	                T_POSICAO Posicao
                        on MonitoramentoVeiculoPosicao.POS_CODIGO = Posicao.POS_CODIGO 
                where Posicao.POS_PROCESSAR = 2 and (";

            sql += string.Join(" or ", monitoramentosVeiculoDataInicial.Select(m => ObterWhereTupla(m)).ToList());

            sql += ") order by Posicao.POS_DATA_VEICULO asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
        }
        private string ObterWhereTupla(Tuple<int, int, DateTime> tupla)
        {
            string where = string.Empty;
            where += @$"( MonitoramentoVeiculo.MON_CODIGO = {tupla.Item1}
                            and Posicao.POS_DATA_VEICULO >= '{tupla.Item3.ToString("yyyy-MM-dd HH:mm:ss")}'
                            and Posicao.VEI_CODIGO = {tupla.Item2}
                            and MonitoramentoVeiculo.VEI_CODIGO = {tupla.Item2} )";
            return where;
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao BuscarPrimeiroWaypointPorMonitoramentoVeiculo(int codigoMonitoramento, int codigoVeiculo, DateTime? dataInicial)
        {
            string sql = $@"
                select top 1
                    Posicao.POS_CODIGO ID,
					Posicao.POS_DATA_VEICULO DataVeiculo,
					Posicao.VEI_CODIGO CodigoVeiculo,
					Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude
                from
                    T_MONITORAMENTO_VEICULO MonitoramentoVeiculo 
                join
	                T_MONITORAMENTO_VEICULO_POSICAO MonitoramentoVeiculoPosicao 
                        on MonitoramentoVeiculo.MOV_CODIGO = MonitoramentoVeiculoPosicao.MOV_CODIGO
				join
	                T_POSICAO Posicao
                        on MonitoramentoVeiculoPosicao.POS_CODIGO = Posicao.POS_CODIGO 
                where
	                MonitoramentoVeiculo.MON_CODIGO = :mon_codigo
	                and MonitoramentoVeiculo.VEI_CODIGO = :vei_codigo
                    and Posicao.POS_PROCESSAR = :pos_processar
                    and Posicao.VEI_CODIGO = :vei_codigo
					and Posicao.POS_DATA_VEICULO >= :data_inicio
                order by
	                Posicao.POS_DATA_VEICULO asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("mon_codigo", codigoMonitoramento);
            query.SetParameter("vei_codigo", codigoVeiculo);
            query.SetParameter("pos_processar", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
            query.SetParameter("data_inicio", dataInicial ?? DateTime.MinValue);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
            query.SetTimeout(600);
            return query.UniqueResult<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarWaypointsPorMonitoramentoVeiculo(int codigoMonitoramento, int? codigoVeiculo, DateTime? dataInicial, DateTime? dataFinal, bool comTemperaturaValida = false, bool ApenasPosicoesMobile = false, bool ApenasPosicoesTecnologia = false)
        {

            string sql = @"
                select
                    Posicao.POS_CODIGO ID,
					Posicao.POS_DATA_VEICULO DataVeiculo,
                    Posicao.POS_DATA_CADASTRO DataCadastro,
					Veiculo.VEI_CODIGO CodigoVeiculo,
                    Veiculo.VEI_PLACA Placa,
                    Posicao.POS_DESCRICAO Descricao,					
                    Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude,
                    Posicao.POS_IGNICAO Ignicao,
                    Posicao.POS_VELOCIDADE Velocidade,
                    Posicao.POS_TEMPERATURA Temperatura,
                    Posicao.POS_SENSOR_TEMPERATURA SensorTemperatura
                from
                    T_POSICAO Posicao
                join
	                T_MONITORAMENTO_VEICULO_POSICAO MonitoramentoVeiculoPosicao 
                        on MonitoramentoVeiculoPosicao.POS_CODIGO = Posicao.POS_CODIGO
                join
	                T_MONITORAMENTO_VEICULO MonitoramentoVeiculo 
                        on MonitoramentoVeiculo.MOV_CODIGO = MonitoramentoVeiculoPosicao.MOV_CODIGO
                join
	                T_VEICULO Veiculo 
                        on Veiculo.VEI_CODIGO = MonitoramentoVeiculo.VEI_CODIGO
                where
	                MonitoramentoVeiculo.MON_CODIGO = :mon_codigo
                    and Posicao.POS_PROCESSAR = :pos_processar";

            if (codigoVeiculo > 0) sql += @"
                    and Posicao.VEI_CODIGO = :vei_codigo";

            if (dataInicial != null) sql += @"
                    and Posicao.POS_DATA_VEICULO >= :data_inicio";

            if (dataFinal != null) sql += @"
                    and Posicao.POS_DATA_VEICULO <= :data_fim";

            if (comTemperaturaValida == true) sql += @"
                    and Posicao.POS_SENSOR_TEMPERATURA = 1";

            if (ApenasPosicoesMobile == true && ApenasPosicoesTecnologia == false)
                sql += @" and Posicao.POS_RASTREADOR = 1";
            else if (ApenasPosicoesTecnologia == true && ApenasPosicoesMobile == false)
                sql += @" and Posicao.POS_RASTREADOR != 1";
            else if (ApenasPosicoesTecnologia == true && ApenasPosicoesMobile == true)
                sql += @"";

            sql += @"
                order by
	                Posicao.POS_DATA_VEICULO asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("mon_codigo", codigoMonitoramento);
            query.SetParameter("pos_processar", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
            if (dataInicial != null) query.SetParameter("data_inicio", dataInicial);
            if (dataFinal != null) query.SetParameter("data_fim", dataFinal);
            if (codigoVeiculo > 0) query.SetParameter("vei_codigo", codigoVeiculo);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao BuscarWaypointPosicaoVizinha(DateTime data, int codigoVeiculo, bool vizinhoAnterior = true)
        {
            string sql = $@"
                select TOP 1
                    Posicao.POS_CODIGO ID,
					Posicao.POS_DATA Data,
					Posicao.POS_DATA_CADASTRO DataCadastro,
                    Posicao.POS_DATA_VEICULO DataVeiculo,
					Posicao.VEI_CODIGO CodigoVeiculo,
					Posicao.POS_ID_EQUIPAMENTO IDEquipamento,
					Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude,
					Posicao.POS_DESCRICAO Descricao,
					Posicao.POS_IGNICAO Ignicao,
					Posicao.POS_VELOCIDADE Velocidade,
					Posicao.POS_TEMPERATURA Temperatura,
					Posicao.POS_SENSOR_TEMPERATURA SensorTemperatura,
					Posicao.POS_EM_ALVO EmAlvo,
                    Posicao.POS_NIVEL_BATERIA NivelBateria,
                    Posicao.POS_NIVEL_SINAL_GPS NivelSinalGPS
                from
	                T_POSICAO Posicao
                where
	                Posicao.VEI_CODIGO = :vei_veiculo
                    and Posicao.POS_PROCESSAR = :pos_processar
	                and Posicao.POS_DATA_VEICULO {((vizinhoAnterior) ? "<" : ">")} :pos_data_veiculo
                order by
	                Posicao.POS_DATA_VEICULO {((vizinhoAnterior) ? "desc" : "asc")}";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetParameter("pos_processar", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
            query.SetParameter("vei_veiculo", codigoVeiculo);
            query.SetParameter("pos_data_veiculo", data);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
            query.SetTimeout(600);
            return query.UniqueResult<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
        }

        public List<Dominio.Entidades.Cliente> BuscarUltimosAlvosPorVeiculoDataInicialeFinal(int Veiculo, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();

            var result = from obj in query select obj;
            result = result.Where(ent => (
                ent.Posicao.DataVeiculo >= dataInicial &&
                ent.Posicao.DataVeiculo <= dataFinal &&
                ent.Posicao.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado &&
                ent.Posicao.Veiculo.Codigo == Veiculo &&
                ent.Posicao.EmAlvo == true
            )).OrderByDescending(ent => ent.Posicao.DataVeiculo);
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicoesAlvo = result.ToList();

            List<Dominio.Entidades.Cliente> clientes = new List<Dominio.Entidades.Cliente>();
            int total = posicoesAlvo.Count;
            for (int i = 0; i < total; i++)
            {
                clientes.Add(posicoesAlvo[i].Cliente);
            }
            return clientes;
        }

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarPrimeiraPosicaoEmAlvoPorVeiculoDataInicialeFinal(double codigoCliente, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();

            var result = from obj in query select obj;
            result = result.Where(ent => (
                ent.Posicao.DataVeiculo >= dataInicial &&
                ent.Posicao.DataVeiculo <= dataFinal &&
                ent.Posicao.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado &&
                ent.Posicao.Veiculo.Codigo == codigoVeiculo &&
                ent.Cliente.CPF_CNPJ == codigoCliente
            )).Fetch(ent => ent.Posicao).OrderBy(ent => ent.Posicao.DataVeiculo);

            Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo posicaoAlvo = result.FirstOrDefault();
            return posicaoAlvo?.Posicao ?? null;
        }

        //public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarPrimeiraPosicaoEmAlvoPorVeiculoDataInicialeFinal(List<long> codigoPosicoes, double codigoCliente)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();

        //    var result = from obj in query select obj;
        //    result = result.Where(ent => (
        //       codigoPosicoes.Contains(ent.Posicao.Codigo) &&
        //        ent.Cliente.CPF_CNPJ == codigoCliente
        //    )).OrderBy(ent => ent.Posicao.DataVeiculo);

        //    Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo posicaoAlvo = result.FirstOrDefault();
        //    return posicaoAlvo?.Posicao ?? null;
        //}


        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarPrimeiraPosicaoEmLocalPorVeiculoDataInicialeFinal(List<long> codigoPosicoes, int codigoLocal)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoLocal> posicoes = new();
            int take = 500;
            int skip = 0;
            int total = codigoPosicoes.Count;
            while (skip < total)
            {
                List<long> codigosParciais = codigoPosicoes.Skip(skip).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoLocal>();

                var result = from obj in query select obj;
                result = result.Where(ent => (
                    codigosParciais.Contains(ent.Posicao.Codigo) &&
                    ent.Local.Codigo == codigoLocal
                ));

                posicoes.AddRange(result.ToList());
                skip += take;
            }

            Dominio.Entidades.Embarcador.Logistica.PosicaoLocal posicaoAlvo = posicoes.OrderBy(ent => ent.Posicao.DataVeiculo).FirstOrDefault();
            return posicaoAlvo?.Posicao ?? null;
        }

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarPrimeiraPosicaoEmAlvoSubareaPorVeiculoDataInicialeFinal(long codigoSubarea, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>();

            var result = from obj in query select obj;
            result = result.Where(ent => (
                ent.PosicaoAlvo.Posicao.DataVeiculo >= dataInicial &&
                ent.PosicaoAlvo.Posicao.DataVeiculo <= dataFinal &&
                ent.PosicaoAlvo.Posicao.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado &&
                ent.PosicaoAlvo.Posicao.Veiculo.Codigo == codigoVeiculo &&
                ent.SubareaCliente.Codigo == codigoSubarea
            )).Fetch(ent => ent.PosicaoAlvo).ThenFetch(ent => ent.Posicao).OrderBy(ent => ent.PosicaoAlvo.Posicao.DataVeiculo);

            Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea posicaoAlvoSubarea = result.FirstOrDefault();
            return posicaoAlvoSubarea?.PosicaoAlvo.Posicao ?? null;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPorIDEquipamentoDataInicialeFinal(string IDEquipamento, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query select obj;
            result = result.Where(ent => (ent.DataVeiculo >= dataInicial && ent.DataVeiculo <= dataFinal && ent.IDEquipamento == IDEquipamento));

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPorCodigoMaiorEVelociadade(Int64 codigo, int velocidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo > codigo && ent.Velocidade > velocidade);

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarProcessar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao processar)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            consulta = consulta.Where(o => o.Processar == processar).OrderBy(o => o.DataVeiculo);
            return consulta.Fetch(obj => obj.Veiculo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarProcessarComLimite(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao> litaSituacoesProcessar, int quantidadePosicoes = 100)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            consulta = consulta.Where(o => litaSituacoesProcessar.Contains(o.Processar)).OrderBy(o => o.DataVeiculo);
            return consulta.Fetch(obj => obj.Veiculo).Take(quantidadePosicoes).ToList();
        }

        public int BuscarContarProcessar(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao> litaSituacoesProcessar)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            consulta = consulta.Where(o => litaSituacoesProcessar.Contains(o.Processar));
            return consulta.Count();
        }

        public void AtualizarProcessar(List<long> codigosPosicoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao processar, DbConnection connection, DbTransaction transaction)
        {
            if (codigosPosicoes.Count > 0)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandTimeout = 300;
                command.Transaction = transaction;

                command.CommandText = "UPDATE t_posicao SET pos_processar = @pos_processar WHERE pos_codigo in (" + String.Join(",", codigosPosicoes) + ")"; // SQL-INJECTION-SAFE

                DbParameter posProcessar = command.CreateParameter();
                posProcessar.ParameterName = "@pos_processar";
                posProcessar.Value = (int)processar;
                command.Parameters.Add(posProcessar);

                command.ExecuteNonQuery();
            }
        }

        public Dominio.Entidades.Embarcador.Logistica.Posicao BuscarPrimeiraPosicao(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            var dataFim = monitoramento.DataFim != null ? monitoramento.DataFim : DateTime.Now;

            var result = query.Where(p => p.Veiculo.Codigo == codigoVeiculo
                                        && p.Data >= monitoramento.DataInicio && p.Data <= dataFim)
                            .OrderBy(p => p.Data)
                            .FirstOrDefault();

            return result;
        }


        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>> BuscarObjetoDeValorPorMonitoramentoAsync(int codigoMonitoramento, int? codigoVeiculo, DateTime? dataInicial, DateTime? dataFinal)
        {
            string sql = @"
                select
                    Posicao.POS_CODIGO ID,
					Posicao.POS_DATA_VEICULO DataVeiculo,
                    Posicao.POS_DATA_CADASTRO DataCadastro,
					Veiculo.VEI_CODIGO CodigoVeiculo,
                    Veiculo.VEI_PLACA Placa,
                    Posicao.POS_DESCRICAO Descricao,					
                    Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude,
                    Posicao.POS_IGNICAO Ignicao,
                    Posicao.POS_VELOCIDADE Velocidade,
                    Posicao.POS_TEMPERATURA Temperatura,
                    Posicao.POS_SENSOR_TEMPERATURA SensorTemperatura,
                    Posicao.POS_EM_ALVO EmAlvo,
                    case
						when Posicao.POS_EM_ALVO = 1
                        then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							WHERE PosicaoAlvo.POS_CODIGO = Posicao.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosClientesAlvo
                from
                    T_POSICAO Posicao
                join
	                T_MONITORAMENTO_VEICULO_POSICAO MonitoramentoVeiculoPosicao 
                        on MonitoramentoVeiculoPosicao.POS_CODIGO = Posicao.POS_CODIGO
                join
	                T_MONITORAMENTO_VEICULO MonitoramentoVeiculo 
                        on MonitoramentoVeiculo.MOV_CODIGO = MonitoramentoVeiculoPosicao.MOV_CODIGO
                join
	                T_VEICULO Veiculo 
                        on Veiculo.VEI_CODIGO = MonitoramentoVeiculo.VEI_CODIGO
                where
	                MonitoramentoVeiculo.MON_CODIGO = :mon_codigo
                    and Posicao.POS_PROCESSAR = :pos_processar";

            if (codigoVeiculo > 0) sql += @"
                    and Posicao.VEI_CODIGO = :vei_codigo";

            if (dataInicial != null) sql += @"
                    and Posicao.POS_DATA_VEICULO >= :data_inicio";

            if (dataFinal != null) sql += @"
                    and Posicao.POS_DATA_VEICULO <= :data_fim";

            sql += @"
                order by
	                Posicao.POS_DATA_VEICULO asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("mon_codigo", codigoMonitoramento);
            query.SetParameter("pos_processar", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
            if (dataInicial != null) query.SetParameter("data_inicio", dataInicial);
            if (dataFinal != null) query.SetParameter("data_fim", dataFinal);
            if (codigoVeiculo > 0) query.SetParameter("vei_codigo", codigoVeiculo);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
            query.SetTimeout(600);
            var result = await query.ListAsync<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            return result.ToList();
        }
        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarObjetoDeValorPorMonitoramento(int codigoMonitoramento, int? codigoVeiculo, DateTime? dataInicial, DateTime? dataFinal)
        {
            string sql = @"
                select
                    Posicao.POS_CODIGO ID,
					Posicao.POS_DATA_VEICULO DataVeiculo,
                    Posicao.POS_DATA_CADASTRO DataCadastro,
					Veiculo.VEI_CODIGO CodigoVeiculo,
                    Veiculo.VEI_PLACA Placa,
                    Posicao.POS_DESCRICAO Descricao,					
                    Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude,
                    Posicao.POS_IGNICAO Ignicao,
                    Posicao.POS_VELOCIDADE Velocidade,
                    Posicao.POS_TEMPERATURA Temperatura,
                    Posicao.POS_SENSOR_TEMPERATURA SensorTemperatura,
                    Posicao.POS_EM_ALVO EmAlvo,
                    case
						when Posicao.POS_EM_ALVO = 1
                        then (
							SELECT ',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]
							FROM T_POSICAO_ALVO PosicaoAlvo
							WHERE PosicaoAlvo.POS_CODIGO = Posicao.POS_CODIGO 
							FOR XML PATH ('')
						)
						else null
					end CodigosClientesAlvo
                from
                    T_POSICAO Posicao
                join
	                T_MONITORAMENTO_VEICULO_POSICAO MonitoramentoVeiculoPosicao 
                        on MonitoramentoVeiculoPosicao.POS_CODIGO = Posicao.POS_CODIGO
                join
	                T_MONITORAMENTO_VEICULO MonitoramentoVeiculo 
                        on MonitoramentoVeiculo.MOV_CODIGO = MonitoramentoVeiculoPosicao.MOV_CODIGO
                join
	                T_VEICULO Veiculo 
                        on Veiculo.VEI_CODIGO = MonitoramentoVeiculo.VEI_CODIGO
                where
	                MonitoramentoVeiculo.MON_CODIGO = :mon_codigo
                    and Posicao.POS_PROCESSAR = :pos_processar";

            if (codigoVeiculo > 0) sql += @"
                    and Posicao.VEI_CODIGO = :vei_codigo";

            if (dataInicial != null) sql += @"
                    and Posicao.POS_DATA_VEICULO >= :data_inicio";

            if (dataFinal != null) sql += @"
                    and Posicao.POS_DATA_VEICULO <= :data_fim";

            sql += @"
                order by
	                Posicao.POS_DATA_VEICULO asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("mon_codigo", codigoMonitoramento);
            query.SetParameter("pos_processar", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
            if (dataInicial != null) query.SetParameter("data_inicio", dataInicial);
            if (dataFinal != null) query.SetParameter("data_fim", dataFinal);
            if (codigoVeiculo > 0) query.SetParameter("vei_codigo", codigoVeiculo);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
        }

        public async Task<DateTime?> BuscarDataUltimaPorGerenciadoraAsync(int veiculoId, EnumTecnologiaGerenciadora gerenciadora)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>()
                .Where(p =>
                    p.Veiculo.Codigo == veiculoId &&
                    p.Gerenciadora == gerenciadora &&
                    p.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado)
                .OrderByDescending(p => p.DataVeiculo)
                .Select(p => (DateTime?)p.DataVeiculo)
                .FirstOrDefaultAsync();
        }

        public async Task<DateTime?> BuscarDataUltimaPorRastreadorAsync(int veiculoId, EnumTecnologiaRastreador rastreador)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>()
                .Where(p =>
                    p.Veiculo.Codigo == veiculoId &&
                    p.Rastreador == rastreador &&
                    p.Processar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado)
                .OrderByDescending(p => p.DataVeiculo)
                .Select(p => (DateTime?)p.DataVeiculo)
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Sobrecargas

        public new long Inserir(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = null)
        {
            if (posicao.Processar == 0)
                posicao.Processar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Pendente;

            if (posicao.DataCadastro == null)
                posicao.DataCadastro = DateTime.Now;

            if (string.IsNullOrWhiteSpace(posicao.Descricao))
                posicao.Descricao = $"{Math.Round(posicao.Latitude, 6)} {Math.Round(posicao.Longitude, 6)}";
            else if (posicao.Descricao.Length >= 100)
                posicao.Descricao = posicao.Descricao.Substring(0, 99);

            return base.Inserir(posicao, Auditado, historioPai);
        }

        public int ObterVelocidadeMediaUltimasNPosicoes(int codigoVeiculo, int ultimasNPosicoes, DateTime dataInicial)
        {
            string sql = $@"
                            Select 
	                            Sum( Iif( POS_VELOCIDADE > 0, POS_VELOCIDADE, 0)) / IsNull( NullIf( Sum( Iif( POS_VELOCIDADE > 0, 1, 0)), 0), 1) As Media
                            From ( Select Top {ultimasNPosicoes} POS_VELOCIDADE
		                            From T_POSICAO
		                            Where VEI_CODIGO = {codigoVeiculo}
                                        And POS_DATA_VEICULO >= '{dataInicial.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
		                            Order By POS_CODIGO Desc) Tmp";


            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetTimeout(10);
            return query.UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.Posicao> Consultar(string descricao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Posicao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));


            return consulta;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarPosicoesVeiculoPorData(List<int> codigosVeiculos, DateTime dataInicial, DateTime dataFinal)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> result = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            int take = 600;
            int start = 0;
            while (start < codigosVeiculos.Count)
            {
                List<int> tmp = codigosVeiculos.Skip(start).Take(take).ToList();

                string sql = $@"
                    select
                        Posicao.POS_CODIGO ID,
                        Posicao.POS_DATA_VEICULO DataVeiculo,
					    Posicao.POS_DATA_CADASTRO DataCadastro,
					    Posicao.VEI_CODIGO CodigoVeiculo,
					    Posicao.POS_LATITUDE Latitude,
	                    Posicao.POS_LONGITUDE Longitude
                    from
	                    T_POSICAO Posicao
                    where
	                    Posicao.VEI_CODIGO in ({String.Join(",", tmp)})
                        and Posicao.POS_PROCESSAR = :pos_processar
	                    and Posicao.POS_DATA_VEICULO >= :data_inicio
	                    and Posicao.POS_DATA_VEICULO <= :data_fim
                    order by
                        Posicao.VEI_CODIGO,
	                    Posicao.POS_DATA_VEICULO ";

                var query = this.SessionNHiBernate.CreateSQLQuery(sql);

                query.SetParameter("pos_processar", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
                query.SetParameter("data_inicio", dataInicial);
                query.SetParameter("data_fim", dataFinal);

                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao)));
                query.SetTimeout(600);

                result.AddRange(query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>());

                start += take;
            }

            return result;
        }

        #endregion
    }
}
