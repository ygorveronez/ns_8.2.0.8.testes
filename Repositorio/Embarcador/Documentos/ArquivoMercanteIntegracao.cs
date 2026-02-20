using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Documentos
{
    public class ArquivoMercanteIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao>
    {
        public ArquivoMercanteIntegracao(UnitOfWork unitOfWork): base(unitOfWork) { }

        #region Métodos Públicos

        public (List<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao> Arquivos, int Total) Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipoIntegracao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consulta = Consultar(situacao, tipoIntegracao);

            return (
                Arquivos: ObterLista(consulta, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros),
                Total: consulta.Count()
            );
        }

        public Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao>();

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

        public IEnumerable<(SituacaoIntegracao Situacao, int Total)> ConsultaTotaisPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacaoIntegracao = null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipoIntegracao = null)
        {
            var consulta = Consultar(situacaoIntegracao, tipoIntegracao);

            return consulta
                .GroupBy(x => x.SituacaoIntegracao)
                .Select(g => new { Situacao = g.Key, Total = g.Count() })
                .AsEnumerable()
                .Select(x => (x.Situacao, x.Total));
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipoIntegracao = null)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao>();

            if (situacao.HasValue)
                consulta = consulta.Where(o => o.SituacaoIntegracao == situacao);

            if (tipoIntegracao != null)
                consulta = consulta.Where(o => o.TipoIntegracao.Tipo == tipoIntegracao);

            return consulta;
        }

        #endregion
    }
}