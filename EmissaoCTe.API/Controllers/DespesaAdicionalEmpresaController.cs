using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DespesaAdicionalEmpresaController : ApiController
    {
        #region Propriedades

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("despesasempresa.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string nomeEmpresa = Request.Params["NomeEmpresa"];
                string descricaoDespesa = Request.Params["DescricaoDespesa"];

                DateTime dataInicial, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.DespesaAdicionalEmpresa repDespesa = new Repositorio.DespesaAdicionalEmpresa(unidadeDeTrabalho);
                List<Dominio.Entidades.DespesaAdicionalEmpresa> listaDespesas = repDespesa.Consultar(this.EmpresaUsuario.Codigo, nomeEmpresa, descricaoDespesa, dataInicial, dataFinal, inicioRegistros, 50);
                int countDespesas = repDespesa.ContarConsulta(this.EmpresaUsuario.Codigo, nomeEmpresa, descricaoDespesa, dataInicial, dataFinal);

                var result = from obj in listaDespesas select new { obj.Codigo, obj.Empresa.NomeFantasia, obj.Descricao, obj.Valor, obj.DescricaoStatus };

                return Json(result, true, null, new string[] { "Codigo", "Empresa|35", "Descrição|35", "Valor|10", "Status|10" }, countDespesas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as despesas.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["CodigoDespesa"], out codigo);

                Repositorio.DespesaAdicionalEmpresa repDespesa = new Repositorio.DespesaAdicionalEmpresa(unidadeDeTrabalho);
                Dominio.Entidades.DespesaAdicionalEmpresa despesa = repDespesa.BuscarPorCodigo(codigo);

                if (despesa == null)
                    return Json<bool>(false, false, "Despesa não encontrada.");

                var retorno = new
                {
                    despesa.Codigo,
                    DataInicial = despesa.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = despesa.DataFinal.ToString("dd/MM/yyyy"),
                    despesa.Descricao,
                    CodigoEmpresa = despesa.Empresa.Codigo,
                    DescricaoEmpresa = string.Concat(despesa.Empresa.CNPJ, " - ", despesa.Empresa.NomeFantasia),
                    despesa.Status,
                    despesa.Tipo,
                    Valor = despesa.Valor.ToString("n2")
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da despesa.");
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
                int codigo, codigoEmpresa = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                decimal valor = 0m;
                decimal.TryParse(Request.Params["Valor"], out valor);

                DateTime dataInicial, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];
                string tipo = Request.Params["Tipo"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                if (valor <= 0)
                    return Json<bool>(false, false, "O valor deve ser maior que zero.");

                if (dataInicial == DateTime.MinValue)
                    return Json<bool>(false, false, "Data inicial inválida.");

                if (dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Data final inválida.");

                Repositorio.DespesaAdicionalEmpresa repDespesa = new Repositorio.DespesaAdicionalEmpresa(unidadeDeTrabalho);

                Dominio.Entidades.DespesaAdicionalEmpresa despesa = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    despesa = repDespesa.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    despesa = new Dominio.Entidades.DespesaAdicionalEmpresa();
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                despesa.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (despesa.Empresa == null || despesa.Empresa.EmpresaPai == null || despesa.Empresa.EmpresaPai.Codigo != this.EmpresaUsuario.Codigo)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                despesa.DataFinal = dataFinal;
                despesa.DataInicial = dataInicial;
                despesa.Descricao = descricao;
                despesa.Status = status;
                despesa.Valor = valor;
                despesa.Tipo = tipo;

                if (despesa.Codigo > 0)
                    repDespesa.Atualizar(despesa);
                else
                    repDespesa.Inserir(despesa);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a despesa adicional para a empresa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

    }
}
