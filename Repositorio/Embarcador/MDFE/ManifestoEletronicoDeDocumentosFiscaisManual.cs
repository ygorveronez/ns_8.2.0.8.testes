using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.MDFE
{
    public class ManifestoEletronicoDeDocumentosFiscaisManual : RepositorioBase<Dominio.Entidades.Embarcador.MDFe.ManifestoEletronicoDeDocumentosFiscaisManual>
    {
        public ManifestoEletronicoDeDocumentosFiscaisManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.MDFe.ManifestoEletronicoDeDocumentosFiscaisManual BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.MDFe.ManifestoEletronicoDeDocumentosFiscaisManual>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
