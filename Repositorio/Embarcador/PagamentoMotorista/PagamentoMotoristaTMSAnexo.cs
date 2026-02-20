using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class PagamentoMotoristaTMSAnexo : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo>
    {
        public PagamentoMotoristaTMSAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo> BuscarPorPagamentoMotorista(int codigoPagamentoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo>();
            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigoPagamentoMotorista select obj;
            return result.ToList();
        }
    }
}
