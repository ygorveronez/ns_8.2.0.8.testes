using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoSaldoMesCTe : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe>
    {
        public ContratoSaldoMesCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Metodos publicos
        public List<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe> BuscarPorCargaCte(int codigoCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe>();
            query = from obj in query where obj.CTe.Codigo == codigoCargaCte select obj;
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe>();
            query = from obj in query where obj.CTe.Carga.Codigo == codigoCarga select obj;
            return query.ToList();
        }

        #endregion
    }
}
