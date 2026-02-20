using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class IntegracaoVeiculo : RepositorioBase<Dominio.Entidades.IntegracaoVeiculo>
    {
        public IntegracaoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.IntegracaoVeiculo BuscarPorCodigo(int codigo, int codigoCCe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.IntegracaoVeiculo> BuscarPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoVeiculo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }
    }
}
