using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class DocumentoCTeController : BaseController
    {
		#region Construtores

		public DocumentoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();


                int cargaPedido = Request.GetIntParam("CodigoCargaPedido");
                int codCarga = Request.GetIntParam("Carga");

                bool ativas = true;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                if (codCarga > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    carga = repCarga.BuscarPorCodigo(codCarga);
                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                        ativas = false;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                if (carga?.TipoServicoCarga == TipoServicoCarga.Feeder && (integracaoIntercab?.ModificarTimelineDeAcordoComTipoServicoDocumento ?? false))
                {
                    grid.AdicionarCabecalho("Nº da Declaração", "Numero", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("CE Mercante", "ChaveAcesso", 10, Models.Grid.Align.left, false);
                }
                else
                {
                    grid.AdicionarCabecalho("Número CT-e", "Numero", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Chave", "ChaveAcesso", 10, Models.Grid.Align.left, false);
                }

                grid.AdicionarCabecalho("Subcontratante", "TransportadorTerceiro", 16, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 17, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatario", "Destinatario", 17, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Valor Carga", "ValorTotalMercadoria", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorAReceber", 10, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

                if (propOrdenar == "Remetente" || propOrdenar == "Destinatario" || propOrdenar == "TransportadorTerceiro")
                    propOrdenar += ".Nome";

                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> CTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCTesPorCargaPedido(cargaPedido, ativas, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                if (CTesParaSubContratacao.Count == 0)
                    CTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCTesPorCargaPedidoPacote(cargaPedido, ativas, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                int quantidadeTotal = repPedidoCTeParaSubContratacao.ContarCTesPorCargaPedido(cargaPedido, ativas) > 0 ? repPedidoCTeParaSubContratacao.ContarCTesPorCargaPedido(cargaPedido, ativas) : repPedidoCTeParaSubContratacao.ContarPorCargaPedidoPacoteComCodigoCargaPedido(cargaPedido);
                grid.setarQuantidadeTotal(quantidadeTotal);
                var dynCtesparaSubContratacao = (from obj in CTesParaSubContratacao
                                                 select new
                                                 {
                                                     obj.Codigo,
                                                     obj.Numero,
                                                     obj.ChaveAcesso,
                                                     TransportadorTerceiro = obj.TransportadorTerceiro.Nome + "(" + obj.TransportadorTerceiro.Localidade.DescricaoCidadeEstado + ")",
                                                     Remetente = obj.Remetente != null ? obj.Remetente.Nome + "(" + obj.Remetente.Localidade.DescricaoCidadeEstado + ")" : "",
                                                     Destinatario = obj.Destinatario != null ? obj.Destinatario.Nome + "(" + obj.Destinatario.Localidade.DescricaoCidadeEstado + ")" : "",
                                                     ValorAReceber = obj.ValorAReceber.ToString("n2"),
                                                     ValorTotalMercadoria = obj.ValorTotalMercadoria.ToString("n2")
                                                 }).ToList();

                grid.AdicionaRows(dynCtesparaSubContratacao);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);
            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);


            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codCargaPedido);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga tipoContratacao = cargaPedido.TipoContratacaoCarga;

                if (cargaPedido != null)
                {
                    dynamic dynCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTe"));
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterDynamicParaCTe(dynCTe, unitOfWork);
                    if (cteIntegracao.Chave.Length == 44 && serDocumento.ValidarChave(cteIntegracao.Chave))
                    {
                        string retorno = "";
                        if (cteIntegracao.Codigo > 0)
                            serCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref retorno, null, cteIntegracao);
                        else
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                            retorno = serCTeSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cteIntegracao, cargaPedido, TipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);
                        }


                        if (string.IsNullOrEmpty(retorno))
                        {
                            if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                            {
                                if (!serCarga.AtualizarTipoServicoCargaMultimodal(cteIntegracao, cargaPedido, out retorno, unitOfWork))
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, retorno);
                                }
                            }
                            else if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                            {
                                if (cargaPedido.Expedidor != null && cargaPedido.Recebedor != null)
                                {
                                    if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.EmitirSempreComoRedespacho)
                                        cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                                    else
                                        cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                                }
                                else if (cargaPedido.Expedidor != null || cargaPedido.Recebedor != null)
                                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                                else
                                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao)
                                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                                serCarga.SetarTipoContratacaoCarga(cargaPedido.Carga, unitOfWork);
                            }

                            if (cteIntegracao.Codigo > 0)
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Atualizou um Documento CT-e.", unitOfWork);
                            else
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Adicionou um Documento CT-e.", unitOfWork);
                            unitOfWork.CommitChanges();
                            if (tipoContratacao != cargaPedido.TipoContratacaoCarga)
                                return new JsonpResult(serCarga.ObterDetalhesDaCarga(cargaPedido.Carga, TipoServicoMultisoftware, unitOfWork));
                            else
                                return new JsonpResult(true);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, retorno);
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A chave informada é inválida, por favor, verifique e tente novamente.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Pedido não encontrado");
                }
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o CT-e Manualmente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirTodosCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));

                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codCargaPedido);

                if (cargaPedido != null)
                {
                    if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                        return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                    if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedido(cargaPedido.Codigo);
                        Servicos.Embarcador.CTe.DocumentoCTe servicoDocumentoCTe = new Servicos.Embarcador.CTe.DocumentoCTe(unitOfWork);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidosCTeParaSubContratacao)
                        {
                            unitOfWork.Start();

                            servicoDocumentoCTe.ExcluirCTesSubContratacao(pedidoCTeParaSubContratacao, cargaPedido, unitOfWork);

                            unitOfWork.CommitChanges();
                        }

                        if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                        {
                            cargaPedido.Carga.TipoServicoCarga = TipoServicoCarga.NaoInformado;
                            repCarga.Atualizar(cargaPedido.Carga);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Excluiu todos os CT-es para subcontratação.", unitOfWork);


                        return new JsonpResult(cargaPedido.TipoContratacaoCarga);
                    }
                    else
                    {
                        return new JsonpResult(false, true, "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite a exclusão dos CT-es para Subcontratação");
                    }
                }
                else
                {
                    return new JsonpResult(false, true, "Pedido não encontrado");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
                int codCTeSubContratacao = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.CTe.CTeTerceiro repCTeParaSubContratacao = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codCargaPedido);

                if (cargaPedido != null)
                {
                    if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                        return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                    if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    {
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao = repCTeParaSubContratacao.BuscarPorCodigo(codCTeSubContratacao);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCTeSubContratacaoECargaPedido(cteParaSubContratacao.Codigo, cargaPedido.Codigo);
                        Servicos.Embarcador.CTe.DocumentoCTe servicoDocumentoCTe = new Servicos.Embarcador.CTe.DocumentoCTe(unitOfWork);

                        unitOfWork.Start();

                        servicoDocumentoCTe.ExcluirCTesSubContratacao(pedidoCTeParaSubContratacao, cargaPedido, unitOfWork);

                        if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada || (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false))
                        {
                            int numeroCTes = repPedidoCTeParaSubContratacao.ContarPorCargaPedido(cargaPedido.Codigo);
                            if (numeroCTes == 0)
                            {
                                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
                                repCargaPedido.Atualizar(cargaPedido);

                                if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                                {
                                    cargaPedido.Carga.TipoServicoCarga = TipoServicoCarga.NaoInformado;
                                    repCarga.Atualizar(cargaPedido.Carga);
                                }
                            }
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Excluiu o CT-e para subcontratação " + cteParaSubContratacao.Descricao + ".", unitOfWork);

                        unitOfWork.CommitChanges();

                        return new JsonpResult(cargaPedido.TipoContratacaoCarga);
                    }
                    else
                    {
                        return new JsonpResult(false, true, "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite a exclusão dos CT-es para Subcontratação");
                    }
                }
                else
                {
                    return new JsonpResult(false, true, "Pedido não encontrado");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCTeTerceiroPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoCTeTerceiro);

                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repCTeTerceiro.BuscarPorCodigo(codigoCTeTerceiro);

                if (cteTerceiro == null)
                    return new JsonpResult(false, "CT-e Terceiro não encontrado.");

                var retorno = new
                {
                    CTe = ObterDetalhesCTe(cteTerceiro),
                    Remetente = ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Remetente, cteTerceiro),
                    Destinatario = ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Destinatario, cteTerceiro),
                    Expedidor = ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Expedidor, cteTerceiro),
                    Recebedor = ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Recebedor, cteTerceiro),
                    Tomador = ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Outros, cteTerceiro),
                    Documentos = ObterDocumentosCTe(cteTerceiro),
                    Rodoviario = ObterModalRodoviarioCTe(cteTerceiro),
                    InformacaoCarga = ObterInformacaoCargaCTe(cteTerceiro),
                    QuantidadesCarga = ObterQuantidadesCargaCTe(cteTerceiro, unitOfWork),
                    Seguros = ObterSegurosCTe(cteTerceiro),
                    TotalServico = ObterTotaisServicoCTe(cteTerceiro),
                    ModalAereo = ObterModalAereoCTe(cteTerceiro)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados do CT-e Terceiro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTeTerceiro = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repCTeTerceiro.BuscarPorCodigo(codigoCTeTerceiro);

                if (cteTerceiro == null)
                    return new JsonpResult(false, "CT-e Terceiro não encontrado.");

                var retorno = new
                {
                    cteTerceiro.Codigo,
                    MetrosCubicos = cteTerceiro.MetrosCubicos.ToString("n6"),
                    Volumes = cteTerceiro.Volumes.ToString(),
                    Dimensoes = (from obj in cteTerceiro.CTeTerceiroDimensoes
                                 select new
                                 {
                                     obj.Codigo,
                                     Altura = new { val = obj.Altura, tipo = "decimal", configDecimal = new { precision = 3 } },
                                     Comprimento = new { val = obj.Comprimento, tipo = "decimal", configDecimal = new { precision = 3 } },
                                     Largura = new { val = obj.Largura, tipo = "decimal", configDecimal = new { precision = 3 } },
                                     MetrosCubicos = new { val = obj.MetrosCubicos, tipo = "decimal", configDecimal = new { precision = 6 } },
                                     Volumes = obj.Volumes.ToString()
                                 }).ToList(),
                    QuantidadePaletes = cteTerceiro.QuantidadePaletes
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das dimensões do CT-e Terceiro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoCargaPedido = Request.GetIntParam("CargaPedido");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é possível atualizar CT-e Terceiro em cargas de transbordo.");

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite a alteração de CT-e Terceiro");

                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repCTeTerceiro.BuscarPorCodigo(codigo, true);

                if (cteTerceiro == null)
                    return new JsonpResult(false, true, "CT-e Terceiro não encontrado.");

                unitOfWork.Start();

                cteTerceiro.Volumes = Request.GetIntParam("Volumes");
                cteTerceiro.MetrosCubicos = Request.GetDecimalParam("MetrosCubicos");

                if (cargaPedido.Carga.TipoOperacao != null)
                {
                    if (cteTerceiro.MetrosCubicos > 0m && cargaPedido.Carga.TipoOperacao.UtilizarFatorCubagem && cargaPedido.Carga.TipoOperacao.FatorCubagem.HasValue)
                    {
                        cteTerceiro.PesoCubado = cteTerceiro.MetrosCubicos * cargaPedido.Carga.TipoOperacao.FatorCubagem.Value;
                        cteTerceiro.FatorCubagem = cargaPedido.Carga.TipoOperacao.FatorCubagem.Value;

                        if (cteTerceiro.PesoCubado > cteTerceiro.Peso)
                            cteTerceiro.PesoBaseParaCalculo = cteTerceiro.PesoCubado;
                        else
                            cteTerceiro.PesoBaseParaCalculo = cteTerceiro.Peso;
                    }
                    else
                        cteTerceiro.PesoCubado = 0m;
                }
                else
                    cteTerceiro.PesoCubado = 0m;

                SalvarDimensoes(cteTerceiro, unitOfWork);

                repCTeTerceiro.Atualizar(cteTerceiro, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o CT-e Terceiro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarIndicarPaletes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoCargaPedido = Request.GetIntParam("CargaPedido");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é possível atualizar CT-e Terceiro em cargas de transbordo.");

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite a alteração de CT-e Terceiro");

                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repCTeTerceiro.BuscarPorCodigo(codigo, true);

                if (cteTerceiro == null)
                    return new JsonpResult(false, true, "CT-e Terceiro não encontrado.");

                unitOfWork.Start();

                cteTerceiro.QuantidadePaletes = Request.GetIntParam("QuantidadePaletes");
                
                repCTeTerceiro.Atualizar(cteTerceiro, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o CT-e Terceiro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();

            return new JsonpResult(configuracoes.ToList());
        }

        
        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento").Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCargaPedido = Request.GetIntParam("CargaPedido");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional repCTeTerceiroDocumentoAdicional = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, $"A situação da carga ({cargaPedido.Carga.SituacaoCarga.ObterDescricao()} não permite que sejam adicionados novos documentos.");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacao();
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(Request.Params("Dados"));

                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Cliente serCliente = new Servicos.Cliente();
                Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

                int contador = 0;

                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe = null;

                List<Dominio.Entidades.Cliente> emitentes = new List<Dominio.Entidades.Cliente>();

                unitOfWork.Start();

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveCTe = linha.Colunas.Where(obj => obj.NomeCampo == "ChaveCTe").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroViagemCTe = linha.Colunas.Where(obj => obj.NomeCampo == "NumeroViagemCTe").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedidoCTe = linha.Colunas.Where(obj => obj.NomeCampo == "NumeroPedidoCTe").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroRomaneioCTe = linha.Colunas.Where(obj => obj.NomeCampo == "NumeroRomaneioCTe").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colProdutoPredominanteCTe = linha.Colunas.Where(obj => obj.NomeCampo == "ProdutoPredominanteCTe").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoPagamentoCTe = linha.Colunas.Where(obj => obj.NomeCampo == "TipoPagamentoCTe").FirstOrDefault();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroNotaFiscal = linha.Colunas.Where(obj => obj.NomeCampo == "NumeroNotaFiscal").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSerieNotaFiscal = linha.Colunas.Where(obj => obj.NomeCampo == "SerieNotaFiscal").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataEmissaoNotaFiscal = linha.Colunas.Where(obj => obj.NomeCampo == "DataEmissaoNotaFiscal").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveNotaFiscal = linha.Colunas.Where(obj => obj.NomeCampo == "ChaveNotaFiscal").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorNotaFiscal = linha.Colunas.Where(obj => obj.NomeCampo == "ValorNotaFiscal").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoNotaFiscal = linha.Colunas.Where(obj => obj.NomeCampo == "PesoNotaFiscal").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVolumesNotaFiscal = linha.Colunas.Where(obj => obj.NomeCampo == "VolumesNotaFiscal").FirstOrDefault();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFCNPJExpedidor = linha.Colunas.Where(obj => obj.NomeCampo == "CPFCNPJExpedidor").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFCNPJRecebedor = linha.Colunas.Where(obj => obj.NomeCampo == "CPFCNPJRecebedor").FirstOrDefault();

                        string retorno = null;

                        if (pedidoCTeParaSubContratacao != null)
                        {
                            if (cteTerceiroNFe == null)
                                cteTerceiroNFe = repCTeTerceiroNFe.BuscarPrimeiroPorCTeTerceiro(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo);

                            string chaveCTe = Utilidades.String.OnlyNumbers((string)colChaveCTe.Valor);

                            if (!Utilidades.Validate.ValidarChave(chaveCTe))
                            {
                                RetornarFalhaLinha(retornoImportacao, i, "A chave do CT-e é inválida.");
                                continue;
                            }

                            string strCnpjEmitente = chaveCTe.Substring(6, 14);
                            double cnpjEmitente = strCnpjEmitente.ToDouble();

                            Dominio.Entidades.Cliente clienteEmitente = emitentes.Where(o => o.CPF_CNPJ == cnpjEmitente).FirstOrDefault();

                            if (clienteEmitente == null)
                            {
                                clienteEmitente = repCliente.BuscarPorCPFCNPJ(cnpjEmitente);

                                if (clienteEmitente != null)
                                    emitentes.Add(clienteEmitente);
                            }

                            if (clienteEmitente == null)
                            {
                                RetornarFalhaLinha(retornoImportacao, i, $"Não encontramos um cliente com o CNPJ {strCnpjEmitente} registrado na base de dados.");
                                continue;
                            }

                            if (cteTerceiroNFe != null)
                            {
                                cteTerceiroNFe.Volumes += SanitizarDadoImportacao((string)colVolumesNotaFiscal?.Valor).ToDecimal();
                                cteTerceiroNFe.Peso += SanitizarDadoImportacao((string)colPesoNotaFiscal?.Valor).ToDecimal();

                                repCTeTerceiroNFe.Atualizar(cteTerceiroNFe);
                            }

                            Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional documentoAdicional = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional()
                            {
                                Chave = chaveCTe,
                                CTeTerceiro = pedidoCTeParaSubContratacao.CTeTerceiro,
                                Emitente = clienteEmitente,
                                Numero = chaveCTe.Substring(23, 9).ToInt()
                            };

                            repCTeTerceiroDocumentoAdicional.Inserir(documentoAdicional);

                            pedidoCTeParaSubContratacao.CTeTerceiro.Peso += SanitizarDadoImportacao((string)colPesoNotaFiscal?.Valor).ToDecimal();
                            pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria += SanitizarDadoImportacao((string)colValorNotaFiscal?.Valor).ToDecimal();

                            repCTeTerceiro.Atualizar(pedidoCTeParaSubContratacao.CTeTerceiro);

                            RetornarSucessoLinha(retornoImportacao, i);

                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();

                        if (colChaveCTe != null)
                            cteIntegracao.Chave = Utilidades.String.OnlyNumbers((string)colChaveCTe.Valor);

                        if (!Utilidades.Validate.ValidarChave(cteIntegracao.Chave))
                        {
                            RetornarFalhaLinha(retornoImportacao, i, "A chave do CT-e é inválida.");
                            continue;
                        }

                        cteIntegracao.TipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;
                        cteIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                        cteIntegracao.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;
                        cteIntegracao.TipoCTE = Dominio.Enumeradores.TipoCTE.Normal;
                        cteIntegracao.TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato;
                        cteIntegracao.TipoModal = TipoModal.Rodoviario;
                        cteIntegracao.SituacaoCTeSefaz = SituacaoCTeSefaz.Autorizada;

                        cteIntegracao.NumeroPedido = SanitizarDadoImportacao((string)colNumeroPedidoCTe?.Valor);
                        cteIntegracao.NumeroViagem = SanitizarDadoImportacao((string)colNumeroViagemCTe?.Valor);
                        cteIntegracao.NumeroRomaneio = SanitizarDadoImportacao((string)colNumeroRomaneioCTe?.Valor);
                        cteIntegracao.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga() { ProdutoPredominante = SanitizarDadoImportacao((string)colProdutoPredominanteCTe?.Valor) };

                        Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal();

                        cteIntegracao.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>() {
                            new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                            {
                                 DataEmissao = SanitizarDadoImportacao((string)colDataEmissaoNotaFiscal?.Valor).ToDateTime("ddMMyyyy"),
                                 Valor = SanitizarDadoImportacao((string)colValorNotaFiscal?.Valor).ToDecimal(),
                                 Numero = SanitizarDadoImportacao((string)colNumeroNotaFiscal?.Valor).ToInt(),
                                 Volumes = SanitizarDadoImportacao((string)colVolumesNotaFiscal?.Valor).ToDecimal(),
                                 Peso = SanitizarDadoImportacao((string)colPesoNotaFiscal?.Valor).ToDecimal(),
                                 Chave = SanitizarDadoImportacao(colChaveNotaFiscal?.Valor),
                                 NumeroReferenciaEDI = cteIntegracao.NumeroViagem,
                                 NumeroPedido = cteIntegracao.NumeroPedido,
                                 NumeroRomaneio = cteIntegracao.NumeroRomaneio
                            }
                        };

                        if (SanitizarDadoImportacao(colTipoPagamentoCTe?.Valor) == "FOB")
                            cteIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                        else
                            cteIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;

                        Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                        if (!ObterClienteImportacao(linha, "Remetente", out remetente, out retorno, unitOfWork))
                        {
                            RetornarFalhaLinha(retornoImportacao, i, retorno);
                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                        if (!ObterClienteImportacao(linha, "Destinatario", out destinatario, out retorno, unitOfWork))
                        {
                            RetornarFalhaLinha(retornoImportacao, i, retorno);
                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                        if (linha.Colunas.Any(obj => obj.NomeCampo == $"CPFCNPJTransportadorTerceiro"))
                        {
                            if (!ObterEmpresaImportacao(linha, "TransportadorTerceiro", out emitente, out retorno, unitOfWork))
                            {
                                RetornarFalhaLinha(retornoImportacao, i, retorno);
                                continue;
                            }
                        }
                        else
                        {
                            string strCnpjEmitente = cteIntegracao.Chave.Substring(6, 14);
                            double cnpjEmitente = strCnpjEmitente.ToDouble();

                            Dominio.Entidades.Cliente clienteEmitente = emitentes.Where(o => o.CPF_CNPJ == cnpjEmitente).FirstOrDefault();

                            if (clienteEmitente == null)
                            {
                                clienteEmitente = repCliente.BuscarPorCPFCNPJ(cnpjEmitente);

                                if (clienteEmitente != null)
                                    emitentes.Add(clienteEmitente);
                            }

                            if (clienteEmitente == null)
                            {
                                RetornarFalhaLinha(retornoImportacao, i, $"Não encontramos um cliente com o CNPJ {strCnpjEmitente} registrado na base de dados.");
                                continue;
                            }

                            emitentes.Add(clienteEmitente);

                            emitente = Servicos.Empresa.ObterEmpresa(clienteEmitente);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa expedidor = null;
                        if (linha.Colunas.Any(obj => obj.NomeCampo == $"CPFCNPJExpedidor"))
                        {
                            string strCnpjExpedidor = cteIntegracao.Chave.Substring(6, 14);
                            double cpfCnpjExpedidor = strCnpjExpedidor.ToDouble();

                            Dominio.Entidades.Cliente clienteExpedidor = repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor);

                            if (clienteExpedidor == null)
                            {
                                RetornarFalhaLinha(retornoImportacao, i, $"Não encontramos um cliente com o CNPJ {strCnpjExpedidor} registrado na base de dados.");
                                continue;
                            }

                            expedidor = Servicos.Embarcador.Pessoa.Pessoa.Converter(clienteExpedidor);
                        }

                        Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa recebedor = null;
                        if (linha.Colunas.Any(obj => obj.NomeCampo == $"CPFCNPJRecebedor"))
                        {
                            string strCnpjRecebedor = cteIntegracao.Chave.Substring(6, 14);
                            double cpfCnpjRecebedor = strCnpjRecebedor.ToDouble();

                            Dominio.Entidades.Cliente clienteRecebedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor);

                            if (clienteRecebedor == null)
                            {
                                RetornarFalhaLinha(retornoImportacao, i, $"Não encontramos um cliente com o CNPJ {strCnpjRecebedor} registrado na base de dados.");
                                continue;
                            }

                            recebedor = Servicos.Embarcador.Pessoa.Pessoa.Converter(clienteRecebedor);
                        }

                        cteIntegracao.Remetente = remetente;
                        cteIntegracao.Destinatario = destinatario;
                        cteIntegracao.Emitente = emitente;
                        cteIntegracao.Expedidor = expedidor;
                        cteIntegracao.Recebedor = recebedor;

                        cteIntegracao.ModalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario()
                        {
                            Lotacao = false
                        };

                        cteIntegracao.LocalidadeInicioPrestacao = new Dominio.ObjetosDeValor.Localidade()
                        {
                            IBGE = cteIntegracao.Expedidor?.Endereco?.Cidade?.IBGE ?? cteIntegracao.Remetente?.Endereco?.Cidade?.IBGE ?? 0
                        };

                        cteIntegracao.LocalidadeFimPrestacao = new Dominio.ObjetosDeValor.Localidade()
                        {
                            IBGE = cteIntegracao.Recebedor?.Endereco?.Cidade?.IBGE ?? cteIntegracao.Destinatario?.Endereco?.Cidade?.IBGE ?? 0
                        };

                        retorno = serCTeSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cteIntegracao, cargaPedido, TipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            RetornarFalhaLinha(retornoImportacao, i, retorno);
                            continue;
                        }

                        pedidoCTeParaSubContratacao.CTeTerceiro.Peso += cteIntegracao.NFEs?.Sum(o => o.Peso) ?? 0m;
                        pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria += cteIntegracao.NFEs?.Sum(o => o.Valor) ?? 0m;

                        repCTeTerceiro.Atualizar(pedidoCTeParaSubContratacao.CTeTerceiro);

                        contador++;

                        RetornarSucessoLinha(retornoImportacao, i);
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro(ex2);
                        RetornarFalhaLinha(retornoImportacao, i, "Ocorreu uma falha ao processar a linha.");
                    }
                }

                if (retornoImportacao.Retornolinhas.Any(o => !o.processou))
                {
                    unitOfWork.Rollback();
                }
                else
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidadePeso = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade()
                    {
                        CTeTerceiro = pedidoCTeParaSubContratacao.CTeTerceiro,
                        Quantidade = pedidoCTeParaSubContratacao.CTeTerceiro.Peso,
                        TipoMedida = "Peso Bruto",
                        Unidade = Dominio.Enumeradores.UnidadeMedida.KG
                    };

                    repCTeTerceiroQuantidade.Inserir(cteTerceiroQuantidadePeso);

                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidadeVolumes = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade()
                    {
                        CTeTerceiro = pedidoCTeParaSubContratacao.CTeTerceiro,
                        Quantidade = pedidoCTeParaSubContratacao.CTeTerceiro.Volumes,
                        TipoMedida = "Volumes",
                        Unidade = Dominio.Enumeradores.UnidadeMedida.UN
                    };

                    repCTeTerceiroQuantidade.Inserir(cteTerceiroQuantidadeVolumes);

                    if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                    {
                        if (cargaPedido.Expedidor != null && cargaPedido.Recebedor != null)
                        {
                            if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.EmitirSempreComoRedespacho)
                                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                            else
                                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                        }
                        else if (cargaPedido.Expedidor != null || cargaPedido.Recebedor != null)
                            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                        else
                            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                        if ((ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao) || (cargaPedido.Carga.TipoOperacao?.SempreEmitirSubcontratacao ?? false))
                            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                        serCarga.SetarTipoContratacaoCarga(cargaPedido.Carga, unitOfWork);
                    }

                    unitOfWork.CommitChanges();
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count;
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CT-e - Chave de Acesso", Propriedade = "ChaveCTe", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "CT-e - Nº da Viagem", Propriedade = "NumeroViagemCTe", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CT-e - Nº do Pedido", Propriedade = "NumeroPedidoCTe", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "CT-e - Nº do Romaneio", Propriedade = "NumeroRomaneioCTe", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "CT-e - Produto Predominante", Propriedade = "ProdutoPredominanteCTe", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "CT-e - Tipo de Pagamento (CIF|FOB)", Propriedade = "TipoPagamentoCTe", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Nota Fiscal - Número", Propriedade = "NumeroNotaFiscal", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Nota Fiscal - Série", Propriedade = "SerieNotaFiscal", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Nota Fiscal - Data de Emissão", Propriedade = "DataEmissaoNotaFiscal", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Nota Fiscal - Chave de Acesso", Propriedade = "ChaveNotaFiscal", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Nota Fiscal - Valor", Propriedade = "ValorNotaFiscal", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Nota Fiscal - Peso", Propriedade = "PesoNotaFiscal", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Nota Fiscal - Unidade Medida", Propriedade = "UnidadeMedidaNotaFiscal", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Nota Fiscal - Volumes", Propriedade = "VolumesNotaFiscal", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "Remetente - CPF/CNPJ", Propriedade = "CPFCNPJRemetente", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = "Remetente - IE", Propriedade = "IERemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = "Remetente - Razão Social", Propriedade = "RazaoSocialRemetente", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = "Remetente - Nome Fantasia", Propriedade = "NomeFantasiaRemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = "Remetente - Endereço", Propriedade = "EnderecoRemetente", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = "Remetente - Bairro", Propriedade = "BairroRemetente", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = "Remetente - Cidade", Propriedade = "CidadeRemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = "Remetente - Estado (UF)", Propriedade = "EstadoRemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = "Remetente - IBGE", Propriedade = "IBGERemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = "Remetente - CEP", Propriedade = "CEPRemetente", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = "Remetente - Tipo Pessoa (Física = 1 | Jurídica = 2)", Propriedade = "TipoPessoaRemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = "Destinatário - CPF/CNPJ", Propriedade = "CPFCNPJDestinatario", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 27, Descricao = "Destinatário - IE", Propriedade = "IEDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 28, Descricao = "Destinatário - Razão Social", Propriedade = "RazaoSocialDestinatario", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 29, Descricao = "Destinatário - Nome Fantasia", Propriedade = "NomeFantasiaDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 30, Descricao = "Destinatário - Endereço", Propriedade = "EnderecoDestinatario", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 31, Descricao = "Destinatário - Bairro", Propriedade = "BairroDestinatario", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 32, Descricao = "Destinatário - Cidade", Propriedade = "CidadeDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 33, Descricao = "Destinatário - Estado (UF)", Propriedade = "EstadoDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 34, Descricao = "Destinatário - IBGE", Propriedade = "IBGEDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 35, Descricao = "Destinatário - CEP", Propriedade = "CEPDestinatario", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 36, Descricao = "Destinatário - Tipo Pessoa (Física = 1 | Jurídica = 2)", Propriedade = "TipoPessoaDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 37, Descricao = "Transportador Terceiro - CPF/CNPJ", Propriedade = "CPFCNPJTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 38, Descricao = "Transportador Terceiro - IE", Propriedade = "IETransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 39, Descricao = "Transportador Terceiro - Razão Social", Propriedade = "RazaoSocialTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 40, Descricao = "Transportador Terceiro - Nome Fantasia", Propriedade = "NomeFantasiaTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 41, Descricao = "Transportador Terceiro - Endereço", Propriedade = "EnderecoTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 42, Descricao = "Transportador Terceiro - Bairro", Propriedade = "BairroTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 43, Descricao = "Transportador Terceiro - Cidade", Propriedade = "CidadeTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 44, Descricao = "Transportador Terceiro - Estado (UF)", Propriedade = "EstadoTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 45, Descricao = "Transportador Terceiro - IBGE", Propriedade = "IBGETransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 46, Descricao = "Transportador Terceiro - CEP", Propriedade = "CEPTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 47, Descricao = "Transportador Terceiro - Tipo Pessoa (Física = 1 | Jurídica = 2)", Propriedade = "TipoPessoaTransportadorTerceiro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 48, Descricao = "Tomador - CPF/CNPJ", Propriedade = "CPFCNPJTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 49, Descricao = "Tomador - IE", Propriedade = "IETomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 50, Descricao = "Tomador - Razão Social", Propriedade = "RazaoSocialTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 51, Descricao = "Tomador - Nome Fantasia", Propriedade = "NomeFantasiaTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 52, Descricao = "Tomador - Endereço", Propriedade = "EnderecoTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 53, Descricao = "Tomador - Bairro", Propriedade = "BairroTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 54, Descricao = "Tomador - Cidade", Propriedade = "CidadeTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 55, Descricao = "Tomador - Estado (UF)", Propriedade = "EstadoTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 56, Descricao = "Tomador - IBGE", Propriedade = "IBGETomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 57, Descricao = "Tomador - CEP", Propriedade = "CEPTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 58, Descricao = "Tomador - Tipo Pessoa (Física = 1 | Jurídica = 2)", Propriedade = "TipoPessoaTomador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 59, Descricao = "Expedidor - CPF/CNPJ", Propriedade = "CPFCNPJExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 60, Descricao = "Expedidor - IE", Propriedade = "IEExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 61, Descricao = "Expedidor - Razão Social", Propriedade = "RazaoSocialExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 62, Descricao = "Expedidor - Nome Fantasia", Propriedade = "NomeFantasiaExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 63, Descricao = "Expedidor - Endereço", Propriedade = "EnderecoExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 64, Descricao = "Expedidor - Bairro", Propriedade = "BairroExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 65, Descricao = "Expedidor - Cidade", Propriedade = "CidadeExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 66, Descricao = "Expedidor - Estado (UF)", Propriedade = "EstadoExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 67, Descricao = "Expedidor - IBGE", Propriedade = "IBGEExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 68, Descricao = "Expedidor - CEP", Propriedade = "CEPExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 69, Descricao = "Expedidor - Tipo Pessoa (Física = 1 | Jurídica = 2)", Propriedade = "TipoPessoaExpedidor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 70, Descricao = "Recebedor - CPF/CNPJ", Propriedade = "CPFCNPJRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 71, Descricao = "Recebedor - IE", Propriedade = "IERecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 72, Descricao = "Recebedor - Razão Social", Propriedade = "RazaoSocialRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 73, Descricao = "Recebedor - Nome Fantasia", Propriedade = "NomeFantasiaRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 74, Descricao = "Recebedor - Endereço", Propriedade = "EnderecoRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 75, Descricao = "Recebedor - Bairro", Propriedade = "BairroRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 76, Descricao = "Recebedor - Cidade", Propriedade = "CidadeRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 77, Descricao = "Recebedor - Estado (UF)", Propriedade = "EstadoRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 78, Descricao = "Recebedor - IBGE", Propriedade = "IBGERecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 79, Descricao = "Recebedor - CEP", Propriedade = "CEPRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 80, Descricao = "Recebedor - Tipo Pessoa (Física = 1 | Jurídica = 2)", Propriedade = "TipoPessoaRecebedor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        public bool ObterClienteImportacao(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, string descCliente, out Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, out string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFCNPJ = linha.Colunas.Where(obj => obj.NomeCampo == $"CPFCNPJ{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = linha.Colunas.Where(obj => obj.NomeCampo == $"IE{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRazaoSocial = linha.Colunas.Where(obj => obj.NomeCampo == $"RazaoSocial{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNomeFantasia = linha.Colunas.Where(obj => obj.NomeCampo == $"NomeFantasia{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEndereco = linha.Colunas.Where(obj => obj.NomeCampo == $"Endereco{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairro = linha.Colunas.Where(obj => obj.NomeCampo == $"Bairro{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCidade = linha.Colunas.Where(obj => obj.NomeCampo == $"Cidade{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEstado = linha.Colunas.Where(obj => obj.NomeCampo == $"Estado{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIBGE = linha.Colunas.Where(obj => obj.NomeCampo == $"IBGE{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEP = linha.Colunas.Where(obj => obj.NomeCampo == $"CEP{descCliente}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoPessoa = linha.Colunas.Where(obj => obj.NomeCampo == $"TipoPessoa{descCliente}").FirstOrDefault();

            pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
            pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();

            if (colCPFCNPJ != null)
            {
                pessoa.CPFCNPJ = Utilidades.String.OnlyNumbers((string)colCPFCNPJ.Valor);

                if (pessoa.CPFCNPJ.Length == 11)
                {
                    if (!Utilidades.Validate.ValidarCPF(pessoa.CPFCNPJ))
                    {
                        mensagem = "O CPF é inválido.";
                        return false;
                    }
                }
                else if (!Utilidades.Validate.ValidarCNPJ(pessoa.CPFCNPJ))
                {
                    mensagem = "O CNPJ é inválido.";
                    return false;
                }
            }
            else
            {
                mensagem = $"O CPF/CNPJ do(a) {descCliente} é obrigatório.";
                return false;
            }

            string cidade = null;
            string estado = null;
            int codigoIBGE = 0;

            if (colIE != null)
                pessoa.RGIE = Utilidades.String.OnlyNumbers((string)colIE.Valor);

            if (colRazaoSocial != null)
                pessoa.RazaoSocial = SanitizarDadoImportacao((string)colRazaoSocial.Valor);

            if (colNomeFantasia != null)
                pessoa.NomeFantasia = SanitizarDadoImportacao((string)colNomeFantasia.Valor);

            pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();

            if (colEndereco != null)
                pessoa.Endereco.Logradouro = SanitizarDadoImportacao((string)colEndereco.Valor);

            if (colBairro != null)
                pessoa.Endereco.Bairro = SanitizarDadoImportacao((string)colBairro.Valor);

            if (colCidade != null)
                cidade = SanitizarDadoImportacao((string)colCidade.Valor);

            if (colEstado != null)
                estado = SanitizarDadoImportacao((string)colEstado.Valor);

            if (colIBGE != null)
                int.TryParse(Utilidades.String.OnlyNumbers((string)colIBGE.Valor), out codigoIBGE);

            if (colCEP != null)
            {
                int.TryParse(Utilidades.String.OnlyNumbers((string)colCEP.Valor), out int cep);

                if (cep > 0)
                    pessoa.Endereco.CEP = cep.ToString("00000000");
            }

            if (codigoIBGE == 0 && !string.IsNullOrWhiteSpace(pessoa.Endereco.CEP))
            {
                using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoSessaoBancoDados.Nova))
                {
                    AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                    AdminMultisoftware.Dominio.Entidades.Localidades.Endereco enderecoCEP = repEndereco.BuscarCEP(pessoa.Endereco.CEP);

                    if (enderecoCEP != null)
                    {
                        codigoIBGE = enderecoCEP.Localidade.CodigoIBGE.ToInt();

                        if (string.IsNullOrWhiteSpace(pessoa.Endereco.Bairro))
                            pessoa.Endereco.Bairro = enderecoCEP.Bairro?.Descricao;

                        if (string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro))
                            pessoa.Endereco.Logradouro = enderecoCEP.Logradouro;
                    }
                }
            }

            decimal latitudeLocalidade = 0;
            decimal longitudeLocalidade = 0;

            if (codigoIBGE == 0)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(cidade), estado);

                if (localidade != null)
                {
                    codigoIBGE = localidade.CodigoIBGE;
                    latitudeLocalidade = localidade.Latitude ?? 0;
                    longitudeLocalidade = localidade.Longitude ?? 0;
                }
            }

            if (codigoIBGE == 0)
            {
                mensagem = "A localidade do cliente é obrigatória.";
                return false;
            }

            pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade()
            {
                IBGE = codigoIBGE,
                Descricao = cidade,
                SiglaUF = estado
            };

            pessoa.AtualizarEnderecoPessoa = false;

            mensagem = null;
            return true;
        }

        public bool ObterEmpresaImportacao(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, string descEmpresa, out Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresa, out string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFCNPJ = linha.Colunas.Where(obj => obj.NomeCampo == $"CPFCNPJ{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = linha.Colunas.Where(obj => obj.NomeCampo == $"IE{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRazaoSocial = linha.Colunas.Where(obj => obj.NomeCampo == $"RazaoSocial{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNomeFantasia = linha.Colunas.Where(obj => obj.NomeCampo == $"NomeFantasia{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEndereco = linha.Colunas.Where(obj => obj.NomeCampo == $"Endereco{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairro = linha.Colunas.Where(obj => obj.NomeCampo == $"Bairro{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCidade = linha.Colunas.Where(obj => obj.NomeCampo == $"Cidade{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEstado = linha.Colunas.Where(obj => obj.NomeCampo == $"Estado{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIBGE = linha.Colunas.Where(obj => obj.NomeCampo == $"IBGE{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEP = linha.Colunas.Where(obj => obj.NomeCampo == $"CEP{descEmpresa}").FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoPessoa = linha.Colunas.Where(obj => obj.NomeCampo == $"TipoPessoa{descEmpresa}").FirstOrDefault();

            empresa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
            empresa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();

            if (colCPFCNPJ != null)
            {
                empresa.CNPJ = Utilidades.String.OnlyNumbers(colCPFCNPJ.Valor);

                if (!Utilidades.Validate.ValidarCNPJ(empresa.CNPJ))
                {
                    mensagem = "O CNPJ é inválido.";
                    return false;
                }
            }
            else
            {
                mensagem = $"O CPF/CNPJ do(a) {descEmpresa} é obrigatório.";
                return false;
            }

            string cidade = null;
            string estado = null;
            int codigoIBGE = 0;

            if (colIE != null)
                empresa.IE = Utilidades.String.OnlyNumbers(colIE.Valor);

            if (colRazaoSocial != null)
                empresa.RazaoSocial = SanitizarDadoImportacao(colRazaoSocial.Valor);

            if (colNomeFantasia != null)
                empresa.NomeFantasia = SanitizarDadoImportacao(colNomeFantasia.Valor);

            if (colEndereco != null)
                empresa.Endereco.Logradouro = SanitizarDadoImportacao(colEndereco.Valor);

            if (colBairro != null)
                empresa.Endereco.Bairro = SanitizarDadoImportacao(colBairro.Valor);

            if (colCidade != null)
                cidade = SanitizarDadoImportacao(colCidade.Valor);

            if (colEstado != null)
                estado = SanitizarDadoImportacao(colEstado.Valor);

            if (colIBGE != null)
                int.TryParse(Utilidades.String.OnlyNumbers(colIBGE.Valor), out codigoIBGE);

            if (colCEP != null)
            {
                int.TryParse(Utilidades.String.OnlyNumbers(colCEP.Valor), out int cep);

                if (cep > 0)
                    empresa.Endereco.CEP = cep.ToString("00000000");
            }

            if (codigoIBGE == 0 && !string.IsNullOrWhiteSpace(empresa.Endereco.CEP))
            {
                using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoSessaoBancoDados.Nova))
                {
                    AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                    AdminMultisoftware.Dominio.Entidades.Localidades.Endereco enderecoCEP = repEndereco.BuscarCEP(empresa.Endereco.CEP);

                    if (enderecoCEP != null)
                    {
                        codigoIBGE = enderecoCEP.Localidade.CodigoIBGE.ToInt();

                        if (string.IsNullOrWhiteSpace(empresa.Endereco.Bairro))
                            empresa.Endereco.Bairro = enderecoCEP.Bairro?.Descricao;

                        if (string.IsNullOrWhiteSpace(empresa.Endereco.Logradouro))
                            empresa.Endereco.Logradouro = enderecoCEP.Logradouro;
                    }
                }
            }

            decimal latitudeLocalidade = 0;
            decimal longitudeLocalidade = 0;

            if (codigoIBGE == 0)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(cidade), estado);

                if (localidade != null)
                {
                    codigoIBGE = localidade.CodigoIBGE;
                    latitudeLocalidade = localidade.Latitude ?? 0;
                    longitudeLocalidade = localidade.Longitude ?? 0;
                }
            }

            if (codigoIBGE == 0)
            {
                mensagem = "A localidade do cliente é obrigatória.";
                return false;
            }

            empresa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade()
            {
                IBGE = codigoIBGE,
                Descricao = cidade,
                SiglaUF = estado
            };

            mensagem = null;
            return true;
        }

        private string SanitizarDadoImportacao(string dado)
        {
            if (dado == null)
                return string.Empty;

            return dado.Replace("\"", "").Trim();
        }

        private void RetornarFalhaLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao, int indice, string mensagem)
        {
            retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false });
        }

        private void RetornarSucessoLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao, int indice)
        {
            retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = true });
        }

        private object ObterDetalhesCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        {
            var retorno = new
            {
                cteTerceiro.Codigo,
                NumeroTerceiro = cteTerceiro.Numero,
                SerieTerceiro = cteTerceiro.Serie,
                Terceiro = new
                {
                    Codigo = cteTerceiro.Emitente.CPF_CNPJ,
                    cteTerceiro.Emitente.Descricao
                },
                cteTerceiro.TipoPagamento,
                cteTerceiro.TipoServico,
                Tipo = cteTerceiro.TipoCTE,
                cteTerceiro.TipoTomador,
                DataEmissao = cteTerceiro.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                CFOP = new
                {
                    cteTerceiro.CFOP.Codigo,
                    Descricao = cteTerceiro.CFOP.CodigoCFOP
                },
                ChaveTerceiro = cteTerceiro.ChaveAcesso,
                LocalidadeEmissao = cteTerceiro.LocalidadeInicioPrestacao != null ? new
                {
                    cteTerceiro.LocalidadeInicioPrestacao.Codigo,
                    Descricao = cteTerceiro.LocalidadeInicioPrestacao.DescricaoCidadeEstado
                } : null,
                LocalidadeInicioPrestacao = cteTerceiro.LocalidadeInicioPrestacao != null ? new
                {
                    cteTerceiro.LocalidadeInicioPrestacao.Codigo,
                    Descricao = cteTerceiro.LocalidadeInicioPrestacao.DescricaoCidadeEstado
                } : null,
                LocalidadeTerminoPrestacao = cteTerceiro.LocalidadeTerminoPrestacao != null ? new
                {
                    cteTerceiro.LocalidadeTerminoPrestacao.Codigo,
                    Descricao = cteTerceiro.LocalidadeTerminoPrestacao.DescricaoCidadeEstado
                } : null,
                TipoModal = cteTerceiro.Modal
            };

            return retorno;
        }

        private object ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador tipo, Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        {
            Dominio.Entidades.ParticipanteCTe cliente = cteTerceiro.ObterParticipante(tipo);

            if (cliente == null)
                return null;

            var retorno = new
            {
                PessoaExterior = new
                {
                    Codigo = cliente.Cliente?.CPF_CNPJ.ToString() ?? "",
                    Descricao = cliente.Nome
                },
                CPFCNPJ = cliente.CPF_CNPJ_SemFormato,
                IE = cliente.IE_RG,
                RazaoSocial = cliente.Nome,
                NomeFantasia = cliente.NomeFantasia,
                TelefonePrincipal = cliente.Telefone1,
                TelefoneSecundario = cliente.Telefone2,
                Atividade = new
                {
                    Codigo = cliente.Atividade?.Codigo ?? 0,
                    Descricao = cliente.Atividade?.Descricao
                },
                CEP = cliente.CEP,
                Endereco = cliente.Endereco,
                Numero = cliente.Numero,
                Bairro = cliente.Bairro,
                Complemento = cliente.Complemento,
                Localidade = new
                {
                    Codigo = cliente.Localidade?.Codigo ?? 0,
                    Descricao = cliente.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                    Estado = cliente.Localidade?.Estado?.Sigla ?? string.Empty
                },
                EmailGeral = cliente.Email,
                EnviarXMLEmailGeral = cliente.EmailStatus,
                EmailContato = cliente.EmailContato,
                EnviarXMLEmailContato = cliente.EmailContatoStatus,
                EmailContador = cliente.EmailContador,
                EnviarXMLEmailContador = cliente.EmailContadorStatus,
                SalvarEndereco = cliente.Localidade?.Estado?.Sigla == "EX" ? false : cliente.SalvarEndereco,
                ParticipanteExterior = cliente.Exterior,
                LocalidadeExterior = cliente.Cidade,
                Pais = new
                {
                    Codigo = cliente.Pais?.Codigo ?? 0,
                    Descricao = cliente.Pais?.Nome
                }
            };

            return retorno;
        }

        private object ObterDocumentosCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        {
            List<dynamic> retorno = new List<dynamic>();
            List<dynamic> retornoOutrosDocumentos = (from obj in cteTerceiro.CTesTerceiroOutrosDocumentos
                                                     select new
                                                     {
                                                         obj.Codigo,
                                                         BaseCalculoICMS = 0.ToString("n2"),
                                                         BaseCalculoICMSST = 0.ToString("n2"),
                                                         Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                                                         CFOP = string.Empty,
                                                         Chave = string.Empty,
                                                         DataEmissao = string.Empty,
                                                         ValorNotaFiscal = obj.Valor.ToString("n2"),
                                                         ValorICMS = 0.ToString("n2"),
                                                         ValorICMSST = 0.ToString("n2"),
                                                         ValorProdutos = 0.ToString("n2"),
                                                         Peso = 0.ToString("n2"),
                                                         PIN = string.Empty,
                                                         Serie = string.Empty,
                                                         obj.Numero,
                                                         Modelo = ((int)obj.Tipo).ToString().PadLeft(2, '0'),
                                                         DescricaoCTe = string.Empty,
                                                         TipoDocumento = Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                                         NumeroReferenciaEDI = string.Empty,
                                                         PINSuframa = string.Empty,
                                                         NCMPredominante = string.Empty,
                                                         NumeroControleCliente = string.Empty
                                                     }).Cast<dynamic>().ToList();

            List<dynamic> retornoNotasFiscais = (from obj in cteTerceiro.CTesTerceiroNotasFiscais
                                                 select new
                                                 {
                                                     obj.Codigo,
                                                     BaseCalculoICMS = 0.ToString("n2"),
                                                     BaseCalculoICMSST = 0.ToString("n2"),
                                                     Descricao = string.Empty,
                                                     CFOP = obj.CFOP,
                                                     Chave = string.Empty,
                                                     DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                                     ValorNotaFiscal = obj.ValorTotal.ToString("n2"),
                                                     ValorICMS = 0.ToString("n2"),
                                                     ValorICMSST = 0.ToString("n2"),
                                                     ValorProdutos = 0.ToString("n2"),
                                                     Peso = obj.Peso.ToString("n2"),
                                                     PIN = string.Empty,
                                                     obj.Serie,
                                                     obj.Numero,
                                                     Modelo = "01",
                                                     DescricaoCTe = string.Empty,
                                                     TipoDocumento = Dominio.Enumeradores.TipoDocumentoCTe.NF,
                                                     NumeroReferenciaEDI = string.Empty,
                                                     PINSuframa = string.Empty,
                                                     NCMPredominante = string.Empty,
                                                     NumeroControleCliente = string.Empty
                                                 }).Cast<dynamic>().ToList();

            List<dynamic> retornoNFes = (from obj in cteTerceiro.CTesTerceiroNFes
                                         select new
                                         {
                                             obj.Codigo,
                                             BaseCalculoICMS = 0.ToString("n2"),
                                             BaseCalculoICMSST = 0.ToString("n2"),
                                             Descricao = string.Empty,
                                             CFOP = string.Empty,
                                             obj.Chave,
                                             DataEmissao = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy") : "",
                                             ValorNotaFiscal = obj.ValorTotal.ToString("n2"),
                                             ValorICMS = 0.ToString("n2"),
                                             ValorICMSST = 0.ToString("n2"),
                                             ValorProdutos = 0.ToString("n2"),
                                             Peso = obj.Peso.ToString("n2"),
                                             PIN = string.Empty,
                                             obj.Serie,
                                             obj.Numero,
                                             Modelo = "55",
                                             DescricaoCTe = string.Empty,
                                             TipoDocumento = Dominio.Enumeradores.TipoDocumentoCTe.NFe,
                                             obj.NumeroReferenciaEDI,
                                             obj.PINSuframa,
                                             NCMPredominante = obj.NCM,
                                             obj.NumeroControleCliente
                                         }).Cast<dynamic>().ToList();

            if (retornoOutrosDocumentos.Count > 0)
                retorno.AddRange(retornoOutrosDocumentos);
            if (retornoNotasFiscais.Count > 0)
                retorno.AddRange(retornoNotasFiscais);
            if (retornoNFes.Count > 0)
                retorno.AddRange(retornoNFes);

            return retorno;
        }

        private object ObterModalRodoviarioCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        {
            var retorno = new
            {
                IndicadorLotacao = cteTerceiro.Lotacao,

                //Não salva, mas é obrigatório pra atualizar o documento
                RNTRC = "12345678",
                PrevisaoEntrega = DateTime.Now.Date.ToString("dd/MM/yyyy")
            };

            return retorno;
        }

        private object ObterModalAereoCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        {
            var retorno = new
            {
                cteTerceiro.NumeroMinuta,
                NumeroOCA = cteTerceiro.NumeroOperacionalConhecimentoAereo
            };

            return retorno;
        }

        private object ObterQuantidadesCargaCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.UnidadeDeMedida repUnidadeMedida = new Repositorio.UnidadeDeMedida(unidadeDeTrabalho);

            var retorno = (from obj in cteTerceiro.CTeTerceiroQuantidades
                           select new
                           {
                               obj.Codigo,
                               CodigoUnidadeMedida = repUnidadeMedida.BuscarPorCodigoUnidade(obj.Unidade.ToString("d").PadLeft(2, '0'))?.Codigo ?? 0,
                               UnidadeMedida = obj.TipoMedida,
                               obj.TipoMedida,
                               Quantidade = obj.Quantidade.ToString("n4")
                           }).ToList();

            return retorno;
        }

        private object ObterSegurosCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        {
            var retorno = (from obj in cteTerceiro.CTeTerceiroSeguros
                           select new
                           {
                               Codigo = obj.Codigo,
                               CodigoResponsavel = obj.Tipo,
                               Responsavel = Dominio.Enumeradores.TipoSeguroHelper.ObterDescricao(obj.Tipo),
                               Seguradora = obj.NomeSeguradora,
                               CNPJSeguradora = string.Empty,
                               NumeroApolice = obj.NumeroApolice,
                               NumeroAverbacao = obj.NumeroAverbacao,
                               Valor = obj.Valor.ToString("n2")
                           }).ToList();

            return retorno;
        }

        private object ObterInformacaoCargaCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        {
            var retorno = new
            {
                ValorTotalCarga = cteTerceiro.ValorTotalMercadoria,
                ProdutoPredominante = cteTerceiro.ProdutoPredominante,
                OutrasCaracteristicas = cteTerceiro.OutrasCaracteristicasDaCarga
            };

            return retorno;
        }

        private object ObterTotaisServicoCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        {
            var retorno = new
            {
                ValorFrete = cteTerceiro.ValorPrestacaoServico.ToString("n2"),
                ValorPrestacaoServico = cteTerceiro.ValorPrestacaoServico.ToString("n2"),
                ValorReceber = cteTerceiro.ValorAReceber.ToString("n2"),
                ICMS = cteTerceiro.CST,
                BaseCalculoICMS = cteTerceiro.BaseCalculoICMS.ToString("n2"),
                AliquotaICMS = cteTerceiro.AliquotaICMS.ToString("n2"),
                ValorICMS = cteTerceiro.ValorICMS.ToString("n2"),
                PercentualReducaoBaseCalculoICMS = cteTerceiro.PercentualReducaoBaseCalculoICMS
            };

            return retorno;
        }

        private void SalvarDimensoes(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeTerceiroDimensao repDimensao = new Repositorio.Embarcador.CTe.CTeTerceiroDimensao(unitOfWork);

            dynamic dimensoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Dimensoes"));

            if (cteTerceiro.CTeTerceiroDimensoes?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic dimensao in dimensoes)
                    if (dimensao.Codigo != null)
                        codigos.Add((int)dimensao.Codigo);

                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao> dimensoesDeletar = (from obj in cteTerceiro.CTeTerceiroDimensoes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < dimensoesDeletar.Count; i++)
                    repDimensao.Deletar(dimensoesDeletar[i]);
            }

            foreach (dynamic dimensao in dimensoes)
            {
                Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao cteTerceiroDimensao = dimensao.Codigo != null ? repDimensao.BuscarPorCodigo((int)dimensao.Codigo, false) : null;
                if (cteTerceiroDimensao == null)
                {
                    cteTerceiroDimensao = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroDimensao();
                    cteTerceiroDimensao.CTeTerceiro = cteTerceiro;
                }
                cteTerceiroDimensao.Altura = (decimal)dimensao.Altura;
                cteTerceiroDimensao.Comprimento = (decimal)dimensao.Comprimento;
                cteTerceiroDimensao.Largura = (decimal)dimensao.Largura;
                cteTerceiroDimensao.MetrosCubicos = (decimal)dimensao.MetrosCubicos;
                cteTerceiroDimensao.Volumes = (int)dimensao.Volumes;

                if (cteTerceiroDimensao.Codigo > 0)
                    repDimensao.Atualizar(cteTerceiroDimensao);
                else
                    repDimensao.Inserir(cteTerceiroDimensao);
            }
        }

        #endregion
    }
}
