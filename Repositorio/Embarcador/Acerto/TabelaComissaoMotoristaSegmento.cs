using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class TabelaComissaoMotoristaSegmento : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento>
    {
        public TabelaComissaoMotoristaSegmento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}