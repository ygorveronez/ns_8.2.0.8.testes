using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Escrituracao
{
    public sealed class EstornoProvisaoSolicitacao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>
    {
        #region Construtores

        public EstornoProvisaoSolicitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao BuscarPorCodigo(int codigo)
        {
            List<SituacaoCargaCancelamentoSolicitacao> situacoesPendentes = SituacaoCargaCancelamentoSolicitacaoHelper.ObterSituacoesPendentes();

            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>()
                .Where(o => o.Codigo == codigo);

            return consulta.FirstOrDefault();
        }
        public List<(int, SituacaoEstornoProvisaoSolicitacao)> BuscarSituacaoesPorEstorno(List<int> codigosStornos)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>();
            consulta = from obj in consulta where codigosStornos.Contains(obj.EstornoProvisao.Codigo) select obj;
            return consulta.Select(x =>ValueTuple.Create(x.EstornoProvisao.Codigo,x.Situacao)).ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao BuscarPorCancelamentoProvisao(int codigo)
        {
            List<SituacaoCargaCancelamentoSolicitacao> situacoesPendentes = SituacaoCargaCancelamentoSolicitacaoHelper.ObterSituacoesPendentes();

            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>()
                .Where(o => o.EstornoProvisao.Codigo == codigo);

            return consulta.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao> BuscarSolicitacoesPorCancelamentoProvisao(int codigo)
        {
            List<SituacaoCargaCancelamentoSolicitacao> situacoesPendentes = SituacaoCargaCancelamentoSolicitacaoHelper.ObterSituacoesPendentes();

            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>()
                .Where(o => o.EstornoProvisao.Codigo == codigo);

            return consulta.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao BuscarPendentePorEstornoProvisao(int codigoEstorno)
        {
            List<SituacaoCargaCancelamentoSolicitacao> situacoesPendentes = SituacaoCargaCancelamentoSolicitacaoHelper.ObterSituacoesPendentes();

            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>()
                .Where(o => o.EstornoProvisao.Codigo == codigoEstorno );

            return consulta.FirstOrDefault();
        }
        public int BuscarProximoNumeroPorEstornoProvisao(int codigoEstornoProvisao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>()
                .Where(o => o.EstornoProvisao.Codigo == codigoEstornoProvisao);

            int? ultimoNumero = consulta.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? ultimoNumero.Value + 1 : 1;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao> BuscarSemRegraAprovacaoPorCodigos(List<int> codigos)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>()
                .Where(o => codigos.Contains(o.EstornoProvisao.Codigo));

            consulta = consulta.Where(obj => obj.Situacao == SituacaoEstornoProvisaoSolicitacao.SemRegraAprovacao);

            return consulta.ToList();
        }

        public bool ExisteRegraAprovacao()
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>();

            return consulta.Any();
        }


        #endregion
    }
}
