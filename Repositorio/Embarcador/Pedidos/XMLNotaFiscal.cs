using Dominio.ObjetosDeValor.Embarcador.Documentos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class XMLNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>
    {
        public XMLNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public XMLNotaFiscal(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.Codigo == codigo select obj;
            result = result.Where(obj => obj.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarNotaDisponivelMontagemContainerPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                .Where(obj => obj.Codigo == codigo);

            IQueryable<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal> queryMontagemContainerNota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal>();

            query = query.Where(obj => !queryMontagemContainerNota.Any(mc => mc.XMLNotaFiscal.Codigo == codigo));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCodigos(IList<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                query = query.Where(o => o.nfAtiva == true && tmp.Contains(o.Codigo));
                query = query.Fetch(obj => obj.Canhoto)
                    .Fetch(obj => obj.Destinatario)
                    .Fetch(obj => obj.Emitente);

                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>> BuscarPorCodigosAsync(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            int take = 2000;
            int start = 0;

            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                query = query.Where(o => o.nfAtiva && tmp.Contains(o.Codigo));

                result.AddRange(await query
                    .Fetch(obj => obj.Canhoto)
                    .Fetch(obj => obj.Destinatario)
                    .Fetch(obj => obj.Emitente)
                    .ToListAsync());
                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>> BuscarTodasPorCodigosAsync(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            int take = 1000;
            int start = 0;

            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                    .Where(o => tmp.Contains(o.Codigo))
                    .Fetch(obj => obj.Canhoto)
                    .Fetch(obj => obj.Destinatario)
                    .Fetch(obj => obj.Emitente);

                var partialResult = await query.ToListAsync();
                result.AddRange(partialResult);

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorNumeroControlePedido(string numeroControle)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query
                                                                                    where
                                                                                       obj.NumeroControlePedido == numeroControle
                                                                                       && obj.nfAtiva == true
                                                                                    select obj;

            return result.ToList();
        }

        public bool BuscarPorNumeroControlePedido(List<string> numerosControlePedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query
                                                                                    where
                                                                                       numerosControlePedidos.Contains(obj.NumeroControlePedido)
                                                                                       && obj.nfAtiva == true
                                                                                    select obj;

            return result.Any();
        }

        public List<int> BuscarNotasAtivasPorChave(List<string> chaves, bool ignorarReentrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = from obj in query
                                                                                          where obj.XMLNotaFiscal.nfAtiva && chaves.Contains(obj.XMLNotaFiscal.Chave) && obj.CargaPedido.Carga.CargaFechada &&
                                                                                                 obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                                                                                 obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                                                                          select obj;

            if (ignorarReentrega)
                result = result.Where(obj => !obj.CargaPedido.Pedido.ReentregaSolicitada);

            return result.Select(obj => obj.XMLNotaFiscal.Numero).ToList();
        }

        public List<string> BuscarCargasAtivasPorChave(List<string> chaves, bool ignorarReentrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = from obj in query
                                                                                          where obj.XMLNotaFiscal.nfAtiva && chaves.Contains(obj.XMLNotaFiscal.Chave) && obj.CargaPedido.Carga.CargaFechada &&
                                                                                                 obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                                                                                 obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                                                                          select obj;

            if (ignorarReentrega)
                result = result.Where(obj => !obj.CargaPedido.Pedido.ReentregaSolicitada);

            return result.Select(obj => obj.CargaPedido.Carga.CodigoCargaEmbarcador).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarXMLPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                .Where(obj => obj.Codigo == codigo).FirstOrDefaultAsync();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>> BuscarListaXMLPorCodigoAsync(List<int> listaCodigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            var result = from obj in query where listaCodigos.Contains(obj.Codigo) select obj;

            result = result.Where(o => o.XML != string.Empty);

            return await result.ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorChaveECarga(string chave, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = from obj in query
                                                                                          where obj.CargaPedido.Carga.Codigo == carga && obj.XMLNotaFiscal.Chave == chave &&
                                                                        obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe
                                                                                          select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Select(obj => obj.XMLNotaFiscal).FirstOrDefault();
        }


        public List<string> BuscarPorChavesDasNotasNaCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = from obj in query
                                                                                          where obj.CargaPedido.Carga.Codigo == carga && obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe
                                                                                          select obj;
            return result.Select(obj => obj.XMLNotaFiscal.Chave).ToList();
        }



        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNumeroEEmitente(int numero, double cpfCnpjEmitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> consultaXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                .Where(o =>
                    o.Numero == numero &&
                    (o.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? o.Emitente.CPF_CNPJ == cpfCnpjEmitente : o.Destinatario.CPF_CNPJ == cpfCnpjEmitente) &&
                    o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe &&
                    o.nfAtiva
                );

            return consultaXMLNotaFiscal.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNumeroSerieEmitente(int numero, string serie, double cpfCnpjEmitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            query = query.Where(o => o.Numero == numero &&
                                     o.Serie == serie &&
                                     o.Emitente.CPF_CNPJ == cpfCnpjEmitente &&
                                     o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && o.nfAtiva);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNumeroSerieEParticipantes(int numero, int serie, double cpfCnpjEmitente, double cpfCnpjDestinatario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            query = query.Where(o => o.Numero == numero &&
                                     o.Serie == serie.ToString() &&
                                     o.Emitente.CPF_CNPJ == cpfCnpjEmitente &&
                                     o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario &&
                                     o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && o.nfAtiva);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNumeroECarga(int numero, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = from obj in query
                                                                                          where obj.CargaPedido.Carga.Codigo == carga && obj.XMLNotaFiscal.Numero == numero &&
                                                                        obj.XMLNotaFiscal.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe
                                                                                          select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Select(obj => obj.XMLNotaFiscal).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNumero(int numero)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.Numero == numero && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;
            result = result.Where(obj => obj.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorNumeros(List<int> numeros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                .Where(obj => obj.nfAtiva == true && numeros.Contains(obj.Numero));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorSerie(int serie)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            query = query.Where(o => o.Serie == serie.ToString() && o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && o.nfAtiva);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorCNPJFilial(string cnpjFilial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            query = query.Where(o => o.Filial.CNPJ == cnpjFilial.ToString() && o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && o.nfAtiva);
            return query.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> _Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscal filtroPesquisaXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.nfAtiva select obj;

            if (filtroPesquisaXMLNotaFiscal.NumeroNotaFiscal > 0)
                result = result.Where(o => o.Numero == filtroPesquisaXMLNotaFiscal.NumeroNotaFiscal);

            if (!string.IsNullOrWhiteSpace(filtroPesquisaXMLNotaFiscal.Serie))
                result = result.Where(o => o.Serie == filtroPesquisaXMLNotaFiscal.Serie);

            if (filtroPesquisaXMLNotaFiscal.CodigoEmitente > 0)
                result = result.Where(o => o.Emitente.CPF_CNPJ == filtroPesquisaXMLNotaFiscal.CodigoEmitente);

            if (filtroPesquisaXMLNotaFiscal.DataEmissao != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao == filtroPesquisaXMLNotaFiscal.DataEmissao);

            if (filtroPesquisaXMLNotaFiscal.CodigoCliente > 0)
                result = result.Where(o => o.Destinatario.CPF_CNPJ == filtroPesquisaXMLNotaFiscal.CodigoCliente);

            if (!string.IsNullOrWhiteSpace(filtroPesquisaXMLNotaFiscal.Chave))
                result = result.Where(o => o.Chave == filtroPesquisaXMLNotaFiscal.Chave);

            if (filtroPesquisaXMLNotaFiscal.CodigoCarga > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                result = result.Where(o => (from obj in queryPedidoXMLNotaFiscal where obj.CargaPedido.Carga.Codigo == filtroPesquisaXMLNotaFiscal.CodigoCarga select obj.XMLNotaFiscal.Codigo).Contains(o.Codigo));
            }

            if (filtroPesquisaXMLNotaFiscal.CodigoCargaEntrega > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> queryCargaEntregaNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
                result = result.Where(o => (from obj in queryCargaEntregaNotaFiscal where obj.CargaEntrega.Codigo == filtroPesquisaXMLNotaFiscal.CodigoCargaEntrega select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo).Contains(o.Codigo));
            }

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> _ConsultarMontagemContainer(int numero, string serie, double emitente, DateTime dataEmissao, string chave, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();

            IQueryable<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> result = from obj in query where obj.XMLNotaFiscal != null && obj.XMLNotaFiscal.nfAtiva select obj;

            // Filtros
            if (numero > 0)
                result = result.Where(o => o.XMLNotaFiscal.Numero == numero);

            if (!string.IsNullOrWhiteSpace(serie))
                result = result.Where(o => o.Serie == serie);

            if (emitente > 0)
                result = result.Where(o => o.XMLNotaFiscal.Emitente.CPF_CNPJ == emitente);

            if (dataEmissao != DateTime.MinValue)
                result = result.Where(o => o.XMLNotaFiscal.DataEmissao == dataEmissao);

            if (!string.IsNullOrWhiteSpace(chave))
                result = result.Where(o => o.XMLNotaFiscal.Chave == chave);

            if (codigoCarga > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                result = result.Where(o => (from obj in queryPedidoXMLNotaFiscal where obj.CargaPedido.Carga.Codigo == codigoCarga select obj.XMLNotaFiscal.Codigo).Contains(o.Codigo));
            }

            IQueryable<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal> queryMontagemNota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal>();
            result = result.Where(c => !queryMontagemNota.Any(m => m.XMLNotaFiscal == c.XMLNotaFiscal));

            return result.Select(c => c.XMLNotaFiscal);
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ConsultarMontagemContainer(int numero, string serie, double emitente, DateTime dataEmissao, string chave, int codigoCarga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = _ConsultarMontagemContainer(numero, serie, emitente, dataEmissao, chave, codigoCarga);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaMontagemContainer(int numero, string serie, double emitente, DateTime dataEmissao, string chave, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = _ConsultarMontagemContainer(numero, serie, emitente, dataEmissao, chave, codigoCarga);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscal filtroPesquisaXMLNotaFiscal, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = _Consultar(filtroPesquisaXMLNotaFiscal);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscal filtroPesquisaXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = _Consultar(filtroPesquisaXMLNotaFiscal);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNotaCTe(int nota, int cte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query select obj;

            if (nota > 0)
                result = result.Where(o => o.Codigo == nota);

            if (cte > 0)
                result = result.Where(o => o.CTEs.Any(c => c.Codigo == cte));

            return result.FirstOrDefault();
        }

        public bool ContemNotaEmOutroCTe(string chave, int cte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(chave))
                result = result.Where(o => o.Chave == chave);

            if (cte > 0)
                result = result.Where(o => o.CTEs.Any(c => c.Codigo != cte));

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNCM(string ncm, decimal pesoBruto, decimal pesoLiquodo, double cnpjDestinatario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.Destinatario.CPF_CNPJ == cnpjDestinatario && obj.Peso == pesoBruto && obj.PesoLiquido == pesoLiquodo && obj.Descricao == ncm && obj.NCM == ncm && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros select obj;
            result = result.Where(obj => obj.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorDescricao(string descricao, decimal pesoBruto, decimal pesoLiquodo, double cnpjDestinatario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.Destinatario.CPF_CNPJ == cnpjDestinatario && obj.Peso == pesoBruto && obj.PesoLiquido == pesoLiquodo && obj.Descricao == descricao && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros select obj;
            result = result.Where(obj => obj.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorChave(string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = ObterQueryBuscarPorChave(chave);

            query = query.Where(obj => obj.nfAtiva == true);

            return query.Fetch(o => o.Canhoto).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscaPorChaves(List<string> chaves)
        {
            const int LOTE = 1000;
            var resultado = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            if (chaves == null || chaves.Count == 0)
                return resultado;

            for (int i = 0; i < chaves.Count; i += LOTE)
            {
                var lote = chaves.GetRange(i, Math.Min(LOTE, chaves.Count - i));

                // Remove o Fetch por agora
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                                   .Where(x => lote.Contains(x.Chave) && x.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && x.nfAtiva);

                resultado.AddRange(query.ToList());
            }

            return resultado;
        }



        public Task<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorChaveAsync(string chave, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = ObterQueryBuscarPorChave(chave);

            query = query.Where(obj => obj.nfAtiva == true);

            return query.Fetch(o => o.Canhoto).FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorChaveTipoDocumento(string chave, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = ObterQueryBuscarPorChaveTipoDocumento(chave, tipoDocumento);

            query = query.Where(obj => obj.nfAtiva == true);

            return query.Fetch(o => o.Canhoto).FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorChaveAsync(string chave, TipoNotaFiscalIntegrada tipoNotaFiscalIntegrada)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = ObterQueryBuscarPorChave(chave);

            query = query.Where(xmlNotaFiscal => xmlNotaFiscal.TipoNotaFiscalIntegrada == tipoNotaFiscalIntegrada);

            return await query.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorChave(List<string> chaves)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            int quantidadeRegistrosConsultarPorVez = 2000;
            int quantidadeConsultas = chaves.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> registrosRetornar = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                registrosRetornar.AddRange(query.WithOptions(o => o.SetTimeout(600))
                                                .Where(o => chaves.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.Chave))
                                                .Fetch(o => o.Canhoto)
                                                .ToList());

            return registrosRetornar;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCargaPedidoComCTeTerceiro(int codigoCargaPedido)
        {
            IQueryable<int> queryPedidoCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido)
                .Select(a => a.CTeTerceiro.Codigo);

            IQueryable<string> queryCTeTerceiroNFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>()
                .Where(o => queryPedidoCTeParaSubContratacao.Contains(o.CTeTerceiro.Codigo))
                .Select(a => a.Chave);

            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                .Where(o => queryCTeTerceiroNFe.Contains(o.Chave))
                .Fetch(o => o.Canhoto)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorChave(string chave, bool ativa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.Chave == chave && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;
            if (ativa)
                result = result.Where(obj => obj.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorChaves(List<string> chaves)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where chaves.Contains(obj.Chave) && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;
            result = result.Where(obj => obj.nfAtiva == true);
            return result.ToList();
        }


        public List<(int codigo, string chaveNota, int codigoCarga, bool notaValida, int numeroNota, string nomeEmitente)> BuscarDadosPorChaves(List<string> chaves)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = from obj in query where chaves.Contains(obj.XMLNotaFiscal.Chave) && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva);
            return result.Select(x => ValueTuple.Create(x.XMLNotaFiscal.Codigo, x.XMLNotaFiscal.Chave, x.CargaPedido.Carga.Codigo, x.XMLNotaFiscal.Emitente.ExcecaoCheckinFilaH && x.XMLNotaFiscal.ModalidadeFrete == ModalidadePagamentoFrete.Pago, x.XMLNotaFiscal.Numero, x.XMLNotaFiscal.Emitente.Nome)).ToList();
        }

        public bool ExistePorChave(string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            query = query.Where(obj => obj.nfAtiva && obj.Chave == chave && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorLocalidadeInicioELocalidadeTerminoPrestacao(int codigoCarga, int codigoInicioPrestacao, int codigoTerminoPrestacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva && ((obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.XMLNotaFiscal.Destinatario.Localidade.Codigo == codigoTerminoPrestacao) || (obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.XMLNotaFiscal.Emitente.Localidade.Codigo == codigoTerminoPrestacao)) select obj;

            if (codigoInicioPrestacao > 0)
                resut = resut.Where(obj => ((obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.XMLNotaFiscal.Emitente.Localidade.Codigo == codigoInicioPrestacao) || (obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.XMLNotaFiscal.Destinatario.Localidade.Codigo == codigoInicioPrestacao)));

            return resut.Select(obj => obj.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarTodasPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva select obj;


            return resut.Select(obj => obj.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarTodasPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva select obj;


            return resut.Select(obj => obj.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarTodosDocumentosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var resut = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva select obj;
            return resut.Select(obj => obj.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCargaPedidoETipoDocumentos(int codigoCargaPedido, TipoDocumento tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var resut = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.TipoDocumento == tipoDocumento && obj.XMLNotaFiscal.nfAtiva select obj;
            return resut.Select(obj => obj.XMLNotaFiscal).Fetch(obj => obj.Canhoto).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCargaParaMDFe(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query
                                                                                         where
                                                                                             obj.CargaPedido.Carga.Codigo == codigoCarga
                                                                                             //&& obj.XMLNotaFiscal.Empresa.Codigo == empresa
                                                                                             && obj.XMLNotaFiscal.nfAtiva
                                                                                             && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe
                                                                                         select obj;

            return resut
                .Select(obj => obj.XMLNotaFiscal)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;

            return resut.Select(obj => obj.XMLNotaFiscal).ToList();
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao?>> BuscarFormaIntegracaoPorCargaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;

            return resut.Select(obj => obj.XMLNotaFiscal.FormaIntegracao).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCargaEPedido(int codigoCarga, int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.CargaPedido.Pedido.Codigo == codigoPedido select obj;

            return resut.Select(obj => obj.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCargas(List<int> codigosCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where codigosCarga.Contains(obj.CargaPedido.Carga.Codigo) select obj;

            return resut.Select(obj => obj.XMLNotaFiscal).Distinct().ToList();
        }

        public bool ExistePorCargasEChave(List<int> codigosCarga, string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where codigosCarga.Contains(obj.CargaPedido.Carga.Codigo) && obj.XMLNotaFiscal.Chave == chave select obj;

            return resut.Select(obj => obj.XMLNotaFiscal).Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCargasEntrega(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> resut = from obj in query where codigos.Contains(obj.CargaEntrega.Codigo) select obj;

            return resut.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCargaEntrega(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> resut = from obj in query where obj.CargaEntrega.Codigo == codigo select obj;

            return resut.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();
        }

        public List<int> BuscarCodigosPorCargaEntrega(int codigoCargaEntregaNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            var resut = from obj in query where obj.CargaEntrega.Codigo == codigoCargaEntregaNotaFiscal select obj;

            return resut.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo).ToList();
        }

        public bool ExistePorCargaEntrega(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> consultaCargaEntregaNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                .Where(o => o.CargaEntrega.Codigo == codigo);

            return consultaCargaEntregaNotaFiscal.Any();
        }

        public List<int> BuscarCodigosRelevantesParaFretePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> queryStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.IrrelevanteParaFrete == false && !queryStage.Any(x => x.Pedido.Codigo == o.CargaPedido.Pedido.Codigo && !x.Stage.RelevanciaCusto));

            return consultaPedidoXMLNotaFiscal.Select(o => o.XMLNotaFiscal.Codigo).ToList();
        }

        public int BuscarQuantidadesRelevantesParaFretePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == codigoCarga && !o.PedidoXMLNotaFiscal.XMLNotaFiscal.IrrelevanteParaFrete);

            return consultaPedidoXMLNotaFiscal.Select(o => o.CargaCTe.Codigo).Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resultPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            resultPedidoXMLNotaFiscal = resultPedidoXMLNotaFiscal.Where(obj => result.Select(o => o.Carga).Contains(obj.CargaPedido.Carga));

            return resultPedidoXMLNotaFiscal.Select(obj => obj.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorEstadoInicioEEstadoTerminoPrestacao(int codigoCarga, string estadoInicioPrestacao, string estadoTerminoPrestacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> resut = from obj in query where obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva && ((obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && (obj.XMLNotaFiscal.Destinatario.Localidade.Estado.Sigla == estadoTerminoPrestacao || obj.XMLNotaFiscal.Destinatario.Localidade.Estado.Sigla == "EX")) || (obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.XMLNotaFiscal.Emitente.Localidade.Estado.Sigla == estadoTerminoPrestacao)) select obj;

            if (!string.IsNullOrWhiteSpace(estadoInicioPrestacao))
                resut = resut.Where(obj => ((obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.XMLNotaFiscal.Emitente.Localidade.Estado.Sigla == estadoInicioPrestacao) || (obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.XMLNotaFiscal.Destinatario.Localidade.Estado.Sigla == estadoInicioPrestacao)));

            return resut.Select(obj => obj.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorNumeroVBELNIntercementSemCarga(string VBELN)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.NumeroDT == VBELN && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;
            result = result.Where(obj => obj.nfAtiva == true);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNumeroSemCarga(int numero, double emitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query
                                                                                    where (obj.Numero == numero)
                                                                  && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe
                                                                                    select obj;
            result = result.Where(obj => obj.nfAtiva == true);

            if (emitente > 0)
                result = result.Where(obj => obj.Emitente.CPF_CNPJ == emitente);

            return result.OrderByDescending(obj => obj.DataEmissao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorNumeroOuNumeroPedidoSemCarga(int numero, string numeroPedido, double emitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query
                                                                                    where (obj.Numero == numero || (obj.NumeroDT == numeroPedido && numero == 0) || (obj.NumeroPedido == numero && obj.NumeroPedido > 0))
                                                                  && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe
                                                                                    select obj;
            result = result.Where(obj => obj.nfAtiva == true);

            if (emitente > 0)
                result = result.Where(obj => obj.Emitente.CPF_CNPJ == emitente);

            return result.FirstOrDefault();
        }

        public string BuscarPINSuframaPorDocumentoCTe(Dominio.Entidades.DocumentosCTE documento, int numeroDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = (from obj in query where obj.nfAtiva select obj);

            if (documento.ModeloDocumentoFiscal == null || documento.ModeloDocumentoFiscal.Numero == "55")
                result = result.Where(obj => obj.Chave == documento.ChaveNFE);
            else if (documento.ModeloDocumentoFiscal.Numero == "01")
                result = result.Where(obj => obj.Numero == numeroDocumento && obj.Serie == documento.Serie);
            else
            {
                result = result.Where(obj => obj.Numero == numeroDocumento && obj.Volumes == documento.Volume);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = result.FirstOrDefault();

                if (xmlNotaFiscal == null)
                {
                    result = (from obj in query where obj.nfAtiva select obj);
                    result = result.Where(obj => obj.Numero == numeroDocumento);
                }
                else
                    return xmlNotaFiscal.PINSUFRAMA;
            }

            return result.FirstOrDefault()?.PINSUFRAMA ?? "";
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPorDocumentoCTe(Dominio.Entidades.DocumentosCTE documento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = (from obj in query where obj.nfAtiva select obj);

            if (documento.ModeloDocumentoFiscal == null || documento.ModeloDocumentoFiscal.Numero == "55")
                result = result.Where(obj => obj.Chave == documento.ChaveNFE);
            else if (documento.ModeloDocumentoFiscal.Numero == "01")
                result = result.Where(obj => obj.Numero == int.Parse(documento.Numero) && obj.Serie == documento.Serie);
            else
            {
                result = result.Where(obj => obj.Numero == int.Parse(documento.Numero) && obj.Volumes == documento.Volume);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = result.FirstOrDefault();

                if (xmlNotaFiscal == null)
                {
                    result = (from obj in query where obj.nfAtiva select obj);
                    result = result.Where(obj => obj.Numero == int.Parse(documento.Numero));
                }
                else
                    return xmlNotaFiscal;
            }

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarCanhotoPendentePorDocumentoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = (from obj in query where obj.CTEs.Contains(cte) && obj.Canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente select obj);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = (from obj in query where obj.CTEs.Contains(cte) select obj);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarPorCodigoCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = (from obj in query where obj.CTEs.Any(o => o.Codigo == codigoCTe) select obj);

            return result.ToList();
        }

        public List<int> BuscarCodigosCTesPorCodigos(List<int> codigosXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            query = query.Where(o => codigosXMLNotaFiscal.Contains(o.Codigo));

            return query.Select(o => o.CTEs.Select(cte => cte.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public Task<List<int>> BuscarCodigosCTesPorCodigosAsync(List<int> codigosXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                .Where(o => codigosXMLNotaFiscal.Contains(o.Codigo));

            return query.Select(o => o.CTEs.Select(cte => cte.Codigo)).SelectMany(o => o).Distinct().ToListAsync(CancellationToken);
        }

        public List<int> BuscarCodigosCTesPorCodigo(int codigoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            query = query.Where(o => o.Codigo == codigoXMLNotaFiscal);

            return query.Select(o => o.CTEs.Select(cte => cte.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public Task<List<int>> BuscarCodigosCTesPorCodigoAsync(int codigoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                .Where(o => o.Codigo == codigoXMLNotaFiscal);

            return query.Select(o => o.CTEs.Select(cte => cte.Codigo)).SelectMany(o => o).Distinct().ToListAsync(CancellationToken);
        }

        public List<int> BuscarNotasPendentesGeradaoDANFE()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            query = query.Where(o => o.nfAtiva == true && (o.GerouPDF == false || o.GerouPDF == null) && o.Chave != "" && o.Chave != null && o.Chave.Length == 44 && o.DataEmissao.Year >= 2018);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarNotasPorStageECarga(int codigoStage, int codigocarga)
        {

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> consultaPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>()
                .Where(pedidoStage => pedidoStage.Stage.Codigo == codigoStage);

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(s => consultaPedidoStage.Any(d => d.Pedido.Codigo == s.CargaPedido.Pedido.Codigo && s.CargaPedido.Carga.Codigo == codigocarga));

            return query.Select(pedidoStage => pedidoStage.XMLNotaFiscal).Distinct().ToList();
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarNotasPorStageECarga(List<int> codigoStage, int codigocarga)
        {

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> consultaPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>()
                .Where(pedidoStage => codigoStage.Contains(pedidoStage.Stage.Codigo));

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(s => consultaPedidoStage.Any(d => d.Pedido.Codigo == s.CargaPedido.Pedido.Codigo && s.CargaPedido.Carga.Codigo == codigocarga));

            return query.Select(pedidoStage => pedidoStage.XMLNotaFiscal).Distinct().ToList();
        }

        public List<string> BuscarChavesNotasPorStage(int codigoStage)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> consultaPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>()
                .Where(pedidoStage => pedidoStage.Stage.Codigo == codigoStage);

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(s => consultaPedidoStage.Any(d => d.Pedido.Codigo == s.CargaPedido.Pedido.Codigo));

            return query.Select(pedidoStage => pedidoStage.XMLNotaFiscal.Chave).Distinct().ToList();
        }

        public bool StageRecebeuAsNotas(int codigoStage)
        {

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> consultaPedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>()
                .Where(pedidoStage => pedidoStage.Stage.Codigo == codigoStage);

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(s => consultaPedidoStage.Any(d => d.Pedido.Codigo == s.CargaPedido.Pedido.Codigo));

            return query.Select(pedidoStage => pedidoStage.XMLNotaFiscal).Any();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosObservacaoNotaFiscal> BuscarDadosObservacaoNotaFiscal(List<int> codigosNotas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosObservacaoNotaFiscal> result = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosObservacaoNotaFiscal>();
            string sqlQuery = @"select Xm.NF_NUMERO_CONTROLE_CLIENTE NumeroControleCliente,
                    Xm.NF_OBSERVACAO_NOTA_FISCAL_PARA_CTE ObservacaoNotaFiscalParaCTe  
                    from T_XML_NOTA_FISCAL Xm
                    where ((Xm.NF_NUMERO_CONTROLE_CLIENTE is not null and Xm.NF_NUMERO_CONTROLE_CLIENTE  <> '') 
                    OR (Xm.NF_OBSERVACAO_NOTA_FISCAL_PARA_CTE is not null and Xm.NF_OBSERVACAO_NOTA_FISCAL_PARA_CTE  <> ''))
                    AND Xm.NFX_CODIGO in ( :codigos )";
            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameterList("codigos", codigosNotas);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosObservacaoNotaFiscal)));
            return query.List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosObservacaoNotaFiscal>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.NotaFiscal> BuscarInfosNotaFiscal(int codigoCarga)
        {

            string sqlQuery = @"SELECT 
                                XmlNotaFiscal.NF_NUMERO AS NumeroNFe,
                                XmlNotaFiscal.NF_CHAVE AS ChaveNFe,
                                XmlNotaFiscal.NF_SERIE AS SerieNFe,
                                CASE
                                    WHEN destinatario.CLI_CODIGO_INTEGRACAO IS NOT NULL THEN destinatario.CLI_CODIGO_INTEGRACAO
                                    ELSE destinatario.CLI_NOME
                                END AS ClienteNFe,
                                localidades.UF_SIGLA AS EstadoNFe,
                                pedido.PED_NUMERO_PEDIDO_EMBARCADOR AS RemessaNFe
                            FROM 
                                T_CARGA_PEDIDO cargaPedido
                            JOIN 
                                T_PEDIDO pedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO
                            JOIN 
                                T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.CPE_CODIGO = cargaPedido.CPE_CODIGO
                            JOIN                              
                                T_XML_NOTA_FISCAL XmlNotaFiscal ON XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
                            JOIN                              
                                T_LOCALIDADES localidades ON pedido.LOC_CODIGO_DESTINO = localidades.LOC_CODIGO
                            JOIN                              
                                T_CLIENTE destinatario ON pedido.CLI_CODIGO = destinatario.CLI_CGCCPF
                            WHERE 
                                cargaPedido.CAR_CODIGO = :codigoCarga";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetParameter("codigoCarga", codigoCarga);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.NotaFiscal)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.NotaFiscal>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.CTe> BuscarCTesRelatorioDetalheCTeMDFePorCarga(int codigoCarga)
        {
            string sqlQuery = @"SELECT
	                              CTe.CON_NUM as NumeroCTe,
	                              CTe.CON_CHAVECTE as ChaveCTe,
	                              EmpresaSerie.ESE_NUMERO as SerieCTe,
	                              ClienteDestinatarioCTe.CLI_CODIGO_INTEGRACAO as CodigoIntegracaoClienteDestinatarioCTe,
	                              ClienteDestinatarioCTe.CLI_NOME as NomeClienteDestinatarioCTe,
	                              LocalidadeClienteDestinatarioCTe.UF_SIGLA as EstadoCTe,
	                              CTe.CON_NUMERO_PEDIDO as RemessaCTe,
	                              ModeloDocumentoFiscal.MOD_ABREVIACAO as TipoDocumento
                                FROM T_CARGA_CTE cargaCTe
		                            JOIN T_CTE CTe ON CTe.CON_CODIGO = cargaCTe.CON_CODIGO
			                            LEFT JOIN T_MODDOCFISCAL ModeloDocumentoFiscal ON ModeloDocumentoFiscal.MOD_CODIGO = CTe.CON_MODELODOC
			                            LEFT JOIN T_EMPRESA_SERIE EmpresaSerie ON EmpresaSerie.ESE_CODIGO = CTe.CON_SERIE
			                            LEFT JOIN T_CTE_PARTICIPANTE DestinatarioCTe ON DestinatarioCTe.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE
				                            LEFT JOIN T_CLIENTE ClienteDestinatarioCTe ON ClienteDestinatarioCTe.CLI_CGCCPF = DestinatarioCTe.CLI_CODIGO
					                            LEFT JOIN T_LOCALIDADES LocalidadeClienteDestinatarioCTe ON LocalidadeClienteDestinatarioCTe.LOC_CODIGO = ClienteDestinatarioCTe.LOC_CODIGO
                                WHERE
                                    cargaCTe.CAR_CODIGO = :codigoCarga
		                            AND CTe.CON_STATUS <> 'C'
                                ORDER BY ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_EMISSAO
            ";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetParameter("codigoCarga", codigoCarga);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.CTe)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CTe>();
        }

        #region Gestão de Notas Fiscais

        public int ContarConsultaGestaoNotasFiscais(FiltroGestaoNotasFiscais filtros)
        {
            var where = ObterWhereConsultaGestaoNotasFiscais(filtros);

            string sqlQuery = $@"SELECT
                                 COUNT(XMLNotaFiscal.NFX_CODIGO)
                                 FROM T_XML_NOTA_FISCAL XMLNotaFiscal
                                 LEFT JOIN T_CLIENTE Emitente ON Emitente.CLI_CGCCPF = XMLNotaFiscal.CLI_CODIGO_REMETENTE
                                 LEFT JOIN T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal ON CTeXMLNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO
                                 LEFT JOIN T_CTE CTe ON CTe.CON_CODIGO = CTeXMLNotaFiscal.CON_CODIGO
                                 LEFT JOIN T_LOCALIDADES LocalidadeOrigem ON LocalidadeOrigem.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO 
                                 LEFT JOIN T_LOCALIDADES LocalidadeDestino ON LocalidadeDestino.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO 
                                 LEFT JOIN T_CTE_PARTICIPANTE RemetenteCTe ON RemetenteCTe.PCT_CODIGO = CTe.CON_REMETENTE_CTE 
                                 LEFT JOIN T_CTE_PARTICIPANTE ExpedidorCTe ON ExpedidorCTe.PCT_CODIGO = CTe.CON_EXPEDIDOR_CTE 
                                 LEFT JOIN T_CTE_PARTICIPANTE RecebedorCTe ON RecebedorCTe.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE 
                                 LEFT JOIN T_CTE_PARTICIPANTE DestinatarioCTe ON DestinatarioCTe.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE 
                                 {where.WhereClause}";

            var sqlDinamico = new SQLDinamico(sqlQuery, where.Parametros);

            ISQLQuery query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return query.UniqueResult<int>();
        }

        public IList<GestaoNotasFiscais> ConsultarGestaoNotasFiscais(FiltroGestaoNotasFiscais filtros, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            var where = ObterWhereConsultaGestaoNotasFiscais(filtros);

            string sqlQuery = $@"SELECT
                                 XMLNotaFiscal.NFX_CODIGO Codigo,
                                 CTe.CON_CODIGO CodigoCTe,
                                 CTe.CON_NUM NumeroCTe,
                                 CTe.CON_VALOR_RECEBER ValorCTe,
                                 CTe.CON_STATUS SituacaoCTe,
                                 XMLNotaFiscal.NF_NUMERO NumeroNFe,
                                 XMLNotaFiscal.NF_SERIE Serie,
                                 XMLNotaFiscal.NF_DATA_EMISSAO DataEmissao,
                                 XMLNotaFiscal.NF_CHAVE ChaveNFe,
                                 Emitente.CLI_FISJUR TipoEmitente,
                                 Emitente.CLI_CGCCPF CPFCNPJEmitente,
                                 Emitente.CLI_NOME NomeEmitente,
                                 XMLNotaFiscal.NF_PESO Peso,
                                 XMLNotaFiscal.NF_VALOR Valor,
                                 ISNULL(ExpedidorCTe.PCT_NOME, RemetenteCTe.PCT_NOME) ClienteOrigem,
                                 LocalidadeOrigem.LOC_DESCRICAO CidadeOrigem,
                                 LocalidadeOrigem.UF_SIGLA UFOrigem,
                                 ISNULL(RecebedorCTe.PCT_NOME, DestinatarioCTe.PCT_NOME) ClienteDestino,
                                 LocalidadeDestino.LOC_DESCRICAO CidadeDestino,
                                 LocalidadeDestino.UF_SIGLA UFDestino,
                                 ISNULL(substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000), XMLNotaFiscal.NF_PLACA_VEICULO_NF) Veiculo,
                                 substring((select ', ' + motoristaCTe1.CMO_NOME_MOTORISTA from T_CTE_MOTORISTA motoristaCTe1 where motoristaCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Motorista,
                                 XMLNotaFiscal.NF_PRODUTO Produto
                                 FROM T_XML_NOTA_FISCAL XMLNotaFiscal
                                 LEFT JOIN T_CLIENTE Emitente ON Emitente.CLI_CGCCPF = XMLNotaFiscal.CLI_CODIGO_REMETENTE
                                 LEFT JOIN T_CTE_XML_NOTAS_FISCAIS CTeXMLNotaFiscal ON CTeXMLNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO
                                 LEFT JOIN T_CTE CTe ON CTe.CON_CODIGO = CTeXMLNotaFiscal.CON_CODIGO
                                 LEFT JOIN T_LOCALIDADES LocalidadeOrigem ON LocalidadeOrigem.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO 
                                 LEFT JOIN T_LOCALIDADES LocalidadeDestino ON LocalidadeDestino.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO 
                                 LEFT JOIN T_CTE_PARTICIPANTE RemetenteCTe ON RemetenteCTe.PCT_CODIGO = CTe.CON_REMETENTE_CTE 
                                 LEFT JOIN T_CTE_PARTICIPANTE ExpedidorCTe ON ExpedidorCTe.PCT_CODIGO = CTe.CON_EXPEDIDOR_CTE 
                                 LEFT JOIN T_CTE_PARTICIPANTE RecebedorCTe ON RecebedorCTe.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE 
                                 LEFT JOIN T_CTE_PARTICIPANTE DestinatarioCTe ON DestinatarioCTe.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE 
                                 {where.WhereClause}
                                 ORDER BY {propriedadeOrdenar} {dirOrdena}";

            if (inicio > 0 || limite > 0)
                sqlQuery += $" OFFSET {inicio} ROWS FETCH FIRST {limite} ROWS ONLY";

            var sqlDinamico = new SQLDinamico(sqlQuery, where.Parametros);

            ISQLQuery query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(GestaoNotasFiscais)));

            return query.List<GestaoNotasFiscais>();
        }

        private (string WhereClause, List<ParametroSQL> Parametros) ObterWhereConsultaGestaoNotasFiscais(FiltroGestaoNotasFiscais filtros)
        {
            string where = string.Empty;
            var parametros = new List<ParametroSQL>();

            if (filtros.CodigoCarga > 0)
            {
                where += $"AND EXISTS (SELECT CargaCTe.CAR_CODIGO FROM T_CARGA_CTE CargaCTe WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO AND CargaCTe.CAR_CODIGO = :CARGACTE_CAR_CODIGO) ";
                parametros.Add(new ParametroSQL("CARGACTE_CAR_CODIGO", filtros.CodigoCarga));
            }

            if (filtros.CodigoCTe > 0)
                where += $"AND CTe.CON_CODIGO = {filtros.CodigoCTe} ";

            if (filtros.CodigoEmpresa > 0)
                where += $"AND CTe.EMP_CODIGO = {filtros.CodigoEmpresa} ";

            if (filtros.CPFCNPJEmitente > 0D)
                where += $"AND XMLNotaFiscal.CLI_CODIGO_REMETENTE = {filtros.CPFCNPJEmitente:F0} ";

            if (filtros.DataEmissaoCargaInicial.HasValue || filtros.DataEmissaoCargaFinal.HasValue)
            {
                where += "AND EXISTS (SELECT Carga.CAR_CODIGO FROM T_CARGA Carga INNER JOIN T_CARGA_CTE CargaCTe ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO  AND Carga.CAR_DATA_CRIACAO < '2021-01-01'";

                if (filtros.DataEmissaoCargaInicial.HasValue)
                    where += $"AND Carga.CAR_DATA_CRIACAO >= '{filtros.DataEmissaoCargaInicial.Value:yyyy-MM-dd}' ";

                if (filtros.DataEmissaoCargaFinal.HasValue)
                    where += $"AND Carga.CAR_DATA_CRIACAO < '{filtros.DataEmissaoCargaFinal.Value.AddDays(1):yyyy-MM-dd}' ";

                where += ") ";
            }

            if (filtros.DataEmissaoCTeInicial.HasValue)
                where += $"AND CTe.CON_DATAHORAEMISSAO >= '{filtros.DataEmissaoCTeInicial.Value:yyyy-MM-dd}' ";

            if (filtros.DataEmissaoCTeFinal.HasValue)
                where += $"AND CTe.CON_DATAHORAEMISSAO < '{filtros.DataEmissaoCTeFinal.Value.AddDays(1):yyyy-MM-dd}' ";

            if (filtros.DataEmissaoNotaFiscalInicial.HasValue)
                where += $"AND XMLNotaFiscal.NF_DATA_EMISSAO >= '{filtros.DataEmissaoNotaFiscalInicial.Value:yyyy-MM-dd}' ";

            if (filtros.DataEmissaoNotaFiscalFinal.HasValue)
                where += $"AND XMLNotaFiscal.NF_DATA_EMISSAO < '{filtros.DataEmissaoNotaFiscalFinal.Value.AddDays(1):yyyy-MM-dd}' ";

            if (filtros.Numero > 0)
                where += $"AND XMLNotaFiscal.NF_NUMERO = {filtros.Numero} ";

            if (filtros.Serie > 0)
                where += $"AND XMLNotaFiscal.NF_SERIE = '{filtros.Serie}' ";

            if (!string.IsNullOrWhiteSpace(filtros.Produto))
                where += $"AND XMLNotaFiscal.NF_PRODUTO like '%{filtros.Produto}%' ";

            if (!string.IsNullOrWhiteSpace(filtros.Veiculo))
            {
                where += $"AND ISNULL(substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000), XMLNotaFiscal.NF_PLACA_VEICULO_NF) LIKE :VEICULO1_VEI_PLACA ";
                parametros.Add(new ParametroSQL("VEICULO1_VEI_PLACA", $"%{filtros.Veiculo}%"));
            }

            if (filtros.PossuiCTe.HasValue)
                where += $"AND CTe.CON_CODIGO IS {(filtros.PossuiCTe.Value ? "NOT" : "")} NULL ";

            return ((where.Length > 0 ? "WHERE " + where.Remove(0, 4) : where), parametros);
        }

        #endregion

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLNotaFiscalPorCargaPedido(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => obj.Codigo == codigo);

            return query
                .SelectMany(obj => obj.NotasFiscais)
                .Select(obj => obj.XMLNotaFiscal)
                .ToList();
        }
        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLNotaFiscalPorCargaPedido(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return query
                .SelectMany(obj => obj.NotasFiscais)
                .Select(obj => obj.XMLNotaFiscal)
                .ToList();
        }

        public int ContarXMLNotaFiscalPorCargaPedido(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => obj.Codigo == codigo);

            return query
                .SelectMany(obj => obj.NotasFiscais)
                .Select(obj => obj.XMLNotaFiscal).Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoPorPedidoXMLNotaFiscal(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> result = from obj in query where obj.Codigo == codigo select obj.CargaPedido;

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> _BuscarNotasSemCanhoto(DateTime dataBase)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> queryCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> resultCanhoto = from obj in queryCanhoto where obj.XMLNotaFiscal != null select obj.XMLNotaFiscal;

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = from obj in query
                                                                                          where
                                                                                             !resultCanhoto.Contains(obj.XMLNotaFiscal)
                                                                                             && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe
                                                                                             && obj.XMLNotaFiscal.nfAtiva == true
                                                                                             && obj.XMLNotaFiscal.DataEmissao > dataBase
                                                                                          //&& obj.CargaPedido.Carga.TipoOperacao.Codigo != 1001
                                                                                          select obj;

            return result;
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarCanhotoExistente(int numero, double emitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = from obj in query where obj.Numero == numero && obj.Emitente.CPF_CNPJ == emitente select obj;

            return result.FirstOrDefault();
        }

        public List<int> BuscarNotasSemCanhoto(DateTime dataBase)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = _BuscarNotasSemCanhoto(dataBase);

            return result
                .Select(o => o.Codigo)
                .Timeout(300)
                .ToList();
        }

        public void AtualizarSituacaoNotasFiscaisPorEntrega(int codigoCargaEntrega, List<SituacaoNotaFiscal> situacoesNotasFiscaisDesconsiderar, SituacaoNotaFiscal novaSituacaoNotaFiscal)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("update NotasFiscais ");
            sql.Append($"  set SituacaoNotaFiscal = {(int)novaSituacaoNotaFiscal} ");
            if (novaSituacaoNotaFiscal == SituacaoNotaFiscal.AgEntrega)
                sql.Append($"  , UltimaSituacaoEntregaDevolucao = null ");
            sql.Append("  from ( ");
            sql.Append("           select XmlNotaFiscal.NF_SITUACAO_ENTREGA SituacaoNotaFiscal, XmlNotaFiscal.NF_ULTIMA_SITUACAO_ENTREGA_DEVOLUCAO UltimaSituacaoEntregaDevolucao ");
            sql.Append("             from T_CARGA_ENTREGA CargaEntrega ");
            sql.Append("             join T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido on CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
            sql.Append("             join T_CARGA_PEDIDO CargaPedido on CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
            sql.Append("             join T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal on PedidoXmlNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
            sql.Append("             join T_XML_NOTA_FISCAL XmlNotaFiscal on XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO ");
            sql.Append($"           where CargaEntrega.CEN_CODIGO = {codigoCargaEntrega} ");
            sql.Append("              and (case when CargaEntrega.CEN_COLETA = 1 then XmlNotaFiscal.CLI_CODIGO_REMETENTE else XmlNotaFiscal.CLI_CODIGO_DESTINATARIO end) = CargaEntrega.CLI_CODIGO_ENTREGA ");

            if (situacoesNotasFiscaisDesconsiderar?.Count > 0)
                sql.Append($"         and XmlNotaFiscal.NF_SITUACAO_ENTREGA not in ({string.Join(", ", (from o in situacoesNotasFiscaisDesconsiderar select (int)o))}) ");

            sql.Append("       ) as NotasFiscais ");

            UnitOfWork.Sessao
                .CreateSQLQuery(sql.ToString())
                .ExecuteUpdate();
        }

        public bool ExisteNotasFiscaisPorEntrega(int codigoCargaEntrega, List<SituacaoNotaFiscal> situacoesNotasFiscaisDesconsiderar, SituacaoNotaFiscal? situacaoNotaFiscal)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" select count(XmlNotaFiscal.NFX_CODIGO) ");
            sql.Append("   from T_CARGA_ENTREGA CargaEntrega ");
            sql.Append("   join T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido on CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
            sql.Append("   join T_CARGA_PEDIDO CargaPedido on CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
            sql.Append("   join T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal on PedidoXmlNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
            sql.Append("   join T_XML_NOTA_FISCAL XmlNotaFiscal on XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO ");
            sql.Append($" where CargaEntrega.CEN_CODIGO = {codigoCargaEntrega} ");
            sql.Append("    and (case when CargaEntrega.CEN_COLETA = 1 then XmlNotaFiscal.CLI_CODIGO_REMETENTE else XmlNotaFiscal.CLI_CODIGO_DESTINATARIO end) = CargaEntrega.CLI_CODIGO_ENTREGA ");

            if (situacoesNotasFiscaisDesconsiderar?.Count > 0)
                sql.Append($" and XmlNotaFiscal.NF_SITUACAO_ENTREGA not in ({string.Join(", ", (from o in situacoesNotasFiscaisDesconsiderar select (int)o))}) ");

            if (situacaoNotaFiscal.HasValue)
                sql.Append($" and XmlNotaFiscal.NF_SITUACAO_ENTREGA = {(int)situacaoNotaFiscal} ");

            int totalRegistros = UnitOfWork.Sessao
                .CreateSQLQuery(sql.ToString())
                .SetTimeout(600)
                .UniqueResult<int>();

            return totalRegistros > 0;
        }

        public Dominio.Entidades.Cliente BuscarFilialPorNumeroTransporteDoPedido(string numeroTransportePedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>()
                .Where(o => o.NumeroTransporte == numeroTransportePedido);

            return consulta.Select(a => a.Destinatario).FirstOrDefault();
        }

        public bool ExisteFacturaFakePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            return query.Any(x => x.CargaPedido.Carga.Codigo == codigoCarga && x.XMLNotaFiscal.FaturaFake == true);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ConsultarNotasComplementar(int codigoCTe, double codigoDestinatario, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = _ConsultarNotasComplementar(codigoCTe, codigoDestinatario);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ConsultarNotasSaida(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalSaida filtroPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = ConsultarNotasSaida(filtroPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaNotasComplementar(int codigoCTe, double codigoDestinatario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = _ConsultarNotasComplementar(codigoCTe, codigoDestinatario);

            return result.Count();
        }

        public int ContarConsultarNotasSaida(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalSaida filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = ConsultarNotasSaida(filtroPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObterQueryBuscarPorChave(string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            query = from obj in query where obj.Chave == chave && obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObterQueryBuscarPorChaveTipoDocumento(string chave, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            query = from obj in query where obj.Chave == chave && obj.TipoDocumento == tipoDocumento select obj;

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> _ConsultarNotasComplementar(int codigoCTe, double codigoDestinatario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.nfAtiva select obj;

            // Filtros
            if (codigoCTe > 0)
                result = result.Where(obj => obj.CTEs.Any(o => o.Codigo == codigoCTe));

            if (codigoDestinatario > 0)
                result = result.Where(o => o.Destinatario.CPF_CNPJ == codigoDestinatario);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ConsultarNotasSaida(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalSaida filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.nfAtiva select obj;

            if (filtroPesquisa.TipoOperacaoNotaFiscal != TipoOperacaoNotaFiscal.Entrada)
                result = result.Where(obj => obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida);

            if (filtroPesquisa.NumeroNotaFiscal > 0)
                result = result.Where(obj => obj.Numero == filtroPesquisa.NumeroNotaFiscal);

            if (filtroPesquisa.DataEmissaoInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao >= filtroPesquisa.DataEmissaoInicial);

            if (filtroPesquisa.DataEmissaoFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao <= filtroPesquisa.DataEmissaoFinal);

            if (!string.IsNullOrEmpty(filtroPesquisa.Chave))
                result = result.Where(obj => obj.Chave == filtroPesquisa.Chave);

            if (!string.IsNullOrEmpty(filtroPesquisa.Serie))
                result = result.Where(obj => obj.Serie == filtroPesquisa.Serie);

            if (filtroPesquisa.CodigoRemetente > 0)
                result = result.Where(obj => obj.Emitente.CPF_CNPJ == filtroPesquisa.CodigoRemetente);

            if (filtroPesquisa.CodigoDestinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == filtroPesquisa.CodigoDestinatario);

            if (filtroPesquisa.Empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtroPesquisa.Empresa);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarNotasPorCargaECliente(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscal filtroPesquisaXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> result = from obj in query where obj.nfAtiva select obj;

            if (filtroPesquisaXMLNotaFiscal.CodigoCliente > 0)
                result = result.Where(o => o.Destinatario.CPF_CNPJ == filtroPesquisaXMLNotaFiscal.CodigoCliente);

            if (filtroPesquisaXMLNotaFiscal.CodigoCarga > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                result = result.Where(o => (from obj in queryPedidoXMLNotaFiscal where obj.CargaPedido.Carga.Codigo == filtroPesquisaXMLNotaFiscal.CodigoCarga select obj.XMLNotaFiscal.Codigo).Contains(o.Codigo));
            }

            if (filtroPesquisaXMLNotaFiscal.CodigoPedido > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                result = result.Where(o => (from obj in queryPedidoXMLNotaFiscal where obj.CargaPedido.Pedido.Codigo == filtroPesquisaXMLNotaFiscal.CodigoPedido select obj.XMLNotaFiscal.Codigo).Contains(o.Codigo));
            }

            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.TorreControle.DevolucaoNotasFiscais> ConsultaRelatorioDevolucaoNotasFiscais(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDevolucaoNotasFiscais = new Repositorio.Embarcador.TorreControle.Consulta.ConsultaDevolucaoNotasFiscais().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaDevolucaoNotasFiscais.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.TorreControle.DevolucaoNotasFiscais)));

            return consultaDevolucaoNotasFiscais.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.DevolucaoNotasFiscais>();
        }

        public int ContarConsultaRelatorioDevolucaoNotasFiscais(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaDevolucaoNotasFiscais = new Repositorio.Embarcador.TorreControle.Consulta.ConsultaDevolucaoNotasFiscais().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaDevolucaoNotasFiscais.SetTimeout(1200).UniqueResult<int>();
        }

    }
}
