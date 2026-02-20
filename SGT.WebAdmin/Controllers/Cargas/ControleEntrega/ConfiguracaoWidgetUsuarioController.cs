using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/ControleEntrega", "Logistica/Monitoramento")]
    public class ConfiguracaoWidgetUsuarioController : BaseController
    {
        #region Construtores

        public ConfiguracaoWidgetUsuarioController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario repConfiguracaoWidgetUsuario = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario configuracao = repConfiguracaoWidgetUsuario.BuscarPorUsuario(this.Usuario.Codigo);

                var response = new
                {
                    ExibirNomeMotorista = configuracao?.ExibirNomeMotorista ?? false,
                    ExibirVersaoAplicativo = configuracao?.ExibirVersaoAplicativo ?? false,
                    ExibirNivelBateria = configuracao?.ExibirNivelBateria ?? false,
                    ExibirSinal = configuracao?.ExibirSinal ?? false,
                    ExibirTelefoneCelular = configuracao?.ExibirTelefoneCelular ?? false,

                    ExibirNumeroCarga = configuracao?.ExibirNumeroCarga ?? false,
                    ExibirProximoCliente = configuracao?.ExibirProximoCliente ?? false,
                    ExibirValorTotalProdutos = configuracao?.ExibirValorTotalProdutos ?? false,
                    ExibirNumeroPedidoCliente = configuracao?.ExibirNumeroPedidoCliente ?? false,
                    ExibirNumeroOrdemPedido = configuracao?.ExibirNumeroOrdemPedido ?? false,
                    ExibirPrimeiroSegundoTrecho = configuracao?.ExibirPrimeiroSegundoTrecho ?? false,
                    ExibirFilial = configuracao?.ExibirFilial ?? false,
                    ExibirExpedidor = configuracao?.ExibirExpedidor ?? false,
                    ExibirNumeroPedido = configuracao?.ExibirNumeroPedido ?? false,
                    ExibirTransportador = configuracao?.ExibirTransportador ?? false,
                    ExibirTipoOperacao = configuracao?.ExibirTipoOperacao ?? false,
                    ExibirPesoBruto = configuracao?.ExibirPesoBruto ?? false,
                    ExibirPesoLiquido = configuracao?.ExibirPesoLiquido ?? false,
                    ExibirTendenciaEntrega = configuracao?.ExibirTendenciaEntrega ?? false,
                    ExibirCanalEntrega = configuracao?.ExibirCanalEntrega ?? false,
                    ExibirTendenciaColeta = configuracao?.ExibirTendenciaColeta ?? false,
                    ExibirCanalVenda = configuracao?.ExibirCanalVenda ?? false,
                    ExibirMesorregiao = configuracao?.ExibirMesorregiao ?? false,
                    ExibirRegiao = configuracao?.ExibirRegiao ?? false,

                    ExibirPrevisaoProximaParada = configuracao?.ExibirPrevisaoProximaParada ?? false,
                    ExibirDistanciaRota = configuracao?.ExibirDistanciaRota ?? false,
                    ExibirTempoRota = configuracao?.ExibirTempoRota ?? false,
                    ExibirEntregaColetasRealizadas = configuracao?.ExibirEntregaColetasRealizadas ?? false,
                    ExibirPesoRestanteEntrega = configuracao?.ExibirPesoRestanteEntrega ?? false,
                    ExibirAnalistaResponsavelMonitoramento = configuracao?.ExibirAnalistaResponsavelMonitoramento ?? false,
                    ExibirPrevisaoRecalculada = configuracao?.ExibirPrevisaoRecalculada ?? false,
                    ConfiguracaoExibicaoDetalhesEntrega = !string.IsNullOrWhiteSpace(configuracao?.ConfiguracaoExibicaoDetalhesEntrega) ? JsonConvert.DeserializeObject<ConfiguracaoExibicaoDetalhesEntrega>(configuracao.ConfiguracaoExibicaoDetalhesEntrega) : null
                };

                return new JsonpResult(response);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int limite = 3;
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario repConfiguracaoWidgetUsuario = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario configuracao = repConfiguracaoWidgetUsuario.BuscarPorUsuario(this.Usuario.Codigo);

                if (configuracao == null)
                {
                    configuracao = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario()
                    {
                        Usuario = this.Usuario
                    };
                }
                else
                {
                    configuracao.Initialize();
                }

                configuracao.ExibirNomeMotorista = Request.GetBoolParam("ExibirNomeMotorista");
                configuracao.ExibirVersaoAplicativo = Request.GetBoolParam("ExibirVersaoAplicativo");
                configuracao.ExibirNivelBateria = Request.GetBoolParam("ExibirNivelBateria");
                configuracao.ExibirSinal = Request.GetBoolParam("ExibirSinal");
                configuracao.ExibirTelefoneCelular = Request.GetBoolParam("ExibirTelefoneCelular");

                configuracao.ExibirNumeroCarga = Request.GetBoolParam("ExibirNumeroCarga");
                configuracao.ExibirProximoCliente = Request.GetBoolParam("ExibirProximoCliente");
                configuracao.ExibirValorTotalProdutos = Request.GetBoolParam("ExibirValorTotalProdutos");
                configuracao.ExibirNumeroPedidoCliente = Request.GetBoolParam("ExibirNumeroPedidoCliente");
                configuracao.ExibirPrimeiroSegundoTrecho = Request.GetBoolParam("ExibirPrimeiroSegundoTrecho");
                configuracao.ExibirFilial = Request.GetBoolParam("ExibirFilial");
                configuracao.ExibirExpedidor = Request.GetBoolParam("ExibirExpedidor");
                configuracao.ExibirNumeroPedido = Request.GetBoolParam("ExibirNumeroPedido");
                configuracao.ExibirNumeroOrdemPedido = Request.GetBoolParam("ExibirNumeroOrdemPedido");
                configuracao.ExibirTransportador = Request.GetBoolParam("ExibirTransportador");
                configuracao.ExibirTipoOperacao = Request.GetBoolParam("ExibirTipoOperacao");
                configuracao.ExibirPesoBruto = Request.GetBoolParam("ExibirPesoBruto");
                configuracao.ExibirPesoLiquido = Request.GetBoolParam("ExibirPesoLiquido");
                configuracao.ExibirTendenciaEntrega = Request.GetBoolParam("ExibirTendenciaEntrega");
                configuracao.ExibirTendenciaColeta = Request.GetBoolParam("ExibirTendenciaColeta");
                configuracao.ExibirCanalEntrega = Request.GetBoolParam("ExibirCanalEntrega");
                configuracao.ExibirModalTransporte = Request.GetBoolParam("ExibirModalTransporte");
                configuracao.ExibirCanalVenda = Request.GetBoolParam("ExibirCanalVenda");
                configuracao.ExibirMesorregiao = Request.GetBoolParam("ExibirMesorregiao");
                configuracao.ExibirRegiao = Request.GetBoolParam("ExibirRegiao");

                configuracao.ExibirPrevisaoProximaParada = Request.GetBoolParam("ExibirPrevisaoProximaParada");
                configuracao.ExibirDistanciaRota = Request.GetBoolParam("ExibirDistanciaRota");
                configuracao.ExibirTempoRota = Request.GetBoolParam("ExibirTempoRota");
                configuracao.ExibirEntregaColetasRealizadas = Request.GetBoolParam("ExibirEntregaColetasRealizadas");
                configuracao.ExibirPesoRestanteEntrega = Request.GetBoolParam("ExibirPesoRestanteEntrega");
                configuracao.ExibirAnalistaResponsavelMonitoramento = Request.GetBoolParam("ExibirAnalistaResponsavelMonitoramento");
                configuracao.ExibirPrevisaoRecalculada = Request.GetBoolParam("ExibirPrevisaoRecalculada");
                configuracao.ConfiguracaoExibicaoDetalhesEntrega = Request.GetStringParam("ConfiguracaoExibicaoDetalhesEntrega");

                if (!ValidarQuantidadeConfiguracoesAtivas(configuracao, limite))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não é possível marcar mais de " + limite + " " + (limite > 1 ? "opção" : "opções"));
                }

                if (configuracao.Codigo == 0)
                    repConfiguracaoWidgetUsuario.Inserir(configuracao, Auditado);
                else
                    repConfiguracaoWidgetUsuario.Atualizar(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private bool ValidarQuantidadeConfiguracoesAtivas(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario configuracao, int limiteOpcoesAtivas)
        {
            int quantidade = 0;

            if (configuracao.ExibirNomeMotorista) quantidade++;
            if (configuracao.ExibirVersaoAplicativo) quantidade++;
            if (configuracao.ExibirNivelBateria) quantidade++;
            if (configuracao.ExibirSinal) quantidade++;

            if (configuracao.ExibirNumeroCarga) quantidade++;
            if (configuracao.ExibirProximoCliente) quantidade++;
            if (configuracao.ExibirValorTotalProdutos) quantidade++;
            if (configuracao.ExibirNumeroPedidoCliente) quantidade++;
            if (configuracao.ExibirNumeroOrdemPedido) quantidade++;
            if (configuracao.ExibirNumeroPedido) quantidade++;
            if (configuracao.ExibirTransportador) quantidade++;
            if (configuracao.ExibirTipoOperacao) quantidade++;
            if (configuracao.ExibirPesoBruto) quantidade++;
            if (configuracao.ExibirPesoLiquido) quantidade++;
            if (configuracao.ExibirCanalEntrega) quantidade++;
            if (configuracao.ExibirCanalVenda) quantidade++;
            if (configuracao.ExibirModalTransporte) quantidade++;
            if (configuracao.ExibirMesorregiao) quantidade++;
            if (configuracao.ExibirTendenciaColeta) quantidade++;
            if (configuracao.ExibirRegiao) quantidade++;

            if (configuracao.ExibirPrevisaoProximaParada) quantidade++;
            if (configuracao.ExibirDistanciaRota) quantidade++;
            if (configuracao.ExibirTempoRota) quantidade++;
            if (configuracao.ExibirEntregaColetasRealizadas) quantidade++;
            if (configuracao.ExibirPesoRestanteEntrega) quantidade++;

            if (configuracao.ExibirPrimeiroSegundoTrecho) quantidade++;
            if (configuracao.ExibirFilial) quantidade++;
            if (configuracao.ExibirAnalistaResponsavelMonitoramento) quantidade++;

            if (configuracao.ExibirTelefoneCelular) quantidade++;
            if (configuracao.ExibirPrevisaoRecalculada) quantidade++;

            if (quantidade > limiteOpcoesAtivas)
            {
                return false;
            }

            return true;
        }
    }
}
