using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.OrdemEmbarque
{
    public class OrdemEmbarque : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>
    {
        #region Construtores

        public OrdemEmbarque(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public OrdemEmbarque(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> BuscarPorCarga(int codigoCarga)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o => o.Carga.Codigo == codigoCarga || o.Carga.CargaAgrupamento.Codigo == codigoCarga);

            return consultaOrdemEmbarque
                .Fetch(o => o.Situacao)
                .ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>> BuscarPorCargaAsync(int codigoCarga)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o => o.Carga.Codigo == codigoCarga || o.Carga.CargaAgrupamento.Codigo == codigoCarga);

            return await consultaOrdemEmbarque
                .Fetch(o => o.Situacao)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> BuscarAtivasPorCarga(int codigoCarga)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o => (o.Carga.Codigo == codigoCarga || o.Carga.CargaAgrupamento.Codigo == codigoCarga) && (o.Situacao == null || !ObterCodigosIntegracaoSituacaoOrdemCancelada().Contains(o.Situacao.CodigoIntegracao)));

            return consultaOrdemEmbarque
                .Fetch(o => o.Situacao)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque BuscarAtivaPorCargaENumeroReboque(int codigoCarga, int codigoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque numeroReboque)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o =>
                    (o.Carga.Codigo == codigoCarga || o.Carga.CargaAgrupamento.Codigo == codigoCarga) &&
                    o.Carga.Filial.Codigo == codigoFilial &&
                    o.NumeroReboque == numeroReboque &&
                    (o.Situacao == null || !ObterCodigosIntegracaoSituacaoOrdemCancelada().Contains(o.Situacao.CodigoIntegracao))
                );

            return consultaOrdemEmbarque
                .Fetch(o => o.Situacao)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque BuscarAtivaPorCargaENumero(int codigoCarga, string numero)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o => (o.Carga.Codigo == codigoCarga || o.Carga.CargaAgrupamento.Codigo == codigoCarga) && o.Numero == numero && (o.Situacao == null || !ObterCodigosIntegracaoSituacaoOrdemCancelada().Contains(o.Situacao.CodigoIntegracao)));

            return consultaOrdemEmbarque
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> BuscarPorCargas(List<int> codigosCarga)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo) || codigosCarga.Contains(o.Carga.CargaAgrupamento.Codigo));

            return consultaOrdemEmbarque
                .Fetch(o => o.Situacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> BuscarAtivasPorCargas(List<int> codigosCarga)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o => (codigosCarga.Contains(o.Carga.Codigo) || codigosCarga.Contains(o.Carga.CargaAgrupamento.Codigo)) && (o.Situacao == null || !ObterCodigosIntegracaoSituacaoOrdemCancelada().Contains(o.Situacao.CodigoIntegracao)));

            return consultaOrdemEmbarque
                .Fetch(o => o.Situacao)
                .ToList();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o => (o.Carga.Codigo == codigoCarga || o.Carga.CargaAgrupamento.Codigo == codigoCarga) && (o.Situacao == null || !ObterCodigosIntegracaoSituacaoOrdemCancelada().Contains(o.Situacao.CodigoIntegracao)));

            return consultaOrdemEmbarque.Count() > 0;
        }

        public bool ExistePorCargaESituacoes(int codigoCarga, List<string> situacoes)
        {
            var consultaOrdemEmbarque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque>()
                .Where(o => (o.Carga.Codigo == codigoCarga || o.Carga.CargaAgrupamento.Codigo == codigoCarga) && situacoes.Contains(o.Situacao.CodigoIntegracao));

            return consultaOrdemEmbarque.Count() > 0;
        }

        #endregion

        #region Métodos privados

        private List<string> ObterCodigosIntegracaoSituacaoOrdemCancelada()
        {
            OrdemEmbarqueSituacao repOrdemEmbarqueSituacao = new OrdemEmbarqueSituacao(UnitOfWork);
            return repOrdemEmbarqueSituacao.GetCodigosIntegracaoSituacaoOrdemCancelada();
        }

        #endregion

    }
}
