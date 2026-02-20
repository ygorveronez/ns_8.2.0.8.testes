using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class MotivoChamadoData : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData>
    {
        public MotivoChamadoData(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData> BuscarAtivosPorMotivoChamado(int codigo)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData>()
                .Where(o => o.Status && o.MotivoChamado.Codigo == codigo)
                .OrderBy(o => o.Codigo)
                .ToList();

            return consulta;
        }

        #endregion
    }
}
