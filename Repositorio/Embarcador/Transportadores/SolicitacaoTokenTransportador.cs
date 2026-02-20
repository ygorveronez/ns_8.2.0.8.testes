using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class SolicitacaoTokenTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador>
    {
        #region Construtores

        public SolicitacaoTokenTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador> Consultar(int codigoSolicitacao)
        {
            var consultaSolicitacaoToken = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador>();

            if (codigoSolicitacao > 0)
                consultaSolicitacaoToken = consultaSolicitacaoToken.Where(x => x.SolicitacaoToken.Codigo == codigoSolicitacao);

            return consultaSolicitacaoToken;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador> Consultar(int codigoSolicitacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSolicitacaoToken = Consultar(codigoSolicitacao);

            return ObterLista(consultaSolicitacaoToken, parametrosConsulta);
        }
        
        public List<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador> BusporSolicitacao(int codigoSolicitacao)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.SolicitacaoTokenTransportador>();
            //query = from obj in query where obj.SolicitacaoToken.Codigo == codigoSolicitacao select obj;
            query = query.Where(x => x.SolicitacaoToken.Codigo == codigoSolicitacao);
            return query.ToList();

        }

        public int ContarConsulta(int codigoSolicitacao)
        {
            var consultaSolicitacaoToken = Consultar(codigoSolicitacao);

            return consultaSolicitacaoToken.Count();
        }       
        
        #endregion Métodos Públicos

       

    }
}
