using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class HistoricoNotaFiscalChamado : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado>
    {
        public HistoricoNotaFiscalChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado> BuscarPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado>();
            var result = query.Where(x => x.Chamado.Codigo == codigoChamado);

            return result.ToList();
        }
        public Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado BuscarPrimeiroRegistroNotaPorCodigoEChamado(int codigoChamado, int codigoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado>();
            var result = query.Where(x => x.Chamado.Codigo == codigoChamado && x.XMLNotaFiscal.Codigo == codigoNotaFiscal);

            return  result.FirstOrDefault();
        }
        #endregion
    }
}