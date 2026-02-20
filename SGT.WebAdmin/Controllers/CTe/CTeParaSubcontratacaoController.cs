//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.Controllers.CTe
//{
//    [CustomAuthorize("Cargas/Carga")]
//    public class CTeParaSubcontratacaoController : BaseController
//    {

//        [AllowAuthenticate]
//        public async Task<IActionResult> Pesquisa()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                int pedido = int.Parse(Request.Params("Codigo"));
//                int codCarga = 0;

//                int.TryParse(Request.Params("Carga"), out codCarga);

//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                grid.header = new List<Models.Grid.Head>();
//                grid.AdicionarCabecalho("Codigo", false);
//                grid.AdicionarCabecalho("Número CT-e", "Numero", 8, Models.Grid.Align.center, true);
//                grid.AdicionarCabecalho("Subcontratente", "TransportadorTerceiro", 20, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Remetente", "Remetente", 20, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Destinatario", "Destinatario", 20, Models.Grid.Align.left, true);

//                grid.AdicionarCabecalho("Valor Carga", "ValorTotalMercadoria", 10, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Valor a Receber", "ValorAReceber", 10, Models.Grid.Align.left, true);

//                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

//                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

//                bool ativas = true;
//                if (codCarga > 0)
//                {
//                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
//                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
//                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
//                        ativas = false;
//                }

//                if (propOrdenar == "Remetente" || propOrdenar == "Destinatario" || propOrdenar == "TransportadorTerceiro")
//                    propOrdenar += ".Nome";

//                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> CTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCTesPorPedido(pedido, ativas, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
//                grid.setarQuantidadeTotal(repPedidoCTeParaSubContratacao.ContarCTesPorPedido(pedido, ativas));
//                var dynCtesparaSubContratacao = (from obj in CTesParaSubContratacao
//                                                select new
//                                                {
//                                                    obj.Codigo,
//                                                    obj.Numero,
//                                                    TransportadorTerceiro = obj.TransportadorTerceiro.Nome + "(" + obj.TransportadorTerceiro.Localidade.DescricaoCidadeEstado + ")",
//                                                    Remetente = obj.Remetente.Nome + "(" + obj.Remetente.Localidade.DescricaoCidadeEstado + ")",
//                                                    Destinatario = obj.Destinatario != null ? obj.Destinatario.Nome + "(" + obj.Destinatario.Localidade.DescricaoCidadeEstado + ")" : "",
//                                                    ValorAReceber = obj.ValorAReceber.ToString("n2"),
//                                                    ValorTotalMercadoria = obj.ValorTotalMercadoria.ToString("n2")
//                                                }).ToList();

//                grid.AdicionaRows(dynCtesparaSubContratacao);
//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        public async Task<IActionResult> Excluir()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                unitOfWork.Start();

//                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(_conexao.StringConexao);

//                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
//                int codCTeSubContratacao = int.Parse(Request.Params("Codigo"));

//                Repositorio.Embarcador.CTe.CTeTerceiro repCTeParaSubContratacao = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
//                Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal repCTeParaSubContratacaoNotaFiscal = new Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal(unitOfWork);
//                Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeParaSubContratacaoOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
//                Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeParaSubContratacaoQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
//                Repositorio.Embarcador.CTe.CTeTerceiroSeguro repCTeParaSubContratacaoSeguro = new Repositorio.Embarcador.CTe.CTeTerceiroSeguro(unitOfWork);
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

//                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
//                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);


//                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
//                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codCargaPedido);

//                if (cargaPedido != null)
//                {
//                    if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
//                    {
//                        Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao = repCTeParaSubContratacao.BuscarPorCodigo(codCTeSubContratacao);
//                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCTeSubContratacaoEPedido(cteParaSubContratacao.Codigo, cargaPedido.Pedido.Codigo);

//                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal> cteParaSubContratacaoNotasFiscais = repCTeParaSubContratacaoNotaFiscal.BuscarPorCTeParaSubContratacao(cteParaSubContratacao.Codigo);
//                        foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal item in cteParaSubContratacaoNotasFiscais)
//                            repCTeParaSubContratacaoNotaFiscal.Deletar(item);

//                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> cteParaSubContratacaoOutrosDocumentos = repCTeParaSubContratacaoOutrosDocumentos.BuscarPorCTeParaSubContratacao(cteParaSubContratacao.Codigo);
//                        foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos item in cteParaSubContratacaoOutrosDocumentos)
//                            repCTeParaSubContratacaoOutrosDocumentos.Deletar(item);

//                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> cteParaSubContratacaoQuantidades = repCTeParaSubContratacaoQuantidade.BuscarPorCTeParaSubContratacao(cteParaSubContratacao.Codigo);
//                        foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade item in cteParaSubContratacaoQuantidades)
//                            repCTeParaSubContratacaoQuantidade.Deletar(item);

//                        List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> cteParaSubContratacaoSeguros = repCTeParaSubContratacaoSeguro.BuscarPorCTeParaSubContratacao(cteParaSubContratacao.Codigo);
//                        foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro item in cteParaSubContratacaoSeguros)
//                            repCTeParaSubContratacaoSeguro.Deletar(item);

//                        cteParaSubContratacao.ChavesNFe.Clear();

//                        repPedidoCTeParaSubContratacao.Deletar(pedidoCTeParaSubContratacao);
//                        repCTeParaSubContratacao.Deletar(cteParaSubContratacao);

//                        if (cargaPedido.Carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
//                        {
//                            int numeroCTes = repPedidoCTeParaSubContratacao.ContarPorPedido(cargaPedido.Pedido.Codigo);
//                            if (numeroCTes == 0)
//                            {
//                                cargaPedido.Carga.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
//                                repCarga.Atualizar(cargaPedido.Carga);
//                            }
//                        }
//                        unitOfWork.CommitChanges();
//                        return new JsonpResult(cargaPedido.Carga.TipoContratacaoCarga);
//                    }
//                    else
//                    {
//                        unitOfWork.Rollback();
//                        return new JsonpResult(false, true, "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite a exclusão dos CT-es para Subcontratação");
//                    }
//                }
//                else
//                {
//                    unitOfWork.Rollback();
//                    return new JsonpResult(false, true, "Pedido não encontrado");
//                }

//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();
//                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
//                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
//                else
//                {
//                    Servicos.Log.TratarErro(ex);
//                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
//                }
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//    }
//}
