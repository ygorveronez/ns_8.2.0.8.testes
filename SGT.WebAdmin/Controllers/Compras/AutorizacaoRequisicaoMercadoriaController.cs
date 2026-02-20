using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Compras/FluxoCompra", "Compras/AutorizacaoRequisicaoMercadoria")]
    public class AutorizacaoRequisicaoMercadoriaController : BaseController
    {
		#region Construtores

		public AutorizacaoRequisicaoMercadoriaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> listaRequisicoes = new List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaRequisicoes, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaRequisicoes, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistro);

                // Retorna Dados
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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> listaRequisicoes = new List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaRequisicoes, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaRequisicoes, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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
                // Repositorios
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao = repRequisicaoMercadoria.BuscarPorCodigo(codigo);

                if (requisicao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynDados = new
                {
                    requisicao.Codigo,
                    requisicao.Numero,
                    EnumSituacao = requisicao.Situacao,
                    Data = requisicao.Data.ToString("dd/MM/yyyy"),
                    Situacao = requisicao.DescricaoSituacao,

                    Colaborador = requisicao.Usuario.Nome,
                    Filial = requisicao.Filial.RazaoSocial,
                    Motivo = requisicao.MotivoCompra.Descricao,
                    requisicao.Observacao,
                };

                return new JsonpResult(dynDados);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Usuario"), out int usuario);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (usuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PodeAprovar", false);

                // Instancia repositorio
                Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

                List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> regras = repAprovacaoAlcadaRequisicaoMercadoria.BuscarPorRequisicaoEUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from requisicaoAutorizacao in regras
                             select new
                             {
                                 requisicaoAutorizacao.Codigo,
                                 Regra = TituloRegra(requisicaoAutorizacao),
                                 Situacao = requisicaoAutorizacao.DescricaoSituacao,
                                 Usuario = requisicaoAutorizacao.Usuario.Nome,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repAprovacaoAlcadaRequisicaoMercadoria.VerificarSePodeAprovar(codigo, requisicaoAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(requisicaoAutorizacao)
                             }).ToList();

                // Retorna Grid
                grid.setarQuantidadeTotal(lista.Count());
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

        public async Task<IActionResult> AprovarMultiplosItens()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
                List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> requisicaos = new List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();

                try
                {
                    requisicaos = ObterItensSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> requisicaosAutorizacoes = BuscarRegrasPorRequisicao(requisicaos, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosItensVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < requisicaosAutorizacoes.Count(); i++)
                {
                    int codigo = requisicaosAutorizacoes[i].RequisicaoMercadoria.Codigo;

                    if (!codigosItensVerificados.Contains(codigo))
                        codigosItensVerificados.Add(codigo);

                    EfetuarAprovacao(requisicaosAutorizacoes[i], false, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, requisicaosAutorizacoes[i].RequisicaoMercadoria, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosItensVerificados)
                    this.VerificarSituacaoRequisicaoMercadoria(repRequisicaoMercadoria.BuscarPorCodigo(cod), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = requisicaosAutorizacoes.Count()
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as solicitações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarMultiplosItens()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
                Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

                // Codigo da regra
                string motivo = Request.Params("Motivo") ?? string.Empty;

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> requisicaos = new List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();

                try
                {
                    requisicaos = ObterItensSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> requisicaosAutorizacoes = BuscarRegrasPorRequisicao(requisicaos, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosItensVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < requisicaosAutorizacoes.Count(); i++)
                {
                    int codigo = requisicaosAutorizacoes[i].RequisicaoMercadoria.Codigo;

                    if (!codigosItensVerificados.Contains(codigo))
                        codigosItensVerificados.Add(codigo);

                    // Metodo de rejeitar avaria
                    requisicaosAutorizacoes[i].Data = DateTime.Now;
                    requisicaosAutorizacoes[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                    requisicaosAutorizacoes[i].Motivo = motivo;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, requisicaosAutorizacoes[i], null, "Reprovou a regra. Motivo: " + requisicaosAutorizacoes[i].Motivo, unitOfWork);

                    // Atualiza banco
                    repAprovacaoAlcadaRequisicaoMercadoria.Atualizar(requisicaosAutorizacoes[i]);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, requisicaosAutorizacoes[i].RequisicaoMercadoria, null, "Reprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosItensVerificados)
                    this.VerificarSituacaoRequisicaoMercadoria(repRequisicaoMercadoria.BuscarPorCodigo(cod), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = requisicaosAutorizacoes.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar as solicitações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Instancia
                Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao = repRequisicaoMercadoria.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> requisicaosAutorizacoes = repAprovacaoAlcadaRequisicaoMercadoria.BuscarPorRequisicaoUsuarioSituacao(codigo, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < requisicaosAutorizacoes.Count(); i++)
                    EfetuarAprovacao(requisicaosAutorizacoes[i], false, unitOfWork);

                this.VerificarSituacaoRequisicaoMercadoria(requisicao, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = requisicaosAutorizacoes.Count()
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria alcada = repAprovacaoAlcadaRequisicaoMercadoria.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (alcada == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida a situacao
                if (alcada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                EfetuarAprovacao(alcada, true, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
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
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Rejeitar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria requisicaoAutorizacao = repAprovacaoAlcadaRequisicaoMercadoria.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (requisicaoAutorizacao == null || requisicaoAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (requisicaoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                requisicaoAutorizacao.Data = DateTime.Now;
                requisicaoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                requisicaoAutorizacao.Motivo = motivo;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, requisicaoAutorizacao, null, "Repovou regra. Motivo: " + motivo, unitOfWork);

                // Atualiza banco
                repAprovacaoAlcadaRequisicaoMercadoria.Atualizar(requisicaoAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, requisicaoAutorizacao.RequisicaoMercadoria, unitOfWork);
                this.VerificarSituacaoRequisicaoMercadoria(requisicaoAutorizacao.RequisicaoMercadoria, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("FuncionarioRequisitado").Nome("Funcionário").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Filial").Nome("Filial").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("MotivoCompra").Nome("Motivo").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Data").Nome("Data").Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Valor")
                propOrdena = "ValorAvaria";
        }

        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria requisicao, bool verificarSeEstaAprovado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Compras.Mercadoria mercadoria in requisicao.RequisicaoMercadoria.Mercadorias)
            {
                if (mercadoria.ProdutoEstoque != null)
                {
                    var formas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria> {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria.GerarPeloEstoque,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria.Estoque
                    };

                    var categoriasProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto> {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.Servicos
                    };
                    //Verifica se foi selecionado bloquear estoque negativo e se motivo da compra é "Gerar pelo estoque"
                    if (ConfiguracaoEmbarcador.ControlarEstoqueNegativo && requisicao.RequisicaoMercadoria.MotivoCompra != null && formas.Contains(requisicao.RequisicaoMercadoria.MotivoCompra.Forma) && !categoriasProdutos.Contains(mercadoria.ProdutoEstoque.Produto.CategoriaProduto))
                    {
                        //Soma produtos com o mesmo código no carrinho
                        decimal somaProdutosRepetidos = requisicao.RequisicaoMercadoria.Mercadorias.Where(x => x.ProdutoEstoque.Produto.Codigo == mercadoria.ProdutoEstoque.Produto.Codigo).Aggregate(0m, (quantidadeTotal, itemAtual) => quantidadeTotal + itemAtual.Quantidade);
                        //Verifica se o produto tem estoque suficiente para finalizar a compra
                        if (mercadoria.ProdutoEstoque.Quantidade < somaProdutosRepetidos)
                        {
                            throw new SemEstoqueException("Produto " + mercadoria.ProdutoEstoque.Produto.Descricao + " indisponível em estoque, quantidade disponíveis: " + Convert.ToInt32(mercadoria.ProdutoEstoque.Quantidade) + ".");
                        }
                    }
                }
            }

            // So modifica a autorizacao quando ela for pendente
            if (requisicao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente && requisicao.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                requisicao.Data = DateTime.Now;
                requisicao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada;

                // Atualiza os dados
                repAprovacaoAlcadaRequisicaoMercadoria.Atualizar(requisicao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, requisicao, null, "Aprovou a regra", unitOfWork);

                // Faz verificacao se a carga esta aprovada
                if (verificarSeEstaAprovado)
                    this.VerificarSituacaoRequisicaoMercadoria(requisicao.RequisicaoMercadoria, unitOfWork);

                this.NotificarAlteracao(true, requisicao.RequisicaoMercadoria, unitOfWork);
            }
        }

        private List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> BuscarRegrasPorRequisicao(List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> solicitacoes, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);
            List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> requisicaoAutorizacao = new List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria>();

            foreach (Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao in solicitacoes)
            {
                List<Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria> regras = repAprovacaoAlcadaRequisicaoMercadoria.BuscarPorRequisicaoUsuarioSituacao(requisicao.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                requisicaoAutorizacao.AddRange(regras);
            }

            return requisicaoAutorizacao;
        }

        private List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> ObterItensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
            List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> listaRequisicoes = new List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaRequisicoes, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                foreach (var dynItemNaoSelecionado in listaItensNaoSelecionados)
                    listaRequisicoes.Remove(new Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria() { Codigo = (int)dynItemNaoSelecionado.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));
                foreach (var dynItemSelecionada in listaItensSelecionados)
                    listaRequisicoes.Add(repRequisicaoMercadoria.BuscarPorCodigo((int)dynItemSelecionada.Codigo));
            }

            // Retorna lista
            return listaRequisicoes;
        }

        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Define icone
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                if (aprovada)
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                else
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;

                // Emite notificação
                string mensagem = string.Format(Localization.Resources.Compras.AutorizacaoRequisicaoMercadoria.UsuarioRequisicao, (aprovada ? Localization.Resources.Gerais.Geral.Aprovada : Localization.Resources.Gerais.Geral.Rejeitada), requisicao.Numero);
                serNotificacao.GerarNotificacaoEmail(requisicao.Usuario, this.Usuario, requisicao.Codigo, "Compras/RequisicaoMercadoria", "", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void VerificarSituacaoRequisicaoMercadoria(Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Repositorio.UnitOfWork unitOfWork)
        {
            //try
            //{
            // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
            if (requisicao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.AgAprovacao)
            {
                // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);
                Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> regras = repAprovacaoAlcadaRequisicaoMercadoria.BuscarRegrasRequisicao(requisicao.Codigo);

                // Flag de rejeicao
                bool rejeitada = false;
                bool aprovada = true;

                foreach (Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria regra in regras)
                {
                    int pendentes = repAprovacaoAlcadaRequisicaoMercadoria.ContarPendentes(requisicao.Codigo, regra.Codigo);

                    int aprovacoes = repAprovacaoAlcadaRequisicaoMercadoria.ContarAprovacoesSolicitacao(requisicao.Codigo, regra.Codigo);

                    int rejeitadas = repAprovacaoAlcadaRequisicaoMercadoria.ContarRejeitadas(requisicao.Codigo, regra.Codigo);

                    int necessariosParaAprovar = regra.NumeroAprovadores;

                    // Situacao
                    if (rejeitadas > 0)
                        rejeitada = true;
                    if (aprovacoes < necessariosParaAprovar)
                        aprovada = false;
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.Aprovada;

                if (rejeitada)
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.Rejeitada;

                if (aprovada || rejeitada)
                {
                    requisicao.Situacao = situacao;

                    repRequisicaoMercadoria.Atualizar(requisicao);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                    if (rejeitada)
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                    else
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                    // Emite notificação
                    string mensagem = string.Format(Localization.Resources.Compras.AutorizacaoRequisicaoMercadoria.RequisicaoFoi, requisicao.Numero, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada));

                    serNotificacao.GerarNotificacao(requisicao.Usuario, this.Usuario, requisicao.Codigo, "Compras/AutorizacaoRequisicaoMercadoria", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                }

                Servicos.Embarcador.Compras.RequisicaoMercadoria.RequisicaoAprovada(requisicao, unitOfWork, Auditado);
            }
            //}
            //catch (Exception ex)
            //{
            //    Servicos.Log.TratarErro(ex);
            //}
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria regra)
        {
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info;
            else
                return "";
        }

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> listaRequisicoes, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("Motivo"), out int motivo);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("Numero"), out int numero);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria situacaoAux))
                situacao = situacaoAux;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            listaRequisicoes = repAprovacaoAlcadaRequisicaoMercadoria.Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, filial, motivo, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repAprovacaoAlcadaRequisicaoMercadoria.ContarConsulta(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, filial, motivo);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> listaRequisicoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

            var lista = from obj in listaRequisicoes
                        select new
                        {
                            obj.Codigo,
                            obj.Numero,
                            Filial = obj.Filial.RazaoSocial,
                            FuncionarioRequisitado = obj.FuncionarioRequisitado?.Nome ?? string.Empty,
                            MotivoCompra = obj.MotivoCompra.Descricao,
                            Data = obj.Data.ToString("dd/MM/yyyy"),
                            Situacao = obj.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria regra)
        {
            return regra.RegraRequisicaoMercadoria?.Descricao ?? string.Empty;
        }

        #endregion
    }
}
