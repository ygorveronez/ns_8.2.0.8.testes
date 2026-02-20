using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracaoCancelamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoIntegracao>
    {
        public LoteEscrituracaoCancelamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoIntegracao> BuscarPorLoteEscrituracao(int codigoLoteEscrituracaoCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento);

            return query.ToList();
        }

        public int ContarPorLoteEscrituracaoETipoIntegracao(int codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoIntegracao>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Select(o => o.Codigo).Count();
        }
    }
}
