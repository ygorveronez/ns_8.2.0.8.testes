using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.CargaRelacionada
{
    [CustomAuthorize("Cargas/CargaRelacionada")]
    public class CargaRelacionadaController : BaseController
    {
		#region Construtores

		public CargaRelacionadaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int totalRegistros;

                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork, out totalRegistros);

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

        public async Task<IActionResult> BuscarJustificativaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                var dynResult = new
                {
                    Codigo = carga?.Codigo ?? codigoCarga,
                    Justificativa = carga?.JustificativaCargaRelacionada ?? string.Empty
                };

                return new JsonpResult(dynResult);
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

        public async Task<IActionResult> AdicionarJustificativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);

                if (carga == null)
                    throw new Exception("Registro não encontrado");

                carga.JustificativaCargaRelacionada = Request.GetStringParam("Justificativa");

                repCarga.Atualizar(carga, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(false, true, "Justificativa registrada");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverJustificativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);

                if (carga == null)
                    throw new Exception("Registro não encontrado");

                carga.JustificativaCargaRelacionada = null;

                repCarga.Atualizar(carga, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(false, true, "Justificativa Removida");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCargaRelacionadaCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRelacionada repCargaRelacionada = new Repositorio.Embarcador.Cargas.CargaRelacionada(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaRelacionada cargaRelacionada = repCargaRelacionada.BuscarRelacionadaPorCodigoCarga(codigoCarga);

                bool possuiRelacionada = cargaRelacionada != null && cargaRelacionada.CargaRelacao.Codigo > 0;

                var dynResult = new
                {
                    Codigo = cargaRelacionada?.Carga?.Codigo ?? codigoCarga,
                    CargaRelacionada = new { Codigo = cargaRelacionada?.CargaRelacao?.Codigo ?? 0, Descricao = cargaRelacionada?.CargaRelacao?.Descricao ?? string.Empty },
                    PossuiRelacionada = possuiRelacionada
                };

                return new JsonpResult(dynResult);
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

        public async Task<IActionResult> AdicionarCargaRelacionada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Codigo");
                int codigoCargaRelacionada = Request.GetIntParam("CargaRelacionada");
                bool possuiRelacionada = Request.GetBoolParam("PossuiRelacionada");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRelacionada repCargaRelacionada = new Repositorio.Embarcador.Cargas.CargaRelacionada(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga novaCargaRelacionada = repCarga.BuscarPorCodigo(codigoCargaRelacionada);
                Dominio.Entidades.Embarcador.Cargas.CargaRelacionada entidadeCargaRelacionada = repCargaRelacionada.BuscarPorCodigo(codigoCarga, false);

                if (carga == null)
                    throw new Exception("Carga não encontrada");

                if (novaCargaRelacionada == null)
                    throw new Exception("Carga relacionada não encontrada");

                if (CargaJaRelacionadaEmOutroRegistro(novaCargaRelacionada.Codigo, unitOfWork))
                    throw new Exception("A Carga selecionada já está relacionada a outro registro!");

                if (entidadeCargaRelacionada != null && possuiRelacionada)
                {
                    entidadeCargaRelacionada.CargaRelacao = novaCargaRelacionada;
                    repCargaRelacionada.Atualizar(entidadeCargaRelacionada);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Atualizado vínculo com a carga relacionada {novaCargaRelacionada.CodigoCargaEmbarcador}", unitOfWork);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaRelacionada novaRelacionada = new Dominio.Entidades.Embarcador.Cargas.CargaRelacionada
                    {
                        Carga = carga,
                        CargaRelacao = novaCargaRelacionada,
                    };

                    repCargaRelacionada.Inserir(novaRelacionada);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Adicionou vínculo com a carga relacionada {novaCargaRelacionada.CodigoCargaEmbarcador}", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(false, true, "Relacionamento registrado");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverCargaRelacionada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRelacionada repCargaRelacionada = new Repositorio.Embarcador.Cargas.CargaRelacionada(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaRelacionada cargaRelacionada = repCargaRelacionada.BuscarRelacionadaPorCodigoCarga(codigoCarga);

                if (cargaRelacionada == null)
                    throw new Exception("Registro não encontrado");

                cargaRelacionada.Initialize();

                repCargaRelacionada.Deletar(cargaRelacionada, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(false, true, "Justificativa Removida");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int totalRegistros;

                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork, out totalRegistros);
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

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, out int totalRegistros)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRelacionada repositorioCargaRelacionada = new Repositorio.Embarcador.Cargas.CargaRelacionada(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaRelacionada filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 8, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Data de Criação", "DataCriacaoCarga", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Situação", "SituacaoCarga", 15, Models.Grid.Align.center);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 20, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoDeCarga", 20, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Canal de Entrega", "CanalEntrega", 20, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Carga Relacionada", "CargaRelacionada", 10, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Justificativa", "Justificativa", 40, Models.Grid.Align.left);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.OcultarCabecalho("Transportador");


            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            totalRegistros = repositorioCargaRelacionada.ContarConsulta(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = totalRegistros > 0 ? repositorioCargaRelacionada.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            grid.AdicionaRows((
                from o in listaCarga
                select new
                {
                    o.Codigo,
                    NumeroCarga = o.CodigoCargaEmbarcador,
                    DataCriacaoCarga = o.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm"),
                    SituacaoCarga = o.SituacaoCarga.ObterDescricao(),
                    Filial = o.Filial?.Descricao ?? string.Empty,
                    TipoOperacao = o.TipoOperacao?.Descricao ?? string.Empty,
                    TipoDeCarga = o.TipoDeCarga?.Descricao ?? string.Empty,
                    CanalEntrega = string.Join(", ", repPedidoStage.BuscarPorListaPedidos(o.Pedidos.Select(p => p.Pedido?.Codigo ?? 0).ToList()).Select(s => s?.CanalEntrega?.Descricao)) ?? string.Empty,
                    Transportador = o.Empresa?.RazaoSocial ?? string.Empty,
                    CargaRelacionada = repositorioCargaRelacionada.BuscarDescricaoRelacionadaPorCodigo(o.Codigo),
                    Justificativa = o.JustificativaCargaRelacionada ?? string.Empty
                }).ToList()
            );

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaRelacionada ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaRelacionada
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTipoDeCarga = Request.GetIntParam("TipoDeCarga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoCanalEntrega = Request.GetIntParam("CanalEntrega"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa?.Codigo ?? 0 : Request.GetIntParam("Transportador"),
                Situacao = Request.GetNullableEnumParam<SituacaoCarga>("Situacao"),
                Relacionada = Request.GetNullableEnumParam<RelacionamentoCarga>("Relacionada")
            };
        }

        private bool CargaJaRelacionadaEmOutroRegistro(int codigoCargaRelacionada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRelacionada repCargaRelacionada = new Repositorio.Embarcador.Cargas.CargaRelacionada(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRelacionada cargaRelacionada = repCargaRelacionada.BuscarRegistrosRelacionadosPorCodigoCarga(codigoCargaRelacionada);
            if (cargaRelacionada != null)
                return true;

            return false;
        }
    }
}
