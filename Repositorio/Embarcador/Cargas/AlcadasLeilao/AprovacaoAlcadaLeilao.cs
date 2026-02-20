using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas.AlcadasLeilao
{
    public class AprovacaoAlcadaLeilao : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao,
        Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao,
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento
    >
    {
        #region Construtores

        public AprovacaoAlcadaLeilao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaLeilaoAprovacao filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (filtrosPesquisa.SituacaoCotacao.HasValue)
            {
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.SituacaoCotacao == filtrosPesquisa.SituacaoCotacao.Value);

                if (filtrosPesquisa.SituacaoCotacao.Value == SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao)
                    return consultaCargaJanelaCarregamento;
            }

            var consultaAprovacaoAlcadaLeilao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.AprovacaoAlcadaLeilao>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAprovacaoAlcadaLeilao = consultaAprovacaoAlcadaLeilao.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao)
                consultaAprovacaoAlcadaLeilao = consultaAprovacaoAlcadaLeilao.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SituacaoCotacao.HasValue)
                return consultaCargaJanelaCarregamento.Where(o => consultaAprovacaoAlcadaLeilao.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaCargaJanelaCarregamento.Where(o =>
                o.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao ||
                consultaAprovacaoAlcadaLeilao.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarSemRegraAprovacaoPorCodigos(List<int> codigos)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => codigos.Contains(o.Codigo) && o.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.SemRegraAprovacao);

            return consultaNaoConformidade.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaLeilaoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaJanelaCarregamento = Consultar(filtrosPesquisa);

            consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento
                .Fetch(o => o.TransportadorCotacao).ThenFetch(o => o.Localidade);

            return ObterLista(consultaCargaJanelaCarregamento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaLeilaoAprovacao filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = Consultar(filtrosPesquisa);

            return consultaCargaJanelaCarregamento.Count();
        }

        #endregion Métodos Públicos
    }
}
