using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class RotaFreteAbastecimentoPreAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento>
    {
        public RotaFreteAbastecimentoPreAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento> BuscarPorRotaFreteAbastecimento(int codigoRotaFreteAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimentoPreAbastecimento>();

            query = query.Where(o => o.RotaFreteAbastecimento.Codigo == codigoRotaFreteAbastecimento);

            return query.ToList();
        }

    }
}