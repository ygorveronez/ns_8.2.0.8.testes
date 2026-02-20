using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoXMLNotaFiscalComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>
    {
        public PedidoXMLNotaFiscalComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarTotalCargaPedidoPorCompomente(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido && obj.TipoComponenteFrete == tipoComponente && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public decimal BuscarTotalMoedaCargaPedidoPorComponente(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();

            query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido && obj.TipoComponenteFrete == tipoComponente && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && obj.ComponenteFilialEmissora == componenteFilialEmissora);

            if (componente != null)
                query = query.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return query.Select(obj => obj.ValorTotalMoeda).Sum() ?? 0;
        }

        public decimal BuscarTotalCargaPedidoPorCompomenteCargaCTe(int cargaPedido, int cargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.PedidoXMLNotaFiscal.CTes.Any(o => o.CargaCTe.Codigo == cargaCTe) select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete BuscarPorPedidoXMLNotaFiscalETipo(int codigoPedidoXMLNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool componenteFreteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();

            var result = from obj in query where obj.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal && obj.ComponenteFilialEmissora == componenteFreteFilialEmissora && obj.TipoComponenteFrete == tipoComponente select obj;

            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, bool compoenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();

            var result = from obj in query where obj.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == codigoCarga && obj.TipoComponenteFrete == tipoComponente && obj.ComponenteFilialEmissora == compoenteFilialEmissora select obj;

            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.CargaPedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();

            query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Fetch(o => o.PedidoXMLNotaFiscal).ThenFetch(o => o.CargaPedido)
                        .Fetch(o => o.ComponenteFrete).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarPorCargaSemFetch(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();

            query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public List<(decimal Valor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponente, int CodigoConponenteFrete)> BuscarValoresPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            var consultaComponenteFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>()
                .Where(componenteFrete => componenteFrete.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal);

            return consultaComponenteFrete
                .Select(componenteFrete => ValueTuple.Create(componenteFrete.ValorComponente, componenteFrete.TipoComponenteFrete, (componenteFrete.ComponenteFrete == null) ? 0 : componenteFrete.ComponenteFrete.Codigo))
                .ToList();
        }

        public decimal BuscarValorTotalDescontoPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            string sqlQuery = @"SELECT SUM(PedidoXMLNotaFiscalComponenteFrete.NFC_VALOR_COMPONENTE)  
                                  FROM T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE PedidoXMLNotaFiscalComponenteFrete 
                                  JOIN T_COMPONENTE_FRETE ComponenteFrete on ComponenteFrete.CFR_CODIGO = PedidoXMLNotaFiscalComponenteFrete.CFR_CODIGO 
                                 WHERE PedidoXMLNotaFiscalComponenteFrete.PNF_CODIGO = :PNF_CODIGO 
                                   AND ComponenteFrete.CFR_DESCONTAR_COMPONENTE_FRETE_LIQUIDO = 1";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("PNF_CODIGO", codigoPedidoXMLNotaFiscal);

            return query.UniqueResult<decimal>();

        }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();

            var result = from obj in query where obj.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal select obj;

            return result.ToList();
        }

        public decimal BuscarTotalCargaPedidoPorCompomentePorEmitenteDestinatario(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente, double remetente, double destinatario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal tipoOperacaoNFe, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido &&
                               obj.ComponenteFilialEmissora == componenteFilialEmissora &&
                               obj.TipoComponenteFrete == tipoComponente
                         select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            result = result.Where(obj =>
            obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ == remetente &&
            obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ == destinatario &&
            obj.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal == tipoOperacaoNFe);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarPorXMLnotaFiscalComPisCofins(int nfPedidoCodigo, Dominio.Entidades.ModeloDocumentoFiscal modelo, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.Codigo == nfPedidoCodigo && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            if (modelo != null)
                result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == modelo.Codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarPorXMLnotaFiscal(int nfPedidoCodigo, Dominio.Entidades.ModeloDocumentoFiscal modelo, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.Codigo == nfPedidoCodigo && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS select obj;
            if (modelo != null)
                result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Codigo == modelo.Codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarPorXMLNotaFiscalComImpostos(int nfPedidoCodigo, bool apenasModeloDocumentoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.Codigo == nfPedidoCodigo && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva select obj;
            if (apenasModeloDocumentoCTe)
                result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarPorCargaPedido(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && obj.TipoComponenteFrete == tipoComponenteFrete select obj;
            if (componenteFrete != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componenteFrete.Codigo);

            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>> BuscarPorCargaPedidoAsync(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && obj.TipoComponenteFrete == tipoComponenteFrete select obj;
            if (componenteFrete != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componenteFrete.Codigo);

            return await result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> BuscarTodosdaCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;

            return result.ToList();
        }

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery("DELETE PedidoXMLNotaFiscalComponenteFrete WHERE Codigo IN (SELECT c.Codigo FROM PedidoXMLNotaFiscalComponenteFrete c WHERE c.PedidoXMLNotaFiscal.CargaPedido.Codigo  = :codigoCargaPedido) ")
                                 .SetInt32("codigoCargaPedido", codigoCargaPedido)
                                 .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoXMLNotaFiscalComponenteFrete WHERE Codigo IN (SELECT c.Codigo FROM PedidoXMLNotaFiscalComponenteFrete c WHERE c.PedidoXMLNotaFiscal.CargaPedido.Codigo  = :codigoCargaPedido) ")
                            .SetInt32("codigoCargaPedido", codigoCargaPedido)
                            .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void DeletarPorCarga(int codigoCarga, bool componenteFilialEmissora)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery("DELETE PedidoXMLNotaFiscalComponenteFrete WHERE Codigo IN (SELECT c.Codigo FROM PedidoXMLNotaFiscalComponenteFrete c WHERE c.PedidoXMLNotaFiscal.CargaPedido.Codigo in (SELECT cp.Codigo FROM CargaPedido cp WHERE cp.Carga.Codigo = :codigoCarga)) AND ComponenteFilialEmissora = :componenteFilialEmissora")
                                 .SetInt32("codigoCarga", codigoCarga)
                                 .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                                 .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoXMLNotaFiscalComponenteFrete WHERE Codigo IN (SELECT c.Codigo FROM PedidoXMLNotaFiscalComponenteFrete c WHERE c.PedidoXMLNotaFiscal.CargaPedido.Codigo in (SELECT cp.Codigo FROM CargaPedido cp WHERE cp.Carga.Codigo = :codigoCarga)) AND ComponenteFilialEmissora = :componenteFilialEmissora")
                            .SetInt32("codigoCarga", codigoCarga)
                            .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                            .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarPorCargaPedidoAgrupado(int cargaPedido, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            var result = from obj in query where obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.ComponenteFilialEmissora == componenteFilialEmissora && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS && obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva group obj by new { obj.ComponenteFrete.Codigo, obj.TipoComponenteFrete, obj.Percentual, obj.IncluirBaseCalculoICMS } into grupo select new Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico { ComponenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete() { Codigo = grupo.Key.Codigo }, TipoComponenteFrete = grupo.Key.TipoComponenteFrete, Percentual = grupo.Key.Percentual, IncluirBaseCalculoImposto = grupo.Key.IncluirBaseCalculoICMS, ValorComponente = grupo.Sum(valor => valor.ValorComponente) };
            return result.ToList();
        }

        public void DeletarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal, bool componenteFilialEmissora)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery("DELETE PedidoXMLNotaFiscalComponenteFrete obj WHERE obj.PedidoXMLNotaFiscal.Codigo = :codigoPedidoXMLNotaFiscal and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                 .SetInt32("codigoPedidoXMLNotaFiscal", codigoPedidoXMLNotaFiscal)
                                 .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                                 .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoXMLNotaFiscalComponenteFrete obj WHERE obj.PedidoXMLNotaFiscal.Codigo = :codigoPedidoXMLNotaFiscal  and obj.ComponenteFilialEmissora = : componenteFilialEmissora")
                                .SetInt32("codigoPedidoXMLNotaFiscal", codigoPedidoXMLNotaFiscal)
                                .SetBoolean("componenteFilialEmissora", componenteFilialEmissora)
                                .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public decimal ObterValorComponentesPorPedidoXMLNotaFiscal(IEnumerable<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();

            query = query.Where(o => codigos.Contains(o.PedidoXMLNotaFiscal.Codigo) && o.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && o.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS && o.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);

            return query.Sum(o => (decimal?)o.ValorComponente) ?? 0m;
        }

        public void DeletarPorPedidoCTeParaSubContratacao(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery(@"DELETE FROM T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE
                                                    WHERE NFC_CODIGO IN (
                                                        SELECT nfc.NFC_CODIGO FROM T_PEDIDO_CTE_PARA_SUB_CONTRATACAO psc
	                                                    JOIN T_PEDIDO_CTE_PARA_SUBCONTRATACAO_PEDIDO_NOTA_FISCAL psn ON psn.PSC_CODIGO = psc.PSC_CODIGO 
                                                        JOIN T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE nfc ON nfc.PNF_CODIGO = psn.PNF_CODIGO
                                                        JOIN T_PEDIDO_XML_NOTA_FISCAL pnf on pnf.PNF_CODIGO = nfc.PNF_CODIGO
                                                        JOIN T_XML_NOTA_FISCAL nfx on nfx.NFX_CODIGO = pnf.NFX_CODIGO
	                                                    WHERE psc.CPE_CODIGO = :codigoCargaPedido
	                                                    AND nfx.NF_ATIVA = 1
                                                    )"
                                                   ).SetInt32("codigoCargaPedido", codigoCargaPedido).SetTimeout(6000).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery(@"DELETE FROM T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE
                                                        WHERE NFC_CODIGO IN (
                                                            SELECT nfc.NFC_CODIGO FROM T_PEDIDO_CTE_PARA_SUB_CONTRATACAO psc
	                                                        JOIN T_PEDIDO_CTE_PARA_SUBCONTRATACAO_PEDIDO_NOTA_FISCAL psn ON psn.PSC_CODIGO = psc.PSC_CODIGO 
                                                            JOIN T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE nfc ON nfc.PNF_CODIGO = psn.PNF_CODIGO
                                                            JOIN T_PEDIDO_XML_NOTA_FISCAL pnf on pnf.PNF_CODIGO = nfc.PNF_CODIGO
                                                            JOIN T_XML_NOTA_FISCAL nfx on nfx.NFX_CODIGO = pnf.NFX_CODIGO
	                                                        WHERE psc.CPE_CODIGO = :codigoCargaPedido
	                                                        AND nfx.NF_ATIVA = 1
                                                        )"
                                                       ).SetInt32("codigoCargaPedido", codigoCargaPedido).SetTimeout(6000).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }
    }
}
