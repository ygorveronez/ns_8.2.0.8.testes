using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/BandaRodagemPneu")]
    public class BandaRodagemPneuController : BaseController
    {
		#region Construtores

		public BandaRodagemPneuController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu bandaRodagemPneu = new Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu();

                try
                {
                    PreencherBandaRodagemPneu(bandaRodagemPneu, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frota.BandaRodagemPneu repositorio = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unitOfWork);

                repositorio.Inserir(bandaRodagemPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                Repositorio.Embarcador.Frota.BandaRodagemPneu repositorio = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu bandaRodagemPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (bandaRodagemPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherBandaRodagemPneu(bandaRodagemPneu, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(bandaRodagemPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                Repositorio.Embarcador.Frota.BandaRodagemPneu repositorio = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu bandaRodagemPneu = repositorio.BuscarPorCodigo(codigo);

                if (bandaRodagemPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    bandaRodagemPneu.Codigo,
                    bandaRodagemPneu.Descricao,
                    Marca = new { bandaRodagemPneu.Marca.Codigo, bandaRodagemPneu.Marca.Descricao },
                    Status = bandaRodagemPneu.Ativo,
                    bandaRodagemPneu.Tipo
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
                Repositorio.Embarcador.Frota.BandaRodagemPneu repositorio = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu bandaRodagemPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (bandaRodagemPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(bandaRodagemPneu, Auditado);

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

        private Dominio.Entidades.Embarcador.Frota.MarcaPneu ObterMarcaPneu(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoMarca = Request.GetIntParam("Marca");
            Repositorio.Embarcador.Frota.MarcaPneu repositorio = new Repositorio.Embarcador.Frota.MarcaPneu(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoMarca) ?? throw new ControllerException("Marca não encontrada");
        }

        private void PreencherBandaRodagemPneu(Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu bandaRodagemPneu, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.GetStringParam("Descricao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            bandaRodagemPneu.Descricao = descricao;
            bandaRodagemPneu.Ativo = Request.GetBoolParam("Status");
            bandaRodagemPneu.Marca = ObterMarcaPneu(unitOfWork);
            bandaRodagemPneu.Tipo = Request.GetEnumParam<TipoBandaRodagemPneu>("Tipo");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                bandaRodagemPneu.Empresa = this.Usuario.Empresa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBandaRodagemPneu filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBandaRodagemPneu()
                {
                    CodigoMarca = Request.GetIntParam("Marca"),
                    Descricao = Request.GetStringParam("Descricao"),
                    SituacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
                    Tipo = Request.GetNullableEnumParam<TipoBandaRodagemPneu>("Tipo")
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Código", "Codigo", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Marca", "Marca", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 25, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 12, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.BandaRodagemPneu repositorio = new Repositorio.Embarcador.Frota.BandaRodagemPneu(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu> listaBandaRodagemPneu = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                var listaBandaRodagemPneuRetornar = (
                    from bandaRodagemPneu in listaBandaRodagemPneu
                    select new
                    {
                        bandaRodagemPneu.Codigo,
                        bandaRodagemPneu.Descricao,
                        bandaRodagemPneu.DescricaoAtivo,
                        Marca = bandaRodagemPneu.Marca.Descricao,
                        Tipo = bandaRodagemPneu.Tipo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaBandaRodagemPneuRetornar);
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
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            if (propriedadeOrdenar == "Marca")
                return "Marca.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
