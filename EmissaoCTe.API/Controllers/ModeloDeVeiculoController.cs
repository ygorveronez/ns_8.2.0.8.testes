using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ModeloDeVeiculoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("modelosdeveiculos.aspx") select obj).FirstOrDefault();
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

                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
                var listaModelosVeiculos = repModeloVeiculo.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countModelosVeiculos = repModeloVeiculo.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = from obj in listaModelosVeiculos select new { obj.Codigo, obj.Status, obj.Descricao, NumeroEixo = obj.NumeroEixo.ToString("n0"), obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|60", "Número Eixo|15", "Status|15" }, countModelosVeiculos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os modelos de veículos.");
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
                int codigo, numeroEixo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["NumeroEixo"], out numeroEixo);
                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];
                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
                Dominio.Entidades.ModeloVeiculo modeloVeiculo;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    modeloVeiculo = new Dominio.Entidades.ModeloVeiculo();
                    modeloVeiculo.Status = "A";
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    modeloVeiculo = repModeloVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                modeloVeiculo.Descricao = descricao;
                modeloVeiculo.NumeroEixo = numeroEixo;
                modeloVeiculo.Empresa = this.EmpresaUsuario;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    modeloVeiculo.Status = status;

                if (codigo > 0)
                    repModeloVeiculo.Atualizar(modeloVeiculo);
                else
                    repModeloVeiculo.Inserir(modeloVeiculo);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o modelo de veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
