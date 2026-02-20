using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/ArquivoEBS")]
    public class ArquivoEBSController : BaseController
    {
		#region Construtores

		public ArquivoEBSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> DownloadEBSProduto()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime dataEntradaInicial, dataEntradaFinal, dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParse(Request.Params("DataEntradaInicial"), out dataEntradaInicial);
                DateTime.TryParse(Request.Params("DataEntradaFinal"), out dataEntradaFinal);
                DateTime.TryParse(Request.Params("DataEmissaoInicial"), out dataEmissaoInicial);
                DateTime.TryParse(Request.Params("DataEmissaoFinal"), out dataEmissaoFinal);

                int produtoEmEBS = 0;
                int.TryParse(Request.Params("ProdutoEmEBS"), out produtoEmEBS);

                int codigoFilial = 0;
                //int.TryParse(Request.Params("Filial"), out codigoFilial);

                List<int> modelosDocumento = JsonConvert.DeserializeObject<List<int>>(Request.Params("ModelosDocumento"));

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);

                List<Dominio.Entidades.LayoutEDI> layoutEBSProduto = repLayoutEDI.BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI.EBSProduto);

                if (layoutEBSProduto == null || layoutEBSProduto.Count == 0)
                    return new JsonpResult(false, "Integração de EBS/Produtos não encontrada.");

                string extensao = string.Empty;

                List<Dominio.Entidades.Produto> listaProdutos = repProduto.BuscarProdutosDocumentoEntrada(produtoEmEBS, modelosDocumento, codigoFilial, dataEntradaInicial, dataEntradaFinal, dataEmissaoInicial, dataEmissaoFinal);

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(codigoFilial > 0 ? repEmpresa.BuscarPorCodigo(codigoFilial) : this.Empresa, layoutEBSProduto.FirstOrDefault(), listaProdutos, unidadeDeTrabalho, _conexao.StringConexao, out extensao);

                unidadeDeTrabalho.Start();
                for (int i = 0; i < listaProdutos.Count; i++)
                {
                    listaProdutos[i].ProdutoEmEBS = true;
                    repProduto.Atualizar(listaProdutos[i]);
                }
                unidadeDeTrabalho.CommitChanges();
                string nomeArquivo = "Produtos.txt";

                return Arquivo(edi, extensao == ".zip" ? "application/zip" : "plain/text", nomeArquivo);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadEBSNotaEntrada()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataEntradaInicial, dataEntradaFinal, dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParse(Request.Params("DataEntradaInicial"), out dataEntradaInicial);
                DateTime.TryParse(Request.Params("DataEntradaFinal"), out dataEntradaFinal);
                DateTime.TryParse(Request.Params("DataEmissaoInicial"), out dataEmissaoInicial);
                DateTime.TryParse(Request.Params("DataEmissaoFinal"), out dataEmissaoFinal);

                int codigoFilial = 0;
                int.TryParse(Request.Params("Filial"), out codigoFilial);

                int documentoEmEBS = 0;
                int.TryParse(Request.Params("DocumentoEmEBS"), out documentoEmEBS);

                List<int> modelosDocumento = JsonConvert.DeserializeObject<List<int>>(Request.Params("ModelosDocumento"));

                if (codigoFilial == 0)
                    return new JsonpResult(false, "Favor informe uma filial para gerar o arquivo de nota de entrada.");

                bool.TryParse(Request.Params("SelecionarTodos"), out bool selecionarTodos);

                List<int> codigosNotas = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaNotas"));

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoFilial);

                List<Dominio.Entidades.LayoutEDI> layoutEBSNotaEntrada = repLayoutEDI.BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI.EBSNotaEntrada);

                if (layoutEBSNotaEntrada == null || layoutEBSNotaEntrada.Count == 0)
                    return new JsonpResult(false, "Integração de EBS de Nota de Entrada não encontrada.");

                string extensao = string.Empty;

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaNotaEntrada = repDocumentoEntradaTMS.BuscarDocumentoEntrada(documentoEmEBS, codigosNotas, selecionarTodos, modelosDocumento, codigoFilial, dataEntradaInicial, dataEntradaFinal, dataEmissaoInicial, dataEmissaoFinal);
                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(empresa, layoutEBSNotaEntrada.FirstOrDefault(), listaNotaEntrada, unidadeDeTrabalho, _conexao.StringConexao, out extensao);

                //rever isso aqui, pode travar todo o sistema e ficar muito lento com o passar do tempo (mais de 200 registros já fica lento)
                unidadeDeTrabalho.Start();
                for (int i = 0; i < listaNotaEntrada.Count; i++)
                {
                    listaNotaEntrada[i].DocumentoEmEBS = true;
                    repDocumentoEntradaTMS.Atualizar(listaNotaEntrada[i]);
                }
                unidadeDeTrabalho.CommitChanges();

                string nomeArquivo = "NotaEntrada.txt";

                return Arquivo(edi, extensao == ".zip" ? "application/zip" : "plain/text", nomeArquivo);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadEBSBaixas()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeTrabalho);

                Dominio.Entidades.LayoutEDI layoutEBSBaixas = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.EBSBaixas);

                if (layoutEBSBaixas == null)
                    return new JsonpResult(false, "Layout para geração de arquivo EBS de baixas não encontrado.");

                Dominio.ObjetosDeValor.EDI.EBS.BaixaFinanceiro ebs = Servicos.Embarcador.Integracao.EDI.EBSBaixaFinanceiro.GerarEBS(dataInicial, dataFinal, unidadeTrabalho, _conexao.StringConexao);

                Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unidadeTrabalho, layoutEBSBaixas, this.Empresa);

                System.IO.MemoryStream arquivo = serGeracaoEDI.GerarArquivoRecursivo(ebs);

                return Arquivo(arquivo, "plain/text", "EBS Baixas " + dataInicial.ToString("dd-MM") + " à " + dataFinal.ToString("dd-MM-yy") + ".txt");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadEBSComissaoMotorista()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("ComissaoMotorista"), out int codigoComissaoMotorista);
                int.TryParse(Request.Params("CodigoEvento"), out int codigoEvento);

                Enum.TryParse(Request.Params("Tipo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEBSComissao tipo);
                Enum.TryParse(Request.Params("TipoSistemaContabil"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistemaContabil tipoSistemaContabil);

                Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unidadeTrabalho);

                Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigoComissaoMotorista);

                if (comissaoFuncionario == null)
                    return new JsonpResult(false, true, "Comissão de motorista não encontrada.");

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeTrabalho);

                Dominio.Entidades.LayoutEDI layoutEDI = null;

                if (tipoSistemaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistemaContabil.EBS)
                {
                    layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.EBSComissaoMotorista);

                    if (layoutEDI == null)
                        return new JsonpResult(false, "Layout para geração de arquivo EBS de comissões de motoristas não encontrado.");
                }

                if (tipoSistemaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistemaContabil.Questor)
                {
                    layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.QuestorComissaoMotorista);

                    if (layoutEDI == null)
                        return new JsonpResult(false, "Layout para geração de arquivo Questor de comissões de motoristas não encontrado.");
                }

                Dominio.ObjetosDeValor.EDI.EBS.ComissaoMotorista ebs = Servicos.Embarcador.Integracao.EDI.EBSComissaoMotorista.GerarEBS(comissaoFuncionario, codigoEvento, tipo, unidadeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unidadeTrabalho, layoutEDI, this.Empresa);

                System.IO.MemoryStream arquivo = serGeracaoEDI.GerarArquivoRecursivo(ebs);

                if (tipoSistemaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistemaContabil.Questor)
                    return Arquivo(arquivo, "text/csv", "FOLHA.CSV");
                else
                    return Arquivo(arquivo, "plain/text", "FOLHA.TXT");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNotasEntrada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada filtrosPesquisa = ObterFiltrosPesquisaNotasEntrada();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data da Emissão", "DataEmissao", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data do Lançamento", "DataEntrada", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Destinatario", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Bruto", "ValorBruto", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Valor Líquido", "ValorTotal", 9, Models.Grid.Align.right, true);

                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaDocumentoEntradaTMS = repDocumentoEntradaTMS.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repDocumentoEntradaTMS.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaDocumentoEntradaTMS
                             select new
                             {
                                 p.Codigo,
                                 Numero = p.Numero.ToString("n0"),
                                 DataEmissao = p.DataEmissao.ToString("dd/MM/yyyy"),
                                 DataEntrada = p.DataEntrada.ToString("dd/MM/yyyy"),
                                 Fornecedor = p.Fornecedor != null ? p.Fornecedor.Nome : string.Empty,
                                 Destinatario = p.Destinatario != null ? p.Destinatario.RazaoSocial + " (" + p.Destinatario.CNPJ_Formatado + ")" : string.Empty,
                                 ValorBruto = p.ValorBruto.ToString("n2"),
                                 ValorTotal = p.ValorTotal.ToString("n2")
                             }).ToList();

                grid.AdicionaRows(lista);
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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada ObterFiltrosPesquisaNotasEntrada()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoEntrada()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataEntradaInicial = Request.GetDateTimeParam("DataEntradaInicial"),
                DataEntradaFinal = Request.GetDateTimeParam("DataEntradaFinal"),
                CodigoDestinatario = Request.GetIntParam("Filial"),
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado,
                TipoDocumentoEmEBS = Request.GetIntParam("DocumentoEmEBS"),
                ModelosDocumento = Request.GetListParam<int>("ModelosDocumento")
            };
        }

        #endregion
    }
}
