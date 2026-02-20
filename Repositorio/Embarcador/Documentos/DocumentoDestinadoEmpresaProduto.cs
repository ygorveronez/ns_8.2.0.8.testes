using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class DocumentoDestinadoEmpresaProduto : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto>
    {
        #region Construtores
        public DocumentoDestinadoEmpresaProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto> BuscarPorDocumento(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaProduto>();

            query = query.Where(o => o.DocumentoDestinadoEmpresa.Codigo == codigo);

            return query.ToList();
        }
    }
}
