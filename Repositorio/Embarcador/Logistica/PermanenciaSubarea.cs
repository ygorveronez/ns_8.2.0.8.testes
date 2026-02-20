using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PermanenciaSubarea : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>
    {
        public PermanenciaSubarea(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPorSubarea(Dominio.Entidades.Embarcador.Logistica.SubareaCliente subarea)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.Subarea == subarea);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPorTipoSubarea(Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente tipoSubarea)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.Subarea.TipoSubarea == tipoSubarea);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }
        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPorSubareas(List<int> subareas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => subareas.Contains(obj.Subarea.Codigo));
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => codigosCarga.Contains(obj.CargaEntrega.Carga.Codigo));
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPorCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.CargaEntrega == cargaEntrega);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPorSubareaECargaEntrega(Dominio.Entidades.Embarcador.Logistica.SubareaCliente subarea, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.CargaEntrega == cargaEntrega && obj.Subarea == subarea);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea BuscarAbertaPorSubareaECargaEntrega(int codigoSubarea, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.CargaEntrega == cargaEntrega && obj.Subarea.Codigo == codigoSubarea && obj.DataFim == null);
            return query.OrderBy(obj => obj.DataInicio).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea BuscarAbertaPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.CargaEntrega.Carga == carga && obj.DataFim == null);
            return query.OrderBy(obj => obj.DataInicio).FirstOrDefault();
        }

        public DateTime BuscarDataUltimaSaidaDaSubareaECargaEntrega(int codigoSubarea, int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
            query = query.Where(obj => obj.Subarea.Codigo == codigoSubarea && obj.DataFim != null && obj.CargaEntrega.Codigo == codigoCargaEntrega);
            var result = query.OrderByDescending(obj => obj.DataFim).FirstOrDefault();
            return result?.DataFim ?? DateTime.MinValue;
        }

        public void ExcluirTodosPorCargaEntrega(int codigoCargaEntrega)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE PermanenciaSubarea obj WHERE obj.CargaEntrega IN (:cargaEntrega)")
                             .SetInt32("cargaEntrega", codigoCargaEntrega)
                             .ExecuteUpdate();
        }

    }
}
