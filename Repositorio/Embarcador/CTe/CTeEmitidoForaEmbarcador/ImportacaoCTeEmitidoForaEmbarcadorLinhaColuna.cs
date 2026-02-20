using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;


namespace Repositorio.Embarcador.CTe
{
    public class ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna : RepositorioBase<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna>
    {
        public ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> BuscarPorImportacaoPendentesGeracaoCTeEmitidoForaEmbarcador(int codigoimportacaoCTeEmitidoForaEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna>();

            query = query.Where(o => o.Linha.ImportacaoCTeEmitidoForaEmbarcador.Codigo == codigoimportacaoCTeEmitidoForaEmbarcador && o.Linha.CTeEmitidoForaEmbarcador == null);

            return query.Fetch(o => o.Linha).ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> BuscarPorLinha(int codigoLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinhaColuna>();

            query = query.Where(o => o.Linha.Codigo == codigoLinha);

            return query.Fetch(o => o.Linha).ToList();
        }
    }
}
