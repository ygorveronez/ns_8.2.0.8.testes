using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class AtendimentoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("atendimento.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);

                string descricao = Request.Params["Descricao"] ?? string.Empty;

                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["Empresa"], out int empresa);

                string nomeEmpresa = Request.Params["NomeEmpresa"] ?? string.Empty;
                string statusStr = (Request.Params["Status"] ?? string.Empty).ToLower();
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status = null;

                if (statusStr.Contains("fech"))
                    status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado;
                else if (statusStr.Contains("abe"))
                    status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto;

                int codigoUsuario = 0;

                List<Dominio.Entidades.Embarcador.Atendimento.Atendimento> lista = repAtendimento.ConsultarAdminCTe(this.EmpresaUsuario.Codigo, codigoUsuario, empresa, nomeEmpresa, dataInicial, dataFinal, descricao, status, inicioRegistros, 50);
                int totalRegistros = repAtendimento.ContarConsultaAdminCTe(this.EmpresaUsuario.Codigo, codigoUsuario, empresa, nomeEmpresa, dataInicial, dataFinal, descricao, status);

                var retorno = (from obj in lista
                               select new {
                                   obj.Codigo,
                                   Numero = obj.Numero,
                                   Autor = obj.Funcionario?.Nome ?? string.Empty,
                                   Empresa = obj.EmpresaFilho.RazaoSocial,
                                   Data = obj.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   Status = obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado ? "Fechado" : obj.DescricaoStatus,
                                   Descricao = obj.ObservacaoSuporte
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Número|10", "Autor|15", "Empresa|20", "Data|15", "Status|15", "Descricao|15" }, totalRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os atendimentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public ActionResult RelatorioAtendimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repAtendimentoTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                DateTime.TryParse(Request.Params["DataInicial"], out DateTime dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out DateTime dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status = null;
                if(Enum.TryParse(Request.Params["Situacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusAux))
                    status = statusAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao = null;
                if (Enum.TryParse(Request.Params["Satisfacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao satisfacaoAux))
                    satisfacao = satisfacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistema? sistema = null;
                if (Enum.TryParse(Request.Params["Sistema"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistema sistemaAux))
                    sistema = sistemaAux;

                Enum.TryParse(Request.Params["TipoRelatorio"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo tipoRelatorio);
                
                int.TryParse(Request.Params["Empresa"], out int empresa);
                int.TryParse(Request.Params["Usuario"], out int codigoUsuario);                
                int.TryParse(Request.Params["TipoAtendimento"], out int tipoAtendimento);

                string tipoArquivo = Request.Params["Arquivo"];

                #region Parametros
                Dominio.Entidades.Empresa empresaPar = repEmpresa.BuscarPorCodigo(empresa);
                Dominio.Entidades.Usuario usuario = null;
                if (codigoUsuario > 0)
                    usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo tipoAtendcimentoPar = repAtendimentoTipo.BuscarPorCodigo(tipoAtendimento);
                Dominio.ObjetosDeValor.Relatorios.RelatorioAtendimento aux = new Dominio.ObjetosDeValor.Relatorios.RelatorioAtendimento();
                if (satisfacao.HasValue) aux.Satisfacao = satisfacao.Value;
                if (status.HasValue) aux.Situacao = status.Value;
                if (sistema.HasValue) aux.Sistema = sistema.Value;

                string titulo = tipoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Emissao ? "Emissão" : "Suporte";

                List<ReportParameter> parametros = new List<ReportParameter>
                {
                    new ReportParameter("Titulo", titulo.ToUpper()),

                    new ReportParameter("Periodo", PeriodoData(dataInicial, dataFinal)),
                    new ReportParameter("Empresa", empresaPar != null ? empresaPar.Descricao : "Todas"),
                    new ReportParameter("TipoAtendimento", tipoAtendcimentoPar != null ? tipoAtendcimentoPar.Descricao : "Todas"),
                    new ReportParameter("Satisfacao", satisfacao.HasValue ? aux.DescricaoSatisfacao : "Todos"),
                    new ReportParameter("Situacao", status.HasValue ? aux.DescricaoStatus : "Todos"),
                    new ReportParameter("Sistema", sistema.HasValue ? aux.DescricaoSistema : "Todos"),
                    new ReportParameter("Usuario", usuario != null ? usuario.Descricao : "Todos"),
                };
                #endregion

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioAtendimento> relatorioAtendimento = repAtendimento.RelatorioAtendimentos(this.EmpresaUsuario.Codigo, codigoUsuario, tipoRelatorio, dataInicial, dataFinal, empresa, tipoAtendimento, satisfacao, status, sistema);

                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Atendimentos", relatorioAtendimento)
                };

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioAtendimento.rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, string.Concat("RelatorioAtendimentos", titulo.Replace("ã", "a"), ".", arquivo.FileNameExtension.ToLower()));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult ConsultarEmissao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                string nomeEmpresa = Request.Params["NomeEmpresa"] ?? string.Empty;
                string descricao = Request.Params["Descricao"] ?? string.Empty;

                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["Empresa"], out int empresa);

                string statusStr = (Request.Params["Status"] ?? string.Empty).ToLower();
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento? status = null;

                if (statusStr.Contains("fech"))
                    status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado;
                else if (statusStr.Contains("abe"))
                    status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto;

                int codigoUsuario = 0;
                if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                    codigoUsuario = this.Usuario.Codigo;

                List<Dominio.Entidades.Embarcador.Atendimento.Atendimento> lista = repAtendimento.ConsultarAdminCTeEmissao(this.EmpresaUsuario.Codigo, codigoUsuario, empresa, nomeEmpresa, dataInicial, dataFinal, descricao, status, inicioRegistros, 50);
                int totalRegistros = repAtendimento.ContarConsultaAdminCTeEmissao(this.EmpresaUsuario.Codigo, codigoUsuario, empresa, nomeEmpresa, dataInicial, dataFinal, descricao, status);

                var retorno = (from obj in lista
                               select new
                               {
                                   obj.Codigo,
                                   Numero = obj.Numero,
                                   Autor = obj.Funcionario?.Nome ?? string.Empty,
                                   Empresa = obj.EmpresaFilho.RazaoSocial,
                                   Data = obj.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   Status = obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado ? "Fechado" : obj.DescricaoStatus,
                                   Descricao = obj.ObservacaoSuporte
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Número|10", "Autor|15", "Empresa|20", "Data|15", "Status|15", "Descricao|15" }, totalRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os atendimentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult AnexoExcluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Atendimento.AtendimentoAnexo repAtendimentoAnexo = new Repositorio.Embarcador.Atendimento.AtendimentoAnexo(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo anexo = repAtendimentoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return Json<bool>(false, false, "Erro ao buscar os dados.");

                if (anexo.Atendimento.Funcionario.Codigo != this.Usuario.Codigo)
                    return Json<bool>(false, false, "Não é possível modificar dados de outros usuários.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivo();
                var extensaoArquivo = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.CaminhoArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return Json<bool>(false, false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.Atendimento, null, "Excluiu o anexo " + anexo.NomeArquivo, unitOfWork);

                // Remove do banco
                repAtendimentoAnexo.Deletar(anexo);

                // Commita
                unitOfWork.CommitChanges();

                return Json<bool>(true, true);
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
        public ActionResult Anexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoAnexo repAtendimentoAnexo = new Repositorio.Embarcador.Atendimento.AtendimentoAnexo(unitOfWork);

                // Validacao permitida
                string[] extensoes = new string[] {".pdf", ".docx", ".doc", ".png", ".jpeg", ".txt", ".xls", ".xlsx", ".xml", ".jpg" };

                // Busca Dados
                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento = repAtendimento.BuscarPorCodigo(codigo);

                // Valida
                if (Request.Files.Count == 0)
                    return Json<bool>(false, false, "Nenhum arquivo recebido.");

                if (atendimento == null)
                    return Json<bool>(false, false, "Erro ao buscar registro.");

                if (atendimento.Funcionario.Codigo != this.Usuario.Codigo)
                    return Json<bool>(false, false, "Não é possível modificar dados de outros usuários.");

                // Extrai dados
                HttpPostedFileBase file = Request.Files[0];
                var nomeArquivo = file.FileName;
                var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = CaminhoArquivo();

                if (!extensoes.Contains(extensaoArquivo))
                    return Json<bool>(false, false, "A extensão " + extensaoArquivo + " não é válida.");

                Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo), file.InputStream);
                
                // Insere no banco
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo anexo = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo()
                {
                    Atendimento = atendimento,
                    CaminhoArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo))),
                    Descricao = string.Empty,
                    Status = true
                };

                repAtendimentoAnexo.Inserir(anexo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.Atendimento, null, "Adicionou o anexo " + anexo.NomeArquivo, unitOfWork);

                // Commita
                unitOfWork.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Atendimento.AtendimentoAnexo repAtendimentoAnexo = new Repositorio.Embarcador.Atendimento.AtendimentoAnexo(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoAnexo anexo = repAtendimentoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return Json<bool>(false, false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivo();
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.CaminhoArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, extencao.Replace(".", ""), anexo.NomeArquivo);
                else
                    return Json<bool> (false, false, "Ocorreu uma falha ao buscar anexo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool> (false, false, "Ocorreu uma falha ao fazer download do anexo.");
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
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoAnexo repAtendimentoAnexo = new Repositorio.Embarcador.Atendimento.AtendimentoAnexo(unitOfWork);
                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento = repAtendimento.BuscarPorCodigo(codigo);

                if(atendimento == null)
                    return Json<bool>(false, false, "Atendimento não encontrado.");
              
                var retorno = new
                {
                    atendimento.Codigo,
                    Numero = atendimento.Numero,
                    Data = atendimento.DataInicial != null ? atendimento.DataInicial.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    Empresa = atendimento.EmpresaFilho != null ? new { atendimento.EmpresaFilho.Codigo, Descricao = atendimento.EmpresaFilho.RazaoSocial } : null,
                    Tipo = atendimento.AtendimentoTipo != null ? new { atendimento.AtendimentoTipo.Codigo,  atendimento.AtendimentoTipo.Descricao } : null,
                    Sistema = atendimento.TipoSistema,
                    Satisfacao = atendimento.NivelSatisfacao,
                    Descricao = atendimento.ObservacaoSuporte,
                    TipoContato = atendimento.TipoContato,
                    Contato = atendimento.ContatoAtendimento,
                    Observacao = atendimento.Observacao,
                    atendimento.Status,
                    Autor = atendimento.Funcionario.Nome,
                    PodeEditar = atendimento.Funcionario.Codigo == this.Usuario.Codigo,
                    atendimento.NecessitouAuxilio,
                    Anexos = from o in repAtendimentoAnexo.BuscarPorAtendimento(atendimento.Codigo)
                             select new
                             {
                                 o.Codigo,
                                 o.NomeArquivo
                             }
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do atendimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult TipoAtendimentoPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repAtendimentoTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo atendimento = repAtendimentoTipo.TipoAtendimentoPadrao(this.EmpresaUsuario.Codigo);

                if (atendimento == null)
                    return Json<object>(null, true);

                return Json(new
                {
                    atendimento.Codigo,
                    atendimento.Descricao,
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter o tipo padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult TipoAtendimentoPadraoEmissao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repAtendimentoTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo atendimento = repAtendimentoTipo.TipoAtendimentoPadraoEmissao(this.EmpresaUsuario.Codigo);

                if (atendimento == null)
                    return Json<object>(null, true);

                return Json(new
                {
                    atendimento.Codigo,
                    atendimento.Descricao,
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter o tipo padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarTipoAtendimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repAtendimentoTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                string descricao = Request.Params["Descricao"] ?? string.Empty;
                string statusStr = Request.Params["Status"] ?? string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;

                if (statusStr == "A")
                    status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                else if (statusStr == "I")
                    status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo;

                List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo> lista = repAtendimentoTipo.ConsultarMultiCTe(this.EmpresaUsuario.Codigo, descricao, status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Suporte, inicioRegistros, 50);
                int totalRegistros = repAtendimentoTipo.ContarConsultaMultiCTe(this.EmpresaUsuario.Codigo, descricao, status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Suporte);

                var retorno = (from obj in lista
                               select new
                               {
                                   obj.Codigo,
                                   Descricao = obj.Descricao
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Descrição|80" }, totalRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os tipos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarTipoAtendimentoEmissao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repAtendimentoTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                string descricao = Request.Params["Descricao"] ?? string.Empty;
                string statusStr = Request.Params["Status"] ?? string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;

                if (statusStr == "A")
                    status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                else if (statusStr == "I")
                    status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo;

                List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo> lista = repAtendimentoTipo.ConsultarMultiCTe(this.EmpresaUsuario.Codigo, descricao, status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Emissao, inicioRegistros, 50);
                int totalRegistros = repAtendimentoTipo.ContarConsultaMultiCTe(this.EmpresaUsuario.Codigo, descricao, status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo.Emissao);

                var retorno = (from obj in lista
                               select new
                               {
                                   obj.Codigo,
                                   Descricao = obj.Descricao
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Descrição|80" }, totalRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os tipos.");
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
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repAtendimentoTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["Empresa"], out int codigoEmpresa);
                int.TryParse(Request.Params["Tipo"], out int codigoTipo);

                DateTime? data = null;
                if (DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataAux))
                    data = dataAux;

                string descricao = Request.Params["Descricao"] ?? string.Empty;
                string observacao = Request.Params["Observacao"] ?? string.Empty;
                string contato = Request.Params["Contato"] ?? string.Empty;

                Enum.TryParse(Request.Params["Satisfacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao satisfacao);
                Enum.TryParse(Request.Params["Sistema"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistema sistema);
                Enum.TryParse(Request.Params["TipoContato"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento tipoContato);
                Enum.TryParse(Request.Params["Situacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusAtendimento);

                bool.TryParse(Request.Params["NecessitouAuxilio"], out bool necessitouAuxilio);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo tipo = repAtendimentoTipo.BuscarPorCodigo(codigoTipo);

                if (empresa == null)
                    empresa = this.EmpresaUsuario;

                if (!data.HasValue)
                    return Json<bool>(false, false, "Data é obrigatória.");

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição é obrigatória.");

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");
                }

                Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento = repAtendimento.BuscarPorCodigoEEmpresa(codigo, this.EmpresaUsuario.Codigo);

                if (atendimento == null)
                {
                    atendimento = new Dominio.Entidades.Embarcador.Atendimento.Atendimento() {
                        EmpresaFilho = empresa,
                        Empresa = this.EmpresaUsuario,
                        Funcionario = this.Usuario
                    };
                }
                else if (atendimento.Funcionario.Codigo != this.Usuario.Codigo) 
                    return Json<bool>(false, false, "Não é possível modificar dados de outros usuários.");

                if (atendimento != null)
                    atendimento.Initialize();

                atendimento.ObservacaoSuporte = descricao;
                atendimento.Observacao = observacao;
                atendimento.DataInicial = data;
                atendimento.DataFinal = data;
                atendimento.AtendimentoTipo = tipo;
                atendimento.NivelSatisfacao = satisfacao;
                atendimento.TipoSistema = sistema;
                atendimento.TipoContato = tipoContato;
                atendimento.ContatoAtendimento = Utilidades.String.RemoveAllSpecialCharactersNotCommon(contato);
                atendimento.Status = statusAtendimento;
                atendimento.NecessitouAuxilio = necessitouAuxilio;

                if (atendimento.Codigo == 0)
                {
                    atendimento.Numero = repAtendimento.BuscarProximoNumero(this.EmpresaUsuario.Codigo);
                }

                if (codigo > 0)
                    repAtendimento.Atualizar(atendimento, Auditado);
                else
                    repAtendimento.Inserir(atendimento, Auditado);

                return Json(new {
                    Codigo = atendimento.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o atendimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarEmissao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repAtendimentoTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["Empresa"], out int codigoEmpresa);
                int.TryParse(Request.Params["Tipo"], out int codigoTipo);

                DateTime? data = null;
                if (DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataAux))
                    data = dataAux;

                string descricao = Request.Params["Descricao"] ?? string.Empty;
                string observacao = Request.Params["Observacao"] ?? string.Empty;
                string contato = Request.Params["Contato"] ?? string.Empty;

                Enum.TryParse(Request.Params["Satisfacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao satisfacao);
                Enum.TryParse(Request.Params["Sistema"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistema sistema);
                Enum.TryParse(Request.Params["TipoContato"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento tipoContato);
                Enum.TryParse(Request.Params["Situacao"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusAtendimento);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo tipo = repAtendimentoTipo.BuscarPorCodigo(codigoTipo);

                if (empresa == null)
                    return Json<bool>(false, false, "Empresa é obrigatória.");

                if (!data.HasValue)
                    return Json<bool>(false, false, "Data é obrigatória.");

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição é obrigatória.");

                Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento = repAtendimento.BuscarPorCodigoEEmpresa(codigo, this.EmpresaUsuario.Codigo);

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    if (atendimento != null && atendimento.Funcionario != this.Usuario)
                        return Json<bool>(false, false, "Registro não pode ser alterado por usuário diferente do que efetuou o lançamento.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");
                }

                if (atendimento == null)
                {
                    atendimento = new Dominio.Entidades.Embarcador.Atendimento.Atendimento()
                    {
                        EmpresaFilho = empresa,
                        Empresa = this.EmpresaUsuario,
                        Funcionario = this.Usuario
                    };
                }
                else if (atendimento.Funcionario.Codigo != this.Usuario.Codigo)
                    return Json<bool>(false, false, "Não é possível modificar dados de outros usuários.");

                if (atendimento != null)
                    atendimento.Initialize();

                atendimento.ObservacaoSuporte = descricao;
                atendimento.Observacao = observacao;
                atendimento.DataInicial = data;
                atendimento.DataFinal = data;
                atendimento.AtendimentoTipo = tipo;
                atendimento.NivelSatisfacao = satisfacao;
                atendimento.TipoSistema = sistema;
                atendimento.TipoContato = tipoContato;
                atendimento.ContatoAtendimento = contato;
                atendimento.Status = statusAtendimento;

                if (atendimento.Codigo == 0)
                {
                    atendimento.Numero = repAtendimento.BuscarProximoNumero(this.EmpresaUsuario.Codigo);
                }

                if (codigo > 0)
                    repAtendimento.Atualizar(atendimento, Auditado);
                else
                    repAtendimento.Inserir(atendimento, Auditado);

                return Json(new
                {
                    Codigo = atendimento.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o atendimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private string CaminhoArquivo()
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoArquivos"], "Atendimento");
            
            return caminho;
        }

        private string PeriodoData(DateTime dataInicial, DateTime dataFinal)
        {
            string data = "";

            if (dataInicial != DateTime.MinValue)
                data += "De " + dataInicial.ToString("dd/MM/yyyy");

            if (dataFinal != DateTime.MinValue)
                data += " Até " + dataFinal.ToString("dd/MM/yyyy");

            if(dataInicial == DateTime.MinValue && dataFinal == DateTime.MinValue)
                data = "Todo";

            return data.Trim();
        }
    }
}
