using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class CodigosServicoNFSe : RepositorioBase<Dominio.Entidades.CodigosServicoNFSe>
    {
        public CodigosServicoNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.CodigosServicoNFSe> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CodigosServicoNFSe>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }
    }
}