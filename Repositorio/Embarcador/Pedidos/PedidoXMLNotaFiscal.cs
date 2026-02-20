using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoXMLNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>
    {
        public PedidoXMLNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PedidoXMLNotaFiscal(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<double> BuscarCpfCnpjDestinatariosPorCargaPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = cargaPedidos.Count / quantidadeRegistrosConsultarPorVez;

            List<double> cpfCnpjsRetornar = new List<double>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                cpfCnpjsRetornar.AddRange(query.Where(o => cargaPedidos.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CargaPedido)).Select(o => o.XMLNotaFiscal.Destinatario.CPF_CNPJ).ToList());

            return cpfCnpjsRetornar.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaECtes(List<int> codigosCTe, int codigoCarga)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            int take = 1000;
            int start = 0;
            while (start < codigosCTe?.Count)
            {
                List<int> tmp = codigosCTe.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                query = query.Where(o => o.XMLNotaFiscal.nfAtiva == true && o.CargaPedido.Carga.Codigo == codigoCarga && tmp.Contains(o.XMLNotaFiscal.Codigo));
                query = query
                    .Fetch(obj => obj.XMLNotaFiscal)
                    .ThenFetch(obj => obj.Canhoto);

                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaAgrupadaECtes(List<int> codigosCTe, int codigoCarga)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            int take = 1000;
            int start = 0;
            while (start < codigosCTe?.Count)
            {
                List<int> tmp = codigosCTe.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                query = query.Where(o => o.XMLNotaFiscal.nfAtiva == true && o.CargaPedido.Carga.CargaAgrupamento.Codigo == codigoCarga && tmp.Contains(o.XMLNotaFiscal.Codigo));
                query = query
                    .Fetch(obj => obj.XMLNotaFiscal)
                    .ThenFetch(obj => obj.Canhoto);

                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedidosENotasFiscais(List<int> codigosCargas, List<int> codigosNotasFiscais)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(notaPedido => codigosCargas.Contains(notaPedido.CargaPedido.Codigo) && codigosNotasFiscais.Contains(notaPedido.XMLNotaFiscal.Codigo));

            return consultaPedidoXMLNotaFiscal
                .Fetch(notaPedido => notaPedido.XMLNotaFiscal)
                .Fetch(notaPedido => notaPedido.CargaPedido)
                .ToList();
        }

        public bool VerificarSeExisteNota(int codigoCarga, ClassificacaoNFe classificacaoNFe)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.ClassificacaoNFe.HasValue && o.XMLNotaFiscal.ClassificacaoNFe == classificacaoNFe);

            return consultaPedidoXMLNotaFiscal.Select(o => o.Codigo).Any();
        }

        public bool VerificarSeExisteNotaPorPedido(int codigoPedido)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.CargaPedido.Pedido.Codigo == codigoPedido);

            return consultaPedidoXMLNotaFiscal.Count() > 0;
        }

        public bool VerificarSeExisteNotaSemQuantidadePallets(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.QuantidadePallets <= 0m);

            return query.Select(o => o.Codigo).Any();
        }

        public bool VerificarSeExistemNotaSemPeso(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && (!o.XMLNotaFiscal.TipoNotaFiscalIntegrada.HasValue || o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet) && o.XMLNotaFiscal.Peso <= 0m);

            return query.Select(o => o.Codigo).Any();
        }

        public bool VerificarSeExisteEmAlgumPedido(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && o.XMLNotaFiscal.nfAtiva && !(o.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || o.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada));

            return query.Any();
        }

        public int SetarOrdemColeta(int carga, int ordem, double cnpj)
        {
            string sqlComand = @"
                  UPDATE pedidoXmlNotaFiscal set pedidoXmlNotaFiscal.PNF_ORDEM_COLETA = :Ordem FROM T_PEDIDO_XML_NOTA_FISCAL pedidoXmlNotaFiscal
                  INNER JOIN T_XML_NOTA_FISCAL xmlNotaFiscal ON xmlNotaFiscal.NFX_CODIGO = pedidoXmlNotaFiscal.NFX_CODIGO
                  INNER JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedido.CPE_CODIGO = pedidoXmlNotaFiscal.CPE_CODIGO
                  WHERE cargaPedido.CAR_CODIGO = :Carga and(cargaPedido.CLI_CODIGO_EXPEDIDOR = :CNPJ or xmlNotaFiscal.CLI_CODIGO_REMETENTE = :CNPJ) ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlComand);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);

            return query.SetTimeout(120).ExecuteUpdate();
        }
        public async Task<int> SetarOrdemColetaAsync(int carga, int ordem, double cnpj)
        {
            string sqlComand = @"
                  UPDATE pedidoXmlNotaFiscal set pedidoXmlNotaFiscal.PNF_ORDEM_COLETA = :Ordem FROM T_PEDIDO_XML_NOTA_FISCAL pedidoXmlNotaFiscal
                  INNER JOIN T_XML_NOTA_FISCAL xmlNotaFiscal ON xmlNotaFiscal.NFX_CODIGO = pedidoXmlNotaFiscal.NFX_CODIGO
                  INNER JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedido.CPE_CODIGO = pedidoXmlNotaFiscal.CPE_CODIGO
                  WHERE cargaPedido.CAR_CODIGO = :Carga and(cargaPedido.CLI_CODIGO_EXPEDIDOR = :CNPJ or xmlNotaFiscal.CLI_CODIGO_REMETENTE = :CNPJ) ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlComand);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);

            return await query.SetTimeout(120).ExecuteUpdateAsync();
        }

        public int SetarOrdemColetaOutroendereco(int carga, int ordem, int outroEndereco)
        {
            string hql = "update pedidoXML set pedidoXML.PNF_ORDEM_COLETA = " + ordem + " from T_PEDIDO_XML_NOTA_FISCAL pedidoXML inner join t_carga_pedido cargaPedido on cargaPedido.CPE_CODIGO = pedidoXML.CPE_CODIGO inner join t_pedido pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO left join T_PEDIDO_ENDERECO pedidoEndereco on  pedidoEndereco.PEN_CODIGO = pedido.PEN_CODIGO_ORIGEM where cargaPedido.CAR_CODIGO = " + carga + " and pedidoEndereco.COE_CODIGO = " + outroEndereco; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            return query.ExecuteUpdate();
        }
        public async Task<int> SetarOrdemColetaOutroenderecoAsync(int carga, int ordem, int outroEndereco)
        {
            string hql = "update pedidoXML set pedidoXML.PNF_ORDEM_COLETA = " + ordem + " from T_PEDIDO_XML_NOTA_FISCAL pedidoXML inner join t_carga_pedido cargaPedido on cargaPedido.CPE_CODIGO = pedidoXML.CPE_CODIGO inner join t_pedido pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO left join T_PEDIDO_ENDERECO pedidoEndereco on  pedidoEndereco.PEN_CODIGO = pedido.PEN_CODIGO_ORIGEM where cargaPedido.CAR_CODIGO = " + carga + " and pedidoEndereco.COE_CODIGO = " + outroEndereco; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            return await query.ExecuteUpdateAsync();
        }

        public int SetarOrdemEntrega(int carga, int ordem, double cnpj)
        {
            string hql = "update PedidoXMLNotaFiscal set OrdemEntrega = :Ordem where Codigo in (select obj.Codigo from PedidoXMLNotaFiscal obj where obj.CargaPedido.Carga.Codigo = :Carga and obj.CargaPedido.Recebedor.CPF_CNPJ = :CNPJ) or Codigo in (select obj.Codigo from PedidoXMLNotaFiscal obj where obj.CargaPedido.Carga.Codigo = :Carga and obj.XMLNotaFiscal.Destinatario.CPF_CNPJ = :CNPJ) ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            return query.ExecuteUpdate();
        }
        public async Task<int> SetarOrdemEntregaAsync(int carga, int ordem, double cnpj)
        {
            string hql = "update PedidoXMLNotaFiscal set OrdemEntrega = :Ordem where Codigo in (select obj.Codigo from PedidoXMLNotaFiscal obj where obj.CargaPedido.Carga.Codigo = :Carga and obj.CargaPedido.Recebedor.CPF_CNPJ = :CNPJ) or Codigo in (select obj.Codigo from PedidoXMLNotaFiscal obj where obj.CargaPedido.Carga.Codigo = :Carga and obj.XMLNotaFiscal.Destinatario.CPF_CNPJ = :CNPJ) ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ordem", ordem);
            query.SetInt32("Carga", carga);
            query.SetDouble("CNPJ", cnpj);
            return await query.ExecuteUpdateAsync();
        }

        public int SetarOrdemEntregaOutroendereco(int carga, int ordem, int outroEndereco)
        {
            string hql = "update pedidoXML set pedidoXML.PNF_ORDEM_ENTREGA = " + ordem + " from T_PEDIDO_XML_NOTA_FISCAL pedidoXML inner join t_carga_pedido cargaPedido on cargaPedido.CPE_CODIGO = pedidoXML.CPE_CODIGO inner join t_pedido pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO left join T_PEDIDO_ENDERECO pedidoEndereco on  pedidoEndereco.PEN_CODIGO = pedido.PEN_CODIGO_DESTINO where cargaPedido.CAR_CODIGO = " + carga + " and pedidoEndereco.COE_CODIGO = " + outroEndereco; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            return query.ExecuteUpdate();
        }
        public async Task<int> SetarOrdemEntregaOutroenderecoAsync(int carga, int ordem, int outroEndereco)
        {
            string hql = "update pedidoXML set pedidoXML.PNF_ORDEM_ENTREGA = " + ordem + " from T_PEDIDO_XML_NOTA_FISCAL pedidoXML inner join t_carga_pedido cargaPedido on cargaPedido.CPE_CODIGO = pedidoXML.CPE_CODIGO inner join t_pedido pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO left join T_PEDIDO_ENDERECO pedidoEndereco on  pedidoEndereco.PEN_CODIGO = pedido.PEN_CODIGO_DESTINO where cargaPedido.CAR_CODIGO = " + carga + " and pedidoEndereco.COE_CODIGO = " + outroEndereco; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);

            return await query.ExecuteUpdateAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> ConsultarNotasVinculadas(int protocoloCarga, int protocoloPedido, string chaveCTe, int inicioRegistros, int maximoRegistros)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (protocoloCarga > 0)
                query = query.Where(obj => obj.CargaPedido.Carga.Protocolo == protocoloCarga);

            if (protocoloPedido > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.Protocolo == protocoloPedido);

            if (!string.IsNullOrWhiteSpace(chaveCTe))
            {
                queryCargaCTe = queryCargaCTe.Where(obj => obj.CargaCTe.CTe.Chave == chaveCTe);
                query = query.Where(obj => queryCargaCTe.Any(o => o.PedidoXMLNotaFiscal.Codigo == obj.Codigo));
            }

            query = query.Where(o => o.XMLNotaFiscal.nfAtiva);
            if (maximoRegistros > 0)
                return query.Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return query.Take(100).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> ConsultarNotasVinculadasPorListaCargas(List<int> codigosCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            query = query.Where(obj => codigosCargas.Contains(obj.CargaPedido.Carga.Codigo) && obj.XMLNotaFiscal.nfAtiva);

            return query.Fetch(n => n.XMLNotaFiscal)
                        .ThenFetch(c => c.Canhoto)
                        .Fetch(n => n.XMLNotaFiscal)
                        .ThenFetch(r => r.Emitente)
                        .Fetch(n => n.XMLNotaFiscal)
                        .ThenFetch(d => d.Destinatario)
                        .Fetch(n => n.CargaPedido)
                        .ToList();
        }

        public int ContarNotasVinculadas(int protocoloCarga, int protocoloPedido, string chaveCTe)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (protocoloCarga > 0)
                query = query.Where(obj => obj.CargaPedido.Carga.Protocolo == protocoloCarga);

            if (protocoloPedido > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.Protocolo == protocoloPedido);

            if (!string.IsNullOrWhiteSpace(chaveCTe))
            {
                queryCargaCTe = queryCargaCTe.Where(obj => obj.CargaCTe.CTe.Chave == chaveCTe);
                query = query.Where(obj => queryCargaCTe.Any(o => o.PedidoXMLNotaFiscal.Codigo == obj.Codigo));
            }

            query = query.Where(o => o.XMLNotaFiscal.nfAtiva);
            return query.Count();
        }

        public Task<int> ContarNotasVinculadasAsync(int protocoloCarga, int protocoloPedido, string chaveCTe)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (protocoloCarga > 0)
                query = query.Where(obj => obj.CargaPedido.Carga.Protocolo == protocoloCarga);

            if (protocoloPedido > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.Protocolo == protocoloPedido);

            if (!string.IsNullOrWhiteSpace(chaveCTe))
            {
                queryCargaCTe = queryCargaCTe.Where(obj => obj.CargaCTe.CTe.Chave == chaveCTe);
                query = query.Where(obj => queryCargaCTe.Any(o => o.PedidoXMLNotaFiscal.Codigo == obj.Codigo));
            }

            query = query.Where(o => o.XMLNotaFiscal.nfAtiva);
            return query.CountAsync();
        }

        public bool VerificarSeExisteEmOutroPedido(int codigoXMLNotaFiscal, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.XMLNotaFiscal.nfAtiva
            && o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada
            && o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada
            && o.CargaPedido.Codigo != codigoCargaPedido && o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal);

            return query.Any();
        }

        public bool VerificarSeExisteEmOutroPedidoAberto(int codigoXMLNotaFiscal, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo != codigoCargaPedido && o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> ObterSeExisteEmOutroPedido(int codigoXMLNotaFiscal, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.XMLNotaFiscal.nfAtiva && o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada
            && o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada
            && o.CargaPedido.Codigo != codigoCargaPedido && o.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal);

            return query.Select(obj => obj.CargaPedido).Fetch(obj => obj.Carga).ToList();
        }

        public bool VerificarSeCargaPossuiNotaCanceladaPeloEmitente(int carga)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            //var result = (from obj in query where obj.CargaPedido.Carga.Codigo == carga && obj.XMLNotaFiscal.nfAtiva == true && obj.XMLNotaFiscal.CanceladaPeloEmitente select obj);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = (from obj in query where obj.Carga.Codigo == carga && obj.NotasFiscais.Any(o => o.XMLNotaFiscal.nfAtiva == true && o.XMLNotaFiscal.CanceladaPeloEmitente == true) select obj);

            return result.Any();
        }

        public List<int> BuscarNumerosNotasCanceladasPeloEmitente(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Carga.Codigo == carga && obj.XMLNotaFiscal.CanceladaPeloEmitente);

            return query.Select(o => o.XMLNotaFiscal.Numero).ToList();
        }

        public bool VerificarSeAlgumaNaoPossuiPeso(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.Peso <= 0
                               && obj.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet
                         select obj;

            return result.Any();
        }
        public bool VerificarSeAlgumaNaoPossuiMetrosCubicos(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.MetrosCubicos <= 0
                               && obj.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet
                         select obj;

            return result.Any();
        }
        public decimal NotasPossuemValorFretePorXMLNotaFiscal(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = (from obj in query where obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Codigo == cargaPedido select obj);

            return result.Sum(o => (decimal?)o.XMLNotaFiscal.ValorFrete) ?? 0m;
        }

        public List<int> NotasPossuemValorFreteZeradoPorPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = (from obj in query where obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Codigo == cargaPedido && obj.ValorFrete <= 0m && obj.ValorTotalComponentes <= 0 select obj);

            return result.Select(o => o.XMLNotaFiscal.Numero).ToList();
        }

        public int ContarNumeroDeDiferentesDestinatarioSaida(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida select obj;
            result = result.Where(obj => cargaPedidos.Contains(obj.CargaPedido));
            return result.Select(obj => obj.XMLNotaFiscal.Destinatario).Distinct().Count();
        }

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_INTEGRACAO_AVIPED where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARGA_ENTREGA_NOTA_FISCAL where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoXMLNotaFiscal obj WHERE obj.CargaPedido.Codigo = :CodigoCargaPedido").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_INTEGRACAO_AVIPED where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CARGA_ENTREGA_NOTA_FISCAL where PNF_CODIGO in (select PNF_CODIGO from T_PEDIDO_XML_NOTA_FISCAL where CPE_CODIGO = :CodigoCargaPedido)").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE PedidoXMLNotaFiscal obj WHERE obj.CargaPedido.Codigo = :CodigoCargaPedido").SetInt32("CodigoCargaPedido", codigoCargaPedido).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        public int ContarNumeroDeDiferentesDestinatarioEntrada(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada select obj;
            result = result.Where(obj => cargaPedidos.Contains(obj.CargaPedido));
            return result.Select(obj => obj.XMLNotaFiscal.Emitente).Distinct().Count();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarNotaFiscalSemEmissaoPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = (from obj in query
                          where obj.XMLNotaFiscal.nfAtiva == true
        && (obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
        || obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
        && obj.XMLNotaFiscal.Chave == chave
                          select obj);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorChaveCargaAtiva(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query
                         where obj.XMLNotaFiscal.Chave == chave
                            && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe
                            && (obj.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada || obj.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                         select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosPorChaveNFe(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(pedXML => pedXML.XMLNotaFiscal.Chave == chave && pedXML.XMLNotaFiscal.TipoDocumento == TipoDocumento.NFe &&
                pedXML.XMLNotaFiscal.nfAtiva && pedXML.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada && pedXML.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return query.Select(pedXML => pedXML.CargaPedido.Pedido).Distinct().ToList();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarUltimaPorChaveNFeAsync(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(pedXML => pedXML.XMLNotaFiscal.Chave == chave &&
                       pedXML.XMLNotaFiscal.TipoDocumento == TipoDocumento.NFe &&
                       pedXML.XMLNotaFiscal.nfAtiva &&
                       pedXML.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada && pedXML.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return query
                .OrderByDescending(obj => obj.CargaPedido.Carga.Codigo)
                .ThenByDescending(obj => obj.Codigo)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarUltimaPorNumeroSerieNFeTransportadorAsync(int numeroNFe, string serieNFe, double cnpjEmitente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(pedXML => pedXML.XMLNotaFiscal.Numero == numeroNFe &&
                       pedXML.XMLNotaFiscal.Serie == serieNFe &&
                       pedXML.XMLNotaFiscal.nfAtiva &&
                       pedXML.XMLNotaFiscal.Emitente.CPF_CNPJ == cnpjEmitente &&
                       pedXML.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada && pedXML.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return query
                .OrderByDescending(obj => obj.CargaPedido.Carga.Codigo)
                .ThenByDescending(obj => obj.Codigo)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaENumeroNota(List<int> notas, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && notas.Contains(obj.XMLNotaFiscal.Numero) && obj.CargaPedido.Carga.Codigo == carga);

            return query.Select(obj => obj.CargaPedido).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaENumeroControle(int carga, string numeroControle, int codigoNotaCobertura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Pedido.NumeroControle == numeroControle && obj.XMLNotaFiscal.Codigo != codigoNotaCobertura
            && (obj.XMLNotaFiscal.NumeroControlePedido == numeroControle)
            && obj.CargaPedido.Carga.Codigo == carga);

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public bool PossuiNotaFilhoPorCargaENumeroControle(int carga, string numeroControle, int codigoNotaCobertura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Pedido.NumeroControle == numeroControle && obj.XMLNotaFiscal.Codigo != codigoNotaCobertura
            && (obj.XMLNotaFiscal.NumeroControlePedido == numeroControle)
            && obj.CargaPedido.Carga.Codigo == carga);

            return query.Any();
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoXmlNotaFiscal BuscarPorNumeroNotaEmitenteTransportador(int numeroNota, int empresa, double emitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o =>
                    o.XMLNotaFiscal.nfAtiva &&
                    o.XMLNotaFiscal.Numero == numeroNota &&
                    o.XMLNotaFiscal.Emitente.CPF_CNPJ == emitente &&
                    o.CargaPedido.Carga.Empresa.Codigo == empresa &&
                    o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaPedidoXMLNotaFiscal
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoXmlNotaFiscal()
                {
                    Codigo = o.Codigo,
                    CodigoCarga = o.CargaPedido.Carga.Codigo,
                    CodigoCargaPedido = o.CargaPedido.Codigo,
                    CodigoPedido = o.CargaPedido.Pedido.Codigo,
                    NumeroNotaFiscal = o.XMLNotaFiscal.Numero
                })
                .FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal> BuscarDadosPedidoXmlNotaFiscal(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.FiltroPesquisaPedidoXmlNotaFiscal filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.XMLNotaFiscal.nfAtiva);

            if ((filtrosPesquisa.CodigoCarga > 0) || (filtrosPesquisa.CodigoCargaPedido > 0))
            {
                if (filtrosPesquisa.CodigoCarga > 0)
                    consultaPedidoXMLNotaFiscal = consultaPedidoXMLNotaFiscal.Where(o => o.CargaPedido.Carga.Codigo == filtrosPesquisa.CodigoCarga);

                if (filtrosPesquisa.CodigoCargaPedido > 0)
                    consultaPedidoXMLNotaFiscal = consultaPedidoXMLNotaFiscal.Where(o => o.CargaPedido.Codigo == filtrosPesquisa.CodigoCargaPedido);
            }
            else
                consultaPedidoXMLNotaFiscal = consultaPedidoXMLNotaFiscal.Where(o => o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal> pedidosXmlNotaFiscal = consultaPedidoXMLNotaFiscal
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal()
                {
                    Codigo = o.Codigo,
                    Carga = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Carga()
                    {
                        Codigo = o.CargaPedido.Carga.Codigo,
                        Numero = o.CargaPedido.Carga.CodigoCargaEmbarcador
                    },
                    CargaPedido = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.CargaPedido()
                    {
                        Codigo = o.CargaPedido.Codigo,
                        TipoTomador = o.CargaPedido.TipoTomador,
                        Pedido = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Pedido()
                        {
                            Codigo = o.CargaPedido.Pedido.Codigo,
                            NumeroOrdem = o.CargaPedido.Pedido.NumeroOrdem,
                            NumeroPedidoEmbarcador = o.CargaPedido.Pedido.NumeroPedidoEmbarcador,
                            Remetente = o.CargaPedido.Pedido.Remetente == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                            {
                                CpfCnpj = o.CargaPedido.Pedido.Remetente.CPF_CNPJ,
                                Nome = o.CargaPedido.Pedido.Remetente.Nome,
                                Ativo = o.CargaPedido.Pedido.Remetente.Ativo,
                                Tipo = o.CargaPedido.Pedido.Remetente.Tipo
                            },
                            Destinatario = o.CargaPedido.Pedido.Destinatario == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                            {
                                CpfCnpj = o.CargaPedido.Pedido.Destinatario.CPF_CNPJ,
                                Nome = o.CargaPedido.Pedido.Destinatario.Nome,
                                Ativo = o.CargaPedido.Pedido.Destinatario.Ativo,
                                Tipo = o.CargaPedido.Pedido.Destinatario.Tipo
                            }
                        },
                        Expedidor = o.CargaPedido.Expedidor == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                        {
                            CpfCnpj = o.CargaPedido.Expedidor.CPF_CNPJ,
                            Nome = o.CargaPedido.Expedidor.Nome,
                            Ativo = o.CargaPedido.Expedidor.Ativo,
                            Tipo = o.CargaPedido.Expedidor.Tipo
                        },
                        Recebedor = o.CargaPedido.Recebedor == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                        {
                            CpfCnpj = o.CargaPedido.Recebedor.CPF_CNPJ,
                            Nome = o.CargaPedido.Recebedor.Nome,
                            Ativo = o.CargaPedido.Recebedor.Ativo,
                            Tipo = o.CargaPedido.Recebedor.Tipo
                        }
                    },
                    XmlNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscal()
                    {
                        Codigo = o.XMLNotaFiscal.Codigo,
                        Numero = o.XMLNotaFiscal.Numero,
                        CFOP = o.XMLNotaFiscal.CFOP,
                        Chave = o.XMLNotaFiscal.Chave,
                        NumeroOrdemPedido = o.XMLNotaFiscal.NumeroOrdemPedidoIntegracaoUnilever,
                        Peso = o.XMLNotaFiscal.Peso,
                        PesoLiquido = o.XMLNotaFiscal.PesoLiquido,
                        ModalidadeFrete = o.XMLNotaFiscal.ModalidadeFrete,
                        TipoNotaFiscalIntegrada = o.XMLNotaFiscal.TipoNotaFiscalIntegrada,
                        TipoOperacaoNFe = o.XMLNotaFiscal.TipoOperacaoNotaFiscal,
                        Emitente = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                        {
                            CpfCnpj = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                            Nome = o.XMLNotaFiscal.Emitente.Nome,
                            Ativo = o.XMLNotaFiscal.Emitente.Ativo,
                            Tipo = o.XMLNotaFiscal.Emitente.Tipo
                        },
                        Destinatario = o.XMLNotaFiscal.Destinatario == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                        {
                            CpfCnpj = o.XMLNotaFiscal.Destinatario.CPF_CNPJ,
                            Nome = o.XMLNotaFiscal.Destinatario.Nome,
                            Ativo = o.XMLNotaFiscal.Destinatario.Ativo,
                            Tipo = o.XMLNotaFiscal.Destinatario.Tipo
                        },
                        Expedidor = o.XMLNotaFiscal.Expedidor == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                        {
                            CpfCnpj = o.XMLNotaFiscal.Expedidor.CPF_CNPJ,
                            Nome = o.XMLNotaFiscal.Expedidor.Nome,
                            Ativo = o.XMLNotaFiscal.Expedidor.Ativo,
                            Tipo = o.XMLNotaFiscal.Expedidor.Tipo
                        },
                        Recebedor = o.XMLNotaFiscal.Recebedor == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                        {
                            CpfCnpj = o.XMLNotaFiscal.Recebedor.CPF_CNPJ,
                            Nome = o.XMLNotaFiscal.Recebedor.Nome,
                            Ativo = o.XMLNotaFiscal.Recebedor.Ativo,
                            Tipo = o.XMLNotaFiscal.Recebedor.Tipo
                        },
                        Empresa = o.XMLNotaFiscal.Empresa == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Empresa()
                        {
                            Codigo = o.XMLNotaFiscal.Empresa.Codigo,
                            CNPJ = o.XMLNotaFiscal.Empresa.CNPJ,
                            CodigoIBGE = o.XMLNotaFiscal.Empresa.Localidade != null && o.XMLNotaFiscal.Empresa.Localidade.Estado != null ? o.XMLNotaFiscal.Empresa.Localidade.Estado.CodigoIBGE : 0,
                            NomeCertificado = o.XMLNotaFiscal.Empresa.NomeCertificado,
                            SenhaCertificado = o.XMLNotaFiscal.Empresa.SenhaCertificado,
                            NomeCertificadoKeyVault = o.XMLNotaFiscal.Empresa.NomeCertificadoKeyVault,
                            TipoAmbiente = o.XMLNotaFiscal.Empresa.TipoAmbiente
                        }
                    }
                })
                .ToList();

            if (pedidosXmlNotaFiscal.Count > 0)
            {
                List<int> codigosCargaPedidos = pedidosXmlNotaFiscal.Select(o => o.CargaPedido.Codigo).Distinct().ToList();

                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> consultaCargaPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>()
                    .Where(o => codigosCargaPedidos.Contains(o.CargaPedido.Codigo));

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.CargaPedidoProduto> cargaPedidosProdutos = consultaCargaPedidoProduto.Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.CargaPedidoProduto()
                {
                    Codigo = o.Codigo,
                    CodigoCargaPedido = o.CargaPedido.Codigo,
                    ProdutoEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcador()
                    {
                        Codigo = o.Produto.Codigo
                    }
                }).ToList();

                List<int> codigosXmlNotasFiscais = pedidosXmlNotaFiscal.Select(o => o.XmlNotaFiscal.Codigo).Distinct().ToList();

                IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> consultaXMLNotaFiscalProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>()
                    .Where(o => codigosXmlNotasFiscais.Contains(o.XMLNotaFiscal.Codigo));

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscalProduto> xmlNotasFiscaisProdutos = consultaXMLNotaFiscalProduto.Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.XmlNotaFiscalProduto()
                {
                    Codigo = o.Codigo,
                    CodigoProduto = o.cProd,
                    CodigoXmlNotaFiscal = o.XMLNotaFiscal.Codigo,
                    NumeroPedidoCompra = o.NumeroPedidoCompra,
                    ProdutoEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcador()
                    {
                        Codigo = o.Produto.Codigo
                    }
                }).ToList();

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcador> produtos = cargaPedidosProdutos.Select(o => o.ProdutoEmbarcador).Concat(xmlNotasFiscaisProdutos.Select(o => o.ProdutoEmbarcador)).ToList();

                if (produtos.Count > 0)
                {
                    List<int> codigosProdutosEmbarcador = produtos.Select(o => o.Codigo).Distinct().ToList();

                    IQueryable<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial> consultaProdutoEmbarcadorFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial>()
                        .Where(o => codigosProdutosEmbarcador.Contains(o.ProdutoEmbarcador.Codigo));

                    List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcadorFilial> produtosEmbarcadorFilial = consultaProdutoEmbarcadorFilial.Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcadorFilial()
                    {
                        Codigo = o.Codigo,
                        CodigoProdutoEmbarcador = o.ProdutoEmbarcador.Codigo,
                        UsoMaterial = o.UsoMaterial,
                        Filial = o.Filial == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Filial()
                        {
                            Codigo = o.Filial.Codigo
                        },
                        FilialSituacao = o.FilialSituacao == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcadorFilialSituacoes()
                        {
                            Situacao = o.FilialSituacao.SituacaoFilial
                        }
                    }).ToList();

                    IQueryable<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor> consultaProdutoEmbarcadorFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor>()
                        .Where(o => codigosProdutosEmbarcador.Contains(o.ProdutoEmbarcador.Codigo));

                    List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcadorFornecedor> produtosEmbarcadorFornecedor = consultaProdutoEmbarcadorFornecedor.Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcadorFornecedor()
                    {
                        Codigo = o.Codigo,
                        CodigoProdutoEmbarcador = o.ProdutoEmbarcador.Codigo,
                        CodigoInterno = o.CodigoInterno,
                        Filial = o.Filial == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Filial()
                        {
                            Codigo = o.Filial.Codigo
                        },
                        Fornecedor = o.Fornecedor == null ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.Cliente()
                        {
                            CpfCnpj = o.Fornecedor.CPF_CNPJ,
                            Nome = o.Fornecedor.Nome,
                            Ativo = o.Fornecedor.Ativo,
                            Tipo = o.Fornecedor.Tipo
                        }
                    }).ToList();

                    for (int i = 0; i < produtos.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.ProdutoEmbarcador produto = produtos[i];

                        produto.Filiais = produtosEmbarcadorFilial.Where(o => o.CodigoProdutoEmbarcador == produto.Codigo).ToList();
                        produto.Fornecedores = produtosEmbarcadorFornecedor.Where(o => o.CodigoProdutoEmbarcador == produto.Codigo).ToList();
                    }
                }

                for (int i = 0; i < pedidosXmlNotaFiscal.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal.PedidoXmlNotaFiscal pedidoXmlNotaFiscal = pedidosXmlNotaFiscal[i];

                    pedidoXmlNotaFiscal.CargaPedido.Produtos = cargaPedidosProdutos.Where(o => o.CodigoCargaPedido == pedidoXmlNotaFiscal.CargaPedido.Codigo).ToList();
                    pedidoXmlNotaFiscal.XmlNotaFiscal.Produtos = xmlNotasFiscaisProdutos.Where(o => o.CodigoXmlNotaFiscal == pedidoXmlNotaFiscal.XmlNotaFiscal.Codigo).ToList();
                }
            }

            return pedidosXmlNotaFiscal;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorChaveTransportador(string chave, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Chave == chave
                            && obj.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.CargaPedido.Carga.Empresa.Codigo == codigoEmpresa);

            return query
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorNumeroNotaEmitenteTransportador(int numeroNota, int empresa, double emitente, double destinatario, bool buscarTodasSituacoesPedido = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Numero == numeroNota
            && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == emitente
            && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario
            && (obj.CargaPedido.Carga.Empresa.Codigo == empresa || obj.CargaPedido.Carga.Empresa.Matriz.Any(emp => emp.Codigo == empresa))
            && obj.CargaPedido.Carga.CargaFechada
            && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
            && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (!buscarTodasSituacoesPedido)
                query = query.Where(obj => obj.CargaPedido.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorNumeroNotaEmitenteTransportadorTodasSituacoes(List<int> numeroNota, int empresa, double emitente, double destinatario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && numeroNota.Contains(obj.XMLNotaFiscal.Numero)
            && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == emitente
            && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario
            && (obj.CargaPedido.Carga.Empresa.Codigo == empresa || obj.CargaPedido.Carga.Empresa.Matriz.Any(emp => emp.Codigo == empresa))
            && obj.CargaPedido.Carga.CargaFechada);

            query = query.OrderByDescending(obj => obj.CargaPedido.Carga.Codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorChaveAtiva(string chave, int codigoCargaPedidoDiferente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo != codigoCargaPedidoDiferente && obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorChaveAtiva(string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorChaveAtivaCarga(string chave, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && obj.CargaPedido.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorChaveAtivaNoCTeSemMesmoPorto(string chave, int portoOrigem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && obj.CargaPedido.Pedido.Porto != null && obj.CargaPedido.Pedido.Porto.Codigo != portoOrigem);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorChaveAtivaNoCTe(string chave, int portoOrigem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && obj.CargaPedido.Pedido.Porto != null && obj.CargaPedido.Pedido.Porto.Codigo == portoOrigem);

            return query.FirstOrDefault();
        }

        public bool BuscarPorChaveAtivaNaCarga(string chave, int portoOrigem, int viagem, int container)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (portoOrigem > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.Porto != null && obj.CargaPedido.Pedido.Porto.Codigo == portoOrigem);

            if (viagem > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.PedidoViagemNavio != null && obj.CargaPedido.Pedido.PedidoViagemNavio.Codigo == viagem);

            if (container > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.Container != null && obj.CargaPedido.Pedido.Container.Codigo == container);

            return query.Any();
        }

        public bool BuscarPorChaveAtivaNaCargaSemMesmoPorto(string chave, int portoOrigem, int viagem, int container)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (portoOrigem > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.Porto != null && obj.CargaPedido.Pedido.Porto.Codigo != portoOrigem);

            if (viagem > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.PedidoViagemNavio != null && obj.CargaPedido.Pedido.PedidoViagemNavio.Codigo != viagem);

            if (container > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.Container != null && obj.CargaPedido.Pedido.Container.Codigo != container);

            return query.Any();
        }

        public bool BuscarPorChaveAtivaNoCTe(string chave, int portoOrigem, int viagem, int container)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(obj => obj.XMLNotaFiscais.Any(x => x.nfAtiva && x.Chave == chave && x.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe) && obj.Status == "A");

            if (portoOrigem > 0 && viagem > 0)
                query = query.Where(obj => (obj.PortoOrigem != null && obj.PortoOrigem.Codigo == portoOrigem) && (obj.Viagem != null && obj.Viagem.Codigo == viagem));

            if (container > 0)
                query = query.Where(obj => (obj.Containers.Any(c => c.Container.Codigo == container)));

            return query.Any();
        }

        public bool BuscarPorChaveAtivaNoCTeSemMesmoPortoViagem(string chave, int portoOrigem, int viagem, int container)
        {
            IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(obj => obj.XMLNotaFiscais.Any(x => x.nfAtiva && x.Chave == chave && x.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe) && obj.Status == "A");

            if (portoOrigem > 0 && viagem > 0)
                query = query.Where(obj => (obj.PortoOrigem != null && obj.PortoOrigem.Codigo != portoOrigem) && (obj.Viagem != null && obj.Viagem.Codigo != viagem));

            if (container > 0)
                query = query.Where(obj => (obj.Containers.Any(c => c.Container.Codigo != container)));

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorChaveExpedidor(string chave, int tipoOperacao, double expedidor, double recebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);

            if (tipoOperacao > 0)
                result = result.Where(obj => obj.CargaPedido.Carga.TipoOperacao.Codigo == tipoOperacao || obj.CargaPedido.Carga.TipoOperacao == null);

            if (expedidor > 0)
                result = result.Where(obj => obj.CargaPedido.Expedidor.CPF_CNPJ == expedidor);

            if (recebedor > 0)
                result = result.Where(obj => obj.CargaPedido.Recebedor.CPF_CNPJ == recebedor);

            return result.Select(obj => obj.CargaPedido.Carga).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorChave(string chave, int tipoOperacao, double expedidor, double recebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Chave == chave && obj.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);

            if (tipoOperacao > 0)
                result = result.Where(obj => obj.CargaPedido.Carga.TipoOperacao.Codigo == tipoOperacao || obj.CargaPedido.Carga.TipoOperacao == null);

            if (expedidor > 0)
                result = result.Where(obj => obj.CargaPedido.Expedidor.CPF_CNPJ == expedidor);
            else
                result = result.Where(obj => obj.CargaPedido.Expedidor == null);

            if (recebedor > 0)
                result = result.Where(obj => obj.CargaPedido.Recebedor.CPF_CNPJ == recebedor);
            else
                result = result.Where(obj => obj.CargaPedido.Recebedor == null);

            return result.Select(obj => obj.CargaPedido.Carga).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorCargaPedidoEDocumentoCTe(int cargaPedido, Dominio.Entidades.DocumentosCTE documento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = (from obj in query where obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Codigo == cargaPedido select obj);

            if (documento.ModeloDocumentoFiscal == null)
                result = result.Where(obj => obj.XMLNotaFiscal.Numero == int.Parse(documento.Numero));
            else if (documento.ModeloDocumentoFiscal.Numero == "55")
                result = result.Where(obj => obj.XMLNotaFiscal.Chave == documento.ChaveNFE);
            else if (documento.ModeloDocumentoFiscal.Numero == "01")
                result = result.Where(obj => obj.XMLNotaFiscal.Numero == int.Parse(documento.Numero) && obj.XMLNotaFiscal.Serie == documento.Serie);
            else
                result = result.Where(obj => obj.XMLNotaFiscal.Numero == int.Parse(documento.Numero));

            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> BuscarModalidadesDeFretePadraoCargaPedidoPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido
            {
                Codigo = obj.CargaPedido.Codigo,
                modalidadePagamentoFrete = obj.XMLNotaFiscal.ModalidadeFrete
            }).ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? BuscarModalidadeDeFretePadraoPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Select(obj => (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete?)obj.XMLNotaFiscal.ModalidadeFrete).FirstOrDefault();
        }

        public int ContarModalidadesDePagamentoPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Select(obj => obj.XMLNotaFiscal.ModalidadeFrete).Distinct().Count();
        }

        public int QuantidadeDestinatariosNaNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Recebedor == null);

            return query.Select(obj => obj.XMLNotaFiscal.Destinatario.CPF_CNPJ).Distinct().Count();
        }

        public int QuantidadePalletsControlePorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(o => (int?)o.XMLNotaFiscal.PalletsControle) ?? 0;
        }

        public bool ContemDocumentoSemNumeroReferenciaEDI(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Any(o => o.XMLNotaFiscal.NumeroReferenciaEDI == "" || o.XMLNotaFiscal.NumeroReferenciaEDI == null);
        }

        public bool ContemDocumentoSemNumeroControleCliente(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Any(o => o.XMLNotaFiscal.NumeroControleCliente == "" || o.XMLNotaFiscal.NumeroControleCliente == null);
        }

        public bool ContemDocumentoSemNCM(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Any(o => o.XMLNotaFiscal.NCM == "" || o.XMLNotaFiscal.NCM == null);
        }

        public bool ContemDocumentoSemPeso(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Any(o => o.XMLNotaFiscal.Peso == 0);
        }

        public bool ContemNotaFiscalEmOutraCarga(int codigoCarga, int codigoNota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo != codigoCarga && o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Codigo == codigoNota && o.CargaPedido.Carga.TipoOperacao != null && o.CargaPedido.Carga.TipoOperacao.TipoIntegracao != null && o.CargaPedido.Carga.TipoOperacao.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Any();
        }

        public bool ContemNotaEletronicaNaCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Any(o => o.XMLNotaFiscal.Modelo == "55");
        }

        public bool ContemNotaNaCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Any();
        }

        public bool ContemDocumentoSemCFOP(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Any(o => o.XMLNotaFiscal.CFOP == "" || o.XMLNotaFiscal.CFOP == null);
        }

        public bool ContemDocumentoLancadoComOutroTipo(int codigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.TipoDocumento != tipoDocumento);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ContemDocumentoLancadoTipo(int codigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.TipoDocumento == tipoDocumento);

            return query.Select(o => o.Codigo).Any();
        }

        public int ObterQuantidadeMercadoriaPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(o => (int?)o.XMLNotaFiscal.Volumes) ?? 0;
        }

        public decimal ObterValorNotaPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal ObterValorMercadoriaPorCarga(int codigo)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return consultaPedidoXMLNotaFiscal.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal ObterValorMercadoriaPorCargaEDestinatario(int codigo, double cpfCnpjDestinatario)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o =>
                    o.CargaPedido.Carga.Codigo == codigo &&
                    o.XMLNotaFiscal.nfAtiva &&
                    (
                        (o.CargaPedido.Recebedor != null && o.CargaPedido.Recebedor.CPF_CNPJ == cpfCnpjDestinatario) ||
                        (o.CargaPedido.Recebedor == null && o.CargaPedido.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario)
                    )
                );

            return consultaPedidoXMLNotaFiscal.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal ObterMetroCubicoPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.MetrosCubicos) ?? 0m;
        }

        public decimal ObterPesoTotalPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.Peso) ?? 0m;
        }

        public bool ContemXMLSemChaveVenda(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva && (o.XMLNotaFiscal.ChaveVenda == null || o.XMLNotaFiscal.ChaveVenda.Length == 0));

            return query.Any();
        }

        public bool ContemmClientesSemCadastro(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Where(o => o.XMLNotaFiscal.Emitente.CodigoIntegracao == null || o.XMLNotaFiscal.Emitente.CodigoIntegracao == "" || o.XMLNotaFiscal.Destinatario.CodigoIntegracao == "" || o.XMLNotaFiscal.Destinatario.CodigoIntegracao == null).Count() > 0;

        }

        public List<Dominio.Entidades.Cliente> RetornarRemetentes(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva);

            return query.Select(o => o.XMLNotaFiscal.Emitente).Distinct().ToList();
        }

        public List<double> ObterCPFCNPJTomadores(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva);

            return query.Select(o => o.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada ? o.XMLNotaFiscal.Destinatario.CPF_CNPJ : o.XMLNotaFiscal.Emitente.CPF_CNPJ).Distinct().ToList();
        }

        public bool ContemDestinatariosParaBloquear(int carga, List<string> cnpjDestinatario)
        {
            List<double> cnpjs = new List<double>();
            foreach (var cnpj in cnpjDestinatario)
                cnpjs.Add(Convert.ToDouble(Utilidades.String.OnlyNumbers(cnpj)));

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga && o.XMLNotaFiscal.nfAtiva);

            return query.Where(o => cnpjs.Contains(o.XMLNotaFiscal.Destinatario.CPF_CNPJ)).Any();
        }

        public IList<string> BuscarListaChaveNFeDuplicadaPorCarga(int codigoCarga)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select Nfe.NF_CHAVE ");
            sql.Append("  from T_PEDIDO_XML_NOTA_FISCAL PedidoNfe ");
            sql.Append(" inner join T_CARGA_PEDIDO CargaPedido on PedidoNfe.CPE_CODIGO=CargaPedido.CPE_CODIGO ");
            sql.Append(" inner join T_CARGA Carga on CargaPedido.CAR_CODIGO=Carga.CAR_CODIGO ");
            sql.Append(" inner join T_XML_NOTA_FISCAL Nfe on PedidoNfe.NFX_CODIGO=Nfe.NFX_CODIGO ");
            sql.Append($"where Carga.CAR_CODIGO = {codigoCarga} ");
            sql.Append($"  and Nfe.NF_TIPO_DOCUMENTO = {(int)TipoDocumento.NFe} ");
            sql.Append("   and Nfe.NF_ATIVA = 1 ");
            sql.Append(" group by Nfe.NF_CHAVE ");
            sql.Append("having cast(count(*) as INT) > 1");

            var consultaChaveNFeDuplicada = this.SessionNHiBernate
                .CreateSQLQuery(sql.ToString());

            return consultaChaveNFeDuplicada.SetTimeout(600).List<string>();
        }

        public IList<int> BuscarListaCodigosNFeDuplicadaPorCarga(int codigoCarga)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select Nfe.NFX_CODIGO ");
            sql.Append(" from T_PEDIDO_XML_NOTA_FISCAL PedidoNfe ");
            sql.Append(" inner join T_CARGA_PEDIDO CargaPedido on PedidoNfe.CPE_CODIGO=CargaPedido.CPE_CODIGO ");
            sql.Append(" inner join T_XML_NOTA_FISCAL Nfe on PedidoNfe.NFX_CODIGO=Nfe.NFX_CODIGO ");
            sql.Append(" inner join T_CARGA carga on carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
            sql.Append($" where carga.CAR_CODIGO = {codigoCarga} ");
            sql.Append($" and Nfe.NF_TIPO_DOCUMENTO = {(int)TipoDocumento.NFe} ");
            sql.Append(" and Nfe.NF_ATIVA = 1 ");
            sql.Append(" group by Nfe.NFX_CODIGO ");
            sql.Append("having cast(count(*) as INT) > 1");

            var consultaChaveNFeDuplicada = this.SessionNHiBernate
                .CreateSQLQuery(sql.ToString());

            return consultaChaveNFeDuplicada.SetTimeout(600).List<int>();
        }

        public decimal ObterValorTotalPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal ObterValorTotalPorCarga(int codigo, bool semNotasDePallet)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            if (semNotasDePallet)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }
        public async Task<decimal> ObterValorTotalPorCargaAsync(int codigo, bool semNotasDePallet)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            if (semNotasDePallet)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            return await query.SumAsync(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal ObterValorTotalPorCargaPedido(int codigo, bool semNotasDePallet)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            if (semNotasDePallet)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal ObterValorTotalProdutosPorCarga(int codigo, bool semNotasDePallet, ClassificacaoNFe? classificacaoNFeDesconsiderar = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            if (semNotasDePallet)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            if (classificacaoNFeDesconsiderar.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNFeDesconsiderar.Value);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.ValorTotalProdutos) ?? 0m;
        }

        public Task<decimal?> ObterValorTotalProdutosPorCargaAsync(int codigo, bool semNotasDePallet, ClassificacaoNFe? classificacaoNFeDesconsiderar = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            if (semNotasDePallet)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            if (classificacaoNFeDesconsiderar.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNFeDesconsiderar.Value);

            return query.SumAsync(o => (decimal?)o.XMLNotaFiscal.ValorTotalProdutos);
        }

        public List<string> ObterNumerosNotasPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Select(o => o.XMLNotaFiscal.Numero.ToString()).ToList();
        }

        public decimal ObterValorFretePorCodigo(IEnumerable<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.Sum(o => (decimal?)o.ValorFrete) ?? 0m;
        }

        public bool VerificarSeExisteNotaPorDestinatario(int carga, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query
                         where (obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida)
       || (obj.XMLNotaFiscal.Emitente.CPF_CNPJ == destinatario && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)
                         select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Carga.Codigo == carga);
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarNotaPorLocalidadeEDestintario(int cargaPedido, int localidade, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query
                         where (obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario && obj.XMLNotaFiscal.Destinatario.Localidade.Codigo == localidade && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida)
       || (obj.XMLNotaFiscal.Emitente.CPF_CNPJ == destinatario && obj.XMLNotaFiscal.Emitente.Localidade.Codigo == localidade && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)
                         select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Codigo == cargaPedido);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarNotaPorDestinatario(int cargaPedido, double destinatario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => ((o.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario && o.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida) ||
                                      (o.XMLNotaFiscal.Emitente.CPF_CNPJ == destinatario && o.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)) &&
                                     o.CargaPedido.Codigo == cargaPedido && o.XMLNotaFiscal.nfAtiva);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarNotaPorLocalidadeDestino(int cargaPedido, int localidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query
                         where (obj.XMLNotaFiscal.Destinatario.Localidade.Codigo == localidade && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida)
       || (obj.XMLNotaFiscal.Emitente.Localidade.Codigo == localidade && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)
                         select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Codigo == cargaPedido);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorCargaOrigemEXMLNotaFiscal(int codigoCarga, int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.CargaOrigem.Codigo == codigoCarga && obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Fetch(obj => obj.CargaPedido).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorCargaPedidoEXMLNotaFiscal(int codigoCargapedido, int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargapedido && obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorXMLNotaFiscal(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal select obj;
            return result
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Carga)
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> BuscarProdutosPorXMLNotaFiscal(int codigoXMLNotasFiscal, int codigoCarga)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                         .Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotasFiscal && obj.CargaPedido.Carga.Codigo == codigoCarga)
                         .SelectMany(obj => obj.CargaPedido.Pedido.Produtos)
                         .ToList();

            return result;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorXMLNotaFiscalECargaOrigem(int codigoXMLNotaFiscal, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.CargaOrigem.Codigo == carga select obj;
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .Fetch(obj => obj.CargaPedido).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorXMLNotaFiscalECarga(int codigoXMLNotaFiscal, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Carga.Codigo == carga select obj;
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .Fetch(obj => obj.CargaPedido).FirstOrDefault();
        }

        public List<int> BuscarNotasFiscaisPorNotasECargaPedido(IList<int> codigosXMLNotaFiscal, int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = (from obj in query where codigosXMLNotaFiscal.Contains(obj.XMLNotaFiscal.Codigo) && obj.CargaPedido.Codigo == cargaPedido select obj.XMLNotaFiscal.Codigo);
            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarProximaCargaPedidoPorXMLNotaFiscal(int codigoXMLNotaFiscal, double recebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query
                         where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Expedidor.CPF_CNPJ == recebedor && obj.XMLNotaFiscal.nfAtiva &&
       (obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                         select obj.CargaPedido;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoAnteriorPorXMLNotaFiscal(int codigoXMLNotaFiscal, double expedidor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query
                         where obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Recebedor.CPF_CNPJ == expedidor && obj.XMLNotaFiscal.nfAtiva
       && (obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                         select obj.CargaPedido;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorCodigo(int codigo)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.Codigo == codigo && o.XMLNotaFiscal.nfAtiva == true);

            return consultaPedidoXMLNotaFiscal.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCodigoAsync(int codigo, CancellationToken cancellationToken)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.Codigo == codigo && o.XMLNotaFiscal.nfAtiva == true);

            return await consultaPedidoXMLNotaFiscal.FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCodigos(List<int> codigos)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => codigos.Contains(o.Codigo) && o.XMLNotaFiscal.nfAtiva == true);

            return consultaPedidoXMLNotaFiscal.ToList();
        }

        public bool PossuiValorFreteEmbarcadorZerado(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.ValorFrete <= 0m && o.CargaPedido.Codigo == codigoCargaPedido);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorCodigoComFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.Codigo == codigo && o.XMLNotaFiscal.nfAtiva);

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.CargaPedido).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorNotaFiscal(int notafiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == notafiscal select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.ToList();
        }

        public Dominio.Entidades.Veiculo BuscarPrimeiroVeiculoPorNotaECargas(int codigoNota, List<int> codigosCarga = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Codigo == codigoNota && obj.CargaPedido.Carga.Veiculo.TipoVeiculo == "0");

            if (!codigosCarga.IsNullOrEmpty())
                query = query.Where(obj => codigosCarga.Contains(obj.CargaPedido.Carga.Codigo));

            return query
                .Select(obj => obj.CargaPedido.Carga.Veiculo)
                .Fetch(obj => obj.Motoristas)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorNotaFiscalCargaPedido(int notafiscal, int cargapedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == notafiscal && obj.CargaPedido.Codigo == cargapedido select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorXMLNotaFiscalECargaPedido(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais, int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = xmlNotasFiscais.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> registrosRetornar = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                registrosRetornar.AddRange(query.WithOptions(o => o.SetTimeout(600))
                                                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva && xmlNotasFiscais.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.XMLNotaFiscal))
                                                .ToList());

            return registrosRetornar;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorNotaFiscalCargaPedidos(int notafiscal, List<int> cargapedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == notafiscal && cargapedidos.Contains(obj.CargaPedido.Codigo) select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> BuscarTotalSumarizadoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);

            return result.GroupBy(o => new { o.CargaPedido.Codigo, o.TipoNotaFiscal }).Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido()
            {
                Codigo = obj.Key.Codigo,
                TipoNotaFiscal = obj.Key.TipoNotaFiscal,
                ValorTotalNotaFiscal = obj.Sum(c => c.XMLNotaFiscal.Valor),
                QuantidadePallets = obj.Sum(c => c.XMLNotaFiscal.QuantidadePallets),
                Cubagem = obj.Sum(c => c.XMLNotaFiscal.PesoCubado),
                MetrosCubicos = obj.Sum(c => c.XMLNotaFiscal.MetrosCubicos),
                TotalNotasFiscais = obj.Count()
            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoNotaFiscal> BuscarPedidoXMLNotaFiscal(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);

            return result.Select(o => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoNotaFiscal()
            {
                CargaPedido = o.CargaPedido.Codigo,
                PedidoXmlNotaFiscal = o.Codigo
            }).ToList();
        }

        public decimal BuscarTotalPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Sum(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarTotalPorCargaPedido(int codigoCargaPedido, bool semNotasDePallet)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva select obj;

            if (semNotasDePallet)
                result = result.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            return result.Sum(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }
        public async Task<decimal> BuscarTotalPorCargaPedidoAsync(int codigoCargaPedido, bool semNotasDePallet)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva select obj;

            if (semNotasDePallet)
                result = result.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            return await result.SumAsync(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarTotalPorCargaPedidos(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCargaPedido.Contains(obj.CargaPedido.Codigo) select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Sum(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPedidoXMLNotaFiscalCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva);

            return query.ToList();
        }

        public decimal BuscarTotalPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarTotalFacturaPorCarga(int codigoCarga, MoedaCotacaoBancoCentral moedaCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga &&
                                        obj.XMLNotaFiscal.nfAtiva &&
                                        obj.XMLNotaFiscal.TipoFatura &&
                                        obj.XMLNotaFiscal.Moeda == moedaCarga
            );

            return query.Sum(obj => obj.XMLNotaFiscal.ValorTotalMoeda ?? (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public async Task<decimal> BuscarTotalPorCargaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva);

            return await query.SumAsync(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarTotalPorCarregamento(int codigoCarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Carregamento.Codigo == codigoCarregamento && obj.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarMetrosCubicosPorCarregamento(int codigoCarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Carregamento.Codigo == codigoCarregamento && obj.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.MetrosCubicos) ?? 0m;
        }

        public decimal BuscarTotalPorCargaPedidosNormais(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva && (obj.CargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || obj.CargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada));

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarMetrosCubicosPorCargaPedidosNormais(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva && (obj.CargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || obj.CargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada));

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.MetrosCubicos) ?? 0m;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPrimeroXMLPorCargaPedidoEModalidade(int codigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidadePagamentoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.ModalidadeFrete == modalidadePagamentoFrete select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPrimeroXMLPorCargaPedidoEModalidadeDiferenciandoTipoNota(int codigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidadePagamentoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal tipoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.ModalidadeFrete == modalidadePagamentoFrete && obj.TipoNotaFiscal != tipoNotaFiscal select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva);

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                        .Fetch(o => o.CargaPedido)
                        .WithOptions(o => o.SetTimeout(60))
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarNotasPorCargaETipoFatura(int codigoCarga, bool tipoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj =>
                obj.CargaPedido.Carga.Codigo == codigoCarga &&
                obj.XMLNotaFiscal.nfAtiva &&
                obj.XMLNotaFiscal.TipoFatura == tipoFatura);

            return query.Fetch(obj => obj.XMLNotaFiscal).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva);

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                         .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                         .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                         .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                         .Fetch(o => o.CargaPedido)
                         .WithOptions(o => o.SetTimeout(60))
                         .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargas(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where cargas.Contains(obj.CargaPedido.Carga.Codigo) select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                        .Fetch(o => o.CargaPedido)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaSemFetch(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarCodigoPedidoQuePossuNotafiscalPorCarga(int cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido && o.XMLNotaFiscal.nfAtiva);
            return query.Select(x => new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal { Codigo = x.Codigo, CargaPedido = x.CargaPedido }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaFetchPedido(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva);
            return result.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaCancelada(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == false);
            return result.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                        .Fetch(o => o.CargaPedido)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaAgrupada(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Carga.CargaAgrupamento.Codigo == carga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                        .Fetch(o => o.CargaPedido)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarComXMLPorCarga(int codigoCarga)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o =>
                    o.CargaPedido.Carga.Codigo == codigoCarga &&
                    o.XMLNotaFiscal.nfAtiva == true &&
                    (o.XMLNotaFiscal.XML != null && o.XMLNotaFiscal.XML != "")
                );

            return consultaPedidoXMLNotaFiscal
                .Fetch(o => o.XMLNotaFiscal)
                .Fetch(o => o.CargaPedido).ThenFetch(o => o.Carga)
                .ToList();
        }

        public int ContarComXMLPorCarga(int codigoCarga)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o =>
                    o.CargaPedido.Carga.Codigo == codigoCarga &&
                    o.XMLNotaFiscal.nfAtiva == true &&
                    (o.XMLNotaFiscal.XML != null && o.XMLNotaFiscal.XML != "")
                );

            return consultaPedidoXMLNotaFiscal.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaOrigem(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.CargaOrigem.Codigo == carga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                        .Fetch(o => o.CargaPedido)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPorCargaENumerosNota(int codigoCarga, List<int> numerosNota)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.XMLNotaFiscal.nfAtiva && o.CargaPedido.CargaOrigem.Codigo == codigoCarga && numerosNota.Contains(o.XMLNotaFiscal.Numero));

            return consultaPedidoXMLNotaFiscal
                .Select(o => o.CargaPedido)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorContainer(int codigoContainer, double cnpjDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Pedido.Container.Codigo == codigoContainer && obj.XMLNotaFiscal.nfAtiva);
            if (cnpjDestinatario > 0)
                query = query.Where(obj => obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == cnpjDestinatario);

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                        .Fetch(o => o.CargaPedido)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedido(int codigoCargaPedido, double cnpjDestinatario, string ieRemetente, string ieDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva);
            if (cnpjDestinatario > 0)
                query = query.Where(obj => obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == cnpjDestinatario);

            if (!string.IsNullOrWhiteSpace(ieRemetente))
                query = query.Where(obj => obj.XMLNotaFiscal.IERemetente == ieRemetente || obj.XMLNotaFiscal.IERemetente == null || obj.XMLNotaFiscal.IERemetente == "");
            if (!string.IsNullOrWhiteSpace(ieDestinatario))
                query = query.Where(obj => obj.XMLNotaFiscal.IEDestinatario == ieDestinatario || obj.XMLNotaFiscal.IEDestinatario == null || obj.XMLNotaFiscal.IEDestinatario == "");

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                        .Fetch(o => o.CargaPedido)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedido(int codigoCargaPedido, bool notaAtiva = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido);
            if (notaAtiva)
                query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva);

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Tomador)
                        .Fetch(o => o.CargaPedido)
                        .ToList();
        }

        public List<(decimal Valor, int Numero)> BuscarPorCargaPedidoSemFactura(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.TipoFatura == false);

            return query.Select(o => ValueTuple.Create(o.XMLNotaFiscal.Valor, o.XMLNotaFiscal.Numero)).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarNotasFiscaisPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == carga && obj.XMLNotaFiscal.nfAtiva);

            return query.Select(obj => obj.XMLNotaFiscal).Fetch(o => o.Canhoto)
                        .Fetch(o => o.Emitente)
                        .Fetch(o => o.Destinatario)
                        .Fetch(o => o.Tomador)
                        .Distinct()
                        .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarNotasFiscaisPorCarga(int carga, double cnpjRemetente, double cnpjDestinatario, int numeroNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == carga && obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == cnpjRemetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == cnpjDestinatario && obj.XMLNotaFiscal.Numero == numeroNota);

            return query.Select(obj => obj.XMLNotaFiscal).Fetch(o => o.Canhoto)
                        .Fetch(o => o.Emitente)
                        .Fetch(o => o.Destinatario)
                        .Fetch(o => o.Tomador)
                        .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarNotasFiscaisPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Pedido.Codigo == pedido && obj.XMLNotaFiscal.nfAtiva);

            return query.Select(obj => obj.XMLNotaFiscal).Fetch(o => o.Canhoto)
                        .Fetch(o => o.Emitente)
                        .Fetch(o => o.Destinatario)
                        .Fetch(o => o.Tomador)
                        .Distinct()
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarNotasFiscaisPorPedidos(List<int> pedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => pedidos.Contains(obj.CargaPedido.Pedido.Codigo) && obj.XMLNotaFiscal.nfAtiva);

            return query.Select(obj => obj.XMLNotaFiscal).Fetch(o => o.Canhoto)
                        .Fetch(o => o.Emitente)
                        .Fetch(o => o.Destinatario)
                        .Fetch(o => o.Tomador)
                        .Distinct()
                        .ToList();
        }

        public List<int> BuscarListaPedidosComNotaPorPedidos(List<int> pedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => pedidos.Contains(obj.CargaPedido.Pedido.Codigo) && obj.XMLNotaFiscal.nfAtiva);

            return query.Select(o => o.CargaPedido.Pedido.Codigo).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal BuscarPrimeiraNotaFiscalPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Pedido.Codigo == pedido && obj.XMLNotaFiscal.nfAtiva);

            return query.Select(obj => obj.XMLNotaFiscal).Fetch(o => o.Canhoto)
                        .Fetch(o => o.Emitente)
                        .Fetch(o => o.Destinatario)
                        .Fetch(o => o.Tomador)
                        .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedidoFetch(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true);

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedidoSemFetch(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva);

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>> BuscarPorCargaPedidoSemFetchAsync(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva);

            return query.ToListAsync();
        }

        public List<int> BuscarCodigoPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva select obj;

            return result.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedidoEParticipantes(int codigoCargaPedido, double cpfCnpjRemetente, double? cpfCnpjExpedidor, double? cpfCnpjRecebedor, double cpfCnpjDestinatario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva && o.XMLNotaFiscal.Emitente.CPF_CNPJ == cpfCnpjRemetente && o.XMLNotaFiscal.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                if (cpfCnpjExpedidor > 0d)
                    query = query.Where(o => o.XMLNotaFiscal.Expedidor.CPF_CNPJ == cpfCnpjExpedidor);
                else
                    query = query.Where(o => o.XMLNotaFiscal.Expedidor == null);
            }

            if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                if (cpfCnpjRecebedor > 0d)
                    query = query.Where(o => o.XMLNotaFiscal.Recebedor.CPF_CNPJ == cpfCnpjRecebedor);
                else
                    query = query.Where(o => o.XMLNotaFiscal.Recebedor == null);
            }

            if (modalidade.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ModalidadeFrete == modalidade);

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente)
                        .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.CargaPedido)
                        .ToList();
        }

        public List<double> BuscarRemetentesPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj.XMLNotaFiscal.Emitente.CPF_CNPJ;
            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Cliente BuscarRemetentePorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && o.XMLNotaFiscal.nfAtiva == true);

            return query.Select(o => o.XMLNotaFiscal.Emitente).FirstOrDefault();
        }

        public List<double> BuscarDestinatariosPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj.XMLNotaFiscal.Destinatario.CPF_CNPJ;
            return result.Distinct().ToList();
        }

        public List<double> BuscarRemetentesPorCargaPedido(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj.XMLNotaFiscal.Emitente.CPF_CNPJ;
            return result.Distinct().ToList();
        }

        public List<double> BuscarDestinatariosPorCargaPedido(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj.XMLNotaFiscal.Destinatario.CPF_CNPJ;
            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarCadastroRemetentesPorCargaPedido(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj.XMLNotaFiscal.Emitente;
            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoPorRemetente(List<int> codigosCargaPedido, double remetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj;
            return result.Select(o => o.CargaPedido).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoPorDestinatario(List<int> codigosCargaPedido, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj;
            return result.Select(o => o.CargaPedido).Distinct().ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarCadastroDestinatariosPorCargaPedido(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj.XMLNotaFiscal.Destinatario;
            return result.Distinct().ToList();
        }

        public List<double> BuscarTomadoresPorCargaPedido(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => codigosCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva);

            return query.Select(o => ((double?)o.XMLNotaFiscal.Tomador.CPF_CNPJ) ?? 0D).Distinct().ToList();
        }

        public List<double> BuscarTomadoresPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva);

            return query.Select(o => ((double?)o.XMLNotaFiscal.Tomador.CPF_CNPJ) ?? 0D).Distinct().ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete> BuscarParticipantesPorCargaPedido(int codigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && o.XMLNotaFiscal.nfAtiva);

            if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Remetente = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                    Expedidor = o.XMLNotaFiscal.Expedidor.CPF_CNPJ,
                    Recebedor = o.XMLNotaFiscal.Recebedor.CPF_CNPJ,
                    Destinatario = o.XMLNotaFiscal.Destinatario.CPF_CNPJ
                }).Distinct().ToList();
            }
            else if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Remetente = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                    Expedidor = o.XMLNotaFiscal.Expedidor.CPF_CNPJ,
                    Destinatario = o.XMLNotaFiscal.Destinatario.CPF_CNPJ
                }).Distinct().ToList();
            }
            else if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Remetente = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                    Recebedor = o.XMLNotaFiscal.Recebedor.CPF_CNPJ,
                    Destinatario = o.XMLNotaFiscal.Destinatario.CPF_CNPJ
                }).Distinct().ToList();
            }
            else
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Remetente = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                    Destinatario = o.XMLNotaFiscal.Destinatario.CPF_CNPJ
                }).Distinct().ToList();
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete> BuscarParticipantesEModalidadesPorCargaPedido(int codigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && o.XMLNotaFiscal.nfAtiva);

            if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Remetente = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                    Expedidor = o.XMLNotaFiscal.Expedidor.CPF_CNPJ,
                    Recebedor = o.XMLNotaFiscal.Recebedor.CPF_CNPJ,
                    Destinatario = o.XMLNotaFiscal.Destinatario.CPF_CNPJ,
                    Modalidade = o.XMLNotaFiscal.ModalidadeFrete
                }).Distinct().ToList();
            }
            else if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Remetente = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                    Expedidor = o.XMLNotaFiscal.Expedidor.CPF_CNPJ,
                    Destinatario = o.XMLNotaFiscal.Destinatario.CPF_CNPJ,
                    Modalidade = o.XMLNotaFiscal.ModalidadeFrete
                }).Distinct().ToList();
            }
            else if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor)
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Remetente = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                    Recebedor = o.XMLNotaFiscal.Recebedor.CPF_CNPJ,
                    Destinatario = o.XMLNotaFiscal.Destinatario.CPF_CNPJ,
                    Modalidade = o.XMLNotaFiscal.ModalidadeFrete
                }).Distinct().ToList();
            }
            else
            {
                return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesCalculoFrete()
                {
                    Remetente = o.XMLNotaFiscal.Emitente.CPF_CNPJ,
                    Destinatario = o.XMLNotaFiscal.Destinatario.CPF_CNPJ,
                    Modalidade = o.XMLNotaFiscal.ModalidadeFrete
                }).Distinct().ToList();
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete> BuscarModalidadesPagamentoPorCargaPedido(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCargaPedido.Contains(obj.CargaPedido.Codigo) && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj.XMLNotaFiscal.ModalidadeFrete;
            return result.Distinct().ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete> BuscarModalidadesPagamentoPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true select obj.XMLNotaFiscal.ModalidadeFrete;
            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedido(int codigoCargaPedido, double remetente, double destinatario, double? tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadeFrete, string ieRemetente, string ieDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario);

            if (modalidadeFrete.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ModalidadeFrete == modalidadeFrete.Value);

            if (tomador.HasValue)
            {
                if (tomador > 0D)
                    query = query.Where(o => o.XMLNotaFiscal.Tomador.CPF_CNPJ == tomador);
                else
                    query = query.Where(o => o.XMLNotaFiscal.Tomador == null);
            }

            if (!string.IsNullOrWhiteSpace(ieRemetente))
                query = query.Where(obj => obj.XMLNotaFiscal.IERemetente == ieRemetente || obj.XMLNotaFiscal.IERemetente == null || obj.XMLNotaFiscal.IERemetente == "");
            if (!string.IsNullOrWhiteSpace(ieDestinatario))
                query = query.Where(obj => obj.XMLNotaFiscal.IEDestinatario == ieDestinatario || obj.XMLNotaFiscal.IEDestinatario == null || obj.XMLNotaFiscal.IEDestinatario == "");

            return query.Fetch(o => o.XMLNotaFiscal).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasPedidosPorCodigosRemetenteDestinatario(List<int> codigos, double remetente, double destinatario, string ieRemetente, string ieDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => codigos.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario);
            if (!string.IsNullOrWhiteSpace(ieRemetente))
                query = query.Where(obj => obj.XMLNotaFiscal.IERemetente == ieRemetente || obj.XMLNotaFiscal.IERemetente == null || obj.XMLNotaFiscal.IERemetente == "");
            if (!string.IsNullOrWhiteSpace(ieDestinatario))
                query = query.Where(obj => obj.XMLNotaFiscal.IEDestinatario == ieDestinatario || obj.XMLNotaFiscal.IEDestinatario == null || obj.XMLNotaFiscal.IEDestinatario == "");

            return
                query.Select(obj => obj.CargaPedido).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPorCodigosRemetenteDestinatario(List<int> codigos, double remetente, double destinatario, string ieRemetente, string ieDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            query = query.Where(obj => codigos.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario);

            if (!string.IsNullOrWhiteSpace(ieRemetente))
                query = query.Where(obj => obj.XMLNotaFiscal.IERemetente == ieRemetente || obj.XMLNotaFiscal.IERemetente == null || obj.XMLNotaFiscal.IERemetente == "");
            if (!string.IsNullOrWhiteSpace(ieDestinatario))
                query = query.Where(obj => obj.XMLNotaFiscal.IEDestinatario == ieDestinatario || obj.XMLNotaFiscal.IEDestinatario == null || obj.XMLNotaFiscal.IEDestinatario == "");

            return
                query.Select(obj => obj.CargaPedido).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Origem).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destino).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Remetente).
                Fetch(obj => obj.Pedido).
                ThenFetch(obj => obj.Destinatario).
                Fetch(obj => obj.Origem).
                Fetch(obj => obj.Destino).
                Fetch(obj => obj.Expedidor).
                Fetch(obj => obj.Recebedor).
                FirstOrDefault();
        }

        public List<string> BuscarIERemetentePorCargaPedido(List<int> codigosCargaPedidos, double remetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => codigosCargaPedidos.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.nfAtiva && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente);

            return query.Select(o => o.XMLNotaFiscal.IERemetente).Distinct().ToList();
        }

        public List<string> BuscarIEDestinatarioPorCargaPedido(List<int> codigosCargaPedidos, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => codigosCargaPedidos.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.nfAtiva && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario);

            return query.Select(o => o.XMLNotaFiscal.IEDestinatario).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedido(List<int> codigosCargaPedidos, double remetente, double destinatario, double? tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadeFrete, string ieRemetente, string ieDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => codigosCargaPedidos.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.nfAtiva && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario);

            if (modalidadeFrete.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ModalidadeFrete == modalidadeFrete.Value);

            if (tomador.HasValue)
            {
                if (tomador > 0D)
                    query = query.Where(o => o.XMLNotaFiscal.Tomador.CPF_CNPJ == tomador);
                else
                    query = query.Where(o => o.XMLNotaFiscal.Tomador == null);
            }

            if (!string.IsNullOrWhiteSpace(ieRemetente))
                query = query.Where(obj => obj.XMLNotaFiscal.IERemetente == ieRemetente || obj.XMLNotaFiscal.IERemetente == null || obj.XMLNotaFiscal.IERemetente == "");
            if (!string.IsNullOrWhiteSpace(ieDestinatario))
                query = query.Where(obj => obj.XMLNotaFiscal.IEDestinatario == ieDestinatario || obj.XMLNotaFiscal.IEDestinatario == null || obj.XMLNotaFiscal.IEDestinatario == "");

            return query.Fetch(o => o.XMLNotaFiscal).ThenFetch(obj => obj.Canhoto).ToList();
        }

        public List<int> BuscarCodigosPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva == true orderby obj.Peso descending select obj.Codigo;

            return result.ToList();
        }

        public bool ContemNotaFiscalSemInscricao(List<int> codigosCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query
                         where codigosCargaPedidos.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.nfAtiva == true
                         && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                         && (obj.XMLNotaFiscal.IERemetente == null || obj.XMLNotaFiscal.IERemetente == "" || obj.XMLNotaFiscal.IEDestinatario == null || obj.XMLNotaFiscal.IEDestinatario == "")
                         select obj;

            return result.Any();
        }

        public int ContarAgrupadosPorRemetenteEDestinatario(List<int> codigosCargaPedidos, bool ratearPorInscricaoEstadual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (ratearPorInscricaoEstadual)
            {
                var result = from obj in query
                             where codigosCargaPedidos.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.nfAtiva == true
                             && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                             select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ, IEEmitente = obj.XMLNotaFiscal.IERemetente, IEDestinatario = obj.XMLNotaFiscal.IEDestinatario };

                return result.Distinct().ToList().Count();
            }
            else
            {
                var result = from obj in query
                             where codigosCargaPedidos.Contains(obj.CargaPedido.Codigo) && obj.XMLNotaFiscal.nfAtiva == true
                             && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                             select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ };

                return result.Distinct().ToList().Count();
            }
        }

        public bool ContemNotaFiscalSemInscricao(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query
                         where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true
                         && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                         && (obj.XMLNotaFiscal.IERemetente == null || obj.XMLNotaFiscal.IERemetente == "" || obj.XMLNotaFiscal.IEDestinatario == null || obj.XMLNotaFiscal.IEDestinatario == "")
                         select obj;

            return result.Any();
        }

        public int ContarAgrupadosPorRemetenteEDestinatario(int codigoCargaPedido, bool ratearPorInscricaoEstadual, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (ratearPorInscricaoEstadual)
            {
                if (codigoCarga > 0)
                {
                    var result = from obj in query
                                 where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva == true
                                 && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                                 select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ, IEEmitente = obj.XMLNotaFiscal.IERemetente, IEDestinatario = obj.XMLNotaFiscal.IEDestinatario };

                    return result.Distinct().ToList().Count();
                }
                else
                {
                    var result = from obj in query
                                 where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true
                                 && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                                 select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ, IEEmitente = obj.XMLNotaFiscal.IERemetente, IEDestinatario = obj.XMLNotaFiscal.IEDestinatario };

                    return result.Distinct().ToList().Count();
                }
            }
            else
            {
                if (codigoCarga > 0)
                {
                    var result = from obj in query
                                 where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva == true
                                 && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                                 select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ };


                    return result.Distinct().ToList().Count();
                }
                else
                {
                    var result = from obj in query
                                 where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true
                                 && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                                 select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ };


                    return result.Distinct().ToList().Count();
                }
            }

        }

        public int ContarAgrupadosPorRemetenteEDestinatarioEModalidade(int codigoCargaPedido, bool ratearPorInscricaoEstadual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (ratearPorInscricaoEstadual)
            {
                var result = from obj in query
                             where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true
                             && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                             select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ, Modalidade = obj.XMLNotaFiscal.ModalidadeFrete, IEEmitente = obj.XMLNotaFiscal.IERemetente, IEDestinatario = obj.XMLNotaFiscal.IEDestinatario };

                return result.Distinct().ToList().Count();
            }
            else
            {
                var result = from obj in query
                             where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true
                             && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                             select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ, Modalidade = obj.XMLNotaFiscal.ModalidadeFrete };

                return result.Distinct().ToList().Count();
            }
        }

        public int ContarAgrupadosPorRemetenteEDestinatarioETomadorEModalidade(int codigoCargaPedido, bool ratearPorInscricaoEstadual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (ratearPorInscricaoEstadual)
            {
                var result = from obj in query
                             where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true
                             && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                             select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ, Modalidade = obj.XMLNotaFiscal.ModalidadeFrete, Tomador = (double?)obj.XMLNotaFiscal.Tomador.CPF_CNPJ, IEEmitente = obj.XMLNotaFiscal.IERemetente, IEDestinatario = obj.XMLNotaFiscal.IEDestinatario };
                return result.Distinct().ToList().Count();
            }
            else
            {
                var result = from obj in query
                             where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true
                             && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao
                             select new { Emitente = obj.XMLNotaFiscal.Emitente.CPF_CNPJ, Destinatario = obj.XMLNotaFiscal.Destinatario.CPF_CNPJ, Modalidade = obj.XMLNotaFiscal.ModalidadeFrete, Tomador = (double?)obj.XMLNotaFiscal.Tomador.CPF_CNPJ };

                return result.Distinct().ToList().Count();
            }
        }

        public int ContarPorCargaPedidoSemCTe(int codigoCargaPedido, bool somenteCargaPedidoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true && obj.CTes.Count == 0 && (obj.PossuiCTe || obj.PossuiNFS) select obj;

            if (somenteCargaPedidoFilialEmissora)
                result = result.Where(obj => obj.CargaPedido.CargaPedidoFilialEmissora);


            return result.Count();
        }

        public bool VerificarSeDentroMunicipioPorCargaPedidoSemCTe(int codigoCargaPedido, bool somenteCargaPedidoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true && obj.CTes.Count == 0 && obj.XMLNotaFiscal.Emitente.Localidade == obj.XMLNotaFiscal.Destinatario.Localidade select obj;

            if (somenteCargaPedidoFilialEmissora)
                result = result.Where(obj => obj.CargaPedido.CargaPedidoFilialEmissora);

            return result.Any();
        }

        //public int ContarPorCargaPedidoSemNFS(int codigoCargaPedido)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

        //    var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.nfAtiva == true && obj.NFSs.Count == 0 select obj.Codigo;

        //    return result.Count();
        //}

        public int ContarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj;
            return result.Count();
        }

        public decimal BuscarValorTotalPorCarga(List<int> codigoCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva && codigoCargas.Contains(obj.CargaPedido.Carga.Codigo) select obj;
            return result.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarValorTotalPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            return result.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarValorTotalPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva == true && obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj;
            return result.Sum(o => (decimal?)o.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarValorTotalPorCargaPedidos(List<int> codigosCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva
                               && codigosCargaPedido.Contains(obj.CargaPedido.Codigo)
                               && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.Valor) ?? 0m;
        }

        public decimal BuscarValorTotalFretePorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.ValorFrete) ?? 0m;
        }

        public decimal BuscarValorTotalFreteAjusteManualPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao);

            return query.Sum(o => (decimal?)o.ValorFreteAjusteManual) ?? 0m;
        }

        public decimal BuscarValorTotalFretePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Carga.Codigo == codigoCarga && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao);

            return query.Sum(o => (decimal?)o.XMLNotaFiscal.ValorFrete) ?? 0m;
        }

        public int ContarPorCargaParaCalculo(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao && obj.XMLNotaFiscal.nfAtiva);

            return query.Count();
        }

        public int ContarPorCargaPedidoAgrupadoPorModalidade(int codigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.ModalidadeFrete == modalidade && obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao);

            return query.Count();
        }

        public int ContarPorCargaPedidoNotasDeSubcontratacao(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Count();
        }

        public int BuscarVolumesPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Sum(obj => (int?)obj.XMLNotaFiscal.Volumes) ?? 0;
        }

        public int BuscarVolumesPorCargaPedidoSemPallet(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true && obj.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);
            return result.Sum(obj => (int?)obj.XMLNotaFiscal.Volumes) ?? 0;
        }

        public decimal BuscarPesoCubadoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.PesoCubado) ?? 0m;
        }

        public decimal BuscarPesoCubadoPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.PesoCubado) ?? 0m;
        }

        public decimal BuscarMetrosCubicosPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.MetrosCubicos) ?? 0m;
        }

        public decimal BuscarMetrosCubicosPorCargaPedidos(List<int> codigosCargaPedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => codigosCargaPedidos.Contains(o.CargaPedido.Codigo) && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.MetrosCubicos) ?? 0m;
        }

        public decimal BuscarMetrosCubicosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.MetrosCubicos) ?? 0m;
        }

        public int BuscarVolumesPorCarga(int codigoCarga, ClassificacaoNFe? classificacaoNFeDesconsiderar = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva);

            if (classificacaoNFeDesconsiderar.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNFeDesconsiderar.Value);

            return query.Sum(obj => (int?)obj.XMLNotaFiscal.Volumes) ?? 0;
        }

        public int BuscarVolumesPorCargaETipoFatura(int codigoCarga, ClassificacaoNFe? classificacaoNFeDesconsiderar = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva && !o.XMLNotaFiscal.TipoFatura);

            if (classificacaoNFeDesconsiderar.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNFeDesconsiderar.Value);

            return query.Sum(obj => (int?)obj.XMLNotaFiscal.Volumes) ?? 0;
        }

        public Task<int?> BuscarVolumesPorCargaAsync(int codigoCarga, ClassificacaoNFe? classificacaoNFeDesconsiderar = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva);

            if (classificacaoNFeDesconsiderar.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNFeDesconsiderar.Value);

            return query.SumAsync(obj => (int?)obj.XMLNotaFiscal.Volumes);
        }

        public Task<int?> BuscarVolumesPorCargaETipoFaturaAsync(int codigoCarga, ClassificacaoNFe? classificacaoNFeDesconsiderar = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva && !o.XMLNotaFiscal.TipoFatura);

            if (classificacaoNFeDesconsiderar.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNFeDesconsiderar.Value);

            return query.SumAsync(obj => (int?)obj.XMLNotaFiscal.Volumes);
        }

        public decimal BuscarPalletsPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.QuantidadePallets) ?? 0m;
        }

        public decimal BuscarPalletsPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.QuantidadePallets) ?? 0m;
        }

        public decimal BuscarPesoPorCarga(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCarga.Contains(obj.CargaPedido.Carga.Codigo) select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Sum(obj => (decimal?)obj.XMLNotaFiscal.Peso) ?? 0m;
        }

        public decimal BuscarMediaPesoPorCarga(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where codigosCarga.Contains(obj.CargaPedido.Carga.Codigo) select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Average(obj => (decimal?)obj.XMLNotaFiscal.Peso) ?? 0m;
        }

        public decimal BuscarPesoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Sum(obj => (decimal?)obj.XMLNotaFiscal.Peso) ?? 0m;
        }

        public decimal BuscarPesoPorCargas(List<int> lstCodigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where lstCodigosCarga.Contains(obj.CargaPedido.Carga.Codigo) select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Sum(obj => (decimal?)obj.XMLNotaFiscal.Peso) ?? 0m;
        }

        public decimal BuscarQtdPorCargas(List<int> lstCodigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where lstCodigosCarga.Contains(obj.CargaPedido.Carga.Codigo) select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Sum(obj => (decimal?)obj.XMLNotaFiscal.QuantidadePallets) ?? 0m;
        }

        public int ContarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.nfAtiva);

            return query.Count();
        }

        public decimal BuscarPesoPorCargaPedido(int codigoCargaPedido, IList<int> notasFiscaisDuplicadas, bool semNotasDePallet)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva);

            if (semNotasDePallet)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            if (semNotasDePallet && query.Count() == 0)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet);

            if (notasFiscaisDuplicadas.Count > 0)
                query = query.Where(o => !notasFiscaisDuplicadas.Contains(o.XMLNotaFiscal.Codigo));

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.Peso) ?? 0m;
        }

        public int BuscarPesoPorCargaPedidos(List<int> codigosCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => codigosCargaPedido.Contains(o.CargaPedido.Codigo) && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (int?)obj.XMLNotaFiscal.Peso) ?? 0;
        }

        public decimal BuscarPesoPedidosNotasDuplicas(int notafiscal, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga && o.XMLNotaFiscal.Codigo == notafiscal && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.CargaPedido.Pedido.PesoTotal) ?? 0m;
        }

        public decimal BuscarPesoCargaPedidosNotasDuplicas(int notafiscal, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga && o.XMLNotaFiscal.Codigo == notafiscal && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.CargaPedido.Peso) ?? 0m;
        }

        public decimal BuscarPesoLiquidoPorCargaPedido(int codigoCargaPedido, IList<int> notasFiscaisDuplicadas, bool semNotasDePallet)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.nfAtiva);

            if (semNotasDePallet)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet);

            if (semNotasDePallet && query.Count() == 0)
                query = query.Where(o => o.XMLNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet);

            if (notasFiscaisDuplicadas.Count > 0)
                query = query.Where(o => !notasFiscaisDuplicadas.Contains(o.XMLNotaFiscal.Codigo));

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.PesoLiquido) ?? 0m;
        }

        public decimal BuscarPesoLiquidoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva);

            return query.Sum(obj => (decimal?)obj.XMLNotaFiscal.PesoLiquido) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLPorCargaPedido(int codigoCargaPedido, bool nfAtivas, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == nfAtivas);
            return result.Select(obj => obj.XMLNotaFiscal)
                .Fetch(o => o.Emitente).ThenFetch(o => o.Localidade)
                .Fetch(o => o.Destinatario).ThenFetch(o => o.Localidade)
                .OrderByDescending(obj => obj.CanceladaPeloEmitente).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarXMLPorCargaPedido(int codigoCargaPedido, bool nfAtivas = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == nfAtivas);
            return result.Select(obj => obj.XMLNotaFiscal).Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ConsultarPorEmitenteEDestinoCargaPedido(string numeroCarregamento, bool filtrarNotasCompativeisPeloDestinatario, string numeroPedido, int numeroNotaInicial, int numeroNotaFinal, double emitente, double destinatario, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, TipoServicoMultisoftware tipoServicoMultisoftware, bool visualizacaoOperadorLogistico, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = ObterConsultaPorEmitenteEDestinoCargaPedido(numeroCarregamento, numeroPedido, numeroNotaInicial, numeroNotaFinal, emitente, filtrarNotasCompativeisPeloDestinatario ? destinatario : 0, dataEmissaoInicial, dataEmissaoFinal, tipoServicoMultisoftware, visualizacaoOperadorLogistico);

            if (maximoRegistros > 0)
                query = query.Skip(inicioRegistros).Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query = query.OrderBy(propOrdenacao + " " + dirOrdenacao);

            return query.Fetch(o => o.Emitente)
                        .Fetch(o => o.Destinatario)
                        .WithOptions(o => o.SetTimeout(60)).ToList();
        }

        public int ContarPorEmitenteEDestinatarioCargaPedido(string numeroCarregamento, bool filtrarNotasCompativeisPeloDestinatario, string numeroPedido, int numeroNotaInicial, int numeroNotaFinal, double emitente, double destinatario, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, TipoServicoMultisoftware tipoServicoMultisoftware, bool visualizacaoOperadorLogistico)
        {
            var query = ObterConsultaPorEmitenteEDestinoCargaPedido(numeroCarregamento, numeroPedido, numeroNotaInicial, numeroNotaFinal, emitente, filtrarNotasCompativeisPeloDestinatario ? destinatario : 0, dataEmissaoInicial, dataEmissaoFinal, tipoServicoMultisoftware, visualizacaoOperadorLogistico);

            return query.Count();
        }

        public List<int> ObterCodigosDocumentosPorEmitenteEDestinoCargaPedido(string numeroCarregamento, bool selecionarTodos, List<int> codigosDocumentos, bool filtrarNotasCompativeisPeloDestinatario, string numeroPedido, int numeroNotaInicial, int numeroNotaFinal, double emitente, double destinatario, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, TipoServicoMultisoftware tipoServicoMultisoftware, bool visualizacaoOperadorLogistico)
        {
            var query = ObterConsultaPorEmitenteEDestinoCargaPedido(numeroCarregamento, numeroPedido, numeroNotaInicial, numeroNotaFinal, emitente, filtrarNotasCompativeisPeloDestinatario ? destinatario : 0, dataEmissaoInicial, dataEmissaoFinal, tipoServicoMultisoftware, visualizacaoOperadorLogistico);

            if (selecionarTodos)
                query = query.Where(o => !codigosDocumentos.Contains(o.Codigo));
            else
                query = query.Where(o => codigosDocumentos.Contains(o.Codigo));

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ConsultarPorOrigemDaCarga(List<int> codigosOrigens, int numeroNotaFiscal, string chaveNotaFiscal, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = ObterConsultaPorOrigemDaCarga(codigosOrigens, numeroNotaFiscal, chaveNotaFiscal);

            if (maximoRegistros > 0)
                query = query.Skip(inicioRegistros).Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query = query.OrderBy(propOrdenacao + " " + dirOrdenacao);

            return query.Fetch(o => o.Emitente)
                        .Fetch(o => o.Destinatario)
                        .WithOptions(o => o.SetTimeout(60)).ToList();
        }

        public int ContarPorOrigemDaCarga(List<int> codigosOrigens, int numeroNotaFiscal, string chaveNotaFiscal)
        {
            var query = ObterConsultaPorOrigemDaCarga(codigosOrigens, numeroNotaFiscal, chaveNotaFiscal);

            return query.Count();
        }

        public List<int> ObterCodigosDocumentosPorPorOrigemDaCarga(List<int> codigosOrigens, int numeroNotaFiscal, string chaveNotaFiscal, bool selecionarTodos, List<int> codigosDocumentos)
        {
            var query = ObterConsultaPorOrigemDaCarga(codigosOrigens, numeroNotaFiscal, chaveNotaFiscal);

            if (selecionarTodos)
                query = query.Where(o => !codigosDocumentos.Contains(o.Codigo));
            else
                query = query.Where(o => codigosDocumentos.Contains(o.Codigo));

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorNumeroEEmitente(int numeroNF, double emitente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.nfAtiva && obj.XMLNotaFiscal.Numero == numeroNF && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == emitente && !obj.CargaPedido.PedidoEncaixado);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> ConsultarParaEnvioImpressao(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && obj.CargaPedido.Codigo == cargaPedido
                            && obj.XMLNotaFiscal != null
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> ConsultarParaEnvioImpressaoPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && obj.CargaPedido.Pedido.Codigo == codigoPedido
                            && obj.XMLNotaFiscal != null
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> ConsultarParaEnvioImpressaoPorPedidos(List<int> codigosPedidos, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && codigosPedidos.Contains(obj.CargaPedido.Pedido.Codigo)
                            && obj.CargaPedido.Carga.Codigo == codigoCarga
                            && obj.XMLNotaFiscal != null
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedidos(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && codigosPedidos.Contains(obj.CargaPedido.Codigo)
                            && obj.XMLNotaFiscal != null
                         select obj;

            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public List<int> BuscarCodigosCargaPedidosSemPallet(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && codigosPedidos.Contains(obj.CargaPedido.Codigo)
                            && obj.XMLNotaFiscal.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet
                            && obj.XMLNotaFiscal != null
                         select obj;

            return result
                .Select(o => o.CargaPedido.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedidosSemFetchCanhoto(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && codigosPedidos.Contains(obj.CargaPedido.Codigo)
                            && obj.XMLNotaFiscal != null
                         select obj;

            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedidosComFetchCargaPedido(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && codigosPedidos.Contains(obj.CargaPedido.Codigo)
                            && obj.XMLNotaFiscal != null
                         select obj;

            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Carga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && obj.CargaPedido.Pedido.Codigo == pedido
                         select obj;

            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public bool PedidoPossuiNotaFiscal(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && obj.CargaPedido.Pedido.Codigo == pedido
                         select obj;

            return result.Any();
        }

        public DateTime BuscarDataEmissaoPrimeiraNFE(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && obj.CargaPedido.Pedido.Codigo == pedido
                         select obj;

            return result.Select(nf => nf.XMLNotaFiscal.DataEmissao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorPedidoENota(int pedido, int xmlNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where
                            obj.XMLNotaFiscal.nfAtiva == true
                            && obj.CargaPedido.Pedido.Codigo == pedido
                         select obj;

            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorPedidos(List<int> pedidos)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.XMLNotaFiscal.nfAtiva == true && pedidos.Contains(o.CargaPedido.Pedido.Codigo));

            return consultaPedidoXMLNotaFiscal
                .Fetch(o => o.CargaPedido)
                .Fetch(obj => obj.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ConsultarParaEnvioImpressaoPorCargaPedidoENota(int codigoCargaPedido, int numeroNota)
        {
            var consultaPedidoXmlNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj =>
                    obj.XMLNotaFiscal.nfAtiva &&
                    (obj.XMLNotaFiscal.Numero == numeroNota) &&
                    (obj.CargaPedido.Codigo == codigoCargaPedido)
                );

            return consultaPedidoXmlNotaFiscal.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> ConsultarPorCargaPedido(int codigoCargaPedido, bool nfAtivas, bool? tipoFatura, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            //var result = from obj in query where obj.XMLNotaFiscal.nfAtiva == nfAtivas && obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj;

            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva == nfAtivas && obj.CargaPedido.Codigo == codigoCargaPedido select obj;

            if (tipoFatura.HasValue)
                result = result.Where(obj => ((bool?)obj.XMLNotaFiscal.TipoFatura ?? false) == tipoFatura);

            return result.Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Emitente).ThenFetch(o => o.Localidade)
                         .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Localidade)
                         .Fetch(o => o.CargaPedido)
                         .OrderByDescending(obj => obj.XMLNotaFiscal.CanceladaPeloEmitente)
                         .OrderBy(propOrdenacao + " " + dirOrdenacao)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public int ContarPorCargaPedido(int codigoCargaPedido, bool nfAtivas = true, bool? tipoFatura = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            //var result = from obj in query where obj.XMLNotaFiscal.nfAtiva == nfAtivas && obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj;
            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva == nfAtivas && obj.CargaPedido.Codigo == codigoCargaPedido select obj;

            if (tipoFatura.HasValue)
                result = result.Where(obj => ((bool?)obj.XMLNotaFiscal.TipoFatura ?? false) == tipoFatura);

            return result.Count();
        }

        public int ContarXMLPorNotaFiscal(int notaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var result = from obj in query where obj.XMLNotaFiscal.Codigo == notaFiscal select obj;
            result = result.Where(obj => obj.XMLNotaFiscal.nfAtiva == true);
            return result.Count();
        }

        public bool VerificarSeExistePorNotaFiscalECarga(int codigoXMLNotaFiscal, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Any();
        }

        public bool PossuiNotaRemesaPallet(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet);

            return query.Any();
        }

        public bool VerificarSeExistePorNotaFiscalECargaPedido(int codigoXMLNotaFiscal, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Codigo == codigoCargaPedido);

            return query.Any();
        }

        public bool VerificarSeExistePorNotaFiscalEmOutraCarga(int codigoXMLNotaFiscal, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Carga.Codigo != codigoCarga);

            return query.Any();
        }

        public Task<bool> VerificarSeExistePorNotaFiscalEmOutraCargaAsync(int codigoXMLNotaFiscal, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Carga.Codigo != codigoCarga);

            return query.AnyAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorNotaFiscalECargaPedido(int codigoXMLNotaFiscal, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Codigo == codigoCargaPedido);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorNotaFiscalECargaPedidoAsync(int codigoXMLNotaFiscal, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.Codigo == codigoCargaPedido);

            return query.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorNotaFiscalECarga(int codigoXMLNotaFiscal, int codigoCargaOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.CargaOrigem.Codigo == codigoCargaOrigem);

            return query.Fetch(obj => obj.CargaPedido).ThenFetch(obj => obj.Pedido).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorNotaFiscalECargaAsync(int codigoXMLNotaFiscal, int codigoCargaOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaPedido.CargaOrigem.Codigo == codigoCargaOrigem);

            return query.FirstOrDefaultAsync();
        }

        public bool VerificarSeExistePorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido);

            return query.Any();
        }

        public bool VerificarSeExistePorCargaPedidoComNota(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.XMLNotaFiscal != null);

            return query.Any();
        }

        public List<Dominio.Entidades.Cliente> BuscarDestinatariosPorCargaPedidos(List<int> codigosCargaPedido)
        {
            if (codigosCargaPedido.Count < 2000)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

                var result = from obj in query
                             where
                             codigosCargaPedido.Contains(obj.CargaPedido.Codigo)
                             && obj.XMLNotaFiscal.Destinatario != null
                             select obj.XMLNotaFiscal.Destinatario;

                return result.ToList();
            }

            List<Dominio.Entidades.Cliente> listaRetornar = new List<Dominio.Entidades.Cliente>();

            List<int> listaOriginal = codigosCargaPedido;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                      .Where(pa => lote.Contains(pa.CargaPedido.Codigo) && pa.XMLNotaFiscal.Destinatario != null);

                listaRetornar.AddRange(query.Select(obj => obj.XMLNotaFiscal.Destinatario).ToList());

                indiceInicial += tamanhoLote;
            }
            return listaRetornar;
        }

        public async Task<List<Dominio.Entidades.Cliente>> BuscarDestinatariosPorCargaPedidosAsync(List<int> codigosCargaPedido)
        {
            List<Dominio.Entidades.Cliente> listaRetornar = new List<Dominio.Entidades.Cliente>();

            List<int> listaOriginal = codigosCargaPedido;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                      .Where(pa => lote.Contains(pa.CargaPedido.Codigo) && pa.XMLNotaFiscal.Destinatario != null);

                listaRetornar.AddRange(await query.Select(obj => obj.XMLNotaFiscal.Destinatario).ToListAsync());

                indiceInicial += tamanhoLote;
            }
            return listaRetornar;
        }

        public List<int> BuscarCodigosNFesSemCanhotoGerado(bool gerarCanhotoSempre)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> queryCanhotos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe &&
                                     o.XMLNotaFiscal.nfAtiva &&
                                     //o.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao &&
                                     situacoes.Contains(o.CargaPedido.Carga.SituacaoCarga) && !o.CargaPedido.Carga.CargaTransbordo && o.CargaPedido.Carga.TipoFreteEscolhido != TipoFreteEscolhido.Cliente &&
                                     !queryCanhotos.Any(c => c.XMLNotaFiscal.Codigo == o.XMLNotaFiscal.Codigo) &&
                                     (gerarCanhotoSempre ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && (o.CargaPedido.Pedido.Remetente.ExigeCanhotoFisico.Value || o.CargaPedido.Pedido.Remetente.GrupoPessoas.ExigeCanhotoFisico.Value)) ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && (o.CargaPedido.Pedido.Destinatario.ExigeCanhotoFisico.Value || o.CargaPedido.Pedido.Destinatario.GrupoPessoas.ExigeCanhotoFisico.Value)) ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && (o.CargaPedido.Expedidor.ExigeCanhotoFisico.Value || o.CargaPedido.Expedidor.GrupoPessoas.ExigeCanhotoFisico.Value)) ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && (o.CargaPedido.Recebedor.ExigeCanhotoFisico.Value || o.CargaPedido.Recebedor.GrupoPessoas.ExigeCanhotoFisico.Value)) ||
                                      (o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && (o.CargaPedido.Tomador.ExigeCanhotoFisico.Value || o.CargaPedido.Tomador.GrupoPessoas.ExigeCanhotoFisico.Value))));

            return query.Select(o => o.Codigo).WithOptions(o => { o.SetTimeout(6000); }).ToList();
        }

        public bool VerificarSeExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Codigo).Any();
        }

        public bool VerificarSeAlgumaEmContingenciaNaoPossuiSegundoCodigoBarras(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = (from obj in query
                          where obj.XMLNotaFiscal.nfAtiva == true &&
                            obj.CargaPedido.Carga.Codigo == codigoCarga &&
                            (obj.XMLNotaFiscal.TipoEmissao == TipoEmissaoNotaFiscal.ContingenciaFSDA || obj.XMLNotaFiscal.TipoEmissao == TipoEmissaoNotaFiscal.ContingenciaFSIA) &&
                            (obj.XMLNotaFiscal.SegundoCodigoBarras == null || obj.XMLNotaFiscal.SegundoCodigoBarras == "")
                          select obj);

            return result.Any();
        }

        public List<int> BuscarNumeroNotasFiscaisPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where obj.CargaPedido.Carga.Codigo == codigoCarga && (obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                         select obj;

            return result.Select(o => o.XMLNotaFiscal.Numero).ToList();
        }

        public List<int> BuscarNumeroNotasFiscaisPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where obj.CargaPedido.Codigo == codigoCargaPedido && (obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                         select obj;

            return result.Select(o => o.XMLNotaFiscal.Numero).ToList();
        }

        public List<string> BuscarNumeroPedidoEmbarcadorPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query
                         where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.CargaPedido.Pedido != null && (obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                         select obj;

            return result.Select(o => o.CargaPedido.Pedido.NumeroPedidoEmbarcador).ToList();
        }

        public bool VerificarSeExisteNumeroOutroDocumentoOutraCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var queryDaCargaAtual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(obj => obj.CargaPedido.Carga.Codigo != codigoCarga && obj.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                                       obj.XMLNotaFiscal.NumeroOutroDocumento != null && obj.XMLNotaFiscal.NumeroOutroDocumento != "");
            queryDaCargaAtual = queryDaCargaAtual.Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.NumeroOutroDocumento != null && obj.XMLNotaFiscal.NumeroOutroDocumento != "");

            query = query.Where(o => queryDaCargaAtual.Where(a => a.XMLNotaFiscal.NumeroOutroDocumento.Equals(o.XMLNotaFiscal.NumeroOutroDocumento) &&
                                                                  a.CargaPedido.Pedido.Remetente == o.CargaPedido.Pedido.Remetente &&
                                                              ((o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? o.CargaPedido.Pedido.Remetente.CPF_CNPJ :
                                                                o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? o.CargaPedido.Pedido.Destinatario.CPF_CNPJ :
                                                                o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? o.CargaPedido.Expedidor.CPF_CNPJ :
                                                                o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? o.CargaPedido.Recebedor.CPF_CNPJ :
                                                                o.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? o.CargaPedido.Tomador.CPF_CNPJ : 0d)
                                                                ==
                                                                (a.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? a.CargaPedido.Pedido.Remetente.CPF_CNPJ :
                                                                a.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? a.CargaPedido.Pedido.Destinatario.CPF_CNPJ :
                                                                a.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? a.CargaPedido.Expedidor.CPF_CNPJ :
                                                                a.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? a.CargaPedido.Recebedor.CPF_CNPJ :
                                                                a.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? a.CargaPedido.Tomador.CPF_CNPJ : 0d))
                                                            ).Any());

            return query.Any();
        }

        public List<int> BuscarCargasPedidoPorCargaNumeroNF(int codigoCarga, int numeroNF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            var result = from obj in query where obj.XMLNotaFiscal.nfAtiva && obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.Numero == numeroNF select obj.CargaPedido.Codigo;

            return result.ToList();
        }

        public List<int> BuscarCodigosCargaPedidoPorCargaETipoDocumento(int codigoCarga, TipoDocumento tipoDocumento)
        {
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.TipoDocumento == tipoDocumento && o.XMLNotaFiscal.nfAtiva);

            return consultaPedidoXMLNotaFiscal
                .Select(o => o.CargaPedido.Codigo)
                .Distinct()
                .ToList();
        }

        public bool ExisteComMesmaLocalidadePorCargaPedido(int codigoCargaPedido, int? codigoLocalidadeOrigem, int? codigoLocalidadeDestino)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.PossuiCTe && o.CargaPedido.Codigo == codigoCargaPedido);

            query = query.Where(o => (codigoLocalidadeOrigem ?? o.XMLNotaFiscal.Emitente.Localidade.Codigo) == (codigoLocalidadeDestino ?? o.XMLNotaFiscal.Destinatario.Localidade.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedido(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => codigosCargaPedido.Contains(obj.CargaPedido.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaPedidoComFetch(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => codigosCargaPedido.Contains(obj.CargaPedido.Codigo));

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.CargaPedido).ThenFetch(obj => obj.Pedido)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorProtocoloCargaEChave(int protocoloCarga, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => obj.CargaPedido.Carga.Protocolo == protocoloCarga && obj.XMLNotaFiscal.Chave == chave);

            return query.FirstOrDefault();
        }

        public List<(int CodigoCarga, int NumeroNota)> BuscarNumeroNotasFiscaisECodigosCargasPorCarga(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => codigosCarga.Contains(o.CargaPedido.Carga.Codigo) && (o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada));

            return query.Select(o => ValueTuple.Create(o.CargaPedido.Carga.Codigo, o.XMLNotaFiscal.Numero)).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaComExpedidor(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva && o.CargaPedido.Expedidor != null);

            return query
                .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido)
                .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarPorCargaComRecebedor(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.XMLNotaFiscal.nfAtiva && o.CargaPedido.Recebedor != null);

            return query
                .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido)
                .Fetch(o => o.XMLNotaFiscal).ThenFetch(o => o.Canhoto)
                .ToList();
        }

        public List<(int CodigoCarga, int QuantidadeNotas)> BuscarQuantidadeNotasCarga(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(o =>
                    codigosCarga.Contains(o.CargaCTe.Carga.Codigo) &&
                    !o.PedidoXMLNotaFiscal.XMLNotaFiscal.IrrelevanteParaFrete &&
                    o.PedidoXMLNotaFiscal.CargaPedido.Carga.SituacaoCarga
                        != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                .GroupBy(o => o.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo)
                .Select(g => new
                {
                    CodigoCarga = g.Key,
                    QuantidadeNotas = g.Count()
                })
                .ToList()
                .Select(x => (x.CodigoCarga, x.QuantidadeNotas))
                .ToList();

            return query;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPedidoXMLNotaFiscalPorCodigoCTe(int codigoCTe, int codigoCarga)
        {
            var dCTe = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>()
                .Where(obj => obj.CTE.Codigo == codigoCTe);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => obj.XMLNotaFiscal.Chave == dCTe.Select(o => o.ChaveNFE).FirstOrDefault() &&
                obj.CargaPedido.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais> ObterSumarizacaoQuantidadesPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            query = query.Where(o => o.CargaPedido.Carga == carga && o.XMLNotaFiscal.nfAtiva);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais
            {
                CodigoCargaPedido = o.CargaPedido.Codigo,
                CodigoPedidoXMLNotaFiscal = o.Codigo,
                CodigoXMLNotaFiscal = o.XMLNotaFiscal.Codigo,
                MetrosCubicos = (decimal?)o.XMLNotaFiscal.MetrosCubicos ?? 0m,
                Peso = (decimal?)o.XMLNotaFiscal.Peso ?? 0m,
                PesoCubado = (decimal?)o.XMLNotaFiscal.PesoCubado ?? 0m,
                PesoLiquido = (decimal?)o.XMLNotaFiscal.PesoLiquido ?? 0m,
                TipoNotaFiscalIntegrada = o.XMLNotaFiscal.TipoNotaFiscalIntegrada,
                Volumes = (int?)o.XMLNotaFiscal.Volumes ?? 0,
                TipoFatura = o.XMLNotaFiscal.TipoFatura
            }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarNotasColetaEntregaEmSegundoTrecho(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(o =>
                    o.CargaPedido.Pedido.Codigo == codigoPedido &&
                    o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    !o.CargaPedido.Carga.TipoOperacao.PedidoColetaEntrega);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarPorCTe(int codigoCTe)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.CTe.Codigo == codigoCTe);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => o.Carga.Codigo == queryCargaCTe.Select(x => x.Carga.Codigo).FirstOrDefault());

            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            return result.Where(o => o.CargaPedido.Codigo == queryCargaPedido.Select(x => x.Codigo).FirstOrDefault()).FirstOrDefault();
        }

        public bool ExisteNotaCargaPedidoPorChave(int codigoCargaPedido, string chaveNotaFiscal)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            return result.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.XMLNotaFiscal.Chave == chaveNotaFiscal && o.XMLNotaFiscal.nfAtiva == true).Select(o => o.Codigo).Any();
        }

        public void UpdatePorCargaLimparAjusteConferenciaDeFrete(int codigoCarga)
        {
            UnitOfWork.Sessao.CreateSQLQuery($"UPDATE T_PEDIDO_XML_NOTA_FISCAL SET PNF_VALOR_FRETE_AJUSTE_MANUAL = 0 where CPE_CODIGO IN (SELECT CPE_CODIGO FROM T_CARGA_PEDIDO WHERE CAR_CODIGO = :codigoCarga)").SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
        }

        public void DeletarPorPedidoCTeParaSubContratacao(List<int> codigos)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_INTEGRACAO_AVIPED where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_PEDIDO_XML_NOTA_FISCAL_NFS where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_ENTREGA_NOTA_FISCAL where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery($"DELETE PedidoXMLNotaFiscal obj WHERE obj.Codigo in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_INTEGRACAO_AVIPED where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_PEDIDO_XML_NOTA_FISCAL_NFS where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_ENTREGA_NOTA_FISCAL where PNF_CODIGO in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery($"DELETE PedidoXMLNotaFiscal obj WHERE obj.Codigo in (:codigos)").SetParameterList("codigos", codigos).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarNotaPalletCompativel(double codigoDestinatario, double codigoRemetente, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
               .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga &&
                       o.CargaPedido.PedidoPallet &&
                       o.CargaPedido.Pedido.Remetente.CPF_CNPJ.Equals(codigoRemetente) &&
                       o.XMLNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet &&
                       o.XMLNotaFiscal.Destinatario.CPF_CNPJ.Equals(codigoDestinatario));

            var subQueryCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => o.Carga.Codigo == codigoCarga &&
                        o.CTe.Destinatario.Cliente.CPF_CNPJ.Equals(codigoDestinatario) &&
                        o.CTe.Remetente.Cliente.CPF_CNPJ.Equals(codigoRemetente));

            query = query.Where(o => !subQueryCte.Where(sub => sub.CTe.XMLNotaFiscais.Contains(o.XMLNotaFiscal)).Any());

            return query.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal> ConsultarParaMontagemCarga(List<int> codigoPedidos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal> result = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal>();

            int take = 1000;
            int start = 0;
            while (start < codigoPedidos?.Count)
            {
                List<int> codigos = codigoPedidos.Skip(start).Take(take).ToList();

                var query = ObterConsultaParaMontagemCarga(codigos, false, parametrosConsulta);

                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal)));

                result.AddRange(query.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoNotaFiscal>().Result);

                start += take;
            }

            return result;
        }

        public int ContarConsultaParaMontagemCarga(List<int> codigoPedidos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            int result = 0;

            int take = 1000;
            int start = 0;
            while (start < codigoPedidos?.Count)
            {
                List<int> codigos = codigoPedidos.Skip(start).Take(take).ToList();

                var query = ObterConsultaParaMontagemCarga(codigos, true, parametrosConsulta);

                result += query.UniqueResultAsync<int>().Result;

                start += take;
            }

            return result;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObterConsultaPorEmitenteEDestinoCargaPedido(string numeroCarregamento, string numeroPedido, int numeroNotaInicial, int numeroNotaFinal, double emitente, double destinatario, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, TipoServicoMultisoftware tipoServicoMultisoftware, bool visualizacaoOperadorLogistico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            query = query.Where(obj => obj.nfAtiva && (obj.SemCarga || (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador && visualizacaoOperadorLogistico))); //&& obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao);

            if (emitente > 0)
                query = query.Where(o => (o.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida && o.Emitente.CPF_CNPJ == emitente) || (o.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada && o.Destinatario.CPF_CNPJ == emitente));
            //query = query.Where(o => o.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? o.Emitente.CPF_CNPJ == emitente : o.Destinatario.CPF_CNPJ == emitente);

            if (destinatario > 0)
                query = query.Where(o => (o.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida && o.Destinatario.CPF_CNPJ == destinatario) || (o.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada && o.Emitente.CPF_CNPJ == destinatario));
            //query = query.Where(o => o.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada ? o.Destinatario.CPF_CNPJ == destinatario : o.Emitente.CPF_CNPJ == destinatario);

            if (numeroNotaInicial > 0)
                query = query.Where(o => o.Numero >= numeroNotaInicial);

            if (numeroNotaFinal > 0)
                query = query.Where(o => o.Numero <= numeroNotaFinal);

            if (!string.IsNullOrWhiteSpace(numeroPedido))
                query = query.Where(o => o.NumeroDT == numeroPedido || o.NumeroDT == null || o.NumeroDT == "");

            if (!string.IsNullOrWhiteSpace(numeroCarregamento))
                query = query.Where(o => o.NumeroCarregamento.Contains(numeroCarregamento));

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Date >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Date <= dataEmissaoFinal.Date);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObterConsultaPorOrigemDaCarga(List<int> codigosOrigens, int numeroNotaFiscal, string chaveNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            query = query.Where(obj => obj.nfAtiva);// && obj.SemCarga);

            query = query.Where(obj => (obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida && codigosOrigens.Contains(obj.Emitente.Localidade.Codigo)) ||
                                       (obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada && codigosOrigens.Contains(obj.Destinatario.Localidade.Codigo)));

            var queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var resultQueryPedidoXMLNotaFiscal = from obj in queryPedidoXMLNotaFiscal select obj;
            query = query.Where(o => !resultQueryPedidoXMLNotaFiscal.Where(a => a.XMLNotaFiscal.Codigo == o.Codigo).Any());

            if (numeroNotaFiscal > 0)
                query = query.Where(obj => obj.Numero == numeroNotaFiscal);

            if (!string.IsNullOrWhiteSpace(chaveNotaFiscal))
                query = query.Where(obj => obj.Chave == chaveNotaFiscal);

            return query;
        }

        private NHibernate.ISQLQuery ObterConsultaParaMontagemCarga(List<int> codigoPedidos, bool somenteContar, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContar)
                sql.Append(@"select distinct(count(0) over ())");
            else
            {
                sql.Append(@$"
                     select 
                       pedidoNotaFiscal.PED_CODIGO as CodigoPedido, 
                       xmlNotaFiscal.NFX_CODIGO as Codigo,
                       xmlNotaFiscal.NF_NUMERO as NumeroNota,
                       xmlNotaFiscal.NF_SERIE as Serie,
                       xmlNotaFiscal.NF_CHAVE as ChaveNota,
                       xmlNotaFiscal.NF_DATA_EMISSAO as DataEmissao,
                       xmlNotaFiscal.NF_PESO as Peso,
                       xmlNotaFiscal.NF_PESO_LIQUIDO as PesoLiquido,
                       xmlNotaFiscal.NF_METROS_CUBICOS as Cubagem,
                       xmlNotaFiscal.NF_VOLUMES as Volumes,
                       remetente.CLI_CGCCPF as RemetenteCPFCNPJ,
                       remetente.CLI_NOME as RemetenteNome,
                       destinatario.CLI_CGCCPF as DestinatarioCPFCNPJ,
                       destinatario.CLI_NOME as DestinatarioNome,
                       localidadeOrigem.LOC_DESCRICAO as CidadeOrigem,
                       localidadeDestino.LOC_DESCRICAO as CidadeDestino,
		               localidadeOrigem.UF_SIGLA as UFOrigem,
                       localidadeDestino.UF_SIGLA as UFDestino,
                       expedidor.CLI_CGCCPF as ExpedidorCPFCNPJ,
                       expedidor.CLI_NOME as ExpedidorNome,
                       recebedor.CLI_CGCCPF as RecebedorCPFCNPJ,
                       recebedor.CLI_NOME as RecebedorNome,
                       filial.FIL_DESCRICAO as Filial,
                       pedido.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedidoEmbarcador,  
                       COALESCE(grupoPessoasRecebedor.GRP_DESCRICAO, grupoPessoasDestinatario.GRP_DESCRICAO) as GrupoPessoa
                ");
            }

            sql.Append(@"
                    from 
                       T_XML_NOTA_FISCAL xmlNotaFiscal
		               join T_PEDIDO_NOTAS_FISCAIS pedidoNotaFiscal on pedidoNotaFiscal.NFX_CODIGO = xmlNotaFiscal.NFX_CODIGO
                       left join T_PEDIDO pedido on pedido.PED_CODIGO = pedidoNotaFiscal.PED_CODIGO
                       left join T_CLIENTE remetente on remetente.CLI_CGCCPF = xmlNotaFiscal.CLI_CODIGO_REMETENTE
                       left join T_CLIENTE destinatario on destinatario.CLI_CGCCPF = xmlNotaFiscal.CLI_CODIGO_DESTINATARIO
                       left join T_LOCALIDADES localidadeOrigem on localidadeOrigem.LOC_CODIGO = remetente.LOC_CODIGO 
	                   left join T_LOCALIDADES localidadeDestino on localidadeDestino.LOC_CODIGO = destinatario.LOC_CODIGO 
                       left join T_FILIAL filial on filial.FIL_CODIGO = xmlNotaFiscal.FIL_CODIGO
                       left join T_CLIENTE expedidor on expedidor.CLI_CGCCPF = xmlNotaFiscal.CLI_CODIGO_EXPEDIDOR
                       left join T_CLIENTE recebedor on recebedor.CLI_CGCCPF = xmlNotaFiscal.CLI_CODIGO_RECEBEDOR   
                       left join T_GRUPO_PESSOAS grupoPessoasDestinatario on grupoPessoasDestinatario.GRP_CODIGO = destinatario.GRP_CODIGO 
                       left join T_GRUPO_PESSOAS grupoPessoasRecebedor on grupoPessoasRecebedor.GRP_CODIGO = recebedor.GRP_CODIGO 
                    where 
                        xmlNotaFiscal.NF_ATIVA = 1 and
                        pedidoNotaFiscal.PED_CODIGO in ( :codigos )"
            );

            if (!somenteContar)
            {
                sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
            }

            var query = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());
            query.SetParameterList("codigos", codigoPedidos);

            return query;
        }

        #endregion

        #region Métodos Públicos - Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEmbarque.RelacaoEmbarque> RelatorioRelacaoEmbarque(int codigoCarga)
        {
            string query = @"SELECT 
	                            NFX.NF_NUMERO NumeroNota,
	                            NFX.NF_SERIE Serie,
	                            C.CLI_NOME Remetente,
	                            D.CLI_NOME Destinatario,
	                            LD.LOC_DESCRICAO + ' - ' + LD.UF_SIGLA Destino,
	                            NFX.NF_PESO Peso,
	                            NFX.NF_VOLUMES Volumes,
	                            NFX.NF_VALOR_TOTAL_PRODUTOS ValorTotal,
	                            NFX.NF_CLASSIFICACAO_NFE ClassificacaoNF,
								(SELECT SUM(XFP_QUANTIDADE) FROM T_XML_NOTA_FISCAL_PRODUTO WHERE NFX_CODIGO = NFX.NFX_CODIGO) Pecas,
                                P.PED_NUMERO_PEDIDO_EMBARCADOR + ' Nº ' + CAST(P.PED_NUMERO AS VARCHAR(20)) NumeroPedido
                            FROM T_PEDIDO_XML_NOTA_FISCAL PN
	                            JOIN T_CARGA_PEDIDO CP ON CP.CPE_CODIGO = PN.CPE_CODIGO
	                            JOIN T_PEDIDO P ON P.PED_CODIGO = CP.PED_CODIGO
	                            JOIN T_XML_NOTA_FISCAL NFX ON NFX.NFX_CODIGO = PN.NFX_CODIGO
	                            JOIN T_CLIENTE C ON C.CLI_CGCCPF = NFX.CLI_CODIGO_REMETENTE
                                inner join T_CLIENTE as D on D.CLI_CGCCPF = NFX.CLI_CODIGO_DESTINATARIO
	                            LEFT JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = D.LOC_CODIGO
                            WHERE 1 = 1";
            query += " AND CP.CAR_CODIGO = " + codigoCarga.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEmbarque.RelacaoEmbarque)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEmbarque.RelacaoEmbarque>();
        }

        #endregion
    }
}
