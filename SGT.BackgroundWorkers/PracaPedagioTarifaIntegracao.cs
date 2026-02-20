using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 6000)]

    public class PracaPedagioTarifaIntegracao : LongRunningProcessBase<PracaPedagioTarifaIntegracao>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao = new Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao = repPracaPedagioTarifaIntegracao.BuscarProximaIntegracaoAgIntgracao();
            if (pracaPedagioTarifaIntegracao != null)
            {
                // Registra o início do processamento da planilha, de "Pendente" para "Processando"
                pracaPedagioTarifaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                repPracaPedagioTarifaIntegracao.Atualizar(pracaPedagioTarifaIntegracao);

                // Processa a planilha importada
                ProcessarIntegracao(unitOfWork, repPracaPedagioTarifaIntegracao, pracaPedagioTarifaIntegracao, _tipoServicoMultisoftware);
            }

            AtualizarRotasFretePracasPedagio(unitOfWork);
        }

        #endregion
        
        private void ProcessarIntegracao(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao, Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                string mensagem = Servicos.Embarcador.Integracao.SemParar.ValePedagio.ConsultarRegistrarTarifasPracasPedagio(pracaPedagioTarifaIntegracao, unitOfWork, tipoServicoMultisoftware);
                IndicarFimDoProcessamento(repPracaPedagioTarifaIntegracao, pracaPedagioTarifaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, mensagem);
            }
            catch (Exception ex)
            {
                IndicarFimDoProcessamento(repPracaPedagioTarifaIntegracao, pracaPedagioTarifaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, ex.Message);
                throw;
            }
        }

        private void IndicarFimDoProcessamento(Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao, Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, string mensagem)
        {
            pracaPedagioTarifaIntegracao.NumeroTentativas += 1;
            pracaPedagioTarifaIntegracao.SituacaoIntegracao = situacao;
            pracaPedagioTarifaIntegracao.ProblemaIntegracao = mensagem;
            repPracaPedagioTarifaIntegracao.Atualizar(pracaPedagioTarifaIntegracao);
        }

        private void AtualizarRotasFretePracasPedagio(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            if (configuracaoRoteirizacao.NumeroDiasParaConsultaPracaPedagio != 0)
            {
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                List<Dominio.Entidades.RotaFrete> rotaFrete = repRotaFrete.ConsultarRotasFretePracaPedagio(configuracaoRoteirizacao.NumeroDiasParaConsultaPracaPedagio);

                Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = repIntegracaoSemParar.Buscar();

                Servicos.Embarcador.Integracao.SemParar.PracasPedagio serPracasPedagio = new Servicos.Embarcador.Integracao.SemParar.PracasPedagio();
                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = serPracasPedagio.Autenticar(unitOfWork, _tipoServicoMultisoftware);

                foreach (Dominio.Entidades.RotaFrete rota in rotaFrete)
                {
                    Repositorio.RotaFretePontosPassagem repRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(unitOfWork);
                    List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagem = repRotaFretePontosPassagem.BuscarPorRotaFrete(rota.Codigo);

                    if (credencial.Autenticado)
                    {
                        string erro = "";
                        string response = "";
                        string request = "";

                        List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioIda = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
                        List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioRetorno = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();

                        string pontosRota = BuscarPontosRota(unitOfWork, pontosPassagem);

                        if (rota.ApenasObterPracasPedagio || (!string.IsNullOrEmpty(rota.PolilinhaRota) && configuracaoRoteirizacao.SempreUtilizarRotaParaBuscarPracasPedagio))
                            pracasPedagioIda = serPracasPedagio.ObterPracasPedagioPorPolilinha(credencial, rota.PolilinhaRota, integracaoSemParar?.DistanciaMinimaQuadrante ?? 0, out erro, unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, rota);

                        else
                        {
                            pracasPedagioIda = serPracasPedagio.ObterPracasPedagioIda(credencial, pontosRota, out erro, unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out request, out response, rota);

                            if (rota.TipoUltimoPontoRoteirizacao != TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                                pracasPedagioRetorno = serPracasPedagio.ObterPracasPedagioVolta(credencial, pontosRota, out erro, unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out request, out response, rota);
                        }

                        Repositorio.RotaFretePracaPedagio repRotaFretePracas = new Repositorio.RotaFretePracaPedagio(unitOfWork);
                        repRotaFretePracas.DeletarPorRotaFrete(rota.Codigo);

                        SetarPracasPedagioRotaFrete(unitOfWork, pracasPedagioIda, rota, EixosSuspenso.Ida);
                        SetarPracasPedagioRotaFrete(unitOfWork, pracasPedagioRetorno, rota, EixosSuspenso.Volta);

                        rota.DataConsultaPracasPedido = DateTime.Now;
                        repRotaFrete.Atualizar(rota, _auditado);
                    }
                }
            }

        }

        private string BuscarPontosRota(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagem)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>();
            foreach (Dominio.Entidades.RotaFretePontosPassagem pontoPassagem in pontosPassagem)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota();

                pontoRota.descricao = pontoPassagem?.Descricao ?? string.Empty;
                if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    pontoRota.codigo = pontoPassagem.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                {
                    pontoRota.codigo = pontoPassagem.PracaPedagio.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? Convert.ToDouble(0);

                pontoRota.lat = (double)pontoPassagem.Latitude;
                pontoRota.lng = (double)pontoPassagem.Longitude;
                pontoRota.distancia = pontoPassagem.Distancia;
                pontoRota.tempo = pontoPassagem.Tempo;
                pontoRota.tempoEstimadoPermanencia = pontoPassagem.TempoEstimadoPermanenencia;
                pontoRota.tipoponto = pontoPassagem.TipoPontoPassagem;
                pontosDaRota.Add(pontoRota);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(pontosDaRota);
        }

        private void SetarPracasPedagioRotaFrete(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio, Dominio.Entidades.RotaFrete rota, EixosSuspenso eixo)
        {
            Repositorio.Embarcador.Logistica.PracaPedagio repPraca = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
            Repositorio.RotaFretePracaPedagio repRotaFretePracas = new Repositorio.RotaFretePracaPedagio(unitOfWork);

            foreach (var praca in pracasPedagio)
            {
                int codigoPraca = praca.Codigo;

                Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio = repPraca.BuscarPorCodigo(codigoPraca);

                Dominio.Entidades.RotaFretePracaPedagio rotaFretePracas = new Dominio.Entidades.RotaFretePracaPedagio()
                {
                    PracaPedagio = pracaPedagio,
                    RotaFrete = rota,
                    EixosSuspenso = eixo
                };

                repRotaFretePracas.Inserir(rotaFretePracas);
            }

        }


    }
}
