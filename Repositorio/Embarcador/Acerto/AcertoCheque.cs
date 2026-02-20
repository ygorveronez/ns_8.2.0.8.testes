using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoCheque : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoCheque>
    {
        public AcertoCheque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCheque> BuscarPorAcerto(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCheque>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.ToList();
        }
        public Dominio.Entidades.Embarcador.Acerto.AcertoCheque BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCheque>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Acerto.AcertoCheque BuscarPorCodigoChequeeAcerto(int codigoCheque, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCheque>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Cheque.Codigo == codigoCheque select obj;
            return result.FirstOrDefault();
        }
    }
}
