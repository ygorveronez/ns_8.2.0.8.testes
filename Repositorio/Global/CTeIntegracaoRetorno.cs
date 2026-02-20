using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class CTeIntegracaoRetorno : RepositorioBase<Dominio.Entidades.CTeIntegracaoRetorno>, Dominio.Interfaces.Repositorios.CTeIntegracaoRetorno
    {
        public CTeIntegracaoRetorno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CTeIntegracaoRetorno BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeIntegracaoRetorno>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(o => o.CTe).ThenFetch(o => o.Empresa).FirstOrDefault();
        }

        public List<Dominio.Entidades.CTeIntegracaoRetorno> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeIntegracaoRetorno>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public List<int> BuscarPendentes(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeIntegracaoRetorno>();
            var result = from obj in query where (obj.CTe.Status == "A" || obj.CTe.Status == "R" || obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "F")  && (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando  || (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha && obj.NumeroTentativas < 3)) select obj;
            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public List<int> BuscarPendentesPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeIntegracaoRetorno>();
            var result = from obj in query where obj.TipoIntegracao.Tipo == tipoIntegracao && (obj.CTe.Status == "A" || obj.CTe.Status == "R" || obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "F") && (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando || (obj.SituacaoIntegracao == Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha && obj.NumeroTentativas < 3)) select obj;
            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.CTeIntegracaoRetorno BuscarUltipoPorPorCTeTipo(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeIntegracaoRetorno>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

    }
}
