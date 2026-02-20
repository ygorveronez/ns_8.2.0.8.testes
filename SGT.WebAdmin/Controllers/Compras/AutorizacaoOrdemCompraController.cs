using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Compras/FluxoCompra", "Compras/AutorizacaoOrdemCompra")]
    public class AutorizacaoOrdemCompraController : BaseController
    {
		#region Construtores

		public AutorizacaoOrdemCompraController(Conexao conexao) : base(conexao) { }

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

                List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> listaOrdens = new List<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaOrdens, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaOrdens, unitOfWork);

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

                List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> listaOrdens = new List<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();

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
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

                // Codigo ordem
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem = repOrdemCompra.BuscarPorCodigo(codigo);

                if (ordem == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynDados = new
                {
                    ordem.Codigo,
                    ordem.Numero,
                    EnumSituacao = ordem.Situacao,
                    Data = ordem.Data.ToString("dd/MM/yyyy"),
                    DataPrevisao = ordem.DataPrevisaoRetorno.ToString("dd/MM/yyyy"),
                    Operador = ordem.Usuario?.Nome ?? string.Empty,
                    Transportador = ordem.Transportador?.Nome ?? string.Empty,
                    Fornecedor = ordem.Fornecedor.Nome ?? string.Empty,
                    Situacao = ordem.DescricaoSituacao ?? string.Empty,
                    CondicaoPagamento = ordem.CondicaoPagamento ?? string.Empty,
                    Observacao = ordem.Observacao ?? string.Empty,
                    MotivoAprovacao = string.Join(", ", ordem.Autorizacoes?.Where(o => !string.IsNullOrWhiteSpace(o.Motivo)).Select(o => o.Motivo).ToList() ?? new List<string>()),
                    Colaborador = ordem.Usuario?.Nome ?? "",
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
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> regras = repAprovacaoAlcadaOrdemCompra.BuscarPorOrdemEUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from ordemAutorizacao in regras
                             select new
                             {
                                 ordemAutorizacao.Codigo,
                                 Regra = TituloRegra(ordemAutorizacao),
                                 Situacao = ordemAutorizacao.DescricaoSituacao,
                                 Usuario = ordemAutorizacao.Usuario != null ? ordemAutorizacao.Usuario.Nome : string.Empty,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repAprovacaoAlcadaOrdemCompra.VerificarSePodeAprovar(codigo, ordemAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(ordemAutorizacao)
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

                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> ordens = new List<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();

                try
                {
                    ordens = ObterItensSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> ordensAutorizacoes = BuscarRegrasPorOrdem(ordens, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosItensVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < ordensAutorizacoes.Count(); i++)
                {
                    int codigo = ordensAutorizacoes[i].OrdemCompra.Codigo;

                    if (!codigosItensVerificados.Contains(codigo))
                        codigosItensVerificados.Add(codigo);

                    EfetuarAprovacao(ordensAutorizacoes[i], false, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ordensAutorizacoes[i].OrdemCompra, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosItensVerificados)
                    this.VerificarSituacaoOrdemCompra(repOrdemCompra.BuscarPorCodigo(cod), unitOfWork);

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
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

                // Codigo da regra
                string motivo = Request.Params("Motivo") ?? string.Empty;

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> ordens = new List<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();

                try
                {
                    ordens = ObterItensSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> ordensAutorizacoes = BuscarRegrasPorOrdem(ordens, this.Usuario.Codigo, unitOfWork);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosItensVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < ordensAutorizacoes.Count(); i++)
                {
                    int codigo = ordensAutorizacoes[i].OrdemCompra.Codigo;

                    if (!codigosItensVerificados.Contains(codigo))
                        codigosItensVerificados.Add(codigo);

                    // Metodo de rejeitar avaria
                    ordensAutorizacoes[i].Data = DateTime.Now;
                    ordensAutorizacoes[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                    ordensAutorizacoes[i].Motivo = motivo;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ordensAutorizacoes[i], null, "Reprovou a regra. Motivo: " + ordensAutorizacoes[i].Motivo, unitOfWork);

                    // Atualiza banco
                    repAprovacaoAlcadaOrdemCompra.Atualizar(ordensAutorizacoes[i]);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ordensAutorizacoes[i].OrdemCompra, null, "Reprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosItensVerificados)
                    this.VerificarSituacaoOrdemCompra(repOrdemCompra.BuscarPorCodigo(cod), unitOfWork);

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
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem = repOrdemCompra.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> ordensAutorizacoes = repAprovacaoAlcadaOrdemCompra.BuscarPorOrdemUsuarioSituacao(codigo, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < ordensAutorizacoes.Count(); i++)
                    EfetuarAprovacao(ordensAutorizacoes[i], false, unitOfWork);

                this.VerificarSituacaoOrdemCompra(ordem, unitOfWork);

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
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

                // Codigo ordem
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra alcada = repAprovacaoAlcadaOrdemCompra.BuscarPorCodigo(codigo);

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
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra ordemAutorizacao = repAprovacaoAlcadaOrdemCompra.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (ordemAutorizacao == null || ordemAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (ordemAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                ordemAutorizacao.Data = DateTime.Now;
                ordemAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                ordemAutorizacao.Motivo = motivo;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ordemAutorizacao, null, "Repovou regra. Motivo: " + motivo, unitOfWork);

                // Atualiza banco
                repAprovacaoAlcadaOrdemCompra.Atualizar(ordemAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, ordemAutorizacao.OrdemCompra, unitOfWork);
                this.VerificarSituacaoOrdemCompra(ordemAutorizacao.OrdemCompra, unitOfWork);

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
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Fornecedor").Nome("Fornecedor").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Data").Nome("Data").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Veiculo").Nome("Veículo").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("ValorTotal").Nome("Valor Total").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("DataPrevisaoRetorno").Nome("Data Prev. Retorno").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("CondicaoPagamento").Nome("Condição de Pagamento").Tamanho(10).Align(Models.Grid.Align.center);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Veiculo")
                propOrdena += ".Placa";
        }

        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra ordem, bool verificarSeEstaAprovado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (ordem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente && ordem.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                ordem.Data = DateTime.Now;
                ordem.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada;

                // Atualiza os dados
                repAprovacaoAlcadaOrdemCompra.Atualizar(ordem);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ordem, null, "Aprovou a regra", unitOfWork);

                // Faz verificacao se a carga esta aprovada
                if (verificarSeEstaAprovado)
                    this.VerificarSituacaoOrdemCompra(ordem.OrdemCompra, unitOfWork);

                this.NotificarAlteracao(true, ordem.OrdemCompra, unitOfWork);
            }
        }

        private List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> BuscarRegrasPorOrdem(List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> solicitacoes, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);
            List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> ordemAutorizacao = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra>();

            foreach (Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem in solicitacoes)
            {
                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> regras = repAprovacaoAlcadaOrdemCompra.BuscarPorOrdemUsuarioSituacao(ordem.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                ordemAutorizacao.AddRange(regras);
            }

            return ordemAutorizacao;
        }

        private List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> ObterItensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
            List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> listaOrdens = new List<Dominio.Entidades.Embarcador.Compras.OrdemCompra>();

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
                    listaOrdens.Remove(new Dominio.Entidades.Embarcador.Compras.OrdemCompra() { Codigo = (int)dynItemNaoSelecionado.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));
                foreach (var dynItemSelecionada in listaItensSelecionados)
                    listaOrdens.Add(repOrdemCompra.BuscarPorCodigo((int)dynItemSelecionada.Codigo));
            }

            // Retorna lista
            return listaOrdens;
        }

        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem, Repositorio.UnitOfWork unitOfWork)
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
                string mensagem = string.Format(Localization.Resources.Compras.AutorizacaoOrdemCompra.UsuarioOrdemCompra, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou), ordem.Numero);
                if (ordem.Usuario != null)
                    serNotificacao.GerarNotificacaoEmail(ordem.Usuario, this.Usuario, ordem.Codigo, "Compras/OrdemCompra", "", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void VerificarSituacaoOrdemCompra(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem, Repositorio.UnitOfWork unitOfWork)
        {
            // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
            if (ordem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.AgAprovacao)
            {
                // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> regras = repAprovacaoAlcadaOrdemCompra.BuscarRegrasOrdem(ordem.Codigo);

                // Flag de rejeicao
                bool rejeitada = false;
                bool aprovada = true;

                foreach (Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regra in regras)
                {
                    int pendentes = repAprovacaoAlcadaOrdemCompra.ContarPendentes(ordem.Codigo, regra.Codigo);

                    int aprovacoes = repAprovacaoAlcadaOrdemCompra.ContarAprovacoesSolicitacao(ordem.Codigo, regra.Codigo);

                    int rejeitadas = repAprovacaoAlcadaOrdemCompra.ContarRejeitadas(ordem.Codigo, regra.Codigo);

                    int necessariosParaAprovar = regra.NumeroAprovadores;

                    // Situacao
                    if (rejeitadas > 0)
                        rejeitada = true;
                    if (aprovacoes < necessariosParaAprovar)
                        aprovada = false;
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aprovada;

                if (rejeitada)
                {
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Rejeitada;

                    Servicos.Embarcador.Compras.FluxoCompra serFluxoCompra = new Servicos.Embarcador.Compras.FluxoCompra(unitOfWork);

                    serFluxoCompra.RejeitarFluxoCompra(ordem.Codigo, unitOfWork, Auditado);

                }
                if (aprovada || rejeitada)
                {
                    ordem.Situacao = situacao;

                    repOrdemCompra.Atualizar(ordem);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                    if (rejeitada)
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                    else
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                    // Emite notificação
                    string mensagem = string.Format(Localization.Resources.Compras.AutorizacaoOrdemCompra.OrdemCompraFoi, ordem.Numero, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada));

                    if (ordem.Usuario != null)
                        serNotificacao.GerarNotificacao(ordem.Usuario, this.Usuario, ordem.Codigo, "Compras/AutorizacaoOrdemCompra", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                }

                Servicos.Embarcador.Compras.OrdemCompra.OrdemCompraAprovada(ordem, unitOfWork, Auditado, TipoServicoMultisoftware);
            }
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra regra)
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

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> listaOrdens, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Operador"), out int operador);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("Numero"), out int numero);

            double.TryParse(Request.Params("Fornecedor"), out double fornecedor);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra situacaoAux))
                situacao = situacaoAux;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            listaOrdens = repAprovacaoAlcadaOrdemCompra.Consultar(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, fornecedor, operador, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repAprovacaoAlcadaOrdemCompra.ContarConsulta(codigoEmpresa, usuario, dataInicial, dataFinal, situacao, numero, fornecedor, operador);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Compras.OrdemCompra> listaOrdens, Repositorio.UnitOfWork unitOfWork)
        {
            var lista = from obj in listaOrdens
                        select new
                        {
                            obj.Codigo,
                            obj.Numero,
                            Fornecedor = obj.Fornecedor.Nome,
                            Data = obj.Data.ToString("dd/MM/yyyy"),
                            Situacao = obj.DescricaoSituacao,
                            ValorTotal = obj.ValorTotal.ToString("n2"),
                            Veiculo = obj.Veiculo?.Placa ?? string.Empty,
                            DataPrevisaoRetorno = obj.DataPrevisaoRetorno.ToString("dd/MM/yyyy"),
                            CondicaoPagamento = obj.CondicaoPagamento ?? string.Empty
                        };

            return lista.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra regra)
        {
            return regra.RegraOrdemCompra?.Descricao;
        }

        #endregion
    }
}
