using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualCancelamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>
    {
        #region Construtores

        public CargaMDFeManualCancelamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao> Consultar(int codigoCargaMDFeManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaMDFeManualIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>()
                .Where(o => o.CargaMDFeManualCancelamento.Codigo == codigoCargaMDFeManualCancelamento);

            if (situacao.HasValue)
                consultaMDFeManualIntegracao = consultaMDFeManualIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaMDFeManualIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao BuscarPorCodigo(int codigo)
        {
            var cargaMDFeManualntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return cargaMDFeManualntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao> Consultar(int codigoMDFeManualIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoMDFeManualIntegracao, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoMDFeManualIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoMDFeManualIntegracao, situacao);

            return consultaIntegracoes.Count();
        }


        //public Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao BuscarPorMotoristaETipo(int motorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao>();

        //    var result = from obj in query where obj.Motorista.Codigo == motorista && obj.TipoIntegracao.Tipo == tipo select obj;

        //    return result.FirstOrDefault();
        //}

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao> BuscarPorCarga(int codigoCargaMDFeManualCancelamento, SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>();

            var result = from obj in query where obj.CargaMDFeManualCancelamento.Codigo == codigoCargaMDFeManualCancelamento select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>();

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

        public int ContarPorCargaMDFeManualCancelamento(int codigoCargaMDFeManualCancelamento, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>();

            var result = from obj in query where obj.CargaMDFeManualCancelamento.Codigo == codigoCargaMDFeManualCancelamento && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public int ContarPorCargaMDFeManualCancelamentoESituacaoDiff(int codigoCargaMDFeManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>();

            var result = from obj in query where obj.CargaMDFeManualCancelamento.Codigo == codigoCargaMDFeManualCancelamento && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public bool ExistePorCargaMDFeManualCancelamentoETipo(int codigoCargaMDFeManualCancelamento, int codigoMDFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var consultaCargaMDFeManualIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>()
                .Where(o => o.CargaMDFeManualCancelamento.Codigo == codigoCargaMDFeManualCancelamento && o.MDFe.Codigo == codigoMDFe && o.TipoIntegracao.Tipo == tipo);

            return consultaCargaMDFeManualIntegracao.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao> BuscarCargaMDFeManualCancelamentoIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao>();

            var result = from obj in query
                         where
                         !obj.CargaMDFeManualCancelamento.GerandoIntegracoes
                         && (
                            obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                            || (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                && (obj.NumeroTentativas < tentativasLimite)
                                && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)
                            )
                         )
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        #endregion
    }
}
