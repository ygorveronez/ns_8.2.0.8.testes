using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IO;
using SGTAdmin.Controllers;
using OfficeOpenXml;
using Dominio.Excecoes.Embarcador;
using Dominio.Entidades;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/Abastecimento")]
    public class AbastecimentoController : BaseController
    {
		#region Construtores

		public AbastecimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAbastecimento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Placa", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Veículo", "TipoVeiculo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Posto", "Posto", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Abastecimento", "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cód. Equipamento", "CodigoEquipamento", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Equipamento", "DescricaoEquipamento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("KM", "KM", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Acerto", "NumeroAcertos", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Horímetro", "Horimetro", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Litros", "Litros", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoAbastecimento", 12, Models.Grid.Align.right, false);
                if (filtrosPesquisa.Situacao.Equals("T"))
                    grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Média", "MediaCombustivel", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Média Horímetro", "MediaHorimetro", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 10, Models.Grid.Align.right, false);

                var propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo.Placa";
                if (propOrdenar == "CodigoEquipamento")
                    propOrdenar = "Equipamento.Codigo";

                var repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                var listaAbastecimento = repAbastecimento.Consulta(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAbastecimento.ContaConsulta(filtrosPesquisa));

                var lista = (from p in listaAbastecimento
                             select new
                             {
                                 p.Codigo,
                                 Placa = p.Veiculo != null ? p.Veiculo.Placa : string.Empty,
                                 TipoVeiculo = p.Veiculo != null ? (p.Veiculo.TipoVeiculo == "0" ? "Tração" : "Reboque") : string.Empty,
                                 Posto = p.Posto != null ? p.Posto.Nome : string.Empty,
                                 Data = p.Data != null ? p.Data.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 KM = p.Kilometragem.ToString("n0"),
                                 p.NumeroAcertos,
                                 Litros = p.Litros.ToString("n4"),
                                 p.DescricaoTipoAbastecimento,
                                 Situacao = p.DescricaoSituacao,
                                 MediaCombustivel = p.Situacao == "I" || p.Situacao == "G" ? "0,0000" : p.MediaCombustivel.ToString("n4"),
                                 CodigoEquipamento = p.Equipamento?.Codigo ?? 0,
                                 DescricaoEquipamento = p.Equipamento?.Descricao ?? "",
                                 Horimetro = p. Horimetro,
                                 MediaHorimetro = p.Situacao == "I" || p.Situacao == "G" ? "0,0000" : p.MediaHorimetro.ToString("n4"),
                                 ValorTotal = p.ValorTotal.ToString("n4"),
                                 DT_Row  = validaCapacidade(p)

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

            string validaCapacidade(Abastecimento p)
            {
                if (p.Equipamento != null)
                {
                    return ((p.Equipamento?.CapacidadeMaximaTanque ?? 0 ) >= p.Litros)
                                                    ? CorGrid.Branco
                                                    : CorGrid.Vermelho;
                }
                else if (p.Veiculo != null)
                {
                    return ((p.Veiculo?.CapacidadeMaximaTanque ?? 0) >= p.Litros)
                                                    ? CorGrid.Branco
                                                    : CorGrid.Vermelho;
                }
                else
                {
                    return CorGrid.Vermelho;
                }
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAbastecimento filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Placa", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Veículo", "TipoVeiculo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Posto", "Posto", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Abastecimento", "Data", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("KM", "KM", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Acerto", "NumeroAcertos", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Litros", "Litros", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoAbastecimento", 12, Models.Grid.Align.right, false);
                if (filtrosPesquisa.Situacao.Equals("T"))
                    grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Média", "MediaCombustivel", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Equipamento", "DescricaoEquipamento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Horimetro", "Horimetro", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 10, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo.Placa";

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeTrabalho);
                int countAbastecimentos = repAbastecimento.ContaConsulta(filtrosPesquisa);
                if (countAbastecimentos > 5000)
                    return new JsonpResult(false, true, "A quantidade de registros para exportação não pode ser superior a 5000.");

                List<Dominio.Entidades.Abastecimento> listaAbastecimento = repAbastecimento.Consulta(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(countAbastecimentos);

                var lista = (from p in listaAbastecimento
                             select new
                             {
                                 p.Codigo,
                                 Placa = p.Veiculo != null ? p.Veiculo.Placa : string.Empty,
                                 TipoVeiculo = p.Veiculo != null ? (p.Veiculo.TipoVeiculo == "0" ? "Tração" : "Reboque") : string.Empty,
                                 Posto = p.Posto != null ? p.Posto.Nome : string.Empty,
                                 Data = p.Data != null && p.Data.HasValue ? p.Data.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 KM = p.Kilometragem.ToString("n0"),
                                 p.NumeroAcertos,
                                 Litros = p.Litros.ToString("n4"),
                                 p.DescricaoTipoAbastecimento,
                                 Situacao = p.DescricaoSituacao,
                                 MediaCombustivel = p.Situacao == "I" ? "0,0000" : p.MediaCombustivel.ToString("n4"),
                                 CodigoEquipamento = p.Equipamento?.Codigo ?? 0,
                                 DescricaoEquipamento = p.Equipamento?.Descricao ?? "",
                                 Horimetro = p.Horimetro,
                                 ValorTotal = p.ValorTotal.ToString("n4")
                             }).ToList();

                grid.AdicionaRows(lista);
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
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAbastecimentoAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

                int codigoMotorista, codigoPagamentoAgregado, veiculo;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("PagamentoAgregado"), out codigoPagamentoAgregado);
                int.TryParse(Request.Params("Veiculo"), out veiculo);

                double cliente;
                double.TryParse(Request.Params("Cliente"), out cliente);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAbastecimento", false);
                grid.AdicionarCabecalho("CodigoPagamento", false);
                grid.AdicionarCabecalho("CodigoAbastecimentoPagamentoAgregado", false);
                grid.AdicionarCabecalho("Data", "Data", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Documento", "Documento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("KM", "KM", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Litros", "Litros", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Vlr Unitário", "ValorUnitario", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Vlr Total", "ValorTotal", 12, Models.Grid.Align.right, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                var dynListaCarga = repAbastecimento.BuscarPorAbastecimentoAgregado(veiculo, dataInicial, dataFinal, codigoMotorista, cliente, codigoPagamentoAgregado, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                var dynRetorno = (from obj in dynListaCarga
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      CodigoAbastecimentoPagamentoAgregado = 0,
                                      CodigoAbastecimento = obj.Codigo,
                                      CodigoPagamento = codigoPagamentoAgregado,
                                      Data = obj.Data.HasValue ? obj.Data.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                      Fornecedor = obj.Posto?.Descricao ?? "",
                                      Documento = obj.Documento,
                                      KM = obj.Kilometragem.ToString("D"),
                                      Litros = obj.Litros.ToString("n2"),
                                      ValorUnitario = obj.ValorUnitario.ToString("n4"),
                                      ValorTotal = obj.ValorTotal.ToString("n2")
                                  }).ToList();

                grid.setarQuantidadeTotal(repAbastecimento.ContarBuscarPorAbastecimentoAgregado(veiculo, dataInicial, dataFinal, codigoMotorista, cliente, codigoPagamentoAgregado));
                grid.AdicionaRows(dynRetorno);

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

                Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();

                PreencheEntidade(abastecimento, unitOfWork);

                abastecimento.Situacao = "A";

                if (abastecimento.Data == DateTime.MinValue || abastecimento.Data == null)
                {
                    abastecimento.Data = DateTime.Now;
                }
                
                abastecimento.DataAlteracao = DateTime.Now;
                abastecimento.FechamentoAbastecimento = null;

                Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                string erro;
                if (!ValidaEntidade(abastecimento, out erro, ConfiguracaoEmbarcador, unitOfWork))
                    return new JsonpResult(false, true, erro);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (abastecimento.TipoMovimento == null)
                    {
                        // É obrigatorio ter movimento financeiro configurado
                        if (configuracaoAbastecimento == null)
                            return new JsonpResult(false, true, "Nenhum movimento financeiro padrão configurado para essa operação.");

                        bool abastecimentoInterno = false;
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOficina? tipoOficina = null;
                        if (abastecimento.Produto != null && abastecimento.Produto.ProdutoCombustivel.HasValue && abastecimento.Produto.ControlaEstoqueCombustivel.HasValue && abastecimento.Produto.ProdutoCombustivel.Value && abastecimento.Produto.ControlaEstoqueCombustivel.Value)
                        {
                            if (abastecimento.Posto != null)
                            {
                                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = repModalidadeFornecedor.BuscarPorCliente(abastecimento.Posto.CPF_CNPJ);
                                if (modalidadeFornecedor != null)
                                    tipoOficina = modalidadeFornecedor.TipoOficina;
                            }
                            if (tipoOficina.HasValue && tipoOficina.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOficina.Interna)
                                abastecimentoInterno = true;
                        }

                        if (abastecimentoInterno)
                            abastecimento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria;
                        else
                            abastecimento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;
                    }
                }

                if (abastecimento.Requisicao)
                {
                    Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoRequisicao(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, ConfiguracaoEmbarcador, this.Usuario, TipoServicoMultisoftware);

                    if (string.IsNullOrWhiteSpace(abastecimento.MotivoInconsistencia))
                    {
                        Servicos.Embarcador.Abastecimento.Abastecimento.GerarRequisicaoAutomatica(unitOfWork, abastecimento);
                        abastecimento.OrdemCompra = Servicos.Embarcador.Abastecimento.Abastecimento.GerarOrdemCompraPeloAbastecimento(abastecimento, this.Usuario, unitOfWork);
                    }
                    
                }
                else
                    Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, ConfiguracaoEmbarcador);

                if (string.IsNullOrWhiteSpace(abastecimento.MotivoInconsistencia))
                {
                    repAbastecimento.Inserir(abastecimento, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.CommitChanges();
                   return new JsonpResult(false, abastecimento.MotivoInconsistencia);
                }
                    
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo, true);

                if (abastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (abastecimento.Situacao.Equals("F"))
                    return new JsonpResult(false, true, "Não é possível atualizar um abastecimento fechado.");

                int codigoVeiculoAnterior = abastecimento.Veiculo?.Codigo ?? 0;
                decimal kilometragemAnterior = abastecimento.Kilometragem;
                int codigoEquipamentoAnterior = abastecimento.Equipamento?.Codigo ?? 0;
                int horimetroAnterior = abastecimento.Horimetro;

                PreencheEntidade(abastecimento, unitOfWork);

                Servicos.Embarcador.Abastecimento.Abastecimento.ReprocessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento, codigoVeiculoAnterior, kilometragemAnterior, codigoEquipamentoAnterior, horimetroAnterior);

                string erro;
                if (!ValidaEntidade(abastecimento, out erro, ConfiguracaoEmbarcador, unitOfWork))
                    return new JsonpResult(false, true, erro);

                if (abastecimento.Situacao == "I")
                    abastecimento.Situacao = "A";

                bool requisicaoPrevia = false;
                if (abastecimento.Requisicao)
                    requisicaoPrevia = true;

                if (abastecimento.Requisicao)
                    Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoRequisicao(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, ConfiguracaoEmbarcador, this.Usuario, TipoServicoMultisoftware);
                else
                    Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, ConfiguracaoEmbarcador);

                abastecimento.DataAlteracao = DateTime.Now;
                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                if (abastecimento.OrdemCompra != null && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && requisicaoPrevia == true)
                {
                    Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unitOfWork);
                    Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(abastecimento.OrdemCompra.Codigo);

                    foreach (var ord in ordemCompra.Mercadorias)
                    {
                        ord.ValorUnitario = abastecimento.ValorUnitario;
                        ord.Quantidade = abastecimento.Litros;
                    }

                    if (ordemCompra.Situacao == SituacaoOrdemCompra.Aberta)
                    {
                        repOrdemCompra.Atualizar(ordemCompra);
                        
                    }
                        
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reabrir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();

                if (abastecimento == null)
                    return new JsonpResult(false, "Falha ao buscar dados.");

                // Valida situacao
                if (abastecimento.Situacao != "F")
                    throw new ControllerException("Só é possível reabrir abastecimentos fechados.");

                // Valida se ja nao esta num acerto de viagem
                if (repAbastecimento.AbastecimentoTemAcerto(abastecimento.Codigo))
                    throw new ControllerException("Não é possível reabrir abastecimentos que já possuem acerto.");

                // Valida se ja nao esta num acerto de viagem
                if (repAbastecimento.AbastecimentoComTituloQuitado(abastecimento.Codigo))
                    throw new ControllerException("Não é possível reabrir abastecimentos que possuem títulos quitados.");

                // Gera movimento reverso                                     
                if (configuracaoAbastecimento == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    throw new ControllerException("Nenhum movimento financeiro padrão configurado para essa operação.");

                string obsMovimentacao = "Reversão Lançamento de abastecimento do veículo " + abastecimento.Veiculo?.Placa ?? "";
                string erro = "";

                if (abastecimento.Produto != null && abastecimento.Produto.ProdutoCombustivel.HasValue && abastecimento.Produto.ControlaEstoqueCombustivel.HasValue && abastecimento.Produto.ProdutoCombustivel.Value && abastecimento.Produto.ControlaEstoqueCombustivel.Value)
                {
                    Dominio.Entidades.Empresa empresa = abastecimento.Empresa;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && abastecimento.Posto != null)
                        empresa = repEmpresa.BuscarPorCNPJ(abastecimento.Posto.CPF_CNPJ_SemFormato);

                    if (empresa != null && !servicoEstoque.MovimentarEstoque(out erro, abastecimento.Produto, abastecimento.Litros, Dominio.Enumeradores.TipoMovimento.Entrada, "ABAST", abastecimento.Codigo.ToString(), abastecimento.ValorUnitario, empresa, abastecimento.Data.Value, TipoServicoMultisoftware))
                        throw new ControllerException(erro);
                }

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && abastecimento.Data.HasValue && abastecimento.TipoMovimento != null)
                {
                    if (!serProcessoMovimento.GerarMovimentacao(out erro, null, abastecimento.Data.Value, abastecimento.ValorTotal, abastecimento.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, 0, abastecimento.TipoMovimento.PlanoDeContaDebito, abastecimento.TipoMovimento.PlanoDeContaCredito))
                        throw new ControllerException(erro);
                }

                if (abastecimento.GerarContasAPagarParaAbastecimentoExternos && abastecimento.TipoMovimentoPagamentoExterno != null && abastecimento.Posto != null)
                {
                    if (!Servicos.Embarcador.Abastecimento.Abastecimento.EstornarTituloPagarAbastecimento(abastecimento, out erro, TipoServicoMultisoftware, Auditado, unitOfWork))
                        throw new ControllerException(erro);
                }

                // Seta status aberto
                abastecimento.Situacao = "A";
                abastecimento.DataAlteracao = DateTime.Now;
                abastecimento.FechamentoAbastecimento = null;
                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, abastecimento, null, "Reabriu o abastecimento pela tela de Abastecimento.", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.AbastecimentoAnexo repositorioAbastecimentoAnexo = new Repositorio.AbastecimentoAnexo(unitOfWork);

                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.AbastecimentoAnexo> anexos = repositorioAbastecimentoAnexo.BuscarPorCodigoAbastecimento(codigo);


                if (abastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = null;
                if (abastecimento.Posto != null)
                    modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorCliente(abastecimento.Posto.CPF_CNPJ);

                var dynAbastecimento = new
                {
                    abastecimento.Codigo,
                    Veiculo = abastecimento.Veiculo != null ? new { Codigo = abastecimento.Veiculo.Codigo, Descricao = $"{abastecimento.Veiculo.Placa} ({(abastecimento.Veiculo.Tipo == "P" ? "PRÓPRIO)" : "TERCEIRO)")}) {(abastecimento.Veiculo.ModeloVeicularCarga?.Descricao ?? "")}", Tipo = abastecimento.Veiculo.Tipo } : null,
                    Motorista = abastecimento.Motorista != null ? new { Codigo = abastecimento.Motorista.Codigo, Descricao = abastecimento.Motorista.Nome } : null,
                    Posto = abastecimento.Posto != null ? new { Codigo = abastecimento.Posto.CPF_CNPJ, Descricao = abastecimento.Posto.Nome } : null,
                    Produto = abastecimento.Produto != null ? new { Codigo = abastecimento.Produto.Codigo, Descricao = abastecimento.Produto.Descricao } : null,
                    Equipamento = abastecimento.Equipamento != null ? new { Codigo = abastecimento.Equipamento.Codigo, Descricao = abastecimento.Equipamento.Descricao } : null,
                    TipoMovimento = abastecimento.TipoMovimento != null ? new { Codigo = abastecimento.TipoMovimento.Codigo, Descricao = abastecimento.TipoMovimento.Descricao } : null,
                    CentroResultado = abastecimento.CentroResultado != null ? new { Codigo = abastecimento.CentroResultado.Codigo, Descricao = abastecimento.CentroResultado.Descricao } : null,
                    abastecimento.TipoAbastecimento,
                    Data = abastecimento.Data.HasValue ? abastecimento.Data.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    Horimetro = abastecimento.Horimetro,
                    KM = abastecimento.Kilometragem,
                    Litros = abastecimento.Litros.ToString("n4"),
                    ValorTotal = abastecimento.ValorTotal.ToString("n2"),
                    ValorUnitario = abastecimento.ValorUnitario.ToString("n4"),
                    Situacao = abastecimento.Situacao,
                    abastecimento.Documento,
                    MediaCombustivel = abastecimento.Situacao == "I" ? "0,0000" : abastecimento.MediaCombustivel.ToString("n4"),
                    TipoVeiculo = abastecimento.Veiculo?.DescricaoTipoVeiculo ?? "Reboque",
                    abastecimento.MoedaCotacaoBancoCentral,
                    DataBaseCRT = abastecimento.DataBaseCRT.HasValue ? abastecimento.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    ValorMoedaCotacao = abastecimento.ValorMoedaCotacao.ToString("n10"),
                    ValorUnitarioMoedaEstrangeira = abastecimento.ValorUnitarioMoedaEstrangeira.ToString("n4"),
                    ValorOriginalMoedaEstrangeira = abastecimento.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    MotivoInconsistencia = abastecimento.MotivoInconsistencia,
                    Observacao = abastecimento.Observacao,
                    DescricaoSituacao = abastecimento.DescricaoSituacao,
                    LocalArmazenamento = abastecimento.LocalArmazenamento != null ? new { Codigo = abastecimento.LocalArmazenamento.Codigo, Descricao = abastecimento.LocalArmazenamento.Descricao } : null,
                    ListaAbastecimentos = abastecimento.Abastecimentos != null && abastecimento.Abastecimentos.Count > 0 ?
                            (from obj in abastecimento.Abastecimentos
                             select new
                             {
                                 obj.Codigo,
                                 Placa = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                 Posto = obj.Posto != null ? obj.Posto.Nome : string.Empty,
                                 Data = obj.Data != null ? obj.Data.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 KM = obj.Kilometragem.ToString("n0"),
                                 obj.NumeroAcertos,
                                 Litros = obj.Litros.ToString("n4"),
                                 obj.DescricaoTipoAbastecimento,
                                 Situacao = obj.DescricaoSituacao,
                                 MediaCombustivel = obj.Situacao == "I" ? "0,0000" : obj.MediaCombustivel.ToString("n4"),
                                 CodigoEquipamento = obj.Equipamento?.Codigo ?? 0,
                                 DescricaoEquipamento = obj.Equipamento?.Descricao ?? "",
                                 Horimetro = obj.Horimetro,
                                 MediaHorimetro = obj.Situacao == "I" ? "0,0000" : obj.MediaHorimetro.ToString("n4"),
                                 MotivoInconsistencia = obj.MotivoInconsistencia,
                                 Observacao = obj.Observacao,
                                 DescricaoSituacao = obj.DescricaoSituacao
                             }).ToList() : null,
                    ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento = modalidadeFornecedorPessoas?.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento ?? false,
                    Requisicao = abastecimento.Requisicao,
                    OrdemCompra = abastecimento.OrdemCompra != null ? new { Codigo = abastecimento.OrdemCompra.Codigo, Descricao = abastecimento.OrdemCompra.Descricao } : null,
                    Anexos = (
                        from obj in anexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList(),
                };

                return new JsonpResult(dynAbastecimento);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo, true);

                if (abastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Valida situacao
                if (abastecimento.Situacao != "A" && abastecimento.Situacao != "I" && abastecimento.Situacao != "R")
                    return new JsonpResult(false, true, "Só é possível excluir abastecimentos abertos ou inconsistentes.");

                repAbastecimento.RemoverVinculosPorCodigo(abastecimento.Codigo);
                repAbastecimento.Deletar(abastecimento, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                {
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                }
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarAbastecimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento repConfiguracaoAbastecimento = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

                int codigoConfiguracaoAbastecimento = int.Parse(Request.Params("Codigo"));
                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracaoAbastecimento = repConfiguracaoAbastecimento.BuscarPorCodigo(codigoConfiguracaoAbastecimento);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoFinanceiraAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();

                if (configuracaoAbastecimento == null)
                    return new JsonpResult(false, "Favor selecione uma Configuração de Abastecimento");

                if (configuracaoFinanceiraAbastecimento == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    return new JsonpResult(false, "Nenhum movimento financeiro padrão configurado para essa operação.");

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                DataSet ds = new DataSet();
                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                    List<Dominio.Entidades.Abastecimento> listaAbastecimento = null;
                    string msgRetornoAbastecimento = "";
                    bool sempreAbastecimentoInterno = false;
                    var abastecimentoInterno = false;

                    if (fileExtension.ToLower() == ".zip" && configuracaoAbastecimento.TipoImportacaoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.Interno)
                    {
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        file.SaveAs(fileLocation);

                        Servicos.Embarcador.Abastecimento.ArquivoInternoTombini svcArquivoInternoTombini = new Servicos.Embarcador.Abastecimento.ArquivoInternoTombini(unitOfWork);
                        Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = svcArquivoInternoTombini.ProcessarArquivoInternoTombini(fileLocation, System.IO.Path.GetFileNameWithoutExtension(file.FileName), configuracaoAbastecimento.Posto.CPF_CNPJ, unitOfWork);
                        listaAbastecimento = retornoAbastecimento.Abastecimentos;
                        msgRetornoAbastecimento = retornoAbastecimento.MsgRetorno;
                        sempreAbastecimentoInterno = true;
                        abastecimentoInterno = true;
                    }

                    if ((fileExtension.ToLower() == ".xls" || fileExtension.ToLower() == ".xlsx") && configuracaoAbastecimento.TipoImportacaoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.PostoAmigao)
                    {
                        ExcelPackage package = new ExcelPackage(file.InputStream);
                        Servicos.Embarcador.Abastecimento.ArquivoPostoAmigao svcArquivoPostoAmigao = new Servicos.Embarcador.Abastecimento.ArquivoPostoAmigao(unitOfWork);
                        Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = svcArquivoPostoAmigao.ProcessarArquivoPostoAmigao(package, unitOfWork);
                        listaAbastecimento = retornoAbastecimento.Abastecimentos;
                        msgRetornoAbastecimento = retornoAbastecimento.MsgRetorno;
                        abastecimentoInterno = false;
                    }

                    if ((fileExtension.ToLower() == ".xls" || fileExtension.ToLower() == ".xlsx") && configuracaoAbastecimento.TipoImportacaoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.PostoReforco)
                    {
                        ExcelPackage package = new ExcelPackage(file.InputStream);
                        Servicos.Embarcador.Abastecimento.ArquivoPostoReforco svcArquivoPostoReforco = new Servicos.Embarcador.Abastecimento.ArquivoPostoReforco(unitOfWork);
                        Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = svcArquivoPostoReforco.ProcessarArquivoPostoReforco(package, unitOfWork);
                        listaAbastecimento = retornoAbastecimento.Abastecimentos;
                        msgRetornoAbastecimento = retornoAbastecimento.MsgRetorno;
                        abastecimentoInterno = false;
                    }

                    if (configuracaoAbastecimento.TipoImportacaoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.Planilha)
                    {
                        ExcelPackage package = new ExcelPackage(file.InputStream);

                        Servicos.Embarcador.Abastecimento.ArquivoPlanilha svcArquivoPlanilha = new Servicos.Embarcador.Abastecimento.ArquivoPlanilha(unitOfWork);
                        Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = svcArquivoPlanilha.ProcessarArquivoPlanilha(configuracaoAbastecimento, package, unitOfWork);

                        // valida a importação via planilha
                        bool importacaoValida = svcArquivoPlanilha.ValidarArquivoImportacaoPlanilha(retornoAbastecimento, out string mensagemErroImportacao, unitOfWork);
                        if (!importacaoValida)
                        {
                            if (!string.IsNullOrWhiteSpace(mensagemErroImportacao))
                                return new JsonpResult(false, mensagemErroImportacao);
                            else
                                return new JsonpResult(false, "Arquivo selecionado não está de acordo com o layout selecionado ou as exigências determinadas por configurações não foram atendidas!");

                        }

                        listaAbastecimento = retornoAbastecimento.Abastecimentos;
                        msgRetornoAbastecimento = retornoAbastecimento.MsgRetorno;
                        abastecimentoInterno = false;
                    }

                    if (fileExtension == ".txt" && configuracaoAbastecimento.TipoImportacaoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.EDI)
                    {
                        MemoryStream streamEDI = new MemoryStream();
                        files[0].InputStream.CopyTo(streamEDI);
                        Servicos.LeituraEDI svcLeituraEDI = new Servicos.LeituraEDI(this.Empresa, configuracaoAbastecimento.LayoutEDI, streamEDI, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, null, null);
                        Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = svcLeituraEDI.GerarAbastecimentos();
                        listaAbastecimento = retornoAbastecimento.Abastecimentos;
                        msgRetornoAbastecimento = retornoAbastecimento.MsgRetorno;
                        streamEDI.Dispose();
                        abastecimentoInterno = false;
                    }

                    string msgRetornoAbastecimentoInconsistente = "";
                    if (listaAbastecimento != null && listaAbastecimento.Count() > 0)
                    {
                        for (int i = 0; i < listaAbastecimento.Count; i++)
                        {
                            if (configuracaoAbastecimento.NaoImportarAbastecimentoDuplicado)
                            {
                                if (listaAbastecimento[i].Veiculo != null && listaAbastecimento[i].Data.HasValue && listaAbastecimento[i].Kilometragem > 0 && !string.IsNullOrWhiteSpace(listaAbastecimento[i].Documento))
                                {
                                    if (repAbastecimento.AbastecimentoDuplicado(listaAbastecimento[i].Data.Value, listaAbastecimento[i].Veiculo.Codigo, listaAbastecimento[i].Documento, listaAbastecimento[i].Kilometragem))
                                        continue;
                                }
                            }

                            Dominio.Entidades.Abastecimento abastecimento = listaAbastecimento[i];

                            if (abastecimento.Data.HasValue && abastecimento.Data > DateTime.Now)
                            {
                                msgRetornoAbastecimento += "Abastecimento com data futura. " + abastecimento.Data.Value.ToString("dd/MM/yyyy HH:mm");
                                continue;
                            }

                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                abastecimento.Empresa = this.Usuario.Empresa;

                            if (abastecimento.Veiculo != null && abastecimento.Veiculo.TipoVeiculo == "1" && abastecimento.Veiculo.Equipamentos != null && abastecimento.Veiculo.Equipamentos.Count > 0)
                            {
                                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = abastecimento.Veiculo.Equipamentos.Where(c => c.EquipamentoAceitaAbastecimento == true)?.FirstOrDefault() ?? null;
                                abastecimento.Equipamento = equipamento;
                                if (abastecimento.Horimetro <= 0 && abastecimento.Kilometragem > 0)
                                {
                                    abastecimento.Horimetro = (int)abastecimento.Kilometragem;
                                    abastecimento.Kilometragem = 0;
                                }
                            }

                            if (abastecimento.Equipamento != null && abastecimento.Horimetro > 0)
                            {
                                abastecimento.Veiculo = null;
                                abastecimento.Kilometragem = 0;
                            }

                            abastecimentoInterno = false;
                            if (!sempreAbastecimentoInterno)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOficina? tipoOficina = null;
                                if (abastecimento.Produto != null && abastecimento.Produto.ProdutoCombustivel.HasValue && abastecimento.Produto.ControlaEstoqueCombustivel.HasValue && abastecimento.Produto.ProdutoCombustivel.Value && abastecimento.Produto.ControlaEstoqueCombustivel.Value)
                                {
                                    if (abastecimento.Posto != null)
                                    {
                                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = repModalidadeFornecedor.BuscarPorCliente(abastecimento.Posto.CPF_CNPJ);
                                        if (modalidadeFornecedor != null)
                                            tipoOficina = modalidadeFornecedor.TipoOficina;
                                    }
                                    if (tipoOficina.HasValue && tipoOficina.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOficina.Interna)
                                        abastecimentoInterno = true;
                                }
                            }

                            // Valida regra de situação
                            // Se o veiculo for de terceiro, o abastecimento fica com situação (I) INCONSISTENTE
                            if (abastecimento.Veiculo?.Tipo == "T")
                            {
                                abastecimento.Situacao = "I";
                                abastecimento.MotivoInconsistencia += " Veículo de terceiro.";
                            }
                            else
                            {
                                abastecimento.Situacao = "A";
                                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                {
                                    if (abastecimentoInterno)
                                        abastecimento.TipoMovimento = configuracaoFinanceiraAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria;
                                    else if (!(configuracaoAbastecimento?.NaoGerarMovimentoFinanceiroFechamentoExterno ?? false))
                                        abastecimento.TipoMovimento = configuracaoFinanceiraAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;
                                }
                            }
                            Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, ConfiguracaoEmbarcador);
                            if (!string.IsNullOrWhiteSpace(abastecimento.MotivoInconsistencia))
                                msgRetornoAbastecimentoInconsistente += "-" + abastecimento.MotivoInconsistencia + "<br/>";


                            // Insere abastecimentos
                            repAbastecimento.Inserir(abastecimento, Auditado);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(msgRetornoAbastecimento))
                            return new JsonpResult(false, "Arquivo selecionado não está de acordo com o layout selecionado, ou nenhum abastecimento novo encontrado no arquivo!<br/>" + msgRetornoAbastecimento);
                        else
                            return new JsonpResult(false, "Arquivo selecionado não está de acordo com o layout selecionado, ou nenhum abastecimento novo encontrado no arquivo!");
                    }

                    string mensagemRetorno = "Importação dos abastecimentos foi realizada com sucesso.";
                    if (!string.IsNullOrWhiteSpace(msgRetornoAbastecimento))
                        mensagemRetorno += "<br/><br/>Contudo não conseguiu importar alguns abastecimentos pelos seguintes motivos:<br/>" + msgRetornoAbastecimento;
                    if (!string.IsNullOrWhiteSpace(msgRetornoAbastecimentoInconsistente))
                        mensagemRetorno += "<br/><br/>Porém importou alguns abastecimentos com as seguintes inconsistências:<br/>" + msgRetornoAbastecimentoInconsistente;

                    return new JsonpResult(true, mensagemRetorno);
                }
                else
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar os abastecimentos. Erro: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAbastecimentoSemAcertoDeViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                int.TryParse(Request.Params("AcertoViagem"), out int codigoAcertoViagem);
                int.TryParse(Request.Params("Kilometragem"), out int kilometragem);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("Horimetro"), out int horimetro);

                DateTime.TryParse(Request.Params("Data"), out DateTime data);

                Enum.TryParse(Request.Params("TipoAbastecimento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcertoViagem);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("CodigoAcertoAbastecimento", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAcertoViagem", false);
                grid.AdicionarCabecalho("CodigoVeiculo", false);
                grid.AdicionarCabecalho("CodigoCombustivel", false);
                grid.AdicionarCabecalho("Combustível", "Combustivel", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CodigoFornecedor", false);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "DataAbastecimento", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Litros", "Litros", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor Unitário", "ValorUnitario", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("ValorTotal", false);
                grid.AdicionarCabecalho("TipoAbastecimento", false);
                grid.AdicionarCabecalho("KM", "Kilometragem", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("VLitros", false);
                grid.AdicionarCabecalho("VValorUnitario", false);
                grid.AdicionarCabecalho("VValorTotal", false);
                grid.AdicionarCabecalho("NumeroDocumento", false);
                grid.AdicionarCabecalho("DT_RowColor", false);
                grid.AdicionarCabecalho("CodigoEquipamento", false);
                grid.AdicionarCabecalho("Horimetro", false);
                grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("DataBaseCRT", false);
                grid.AdicionarCabecalho("ValorMoedaCotacao", false);
                grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);
                grid.AdicionarCabecalho("CodigoFechamentoAbastecimento", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "DataAbastecimento")
                    propOrdenacao = "Data";
                else
                    propOrdenacao = "Kilometragem";

                List<Dominio.Entidades.Abastecimento> listaAbastecimento = repAcertoAbastecimento.ConsultarAbastecimentosDoAcertoViagem(kilometragem, horimetro, data, acertoViagem, codigoVeiculo, tipoAbastecimento, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int quantidade = repAcertoAbastecimento.ContarConsultarAbastecimentosDoAcertoViagem(kilometragem, horimetro, data, acertoViagem, codigoVeiculo, tipoAbastecimento);

                grid.setarQuantidadeTotal(quantidade);

                var dynListaRetorno = (from obj in listaAbastecimento
                                       select new
                                       {
                                           CodigoAcertoAbastecimento = 0,
                                           CodigoAcertoViagem = codigoAcertoViagem,
                                           Codigo = obj.Codigo,
                                           CodigoVeiculo = obj.Veiculo != null ? obj.Veiculo.Codigo : 0,
                                           CodigoCombustivel = obj.Produto != null ? obj.Produto.Codigo : 0,
                                           Combustivel = obj.Produto != null ? obj.Produto.Descricao : string.Empty,
                                           CodigoFornecedor = obj.Posto != null ? obj.Posto.CPF_CNPJ : 0,
                                           Fornecedor = obj.Posto != null ? obj.Posto.Nome : string.Empty,
                                           DataAbastecimento = obj.Data.HasValue ? obj.Data.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                           Litros = obj.Litros.ToString("n4"),
                                           ValorUnitario = obj.ValorUnitario.ToString("n4"),
                                           ValorTotal = obj.ValorTotal.ToString("n4"),
                                           TipoAbastecimento = tipoAbastecimento,
                                           Kilometragem = obj.Kilometragem.ToString("n0"),
                                           NumeroDocumento = obj.Documento,
                                           VLitros = obj.Litros,
                                           VValorUnitario = obj.ValorUnitario,
                                           VValorTotal = obj.ValorTotal,
                                           DT_RowColor = "#FFFFFF",
                                           CodigoEquipamento = obj.Equipamento?.Codigo ?? 0,
                                           Horimetro = obj.Horimetro,
                                           obj.MoedaCotacaoBancoCentral,
                                           DataBaseCRT = obj.DataBaseCRT.HasValue ? obj.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                           ValorMoedaCotacao = obj.ValorMoedaCotacao.ToString("n10"),
                                           ValorOriginalMoedaEstrangeira = obj.ValorOriginalMoedaEstrangeira.ToString("n2"),
                                           CodigoFechamentoAbastecimento = obj.FechamentoAbastecimento?.Codigo ?? 0
                                       }).ToList();

                grid.AdicionaRows(dynListaRetorno);
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

        public async Task<IActionResult> AdicionarAbastecimento()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Pega codigo
                int codigo;
                int.TryParse(Request.Params("CodigoAbastecimento"), out codigo);

                // Instancia Repositorios/Entidade
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo, true);

                // Abastecimento nao encontrado
                if (abastecimento == null)
                    return new JsonpResult(false, "Falha ao buscar dados.");

                // Valida situacao
                if (abastecimento.Situacao != "A")
                    return new JsonpResult(false, "O abastecimento não se encontra aberto.");

                // Seta staus aberto
                if (abastecimento.LitrosOriginal == 0)
                    abastecimento.LitrosOriginal = abastecimento.Litros;

                if (abastecimento.Abastecimentos == null)
                    abastecimento.Abastecimentos = new List<Dominio.Entidades.Abastecimento>();

                dynamic abastecimentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Abastecimentos"));
                foreach (var abast in abastecimentos)
                {
                    int codigoAbastecimento = (int)abast.Codigo;

                    if (!abastecimento.Abastecimentos.Any(o => o.Codigo == codigoAbastecimento))
                    {
                        Dominio.Entidades.Abastecimento aba = repAbastecimento.BuscarPorCodigo(codigoAbastecimento, true);
                        aba.Situacao = "G";
                        aba.Integrado = false;
                        repAbastecimento.Atualizar(aba, Auditado);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, abastecimento, null, $"Agrupou o abastecimento, código: {aba.Codigo} - {aba.Descricao}", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, aba, null, $"Agrupado ao abastecimento, código: {abastecimento.Codigo} - {abastecimento.Descricao}", unitOfWork);

                        abastecimento.Litros += aba.Litros;
                        abastecimento.Abastecimentos.Add(aba);
                    }
                }

                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                unitOfWork.CommitChanges();

                var dynAbastecimento = new
                {
                    Litros = abastecimento.Litros.ToString("n4")
                };

                return new JsonpResult(dynAbastecimento, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao agrupar abastecimentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverAbastecimento()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Pega codigo
                int codigo, codigoAbastecimento;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoAbastecimento"), out codigoAbastecimento);

                // Instancia Repositorios/Entidade
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

                Dominio.Entidades.Abastecimento abastecimentoAgrupado = repAbastecimento.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento, true);

                // Abastecimento nao encontrado
                if (abastecimento == null)
                    return new JsonpResult(false, "Falha ao buscar dados.");

                if (abastecimentoAgrupado == null)
                    return new JsonpResult(false, "Falha ao buscar dados.");

                // Valida situacao
                if (abastecimento.Situacao != "A")
                    return new JsonpResult(false, "Só é possível desagrupar se o abasecimento estiver em aberto.");

                // Seta staus aberto
                abastecimentoAgrupado.Situacao = "A";
                abastecimentoAgrupado.DataAlteracao = DateTime.Now;
                abastecimentoAgrupado.Integrado = false;

                repAbastecimento.Atualizar(abastecimentoAgrupado, Auditado);

                abastecimento.Litros -= abastecimentoAgrupado.Litros;
                abastecimento.DataAlteracao = DateTime.Now;
                if (abastecimento.Litros <= 0)
                    abastecimento.Litros = abastecimento.LitrosOriginal;

                abastecimento.Abastecimentos.Remove(abastecimentoAgrupado);
                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, abastecimento, null, $"Desagrupou o abastecimento, código: {abastecimentoAgrupado.Codigo} - {abastecimentoAgrupado.Descricao}.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, abastecimentoAgrupado, null, $"Desagrupado do abastecimento, código: {abastecimento.Codigo} - {abastecimento.Descricao}.", unitOfWork);

                unitOfWork.CommitChanges();

                var dynAbastecimento = new
                {
                    Litros = abastecimento.Litros.ToString("n4")
                };
                return new JsonpResult(dynAbastecimento, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarKilometragem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo, true);

                if (abastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!abastecimento.Situacao.Equals("F"))
                    return new JsonpResult(false, true, "Não foi possível atualizar a quilometragem.");

                abastecimento.Kilometragem = Request.GetIntParam("KM");

                abastecimento.DataAlteracao = DateTime.Now;
                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a quilometragem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarHorimetro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo, true);

                if (abastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!abastecimento.Situacao.Equals("F"))
                    return new JsonpResult(false, true, "Não foi possível atualizar a Horímetro.");

                abastecimento.Horimetro = Request.GetIntParam("Horimetro");

                abastecimento.DataAlteracao = DateTime.Now;
                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a Horímetro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private void PreencheEntidade(Dominio.Entidades.Abastecimento abastecimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoProduto = Request.GetIntParam("Produto");
            int codigoMovimento = Request.GetIntParam("TipoMovimento");
            int codigoEquipamento = Request.GetIntParam("Equipamento");
            int codigoCentroResultado = Request.GetIntParam("CentroResultado");
            int codigoLocalArmazenamento = Request.GetIntParam("LocalArmazenamento");

            double codigoPosto = Request.GetDoubleParam("Posto");

            abastecimento.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            abastecimento.Motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;
            abastecimento.Posto = codigoPosto > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPosto) : null;
            abastecimento.Produto = codigoProduto > 0 ? repProduto.BuscarPorCodigo(codigoProduto) : null;
            abastecimento.TipoMovimento = codigoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoMovimento) : null;
            abastecimento.Equipamento = codigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(codigoEquipamento) : null;
            abastecimento.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
            abastecimento.LocalArmazenamento = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : null;

            abastecimento.Kilometragem = Request.GetIntParam("KM");
            abastecimento.TipoAbastecimento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento>("TipoAbastecimento");
            abastecimento.Litros = Request.GetDecimalParam("Litros");
            abastecimento.ValorUnitario = Request.GetDecimalParam("ValorUnitario");
            abastecimento.Status = "A";
            abastecimento.Data = Request.GetDateTimeParam("Data");
            abastecimento.Documento = Request.GetStringParam("Documento");
            abastecimento.Horimetro = Request.GetIntParam("Horimetro");

            if (abastecimento.Codigo == 0 && (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                abastecimento.Empresa = this.Usuario.Empresa;

            abastecimento.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
            abastecimento.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
            abastecimento.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
            abastecimento.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");
            abastecimento.Observacao = Request.GetStringParam("Observacao");
            abastecimento.Requisicao = Request.GetBoolParam("Requisicao");
        }

        private bool ValidaEntidade(Dominio.Entidades.Abastecimento abastecimento, out string erro, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            erro = string.Empty;

            Dominio.Entidades.Veiculo veiculo = abastecimento.Veiculo;
            if (veiculo != null)
            {
                if (veiculo.Modelo != null && (veiculo.Modelo.PossuiArla32 == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao && abastecimento.Produto.CodigoNCM.StartsWith("310210")))
                {
                    erro = "O modelo do veículo selecionado não permite o lançamento de ARLA 32.";
                    return false;
                }

                if (veiculo.Tipo == "T" && abastecimento.TipoMovimento == null)
                {
                    erro = "Movimento Financeiro é obrigatório quando Veículo é de Terceiro.";
                    return false;
                }
            }

            if (abastecimento.Data.HasValue && abastecimento.Data.Value > DateTime.Now)
            {
                erro = "Não é possível lançar um abastecimento com data futura.";
                return false;
            }

            if (veiculo == null && abastecimento.Equipamento == null)
            {
                erro = "Favor informe um veículo ou um equipamento para o lançamento do abastecimento.";
                return false;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && abastecimento.Requisicao)
            {
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                decimal kmUltimoKMVeiculo = repAbastecimento.BuscarUltimoKMAbastecimento(abastecimento.Veiculo.Codigo, abastecimento.Data.Value, abastecimento.Codigo, abastecimento.TipoAbastecimento);
                if (abastecimento.TipoAbastecimento != TipoAbastecimento.Arla && configuracaoEmbarcador.KMLimiteEntreAbastecimentos > 0 && kmUltimoKMVeiculo > 0 && kmUltimoKMVeiculo < abastecimento.Kilometragem && abastecimento.Kilometragem > 0)
                {
                    if ((abastecimento.Kilometragem - kmUltimoKMVeiculo) > configuracaoEmbarcador.KMLimiteEntreAbastecimentos)
                    {                        
                        erro  = " A diferença de KM entre o último abastecimento (" + kmUltimoKMVeiculo.ToString("n0") + ") e o atual (" + abastecimento.Kilometragem.ToString("n0") + ") excede o limite cadastrado (" + configuracaoEmbarcador.KMLimiteEntreAbastecimentos.ToString("n0") + ")";
                        return false;
                    }
                }

                if (abastecimento.Kilometragem < abastecimento.KilometragemAnterior)
                {
                    erro = " Quilometragem não permitida abaixo do atual.";
                    return false;
                }
            }

            return string.IsNullOrEmpty(erro);
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAbastecimento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAbastecimento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAbastecimento()
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Veiculo = Request.GetIntParam("Veiculo"),
                Produto = Request.GetIntParam("Produto"),
                Equipamento = Request.GetIntParam("Equipamento"),
                Motorista = Request.GetIntParam("Motorista"),
                ClientePosto = Request.GetDoubleParam("Posto"),
                Quilometragem = Request.GetIntParam("Kilometragem"),
                Horimetro = Request.GetIntParam("Horimetro"),
                Situacao = Request.GetStringParam("Situacao"),
                Documento = Request.GetStringParam("Documento"),
                Placa = Request.GetStringParam("Placa"),
                NumeroDocumentoInicial = Request.GetIntParam("NumeroDocumentoInicial"),
                NumeroDocumentoFinal = Request.GetIntParam("NumeroDocumentoFinal"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                TipoAbastecimento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento>("TipoAbastecimento"),
                CodigoAbastecimentoIgnorar = Request.GetIntParam("Abastecimento"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado")
            };

            if ((this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas?.Count > 0)
            {
                filtrosPesquisa.CodigosEmpresa = Request.GetListParam<int>("Empresa");
                if (filtrosPesquisa.CodigosEmpresa == null || filtrosPesquisa.CodigosEmpresa.Count == 00)
                    filtrosPesquisa.CodigosEmpresa = this.Usuario.Empresas.Select(c => c.Codigo).ToList();
            }

            string[] codigosSituacoes = { "A", "I", "F", "G", "R" };
            if (!codigosSituacoes.Contains(filtrosPesquisa.Situacao))
                filtrosPesquisa.Situacao = "T";

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                filtrosPesquisa.CodigoUsuarioLogado = this.Usuario.Codigo;
            }



            return filtrosPesquisa;
        }

        #endregion Métodos Privados
    }
}
