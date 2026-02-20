using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class SolicitacaoToken : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken>
    {
        #region Construtores

        public SolicitacaoToken(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa)
        {
            var consultaSolicitacaoToken = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken>();
            var aprovacaoAlcada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.Alcada.AprovacaoAlcadaAutorizacaoToken>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.NumeroProtocolo > 0)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.NumeroProtocolo == filtrosPesquisa.NumeroProtocolo);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.Transportadores.Any(t => t.Codigo == filtrosPesquisa.CodigoEmpresa));

            if (filtrosPesquisa.DataInicioVigencia != null && filtrosPesquisa.DataFimVigencia == null)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.DataInicioVigencia >= filtrosPesquisa.DataInicioVigencia);

            if (filtrosPesquisa.DataFimVigencia != null && filtrosPesquisa.DataInicioVigencia == null)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.DataFimVigencia <= filtrosPesquisa.DataFimVigencia);

            if (filtrosPesquisa.DataInicioVigencia != null && filtrosPesquisa.DataFimVigencia != null)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.DataInicioVigencia >= filtrosPesquisa.DataInicioVigencia && obj.DataFimVigencia <= filtrosPesquisa.DataFimVigencia);

            if (filtrosPesquisa.EtapaAutorizacao > 0)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.Situacao == filtrosPesquisa.EtapaAutorizacao);

            if (filtrosPesquisa.Situacao > 0)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => obj.SituacaoAutenticacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.Usuario > 0)
            {
                var codigos = aprovacaoAlcada.Where(obj => obj.RegraAutorizacao.Aprovadores.Any(ap => ap.Codigo == filtrosPesquisa.Usuario)).Select(obj => obj.OrigemAprovacao.Codigo).ToList();
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => codigos.Contains(obj.Codigo));
            }

            if(filtrosPesquisa.Prioridade < 99)
            {
                var codigos = aprovacaoAlcada.Where(obj => obj.RegraAutorizacao.PrioridadeAprovacao == filtrosPesquisa.Prioridade).Select(obj => obj.OrigemAprovacao.Codigo).ToList();
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(obj => codigos.Contains(obj.Codigo));
            }


            return consultaSolicitacaoToken;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSolicitacaoToken = Consultar(filtrosPesquisa);

            return ObterLista(consultaSolicitacaoToken, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaSolicitacaoToken filtrosPesquisa)
        {
            var consultaSolicitacaoToken = Consultar(filtrosPesquisa);

            return consultaSolicitacaoToken.Count();
        }

        public int ObterUltimoNumeroProtocolo()
        {
            var consultaSolicitacaoToken = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken>();

            return consultaSolicitacaoToken.Count() > 0 ? consultaSolicitacaoToken.Max(x => x.NumeroProtocolo) : 0;
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken> BuscarSolicitacoesEmLiberacao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken>();
            query = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoToken.EmLiberacaoSistematica select obj;

            return query.ToList();
        }
        #endregion Métodos Públicos
    }
}
