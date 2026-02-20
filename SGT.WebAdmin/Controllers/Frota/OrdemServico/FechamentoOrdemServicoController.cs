using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    public class FechamentoOrdemServicoController : BaseController
    {
		#region Construtores

		public FechamentoOrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterDetalhesGeraisFechamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesFechamentoOrdemServico(ordemServico, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os detalhes do fechamento da ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frota/OrdemServico");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                    if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.OrdemServico_NaoPermitirLancarDescontoFechamento))
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para aplicar desconto no fechamento.");

                int.TryParse(Request.Params("OrdemServico"), out int codigo);

                decimal.TryParse(Request.Params("Desconto"), out decimal desconto);

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (ordemServico.TipoOficina == TipoOficina.Interna)
                    return new JsonpResult(false, true, "Não é possível lançar um desconto para uma oficina interna.");

                if ((ordemServico.ProdutosFechamento.Sum(o => o.ValorDocumento) - desconto) < 0)
                    return new JsonpResult(false, true, "Este valor de desconto deixará a ordem de serviço com o valor total negativo, não sendo permitido.");

                ordemServico.Desconto = desconto;

                repOrdemServico.Atualizar(ordemServico);

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesFechamentoOrdemServico(ordemServico, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar os dados do fechamento.");
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
                int codigo = Request.GetIntParam("OrdemServico");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código Produto", "CodigoProduto", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Produto", "Produto", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 12, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Finalidade", "FinalidadeProduto", 12, Models.Grid.Align.left, true, true, true);
                if (ConfiguracaoEmbarcador.UtilizaMultiplosLocaisArmazenamento)
                    grid.AdicionarCabecalho("Local Armazenamento", "LocalArmazenamento", 12, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Quantidade", "QuantidadeDocumento", 5, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("UN", "UnidadeMedida", 8, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho("Valor Un.", "ValorUnitario", 8, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Valor", "ValorDocumento", 8, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Produto" || propOrdena == "FinalidadeProduto")
                    propOrdena += ".Descricao";

                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repProdutoFechamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);

                int countProdutos = repProdutoFechamento.ContarConsulta(codigo);

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> listaProdutoFechamento = new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto>();

                if (countProdutos > 0)
                    listaProdutoFechamento = repProdutoFechamento.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countProdutos);

                grid.AdicionaRows((from obj in listaProdutoFechamento
                                   select new
                                   {
                                       obj.Codigo,
                                       CodigoProduto = obj.Produto.CodigoProduto,
                                       Produto = obj.Produto.Descricao,
                                       Tipo = obj.Produto.UnidadeDeMedida?.ObterDescricao() ?? string.Empty,
                                       FinalidadeProduto = obj.FinalidadeProduto?.Descricao ?? string.Empty,
                                       LocalArmazenamento = obj.LocalArmazenamento?.Descricao ?? string.Empty,
                                       QuantidadeDocumento = obj.QuantidadeDocumento.ToString("n2"),
                                       UnidadeMedida = UnidadeDeMedidaHelper.ObterSigla(obj.Produto.UnidadeDeMedida),
                                       ValorDocumento = obj.ValorDocumento.ToString("n2"),
                                       ValorUnitario = obj.ValorUnitario.ToString("n2"),
                                       DT_RowColor = obj.CorSituacao,
                                       DT_FontColor = obj.Situacao == SituacaoProdutoFechamentoOrdemServicoFrota.NaoOrcado ? "#FFFFFF" : "#000000"
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

        public async Task<IActionResult> ConsultarServicos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("OrdemServico");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoOrdemServico", false);
                grid.AdicionarCabecalho("CodigoServico", false);
                grid.AdicionarCabecalho("Serviço", "Servico", 24, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Observação", "ObservacaoFechamento", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tempo Estimado", "TempoEstimado", 12, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho("Tempo Executado", "TempoExecutado", 12, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho("ServicoConcluido", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Servico")
                    propOrdena += ".Descricao";

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repOrdemServicoFrotaServicoVeiculo = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                int countProdutos = repOrdemServicoFrotaServicoVeiculo.ContarConsulta(codigo);

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> listaServicos = new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

                if (countProdutos > 0)
                    listaServicos = repOrdemServicoFrotaServicoVeiculo.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countProdutos);

                grid.AdicionaRows((from obj in listaServicos
                                   select new
                                   {
                                       obj.Codigo,
                                       CodigoOrdemServico = obj.OrdemServico.Codigo,
                                       CodigoServico = obj.Servico.Codigo,
                                       Servico = obj.Servico.Descricao,
                                       TempoEstimado = obj.TempoEstimado.ToString("n0") + " (min)",
                                       TempoExecutado = obj.TempoExecutado.ToString("n0") + " (min)",
                                       obj.ServicoConcluido,
                                       obj.ObservacaoFechamento,
                                       DT_RowColor = obj.ServicoConcluido == ServicoVeiculoExecutado.Executado ? CorGrid.Blue : obj.ServicoConcluido == ServicoVeiculoExecutado.NaoExecutado ? CorGrid.Vermelho : obj.TempoExecutado == 0 ? CorGrid.Branco : obj.TempoEstimado < obj.TempoExecutado ? CorGrid.Amarelo : CorGrid.Verde,
                                       DT_FontColor = obj.ServicoConcluido != ServicoVeiculoExecutado.NaoDefinido ? CorGrid.Branco : CorGrid.Black
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

        public async Task<IActionResult> ConsultarServicosTempoExecucao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("OrdemServico");
                int codigoServico = Request.GetIntParam("Servico");
                int codigoManutencao = Request.GetIntParam("Manutencao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoOrdemServico", false);
                grid.AdicionarCabecalho("CodigoServico", false);
                grid.AdicionarCabecalho("CodigoMecanico", false);
                grid.AdicionarCabecalho("Serviço", "Servico", 24, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mecânico", "Mecanico", 24, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Hora Início", "HoraInicio", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Hora Fim", "HoraFim", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tempo", "TempoExecutado", 10, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Servico")
                    propOrdena += ".Descricao";
                else if (propOrdena == "Mecanico")
                    propOrdena += ".Nome";

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao repOrdemServicoFrotaServicoVeiculoTempoExecucao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao(unidadeTrabalho);

                int countProdutos = repOrdemServicoFrotaServicoVeiculoTempoExecucao.ContarConsulta(codigo, codigoServico, codigoManutencao);

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao> listaServicos = new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao>();

                if (countProdutos > 0)
                    listaServicos = repOrdemServicoFrotaServicoVeiculoTempoExecucao.Consultar(codigo, codigoServico, codigoManutencao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countProdutos);

                grid.AdicionaRows((from obj in listaServicos
                                   select new
                                   {
                                       obj.Codigo,
                                       CodigoOrdemServico = obj.OrdemServico.Codigo,
                                       CodigoServico = obj.Servico.Codigo,
                                       CodigoMecanico = obj.Mecanico.Codigo,
                                       Servico = obj.Servico.Descricao,
                                       Mecanico = obj.Mecanico.Nome,
                                       Data = obj.Data.ToString("dd/MM/yyyy"),
                                       HoraInicio = obj.HoraInicio?.ToString(@"hh\:mm") ?? string.Empty,
                                       HoraFim = obj.HoraFim?.ToString(@"hh\:mm") ?? string.Empty,
                                       TempoExecutado = obj.TempoExecutado.ToString("n0") + " (min)"
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

        public async Task<IActionResult> ConsultarDocumentos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("OrdemServico");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 45, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho("Valor", "ValorTotal", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 15, Models.Grid.Align.left, false, true, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Numero" || propOrdena == "ValorTotal")
                    propOrdena = "DocumentoEntrada." + propOrdena;
                else if (propOrdena == "Fornecedor")
                    propOrdena += ".Nome";

                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento repDocumentoFechamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento(unidadeTrabalho);

                int countDocumentos = repDocumentoFechamento.ContarConsulta(codigo);

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento> listaDocumentoFechamento = new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento>();

                if (countDocumentos > 0)
                    listaDocumentoFechamento = repDocumentoFechamento.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countDocumentos);

                grid.AdicionaRows((from obj in listaDocumentoFechamento
                                   select new
                                   {
                                       obj.Codigo,
                                       Numero = obj.DocumentoEntrada.Numero + " - " + obj.DocumentoEntrada.Serie,
                                       ValorTotal = obj.DocumentoEntrada.ValorTotal.ToString("n2"),
                                       Fornecedor = obj.DocumentoEntrada.Fornecedor?.Nome ?? string.Empty,
                                       Tipo = "NF-e"
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

        public async Task<IActionResult> LiberarVeiculoDaManutencao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo, true);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (ordemServico.Situacao != SituacaoOrdemServicoFrota.EmManutencao)
                    return new JsonpResult(false, true, "A ordem de serviço não se encontra em manutenção.");

                if (ordemServico.TipoOrdemServico != null && ordemServico.TipoOrdemServico.InformarMotivoLiberarVeiculoManutencao && string.IsNullOrEmpty(motivo))
                    return new JsonpResult(false, true, "É obrigatório informar o motivo de liberação para esse tipo de ordem de serviço");

                unidadeTrabalho.Start();
                ordemServico.DataLiberacao = DateTime.Now;
                ordemServico.Situacao = SituacaoOrdemServicoFrota.AgNotaFiscal;
                ordemServico.MotivoLiberacaoVeiculo = motivo;
                repOrdemServico.Atualizar(ordemServico);

                if (ordemServico.Veiculo != null)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(ordemServico.Veiculo.Codigo, true);
                    veiculo.SituacaoVeiculo = SituacaoVeiculo.Disponivel;
                    repVeiculo.Atualizar(veiculo, Auditado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ordemServico, null, "Liberado veículo/equipamento da manutenção antes de fechar a O.S.", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new
                {
                    OrdemServico = Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico),
                    Fechamento = Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesFechamentoOrdemServico(ordemServico, unidadeTrabalho)
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao liberar da manutenção.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frota/OrdemServico");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Finalizar))
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para finalizar a Ordem de Serviço.");

                int codigo = Request.GetIntParam("OrdemServico");
                DateTime? dataFechamento = Request.GetNullableDateTimeParam("DataFechamentoEditavel");

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo, true);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                // OS Interna

                if (ordemServico.TipoOficina == TipoOficina.Interna)
                {

                    if (!repOrdemServico.ContemProdutoLancado(codigo) && !repOrdemServico.ContemTempoServico(codigo))
                        return new JsonpResult(false, true, "Nenhum produto ou tempo de serviço foi lançado. Por favor, verifique.");
                }

                // OS Externa

                if (ordemServico.TipoOficina == TipoOficina.Externa)
                {
                    if (!repOrdemServico.ContemDocumentoEntrada(codigo))
                        return new JsonpResult(false, true, "Nenhum documento de entrada foi vinculado.");
                }

                string erro = string.Empty;

                unidadeTrabalho.Start();

                if (!Servicos.Embarcador.Frota.OrdemServico.FinalizarOrdemServico(out erro, ref ordemServico, Usuario, unidadeTrabalho, TipoServicoMultisoftware, Auditado, dataFechamento))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ordemServico, null, "Finalizado", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new
                {
                    OrdemServico = Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico),
                    Fechamento = Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesFechamentoOrdemServico(ordemServico, unidadeTrabalho)
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao fechar a ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reabrir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frota/OrdemServico");
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && !this.Usuario.UsuarioAdministrador)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ReAbrir))
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para reabrir a Ordem de Serviço.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo, true);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                string erro = string.Empty;

                unidadeTrabalho.Start();
                ordemServico.DataReabertura = DateTime.Now;

                if (ordemServico.Situacao == SituacaoOrdemServicoFrota.AgAutorizacao)
                {
                    ordemServico.Situacao = SituacaoOrdemServicoFrota.EmDigitacao;
                    ordemServico.DataAlteracao = DateTime.Now;
                }
                else if (ordemServico.Situacao == SituacaoOrdemServicoFrota.EmManutencao || ordemServico.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal)
                {
                    ordemServico.Situacao = SituacaoOrdemServicoFrota.AgAutorizacao;
                    ordemServico.DataAlteracao = DateTime.Now;
                }
                else if (!Servicos.Embarcador.Frota.OrdemServico.ReabrirOrdemServico(out erro, ref ordemServico, Usuario, unidadeTrabalho, TipoServicoMultisoftware))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ordemServico, null, "Reaberto", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new
                {
                    OrdemServico = Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico),
                    Fechamento = Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesFechamentoOrdemServico(ordemServico, unidadeTrabalho),
                    ordemServico.Situacao
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao reabrir a ordem de serviço.");
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
                unidadeTrabalho.Start();
                string erro;
                int.TryParse(Request.Params("Produto"), out int codigoProduto);
                int.TryParse(Request.Params("OrdemServico"), out int codigoOrdemServico);
                int.TryParse(Request.Params("FinalidadeProduto"), out int codigoFinalidadeProduto);
                int.TryParse(Request.Params("LocalArmazenamento"), out int codigoLocalArmazenamento);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);
                decimal.TryParse(Request.Params("ValorUnitario"), out decimal valorUnitario);
                decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);

                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);
                Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProduto = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unidadeTrabalho);
                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unidadeTrabalho);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigoOrdemServico);

                if (ordemServico.Situacao != SituacaoOrdemServicoFrota.EmManutencao && ordemServico.Situacao != SituacaoOrdemServicoFrota.AgNotaFiscal)
                    return new JsonpResult(false, true, "Não é possível adicionar um produto ao fechamento na situação atual da ordem de serviço.");

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto fechamentoProduto = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto();
                fechamentoProduto.Autorizado = true;
                fechamentoProduto.Garantia = false;
                fechamentoProduto.Origem = TipoLancamento.Manual;
                fechamentoProduto.OrdemServico = ordemServico;
                fechamentoProduto.Produto = repProduto.BuscarPorCodigo(codigoProduto);
                fechamentoProduto.FinalidadeProduto = repFinalidadeProduto.BuscarPorCodigo(codigoFinalidadeProduto);
                fechamentoProduto.QuantidadeDocumento = quantidade;
                fechamentoProduto.ValorDocumento = valor;
                fechamentoProduto.ValorUnitario = valorUnitario;
                fechamentoProduto.LocalArmazenamento = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;

                if (!servicoEstoque.MovimentarEstoqueReserva(out erro, fechamentoProduto.Produto, quantidade, Dominio.Enumeradores.TipoMovimento.Entrada, ordemServico.Empresa, DateTime.Now, TipoServicoMultisoftware, ordemServico.LocalManutencao, fechamentoProduto.LocalArmazenamento))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                repFechamentoProduto.Inserir(fechamentoProduto);

                unidadeTrabalho.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamentoProduto.OrdemServico, $"Adicionou o produto {fechamentoProduto.Produto.Descricao}", unidadeTrabalho);

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesFechamentoOrdemServico(fechamentoProduto.OrdemServico, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeTrabalho.Rollback();
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
                unidadeTrabalho.Start();
                string erro;
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Produto"), out int codigoProduto);
                int.TryParse(Request.Params("FinalidadeProduto"), out int codigoFinalidadeProduto);
                int.TryParse(Request.Params("LocalArmazenamento"), out int codigoLocalArmazenamento);

                decimal.TryParse(Request.Params("Valor"), out decimal valor);
                decimal.TryParse(Request.Params("ValorUnitario"), out decimal valorUnitario);
                decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);

                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);
                Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico repFinalidadeProduto = new Repositorio.Embarcador.Frota.FinalidadeProdutoOrdemServico(unidadeTrabalho);
                Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unidadeTrabalho);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto fechamentoProduto = repFechamentoProduto.BuscarPorCodigo(codigo);

                if (fechamentoProduto.OrdemServico.Situacao != SituacaoOrdemServicoFrota.EmManutencao && fechamentoProduto.OrdemServico.Situacao != SituacaoOrdemServicoFrota.AgNotaFiscal)
                    return new JsonpResult(false, true, "Não é possível atualizar um produto do fechamento na situação atual da ordem de serviço.");

                if (fechamentoProduto.Origem != TipoLancamento.Manual)
                    return new JsonpResult(false, true, "Não é possível atualizar este produto, pois ele não foi adicionado manualmente.");

                fechamentoProduto.Initialize();

                Dominio.Enumeradores.TipoMovimento estoqueEntradaSaida = Dominio.Enumeradores.TipoMovimento.Saida;
                bool teveAlteracao = false;
                decimal quantidadeDiferencaAtualizar = 0;
                if (fechamentoProduto.QuantidadeDocumento > quantidade)
                {
                    estoqueEntradaSaida = Dominio.Enumeradores.TipoMovimento.Saida;
                    teveAlteracao = true;
                    quantidadeDiferencaAtualizar = fechamentoProduto.QuantidadeDocumento - quantidade;
                }
                else if (fechamentoProduto.QuantidadeDocumento < quantidade)
                {
                    estoqueEntradaSaida = Dominio.Enumeradores.TipoMovimento.Entrada;
                    teveAlteracao = true;
                    quantidadeDiferencaAtualizar = quantidade - fechamentoProduto.QuantidadeDocumento;
                }

                fechamentoProduto.Autorizado = true;
                fechamentoProduto.Garantia = false;
                fechamentoProduto.Produto = repProduto.BuscarPorCodigo(codigoProduto);
                fechamentoProduto.FinalidadeProduto = repFinalidadeProduto.BuscarPorCodigo(codigoFinalidadeProduto);
                fechamentoProduto.QuantidadeDocumento = quantidade;
                fechamentoProduto.ValorDocumento = valor;
                fechamentoProduto.ValorUnitario = valorUnitario;
                fechamentoProduto.LocalArmazenamento = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;

                if (teveAlteracao)
                {
                    if (!servicoEstoque.MovimentarEstoqueReserva(out erro, fechamentoProduto.Produto, quantidadeDiferencaAtualizar, estoqueEntradaSaida, fechamentoProduto.OrdemServico.Empresa, DateTime.Now, TipoServicoMultisoftware, fechamentoProduto.OrdemServico.LocalManutencao, fechamentoProduto.LocalArmazenamento))
                    {
                        unidadeTrabalho.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                repFechamentoProduto.Atualizar(fechamentoProduto);

                unidadeTrabalho.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamentoProduto.OrdemServico, fechamentoProduto.GetChanges(), $"Atualizou os dados do produto {fechamentoProduto.Produto.Descricao}", unidadeTrabalho);

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesFechamentoOrdemServico(fechamentoProduto.OrdemServico, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeTrabalho.Rollback();
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
                string erro;
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto fechamentoProduto = repFechamentoProduto.BuscarPorCodigo(codigo);

                if (fechamentoProduto.OrdemServico.Situacao != SituacaoOrdemServicoFrota.EmManutencao && fechamentoProduto.OrdemServico.Situacao != SituacaoOrdemServicoFrota.AgNotaFiscal)
                    return new JsonpResult(false, true, "Não é possível excluir um produto do fechamento na situação atual da ordem de serviço.");

                if (fechamentoProduto.Origem != TipoLancamento.Manual)
                    return new JsonpResult(false, true, "Não é possível atualizar este produto, pois ele não foi adicionado manualmente.");

                if (!servicoEstoque.MovimentarEstoqueReserva(out erro, fechamentoProduto.Produto, fechamentoProduto.QuantidadeDocumento, Dominio.Enumeradores.TipoMovimento.Saida, fechamentoProduto.OrdemServico.Empresa, DateTime.Now, TipoServicoMultisoftware, fechamentoProduto.OrdemServico.LocalManutencao, fechamentoProduto.LocalArmazenamento))
                    return new JsonpResult(false, true, erro);

                repFechamentoProduto.Deletar(fechamentoProduto);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamentoProduto.OrdemServico, $"Excluiu o produto {fechamentoProduto.Produto.Descricao}", unidadeTrabalho);

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesFechamentoOrdemServico(fechamentoProduto.OrdemServico, unidadeTrabalho));
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

                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto fechamentoProduto = repFechamentoProduto.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    fechamentoProduto.Codigo,
                    fechamentoProduto.Autorizado,
                    fechamentoProduto.Garantia,
                    Produto = new { fechamentoProduto.Produto.Descricao, fechamentoProduto.Produto.Codigo },
                    FinalidadeProduto = new { Descricao = fechamentoProduto.FinalidadeProduto?.Descricao ?? string.Empty, Codigo = fechamentoProduto.FinalidadeProduto?.Codigo ?? 0 },
                    Quantidade = fechamentoProduto.QuantidadeDocumento.ToString("n2"),
                    Valor = fechamentoProduto.ValorDocumento.ToString("n2"),
                    ValorUnitario = fechamentoProduto.ValorUnitario.ToString("n2"),
                    fechamentoProduto.Origem,
                    LocalArmazenamento = new { Descricao = fechamentoProduto.LocalArmazenamento?.Descricao ?? string.Empty, Codigo = fechamentoProduto.LocalArmazenamento?.Codigo ?? 0 }
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

        public async Task<IActionResult> RetornarProdutoPorCodigoBarras()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoBarras = Request.Params("CodigoBarrasLocalizar");

                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);
                Servicos.Embarcador.Produto.Produto serProduto = new Servicos.Embarcador.Produto.Produto(unidadeTrabalho);

                codigoBarras = serProduto.ContornarLeituraCodigoBarras(codigoBarras);
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigoProduto(codigoBarras);

                if (produto == null)
                    return new JsonpResult(false, true, "Não foi encontrado o produto com o código de barras informado!");

                if (produto.FinalidadeProdutoOrdemServico == null)
                    return new JsonpResult(false, true, "O produto selecionado não possui finalidade de OS em seu cadastro!");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unidadeTrabalho);
                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoqueProduto = repProdutoEstoque.BuscarPorProduto(produto.Codigo, codigoEmpresa, produto.LocalArmazenamentoProduto?.Codigo ?? null);
                if (estoqueProduto == null)
                    return new JsonpResult(false, true, "Não foi encontrado estoque do produto selecionado!");

                return new JsonpResult(new
                {
                    Produto = new { produto.Descricao, produto.Codigo },
                    FinalidadeProduto = new { Descricao = produto.FinalidadeProdutoOrdemServico?.Descricao ?? string.Empty, Codigo = produto.FinalidadeProdutoOrdemServico?.Codigo ?? 0 },
                    ValorUnitario = estoqueProduto.CustoMedio.ToString("n2"),
                    LocalArmazenamento = new { Descricao = produto.LocalArmazenamentoProduto?.Descricao ?? string.Empty, Codigo = produto.LocalArmazenamentoProduto?.Codigo ?? 0 },
                    Quantidade = 1
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

        public async Task<IActionResult> BuscarTempoServicoExecucaoPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao repOrdemServicoFrotaServicoVeiculoTempoExecucao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao tempo = repOrdemServicoFrotaServicoVeiculoTempoExecucao.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    tempo.Codigo,
                    OrdemServico = tempo.OrdemServico.Codigo,
                    Servico = new
                    {
                        tempo.Servico.Descricao,
                        tempo.Servico.Codigo
                    },
                    Mecanico = new
                    {
                        Descricao = tempo.Mecanico.Nome,
                        tempo.Mecanico.Codigo
                    },
                    Data = tempo.Data.ToString("dd/MM/yyyy"),
                    HoraInicio = tempo.HoraInicio?.ToString(@"hh\:mm") ?? string.Empty,
                    HoraFim = tempo.HoraFim?.ToString(@"hh\:mm") ?? string.Empty,
                    Tempo = tempo.TempoExecutado.ToString("n0")
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados de tempo de execução.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarTempoServicoExecucao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("OrdemServico");
                int codigoServico = Request.GetIntParam("Servico");
                int codigoManutencao = Request.GetIntParam("Manutencao");
                int codigoMecanico = Request.GetIntParam("Mecanico");
                int tempo = Request.GetIntParam("Tempo");

                DateTime.TryParse(Request.Params("Data"), out DateTime data);
                TimeSpan.TryParse(Request.Params("HoraInicio"), out TimeSpan horaInicio);
                TimeSpan.TryParse(Request.Params("HoraFim"), out TimeSpan horaFim);

                TimeSpan? horaInicioAux = null, horaFimAux = null;
                if (horaInicio > TimeSpan.MinValue)
                    horaInicioAux = horaInicio;
                if (horaFim > TimeSpan.MinValue)
                    horaFimAux = horaFim;

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao repOrdemServicoFrotaServicoVeiculoTempoExecucao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao tempoExecucaoAberto = repOrdemServicoFrotaServicoVeiculoTempoExecucao.BuscarTempoExecucaoEmAberto(codigoMecanico, codigoServico, codigo, codigoManutencao);

                if (tempoExecucaoAberto != null)
                    return new JsonpResult(false, true, "Favor finalize o serviço em aberto antes de lançar um novo.");

                unidadeTrabalho.Start();

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repOrdemServicoFrotaServicoVeiculo = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao tempoExecucao = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao()
                {
                    Data = data,
                    HoraFim = horaFimAux,
                    HoraInicio = horaInicioAux,
                    Mecanico = repUsuario.BuscarPorCodigo(codigoMecanico),
                    OrdemServico = repOrdemServicoFrota.BuscarPorCodigo(codigo),
                    Servico = repServicoVeiculoFrota.BuscarPorCodigo(codigoServico),
                    Manutencao = repOrdemServicoFrotaServicoVeiculo.BuscarPorCodigo(codigoManutencao),
                    TempoExecutado = tempo
                };
                repOrdemServicoFrotaServicoVeiculoTempoExecucao.Inserir(tempoExecucao);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = repOrdemServicoFrotaServicoVeiculo.BuscarPorCodigo(codigoManutencao);
                if (servico != null && tempoExecucao.TempoExecutado > 0)
                {
                    servico.TempoExecutado += tempoExecucao.TempoExecutado;
                    repOrdemServicoFrotaServicoVeiculo.Atualizar(servico);
                }

                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o tempo de execução do serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarTempoServicoExecucao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoOrdemServico = Request.GetIntParam("OrdemServico");
                int codigoManutencao = Request.GetIntParam("Manutencao");
                int codigoServico = Request.GetIntParam("Servico");
                int codigoMecanico = Request.GetIntParam("Mecanico");
                int tempo = Request.GetIntParam("Tempo");

                DateTime.TryParse(Request.Params("Data"), out DateTime data);
                TimeSpan.TryParse(Request.Params("HoraInicio"), out TimeSpan horaInicio);
                TimeSpan.TryParse(Request.Params("HoraFim"), out TimeSpan horaFim);

                TimeSpan? horaInicioAux = null, horaFimAux = null;
                if (horaInicio > TimeSpan.MinValue)
                    horaInicioAux = horaInicio;
                if (horaFim > TimeSpan.MinValue)
                    horaFimAux = horaFim;

                unidadeTrabalho.Start();

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao repOrdemServicoFrotaServicoVeiculoTempoExecucao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repOrdemServicoFrotaServicoVeiculo = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao tempoExecucao = repOrdemServicoFrotaServicoVeiculoTempoExecucao.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = repOrdemServicoFrotaServicoVeiculo.BuscarPorCodigo(codigoManutencao);
                if (servico != null && tempoExecucao.TempoExecutado > 0)
                    servico.TempoExecutado -= tempoExecucao.TempoExecutado;

                tempoExecucao.Data = data;
                tempoExecucao.HoraFim = horaFimAux;
                tempoExecucao.HoraInicio = horaInicioAux;
                tempoExecucao.Mecanico = repUsuario.BuscarPorCodigo(codigoMecanico);
                tempoExecucao.OrdemServico = repOrdemServicoFrota.BuscarPorCodigo(codigoOrdemServico);
                tempoExecucao.Servico = repServicoVeiculoFrota.BuscarPorCodigo(codigoServico);
                tempoExecucao.TempoExecutado = tempo;

                repOrdemServicoFrotaServicoVeiculoTempoExecucao.Atualizar(tempoExecucao);

                if (servico != null && tempoExecucao.TempoExecutado > 0)
                {
                    servico.TempoExecutado += tempoExecucao.TempoExecutado;
                    repOrdemServicoFrotaServicoVeiculo.Atualizar(servico);
                }

                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o tempo de execução do serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirTempoServicoExecucao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoOrdemServico = Request.GetIntParam("OrdemServico");
                int codigoManutencao = Request.GetIntParam("Manutencao");
                int codigoServico = Request.GetIntParam("Servico");

                unidadeTrabalho.Start();

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao repOrdemServicoFrotaServicoVeiculoTempoExecucao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repOrdemServicoFrotaServicoVeiculo = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculoTempoExecucao tempoExecucao = repOrdemServicoFrotaServicoVeiculoTempoExecucao.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico = repOrdemServicoFrotaServicoVeiculo.BuscarPorCodigo(codigoManutencao);
                if (servico != null && tempoExecucao.TempoExecutado > 0)
                {
                    servico.TempoExecutado -= tempoExecucao.TempoExecutado;
                    repOrdemServicoFrotaServicoVeiculo.Atualizar(servico);
                }

                repOrdemServicoFrotaServicoVeiculoTempoExecucao.Deletar(tempoExecucao);

                unidadeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o tempo de execução do serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> InformarServicoConcluido()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                ServicoVeiculoExecutado servicoVeiculoExecutado = Request.GetEnumParam<ServicoVeiculoExecutado>("ServicoVeiculoExecutado");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = repServicoOrdemServico.BuscarPorCodigo(codigo);

                if (servicoOrdemServico == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                if (servicoOrdemServico.ServicoConcluido == servicoVeiculoExecutado)
                    return new JsonpResult(false, true, "Já está definido com a opção informada.");

                servicoOrdemServico.ServicoConcluido = servicoVeiculoExecutado;

                repServicoOrdemServico.Atualizar(servicoOrdemServico);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao informar serviço concluído na ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacaoFechamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = repServicoOrdemServico.BuscarPorCodigo(codigo);

                if (servicoOrdemServico == null)
                    return new JsonpResult(false, true, "Manutenção não encontrada.");

                servicoOrdemServico.ObservacaoFechamento = Request.GetStringParam("Observacao");

                repServicoOrdemServico.Atualizar(servicoOrdemServico);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a observação.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
