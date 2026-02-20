using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ConfiguracaoEmissaoEmailController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("configuracaoemissaoemail.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string nome = Request.Params["Nome"];
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                string status = Request.Params["Status"];

                Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);

                List<Dominio.Entidades.ConfiguracaoEmissaoEmail> listaConfiguracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Consultar(nome, cnpj, status, inicioRegistros, 50);
                int countConsulta = repConfiguracaoEmissaoEmail.ContarConsulta(nome, cnpj, status);
                
                var retorno = from obj in listaConfiguracaoEmissaoEmail
                              select new
                              {
                                  obj.Codigo,
                                  Empresa = obj.Empresa != null ? obj.Empresa.RazaoSocial : string.Empty,
                                  Email = obj.Email != null ? obj.Email.Email : string.Empty,
                                  Cliente = obj.ClienteRemetente != null ? obj.ClienteRemetente.Nome : string.Empty
                              };
                return Json(retorno, true, null, new String[] { "Codigo", "Empresa|30", "Email|30", "Cliente|20" }, countConsulta);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar configuração emissão e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                if (!int.TryParse(Request.Params["Codigo"], out codigo))
                    return Json<bool>(false, false, "Código inválido!");

                Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unidadeDeTrabalho);

                Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.BuscarPorCodigo(codigo);

                if (configuracaoEmissaoEmail != null)
                {
                    var retorno = new
                    {
                        configuracaoEmissaoEmail.Codigo,
                        CodigoEmail = configuracaoEmissaoEmail.Email.Codigo,
                        Email = configuracaoEmissaoEmail.Email.Email,
                        CodigoEmpresa = configuracaoEmissaoEmail.Empresa.Codigo,
                        CNPJEmpresa = configuracaoEmissaoEmail.Empresa.CNPJ,
                        RazaoEmpresa = configuracaoEmissaoEmail.Empresa.RazaoSocial,
                        CNPJCliente = configuracaoEmissaoEmail.ClienteRemetente != null ? configuracaoEmissaoEmail.ClienteRemetente.CPF_CNPJ : 0,
                        RazaoCliente = configuracaoEmissaoEmail.ClienteRemetente != null ? configuracaoEmissaoEmail.ClienteRemetente.Nome : string.Empty,
                        configuracaoEmissaoEmail.Agrupar,
                        configuracaoEmissaoEmail.Emitir,
                        configuracaoEmissaoEmail.TipoDocumento,
                        configuracaoEmissaoEmail.TempoEmitir,
                        configuracaoEmissaoEmail.Status,
                        configuracaoEmissaoEmail.Tipo,
                        configuracaoEmissaoEmail.TamanhoPalavra,
                        configuracaoEmissaoEmail.PalavraChave,
                        configuracaoEmissaoEmail.GerarMDFe
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Configuração emissão e-mail não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados da Configuração emissão e-mail. Atualize a página e tente novamente.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, tempoEmissao, codigoEmpresa, codigoEmail, tamanhoPalavra= 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["TempoEmissao"], out tempoEmissao);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);
                int.TryParse(Request.Params["CodigoEmail"], out codigoEmail);
                int.TryParse(Request.Params["TamanhoPalavra"], out tamanhoPalavra);

                string status = Request.Params["Status"];
                string tipo = Request.Params["Tipo"];
                string palavraChave = Request.Params["PalavraChave"] ?? string.Empty;

                Dominio.Enumeradores.TipoDocumento tipoDocumento;
                Enum.TryParse<Dominio.Enumeradores.TipoDocumento>(Request.Params["TipoDocumento"], out tipoDocumento);

                Dominio.Enumeradores.OpcaoSimNao emitir;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["Emitir"], out emitir);

                Dominio.Enumeradores.OpcaoSimNao gerarMDFe;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["GerarMDFe"], out gerarMDFe);

                Dominio.Enumeradores.TipoAgrupamentoEmissaoEmail agrupar;
                Enum.TryParse<Dominio.Enumeradores.TipoAgrupamentoEmissaoEmail>(Request.Params["Agrupar"], out agrupar);

                double cnpjCliente;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Cliente"]), out cnpjCliente);

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);

                Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inserção negada.");

                    configuracaoEmissaoEmail = new Dominio.Entidades.ConfiguracaoEmissaoEmail();
                }

                configuracaoEmissaoEmail.ClienteRemetente = cnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCliente) : null;
                configuracaoEmissaoEmail.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                configuracaoEmissaoEmail.Email = repEmail.BuscarPorCodigo(codigoEmail);
                configuracaoEmissaoEmail.Agrupar = agrupar;
                configuracaoEmissaoEmail.Emitir = emitir;
                configuracaoEmissaoEmail.TipoDocumento = tipoDocumento;
                configuracaoEmissaoEmail.TempoEmitir = tempoEmissao;
                configuracaoEmissaoEmail.Status = status;
                configuracaoEmissaoEmail.Tipo = tipo;
                configuracaoEmissaoEmail.PalavraChave = palavraChave;
                configuracaoEmissaoEmail.TamanhoPalavra = tamanhoPalavra;
                configuracaoEmissaoEmail.GerarMDFe = gerarMDFe;

                if (configuracaoEmissaoEmail.Codigo > 0)
                    repConfiguracaoEmissaoEmail.Atualizar(configuracaoEmissaoEmail);
                else
                    repConfiguracaoEmissaoEmail.Inserir(configuracaoEmissaoEmail);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Configuração Emissão E-mail");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
        
        [AcceptVerbs("POST")]
        public ActionResult ConsultarEmail()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string email = Request.Params["Email"];

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> listaEmail = repConfigEmailDocTransporte.Consultar(email,"Codigo","desc", inicioRegistros,50, true);
                int countConsulta = repConfigEmailDocTransporte.ContarConsulta(email);

                var retorno = from obj in listaEmail
                              select new
                              {
                                  obj.Codigo,
                                  Email = obj.Email
                              };
                return Json(retorno, true, null, new String[] { "Codigo", "E-mail|80" }, countConsulta);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar configuração emissão e-mail.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

    }
}