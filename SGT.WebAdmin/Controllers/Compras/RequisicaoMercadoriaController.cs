using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Compras
{
    [CustomAuthorize(new string[] { "PesquisaAutorizacoes", "DetalhesAutorizacao" }, "Compras/RequisicaoMercadoria")]
    public class RequisicaoMercadoriaController : BaseController
    {
		#region Construtores

		public RequisicaoMercadoriaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMercadorias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Compras.Mercadoria repMercadoria = new Repositorio.Embarcador.Compras.Mercadoria(unitOfWork);
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaMercadoria();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dadosu
                int.TryParse(Request.Params("Codigo"), out int requisicao);
                int totalRegistros = repMercadoria.ContarConsultaPorRequisicao(requisicao);

                List<Dominio.Entidades.Embarcador.Compras.Mercadoria> listaGrid = repMercadoria.ConsultarPorRequisicao(requisicao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
                                Produto = obj.ProdutoEstoque.Produto.Descricao,
                                Quantidade = obj.Quantidade.ToString("n2"),
                                EstoqueAtual = obj.ProdutoEstoque.Quantidade.ToString("n4"),
                                CustoUnitario = obj.CustoUnitario.ToString("n2"),
                                CustoTotal = obj.CustoTotal.ToString("n2"),
                                Unidade = obj.ProdutoEstoque?.Produto?.UnidadeMedida?.Sigla ?? string.Empty
                            };

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                // Busca
                List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> listaAutorizacao = repAprovacaoAlcadaRequisicaoMercadoria.ConsultarAutorizacoesPorRequisicao(codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repAprovacaoAlcadaRequisicaoMercadoria.ContarConsultaAutorizacoesPorRequisicao(codigo);

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario?.Nome,
                                 Regra = obj.RegraRequisicaoMercadoria.Descricao,
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 DT_RowColor = CorAprovacao(obj.Situacao)
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao = repRequisicaoMercadoria.BuscarPorCodigo(codigo);

                // Valida
                if (requisicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    requisicao.Codigo,
                    requisicao.Situacao,
                    requisicao.Numero,
                    Colaborador = requisicao.Usuario.Nome,
                    Filial = new { requisicao.Filial.Codigo, Descricao = requisicao.Filial.RazaoSocial },
                    Motivo = new { requisicao.MotivoCompra.Codigo, requisicao.MotivoCompra.Descricao, requisicao.MotivoCompra.ExigeInformarVeiculoObrigatoriamente },
                    Veiculo = new { requisicao.Veiculo?.Codigo, requisicao.Veiculo?.Descricao },
                    Data = requisicao.Data.ToString("dd/MM/yyyy"),
                    requisicao.Observacao,
                    FuncionarioRequisitado = requisicao.FuncionarioRequisitado != null ? new { requisicao.FuncionarioRequisitado.Codigo, Descricao = requisicao.FuncionarioRequisitado.Nome } : null,
                    Resumo = ResumoAutorizacao(requisicao, unitOfWork),

                    Produtos = (from p in requisicao.Mercadorias
                                select new
                                {
                                    p.ProdutoEstoque?.Produto?.Codigo,
                                    Produto = new { p.ProdutoEstoque?.Produto?.Codigo, p.ProdutoEstoque?.Produto?.Descricao },
                                    Quantidade = p.Quantidade.ToString("n2"),
                                    EstoqueAtual = p.ProdutoEstoque?.Quantidade.ToString("n4"),
                                    CustoUnitario = p.CustoUnitario.ToString("n2"),
                                    CustoTotal = p.CustoTotal.ToString("n2"),
                                    Unidade = UnidadeDeMedidaHelper.ObterSigla(p.ProdutoEstoque?.Produto?.UnidadeDeMedida ?? null),
                                    LocalArmazenamento = new { p.ProdutoEstoque?.Produto.LocalArmazenamentoProduto?.Codigo, p.ProdutoEstoque?.Produto?.Descricao }
                                }).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria autorizacao = repAprovacaoAlcadaRequisicaoMercadoria.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Delegada ? "(Delegada)" : autorizacao.RegraRequisicaoMercadoria.Descricao,
                    Situacao = autorizacao.DescricaoSituacao,
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,

                    PodeAprovar = autorizacao.Usuario != null && autorizacao.Usuario.Codigo == this.Usuario.Codigo && autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,

                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = !string.IsNullOrWhiteSpace(autorizacao.Motivo) ? autorizacao.Motivo : string.Empty,
                };

                return new JsonpResult(retorno);
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
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
                // Busca informacoes
                Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao = new Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria();

                // Preenche entidade com dados
                PreencheEntidade(ref requisicao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(requisicao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                unitOfWork.Start();
                repRequisicaoMercadoria.Inserir(requisicao, Auditado);
                SalvarMercadorias(requisicao, unitOfWork, Auditado, null);

                Servicos.Embarcador.Compras.RequisicaoMercadoria.EtapaAprovacao(ref requisicao, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (SemEstoqueException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao = repRequisicaoMercadoria.BuscarPorCodigo(codigo, true);

                // Valida
                if (requisicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (requisicao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.SemRegra)
                    return new JsonpResult(false, true, "A situação não permite essa operação.");

                // Busca as regras
                Servicos.Embarcador.Compras.RequisicaoMercadoria.EtapaAprovacao(ref requisicao, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, Auditado);
                repRequisicaoMercadoria.Atualizar(requisicao);
                unitOfWork.CommitChanges();

                bool possuiRegra = requisicao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.SemRegra;

                // Retorna sucesso
                return new JsonpResult(possuiRegra);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao = repRequisicaoMercadoria.BuscarPorCodigo(codigo, true);

                // Valida
                if (requisicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                SalvarMercadorias(requisicao, unitOfWork, Auditado, null);

                // Persiste dados
                unitOfWork.Start();
                repRequisicaoMercadoria.Atualizar(requisicao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (SemEstoqueException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os produtos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao = repRequisicaoMercadoria.BuscarPorCodigo(codigo);

                // Valida
                if (requisicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                unitOfWork.Start();
                repRequisicaoMercadoria.Deletar(requisicao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BaixarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));
                string nomeEmpresa = Cliente.NomeFantasia;
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R153_RequisicaoMercadoria, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R153_RequisicaoMercadoria, TipoServicoMultisoftware, "Relatório de Requisição de Mercadoria", "Compras", "RequisicaoMercadoria.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Compras.RequisicaoMercadoria> dadosRequisicaoMercadoria = repRequisicaoMercadoria.RelatorioRequisicaoMercadoria(codigo);
                if (dadosRequisicaoMercadoria.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioRequisicaoMercadoria(codigo, stringConexao, nomeEmpresa, relatorioControleGeracao, dadosRequisicaoMercadoria));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de requisições de mercadoria para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("Filial").Nome("Filial").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("FuncionarioRequisitado").Nome("Funcionário Requisitado").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("MotivoCompra").Nome("Motivo").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Colaborador").Nome("Colaborador").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Data").Nome("Data").Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Veiculo").Nome("Veículo").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Descricao");

            return grid;
        }

        private Models.Grid.Grid GridPesquisaMercadoria()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Produto").Nome("Produto").Tamanho(35).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("Quantidade").Nome("Quantidade").Tamanho(17).Align(Models.Grid.Align.right);
            grid.Prop("Unidade").Nome("Unidade").Tamanho(17).Align(Models.Grid.Align.right);
            grid.Prop("CustoUnitario").Nome("Custo Unitário").Tamanho(17).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("CustoTotal").Nome("Custo Total").Tamanho(17).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("EstoqueAtual").Nome("Estoque Atual").Tamanho(17).Align(Models.Grid.Align.right).Ord(false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRequisicaoMercadoria filtrosPesquisa = ObterFiltrosPesquisa();

            List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> listaGrid = repRequisicaoMercadoria.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRequisicaoMercadoria.ContarConsulta(filtrosPesquisa);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Numero,
                            FuncionarioRequisitado = obj.FuncionarioRequisitado?.Nome ?? string.Empty,
                            Filial = obj.Filial.RazaoSocial,
                            MotivoCompra = obj.MotivoCompra.Descricao,
                            Colaborador = obj.Usuario?.Nome ?? "",
                            Data = obj.Data.ToString("dd/MM/yyyy"),
                            Situacao = obj.DescricaoSituacao,
                            Veiculo = obj.Veiculo?.Placa,
                            obj.Descricao
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);
            Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("Motivo"), out int motivoCompra);
            int.TryParse(Request.Params("Veiculo"), out int veiculo);
            int.TryParse(Request.Params("FuncionarioRequisitado"), out int funcionarioRequisitado);

            DateTime.TryParse(Request.Params("Data"), out DateTime data);

            string observacao = Request.Params("Observacao") ?? string.Empty;

            // Vincula dados
            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;
            requisicao.Numero = repRequisicaoMercadoria.BuscarProximoNumero(codigoEmpresa);
            requisicao.Usuario = this.Usuario;
            requisicao.Filial = repEmpresa.BuscarPorCodigo(filial);
            requisicao.MotivoCompra = repMotivoCompra.BuscarPorCodigo(motivoCompra);
            requisicao.Veiculo = repVeiculo.BuscarPorCodigo(veiculo);
            requisicao.Data = data;
            requisicao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.AgAprovacao;
            requisicao.Modo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Requisicao;
            requisicao.Observacao = observacao;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                requisicao.Filial = this.Usuario.Empresa;
            if (funcionarioRequisitado > 0)
                requisicao.FuncionarioRequisitado = repUsuario.BuscarPorCodigo(funcionarioRequisitado);
            else
                requisicao.FuncionarioRequisitado = null;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, out string msgErro)
        {
            msgErro = "";

            if (requisicao.Filial == null)
            {
                msgErro = "Filial é obrigatória.";
                return false;
            }

            if (requisicao.MotivoCompra == null)
            {
                msgErro = "Motivo é obrigatória.";
                return false;
            }

            if (requisicao.Data == DateTime.MinValue)
            {
                msgErro = "Data é obrigatória.";
                return false;
            }

            if (requisicao.MotivoCompra.ExigeInformarVeiculoObrigatoriamente && requisicao.Veiculo == null)
            {
                msgErro = "Motivo de Compra exige que um veículo seja informado.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Filial") propOrdenar = "Filial.Descricao";
            else if (propOrdenar == "MotivoCompra") propOrdenar = "MotivoCompra.Descricao";
        }

        private void SalvarMercadorias(Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Compras.Mercadoria repMercadoria = new Repositorio.Embarcador.Compras.Mercadoria(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

            List<dynamic> mercadoriasRequisicao = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Produtos"));
            if (mercadoriasRequisicao == null) return;

            List<int> codigosMercadorias = new List<int>();
            foreach (dynamic codigo in mercadoriasRequisicao)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
            }
            codigosMercadorias = codigosMercadorias.Where(o => o > 0).Distinct().ToList();

            List<int> codigosExcluir = repMercadoria.BuscarNaoPesentesNaLista(requisicao.Codigo, codigosMercadorias);

            int codigoEmpresa = requisicao.Filial?.Codigo ?? 0;

            foreach (dynamic dynMercadoria in mercadoriasRequisicao)
            {
                int.TryParse((string)dynMercadoria.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Compras.Mercadoria mercadoria = repMercadoria.BuscarPorRequisicaoEMercadoria(requisicao.Codigo, codigo);

                if (mercadoria == null)
                    mercadoria = new Dominio.Entidades.Embarcador.Compras.Mercadoria();
                else mercadoria.Initialize();

                decimal quantidade = Utilidades.Decimal.Converter((string)dynMercadoria.Quantidade);
                decimal custoUnitario = Utilidades.Decimal.Converter((string)dynMercadoria.CustoUnitario);

                mercadoria.RequisicaoMercadoria = requisicao;

                int.TryParse((string)dynMercadoria.Produto.Codigo, out int codigoProduto);
                int.TryParse((string)dynMercadoria.LocalArmazenamento.Codigo, out int codigoLocalArmazenamento);

                bool produtoSemEstoque = dynMercadoria.EstoqueAtual == null;
                if (produtoSemEstoque)
                {
                    Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(codigoProduto);
                    Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;

                    mercadoria.ProdutoEstoque = repProdutoEstoque.BuscarPorProduto(codigoProduto, codigoEmpresa, codigoLocalArmazenamento);
                    if (mercadoria.ProdutoEstoque == null)
                        mercadoria.ProdutoEstoque = servicoEstoque.AdicionarEstoque(produto, requisicao.Filial, TipoServicoMultisoftware, ConfiguracaoEmbarcador, localArmazenamento);
                    else if ((mercadoria.ProdutoEstoque.Empresa == null && codigoEmpresa > 0) || (mercadoria.ProdutoEstoque.LocalArmazenamento == null && codigoLocalArmazenamento > 0))
                    {
                        if (mercadoria.ProdutoEstoque.Empresa == null && codigoEmpresa > 0)
                            mercadoria.ProdutoEstoque.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                        if (mercadoria.ProdutoEstoque.LocalArmazenamento == null && codigoLocalArmazenamento > 0)
                            mercadoria.ProdutoEstoque.LocalArmazenamento = repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento);

                        repProdutoEstoque.Atualizar(mercadoria.ProdutoEstoque);
                    }
                }
                else if (ConfiguracaoEmbarcador.UtilizaMultiplosLocaisArmazenamento)
                    mercadoria.ProdutoEstoque = repProdutoEstoque.BuscarPorProduto(codigoProduto, codigoEmpresa, codigoLocalArmazenamento);
                else
                {
                    mercadoria.ProdutoEstoque = repProdutoEstoque.BuscarPorProduto(codigoProduto, codigoEmpresa);
                    if (mercadoria.ProdutoEstoque == null)
                        mercadoria.ProdutoEstoque = repProdutoEstoque.BuscarPorProduto(codigoProduto);
                }

                mercadoria.Quantidade = quantidade;
                mercadoria.CustoUnitario = custoUnitario;
                mercadoria.Modo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Requisicao;
                mercadoria.LocalArmazenamento = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;

                if (mercadoria.ProdutoEstoque != null)
                {
                    
                    var formas =new  List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria> { 
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria.GerarPeloEstoque,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria.Estoque
                    };

                    var categoriasProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto> {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.Servicos
                    };

                    //Verifica se foi selecionado bloquear estoque negativo e se motivo da compra é "Gerar pelo estoque"
                    if (ConfiguracaoEmbarcador.ControlarEstoqueNegativo && requisicao.MotivoCompra != null && formas.Contains(requisicao.MotivoCompra.Forma) && !categoriasProdutos.Contains(mercadoria.ProdutoEstoque.Produto.CategoriaProduto)) 
                    {
                        //Soma produtos com o mesmo código no carrinho
                        decimal somaProdutosRepetidos = mercadoriasRequisicao.Where(x => x.Produto.Codigo == codigoProduto).Aggregate(0m, (quantidadeTotal, itemAtual) => quantidadeTotal + (Utilidades.Decimal.Converter((string)itemAtual.Quantidade)));
                        //Verifica se o produto tem estoque suficiente para finalizar a compra
                        if (mercadoria.ProdutoEstoque.Quantidade < somaProdutosRepetidos)
                        {
                            throw new SemEstoqueException("Produto " + mercadoria.ProdutoEstoque.Produto.Descricao + " indisponível em estoque, quantidade disponíveis: " + Convert.ToInt32(mercadoria.ProdutoEstoque.Quantidade) + ".");
                        }
                    }
                    if (mercadoria.Codigo == 0)
                        repMercadoria.Inserir(mercadoria, auditado, historicoPai);
                    else
                        repMercadoria.Atualizar(mercadoria, auditado, historicoPai);
                }
            }
            foreach (int excluir in codigosExcluir)
            {
                Dominio.Entidades.Embarcador.Compras.Mercadoria objParExcluir = repMercadoria.BuscarPorRequisicaoEMercadoria(requisicao.Codigo, excluir);
                if (objParExcluir != null) repMercadoria.Deletar(objParExcluir, auditado, historicoPai);
            }
        }

        private dynamic ResumoAutorizacao(Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

            int aprovacoesNecessarias = repAprovacaoAlcadaRequisicaoMercadoria.ContarAprovacoesNecessarias(requisicao.Codigo);
            int aprovacoes = repAprovacaoAlcadaRequisicaoMercadoria.ContarAprovacoes(requisicao.Codigo);
            int reprovacoes = repAprovacaoAlcadaRequisicaoMercadoria.ContarReprovacoes(requisicao.Codigo);

            return new
            {
                Solicitante = requisicao.Usuario?.Nome ?? string.Empty,
                DataSolicitacao = requisicao.DataAlteracao?.ToString("dd/MM/yyyy") ?? string.Empty,
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = requisicao.DescricaoSituacao,
            };
        }

        private string CorAprovacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

            return "";
        }

        private void GerarRelatorioRequisicaoMercadoria(int codigoRequisicaoMercadoria, string stringConexao, string nomeEmpresa, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Compras.RequisicaoMercadoria> dadosRequisicaoMercadoria)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                var result = ReportRequest.WithType(ReportType.RequisicaoMercadoria)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("CodigoRequisicaoMercadoria", codigoRequisicaoMercadoria.ToString())
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("DadosRequisicaoMercadoria", dadosRequisicaoMercadoria.ToJson())
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRequisicaoMercadoria ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRequisicaoMercadoria()
            {
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                Modo = Request.GetNullableEnumParam<ModoRequisicaoMercadoria>("Modo"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                Filial = Request.GetIntParam("Filial"),
                Motivo = Request.GetIntParam("Motivo"),
                Situacao = Request.GetNullableEnumParam<SituacaoRequisicaoMercadoria>("Situacao"),
                FuncionarioRequisitado = Request.GetIntParam("FuncionarioRequisitado"),
                Numero = Request.GetIntParam("Numero"),
                Veiculo = Request.GetIntParam("Veiculo")
            };
        }

        #endregion
    }
}

