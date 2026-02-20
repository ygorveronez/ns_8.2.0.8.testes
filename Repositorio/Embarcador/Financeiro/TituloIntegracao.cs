using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Entidades.Embarcador.Cargas;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>
    {
        #region Construtores

        public TituloIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao> Consultar(long codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>()
                .Where(o => o.Titulo.Codigo == codigoTitulo);

            if (situacao.HasValue)
                consultaIntegracao = consultaIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaIntegracao;
        }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao> BuscarPorTitulo(long codigoTitulo)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>()
                .Where(o => o.Titulo.Codigo == codigoTitulo)
                .ToList();
            return integracao;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao> BuscarPendentesIntegracao(int quantideRegistro, int numeroTentativas, int minutosACadaTentativa)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>()
                .Fetch(o => o.TipoIntegracao)
                .Fetch(o => o.Titulo)
                .Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                           (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                           o.NumeroTentativas < numeroTentativas && o.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)))
                .Take(quantideRegistro)
                .ToList();
            return integracao;
        }


        public Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao BuscarPorTituloETipo(long codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoDocumento)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>()
                .Where(o => o.Titulo.Codigo == codigoTitulo && o.TipoIntegracao.Tipo == tipo && o.TipoAcaoIntegracao == tipoAcaoDocumento)
                .FirstOrDefault();

            return integracao;
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao BuscarPorTituloETipoIntegracao(long codigoTitulo, int codigoTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao BuscarPorTituloETipoIntegracaoEAcao(long codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo && o.TipoIntegracao.Tipo == tipo && o.TipoAcaoIntegracao == tipoAcaoDocumento);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao> Consultar(long codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoTitulo, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(long codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoTitulo, situacao);

            return consultaIntegracoes.Count();
        }
        public List<CargaCTeIntegracaoArquivo> BuscarArquivosPorIntegacao(int codigo, int inicio, int limite)
        {
            var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>();
            var resultIntegracao = from obj in queryIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>();
            var resultIntegracao = from obj in queryIntegracao where obj.Codigo == codigo select obj;
            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;
            return resultCargaCTeIntegracaoArquivo.Count();
        }

        public CargaCTeIntegracaoArquivo BuscarIntegracaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ExistePorTituloETipo(int codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoDocumento)
        {
            var consultaCargaCTeAgrupadoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao>()
                .Where(o => o.Titulo.Codigo == codigoTitulo && o.TipoIntegracao.Tipo == tipo && o.TipoAcaoIntegracao == tipoAcaoDocumento);

            return consultaCargaCTeAgrupadoIntegracao.Count() > 0;
        }
        #endregion
    }
}

