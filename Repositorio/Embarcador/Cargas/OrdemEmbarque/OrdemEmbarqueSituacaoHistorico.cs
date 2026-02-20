using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.OrdemEmbarque
{
    public class OrdemEmbarqueSituacaoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico>
    {
        #region Construtores

        public OrdemEmbarqueSituacaoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public OrdemEmbarqueSituacaoHistorico(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico> BuscarPorCarga(int codigoCarga)
        {
            var consultaOrdemEmbarqueSituacaoHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico>()
                .Where(o => o.OrdemEmbarque.Carga.Codigo == codigoCarga || o.OrdemEmbarque.Carga.CargaAgrupamento.Codigo == codigoCarga);

            return consultaOrdemEmbarqueSituacaoHistorico
                .Fetch(o => o.OrdemEmbarque)
                .Fetch(o => o.Situacao)
                .ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico>> BuscarPorCargaAsync(int codigoCarga)
        {
            var consultaOrdemEmbarqueSituacaoHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico>()
                .Where(o => o.OrdemEmbarque.Carga.Codigo == codigoCarga || o.OrdemEmbarque.Carga.CargaAgrupamento.Codigo == codigoCarga);

            return await consultaOrdemEmbarqueSituacaoHistorico
                .Fetch(o => o.OrdemEmbarque)
                .Fetch(o => o.Situacao)
                .ToListAsync();
        }

        #endregion
    }
}
