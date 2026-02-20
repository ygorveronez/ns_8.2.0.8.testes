using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Cargas.Retornos
{
    [CustomAuthorize("Cargas/RetornoCarga")]
    public class RetornoCargaController : BaseController
    {
		#region Construtores

		public RetornoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioRetornoCarga.BuscarPorCodigo(codigo);

                if (retornoCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.RetornoCarga.NaoFoiPossivelEncontrarORegistro);

                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                List<Dominio.Entidades.Cliente> destinatarios = repositorioCliente.BuscarDestinatariosCarga(retornoCarga.Carga.Codigo);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaEntregaCarga = repositorioCargaPedido.BuscarUltimaEntregaCarga(retornoCarga.Carga.Codigo);
                bool utilizarRecebedorUltimaEntrega = (ultimaEntregaCarga.Recebedor != null && !(ultimaEntregaCarga.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false));
                Dominio.Entidades.Cliente ultimoClienteDestino = utilizarRecebedorUltimaEntrega ? ultimaEntregaCarga.Recebedor : ultimaEntregaCarga.Pedido.Destinatario;
                Repositorio.Embarcador.Pessoas.ClienteDescarga repositorioClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repositorioClienteDescarga.BuscarPorPessoa(ultimoClienteDestino.CPF_CNPJ);
                Dominio.Entidades.Veiculo reboque = null;
                Dominio.Entidades.Veiculo segundoReboque = null;

                if (!(clienteDescarga?.DeixarReboqueParaDescarga ?? false) && (retornoCarga.Reboques.Count() > 0))
                {
                    reboque = retornoCarga.Reboques.ElementAt(0);

                    if (retornoCarga.Reboques.Count() > 1)
                        segundoReboque = retornoCarga.Reboques.ElementAt(1);
                }

                var retornoCargaRetornar = new
                {
                    retornoCarga.Codigo,
                    Situacao = retornoCarga.SituacaoRetornoCarga,
                    ClienteColeta = new { Codigo = retornoCarga.ClienteColeta?.Codigo ?? 0, Descricao = retornoCarga.ClienteColeta?.Descricao ?? "" },
                    TipoRetornoCarga = new { Codigo = retornoCarga.TipoRetornoCarga?.Codigo ?? 0, Descricao = retornoCarga.TipoRetornoCarga?.Descricao ?? "" },
                    PontosDeColeta = retornoCarga.OrigemRetorno?.Codigo ?? 0,
                    ExigeClienteColeta = retornoCarga.TipoRetornoCarga?.ExigeClienteColeta ?? false,
                    DescricaoSituacao = retornoCarga.SituacaoRetornoCarga.ObterDescricao(),
                    Carga = retornoCarga.Carga.CodigoCargaEmbarcador,
                    Filial = retornoCarga.Carga.Filial?.Descricao ?? "",
                    Origem = retornoCarga.Carga.DadosSumarizados.Origens,
                    Destino = retornoCarga.Carga.DadosSumarizados.Destinos,
                    Remetente = retornoCarga.Carga.DadosSumarizados.RemetentesReais,
                    Empresa = new { retornoCarga.Carga.Empresa.Codigo, retornoCarga.Carga.Empresa.Descricao },
                    Destinatarios = retornoCarga.Carga.DadosSumarizados.DestinatariosReais,
                    Destinatario = new { Codigo = ultimoClienteDestino.CPF_CNPJ, ultimoClienteDestino.Descricao },
                    retornoCarga.RetornarSomenteComTracao,
                    NumeroReboques = retornoCarga.Carga.ModeloVeicularCarga?.NumeroReboques ?? 0,
                    PrevisaoChegada = retornoCarga?.DataPrevisaoEntrega ?? null,
                    Reboque = new { Codigo = reboque?.Codigo ?? 0, Descricao = reboque?.Placa ?? "" },
                    SegundoReboque = new { Codigo = segundoReboque?.Codigo ?? 0, Descricao = segundoReboque?.Placa ?? "" },
                    TodosDestinatarios = (from o in destinatarios
                                          where o.CPF_CNPJ != ultimoClienteDestino.CPF_CNPJ
                                          select new
                                          {
                                              Codigo = o.CPF_CNPJ,
                                              o.Descricao
                                          }).ToList()
                };

                return new JsonpResult(retornoCargaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.RetornoCarga.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioRetornoCarga.BuscarPorCodigo(codigo, true);

                if (retornoCarga.SituacaoRetornoCarga != SituacaoRetornoCarga.AgInformarRetorno)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.RetornoCarga.NaoFoiPossivelCancelarRetornoCargaAtual, retornoCarga.SituacaoRetornoCarga.ObterDescricao()));

                unitOfWork.Start();

                retornoCarga.SituacaoRetornoCarga = SituacaoRetornoCarga.CanceladoRetorno;

                repositorioRetornoCarga.Atualizar(retornoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.RetornoCarga.OcorreuUmaFalhaAoCancelarRetorno);
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
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Cargas.RetornoCarga.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.RetornoCarga.OcorreuUmaFalhaAoExportar);
            }
        }

        public async Task<IActionResult> GerarCargaRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                bool retornoSomenteComTracao = Request.GetBoolParam("RetornarSomenteComTracao");
                int codigoTipoRetornoCarga = Request.GetIntParam("TipoRetornoCarga");
                double codigoPontosColeta = Request.GetDoubleParam("PontosDeColeta");
                int codigoReboque = Request.GetIntParam("Reboque");
                int codigoSegundoReboque = Request.GetIntParam("SegundoReboque");
                double codigoClienteColeta = Request.GetDoubleParam("ClienteColeta");

                Servicos.Embarcador.Carga.Retornos.RetornoCarga serRetornos = new Servicos.Embarcador.Carga.Retornos.RetornoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repositorioRetornoCarga.BuscarPorCodigo(codigo);
                if (retornoCarga == null)
                    throw new ServicoException(Localization.Resources.Cargas.RetornoCarga.NaoFoiPossivelEncontrarORegistro);

                serRetornos.SolicitarGeracaoCargaRetorno(retornoCarga, codigoTipoRetornoCarga, codigoPontosColeta, codigoReboque, codigoSegundoReboque, codigoClienteColeta, retornoSomenteComTracao, Auditado);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.RetornoCarga.OcorreuUmaFalhaAoGerarCargaRetorno);
            }
            finally
            {
                unitOfWork.Dispose();
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
                return new JsonpResult(false, Localization.Resources.Cargas.RetornoCarga.OcorreuUmaFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados 

        private Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCarga()
            {
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = Request.GetIntParam("Empresa"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                Situacao = Request.GetEnumParam("Situacao", SituacaoRetornoCarga.Todas)
            };
        }

        public Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.Carga, "Carga", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.DataCarga, "DataCriacaoCarga", 10, Models.Grid.Align.center, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.Filial, "Filial", 20, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.Origem, "Origem", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.Destino, "Destino", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.Transportador, "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.Motorista, "Motorista", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.RetornoCarga.Situacao, "DescricaoSituacao", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCarga filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(unitOfWork);
                int totalRegistros = repositorioRetornoCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga> retornosCarga = totalRegistros > 0 ? repositorioRetornoCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga>();

                var retornosCargaRetornar = (
                    from retornoCarga in retornosCarga
                    select new
                    {
                        retornoCarga.Codigo,
                        Carga = retornoCarga.Carga.CodigoCargaEmbarcador,
                        retornoCarga.Carga.DataCriacaoCarga,
                        Filial = retornoCarga.Carga.Filial?.Descricao ?? "",
                        Origem = retornoCarga.Carga.DadosSumarizados.Origens,
                        Destino = retornoCarga.Carga.DadosSumarizados.Destinos,
                        Empresa = retornoCarga.Carga.Empresa.Descricao,
                        Motorista = retornoCarga.Carga.NomeMotoristas,
                        Veiculo = retornoCarga.Carga.PlacasVeiculos,
                        DescricaoSituacao = retornoCarga.SituacaoRetornoCarga.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(retornosCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar != "DescricaoSituacao")
                return $"Carga.{propriedadeOrdenar}";

            if (propriedadeOrdenar == "Empresa")
                return "Empresa.RazaoSocial";

            if (propriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "SituacaoRetornoCarga";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
