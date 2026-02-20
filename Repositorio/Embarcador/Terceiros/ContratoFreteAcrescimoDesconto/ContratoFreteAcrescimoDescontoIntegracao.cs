using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFreteAcrescimoDescontoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao>
    {
        public ContratoFreteAcrescimoDescontoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao BuscarPrimeiroPorContratoFreteAcrescimoDesconto(int codigoContratoFreteAcrescimoDesconto)
        {
            var contratoFreteAcrescimoDescontoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao>()
                .Where(o => o.ContratoFreteAcrescimoDesconto.Codigo == codigoContratoFreteAcrescimoDesconto)
                .FirstOrDefault();

            return contratoFreteAcrescimoDescontoIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao> BuscarPorContratoFreteAcrescimoDesconto(int codigoContratoFreteAcrescimoDesconto)
        {
            var contratoFreteAcrescimoDescontoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao>()
                .Where(o => o.ContratoFreteAcrescimoDesconto.Codigo == codigoContratoFreteAcrescimoDesconto)
                .ToList();

            return contratoFreteAcrescimoDescontoIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao> Consultar(int codigoContratoFreteAcrescimoDesconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoContratoFreteAcrescimoDesconto, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoContratoFreteAcrescimoDesconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoContratoFreteAcrescimoDesconto, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao> BuscarIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))) select obj;

            return result.Fetch(c => c.CIOT)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo BuscarIntegracaoCIOTPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo> BuscarArquivosPorIntegracao(int codigo, int inicio, int limite)
        {
            var queryContratoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao>();
            var resultContratoIntegracao = from obj in queryContratoIntegracao where obj.Codigo == codigo select obj;

            var queryCIOTIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo>();
            var resultCIOTIntegracaoArquivo = from obj in queryCIOTIntegracaoArquivo where resultContratoIntegracao.Any(p => p.CIOT.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCIOTIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var queryContratoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao>();
            var resultContratoIntegracao = from obj in queryContratoIntegracao where obj.Codigo == codigo select obj;

            var queryCIOTIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo>();
            var resultCIOTIntegracaoArquivo = from obj in queryCIOTIntegracaoArquivo where resultContratoIntegracao.Any(p => p.CIOT.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCIOTIntegracaoArquivo.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao> Consultar(int codigoContratoFreteAcrescimoDesconto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaContratoFreteAcrescimoDescontoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao>()
                .Where(o => o.ContratoFreteAcrescimoDesconto.Codigo == codigoContratoFreteAcrescimoDesconto);

            if (situacao.HasValue)
                consultaContratoFreteAcrescimoDescontoIntegracao = consultaContratoFreteAcrescimoDescontoIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaContratoFreteAcrescimoDescontoIntegracao;
        }

        #endregion
    }
}
