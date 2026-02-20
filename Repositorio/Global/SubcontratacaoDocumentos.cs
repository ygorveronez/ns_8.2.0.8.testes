using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class SubcontratacaoDocumentos : RepositorioBase<Dominio.Entidades.SubcontratacaoDocumentos>
    {
        public SubcontratacaoDocumentos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.SubcontratacaoDocumentos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SubcontratacaoDocumentos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.SubcontratacaoDocumentos> BuscarPorSubcontratacao(int codigoSubcontratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SubcontratacaoDocumentos>();
            var result = from obj in query where obj.Subcontratacao.Codigo == codigoSubcontratacao select obj;
            return result.ToList();
        }

        public Dominio.Entidades.SubcontratacaoDocumentos BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SubcontratacaoDocumentos>();
            var result = from obj in query where obj.Documento.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public int ContarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SubcontratacaoDocumentos>();
            var result = from obj in query where obj.Documento.Codigo == codigoCTe select obj;
            return result.Count();
        }
    }
}
