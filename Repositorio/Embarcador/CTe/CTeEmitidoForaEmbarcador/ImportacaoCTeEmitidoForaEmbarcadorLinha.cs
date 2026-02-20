using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class ImportacaoCTeEmitidoForaEmbarcadorLinha : RepositorioBase<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha>
    {
        public ImportacaoCTeEmitidoForaEmbarcadorLinha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<int> BuscarCodigosLinhasPendentesGeracaoImportacaoCTeEmitidoForaEmbarcador(int codigoImportacaoCTeEmitidoForaEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha>();

            query = query.Where(o => o.ImportacaoCTeEmitidoForaEmbarcador.Codigo == codigoImportacaoCTeEmitidoForaEmbarcador && o.CTeEmitidoForaEmbarcador == null);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha> BuscarPorImportacaoCTeEmitidoForaEmbarcador(int codigoImportacaoCTeEmitidoForaEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcadorLinha>();
            query = query.Where(o => o.ImportacaoCTeEmitidoForaEmbarcador.Codigo == codigoImportacaoCTeEmitidoForaEmbarcador);
            return query.ToList();
        }
    }
}
