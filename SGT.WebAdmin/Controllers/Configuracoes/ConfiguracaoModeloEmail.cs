using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoModeloEmail")]
    public class ConfiguracaoModeloEmailController : BaseController
    {
        #region Construtores

        public ConfiguracaoModeloEmailController(Conexao conexao) : base(conexao) { }

        #endregion
        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repositorioConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail();

                PreencherEntidade(modeloEmail);

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> existePorTipo = repositorioConfiguracaoModeloEmail.BuscarPorTipoModeloEmail(modeloEmail.TipoModeloEmail);

                if (modeloEmail.TipoModeloEmail == TipoModeloEmail.GestaoCustoContabilDevolucao || modeloEmail.TipoModeloEmail == TipoModeloEmail.ImprocedenciaCenarioPosEntregaDevolucao)
                {
                    if (existePorTipo.Count > 0)
                        return new JsonpResult(false, true, $"Já existe um modelo de e-mail cadastrado para o tipo {modeloEmail.TipoModeloEmail.ObterDescricao()}.");
                }

                repositorioConfiguracaoModeloEmail.Inserir(modeloEmail, Auditado);

                var retorno = new
                {
                    modeloEmail.Codigo
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repositorioConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail = repositorioConfiguracaoModeloEmail.BuscarPorCodigo(codigo, false);

                if (modeloEmail == null)
                    return new JsonpResult(false, "Modelo de email não encontrado");

                PreencherEntidade(modeloEmail);

                repositorioConfiguracaoModeloEmail.Atualizar(modeloEmail, Auditado);

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repositorioConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo repositorioConfiguracaoModeloEmailAnexo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail = repositorioConfiguracaoModeloEmail.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo modeloEmailAnexo = repositorioConfiguracaoModeloEmailAnexo.BuscarPorModeloEmail(codigo);

                if (modeloEmail == null)
                    return new JsonpResult(false, "Modelo de email não encontrado");

                var retorno = new
                {
                    modeloEmail.Codigo,
                    modeloEmail.Descricao,
                    Status = modeloEmail.Ativo,
                    modeloEmail.Assunto,
                    modeloEmail.Corpo,
                    modeloEmail.RodaPe,
                    Tipo = modeloEmail.TipoModeloEmail,
                    EnviarPara = modeloEmail.TipoEnviarPara,
                    GatilhoNotificacao = modeloEmail.TipoGatilhoNotificacao,
                    CodigoAnexo = modeloEmailAnexo != null ? modeloEmailAnexo.Codigo : 0
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

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo repositorioConfiguracaoModeloEmailAnexo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repositorioConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);

                int codigoConfiguracaoModeloEmail = Request.GetIntParam("CodigoModeloEmail");
                string descricao = Request.GetStringParam("Descricao");

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "ModeloEmailAnexo");
                
                var anexoExistente = repositorioConfiguracaoModeloEmailAnexo.BuscarPorModeloEmail(codigoConfiguracaoModeloEmail);
                if (anexoExistente != null)
                {

                    if (Utilidades.IO.FileStorageService.Storage.Exists(anexoExistente.CaminhoArquivo))
                    {
                        Utilidades.IO.FileStorageService.Storage.Delete(anexoExistente.CaminhoArquivo);
                    }

                    repositorioConfiguracaoModeloEmailAnexo.Deletar(anexoExistente);
                }

                for (var i = 0; i < files.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo modeloEmailAnexo = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo();

                    Servicos.DTO.CustomFile file = files[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);

                    file.SaveAs(caminho);

                    modeloEmailAnexo.CaminhoArquivo = caminho;
                    modeloEmailAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(Path.GetFileName(nomeArquivo)));
                    modeloEmailAnexo.Descricao = descricao;
                    modeloEmailAnexo.ConfiguracaoModeloEmail = repositorioConfiguracaoModeloEmail.BuscarPorCodigo(codigoConfiguracaoModeloEmail, false);

                    repositorioConfiguracaoModeloEmailAnexo.Inserir(modeloEmailAnexo);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo repositorioConfiguracaoModeloEmailAnexo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repositorioConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);

                int codigoConfiguracaoModeloEmailAnexo = Request.GetIntParam("CodigoAnexo");


                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo modeloEmailAnexo = repositorioConfiguracaoModeloEmailAnexo.BuscarPorCodigo(codigoConfiguracaoModeloEmailAnexo, false);

                if (modeloEmailAnexo == null || modeloEmailAnexo?.ConfiguracaoModeloEmail == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] arquivoBinario = Utilidades.File.LerArquivo(modeloEmailAnexo.CaminhoArquivo);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, $"application/{modeloEmailAnexo.ExtensaoArquivo}", modeloEmailAnexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu um falha ao relizar o download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos privados
        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "Status", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo", "TipoModeloEmail", 7, Models.Grid.Align.left, true);

            Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoModeloEmail filtrosPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repositorioConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);

            int totalRegistros = repositorioConfiguracaoModeloEmail.ContarConsulta(filtrosPesquisa);

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> listaModelosEmail = totalRegistros > 0 ? repositorioConfiguracaoModeloEmail.Consultar(filtrosPesquisa, grid.ObterParametrosConsulta()).ToList() : new List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail>();

            grid.setarQuantidadeTotal(totalRegistros);

            grid.AdicionaRows((from modelo in listaModelosEmail
                               select new
                               {
                                   modelo.Codigo,
                                   modelo.Descricao,
                                   Status = modelo.Ativo ? "Ativo" : "Inativo",
                                   TipoModeloEmail = modelo.TipoModeloEmail.ObterDescricao()
                               }).ToList());

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoModeloEmail ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoModeloEmail()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetNullableBoolParam("Status"),
                Tipo = Request.GetNullableEnumParam<TipoModeloEmail>("Tipo"),
            };
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail PreencherEntidade(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail)
        {
            modeloEmail.Descricao = Request.GetStringParam("Descricao");
            modeloEmail.Ativo = Request.GetBoolParam("Status");
            modeloEmail.Assunto = Request.GetStringParam("Assunto");
            modeloEmail.Corpo = Request.GetStringParam("Corpo");
            modeloEmail.RodaPe = Request.GetStringParam("RodaPe");
            modeloEmail.TipoModeloEmail = Request.GetEnumParam<TipoModeloEmail>("Tipo");
            modeloEmail.TipoGatilhoNotificacao = Request.GetEnumParam<TipoGatilhoNotificacao>("GatilhoNotificacao");
            modeloEmail.TipoEnviarPara = Request.GetEnumParam<TipoEnviarPara>("EnviarPara");

            return modeloEmail;
        }

        #endregion
    }
}