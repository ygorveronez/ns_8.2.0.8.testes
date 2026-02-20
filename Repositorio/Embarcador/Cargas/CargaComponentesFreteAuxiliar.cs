using System.Collections.Generic;
using System.Linq;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaComponentesFreteAuxiliar : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar>
    {
        public CargaComponentesFreteAuxiliar(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar BuscarPrimeiroPorCargaPorCompomente(int carga,  Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFreteAuxiliar>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.FirstOrDefault();
        }        
    }
}
