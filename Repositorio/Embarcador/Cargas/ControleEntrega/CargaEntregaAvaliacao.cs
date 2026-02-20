using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaAvaliacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao>
    {
        public CargaEntregaAvaliacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao> BuscarPorControleEntrega(int codigoControleEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao>();

            var result = from obj in query where obj.CargaEntrega.Codigo == codigoControleEntrega select obj;

            return result.ToList();
        }
    }
}
