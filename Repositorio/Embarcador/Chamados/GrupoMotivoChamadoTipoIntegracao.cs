using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class GrupoMotivoChamadoTipoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao>
    {
        public GrupoMotivoChamadoTipoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao> BuscarPorGrupoMotivo(int grupoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao>();

            query = query.Where(x => x.GrupoMotivoChamado.Codigo == grupoMotivoChamado);

            return query.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposIntegracoesPorGrupo(int grupoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao>();
            query = query.Where(x => x.GrupoMotivoChamado.Codigo == grupoMotivoChamado);
            return query.Select(x => x.TipoIntegracao).ToList();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
