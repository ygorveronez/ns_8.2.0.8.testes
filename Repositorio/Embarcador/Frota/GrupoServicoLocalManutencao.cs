using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class GrupoServicoLocalManutencao : RepositorioBase<Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao>
    {
        public GrupoServicoLocalManutencao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao> BuscarPorGrupoServico(int codigoGrupoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao>();

            query = query.Where(o => o.GrupoServico.Codigo == codigoGrupoServico);

            return query.ToList();
        }

        #endregion
    }
}