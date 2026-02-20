using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Pessoas/AutorizacaoFuncionarioComissao")]
    public class AutorizacaoFuncionarioComissaoController : BaseController
    {
		#region Construtores

		public AutorizacaoFuncionarioComissaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(7).Align(Models.Grid.Align.right);
            grid.Prop("DataGeracao").Nome("Data Geração").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Funcionario").Nome("Funcionário").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Operador").Nome("Operador").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.center);

            grid.Prop("ValorFinal").Nome("Valor Final").Tamanho(7).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("PercentualComissao").Nome("% Comissão").Tamanho(7).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("ValorComissao").Nome("Valor Comissão").Tamanho(7).Align(Models.Grid.Align.right).Ord(false);

            return grid;
        }

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

                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> listaComissoes = new List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaComissoes, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaComissoes, unitOfWork);

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

                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> listaOrdens = new List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaOrdens, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaOrdens, unitOfWork);

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
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);

                // Codigo funcionarioComissao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao = repFuncionarioComissao.BuscarPorCodigo(codigo);

                if (funcionarioComissao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynDados = new
                {
                    funcionarioComissao.Codigo,
                    funcionarioComissao.Numero,
                    EnumSituacao = funcionarioComissao.Situacao,
                    DataInicial = funcionarioComissao.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = funcionarioComissao.DataFinal.ToString("dd/MM/yyyy"),
                    Operador = funcionarioComissao.Operador.Nome,
                    Funcionario = funcionarioComissao.Funcionario.Nome,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissaoHelper.ObterDescricao(funcionarioComissao.Situacao),
                    ValorFinal = funcionarioComissao.ValorTotalFinal.ToString("n2"),
                    funcionarioComissao.QuantidadeTitulos,
                    PercentualComissao = funcionarioComissao.PercentualComissao.ToString("n2"),
                    PercentualComissaoAcrescimo = funcionarioComissao.PercentualComissaoAcrescimo.ToString("n2"),
                    PercentualComissaoTotal = funcionarioComissao.PercentualComissaoTotal.ToString("n2"),
                    ValorComissao = funcionarioComissao.ValorComissao.ToString("n2"),
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
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> regras = repAprovacaoAlcadaFuncionarioComissao.BuscarPorFuncionarioComissaoEUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from funcionarioComissaoAutorizacao in regras
                             select new
                             {
                                 funcionarioComissaoAutorizacao.Codigo,
                                 Regra = TituloRegra(funcionarioComissaoAutorizacao),
                                 Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegraHelper.ObterDescricao(funcionarioComissaoAutorizacao.Situacao),
                                 Usuario = funcionarioComissaoAutorizacao.Usuario != null ? funcionarioComissaoAutorizacao.Usuario.Nome : string.Empty,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repAprovacaoAlcadaFuncionarioComissao.VerificarSePodeAprovar(codigo, funcionarioComissaoAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(funcionarioComissaoAutorizacao)
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

                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> ordens = new List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();

                try
                {
                    ordens = ObterItensSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> ordensAutorizacoes = BuscarRegrasPorOrdem(ordens, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosItensVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < ordensAutorizacoes.Count(); i++)
                {
                    int codigo = ordensAutorizacoes[i].FuncionarioComissao.Codigo;

                    if (!codigosItensVerificados.Contains(codigo))
                        codigosItensVerificados.Add(codigo);

                    EfetuarAprovacao(ordensAutorizacoes[i], false, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ordensAutorizacoes[i].FuncionarioComissao, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosItensVerificados)
                    this.VerificarSituacaoFuncionarioComissao(repFuncionarioComissao.BuscarPorCodigo(cod), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = ordensAutorizacoes.Count()
                });
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
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

                // Codigo da regra
                string motivo = Request.Params("Motivo") ?? string.Empty;

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> ordens = new List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();

                try
                {
                    ordens = ObterItensSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> ordensAutorizacoes = BuscarRegrasPorOrdem(ordens, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosItensVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < ordensAutorizacoes.Count(); i++)
                {
                    int codigo = ordensAutorizacoes[i].FuncionarioComissao.Codigo;

                    if (!codigosItensVerificados.Contains(codigo))
                        codigosItensVerificados.Add(codigo);

                    // Metodo de rejeitar avaria
                    ordensAutorizacoes[i].Data = DateTime.Now;
                    ordensAutorizacoes[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                    ordensAutorizacoes[i].Motivo = motivo;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ordensAutorizacoes[i], null, "Reprovou a regra. Motivo: " + ordensAutorizacoes[i].Motivo, unitOfWork);

                    // Atualiza banco
                    repAprovacaoAlcadaFuncionarioComissao.Atualizar(ordensAutorizacoes[i]);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ordensAutorizacoes[i].FuncionarioComissao, null, "Reprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosItensVerificados)
                    this.VerificarSituacaoFuncionarioComissao(repFuncionarioComissao.BuscarPorCodigo(cod), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = ordensAutorizacoes.Count()
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
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao = repFuncionarioComissao.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> ordensAutorizacoes = repAprovacaoAlcadaFuncionarioComissao.BuscarPorFuncionarioComissaoUsuarioSituacao(codigo, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < ordensAutorizacoes.Count(); i++)
                    EfetuarAprovacao(ordensAutorizacoes[i], false, unitOfWork);

                this.VerificarSituacaoFuncionarioComissao(funcionarioComissao, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = ordensAutorizacoes.Count()
                });
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
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

                // Codigo funcionarioComissao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao alcada = repAprovacaoAlcadaFuncionarioComissao.BuscarPorCodigo(codigo);

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
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao funcionarioComissaoAutorizacao = repAprovacaoAlcadaFuncionarioComissao.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (funcionarioComissaoAutorizacao == null || funcionarioComissaoAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (funcionarioComissaoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                funcionarioComissaoAutorizacao.Data = DateTime.Now;
                funcionarioComissaoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                funcionarioComissaoAutorizacao.Motivo = motivo;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, funcionarioComissaoAutorizacao, null, "Repovou regra. Motivo: " + motivo, unitOfWork);

                // Atualiza banco
                repAprovacaoAlcadaFuncionarioComissao.Atualizar(funcionarioComissaoAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, funcionarioComissaoAutorizacao.FuncionarioComissao, unitOfWork);
                this.VerificarSituacaoFuncionarioComissao(funcionarioComissaoAutorizacao.FuncionarioComissao, unitOfWork);

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

        /* EfetuarAprovacao
         * Aprova a autorizacao da carga
         */
        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao funcionarioComissao, bool verificarSeEstaAprovado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (funcionarioComissao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente && funcionarioComissao.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                funcionarioComissao.Data = DateTime.Now;
                funcionarioComissao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada;

                // Atualiza os dados
                repAprovacaoAlcadaFuncionarioComissao.Atualizar(funcionarioComissao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, funcionarioComissao, null, "Aprovou a regra", unitOfWork);

                // Faz verificacao se a carga esta aprovada
                if (verificarSeEstaAprovado)
                    this.VerificarSituacaoFuncionarioComissao(funcionarioComissao.FuncionarioComissao, unitOfWork);

                this.NotificarAlteracao(true, funcionarioComissao.FuncionarioComissao, unitOfWork);
            }
        }



        private List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> BuscarRegrasPorOrdem(List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> solicitacoes, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> funcionarioComissaoAutorizacao = new List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao>();

            foreach (Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao in solicitacoes)
            {
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> regras = repAprovacaoAlcadaFuncionarioComissao.BuscarPorFuncionarioComissaoUsuarioSituacao(funcionarioComissao.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                funcionarioComissaoAutorizacao.AddRange(regras);
            }

            return funcionarioComissaoAutorizacao;
        }



        private List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> ObterItensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> listaOrdens = new List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaOrdens, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                foreach (var dynItemNaoSelecionado in listaItensNaoSelecionados)
                    listaOrdens.Remove(new Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao() { Codigo = (int)dynItemNaoSelecionado.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));
                foreach (var dynItemSelecionada in listaItensSelecionados)
                    listaOrdens.Add(repFuncionarioComissao.BuscarPorCodigo((int)dynItemSelecionada.Codigo));
            }

            // Retorna lista
            return listaOrdens;
        }



        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork)
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
                string mensagem = string.Format(Localization.Resources.Pessoas.AutorizacaoFuncionarioComissao.UsuarioComissaoFuncionario, (aprovada ? Localization.Resources.Gerais.Geral.Aprovada : Localization.Resources.Gerais.Geral.Rejeitada), funcionarioComissao.Numero);
                serNotificacao.GerarNotificacaoEmail(funcionarioComissao.Operador, Usuario, funcionarioComissao.Codigo, "Pessoas/FuncionarioComissao", "", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }



        private void VerificarSituacaoFuncionarioComissao(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork)
        {
            //try
            //{
            // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
            if (funcionarioComissao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.AgAprovacao)
            {
                // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> regras = repAprovacaoAlcadaFuncionarioComissao.BuscarRegraFuncionarioComissao(funcionarioComissao.Codigo);

                // Flag de rejeicao
                bool rejeitada = false;
                bool aprovada = true;

                foreach (Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regra in regras)
                {
                    int pendentes = repAprovacaoAlcadaFuncionarioComissao.ContarPendentes(funcionarioComissao.Codigo, regra.Codigo);

                    int aprovacoes = repAprovacaoAlcadaFuncionarioComissao.ContarAprovacoesSolicitacao(funcionarioComissao.Codigo, regra.Codigo);

                    int rejeitadas = repAprovacaoAlcadaFuncionarioComissao.ContarRejeitadas(funcionarioComissao.Codigo, regra.Codigo);

                    int necessariosParaAprovar = regra.NumeroAprovadores;

                    // Situacao
                    if (rejeitadas > 0)
                        rejeitada = true;
                    if (aprovacoes < necessariosParaAprovar)
                        aprovada = false;
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Aprovada;

                if (rejeitada)
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Rejeitada;

                if (aprovada || rejeitada)
                {
                    funcionarioComissao.Situacao = situacao;

                    repFuncionarioComissao.Atualizar(funcionarioComissao);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                    if (rejeitada)
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                    else
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                    // Emite notificação
                    string mensagem = string.Format(Localization.Resources.Pessoas.AutorizacaoFuncionarioComissao.ComissaoFuncionarioFoi, funcionarioComissao.Numero, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada));

                    serNotificacao.GerarNotificacao(funcionarioComissao.Operador, this.Usuario, funcionarioComissao.Codigo, "Pessoas/FuncionarioComissao", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                }

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Servicos.Embarcador.Pessoa.FuncionarioComissao.FuncionarioComissaoAprovada(funcionarioComissao, unitOfWork, TipoServicoMultisoftware, tipoAmbiente);
            }
            //}
            //catch (Exception ex)
            //{
            //    Servicos.Log.TratarErro(ex);
            //}
        }



        /* CoresRegras
         * Retorna a cor da linha de acordo com a situacoa
         */
        private string CoresRegras(Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao regra)
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


        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> listaComissoes, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Operador"), out int operador);
            int.TryParse(Request.Params("Funcionario"), out int funcionario);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("Numero"), out int numero);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao situacaoAux))
                situacao = situacaoAux;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            listaComissoes = repAprovacaoAlcadaFuncionarioComissao.Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, funcionario, operador, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repAprovacaoAlcadaFuncionarioComissao.ContarConsulta(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, funcionario, operador);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> listaComissoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

            var lista = from obj in listaComissoes
                        select new
                        {
                            obj.Codigo,
                            obj.Numero,
                            Funcionario = obj.Funcionario.Nome,
                            Operador = obj.Operador.Nome,
                            DataGeracao = obj.DataGeracao.ToString("dd/MM/yyyy"),
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissaoHelper.ObterDescricao(obj.Situacao),
                            ValorFinal = obj.ValorTotalFinal.ToString("n2"),
                            PercentualComissao = obj.PercentualComissaoTotal.ToString("n2"),
                            ValorComissao = obj.ValorComissao.ToString("n2")
                        };

            return lista.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao regra)
        {
            return regra.RegraFuncionarioComissao?.Descricao;
        }
        #endregion
    }
}
