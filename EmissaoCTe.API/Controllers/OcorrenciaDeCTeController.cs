using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class OcorrenciaDeCTeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao(string arquivo = null)
        {
            if (string.IsNullOrWhiteSpace(arquivo))
                arquivo = "ocorrenciasdectes.aspx";
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals(arquivo) select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string descricaoTipoOcorrencia = Request.Params["DescricaoTipoOcorrencia"];
                string observacaoOcorrencia = Request.Params["ObservacaoOcorrencia"];
                string numeroNF = Request.Params["NumeroNF"];

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.OcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);

                List<Dominio.Entidades.OcorrenciaDeCTe> listaOcorrenciaDeCTe = repOcorrenciaDeCTe.Consultar(this.EmpresaUsuario.Codigo, descricaoTipoOcorrencia, observacaoOcorrencia, numeroNF, inicioRegistros, 50);
                int countOcorrenciaDeCTe = repOcorrenciaDeCTe.ContarConsulta(this.EmpresaUsuario.Codigo, descricaoTipoOcorrencia, observacaoOcorrencia, numeroNF);

                var retorno = (from obj in listaOcorrenciaDeCTe select new { obj.Codigo, DataDaOcorrencia = obj.DataDaOcorrencia.ToString("dd/MM/yyyy HH:mm"), DescricaoCTe = string.Concat(obj.CTe.Numero, " - ", obj.CTe.Serie.Numero), DescricaoOcorrencia = obj.Ocorrencia.Descricao, obj.Ocorrencia.DescricaoTipo }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "CT-e|15", "Ocorrência|45", "Tipo da Ocorrência|15" }, countOcorrenciaDeCTe);
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
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.OcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                Repositorio.OcorrenciaDeCTeAnexos repOcorrenciaDeCTeAnexos = new Repositorio.OcorrenciaDeCTeAnexos(unitOfWork);

                Dominio.Entidades.OcorrenciaDeCTe ocorrencia = repOcorrenciaDeCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                List<Dominio.Entidades.OcorrenciaDeCTeAnexos> anexos = repOcorrenciaDeCTeAnexos.BuscarPorOcorrencia(codigo);

                if (ocorrencia != null)
                {
                    var retorno = new
                    {
                        ocorrencia.Codigo,
                        CodigoCTe = ocorrencia.CTe.Codigo,
                        DescricaoCTe = string.Concat(ocorrencia.CTe.Numero, "-", ocorrencia.CTe.Serie.Numero),
                        CodigoTipoOcorrencia = ocorrencia.Ocorrencia.Codigo,
                        DescricaoTipoOcorrencia = ocorrencia.Ocorrencia.Descricao,
                        DataDaOcorrencia = ocorrencia.DataDaOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                        ocorrencia.Observacao,
                        Anexos = (from anexo in anexos select RetornaDynAnexo(anexo)).ToList()
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Ocorrência não encontrada.");
                }
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
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão negada.");

                int codigoCTe, codigoTipoOcorrencia = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoTipoOcorrencia"], out codigoTipoOcorrencia);
                DateTime dataDaOcorrencia;
                DateTime.TryParseExact(Request.Params["DataDaOcorrencia"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataDaOcorrencia);
                string observacao = Request.Params["Observacao"];

                if (dataDaOcorrencia == DateTime.MinValue)
                    return Json<bool>(false, false, "Data da ocorrência inválida.");
                if (codigoCTe <= 0)
                    return Json<bool>(false, false, "CT-e inválido.");
                if (codigoTipoOcorrencia <= 0)
                    return Json<bool>(false, false, "Ocorrência inválida.");

                Repositorio.OcorrenciaDeCTe repOcorrenciaCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                //if (repOcorrenciaCTe.ContarPorCTeETipoDeOcorrencia(this.EmpresaUsuario.Codigo, codigoCTe, "F") > 0) //todo:verificar como a regra final, Cesar (23/01/2016) pediu para tirar essa validação para enviar ocorrências para a Natura
                //    return Json<bool>(false, false, "Não é possível salvar esta ocorrência pois já existe uma ocorrência final para este CT-e.");

                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = new Dominio.Entidades.OcorrenciaDeCTe();

                ocorrenciaDeCTe.CTe = repCTe.BuscarPorCodigo(codigoCTe);
                ocorrenciaDeCTe.DataDaOcorrencia = dataDaOcorrencia;
                ocorrenciaDeCTe.DataDeCadastro = DateTime.Now;
                ocorrenciaDeCTe.Observacao = observacao;
                ocorrenciaDeCTe.Ocorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);

                repOcorrenciaCTe.Inserir(ocorrenciaDeCTe);

                return Json(new
                {
                    Codigo = ocorrenciaDeCTe.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a ocorrência de CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult GerarEmLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string arquivoPermissao = "ocorrenciasdecteslote.aspx";
                if (this.Permissao(arquivoPermissao) == null || this.Permissao(arquivoPermissao).PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão negada.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int[] codigosCTe = JsonConvert.DeserializeObject<int[]>(Request.Params["CodigoCTes"]);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigosCTe);

                int codigoTipoOcorrencia = 0;
                int.TryParse(Request.Params["CodigoTipoOcorrencia"], out codigoTipoOcorrencia);

                DateTime dataDaOcorrencia;
                DateTime.TryParseExact(Request.Params["DataDaOcorrencia"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataDaOcorrencia);

                string observacao = Request.Params["Observacao"];

                if (dataDaOcorrencia == DateTime.MinValue)
                    return Json<bool>(false, false, "Data da ocorrência inválida.");

                if (ctes == null)
                    return Json<bool>(false, false, "CT-e inválido.");

                if (codigoTipoOcorrencia <= 0)
                    return Json<bool>(false, false, "Ocorrência inválida.");

                Repositorio.OcorrenciaDeCTe repOcorrenciaCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                Dominio.Entidades.TipoDeOcorrenciaDeCTe ocorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);

                for (var i = 0; i < ctes.Count(); i++)
                {
                    Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = new Dominio.Entidades.OcorrenciaDeCTe();
                    ocorrenciaDeCTe.CTe = ctes[i];
                    ocorrenciaDeCTe.DataDaOcorrencia = dataDaOcorrencia;
                    ocorrenciaDeCTe.DataDeCadastro = DateTime.Now;
                    ocorrenciaDeCTe.Observacao = observacao;
                    ocorrenciaDeCTe.Ocorrencia = ocorrencia;

                    repOcorrenciaCTe.Inserir(ocorrenciaDeCTe);
                }


                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar as ocorrências de CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe, inicioRegistros = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.OcorrenciaDeCTe repOcorrenciaCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                List<Dominio.Entidades.OcorrenciaDeCTe> listaOcorrencias = repOcorrenciaCTe.ConsultarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe, inicioRegistros, 50);
                int countOcorrencias = repOcorrenciaCTe.ContarConsultaPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);

                var retorno = (from obj in listaOcorrencias
                              select new
                              {
                                  obj.Codigo,
                                  DataDaOcorrencia = obj.DataDaOcorrencia.ToString("dd/MM/yyyy"),
                                  DescricaoOcorrencia = obj.Ocorrencia.Descricao,
                                  DescricaoTipoOcorrencia = obj.Ocorrencia.DescricaoTipo,
                                  obj.Observacao,
                              }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "Ocorrência|30", "Tipo|15", "Observação|40" }, countOcorrencias);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as ocorrências para este CT-e.");
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
                Repositorio.OcorrenciaDeCTe repOcorrenciaCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                Repositorio.OcorrenciaDeCTeAnexos repOcorrenciaDeCTeAnexos = new Repositorio.OcorrenciaDeCTeAnexos(unitOfWork);

                Dominio.Entidades.OcorrenciaDeCTe ocorencia = repOcorrenciaCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoOcorrencia);

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
                Dominio.Entidades.OcorrenciaDeCTeAnexos anexo = new Dominio.Entidades.OcorrenciaDeCTeAnexos();

                anexo.Ocorrencia = ocorencia;
                anexo.NomeArquivo = file.FileName;
                anexo.GuidArquivo = guidAqruivo;

                repOcorrenciaDeCTeAnexos.Inserir(anexo);

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
                Repositorio.OcorrenciaDeCTeAnexos repOcorrenciaDeCTeAnexos = new Repositorio.OcorrenciaDeCTeAnexos(unitOfWork);
                Dominio.Entidades.OcorrenciaDeCTeAnexos anexo = repOcorrenciaDeCTeAnexos.BuscarPorCodigoEOcorrencia(codigo, codigoOcorrencia, this.EmpresaUsuario.Codigo);

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
                Repositorio.OcorrenciaDeCTeAnexos repOcorrenciaDeCTeAnexos = new Repositorio.OcorrenciaDeCTeAnexos(unitOfWork);
                Dominio.Entidades.OcorrenciaDeCTeAnexos anexo = repOcorrenciaDeCTeAnexos.BuscarPorCodigoEOcorrencia(codigo, codigoOcorrencia, this.EmpresaUsuario.Codigo);

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
                repOcorrenciaDeCTeAnexos.Deletar(anexo);

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

        [AcceptVerbs("POST")]
        public ActionResult EnviarEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.OcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                Repositorio.OcorrenciaDeCTeAnexos repOcorrenciaDeCTeAnexos = new Repositorio.OcorrenciaDeCTeAnexos(unitOfWork);

                Dominio.Entidades.OcorrenciaDeCTe ocorrencia = repOcorrenciaDeCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                List<Dominio.Entidades.OcorrenciaDeCTeAnexos> anexosOcorrencia = repOcorrenciaDeCTeAnexos.BuscarPorOcorrencia(codigo);

                if (ocorrencia != null)
                {
                    Servicos.Email serEmail = new Servicos.Email(unitOfWork);

                    string assunto = "Ocorrência CT-e " + ocorrencia.CTe.Numero + " transportadora " + ocorrencia.CTe.Empresa.RazaoSocial;

                    string emails = string.Empty;
                    string emailsCopia = string.Empty;

                    emails = ocorrencia.CTe.Remetente.Email;
                    if (!string.IsNullOrWhiteSpace(ocorrencia.CTe.Remetente.EmailTransportador))
                    {
                        if (!string.IsNullOrWhiteSpace(emails))
                            emails = string.Concat(emails, ";", ocorrencia.CTe.Remetente.EmailTransportador);
                        else
                            emails = ocorrencia.CTe.Remetente.EmailTransportador;
                    }

                    emailsCopia = ocorrencia.CTe.Empresa.Email;

                    if (ocorrencia.CTe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                    {
                        emails = ocorrencia.CTe.Empresa.Email;
                        emailsCopia = "";
                    }

                    string[] listaEmails = emails.Split(';');
                    string[] listaEmailsCopia = emailsCopia.Split(';');

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("<p>Prezado cliente ").Append(ocorrencia.CTe.Remetente.Nome).Append("<br /><br />");
                    sb.Append("Segue dados do lançamento da ocorrência:").Append("<br />");
                    sb.Append("Transportadora: " + ocorrencia.CTe.Empresa.CNPJ_Formatado + " " + ocorrencia.CTe.Empresa.RazaoSocial).Append("<br />");
                    sb.Append("CT-e: " + ocorrencia.CTe.Numero + "/" + ocorrencia.CTe.Serie.Numero).Append("<br />");
                    sb.Append("Ocorrência: " + ocorrencia.Ocorrencia.Descricao + " " + ocorrencia.DataDaOcorrencia.ToString("dd/MM/yyyy")).Append("<br />");
                    sb.Append("Observação: " + ocorrencia.Observacao).Append("<br /><br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("Favor não responder este e-mail.<br />");
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br /><br />");
                    ss.Append("E-mail enviado para:<br />");
                    if (!string.IsNullOrWhiteSpace(emails))
                        ss.Append(emails + "<br />");
                    if (!string.IsNullOrWhiteSpace(emailsCopia))
                        ss.Append(emailsCopia + "<br />");

                    List<Attachment> anexos = null;
                    if (anexosOcorrencia.Count > 0)
                    {
                        anexos = new List<Attachment>();

                        foreach (Dominio.Entidades.OcorrenciaDeCTeAnexos anexoOcorrencia in anexosOcorrencia)
                        {
                            string extensao = System.IO.Path.GetExtension(anexoOcorrencia.NomeArquivo).ToLower();
                            string caminho = this.CaminhoArquivo();
                            string arquivoFisico = anexoOcorrencia.GuidArquivo + extensao;

                            arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                            if (Utilidades.IO.FileStorageService.Storage.Exists(arquivoFisico))
                            {
                                Attachment anexo = new Attachment(arquivoFisico);
                                anexo.Name = System.IO.Path.GetFileName(anexoOcorrencia.NomeArquivo);
                                anexos.Add(anexo);
                            }
                        }
                    }

                    string emailResposta = listaEmailsCopia.FirstOrDefault();

                    foreach (string email in listaEmails)
                    {
                        if (Utilidades.Email.ValidarEmails(email, ';'))
                            serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email, "", "", assunto, sb.ToString(), string.Empty, anexos, ss.ToString(), false, emailResposta, 0, unitOfWork);
                    }

                    foreach (string email in listaEmailsCopia)
                    {
                        if (Utilidades.Email.ValidarEmails(email, ';'))
                            serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email, "", "", assunto, sb.ToString(), string.Empty, anexos, ss.ToString(), false, emailResposta, 0, unitOfWork);
                    }

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Ocorrência não encontrada.");
                }
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
        public ActionResult SelecionarTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);

                string tipoOcorrencia = Request.Params["TipoOcorrencia"];
                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.ConsultaCTe> listaCTes = repCTe.Consultar(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, tipoOcorrencia, string.Empty, false, null, 0, 2000);
                int countCTes = repCTe.ContarConsulta(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, tipoOcorrencia, string.Empty, false, null);

                var retorno = (from cte in listaCTes
                               select new
                               {
                                   cte.Codigo,
                                   Numero = cte.Numero.ToString(),
                                   ValorFrete = cte.Valor.ToString("n2")
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao selecionar os CT-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        private string CaminhoArquivo()
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"], "Anexos", "Ocorrencia");
        }
        private dynamic RetornaDynAnexo(Dominio.Entidades.OcorrenciaDeCTeAnexos anexo)
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
