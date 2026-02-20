using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira
{
    public class ConsultaAbastecimentoAngellira : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira>
    {
        public ConsultaAbastecimentoAngellira(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.ConsultaAbastecimentoAngelLira>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }
    }
}
