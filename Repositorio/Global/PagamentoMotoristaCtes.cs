using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PagamentoMotoristaCtes : RepositorioBase<Dominio.Entidades.PagamentoMotoristaCtes>, Dominio.Interfaces.Repositorios.PagamentoMotoristaCtes
    {
        public PagamentoMotoristaCtes(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PagamentoMotoristaCtes BuscarPorCodigo(int codigo, int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaCtes>();

            var result = from obj in query where obj.ConhecimentoDeTransporteEletronico.Codigo == codigo && obj.PagamentoMotorista.Codigo == codigoPagamento select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PagamentoMotoristaCtes> BuscarPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PagamentoMotoristaCtes>();

            var result = from obj in query where obj.PagamentoMotorista.Codigo == codigoPagamento orderby obj.ConhecimentoDeTransporteEletronico.Numero select obj;

            return result.ToList();
        }
    }
}
