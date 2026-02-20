using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "WMS/AutorizacaoDescarteLote")]
    public class AutorizacaoDescarteLoteController : BaseController
    {
		#region Construtores

		public AutorizacaoDescarteLoteController(Conexao conexao) : base(conexao) { }

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

                List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> listaDescartes = new List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaDescartes, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaDescartes, unitOfWork);

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

                List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> listaDescartes = new List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaDescartes, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaDescartes, unitOfWork);

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
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte = repDescarteLoteProdutoEmbarcador.BuscarPorCodigo(codigo);

                if (descarte == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynDados = new
                {
                    descarte.Codigo,
                    EnumSituacao = descarte.Situacao,
                    Data = descarte.Data.ToString("dd/MM/yyyy HH:mm"),
                    Situacao = descarte.DescricaoSituacao,
                    QuantidadeDescartada = descarte.Quantidade.ToString("n3"),
                    Motivo = descarte.Motivo,

                    Numero = descarte.Lote.Numero.ToString(),
                    DataVencimento = descarte.Lote.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                    QuantidadeLote = descarte.Lote.QuantidadeLote.ToString("n3"),
                    QuantidadeAtual = descarte.Lote.QuantidadeAtual.ToString("n3"),
                    CodigoBarras = descarte.Lote.CodigoBarras,
                    DepositoPosicao = descarte.Lote.DepositoPosicao.Abreviacao,
                    ProdutoEmbarcador = descarte.Lote.ProdutoEmbarcador.Descricao,
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
                Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

                List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> regras = repAprovacaoAlcadaDescarteLoteProduto.BuscarPorDescarteEUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from descarteAutorizacao in regras
                             select new
                             {
                                 descarteAutorizacao.Codigo,
                                 Regra = TituloRegra(descarteAutorizacao),
                                 Situacao = descarteAutorizacao.DescricaoSituacao,
                                 Usuario = descarteAutorizacao.Usuario?.Nome,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repAprovacaoAlcadaDescarteLoteProduto.VerificarSePodeAprovar(codigo, descarteAutorizacao.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(descarteAutorizacao)
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

        public async Task<IActionResult> AprovarMultiplosDescartes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);
                List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> descartes = new List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>();

                try
                {
                    descartes = ObterDescartesSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> descartesAutorizacoes = BuscarRegrasPorDescartes(descartes, this.Usuario.Codigo, unitOfWork, out bool descartesSemEstoque);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosDescartesVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < descartesAutorizacoes.Count(); i++)
                {
                    int codigo = descartesAutorizacoes[i].Descarte.Codigo;

                    if (!codigosDescartesVerificados.Contains(codigo))
                        codigosDescartesVerificados.Add(codigo);

                    EfetuarAprovacao(descartesAutorizacoes[i], false, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, descartesAutorizacoes[i].Descarte, null, "Aprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosDescartesVerificados)
                    this.VerificarSituacaoDescarte(repDescarteLoteProdutoEmbarcador.BuscarPorCodigo(cod), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = descartesAutorizacoes.Count(),
                    Msg = descartesSemEstoque ? "Alguns descartes não foram possíveis de aprovar por não conterem estoque suficiente." : ""
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

        public async Task<IActionResult> ReprovarMultiplosDescartes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                // Repositorios
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

                // Codigo da regra
                string motivo = Request.Params("Motivo") ?? string.Empty;

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> descartes = new List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>();

                try
                {
                    descartes = ObterDescartesSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> descartesAutorizacoes = BuscarRegrasPorDescartes(descartes, this.Usuario.Codigo, unitOfWork, out bool descartesSemEstoque);

                // Inicia transacao
                unitOfWork.Start();

                List<int> codigosDescartesVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < descartesAutorizacoes.Count(); i++)
                {
                    int codigo = descartesAutorizacoes[i].Descarte.Codigo;

                    if (!codigosDescartesVerificados.Contains(codigo))
                        codigosDescartesVerificados.Add(codigo);

                    // Metodo de rejeitar avaria
                    descartesAutorizacoes[i].Data = DateTime.Now;
                    descartesAutorizacoes[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                    descartesAutorizacoes[i].Motivo = motivo;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, descartesAutorizacoes[i], null, "Reprovou a regra. Motivo: " + descartesAutorizacoes[i].Motivo, unitOfWork);

                    // Atualiza banco
                    repAprovacaoAlcadaDescarteLoteProduto.Atualizar(descartesAutorizacoes[i]);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, descartesAutorizacoes[i].Descarte, null, "Reprovou múltiplas regras", unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosDescartesVerificados)
                    this.VerificarSituacaoDescarte(repDescarteLoteProdutoEmbarcador.BuscarPorCodigo(cod), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = descartesAutorizacoes.Count()
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
                Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigoDescarte);

                Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte = repDescarteLoteProdutoEmbarcador.BuscarPorCodigo(codigoDescarte);
                List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> descartesAutorizacoes = repAprovacaoAlcadaDescarteLoteProduto.BuscarPorDescarteUsuarioSituacao(codigoDescarte, this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                if (!Servicos.Embarcador.ProdutoEmbarcador.Lote.ValidaDescarteLote(descarte, unitOfWork))
                    return new JsonpResult(false, "Não é possível aprovar o descarte por não conter estoque suficiente.");

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < descartesAutorizacoes.Count(); i++)
                    EfetuarAprovacao(descartesAutorizacoes[i], false, unitOfWork);

                this.VerificarSituacaoDescarte(descarte, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = descartesAutorizacoes.Count()
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
                Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto alcada = repAprovacaoAlcadaDescarteLoteProduto.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (alcada == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida a situacao
                if (alcada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                if (!Servicos.Embarcador.ProdutoEmbarcador.Lote.ValidaDescarteLote(alcada.Descarte, unitOfWork))
                    return new JsonpResult(false, "Não é possível aprovar o descarte por não conter estoque suficiente.");

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
                Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

                // Codigo da regra
                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto descarteAutorizacao = repAprovacaoAlcadaDescarteLoteProduto.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (descarteAutorizacao == null || descarteAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (descarteAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                descarteAutorizacao.Data = DateTime.Now;
                descarteAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada;
                descarteAutorizacao.Motivo = motivo;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, descarteAutorizacao, null, "Repovou regra. Motivo: " + motivo, unitOfWork);

                // Atualiza banco
                repAprovacaoAlcadaDescarteLoteProduto.Atualizar(descarteAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, descarteAutorizacao.Descarte, unitOfWork);
                this.VerificarSituacaoDescarte(descarteAutorizacao.Descarte, unitOfWork);

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
            grid.Prop("Data").Nome("Data Descarte").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("Numero").Nome("Número").Tamanho(7);
            grid.Prop("ProdutoEmbarcador").Nome("Produto Embarcador").Tamanho(20);
            grid.Prop("Quantidade").Nome("Quantidade").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Motivo").Nome("Motivo").Tamanho(20);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Valor")
                propOrdena = "ValorAvaria";
        }

        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto descarte, bool verificarSeEstaAprovado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (descarte.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente && descarte.Usuario.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                descarte.Data = DateTime.Now;
                descarte.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada;

                // Atualiza os dados
                repAprovacaoAlcadaDescarteLoteProduto.Atualizar(descarte);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, descarte, null, "Aprovou a regra", unitOfWork);

                // Faz verificacao se a carga esta aprovada
                if (verificarSeEstaAprovado)
                    this.VerificarSituacaoDescarte(descarte.Descarte, unitOfWork);

                this.NotificarAlteracao(true, descarte.Descarte, unitOfWork);
            }
        }

        private List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> BuscarRegrasPorDescartes(List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> solicitacoes, int usuario, Repositorio.UnitOfWork unitOfWork, out bool descartesSemEstoque)
        {
            descartesSemEstoque = false;

            Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);
            List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> descarteAutorizacao = new List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto>();

            foreach (Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte in solicitacoes)
            {
                List<Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto> regras = repAprovacaoAlcadaDescarteLoteProduto.BuscarPorDescarteUsuarioSituacao(descarte.Codigo, usuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

                if (Servicos.Embarcador.ProdutoEmbarcador.Lote.ValidaDescarteLote(descarte, unitOfWork))
                    descarteAutorizacao.AddRange(regras);
                else
                    descartesSemEstoque = true;
            }

            return descarteAutorizacao;
        }

        private List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> ObterDescartesSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);
            List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> listaDescartes = new List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>();

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaDescartes, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaDescartesNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DescartesNaoSelecionados"));
                foreach (var dybDescartesNaoSelecionada in listaDescartesNaoSelecionados)
                    listaDescartes.Remove(new Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador() { Codigo = (int)dybDescartesNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaDescartesSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DescartesSelecionados"));
                foreach (var dynDescartesSelecionada in listaDescartesSelecionados)
                    listaDescartes.Add(repDescarteLoteProdutoEmbarcador.BuscarPorCodigo((int)dynDescartesSelecionada.Codigo));
            }

            // Retorna lista
            return listaDescartes;
        }

        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, Repositorio.UnitOfWork unitOfWork)
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
                string titulo = Localization.Resources.WMS.AutorizacaoDescarteLote.DescarteLote;
                string mensagem = string.Format(Localization.Resources.WMS.AutorizacaoDescarteLote.UsuarioDescarteQuantia, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou), descarte.Quantidade.ToString("n2"), descarte.Lote.ProdutoEmbarcador.Descricao);
                serNotificacao.GerarNotificacaoEmail(descarte.Usuario, this.Usuario, descarte.Codigo, "WMS/DescarteLoteProduto", titulo, mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void VerificarSituacaoDescarte(Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
                if (descarte.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.AgAprovacao)
                {
                    // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                    Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);
                    Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);
                    Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                    List<Dominio.Entidades.Embarcador.WMS.RegraDescarte> regras = repAprovacaoAlcadaDescarteLoteProduto.BuscarRegrasDescarte(descarte.Codigo);

                    // Flag de rejeicao
                    bool rejeitada = false;
                    bool aprovada = true;

                    foreach (Dominio.Entidades.Embarcador.WMS.RegraDescarte regra in regras)
                    {
                        int pendentes = repAprovacaoAlcadaDescarteLoteProduto.ContarPendentes(descarte.Codigo, regra.Codigo);

                        int aprovacoes = repAprovacaoAlcadaDescarteLoteProduto.ContarAprovacoesSolicitacao(descarte.Codigo, regra.Codigo);

                        int rejeitadas = repAprovacaoAlcadaDescarteLoteProduto.ContarRejeitadas(descarte.Codigo, regra.Codigo);

                        int necessariosParaAprovar = regra.NumeroAprovadores;

                        // Situacao
                        if (rejeitadas > 0)
                            rejeitada = true;
                        if (aprovacoes < necessariosParaAprovar)
                            aprovada = false;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.Finalizado;

                    if (rejeitada)
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.Rejeitada;

                    if (aprovada || rejeitada)
                    {
                        descarte.Situacao = situacao;
                        descarte.DataAprovacao = DateTime.Now;
                        descarte.UsuarioAprovador = this.Usuario;

                        repDescarteLoteProdutoEmbarcador.Atualizar(descarte);

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                        if (rejeitada)
                            icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                        else
                            icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                        // Emite notificação
                        string mensagem = string.Format(Localization.Resources.WMS.AutorizacaoDescarteLote.SolicitacaoDescarteQuantiaFoi, descarte.Quantidade.ToString("n2"), descarte.Lote.ProdutoEmbarcador.Descricao, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada));

                        serNotificacao.GerarNotificacao(descarte.Usuario, this.Usuario, descarte.Codigo, "WMS/DescarteLoteProduto", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                    }

                    Servicos.Embarcador.ProdutoEmbarcador.Lote.DescarteLoteAprovado(descarte, unitOfWork, Auditado, TipoServicoMultisoftware);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto regra)
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

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> listaDescartes, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Produto"), out int produto);
            int.TryParse(Request.Params("Usuario"), out int usuario);

            string numero = Request.Params("Numero") ?? string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador situacaoAux))
                situacao = situacaoAux;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            listaDescartes = repAprovacaoAlcadaDescarteLoteProduto.Consultar(usuario, dataInicial, dataFinal, situacao, numero, produto, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repAprovacaoAlcadaDescarteLoteProduto.ContarConsulta(usuario, dataInicial, dataFinal, situacao, numero, produto);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> listaDescartes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao repTempoEtapaSolicitacao = new Repositorio.Embarcador.Avarias.TempoEtapaSolicitacao(unitOfWork);
            Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

            var lista = from descarte in listaDescartes
                        select new
                        {
                            descarte.Codigo,
                            Data = descarte.Data.ToString("dd/MM/yyyy"),
                            Numero = descarte.Lote.Numero.ToString(),
                            ProdutoEmbarcador = descarte.Lote.ProdutoEmbarcador.Descricao,
                            Quantidade = descarte.Quantidade.ToString("n3"),
                            Motivo = descarte.Motivo,
                            Situacao = descarte.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto regra)
        {
            return regra.RegraDescarte?.Descricao;
        }

        #endregion
    }
}
