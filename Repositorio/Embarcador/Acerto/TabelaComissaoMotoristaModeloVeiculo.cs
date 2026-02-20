using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class TabelaComissaoMotoristaModeloVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo>
    {
        public TabelaComissaoMotoristaModeloVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}