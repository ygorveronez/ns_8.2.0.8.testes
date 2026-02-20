using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.NFS
{
    public class NFSManualIntegracaoAvon : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon>
    {
        public NFSManualIntegracaoAvon(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon BuscarPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon BuscarPorLancamentoNFSManual(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && obj.Situacao == situacao select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon> Consultar(int codigoLancamentoNFSManual, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual select obj;

            return result.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoAvon>();

            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual select obj;

            return result.Count();
        }
    }
}
