using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public sealed class LicitacaoParticipacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao>
    {
        #region Construtores

        public LicitacaoParticipacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> ConsultarAprovacao(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacaoParticipacao filtrosPesquisa)
        {
            var consultaLicitacaoParticipacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao>();

            if (filtrosPesquisa.CodigoTabelaFrete > 0)
                consultaLicitacaoParticipacao = consultaLicitacaoParticipacao.Where(o => o.Licitacao.TabelaFrete.Codigo == filtrosPesquisa.CodigoTabelaFrete);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoLicitacao))
                consultaLicitacaoParticipacao = consultaLicitacaoParticipacao.Where(o => o.Licitacao.Descricao.Contains(filtrosPesquisa.DescricaoLicitacao));

            if (filtrosPesquisa.NumeroLicitacao > 0)
                consultaLicitacaoParticipacao = consultaLicitacaoParticipacao.Where(o => o.Licitacao.Numero == filtrosPesquisa.NumeroLicitacao);

            if (filtrosPesquisa.NumeroLicitacaoParticipacao > 0)
                consultaLicitacaoParticipacao = consultaLicitacaoParticipacao.Where(o => o.Numero == filtrosPesquisa.NumeroLicitacaoParticipacao);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaLicitacaoParticipacao = consultaLicitacaoParticipacao.Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaLicitacaoParticipacao = consultaLicitacaoParticipacao.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);
            else
            {

                consultaLicitacaoParticipacao = consultaLicitacaoParticipacao.Where(o => (
                    (o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLicitacaoParticipacao.AguardandoOferta) &&
                    (o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLicitacaoParticipacao.Cancelada)
                ));
            }

            return consultaLicitacaoParticipacao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao BuscarPorLicitacaoETransportador(int codigoLicitacao, int codigoTransportador)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao>()
                .Where(participacao => (participacao.Licitacao.Codigo == codigoLicitacao) && (participacao.Transportador.Codigo == codigoTransportador))
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao>  BuscarPorLicitacao(int codigoLicitacao)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao>()
                .Where(participacao => participacao.Licitacao.Codigo == codigoLicitacao && participacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLicitacaoParticipacao.Cancelada)
                .ToList();
        }

        public int BuscarProximoNumero()
        {
            var consultaLicitacaoParticipacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao>();
            int? ultimoNumero = consultaLicitacaoParticipacao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao> ConsultarAprovacao(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacaoParticipacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta paramentrosConsulta)
        {
            var consultaLicitacaoParticipacao = ConsultarAprovacao(filtrosPesquisa);

            return ObterLista(consultaLicitacaoParticipacao, paramentrosConsulta);
        }

        public int ContarConsultaAprovacao(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacaoParticipacao filtrosPesquisa)
        {
            var consultaLicitacaoParticipacao = ConsultarAprovacao(filtrosPesquisa);

            return consultaLicitacaoParticipacao.Count();
        }

        #endregion
    }
}
