using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.MDFE
{
    public class MDFePagamentoParcela : RepositorioBase<Dominio.Entidades.MDFePagamentoParcela>
    {
        #region Construtores
        public MDFePagamentoParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MDFePagamentoParcela(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #endregion

        #region Metodos Públicos

        public List<Dominio.Entidades.MDFePagamentoParcela> BuscarPorInformacoesBancarias(int codigoInformacaoBancaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFePagamentoParcela>();

            query = query.Where(o => o.InformacoesBancarias.Codigo == codigoInformacaoBancaria);

            return query.ToList();
        }

        public Dominio.Entidades.MDFePagamentoParcela BuscarPrimeiroPorInformacoesBancarias(int codigoInformacaoBancaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFePagamentoParcela>();

            query = query.Where(o => o.InformacoesBancarias.Codigo == codigoInformacaoBancaria);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.MDFePagamentoParcela BuscarPorInformacoesBancariasENumeroParcela(int codigoInformacaoBancaria, int numeroParcela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFePagamentoParcela>();
            query = query.Where(o => o.InformacoesBancarias.Codigo == codigoInformacaoBancaria && o.NumeroParcela == numeroParcela);
            return query.FirstOrDefault();
        }

        #endregion
    }
}
