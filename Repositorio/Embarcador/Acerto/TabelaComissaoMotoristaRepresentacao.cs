using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class TabelaComissaoMotoristaRepresentacao : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao>
    {
        public TabelaComissaoMotoristaRepresentacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao> BuscarPorTabela(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao>();
            var result = from obj in query where obj.TabelaComissaoMotorista.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}