using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class PagamentoContratoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>
    {
        public PagamentoContratoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
        public bool ExisteIntegracaoParaContratoFrete(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Codigo == codigo select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Carga.Codigo == codigoCarga select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> BuscarPorContratoFrete(int codigoContratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Codigo == codigoContratoFrete select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> BuscarPorAutorizacaoPagamento(int codigoAutorizacaoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.AutorizacaoPagamentoContratoFrete.Codigo == codigoAutorizacaoPagamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query
                         where obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> BuscarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Carga.Codigo == codigoCarga select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return result.Fetch(o => o.ContratoFrete).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> BuscarPagamentoContratoFreteIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))) select obj;

            return result.WithOptions(o=> o.SetTimeout(120)).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public int ContarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            query = query.Where(o => o.ContratoFrete.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> BuscarCTeIntegracaoSemLote(string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == TipoEnvioIntegracao.Lote && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao select obj;

            return result.Fetch(o => o.TipoIntegracao)
                         .Fetch(o => o.ContratoFrete)
                         .ThenFetch(o => o.Carga)
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarTipoIntegracaoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) select obj.TipoIntegracao;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> Consultar(int codigoContratoFrete, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query select obj;

            if (codigoContratoFrete > 0)
                result = result.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.ContratoFrete)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoCarga, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query select obj;

            if (codigoCarga > 0)
                result = result.Where(o => o.ContratoFrete.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> ConsultarPorAutorizacaoPagamento(int codigoAutorizacaoPagamento, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query select obj;

            if (codigoAutorizacaoPagamento > 0)
                result = result.Where(o => o.AutorizacaoPagamentoContratoFrete.Codigo == codigoAutorizacaoPagamento);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.ContratoFrete)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsultaPorAutorizacaoPagamento(int codigoAutorizacaoPagamento, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query select obj;

            if (codigoAutorizacaoPagamento > 0)
                result = result.Where(o => o.AutorizacaoPagamentoContratoFrete.Codigo == codigoAutorizacaoPagamento);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Carga.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public int ContarPorCargaESituacaoDiff(int codigoCarga, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao != situacaoDiff select obj;

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracao[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

            var result = from obj in query where obj.ContratoFrete.Carga.Codigo == codigoCarga && situacao.Contains(obj.SituacaoIntegracao) select obj;

            return result.Count();
        }
    }
}
