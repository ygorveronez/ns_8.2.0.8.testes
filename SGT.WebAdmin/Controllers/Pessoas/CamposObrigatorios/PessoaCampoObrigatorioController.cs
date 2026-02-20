using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas.CamposObrigatorios
{
    [CustomAuthorize("Pessoas/PessoaCampoObrigatorio")]
    public class PessoaCampoObrigatorioController : BaseController
    {
		#region Construtores

		public PessoaCampoObrigatorioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio repPessoaCampoObrigatorio = new Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio pessoaCampoObrigatorio = new Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio();

                PreencherEntidade(pessoaCampoObrigatorio, unitOfWork);

                unitOfWork.Start();

                repPessoaCampoObrigatorio.Inserir(pessoaCampoObrigatorio, Auditado);

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

                Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio repPessoaCampoObrigatorio = new Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio pessoaCampoObrigatorio = repPessoaCampoObrigatorio.BuscarPorCodigo(codigo, true);

                if (pessoaCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(pessoaCampoObrigatorio, unitOfWork);

                unitOfWork.Start();

                repPessoaCampoObrigatorio.Atualizar(pessoaCampoObrigatorio, Auditado);

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

                Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio repPessoaCampoObrigatorio = new Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio pessoaCampoObrigatorio = repPessoaCampoObrigatorio.BuscarPorCodigo(codigo, false);

                if (pessoaCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    pessoaCampoObrigatorio.Codigo,
                    Situacao = pessoaCampoObrigatorio.Ativo,
                    pessoaCampoObrigatorio.Cliente,
                    pessoaCampoObrigatorio.Fornecedor,
                    pessoaCampoObrigatorio.Terceiro,
                    Campos = pessoaCampoObrigatorio.Campos.Select(o => new
                    {
                        o.Codigo,
                        o.Descricao
                    }).ToList()
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

                Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio repPessoaCampoObrigatorio = new Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio pessoaCampoObrigatorio = repPessoaCampoObrigatorio.BuscarPorCodigo(codigo, true);

                if (pessoaCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                pessoaCampoObrigatorio.Campos = null;

                repPessoaCampoObrigatorio.Deletar(pessoaCampoObrigatorio, Auditado);

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
                Models.Grid.Grid grid = ObterGridPesquisa();

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarParaPessoa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool cliente = Request.GetBoolParam("Cliente");
                bool fornecedor = Request.GetBoolParam("Fornecedor");
                bool terceiro = Request.GetBoolParam("Terceiro");

                Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio repPessoaCampoObrigatorio = new Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio pessoaCampoObrigatorio = repPessoaCampoObrigatorio.BuscarParaPessoa(cliente, fornecedor, terceiro);

                if (pessoaCampoObrigatorio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    pessoaCampoObrigatorio.Codigo,
                    pessoaCampoObrigatorio.Cliente,
                    pessoaCampoObrigatorio.Fornecedor,
                    pessoaCampoObrigatorio.Terceiro,
                    Campos = pessoaCampoObrigatorio.Campos.Select(o => new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.Campo
                    }).ToList()
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

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio pessoaCampoObrigatorio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio repPessoaCampoObrigatorio = new Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio(unitOfWork);

            bool cliente = Request.GetBoolParam("Cliente");
            bool fornecedor = Request.GetBoolParam("Fornecedor");
            bool terceiro = Request.GetBoolParam("Terceiro");

            if (!cliente && !fornecedor && !terceiro)
                throw new ControllerException("Favor selecione ao menos um tipo de pessoa.");

            pessoaCampoObrigatorio.Cliente = cliente;
            pessoaCampoObrigatorio.Fornecedor = fornecedor;
            pessoaCampoObrigatorio.Terceiro = terceiro;
            pessoaCampoObrigatorio.Ativo = Request.GetBoolParam("Situacao");

            if (repPessoaCampoObrigatorio.ExistePorTipoPessoa(pessoaCampoObrigatorio.Codigo, cliente, fornecedor, terceiro))
                throw new ControllerException("Já existe uma configuração ativa com estes tipos de pessoas.");

            PreencherCamposEntidade(pessoaCampoObrigatorio, unitOfWork);
        }

        private void PreencherCamposEntidade(Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio pessoaCampoObrigatorio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo repPessoaCampo = new Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo(unitOfWork);

            dynamic campos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Campos"));

            if (pessoaCampoObrigatorio.Campos == null)
            {
                pessoaCampoObrigatorio.Campos = new List<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic campo in campos)
                    codigos.Add((int)campo.Codigo);

                List<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo> camposDeletar = pessoaCampoObrigatorio.Campos.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo campoDeletar in camposDeletar)
                    pessoaCampoObrigatorio.Campos.Remove(campoDeletar);
            }

            foreach (dynamic campo in campos)
            {
                Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo pessoaCampo = repPessoaCampo.BuscarPorCodigo((int)campo.Codigo, false);
                pessoaCampoObrigatorio.Campos.Add(pessoaCampo);
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Enumeradores.OpcaoSimNaoPesquisa cliente = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("Cliente");
                Dominio.Enumeradores.OpcaoSimNaoPesquisa fornecedor = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("Fornecedor");
                Dominio.Enumeradores.OpcaoSimNaoPesquisa terceiro = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("Terceiro");
                bool? ativo = Request.GetNullableBoolParam("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cliente", "DescricaoCliente", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Fornecedor", "DescricaoFornecedor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador Terceiro", "DescricaoTerceiro", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Ativo", 20, Models.Grid.Align.left, false);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio repPessoaCampoObrigatorio = new Repositorio.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio> listaPessoaCamposObrigatorios = repPessoaCampoObrigatorio.Consultar(cliente, fornecedor, terceiro, ativo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repPessoaCampoObrigatorio.ContarConsulta(cliente, fornecedor, terceiro, ativo);

                var retorno = listaPessoaCamposObrigatorios.Select(o => new
                {
                    o.Codigo,
                    o.DescricaoCliente,
                    o.DescricaoFornecedor,
                    o.DescricaoTerceiro,
                    Ativo = o.DescricaoAtivo
                }).ToList();

                grid.AdicionaRows(retorno);
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
            return propriedadeOrdenar;
        }

        #endregion
    }
}
