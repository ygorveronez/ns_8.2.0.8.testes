using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ServicoNFSeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("servicosnfse.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];
                
                Repositorio.ServicoNFSe repServico = new Repositorio.ServicoNFSe(unitOfWork);
                List<Dominio.Entidades.ServicoNFSe> listaServicos = repServico.Consultar(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai.Codigo, descricao, status, inicioRegistros, 50);
                int countServicos = repServico.ContarConsulta(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai.Codigo, descricao, status);

                var retorno = from obj in listaServicos
                              select new
                              {
                                  obj.Codigo,
                                  obj.Numero,
                                  obj.Descricao,
                                  Aliquota = obj.Aliquota.ToString("n4")
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Número|15", "Descrição|60", "Alíquota|15" }, countServicos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os serviços.");
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
                int.TryParse(Request.Params["CodigoServico"], out codigo);

                Repositorio.ServicoNFSe repServico = new Repositorio.ServicoNFSe(unitOfWork);

                Dominio.Entidades.ServicoNFSe servico = repServico.BuscarPorCodigo(codigo);

                if (servico == null)
                    return Json<bool>(false, false, "Serviço não encontrado. Atualize a página e tente novamente.");

                var retorno = new
                {
                    Aliquota = servico.Aliquota.ToString("n4"),
                    servico.Codigo,
                    servico.CNAE,
                    servico.CodigoTributacao,
                    servico.Descricao,
                    servico.Numero,
                    servico.NBS,
                    servico.Status,
                    ISSRetido = servico.ISSRetido ? "1" : "0",
                    servico.ISSIncluso
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do serviço.");
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
                int codigo, issRetido = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["ISSRetido"], out issRetido);

                decimal aliquota;
                decimal.TryParse(Request.Params["Aliquota"], out aliquota);

                string numero = Request.Params["Numero"];
                string nbs = Request.Params["NBS"];
                string cnae = Request.Params["CNAE"];
                string codigoTributacao = Request.Params["CodigoTributacao"];
                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");

                Dominio.Enumeradores.InclusaoISSNFSe issIncluso;
                Enum.TryParse<Dominio.Enumeradores.InclusaoISSNFSe>(Request.Params["ISSIncluso"], out issIncluso);

                Repositorio.ServicoNFSe repServico = new Repositorio.ServicoNFSe(unitOfWork);

                Dominio.Entidades.ServicoNFSe servico = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    servico = repServico.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    servico = new Dominio.Entidades.ServicoNFSe();
                }

                servico.Aliquota = aliquota;
                servico.CNAE = cnae;
                servico.CodigoTributacao = codigoTributacao;
                servico.Descricao = descricao;
                servico.Empresa = this.EmpresaUsuario;
                servico.Numero = numero;
                servico.NBS = nbs;
                servico.Status = status;
                servico.ISSRetido = issRetido == 1 ? true : false;
                servico.ISSIncluso = issIncluso;

                if (servico.Codigo > 0)
                    repServico.Atualizar(servico);
                else
                    repServico.Inserir(servico);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o serviço.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
