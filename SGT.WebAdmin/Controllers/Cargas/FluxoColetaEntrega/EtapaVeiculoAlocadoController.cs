using SGTAdmin.Controllers;
using System;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class EtapaVeiculoAlocadoController : BaseController
    {
		#region Construtores

		public EtapaVeiculoAlocadoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado repEtapaVeiculoAlocado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado(unitOfWork);

                int.TryParse(Request.Params("CodigoColetaEntrega"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado etapaVeiculoAlocado = repEtapaVeiculoAlocado.BuscarPorFluxoColetaEntrega(codigo);

                bool etapaLiberada = true;
                var etapasOrdenadas = etapaVeiculoAlocado.FluxoColetaEntrega.EtapasOrdenadas;
                if (etapasOrdenadas[etapaVeiculoAlocado.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.VeiculoAlocado &&
                    etapasOrdenadas[etapaVeiculoAlocado.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaFornecedor &&
                    etapasOrdenadas[etapaVeiculoAlocado.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Integracao &&
                    etapasOrdenadas[etapaVeiculoAlocado.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaCD)
                    etapaLiberada = false;

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Veiculo tracao = null;
                Dominio.Entidades.Veiculo veiculo = null;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = etapaVeiculoAlocado.Filial;
                Dominio.Entidades.Usuario motorista = etapaVeiculoAlocado.Motorista;
                Dominio.Entidades.Empresa transportador = etapaVeiculoAlocado.Transportador;

                bool exigeConfirmacaoTracao = etapaVeiculoAlocado.FluxoColetaEntrega.Carga.TipoOperacao?.ExigePlacaTracao ?? false;
                if (exigeConfirmacaoTracao)
                {
                    exigeConfirmacaoTracao = true;
                    if (etapaVeiculoAlocado.Veiculo != null)
                    {
                        if (etapaVeiculoAlocado.Veiculo.TipoVeiculo == "0")
                            tracao = etapaVeiculoAlocado.Veiculo;
                    }
                    if (etapaVeiculoAlocado.VeiculosVinculados.Count > 0)
                        veiculo = etapaVeiculoAlocado.VeiculosVinculados.FirstOrDefault();
                }
                else
                {
                    veiculo = etapaVeiculoAlocado.Veiculo;
                }

                if (veiculo == null)
                    veiculo = etapaVeiculoAlocado.FluxoColetaEntrega.Carga.VeiculosVinculados.FirstOrDefault();

                if (tracao == null)
                    tracao = etapaVeiculoAlocado.FluxoColetaEntrega.Carga.Veiculo;

                if (filial == null)
                    filial = etapaVeiculoAlocado.FluxoColetaEntrega.Carga.Filial;

                if (motorista == null)
                    motorista = etapaVeiculoAlocado.FluxoColetaEntrega.Carga.Motoristas.FirstOrDefault();

                if (transportador == null)
                    transportador = etapaVeiculoAlocado.FluxoColetaEntrega.Carga.Empresa;

                var retorno = new
                {
                    etapaVeiculoAlocado.Codigo,
                    ExigePlacaTracao = exigeConfirmacaoTracao,
                    Carga = servicoCarga.ObterNumeroCarga(etapaVeiculoAlocado.FluxoColetaEntrega.Carga, configuracaoEmbarcador),
                    Tracao = tracao != null ? new { tracao.Codigo, Descricao = tracao.Placa } : new { Codigo = 0, Descricao = "" },
                    DataInformada = etapaVeiculoAlocado.DataInformada != DateTime.MinValue ? etapaVeiculoAlocado.DataInformada.ToString("dd/MM/yyyy HH:mm") : "",
                    EtapaLiberada = etapaLiberada,
                    Filial = new { Codigo = filial?.Codigo ?? 0, Descricao = filial?.Descricao ?? "" },
                    Motorista = motorista != null ? new { motorista.Codigo, motorista.Descricao } : null,
                    Veiculo = new { Codigo = veiculo?.Codigo ?? 0, Descricao = veiculo?.Placa ?? string.Empty },
                    Transportador = transportador != null ? new { transportador.Codigo, transportador.Descricao } : null,
                    etapaVeiculoAlocado.Observacao
                };

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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado repEtapaVeiculoAlocado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoFilial = Request.GetIntParam("Filial");
                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado etapaVeiculoAlocado = repEtapaVeiculoAlocado.BuscarPorCodigo(codigo, true);

                if (etapaVeiculoAlocado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = etapaVeiculoAlocado.FluxoColetaEntrega.Carga;

                carga.Initialize();

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                {
                    Carga = carga,
                    CodigoEmpresa = Request.GetIntParam("Transportador"),
                    CodigoModeloVeicular = carga.ModeloVeicularCarga?.Codigo ?? 0,
                    CodigoMotorista = Request.GetIntParam("Motorista"),
                    CodigoTipoCarga = carga.TipoDeCarga?.Codigo ?? 0,
                    CodigoTipoOperacao = carga.ModeloVeicularCarga?.Codigo ?? 0,
                    CodigoTracao = Request.GetIntParam("Tracao"),
                    CodigoReboque = Request.GetIntParam("Veiculo")
                };
                
                etapaVeiculoAlocado.Motorista = repUsuario.BuscarPorCodigo(dadosTransporte.CodigoMotorista);
                etapaVeiculoAlocado.Transportador = repEmpresa.BuscarPorCodigo(dadosTransporte.CodigoEmpresa);
                etapaVeiculoAlocado.DataInformada = Request.GetDateTimeParam("DataInformada");
                etapaVeiculoAlocado.Observacao = Request.GetStringParam("Observacao");

                bool exigeConfirmacaoTracao = etapaVeiculoAlocado.FluxoColetaEntrega.Carga.TipoOperacao?.ExigePlacaTracao ?? false;

                if (dadosTransporte.CodigoReboque > 0)
                {
                    etapaVeiculoAlocado.Veiculo = repVeiculo.BuscarPorCodigo(dadosTransporte.CodigoReboque);

                    if (etapaVeiculoAlocado.Veiculo.TipoVeiculo == "1")
                    {
                        if (etapaVeiculoAlocado.Veiculo.VeiculosTracao != null && etapaVeiculoAlocado.Veiculo.VeiculosTracao.Count > 0)
                        {
                            Dominio.Entidades.Veiculo tracao = (from obj in etapaVeiculoAlocado.Veiculo.VeiculosTracao where obj.Ativo select obj).FirstOrDefault();

                            if (tracao != null)
                                etapaVeiculoAlocado.Veiculo = tracao;
                        }
                    }
                }

                if (etapaVeiculoAlocado.Veiculo != null)
                {
                    foreach (Dominio.Entidades.Veiculo veiculoVinculado in etapaVeiculoAlocado.Veiculo.VeiculosVinculados)
                        etapaVeiculoAlocado.VeiculosVinculados.Add(veiculoVinculado);
                }

                if (exigeConfirmacaoTracao)
                {
                    if (etapaVeiculoAlocado.Veiculo.VeiculosTracao != null && etapaVeiculoAlocado.Veiculo.VeiculosTracao.Count == 0)
                        etapaVeiculoAlocado.VeiculosVinculados.Add(etapaVeiculoAlocado.Veiculo);

                    etapaVeiculoAlocado.Veiculo = repVeiculo.BuscarPorCodigo(dadosTransporte.CodigoTracao);
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.VeiculoAlocado;
                bool inserindoEtapa = (etapaVeiculoAlocado.FluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual == etapa);
                DateTime? etapaTempoAnterior = null;

                if (inserindoEtapa)
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaVeiculoAlocado.FluxoColetaEntrega, unitOfWork);
                else
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaVeiculoAlocado.FluxoColetaEntrega, etapa, unitOfWork);

                DateTime? dataAtual = inserindoEtapa ? DateTime.Now : Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataProximaEtapa(etapaVeiculoAlocado.FluxoColetaEntrega, etapa, unitOfWork);

                if (inserindoEtapa)
                {
                    if (etapaTempoAnterior != null && etapaTempoAnterior > etapaVeiculoAlocado.DataInformada)
                        return new JsonpResult(false, true, "A data informada não pode ser inferior a data anterior.");

                    if (etapaVeiculoAlocado.DataInformada > dataAtual)
                        return new JsonpResult(false, true, "A data informada não pode ser superior a data atual.");

                    if (etapaVeiculoAlocado.Motorista == null)
                        return new JsonpResult(false, true, "Motorista é obrigatório.");

                    if (etapaVeiculoAlocado.Veiculo == null)
                        return new JsonpResult(false, true, "Veículo é obrigatório.");

                    if (etapaVeiculoAlocado.Transportador == null)
                        return new JsonpResult(false, true, "Transportador é obrigatório.");
                }

                if (codigoFilial > 0)
                    etapaVeiculoAlocado.Filial = repFilial.BuscarPorCodigo(codigoFilial);

                unitOfWork.Start();

                repEtapaVeiculoAlocado.Atualizar(etapaVeiculoAlocado);

                if (inserindoEtapa)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(etapaVeiculoAlocado.FluxoColetaEntrega.Carga, etapa, unitOfWork);
                else
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AtualizarTempoEtapas(etapaVeiculoAlocado.FluxoColetaEntrega, etapa, unitOfWork);

                var retorno = svcCarga.SalvarDadosTransporteCarga(dadosTransporte, out string mensagemErro, Usuario, false, TipoServicoMultisoftware, WebServiceConsultaCTe, Cliente, Auditado, unitOfWork);

                if (retorno == null)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                //if (inserindoEtapa)
                //    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ReplicarInformacoesAlocacao(etapaVeiculoAlocado, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaVeiculoAlocado.FluxoColetaEntrega, null, "Informou Etapa Veículo alocado", unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
