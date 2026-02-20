using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo>
    {
        #region Construtores

        public TabelaFreteVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo> BuscarPorTabela(int codigoTabelaFrete)
        {
            var consultaTabelaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo>()
                .Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete);

            return consultaTabelaFreteVeiculo
                .Fetch(obj => obj.Veiculo).ThenFetch(obj => obj.ModeloVeicularCarga)
                .Fetch(obj => obj.Veiculo).ThenFetch(obj => obj.Empresa)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo> BuscarPorTabelaETipoVeiculo(int codigoTabelaFrete, string tipoVeiculo)
        {
            var consultaTabelaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo>()
                .Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && o.Veiculo.TipoVeiculo == tipoVeiculo);

            return consultaTabelaFreteVeiculo.ToList();
        }

        public bool ExistePorTabelaEVeiculo(int codigoTabelaFrete, List<int> codigoVeiculos)
        {
            var consultaTabelaFreteVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteVeiculo>()
                .Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && codigoVeiculos.Contains(o.Veiculo.Codigo));

            return consultaTabelaFreteVeiculo.Any();
        }

        #endregion Métodos Públicos
    }
}
