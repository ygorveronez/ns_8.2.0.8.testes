using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class DadosRecebedor : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor>
    {
        public DadosRecebedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor> _Consultar(string nome, string cPF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cPF))
                result = result.Where(o => o.CPF.Contains(cPF));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor> Consultar(string nome, string cPF, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(nome, cPF);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(string nome, string cPF)
        {
            var result = _Consultar(nome, cPF);
            return result.Count();
        }
    }

}