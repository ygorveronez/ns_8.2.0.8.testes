using NHibernate;
using NHibernate.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public sealed class CarregamentoSolicitacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao>
    {
        #region Construtores

        public CarregamentoSolicitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao BuscarPendentePorCarregamento(int codigoCarregamento)
        {
            List<SituacaoCarregamentoSolicitacao> situacoesPendentes = SituacaoCarregamentoSolicitacaoHelper.ObterSituacoesPendentes();

            var consultaCarregamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento && situacoesPendentes.Contains(o.Situacao))
                .OrderByDescending(o => o.Numero);

            return consultaCarregamentoSolicitacao.FirstOrDefault();
        }

        public int BuscarProximoNumeroPorCarregamento(int codigoCarregamento)
        {
            var consultaCarregamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento);

            int? ultimoNumero = consultaCarregamentoSolicitacao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? ultimoNumero.Value + 1 : 1;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao> BuscarSemRegraAprovacaoPorCodigos(List<int> codigosCarregamentoSolicitacao)
        {
            var consultaCarregamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao>()
                .Where(o => codigosCarregamentoSolicitacao.Contains(o.Codigo) && o.Situacao == SituacaoCarregamentoSolicitacao.SemRegraAprovacao);

            return consultaCarregamentoSolicitacao
                .Fetch(o => o.Carregamento)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao BuscarUltimaPorCarregamento(int codigoCarregamento)
        {
            var consultaCarregamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento);

            return consultaCarregamentoSolicitacao
                .OrderByDescending(o => o.Numero)
                .FirstOrDefault();
        }

        #endregion
    }
}
