using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/ManutencaoCentroResultado")]
    public class ManutencaoCentroResultadoController : BaseController
    {
		#region Construtores

		public ManutencaoCentroResultadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        public async Task<IActionResult> BuscarMotoristas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridMotoristas(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar motoristas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarTracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridTracao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar motoristas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarReboques()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridReboques(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar motoristas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoMotorista = Request.GetIntParam("Codigo");
                int codigoCentroResultado = Request.GetIntParam("CodigoCentroResultado");

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repositorioCentroResultado.BuscarPorCodigo(codigoCentroResultado);

                if (motorista == null || centroResultado == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                if (motorista.CentroResultado != null && centroResultado.Codigo == motorista.CentroResultado.Codigo)
                    return new JsonpResult(false, true, "O Motorista já está vinculado a este Centro de Resultado.");

                unitOfWork.Start();

                motorista.CentroResultado = centroResultado;
                repMotorista.Atualizar(motorista);

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoMotorista(unitOfWork, motorista, this.Usuario.Codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarTracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("Codigo");
                int codigoCentroResultado = Request.GetIntParam("CodigoCentroResultado");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repositorioCentroResultado.BuscarPorCodigo(codigoCentroResultado);

                if (veiculo == null || centroResultado == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                if (veiculo.CentroResultado != null && centroResultado.Codigo == veiculo.CentroResultado.Codigo)
                    return new JsonpResult(false, true, "O Veículo já está vinculado a este Centro de Resultado.");

                unitOfWork.Start();

                veiculo.CentroResultado = centroResultado;
                repVeiculo.Atualizar(veiculo);

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoVeiculo(unitOfWork, veiculo, this.Usuario.Codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarReboque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("Codigo");
                int codigoCentroResultado = Request.GetIntParam("CodigoCentroResultado");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repositorioCentroResultado.BuscarPorCodigo(codigoCentroResultado);

                if (veiculo == null || centroResultado == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                if (veiculo.CentroResultado != null && centroResultado.Codigo == veiculo.CentroResultado.Codigo)
                    return new JsonpResult(false, true, "O Veículo já está vinculado a este Centro de Resultado.");

                unitOfWork.Start();

                veiculo.CentroResultado = centroResultado;
                repVeiculo.Atualizar(veiculo);

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoVeiculo(unitOfWork, veiculo, this.Usuario.Codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirMotoristaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoMotorista = Request.GetIntParam("Codigo");

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                if (motorista == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                motorista.CentroResultado = null;
                repMotorista.Atualizar(motorista);

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoMotorista(unitOfWork, motorista, this.Usuario.Codigo);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirTracaoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("Codigo");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                unitOfWork.Start();

                veiculo.CentroResultado = null;
                repVeiculo.Atualizar(veiculo);

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoVeiculo(unitOfWork, veiculo, this.Usuario.Codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirReboquePorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("Codigo");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                unitOfWork.Start();

                veiculo.CentroResultado = null;
                repVeiculo.Atualizar(veiculo);

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoVinculoVeiculo(unitOfWork, veiculo, this.Usuario.Codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarMotoristas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridMotoristas(unitOfWork);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarReboques()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridReboques(unitOfWork);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarTracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridTracao(unitOfWork);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        #endregion

        #region Metódos Privados

        private void RegistrarHistoricoVinculoVeiculo(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
            {
                Veiculo = veiculo,
                DataHora = DateTime.Now,
                Usuario = Usuario,
                KmRodado = veiculo.KilometragemAtual,
                KmAtualModificacao = 0,
                DiasVinculado = 0
            };
            repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);

            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
            {
                HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                CentroResultado = veiculo.CentroResultado,
                DataHora = DateTime.Now,
            };

            repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
        }

        private Models.Grid.Grid ObterGridMotoristas(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroResultado = Request.GetIntParam("Codigo");

            Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CPF", "CPF", 20, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Nome", "Nome", 40, Models.Grid.Align.left);

            int totalRegistros = repositorioCentroResultado.ContarMotoristasPorCentroResultado(codigoCentroResultado);
            List<Dominio.Entidades.Usuario> motoristas = totalRegistros > 0 ? repositorioCentroResultado.BuscarMotoristasPorCentroResultado(codigoCentroResultado) : new List<Dominio.Entidades.Usuario>();

            grid.AdicionaRows((
                from o in motoristas
                select new
                {
                    o.Codigo,
                    CPF = o.CPF_Formatado,
                    Nome = o.Nome,
                }).ToList()
            );

            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Models.Grid.Grid ObterGridTracao(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroResultado = Request.GetIntParam("Codigo");

            Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Nº Frota", "NumeroFrota", 40, Models.Grid.Align.left);

            int totalRegistros = repositorioCentroResultado.ContarTracaoPorCentroResultado(codigoCentroResultado);
            List<Dominio.Entidades.Veiculo> tracoes = totalRegistros > 0 ? repositorioCentroResultado.BuscarTracaoPorCentroResultado(codigoCentroResultado) : new List<Dominio.Entidades.Veiculo>();

            grid.AdicionaRows((
                from o in tracoes
                select new
                {
                    o.Codigo,
                    Placa = o.Placa_Formatada,
                    NumeroFrota = o.NumeroFrota ?? string.Empty,
                }).ToList()
            );

            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Models.Grid.Grid ObterGridReboques(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroResultado = Request.GetIntParam("Codigo");

            Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Nº Frota", "NumeroFrota", 40, Models.Grid.Align.left);

            int totalRegistros = repositorioCentroResultado.ContarReboquePorCentroResultado(codigoCentroResultado);
            List<Dominio.Entidades.Veiculo> reboques = totalRegistros > 0 ? repositorioCentroResultado.BuscarReboquePorCentroResultado(codigoCentroResultado) : new List<Dominio.Entidades.Veiculo>();

            grid.AdicionaRows((
                from o in reboques
                select new
                {
                    o.Codigo,
                    Placa = o.Placa_Formatada,
                    NumeroFrota = o.NumeroFrota ?? string.Empty,
                }).ToList()
            );

            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        #endregion
    }
}
