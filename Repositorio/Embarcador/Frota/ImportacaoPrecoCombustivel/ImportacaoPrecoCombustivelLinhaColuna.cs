using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota.ImportacaoPrecoCombustivel
{
    public class ImportacaoPrecoCombustivelLinhaColuna : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna>
    {
        public ImportacaoPrecoCombustivelLinhaColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> BuscarPorImportacao(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna>();

            query = query.Where(o => o.Linha.ImportacaoPrecoCombustivel.Codigo == codigoImportacaoPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> BuscarPorImportacaoPendentesGeracaoPostoCombustivelTabelaValores(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna>();

            query = query.Where(o => o.Linha.ImportacaoPrecoCombustivel.Codigo == codigoImportacaoPedido && o.Linha.PostoCombustivelTabelaValores == null);

            return query.Fetch(o => o.Linha).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> BuscarPorLinha(int codigoLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPrecoCombustivel.ImportacaoPrecoCombustivelLinhaColuna>();

            query = query.Where(o => o.Linha.Codigo == codigoLinha);

            return query.Fetch(o => o.Linha).ToList();
        }
    }
}
