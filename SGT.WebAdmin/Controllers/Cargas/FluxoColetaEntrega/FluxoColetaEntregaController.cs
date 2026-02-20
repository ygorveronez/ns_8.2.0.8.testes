using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class FluxoColetaEntregaController : BaseController
    {
		#region Construtores

		public FluxoColetaEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterFluxoColetaEntrega(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repositorioFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny);

                int codigoFilial = int.Parse(Request.Params("Filial"));
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                bool exibirCargasCanceladas = false;
                bool.TryParse(Request.Params("ExibirCargasCanceladas"), out exibirCargasCanceladas);
                

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaFluxoColetaEntrega = !string.IsNullOrEmpty(Request.Params("EtapaFluxoColetaEntrega")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega)int.Parse(Request.Params("EtapaFluxoColetaEntrega")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Todas;

                string numeroCarga = Request.Params("CodigoCargaEmbarcador");

                int inicio = int.Parse(Request.Params("inicio"));
                int limite = int.Parse(Request.Params("limite"));
                
                List<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega> fluxoColetaEntrega = await repositorioFluxoColetaEntrega.ConsultarAsync(etapaFluxoColetaEntrega, numeroCarga, dataInicial, dataFinal, codigoFilial, exibirCargasCanceladas, "Codigo", "desc", inicio, limite);
                int total = await repositorioFluxoColetaEntrega.ContarConsultaAsync(etapaFluxoColetaEntrega, numeroCarga, dataInicial, dataFinal, codigoFilial, exibirCargasCanceladas);

                List<dynamic> lista = (from obj in fluxoColetaEntrega select ObterDetalhesFluxoColetaEntrega(obj, tipoIntegracao, ConfiguracaoEmbarcador, unitOfWork)).ToList();
                return new JsonpResult(lista, total);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o fluxo de pátio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega = repFluxoColetaEntrega.BuscarPorCodigo(codigo, false);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                var retorno = ObterDetalhesFluxoColetaEntrega(fluxoColetaEntrega, tipoIntegracao, configuracaoEmbarcador, unitOfWork);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por codigo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }



        private dynamic ObterDetalhesFluxoColetaEntrega(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            string numeroSM = "";
            if (tipoIntegracao != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(fluxoColetaEntrega.Carga.Codigo, tipoIntegracao.Codigo);
                if (cargaCargaIntegracao != null)
                {
                    numeroSM = cargaCargaIntegracao.Protocolo;
                }
            }

            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCarga(fluxoColetaEntrega.Carga.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = fluxoColetaEntrega.Carga.Pedidos.FirstOrDefault().Pedido;

            string numeroPedido = String.Join(", ", (from o in fluxoColetaEntrega.Carga.Pedidos select o.Pedido.NumeroPedidoEmbarcador).Distinct());

            var retorno = new
            {
                fluxoColetaEntrega.Codigo,
                Agendamento = pedido.PrevisaoEntrega.HasValue ? pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") : "",
                NumeroPedido = numeroPedido,
                SenhaCarregamento = pedido.SenhaAgendamento,
                NumeroPedidoCliente = pedido.CodigoPedidoCliente,
                NumeroSM = numeroSM,
                SituacaoChamado = chamado?.DescricaoSituacao ?? "",
                Motorista = fluxoColetaEntrega.Carga.RetornarMotoristas,
                Carga = fluxoColetaEntrega.Carga.Codigo,
                NumeroCarregamento = servicoCarga.ObterNumeroCarga(fluxoColetaEntrega.Carga, configuracaoEmbarcador),
                Coleta = pedido.DataInicialColeta.HasValue ? pedido.DataInicialColeta.Value.ToString("dd/MM/yyyy HH:mm") : "",
                fluxoColetaEntrega.SituacaoEtapaFluxoColetaEntrega,
                Destinatario = fluxoColetaEntrega.Carga.DadosSumarizados.Destinatarios,
                Remetente = fluxoColetaEntrega.Carga.DadosSumarizados.Remetentes,
                CodigoCD = fluxoColetaEntrega.Carga.Filial?.CodigoFilialEmbarcador ?? "",
                fluxoColetaEntrega.EtapaAtual,
                DataAgSenha = fluxoColetaEntrega.DataAgSenha?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataAgPendenciaAlocarVeiculo = fluxoColetaEntrega.DataAgPendenciaAlocarVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataVeiculoAlocado = fluxoColetaEntrega.DataVeiculoAlocado?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataSaidaCD = fluxoColetaEntrega.DataSaidaCD?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataIntegracao = fluxoColetaEntrega.DataIntegracao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataChegadaFornecedor = fluxoColetaEntrega.DataChegadaFornecedor?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataEmissaoCTe = fluxoColetaEntrega.DataEmissaoCTe?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataEmissaoMDFe = fluxoColetaEntrega.DataEmissaoMDFe?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataEmissaoCTeSubContratacao = fluxoColetaEntrega.DataEmissaoCTeSubContratacao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataSaidaFornecedor = fluxoColetaEntrega.DataSaidaFornecedor?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataChegadaCD = fluxoColetaEntrega.DataChegadaCD?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataAgOcorrencia = fluxoColetaEntrega.DataAgOcorrencia?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFinalizacao = fluxoColetaEntrega.DataFinalizacao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Placas = ObterPlacas(fluxoColetaEntrega.Carga.Veiculo, fluxoColetaEntrega.Carga.VeiculosVinculados),
                Etapas = (from ept in fluxoColetaEntrega.EtapasOrdenadas
                          select new
                          {
                              ept.EtapaFluxoColetaEntrega,
                              ept.Ordem
                          }).ToList(),
            };
            return retorno;
        }
        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            if (veiculo != null)
            {
                List<string> placas = new List<string>() { veiculo.Placa };
                placas.AddRange(veiculosVinculados.Select(o => o.Placa));

                return string.Join(", ", placas);
            }
            else
                return "";

        }

    }
}
