using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class RegrasNFSManualFilial : RepositorioBase<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual>
    {
        public RegrasNFSManualFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual>();
            var result = from obj in query where obj.RegrasAutorizacaoNFSManual.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}