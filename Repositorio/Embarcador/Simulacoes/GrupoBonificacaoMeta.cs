using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Simulacoes
{
    public class GrupoBonificacaoMeta : RepositorioBase<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta>
    {
        public GrupoBonificacaoMeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta> BuscarPorGrupoBonificacao(int codigoGrupoBonificacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta>();

            query = query.Where(o => o.GrupoBonificacao.Codigo == codigoGrupoBonificacao);

            return query.ToList();
        }

        #endregion
    }
}
