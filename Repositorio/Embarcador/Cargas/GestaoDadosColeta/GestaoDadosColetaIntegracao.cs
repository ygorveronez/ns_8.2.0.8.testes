using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.GestaoDadosColeta
{
    public class GestaoDadosColetaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao>
    {
        #region Construtores

        public GestaoDadosColetaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao> Consultar(int codigoGestaoDadosColeta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaGestaoDadosColeta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao>()
                .Where(o => o.GestaoDadosColeta.Codigo == codigoGestaoDadosColeta);

            if (situacao.HasValue)
                consultaGestaoDadosColeta = consultaGestaoDadosColeta.Where(o => o.SituacaoIntegracao == situacao);

            return consultaGestaoDadosColeta;
        }

        private IOrderedQueryable<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo> ConsultarArquivosPorIntergacao(int codigo)
        {
            var queryGestaoDadosColetaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao>();
            var resultGestaoDadosColetaIntegracao = from obj in queryGestaoDadosColetaIntegracao where obj.Codigo == codigo select obj;

            var queryGestaoDadosColetaIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo>();

            return from obj in queryGestaoDadosColetaIntegracaoArquivo where resultGestaoDadosColetaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao> Consultar(int codigoGestaoDadosColeta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoGestaoDadosColeta, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoGestaoDadosColeta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoGestaoDadosColeta, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao> BuscarIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaSegundos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao>();

            var result = from obj in query
                         where obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite &&
                               obj.DataIntegracao <= DateTime.Now.AddSeconds(-tempoProximaTentativaSegundos)))
                         select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao> BuscarPorGestaoDadosColeta(int codigo)
        {
            List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao> gestaoDadosColetaIntegracoes = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao>()
                .Where(o => o.GestaoDadosColeta.Codigo == codigo)
                .ToList();

            return gestaoDadosColetaIntegracoes;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo> BuscarArquivosPorIntergacao(int codigo, int inicio, int limite)
        {
            return ConsultarArquivosPorIntergacao(codigo).Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntergacao(int codigo)
        {
            return ConsultarArquivosPorIntergacao(codigo).Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo BuscarIntergacaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao>();
            var result = from obj in query where obj.GestaoDadosColeta.CargaEntrega.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }
    

        #endregion Métodos Públicos
    }
}
