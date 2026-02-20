using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/RestricaoRodagem")]
    public class RestricaoRodagemController : BaseController
    {
		#region Construtores

		public RestricaoRodagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricaoRodagem = new Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem();

                PreencherRestricaoRodagem(restricaoRodagem, unitOfWork);

                unitOfWork.Start();

                AdicionarOuAtualizarClientesDestino(restricaoRodagem, unitOfWork);

                new Repositorio.Embarcador.Logistica.RestricaoRodagem(unitOfWork).Inserir(restricaoRodagem, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.RestricaoRodagem repositorio = new Repositorio.Embarcador.Logistica.RestricaoRodagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricaoRodagem = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (restricaoRodagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRestricaoRodagem(restricaoRodagem, unitOfWork);

                unitOfWork.Start();

                AdicionarOuAtualizarClientesDestino(restricaoRodagem, unitOfWork);

                repositorio.Atualizar(restricaoRodagem, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Logistica.RestricaoRodagem repositorio = new Repositorio.Embarcador.Logistica.RestricaoRodagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricaoRodagem = repositorio.BuscarPorCodigo(codigo);

                if (restricaoRodagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    Dados = new
                    {
                        restricaoRodagem.Codigo,
                        CentroCarregamento = new { restricaoRodagem.CentroCarregamento.Codigo, restricaoRodagem.CentroCarregamento.Descricao },
                        restricaoRodagem.DiaSemana,
                        restricaoRodagem.FinalPlaca,
                        HoraFinal = restricaoRodagem.HoraFinal.ToString("hh':'mm"),
                        HoraInicial = restricaoRodagem.HoraInicial.ToString("hh':'mm")
                    },
                    ClientesDestino = (
                        from clienteDestino in restricaoRodagem.ClientesDestino
                        select new
                        {
                            clienteDestino.Codigo,
                            clienteDestino.Descricao
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.RestricaoRodagem repositorio = new Repositorio.Embarcador.Logistica.RestricaoRodagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricaoRodagem = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (restricaoRodagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(restricaoRodagem, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();
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
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void AdicionarOuAtualizarClientesDestino(Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricaoRodagem, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic clientesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ClientesDestino"));

            ExcluirClientesDestinoRemovidos(restricaoRodagem, clientesDestino, unitOfWork);
            InserirClientesDestinoAdicionados(restricaoRodagem, clientesDestino, unitOfWork);
        }

        private void ExcluirClientesDestinoRemovidos(Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricaoRodagem, dynamic clientesDestino, Repositorio.UnitOfWork unitOfWork)
        {
            if (restricaoRodagem.ClientesDestino?.Count > 0)
            {
                List<double> listaCpfCnpjAtualizados = new List<double>();

                foreach (var clienteDestino in clientesDestino)
                    listaCpfCnpjAtualizados.Add(((string)clienteDestino.Codigo).ToDouble());

                List<Dominio.Entidades.Cliente> listaClienteDestinoRemover = (from clienteDestino in restricaoRodagem.ClientesDestino where !listaCpfCnpjAtualizados.Contains(clienteDestino.CPF_CNPJ) select clienteDestino).ToList();

                foreach (var clienteDestino in listaClienteDestinoRemover)
                    restricaoRodagem.ClientesDestino.Remove(clienteDestino);

                if (listaClienteDestinoRemover.Count > 0)
                {
                    string descricaoAcao = listaClienteDestinoRemover.Count == 1 ? "Cliente de destino removido" : "Múltiplos clientes de destino removidos";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, restricaoRodagem, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherRestricaoRodagem(Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricaoRodagem, Repositorio.UnitOfWork unitOfWork)
        {
            int finalPlaca = Request.GetIntParam("FinalPlaca");

            if ((finalPlaca < 0) || (finalPlaca > 9))
                throw new ControllerException("Final da placa deve estar entre os números 0 e 9.");

            restricaoRodagem.CentroCarregamento = ObterCentroCarregamento(unitOfWork);
            restricaoRodagem.DiaSemana = Request.GetNullableEnumParam<DiaSemana>("DiaSemana") ?? throw new ControllerException("Dia da semana não informado.");
            restricaoRodagem.FinalPlaca = finalPlaca;
            restricaoRodagem.HoraInicial = Request.GetNullableTimeParam("HoraInicial") ?? throw new ControllerException("Hora de inicial não informada.");
            restricaoRodagem.HoraFinal = Request.GetNullableTimeParam("HoraFinal") ?? throw new ControllerException("Hora final não informada.");
        }

        private void InserirClientesDestinoAdicionados(Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricaoRodagem, dynamic clientesDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            int totalClientesDestinoAdicionados = 0;

            if (restricaoRodagem.ClientesDestino == null)
                restricaoRodagem.ClientesDestino = new List<Dominio.Entidades.Cliente>();

            foreach (var clienteDestino in clientesDestino)
            {
                double cpfCnpj = ((string)clienteDestino.Codigo).ToDouble();
                Dominio.Entidades.Cliente clienteDestinoAdicionar = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpj) ?? throw new ControllerException("Cliente de destino não encontrado");

                if (!restricaoRodagem.ClientesDestino.Contains(clienteDestinoAdicionar))
                {
                    restricaoRodagem.ClientesDestino.Add(clienteDestinoAdicionar);

                    totalClientesDestinoAdicionados++;
                }
            }

            if (restricaoRodagem.IsInitialized() && (totalClientesDestinoAdicionados > 0))
            {
                string descricaoAcao = totalClientesDestinoAdicionados == 1 ? "Cliente de destino adicionado" : "Múltiplos clientes de destino adicionados";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, restricaoRodagem, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

            return repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento) ?? throw new ControllerException("Centro de carregamento não encontrado.");
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                DiaSemana = Request.GetNullableEnumParam<DiaSemana>("DiaSemana")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Centro de Carregamento", "CentroCarregamento", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Dia da Semana", "DiaSemana", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Final da Placa", "FinalPlaca", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Início Restrição", "HoraInicial", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Fim Restrição", "HoraFinal", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.RestricaoRodagem repositorio = new Repositorio.Embarcador.Logistica.RestricaoRodagem(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem> listaRestricaoRodagem = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem>();
                
                var listaRestricaoRodagemRetornar = (
                    from restricao in listaRestricaoRodagem
                    select new
                    {
                        restricao.Codigo,
                        CentroCarregamento = restricao.CentroCarregamento.Descricao,
                        DiaSemana = restricao.DiaSemana.ObterDescricao(),
                        restricao.FinalPlaca,
                        HoraFinal = restricao.HoraFinal.ToString("hh':'mm"),
                        HoraInicial = restricao.HoraInicial.ToString("hh':'mm")
                    }
                ).ToList();

                grid.AdicionaRows(listaRestricaoRodagemRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CentroCarregamento")
                return "CentroCarregamento.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
