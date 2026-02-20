using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/ComissaoFuncionario")]
    public class ComissaoFuncionarioController : BaseController
    {
		#region Construtores

		public ComissaoFuncionarioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario situacaoComissaoFuncionario = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario)int.Parse(Request.Params("SituacaoComissaoFuncionario"));

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Operador", "UsuarioGerouComissao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Inicial", "DataInicio", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Final", "DataFim", 15, Models.Grid.Align.center, false);
                if (ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                {
                    grid.AdicionarCabecalho("PercentualBaseCalculoComissao", false);
                    grid.AdicionarCabecalho("ValorDiaria", false);
                }
                else
                {
                    grid.AdicionarCabecalho("Percentual BC", "PercentualBaseCalculoComissao", 10, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Valor da Diária", "ValorDiaria", 10, Models.Grid.Align.right, false);
                }
                if (situacaoComissaoFuncionario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacaoComissaoFuncionario", 20, Models.Grid.Align.center, false);


                List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario> listaComissaoFuncionario = repComissaoFuncionario.Consultar(dataInicio, dataFim, situacaoComissaoFuncionario, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repComissaoFuncionario.ContarConsulta(dataInicio, dataFim, situacaoComissaoFuncionario));
                var lista = (from p in listaComissaoFuncionario
                             select new
                             {
                                 p.Codigo,
                                 DataInicio = p.DataInicio.ToString("dd/MM/yyyy"),
                                 DataFim = p.DataFim.ToString("dd/MM/yyyy"),
                                 PercentualBaseCalculoComissao = p.PercentualBaseCalculoComissao.ToString("n2"),
                                 p.DescricaoSituacaoComissaoFuncionario,
                                 UsuarioGerouComissao = p.UsuarioGerouComissao.Nome,
                                 ValorDiaria = p.ValorDiaria.ToString("n2"),
                                 Localidade = p.Localidade?.DescricaoCidadeEstado ?? "",
                                 Motorista = p.Motorista != null ? p.Motorista.Nome : string.Empty,
                                 Descricao = p.DataInicio.ToString("dd/MM/yyyy") + " à " + p.DataFim.ToString("dd/MM/yyyy")
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

        public async Task<IActionResult> Adicionar()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Criar))
                {
                    Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                    Repositorio.Embarcador.Pessoas.Cargo repCargo = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                    DateTime dataInicio;
                    DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

                    DateTime dataFim;
                    DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                    int motorista = 0;
                    int.TryParse(Request.Params("Motorista"), out motorista);
                    int cargoMotorista = Request.GetIntParam("CargoMotorista");
                    bool incluirCargasAberto = Request.GetBoolParam("IncluirCargasAberto");

                    if (incluirCargasAberto)
                    {
                        DateTime.TryParseExact(Request.Params("DataInicioCargasAberto"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                        DateTime.TryParseExact(Request.Params("DataFimCargasAberto"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);
                    }

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.VerificarComissaoPeriodoJaExiste(dataInicio, dataFim, motorista, cargoMotorista);

                    if (comissaoFuncionario == null)
                    {
                        comissaoFuncionario = new Dominio.Entidades.Embarcador.RH.ComissaoFuncionario();
                        int localidade = 0;
                        int.TryParse(Request.Params("Localidade"), out localidade);

                        int centroResultado = 0;
                        int.TryParse(Request.Params("CentroResultado"), out centroResultado);

                        comissaoFuncionario.DataAlteracao = DateTime.Now;
                        comissaoFuncionario.DataFim = dataFim;
                        comissaoFuncionario.DataInicio = dataInicio;
                        comissaoFuncionario.Localidade = localidade > 0 ? repLocalidade.BuscarPorCodigo(localidade) : null;
                        comissaoFuncionario.CentroResultado = centroResultado > 0 ? repCentroResultado.BuscarPorCodigo(centroResultado) : null;
                        comissaoFuncionario.CargoMotorista = cargoMotorista > 0 ? repCargo.BuscarPorCodigo(cargoMotorista) : null;
                        comissaoFuncionario.Motorista = motorista > 0 ? repUsuario.BuscarPorCodigo(motorista) : null;
                        comissaoFuncionario.MesagemBaseCalculoComissao = Request.Params("MesagemBaseCalculoComissao");
                        comissaoFuncionario.NumeroDiasEmViagem = Request.GetIntParam("NumeroDiasEmViagem");
                        comissaoFuncionario.PercentualBaseCalculoComissao = Request.GetDecimalParam("PercentualBaseCalculoComissao");
                        comissaoFuncionario.PercentualComissao = (Request.GetDecimalParam("PercentualComissao"));
                        comissaoFuncionario.UsuarioGerouComissao = this.Usuario;
                        comissaoFuncionario.ImportarPlanilhaListagemMotoristas = Request.GetBoolParam("ImportarPlanilhaListagemMotoristas");
                        
                        if(comissaoFuncionario.ImportarPlanilhaListagemMotoristas)
                            comissaoFuncionario.SituacaoComissaoFuncionario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.AgImportacaoPlanilha;
                        else
                            comissaoFuncionario.SituacaoComissaoFuncionario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.EmGeracao;

                        comissaoFuncionario.ValorDiaria = (Request.GetDecimalParam("ValorDiaria"));
                        repComissaoFuncionario.Inserir(comissaoFuncionario, Auditado);
                        GerarLog(comissaoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario.Criar, unitOfWork);
                        unitOfWork.CommitChanges();
                        return new JsonpResult(comissaoFuncionario.Codigo);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        if (motorista > 0)
                            return new JsonpResult(false, true, "Já existe uma comissão para este motorista que possui conflito de datas com o periodo selecionado de " + dataInicio.ToString("dd/MM/yyyy") + " até " + dataFim.ToString("dd/MM/yyyy") + ".");
                        else
                            return new JsonpResult(false, true, "Já existe uma comissão que possui conflito de datas com o periodo selecionado de " + dataInicio.ToString("dd/MM/yyyy") + " até " + dataFim.ToString("dd/MM/yyyy") + ".");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
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

        public async Task<IActionResult> TentarGerarNovamente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Criar))
                {

                    int codigo = int.Parse(Request.Params("Codigo"));
                    Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigo);

                    if (comissaoFuncionario.SituacaoComissaoFuncionario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.FalhaNaGeracao)
                    {
                        comissaoFuncionario.SituacaoComissaoFuncionario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.EmGeracao;
                        repComissaoFuncionario.Atualizar(comissaoFuncionario);
                        GerarLog(comissaoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario.GerarNovamente, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionario, null, "Tentou Gerar Novamente.", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite o reenvio da mesma.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar gerar novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarComissao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Cancelar))
                {
                    int codigo = int.Parse(Request.Params("Codigo"));
                    Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigo);

                    if (comissaoFuncionario.SituacaoComissaoFuncionario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.FalhaNaGeracao ||
                        comissaoFuncionario.SituacaoComissaoFuncionario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Gerada ||
                        comissaoFuncionario.SituacaoComissaoFuncionario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.AgImportacaoPlanilha)
                    {
                        comissaoFuncionario.SituacaoComissaoFuncionario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada;
                        repComissaoFuncionario.Atualizar(comissaoFuncionario);
                        GerarLog(comissaoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario.Cancelar, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionario, null, "Cancelou Comissão.", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite o cancelamento da mesma.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar gerar novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReabrirComissao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ReAbrir))
                {
                    int codigo = int.Parse(Request.Params("Codigo"));
                    Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigo);

                    if (comissaoFuncionario.SituacaoComissaoFuncionario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada)
                    {
                        comissaoFuncionario.SituacaoComissaoFuncionario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Gerada;
                        repComissaoFuncionario.Atualizar(comissaoFuncionario);
                        GerarLog(comissaoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario.Cancelar, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionario, null, "Reabriu Comissão.", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite a reabertura da mesma.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar gerar novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarComissao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Finalizar))
                {
                    int codigo = int.Parse(Request.Params("Codigo"));
                    Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigo);

                    if (comissaoFuncionario.SituacaoComissaoFuncionario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Gerada)
                    {
                        comissaoFuncionario.SituacaoComissaoFuncionario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada;
                        repComissaoFuncionario.Atualizar(comissaoFuncionario);
                        GerarLog(comissaoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario.Cancelar, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionario, null, "Finalizou Comissão.", unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite a finalização da mesma.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar gerar novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                {
                    Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                    Repositorio.Embarcador.Pessoas.Cargo repCargo = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);
                    Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));

                    if (comissaoFuncionario.SituacaoComissaoFuncionario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Gerada)
                    {

                        int localidade = 0, motorista = 0, centroResultado = 0;
                        int.TryParse(Request.Params("Localidade"), out localidade);
                        int.TryParse(Request.Params("Motorista"), out motorista);
                        int cargoMotorista = Request.GetIntParam("CargoMotorista");
                        int.TryParse(Request.Params("CentroResultado"), out centroResultado);

                        bool incluirCargasAberto = Request.GetBoolParam("IncluirCargasAberto");

                        if (incluirCargasAberto)
                        {
                            DateTime dataInicio = comissaoFuncionario.DataInicio;
                            DateTime dataFim = comissaoFuncionario.DataFim;

                            DateTime.TryParseExact(Request.Params("DataInicioCargasAberto"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                            DateTime.TryParseExact(Request.Params("DataFimCargasAberto"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                            comissaoFuncionario.DataInicio = dataInicio;
                            comissaoFuncionario.DataFim = dataFim;
                        }

                        comissaoFuncionario.DataAlteracao = DateTime.Now;
                        comissaoFuncionario.Localidade = localidade > 0 ? repLocalidade.BuscarPorCodigo(localidade) : null;
                        comissaoFuncionario.CentroResultado = centroResultado > 0 ? repCentroResultado.BuscarPorCodigo(centroResultado) : null;
                        comissaoFuncionario.CargoMotorista = cargoMotorista > 0 ? repCargo.BuscarPorCodigo(cargoMotorista) : null;
                        comissaoFuncionario.Motorista = motorista > 0 ? repUsuario.BuscarPorCodigo(motorista) : null;
                        comissaoFuncionario.MesagemBaseCalculoComissao = Request.Params("MesagemBaseCalculoComissao");
                        comissaoFuncionario.ImportarPlanilhaListagemMotoristas = Request.GetBoolParam("ImportarPlanilhaListagemMotoristas");

                        GerarLog(comissaoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario.Alterar, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionario, null, "Atualizou Comissão.", unitOfWork);

                        repComissaoFuncionario.Atualizar(comissaoFuncionario);
                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite a atualização da mesma.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Você não possui permissões para executar está ação.");
                }
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigo);

                var dynComissaoFuncionario = new
                {
                    comissaoFuncionario.Codigo,
                    DataFim = comissaoFuncionario.DataFim.ToString("dd/MM/yyyy"),
                    DataInicio = comissaoFuncionario.DataInicio.ToString("dd/MM/yyyy"),
                    Localidade = comissaoFuncionario.Localidade != null ? new { comissaoFuncionario.Localidade.Codigo, Descricao = comissaoFuncionario.Localidade.DescricaoCidadeEstado } : null,
                    CentroResultado = comissaoFuncionario.CentroResultado != null ? new { comissaoFuncionario.CentroResultado.Codigo, Descricao = comissaoFuncionario.CentroResultado.Descricao } : null,
                    CargoMotorista = comissaoFuncionario.CargoMotorista != null ? new { comissaoFuncionario.CargoMotorista.Codigo, Descricao = comissaoFuncionario.CargoMotorista.Descricao } : null,
                    Motorista = comissaoFuncionario.Motorista != null ? new { comissaoFuncionario.Motorista.Codigo, Descricao = comissaoFuncionario.Motorista.Nome } : null,
                    comissaoFuncionario.MesagemBaseCalculoComissao,
                    comissaoFuncionario.NumeroDiasEmViagem,
                    comissaoFuncionario.PercentualGerado,
                    PercentualBaseCalculoComissao = comissaoFuncionario.PercentualBaseCalculoComissao.ToString("n2"),
                    PercentualComissao = comissaoFuncionario.PercentualComissao.ToString("n2"),
                    comissaoFuncionario.MensagemFalhaGeracao,
                    ValorDiaria = comissaoFuncionario.ValorDiaria.ToString("n2"),
                    comissaoFuncionario.SituacaoComissaoFuncionario,
                    comissaoFuncionario.ImportarPlanilhaListagemMotoristas
                };
                return new JsonpResult(dynComissaoFuncionario);
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

        public async Task<IActionResult> BaixarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R029_ComissaoMotoristas, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R029_ComissaoMotoristas, TipoServicoMultisoftware, "Relatorio Comissão dos Motoristas", "RH", "ComissaoMotoristas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, true);

                int codigo = int.Parse(Request.Params("Codigo"));
                Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivo = Request.GetEnumParam<Dominio.Enumeradores.TipoArquivoRelatorio>("TipoArquivo");

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, tipoArquivo, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                bool utilizarComissaoPorCargo = ConfiguracaoEmbarcador.UtilizarComissaoPorCargo;
                Task.Factory.StartNew(() => GerarRelatorioComissoes(codigo, nomeCliente, stringConexao, utilizarComissaoPorCargo, relatorioControleGeracao));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        public async Task<IActionResult> ExportarCSV()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);

                IList<Dominio.ObjetosDeValor.Embarcador.RH.ExportacaoComissaoMotorista> comissoes = repComissaoFuncionarioMotorista.ConsultarParaExportacao(codigo);

                System.Text.StringBuilder sbCSV = new System.Text.StringBuilder();

                if (!ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                    sbCSV.Append("Nome do Motorista;CPF do Motorista;Código de Integração;Valor do Frete Líquido;Modelo Veicular de Carga;").AppendLine();
                else
                    sbCSV.Append("Nome do Motorista;CPF do Motorista;Código de Contábil;Valor da Comissão;").AppendLine();

                foreach (Dominio.ObjetosDeValor.Embarcador.RH.ExportacaoComissaoMotorista comissao in comissoes)
                {
                    if (!ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                    {
                        sbCSV.Append(comissao.NomeMotorista).Append(";");
                        sbCSV.Append(comissao.CPFMotorista).Append(";");
                        sbCSV.Append(comissao.CodigoIntegracao).Append(";");
                        sbCSV.Append(comissao.ValorFreteLiquido.ToString("n2")).Append(";");
                        sbCSV.Append(comissao.ModeloVeicularCarga).Append(";");
                        sbCSV.AppendLine();
                    }
                    else
                    {
                        sbCSV.Append(comissao.NomeMotorista).Append(";");
                        sbCSV.Append(comissao.CPFMotorista).Append(";");
                        sbCSV.Append(comissao.CodigoContabil).Append(";");
                        sbCSV.Append(comissao.ValorComissao.ToString("n2")).Append(";");
                        sbCSV.AppendLine();
                    }
                }

                return Arquivo(System.Text.Encoding.UTF8.GetBytes(sbCSV.ToString()), "text/csv", "Comissões dos Motoristas.csv");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoComissaoFuncionario(unitOfWork);
                string dados = Request.Params("Dados");
                int codigoComissaoFuncionario = Request.GetIntParam("CodigoComissaoFuncionario");
                Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigoComissaoFuncionario);

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                if (comissaoFuncionario == null)
                    return new JsonpResult(false, true, "Comissão de Funcionário não encontrada.");

                unitOfWork.FlushAndClear();
                unitOfWork.Start();

                bool erro = linhas.Count == 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    Dominio.ObjetosDeValor.Embarcador.RH.ImportacaoComissaoFuncionarioMotorista comissaoMotorista = new Dominio.ObjetosDeValor.Embarcador.RH.ImportacaoComissaoFuncionarioMotorista();

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFFuncionario = (from obj in linha.Colunas where obj.NomeCampo == "CPFMotorista" select obj).FirstOrDefault();
                    comissaoMotorista.CPFFuncionario = "";
                    if (colCPFFuncionario != null)
                        comissaoMotorista.CPFFuncionario = colCPFFuncionario.Valor;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPercentualComissaoMedia = (from obj in linha.Colunas where obj.NomeCampo == "PercentualComissaoMedia" select obj).FirstOrDefault();
                    comissaoMotorista.PercentualComissaoMedia = 0;
                    if (colPercentualComissaoMedia != null)
                        comissaoMotorista.PercentualComissaoMedia = decimal.Parse(colPercentualComissaoMedia.Valor);

                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCPF(comissaoMotorista.CPFFuncionario);

                    if (motorista == null)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Motorista não encontrado no sistema", i));
                        erro = true;
                    }
                    else
                    {
                        Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                        bool sucessoProcessamento = serComissaoFuncionario.GerarComissaoMotoristas(motorista.Codigo, comissaoFuncionario.CargoMotorista != null ? comissaoFuncionario.CargoMotorista.Codigo : 0, comissaoFuncionario.Codigo, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao, comissaoMotorista.PercentualComissaoMedia, true, unitOfWork);

                        if (!sucessoProcessamento)
                            erro = true;

                        contador++;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);
                         
                    }
                }
                if (!erro)
                {
                    unitOfWork.CommitChanges();

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissao = repComissaoFuncionario.BuscarPorCodigo(codigoComissaoFuncionario);
                    comissao.SituacaoComissaoFuncionario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Gerada;
                    repComissaoFuncionario.Atualizar(comissao);
                }
                else
                {
                    unitOfWork.Rollback();

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissao = repComissaoFuncionario.BuscarPorCodigo(codigoComissaoFuncionario);
                    comissao.SituacaoComissaoFuncionario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.AgImportacaoPlanilha;
                    repComissaoFuncionario.Atualizar(comissao);
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
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoComissaoFuncionario(unitOfWork);

            return new JsonpResult(configuracoes.ToList());
        }
        #endregion

        #region Métodos Privados

        private void GerarRelatorioComissoes(int codigoComissao, string nomeCliente, string stringConexao, bool utilizarComissaoPorCargo, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
            try
            {
                IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista> comissoesFuncionarioMotorista = serComissaoFuncionario.BuscarListaDataSetComissao(codigoComissao, unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento> comissaoFuncionarioMotoristaAbastecimento = serComissaoFuncionario.BuscarListaDataSetComissaoAbastecimento(codigoComissao, unitOfWork);
                if (comissaoFuncionarioMotoristaAbastecimento == null || comissaoFuncionarioMotoristaAbastecimento.Count == 0)
                {
                    comissaoFuncionarioMotoristaAbastecimento = new List<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento>();
                    comissaoFuncionarioMotoristaAbastecimento.Add(new Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento { CodigoComissaoFuncionarioMotorista = -1 });
                }

                ReportRequest.WithType(ReportType.ComissaoMotoristas)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoComissao", codigoComissao)
                    .AddExtraData("nomeCliente",nomeCliente )                    
                    .AddExtraData("utilizarComissaoPorCargo", utilizarComissaoPorCargo)                    
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("retornaArrayByte", false)
                    .CallReport()
                    .GetContentFile();
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

        private void GerarLog(Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoComissaoFuncionario tipoAcaoComissaoFuncionario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RH.ComissaoFuncionarioLog repComissaoFuncionarioLog = new Repositorio.Embarcador.RH.ComissaoFuncionarioLog(unitOfWork);
            Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioLog comissaoFuncionarioLog = new Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioLog();
            comissaoFuncionarioLog.ComissaoFuncionario = comissaoFuncionario;
            comissaoFuncionarioLog.DataHora = DateTime.Now;
            comissaoFuncionarioLog.TipoAcao = tipoAcaoComissaoFuncionario;
            comissaoFuncionarioLog.Usuario = this.Usuario;
            repComissaoFuncionarioLog.Inserir(comissaoFuncionarioLog);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoComissaoFuncionario(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CPF Motorista", Propriedade = "CPFMotorista", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Nome Motorista", Propriedade = "NomeMotorista", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "% Comissão Média", Propriedade = "PercentualComissaoMedia", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }
        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = true };
            return retorno;
        }
        #endregion
    }
}
