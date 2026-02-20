using System;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class VeiculoMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.VeiculoMDFe repVeiculo = new Repositorio.VeiculoMDFe(unitOfWork);
                Dominio.Entidades.VeiculoMDFe veiculo = repVeiculo.BuscarPorMDFe(codigoMDFe);

                var retorno = veiculo != null ? new Dominio.ObjetosDeValor.VeiculoMDFe()
                {
                    CapacidadeKG = veiculo.CapacidadeKG,
                    CapacidadeM3 = veiculo.CapacidadeM3,
                    Codigo = veiculo.Codigo,
                    Tara = veiculo.Tara,
                    RNTRC = veiculo.RNTRC,
                    Placa = veiculo.Placa,
                    RENAVAM = veiculo.RENAVAM,
                    CPFCNPJ = veiculo.CPFCNPJProprietario,
                    IE = veiculo.IEProprietario,
                    Nome = veiculo.NomeProprietario,
                    TipoCarroceria = veiculo.TipoCarroceria,
                    TipoProprietario = veiculo.TipoProprietario,
                    TipoRodado = veiculo.TipoRodado,
                    UF = veiculo.UF.Sigla,
                    UFProprietario = veiculo.UFProprietario != null ? veiculo.UFProprietario.Sigla : string.Empty,
                    Excluir = false
                } : null;

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
