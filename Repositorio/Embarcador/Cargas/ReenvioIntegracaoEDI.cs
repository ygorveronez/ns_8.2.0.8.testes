using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class ReenvioIntegracaoEDI : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI>
    {

        public ReenvioIntegracaoEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI> _Consultar(int layout, int usuario, string carga, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI>();

            var result = from obj in query select obj;

            // Filtros
            if (layout > 0)
                result = result.Where(o => o.Layouts.Any(l => l.Codigo == layout));

            if (!string.IsNullOrEmpty(carga))
                result = result.Where(o => o.Cargas.Any(c => c.CodigoCargaEmbarcador == carga));

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio.Date <= dataFinal);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI> Consultar(int layout, int usuario, string carga, DateTime dataInicial, DateTime dataFinal, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(layout, usuario, carga, dataInicial, dataFinal);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int layout, int usuario, string carga, DateTime dataInicial, DateTime dataFinal)
        {
            var result = _Consultar(layout, usuario, carga, dataInicial, dataFinal);

            return result.Count();
        }
    }
}
