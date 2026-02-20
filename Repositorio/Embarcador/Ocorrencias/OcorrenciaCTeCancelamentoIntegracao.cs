using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaCTeCancelamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao>
    {
        public OcorrenciaCTeCancelamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Públicos

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao> BuscarPorIntegracaoPendente(int numeroTentativasLimite, double tempoProximaTentativaEmMinutos, int limiteRegistros)
        {
            var consultaOcorrenciaCTeCancelamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao>()
                .Where(o =>
                    o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || (
                        o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao &&
                        o.NumeroTentativas < numeroTentativasLimite &&
                        o.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaEmMinutos)
                    )
                );

            return consultaOcorrenciaCTeCancelamentoIntegracao
                .OrderBy(o => o.Codigo)
                .Skip(0)
                .Take(limiteRegistros)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao>();

            var result = from obj in query where obj.OcorrenciaCTeIntegracao.CargaCTe.Codigo == codigoCargaCTe select obj;

            return result.ToList();
        }
        public int ContarPorOcorrenciaCancelamento(int codigoOcorrenciaCancelamento, SituacaoIntegracao? situacao, TipoIntegracao? tipoIntegracao = null)
        {
            var consultaOcorrenciaCancelamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao>()
                .Where(o => o.OcorrenciaCancelamento.Codigo == codigoOcorrenciaCancelamento);

            if (situacao.HasValue)
                consultaOcorrenciaCancelamentoIntegracao = consultaOcorrenciaCancelamentoIntegracao.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipoIntegracao.HasValue)
                consultaOcorrenciaCancelamentoIntegracao = consultaOcorrenciaCancelamentoIntegracao.Where(o => o.TipoIntegracao.Tipo == tipoIntegracao.Value);

            return consultaOcorrenciaCancelamentoIntegracao.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao> Consultar(int codigoOcorrenciaCancelamento, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(codigoOcorrenciaCancelamento, situacao);

            query = query
                .Fetch(obj => obj.OcorrenciaCancelamento)
                .Fetch(obj => obj.TipoIntegracao);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(int codigoOcorrenciaCancelamento, SituacaoIntegracao? situacao)
        {
            var consultaOcorrenciaCancelamentoIntegracao = Consultar(codigoOcorrenciaCancelamento, situacao);

            return consultaOcorrenciaCancelamentoIntegracao.Count();
        }

        #endregion

        #region Privados

        public IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao> Consultar(int codigoOcorrenciaCancelamento, SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao>();

            if (codigoOcorrenciaCancelamento > 0)
                query = query.Where(o => o.OcorrenciaCancelamento.Codigo == codigoOcorrenciaCancelamento);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            return query;
        }

        #endregion

    }
}
