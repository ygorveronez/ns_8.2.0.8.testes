using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class ParadasMonitoramentosFinalizados : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados>
    {

        #region Métodos públicos

        public ParadasMonitoramentosFinalizados(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados> BuscarPorDataEVeiculo(List<int> codigosVeiculo, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados>();

            var result = from obj in query select obj;
            result = result.Where(p => codigosVeiculo.Contains(p.Veiculo.Codigo) &&
            p.DataInicio >= dataInicio &&
            p.DataFim <= dataFim);

            return result.ToList();
        }

        #endregion

        #region Métodos privados
        #endregion

    }



}
