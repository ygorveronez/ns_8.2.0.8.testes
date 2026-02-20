using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NFS
{
    public class NFSManualIntegracaoNatura : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoNatura>
    {
        public NFSManualIntegracaoNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoNatura> BuscarPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoNatura>();
            query = query.Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);
            return query.ToList();
        }
    }
}
