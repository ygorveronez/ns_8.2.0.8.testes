using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class RotaFreteVeiculo : RepositorioBase<Dominio.Entidades.RotaFreteVeiculo>
    {
        public RotaFreteVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RotaFreteVeiculo BuscarPorCodigo(int codigo)
        {
            var consultaRotaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteVeiculo>()
                .Where(o => o.Codigo == codigo);

            return consultaRotaFreteVeiculo.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFreteVeiculo> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            var consultaRotaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteVeiculo>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return consultaRotaFreteVeiculo.ToList();
        }
        
        public List<Dominio.Entidades.RotaFreteVeiculo> BuscarPorRotaFrete(List<int> rotas)
        {
            var consultaRotaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteVeiculo>()
                .Where(o => rotas.Contains(o.RotaFrete.Codigo));

            return consultaRotaFreteVeiculo.ToList();
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosPorRotaFrete(int codigoRotaFrete)
        {
            var consultaRotaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteVeiculo>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return consultaRotaFreteVeiculo.Select(o => o.Veiculo).ToList();
        }

    }
}
