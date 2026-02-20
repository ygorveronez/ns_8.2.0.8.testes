using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;

namespace Repositorio.Embarcador.WMS
{
    public class SeparacaoPedidoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao>
    {
        public SeparacaoPedidoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao> BuscarPorSeparacaoPedido(int separacaoPedido)
        {
            var separacaoPedidoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao>()
                .Where(o => o.SeparacaoPedido.Codigo == separacaoPedido)
                .ToList();

            return separacaoPedidoIntegracao;
        }

        public Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.SeparacaoPedidoPedido)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao BuscarPorNotaESeparacaoPedido(int notaFiscal, int separacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == notaFiscal && obj.SeparacaoPedido.Codigo == separacaoPedido select obj;


            return result
                .Fetch(obj => obj.SeparacaoPedidoPedido)
                .FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao> Consultar(int codigoSeparacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoSeparacaoPedido, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoSeparacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoSeparacaoPedido, situacao);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao> BuscarIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))) select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao> Consultar(int codigoSeparacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaSeparacaoPedidoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao>()
                .Where(o => o.SeparacaoPedido.Codigo == codigoSeparacaoPedido);

            if (situacao.HasValue)
                consultaSeparacaoPedidoIntegracao = consultaSeparacaoPedidoIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaSeparacaoPedidoIntegracao;
        }

        #endregion
    }
}
