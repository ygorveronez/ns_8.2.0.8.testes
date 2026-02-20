using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class AlertaMonitor : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>
    {

        #region Métodos públicos

        public AlertaMonitor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.AlertaMonitor BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarTodosEmAberto()
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto)
                    //.Fetch(obj => obj.AlertaMonitorNotificacao)
                    .Fetch(obj => obj.Veiculo);

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarEmAbertoPorVeiculoeTipoAlertaeCodigoMenor(int veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => (ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto || ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmTratativa) &&
                                            ent.Veiculo.Codigo == veiculo &&
                                            ent.TipoAlerta == tipoAlerta &&
                                            ent.Codigo <= codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == carga);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == codigoCarga).OrderBy(obj => obj.Data);
            return ObterLista(result, parametrosConsulta);
        }

        public int ContarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == codigoCarga);
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarPorCargaETipoAlerta(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoAlerta == tipoAlerta).OrderBy(obj => obj.Data);
            return ObterLista(result, parametrosConsulta);
        }

        public int ContarPorCargaETipoAlerta(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoAlerta == tipoAlerta);
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarEmAbertoPorCargaeTipoAlertaEResponsavel(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, int codigoResponsavel = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            query = query.Where(ent => ent.Carga.Codigo == carga && ent.TipoAlerta == tipoAlerta);

            if (codigoResponsavel > 0)
                query = query.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto ||
                                            (ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmTratativa && ent.Responsavel.Codigo == codigoResponsavel));
            else
                query = query.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto);


            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarEmAbertoPorCarga(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && ent.Carga.Codigo == carga && ent.TipoAlerta == tipoAlerta);


            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarEmAbertoPorCarga(int carga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && ent.Carga.Codigo == carga);


            return ObterLista(result, parametrosConsulta);

        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarEmAbertoPorCargaeTipoAlerta(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && ent.Carga.Codigo == carga && ent.TipoAlerta == tipoAlerta);
            return ObterLista(result, parametrosConsulta);
        }


        public int ContarEmAbertoPorVeiculo(int veiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && ent.Veiculo.Codigo == veiculo);

            return result.Count();

        }

        public int ContarEmAbertoPorVeiculoeTipoAlerta(int veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && ent.Veiculo.Codigo == veiculo && ent.TipoAlerta == tipoAlerta);

            return result.Count();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.AlertaMonitor BuscarAlertaPorCargaChamado(int carga, int codigoChamado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && ent.Chamado.Codigo == codigoChamado && ent.Carga.Codigo == carga);
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return result.ToList();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertasResumo> BuscarTotaisAlertasEmAberto(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus statusMonitoramento)
        {
            string SQL = $@"SELECT ALE_TIPO as TipoAlerta, COUNT(VEI_CODIGO) Total 
                        FROM(
                            SELECT AMO.ALE_TIPO, AMO.VEI_CODIGO 
                            FROM T_ALERTA_MONITOR AMO
                            WHERE 
                              ALE_STATUS = 0";

            if (statusMonitoramento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Todos)
            {
                SQL += $@"
                              AND EXISTS(
                                SELECT top 1 * FROM T_MONITORAMENTO MON
                                JOIN T_CARGA CAR ON CAR.CAR_CODIGO = MON.CAR_CODIGO AND CAR.CAR_VEICULO = AMO.VEI_CODIGO 
                                WHERE MON.MON_STATUS = {(int)statusMonitoramento}
                              )";
            }

            SQL += $@"
                            GROUP BY AMO.ALE_TIPO, AMO.VEI_CODIGO
                        ) AS GRUPO
                        GROUP BY ALE_TIPO";

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(SQL);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertasResumo)));
            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertasResumo>();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarEmAbertoPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.TipoAlerta == tipo && ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarEmAberto()
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasEmAbertoPorCargaETipoDeAlerta(List<int> codigosCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto &&
                                         codigosCarga.Contains(ent.Carga.Codigo) && tiposAlerta.Contains(ent.TipoAlerta));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasPorCargaETipoDeAlerta(List<int> codigosCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            query = query.Where(ent => codigosCarga.Contains(ent.Carga.Codigo) && tiposAlerta.Contains(ent.TipoAlerta));

            return query
                .Fetch(obj => obj.CargaEntrega).ThenFetch(c => c.Cliente)
                .Fetch(obj => obj.MonitoramentoEvento)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasPorCargaETipoDeAlerta(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta, DateTime? dataInicial, DateTime? dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            query = query.Where(ent => ent.Carga.Codigo == codigoCarga && tiposAlerta.Contains(ent.TipoAlerta));
            if (dataInicial != null) query = query.Where(ent => ent.Data >= dataInicial);
            if (dataFinal != null) query = query.Where(ent => ent.Data <= dataFinal);
            return query.Fetch(obj => obj.Veiculo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasPorVeiculoETipoDeAlerta(int codigoVeiculo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta, DateTime? dataInicial, DateTime? dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            query = query.Where(ent => ent.Veiculo.Codigo == codigoVeiculo && tiposAlerta.Contains(ent.TipoAlerta));
            if (dataInicial != null) query = query.Where(ent => ent.Data >= dataInicial);
            if (dataFinal != null) query = query.Where(ent => ent.Data <= dataFinal);
            return query.Fetch(obj => obj.Veiculo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasPorVeiculoETipoDeAlerta(List<int> codigosVeiculo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta, DateTime? dataInicial, DateTime? dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            query = query.Where(ent => codigosVeiculo.Contains(ent.Veiculo.Codigo) && tiposAlerta.Contains(ent.TipoAlerta));
            if (dataInicial != null) query = query.Where(ent => ent.Data >= dataInicial);
            if (dataFinal != null) query = query.Where(ent => ent.Data <= dataFinal);
            return query.Fetch(obj => obj.Veiculo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasEmAbertoPorVeiculoETipoDeAlerta(List<int> codigosVeiculos, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = new List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            int total = codigosVeiculos.Count, limit = 1000, index = 0;
            while (index < total)
            {
                List<int> codigosVeiculosParciais = codigosVeiculos.Skip(index).Take(limit).ToList();

                IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
                query = query.Where(
                    ent =>
                        ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto &&
                        codigosVeiculosParciais.Contains(ent.Veiculo.Codigo) &&
                        tiposAlerta.Contains(ent.TipoAlerta)
                    ).OrderBy(ent => ent.Data);

                List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertasParciais = query.Fetch(obj => obj.Carga).Fetch(ent => ent.Veiculo).ToList();
                alertas = alertas.Concat(alertasParciais).ToList();
                index += limit;
            }
            alertas.Sort((x, y) => x.Data.CompareTo(y.Data));
            return alertas;
        }

        public List<int> BuscarCodigosVeiculosComAlertaEmAbertoPorVeiculoETipoDeAlerta(List<int> codigosVeiculos, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            List<int> codigosVeiculosComAlerta = new List<int>();

            int total = codigosVeiculos.Count, limit = 1000, index = 0;
            while (index < total)
            {
                List<int> codigosVeiculosParciais = codigosVeiculos.Skip(index).Take(limit).ToList();

                IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
                query = query.Where(
                    ent =>
                        ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto &&
                        codigosVeiculosParciais.Contains(ent.Veiculo.Codigo) &&
                        tiposAlerta.Contains(ent.TipoAlerta)
                    );

                List<int> codigosVeiculosComAlertaParciais = query.Select(obj => obj.Veiculo.Codigo).ToList();
                codigosVeiculosComAlerta = codigosVeiculosComAlerta.Concat(codigosVeiculosComAlertaParciais).ToList();
                index += limit;
            }
            return codigosVeiculosComAlerta;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> BuscarUltimoAlertaVeiculo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> lista = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>
            {
                tipoAlerta
            };

            return BuscarUltimoAlertaVeiculo(lista);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> BuscarUltimoAlertaVeiculo(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            string sql = $@"SELECT 
                        ALE_CODIGO AS Codigo,
                        ALE_TIPO AS TipoAlerta,
                        VEI_CODIGO AS Veiculo,
                        ALE_DATA AS Data,
                        ALE_STATUS AS Status,
                        ALE_DATA_CADASTRO AS DataCadastro
                      FROM (
                            SELECT *, ROW_NUMBER ( ) OVER (  PARTITION BY VEI_CODIGO ORDER BY ALE_DATA DESC)  IDLINHA  
                            FROM T_ALERTA_MONITOR
                            WHERE ALE_TIPO IN ({GetFiltro(tiposAlerta)})  

                        ) GRUPO WHERE IDLINHA = 1";


            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(sql);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta>();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAlertaMonitor filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> consulta = Consultar(filtroPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> consulta = Consultar(descricao);

            return consulta.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> BuscarUltimoAlertaObjetoDeValorPorVeiculosTiposDeAlerta(List<int> codigosVeiculos, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor>();
            if (codigosVeiculos != null && codigosVeiculos.Count > 0 && tiposAlerta != null && tiposAlerta.Count > 0)
            {
                string filtroTiposAlertas = string.Join(",", tiposAlerta.Select(t => (int)t).ToArray());
                int total = codigosVeiculos.Count, limit = 1000, index = 0;
                while (index < total)
                {
                    List<int> codigosVeiculosParciais = codigosVeiculos.Skip(index).Take(limit).ToList();

                    string sql = $@"WITH UltimoAlerta AS
                                    (
                                        SELECT
                                            ALE_CODIGO              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.Codigo)},
                                            CAR_CODIGO              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.CodigoCarga)},
                                            VEI_CODIGO              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.CodigoVeiculo)},
                                            ALE_DATA_CADASTRO       AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.DataCadastro)},
                                            MEV_CODIGO              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.CodigoMonitoramentoEvento)},
                                            ALE_DATA                AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.Data)},
                                            ALE_DATA_FIM            AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.DataFim)},
                                            ALE_TIPO                AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.TipoAlerta)},
                                            ALE_STATUS              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.Status)},
                                            ALE_ALERTA_REPROGRAMADO AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.AlertaReprogramado)},
                                            ALE_TEMPO_REPROGRAMADO  AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.TempoReprogramado)},
                                            ROW_NUMBER() OVER (
                                                PARTITION BY Alerta.CAR_CODIGO, Alerta.VEI_CODIGO, Alerta.MEV_CODIGO
                                                ORDER BY Alerta.ALE_CODIGO DESC
                                            )                       AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.rn)}
                                        FROM dbo.T_ALERTA_MONITOR Alerta
                                        WHERE
                                            Alerta.ALE_TIPO IN ({filtroTiposAlertas})
                                            AND Alerta.VEI_CODIGO IN ({string.Join(", ", codigosVeiculosParciais)})
                                    )
                                    SELECT *
                                    FROM UltimoAlerta
                                    WHERE rn = 1;";

                    NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
                    query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor)));
                    IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertasParciais = query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor>();

                    alertas = alertas.Concat(alertasParciais).ToList();
                    index += limit;
                }
                alertas.Sort((x, y) => x.Data.CompareTo(y.Data));
            }
            return alertas;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> BuscarUltimoAlertaCargaETipoDeAlerta(int carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor>();
            if (carga > 0 && tiposAlerta != null && tiposAlerta.Count > 0)
            {
                string sql = $@"WITH UltimoAlerta AS
                                (
                                    SELECT
                                        ALE_CODIGO              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.Codigo)},
                                        CAR_CODIGO              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.CodigoCarga)},
                                        VEI_CODIGO              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.CodigoVeiculo)},
                                        ALE_DATA_CADASTRO       AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.DataCadastro)},
                                        MEV_CODIGO              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.CodigoMonitoramentoEvento)},
                                        ALE_DATA                AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.Data)},
                                        ALE_DATA_FIM            AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.DataFim)},
                                        ALE_TIPO                AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.TipoAlerta)},
                                        ALE_STATUS              AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.Status)},
                                        ALE_ALERTA_REPROGRAMADO AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.AlertaReprogramado)},
                                        ALE_TEMPO_REPROGRAMADO  AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.TempoReprogramado)},
                                        ROW_NUMBER() OVER
                                        (
                                            PARTITION BY Alerta.CAR_CODIGO, Alerta.VEI_CODIGO
                                            ORDER BY Alerta.ALE_CODIGO DESC
                                        )                       AS {nameof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor.rn)}
                                    FROM t_alerta_monitor Alerta
                                    WHERE
                                        Alerta.CAR_CODIGO = {carga}
                                        AND Alerta.ALE_TIPO in ({string.Join(",", tiposAlerta.Select(t => (int)t).ToArray())})
                                )
                                SELECT *
                                FROM UltimoAlerta Alerta
                                WHERE Alerta.rn = 1;";

                NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor)));
                alertas = query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor>();
            }

            return alertas;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarTodosEmAbertoQueEnviaEmailEPossuiSequenciaTratativaMenor()
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> subQuery = from o
                          in this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa>()
                                                                                             where ((o.EnvioEmail == true || o.EnvioEmailCliente == true || o.EnvioEmailTransportador == true) && o.Sequencia > 0)
                                                                                             select o.MonitoramentoEvento.TipoAlerta;

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && subQuery.Contains(ent.TipoAlerta)).Fetch(obj => obj.Veiculo);

            //var result = from obj in query select obj;
            //result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto)
            //        //.Fetch(obj => obj.AlertaMonitorNotificacao)
            //        .Fetch(obj => obj.Veiculo);

            return result.ToList();

        }

        public void ExcluirTodosPorCargaEntrega(int codigoCargaEntrega)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM AlertasAcompanhamentoCarga WHERE Codigo IN (SELECT c.Codigo FROM AlertasAcompanhamentoCarga c WHERE c.AlertaMonitor.CargaEntrega IN (:cargaEntrega))").SetInt32("cargaEntrega", codigoCargaEntrega).ExecuteUpdate();
            UnitOfWork.Sessao.CreateQuery("DELETE FROM AlertaMonitorNotificacao WHERE Codigo IN (SELECT c.Codigo FROM AlertaMonitorNotificacao c WHERE c.AlertaMonitor.CargaEntrega IN (:cargaEntrega))").SetInt32("cargaEntrega", codigoCargaEntrega).ExecuteUpdate();
            UnitOfWork.Sessao.CreateQuery("DELETE AlertaMonitor obj WHERE obj.CargaEntrega IN (:cargaEntrega)").SetInt32("cargaEntrega", codigoCargaEntrega).ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasEmAbertoPorCargaETipoDeAlerta(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, int codigoMonitoramentoEvento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto &&
                                         ent.Carga.Codigo == codigoCarga &&
                                         ent.TipoAlerta == tipoAlerta &&
                                         ent.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento
                                         );

            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasEmAbertoPorCargaEntregaETipoDeAlerta(int codigoCargaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, int codigoMonitoramentoEvento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto &&
                                         ent.CargaEntrega.Codigo == codigoCargaEntrega &&
                                         ent.TipoAlerta == tipoAlerta &&
                                         ent.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento
                                         );

            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasEmAbertoPorPlacaETipoDeAlerta(string placa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, int codigoMonitoramentoEvento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> result = from obj in query select obj;
            result = result.Where(ent => ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto &&
                                         ent.Veiculo.Placa == placa &&
                                         ent.TipoAlerta == tipoAlerta &&
                                         ent.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento
                                         );

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasEmAbertoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            query = query.Where(ent => ent.Carga.Codigo == codigoCarga &&
                                      (ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto ||
                                       ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmTratativa));
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> BuscarAlertasEmAbertoPorNDias(DateTime dataCriacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            query = query.Where(ent => (ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto ||
                                        ent.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmTratativa) &&
                                        ent.DataCadastro <= dataCriacao);
            return query.Take(500).ToList();
        }

        #endregion

        #region Métodos privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAlertaMonitor filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            if (filtroPesquisa.Codigo > 0)
                consulta = consulta.Where(o => o.Codigo == filtroPesquisa.Codigo);

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtroPesquisa.Descricao));

            if (filtroPesquisa.Data != null)
                consulta = consulta.Where(o => o.Data == filtroPesquisa.Data);

            if (filtroPesquisa.DataInicial != null)
                consulta = consulta.Where(o => o.Data >= filtroPesquisa.DataInicial);

            if (filtroPesquisa.DataFinal != null)
                consulta = consulta.Where(o => o.Data <= filtroPesquisa.DataFinal);

            if (filtroPesquisa.DataCadastro != null)
                consulta = consulta.Where(o => o.DataCadastro == filtroPesquisa.DataCadastro);

            if (filtroPesquisa.DataFim != null)
                consulta = consulta.Where(o => o.DataFim == filtroPesquisa.DataFim);

            if (filtroPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto
                || filtroPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado)
                consulta = consulta.Where(o => o.Status == filtroPesquisa.Status);

            if (filtroPesquisa.TipoAlerta != null)
                consulta = consulta.Where(o => o.TipoAlerta == filtroPesquisa.TipoAlerta);

            if (filtroPesquisa.Carga != null)
                consulta = consulta.Where(o => o.Carga == filtroPesquisa.Carga);

            if (filtroPesquisa.Veiculo != null)
                consulta = consulta.Where(o => o.Veiculo == filtroPesquisa.Veiculo);

            return consulta;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> Consultar(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(descricao));

            return consulta;
        }

        private string GetFiltro(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            string filtro = "";
            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipo in tiposAlerta)
            {
                if (filtro != "")
                    filtro = filtro + ",";
                filtro = filtro + Convert.ToString((int)tipo);
            }

            return filtro;

        }

        #endregion

    }



}
