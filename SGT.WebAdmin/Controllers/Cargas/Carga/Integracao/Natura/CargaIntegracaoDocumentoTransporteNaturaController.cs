using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao.Natura
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoDocumentoTransporteNaturaController : BaseController
    {
		#region Construtores

		public CargaIntegracaoDocumentoTransporteNaturaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long numeroDocumentoTransporte = Request.GetLongParam("Numero");

                int numeroNF = Request.GetIntParam("NumeroNF");
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCargaPedido = Request.GetIntParam("CargaPedido");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Recebedor", "Recebedor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDocumentoTransporte = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeTrabalho);
                Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDTNatura = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaIntegracaoNatura repCargaIntegracaoNatura = new Repositorio.Embarcador.Cargas.CargaIntegracaoNatura(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Integracao.DTNatura> documentosTransporte = null;
                int count = repCargaIntegracaoNatura.ContarConsultaPorCargaPedidoOuCarga(codigoCarga, codigoCargaPedido);

                if (count > 0)
                    documentosTransporte = repCargaIntegracaoNatura.ConsultarPorCargaPedidoOuCarga(codigoCarga, codigoCargaPedido, grid.inicio, grid.limite);
                else
                {
                    documentosTransporte = repDocumentoTransporte.Consultar(numeroDocumentoTransporte, numeroNF, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                    count = repDocumentoTransporte.ContarConsulta(numeroDocumentoTransporte, numeroNF);
                }

                grid.setarQuantidadeTotal(count);

                grid.AdicionaRows((from obj in documentosTransporte
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       Data = obj.Data.ToString("dd/MM/yyyy"),
                                       Recebedor = obj.Recebedor?.Descricao ?? string.Empty,
                                       NotasFiscais = string.Join(", ", repNotaFiscalDTNatura.BuscarNumerosPorDT(obj.Codigo)),
                                       obj.ValorFrete
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos de transporte da Natura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Desvincular()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCargaPedido = Request.GetIntParam("CargaPedido");

                Repositorio.Embarcador.Cargas.CargaIntegracaoNatura repCargaIntegracaoNatura = new Repositorio.Embarcador.Cargas.CargaIntegracaoNatura(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
                Servicos.Embarcador.Carga.DocumentoEmissao servicoDocumentoEmissao = new Servicos.Embarcador.Carga.DocumentoEmissao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> cargaIntegracoesNatura = repCargaIntegracaoNatura.BuscarPorCargaPedido(codigoCargaPedido);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura cargaIntegracaoNatura in cargaIntegracoesNatura)
                    repCargaIntegracaoNatura.Deletar(cargaIntegracaoNatura);

                servicoDocumentoEmissao.DeletarTodos(cargaPedido, Auditado);
                
                if (!repPedidoXMLNotaFiscal.VerificarSeExistePorCarga(carga.Codigo))
                {
                    carga.ValorFreteEmbarcador = 0;

                    repCarga.Atualizar(carga);
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos de transporte da Natura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Vincular()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCargaPedido = Request.GetIntParam("CargaPedido");

                List<int> codigosDocumentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Documentos"));

                Repositorio.Embarcador.Cargas.CargaIntegracaoNatura repCargaIntegracaoNatura = new Repositorio.Embarcador.Cargas.CargaIntegracaoNatura(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
                Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDTNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, "Não é possível vincular os documentos de transporte na etapa atual da carga.");

                if (cargaPedido.NotasFiscais.Any())
                    return new JsonpResult(false, true, "Já existem notas fiscais vinculadas à este pedido. Remova-as antes de vincular os documentos de transporte.");

                unidadeTrabalho.Start();

                carga.ValorFreteEmbarcador = 0;

                repCarga.Atualizar(carga);

                string erro;

                Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura svcNatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura(unidadeTrabalho);

                if (!svcNatura.VincularCargaAoDT(Usuario, codigosDocumentos, codigoCargaPedido, TipoServicoMultisoftware, unidadeTrabalho, out erro))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos de transporte da Natura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
