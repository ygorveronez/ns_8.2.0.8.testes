using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class ParametroBaseCalculoTabelaFreteIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFreteIntegracao>
    {
        #region Constructores
        public ParametroBaseCalculoTabelaFreteIntegracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFreteIntegracao BuscarPorParametro(int codigoParametro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFreteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFreteIntegracao>();
            query = from obj in query where obj.ParametrosTabelaFrete.Codigo == codigoParametro select obj;
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFreteIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFreteIntegracao> consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFreteIntegracao>()
                .Where(obj =>
                    (
                        obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                        (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                    ) &&
                    obj.TipoIntegracao.Ativo
                );

            return consultaIntegracoes.OrderBy(o => o.Codigo).Take(25).ToList();
        }
        #endregion
    }
}
