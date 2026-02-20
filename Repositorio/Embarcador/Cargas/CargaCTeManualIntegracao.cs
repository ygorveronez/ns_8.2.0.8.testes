using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCTeManualIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao>
    {
        #region Construtores

        public CargaCTeManualIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao> Consultar(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaCteManualIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                consultaCteManualIntegracao = consultaCteManualIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaCteManualIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao BuscarPorCodigo(int codigo)
        {
            var cargaCteManualntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return cargaCteManualntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao> Consultar(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoCarga, situacao);

            consultaIntegracoes = consultaIntegracoes
                .Fetch(o => o.TipoIntegracao)
                .Fetch(o => o.CTe);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoCarga, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao> BuscarPorCarga(int codigoCarga, SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao BuscarPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao>();

            var result = from obj in query 
                         where obj.Carga.Codigo == codigoCarga 
                         && obj.TipoIntegracao.Tipo == tipoIntegracao
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao BuscarPorCargaECTeTipo(int codigoCarga,int codigoCte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao>();

            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga
                         && obj.CTe.Codigo == codigoCte
                         && obj.TipoIntegracao.Tipo == tipoIntegracao
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao>();

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
                            && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        #endregion
    }
}
