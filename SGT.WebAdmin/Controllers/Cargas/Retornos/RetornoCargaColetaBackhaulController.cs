using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Cargas.Retornos
{
    [CustomAuthorize("Cargas/RetornoCargaColetaBackhaul")]
    public class RetornoCargaColetaBackhaulController : BaseController
    {
		#region Construtores

		public RetornoCargaColetaBackhaulController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul repositorioRetornoCargaColetaBackhaul = new Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul retornoCargaColetaBackhaul = repositorioRetornoCargaColetaBackhaul.BuscarPorCodigo(codigo);

                if (retornoCargaColetaBackhaul == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = retornoCargaColetaBackhaul.RetornoCarga;
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaEntregaCarga = repositorioCargaPedido.BuscarUltimaEntregaCarga(retornoCarga.Carga.Codigo);
                bool utilizarRecebedorUltimaEntrega = (ultimaEntregaCarga.Recebedor != null && !(ultimaEntregaCarga.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false));
                Dominio.Entidades.Cliente ultimoClienteDestino = utilizarRecebedorUltimaEntrega ? ultimaEntregaCarga.Recebedor : ultimaEntregaCarga.Pedido.Destinatario;

                var retornoCargaColetaBackhaulRetornar = new
                {
                    retornoCargaColetaBackhaul.Codigo,
                    retornoCargaColetaBackhaul.Situacao,
                    DescricaoSituacao = retornoCargaColetaBackhaul.Situacao.ObterDescricao(),
                    Carga = retornoCarga.Carga.CodigoCargaEmbarcador,
                    Filial = retornoCarga.Carga.Filial.Descricao,
                    Origem = retornoCarga.Carga.DadosSumarizados.Origens,
                    Destino = retornoCarga.Carga.DadosSumarizados.Destinos,
                    Remetente = retornoCarga.Carga.DadosSumarizados.RemetentesReais,
                    Empresa = retornoCarga.Carga.Empresa.Descricao,
                    Destinatarios = retornoCarga.Carga.DadosSumarizados.DestinatariosReais,
                    Destinatario = ultimoClienteDestino.Descricao,
                    MotivoCancelamento = retornoCargaColetaBackhaul.MotivoCancelamento?.Descricao ?? ""
                };

                return new JsonpResult(retornoCargaColetaBackhaulRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarCargaRetornoColetaBackhaul()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul repositorioRetornoCargaColetaBackhaul = new Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul retornoCargaColetaBackhaul = repositorioRetornoCargaColetaBackhaul.BuscarPorCodigo(codigo, true);

                if (retornoCargaColetaBackhaul == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if (retornoCargaColetaBackhaul.Situacao != SituacaoRetornoCargaColetaBackhaul.AguardandoGerarCarga)
                    return new JsonpResult(false, true, $"Não é possível gerar a carga de retorno de coleta backhaul na atual situação ({retornoCargaColetaBackhaul.Situacao.ObterDescricao()}).");

                int codigoMotivoCancelamento = Request.GetIntParam("MotivoCancelamento");
                Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul repositorioMotivoCancelamentoRetornoCargaColetaBackhaul = new Repositorio.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.MotivoCancelamentoRetornoCargaColetaBackhaul motivoCancelamento = repositorioMotivoCancelamentoRetornoCargaColetaBackhaul.BuscarPorCodigo(codigoMotivoCancelamento);

                if (motivoCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o motivo de cancelamento");

                unitOfWork.Start();

                retornoCargaColetaBackhaul.MotivoCancelamento = motivoCancelamento;

                if (motivoCancelamento.GerarCargaColeta)
                    retornoCargaColetaBackhaul.Situacao = SituacaoRetornoCargaColetaBackhaul.GerandoCarga;
                else
                {
                    retornoCargaColetaBackhaul.Situacao = SituacaoRetornoCargaColetaBackhaul.RetornoCancelado;

                    Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(unitOfWork);

                    retornoCargaColetaBackhaul.RetornoCarga.Initialize();
                    retornoCargaColetaBackhaul.RetornoCarga.SituacaoRetornoCarga = SituacaoRetornoCarga.RetornoGerado;

                    repositorioRetornoCarga.Atualizar(retornoCargaColetaBackhaul.RetornoCarga, Auditado);
                }

                repositorioRetornoCargaColetaBackhaul.Atualizar(retornoCargaColetaBackhaul, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a carga de retorno de coleta backhaul.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarCargaRetornoColetaBackhaul()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul repositorioRetornoCargaColetaBackhaul = new Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul retornoCargaColetaBackhaul = repositorioRetornoCargaColetaBackhaul.BuscarPorCodigo(codigo, true);

                if (retornoCargaColetaBackhaul == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if (retornoCargaColetaBackhaul.Situacao != SituacaoRetornoCargaColetaBackhaul.AguardandoGerarCarga)
                    return new JsonpResult(false, true, $"Não é possível gerar a carga de retorno de coleta backhaul na atual situação ({retornoCargaColetaBackhaul.Situacao.ObterDescricao()}).");

                unitOfWork.Start();

                retornoCargaColetaBackhaul.Situacao = SituacaoRetornoCargaColetaBackhaul.GerandoCarga;

                repositorioRetornoCargaColetaBackhaul.Atualizar(retornoCargaColetaBackhaul, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a carga de retorno de coleta backhaul.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data da Carga", "DataCriacaoCarga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veiculo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCargaColetaBackhaul filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul repositorioRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul(unitOfWork);
                int totalRegistros = repositorioRetornoCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul> retornosCargaColetaBackhaul = totalRegistros > 0 ? repositorioRetornoCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul>();

                var retornosCargaColetaBackhaulRetornar = (
                    from retornoCargaColetaBackhaul in retornosCargaColetaBackhaul
                    select new
                    {
                        retornoCargaColetaBackhaul.Codigo,
                        Carga = retornoCargaColetaBackhaul.RetornoCarga.Carga.CodigoCargaEmbarcador,
                        retornoCargaColetaBackhaul.RetornoCarga.Carga.DataCriacaoCarga,
                        Filial = retornoCargaColetaBackhaul.RetornoCarga.Carga.Filial.Descricao,
                        Origem = retornoCargaColetaBackhaul.RetornoCarga.Carga.DadosSumarizados.Origens,
                        Destino = retornoCargaColetaBackhaul.RetornoCarga.Carga.DadosSumarizados.Destinos,
                        Empresa = retornoCargaColetaBackhaul.RetornoCarga.Carga.Empresa.Descricao,
                        Motorista = retornoCargaColetaBackhaul.RetornoCarga.Carga.NomeMotoristas,
                        Veiculo = retornoCargaColetaBackhaul.RetornoCarga.Carga.PlacasVeiculos,
                        DescricaoSituacao = retornoCargaColetaBackhaul.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(retornosCargaColetaBackhaulRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        #endregion

        #region Métodos Privados 

        private Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCargaColetaBackhaul ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.Retornos.FiltroPesquisaRetornoCargaColetaBackhaul()
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
                Situacao = Request.GetNullableEnumParam<SituacaoRetornoCargaColetaBackhaul>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar != "DescricaoSituacao")
                return $"RetornoCarga.Carga.{propriedadeOrdenar}";

            if (propriedadeOrdenar == "Empresa")
                return "Empresa.RazaoSocial";

            if (propriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "Situacao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
