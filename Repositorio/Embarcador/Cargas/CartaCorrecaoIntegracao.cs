using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CartaCorrecaoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao>
    {
        public CartaCorrecaoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao> Consultar(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao>()
                .Where(o => o.CartaCorrecao.CTe.Codigo == codigoCTe);

            if (situacao.HasValue)
                consultaIntegracao = consultaIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao BuscarPorCodigo(int codigo)
        {
            var cargaMDFeManualntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return cargaMDFeManualntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao> Consultar(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoCTe, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoCTe, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao> BuscarPorCTe(int codigoCTe, SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao>();

            var result = from obj in query where obj.CartaCorrecao.CTe.Codigo == codigoCTe select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao>();

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