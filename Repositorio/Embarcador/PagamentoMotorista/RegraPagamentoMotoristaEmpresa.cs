using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class RegraPagamentoMotoristaEmpresa : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa>
    {
        public RegraPagamentoMotoristaEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa>();
            var result = from obj in query where obj.RegrasPagamentoMotorista.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
