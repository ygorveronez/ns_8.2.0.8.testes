using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/MensagemAviso")]
    public class MensagemAvisoAnexoController : BaseController
    {
        public ActionResult AnexarArquivos()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAvisoAnexo repositorioMensagemAvisoAnexo = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAvisoAnexo(unitOfWork);
                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso repositorioMensagemAviso = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAviso(unitOfWork);

                AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso mensagemAviso = repositorioMensagemAviso.BuscarPorCodigo(codigo);

                if (mensagemAviso == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                IList<Servicos.DTO.CustomFile> arquivos = new List<Servicos.DTO.CustomFile>();
                var files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        arquivos.Add(new Servicos.DTO.CustomFile(
                            files.GetKey(i),
                            file.FileName,
                            file.ContentType,
                            file.ContentLength,
                            file.InputStream
                        ));
                    }
                }

                string[] descricoes = Request.Params.GetValues("Descricao") ?? new string[0];

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                for (int i = 0; i < arquivos.Count; i++)
                {
                    var arquivo = arquivos[i];

                    long arquivoLength = 0;
                    if (arquivo.InputStream != null && arquivo.InputStream.CanSeek)
                    {
                        arquivoLength = arquivo.InputStream.Length;
                    }
                    else
                    {
                        try
                        {
                            var pos = arquivo.InputStream.Position;
                            using (var ms = new MemoryStream())
                            {
                                arquivo.InputStream.Seek(0, SeekOrigin.Begin);
                                arquivo.InputStream.CopyTo(ms);
                                arquivoLength = ms.Length;
                                arquivo.InputStream.Seek(pos, SeekOrigin.Begin);
                            }
                        }
                        catch
                        {
                            arquivoLength = 0;
                        }
                    }
                    if (arquivoLength > (5 * 1024 * 1024))
                        return new JsonpResult(false, true, "O arquivo não pode ser maior que 5 MB.");

                    string extensaoArquivo = Path.GetExtension(arquivo.FileName)?.ToLower() ?? string.Empty;
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                    byte[] conteudoArquivo;
                    using (var memoryStream = new MemoryStream())
                    {
                        arquivo.InputStream.Seek(0, SeekOrigin.Begin);
                        arquivo.InputStream.CopyTo(memoryStream);
                        conteudoArquivo = memoryStream.ToArray();
                    }

                    string nomeArquivo = RemoverCaracteresEspeciais(RemoveDiacritics(Path.GetFileName(arquivo.FileName)));

                    AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo anexo = new AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo()
                    {
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                        GuidArquivo = guidArquivo,
                        NomeArquivo = nomeArquivo,
                        Extensao = extensaoArquivo,
                        Arquivo = conteudoArquivo
                    };

                    PreecherInformacoesAdicionais(anexo, mensagemAviso);

                    repositorioMensagemAvisoAnexo.Inserir(anexo);
                }

                List<AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo> anexos = repositorioMensagemAvisoAnexo.BuscarTodos()
                    .Where(x => x.MensagemAviso != null && x.MensagemAviso.Codigo == mensagemAviso.Codigo)
                    .ToList();

                var listaDinamicaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                        anexo.Extensao,
                        TipoAnexo = new { Codigo = 0, Descricao = string.Empty }
                    }
                ).ToList();

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult DownloadAnexo()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAvisoAnexo repositorioMensagemAvisoAnexo = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAvisoAnexo(unitOfWork);

                int codigo;
                string guidArquivo = Request.Params["GuidArquivo"];
                AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo anexo = null;

                if (!string.IsNullOrEmpty(guidArquivo))
                {
                    anexo = repositorioMensagemAvisoAnexo.BuscarPorGuid(guidArquivo);
                }
                else if (int.TryParse(Request.Params["Codigo"], out codigo))
                {
                    anexo = repositorioMensagemAvisoAnexo.BuscarPorCodigo(codigo);
                }

                if (anexo == null || anexo.Arquivo == null || anexo.Arquivo.Length == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");

                string mimeType = "application/octet-stream";
                if (!string.IsNullOrEmpty(anexo.NomeArquivo))
                    mimeType = MimeMapping.GetMimeMapping(anexo.NomeArquivo);

                return Arquivo(anexo.Arquivo, mimeType, anexo.NomeArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult ExcluirAnexo()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                AdminMultisoftware.Repositorio.MensagemAviso.MensagemAvisoAnexo repositorioMensagemAvisoAnexo = new AdminMultisoftware.Repositorio.MensagemAviso.MensagemAvisoAnexo(unitOfWork);
                AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo anexo = repositorioMensagemAvisoAnexo.BuscarPorCodigo(codigo);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (anexo.MensagemAviso == null)
                    return new JsonpResult(false, true, "Não é permitido excluir o anexo.");

                repositorioMensagemAvisoAnexo.Deletar(anexo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (var c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        private string RemoverCaracteresEspeciais(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var sb = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '-')
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private void PreecherInformacoesAdicionais(
            AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo anexo,
            AdminMultisoftware.Dominio.Entidades.MensagemAviso.MensagemAviso mensagemAviso)
        {
            anexo.MensagemAviso = mensagemAviso;
        }
    }
}
