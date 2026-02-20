using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class FretePorKMTipoDeVeiculoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("fretesporkmtipodeveiculo.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.FretePorKMTipoDeVeiculo repFretePorKMTipoDeVeiculo = new Repositorio.FretePorKMTipoDeVeiculo(unitOfWork);

                string tipoVeiculo = Request.Params["TipoVeiculo"];
                string status = Request.Params["Status"];

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                int.TryParse(Request.Params["KMInicial"], out int KMInicial);
                int.TryParse(Request.Params["KMFinal"], out int KMFinal);

                List<Dominio.Entidades.FretePorKMTipoDeVeiculo> listaFreteTipoVeiculo = repFretePorKMTipoDeVeiculo.Consultar(this.EmpresaUsuario.Codigo, status, tipoVeiculo, KMInicial, KMFinal, inicioRegistros, 50);
                int countFreteTipoVeiculo = repFretePorKMTipoDeVeiculo.ContarConsulta(this.EmpresaUsuario.Codigo, status, tipoVeiculo, KMInicial, KMFinal);

                var retorno = (from obj in listaFreteTipoVeiculo select 
                               new
                               {
                                   obj.Codigo,
                                   TipoVeiculo = obj.TipoVeiculo.Descricao,
                                   Valor = obj.Valor.ToString("n2"),
                                   KMFranquia = obj.KMFranquia.ToString("n0"),
                                   ExcedentePorKM = obj.ExcedentePorKM.ToString("n2"),
                                   Status = obj.DescricaoStatus
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Tipo Veículo|30", "Valor|15", "KM Franquia|15", "Excedente|15", "Status|15" }, countFreteTipoVeiculo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao obter os fretes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterTabelasDeFreteAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.FretePorKMTipoDeVeiculo repFretePorKMTipoDeVeiculo = new Repositorio.FretePorKMTipoDeVeiculo(unitOfWork);

                List<Dominio.Entidades.FretePorKMTipoDeVeiculo> listaFreteTipoVeiculo = repFretePorKMTipoDeVeiculo.BuscarTabelasAtivasPorTipo(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoCalculoFreteKMTipoVeiculo.Acerto);

                var retorno = (from obj in listaFreteTipoVeiculo
                               select 
                               new
                               {
                                   obj.Codigo,
                                   TipoVeiculo = obj.TipoVeiculo.Codigo,
                                   Valor = obj.Valor,
                                   KMFranquia = obj.KMFranquia,
                                   ExcedentePorKM = obj.ExcedentePorKM
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha obter os fretes.");
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
                Repositorio.FretePorKMTipoDeVeiculo repFrete = new Repositorio.FretePorKMTipoDeVeiculo(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.FretePorKMTipoDeVeiculo frete = repFrete.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (frete == null)
                    return Json<bool>(false, false, "Frete não encontrado.");

                var retorno = new
                {
                    frete.Codigo,
                    TipoVeiculo = new { frete.TipoVeiculo.Codigo, frete.TipoVeiculo.Descricao },
                    Valor = frete.Valor.ToString("n2"),
                    KMFranquia = frete.KMFranquia.ToString("n0"),
                    ExcedentePorKM = frete.ExcedentePorKM.ToString("n2"),
                    frete.Status,
                    frete.TipoCalculo
                };

                return Json(retorno, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do frete.");
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
                Repositorio.FretePorKMTipoDeVeiculo repFrete = new Repositorio.FretePorKMTipoDeVeiculo(unitOfWork);
                Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["TipoVeiculo"], out int codigoTipoVeiculo);

                decimal.TryParse(Request.Params["Valor"], out decimal valor);
                decimal.TryParse(Request.Params["KMFranquia"], out decimal KMFranquia);
                decimal.TryParse(Request.Params["ExcedentePorKM"], out decimal excedentePorKM);
                
                Enum.TryParse(Request.Params["TipoCalculo"], out Dominio.Enumeradores.TipoCalculoFreteKMTipoVeiculo tipoCalculo);

                string status = Request.Params["Status"];

                Dominio.Entidades.FretePorKMTipoDeVeiculo freteAux = repFrete.BuscarPorTipoVeiculo(codigoTipoVeiculo, this.EmpresaUsuario.Codigo);

                if (freteAux != null && freteAux.Codigo != codigo)
                    return Json<bool>(false, false, "Já existe um frete para esse Tipo de Veículo");

                Dominio.Entidades.FretePorKMTipoDeVeiculo frete = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada!");

                    frete = repFrete.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada!");

                    frete = new Dominio.Entidades.FretePorKMTipoDeVeiculo
                    {
                        Empresa = this.EmpresaUsuario
                    };
                }

                frete.TipoVeiculo = repTipoVeiculo.BuscarPorCodigo(codigoTipoVeiculo, this.EmpresaUsuario.Codigo);
                frete.Valor = valor;
                frete.KMFranquia = KMFranquia;
                frete.ExcedentePorKM = excedentePorKM;
                frete.Status = status;
                frete.TipoCalculo = tipoCalculo;

                if (frete.TipoVeiculo == null)
                    return Json<bool>(false, false, "Tipo de Veículo é obrigatório.");

                if (codigo > 0)
                    repFrete.Atualizar(frete);
                else
                    repFrete.Inserir(frete);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha genérica ao salvar o frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
