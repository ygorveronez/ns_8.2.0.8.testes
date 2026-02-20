using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class NFSeIntegracaoRetorno : RepositorioBase<Dominio.Entidades.NFSeIntegracaoRetorno>, Dominio.Interfaces.Repositorios.NFSeIntegracaoRetorno
    {
        public NFSeIntegracaoRetorno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NFSeIntegracaoRetorno BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeIntegracaoRetorno>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(o => o.NFSe).ThenFetch(o => o.Empresa).FirstOrDefault();
        }

        public List<Dominio.Entidades.NFSeIntegracaoRetorno> BuscarPorNFSe(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeIntegracaoRetorno>();
            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe select obj;
            return result.ToList();
        }

        public List<int> BuscarPendentes(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeIntegracaoRetorno>();
            var result = from obj in query where (obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado || obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Cancelado) && (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando || (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha && obj.NumeroTentativas < 3)) select obj;
            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public List<int> BuscarPendentesPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeIntegracaoRetorno>();
            var result = from obj in query where obj.TipoIntegracao.Tipo == tipoIntegracao && (obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado || obj.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Cancelado) && (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando || (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha && obj.NumeroTentativas < 3)) select obj;
            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.NFSeIntegracaoRetorno BuscarUltipoPorPorNFSeTipo(int codigoNFSe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeIntegracaoRetorno>();
            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

    }
}
