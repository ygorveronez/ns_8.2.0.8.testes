using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class VeiculoCTeController : ApiController
    {
        #region Métodos Publicos

        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe <= 0)
                    return Json<bool>(true, false, "Parâmetros inválidos.");
                
                Repositorio.VeiculoCTE repositorioVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);
                List<Dominio.Entidades.VeiculoCTE> veiculos = repositorioVeiculoCTe.BuscarPorCTe(codigoCTe);
                var retorno = from obj in veiculos
                                select new
                                {
                                    Id = obj.Codigo,
                                    obj.Veiculo.Codigo,
                                    obj.Veiculo.CapacidadeKG,
                                    obj.Veiculo.CapacidadeM3,
                                    obj.Veiculo.DescricaoTipo,
                                    obj.Veiculo.DescricaoTipoCarroceria,
                                    obj.Veiculo.DescricaoTipoCombustivel,
                                    obj.Veiculo.DescricaoTipoRodado,
                                    obj.Veiculo.DescricaoTipoVeiculo,
                                    UF = obj.Veiculo.Estado.Sigla,
                                    obj.Veiculo.Placa,
                                    obj.Veiculo.Renavam,
                                    obj.Veiculo.Tara,
                                    TipoDoVeiculo = obj.Veiculo.TipoDoVeiculo != null ? obj.Veiculo.TipoDoVeiculo.Descricao : string.Empty,
                                    Excluir = false
                                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as informações de veículos do CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
