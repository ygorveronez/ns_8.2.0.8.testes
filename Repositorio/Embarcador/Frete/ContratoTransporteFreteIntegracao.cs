using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoTransporteFreteIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>
    {
        #region Construtores

        public ContratoTransporteFreteIntegracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        #endregion

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao BuscarPorContrato(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>();
            query = from obj in query where obj.ContratoTransporteFrete.NumeroContrato == codigoContrato select obj;
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, bool integracaoAnexos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>();

            var result = from obj in query
                         where
                            (
                                obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                                (
                                    obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao
                                    && obj.NumeroTentativas < numeroTentativas
                                    && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                )
                            )
                            && obj.TipoIntegracao.Ativo && obj.IntegrarAnexos == integracaoAnexos
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao BuscarIntegracaoPendentePorCodigoContrato(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>();

            query = query.Where(obj => obj.ContratoTransporteFrete.Codigo == codigoContrato 
                && obj.SituacaoIntegracao != SituacaoIntegracao.AgIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao BuscarIntegracaoPorCodigoContrato(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>();

            query = query.Where(obj => obj.ContratoTransporteFrete.Codigo == codigoContrato);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao> BuscarIntegracoesPorCodigoContrato(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>();

            query = query.Where(obj => obj.ContratoTransporteFrete.Codigo == codigoContrato);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao> BuscarIntegracoesRejeitadas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>();

            var result = query.Where(o => o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && o.TipoIntegracao.Ativo);

            return result.ToList();
        }

        #endregion
    }
}
