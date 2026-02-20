using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class Guarita : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaRetornada
    {
        #region Atributos Privados

        private readonly GuaritaBase _guaritaBase;

        #endregion Atributos Privados

        #region Construtores

        public Guarita(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public Guarita(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.Guarita, cliente)
        {
            _guaritaBase = new GuaritaBase(unitOfWork, auditado);
        }

        #endregion Construtores

        #region Métodos Privados

        private void ValidarInformacoesPesagem(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, TipoIntegracaoBalanca tipoIntegracaoBalanca, decimal peso)
        {
            Repositorio.Embarcador.Logistica.PesagemIntegracao repositorioPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao> pesagensIntegracao = repositorioPesagemIntegracao.BuscarPorPesagem(pesagem.Codigo);

            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao pesagemIntegracaoToledo = pesagensIntegracao.Where(o => o.TipoIntegracao.Tipo == TipoIntegracao.Toledo).LastOrDefault();
            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao pesagemIntegracao = pesagensIntegracao.Where(o => o.TipoIntegracaoBalanca == tipoIntegracaoBalanca).FirstOrDefault();

            if (pesagem.StatusBalanca != StatusBalanca.FalhaIntegracao)
            {
                if (pesagemIntegracaoToledo != null)
                    throw new ServicoException($"Existe um Ticket com a {pesagemIntegracaoToledo.TipoIntegracao.DescricaoTipo}, o mesmo se encontra no status {pesagem.StatusBalanca.ObterDescricao()}, não sendo permitido atualizar o peso manualmente.");

                if (pesagemIntegracao != null && pesagemIntegracao.TipoIntegracao.Tipo != TipoIntegracao.Qbit && pesagemIntegracao.TipoIntegracao.Tipo != TipoIntegracao.BalancaKIKI && pesagemIntegracao.TipoIntegracao.Tipo != TipoIntegracao.Deca)
                    throw new ServicoException($"Existe uma integração com a {pesagemIntegracao.TipoIntegracao.DescricaoTipo}, a mesma se encontra no status {pesagem.StatusBalanca.ObterDescricao()}, não sendo permitido atualizar o peso manualmente.");
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            _guaritaBase.Adicionar(fluxoGestaoPatioEtapaAdicionar, EtapaFluxoGestaoPatio.Guarita);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            _guaritaBase.DefinirCarga(fluxoGestaoPatio, carga, etapaLiberada);
        }

        public void EtapaRetornada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita != null)
            {
                guarita.Situacao = SituacaoCargaGuarita.AguardandoLiberacao;
                guarita.DataLiberacaoVeiculo = null;

                repositorioGuarita.Atualizar(guarita);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            _guaritaBase.TrocarCarga(fluxoGestaoPatio, cargaNova);
        }

        public void SalvarInformacoesPesagemFinal(Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoGuaritaDadosPesagem dadosPesagem)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCodigo(dadosPesagem.CodigoGuarita);

            if (guarita == null)
                throw new ServicoException("Guarita não foi encontrada.");

            if (guarita.PesagemInicial <= 0)
                throw new ServicoException("Não é possível salvar a pesagem final sem ter informado a pesagem inicial.");

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(guarita.FluxoGestaoPatio);

            if (sequenciaGestaoPatio.GuaritaSaidaPermiteInformarLacrePesagem && string.IsNullOrWhiteSpace(dadosPesagem.NumeroLacre))
                throw new ServicoException("O lacre na pesagem final não foi informada.");

            decimal pesagemFinalAnterior = guarita.PesagemFinal;

            guarita.Initialize();
            guarita.PesagemFinal = dadosPesagem.PesagemFinal;
            guarita.PorcentagemPerda = dadosPesagem.PorcentagemPerda;
            guarita.NumeroLacre = dadosPesagem.NumeroLacre;
            guarita.UsuarioPesagemFinal = dadosPesagem.Usuario;
            guarita.BalancaPesagemFinal = null;
            guarita.OrigemPesagemFinal = OrigemPesagemGuarita.Manual;

            repositorioCargaJanelaCarregamentoGuarita.Atualizar(guarita, _auditado);

            Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repositorioPesagem.BuscarPorGuarita(dadosPesagem.CodigoGuarita);

            if (pesagem != null && pesagemFinalAnterior != dadosPesagem.PesagemFinal) //Ideia para atualizar o peso quando o WS está com problema depois de ticket criado
            {
                ValidarInformacoesPesagem(pesagem, TipoIntegracaoBalanca.PesagemFinal, dadosPesagem.PesagemFinal);

                pesagem.Initialize();
                pesagem.PesoFinal = dadosPesagem.PesagemFinal;
                repositorioPesagem.Atualizar(pesagem, _auditado);
            }

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = guarita.FluxoGestaoPatio.GetEtapas();
            int ordemGuarita = etapas.Where(obj => obj.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioViagem).Select(x => x.Ordem).FirstOrDefault();
            int ordemAtual = guarita.FluxoGestaoPatio.GetEtapaAtual().Ordem;

            if (ordemGuarita > ordemAtual)
                Servicos.Auditoria.Auditoria.Auditar(_auditado, guarita.FluxoGestaoPatio, "Informações da Pesagem Final alteradas com a etapa ainda bloqueada", _unitOfWork);
            else
                Servicos.Auditoria.Auditoria.Auditar(_auditado, guarita.FluxoGestaoPatio, "Informações da Pesagem Final alteradas", _unitOfWork);

            Servicos.Embarcador.Carga.CargaAprovacaoPesagem servicoCargaAprovacaoPesagem = new Servicos.Embarcador.Carga.CargaAprovacaoPesagem(_unitOfWork);
            servicoCargaAprovacaoPesagem.CriarAprovacao(guarita, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
        }

        public void SalvarInformacoesPesagemInicial(Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoGuaritaDadosPesagem dadosPesagem)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCodigo(dadosPesagem.CodigoGuarita);

            if (guarita == null)
                throw new ServicoException("Guarita não foi encontrada.");

            if (dadosPesagem.PesagemInicial <= 0)
                throw new ServicoException("A pesagem inicial não foi informada.");

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(guarita.FluxoGestaoPatio);

            //if (sequenciaGestaoPatio.GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem && (dadosPesagem.QuantidadeCaixas <= 0))
            //    throw new ServicoException("A quantidade de caixas na pesagem inicial não foi informada.");

            if (sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesProdutor && !dadosPesagem.ProdutorRural.HasValue)
                throw new ServicoException("O produtor rural na pesagem inicial não foi informado.");

            decimal pesagemInicialAnterior = guarita.PesagemInicial;

            guarita.Initialize();
            guarita.PesagemInicial = dadosPesagem.PesagemInicial;
            guarita.PesagemPedido = dadosPesagem.Pedido;
            guarita.PesagemPressao = dadosPesagem.Pressao;
            guarita.PesagemProdutorRural = dadosPesagem.ProdutorRural;
            guarita.PesagemQuantidadeCaixas = dadosPesagem.QuantidadeCaixas;
            guarita.UsuarioPesagemInicial = dadosPesagem.Usuario;
            guarita.BalancaPesagemInicial = null;
            guarita.OrigemPesagemInicial = OrigemPesagemGuarita.Manual;

            repositorioCargaJanelaCarregamentoGuarita.Atualizar(guarita, _auditado);

            Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repositorioPesagem.BuscarPorGuarita(dadosPesagem.CodigoGuarita);

            if (pesagem != null && pesagemInicialAnterior != dadosPesagem.PesagemInicial) //Ideia para atualizar o peso quando o WS está com problema depois de ticket criado
            {
                ValidarInformacoesPesagem(pesagem, TipoIntegracaoBalanca.PesagemInicial, dadosPesagem.PesagemInicial);

                pesagem.Initialize();
                pesagem.PesoInicial = dadosPesagem.PesagemInicial;
                repositorioPesagem.Atualizar(pesagem, _auditado);
            }
        }

        public void SalvarObservacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita, string observacao)
        {
            if (guarita == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);

            guarita.Observacao = observacao;
            repositorioCargaJanelaCarregamentoGuarita.Atualizar(guarita);
        }

        public void ValidarRegrasPesagemInicial(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, decimal peso)
        {
            Repositorio.Embarcador.Logistica.PesagemIntegracao repositorioPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao pesagemIntegracao = repositorioPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.PesagemInicial);

            if (pesagemIntegracao?.Balanca == null || pesagemIntegracao.TipoIntegracao.Tipo != TipoIntegracao.Deca)
                return;

            if (pesagemIntegracao.Balanca.PercentualToleranciaPesagemEntrada == 0)
                return;

            decimal pesoCarga = pesagem.Guarita.CargaBase.DadosSumarizados?.PesoTotal ?? 0;
            decimal capacidade = pesagem.Guarita.CargaBase.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 0;

            if (pesoCarga > 0 && capacidade > 0 && peso > 0)
            {
                decimal calculoPercentual = ((peso + pesoCarga) / capacidade) * 100;
                decimal diferencaDoPercentual = calculoPercentual - 100;

                if (diferencaDoPercentual > 0 && diferencaDoPercentual > pesagemIntegracao.Balanca.PercentualToleranciaPesagemEntrada)
                    throw new ServicoException($"Capacidade do veículo excedida em {diferencaDoPercentual.ToString("n2")}% ({(peso + pesoCarga - capacidade).ToString("n2")} KG). Deseja continuar?");
            }
        }

        public void ValidarRegrasPesagemFinal(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, decimal peso)
        {
            Repositorio.Embarcador.Logistica.PesagemIntegracao repositorioPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao pesagemIntegracao = repositorioPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.PesagemFinal);

            if (pesagemIntegracao?.Balanca == null || pesagemIntegracao.TipoIntegracao.Tipo != TipoIntegracao.Deca)
                return;

            if (pesagemIntegracao.Balanca.PercentualToleranciaPesagemSaida == 0)
                return;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            decimal pesoEntrada = pesagem.PesoInicial;
            decimal pesoNF = repositorioPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(pesagem.Guarita.Carga.Codigo);
            decimal capacidade = pesagem.Guarita.CargaBase.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 0;

            string mensagem = "";
            if (pesoNF > 0 && pesoEntrada > 0 && peso > 0)
            {
                decimal calculoPercentual = (((peso - pesoEntrada) / pesoNF) - 1) * 100;
                if (calculoPercentual != 0 && (calculoPercentual > pesagemIntegracao.Balanca.PercentualToleranciaPesagemSaida || calculoPercentual < -pesagemIntegracao.Balanca.PercentualToleranciaPesagemSaida))
                    mensagem = $"Houve variação entre o peso teórico e peso físico das mercadorias de {calculoPercentual.ToString("n2")}% ({(peso - pesoEntrada - pesoNF).ToString("n2")} KG). ";
            }

            if (capacidade > 0)
            {
                decimal diferencaDoPercentual = ((peso / capacidade) * 100) - 100;
                if (diferencaDoPercentual > 0)
                    mensagem += $"Capacidade do veículo excedida em {diferencaDoPercentual.ToString("n2")}% ({(peso - capacidade).ToString("n2")} KG).";
            }

            if (!string.IsNullOrWhiteSpace(mensagem))
                throw new ServicoException($"{mensagem} Deseja continuar?");
        }

        #endregion Métodos Públicos

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataEntregaGuaritaPrevista.HasValue)
                fluxoGestaoPatio.DataEntregaGuaritaPrevista = preSetTempoEtapa.DataEntregaGuaritaPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (guarita.Situacao != SituacaoCargaGuarita.AguardandoLiberacao)
                throw new ServicoException("A situação atual não permite a liberação.");

            if (!guarita.EtapaGuaritaLiberada)
                throw new ServicoException("Ainda não foi autorizada a entrada do veículo.");

            guarita.Initialize();
            guarita.DataEntregaGuarita = DateTime.Now;
            guarita.Situacao = SituacaoCargaGuarita.Liberada;

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, guarita, null, $"Liberou a {(guarita.Carga == null ? "pré carga" : "carga")}", _unitOfWork);

            LiberarProximaEtapa(fluxoGestaoPatio);
            repositorioGuarita.Atualizar(guarita, _auditado);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataEntregaGuaritaPrevista = preSetTempoEtapa.DataEntregaGuaritaPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataEntregaGuarita.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgEntradaGuarita = tempoEtapaAnterior;
            fluxoGestaoPatio.DataEntregaGuarita = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null)
                return false;

            if (fluxoGestaoPatio.CargaBase.Veiculo == null)
                return false;

            guarita.EtapaGuaritaLiberada = true;
            guarita.Situacao = SituacaoCargaGuarita.AguardandoLiberacao;

            repositorioGuarita.Atualizar(guarita);

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataEntregaGuarita;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataEntregaGuaritaPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null || fluxoGestaoPatio.CargaBase.Veiculo == null)
                return;

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

            guarita.EtapaGuaritaLiberada = false;
            guarita.Situacao = (sequenciaGestaoPatio?.ChegadaVeiculo ?? false) ? SituacaoCargaGuarita.AgChegadaVeiculo : SituacaoCargaGuarita.AguardandoLiberacao;

            repositorioGuarita.Atualizar(guarita);
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgEntradaGuarita = 0;
            fluxoGestaoPatio.DataEntregaGuarita = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataEntregaGuaritaPrevista.HasValue)
                fluxoGestaoPatio.DataEntregaGuaritaReprogramada = fluxoGestaoPatio.DataEntregaGuaritaPrevista.Value.Add(tempoReprogramar);
        }

        #endregion Métodos Públicos Sobrescritos
    }
}
