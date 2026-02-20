using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class PesagemIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>
    {
        public PesagemIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao> BuscarPorPesagem(int pesagem)
        {
            var pesagemIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>()
                .Where(o => o.Pesagem.Codigo == pesagem)
                .ToList();

            return pesagemIntegracao;
        }

        public Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.Pesagem)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao> Consultar(int codigoPesagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoPesagem, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoPesagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoPesagem, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao> BuscarIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaSegundos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>();

            var result = from obj in query
                         where obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite &&
                               obj.DataIntegracao <= DateTime.Now.AddSeconds(-tempoProximaTentativaSegundos)))
                         select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao BuscarPorPesagemETipoIntegracao(int codigoPesagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoBalanca tipoIntegracaoBalanca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>();

            var result = from obj in query where obj.Pesagem.Codigo == codigoPesagem && obj.TipoIntegracaoBalanca == tipoIntegracaoBalanca select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao BuscarUltimaPorPesagem(int codigoPesagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>();

            var result = from obj in query where obj.Pesagem.Codigo == codigoPesagem orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao> Consultar(int codigoPesagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaPesagemIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>()
                .Where(o => o.Pesagem.Codigo == codigoPesagem);

            if (situacao.HasValue)
                consultaPesagemIntegracao = consultaPesagemIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaPesagemIntegracao;
        }

        #endregion
    }
}
