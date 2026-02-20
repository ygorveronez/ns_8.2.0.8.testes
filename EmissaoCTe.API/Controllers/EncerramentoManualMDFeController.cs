using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EncerramentoManualMDFeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("encerramentomanualmdfe.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult EncerrarMDFe()
        {
            try
            {
                int codigoLocalideEncerramento, codigoEmpresa;
                int.TryParse(Request.Params["CodigoLocalidadeEncerramento"], out codigoLocalideEncerramento);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                string chaveMDFe = Utilidades.String.OnlyNumbers(Request.Params["Chave"]);
                string protocolo = Utilidades.String.OnlyNumbers(Request.Params["Protocolo"]);


                // Verifica se a chave e valida
                if (!Utilidades.Validate.ValidarChave(chaveMDFe))
                    return Json<bool>(false, false, "A chave informada é inválida.");

                // Verifica se a chave e valida
                if (protocolo.Length != 15)
                    return Json<bool>(false, false, "O protocolo informada é inválido.");

                DateTime dataEncerramento;
                DateTime.TryParseExact(Request.Params["DataEncerramento"] + " " + Request.Params["HoraEncerramento"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                return EfetuarEncerramentoMDFe(codigoEmpresa, codigoLocalideEncerramento, chaveMDFe, protocolo, dataEncerramento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EncerrarMDFeMultiCTe()
        {
            try
            {
                int codigoLocalideEncerramento;
                int.TryParse(Request.Params["CodigoLocalidadeEncerramento"], out codigoLocalideEncerramento);
                int codigoEmpresa = this.EmpresaUsuario.Codigo;

                string chaveMDFe = Request.Params["Chave"];
                string protocolo = Request.Params["Protocolo"];

                DateTime dataEncerramento;
                DateTime.TryParseExact(Request.Params["DataEncerramento"] + " " + Request.Params["HoraEncerramento"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                return EfetuarEncerramentoMDFe(codigoEmpresa, codigoLocalideEncerramento, chaveMDFe, protocolo, dataEncerramento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao processar os dados do encerramento.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoEmpresa, codigoUsuario, codigoLocalidade, inicioRegistros;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);
                int.TryParse(Request.Params["CodigoUsuario"], out codigoUsuario);
                int.TryParse(Request.Params["CodigoLocalidade"], out codigoLocalidade);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string nomeDaEmpresa = Request.Params["NomeDaEmpresa"];
                string CNPJ = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                string chaveMDFe = Utilidades.String.OnlyNumbers(Request.Params["ChaveMDFe"]);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.EncerramentoManualMDFe repEncerramentoManualMDFe = new Repositorio.EncerramentoManualMDFe(unitOfWork);
                List<Dominio.Entidades.EncerramentoManualMDFe> listaDeMdfes = repEncerramentoManualMDFe.Consultar(codigoEmpresa, codigoUsuario, codigoLocalidade, nomeDaEmpresa, CNPJ, chaveMDFe, dataInicial, dataFinal, inicioRegistros, 10);

                int countRetorno = repEncerramentoManualMDFe.ContarConsulta(codigoEmpresa, codigoUsuario, codigoLocalidade, nomeDaEmpresa, CNPJ, chaveMDFe, dataInicial, dataFinal, inicioRegistros, 10);
                var retorno = (from obj in listaDeMdfes
                               select new
                               {
                                   obj.Codigo,
                                   NomeDaEmpresa = obj.Empresa.NomeFantasia,
                                   CNPJ = obj.Empresa.CNPJ_Formatado,
                                   ChaveDaMDFe = obj.ChaveMDFe,
                                   DataHoraEncerramento = obj.DataHoraEncerramento.ToString("dd/MM/yyyy HH:mm"),
                               }).ToList();

                unitOfWork.Dispose();

                return Json(retorno, true, null, new string[] { "Codigo", "Nome Da Empresa|28", "CNPJ|15", "Chave Da MDFe|28", "Data Hora Encerramento|20" }, countRetorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarMultiCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int codigoEmpresa = this.EmpresaUsuario.Codigo;
                int codigoUsuario = this.Usuario.Codigo;

                string chaveMDFe = Utilidades.String.OnlyNumbers(Request.Params["ChaveMDFe"]);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.EncerramentoManualMDFe repEncerramentoManualMDFe = new Repositorio.EncerramentoManualMDFe(unitOfWork);
                List<Dominio.Entidades.EncerramentoManualMDFe> listaDeMdfes = repEncerramentoManualMDFe.Consultar(codigoEmpresa, codigoUsuario, 0, string.Empty, string.Empty, chaveMDFe, dataInicial, dataFinal, inicioRegistros, 10);

                int countRetorno = repEncerramentoManualMDFe.ContarConsulta(codigoEmpresa, codigoUsuario, 0, string.Empty, string.Empty, chaveMDFe, dataInicial, dataFinal, inicioRegistros, 10);
                var retorno = (from obj in listaDeMdfes
                               select new
                               {
                                   obj.Codigo,
                                   obj.ChaveMDFe,
                                   obj.Protocolo,
                                   DataHoraEncerramento = obj.DataHoraEncerramento.ToString("dd/MM/yyyy HH:mm"),
                                   obj.Log
                               }).ToList();

                unitOfWork.Dispose();

                return Json(retorno, true, null, new string[] { "Codigo", "Chave Da MDFe|33", "Protocolo|12", "Data Hora|11", "Log|44" }, countRetorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ValidarChave()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string chaveMDFe = Utilidades.String.OnlyNumbers(Request.Params["ChaveMDFe"]);

                // Verifica se a chave e valida
                if (!Utilidades.Validate.ValidarChave(chaveMDFe))
                    return Json("A chave informada é inválida.", true, "");

                // Verifica se chave existe na base
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repoMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                if (repoMDFe.BuscarPorChave(chaveMDFe) != null)
                    return Json("Não é possível encerrar manualmente um MDF-e que está no sistema.", true, "");

                unitOfWork.Dispose();

                return Json("", true, "");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe;
                int.TryParse(Request.Params["Codigo"], out codigoMDFe);

                Repositorio.EncerramentoManualMDFe repEncerramentoManualMDFe = new Repositorio.EncerramentoManualMDFe(unitOfWork);
                Dominio.Entidades.EncerramentoManualMDFe mdfe = repEncerramentoManualMDFe.BuscaPorCodigo(codigoMDFe);

                if (mdfe != null)
                {
                    var retorno = new
                    {
                        mdfe.Codigo,
                        NomeDaEmpresa = mdfe.Empresa.NomeFantasia,
                        mdfe.ChaveMDFe,
                        mdfe.Protocolo,
                        DataEncerramento = mdfe.DataHoraEncerramento.ToString("dd/MM/yyyy"),
                        HoraEncerramento = mdfe.DataHoraEncerramento.ToString("HH:mm"),
                        Localidade = mdfe.Localidade.Descricao,
                        mdfe.Log
                    };

                    unitOfWork.Dispose();

                    return Json(retorno, true);
                }

                unitOfWork.Dispose();

                return Json<bool>(false, false, "MDF-e não econtrado.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar o MDF-e.");
            }
        }
        #endregion

        private JsonResult EfetuarEncerramentoMDFe(int codigoEmpresa, int codigoLocalideEncerramento, string chaveMDFe, string protocolo, DateTime dataEncerramento)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.EncerramentoManualMDFe repEncerramentoManualMDFe = new Repositorio.EncerramentoManualMDFe(unidadeDeTrabalho);

                Dominio.Entidades.Localidade localidadeEncerramento = repLocalidade.BuscarPorCodigo(codigoLocalideEncerramento);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                bool retorno = Servicos.Embarcador.Carga.MDFe.EncerrarMDFeEmissorExterno(out string erroMDFe, chaveMDFe, localidadeEncerramento, protocolo, empresa, dataEncerramento, Usuario, unidadeDeTrabalho);

                if (retorno)
                {
                    Dominio.Entidades.EncerramentoManualMDFe log = new Dominio.Entidades.EncerramentoManualMDFe();

                    // Apos integrar o encerramento do MDFe, inserir o log
                    log.ChaveMDFe = chaveMDFe;
                    log.Protocolo = protocolo;
                    log.DataHoraEncerramento = DateTime.Now;
                    log.DataHoraEvento = dataEncerramento;
                    log.Empresa = empresa;
                    log.Localidade = localidadeEncerramento;
                    log.Usuario = this.Usuario;
                    log.Log = string.Concat(log.DataHoraEncerramento.ToString("dd/MM/yyyy HH:mm:ss"), " - Encerramento enviado por ", log.Usuario.Nome);

                    repEncerramentoManualMDFe.Inserir(log);

                    return Json<bool>(true, true);
                }

                return Json<bool>(false, false, !string.IsNullOrWhiteSpace(erroMDFe) ? erroMDFe : "Não foi possível salvar os dados para encerramento.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
