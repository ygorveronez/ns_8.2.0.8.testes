using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public sealed class OcorrenciaCancelamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao>
    {
        #region Construtores

        public OcorrenciaCancelamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao> Consultar(int codigoOcorrenciaCancelamento, SituacaoIntegracao? situacao)
        {
            var consultaOcorrenciaCancelamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao>();

            if (codigoOcorrenciaCancelamento > 0)
                consultaOcorrenciaCancelamentoIntegracao = consultaOcorrenciaCancelamentoIntegracao.Where(o => o.OcorrenciaCancelamento.Codigo == codigoOcorrenciaCancelamento);

            if (situacao.HasValue)
                consultaOcorrenciaCancelamentoIntegracao = consultaOcorrenciaCancelamentoIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaOcorrenciaCancelamentoIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao BuscarPorCodigo(int codigo)
        {
            var consultaOcorrenciaCancelamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao>()
                .Where(o => o.Codigo == codigo);

            return consultaOcorrenciaCancelamentoIntegracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var consultaOcorrenciaCancelamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return consultaOcorrenciaCancelamentoIntegracao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao> BuscarPorIntegracaoPendente(int numeroTentativasLimite, double tempoProximaTentativaEmMinutos, int limiteRegistros)
        {
            var consultaOcorrenciaCancelamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao>()
                .Where(o =>
                    o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || (
                        o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao &&
                        o.NumeroTentativas < numeroTentativasLimite &&
                        o.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaEmMinutos)
                    )
                );

            return consultaOcorrenciaCancelamentoIntegracao
                .OrderBy(o => o.Codigo)
                .Skip(0)
                .Take(limiteRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao> Consultar(int codigoOcorrenciaCancelamento, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOcorrenciaCancelamentoIntegracao = Consultar(codigoOcorrenciaCancelamento, situacao);

            consultaOcorrenciaCancelamentoIntegracao = consultaOcorrenciaCancelamentoIntegracao
                .Fetch(obj => obj.OcorrenciaCancelamento)
                .Fetch(obj => obj.TipoIntegracao);

            return ObterLista(consultaOcorrenciaCancelamentoIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(int codigoOcorrenciaCancelamento, SituacaoIntegracao? situacao)
        {
            var consultaOcorrenciaCancelamentoIntegracao = Consultar(codigoOcorrenciaCancelamento, situacao);

            return consultaOcorrenciaCancelamentoIntegracao.Count();
        }

        public int ContarPorOcorrenciaCancelamento(int codigoOcorrenciaCancelamento, SituacaoIntegracao? situacao, TipoIntegracao? tipoIntegracao = null)
        {
            var consultaOcorrenciaCancelamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao>()
                .Where(o => o.OcorrenciaCancelamento.Codigo == codigoOcorrenciaCancelamento);

            if (situacao.HasValue)
                consultaOcorrenciaCancelamentoIntegracao = consultaOcorrenciaCancelamentoIntegracao.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipoIntegracao.HasValue)
                consultaOcorrenciaCancelamentoIntegracao = consultaOcorrenciaCancelamentoIntegracao.Where(o => o.TipoIntegracao.Tipo == tipoIntegracao.Value);

            return consultaOcorrenciaCancelamentoIntegracao.Count();
        }

        #endregion
    }
}
