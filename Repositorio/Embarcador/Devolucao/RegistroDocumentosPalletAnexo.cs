using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Devolucao
{
    public class RegistroDocumentosPalletAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo>
    {
        public RegistroDocumentosPalletAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo> BuscarPorCodigoGestaoDevolucao(long codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigo);
            return query.ToList();
        }
    }
}
