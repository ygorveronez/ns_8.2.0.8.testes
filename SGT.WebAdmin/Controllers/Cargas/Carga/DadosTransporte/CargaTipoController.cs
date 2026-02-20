using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosTransporte
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaTipoController : BaseController
    {
        #region Construtores

        public CargaTipoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> SalvarDadosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_SalvarDadosTransporte) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                dynamic motoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Motoristas"));
                List<int> codigosMotoristas = [];
                foreach (var motorista in motoristas)
                {
                    codigosMotoristas.Add((int)motorista.Codigo);
                }

                string mensagemValidarMotoristas = servicoCarga.CriarMensagemValidarMotoristas(codigosMotoristas, TipoServicoMultisoftware, unitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagemValidarMotoristas))
                    throw new ServicoException(mensagemValidarMotoristas);

                int codigo = int.Parse(Request.Params("codigo"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.DadosCarga dadosCarga = ObterDadosCarga(carga, codigosMotoristas);

                dynamic retorno = servicoCarga.AvancarACargaSomenteComTipoDeCargaEModeloVeicular(dadosCarga, unitOfWork);

                return new JsonpResult(retorno);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.DadosCarga ObterDadosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<int> codigosMotoristas)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.DadosCarga
            {
                Carga = carga,
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoModeloVeiculo = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoTipoContainer = Request.GetIntParam("TipoContainer"),
                Justificativa = Request.Params("Justificativa"),
                CodigoTransportador = Request.GetIntParam("CodigoTransportador", 0),
                CodigoVeiculo = Request.GetIntParam("CodigoVeiculo", 0),
                CodigoReboque = Request.GetIntParam("CodigoReboque", 0),
                CodigoSegundoReboque = Request.GetIntParam("CodigoSegundoReboque", 0),
                CodigoTerceiroReboque = Request.GetIntParam("CodigoTerceiroReboque", 0),
                Usuario = Usuario,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                Auditado = Auditado,
                CodigosMotoristas = codigosMotoristas,
            };
        }

        #endregion
    }
}
