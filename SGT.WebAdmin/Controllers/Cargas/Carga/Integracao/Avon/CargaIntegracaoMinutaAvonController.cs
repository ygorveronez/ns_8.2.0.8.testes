using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao.Avon
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoMinutaAvonController : BaseController
    {
		#region Construtores

		public CargaIntegracaoMinutaAvonController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaIntegracaoAvon repIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoAvon(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon integracao = repIntegracao.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (integracao.ArquivoRequisicao == null && integracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Minuta " + integracao.NumeroMinuta + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da DACTE.");
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.CargaIntegracaoAvon repIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoAvon(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Minuta", "NumeroMinuta", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Consulta", "DataConsulta", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 30, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon> integracoes = repIntegracao.Consultar(codigoCarga, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repIntegracao.ContarConsulta(codigoCarga));

                var retorno = (from obj in integracoes
                               select new
                               {
                                   obj.Codigo,
                                   obj.NumeroMinuta,
                                   DataConsulta = obj.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoSituacao,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarMinuta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Carga.CTe serCTe = new Servicos.Embarcador.Carga.CTe(unidadeDeTrabalho);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Configuracoes.IntegracaoAvon repIntegracaoAvon = new Repositorio.Embarcador.Configuracoes.IntegracaoAvon(unidadeDeTrabalho);

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, "Carga não encontrada.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    return new JsonpResult(false, "O status da carga não permite que seja realizada uma consulta de minutas da Avon.");

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon configuracaoIntegracaoAvon = repIntegracaoAvon.BuscarPorEmpresa(carga.Empresa.Codigo);

                if (configuracaoIntegracaoAvon == null || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.EnterpriseID) || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.TokenProducao))
                    return new JsonpResult(false, $"A configuração de integração da empresa {carga.Empresa.Descricao} da Avon não foi realizada ou os dados estão incorretos.");

                long numeroMinuta;
                long.TryParse(Request.Params("NumeroMinuta"), out numeroMinuta);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon infoIntegracao = Servicos.Embarcador.Integracao.Avon.IntegracaoMinutaAvon.ConsultarNFes(numeroMinuta, carga.Empresa, unidadeDeTrabalho, this.ClienteAcesso.URLHomologacao);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon cargaIntegracaoAvon = SalvarInformacoesIntegracao(carga, infoIntegracao, unidadeDeTrabalho);

                string connectionString = _conexao.StringConexao;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipante = serCTe.BuscarTipoEmissaoCTeParticipantes(carga.Pedidos.First(), TipoServicoMultisoftware, unidadeDeTrabalho, false);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos = serCTe.BuscarTipoEmissaoDocumentosCTe(carga.Pedidos.First(), TipoServicoMultisoftware, unidadeDeTrabalho);
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = TipoServicoMultisoftware;


                Task t = Task.Factory.StartNew(() => { SalvarNotasFiscaisCarga(codigoCarga, infoIntegracao.Documentos, connectionString, tipoEmissaoCTeParticipante, tipoEmissaoCTeDocumentos, tipoServicoMultisoftware); });

                return new JsonpResult(new
                {
                    DataConsulta = cargaIntegracaoAvon.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                    cargaIntegracaoAvon.GUID,
                    cargaIntegracaoAvon.Mensagem,
                    cargaIntegracaoAvon.NumeroMinuta,
                    PesoTotalDocumentos = cargaIntegracaoAvon.PesoTotalDocumentos.ToString("n2"),
                    QuantidadeDocumentos = cargaIntegracaoAvon.QuantidadeDocumentos.ToString("n0"),
                    cargaIntegracaoAvon.Situacao,
                    Usuario = cargaIntegracaoAvon.Usuario.Nome,
                    ValorTotalDocumentos = cargaIntegracaoAvon.ValorTotalDocumentos.ToString("n2"),
                    cargaIntegracaoAvon.Manual
                });
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar a minuta.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarMinutaManual()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.CargaIntegracaoAvon repCargaIntegracaoAvon = new Repositorio.Embarcador.Cargas.CargaIntegracaoAvon(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, "Carga não encontrada.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    return new JsonpResult(false, "O status da carga não permite que seja salva uma minuta da Avon.");

                long numeroMinuta;
                long.TryParse(Request.Params("NumeroMinuta"), out numeroMinuta);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon cargaIntegracaoAvon = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon()
                {
                    Carga = carga,
                    DataConsulta = DateTime.Now,
                    GUID = string.Empty,
                    Mensagem = "Minuta salva manualmente (não foi consultada no portal da Avon).",
                    NumeroMinuta = numeroMinuta,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Sucesso,
                    Usuario = this.Usuario,
                    Manual = true
                };

                repCargaIntegracaoAvon.Inserir(cargaIntegracaoAvon);

                return new JsonpResult(new
                {
                    DataConsulta = cargaIntegracaoAvon.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                    cargaIntegracaoAvon.GUID,
                    cargaIntegracaoAvon.Mensagem,
                    cargaIntegracaoAvon.NumeroMinuta,
                    PesoTotalDocumentos = cargaIntegracaoAvon.PesoTotalDocumentos.ToString("n2"),
                    QuantidadeDocumentos = cargaIntegracaoAvon.QuantidadeDocumentos.ToString("n0"),
                    cargaIntegracaoAvon.Situacao,
                    Usuario = cargaIntegracaoAvon.Usuario.Nome,
                    ValorTotalDocumentos = cargaIntegracaoAvon.ValorTotalDocumentos.ToString("n2"),
                    cargaIntegracaoAvon.Manual
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar a minuta.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirMinuta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, "Carga não encontrada.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    return new JsonpResult(false, "A situação da carga não permite que seja realizada a exclusão da minuta da Avon.");

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaIntegracaoAvon repCargaIntegracaoAvon = new Repositorio.Embarcador.Cargas.CargaIntegracaoAvon(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon cargaIntegracaoAvon = repCargaIntegracaoAvon.BuscarPorCarga(carga.Codigo);

                if (cargaIntegracaoAvon.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.SalvandoNotasFiscais && cargaIntegracaoAvon.DataConsulta.AddMinutes(10) > DateTime.Now)
                    return new JsonpResult(false, true, "As notas fiscais estão sendo consultadas, aguarde mais alguns minutos para realizar a exclusão.");

                //unidadeDeTrabalho.Start();
                if (!cargaIntegracaoAvon.Manual)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(carga.Pedidos.First().Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXML in pedidoXMLNotasFiscais)
                    {
                        unidadeDeTrabalho.FlushAndClear();

                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXML.XMLNotaFiscal;

                        repPedidoXMLNotaFiscal.Deletar(pedidoXML);
                        repXMLNotaFiscal.Deletar(xmlNotaFiscal);
                    }
                }
                //repCargaIntegracaoAvon.Deletar(cargaIntegracaoAvon);

                cargaIntegracaoAvon.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Excluida;

                repCargaIntegracaoAvon.Atualizar(cargaIntegracaoAvon);

                //unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao excluir a minuta.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarNotasFiscaisCarga(int codigoCarga, List<Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica> notasFiscais, string connectionString, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipante, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(connectionString);

            Servicos.Embarcador.Hubs.IntegracaoAvon svcHubIntegracaoAvon = new Servicos.Embarcador.Hubs.IntegracaoAvon();

            Repositorio.Embarcador.Cargas.CargaIntegracaoAvon repCargaIntegracaoAvon = new Repositorio.Embarcador.Cargas.CargaIntegracaoAvon(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon cargaIntegracaoAvon = repCargaIntegracaoAvon.BuscarPorCarga(codigoCarga);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeDeTrabalho).BuscarPrimeiroRegistro();

            int notasGeradas = 0;
            decimal quantidadeNotasPorPedido = 990m;

            try
            {
                if (notasFiscais == null || notasFiscais.Count == 0)
                    return;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeDeTrabalho);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = carga.Pedidos.First();

                int totalNotasFiscais = notasFiscais.Count;
                int totalPedidos = 1;
                int totalPedidosExistentes = carga.Pedidos.Count;

                if (tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual)
                {
                    if (totalPedidosExistentes > 1)
                    {
                        cargaIntegracaoAvon.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Problemas;
                        cargaIntegracaoAvon.Mensagem = "Existe mais de um pedido nesta carga. É necessário que a carga esteja com apenas um pedido para realizar a leitura da minuta.";

                        repCargaIntegracaoAvon.Atualizar(cargaIntegracaoAvon);

                        svcHubIntegracaoAvon.InformarMinutaAtualizada(codigoCarga, notasGeradas, cargaIntegracaoAvon.Situacao);

                        return;
                    }

                    Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica notaFiscal = notasFiscais[0];

                    if (primeiroCargaPedido.Recebedor == null && tipoEmissaoCTeParticipante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || tipoEmissaoCTeParticipante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                    {
                        primeiroCargaPedido.Recebedor = primeiroCargaPedido.Pedido.Destinatario;
                        primeiroCargaPedido.Pedido.Recebedor = primeiroCargaPedido.Pedido.Destinatario;
                    }

                    primeiroCargaPedido.Pedido.Remetente = ObterPessoa(notaFiscal.Emitente, unidadeDeTrabalho, connectionString);
                    primeiroCargaPedido.Pedido.Destinatario = ObterPessoa(notaFiscal.Destinatario, unidadeDeTrabalho, connectionString);

                    repPedido.Atualizar(primeiroCargaPedido.Pedido);
                    repCargaPedido.Atualizar(primeiroCargaPedido);

                    totalPedidos = (int)Math.Ceiling(totalNotasFiscais / quantidadeNotasPorPedido);

                    for (var i = totalPedidosExistentes; i < totalPedidos; i++)
                    {
                        unidadeDeTrabalho.Start();
                        int codigoCargaPedido = 0;
                        string retorno = Servicos.Embarcador.Carga.CargaPedido.CriarPedidoNormalOuSubcontratacao(DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, 0, 0, "", "", "", carga.Codigo, primeiroCargaPedido.Pedido.Remetente.CPF_CNPJ, primeiroCargaPedido.Pedido.Destinatario.CPF_CNPJ, primeiroCargaPedido.Recebedor?.CPF_CNPJ ?? 0d, "", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.Normal, unidadeDeTrabalho, connectionString, tipoServicoMultisoftware, configuracaoTMS, out codigoCargaPedido, 0, configuracaoGeralCarga);

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unidadeDeTrabalho.Rollback();

                            cargaIntegracaoAvon.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Problemas;
                            cargaIntegracaoAvon.Mensagem = retorno;

                            repCargaIntegracaoAvon.Atualizar(cargaIntegracaoAvon);

                            svcHubIntegracaoAvon.InformarMinutaAtualizada(codigoCarga, notasGeradas, cargaIntegracaoAvon.Situacao);
                        }
                        else
                        {
                            unidadeDeTrabalho.CommitChanges();
                        }
                    }
                }

                int indicePedidoAtual = 0;

                List<int> codigosCargaPedidos = repCargaPedido.BuscarCodigosPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigosCargaPedidos[indicePedidoAtual]);

                foreach (Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica notaFiscal in notasFiscais)
                {
                    if (tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual)
                    {
                        int indicePedido = (int)Math.Floor(notasGeradas / quantidadeNotasPorPedido);

                        if (indicePedidoAtual != indicePedido)
                        {
                            indicePedidoAtual = indicePedido;

                            cargaPedido = repCargaPedido.BuscarPorCodigo(codigosCargaPedidos[indicePedidoAtual]);
                        }
                    }

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

                    xmlNotaFiscal.Chave = notaFiscal.Chave;
                    xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal>();
                    xmlNotaFiscal.DataEmissao = notaFiscal.DataEmissao;
                    xmlNotaFiscal.Emitente = ObterPessoa(notaFiscal.Emitente, unidadeDeTrabalho, connectionString);
                    xmlNotaFiscal.Destinatario = ObterPessoa(notaFiscal.Destinatario, unidadeDeTrabalho, connectionString);
                    xmlNotaFiscal.Modelo = "55";
                    xmlNotaFiscal.Numero = notaFiscal.Numero;
                    xmlNotaFiscal.Peso = notaFiscal.Peso;
                    xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
                    xmlNotaFiscal.PesoLiquido = notaFiscal.Peso;
                    xmlNotaFiscal.Serie = notaFiscal.Serie.ToString();
                    xmlNotaFiscal.Volumes = notaFiscal.Quantidade;
                    xmlNotaFiscal.Valor = notaFiscal.Valor;
                    xmlNotaFiscal.XML = notaFiscal.Documento;
                    xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
                    xmlNotaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                    xmlNotaFiscal.CNPJTranposrtador = string.Empty;
                    xmlNotaFiscal.PlacaVeiculoNotaFiscal = string.Empty;
                    xmlNotaFiscal.Filial = carga.Filial;
                    xmlNotaFiscal.Empresa = carga.Empresa;
                    xmlNotaFiscal.nfAtiva = true;
                    xmlNotaFiscal.CanceladaPeloEmitente = repDocumentoDestinadoEmpresa.VerificarNotaCanceladaEmitente(xmlNotaFiscal.Chave);
                    xmlNotaFiscal.DataRecebimento = DateTime.Now;

                    repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();

                    pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                    pedidoXMLNotaFiscal.XMLNotaFiscal = xmlNotaFiscal;
                    pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                    repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                    notasGeradas++;

                    if ((notasGeradas % 20) == 0)
                    {
                        svcHubIntegracaoAvon.InformarMinutaAtualizada(codigoCarga, notasGeradas, cargaIntegracaoAvon.Situacao);

                        unidadeDeTrabalho.FlushAndClear();
                    }
                }

                cargaIntegracaoAvon.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Sucesso;

                repCargaIntegracaoAvon.Atualizar(cargaIntegracaoAvon);

                svcHubIntegracaoAvon.InformarMinutaAtualizada(codigoCarga, notasGeradas, cargaIntegracaoAvon.Situacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracaoAvon.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Problemas;
                cargaIntegracaoAvon.Mensagem = "Ocorreu uma falha ao salvar os documentos da minuta. Gerados " + notasGeradas.ToString() + " de " + cargaIntegracaoAvon.QuantidadeDocumentos.ToString() + " documentos.";

                repCargaIntegracaoAvon.Atualizar(cargaIntegracaoAvon);

                svcHubIntegracaoAvon.InformarMinutaAtualizada(codigoCarga, notasGeradas, cargaIntegracaoAvon.Situacao);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        private Dominio.Entidades.Cliente ObterPessoa(Dominio.ObjetosDeValor.CrossTalk.Pessoa pessoa, Repositorio.UnitOfWork unidadeDeTrabalho, string connectionString)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            double cpfCnpj;
            double.TryParse(Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ), out cpfCnpj);

            if (cpfCnpj <= 0d)
                return null;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente();
                cliente.DataCadastro = DateTime.Now;

                string tipoPessoa = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ).Length == 11 ? "F" : "J";

                cliente.Atividade = Servicos.Atividade.ObterAtividade(Empresa.Codigo, tipoPessoa, connectionString);
                cliente.Bairro = string.IsNullOrWhiteSpace(pessoa.Bairro) ? "Não informado" : pessoa.Bairro;
                cliente.CEP = pessoa.CEP;
                cliente.Complemento = pessoa.Complemento;
                cliente.CPF_CNPJ = cpfCnpj;
                cliente.Endereco = string.IsNullOrWhiteSpace(pessoa.Logradouro) ? "Não informado" : pessoa.Logradouro;
                cliente.IE_RG = pessoa.IE;
                cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(pessoa.CodigoMunicipio);
                cliente.Nome = pessoa.Nome;
                cliente.Numero = string.IsNullOrWhiteSpace(pessoa.Numero) ? "S/N" : pessoa.Numero;
                cliente.Telefone1 = pessoa.Fone;
                cliente.Tipo = tipoPessoa;

                if (cliente.Localidade == null)
                {
                    cliente.Localidade = repLocalidade.BuscarPorUFDescricao(pessoa.UF, pessoa.DescricaoMunicipio).FirstOrDefault();

                    if (cliente.Localidade == null)
                    {
                        Dominio.Entidades.Localidade localidade = new Dominio.Entidades.Localidade();

                        localidade.Codigo = repLocalidade.BuscarPorMaiorCodigo() + 1;
                        localidade.CodigoIBGE = pessoa.CodigoMunicipio;
                        localidade.Descricao = pessoa.DescricaoMunicipio;
                        localidade.DataAtualizacao = DateTime.Now;
                        localidade.Estado = new Dominio.Entidades.Estado() { Sigla = pessoa.UF };

                        repLocalidade.Inserir(localidade);

                        cliente.Localidade = localidade;
                    }
                }

                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                    if (grupoPessoas != null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                    }
                }
                cliente.Ativo = true;
                repCliente.Inserir(cliente);
            }

            return cliente;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon SalvarInformacoesIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon infoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoAvon repCargaIntegracaoAvon = new Repositorio.Embarcador.Cargas.CargaIntegracaoAvon(unidadeDeTrabalho);

            unidadeDeTrabalho.Start();

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon cargaIntegracaoAvon = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon();

            cargaIntegracaoAvon.Carga = carga;
            cargaIntegracaoAvon.DataConsulta = infoIntegracao.DataConsulta;
            cargaIntegracaoAvon.GUID = infoIntegracao.GUID;
            cargaIntegracaoAvon.Mensagem = infoIntegracao.Mensagem;
            cargaIntegracaoAvon.NumeroMinuta = infoIntegracao.NumeroMinuta;
            cargaIntegracaoAvon.Usuario = this.Usuario;
            cargaIntegracaoAvon.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(infoIntegracao.Requisicao, "xml", unidadeDeTrabalho);
            cargaIntegracaoAvon.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(infoIntegracao.Resposta, "xml", unidadeDeTrabalho);

            if (infoIntegracao.Documentos != null && infoIntegracao.Documentos.Count > 0)
            {
                cargaIntegracaoAvon.QuantidadeDocumentos = infoIntegracao.Documentos.Count;
                cargaIntegracaoAvon.PesoTotalDocumentos = (from obj in infoIntegracao.Documentos select obj.Peso).Sum();
                cargaIntegracaoAvon.ValorTotalDocumentos = (from obj in infoIntegracao.Documentos select obj.Valor).Sum();
                cargaIntegracaoAvon.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.SalvandoNotasFiscais;
            }
            else
            {
                cargaIntegracaoAvon.QuantidadeDocumentos = 0;
                cargaIntegracaoAvon.PesoTotalDocumentos = 0m;
                cargaIntegracaoAvon.ValorTotalDocumentos = 0m;
                cargaIntegracaoAvon.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon.Problemas;
            }

            repCargaIntegracaoAvon.Inserir(cargaIntegracaoAvon);

            unidadeDeTrabalho.CommitChanges();

            return cargaIntegracaoAvon;
        }

        #endregion
    }
}
