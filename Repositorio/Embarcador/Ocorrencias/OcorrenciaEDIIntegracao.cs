using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>
    {
        public OcorrenciaEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public OcorrenciaEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao> Consultar(int ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, bool filialEmissora, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query where obj.FilialEmissora == filialEmissora select obj;

            if (ocorrencia > 0)
                result = result.Where(obj => obj.CargaOcorrencia.Codigo == ocorrencia);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public bool VerificarSeExistePorOcorrencia(int codigoOcorrencia, int codigoTipoIntegracao, int codigoLayoutEDI, int xmlNotaFiscal, int tipoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            if (xmlNotaFiscal > 0)//todo: quado é por nota fiscal não permite gerar a duas integrações do mesmo tipo para a mesma nota
                query = query.Where(obj => obj.XMLNotaFiscal.Codigo == xmlNotaFiscal && obj.CargaOcorrencia.TipoOcorrencia.Codigo == tipoOcorrencia && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada && obj.LayoutEDI.Codigo == codigoLayoutEDI);
            else
                query = query.Where(obj => obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.LayoutEDI.Codigo == codigoLayoutEDI);

            return query.Select(o => o.Codigo).Any();
        }

        public int ContarConsulta(int ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, bool filialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query where obj.FilialEmissora == filialEmissora select obj;

            if (ocorrencia > 0)
                result = result.Where(obj => obj.CargaOcorrencia.Codigo == ocorrencia);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao> BuscarIntegracoesPendentes(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query
                         where
                         !obj.CargaOcorrencia.GerandoIntegracoes
                         && (
                             obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                             || (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                && (obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao || obj.NumeroTentativas < obj.CargaOcorrencia.TipoOcorrencia.QuantidadeReenvioIntegracao)
                                && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                             )
                         )
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez)
                .Fetch(obj => obj.LayoutEDI)
                .Fetch(obj => obj.CargaOcorrencia)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.Empresa)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao> BuscarPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao BuscarPorLayout(int ocorrencia, Dominio.Enumeradores.TipoLayoutEDI tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query
                         where
                         obj.CargaOcorrencia.Codigo == ocorrencia
                         && obj.LayoutEDI.Tipo == tipo
                         select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorOcorrencia(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoCarga select obj.Codigo;

            return result.Count();
        }


        public int ContarPorOcorrenciaESituacaoDiff(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public int ContarPorOcorrencia(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao> BuscarPorOcorrencia(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, bool filialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query
                         where
                            obj.CargaOcorrencia.Codigo == codigoOcorrencia
                            && obj.FilialEmissora == filialEmissora
                         select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorOcorrenciaEFilialEmissora(int codigoOcorrencia, bool filialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao>();

            var result = from obj in query
                         where
                             obj.CargaOcorrencia.Codigo == codigoOcorrencia
                             && obj.FilialEmissora == filialEmissora
                         select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

    }
}
