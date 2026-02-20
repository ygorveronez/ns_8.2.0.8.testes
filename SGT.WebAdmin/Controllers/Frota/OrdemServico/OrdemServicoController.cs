using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    [CustomAuthorize("Frota/OrdemServico")]
    public class OrdemServicoController : BaseController
    {
		#region Construtores

		public OrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "DataProgramada", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Equipamento", "Equipamento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Prioridade", "Prioridade", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número Frota", "NumeroFrota", 8, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 13, Models.Grid.Align.left, true, true, true);
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    grid.AdicionarCabecalho("Local de Manutenção", "LocalManutencao", 15, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("LocalManutencao", false);
                grid.AdicionarCabecalho("Operador", "Operador", 15, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Tipo de Manutenção", "TipoManutencao", 10, Models.Grid.Align.left, true, true, true);
                if (filtrosPesquisa.Situacao == null || filtrosPesquisa.Situacao.Count > 1)
                    grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true, true, true);
                else
                    grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("CodigoVeiculo", false);
                grid.AdicionarCabecalho("CodigoMotorista", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("VeiculoEquipamento", false);
                grid.AdicionarCabecalho("CodigoEquipamento", false);
                grid.AdicionarCabecalho("QuilometragemVeiculo", false);
                grid.AdicionarCabecalho("Guarita/OS", "GuaritaOS", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor OS", "ValorOS", 8, Models.Grid.Align.right, true, true, true);
                grid.AdicionarCabecalho("CodigoCentroResultado", false);
                grid.AdicionarCabecalho("CentroResultado", false);
                grid.AdicionarCabecalho("TipoOrdemServico", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Veiculo")
                    propOrdena += ".Placa";
                else if (propOrdena == "Motorista")
                    propOrdena += ".Nome";
                else if (propOrdena == "LocalManutencao")
                    propOrdena += ".Nome";
                else if (propOrdena == "Operador")
                    propOrdena += ".Nome";
                else if (propOrdena == "NumeroFrota")
                    propOrdena = "Veiculo.NumeroFrota";

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repOrdemServicoFechamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);

                int countOrdemServico = repOrdemServico.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> listaOrdemServico = new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

                if (countOrdemServico > 0)
                    listaOrdemServico = repOrdemServico.Consultar(filtrosPesquisa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countOrdemServico);


                grid.AdicionaRows((from obj in listaOrdemServico
                                   select new
                                   {
                                       obj.Codigo,
                                       Numero = obj.Numero.ToString(),
                                       DataProgramada = obj.DataProgramada.ToString("dd/MM/yyyy"),
                                       Veiculo = obj.Veiculo?.Placa ?? string.Empty,
                                       Equipamento = obj.Equipamento?.Descricao ?? string.Empty,
                                       Motorista = obj.Motorista?.Nome ?? string.Empty,
                                       LocalManutencao = obj.LocalManutencao?.Nome ?? string.Empty,
                                       Operador = obj.Operador.Nome,
                                       TipoManutencao = obj.DescricaoTipoManutencao,
                                       Situacao = obj.DescricaoSituacao,
                                       CodigoVeiculo = obj.Veiculo?.Codigo ?? 0,
                                       CodigoMotorista = obj.Motorista?.Codigo ?? 0,
                                       obj.Descricao,
                                       NumeroFrota = obj.Veiculo?.NumeroFrota ?? string.Empty,
                                       VeiculoEquipamento = (obj.Veiculo?.Placa ?? "") + " " + (obj.Equipamento?.Descricao ?? ""),
                                       CodigoEquipamento = obj.Equipamento?.Codigo ?? 0,
                                       obj.QuilometragemVeiculo,
                                       GuaritaOS = this.ObterGuaritaOS(obj.Codigo, unidadeTrabalho),
                                       ValorOS = obj.Orcamento?.ValorTotalOrcado.ToString("n2") ?? 0.ToString("n2"),
                                       CodigoCentroResultado = obj.Veiculo?.CentroResultado?.Codigo ?? 0,
                                       CentroResultado = obj.Veiculo?.CentroResultado?.Descricao ?? string.Empty,
                                       Prioridade = obj.Prioridade.HasValue ? obj.Prioridade.Value.ObterDescricao() : "",
                                       DT_RowClass =
                                            obj.Situacao == SituacaoOrdemServicoFrota.Finalizada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Sucess(IntensidadeCor._100) :
                                            obj.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Info(IntensidadeCor._100) :
                                            obj.TipoOficina == TipoOficina.Externa ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Danger(IntensidadeCor._100) :
                                            obj.TipoOficina == TipoOficina.Interna ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Warning(IntensidadeCor._100) :
                                            string.Empty,
                                       TipoOrdemServico = obj.TipoOrdemServico?.Descricao ?? string.Empty
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeTrabalho.Start();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unidadeTrabalho);

                Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico = new Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico();
                PreencherObjetoOrdemServico(objetoOrdemServico, unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = Servicos.Embarcador.Frota.OrdemServico.AbrirOrdemServico(objetoOrdemServico, Auditado, unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();
                int kilometragemAtualTotal = ((ordemServico?.Veiculo?.KilometragemAtual ?? 0) + (configuracaoVeiculo.KMLimiteAberturaOrdemServico));

                if (configuracaoVeiculo.KMLimiteAberturaOrdemServico > 0 && ordemServico.Veiculo != null)
                    if (ordemServico.QuilometragemVeiculo > kilometragemAtualTotal)
                        throw new ControllerException($"O Veículo superou o limite configurado de {kilometragemAtualTotal}.");

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico));
            }
            catch (BaseException ex)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar a ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
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

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações da ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarExecucaoServicos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                unidadeTrabalho.Start();

                Servicos.Embarcador.Frota.OrdemServico.ConfirmarExecucaoServicos(ordemServico, unidadeTrabalho);

                Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico));
            }
            catch (ServicoException ex)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao confirmar a execução dos serviços.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AutorizarOrcamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento repDocumentoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento(unidadeTrabalho);
                Servicos.Embarcador.Frota.OrdemServico servicoOrdemServico = new Servicos.Embarcador.Frota.OrdemServico(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (ordemServico.Situacao != SituacaoOrdemServicoFrota.AgAutorizacao)
                    return new JsonpResult(false, true, "Não é possível autorizar o orçamento na situação atual da ordem de serviço.");

                unidadeTrabalho.Start();

                ordemServico.DataAlteracao = DateTime.Now;

                servicoOrdemServico.EtapaAprovacao(ordemServico, TipoServicoMultisoftware, ConfiguracaoEmbarcador.BloquearSemRegraAprovacaoOrdemServico);

                repOrdemServico.Atualizar(ordemServico);

                int countDocumentosOrdemServico = repDocumentoOrdemServico.QuantidadeDocumentosOrdemServico(codigo);
                if (countDocumentosOrdemServico > 0)//Quando existe documentos, que são oriundos de Documento de Entrada, refaz a verificação
                    Servicos.Embarcador.Frota.OrdemServico.RefazerProdutosFechamento(codigo, unidadeTrabalho);

                Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao autorizar o orçamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarOrcamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string motivo = Request.Params("Motivo");

                if (string.IsNullOrWhiteSpace(motivo) || motivo.Length < 25)
                    return new JsonpResult(false, true, "Informe o motivo pelo qual o orçamento foi rejeitado (25 ou mais caracteres).");

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (ordemServico.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota.AgAutorizacao)
                    return new JsonpResult(false, true, "Não é possível rejeitar o orçamento na situação atual da ordem de serviço.");

                unidadeTrabalho.Start();

                ordemServico.DataAlteracao = DateTime.Now;
                ordemServico.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota.Rejeitada;
                ordemServico.Motivo = motivo;

                repOrdemServico.Atualizar(ordemServico);

                Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao rejeitar o orçamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivo = Request.Params("Motivo");

                if (string.IsNullOrWhiteSpace(motivo) || motivo.Length < 25)
                    return new JsonpResult(false, true, "Informe o motivo pelo qual a ordem de serviço foi cancelada (25 ou mais caracteres).");

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo, true);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (ordemServico.Situacao == SituacaoOrdemServicoFrota.Finalizada || ordemServico.Situacao == SituacaoOrdemServicoFrota.Rejeitada ||
                    ordemServico.Situacao == SituacaoOrdemServicoFrota.Cancelada || ordemServico.Situacao == SituacaoOrdemServicoFrota.AprovacaoRejeitada)
                    return new JsonpResult(false, true, "Não é possível cancelar a ordem de serviço na situação atual.");

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaDocumentosEntrada = repDocumentoEntrada.BuscarPorOrdemServico(codigo);
                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada in listaDocumentosEntrada)
                {
                    if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Aberto || documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
                        return new JsonpResult(false, true, "Não é possível cancelar a ordem de serviço quando ela está em um Documento de Entrada.");
                }

                unidadeTrabalho.Start();

                ordemServico.DataAlteracao = DateTime.Now;
                ordemServico.SituacaoAnteriorCancelamento = ordemServico.Situacao;
                ordemServico.Situacao = SituacaoOrdemServicoFrota.Cancelada;
                ordemServico.Motivo = motivo;

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> fechamentoProdutos = repFechamentoProduto.BuscarPorOrdemServico(ordemServico.Codigo);
                if (fechamentoProdutos != null && fechamentoProdutos.Count > 0)
                {
                    string erro;
                    foreach (var fechamentoProduto in fechamentoProdutos)
                    {
                        if (!servicoEstoque.MovimentarEstoqueReserva(out erro, fechamentoProduto.Produto, fechamentoProduto.QuantidadeDocumento, Dominio.Enumeradores.TipoMovimento.Saida, ordemServico.Empresa, DateTime.Now, TipoServicoMultisoftware, ordemServico.LocalManutencao, fechamentoProduto.LocalArmazenamento))
                            return new JsonpResult(false, true, erro);
                    }
                }

                if (VeiculoSeEncontraEmManutencaoEmOutraOrdemServico(ordemServico, unidadeTrabalho))
                {
                    Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unidadeTrabalho);
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);

                    Dominio.Entidades.Veiculo veiculo = ordemServico.Veiculo;

                    veiculo.SituacaoVeiculo = SituacaoVeiculo.Disponivel;
                    veiculo.VeiculoVazio = true;
                    veiculo.DataHoraPrevisaoDisponivel = null;
                    veiculo.LocalidadeAtual = null;
                    veiculo.AvisadoCarregamento = false;
                    repVeiculo.Atualizar(veiculo);

                    Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacao = new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();
                    situacao.DataHoraEmissao = DateTime.Now;
                    situacao.DataHoraSituacao = DateTime.Now;
                    situacao.Veiculo = veiculo;
                    situacao.Localidade = null;
                    situacao.Motorista = null;
                    situacao.DataHoraRetornoViagem = DateTime.Now.Date;
                    situacao.LocalidadeRetornoViagem = null;
                    situacao.Situacao = SituacaoVeiculo.Disponivel;

                    repSituacaoVeiculo.Inserir(situacao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, situacao, null, "Cancelou ordem de serviço", unidadeTrabalho);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Cancelou ordem de serviço", unidadeTrabalho);
                }

                repOrdemServico.Atualizar(ordemServico);

                if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                    Servicos.Embarcador.Frota.OrdemServicoManutencao.ReverterAtualizacaoKMVeiculoPneu(ordemServico, unidadeTrabalho, Auditado);

                Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ordemServico, null, "Cancelado", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Frota.OrdemServico.ObterDetalhesOrdemServico(ordemServico));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar a ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailOrdemServico()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (string.IsNullOrWhiteSpace(ordemServico.LocalManutencao?.Email))
                    return new JsonpResult(false, true, "Não há endereço de e-mail configurado no local de manutenção!");

                if (ordemServico.Situacao != SituacaoOrdemServicoFrota.EmManutencao && ordemServico.Situacao != SituacaoOrdemServicoFrota.Finalizada)
                    return new JsonpResult(false, true, "Somente possível enviar OS por e-mail, caso o status seja Em manutenção ou Finalizada!");

                Servicos.Embarcador.Frota.OrdemServico servicoOrdemServico = new Servicos.Embarcador.Frota.OrdemServico(unidadeTrabalho);
                bool statusEnvioEmail = servicoOrdemServico.EnviarEmailOrdemServico(ordemServico);
                if (!statusEnvioEmail)
                    return new JsonpResult(statusEnvioEmail, true, "Problemas ao enviar e-mail da OS!");

                return new JsonpResult(statusEnvioEmail);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar e-mail com a ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadDetalhesOS()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                byte[] relatorio = Servicos.Embarcador.Frota.OrdemServico.GerarRelatorioDetalhesOS(codigo, unidadeTrabalho);

                return Arquivo(relatorio, "application/pdf", "Ordem de Serviço nº " + ordemServico.Numero + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório da ordem de serviço.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SugerirGrupoServico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frota.GrupoServico repGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoEquipamento = Request.GetIntParam("Equipamento");
                int km = Request.GetIntParam("QuilometragemVeiculo");
                int horimetro = Request.GetIntParam("Horimetro");

                Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = codigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(codigoEquipamento) : null;

                Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico = repGrupoServico.BuscarGrupoServicoSugerido(km, horimetro, veiculo, equipamento);

                var dynGrupoServico = new
                {
                    Codigo = grupoServico?.Codigo ?? 0,
                    Descricao = grupoServico?.Descricao ?? string.Empty
                };

                return new JsonpResult(dynGrupoServico);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao sugerir grupo de serviço.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarPorLote()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeTrabalho.Start();

                string numerosOS = PreencherLoteObjetoOrdemServico(unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new { Numero = numerosOS });
            }
            catch (ControllerException ex)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (ServicoException ex)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a ordem de serviço em lote.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarObservacao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo, true);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (ordemServico.Situacao == SituacaoOrdemServicoFrota.Finalizada || ordemServico.Situacao == SituacaoOrdemServicoFrota.Cancelada ||
                    ordemServico.Situacao == SituacaoOrdemServicoFrota.Rejeitada || ordemServico.Situacao == SituacaoOrdemServicoFrota.AprovacaoRejeitada)
                    return new JsonpResult(false, true, "Não é possível atualizar na situação atual da ordem de serviço.");

                unidadeTrabalho.Start();

                ordemServico.DataAlteracao = DateTime.Now;
                ordemServico.Observacao = observacao;

                repOrdemServico.Atualizar(ordemServico, Auditado);

                Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarLocalManutencao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("LocalManutencao")), out double cpfCnpjLocalManutencao);

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigo, true);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Ordem de serviço não encontrada.");

                if (ordemServico.Situacao == SituacaoOrdemServicoFrota.AgAutorizacao || ordemServico.Situacao == SituacaoOrdemServicoFrota.EmDigitacao)
                {
                    unidadeTrabalho.Start();
                    ordemServico.DataAlteracao = DateTime.Now;
                    ordemServico.LocalManutencao = cpfCnpjLocalManutencao > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjLocalManutencao) : null;
                    repOrdemServico.Atualizar(ordemServico, Auditado);
                    Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unidadeTrabalho);
                    unidadeTrabalho.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, true, "O Status da Ordem não permite alteração!");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o local da manutenção.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ValidarOrdemEmAndamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);

                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoEquipamento = Request.GetIntParam("Equipamento");

                if (codigoVeiculo == 0 && codigoEquipamento == 0)
                    return new JsonpResult(false, true, "Favor selecionar um veículo ou um equipamento.");

                if (repOrdemServico.ContemOrdemEmAndamento(codigoVeiculo, codigoEquipamento))
                    return new JsonpResult(new { ExisteOutraOS = true });

                return new JsonpResult(new { ExisteOutraOS = false });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar ordem em andamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DadosUsuarioLogado()
        {
            try
            {
                var dadosUsuarioLogado = new
                {
                    Usuario.Codigo,
                    Usuario.Nome,
                    CpfCnpjCliente = Usuario.Cliente?.CPF_CNPJ ?? 0d,
                    NomeCliente = Usuario.Cliente?.Nome ?? string.Empty
                };

                return new JsonpResult(dadosUsuarioLogado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados do usuário");
            }
        }

        #endregion

        #region Importação

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoProduto()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Data", Propriedade = "Data", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Veículo", Propriedade = "Veiculo", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Equipamento", Propriedade = "Equipamento", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "KM Atual", Propriedade = "KMAtual", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Horimetro Atual", Propriedade = "HorimetroAtual", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "CPF Motorista", Propriedade = "Motorista", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 500, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Serviço", Propriedade = "Servico", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "CustoServico", Propriedade = "Custo", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Tipo OS", Propriedade = "Tipo", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Local Manutenção", Propriedade = "Local", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Valor Orçado", Propriedade = "ValorOrcado", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProduto();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProduto();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                        Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                        Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unitOfWork);
                        Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repOrdemServicoFrotaTipo = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);

                        Dominio.Entidades.Usuario motorista = null;
                        Dominio.Entidades.Veiculo veiculo = null;
                        Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = null;
                        Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoVeiculo = null;
                        Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo tipoOrdemServico = null;
                        Dominio.Entidades.Cliente local = null;
                        DateTime dataOrdem = DateTime.Now;
                        int kmAtual = 0;
                        int horimetroAtual = 0;
                        string observacao = "";
                        decimal custoServico = 0;
                        decimal valorOrcado = 0;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colData = (from obj in linha.Colunas where obj.NomeCampo == "Data" select obj).FirstOrDefault();
                        DateTime.TryParseExact(colData?.Valor ?? string.Empty, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataOrdem);
                        if (dataOrdem == DateTime.MinValue)
                            DateTime.TryParseExact(colData?.Valor ?? string.Empty, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataOrdem);

                        if (dataOrdem == DateTime.MinValue)
                            dataOrdem = DateTime.Now;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVeiculo = (from obj in linha.Colunas where obj.NomeCampo == "Veiculo" select obj).FirstOrDefault();
                        if (colVeiculo != null && colVeiculo.Valor != null)
                            veiculo = repVeiculo.BuscarPorPlaca(colVeiculo.Valor.Trim());

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEquipamento = (from obj in linha.Colunas where obj.NomeCampo == "Equipamento" select obj).FirstOrDefault();
                        if (colEquipamento != null && colEquipamento.Valor != null)
                            equipamento = repEquipamento.BuscarPorDescricao(colEquipamento.Valor.Trim());

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colKMAtual = (from obj in linha.Colunas where obj.NomeCampo == "KMAtual" select obj).FirstOrDefault();
                        if (colKMAtual != null && colKMAtual.Valor != null)
                            int.TryParse(colKMAtual.Valor.Trim(), out kmAtual);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHorimetroAtual = (from obj in linha.Colunas where obj.NomeCampo == "HorimetroAtual" select obj).FirstOrDefault();
                        if (colHorimetroAtual != null && colHorimetroAtual.Valor != null)
                            int.TryParse(colHorimetroAtual.Valor.Trim(), out horimetroAtual);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMotorista = (from obj in linha.Colunas where obj.NomeCampo == "Motorista" select obj).FirstOrDefault();
                        if (colMotorista != null && colMotorista.Valor != null)
                            motorista = repUsuario.BuscarPorCPF(colMotorista.Valor.Trim());

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                        if (colObservacao != null && colObservacao.Valor != null)
                            observacao = colObservacao.Valor.Trim();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colServico = (from obj in linha.Colunas where obj.NomeCampo == "Servico" select obj).FirstOrDefault();
                        if (colServico != null && colServico.Valor != null)
                            servicoVeiculo = repServicoVeiculoFrota.BuscarPorDescricao(colServico.Valor.Trim());

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCustoServico = (from obj in linha.Colunas where obj.NomeCampo == "CustoServico" select obj).FirstOrDefault();
                        if (colCustoServico != null && colCustoServico.Valor != null)
                            decimal.TryParse(colCustoServico.Valor.Trim(), out custoServico);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorOrcado = (from obj in linha.Colunas where obj.NomeCampo == "ValorOrcado" select obj).FirstOrDefault();
                        if (colValorOrcado != null && colValorOrcado.Valor != null)
                            decimal.TryParse(colValorOrcado.Valor.Trim(), out valorOrcado);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipo = (from obj in linha.Colunas where obj.NomeCampo == "Tipo" select obj).FirstOrDefault();
                        if (colTipo != null && colTipo.Valor != null)
                            tipoOrdemServico = repOrdemServicoFrotaTipo.BuscarPorDescricao(colTipo.Valor.Trim());

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocal = (from obj in linha.Colunas where obj.NomeCampo == "Local" select obj).FirstOrDefault();
                        if (colLocal != null && colLocal.Valor != null)
                            local = repCliente.BuscarPorCPFCNPJ(double.Parse(colLocal.Valor.Trim()));

                        if (veiculo == null && equipamento == null)
                            retorno += " Não foi localizado nenhum veículo e equipamento.";
                        if (servicoVeiculo == null)
                            retorno += " Não foi localizado nenhum serviço.";
                        if (tipoOrdemServico == null)
                            retorno += " Não foi localizado tipo de OS.";
                        if (local == null)
                            retorno += " Não foi localizado nenhum local de manutenção.";

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico = new Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico()
                            {
                                CadastrandoVeiculoEquipamento = false,
                                Observacao = observacao,
                                Operador = this.Usuario,
                                Veiculo = veiculo,
                                Equipamento = equipamento,
                                QuilometragemVeiculo = kmAtual,
                                Horimetro = horimetroAtual,
                                ServicoVeiculo = servicoVeiculo,
                                DataManutencao = dataOrdem,
                                DataProgramada = dataOrdem,
                                TipoOrdemServico = tipoOrdemServico,
                                LocalManutencao = local,
                                Empresa = null,
                                Motorista = motorista,
                                Custo = custoServico,
                                ValorOrcado = valorOrcado
                            };

                            Servicos.Embarcador.Frota.OrdemServico.GerarFinalizarOrdemServicoCompleta(objetoOrdemServico, Usuario, Auditado, unitOfWork, TipoServicoMultisoftware);

                            contador++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha. " + ex2.Message, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        #endregion

        #region Métodos Privados

        private void PreencherObjetoOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico ordemServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unidadeTrabalho);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeTrabalho);
            Repositorio.Embarcador.Frota.GrupoServico repGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao repTipoLocalManutencao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao(unidadeTrabalho);


            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            long.TryParse(Request.Params("Tipo"), out long codigoTipoOrdemServico);
            long.TryParse(Request.Params("TipoLocalManutencao"), out long codigoTipoLocalManutencao);
            int.TryParse(Request.Params("CentroResultado"), out int codigoCentroResultado);
            int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
            int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
            int.TryParse(Request.Params("Equipamento"), out int codigoEquipamento);
            int.TryParse(Request.Params("Horimetro"), out int horimetro);
            int.TryParse(Request.Params("QuilometragemVeiculo"), out int kmAtual);
            int codigoGrupoServico = Request.GetIntParam("GrupoServico");
            int codigoResponsavel = Request.GetIntParam("Responsavel");

            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("LocalManutencao")), out double cpfCnpjLocalManutencao);

            DateTime.TryParseExact(Request.Params("DataProgramada"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataProgramada);

            string observacao = Request.Params("Observacao");
            string condicaoPagamento = Request.Params("CondicaoPagamento");


            ordemServico.DataProgramada = dataProgramada;
            ordemServico.LocalManutencao = cpfCnpjLocalManutencao > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjLocalManutencao) : null;
            ordemServico.Motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;
            ordemServico.Observacao = observacao;
            ordemServico.CondicaoPagamento = condicaoPagamento;
            ordemServico.Operador = Usuario;
            ordemServico.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            ordemServico.Equipamento = codigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(codigoEquipamento) : null;
            ordemServico.Horimetro = horimetro;
            ordemServico.QuilometragemVeiculo = kmAtual > 0 ? kmAtual : ordemServico.Veiculo?.KilometragemAtual ?? 0;
            ordemServico.TipoOrdemServico = codigoTipoOrdemServico > 0L ? repTipoOrdemServico.BuscarPorCodigo(codigoTipoOrdemServico, false) : null;
            ordemServico.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
            ordemServico.LancarServicosManualmente = Request.GetBoolParam("LancarServicosManualmente");
            ordemServico.GrupoServico = codigoGrupoServico > 0 ? repGrupoServico.BuscarPorCodigo(codigoGrupoServico) : null;
            ordemServico.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
            ordemServico.Responsavel = codigoResponsavel > 0 ? repUsuario.BuscarPorCodigo(codigoResponsavel) : null;
            ordemServico.DataLimiteExecucao = Request.GetNullableDateTimeParam("DataLimiteExecucao");
            ordemServico.Prioridade = Request.GetEnumParam<PrioridadeOrdemServico>("Prioridade");
            ordemServico.TipoLocalManutencao = codigoTipoLocalManutencao > 0L ? repTipoLocalManutencao.BuscarPorCodigo(codigoTipoLocalManutencao, false) : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoEquipamento = Request.GetIntParam("Equipamento"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoServico = Request.GetIntParam("Servico"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0,
                CpfCnpjLocalManutencao = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor ? Usuario.Cliente?.CPF_CNPJ ?? 0d : Request.GetDoubleParam("LocalManutencao"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                TipoManutencao = Request.GetNullableEnumParam<TipoManutencaoOrdemServicoFrota>("TipoManutencao"),
                Situacao = Request.GetListEnumParam<SituacaoOrdemServicoFrota>("Situacao"),
                TipoOrdemServico = Request.GetNullableEnumParam<TipoOficina>("TipoOrdemServico"),
                CodigoGrupoServico = Request.GetIntParam("GrupoServico"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                Prioridade = Request.GetNullableEnumParam<PrioridadeOrdemServico>("Prioridade"),
                NumeroFogoPneu = Request.GetStringParam("NumeroFogoPneu")
            };

            string numeroInicial = Request.GetStringParam("NumeroInicial");
            if (!string.IsNullOrWhiteSpace(numeroInicial) && filtrosPesquisa.NumeroInicial == 0)
            {
                filtrosPesquisa.Placa = numeroInicial;
                filtrosPesquisa.NumeroInicial = 0;
            }

            if ((this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas?.Count > 0)
            {
                filtrosPesquisa.CodigosEmpresa = Request.GetListParam<int>("Empresa");
                if (filtrosPesquisa.CodigosEmpresa == null || filtrosPesquisa.CodigosEmpresa.Count == 0)
                    filtrosPesquisa.CodigosEmpresa = this.Usuario.Empresas.Select(c => c.Codigo).ToList();
            }

            return filtrosPesquisa;
        }

        private string PreencherLoteObjetoOrdemServico(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unidadeTrabalho);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeTrabalho);
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);

            dynamic itens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Itens"));
            if (itens == null || itens?.Count == 0)
                throw new ControllerException("Favor informar algum veículo ou equipamento para geração!");

            int codigoServico = Request.GetIntParam("Servico");
            int codigoProdutoOrcado = Request.GetIntParam("ProdutoOrcado");
            long codigoTipoOrdemServico = Request.GetLongParam("Tipo");
            double cpfCnpjLocalManutencao = Request.GetDoubleParam("LocalManutencao");
            DateTime dataProgramada = Request.GetDateTimeParam("DataProgramada");
            string observacao = Request.GetStringParam("Observacao");
            string condicaoPagamento = Request.GetStringParam("CondicaoPagamento");

            Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servicoVeiculo = repServicoVeiculoFrota.BuscarPorCodigo(codigoServico);
            decimal custoMedio = repServicoOrdemServico.BuscarCustoMedio(codigoServico);
            if (!servicoVeiculo.PermiteLancamentoSemValor && custoMedio <= 0m)
                throw new ControllerException("A manutenção não possui custo estimado, não sendo possível confirmar a execução das mesmas.");

            Dominio.Entidades.Cliente localManutencao = cpfCnpjLocalManutencao > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjLocalManutencao) : null;
            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo tipoOrdemServico = codigoTipoOrdemServico > 0L ? repTipoOrdemServico.BuscarPorCodigo(codigoTipoOrdemServico, false) : null;
            Dominio.Entidades.Produto produtoOrcado = codigoProdutoOrcado > 0 ? repProduto.BuscarPorCodigo(codigoProdutoOrcado) : null;

            string numerosOS = "";
            foreach (var item in itens)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico = new Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico();

                int codigoVeiculo = ((string)item.Veiculo.Codigo).ToInt();
                int codigoEquipamento = ((string)item.Equipamento.Codigo).ToInt();
                int codigoMotorista = ((string)item.Motorista.Codigo).ToInt();
                int horimetro = ((string)item.Horimetro).ToInt();
                int kmAtual = ((string)item.QuilometragemVeiculo).ToInt();

                decimal valorProdutos = Utilidades.Decimal.Converter((string)item.ValorProdutos);
                decimal valorMaoObra = Utilidades.Decimal.Converter((string)item.ValorMaoObra);

                objetoOrdemServico.DataProgramada = dataProgramada;
                objetoOrdemServico.LocalManutencao = localManutencao;
                objetoOrdemServico.Motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;
                objetoOrdemServico.Observacao = observacao;
                objetoOrdemServico.CondicaoPagamento = condicaoPagamento;
                objetoOrdemServico.Operador = Usuario;
                objetoOrdemServico.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
                objetoOrdemServico.Equipamento = codigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(codigoEquipamento) : null;
                objetoOrdemServico.Horimetro = horimetro;
                objetoOrdemServico.QuilometragemVeiculo = kmAtual > 0 ? kmAtual : objetoOrdemServico.Veiculo?.KilometragemAtual ?? 0;
                objetoOrdemServico.TipoOrdemServico = tipoOrdemServico;
                objetoOrdemServico.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
                objetoOrdemServico.ServicoVeiculo = servicoVeiculo;
                objetoOrdemServico.CentroResultado = objetoOrdemServico?.Veiculo?.CentroResultado ?? (objetoOrdemServico.Equipamento?.CentroResultado ?? null);


                //Gera OS
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = Servicos.Embarcador.Frota.OrdemServico.GerarOrdemServicoAteEtapaDeFechamento(objetoOrdemServico, Auditado, unidadeTrabalho, valorProdutos, valorMaoObra, produtoOrcado);

                if (string.IsNullOrWhiteSpace(numerosOS))
                    numerosOS = ordemServico.Numero.ToString();
                else
                    numerosOS += ", " + ordemServico.Numero.ToString();
            }


            return numerosOS;
        }

        private bool VeiculoSeEncontraEmManutencaoEmOutraOrdemServico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (ordemServico.Veiculo == null)
                return false;

            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

            List<SituacaoOrdemServicoFrota> listaStatusOS = new List<SituacaoOrdemServicoFrota>() {
                SituacaoOrdemServicoFrota.EmManutencao, SituacaoOrdemServicoFrota.EmDigitacao, SituacaoOrdemServicoFrota.AgAutorizacao,
                SituacaoOrdemServicoFrota.Rejeitada, SituacaoOrdemServicoFrota.DivergenciaOrcadoRealizado, SituacaoOrdemServicoFrota.SemRegraAprovacao,
                SituacaoOrdemServicoFrota.AguardandoAprovacao, SituacaoOrdemServicoFrota.AprovacaoRejeitada
            };

            return repOrdemServicoFrota.ContemOrdemServicoEmManutencaoEmAbertoVeiculo(ordemServico.Codigo, ordemServico.Veiculo.Codigo, listaStatusOS);
        }

        private string ObterGuaritaOS(int codigoOS, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unidadeTrabalho);

            string iconeSaida = null;
            if (repGuaritaTMS.ContemRegistroGuaritaOrdemDeServico(codigoOS, TipoEntradaSaida.Saida))
                iconeSaida = "<i class=\"far fa-arrow-circle-up fa-lg\" style=\"color:gold\"></i>";
            else
                iconeSaida = "<i class=\"far fa-arrow-circle-up fa-lg\" style=\"color:silver\"></i>";

            string iconeEntrada = null;
            if (repGuaritaTMS.ContemRegistroGuaritaOrdemDeServico(codigoOS, TipoEntradaSaida.Entrada))
                iconeEntrada = "<i class=\"far fa-arrow-circle-down fa-lg\" style=\"color:green\"></i>";
            else
                iconeEntrada = "<i class=\"far fa-arrow-circle-down fa-lg\" style=\"color:silver\"></i>";

            return iconeSaida+iconeEntrada;
        }

        #endregion
    }
}
