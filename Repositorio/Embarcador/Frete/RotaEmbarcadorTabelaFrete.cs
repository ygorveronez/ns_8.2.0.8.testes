using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class RotaEmbarcadorTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete>
    {
        public RotaEmbarcadorTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete> BuscarPorTabelaFrete(int tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete select obj;

            return result
                .Fetch(obj => obj.RotaFrete)
                .Fetch(obj => obj.ComponenteFrete)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete BuscarPorRotaFixa(int codigoRota, int codigoTabela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RotaEmbarcadorTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabela && obj.RotaFrete.Codigo == codigoRota && obj.ValorAdicionalFixoPorRota select obj;

            return result.FirstOrDefault();
        }

    }
}
