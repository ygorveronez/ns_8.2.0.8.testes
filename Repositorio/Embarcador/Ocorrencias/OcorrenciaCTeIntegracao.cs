using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;


namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaCTeIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>
    {
        public OcorrenciaCTeIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<TipoIntegracao> ObterTiposIntegracoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> BuscarPorOcorrencia(int codigOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigOcorrencia select obj;

            return result.ToList();
        }

        public List<int> BuscarCargaCTesPorCargaCTes(List<int> cargaCTes, int tipoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where cargaCTes.Contains(obj.CargaCTe.Codigo) && obj.CargaOcorrencia.TipoOcorrencia.Codigo == tipoOcorrencia select obj.CargaCTe.Codigo;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> BuscarPorOcorrencia(int codigOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigOcorrencia select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorOcorrencia(int codigOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigOcorrencia select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> BuscarCTeIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query
                         where
                         !obj.CargaOcorrencia.GerandoIntegracoes
                         && obj.TipoIntegracao.TipoEnvio == tipoEnvio
                         && (
                            obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                            || (
                                obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                && (obj.NumeroTentativas < tentativasLimite || obj.NumeroTentativas < obj.CargaOcorrencia.TipoOcorrencia.QuantidadeReenvioIntegracao)
                                && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)
                            )
                         )
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public int ContarPorOcorrenciaETipoIntegracao(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoOcorrencia && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> BuscarCTeIntegracaoSemLote(string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where !obj.CargaOcorrencia.GerandoIntegracoes && obj.TipoIntegracao.TipoEnvio == TipoEnvioIntegracao.Lote && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && obj.Lote == null select obj;

            return result.Fetch(o => o.TipoIntegracao).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarTipoIntegracaoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) select obj.TipoIntegracao;

            return result.Distinct().ToList();
        }

        public List<int> BuscarCodigosCargaCTePorOcorrenciaSemIntegracao(int codigoCargaOcorrencia, TipoIntegracao tipoIntegracao, int quantidadeRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>().Where(o => o.CargaOcorrencia.Codigo == codigoCargaOcorrencia && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.CargaCTe.Codigo);

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoCargaOcorrencia && !subQueryIntegracoes.Contains(o.CargaCTe.Codigo));

            if (tipoIntegracao == TipoIntegracao.Natura)
                query = query.Where(o => o.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Outros);

            return query.Select(o => o.CargaCTe.Codigo).Take(quantidadeRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> Consultar(int codigoOcorrencia, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query select obj;

            if (codigoOcorrencia > 0)
                result = result.Where(o => o.CargaOcorrencia.Codigo == codigoOcorrencia);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.CargaCTe).ThenFetch(obj => obj.CTe).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.CargaCTe).ThenFetch(obj => obj.PreCTe).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoOcorrencia, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query select obj;

            if (codigoOcorrencia > 0)
                result = result.Where(o => o.CargaOcorrencia.Codigo == codigoOcorrencia);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }

        public int ContarPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;

            return result.Count();
        }

        public int ContarPorOcorrenciaESituacaoDiff(int codigoOcorrencia, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.SituacaoIntegracao != situacaoDiff select obj;

            return result.Count();
        }

        public int ContarPorOcorrencia(int codigoOcorrencia, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public int ContarPorOcorrencia(int codigoOcorrencia, SituacaoIntegracao[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && situacao.Contains(obj.SituacaoIntegracao) select obj;

            return result.Count();
        }

        public int ContarPorLote(int codigoLote, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> BuscarRegistrosComRetornoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

            var result = from obj in query where obj.PendenteRetorno select obj;

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao BuscarPorCargaCTeETipoIntegracao(int codigoCargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var consultaNFSManualCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>()
                .Where(o => o.CargaCTe.Codigo == codigoCargaCTe && o.TipoIntegracao.Tipo == tipoIntegracao);

            return consultaNFSManualCTeIntegracao.FirstOrDefault();
        }
    }
}
