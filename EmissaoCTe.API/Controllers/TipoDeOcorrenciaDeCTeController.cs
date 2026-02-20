using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoDeOcorrenciaDeCTeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("tipodeocorrenciacte.aspx") select obj).FirstOrDefault();
        }

        #endregion


        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                bool cadastro = !string.IsNullOrWhiteSpace(Request.Params["Tipo"]);
                string descricao = Request.Params["Descricao"];
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> listaTipoOcorrencia = repTipoOcorrencia.Consultar(this.EmpresaUsuario.Codigo, descricao, inicioRegistros, 50); 
                int countTipoOcorrencia = repTipoOcorrencia.ContarConsulta(this.EmpresaUsuario.Codigo, descricao);
                if (!cadastro && countTipoOcorrencia == 0)
                {
                    listaTipoOcorrencia = repTipoOcorrencia.Consultar(0, descricao, inicioRegistros, 50);
                    countTipoOcorrencia = repTipoOcorrencia.ContarConsulta(0, descricao);
                }

                var retorno = (from obj in listaTipoOcorrencia
                               select new
                               {
                                   obj.Codigo,
                                   obj.CodigoProceda,
                                   obj.Descricao,
                                   Cliente = obj.Pessoa != null ? obj.Pessoa.CPF_CNPJ_Formatado + obj.Pessoa.Nome : string.Empty,
                                  obj.DescricaoTipo
                              }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Código EDI|15", "Descricao|40", "Cliente|20", "Tipo|15" }, countTipoOcorrencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os tipos de ocorrência para CT-es.");
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
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = repTipoDeOcorrencia.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    tipoDeOcorrencia.Codigo,
                    tipoDeOcorrencia.Descricao,
                    tipoDeOcorrencia.CodigoProceda,
                    tipoDeOcorrencia.CodigoIntegracao,
                    tipoDeOcorrencia.Tipo,
                    CNPJCliente = tipoDeOcorrencia.Pessoa != null ? tipoDeOcorrencia.Pessoa.CPF_CNPJ : 0,
                    Cliente = tipoDeOcorrencia.Pessoa != null ? tipoDeOcorrencia.Pessoa.CPF_CNPJ_Formatado + " " + tipoDeOcorrencia.Pessoa.Nome : string.Empty
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter detalhes.");
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
                int.TryParse(Request.Params["Codigo"], out int codigo);

                string descricao = Request.Params["Descricao"];
                string codigoProceda = Request.Params["CodigoProceda"];
                string codigoIntegracao = Request.Params["CodigoIntegracao"];
                string tipo = Request.Params["Tipo"];

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CNPJCliente"]), out double cnpjCliente);

                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de Duplicata negada!");

                    tipoDeOcorrencia = repTipoDeOcorrencia.BuscarPorCodigo(codigo);

                    if (tipoDeOcorrencia.OutroEmitente == null || tipoDeOcorrencia.OutroEmitente != this.EmpresaUsuario)
                        return Json<bool>(false, false, "Somente permitido alterar ocorrências proprias!");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de Duplicata negada!");

                    tipoDeOcorrencia = new Dominio.Entidades.TipoDeOcorrenciaDeCTe();
                    tipoDeOcorrencia.OutroEmitente = this.EmpresaUsuario;
                }

                tipoDeOcorrencia.Descricao = descricao;
                tipoDeOcorrencia.Pessoa = cnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCliente) : null;
                tipoDeOcorrencia.CodigoProceda = codigoProceda;
                tipoDeOcorrencia.CodigoIntegracao = codigoIntegracao;
                tipoDeOcorrencia.Tipo = tipo;

                if (codigo > 0)
                    repTipoDeOcorrencia.Atualizar(tipoDeOcorrencia);
                else
                    repTipoDeOcorrencia.Inserir(tipoDeOcorrencia);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
