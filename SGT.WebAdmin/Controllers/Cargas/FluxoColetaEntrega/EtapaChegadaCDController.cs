using SGTAdmin.Controllers;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class EtapaChegadaCDController : BaseController
    {
		#region Construtores

		public EtapaChegadaCDController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD repEtapaChegadaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD(unitOfWork);

                int.TryParse(Request.Params("CodigoColetaEntrega"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD etapaChegadaCD = repEtapaChegadaCD.BuscarPorFluxoColetaEntrega(codigo);

                bool etapaLiberada = true;
                if (etapaChegadaCD.FluxoColetaEntrega.EtapasOrdenadas[etapaChegadaCD.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaCD)
                    etapaLiberada = false;

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    etapaChegadaCD.Codigo,
                    Carga = servicoCarga.ObterNumeroCarga(etapaChegadaCD.FluxoColetaEntrega.Carga, configuracaoEmbarcador),
                    Filial = new { Codigo = etapaChegadaCD.Filial?.Codigo ?? 0, Descricao = etapaChegadaCD.Filial?.Descricao ?? "" },
                    DataInformada = !etapaLiberada ? etapaChegadaCD.DataInformada.ToString("dd/MM/yyyy HH:mm") : "",
                    EtapaLiberada = etapaLiberada,
                    etapaChegadaCD.LocalVeiculoFluxoColetaEntrega,
                    etapaChegadaCD.Observacao
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

                unitOfWork.Start();
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD repEtapaChegadaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Filial"), out int filial);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD etapaChegadaCD = repEtapaChegadaCD.BuscarPorCodigo(codigo, true);

                if (etapaChegadaCD == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                DateTime.TryParseExact(Request.Params("DataInformada"), "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime dataInformada);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaCD;

                bool inserindoEtapa = true;
                if (etapaChegadaCD.FluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual != etapa)
                    inserindoEtapa = false;

      
                //LocalVeiculoFluxoColetaEntrega

                DateTime? etapaTempoAnterior = null;
                if (inserindoEtapa)
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaChegadaCD.FluxoColetaEntrega, unitOfWork);
                else
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaChegadaCD.FluxoColetaEntrega, etapa, unitOfWork);

                DateTime? dataAtual = DateTime.Now;
                if (!inserindoEtapa)
                    dataAtual = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataProximaEtapa(etapaChegadaCD.FluxoColetaEntrega, etapa, unitOfWork);

                if (etapaTempoAnterior != null && etapaTempoAnterior > dataInformada)
                    return new JsonpResult(false, true, "A data informada não pode ser inferior a data anterior.");

                if (dataInformada > dataAtual)
                    return new JsonpResult(false, true, "A data informada não pode ser superior a data atual.");

                etapaChegadaCD.DataInformada = dataInformada;
                etapaChegadaCD.Observacao = Request.Params("Observacao");

                if (filial > 0)
                    etapaChegadaCD.Filial = repFilial.BuscarPorCodigo(filial);
                else
                    etapaChegadaCD.Filial = null;

                if (!Enum.TryParse(Request.Params("LocalVeiculoFluxoColetaEntrega"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalVeiculoFluxoColetaEntrega localVeiculoFluxoColetaEntrega))
                    localVeiculoFluxoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalVeiculoFluxoColetaEntrega.NaoInformado;

                etapaChegadaCD.LocalVeiculoFluxoColetaEntrega = localVeiculoFluxoColetaEntrega;

                repEtapaChegadaCD.Atualizar(etapaChegadaCD);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaChegadaCD.FluxoColetaEntrega, null, "Informou Etapa Chegada no CD", unitOfWork);
                
                if (inserindoEtapa)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(etapaChegadaCD.FluxoColetaEntrega.Carga, etapa, unitOfWork);
                else
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AtualizarTempoEtapas(etapaChegadaCD.FluxoColetaEntrega, etapa, unitOfWork);

                Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ValidarOcorrenciaPendente(etapaChegadaCD.FluxoColetaEntrega.Carga, unitOfWork);

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
