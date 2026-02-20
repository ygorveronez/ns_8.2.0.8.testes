using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class TabelaComissaoMotoristaTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao>
    {
        public TabelaComissaoMotoristaTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
