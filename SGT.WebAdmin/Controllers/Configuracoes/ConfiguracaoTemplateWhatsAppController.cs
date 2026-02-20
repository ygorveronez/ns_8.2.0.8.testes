using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoTemplateWhatsApp")]
    public class ConfiguracaoTemplateWhatsAppController : BaseController
    {
		#region Construtores

		public ConfiguracaoTemplateWhatsAppController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.GetStringParam("Nome");
                string idioma = Request.GetStringParam("Idioma");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nome", "Nome", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Idioma", "Idioma", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "Status", 30, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp repConfiguracaoTemplateWhatsApp = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp(unitOfWork);

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp> listaTemplatesWhatsApp = repConfiguracaoTemplateWhatsApp.BuscarTodos();

                grid.setarQuantidadeTotal(repConfiguracaoTemplateWhatsApp.BuscarTodos().Count());

                grid.AdicionaRows((from t in listaTemplatesWhatsApp
                                   select new
                                   {
                                       t.Codigo,
                                       t.Nome,
                                       t.Idioma,
                                       t.Mensagem,
                                       Status = t.Status.ObterDescricao(),
                                   }).ToList().OrderByDescending(c => c.Codigo));

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp repTemplateWhatsApp = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp configuracaoTemplateMensagemWhatsApp = repTemplateWhatsApp.BuscarPorCodigo(codigo, false);

                var retorno = new
                {
                    configuracaoTemplateMensagemWhatsApp.Codigo,
                    NomeTemplate = configuracaoTemplateMensagemWhatsApp.Nome,
                    configuracaoTemplateMensagemWhatsApp.Idioma,
                    CorpoMensagem = configuracaoTemplateMensagemWhatsApp.Mensagem,
                    configuracaoTemplateMensagemWhatsApp.TipoTemplate
                };

                return new JsonpResult(retorno);
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


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.GetStringParam("Nome");
                string idioma = Request.GetStringParam("Idioma");
                string mensagem = Request.GetStringParam("CorpoMensagem");
                TipoTemplateWhatsApp tipoTemplate = Request.GetEnumParam<TipoTemplateWhatsApp>("TipoTemplate");

                if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(idioma) || string.IsNullOrEmpty(mensagem))
                    throw new ControllerException(Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.NenhumCampoPodeFicarVazio);

                if (!ValidarNome(nome))
                    throw new ControllerException(Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.NomeDoTemplateNaoPodeConterCaracteresEspeciais);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp repConfiguracaoTemplateWhatsApp = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTemplateWhatsApp(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp configuracaoTemplateMensagemWhatsApp = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp();

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp> templatesSalvos = repConfiguracaoTemplateWhatsApp.BuscarTodos();

                if (templatesSalvos.Any(t => t.Nome == nome))
                    throw new ControllerException(Localization.Resources.Configuracoes.ConfiguracaoTemplateWhatsApp.NomeDoTemplateJaCadastrado);

                configuracaoTemplateMensagemWhatsApp.Nome = nome;
                configuracaoTemplateMensagemWhatsApp.Idioma = idioma;
                configuracaoTemplateMensagemWhatsApp.Mensagem = mensagem;
                configuracaoTemplateMensagemWhatsApp.TipoTemplate = tipoTemplate;

                Servicos.Embarcador.Integracao.META.Templates.TemplateMensagemWhatsApp svcTemplates = new Servicos.Embarcador.Integracao.META.Templates.TemplateMensagemWhatsApp(unitOfWork);

                svcTemplates.CriarNovoTemplate(configuracaoTemplateMensagemWhatsApp);

                configuracaoTemplateMensagemWhatsApp.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTemplateWhatsApp.AguardandoAprovacao;

                repConfiguracaoTemplateWhatsApp.Inserir(configuracaoTemplateMensagemWhatsApp);

                svcTemplates.AtualizarStatusTemplate(configuracaoTemplateMensagemWhatsApp);


                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        static bool ValidarNome(string nome)
        {
            Regex regex = new Regex(@"^[a-z0-9_]+$");

            return regex.IsMatch(nome);

        }

    }
}
