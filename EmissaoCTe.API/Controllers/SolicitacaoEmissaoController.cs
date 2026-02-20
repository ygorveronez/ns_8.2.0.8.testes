using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace EmissaoCTe.API.Controllers
{
    public class SolicitacaoEmissaoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("solicitacaoretornoarquivos.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, inicioRegistros, codigoUsuarioEnvio, codigoUsuarioRetorno;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["CodigoUsuarioEnvio"], out codigoUsuarioEnvio);
                int.TryParse(Request.Params["CodigoUsuarioRetorno"], out codigoUsuarioRetorno);

                string assunto = Request.Params["Assunto"];
                string texto = Request.Params["Texto"];
                string usuarioEnvio = Request.Params["UsuarioEnvio"];
                string nomeTransportador = Request.Params["Transportador"];

                DateTime dataEnvio, dataRetorno, dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataEnvio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEnvio);
                DateTime.TryParseExact(Request.Params["DataRetorno"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataRetorno);
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                if(dataFinal != DateTime.MinValue)
                    dataFinal = dataFinal.AddDays(1);

                Dominio.Enumeradores.StatusSolicitacaoEmissao statusAux;
                Dominio.Enumeradores.StatusSolicitacaoEmissao? status = null;
                if (Enum.TryParse<Dominio.Enumeradores.StatusSolicitacaoEmissao>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                Repositorio.SolicitacaoEmissao repSolicitacoes = new Repositorio.SolicitacaoEmissao(unitOfWork);

                List<Dominio.Entidades.SolicitacaoEmissao> listaSolicitacoes = repSolicitacoes.Consultar(codigo, codigoUsuarioEnvio, codigoUsuarioRetorno, assunto, texto, dataEnvio, dataRetorno, status, usuarioEnvio, nomeTransportador, dataInicial, dataFinal, inicioRegistros, 50);

                int countDocumentos = repSolicitacoes.ContarConsulta(codigo, codigoUsuarioEnvio, codigoUsuarioRetorno, assunto, texto, dataEnvio, dataRetorno, status, usuarioEnvio, nomeTransportador, dataInicial, dataFinal);

                var retorno = (from obj in listaSolicitacoes
                               select new
                               {
                                   obj.Codigo,
                                   Envio = obj.UsuarioEnvio.Nome,
                                   DataEnvio = obj.DataEnvio.ToString(),
                                   obj.Assunto,
                                   Status = obj.Status == Dominio.Enumeradores.StatusSolicitacaoEmissao.Pendente ? "Pendente" : obj.Status == Dominio.Enumeradores.StatusSolicitacaoEmissao.Alocado ? "Alocado" : obj.Status == Dominio.Enumeradores.StatusSolicitacaoEmissao.Finalizado ? "Finalizado" : string.Empty,
                                   Transportador = obj.Empresa != null ? obj.Empresa.CNPJ + " " + obj.Empresa.RazaoSocial : string.Empty,
                                   Retorno = obj.UsuarioRetorno != null ? obj.UsuarioRetorno.Nome : string.Empty,
                                   DataRetorno = obj.DataRetorno != null ? obj.DataRetorno.ToString() : string.Empty,
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Usuário Envio|10", "Dt. Envio|10", "Assunto|20", "Status|5", "Transportador|15", "Usuário Retorno|10", "Dt. Retorno|10" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as duplicatas.");
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
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.SolicitacaoEmissao repSolicitacao = new Repositorio.SolicitacaoEmissao(unitOfWork);

                Dominio.Entidades.SolicitacaoEmissao solicitacao = repSolicitacao.BuscaPorCodigo(codigo);

                if (solicitacao == null)
                    return Json<bool>(false, false, "Solicitação não encontrada.");

                var retorno = new
                {
                    solicitacao.Codigo,
                    solicitacao.Assunto,
                    solicitacao.Texto,
                    Transportador = solicitacao.Empresa != null ? solicitacao.Empresa.CNPJ + " " + solicitacao.Empresa.RazaoSocial : string.Empty,
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da solicitação.");
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
                string assunto = Request.Params["Assunto"];
                string texto = Request.Params["Texto"];

                Dominio.Entidades.SolicitacaoEmissao solicitacaoEmissao = new Dominio.Entidades.SolicitacaoEmissao();

                Repositorio.SolicitacaoEmissao repSolicitacaoEmissao = new Repositorio.SolicitacaoEmissao(unitOfWork);

                solicitacaoEmissao.Assunto = assunto;
                solicitacaoEmissao.Texto = texto;
                solicitacaoEmissao.UsuarioEnvio = this.Usuario;
                solicitacaoEmissao.DataEnvio = DateTime.Now;
                solicitacaoEmissao.DataRetorno = null;
                solicitacaoEmissao.Status = Dominio.Enumeradores.StatusSolicitacaoEmissao.Pendente;

                repSolicitacaoEmissao.Inserir(solicitacaoEmissao);
                
                return Json(solicitacaoEmissao.Codigo, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Solicitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoEmpresa = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                Repositorio.SolicitacaoEmissao repSolicitacaoEmissao = new Repositorio.SolicitacaoEmissao(unitOfWork);
                Dominio.Entidades.SolicitacaoEmissao solicitacaoEmissao = repSolicitacaoEmissao.BuscaPorCodigo(codigo);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (solicitacaoEmissao == null)
                    return Json<bool>(false, false, "Solicitação não encontrada.");

                if (solicitacaoEmissao.UsuarioRetorno != null && solicitacaoEmissao.UsuarioRetorno != this.Usuario)
                    return Json<bool>(false, false, "Não é possível retornar Solicitação, a mesma está alocada para o usuário "+ solicitacaoEmissao.UsuarioRetorno.Nome + ".");

                if (empresa == null)
                    return Json<bool>(false, false, "Transportador não encontrado.");

                solicitacaoEmissao.Empresa = empresa;
                solicitacaoEmissao.Status = Dominio.Enumeradores.StatusSolicitacaoEmissao.Finalizado;
                solicitacaoEmissao.DataRetorno = DateTime.Now;

                repSolicitacaoEmissao.Atualizar(solicitacaoEmissao);                

                return Json(solicitacaoEmissao.Codigo, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Solicitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Alocar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.SolicitacaoEmissao repSolicitacaoEmissao = new Repositorio.SolicitacaoEmissao(unitOfWork);
                Dominio.Entidades.SolicitacaoEmissao solicitacaoEmissao = repSolicitacaoEmissao.BuscaPorCodigo(codigo);

                if (solicitacaoEmissao == null)
                    return Json<bool>(false, false, "Solicitação não encontrada.");

                if (solicitacaoEmissao.Status == Dominio.Enumeradores.StatusSolicitacaoEmissao.Alocado)
                {
                    if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    {
                        solicitacaoEmissao.Status = Dominio.Enumeradores.StatusSolicitacaoEmissao.Pendente;
                        solicitacaoEmissao.UsuarioRetorno = null;

                        repSolicitacaoEmissao.Atualizar(solicitacaoEmissao);

                        return Json<bool>(false, false, "Removido usuário alocado.");
                    }                     
                    else
                        return Json<bool>(false, false, "Solicitação já está alocada para " + solicitacaoEmissao.UsuarioRetorno.Nome + ".");
                }

                if (solicitacaoEmissao.Status == Dominio.Enumeradores.StatusSolicitacaoEmissao.Finalizado)
                    return Json<bool>(false, false, "Solicitação já foi finalizada, não é possível alocar.");

                solicitacaoEmissao.Status = Dominio.Enumeradores.StatusSolicitacaoEmissao.Alocado;
                solicitacaoEmissao.UsuarioRetorno = this.Usuario;

                repSolicitacaoEmissao.Atualizar(solicitacaoEmissao);

                return Json(solicitacaoEmissao.Codigo, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao Alocar Solicitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarSolicitacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            int codigo = 0;
            int.TryParse(Request.Params["Codigo"], out codigo);

            try
            {
                if (codigo == 0)
                    return Json<bool>(false, false, "Código solicitação invalida.");

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];

                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoSolicitacaoEmissao"], codigo.ToString(), "Envio");

                    Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(caminho, System.IO.Path.GetFileName(file.FileName)), file.InputStream);
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }
                
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                Repositorio.SolicitacaoEmissao repSolicitacaoEmissao = new Repositorio.SolicitacaoEmissao(unitOfWork);
                Dominio.Entidades.SolicitacaoEmissao solicitacaoEmissao = repSolicitacaoEmissao.BuscaPorCodigo(codigo);
                if (solicitacaoEmissao != null)
                    repSolicitacaoEmissao.Deletar(solicitacaoEmissao);

                return Json<bool>(false, false, "Ocorreu uma falha ao Enviar Solicitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult RetornarSolicitacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            int codigo = 0;
            int.TryParse(Request.Params["Codigo"], out codigo);

            try
            {
                if (codigo == 0)
                    return Json<bool>(false, false, "Código solicitação invalida.");

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];

                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoSolicitacaoEmissao"], codigo.ToString(), "Retorno");

                    Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(caminho, System.IO.Path.GetFileName(file.FileName)), file.InputStream);
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                Repositorio.SolicitacaoEmissao repSolicitacaoEmissao = new Repositorio.SolicitacaoEmissao(unitOfWork);
                Dominio.Entidades.SolicitacaoEmissao solicitacaoEmissao = repSolicitacaoEmissao.BuscaPorCodigo(codigo);
                if (solicitacaoEmissao != null)
                {
                    solicitacaoEmissao.Status = Dominio.Enumeradores.StatusSolicitacaoEmissao.Alocado;
                    repSolicitacaoEmissao.Atualizar(solicitacaoEmissao);
                }

                return Json<bool>(false, false, "Ocorreu uma falha ao Retornar Solicitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadEnvios()
        {
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                if (codigo == 0)
                    return Json<bool>(false, false, "Código solicitação invalida.");

                return Arquivo(this.ObterArquivos(codigo, "Envio"), "application/zip", "Solicitacao_"+codigo.ToString()+".zip");
                
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Solicitação.");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadRetornos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                if (codigo == 0)
                    return Json<bool>(false, false, "Código solicitação invalida.");

                Repositorio.SolicitacaoEmissao repSolicitacaoEmissao = new Repositorio.SolicitacaoEmissao(unitOfWork);
                Dominio.Entidades.SolicitacaoEmissao solicitacaoEmissao = repSolicitacaoEmissao.BuscaPorCodigo(codigo);

                if (solicitacaoEmissao == null)
                    return Json<bool>(false, false, "Solicitação não encontrada.");

                if (solicitacaoEmissao.Status != Dominio.Enumeradores.StatusSolicitacaoEmissao.Finalizado)
                    return Json<bool>(false, false, "Solicitação não está finalizada para download do retorno.");

                return Arquivo(this.ObterArquivos(codigo, "Retorno"), "application/zip", "RetornoSolicitacao_" + codigo.ToString() + ".zip");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Solicitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        public System.IO.MemoryStream ObterArquivos(int codigo, string pasta)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoSolicitacaoEmissao"], codigo.ToString(), pasta);

            IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho);

            MemoryStream fZip = new MemoryStream();
            using (ZipOutputStream zipOStream = new ZipOutputStream(fZip))
            {
                zipOStream.SetLevel(9);

                foreach (string fileinfo in arquivos)
                {
                    string fileName = Utilidades.IO.FileStorageService.Storage.Combine(caminho, System.IO.Path.GetFileName(fileinfo));

                    byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileName);

                    ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(fileName));

                    entry.DateTime = DateTime.Now;

                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(arquivo, 0, arquivo.Length);
                    zipOStream.CloseEntry();
                }


                zipOStream.IsStreamOwner = false;
                zipOStream.Close();
            }


            fZip.Position = 0;

            return fZip;
        }

        #endregion

    }
}
