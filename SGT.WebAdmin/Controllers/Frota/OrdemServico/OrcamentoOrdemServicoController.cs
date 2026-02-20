using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    public class OrcamentoOrdemServicoController : BaseController
    {
		#region Construtores

		public OrcamentoOrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        private static string CaminhoImagens = "Imagens Orçamento Frota";

        #region Métodos Públicos

        public async Task<IActionResult> ObterDetalhesGeraisOrcamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento orcamento = repOrcamento.BuscarPorOrdemServico(codigo);

                if (orcamento == null)
                    return new JsonpResult(false, true, "Orçamento não encontrado.");

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> servicosOrcamento = repOrcamentoServico.BuscarPorOrcamento(orcamento.Codigo);

                var retorno = new
                {
                    Orcamento = ObterDetalhesOrcamento(orcamento, unidadeTrabalho),
                    Servicos = (from obj in servicosOrcamento
                                select new
                                {
                                    obj.Codigo,
                                    Descricao = obj.Manutencao.Servico.Descricao
                                }).ToList()

                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados do orçamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarServicoPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repServicoOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = repServicoOrcamento.BuscarPorCodigo(codigo);

                if (orcamentoServico == null)
                    return new JsonpResult(false, true, "Orçamento não encontrado para este serviço.");

                return new JsonpResult(ObterDetalhesServico(orcamentoServico, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados do orçamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarServico()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                decimal.TryParse(Request.Params("ValorProdutos"), out decimal valorProdutos);
                decimal.TryParse(Request.Params("ValorMaoObra"), out decimal valorMaoObra);

                string observacao = Request.Params("Observacao");
                string orcadoPor = Request.Params("OrcadoPor");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = repOrcamentoServico.BuscarPorCodigo(codigo);

                if (orcamentoServico == null)
                    return new JsonpResult(false, true, "Orçamento não encontrado para este serviço.");

                unidadeTrabalho.Start();

                orcamentoServico.Observacao = observacao;
                orcamentoServico.OrcadoPor = orcadoPor;
                orcamentoServico.ValorMaoObra = valorMaoObra;
                orcamentoServico.ValorProdutos = valorProdutos;

                repOrcamentoServico.Atualizar(orcamentoServico);

                Servicos.Embarcador.Frota.OrdemServicoOrcamento.AtualizarValoresOrcamento(orcamentoServico.Orcamento, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(ObterDetalhesOrcamento(orcamentoServico.Orcamento, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao atualizar o orçamento do serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarOrcamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo, parcelas;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("Parcelas"), out parcelas);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento orcamento = repOrcamento.BuscarPorCodigo(codigo);

                if (orcamento == null)
                    return new JsonpResult(false, true, "Orçamento não encontrado.");

                orcamento.Observacao = observacao;
                orcamento.Parcelas = parcelas;

                repOrcamento.Atualizar(orcamento);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao atualizar o orçamento do serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AnexarImagemOrcamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = repOrcamentoServico.BuscarPorCodigo(codigo);

                if (orcamentoServico == null)
                    return new JsonpResult(false, true, "Orçamento não encontrado para este serviço.");

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, true, "Imagem não encontrada, favor verifique se a imagem está no formato correto.");

                Servicos.DTO.CustomFile file = files[0];

                string extensao = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                string[] extensoesValidas = new string[] { ".jpg", ".png", ".bmp" };

                if (!extensoesValidas.Contains(extensao))
                    return new JsonpResult(false, true, "A extensão " + extensao + " não é suportada pelo sistema. Extensões suportadas: " + string.Join(", ", extensoesValidas) + ".");

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos, CaminhoImagens);
                string nomeArquivo = orcamentoServico.Codigo.ToString() + extensao;

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                file.SaveAs(caminho);

                orcamentoServico.Imagem = nomeArquivo;

                repOrcamentoServico.Atualizar(orcamentoServico);

                return new JsonpResult(ObterDetalhesServico(orcamentoServico, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao anexar a imagem ao orçamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> RemoverImagemOrcamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = repOrcamentoServico.BuscarPorCodigo(codigo);

                if (orcamentoServico == null)
                    return new JsonpResult(false, true, "Orçamento não encontrado para este serviço.");

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos, CaminhoImagens, orcamentoServico.Imagem);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                orcamentoServico.Imagem = null;

                repOrcamentoServico.Atualizar(orcamentoServico);

                return new JsonpResult(ObterDetalhesServico(orcamentoServico, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao remover a imagem ao orçamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterVisualizacaoImagemOrcamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = repOrcamentoServico.BuscarPorCodigo(codigo);

                if (orcamentoServico == null)
                    return new JsonpResult(false, true, "Orçamento não encontrado para este serviço.");

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos, CaminhoImagens, orcamentoServico.Imagem);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    return new JsonpResult(false, true, "O arquivo não existe mais em nossos servidores.");

                string extensao = System.IO.Path.GetExtension(orcamentoServico.Imagem).ToLowerInvariant();
                string preappend = string.Empty;

                if (extensao == ".jpg")
                    preappend = "data:image/jpeg;base64,";
                else
                    preappend = "data:image/" + extensao.Replace(".", "") + ";base64,";

                return new JsonpResult(preappend + Convert.ToBase64String(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho)));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao anexar a imagem ao orçamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadImagemOrcamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = repOrcamentoServico.BuscarPorCodigo(codigo);

                if (orcamentoServico == null)
                    return new JsonpResult(false, true, "Orçamento não encontrado para este serviço.");

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos, CaminhoImagens, orcamentoServico.Imagem);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    return new JsonpResult(false, true, "O arquivo não existe mais em nossos servidores.");

                string extensao = System.IO.Path.GetExtension(orcamentoServico.Imagem).ToLowerInvariant();
                string mimeType = string.Empty;

                if (extensao == ".jpg")
                    mimeType = "image/jpeg";
                else
                    mimeType = "image/" + extensao.Replace(".", "");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), mimeType, "Imagem Orçamento Serviço" + extensao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao anexar a imagem ao orçamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarProdutos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Produto", "Produto", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 15, Models.Grid.Align.right, true, false, true);
                grid.AdicionarCabecalho("UN", "UnidadeMedida", 10, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho("Valor Unit.", "Valor", 15, Models.Grid.Align.right, true, true, true);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 15, Models.Grid.Align.right, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Produto")
                    propOrdena += ".Descricao";

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);

                int countOrcamentoServicoProduto = repOrcamentoServicoProduto.ContarConsulta(codigo);

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto> listaOrcamentoServicoProduto = new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto>();

                if (countOrcamentoServicoProduto > 0)
                    listaOrcamentoServicoProduto = repOrcamentoServicoProduto.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countOrcamentoServicoProduto);

                grid.AdicionaRows((from obj in listaOrcamentoServicoProduto
                                   select new
                                   {
                                       obj.Codigo,
                                       Produto = obj.Produto.Descricao,
                                       Quantidade = obj.Quantidade.ToString("n2"),
                                       UnidadeMedida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(obj.Produto.UnidadeDeMedida),
                                       Valor = obj.Valor.ToString("n2"),
                                       ValorTotal = obj.ValorTotal.ToString("n2")
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarProduto()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Produto"), out int codigoProduto);
                int.TryParse(Request.Params("OrcamentoServico"), out int codigoOrcamentoServico);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);
                decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);

                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto orcamentoServicoProduto = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto();
                orcamentoServicoProduto.Autorizado = true;
                orcamentoServicoProduto.Garantia = false;
                orcamentoServicoProduto.OrcamentoServico = repOrcamentoServico.BuscarPorCodigo(codigoOrcamentoServico);
                orcamentoServicoProduto.Produto = repProduto.BuscarPorCodigo(codigoProduto);
                orcamentoServicoProduto.Quantidade = quantidade;
                orcamentoServicoProduto.Valor = valor;

                repOrcamentoServicoProduto.Inserir(orcamentoServicoProduto);

                decimal totalProdutos = repOrcamentoServicoProduto.BuscarValorTotalProdutosPorOrcamentoServico(codigoOrcamentoServico);
                return new JsonpResult(new { TotalizadorProdutos = totalProdutos.ToString("n2") });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar o produto.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarProduto()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Produto"), out int codigoProduto);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);
                decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);

                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto orcamentoServicoProduto = repOrcamentoServicoProduto.BuscarPorCodigo(codigo);
                orcamentoServicoProduto.Autorizado = true;
                orcamentoServicoProduto.Garantia = false;
                orcamentoServicoProduto.Produto = repProduto.BuscarPorCodigo(codigoProduto);
                orcamentoServicoProduto.Quantidade = quantidade;
                orcamentoServicoProduto.Valor = valor;

                repOrcamentoServicoProduto.Atualizar(orcamentoServicoProduto);

                decimal totalProdutos = repOrcamentoServicoProduto.BuscarValorTotalProdutosPorOrcamentoServico(orcamentoServicoProduto.OrcamentoServico.Codigo);
                return new JsonpResult(new { TotalizadorProdutos = totalProdutos.ToString("n2") });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao atualizar o produto.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirProduto()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto orcamentoServicoProduto = repOrcamentoServicoProduto.BuscarPorCodigo(codigo);

                int codigoOrcamentoServico = orcamentoServicoProduto.OrcamentoServico.Codigo;

                repOrcamentoServicoProduto.Deletar(orcamentoServicoProduto);

                decimal totalProdutos = repOrcamentoServicoProduto.BuscarValorTotalProdutosPorOrcamentoServico(codigoOrcamentoServico);
                return new JsonpResult(new { TotalizadorProdutos = totalProdutos.ToString("n2") });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao excluir o produto.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProdutoPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto orcamentoServicoProduto = repOrcamentoServicoProduto.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    orcamentoServicoProduto.Codigo,
                    orcamentoServicoProduto.Autorizado,
                    orcamentoServicoProduto.Garantia,
                    Produto = new
                    {
                        orcamentoServicoProduto.Produto.Descricao,
                        orcamentoServicoProduto.Produto.Codigo
                    },
                    Quantidade = orcamentoServicoProduto.Quantidade.ToString("n2"),
                    Valor = orcamentoServicoProduto.Valor.ToString("n2"),
                    ValorTotal = orcamentoServicoProduto.ValorTotal.ToString("n2")
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados do produto.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private object ObterDetalhesOrcamento(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento orcamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);
            decimal totalProdutosDosServicos = repOrcamentoServicoProduto.BuscarValorTotalProdutosPorOrcamento(orcamento.Codigo);

            return new
            {
                orcamento.Codigo,
                orcamento.Observacao,
                orcamento.Parcelas,
                ValorTotalMaoObra = orcamento.ValorTotalMaoObra.ToString("n2"),
                ValorTotalOrcado = orcamento.ValorTotalOrcado.ToString("n2"),
                ValorTotalPreAprovado = orcamento.ValorTotalPreAprovado.ToString("n2"),
                ValorTotalProdutos = orcamento.ValorTotalProdutos.ToString("n2"),
                ValorTotalListaProdutosDosServicos = totalProdutosDosServicos.ToString("n2")
            };
        }

        public object ObterDetalhesServico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico servicoOrcamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);
            decimal totalProdutos = repOrcamentoServicoProduto.BuscarValorTotalProdutosPorOrcamentoServico(servicoOrcamento.Codigo);

            bool possuiImagem = false;
            if (!string.IsNullOrWhiteSpace(servicoOrcamento.Imagem))
            {
                string caminhoImagem = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos, CaminhoImagens, servicoOrcamento.Imagem);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoImagem))
                    possuiImagem = true;
            }

            return new
            {
                servicoOrcamento.Codigo,
                servicoOrcamento.Observacao,
                servicoOrcamento.OrcadoPor,
                servicoOrcamento.ValorMaoObra,
                servicoOrcamento.ValorProdutos,
                PossuiImagem = possuiImagem,
                TotalizadorProdutos = totalProdutos.ToString("n2")
            };
        }

        #endregion
    }
}
