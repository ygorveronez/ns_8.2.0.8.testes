using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PagamentoAgregado
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "PagamentosAgregados/AutorizacaoPagamentoAgregado")]
    public class AutorizacaoPagamentoAgregadoController : BaseController
    {
		#region Construtores

		public AutorizacaoPagamentoAgregadoController(Conexao conexao) : base(conexao) { }

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

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> listaPagamentos = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaPagamentos, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaPagamentos, unitOfWork);

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

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> listaPagamentos = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaPagamentos, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaPagamentos, unitOfWork);

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
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigo);

                if (pagamento == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynDados = new
                {
                    pagamento.Codigo,
                    EnumSituacao = pagamento.Situacao,
                    Situacao = pagamento.DescricaoSituacao,
                    DataPagamento = pagamento.DataPagamento.ToString("dd/MM/yyyy"),
                    Valor = pagamento.Valor.ToString("n3"),

                    Numero = pagamento.Numero.ToString(),
                    DataInicial = pagamento.DataInicial.HasValue ? pagamento.DataInicial.Value.ToString("dd/MM/yyyy") : pagamento.DataInicialOcorrencia.Value.ToString("dd/MM/yyyy"),
                    DataFinal = pagamento.DataFinal.HasValue ? pagamento.DataFinal.Value.ToString("dd/MM/yyyy") : pagamento.DataFinalOcorrencia.Value.ToString("dd/MM/yyyy"),

                    Cliente = pagamento.Cliente.Nome,
                    Motivo = pagamento.Observacao
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
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> regras = repAprovacaoAlcadaPagamentoAgregado.BuscarPorPagamentoEUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from pagamentoAutorizacao in regras
                             select new
                             {
                                 pagamentoAutorizacao.Codigo,
                                 Regra = TituloRegra(pagamentoAutorizacao),
                                 Situacao = pagamentoAutorizacao.DescricaoSituacao,
                                 Usuario = pagamentoAutorizacao.Usuario.Nome,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repAprovacaoAlcadaPagamentoAgregado.VerificarSePodeAprovar(codigo, pagamentoAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(pagamentoAutorizacao)
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

        public async Task<IActionResult> AprovarMultiplosPagamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> pagamentos = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();

                try
                {
                    pagamentos = ObterPagamentosSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> pagamentosAutorizacoes = BuscarRegrasPorPagamentos(pagamentos, this.Usuario.Codigo, unitOfWork, out bool pagamentosSemEstoque);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosPagamentosVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < pagamentosAutorizacoes.Count(); i++)
                {
                    int codigo = pagamentosAutorizacoes[i].PagamentoAgregado.Codigo;

                    if (!codigosPagamentosVerificados.Contains(codigo))
                        codigosPagamentosVerificados.Add(codigo);

                    string retorno = string.Empty;
                    retorno = EfetuarAprovacao(pagamentosAutorizacoes[i], false, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu um erro ao aprovar as solicitações. " + retorno);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentosAutorizacoes[i].PagamentoAgregado, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao                
                foreach (int cod in codigosPagamentosVerificados)
                {
                    string retorno = string.Empty;
                    retorno = this.VerificarSituacaoPagamento(repPagamentoAgregado.BuscarPorCodigo(cod), unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu um erro ao aprovar as solicitações. " + retorno);
                    }
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = pagamentosAutorizacoes.Count(),
                    Msg = pagamentosSemEstoque ? "Alguns pagamentos não foram possíveis de aprovar por não conterem estoque suficiente." : ""
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

        public async Task<IActionResult> ReprovarMultiplosPagamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

                // Codigo da regra
                string motivo = Request.Params("Motivo") ?? string.Empty;

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> pagamentos = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();

                try
                {
                    pagamentos = ObterPagamentosSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> pagamentosAutorizacoes = BuscarRegrasPorPagamentos(pagamentos, this.Usuario.Codigo, unitOfWork, out bool pagamentosSemEstoque);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosPagamentosVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < pagamentosAutorizacoes.Count(); i++)
                {
                    int codigo = pagamentosAutorizacoes[i].PagamentoAgregado.Codigo;

                    if (!codigosPagamentosVerificados.Contains(codigo))
                        codigosPagamentosVerificados.Add(codigo);

                    // Metodo de rejeitar avaria
                    pagamentosAutorizacoes[i].Data = DateTime.Now;
                    pagamentosAutorizacoes[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                    pagamentosAutorizacoes[i].Motivo = motivo;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentosAutorizacoes[i], null, "Reprovou a regra. Motivo: " + pagamentosAutorizacoes[i].Motivo, unitOfWork);

                    // Atualiza banco
                    repAprovacaoAlcadaPagamentoAgregado.Atualizar(pagamentosAutorizacoes[i]);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentosAutorizacoes[i].PagamentoAgregado, null, "Reprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao                
                foreach (int cod in codigosPagamentosVerificados)
                {
                    string retorno = string.Empty;
                    retorno = this.VerificarSituacaoPagamento(repPagamentoAgregado.BuscarPorCodigo(cod), unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu um erro ao aprovar as solicitações. " + retorno);
                    }
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = pagamentosAutorizacoes.Count()
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
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigoPagamento);

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento = repPagamentoAgregado.BuscarPorCodigo(codigoPagamento);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> pagamentosAutorizacoes = repAprovacaoAlcadaPagamentoAgregado.BuscarPorPagamentoUsuarioSituacao(codigoPagamento, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < pagamentosAutorizacoes.Count(); i++)
                {
                    string retorno2 = string.Empty;
                    retorno2 = EfetuarAprovacao(pagamentosAutorizacoes[i], false, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retorno2))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu um erro ao aprovar as solicitações. " + retorno2);
                    }
                }

                string retorno = string.Empty;
                retorno = this.VerificarSituacaoPagamento(pagamento, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Ocorreu um erro ao aprovar a solicitação. " + retorno);
                }

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = pagamentosAutorizacoes.Count()
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
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado alcada = repAprovacaoAlcadaPagamentoAgregado.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (alcada == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida a situacao
                if (alcada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                string retorno = string.Empty;
                retorno = EfetuarAprovacao(alcada, true, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Ocorreu um erro ao aprovar as solicitações. " + retorno);
                }

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
                Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado pagamentoAutorizacao = repAprovacaoAlcadaPagamentoAgregado.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (pagamentoAutorizacao == null || pagamentoAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (pagamentoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                pagamentoAutorizacao.Data = DateTime.Now;
                pagamentoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                pagamentoAutorizacao.Motivo = motivo;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoAutorizacao, null, "Repovou regra. Motivo: " + motivo, unitOfWork);

                // Atualiza banco
                repAprovacaoAlcadaPagamentoAgregado.Atualizar(pagamentoAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, pagamentoAutorizacao.PagamentoAgregado, unitOfWork);
                string retorno = string.Empty;
                retorno = this.VerificarSituacaoPagamento(pagamentoAutorizacao.PagamentoAgregado, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Ocorreu um erro ao aprovar a solicitação. " + retorno);
                }

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
            grid.Prop("Data").Nome("Data Pagamento").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("Numero").Nome("Número").Tamanho(7);
            grid.Prop("Cliente").Nome("Agregado").Tamanho(20);
            grid.Prop("Valor").Nome("Valor").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Motivo").Nome("Motivo").Tamanho(20);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Motivo")
                propOrdena = "Observacao";
        }

        private string EfetuarAprovacao(Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado pagamento, bool verificarSeEstaAprovado, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = string.Empty;
            Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (pagamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente && pagamento.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                pagamento.Data = DateTime.Now;
                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada;

                // Atualiza os dados
                repAprovacaoAlcadaPagamentoAgregado.Atualizar(pagamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamento, null, "Aprovou a regra", unitOfWork);

                // Faz verificacao se a carga esta aprovada
                if (verificarSeEstaAprovado)
                {
                    retorno = this.VerificarSituacaoPagamento(pagamento.PagamentoAgregado, unitOfWork);
                    return retorno;

                }

                this.NotificarAlteracao(true, pagamento.PagamentoAgregado, unitOfWork);
            }
            return retorno;
        }

        private List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> BuscarRegrasPorPagamentos(List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> solicitacoes, int usuario, Repositorio.UnitOfWork unitOfWork, out bool pagamentosSemEstoque)
        {
            pagamentosSemEstoque = false;

            Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> pagamentoAutorizacao = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado>();

            foreach (Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento in solicitacoes)
            {
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado> regras = repAprovacaoAlcadaPagamentoAgregado.BuscarPorPagamentoUsuarioSituacao(pagamento.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);
                pagamentoAutorizacao.AddRange(regras);
            }

            return pagamentoAutorizacao;
        }

        private List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> ObterPagamentosSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> listaPagamentos = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaPagamentos, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaPagamentosNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PagamentosNaoSelecionados"));
                foreach (var dybPagamentosNaoSelecionada in listaPagamentosNaoSelecionados)
                    listaPagamentos.Remove(new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado() { Codigo = (int)dybPagamentosNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaPagamentosSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PagamentosSelecionados"));
                foreach (var dynPagamentosSelecionada in listaPagamentosSelecionados)
                    listaPagamentos.Add(repPagamentoAgregado.BuscarPorCodigo((int)dynPagamentosSelecionada.Codigo));
            }

            // Retorna lista
            return listaPagamentos;
        }

        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unitOfWork)
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
                string titulo = Localization.Resources.PagamentoAgregado.AutorizacaoPagamentoAgregado.PagamentoAgregado;
                string mensagem = string.Format(Localization.Resources.PagamentoAgregado.AutorizacaoPagamentoAgregado.UsuarioPagamentoValorCliente, (aprovada ? Localization.Resources.Gerais.Geral.Aprovada : Localization.Resources.Gerais.Geral.Rejeitada), pagamento.Valor.ToString("n2"), pagamento.Cliente.Nome);
                serNotificacao.GerarNotificacaoEmail(pagamento.Usuario, this.Usuario, pagamento.Codigo, "PagamentosAgregados/PagamentoAgregado", titulo, mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private string VerificarSituacaoPagamento(Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = string.Empty;
            try
            {
                // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
                if (pagamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.AgAprovacao)
                {
                    // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                    Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);
                    Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                    Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                    List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> regras = repAprovacaoAlcadaPagamentoAgregado.BuscarRegrasPagamento(pagamento.Codigo);

                    // Flag de rejeicao
                    bool rejeitada = false;
                    bool aprovada = true;

                    foreach (Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regra in regras)
                    {
                        int pendentes = repAprovacaoAlcadaPagamentoAgregado.ContarPendentes(pagamento.Codigo, regra.Codigo);

                        int aprovacoes = repAprovacaoAlcadaPagamentoAgregado.ContarAprovacoesSolicitacao(pagamento.Codigo, regra.Codigo);

                        int rejeitadas = repAprovacaoAlcadaPagamentoAgregado.ContarRejeitadas(pagamento.Codigo, regra.Codigo);

                        int necessariosParaAprovar = regra.NumeroAprovadores;

                        // Situacao
                        if (rejeitadas > 0)
                            rejeitada = true;
                        if (aprovacoes < necessariosParaAprovar)
                            aprovada = false;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Finalizado;

                    if (rejeitada)
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada;

                    if (aprovada || rejeitada)
                    {
                        pagamento.Situacao = situacao;
                        pagamento.DataAprovacao = DateTime.Now;
                        pagamento.UsuarioAprovador = this.Usuario;

                        repPagamentoAgregado.Atualizar(pagamento);

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                        if (rejeitada)
                            icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                        else
                            icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                        // Emite notificação
                        string mensagem = string.Format(Localization.Resources.PagamentoAgregado.AutorizacaoPagamentoAgregado.SolicitacaoPagamentoValorFoi, pagamento.Valor.ToString("n2"), pagamento.Cliente.Nome, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada));

                        serNotificacao.GerarNotificacao(pagamento.Usuario, this.Usuario, pagamento.Codigo, "PagamentosAgregados/AutorizacaoPagamentoAgregado", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                    }
                    string retornoContrato = "";
                    Servicos.Embarcador.PagamentoAgregado.PagamentoAgregado.PagamentoAgregadoAprovado(out retornoContrato, pagamento, unitOfWork, Auditado, TipoServicoMultisoftware);
                    return retornoContrato;
                }
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return ex.Message;
            }
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado regra)
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

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> listaPagamentos, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

            // Converte parametros
            double.TryParse(Request.Params("Cliente"), out double cliente);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("Numero"), out int numero);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado situacaoAux))
                situacao = situacaoAux;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            listaPagamentos = repAprovacaoAlcadaPagamentoAgregado.Consultar(usuario, dataInicial, dataFinal, situacao, numero, cliente, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repAprovacaoAlcadaPagamentoAgregado.ContarConsulta(usuario, dataInicial, dataFinal, situacao, numero, cliente);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> listaPagamentos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado repAprovacaoAlcadaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado(unitOfWork);

            var lista = from pagamento in listaPagamentos
                        select new
                        {
                            pagamento.Codigo,
                            Data = pagamento.DataPagamento.ToString("dd/MM/yyyy"),
                            Numero = pagamento.Numero.ToString(),
                            Cliente = pagamento.Cliente.Nome,
                            Valor = pagamento.Valor.ToString("n3"),
                            Motivo = pagamento.Observacao,
                            Situacao = pagamento.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.PagamentoAgregado.AprovacaoAlcadaPagamentoAgregado regra)
        {
            return regra.RegraPagamentoAgregado?.Descricao;
        }

        #endregion
    }
}
