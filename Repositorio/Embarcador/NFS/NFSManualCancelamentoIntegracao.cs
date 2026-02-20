using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NFS
{
    public class NFSManualCancelamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao>
    {
        public NFSManualCancelamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao> BuscarPorNFSManualCancelamento(int codigoNFSManualCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposPorNFSManualCancelamento(int codigoNFSManualCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento);

            return query.Select(obj => obj.TipoIntegracao.Tipo).ToList();
        }

        public int ContarPorNFSManualCancelamentoETipoIntegracao(int codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao>();

            query = query.Where(obj => obj.NFSManualCancelamento.Codigo == codigoNFSManualCancelamento && obj.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> BuscarNFSManuaisCanceladasProntasParaIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>()
                .Where(obj => obj.SituacaoNFSManualCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.Cancelada);

            var result = from obj in query select obj;

            return result.ToList();
        }
    }
}