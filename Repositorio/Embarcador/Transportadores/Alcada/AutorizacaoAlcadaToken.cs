using System.Collections.Generic;
using System.Linq;


namespace Repositorio.Embarcador.Transportadores.Alcada
{
    public class AutorizacaoAlcadaToken : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken,
        Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken,
        Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken
    >
    {
        #region Construtores

        public AutorizacaoAlcadaToken(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados 

        public IQueryable<Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa)
        {
            var consultaSolicitacaoToken = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.NumeroProtocolo > 0)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.OrigemAprovacao.NumeroProtocolo == filtrosPesquisa.NumeroProtocolo);

            return consultaSolicitacaoToken;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSolicitacaoToken = Consultar(filtrosPesquisa);

            return ObterLista(consultaSolicitacaoToken, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa)
        {
            var consultaSolicitacaoToken = Consultar(filtrosPesquisa);

            return consultaSolicitacaoToken.Count();
        }

        #endregion 
    }
}
