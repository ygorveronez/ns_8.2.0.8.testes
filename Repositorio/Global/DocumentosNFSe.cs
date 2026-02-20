using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class DocumentosNFSe : RepositorioBase<Dominio.Entidades.DocumentosNFSe>
    {
        public DocumentosNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DocumentosNFSe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosNFSe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentosNFSe> BuscarPorNFSe(int nfse)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == nfse select obj;

            return result.ToList();
        }

        public Dominio.Entidades.DocumentosNFSe BuscarPorNFSeENFe(int nfse, string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == nfse && obj.Chave.Equals(chaveNFe) select obj;

            return result.FirstOrDefault();
        }
    }
}
