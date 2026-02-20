using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Servicos.Extensions;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Impressao
{
    public class CargaImpressaoDocumentosController : BaseController
    {
        #region Construtores

        public CargaImpressaoDocumentosController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
        public async Task<IActionResult> InformarImpressaoRealizada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarImpressao))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga = int.Parse(Request.Params("Carga"));

                if (serCarga.ConfirmarImpressaoDocumentosCarga(codigoCarga, Auditado, unitOfWork))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, "Não é possível confirmar a impressão dos documentos nesta situação");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os CTes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        //[AcceptVerbs("POST", "GET")]
        //public async Task<IActionResult> EnviarDocumentosParaImpressao()
        //{
        //    Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        unidadeTrabalho.Start();
        //        int codigoCarga;
        //        int.TryParse(Request.Params("Carga"), out codigoCarga);
        //        Servicos.Embarcador.Carga.Impressao serCargaImpressao = new Servicos.Embarcador.Carga.Impressao();

        //        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
        //        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
        //        string retorno = serCargaImpressao.EnviarDocumentosParaImpressao(carga, unidadeTrabalho);

        //        if (string.IsNullOrWhiteSpace(retorno))
        //        {
        //            serCargaImpressao.EnviarMDFeParaImpressao(carga, unidadeTrabalho);
        //            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Enviou os documentos para emissão.", unidadeTrabalho);
        //            unidadeTrabalho.CommitChanges();
        //            return new JsonpResult(true);
        //        }
        //        else
        //        {
        //            unidadeTrabalho.Rollback();
        //            return new JsonpResult(false, true, retorno);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        unidadeTrabalho.Rollback();
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, false, "Ocorreu uma falha ao realizar ao enviar para impressão.");
        //    }
        //    finally
        //    {
        //        unidadeTrabalho.Dispose();
        //    }
        //}

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarBoletosParaImpressao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaPedido = Request.GetIntParam("CargaPedido");
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não encontrado.");

                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto repositorioCargaImpressaoBoleto = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto> boletos = repositorioCargaImpressaoBoleto.BuscarPorCargaPedido(cargaPedido.Codigo);

                if (boletos.Count() > 0)
                {
                    unitOfWork.Start();

                    foreach (Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto boleto in boletos)
                    {
                        boleto.SituacaoImpressao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Solicitado;
                        repositorioCargaImpressaoBoleto.Atualizar(boleto);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Enviou boletos para emissão.", unitOfWork);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar ao enviar para impressão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarNotaParaImpressao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaPedido = Request.GetIntParam("CargaPedido");
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não encontrado.");

                int numeroNota = Request.GetIntParam("Nota");
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlNotaFiscal = repositorioPedidoXMLNotaFiscal.ConsultarParaEnvioImpressaoPorCargaPedidoENota(cargaPedido.Codigo, numeroNota);

                if (xmlNotaFiscal == null)
                    return new JsonpResult(false, true, "Nota Fiscal não encontrada.");

                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe repositorioCargaImpressaoNFe = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe cargaImpressaoNFe = repositorioCargaImpressaoNFe.BuscarPorCargaPedidoENumero(cargaPedido.Codigo, xmlNotaFiscal.XMLNotaFiscal.Numero);

                unitOfWork.Start();

                if (cargaImpressaoNFe != null)
                {
                    cargaImpressaoNFe.SituacaoImpressao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Solicitado;

                    repositorioCargaImpressaoNFe.Atualizar(cargaImpressaoNFe);
                }
                else
                {
                    cargaImpressaoNFe = new Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe
                    {
                        CargaPedido = xmlNotaFiscal.CargaPedido,
                        SituacaoImpressao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Solicitado,
                        XML = xmlNotaFiscal.XMLNotaFiscal.XML,
                        Chave = xmlNotaFiscal.XMLNotaFiscal.Chave,
                        Numero = xmlNotaFiscal.XMLNotaFiscal.Numero,
                        Serie = xmlNotaFiscal.XMLNotaFiscal.Serie
                    };

                    repositorioCargaImpressaoNFe.Inserir(cargaImpressaoNFe);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Enviou nota para emissão.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar ao enviar para impressão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarNotasBoletosParaImpressao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe repCargaImpressaoNFe = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe(unitOfWork);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto repCargaImpressaoBoleto = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                int codigoCarga = Request.GetIntParam("Carga");

                List<int> listaPedidos = repPedido.BuscarCodigosPedidosPorCarga(codigoCarga);
                if (listaPedidos == null || listaPedidos.Count == 0)
                    return new JsonpResult(false, false, "Não existem pedidos associados a carga para solicitar para impressão.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto> boletos = repCargaImpressaoBoleto.BuscarPorPedidos(listaPedidos); //repCargaImpressaoBoleto.BuscarPorCarga(codigoCarga);

                unitOfWork.Start();

                // Busca todas notas da carga
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = ObterCargaPedidosOrdenado(carga, unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    //Tarefa 5151, só imprime notas sem recebedor
                    if (cargaPedido.Recebedor == null || !configuracaoTMS.NaoImprimirNotasBoletosComRecebedor)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> xmlNotasFiscais = repPedidoXMLNotaFiscal.ConsultarParaEnvioImpressaoPorPedido(cargaPedido.Pedido.Codigo); //repPedidoXMLNotaFiscal.ConsultarParaEnvioImpressao(cargaPedido.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlNotaFiscal in xmlNotasFiscais)
                        {
                            // Verifica se ja existe carga impressao 
                            Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe cargaImpressaoNFe = repCargaImpressaoNFe.BuscarPorCargaPedidoENumero(cargaPedido.Codigo, xmlNotaFiscal.XMLNotaFiscal.Numero);

                            // Seta situação
                            if (cargaImpressaoNFe != null)
                            {
                                cargaImpressaoNFe.SituacaoImpressao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Solicitado;
                                repCargaImpressaoNFe.Atualizar(cargaImpressaoNFe);
                            }
                            else
                            {
                                // ou cria nova
                                cargaImpressaoNFe = new Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe
                                {
                                    CargaPedido = xmlNotaFiscal.CargaPedido,
                                    SituacaoImpressao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Solicitado,
                                    XML = xmlNotaFiscal.XMLNotaFiscal.XML,
                                    Chave = xmlNotaFiscal.XMLNotaFiscal.Chave,
                                    Numero = xmlNotaFiscal.XMLNotaFiscal.Numero,
                                    Serie = xmlNotaFiscal.XMLNotaFiscal.Serie
                                };
                                repCargaImpressaoNFe.Inserir(cargaImpressaoNFe);
                            }
                        }
                    }
                }

                // Seta boletos Para impressão
                foreach (Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto boleto in boletos)
                {
                    boleto.SituacaoImpressao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Solicitado;
                    repCargaImpressaoBoleto.Atualizar(boleto);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Enviou notas e boletos para emissão.", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar ao enviar para impressão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarCTesParaImpressao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Impressao serCargaImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                unitOfWork.Start();

                string retorno = serCargaImpressao.EnviarDocumentosParaImpressao(carga);

                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, retorno);
                }
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Enviou os CT-es para Impressão.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar ao enviar para impressão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarMDFesParaImpressao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Impressao serCargaImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                unitOfWork.Start();

                string retorno = serCargaImpressao.EnviarMDFeParaImpressao(carga);

                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, retorno);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Enviou os MDF-es para emissão.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar ao enviar para impressão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioDeTroca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigo = Request.GetIntParam("Carga");
                bool relatorioTroca = Request.GetBoolParam("Recolhimento");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                string mensagem = string.Empty;
                byte[] arquivo = new Servicos.Embarcador.Carga.Carregamento().RelatorioTroca(carga, relatorioTroca, unitOfWork, ref mensagem);

                if (arquivo == null && !string.IsNullOrWhiteSpace(mensagem))
                    return new JsonpResult(false, mensagem);

                string nomeArquivo = "Relatório de Entregas";
                if (relatorioTroca)
                    nomeArquivo = "Relatório de Troca";

                return Arquivo(arquivo, "application/pdf", string.Concat(nomeArquivo, " - ", carga.CodigoCargaEmbarcador, ".pdf"));
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioBoletimViagem()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioPosicao = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioPosicao.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] pdf = ReportRequest.WithType(ReportType.BoletimViagem)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoCarga", codigo.ToString())
                    .CallReport()
                    .GetContentFile();

                return Arquivo(pdf, "application/pdf", $"Boletim de Viagem {carga.Descricao}.pdf");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o Boletim de viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioDiarioBordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                byte[] todosPdfDiarioBordoCompactado = servicoImpressao.ObterTodosPdfTipoImpressaoDiarioBordoCompactado(carga);

                return Arquivo(todosPdfDiarioBordoCompactado, "application/zip", $"Diário de Bordo {carga.CodigoCargaEmbarcador}.zip");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o diário de bordo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioPlanoViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                byte[] pdf = servicoImpressao.ObterPdfPlanoViagem(carga);

                return Arquivo(pdf, "application/pdf", $"Plano de Viagem {carga.CodigoCargaEmbarcador}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o plano de viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioBoletimViagemEmbarque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList cargaEntregaCheckList = repCargaEntregaCheckList.BuscarPrimeiroPorCarga(codigoCarga, TipoCheckList.Desembarque);

                if (cargaEntregaCheckList == null)
                    return new JsonpResult(false, true, "Nenhum CheckList encontardo para essa carga.");

                byte[] pdf = ReportRequest.WithType(ReportType.BoletimViagemEmbarque)
                                    .WithExecutionType(ExecutionType.Sync)
                                    .AddExtraData("CodigoCarga", codigoCarga.ToString())
                                    .CallReport()
                                    .GetContentFile();

                return Arquivo(pdf, "application/pdf", $"BoletimViagemEmbarque.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o Boletim de viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLotePDF()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesNFesAutorizadosPorCarga(codigoCarga, false, false);
                List<int> mdfes = repCargaMDFe.BuscarCodigosMDFePorAutorizadosCarga(codigoCarga);
                List<int> valePedagios = repCargaValePedagio.BuscarCodigosValePedagiosPorCarga(codigoCarga);

                if (ctes.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                byte[] arquivo = svcCTe.ObterLotePDFsAgrupado(codigoCarga, ctes, mdfes, valePedagios, unidadeTrabalho, TipoServicoMultisoftware, Cliente.NomeFantasia);

                return Arquivo(arquivo, "application/pdf", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_Documentos.pdf"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de PDF.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
        public async Task<IActionResult> DownloadLoteDocumentoCompactado()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, false, false);

                List<int> mdfes = repCargaMDFe.BuscarCodigosMDFePorAutorizadosCarga(codigoCarga);

                List<int> valePedagios = repCargaValePedagio.BuscarCodigosValePedagiosSemPararPorCarga(codigoCarga);

                if (ctes.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLotePDFs(codigoCarga, ctes, mdfes, valePedagios, unidadeTrabalho, TipoServicoMultisoftware, Cliente.NomeFantasia);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_Documentos.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de PDF.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadNFeBoleto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe repCargaImpressaoNFe = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe(unitOfWork);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto repCargaImpressaoBoleto = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                int codigoCarga = Request.GetIntParam("Carga");

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho dos arquivos não está disponível.");

                string diretorioBoletos = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, "BOLETOS");
                string diretorioDanfes = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, "DANFE");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga == null)
                    return new JsonpResult(true, false, "Carga não localizada.");

                List<int> listaPedidos = repPedido.BuscarCodigosPedidosPorCarga(codigoCarga);
                if (listaPedidos == null || listaPedidos.Count == 0)
                    return new JsonpResult(false, false, "Não existem pedidos associados a carga para solicitar para realizar o download.");

                string mensagem = string.Empty;
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                System.IO.MemoryStream arquivo = serCarga.ObterPDFsNFesBoletos(listaPedidos, unitOfWork, ref mensagem);
                if (!string.IsNullOrWhiteSpace(mensagem))
                    return new JsonpResult(false, false, mensagem);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_NFEsBoletos.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de PDF.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ImprimirCRT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);

                byte[] arquivo = servicoImpressao.ObterCRTsCompactado(carga);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_CRTs.zip"));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do CRT.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioDeRomaneio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRomaneio tipoRomaneio = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao().TipoRomaneio;

                Repositorio.Embarcador.Cargas.Carga repositorioPosicao = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioPosicao.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] pdf = ReportRequest.WithType(ReportType.Romaneio)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoCarga", codigo.ToString())
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o Relatório de Romaneio.");

                return Arquivo(pdf, "application/pdf", $"Relatório de Romaneio {carga.Descricao}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o relatório de romaneio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioPedidoPacote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioPosicao = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioPosicao.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.CargaNaoEncontrada);

                byte[] pdf = servicoImpressao.ObterPdfPedidoPacoteCarga(codigoCarga);

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o Relatório de Pedidos e Pacotes.");

                return Arquivo(pdf, "application/pdf", $"Relatório de Pedidos e Pacotes {carga.Descricao}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o relatório de pedidos e pacotes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioDeEmbarque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                byte[] pdf = servicoImpressao.ObterPdfRelatorioDeEmbarque(codigo);

                return Arquivo(pdf, "application/pdf", $"Relatório de Embarque {carga.CodigoCargaEmbarcador}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o relatório de embarque.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> SalvarObservacaoRelatorioEmbarque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string observacaoRelatorioEmbarque = Request.GetStringParam("ObservacaoRelatorioDeEmbarque");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível salvar as Observações do relatório de Embarque.");

                carga.DadosSumarizados.ObservacaoRelatorioDeEmbarque = observacaoRelatorioEmbarque;

                repositorioCarga.Atualizar(carga);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> ObterCargaPedidosOrdenado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRoteirizacao repCargaRoteirizacao = new Repositorio.Embarcador.Cargas.CargaRoteirizacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota repCargaRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in carga.Pedidos /*where obj.PedidoSemNFe == false*/ select obj).ToList();
            Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao cargaRoteirizacao = repCargaRoteirizacao.BuscarPorCarga(carga.Codigo);

            if (cargaRoteirizacao == null)
                return cargaPedidos;

            List<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota> rotas = repCargaRoteirizacaoClientesRota.BuscarPorCargaRoteirizacao(cargaRoteirizacao.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosOrdenado = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota rota in rotas)
                cargaPedidosOrdenado.AddRange((from obj in cargaPedidos where (obj.Recebedor ?? obj.Pedido.Destinatario) == rota.Cliente select obj));

            return cargaPedidosOrdenado;
        }

        #endregion
    }
}
