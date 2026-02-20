using NHibernate;
using NHibernate.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class CargaCancelamentoSolicitacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>
    {
        #region Construtores

        public CargaCancelamentoSolicitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao BuscarPendentePorCargaCancelamento(int codigoCargaCancelamento)
        {
            List<SituacaoCargaCancelamentoSolicitacao> situacoesPendentes = SituacaoCargaCancelamentoSolicitacaoHelper.ObterSituacoesPendentes();

            var consultaCargaCancelamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>()
                .Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento && situacoesPendentes.Contains(o.Situacao))
                .OrderByDescending(o => o.Numero);

            return consultaCargaCancelamentoSolicitacao.FirstOrDefault();
        }

        public int BuscarProximoNumeroPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var consultaCargaCancelamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>()
                .Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento);

            int? ultimoNumero = consultaCargaCancelamentoSolicitacao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? ultimoNumero.Value + 1 : 1;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao> BuscarSemRegraAprovacaoPorCodigos(List<int> codigosCargaCancelamentoSolicitacao)
        {
            var consultaCargaCancelamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>()
                .Where(o => codigosCargaCancelamentoSolicitacao.Contains(o.Codigo) && o.Situacao == SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao);

            return consultaCargaCancelamentoSolicitacao
                .Fetch(o => o.CargaCancelamento)
                .ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao> BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaCancelamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>()
                .Where(o => o.CargaCancelamento.Carga.Codigo == codigoCarga);

            return consultaCargaCancelamentoSolicitacao.ToList();
        }

        public bool ExisteAprovadaPorCargaCancelamento(int codigoCargaCancelamento)
        {
            var consultaCargaCancelamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>()
                .Where(o => o.CargaCancelamento.Codigo == codigoCargaCancelamento && o.Situacao == SituacaoCargaCancelamentoSolicitacao.Aprovada);

            return consultaCargaCancelamentoSolicitacao.Count() > 0;
        }

        #endregion
    }
}
