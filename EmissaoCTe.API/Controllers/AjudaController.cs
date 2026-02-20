using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class AjudaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("cadastroajuda.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Ajuda repAjuda = new Repositorio.Ajuda(unitOfWork);

                string status = Request.Params["Status"] ?? string.Empty;
                string descricao = Request.Params["Descricao"] ?? string.Empty;

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                
                List<Dominio.Entidades.Ajuda> listaAjuda = repAjuda.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int count = repAjuda.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = (from obj in listaAjuda select new {
                    obj.Codigo,
                    obj.Descricao,
                    DataCadastro = obj.DataCadastro.ToString("dd/MM/yyyy"),
                    TipoAjuda = obj.DescricaoTipoAjuda,
                    Status = obj.DescricaoStatus
                }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Descrição|40", "Data|20", "Tipo de Ajuda|15", "Status|15" }, count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterAjudasPorEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Ajuda repAjuda = new Repositorio.Ajuda(unitOfWork);

                List<Dominio.Entidades.Ajuda> ajudas = repAjuda.BuscarTodasAjudas(this.EmpresaUsuario.EmpresaPai.Codigo);

                var retorno = from o in ajudas select
                              new
                              {
                                  o.Codigo,
                                  o.Descricao,
                                  o.LinkVideo,
                                  o.NomeArquivo,
                                  Tipo = o.TipoAjuda
                              };

                return Json(retorno.ToList(), true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Ajuda repAjuda = new Repositorio.Ajuda(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.Ajuda ajuda = repAjuda.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                if(ajuda == null)
                    return Json<bool>(false, false, "Falha ao obter os dados.");

                var retorno = new {
                    ajuda.Codigo,
                    ajuda.Descricao,
                    ajuda.Status,
                    LinkVideo = ajuda.LinkVideoYouTube,
                    Log = ajuda.Log ?? string.Empty,
                    ajuda.TipoAjuda,
                    Arquivo = ajuda.NomeArquivo ?? string.Empty
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult InserirArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Ajuda repAjuda = new Repositorio.Ajuda(unitOfWork);

                string[] extensoesValidas = { ".jpg", ".png", ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".txt" };
                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.Ajuda ajuda = repAjuda.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (ajuda == null)
                    return Json<bool>(false, false, "Falha ao obter os dados.");

                if (!string.IsNullOrWhiteSpace(ajuda.NomeArquivo))
                    return Json<bool>(false, false, "Já existe um arquivo para essa ajuda.");

                if (Request.Files.Count == 0)
                    return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");

                // Converte arquivo upado
                System.Web.HttpPostedFileBase file = Request.Files[0];

                // Valida
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!extensoesValidas.Contains(extensao))
                    return Json<bool>(false, false, "Extensão " + extensao.Substring(1) + " inválida.");

                // Salva na pasta configurada
                string caminho = CaminhoArquivos();
                string arquivoFisico = ajuda.Codigo.ToString() + extensao;
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                Utilidades.IO.FileStorageService.Storage.SaveStream(arquivoFisico, file.InputStream);
                
                // Cria Entidade e insere
                ajuda.NomeArquivo = file.FileName;
                ajuda.CaminhoArquivo = arquivoFisico;

                // Atualiza LOG
                ajuda.Log += Environment.NewLine + "Arquivo " + ajuda.NomeArquivo + " inserido por " + this.Usuario.Nome + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                repAjuda.Atualizar(ajuda);

                // Retorna informacoes
                return Json(new
                {
                    Codigo = ajuda.Codigo,
                    Nome = ajuda.NomeArquivo,
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");
            }

        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Ajuda repAjuda = new Repositorio.Ajuda(unitOfWork);

                // Busca Anexo
                string download = Request.Params["Download"] ?? string.Empty; // Quando essa flag for MultiCTe, significa que deve pegar o codigo da empresa pai, e não da empresa do usuario
                int.TryParse(Request.Params["Codigo"], out int codigo);
                int codigoEmpresa = download == "MultiCTe" ? this.EmpresaUsuario.EmpresaPai.Codigo : this.EmpresaUsuario.Codigo;

                Dominio.Entidades.Ajuda ajuda = repAjuda.BuscarPorCodigo(codigoEmpresa, codigo);

                // Valida
                if (ajuda == null)
                    return Json<bool>(false, false, "Falha ao obter os dados.");

                string extencao = System.IO.Path.GetExtension(ajuda.NomeArquivo).ToLower();
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(ajuda.CaminhoArquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, extencao.Replace(".", ""), ajuda.NomeArquivo);
                else
                    return Json<bool>(false, false, "Ocorreu uma falha ao buscar arquivo.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult RemoverArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                throw new NotImplementedException();

                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Ajuda repAjuda = new Repositorio.Ajuda(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params["Codigo"], out int codigo);
                Dominio.Entidades.Ajuda ajuda = repAjuda.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                // Valida
                if (ajuda == null)
                    return Json<bool>(false, false, "Falha ao obter os dados.");
                
                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(ajuda.CaminhoArquivo))
                    return Json<bool>(false, false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(ajuda.CaminhoArquivo);

                // Remove do banco
                ajuda.NomeArquivo = "";
                ajuda.CaminhoArquivo = "";
                repAjuda.Atualizar(ajuda);

                // Commita
                unitOfWork.CommitChanges();

                return Json(true, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Ajuda repAjuda = new Repositorio.Ajuda(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                Dominio.Entidades.Ajuda ajuda = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    ajuda = repAjuda.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    ajuda = new Dominio.Entidades.Ajuda()
                    {
                        DataCadastro = DateTime.Now,
                        Log = "Criado por " + this.Usuario.Nome + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        Empresa = this.EmpresaUsuario
                    };
                }

                if (ajuda == null)
                    return Json<bool>(false, false, "Não foi possível encontrar os dados.");

                Enum.TryParse(Request.Params["Tipo"], out Dominio.Enumeradores.TipoAjuda tipo);
                string descricao = Request.Params["Descricao"] ?? string.Empty;
                string status = Request.Params["Status"] ?? string.Empty;
                string linkVideo = Request.Params["LinkVideo"] ?? string.Empty;

                ajuda.Descricao = descricao;
                ajuda.Status = status;
                ajuda.LinkVideo = linkVideo;
                ajuda.TipoAjuda = tipo;

                if(ajuda.Codigo > 0 )
                    ajuda.Log += Environment.NewLine + "Atualizado por " + this.Usuario.Nome + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                if (codigo > 0)
                    repAjuda.Atualizar(ajuda);
                else
                    repAjuda.Inserir(ajuda);

                unitOfWork.CommitChanges();

                return Json(new { ajuda.Codigo }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private string CaminhoArquivos()
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"], "Anexos", "ArquivosAjuda");

            return caminho;
        }
    }
}
