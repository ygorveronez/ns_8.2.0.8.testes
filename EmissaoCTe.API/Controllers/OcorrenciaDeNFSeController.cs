using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class OcorrenciaDeNFSeController : ApiController
    {
        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("ocorrenciasdenfse.aspx") select obj).FirstOrDefault();
        }


        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string descricaoTipoOcorrencia = Request.Params["DescricaoTipoOcorrencia"];
                string observacaoOcorrencia = Request.Params["ObservacaoOcorrencia"];

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                int numeroNFSe = 0;
                int.TryParse(Request.Params["NumeroNFSe"], out numeroNFSe);

                Repositorio.OcorrenciaDeNFSe repOcorrenciaDeNFSe = new Repositorio.OcorrenciaDeNFSe(unitOfWork);

                List<Dominio.Entidades.OcorrenciaDeNFSe> itens = repOcorrenciaDeNFSe.Consultar(this.EmpresaUsuario.Codigo, descricaoTipoOcorrencia, observacaoOcorrencia, numeroNFSe, inicioRegistros, 50);
                int totalRegistros = repOcorrenciaDeNFSe.ContarConsulta(this.EmpresaUsuario.Codigo, descricaoTipoOcorrencia, observacaoOcorrencia, numeroNFSe);

                var retorno = (from obj in itens select 
                              new {
                                  obj.Codigo,
                                  DataDaOcorrencia = obj.DataDaOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                                  NFSe = string.Concat(obj.NFSe.Numero, " - ", obj.NFSe.Serie.Numero),
                                  DescricaoOcorrencia = obj.Ocorrencia.Descricao,
                                  obj.Ocorrencia.DescricaoTipo
                              }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "NFS-e|15", "Ocorrência|35", "Tipo da Ocorrência|20" }, totalRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as ocorrências.");
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
                Repositorio.OcorrenciaDeNFSe repOcorrenciaDeNFSe = new Repositorio.OcorrenciaDeNFSe(unitOfWork);
                Repositorio.OcorrenciaDeNFSeAnexos repOcorrenciaDeNFSeAnexos = new Repositorio.OcorrenciaDeNFSeAnexos(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Dominio.Entidades.OcorrenciaDeNFSe ocorrencia = repOcorrenciaDeNFSe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                List<Dominio.Entidades.OcorrenciaDeNFSeAnexos> anexos = repOcorrenciaDeNFSeAnexos.BuscarPorOcorrencia(codigo);

                if (ocorrencia == null)
                    return Json<bool>(false, false, "Ocorrência não encontrada.");

                var retorno = new
                {
                    ocorrencia.Codigo,
                    CodigoNFSe = ocorrencia.NFSe.Codigo,
                    DescricaoNFSe = string.Concat(ocorrencia.NFSe.Numero, "-", ocorrencia.NFSe.Serie.Numero),
                    CodigoTipoOcorrencia = ocorrencia.Ocorrencia.Codigo,
                    DescricaoTipoOcorrencia = ocorrencia.Ocorrencia.Descricao,
                    DataDaOcorrencia = ocorrencia.DataDaOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                    ocorrencia.Observacao,
                    Anexos = (from anexo in anexos select RetornaDynAnexo(anexo)).ToList()
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da ocorrência.");
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
                Repositorio.OcorrenciaDeNFSe repOcorrenciaNFSe = new Repositorio.OcorrenciaDeNFSe(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão negada.");

                int codigoNFSe, codigoTipoOcorrencia = 0;
                int.TryParse(Request.Params["CodigoNFSe"], out codigoNFSe);
                int.TryParse(Request.Params["CodigoTipoOcorrencia"], out codigoTipoOcorrencia);

                DateTime dataDaOcorrencia;
                DateTime.TryParseExact(Request.Params["DataDaOcorrencia"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataDaOcorrencia);
                string observacao = Request.Params["Observacao"];

                if (dataDaOcorrencia == DateTime.MinValue)
                    return Json<bool>(false, false, "Data da ocorrência inválida.");

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);
                if (nfse == null)
                    return Json<bool>(false, false, "NFS-e inválido.");

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);
                if (tipoOcorrencia == null)
                    return Json<bool>(false, false, "Ocorrência inválida.");

                Dominio.Entidades.OcorrenciaDeNFSe ocorrenciaDeNFSe = new Dominio.Entidades.OcorrenciaDeNFSe();

                ocorrenciaDeNFSe.NFSe = nfse;
                ocorrenciaDeNFSe.DataDaOcorrencia = dataDaOcorrencia;
                ocorrenciaDeNFSe.DataDeCadastro = DateTime.Now;
                ocorrenciaDeNFSe.Observacao = observacao;
                ocorrenciaDeNFSe.Ocorrencia = tipoOcorrencia;

                repOcorrenciaNFSe.Inserir(ocorrenciaDeNFSe);

                return Json(new
                {
                    Codigo = ocorrenciaDeNFSe.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a ocorrência de NFS-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoNFSe, inicioRegistros = 0;
                int.TryParse(Request.Params["CodigoNFSe"], out codigoNFSe);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.OcorrenciaDeNFSe repOcorrenciaDeNFSe = new Repositorio.OcorrenciaDeNFSe(unitOfWork);
                List<Dominio.Entidades.OcorrenciaDeNFSe> listaOcorrencias = repOcorrenciaDeNFSe.ConsultarPorNFSe(this.EmpresaUsuario.Codigo, codigoNFSe, inicioRegistros, 50);
                int countOcorrencias = repOcorrenciaDeNFSe.ContarConsultaPorNFSe(this.EmpresaUsuario.Codigo, codigoNFSe);

                var retorno = from obj in listaOcorrencias
                              select new
                              {
                                  obj.Codigo,
                                  DataDaOcorrencia = obj.DataDaOcorrencia.ToString("dd/MM/yyyy"),
                                  DescricaoOcorrencia = obj.Ocorrencia.Descricao,
                                  DescricaoTipoOcorrencia = obj.Ocorrencia.DescricaoTipo,
                                  obj.Observacao,
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "Ocorrência|30", "Tipo|15", "Observação|40" }, countOcorrencias);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as ocorrências para esta NFS-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Anexar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Config extensoes validas
                string[] extensoesValidas = { ".pdf", ".jpg", ".jpeg", ".png" };

                // Converter dados
                int codigoOcorrencia = 0;
                int.TryParse(Request.Params["Codigo"], out codigoOcorrencia);

                // Busca entidades
                Repositorio.OcorrenciaDeNFSe repOcorrenciaNFSe = new Repositorio.OcorrenciaDeNFSe(unitOfWork);
                Repositorio.OcorrenciaDeNFSeAnexos repOcorrenciaDeNFSeAnexos = new Repositorio.OcorrenciaDeNFSeAnexos(unitOfWork);

                Dominio.Entidades.OcorrenciaDeNFSe ocorencia = repOcorrenciaNFSe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoOcorrencia);

                if (ocorencia == null)
                    return Json<bool>(false, false, "Ocorrência não encontrada.");

                if (Request.Files.Count == 0)
                    return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");

                // Manipula arquivo
                HttpPostedFileBase file = Request.Files[0];

                // Valida extensao
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!extensoesValidas.Contains(extensao))
                    return Json<bool>(false, false, "Extensão " + extensao.Substring(1) + " inválida.");

                // Inicia instancia
                unitOfWork.Start();

                // Insere
                string guidAqruivo = Guid.NewGuid().ToString().Replace("-", "");
                Dominio.Entidades.OcorrenciaDeNFSeAnexos anexo = new Dominio.Entidades.OcorrenciaDeNFSeAnexos();

                anexo.Ocorrencia = ocorencia;
                anexo.NomeArquivo = file.FileName;
                anexo.GuidArquivo = guidAqruivo;

                repOcorrenciaDeNFSeAnexos.Inserir(anexo);

                // Salva na pasta configurada
                string caminho = this.CaminhoArquivo();
                string arquivoFisico = guidAqruivo + extensao;
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                Utilidades.IO.FileStorageService.Storage.SaveStream(arquivoFisico, file.InputStream);
                
                // Fecha instancia
                unitOfWork.CommitChanges();

                return Json(RetornaDynAnexo(anexo), true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET")]
        public ActionResult DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Converter dados
                int codigo = 0;
                int codigoOcorrencia = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Ocorrencia"], out codigoOcorrencia);

                // Busca entidades
                Repositorio.OcorrenciaDeNFSeAnexos repOcorrenciaDeNFSeAnexos = new Repositorio.OcorrenciaDeNFSeAnexos(unitOfWork);
                Dominio.Entidades.OcorrenciaDeNFSeAnexos anexo = repOcorrenciaDeNFSeAnexos.BuscarPorCodigoEOcorrencia(codigo, codigoOcorrencia, this.EmpresaUsuario.Codigo);

                if (anexo == null)
                    return Json<bool>(false, false, "Anexo não encontrado.");

                // Busca arquivo fisico
                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string caminho = this.CaminhoArquivo();
                string arquivoFisico = anexo.GuidArquivo + extensao;

                // Monta caminho absoluto
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                // Arquivo fisico nao existe
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoFisico))
                    return Json<bool>(false, false, "Anexo não encontrado.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoFisico), MimeMapping.GetMimeMapping(arquivoFisico), anexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Converter dados
                int codigo = 0;
                int codigoOcorrencia = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Ocorrencia"], out codigoOcorrencia);

                // Busca entidades
                Repositorio.OcorrenciaDeNFSeAnexos repOcorrenciaDeNFSeAnexos = new Repositorio.OcorrenciaDeNFSeAnexos(unitOfWork);
                Dominio.Entidades.OcorrenciaDeNFSeAnexos anexo = repOcorrenciaDeNFSeAnexos.BuscarPorCodigoEOcorrencia(codigo, codigoOcorrencia, this.EmpresaUsuario.Codigo);

                if (anexo == null)
                    return Json<bool>(false, false, "Anexo não encontrado.");

                // Busca arquivo fisico
                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string caminho = this.CaminhoArquivo();
                string arquivoFisico = anexo.GuidArquivo + extensao;

                // Monta caminho absoluto
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                // Inicia instancia
                unitOfWork.Start();

                // Deleta registro
                repOcorrenciaDeNFSeAnexos.Deletar(anexo);

                // Deleta o arquivo fisico
                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivoFisico))
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivoFisico);

                // Fecha instancia
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return Json("Arquivo excluído com sucesso.", true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao excluir o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        #endregion

        private string CaminhoArquivo()
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"], "Anexos", "Ocorrencia");

            return caminho;
        }

        private dynamic RetornaDynAnexo(Dominio.Entidades.OcorrenciaDeNFSeAnexos anexo)
        {
            return new
            {
                Codigo = anexo.Codigo,
                Nome = anexo.NomeArquivo,
                Ocorrencia = anexo.Ocorrencia.Codigo
            };
        }
    }
}