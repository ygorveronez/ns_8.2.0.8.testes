using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoRotasFrete : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete>
    {
        public VeiculoRotasFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete> BuscarPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete>();

            query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete BuscarPorVeiculoERotaFrete(int codigoVeiculo, int codigoRotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete>();

            query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo && o.RotaFrete.Codigo == codigoRotaFrete);

            return query.FirstOrDefault();
        }

        public List<int> BuscarCodigosRotasFretesPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete>();

            query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            return query
                .Select(o => o.RotaFrete.Codigo)
                .ToList();
        }

        #endregion

    }
}