using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class ImportacaoPedagioLinhaColuna : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna>
    {
        public ImportacaoPedagioLinhaColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna> BuscarPorImportacaoPendentesGeracaoPedagio(int codigoImportacaoPedagio)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna>();

            query = query.Where(o => o.Linha.ImportacaoPedagio.Codigo == codigoImportacaoPedagio && o.Linha.Pedagio == null);

            return query
                .Fetch(o => o.Linha)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna> BuscarPorLinha(int codigoLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna>();

            query = query.Where(o => o.Linha.Codigo == codigoLinha);

            return query.Fetch(o => o.Linha).ToList();
        }
    }
}
