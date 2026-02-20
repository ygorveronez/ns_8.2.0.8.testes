using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Fechamento
{

    public sealed class FechamentoJustificativaAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto>
    {
        #region Construtores

        public FechamentoJustificativaAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto> Consultar(Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoJustificativaAcrescimoDesconto filtroPesquisa)
        {
            var consultaFechamentoJustificativaAcrescimoDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto>();
            consultaFechamentoJustificativaAcrescimoDesconto = from obj in consultaFechamentoJustificativaAcrescimoDesconto select obj;

            if (!string.IsNullOrEmpty(filtroPesquisa.Descricao))
                consultaFechamentoJustificativaAcrescimoDesconto = consultaFechamentoJustificativaAcrescimoDesconto.Where(obj => obj.Descricao.Contains(filtroPesquisa.Descricao));

            if (filtroPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaFechamentoJustificativaAcrescimoDesconto = consultaFechamentoJustificativaAcrescimoDesconto.Where(obj => !obj.Situacao);

            else if (filtroPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaFechamentoJustificativaAcrescimoDesconto = consultaFechamentoJustificativaAcrescimoDesconto.Where(obj => obj.Situacao);

            if (!filtroPesquisa.TipoJustificativa.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativaPesquisa.Todos))
                consultaFechamentoJustificativaAcrescimoDesconto = consultaFechamentoJustificativaAcrescimoDesconto.Where(obj => obj.TipoJustificativa == (filtroPesquisa.TipoJustificativa));

            return consultaFechamentoJustificativaAcrescimoDesconto;
        }

        #endregion

        #region Métodos Públicos

        public bool ExisteDuplicidade(Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto fechamentoJustificativaAcrescimoDesconto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto>();
            query = query.Where(i => i.Codigo != fechamentoJustificativaAcrescimoDesconto.Codigo);
            query = query.Where(i => i.Descricao.Equals(fechamentoJustificativaAcrescimoDesconto.Descricao));

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto> Consultar(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoJustificativaAcrescimoDesconto filtrosPesquisa)
        {
            var consultaFechamentoJustificativaAcrescimoDesconto = Consultar(filtrosPesquisa);

            return ObterLista(consultaFechamentoJustificativaAcrescimoDesconto, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoJustificativaAcrescimoDesconto filtrosPesquisa)
        {
            var consultaFechamentoJustificativaAcrescimoDesconto = Consultar(filtrosPesquisa);

            return consultaFechamentoJustificativaAcrescimoDesconto.Count();
        }
        #endregion
    }

}
