using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ArquivoTransportadorController : ApiController
    {
        #region Métodos Globais
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ArquivoTransportador repArquivoTransportador = new Repositorio.ArquivoTransportador(unitOfWork);

                DateTime.TryParse(Request.Params["DataInicial"], out DateTime dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out DateTime dataFinal);

                string descricao = Request.Params["Descricao"] ?? string.Empty;

                bool? status = null;
                if (bool.TryParse(Request.Params["Status"], out bool statusAux))
                    status = statusAux;

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                List<Dominio.Entidades.ArquivoTransportador> arquivos = repArquivoTransportador.Consultar(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, descricao, status, inicioRegistros, 50);
                int count = repArquivoTransportador.ContarConsulta(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, descricao, status);

                var lista = from obj in arquivos
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                Data = obj.Data.ToString("dd/MM/yyyy"),
                                Status = obj.DescricaoAtivo
                            };

                return Json(lista, true, null, new string[] { "Código", "Descrição|43", "Data|25", "Status|25" }, count);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult InserirArquivo()
        {
            Repositorio.UnitOfWork unidadeDeTarabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ArquivoTransportador repArquivoTransportador = new Repositorio.ArquivoTransportador(unidadeDeTarabalho);

                string[] extensoesValidas = { ".jpg", ".png", ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".txt" };

                int.TryParse(Request.Params["Codigo"], out int codigo);
                Dominio.Entidades.ArquivoTransportador arquivo = repArquivoTransportador.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (Request.Files.Count == 0)
                    return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");

                if (arquivo == null)
                    return Json<bool>(false, false, "Não foi possível buscar os dados.");

                // Converte arquivo upado
                System.Web.HttpPostedFileBase file = Request.Files[0];

                // Valida
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!extensoesValidas.Contains(extensao))
                    return Json<bool>(false, false, "Extensão " + extensao.Substring(1) + " inválida.");
                
                arquivo.Log += "; Arquivo Atualizado por " + this.Usuario.Nome + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                // Cria Entidade e insere
                arquivo.NomeArquivo = file.FileName;
                if (arquivo.NomeArquivo.Length > 45)
                {
                    string auxNome = arquivo.NomeArquivo.Replace(extensao, "");
                    auxNome = auxNome.Substring(0, (auxNome.Length - extensao.Length)) + extensao;
                    arquivo.NomeArquivo = auxNome;
                }


                // Salva na pasta configurada
                string caminho = this.CaminhoArquivos();
                string arquivoFisico = arquivo.Codigo.ToString() + extensao;
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivoFisico))
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivoFisico);

                Utilidades.IO.FileStorageService.Storage.SaveStream(arquivoFisico, file.InputStream);
                
                arquivo.CaminhoArquivo = arquivoFisico;

                unidadeDeTarabalho.Start();
                repArquivoTransportador.Atualizar(arquivo);
                unidadeDeTarabalho.CommitChanges();

                // Retorna informacoes
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTarabalho.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");
            }
            finally
            {
                unidadeDeTarabalho.Dispose();
            }
        }
        
        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ArquivoTransportador repArquivoTransportador = new Repositorio.ArquivoTransportador(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.ArquivoTransportador arquivo = repArquivoTransportador.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (arquivo == null)
                    return Json<bool>(false, false, "Não foi possível buscar os dados.");
                
                return Json(new
                {
                    Codigo = arquivo.Codigo,
                    arquivo.Descricao,
                    Data = arquivo.Data.ToString("dd/MM/yyyy"),
                    arquivo.Log,
                    Satus = arquivo.Ativo.ToString().ToLower(),
                    PossuiArquivo = !string.IsNullOrWhiteSpace(arquivo.CaminhoArquivo)
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao bsucar dados.");
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
                Repositorio.ArquivoTransportador repArquivoTransportador = new Repositorio.ArquivoTransportador(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.ArquivoTransportador arquivo = repArquivoTransportador.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (arquivo == null && codigo > 0)
                    return Json<bool>(false, false, "Não foi possível buscar os dados.");

                if (arquivo == null)
                    arquivo = new Dominio.Entidades.ArquivoTransportador();

                PreencherEntidade(ref arquivo);

                if(arquivo.Codigo == 0)
                {
                    arquivo.Log = "Inserido por " + this.Usuario.Nome + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    repArquivoTransportador.Inserir(arquivo);
                }
                else
                {
                    arquivo.Log += "; Atualizado por " + this.Usuario.Nome + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    repArquivoTransportador.Atualizar(arquivo);
                }

                return Json(new
                {
                    Codigo = arquivo.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ArquivoTransportador repArquivoTransportador = new Repositorio.ArquivoTransportador(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.ArquivoTransportador arquivo = repArquivoTransportador.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (arquivo == null)
                    return Json<bool>(false, false, "Não foi possível encontrar o arquivo.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo.CaminhoArquivo))
                    return Json<bool>(false, false, "O arquivo não existe.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo.CaminhoArquivo), MimeMapping.GetMimeMapping(arquivo.CaminhoArquivo), arquivo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do log.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        private string CaminhoArquivos()
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"], "ArquivosTransportador");

            return caminho;
        }

        private void PreencherEntidade(ref Dominio.Entidades.ArquivoTransportador arquivo)
        {
            DateTime.TryParse(Request.Params["Data"], out DateTime dataFinal);
            string descricao = Request.Params["Descricao"] ?? string.Empty;
            bool.TryParse(Request.Params["Status"], out bool ativo);

            arquivo.Data = dataFinal;
            arquivo.Descricao = descricao;
            arquivo.Ativo = ativo;

            if(arquivo.Codigo == 0)
                arquivo.Empresa = this.EmpresaUsuario;
        }
    }
}