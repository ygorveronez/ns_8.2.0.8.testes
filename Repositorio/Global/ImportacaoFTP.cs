using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class ImportacaoFTP : RepositorioBase<Dominio.Entidades.ImportacaoFTP>, Dominio.Interfaces.Repositorios.ImportacaoFTP
    {
        public ImportacaoFTP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.ImportacaoFTP> BuscarPorStatus(Dominio.Enumeradores.StatusImportacaoFTP status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImportacaoFTP>();

            var result = from obj in query where obj.Status == status select obj;

            return result.Take(100).ToList();
        }

    }
}
