using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaRegistroEncerramento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento>
    {
        public CargaRegistroEncerramento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento BuscarPorCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento>();
            var result = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento>()
                .Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public DateTime? BuscarMaiorDataEncerramento(List<int> cargas)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento>();
            var result = from obj in query where cargas.Contains(obj.Carga.Codigo) && obj.DataEncerramento.HasValue orderby obj.DataEncerramento.Value descending select obj;
            
            if (result.Count() > 0)
                return result.FirstOrDefault()?.DataEncerramento.Value;
            else
                return null;
        }


        public List<int> BuscarCodigosCargasEncerramentoPendentesPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga situacaoEncerramentoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento>();
            var queryIntegracoes = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            var codigoIntegracoesErros = queryIntegracoes.Where(o => o.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado).Select(o => o.CargaRegistroEncerramento.Codigo);

            var result = query.Where(o => o.Situacao == situacaoEncerramentoCarga && !codigoIntegracoesErros.Contains(o.Codigo)).Select(o => o.Codigo);

            return result.Take(25).ToList();
        }

        public List<int> BuscarCodigosCargasEncerramentoIntegracoesPendentes()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento>();
            var queryIntegracoes = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            var codigoIntegracoesErros = queryIntegracoes.Where(o => o.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado).Select(o => o.CargaRegistroEncerramento.Codigo);

            var result = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga.AgIntegracao 
                                    && o.EncerrouMDFes
                                    && !codigoIntegracoesErros.Contains(o.Codigo)).Select(o => o.Codigo);

            return result.Take(25).ToList();
        }

        public List<int> BuscarCodigosCargasEncerramentoPossuiIntegracaoPendente()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento>();
            var queryIntegracoes = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao>();

            var codigoIntegracoesErros = queryIntegracoes.Where(o => o.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado).Select(o => o.CargaRegistroEncerramento.Codigo);

            var result = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga.AgIntegracao && o.EncerrouMDFes 
                                        && o.PossuiIntegracao
                                        && !codigoIntegracoesErros.Contains(o.Codigo)).Select(o => o.Codigo);

            return result.Take(25).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFluxoEncerramentoCarga filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFluxoEncerramentoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            query = query.Fetch(o => o.Carga);

            return ObterLista(query, parametroConsulta);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFluxoEncerramentoCarga filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where(obj => obj.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.DataInicial.HasValue)
                query = query.Where(obj => obj.DataEncerramento.Value >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFinal.HasValue)
                query = query.Where(obj => obj.DataEncerramento.Value < filtrosPesquisa.DataFinal.Value);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(obj => obj.Situacao == filtrosPesquisa.Situacao.Value);

            return query;
        }


    }
}
