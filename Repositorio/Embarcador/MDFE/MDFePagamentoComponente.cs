using Dominio.ObjetosDeValor.Enumerador;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.MDFE
{
    public class MDFePagamentoComponente : RepositorioBase<Dominio.Entidades.MDFePagamentoComponente>
    {
        #region Construtores

        public MDFePagamentoComponente(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MDFePagamentoComponente(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Metodos Públicos

        public List<Dominio.Entidades.MDFePagamentoComponente> BuscarPorInformacoesBancarias(int codigoInformacaoBancaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFePagamentoComponente>();

            query = query.Where(o => o.InformacoesBancarias.Codigo == codigoInformacaoBancaria);

            return query.ToList();
        }

        public Dominio.Entidades.MDFePagamentoComponente BuscarPrimeiroPorInformacoesBancarias(int codigoInformacaoBancaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFePagamentoComponente>();

            query = query.Where(o => o.InformacoesBancarias.Codigo == codigoInformacaoBancaria);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.MDFePagamentoComponente BuscarPorInformacoesBancariasETipoComponente(int codigoInformacaoBancaria, TipoComponentePagamento? tipoComponentePagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFePagamentoComponente>();

            query = query.Where(o => o.InformacoesBancarias.Codigo == codigoInformacaoBancaria && o.TipoComponente == tipoComponentePagamento);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
