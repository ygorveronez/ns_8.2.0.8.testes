using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PropostaConfiguracaoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("configuracoesproposta.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.PropostaConfiguracao repPropostaConfiguracao = new Repositorio.PropostaConfiguracao(unitOfWork);
                Dominio.Entidades.PropostaConfiguracao propostaConfiguracao = repPropostaConfiguracao.BuscaPorEmpresa(this.EmpresaUsuario.Codigo);

                if (propostaConfiguracao == null)
                    propostaConfiguracao = new Dominio.Entidades.PropostaConfiguracao();

                var retorno = new
                {
                    TextoCTRN = !string.IsNullOrWhiteSpace(propostaConfiguracao.TextoCTRN) ? propostaConfiguracao.TextoCTRN : string.Empty,
                    TextoCustosAdicionais = !string.IsNullOrWhiteSpace(propostaConfiguracao.TextoCustosAdicionais) ? propostaConfiguracao.TextoCustosAdicionais : string.Empty,
                    TextoFormaCobranca = !string.IsNullOrWhiteSpace(propostaConfiguracao.TextoFormaCobranca) ? propostaConfiguracao.TextoFormaCobranca : string.Empty,
                    DiasValidade = propostaConfiguracao.DiasValidade != null ? propostaConfiguracao.DiasValidade : 30
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as configurações de proposta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        [ValidateInput(false)]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.PropostaConfiguracao repPropostaConfiguracao = new Repositorio.PropostaConfiguracao(unidadeDeTrabalho);
                Dominio.Entidades.PropostaConfiguracao propostaConfiguracao = repPropostaConfiguracao.BuscaPorEmpresa(this.EmpresaUsuario.Codigo);

                if (propostaConfiguracao == null)
                    propostaConfiguracao = new Dominio.Entidades.PropostaConfiguracao();

                int diasValidade = 0;
                int.TryParse(Request.Params["DiasValidade"], out diasValidade);

                string textoCustosAdicionais = Request.Params["TextoCustosAdicionais"];
                string textoFormaCobranca = Request.Params["TextoFormaCobranca"];
                string textoCTRN = Request.Params["TextoCTRN"];
                
                if (propostaConfiguracao.Codigo > 0 && this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para alteração negada!");
                else if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão negada!");

                // Preenchimento dos dados
                if (!string.IsNullOrWhiteSpace(Request.Params["DiasValidade"]))
                    propostaConfiguracao.DiasValidade = diasValidade;
                else
                    propostaConfiguracao.DiasValidade = null;
                propostaConfiguracao.TextoCustosAdicionais = textoCustosAdicionais;
                propostaConfiguracao.TextoFormaCobranca = textoFormaCobranca;
                propostaConfiguracao.TextoCTRN = textoCTRN;

                unidadeDeTrabalho.Start();

                if (propostaConfiguracao.Codigo > 0)
                    repPropostaConfiguracao.Atualizar(propostaConfiguracao);
                else
                {
                    propostaConfiguracao.Empresa = this.EmpresaUsuario;
                    repPropostaConfiguracao.Inserir(propostaConfiguracao);
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar as configuração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
        #endregion
    }
}