using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>
    {

        #region Atributos públicos

        public MonitoramentoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos públicos

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> BuscarPorCodigosMonitoramentoLista(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> result = new List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();

            int take = 600;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();
                var filter = from obj in query
                             where tmp.Contains(obj.Monitoramento.Codigo)
                             select obj;

                result.AddRange(filter.OrderBy(x => x.DataInicio).ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> BuscarTodosPorMonitoramento(int codigoMonitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Monitoramento.Codigo == codigoMonitoramento);
            return result.OrderBy(ent => ent.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> BuscarTodosPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Veiculo.Codigo == codigoVeiculo);
            return result.OrderBy(ent => ent.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> BuscarTodosPorMonitoramentoVeiculo(int codigoMonitoramento, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Monitoramento.Codigo == codigoMonitoramento && ent.Veiculo.Codigo == codigoVeiculo);
            return result.OrderBy(ent => ent.DataInicio).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo BuscarUltimoPorMonitoramentoVeiculo(int codigoMonitoramento, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Monitoramento.Codigo == codigoMonitoramento && ent.Veiculo.Codigo == codigoVeiculo);
            return result.OrderByDescending(ent => ent.DataInicio).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo BuscarAbertoPorMonitoramento(int codigoMonitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Monitoramento.Codigo == codigoMonitoramento && ent.DataFim == null);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> BuscarAbertosPorMonitoramento(int codigoMonitoramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo>();
            var result = from obj in query select obj;
            result = result.Where(ent => ent.Monitoramento.Codigo == codigoMonitoramento && ent.DataFim == null);
            return result.OrderBy(ent => ent.DataInicio).ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo BuscarUltimoPorMonitoramentoVeiculoOV(int codigoMonitoramento, int codigoVeiculo)
        {
            string sql = $@"
                select
                    MOV_CODIGO as Codigo,
                    MON_CODIGO as CodigoMonitoramento,
                    VEI_CODIGO as CodigoVeiculo,
                    MOV_DATA_INICIO as DataInicio,
                    MOV_DATA_FIM as DataFim,
                    MOV_POLILINHA as Polilinha,
                    MOV_DISTANCIA as Distancia
                from
                    T_MONITORAMENTO_VEICULO 
                where
                    MON_CODIGO = :mon_codigo
                    and VEI_CODIGO = :vei_codigo
                order by
                    MOV_DATA_INICIO desc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("mon_codigo", codigoMonitoramento);
            query.SetParameter("vei_codigo", codigoVeiculo);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo)));
            return query.UniqueResult<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo>();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo BuscarUltimoPorMonitoramentoVeiculoOVSimples(int codigoMonitoramento, int codigoVeiculo)
        {
            string sql = $@"
                select
                    MOV_CODIGO as Codigo,
                    MON_CODIGO as CodigoMonitoramento,
                    VEI_CODIGO as CodigoVeiculo
                from
                    T_MONITORAMENTO_VEICULO 
                where
                    MON_CODIGO = :mon_codigo
                    and VEI_CODIGO = :vei_codigo
                order by
                    MOV_DATA_INICIO desc
                offset 
                    0 rows 
                fetch first 
                    1 rows only";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("mon_codigo", codigoMonitoramento);
            query.SetParameter("vei_codigo", codigoVeiculo);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo)));
            return query.UniqueResult<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo>();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo BuscarUltimoPorVeiculoSimples(int codigoVeiculo)
        {
            string sql = $@"
                select
                    MOV_CODIGO as Codigo,
                    T_MONITORAMENTO.CAR_CODIGO Carga,
                    T_MONITORAMENTO.MON_CODIGO as CodigoMonitoramento,
                    T_MONITORAMENTO.VEI_CODIGO as CodigoVeiculo
                from
                    T_MONITORAMENTO_VEICULO 
	                inner join T_MONITORAMENTO ON T_MONITORAMENTO.MON_CODIGO = T_MONITORAMENTO_VEICULO.MON_CODIGO
                where
                    T_MONITORAMENTO_VEICULO.VEI_CODIGO = :vei_codigo
                    and MOV_DATA_FIM is not null
                order by
                    MOV_DATA_INICIO desc
                offset 
                    0 rows 
                fetch first 
                    1 rows only";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("vei_codigo", codigoVeiculo);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo)));
            return query.UniqueResult<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo>();
        }

        #endregion

    }
}
