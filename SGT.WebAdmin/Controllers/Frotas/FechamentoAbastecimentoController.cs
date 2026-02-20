using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Servicos.Embarcador.Financeiro;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize(new string[] { "BuscarAbastecimentos" }, "Frotas/FechamentoAbastecimento")]
    public class FechamentoAbastecimentoController : BaseController
    {
		#region Construtores

		public FechamentoAbastecimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> GerarFechamentoAbastecimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
               
                Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento = repFechamentoAbastecimento.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

                Servicos.DTO.ParametrosFechamentoAbastecimento parametrosFechamentoAbastecimento = new Servicos.DTO.ParametrosFechamentoAbastecimento();
                Servicos.Embarcador.Abastecimento.Interfaces.IFechamentoAbastecimento ServicoFechamentoAbastecimento = new Servicos.Embarcador.Abastecimento.FechamentoAbastecimento(TipoServicoMultisoftware, Auditado, unitOfWork, Usuario);

                if (fechamentoAbastecimento == null)
                {
                    fechamentoAbastecimento = new Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento();
                    parametrosFechamentoAbastecimento.CodigoFechamento = fechamentoAbastecimento.Codigo;
                    parametrosFechamentoAbastecimento.CodigoPosto = Request.GetDoubleParam("Posto");
                    parametrosFechamentoAbastecimento.CodigoVeiculo  = Request.GetIntParam("Veiculo");
                    parametrosFechamentoAbastecimento.CodigoEquipamento = Request.GetIntParam("Equipamento");
                    parametrosFechamentoAbastecimento.DataInicio = Request.GetDateTimeParam("DataInicio");
                    parametrosFechamentoAbastecimento.DataFim = Request.GetDateTimeParam("DataFim");
                   
                    List<int> codigosEmpresa = new List<int>();
                    if ((this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas?.Count > 0)
                    {
                        codigosEmpresa = Request.GetListParam<int>("Empresa");
                        if (codigosEmpresa == null || codigosEmpresa.Count == 0)
                            codigosEmpresa = this.Usuario.Empresas.Select(c => c.Codigo).ToList();

                        
                    }

                    parametrosFechamentoAbastecimento.CodigosEmpresa = codigosEmpresa;

                    string mensagemErro = string.Empty;

                    if (!ServicoFechamentoAbastecimento.GerarFechamentoAbastecimento(parametrosFechamentoAbastecimento, fechamentoAbastecimento, ref mensagemErro))
                        return new JsonpResult(false, true,  mensagemErro);

                }

                return new JsonpResult(fechamentoAbastecimento.Codigo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o fechamento.");
            }
            
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Frotas.FechamentoAbastecimentoEmpresa repFechamentoAbasteciemntoEmpresa = new Repositorio.Embarcador.Frotas.FechamentoAbastecimentoEmpresa(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento = repFechamentoAbastecimento.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa> fechamentoAbastecimentoEmpresa = repFechamentoAbasteciemntoEmpresa.BuscarPorFechametoAbastecimento(codigo);

                if (fechamentoAbastecimento == null)
                    return new JsonpResult(false, true, "Fechamento não encontrado.");

                var dynFechamentoAbastecimento = new
                {
                    fechamentoAbastecimento.Codigo,
                    Posto = fechamentoAbastecimento.Posto != null ? new { fechamentoAbastecimento.Posto.Codigo, Descricao = fechamentoAbastecimento.Posto.Nome } : null,
                    Veiculo = fechamentoAbastecimento.Veiculo != null ? new { fechamentoAbastecimento.Veiculo.Codigo, Descricao = fechamentoAbastecimento.Veiculo.Placa } : null,
                    DataInicio = fechamentoAbastecimento.DataInicio.HasValue ? fechamentoAbastecimento.DataInicio.Value.ToString("dd/MM/yyyy") : null,
                    DataFim = fechamentoAbastecimento.DataFim.HasValue ? fechamentoAbastecimento.DataFim.Value.ToString("dd/MM/yyyy") : null,
                    fechamentoAbastecimento.Situacao,
                    Equipamento = fechamentoAbastecimento.Equipamento != null ? new { fechamentoAbastecimento.Equipamento.Codigo, fechamentoAbastecimento.Equipamento.Descricao } : null,
                    Empresa = (
                        from obj in fechamentoAbastecimentoEmpresa
                        select new
                        {
                            Codigo = obj.Empresa.Codigo,
                            Descricao = obj.Empresa.Descricao,
                        }
                    ).ToList(),
                };
                return new JsonpResult(dynFechamentoAbastecimento);
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

        public async Task<IActionResult> BuscarAbastecimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                Models.Grid.EditableCell editableKM = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aInt, 10);
                Models.Grid.EditableCell editableLitros = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 7);
                Models.Grid.EditableCell editableValorUnitario = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 7);
                Models.Grid.EditableCell editableValor = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 7);
                Models.Grid.EditableCell editableHorimetro = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aInt, 7);

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("AdicionaAoFechamento", false);
                grid.AdicionarCabecalho("Veículo", "Placa", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Equipamento", "Equipamento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Abastec.", "TipoAbastecimento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Posto", "Posto", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("KM", "KM", 10, Models.Grid.Align.right, true, false, false, false, true, editableKM);
                grid.AdicionarCabecalho("Horímetro", "Horimetro", 10, Models.Grid.Align.right, true, false, false, false, true, editableHorimetro);
                grid.AdicionarCabecalho("Litros", "Litros", 10, Models.Grid.Align.right, true, false, false, false, true, editableLitros);
                grid.AdicionarCabecalho("Valor Unitário", "ValorUnitario", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false, false, false, false, true, editableValor);
                grid.AdicionarCabecalho("KM Total", "KMTotal", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Horímetro Total", "HorimetroTotal", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Média", "Media", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Média Horímetro", "MediaHorimetro", 10, Models.Grid.Align.right, false);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoEquipamento = Request.GetIntParam("Equipamento");
                double posto = Request.GetDoubleParam("Posto");
                string situacao = Request.GetStringParam("Situacao");

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento = repFechamentoAbastecimento.BuscarPorCodigo(codigo);

                IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento> listaAbastecimentos = repFechamentoAbastecimento.ConsultarAbastecimentosPorFechamento(codigo, 0, codigoVeiculo, codigoEquipamento, posto, situacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFechamentoAbastecimento.ContarAbastecimentosPorFechamento(codigo, 0, codigoVeiculo, codigoEquipamento, posto, situacao));

                var lista = (from obj in listaAbastecimentos select RetornarAbastecimentoDadosGrid(obj, true)).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os abastecimentos do fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaFechamentoAbastecimento filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Código", "Codigo", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Posto", "Posto", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Placa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Equipamento", "Equipamento", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Início", "DataInicio", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Fim", "DataFim", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Veiculo." + propOrdenar;
                else if (propOrdenar == "Posto")
                    propOrdenar = "Posto.Nome";
                else if (propOrdenar == "Equipamento")
                    propOrdenar = "Equipamento.Descricao";

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento> listaFechamentos = repFechamentoAbastecimento.ConsultarFechamentos(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFechamentoAbastecimento.ContarConsultarFechamentos(filtrosPesquisa));

                var lista = (from obj in listaFechamentos
                             select new
                             {
                                 obj.Codigo,
                                 Posto = obj.Posto?.Nome ?? string.Empty,
                                 Placa = obj.Veiculo?.Placa ?? string.Empty,
                                 DataInicio = obj.DataInicio.HasValue ? obj.DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataFim = obj.DataFim.HasValue ? obj.DataFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 Situacao = obj.DescricaoSituacao,
                                 Equipamento = obj.Equipamento?.Descricao ?? string.Empty
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
        }

        public async Task<IActionResult> FecharAbastecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string mensagemErro = string.Empty;

                // Codigo do fechamento
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Servicos.DTO.ParametrosFechamentoAbastecimento parametrosFechamentoAbastecimento = new Servicos.DTO.ParametrosFechamentoAbastecimento();
                Servicos.Embarcador.Abastecimento.Interfaces.IFechamentoAbastecimento ServicoFechamentoAbastecimento = new Servicos.Embarcador.Abastecimento.FechamentoAbastecimento(TipoServicoMultisoftware, Auditado,unitOfWork, Usuario);

                parametrosFechamentoAbastecimento.CodigoFechamento = codigo;
           
                if (!ServicoFechamentoAbastecimento.FecharAbastecimento(parametrosFechamentoAbastecimento, ref mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o fechamento.");
            }
        }

        public async Task<IActionResult> ReabrirFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);                
                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento = repFechamentoAbastecimento.BuscarPorCodigo(codigo, true);

                if (fechamentoAbastecimento == null)
                    return new JsonpResult(false, "Fechamento não encontrado.");

                if (fechamentoAbastecimento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Finalizado)
                    return new JsonpResult(false, "A situação do Fechamento não permite essa ação.");

                if (repFechamentoAbastecimento.AbastecimentosComTituloQuitado(codigo))
                    return new JsonpResult(false, "Existe abastecimento com títulos quitados, favor realizar a reversão do título antes.");

                // Busca os abastecimentos
                int totalAbastecimentos = repFechamentoAbastecimento.ContarConsultarPorFechamento(codigo, "T");
                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repFechamentoAbastecimento.ConsultarPorFechamento(codigo, "T", "Codigo", "", 0, totalAbastecimentos);

                string erro = "";
                int resetUnitOfWork = 0;
                for (int i = 0; i < listaAbastecimentos.Count; i++)
                {
                    unitOfWork.Start();
                    Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(listaAbastecimentos[i].Codigo, true);

                    // Reabre abastecimento
                    abastecimento.Situacao = "A";
                    abastecimento.DataAlteracao = DateTime.Now;
                    abastecimento.Integrado = false;

                    repAbastecimento.Atualizar(abastecimento, Auditado);

                    if (abastecimento.Produto != null && abastecimento.Produto.ProdutoCombustivel.HasValue && abastecimento.Produto.ControlaEstoqueCombustivel.HasValue && abastecimento.Produto.ProdutoCombustivel.Value && abastecimento.Produto.ControlaEstoqueCombustivel.Value)
                    {
                        Dominio.Entidades.Empresa empresa = abastecimento.Empresa;
                        if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && abastecimento.Posto != null)
                            empresa = repEmpresa.BuscarPorCNPJ(abastecimento.Posto.CPF_CNPJ_SemFormato);
                        if (empresa != null && !servicoEstoque.MovimentarEstoque(out erro, abastecimento.Produto, abastecimento.Litros, Dominio.Enumeradores.TipoMovimento.Entrada, "ABAST", abastecimento.Codigo.ToString(), abastecimento.ValorUnitario, empresa, abastecimento.Data.Value, TipoServicoMultisoftware, null, abastecimento.LocalArmazenamento))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, erro);
                        }
                    }

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && abastecimento.TipoMovimento != null)
                    {
                        string obsMovimentacao = "Reversão Fechamento de abastecimento do veículo " + abastecimento.Veiculo?.Placa ?? "";
                        if (!serProcessoMovimento.GerarMovimentacao(out erro, null, abastecimento.Data.Value, abastecimento.ValorTotal, abastecimento.Codigo.ToString(), obsMovimentacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, 0, abastecimento.TipoMovimento.PlanoDeContaDebito, abastecimento.TipoMovimento.PlanoDeContaCredito))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, erro);
                        }
                    }

                    if (abastecimento.GerarContasAPagarParaAbastecimentoExternos && abastecimento.TipoMovimentoPagamentoExterno != null && abastecimento.Posto != null)
                    {
                        if (!Servicos.Embarcador.Abastecimento.Abastecimento.EstornarTituloPagarAbastecimento(abastecimento, out erro, TipoServicoMultisoftware, Auditado, unitOfWork))
                            return new JsonpResult(false, true, erro);
                    }

                    if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita && abastecimento.Veiculo != null && (int)abastecimento.Kilometragem > 0)
                    {
                        if (abastecimento.Veiculo.KilometragemAtual == (int)abastecimento.Kilometragem && abastecimento.Veiculo.KilometragemAnterior > 0)
                        {
                            abastecimento.Veiculo.Initialize();
                            abastecimento.Veiculo.KilometragemAtual = abastecimento.Veiculo.KilometragemAnterior;
                            repVeiculo.Atualizar(abastecimento.Veiculo, Auditado, null, "Atualizada a Quilometragem Atual do Veículo via Reabertura de Fechamento de Abastecimento");
                        }
                    }

                    if (abastecimento.Equipamento != null && abastecimento.Equipamento.TrocaHorimetro)
                    {
                        Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(abastecimento.Equipamento.Codigo);
                        int ultimohorimetro = repAbastecimento.BuscarUltimoHorimetroAbastecimento(abastecimento.Equipamento.Codigo, abastecimento.Data.Value, abastecimento.Codigo, abastecimento.TipoAbastecimento);
                        if (ultimohorimetro == 0)
                        {
                            ultimohorimetro = equipamento.HorimetroAtualHistoricoHorimetro;
                        }
                        equipamento.Horimetro = equipamento.Horimetro - ((int)abastecimento.Horimetro - ultimohorimetro);
                        equipamento.HorimetroAtual = equipamento.HorimetroAtual - ((int)abastecimento.Horimetro - ultimohorimetro);
                        repEquipamento.Atualizar(equipamento);                        
                    }

                    unitOfWork.CommitChanges();

                    if (resetUnitOfWork > 20)
                    {
                        unitOfWork.FlushAndClear();
                        resetUnitOfWork = 0;
                    }
                    resetUnitOfWork++;
                }

                // Reabre fechamento                
                unitOfWork.Start();
                repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                fechamentoAbastecimento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Pendente;
                repFechamentoAbastecimento.Atualizar(fechamentoAbastecimento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar reabrir o fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Codigo do fechamento
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Instacia respositorios
                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                // Busca fechamento
                Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento = repFechamentoAbastecimento.BuscarPorCodigo(codigo, true);

                if (fechamentoAbastecimento == null)
                    return new JsonpResult(false, "Fechamento não encontrado.");

                if (fechamentoAbastecimento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.FalhaNaGeracao &&
                    fechamentoAbastecimento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Pendente)
                    return new JsonpResult(false, true, "A atual situação do fechamento(" + fechamentoAbastecimento.DescricaoSituacao + ") não permite o cancelamento da mesma.");

                // Busca os abastecimentos
                int totalAbastecimentos = repFechamentoAbastecimento.ContarConsultarPorFechamento(codigo, "T");
                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repFechamentoAbastecimento.ConsultarPorFechamento(codigo, "T", "Codigo", "", 0, totalAbastecimentos);

                int resetUnitOfWork = 0;
                for (int i = 0; i < listaAbastecimentos.Count; i++)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(listaAbastecimentos[i].Codigo, true);

                    // Desvinula fechamento dos abastecimentos e reabre
                    abastecimento.Situacao = "A";
                    abastecimento.DataAlteracao = DateTime.Now;
                    abastecimento.FechamentoAbastecimento = null;
                    abastecimento.Integrado = false;

                    repAbastecimento.Atualizar(abastecimento, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, abastecimento, null, "Cancelou o fechamento do abastecimento.", unitOfWork);

                    if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita && abastecimento.Veiculo != null && (int)abastecimento.Kilometragem > 0)
                    {
                        if (abastecimento.Veiculo.KilometragemAtual == (int)abastecimento.Kilometragem && abastecimento.Veiculo.KilometragemAnterior > 0)
                        {
                            abastecimento.Veiculo.Initialize();
                            abastecimento.Veiculo.KilometragemAtual = abastecimento.Veiculo.KilometragemAnterior;
                            repVeiculo.Atualizar(abastecimento.Veiculo, Auditado, null, "Atualizada a Quilometragem Atual do Veículo via Cancelamento de Fechamento de Abastecimento");
                        }
                    }

                    unitOfWork.CommitChanges();

                    if (resetUnitOfWork > 20)
                    {
                        unitOfWork.FlushAndClear();
                        resetUnitOfWork = 0;
                    }
                    resetUnitOfWork++;
                }

                // Deleta o fechamento
                unitOfWork.FlushAndClear();

                unitOfWork.Start();
                repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                fechamentoAbastecimento = repFechamentoAbastecimento.BuscarPorCodigo(codigo, true);

                repFechamentoAbastecimento.Deletar(fechamentoAbastecimento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarColunaAbastecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigoAbastecimento);

                decimal.TryParse(Request.Params("KM"), out decimal kilometragem);
                decimal.TryParse(Request.Params("Litros"), out decimal litros);
                decimal.TryParse(Request.Params("Valor"), out decimal valor);

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento, true);

                if (!abastecimento.Situacao.Equals("A"))
                    return new JsonpResult(false, true, "A atual situação do abastecimento (" + abastecimento.DescricaoSituacao + ") não permite sua alteração. ");

                abastecimento.Kilometragem = kilometragem;
                abastecimento.Horimetro = Request.GetIntParam("Horimetro");

                // Só recalcula o valor quando a quantia de litros ou valor total for modificado
                if (abastecimento.Litros != litros || abastecimento.ValorTotal != valor)
                {
                    abastecimento.Litros = litros;
                    abastecimento.ValorUnitario = valor / litros;
                }

                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                unitOfWork.CommitChanges();

                Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento objetoAbastecimento = repFechamentoAbastecimento.ConsultarAbastecimentosPorFechamento(0, codigoAbastecimento, 0, 0, 0).First();
                return new JsonpResult(RetornarAbastecimentoDadosGrid(objetoAbastecimento, true));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o abastecimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarAbastecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigoAbastecimento);

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento, true);

                if (!abastecimento.Situacao.Equals("A"))
                    return new JsonpResult(false, true, "A atual situação do abastecimento (" + abastecimento.DescricaoSituacao + ") não permite sua alteração. ");

                decimal.TryParse(Request.Params("KM"), out decimal kilometragem);
                decimal.TryParse(Request.Params("Litros"), out decimal litros);
                decimal.TryParse(Request.Params("ValorUnitario"), out decimal valorUnitario);

                abastecimento.Kilometragem = kilometragem;
                abastecimento.Litros = litros;
                abastecimento.ValorUnitario = valorUnitario;
                abastecimento.Horimetro = Request.GetIntParam("Horimetro");
                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                unitOfWork.CommitChanges();

                Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento objetoAbastecimento = repFechamentoAbastecimento.ConsultarAbastecimentosPorFechamento(0, codigoAbastecimento, 0, 0, 0).First();
                return new JsonpResult(RetornarAbastecimentoDadosGrid(objetoAbastecimento, true));
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

        public async Task<IActionResult> AdicionarAoFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigoAbastecimento);
                int.TryParse(Request.Params("Fechamento"), out int codigoFechamento);

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento = repFechamentoAbastecimento.BuscarPorCodigo(codigoFechamento);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento, true);

                if (fechamentoAbastecimento == null || abastecimento == null)
                    return new JsonpResult(false, true, "Erro ao buscar informações.");

                if (abastecimento.FechamentoAbastecimento != null)
                    return new JsonpResult(false, true, "Abastecimento selecionado já possui um fechamento.");

                abastecimento.FechamentoAbastecimento = fechamentoAbastecimento;
                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                unitOfWork.CommitChanges();

                Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento objetoAbastecimento = repFechamentoAbastecimento.ConsultarAbastecimentosPorFechamento(0, codigoAbastecimento, 0, 0, 0).First();
                return new JsonpResult(RetornarAbastecimentoDadosGrid(objetoAbastecimento, true));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar ao fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoveDoFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigoAbastecimento);

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento, true);

                if (abastecimento == null)
                    return new JsonpResult(false, true, "Erro ao buscar informações.");

                if (abastecimento.FechamentoAbastecimento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Pendente)
                    return new JsonpResult(false, true, "Não é possível remover abastecimentos de um fechamento finalizado.");

                abastecimento.FechamentoAbastecimento = null;
                abastecimento.Integrado = false;

                repAbastecimento.Atualizar(abastecimento, Auditado);

                unitOfWork.CommitChanges();

                Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento objetoAbastecimento = repFechamentoAbastecimento.ConsultarAbastecimentosPorFechamento(0, codigoAbastecimento, 0, 0, 0).First();
                return new JsonpResult(RetornarAbastecimentoDadosGrid(objetoAbastecimento, true));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover do fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTotalizadoresAbastecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoEquipamento = Request.GetIntParam("Equipamento");
                double posto = Request.GetDoubleParam("Posto");
                string situacao = Request.GetStringParam("Situacao");

                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repFechamentoAbastecimento.ConsultarPorFechamento(codigo, "", "Codigo", "", 0, 0);
                List<Dominio.Entidades.Produto> listaProdutosAbastecimentos = listaAbastecimentos.Select(o => o.Produto).Distinct().ToList();

                var dynTotalizadores = new
                {
                    TotalizadorKM = repFechamentoAbastecimento.BuscarKMAbastecimentoPorFechamento(codigo, codigoVeiculo, codigoEquipamento, posto, situacao, 0).ToString("n0"),
                    TotalizadorHorimetro = repFechamentoAbastecimento.BuscarHorimetroAbastecimentoPorFechamento(codigo, codigoVeiculo, codigoEquipamento, posto, situacao, 0).ToString("n0"),
                    TotalizadorLitros = repFechamentoAbastecimento.BuscarLitrosAbastecimentoPorFechamento(codigo, codigoVeiculo, codigoEquipamento, posto, situacao, 0).ToString("n2"),
                    TotalizadorValorTotal = repFechamentoAbastecimento.BuscarValorTotalAbastecimentoPorFechamento(codigo, codigoVeiculo, codigoEquipamento, posto, situacao, 0).ToString("n2"),

                    ListaSumarizadores = (from obj in listaProdutosAbastecimentos
                                          select new
                                          {
                                              Descricao = obj.Descricao,
                                              TotalizadorKM = repFechamentoAbastecimento.BuscarKMAbastecimentoPorFechamento(codigo, codigoVeiculo, codigoEquipamento, posto, situacao, obj.Codigo).ToString("n0"),
                                              TotalizadorHorimetro = repFechamentoAbastecimento.BuscarHorimetroAbastecimentoPorFechamento(codigo, codigoVeiculo, codigoEquipamento, posto, situacao, obj.Codigo).ToString("n0"),
                                              TotalizadorLitros = repFechamentoAbastecimento.BuscarLitrosAbastecimentoPorFechamento(codigo, codigoVeiculo, codigoEquipamento, posto, situacao, obj.Codigo).ToString("n2"),
                                              TotalizadorValorTotal = repFechamentoAbastecimento.BuscarValorTotalAbastecimentoPorFechamento(codigo, codigoVeiculo, codigoEquipamento, posto, situacao, obj.Codigo).ToString("n2"),
                                          }).ToList()
                };

                return new JsonpResult(dynTotalizadores);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar totalizadores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaFechamentoAbastecimento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaFechamentoAbastecimento()
            {
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento>("Situacao"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoEquipamento = Request.GetIntParam("Equipamento")
            };
        }

        private dynamic RetornarAbastecimentoDadosGrid(Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoFechamentoAbastecimento abastecimento, bool adicionaAoFechamento)
        {
            var retorno = new
            {
                abastecimento.Codigo,
                abastecimento.Placa,
                abastecimento.Posto,
                abastecimento.Equipamento,
                Data = abastecimento.DataFormatada,
                KM = abastecimento.KM.ToString("n0"),
                Horimetro = abastecimento.Horimetro.ToString("n0"),
                Litros = abastecimento.Litros.ToString("n2"),
                ValorUnitario = abastecimento.ValorUnitario.ToString("n4"),
                Valor = abastecimento.ValorTotal.ToString("n2"),
                KMTotal = abastecimento.KMTotal.ToString("n0"),
                Media = abastecimento.Media.ToString("n4"),
                HorimetroTotal = abastecimento.HorimetroTotal.ToString("n0"),
                MediaHorimetro = abastecimento.MediaHorimetro.ToString("n4"),
                TipoAbastecimento = abastecimento.TipoAbastecimentoFormatado,
                AdicionaAoFechamento = adicionaAoFechamento,
                DT_Enable = adicionaAoFechamento,
                DT_RowColor = abastecimento.ContagemDuplicado > 1 ? "#FF8C69" : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco
            };

            return retorno;
        }

        private void AtualizarKMVeiculoPneu(Dominio.Entidades.Abastecimento abastecimento, Repositorio.UnitOfWork unitOfWork)
        {
            if ((abastecimento.Kilometragem > 0 || abastecimento.Horimetro > 0) && (abastecimento.Veiculo != null || abastecimento.Equipamento != null))
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                decimal qtdKMRodado = 0;
                Dominio.Entidades.Veiculo veiculo = null;
                //atualiza km do veículo e seus pneus
                if (abastecimento.Kilometragem > 0 && abastecimento.Veiculo != null)
                {
                    veiculo = repVeiculo.BuscarPorCodigo(abastecimento.Veiculo.Codigo);

                    if (veiculo != null && veiculo.KilometragemAtual < abastecimento.Kilometragem)
                    {
                        qtdKMRodado = abastecimento.Kilometragem - (decimal)veiculo.KilometragemAtual;

                        if (veiculo.Pneus != null && veiculo.Pneus.Count > 0 && qtdKMRodado > 0)
                        {
                            foreach (var eixo in veiculo.Pneus)
                            {
                                Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                if (pneu != null)
                                {
                                    pneu.KmAnteriorRodado = 0;// pneu.KmAtualRodado;
                                    pneu.KmAtualRodado = (int)(pneu.KmAtualRodado + qtdKMRodado);
                                    if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                        pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                    repPneu.Atualizar(pneu);
                                }
                            }
                        }
                        if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                        {
                            veiculo.KilometragemAnterior = veiculo.KilometragemAtual;
                            veiculo.KilometragemAtual = (int)abastecimento.Kilometragem;
                        }

                        repVeiculo.Atualizar(veiculo, Auditado, null, "Atualizada a Quilometragem Atual do Veículo via Fechamento de Abastecimento");
                    }
                }

                if (abastecimento.Equipamento != null && abastecimento.Horimetro > 0)
                {
                    Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(abastecimento.Equipamento.Codigo);
                    if (equipamento != null)
                    {
                        if (equipamento.TrocaHorimetro)
                        {
                            if (equipamento != null && equipamento.HorimetroAtual < abastecimento.Horimetro)
                            {
                                equipamento.Horimetro = equipamento.Horimetro + ((int)abastecimento.Horimetro - equipamento.HorimetroAtual);
                                equipamento.HorimetroAtual = (int)abastecimento.Horimetro;
                                repEquipamento.Atualizar(equipamento);
                            }
                        }
                        else
                        {
                            if (equipamento != null && equipamento.Horimetro < abastecimento.Horimetro)
                            {
                                equipamento.Horimetro = (int)abastecimento.Horimetro;
                                repEquipamento.Atualizar(equipamento);
                            }
                        }
                    }                    
                }

                //atualiza km dos reboques e seus pneus
                if (veiculo != null && veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                {
                    foreach (var reboque in veiculo.VeiculosVinculados)
                    {
                        if (reboque != null && qtdKMRodado > 0)
                        {
                            if (reboque.Pneus != null && reboque.Pneus.Count > 0 && qtdKMRodado > 0)
                            {
                                foreach (var eixo in reboque.Pneus)
                                {
                                    Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                    if (pneu != null)
                                    {
                                        pneu.KmAnteriorRodado = 0;// pneu.KmAtualRodado;
                                        pneu.KmAtualRodado = (int)(pneu.KmAtualRodado + qtdKMRodado);
                                        if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                            pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                        repPneu.Atualizar(pneu);
                                    }
                                }
                            }
                            if (!ConfiguracaoEmbarcador.MovimentarKMApenasPelaGuarita && qtdKMRodado > 0)
                            {
                                reboque.KilometragemAnterior = reboque.KilometragemAtual;
                                reboque.KilometragemAtual = reboque.KilometragemAtual + (int)qtdKMRodado;
                            }

                            repVeiculo.Atualizar(reboque, Auditado, null, "Atualizada a Quilometragem Atual do Reboque via Fechamento de Abastecimento");
                        }
                    }
                }
            }
        }

        private bool GerarTituloPagamentoExterno(Dominio.Entidades.Abastecimento abastecimento, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento repGrupoPessoasVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento(unitOfWork);

            Dominio.Entidades.Cliente fornecedor = abastecimento.Posto;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = fornecedor?.GrupoPessoas ?? null;

            bool usaGrupoPessoas = false;
            bool gerarDuplicataNotaEntrada = false;
            int parcelasDuplicataNotaEntrada = 0;
            string intervaloDiasDuplicataNotaEntrada = string.Empty;
            int diaPadraoDuplicataNotaEntrada = 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo = fornecedor?.FormaTituloFornecedor ?? grupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;

            if (fornecedor != null && fornecedor.GerarDuplicataNotaEntrada)
            {
                gerarDuplicataNotaEntrada = fornecedor.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = fornecedor.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = fornecedor.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = fornecedor.DiaPadraoDuplicataNotaEntrada;
            }
            else if (grupoPessoas != null && grupoPessoas.GerarDuplicataNotaEntrada)
            {
                usaGrupoPessoas = true;
                gerarDuplicataNotaEntrada = grupoPessoas.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = grupoPessoas.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = grupoPessoas.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = grupoPessoas.DiaPadraoDuplicataNotaEntrada;
            }

            if (!gerarDuplicataNotaEntrada || parcelasDuplicataNotaEntrada == 0)
            {
                if (GerarTitulo(usuario, abastecimento, abastecimento.Posto, abastecimento.TipoMovimentoPagamentoExterno, abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, abastecimento.ValorTotal, 1, formaTitulo, TipoServicoMultisoftware, unitOfWork, unitOfWork.StringConexao, Auditado))
                    return true;
                else
                    return false;
            }

            int quantidadeParcelas = parcelasDuplicataNotaEntrada;
            decimal valorTotal = abastecimento.ValorTotal;
            decimal valorParcela = Math.Round((valorTotal / quantidadeParcelas), 2);
            decimal valorDiferenca = valorTotal - Math.Round((valorParcela * quantidadeParcelas), 2);
            string[] arrayDias = null;

            bool permiteMultiplosVencimentos;
            if (usaGrupoPessoas)
                permiteMultiplosVencimentos = grupoPessoas.PermitirMultiplosVencimentos;
            else
                permiteMultiplosVencimentos = fornecedor.Modalidades?.Where(f => f.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)?.FirstOrDefault()?.ModalidadesFornecedores?.FirstOrDefault()?.PermitirMultiplosVencimentos ?? false;

            if (permiteMultiplosVencimentos)
            {
                int diaEmissao = abastecimento.Data.HasValue ? abastecimento.Data.Value.Date.Day : DateTime.Now.Date.Day;
                int diaVencimento = 0;

                if (usaGrupoPessoas)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento = repGrupoPessoasVencimento.BuscarDiaVencimento(grupoPessoas.Codigo, diaEmissao);
                    diaVencimento = vencimento?.Vencimento ?? 0;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = repVencimento.BuscarDiaVencimento(fornecedor.CPF_CNPJ, diaEmissao);
                    diaVencimento = vencimento?.Vencimento ?? 0;
                }

                if (diaVencimento > 0)
                {
                    DateTime novaData = ProximaDataTabelaVencimento(abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, diaVencimento);

                    if (GerarTitulo(usuario, abastecimento, abastecimento.Posto, abastecimento.TipoMovimentoPagamentoExterno, abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, novaData, abastecimento.ValorTotal, 1, formaTitulo, TipoServicoMultisoftware, unitOfWork, unitOfWork.StringConexao, Auditado))
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                var x = intervaloDiasDuplicataNotaEntrada;
                if (x.IndexOf(".") >= 0)
                {
                    arrayDias = x.Split('.');
                    if (arrayDias.Length != quantidadeParcelas)
                    {
                        return false;
                    }
                    for (var i = 0; i < arrayDias.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(arrayDias[i]) || !(int.Parse(arrayDias[i]) > 0))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    arrayDias = new string[1];
                    arrayDias[0] = x;
                    if (string.IsNullOrWhiteSpace(arrayDias[0]) || !(int.Parse(arrayDias[0]) > 0))
                    {
                        return false;
                    }
                }
                var dataVencimento = abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date;

                for (var i = 0; i < quantidadeParcelas; i++)
                {
                    decimal valor = 0;
                    if (i == 0)
                        valor = Math.Round((valorParcela + valorDiferenca), 2);
                    else
                        valor = Math.Round(valorParcela, 2);

                    if (arrayDias.Length > 1)
                        dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[i]));
                    else
                        dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[0]));

                    DateTime novaData = dataVencimento;
                    if (i == 0 && diaPadraoDuplicataNotaEntrada > 0 && diaPadraoDuplicataNotaEntrada <= 31)
                    {
                        try
                        {
                            if (dataVencimento.Day > diaPadraoDuplicataNotaEntrada)
                                dataVencimento = dataVencimento.AddMonths(1);

                            novaData = new DateTime(dataVencimento.Year, dataVencimento.Month, diaPadraoDuplicataNotaEntrada);
                        }
                        catch
                        {
                            novaData = dataVencimento;
                        }
                    }
                    dataVencimento = novaData;

                    if (!GerarTitulo(usuario, abastecimento, abastecimento.Posto, abastecimento.TipoMovimentoPagamentoExterno, abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, dataVencimento, abastecimento.ValorTotal, i + 1, formaTitulo, TipoServicoMultisoftware, unitOfWork, unitOfWork.StringConexao, Auditado))
                        return false;
                }
            }
            return true;
        }

        private bool GerarTitulo(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Abastecimento abastecimento, Dominio.Entidades.Cliente fornecedor, Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, DateTime dataEmissao, DateTime dataVencimento, decimal valor, int sequencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(stringConexao);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

            titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
            titulo.DataEmissao = dataEmissao;
            titulo.DataVencimento = dataVencimento;
            titulo.DataProgramacaoPagamento = dataVencimento;
            titulo.Pessoa = fornecedor;
            titulo.GrupoPessoas = fornecedor.GrupoPessoas;
            if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
            titulo.Sequencia = sequencia;
            titulo.ValorOriginal = valor;
            titulo.ValorPendente = valor;
            titulo.Desconto = 0;
            titulo.Acrescimo = 0;
            titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.Observacao = "Abastecimento Externo";
            titulo.Empresa = abastecimento.Empresa;
            titulo.ValorTituloOriginal = titulo.ValorOriginal;
            titulo.TipoDocumentoTituloOriginal = "Abastecimento";
            titulo.NumeroDocumentoTituloOriginal = !string.IsNullOrEmpty(abastecimento.Documento) ? abastecimento.Documento : Utilidades.String.OnlyNumbers(abastecimento.Codigo.ToString("n0"));
            titulo.FormaTitulo = formaTitulo;
            titulo.NossoNumero = string.Empty;
            titulo.TipoMovimento = tipoMovimento;
            titulo.Provisao = false;
            titulo.DataLancamento = DateTime.Now;
            titulo.Usuario = usuario;
            titulo.Abastecimento = abastecimento;

            repTitulo.Inserir(titulo, auditado);

            if (!svcProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.NumeroDocumentoTituloOriginal, titulo.Observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa.GrupoPessoas, titulo.DataEmissao.Value))
                return false;

            return true;
        }

        private DateTime ProximaDataTabelaVencimento(DateTime dataEmissao, int vencimento)
        {
            DateTime novaData;
            DateTime proximoMesAno = dataEmissao.Date;
            int novoDia = vencimento;
            if (proximoMesAno.Day > novoDia)
                proximoMesAno = proximoMesAno.AddMonths(1);
            int diasMes = DateTime.DaysInMonth(proximoMesAno.Year, proximoMesAno.Month);
            if (novoDia > diasMes)
                novoDia = diasMes;

            try
            {
                novaData = new DateTime(proximoMesAno.Year, proximoMesAno.Month, novoDia);
            }
            catch
            {
                novaData = dataEmissao.Date;
            }

            return novaData;
        }

        #endregion
    }
}
