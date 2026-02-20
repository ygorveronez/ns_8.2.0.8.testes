using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Patrimonio
{
    [CustomAuthorize("Patrimonio/TransferenciaBem")]
    public class TransferenciaBemController : BaseController
    {
		#region Construtores

		public TransferenciaBemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFuncionario = 0;
                int.TryParse(Request.Params("Funcionario"), out codigoFuncionario);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                DateTime.TryParse(Request.Params("DataEnvio"), out DateTime dataEnvio);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Funcionário Responsável", "Funcionario", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Envio", "DataEnvio", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Almoxarifado", "Almoxarifado", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Centro de Resultado", "CentroResultado", 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Patrimonio.BemTransferencia repBemTransferencia = new Repositorio.Embarcador.Patrimonio.BemTransferencia(unitOfWork);
                List<Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia> transferenciasBem = repBemTransferencia.Consultar(codigoEmpresa, codigoFuncionario, dataEnvio, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repBemTransferencia.ContarConsulta(codigoEmpresa, codigoFuncionario, dataEnvio));

                var lista = (from p in transferenciasBem
                             select new
                             {
                                 p.Codigo,
                                 Funcionario = p.Funcionario != null ? p.Funcionario.Descricao : string.Empty,
                                 DataEnvio = p.DataEnvio.ToString("dd/MM/yyyy"),
                                 Almoxarifado = p.Almoxarifado != null ? p.Almoxarifado.Descricao : string.Empty,
                                 CentroResultado = p.CentroResultado != null ? p.CentroResultado.Descricao : string.Empty
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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Patrimonio.BemTransferencia repBemTransferencia = new Repositorio.Embarcador.Patrimonio.BemTransferencia(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia bemTransferencia = new Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia();

                PreencherBemTransferencia(bemTransferencia, unitOfWork);
                repBemTransferencia.Inserir(bemTransferencia, Auditado);

                SalvarBens(bemTransferencia, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Patrimonio.BemTransferencia repBemTransferencia = new Repositorio.Embarcador.Patrimonio.BemTransferencia(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia bemTransferencia = repBemTransferencia.BuscarPorCodigo(codigo, true);

                PreencherBemTransferencia(bemTransferencia, unitOfWork);
                repBemTransferencia.Atualizar(bemTransferencia, Auditado);

                SalvarBens(bemTransferencia, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Patrimonio.BemTransferencia repBemTransferencia = new Repositorio.Embarcador.Patrimonio.BemTransferencia(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia bemTransferencia = repBemTransferencia.BuscarPorCodigo(codigo);

                var dynBemTransferencia = new
                {
                    bemTransferencia.Codigo,
                    DataEnvio = bemTransferencia.DataEnvio.ToString("dd/MM/yyyy"),
                    DataRecebimento = bemTransferencia.DataRecebimento.HasValue ? bemTransferencia.DataRecebimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    bemTransferencia.ObservacaoEnvio,
                    bemTransferencia.ObservacaoSaida,
                    CentroResultado = bemTransferencia.CentroResultado != null ? new { bemTransferencia.CentroResultado.Codigo, bemTransferencia.CentroResultado.Descricao } : null,
                    Almoxarifado = bemTransferencia.Almoxarifado != null ? new { bemTransferencia.Almoxarifado.Codigo, bemTransferencia.Almoxarifado.Descricao } : null,
                    Funcionario = bemTransferencia.Funcionario != null ? new { bemTransferencia.Funcionario.Codigo, bemTransferencia.Funcionario.Descricao } : null,
                    FuncionarioEnvio = bemTransferencia.FuncionarioEnvio != null ? new { bemTransferencia.FuncionarioEnvio.Codigo, bemTransferencia.FuncionarioEnvio.Descricao } : null,
                    Pessoa = bemTransferencia.Pessoa != null ? new { bemTransferencia.Pessoa.Codigo, bemTransferencia.Pessoa.Descricao } : null,
                    Bens = (from obj in bemTransferencia.Bens
                            select new
                            {
                                BEM = new
                                {
                                    obj.Bem.Codigo,
                                    obj.Bem.Descricao,
                                    obj.Bem.NumeroSerie,
                                    DataFimGarantia = obj.Bem.DataFimGarantia.ToString("dd/MM/yyyy"),
                                    ValorBem = obj.Bem.ValorBem.ToString("n4")
                                }
                            }).ToList()
                };

                return new JsonpResult(dynBemTransferencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Patrimonio.BemTransferencia repBemTransferencia = new Repositorio.Embarcador.Patrimonio.BemTransferencia(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia bemTransferencia = repBemTransferencia.BuscarPorCodigo(codigo, true);

                if (bemTransferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repBemTransferencia.Deletar(bemTransferencia, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherBemTransferencia(Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia bemTransferencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params("Almoxarifado"), out int codigoAlmoxarifado);
            int.TryParse(Request.Params("CentroResultado"), out int codigoCentroResultado);
            int.TryParse(Request.Params("Funcionario"), out int codigoFuncionario);
            int.TryParse(Request.Params("FuncionarioEnvio"), out int codigoFuncionarioEnvio);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            double.TryParse(Request.Params("Pessoa"), out double pessoa);

            string observacaoSaida = Request.Params("ObservacaoSaida");
            string observacaoEnvio = Request.Params("ObservacaoEnvio");

            DateTime.TryParse(Request.Params("DataEnvio"), out DateTime dataEnvio);
            DateTime.TryParse(Request.Params("DataRecebimento"), out DateTime dataRecebimento);

            bemTransferencia.DataEnvio = dataEnvio;
            if (dataRecebimento > DateTime.MinValue)
                bemTransferencia.DataRecebimento = dataRecebimento;
            else
                bemTransferencia.DataRecebimento = null;
            bemTransferencia.ObservacaoSaida = observacaoSaida;
            bemTransferencia.ObservacaoEnvio = observacaoEnvio;

            bemTransferencia.Almoxarifado = repAlmoxarifado.BuscarPorCodigo(codigoAlmoxarifado);
            bemTransferencia.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);
            bemTransferencia.Funcionario = repFuncionario.BuscarPorCodigo(codigoFuncionario);
            bemTransferencia.FuncionarioEnvio = repFuncionario.BuscarPorCodigo(codigoFuncionarioEnvio);
            bemTransferencia.Pessoa = repCliente.BuscarPorCPFCNPJ(pessoa);
            if (codigoEmpresa > 0)
                bemTransferencia.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            else
                bemTransferencia.Empresa = bemTransferencia.Funcionario.Empresa;
        }

        private void SalvarBens(Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia bemTransferencia, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Patrimonio.BemTransferenciaItem repBemTransferenciaItem = new Repositorio.Embarcador.Patrimonio.BemTransferenciaItem(unidadeTrabalho);
            Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unidadeTrabalho);

            dynamic bens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Bens"));
            if (bemTransferencia.Bens != null && bemTransferencia.Bens.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var bem in bens)
                    if (bem.BEM.Codigo != null)
                        codigos.Add((int)bem.BEM.Codigo);

                List<Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem> bemItemDeletar = (from obj in bemTransferencia.Bens where !codigos.Contains(obj.Bem.Codigo) select obj).ToList();

                for (var i = 0; i < bemItemDeletar.Count; i++)
                    repBemTransferenciaItem.Deletar(bemItemDeletar[i]);
            }
            else
                bemTransferencia.Bens = new List<Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem>();

            foreach (var bem in bens)
            {
                Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem bemItem = bem.BEM.Codigo != null ? repBemTransferenciaItem.BuscarPorBemETransferencia((int)bem.BEM.Codigo, bemTransferencia.Codigo) : null;
                if (bemItem == null)
                {
                    bemItem = new Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem();

                    int.TryParse((string)bem.BEM.Codigo, out int codigoBem);
                    bemItem.Bem = repBem.BuscarPorCodigo(codigoBem);
                    bemItem.BemTransferencia = bemTransferencia;

                    repBemTransferenciaItem.Inserir(bemItem);
                }
            }
        }

        #endregion
    }
}
