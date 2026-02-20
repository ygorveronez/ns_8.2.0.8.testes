using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.WMS
{
    public class SeparacaoPedido : RepositorioBase<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido>
    {
        public SeparacaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos


        public Dominio.Entidades.Embarcador.WMS.SeparacaoPedido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.LocalEntrega)
                .FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido>();

            return (query.Max(o => (int?)o.Numero) ?? 0) + 1;
        }

        public List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedido filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedido filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarPedidos(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedidoPedidos filtrosPesquisa, string propOrdenacao, string dirOrdenacao)
        {
            var result = QueryPedidos(filtrosPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(filtrosPesquisa.Inicio)
                .Take(filtrosPesquisa.Limite)
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Destino)
                .Fetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.ResponsavelRedespacho)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public int ContarConsultaPedidos(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedidoPedidos filtrosPesquisa)
        {
            var result = QueryPedidos(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntegracao(int codigo, int inicio, int limite)
        {
            var querySeparacaoPedidoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao>();
            var resultSeparacaoPedidoIntegracao = from obj in querySeparacaoPedidoIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultSeparacaoPedidoIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var querySeparacaoPedidoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao>();
            var resultSeparacaoPedidoIntegracao = from obj in querySeparacaoPedidoIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultSeparacaoPedidoIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarIntegracaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedido filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= filtrosPesquisa.DataFinal);

            if (filtrosPesquisa.Situacao.HasValue)
                result = result.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoPedido > 0)
                result = result.Where(o => o.Pedidos.Any(p => p.Pedido.Codigo == filtrosPesquisa.CodigoPedido));

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> QueryPedidos(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaSeparacaoPedidoPedidos filtrosPesquisa)
        {
            if (filtrosPesquisa.CodigoSeparacaoPedido > 0)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido>();
                var result = from obj in query where obj.SeparacaoPedido.Codigo == filtrosPesquisa.CodigoSeparacaoPedido select obj;

                return result.Select(obj => obj.Pedido);
            }
            else
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                var result = from obj in query where obj.DisponivelParaSeparacao select obj;

                if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                    result = result.Where(obj => (obj.DataSolicitacaoReentrega.Value.Date >= filtrosPesquisa.DataInicial) || (obj.DataCarregamentoPedido != null && obj.DataCarregamentoPedido.Value.Date >= filtrosPesquisa.DataInicial) || (obj.DataCarregamentoPedido == null && obj.DataCriacao.Value.Date >= filtrosPesquisa.DataInicial));
                
                if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                    result = result.Where(obj => (obj.DataSolicitacaoReentrega.Value.Date <= filtrosPesquisa.DataFinal) || (obj.DataCarregamentoPedido != null && obj.DataCarregamentoPedido.Value.Date <= filtrosPesquisa.DataFinal) || (obj.DataCarregamentoPedido == null && obj.DataCriacao.Value.Date <= filtrosPesquisa.DataFinal));
                    
                if (filtrosPesquisa.CodigoPedido > 0)
                    result = result.Where(o => o.Codigo == filtrosPesquisa.CodigoPedido);

                if (filtrosPesquisa.CodigosFilial.Count > 0)
                    result = result.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Filial.Codigo));

                if (filtrosPesquisa.CodigoOrigem > 0)
                    result = result.Where(o => o.Origem.Codigo == filtrosPesquisa.CodigoOrigem);

                if (filtrosPesquisa.CodigoDestino > 0)
                    result = result.Where(o => o.Destino.Codigo == filtrosPesquisa.CodigoDestino);

                if (filtrosPesquisa.CpfCnpjRemetentes.Count > 0)
                    result = result.Where(o => filtrosPesquisa.CpfCnpjRemetentes.Contains(o.Remetente.CPF_CNPJ));

                if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                    result = result.Where(o => o.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario);

                if (filtrosPesquisa.CpfCnpjLocalExpedicao > 0)
                    result = result.Where(o => o.LocalExpedicao.CPF_CNPJ == filtrosPesquisa.CpfCnpjLocalExpedicao);

                if (filtrosPesquisa.SomentePedidosDeReentrega)
                {
                    /*var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                    var resultQueryCargaPedido = from obj in queryCargaPedido select obj;

                    result = result.Where(o => resultQueryCargaPedido.Where(a => a.Pedido.Codigo == o.Codigo && a.Carga.Entregas.Any(c => c.PossuiReentrega)).Any());*/
                    result = result.Where(o => o.ReentregaSolicitada);
                }

                if (filtrosPesquisa.SomentePedidosEmAberto)
                {
                    var queryChamado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
                    var resultQueryChamado = from obj in queryChamado select obj;

                    result = result.Where(o => resultQueryChamado.Where(a => a.Carga.Codigo == o.Codigo && (a.Situacao == SituacaoChamado.Aberto || a.Situacao == SituacaoChamado.EmTratativa)).Any());
                }

                if (filtrosPesquisa.NumerosNotaFiscal.Count > 0)
                {
                    result = result.Where(o => o.NotasFiscais.Any(n => filtrosPesquisa.NumerosNotaFiscal.Contains(o.Numero)));
                }

                var querySeparacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido>();
                var resultSeparacao = from obj in querySeparacao where obj.SeparacaoPedido.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedido.Aberto select obj;

                result = result.Where(obj => !resultSeparacao.Select(o => o.Pedido.Codigo).Contains(obj.Codigo));

                return result;
            }
        }

        #endregion
    }
}
