using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class AprovacaoAlcadaControleReajusteFretePlanilha : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>
    {
        #region Construtores

        public AprovacaoAlcadaControleReajusteFretePlanilha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaControleReajusteFretePlanilhaAprovacao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>();

            var resultAutorizacao = from obj in queryAutorizacao select obj.ControleReajusteFretePlanilha;
            var result = from obj in query where resultAutorizacao.Contains(obj) select obj;

            if (filtrosPesquisa.Numero > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.Situacao.HasValue)
                result = result.Where(obj => obj.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.CodigoTipoOperacao> 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoFilial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoUsuario > 0)
                result = result.Where(obj => obj.ControleAutorizacoes.Any(aut => aut.Usuario.Codigo == filtrosPesquisa.CodigoUsuario));

            bool situacaoPendentes = filtrosPesquisa.Situacao == SituacaoControleReajusteFretePlanilha.AgAprovacao;
            SituacaoAlcadaRegra situacaoAutorizacaoPendente = SituacaoAlcadaRegra.Pendente;

            if (situacaoPendentes)
                result = result.Where(obj =>
                    obj.ControleAutorizacoes.Any(aut =>
                        filtrosPesquisa.CodigoUsuario > 0 ? (aut.Usuario.Codigo == filtrosPesquisa.CodigoUsuario && aut.Situacao == situacaoAutorizacaoPendente) : (aut.Situacao == situacaoAutorizacaoPendente)
                    )
                );

            return result;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public int BuscarNumeroAprovacoesNecessariasPorRegra(int codigoControle, int codigoRegra)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao => (aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle));

            int numeroAprovacoesNecessarias = aprovacoes
                .Where(aprovacao => aprovacao.RegraControleReajusteFretePlanilha.Codigo == codigoRegra)
                .Select(aprovacao => aprovacao.NumeroAprovadores)
                .FirstOrDefault();

            return numeroAprovacoesNecessarias;
        }

        public List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> BuscarPendentes(int codigoControle, int codigoUsuario)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao =>
                    aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle &&
                    aprovacao.Usuario.Codigo == codigoUsuario &&
                    aprovacao.Situacao == SituacaoAlcadaRegra.Pendente
                );

            return aprovacoes.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha BuscarPorCodigo(int codigo)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao => (aprovacao.Codigo == codigo));

            return aprovacoes.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> BuscarPorControleEUsuario(int codigoControle, int codigoUsuario)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao => aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle);

            if (codigoUsuario > 0)
                aprovacoes = aprovacoes.Where(o => o.Usuario.Codigo == codigoUsuario);

            return aprovacoes.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> BuscarRegras(int codigoControle)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao => (aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle));

            var regras = aprovacoes
                .Where(aprovacao => aprovacao.RegraControleReajusteFretePlanilha != null)
                .Select(aprovacao => aprovacao.RegraControleReajusteFretePlanilha)
                .Distinct()
                .ToList();

            return regras;
        }

        public List<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaControleReajusteFretePlanilhaAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaControleReajusteFretePlanilha = Consultar(filtrosPesquisa);

            return ObterLista(consultaControleReajusteFretePlanilha, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> ConsultarAutorizacoesPorControle(int codigoControle, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao => aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle);

            return ObterLista(aprovacoes, parametrosConsulta);
        }

        public int ContarAprovacoes(int codigoControle)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao =>
                    (aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle) &&
                    (aprovacao.Situacao == SituacaoAlcadaRegra.Aprovada)
                );

            return aprovacoes.Count();
        }

        public int ContarAprovacoes(int codigoControle, int codigoRegra)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao =>
                    (aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle) &&
                    ((aprovacao.RegraControleReajusteFretePlanilha == null) || (aprovacao.RegraControleReajusteFretePlanilha.Codigo == codigoRegra)) &&
                    (aprovacao.Situacao == SituacaoAlcadaRegra.Aprovada)
                );

            return aprovacoes.Count();
        }

        public int ContarAprovacoesNecessarias(int codigoControle)
        {
            var regrasAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(o => (o.ControleReajusteFretePlanilha.Codigo == codigoControle) && (o.RegraControleReajusteFretePlanilha != null))
                .Select(aprovacao => new { aprovacao.RegraControleReajusteFretePlanilha, aprovacao.NumeroAprovadores })
                .Distinct()
                .ToList();

            int numeroAprovacoesNecessarias = regrasAutorizacao.Sum(o => o.NumeroAprovadores);

            return numeroAprovacoesNecessarias;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaControleReajusteFretePlanilhaAprovacao filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public int ContarConsultaAutorizacoesPorControle(int codigoControle)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao => aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle);

            return aprovacoes.Count();
        }

        public int ContarReprovacoes(int codigoControle)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao =>
                    (aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle) &&
                    (aprovacao.Situacao == SituacaoAlcadaRegra.Rejeitada)
                );

            return aprovacoes.Count();
        }

        public int ContarReprovacoes(int codigoControle, int codigoRegra)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>()
                .Where(aprovacao =>
                    (aprovacao.ControleReajusteFretePlanilha.Codigo == codigoControle) &&
                    ((aprovacao.RegraControleReajusteFretePlanilha == null) || (aprovacao.RegraControleReajusteFretePlanilha.Codigo == codigoRegra)) &&
                    (aprovacao.Situacao == SituacaoAlcadaRegra.Rejeitada)
                );

            return aprovacoes.Count();
        }

        #endregion Métodos Públicos
    }
}
