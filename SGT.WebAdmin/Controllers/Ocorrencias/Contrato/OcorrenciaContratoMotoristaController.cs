using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencia
{
    [CustomAuthorize("Ocorrencias/Ocorrencia")]
    public class OcorrenciaContratoMotoristaController : BaseController
    {
		#region Construtores

		public OcorrenciaContratoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarDadosContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                // Parametros
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(this.Usuario.ClienteTerceiro?.CPF_CNPJ_SemFormato ?? string.Empty);
                int transportador = empresa?.Codigo ?? 0;
                int.TryParse(Request.Params("TipoOcorrencia"), out int tipoOcorrencia);

                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarContratoAtivo(transportador, tipoOcorrencia, DateTime.Now.Date);

                // Valida
                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new 
                {
                    Contrato = contrato.Codigo,
                    ContratoDescricao = contrato.Descricao,
                    contrato.DescontarValoresOutrasCargas,
                    contrato.QuantidadeMotoristas,
                    ValorDiario = contrato.ValorDiariaPorMotorista.ToString("n2"),
                    ValorQuinzenal = contrato.ValorQuinzenaPorMotorista.ToString("n2"),
                    TotalOcorrenciaMotorista = (contrato.ValorQuinzenaPorMotorista > 0 ? contrato.ValorQuinzenaPorMotorista : 0).ToString("n2"),
                    ValorKmExcedente = contrato.OutrosValoresValorKmExcedente.ToString("n2")
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
