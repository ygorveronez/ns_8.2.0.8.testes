using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeAquaviarioManualIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao>
    {
        #region Construtores

        public CargaMDFeAquaviarioManualIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao> Consultar(int codigoMDFeManualIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaMDFeManualIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao>()
                .Where(o => o.CargaMDFeManual.Codigo == codigoMDFeManualIntegracao);

            if (situacao.HasValue)
                consultaMDFeManualIntegracao = consultaMDFeManualIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaMDFeManualIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao BuscarPorCodigo(int codigo)
        {
            var cargaMDFeManualntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return cargaMDFeManualntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao> Consultar(int codigoMDFeManualIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
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

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao> BuscarPorCarga(int codigoCargaMDFeManual, SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao>();

            var result = from obj in query where obj.CargaMDFeManual.Codigo == codigoCargaMDFeManual select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao>();

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
