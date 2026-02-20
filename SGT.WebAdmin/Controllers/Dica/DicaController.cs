using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Controllers.Dica
{
    [CustomAuthorize("Dica/Dica")]
    public class DicaController : BaseController
    {
		#region Construtores

		public DicaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Global.Dica repositorioDica = new Repositorio.Global.Dica(unitOfWork);

                int issue = Request.GetIntParam("Issue");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAjuda", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Título", "Titulo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data de criação", "DataCriacao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Dica> dicas = repositorioDica.Consultar(issue, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioDica.ContarConsulta(issue));

                var retorno = (from dica in dicas
                               select new
                               {
                                   dica.Codigo,
                                   dica.CodigoAjuda,
                                   Usuario = dica.Usuario?.Descricao ?? string.Empty,
                                   Titulo = dica.Titulo ?? string.Empty,
                                   DataCriacao = dica.DataCriacao.ToString("dd/MM/yyyy") ?? string.Empty,
                                   Descricao = RemoverHTML(dica.Descricao ?? string.Empty),
                               }).ToList();

                grid.AdicionaRows(retorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Global.Dica repositorioDica = new Repositorio.Global.Dica(unitOfWork);

                if (!this.Usuario.PermiteInserirDicas)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                Dominio.Entidades.Dica dica = new Dominio.Entidades.Dica();

                PreencherDica(dica, unitOfWork);

                dica.CodigoAjuda = Request.GetIntParam("Issue");
                dica.Usuario = this.Usuario;
                dica.DataCriacao = DateTime.Now;

                repositorioDica.Inserir(dica, Auditado);

                return new JsonpResult(new { Codigo = dica?.Codigo ?? 0 });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                Repositorio.Global.Dica repositorioDica = new Repositorio.Global.Dica(unitOfWork);

                if (!this.Usuario.PermiteInserirDicas)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                int codigoDica = Request.GetIntParam("Codigo");

                Dominio.Entidades.Dica dica = repositorioDica.BuscarPorCodigo(codigoDica, false);

                if (dica == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherDica(dica, unitOfWork);

                repositorioDica.Atualizar(dica, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                Repositorio.Global.Dica repositorioDica = new Repositorio.Global.Dica(unitOfWork);

                int codigoDica = Request.GetIntParam("Codigo");

                Dominio.Entidades.Dica dica = repositorioDica.BuscarPorCodigo(codigoDica, false);

                if (dica == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var dynDica = new
                {
                    dica.Codigo,
                    Issue = dica.CodigoAjuda,
                    dica.Titulo,
                    dica.Descricao,
                    dica.LinkVideo,
                    Anexos = ObterAnexos(dica),
                };

                return new JsonpResult(dynDica);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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
                Repositorio.Global.Dica repositorioDica = new Repositorio.Global.Dica(unitOfWork);

                if (!this.Usuario.PermiteInserirDicas)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                int codigoDica = Request.GetIntParam("Codigo");

                Dominio.Entidades.Dica dica = repositorioDica.BuscarPorCodigo(codigoDica, false);

                if (dica == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start(); 

                repositorioDica.Deletar(dica, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPermissaoUsuario()
        {
            try
            {
                var retorno = new
                {
                    PermiteInserirDicas = this.Usuario.PermiteInserirDicas,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarConfiguracaoPadrao);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherDica(Dominio.Entidades.Dica dica, Repositorio.UnitOfWork unitOfWork)
        {
            dica.Titulo = Request.GetStringParam("Titulo");
            dica.Descricao = Request.GetStringParam("Descricao");
            dica.LinkVideo = Request.GetStringParam("LinkVideo");
        }

        private string RemoverHTML(string value)
        {
            var passo1 = Regex.Replace(value, @"<p>|<li>|<td>", " ").Trim(); //Converte tags que separam o texto de alguma forma, em um espaçamento simples para o texto não ficar "amontoado"
            var passo2 = Regex.Replace(passo1, @"<[^>]+>|&nbsp;", "").Trim(); //Remove o restante das tags
            var passo3 = Regex.Replace(passo2, @"\s{2,}", " "); //Remove espaçamentos com mais de um espaço
            return passo3;
        }

        private dynamic ObterAnexos(Dominio.Entidades.Dica dica)
        {
            if (dica?.Anexos == null)
                return null;

            return (
                from anexo in dica.Anexos
                select new
                {
                    anexo.Codigo,
                    anexo.Descricao,
                    anexo.NomeArquivo,
                }
            ).ToList();
        }

        #endregion
    }
}
