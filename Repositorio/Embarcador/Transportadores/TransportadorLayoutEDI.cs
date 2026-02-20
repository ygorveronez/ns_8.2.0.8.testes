using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorLayoutEDI : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>
    {
        public TransportadorLayoutEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> BuscarPorEmpresa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            var result = from obj in query where obj.Empresa.Codigo == codigo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> BuscarPorTipoLayoutEDI(Dominio.Enumeradores.TipoLayoutEDI tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            var result = from obj in query where obj.LayoutEDI.Tipo == tipo select obj;

            return result.ToList();
        }

        #endregion
    }
}
