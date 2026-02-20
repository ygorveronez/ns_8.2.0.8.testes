using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFreteCarga : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga>
    {
        public FechamentoFreteCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga> _Consultar(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga> Consultar(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(fechamento);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int fechamento)
        {
            var result = _Consultar(fechamento);

            return result.Count();
        }

        public int ConsultarDistanciaTotalPorFechamento(int fechamento)
        {
            var queryFechamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga>();
            var resultFechamento = from o in queryFechamento where o.Fechamento.Codigo == fechamento select o.Carga;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
            var result = from obj in query
                         where resultFechamento.Any(c => c.Codigo == obj.Carga.Codigo)
                         select obj;

            return result.Sum(obj => (int?)obj.DistanciaKM) ?? 0;
        }
    }
}
