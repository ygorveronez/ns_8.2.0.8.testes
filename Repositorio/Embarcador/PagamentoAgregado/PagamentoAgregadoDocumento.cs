using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoAgregado
{
    public class PagamentoAgregadoDocumento : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento>
    {
        public PagamentoAgregadoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado BuscarPorContratoEmAberto(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigoContrato && obj.PagamentoAgregado.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Iniciada select obj;
            return result.Select(c => c.PagamentoAgregado).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento>();
            var result = from obj in query where obj.StatusPagamentoAgregado == status select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> BuscarPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return result
                .Fetch(o => o.ConhecimentoDeTransporteEletronico)
                .ThenFetch(o => o.Remetente)
                .Fetch(o => o.ConhecimentoDeTransporteEletronico)
                .ThenFetch(o => o.Destinatario)
                .Fetch(o => o.ConhecimentoDeTransporteEletronico)
                .ThenFetch(o => o.Motoristas)
                .ToList();
        }

        public List<int> BuscarCodigosPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return result.Select(obj => obj.Codigo).ToList();
        }
    }
}
