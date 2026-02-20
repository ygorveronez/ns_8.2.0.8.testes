using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class ImportacaoPedagioLinha : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha>
    {
        public ImportacaoPedagioLinha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<int> BuscarCodigosLinhasPendentesGeracaoPedagio(int codigoImportacaoPedagio)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha>();

            query = query.Where(o => o.ImportacaoPedagio.Codigo == codigoImportacaoPedagio && o.Pedagio == null);
            
            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha> BuscarPorImportacaoPedagio(int codigoImportacaoPedagio)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha>();
            
            query = query.Where(o => o.ImportacaoPedagio.Codigo == codigoImportacaoPedagio);

            return query
                .Fetch(o => o.Pedagio)
                .OrderBy(o => o.Numero).ToList();
        }

        public int ContarPedagiosPorImportacaoPrecoCombustivel(int codigoImportacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha>();

            query = query.Where(o => o.ImportacaoPedagio.Codigo == codigoImportacao && o.Pedagio != null);
            
            return query
                .Select(o => o.Pedagio.Codigo)
                .Distinct()
                .Count();
        }
    }
}
