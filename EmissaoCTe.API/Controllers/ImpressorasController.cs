using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ImpressorasController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            //Request.UrlReferrer;
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("impressoras.aspx") || obj.Pagina.Formulario.ToLower().Equals("cadastroImpressora.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo, numeroUnidade;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Unidade"]), out numeroUnidade);

                string nomeImpressora = Request.Params["Impressora"];
                string status = Request.Params["Status"];
                string log = "";

                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unidadeDeTrabalho);

                Dominio.Entidades.Impressora impressoraValidacao = repImpressora.BuscarPorUnidade(numeroUnidade, "A");
                if (status == "A" && codigo > 0 && impressoraValidacao != null && impressoraValidacao.Codigo != codigo)
                    return Json<bool>(false, false, "Impossível salvar, já existe uma impressora Ativa para a unidade " + numeroUnidade.ToString());

                Dominio.Entidades.Impressora impressora;
                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de impressora negada!");
                    impressora = repImpressora.BuscarPorCodigo(codigo);

                    if (impressora.Status != status)
                    {
                        if (status == "I")
                            log = "Status Inativo.";
                        else
                            log = "Status Ativo.";
                    }

                    if (impressora.NumeroDaUnidade != numeroUnidade)
                    {
                        if (!string.IsNullOrWhiteSpace(log))
                           log = string.Concat(log, " ", "Unidade de " + impressora.NumeroDaUnidade.ToString() + " para " + numeroUnidade.ToString() + " .");
                        else
                            log = "Unidade de " + impressora.NumeroDaUnidade.ToString() + " para " + numeroUnidade.ToString() + " .";
                    }

                    if (impressora.NomeImpressora != nomeImpressora)
                    {
                        if (!string.IsNullOrWhiteSpace(log))
                           log = string.Concat(log, " ", "Impressora de " + impressora.NomeImpressora + " para " + nomeImpressora + " .");
                        else
                            log = "Impressora de " + impressora.NomeImpressora + " para " + nomeImpressora + " .";
                    }


                    if (!string.IsNullOrWhiteSpace(log))
                    {
                        if (this.UsuarioAdministrativo != null)
                            log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, ": ", log);
                        else
                            log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, ": ", log);
                    }
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de impressora negada!");
                    impressora = new Dominio.Entidades.Impressora();

                    if (this.UsuarioAdministrativo != null)
                        log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Inserido por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, ".");
                    else
                        log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Inserido por ", this.Usuario.CPF, " - ", this.Usuario.Nome, ".");
                }

                impressora.NumeroDaUnidade = numeroUnidade;
                impressora.NomeImpressora = nomeImpressora;
                impressora.Status = status;
                impressora.Documento = "C";
                impressora.CodigoIntegracao = numeroUnidade.ToString();

                if (string.IsNullOrWhiteSpace(impressora.Log))
                    impressora.Log = log;
                else
                    impressora.Log = string.Concat(impressora.Log, "\n", log);

                if (codigo > 0)
                    repImpressora.Atualizar(impressora);
                else
                    repImpressora.Inserir(impressora);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Impressora.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int numeroUnidade, inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Unidade"]), out numeroUnidade);                

                string nomeImpressora = Request.Params["Impressora"];
                string status = Request.Params["Status"];

                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unidadeDeTrabalho);
                var listaImpressoras = repImpressora.Consultar(numeroUnidade, nomeImpressora, status, string.Empty, inicioRegistros, 50);
                int countImpressoras = repImpressora.ContarConsulta(numeroUnidade, nomeImpressora, status, string.Empty);

                var retorno = (from obj in listaImpressoras
                               select new
                               {
                                   obj.Log,
                                   obj.Codigo,
                                   obj.NumeroDaUnidade,
                                   obj.NomeImpressora,
                                   obj.Status
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Log", "Código|10", "Unidade|20", "Impressora|40", "Status|10" }, countImpressoras);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as naturezas das operações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
