using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Simulacoes
{
    public class GrupoBonificacaoVigencia : RepositorioBase<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia>
    {
        public GrupoBonificacaoVigencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia> BuscarPorGrupoBonificacao(int codigoGrupoBonificacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia>();

            query = query.Where(o => o.GrupoBonificacao.Codigo == codigoGrupoBonificacao);

            return query.ToList();
        }

        #endregion
    }
}
