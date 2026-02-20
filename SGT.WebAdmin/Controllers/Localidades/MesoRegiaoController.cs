using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Localidades
{
    public class MesoRegiaoController : BaseController
    {
		#region Construtores

		public MesoRegiaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Localidades.MesoRegiao repMesoRegiao = new Repositorio.Embarcador.Localidades.MesoRegiao(unitOfWork);
                Dominio.Entidades.Embarcador.Localidades.MesoRegiao mesoRegiao = new Dominio.Entidades.Embarcador.Localidades.MesoRegiao();

                PreencherMesoRegiao(mesoRegiao);

                repMesoRegiao.Inserir(mesoRegiao);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar Meso Região.");
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

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Localidades.MesoRegiao repMesoRegiao = new Repositorio.Embarcador.Localidades.MesoRegiao(unitOfWork);
                Dominio.Entidades.Embarcador.Localidades.MesoRegiao mesoRegiao = repMesoRegiao.BuscarPorCodigo(codigo, false);

                if (mesoRegiao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMesoRegiao(mesoRegiao);

                repMesoRegiao.Atualizar(mesoRegiao);
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
                Repositorio.Embarcador.Localidades.MesoRegiao repMesoRegiao = new Repositorio.Embarcador.Localidades.MesoRegiao(unitOfWork);
                Dominio.Entidades.Embarcador.Localidades.MesoRegiao mesoRegiao = repMesoRegiao.BuscarPorCodigo(codigo, false);

                if (mesoRegiao == null)
                    throw new ControllerException("Meso Regioão não encontrada");

                return new JsonpResult(new
                {
                    mesoRegiao.Codigo,
                    mesoRegiao.Descricao,
                    mesoRegiao.CodigoIntegracao,
                    mesoRegiao.Situacao
                });
            }
            catch (ControllerException exception)
            {
                return new JsonpResult(false, true, exception.Message);
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
                Repositorio.Embarcador.Localidades.MesoRegiao repMesoRegiao = new Repositorio.Embarcador.Localidades.MesoRegiao(unitOfWork);
                Dominio.Entidades.Embarcador.Localidades.MesoRegiao mesoRegiao = repMesoRegiao.BuscarPorCodigo(codigo, false);

                if (mesoRegiao == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repMesoRegiao.Deletar(mesoRegiao);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException exception)
            {
                return new JsonpResult(false, true, exception.Message);
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


        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao obter as configurações para importação.");
            }
        }

        public async Task<IActionResult> CadastrarMesoRegiaoImportacao()
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = ImportarRegistros(unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao importa a planilha.");
            }
        }

        #endregion

        #region Metodos Privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Localidades.MesoRegiao repMesoRegioa = new Repositorio.Embarcador.Localidades.MesoRegiao(unitOfWork);

                string descricao = Request.GetStringParam("Descricao");
                bool status = Request.GetBoolParam("Situacao");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");

                var grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Codigo Integração", "CodigoIntegracao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.left, true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                var listaMesoRegiao = repMesoRegioa.Consultar(descricao, status, codigoIntegracao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                var totalRegistros = repMesoRegioa.ContarConsulta(descricao, status, codigoIntegracao);

                var listaMesoRegiaoRetornar = (
                    from mesoRegiao in listaMesoRegiao
                    select new
                    {
                        mesoRegiao.Codigo,
                        mesoRegiao.Descricao,
                        mesoRegiao.CodigoIntegracao,
                        mesoRegiao.Situacao
                    }
                ).ToList();

                grid.AdicionaRows(listaMesoRegiaoRetornar);
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

            return propriedadeOrdenar;
        }

        private void PreencherMesoRegiao(Dominio.Entidades.Embarcador.Localidades.MesoRegiao mesoRegiao)
        {
            mesoRegiao.Descricao = Request.GetStringParam("Descricao");
            mesoRegiao.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            mesoRegiao.Situacao = Request.GetBoolParam("Situacao");
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilha()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Descrição", Propriedade = "Descricao", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "CodigoIntegração", Propriedade = "CodigoIntegracao", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Situação", Propriedade = "Situacao", Tamanho = 100, Obrigatorio = false });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarRegistros(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Localidades.MesoRegiao repMesoRegiao = new Repositorio.Embarcador.Localidades.MesoRegiao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            int totalRegistrosImportados = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(Request.Params("Dados"));
            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    string codigoIntegracao = ObterValorCampoTexto(linha, "CodigoIntegracao", true);
                    string descricao = ObterValorCampoTexto(linha, "Descricao", true);
                    string situacao = ObterValorCampoTexto(linha, "Situacao");

                    Dominio.Entidades.Embarcador.Localidades.MesoRegiao mesoRegiao = repMesoRegiao.BuscaPorCodigoIntegracao(codigoIntegracao);

                    if (mesoRegiao == null)
                        mesoRegiao = new Dominio.Entidades.Embarcador.Localidades.MesoRegiao();

                    mesoRegiao.CodigoIntegracao = codigoIntegracao;
                    mesoRegiao.Descricao = descricao;

                    if (!string.IsNullOrEmpty(situacao))
                        mesoRegiao.Situacao = situacao == "Ativo" ? true : false;

                    if (mesoRegiao.Codigo > 0)
                        repMesoRegiao.Atualizar(mesoRegiao);
                    else
                        repMesoRegiao.Inserir(mesoRegiao);

                    totalRegistrosImportados++;
                    retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i));
                }
                catch (ControllerException exception)
                {
                    retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(exception.Message, i));
                }
            }

            retornoImportacao.Importados = totalRegistrosImportados;
            retornoImportacao.Total = linhas.Count;

            return retornoImportacao;
        }

        private string ObterValorCampoTexto(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, string nomeCampo, bool obrigatorio = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coluna = linha.Colunas?.Where(o => o.NomeCampo == nomeCampo).FirstOrDefault();
            string valorCampo = ((string)coluna?.Valor ?? "").Trim();

            if (string.IsNullOrEmpty(valorCampo) && obrigatorio)
                throw new ControllerException($"Campo Obrigatorio esta vazio");

            if (string.IsNullOrEmpty(valorCampo))
                return string.Empty;

            return valorCampo;
        }
        #endregion
    }
}

