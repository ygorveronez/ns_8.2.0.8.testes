using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public class FluxoColetaEntrega
    {
        public static void ValidarOcorrenciaPendente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega = repFluxoColetaEntrega.BuscarPorCarga(carga.Codigo);

            if (fluxoColetaEntrega != null)
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repCargaOcorrencia.BuscarPorCarga(carga.Codigo);

                if (ocorrencias.Count <= 0 || ocorrencias.All(obj => obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada || obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada || obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada))
                {
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(fluxoColetaEntrega.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Ocorrencia, unitOfWork);
                }
            }
        }       
        
        public async static Task AjustarFluxoGestaoColetaEntregaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {

            if (carga.Filial?.ControlaFluxoColetaEntrega ?? false)
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                
                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega = repFluxoColetaEntrega.BuscarPorCarga(carga.Codigo);
                if (fluxoColetaEntrega == null)
                {
                    fluxoColetaEntrega = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega
                    {
                        Carga = carga,
                        EtapaAtual = 0,
                        EtapaFluxoColetaEntregaEtapaAtual = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.AgSenha,
                        SituacaoEtapaFluxoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoColetaEntrega.Aguardando,
                        EtapaAtualLiberada = true
                    };
                    await repFluxoColetaEntrega.InserirAsync(fluxoColetaEntrega);


                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega> etapas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega>();
                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.AgSenha);

                    if (EtapaAgSenha.CriarEtapaAgSenha(fluxoColetaEntrega, unitOfWork))
                    {
                        fluxoColetaEntrega.EtapaAtual++;
                        fluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.VeiculoAlocado;
                        fluxoColetaEntrega.DataAgSenha = DateTime.Now;
                        fluxoColetaEntrega.TempoAgSenha = 0;
                    }

                    EtapaVeiculoAlocado.CriarEtapaVeiculoAlocado(fluxoColetaEntrega, unitOfWork);
                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.VeiculoAlocado);

                    EtapaSaidaCD.CriarEtapaSaidaCD(fluxoColetaEntrega, unitOfWork);
                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaCD);
                                        
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny);

                    if (tipoIntegracao != null)
                        etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Integracao);

                    EtapaChegadaFornecedor.CriarEtapaChegadaFornecedor(fluxoColetaEntrega, unitOfWork);
                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaFornecedor);

                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTe);

                    if (carga.EmpresaFilialEmissora != null && !carga.EmiteMDFeFilialEmissora)
                        etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTeSubcontratacao);

                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.MDFe);

                    if (carga.EmpresaFilialEmissora != null && carga.EmiteMDFeFilialEmissora)
                        etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTeSubcontratacao);

                    EtapaSaidaFornecedor.CriarEtapaSaidaFornecedor(fluxoColetaEntrega, unitOfWork);
                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaFornecedor);

                    EtapaChegadaCD.CriarEtapaChegadaCD(fluxoColetaEntrega, unitOfWork);
                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaCD);

                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Ocorrencia);

                    EtapaProcessoFinalizado.CriarEtapaProcessoFinalizado(fluxoColetaEntrega, unitOfWork);
                    etapas.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Finalizado);

                    await repFluxoColetaEntrega.AtualizarAsync(fluxoColetaEntrega);

                    AtualizarEtapasFluxoColetaEntrega(fluxoColetaEntrega, etapas, unitOfWork);                    
                }
            }
        }

        private static void AtualizarEtapasFluxoColetaEntrega(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega> etapas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas repFluxoColetaEntregaEtapas = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas(unitOfWork);

            for (int i = 0; i < etapas.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas fluxoColetaEntregaEtapas = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas
                {
                    EtapaFluxoColetaEntrega = etapas[i],
                    Ordem = i,
                    FluxoColetaEntrega = fluxoColetaEntrega
                };
                repFluxoColetaEntregaEtapas.Inserir(fluxoColetaEntregaEtapas);
            }
        }

        public static void SetarRejeitarEtapa(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaRejeitou, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega = repFluxoColetaEntrega.BuscarPorCarga(codigoCarga);

            if (fluxoColetaEntrega != null && fluxoColetaEntrega.EtapasOrdenadas.Any(obj => obj.EtapaFluxoColetaEntrega == etapaRejeitou))
            {

                if (fluxoColetaEntrega.EtapasOrdenadas[fluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega == etapaRejeitou)
                {
                    SetarTemposEtapa(etapaRejeitou, fluxoColetaEntrega, unitOfWork);
                }
                else
                    AprovarEtapasAnteriores(fluxoColetaEntrega.EtapaAtual, fluxoColetaEntrega, fluxoColetaEntrega.EtapasOrdenadas, unitOfWork);

                fluxoColetaEntrega.SituacaoEtapaFluxoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoColetaEntrega.Rejeitado;


                repFluxoColetaEntrega.Atualizar(fluxoColetaEntrega);
            }
        }

        public static void SetarProximaEtapa(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaAprovou, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas repFluxoColetaEntregaEtapas = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega = repFluxoColetaEntrega.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas> etapas = repFluxoColetaEntregaEtapas.BuscarPorColetaEntrega(fluxoColetaEntrega.Codigo);

            if (fluxoColetaEntrega != null && etapas.Any(obj => obj.EtapaFluxoColetaEntrega == etapaAprovou))
            {
                int indexEtapa = (from obj in etapas where obj.EtapaFluxoColetaEntrega == etapaAprovou select obj.Ordem).FirstOrDefault();
                if (fluxoColetaEntrega.EtapaAtual >= indexEtapa) //etapas.ToList()[fluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega == etapaAprovou)
                {
                    SetarTemposEtapa(etapaAprovou, fluxoColetaEntrega, unitOfWork);
                }
                else
                    AprovarEtapasAnteriores(fluxoColetaEntrega.EtapaAtual, fluxoColetaEntrega, etapas, unitOfWork);

                if (etapas.Count() == fluxoColetaEntrega.EtapaAtual + 1)
                    fluxoColetaEntrega.SituacaoEtapaFluxoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoColetaEntrega.Aprovado;
                else
                {
                    fluxoColetaEntrega.SituacaoEtapaFluxoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoColetaEntrega.Aguardando;
                    fluxoColetaEntrega.EtapaAtual = fluxoColetaEntrega.EtapaAtual + 1;
                    fluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual = etapas.ToList()[fluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega;
                    LiberarProximaEtapa(etapas.ToList()[fluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega, carga, fluxoColetaEntrega, unitOfWork);
                }
                repFluxoColetaEntrega.Atualizar(fluxoColetaEntrega);
            }
        }

        private static void AprovarEtapasAnteriores(int etapaLimite, Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas> etapas, Repositorio.UnitOfWork unitOfWork)
        {
            for (int i = 0; i < etapaLimite; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas etapa = etapas[i];
                SetarTemposEtapa(etapa.EtapaFluxoColetaEntrega, fluxoColetaEntrega, unitOfWork);
            }
        }

        private static void LiberarProximaEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);

            bool liberouEtapa = true;

            switch (etapa)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.AgSenha:
                    EtapaAgSenha.LiberarEtapaAgSenha(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.PendenciaAlocarVeiculo:
                    EtapaAgAlocarVeiculo.LiberarEtapaAgAlocarVeiculo(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.VeiculoAlocado:
                    EtapaVeiculoAlocado.LiberarEtapaVeiculoAlocado(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaCD:
                    EtapaSaidaCD.LiberarEtapaSaidaCD(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaFornecedor:
                    EtapaChegadaFornecedor.LiberarEtapaChegadaFornecedor(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTe:
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    carga.EtapaFaturamentoLiberado = true;
                    repCarga.Atualizar(carga);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.MDFe:
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTeSubcontratacao:
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaFornecedor:
                    EtapaSaidaFornecedor.LiberarEtapaSaidaFornecedor(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaCD:
                    EtapaChegadaCD.LiberarEtapaChegadaCD(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Ocorrencia:
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Finalizado:
                    EtapaProcessoFinalizado.LiberarEtapaProcessoFinalizado(fluxoColetaEntrega, unitOfWork);
                    break;
                default:
                    break;
            }

            if (liberouEtapa)
            {
                fluxoColetaEntrega.EtapaAtualLiberada = true;
                repFluxoColetaEntrega.Atualizar(fluxoColetaEntrega);
            }
        }

        private static void SetarTemposEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa, Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            switch (etapa)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.AgSenha:
                    SetarTemposAgSenha(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.PendenciaAlocarVeiculo:
                    SetarTemposAgAlocarVeiculo(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.VeiculoAlocado:
                    SetarTemposVeiculoAlocado(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaCD:
                    SetarTemposSaidaCD(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaFornecedor:
                    SetarTemposChegadaFornecedor(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTe:
                    SetarTemposEmissaoCTe(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.MDFe:
                    SetarTemposEmissaoMDFe(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTeSubcontratacao:
                    SetarTemposEmissaoCTeSubContratacao(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaFornecedor:
                    SetarTemposSaidaFornecedor(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaCD:
                    SetarTemposChegadaCD(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Ocorrencia:
                    SetarTemposAgOcorrencia(fluxoColetaEntrega, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Finalizado:
                    SetarTemposFinalizacao(fluxoColetaEntrega, unitOfWork);
                    break;
                default:
                    break;
            }
        }

        private static void SetarTemposAgSenha(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha repEtapaAgSenha = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha(unitOfWork);
            DateTime dataAtual = repEtapaAgSenha.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo).DataInformada;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.AgSenha, fluxoColetaEntrega, unitOfWork);

            fluxoColetaEntrega.DataAgSenha = dataAtual;
            fluxoColetaEntrega.TempoAgSenha = tempoMinutos;
        }

        private static void SetarTemposAgAlocarVeiculo(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo repEtapaAgAlocarVeiculo = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo(unitOfWork);
            DateTime dataAtual = repEtapaAgAlocarVeiculo.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo).DataInformada;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.PendenciaAlocarVeiculo, fluxoColetaEntrega, unitOfWork);


            fluxoColetaEntrega.DataAgPendenciaAlocarVeiculo = dataAtual;
            fluxoColetaEntrega.TempoAgPendenciaAlocarVeiculo = tempoMinutos;
        }

        private static void SetarTemposVeiculoAlocado(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado repEtapaVeiculoAlocado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado(unitOfWork);
            DateTime dataAtual = repEtapaVeiculoAlocado.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo).DataInformada;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.VeiculoAlocado, fluxoColetaEntrega, unitOfWork);

            fluxoColetaEntrega.DataVeiculoAlocado = dataAtual;
            fluxoColetaEntrega.TempoVeiculoAlocado = tempoMinutos;
        }

        private static void SetarTemposSaidaCD(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD repEtapaSaidaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD(unitOfWork);
            DateTime dataAtual = repEtapaSaidaCD.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo).DataInformada;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaCD, fluxoColetaEntrega, unitOfWork);

            fluxoColetaEntrega.DataSaidaCD = dataAtual;
            fluxoColetaEntrega.TempoSaidaCD = tempoMinutos;
        }


        private static void SetarTemposChegadaFornecedor(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor repEtapaChegadaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor(unitOfWork);
            DateTime dataAtual = repEtapaChegadaFornecedor.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo).DataInformada;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaFornecedor, fluxoColetaEntrega, unitOfWork);

            fluxoColetaEntrega.DataChegadaFornecedor = dataAtual;
            fluxoColetaEntrega.TempoChegadaFornecedor = tempoMinutos;
        }


        private static void SetarTemposEmissaoCTe(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime dataAtual = DateTime.Now;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTe, fluxoColetaEntrega, unitOfWork);

            fluxoColetaEntrega.DataEmissaoCTe = dataAtual;
            fluxoColetaEntrega.TempoEmissaoCTe = tempoMinutos;
        }

        private static void SetarTemposEmissaoMDFe(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime dataAtual = DateTime.Now;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.MDFe, fluxoColetaEntrega, unitOfWork);

            fluxoColetaEntrega.DataEmissaoMDFe = dataAtual;
            fluxoColetaEntrega.TempoEmissaoMDFe = tempoMinutos;
        }

        private static void SetarTemposEmissaoCTeSubContratacao(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime dataAtual = DateTime.Now;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTeSubcontratacao, fluxoColetaEntrega, unitOfWork);

            fluxoColetaEntrega.DataEmissaoCTeSubContratacao = dataAtual;
            fluxoColetaEntrega.TempoEmissaoCTeSubContratacao = tempoMinutos;
        }

        private static void SetarTemposChegadaCD(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD repEtapaChegadaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD(unitOfWork);
            DateTime dataAtual = repEtapaChegadaCD.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo).DataInformada;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaCD, fluxoColetaEntrega, unitOfWork);


            fluxoColetaEntrega.DataChegadaCD = dataAtual;
            fluxoColetaEntrega.TempoChegadaCD = tempoMinutos;

        }

        private static void SetarTemposAgOcorrencia(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {

            //todo: rever isso.
            //DateTime dataAtual = DateTime.Now;
            DateTime? dataEtapaAnterior = ObterDataEtapaAnterior(fluxoColetaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Ocorrencia, unitOfWork);
            //decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Ocorrencia, fluxoColetaEntrega, unitOfWork);


            fluxoColetaEntrega.DataAgOcorrencia = dataEtapaAnterior;
            fluxoColetaEntrega.TempoAgOcorrencia = 0;

        }


        private static void SetarTemposFinalizacao(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado repEtapaProcessoFinalizado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado(unitOfWork);
            DateTime dataAtual = repEtapaProcessoFinalizado.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo).DataInformada;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Finalizado, fluxoColetaEntrega, unitOfWork);


            fluxoColetaEntrega.DataFinalizacao = dataAtual;
            fluxoColetaEntrega.TempoFinalizacao = tempoMinutos;

        }
        private static void SetarTemposSaidaFornecedor(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor repEtapaSaidaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor(unitOfWork);
            DateTime dataAtual = repEtapaSaidaFornecedor.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo).DataInformada;
            decimal tempoMinutos = ObterTempoAnterior(dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaFornecedor, fluxoColetaEntrega, unitOfWork);


            fluxoColetaEntrega.DataSaidaFornecedor = dataAtual;
            fluxoColetaEntrega.TempoSaidaFornecedor = tempoMinutos;

        }


        public static DateTime? ObterDataProximaEtapa(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaAtual, Repositorio.UnitOfWork unitOfWork)
        {
            int ordem = (from obj in fluxoColetaEntrega.Etapas where obj.EtapaFluxoColetaEntrega == etapaAtual select obj.Ordem).FirstOrDefault();

            return ObterDataEtapa(fluxoColetaEntrega, ordem + 1, unitOfWork);
        }

        public static void AtualizarTempoEtapas(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaAtual, Repositorio.UnitOfWork unitOfWork)
        {
            int ordem = (from obj in fluxoColetaEntrega.Etapas where obj.EtapaFluxoColetaEntrega == etapaAtual select obj.Ordem).FirstOrDefault();
            SetarTemposEtapa(etapaAtual, fluxoColetaEntrega, unitOfWork);
            if (ordem + 1 < fluxoColetaEntrega.Etapas.Count())
            {
                if (fluxoColetaEntrega.EtapaAtual > ordem + 1 || fluxoColetaEntrega.SituacaoEtapaFluxoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoColetaEntrega.Aprovado)
                {
                    SetarTemposEtapa(fluxoColetaEntrega.EtapasOrdenadas[ordem + 1].EtapaFluxoColetaEntrega, fluxoColetaEntrega, unitOfWork);
                }
            }
        }


        public static DateTime? ObterDataEtapaAnterior(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas repFluxoColetaEntregaEtapas = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas> etapas = repFluxoColetaEntregaEtapas.BuscarPorColetaEntrega(fluxoColetaEntrega.Codigo);
            int ordem = (from obj in etapas where obj.EtapaFluxoColetaEntrega == etapaAtual select obj.Ordem).FirstOrDefault();
            return ObterDataEtapa(fluxoColetaEntrega, ordem - 1, unitOfWork);
        }

        public static DateTime? ObterDataEtapaAnterior(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterDataEtapa(fluxoColetaEntrega, fluxoColetaEntrega.EtapaAtual - 1, unitOfWork);
        }

        private static DateTime? ObterDataEtapa(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, int etapaIndex, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas repFluxoColetaEntregaEtapas = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntregaEtapas> etapas = repFluxoColetaEntregaEtapas.BuscarPorColetaEntrega(fluxoColetaEntrega.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega? etapa = null;

            if (etapaIndex >= 0 && etapaIndex < etapas.Count())
                etapa = etapas[etapaIndex].EtapaFluxoColetaEntrega;

            switch (etapa)
            {
                case null:
                    return null;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.AgSenha:
                    return fluxoColetaEntrega.DataAgSenha;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.PendenciaAlocarVeiculo:
                    return fluxoColetaEntrega.DataAgPendenciaAlocarVeiculo;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.VeiculoAlocado:
                    return fluxoColetaEntrega.DataVeiculoAlocado;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaCD:
                    return fluxoColetaEntrega.DataSaidaCD;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaFornecedor:
                    return fluxoColetaEntrega.DataChegadaFornecedor;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTe:
                    return fluxoColetaEntrega.DataEmissaoCTe;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.MDFe:
                    return fluxoColetaEntrega.DataEmissaoMDFe;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTeSubcontratacao:
                    return fluxoColetaEntrega.DataEmissaoCTeSubContratacao;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaFornecedor:
                    return fluxoColetaEntrega.DataSaidaFornecedor;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaCD:
                    return fluxoColetaEntrega.DataChegadaCD;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Ocorrencia:
                    return fluxoColetaEntrega.DataAgOcorrencia;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Finalizado:
                    return fluxoColetaEntrega.DataFinalizacao;
                default:
                    return null;
            }
        }


        public static void ReplicarSenha(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, string senha, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in repCargaPedido.BuscarPorCarga(fluxoColetaEntrega.Carga.Codigo))
            {
                pedido.Pedido.SenhaAgendamento = senha;
                repPedido.Atualizar(pedido.Pedido);
            }
        }

        //public static void ReplicarInformacoesAlocacao(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado etapaVeiculoAlocado, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

        //    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(etapaVeiculoAlocado.FluxoColetaEntrega.Carga.Codigo);

        //    if (carga.Motoristas == null)
        //        carga.Motoristas = new List<Dominio.Entidades.Usuario>();

        //    if (!carga.Motoristas.Contains(etapaVeiculoAlocado.Motorista))
        //        carga.Motoristas.Add(etapaVeiculoAlocado.Motorista);

        //    carga.Veiculo = etapaVeiculoAlocado.Veiculo;
        //    carga.Empresa = etapaVeiculoAlocado.Transportador;

        //    repCarga.Atualizar(carga);
        //}

        public static void ReplicarInformacoesAlocacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado repEtapaVeiculoAlocado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado etapaVeiculoAlocado = repEtapaVeiculoAlocado.BuscarPorCarga(carga.Codigo);

            if (etapaVeiculoAlocado != null)
            {
                if (carga.Motoristas != null)
                    etapaVeiculoAlocado.Motorista = carga.Motoristas.FirstOrDefault();

                etapaVeiculoAlocado.Veiculo = carga.Veiculo;

                if (etapaVeiculoAlocado.VeiculosVinculados == null)
                    etapaVeiculoAlocado.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                else
                    etapaVeiculoAlocado.VeiculosVinculados.Clear();

                foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
                {
                    etapaVeiculoAlocado.VeiculosVinculados.Add(veiculo);
                }

                etapaVeiculoAlocado.Transportador = carga.Empresa;

                repEtapaVeiculoAlocado.Atualizar(etapaVeiculoAlocado);
            }
        }


        public static decimal ObterTempoAnterior(DateTime dataAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaFluxoColeta, Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            double tempoMinutos = 0;
            DateTime? dataEtapaAnterior = ObterDataEtapaAnterior(fluxoColetaEntrega, etapaFluxoColeta, unitOfWork);
            if (dataEtapaAnterior != null)
                tempoMinutos = obterDiferencaEntreDatasEmMinutos(dataEtapaAnterior.Value, dataAtual);

            return (decimal)tempoMinutos;
        }


        private static double obterDiferencaEntreDatasEmMinutos(DateTime inicial, DateTime final)
        {
            TimeSpan diferenca = final - inicial;
            return diferenca.TotalMinutes;
        }

    }
}
