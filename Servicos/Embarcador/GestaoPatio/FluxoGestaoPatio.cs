using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class FluxoGestaoPatio
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio _configuracaoFluxoGestaoPatio;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;

        #endregion Atributos Privados

        #region Construtores

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null, cliente: null, configuracaoEmbarcador: null, configuracaoFluxoGestaoPatio: null) { }

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, auditado: null, cliente: null, configuracaoEmbarcador: configuracaoEmbarcador, configuracaoFluxoGestaoPatio: null) { }

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio configuracaoFluxoGestaoPatio) : this(unitOfWork, auditado: null, cliente: null, configuracaoEmbarcador: null, configuracaoFluxoGestaoPatio: configuracaoFluxoGestaoPatio) { }

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : this(unitOfWork, auditado: null, cliente: cliente, configuracaoEmbarcador: null, configuracaoFluxoGestaoPatio: null) { }

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null, configuracaoEmbarcador: null, configuracaoFluxoGestaoPatio: null) { }

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, auditado, cliente: null, configuracaoEmbarcador: configuracaoEmbarcador, configuracaoFluxoGestaoPatio: null) { }

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio configuracaoFluxoGestaoPatio) : this(unitOfWork, auditado, cliente: null, configuracaoEmbarcador: null, configuracaoFluxoGestaoPatio: configuracaoFluxoGestaoPatio) { }

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : this(unitOfWork, auditado, cliente, configuracaoEmbarcador: null, configuracaoFluxoGestaoPatio: null) { }

        public FluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio configuracaoFluxoGestaoPatio)
        {
            _auditado = auditado;
            _cliente = cliente;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _configuracaoFluxoGestaoPatio = configuracaoFluxoGestaoPatio ?? new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio();
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio Adicionar(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase, Dominio.Entidades.Embarcador.Filiais.Filial filial, TipoFluxoGestaoPatio tipo, DateTime? dataPrevisaoInicioPadrao, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (!PermitirAdicionar(tipo, cargaBase, filial))
                return null;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioAdicionado = ObterFluxoGestaoPatio(cargaBase, filial, tipo);

            if (fluxoGestaoPatioAdicionado != null)
                return fluxoGestaoPatioAdicionado;

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasHabilitadas = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterEtapasOrdenadas(tipo, filial.Codigo, cargaBase.TipoOperacao?.Codigo ?? 0);

            if (etapasHabilitadas.Count == 0)
                return null;

            DateTime dataPrevisaoInicio = dataPrevisaoInicioPadrao ?? DateTime.Now;

            if ((tipo == TipoFluxoGestaoPatio.Origem) && (cargaJanelaCarregamento?.InicioCarregamento != null))
                dataPrevisaoInicio = cargaJanelaCarregamento.InicioCarregamento;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = new Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio()
            {
                Carga = cargaBase.IsCarga() ? (Dominio.Entidades.Embarcador.Cargas.Carga)cargaBase : null,
                EtapaAtual = 0,
                EtapaFluxoGestaoPatioAtual = etapasHabilitadas[0].Etapa,
                EtapaAtualLiberada = true,
                Filial = filial,
                PreCarga = !cargaBase.IsCarga() ? (Dominio.Entidades.Embarcador.PreCargas.PreCarga)cargaBase : null,
                SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Aguardando,
                Tipo = tipo,
                Veiculo = cargaBase.Veiculo
            };

            if (filial.InformarEquipamentoFluxoPatio)
                fluxoGestaoPatio.Equipamento = ObterEquipamentoVinculado(cargaBase);

            repositorioFluxoGestaoPatio.Inserir(fluxoGestaoPatio);

            AdicionarEtapas(fluxoGestaoPatio, cargaJanelaCarregamento, dataPrevisaoInicio, etapasHabilitadas);
            AdicionarTempoEtapas(fluxoGestaoPatio, cargaJanelaCarregamento, dataPrevisaoInicio, etapasHabilitadas, preSetTempoEtapa);
            DisponibilidadeVeiculo.GeraControleDisponibilidadeVeiculo(fluxoGestaoPatio, _unitOfWork);

            return fluxoGestaoPatio;
        }

        private Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas AdicionarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao etapa)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas fluxoGestaoPatioEtapas = new Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas
            {
                EtapaFluxoGestaoPatio = etapa.Etapa,
                Ordem = etapa.Ordem,
                FluxoGestaoPatio = fluxoGestaoPatio,
                EtapaLiberada = (etapa.Ordem == 0)
            };

            repositorioFluxoGestaoPatioEtapas.Inserir(fluxoGestaoPatioEtapas);

            return fluxoGestaoPatioEtapas;
        }

        private void AdicionarEtapas(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dataPrevisaoInicio, List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> ordenacaoEtapas)
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar()
            {
                CargaJanelaCarregamento = cargaJanelaCarregamento,
                DataPrevisaoInicio = dataPrevisaoInicio,
                FluxoGestaoPatio = fluxoGestaoPatio
            };

            for (int i = 0; i < ordenacaoEtapas.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao etapa = ordenacaoEtapas[i];
                IFluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapaAdicionar(etapa.Etapa, _unitOfWork, _auditado);

                fluxoGestaoPatioEtapaAdicionar.EtapaLiberada = (i == 0);

                fluxoGestaoPatioEtapa?.Adicionar(fluxoGestaoPatioEtapaAdicionar);

                fluxoGestaoPatioEtapaAdicionar.DataPrevisaoInicio = dataPrevisaoInicio.AddMinutes(etapa.Tempo);
            }
        }

        private void AdicionarFluxoGestaoPatioDestino(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.GerarFluxoPatioDestino)
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();
            bool cargaPermiteGerarFluxoPatioDestino = (carga.CargaAgrupamento == null) || configuracaoEmbarcador.GerarFluxoPatioPorCargaAgrupada || !(configuracaoGestaoPatio?.AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas ?? false);

            if (!cargaPermiteGerarFluxoPatioDestino)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = ObterCargaJanelaCarregamento(carga);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisDestino = null;

            if (carga.CargaAgrupamento != null)
                filiaisDestino = repositorioCargaPedido.BuscarFiliaisPorDestinatariosDaCargaOrigem(carga.Codigo);
            else
                filiaisDestino = repositorioCargaPedido.BuscarFiliaisPorDestinatariosDaCarga(carga.Codigo);

            if (filiaisDestino.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repositorioCargaEntrega.BuscarEntregasPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiaisDestino)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = (from o in cargaEntregas where o.Cliente != null && o.Cliente.CPF_CNPJ_SemFormato == filial.CNPJ select o).FirstOrDefault();
                DateTime? dataPrevisaoInicio = cargaEntrega?.DataPrevista ?? carga.DataInicioViagemPrevista ?? cargaJanelaCarregamento?.TerminoCarregamento ?? carga.DataCarregamentoCarga;

                Adicionar(carga, filial, TipoFluxoGestaoPatio.Destino, dataPrevisaoInicio, cargaJanelaCarregamento, preSetTempoEtapa: null);
            }
        }

        private void AdicionarPorCargasOriginais(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || !fluxoGestaoPatio.Carga.CargaAgrupada)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = ObterCargasOriginaisPorCargaAgrupada(fluxoGestaoPatio.Carga);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelasCarregamentoOriginais = repositorioCargaJanelaCarregamento.BuscarPorCargasOriginais(fluxoGestaoPatio.Carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal in cargasOriginais)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = cargasJanelasCarregamentoOriginais.Where(o => o.Carga.Codigo == cargaOriginal.Codigo).FirstOrDefault();
                Adicionar(cargaOriginal, cargaOriginal.Filial, fluxoGestaoPatio.Tipo, cargaOriginal.DataCarregamentoCarga, cargaJanelaCarregamento, preSetTempoEtapa: null);
            }
        }

        private void AdicionarTempoEtapas(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dataPrevisaoInicio, List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasHabilitadas, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa == null)
                preSetTempoEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa();

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas>();

            foreach (Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao etapa in etapasHabilitadas)
                etapas.Add(AdicionarEtapa(fluxoGestaoPatio, etapa));

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaReferencia = ObterEtapaReferenciaDefinicaoDataPrevisaoInicio(etapas);
            DateTime? dataInicioViagem = ObterDataInicioViagem(fluxoGestaoPatio, dataPrevisaoInicio);
            DateTime dataPrevisaoInicioPorEtapa = ObterDataPrevisaoInicioPorEtapaReferencia(etapaReferencia, cargaJanelaCarregamento, dataPrevisaoInicio, dataInicioViagem);

            dataPrevisaoInicioPorEtapa = DefinirDataPrevisaoInicioEtapa(fluxoGestaoPatio, etapaReferencia.EtapaFluxoGestaoPatio, dataPrevisaoInicioPorEtapa, cargaJanelaCarregamento?.TerminoCarregamento, dataInicioViagem, preSetTempoEtapa);

            DefinirDataPrevisaoInicioEtapasAnteriores(fluxoGestaoPatio, etapas, etapasHabilitadas, etapaReferencia, dataPrevisaoInicioPorEtapa, cargaJanelaCarregamento?.TerminoCarregamento, dataInicioViagem, preSetTempoEtapa);
            DefinirDataPrevisaoInicioEtapasPosteriores(fluxoGestaoPatio, etapas, etapasHabilitadas, etapaReferencia, dataPrevisaoInicioPorEtapa, cargaJanelaCarregamento?.TerminoCarregamento, dataInicioViagem, preSetTempoEtapa);

            new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork).Atualizar(fluxoGestaoPatio);
        }

        private void AnteciparEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas, EtapaFluxoGestaoPatio etapaFluxo)
        {
            int etapaAtual = fluxoGestaoPatio.EtapaAtual;
            EtapaFluxoGestaoPatio etapaFluxoAtual = etapas[etapaAtual].EtapaFluxoGestaoPatio;

            if (!ObterConfiguracaoGestaoPatio().IsPermiteAnteciparEtapa(etapaFluxo) || etapaFluxoAtual == etapaFluxo)
                return;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaAntecipar = etapas.Find(o => o.EtapaFluxoGestaoPatio == etapaFluxo);

            if (etapaAntecipar.EtapaLiberada)
                return;

            RemoverLiberacao(fluxoGestaoPatio, etapaFluxoAtual);

            etapas.Remove(etapaAntecipar);

            etapas.Insert(etapaAtual, etapaAntecipar);

            for (int indiceEtapa = etapaAtual; indiceEtapa < etapas.Count; indiceEtapa++)
                etapas[indiceEtapa].Ordem = indiceEtapa;

            fluxoGestaoPatio.EtapaAtual = etapaAtual;
            fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual = etapaAntecipar.EtapaFluxoGestaoPatio;
            fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Aguardando;

            new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork).Atualizar(fluxoGestaoPatio);

            LiberarProximaEtapa(fluxoGestaoPatio, etapaAntecipar);

            AuditarAnteciparEtapa(fluxoGestaoPatio, etapaFluxo);
        }

        private void AuditarAnteciparEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            if ((fluxoGestaoPatio != null) && (_auditado != null))
            {
                string descricaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork).ObterDescricaoEtapa(fluxoGestaoPatio, etapaFluxo)?.Descricao ?? string.Empty;

                Auditoria.Auditoria.Auditar(_auditado, fluxoGestaoPatio, null, $"Antecipou a etapa {descricaoEtapa}", _unitOfWork);
            }
        }

        private void AuditarAvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            if ((fluxoGestaoPatio != null) && (_auditado != null))
            {
                string descricaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork).ObterDescricaoEtapa(fluxoGestaoPatio, etapaFluxo)?.Descricao ?? string.Empty;

                Auditoria.Auditoria.Auditar(_auditado, fluxoGestaoPatio, null, $"Avançou a etapa {descricaoEtapa}", _unitOfWork);
            }
        }

        private void AuditarVoltarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            if ((fluxoGestaoPatio != null) && (_auditado != null))
            {
                string descricaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork).ObterDescricaoEtapa(fluxoGestaoPatio, etapaFluxo)?.Descricao ?? string.Empty;

                Auditoria.Auditoria.Auditar(_auditado, fluxoGestaoPatio, null, $"Voltou a etapa {descricaoEtapa}", _unitOfWork);
            }
        }

        private void DefinirCargaPorPreCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo, bool etapaLiberada)
        {
            IFluxoGestaoPatioEtapaAlterarCarga fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapaAlterarCarga(etapaFluxo, _unitOfWork, _auditado);

            fluxoGestaoPatioEtapa?.DefinirCarga(fluxoGestaoPatio, fluxoGestaoPatio.Carga, etapaLiberada);
        }

        private DateTime DefinirDataPrevisaoInicioEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapa, DateTime dataPrevisaoInicio, DateTime? dataFimCarregamento, DateTime? dataInicioViagem, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if ((etapa == EtapaFluxoGestaoPatio.FimCarregamento) && dataFimCarregamento.HasValue)
                dataPrevisaoInicio = dataFimCarregamento.Value;

            if ((etapa == EtapaFluxoGestaoPatio.InicioViagem) && dataInicioViagem.HasValue)
                dataPrevisaoInicio = dataInicioViagem.Value;

            if ((etapa == EtapaFluxoGestaoPatio.DocumentosTransporte) && fluxoGestaoPatio.DataFimCarregamentoPrevista.HasValue && fluxoGestaoPatio.CargaBase?.ModeloVeicularCarga?.TempoEmissaoFluxoPatio > 0)
                dataPrevisaoInicio = fluxoGestaoPatio.DataFimCarregamentoPrevista.Value.AddMinutes(fluxoGestaoPatio.CargaBase.ModeloVeicularCarga.TempoEmissaoFluxoPatio);

            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapa, _unitOfWork, _auditado, _cliente);

            fluxoGestaoPatioEtapa.DefinirDataPrevista(fluxoGestaoPatio, dataPrevisaoInicio, preSetTempoEtapa);

            return dataPrevisaoInicio;
        }

        private void DefinirDataPrevisaoInicioEtapasAnteriores(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas, List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasFilial, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaReferencia, DateTime dataPrevisaoInicioPorEtapa, DateTime? dataFimCarregamento, DateTime? dataInicioViagem, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapasAnteriores = (from etapa in etapas where etapa.Ordem < etapaReferencia.Ordem orderby etapa.Ordem descending select etapa).ToList();
            DateTime dataPrevisaoInicio = dataPrevisaoInicioPorEtapa;

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa in etapasAnteriores)
            {
                dataPrevisaoInicio = dataPrevisaoInicio.AddMinutes(-ObterTempoEtapa(etapasFilial, etapa.EtapaFluxoGestaoPatio));
                dataPrevisaoInicio = DefinirDataPrevisaoInicioEtapa(fluxoGestaoPatio, etapa.EtapaFluxoGestaoPatio, dataPrevisaoInicio, dataFimCarregamento, dataInicioViagem, preSetTempoEtapa);
            }
        }

        private void DefinirDataPrevisaoInicioEtapasPosteriores(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas, List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasFilial, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaReferencia, DateTime dataPrevisaoInicioPorEtapa, DateTime? dataFimCarregamento, DateTime? dataInicioViagem, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapasPosteriores = (from etapa in etapas where etapa.Ordem > etapaReferencia.Ordem orderby etapa.Ordem ascending select etapa).ToList();
            DateTime dataPrevisaoInicio = dataPrevisaoInicioPorEtapa;
            int tempoEtapaAnterior = ObterTempoEtapa(etapasFilial, etapaReferencia.EtapaFluxoGestaoPatio);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa in etapasPosteriores)
            {
                dataPrevisaoInicio = dataPrevisaoInicio.AddMinutes(tempoEtapaAnterior);
                dataPrevisaoInicio = DefinirDataPrevisaoInicioEtapa(fluxoGestaoPatio, etapa.EtapaFluxoGestaoPatio, dataPrevisaoInicio, dataFimCarregamento, dataInicioViagem, preSetTempoEtapa);
                tempoEtapaAnterior = ObterTempoEtapa(etapasFilial, etapa.EtapaFluxoGestaoPatio);
            }
        }

        private bool DefinirTempoEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo, DateTime dataFluxo)
        {
            decimal tempoEtapaAnterior = ObterTempoEtapaAnterior(fluxoGestaoPatio, dataFluxo);
            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapaFluxo, _unitOfWork, _auditado, _cliente);

            return fluxoGestaoPatioEtapa?.DefinirTempo(fluxoGestaoPatio, dataFluxo, tempoEtapaAnterior) ?? false;
        }

        private int DefinirTempoEtapasAnteriores(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas, EtapaFluxoGestaoPatio etapaFluxo, DateTime dataFluxo)
        {
            int aprovarAte = 0;

            while (etapas[aprovarAte].EtapaFluxoGestaoPatio != etapaFluxo)
                aprovarAte++;

            for (int i = 0; i <= aprovarAte; i++)
            {
                if (DefinirTempoEtapa(fluxoGestaoPatio, etapas[i].EtapaFluxoGestaoPatio, dataFluxo))
                    ExecutarAcoesPorEtapaFinalizada(fluxoGestaoPatio, etapas[i].EtapaFluxoGestaoPatio, dataFluxo);
            }

            return aprovarAte + 1;
        }

        private void EtapaInicioViagemAvancada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            if ((etapaFluxo == EtapaFluxoGestaoPatio.InicioViagem) && (fluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem))
            {
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

                fluxoGestaoPatio.VeiculoAtivo = true;

                repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
            }
        }

        private void EtapaRetornarda(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            IFluxoGestaoPatioEtapaRetornada fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapaRetornada(etapaFluxo, _unitOfWork);

            fluxoGestaoPatioEtapa?.EtapaRetornada(fluxoGestaoPatio);
        }

        private void ExecutarAcoesPorEtapaFinalizada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo, DateTime dataFluxo)
        {
            if (fluxoGestaoPatio.Carga == null)
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (fluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem)
            {
                CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo servicoOcorrenciaAutomatica = new CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(_unitOfWork, configuracaoEmbarcador);
                FluxoGestaoPatioIntegracao servicoFluxoGestaoPatioIntegracao = new FluxoGestaoPatioIntegracao(_unitOfWork);
                MultaAtrasoRetirada servicoMultaAtrasoRetirada = new MultaAtrasoRetirada(_unitOfWork, configuracaoEmbarcador);

                servicoOcorrenciaAutomatica.GerarOcorrenciaPorFinalizacaoEtapaFluxoPatio(fluxoGestaoPatio.Carga, etapaFluxo, dataFluxo, TipoServicoMultisoftware.MultiEmbarcador, _cliente);
                servicoMultaAtrasoRetirada.GerarOcorrenciaPorFinalizacaoEtapaFluxoPatio(fluxoGestaoPatio.Carga, etapaFluxo, dataFluxo, TipoServicoMultisoftware.MultiEmbarcador, _cliente);
                Pedido.Pedido.VerificarOcorrenciaPedidoPorEtapaFluxoPatio(fluxoGestaoPatio.Carga, etapaFluxo, dataFluxo, _cliente, _unitOfWork);
                servicoFluxoGestaoPatioIntegracao.AdicionarIntegracoes(fluxoGestaoPatio, etapaFluxo);

                if (etapaFluxo == EtapaFluxoGestaoPatio.InicioViagem)
                {
                    FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
                    Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);
                    Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

                    if (fluxoGestaoPatio.EmissaoCargaLiberada)
                    {
                        if (sequenciaGestaoPatio?.GuaritaSaidaIniciarEmissaoDocumentosTransporte ?? false)
                        {
                            string mensagem;
                            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                            svcCarga.IniciarEmissaoDocumentosCarga(out mensagem, out object retorno, fluxoGestaoPatio.Carga, _auditado?.Usuario, _auditado, configuracaoEmbarcador, TipoServicoMultisoftware.MultiEmbarcador, _unitOfWork, null);
                        }
                    }

                    if (configuracaoGestaoPatio?.GuaritaSaidaIniciarViagemControleEntregaAoFinalizarEtapa ?? false)
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(fluxoGestaoPatio.Carga.Codigo, DateTime.Now, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, configuracaoEmbarcador, TipoServicoMultisoftware.MultiEmbarcador, null, _auditado, _unitOfWork);
                }

                if (etapaFluxo == EtapaFluxoGestaoPatio.FimCarregamento)
                {
                    Integracao.Loggi.IntegracaoLoggi servicoIntegracaoLoggi = new Integracao.Loggi.IntegracaoLoggi(_unitOfWork, _auditado);
                    Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes repositorioCargaPedidoIntegracaoPacotes = new Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes(_unitOfWork);

                    servicoIntegracaoLoggi.GerarRegistroIntegracoesCargaPedidoPacote(fluxoGestaoPatio.Carga.Codigo);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes> cargaPedidoIntegracaoPacote = repositorioCargaPedidoIntegracaoPacotes.BuscarPorCargaPedidoIntegracaoPacotes(fluxoGestaoPatio.Carga.Codigo);
                    foreach (var integracaoPendente in cargaPedidoIntegracaoPacote)
                        servicoIntegracaoLoggi.ConsultarPacotesCarga(integracaoPendente, true);
                }
            }
            else
            {
                Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Logistica.CargaJanelaDescarregamento(_unitOfWork, _auditado);

                servicoCargaJanelaDescarregamento.AtualizarSituacao(fluxoGestaoPatio.Carga, etapaFluxo);
            }
        }

        private void ExecutarAcoesPorEtapaLiberada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            if (fluxoGestaoPatio.Carga == null)
                return;

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

            if ((etapaFluxo == EtapaFluxoGestaoPatio.Guarita) && (sequenciaGestaoPatio?.GuaritaEntradaPermiteInformacoesPesagem ?? false))//Pesagem Inicial
            {
                Servicos.Embarcador.Logistica.Pesagem servicoPesagem = new Servicos.Embarcador.Logistica.Pesagem(_unitOfWork);
                servicoPesagem.GerarPesagemInicialPorFluxoGestaoPatio(fluxoGestaoPatio, sequenciaGestaoPatio);
            }
            else if ((etapaFluxo == EtapaFluxoGestaoPatio.InicioViagem) && (sequenciaGestaoPatio?.GuaritaSaidaPermiteInformacoesPesagem ?? false)) //Pesagem Final
            {
                Servicos.Embarcador.Logistica.Pesagem servicoPesagem = new Servicos.Embarcador.Logistica.Pesagem(_unitOfWork);
                servicoPesagem.GerarPesagemFinalPorFluxoGestaoPatio(fluxoGestaoPatio, sequenciaGestaoPatio);
            }
            else if ((etapaFluxo == EtapaFluxoGestaoPatio.CheckList) && (sequenciaGestaoPatio?.CheckListUtilizarVigencia ?? false))
            {
                Servicos.Embarcador.GestaoPatio.CheckList servicoCheckList = new Servicos.Embarcador.GestaoPatio.CheckList(_unitOfWork, _auditado, _cliente);
                servicoCheckList.ResponderPerguntasComCheckListAnterior(fluxoGestaoPatio);
            }
        }

        private void ExecutarAcoesPorFluxoPatioFinalizado(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (fluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem)
                AdicionarFluxoGestaoPatioDestino(fluxoGestaoPatio.Carga);

            if (fluxoGestaoPatio.Veiculo != null)
                DisponibilidadeVeiculo.FimViagemDisponibilidadeVeiculo(fluxoGestaoPatio.Veiculo.Codigo, _unitOfWork);

            if (fluxoGestaoPatio.Carga != null)
            {
                Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);

                servicoCarga.SetarPrevisaoEntregaPedidos(fluxoGestaoPatio.Carga, _unitOfWork);
            }
        }

        private bool FluxosGestaoPatioCompativeisPorCargaAgrupada(List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosCargasOriginais, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoCargaAgrupada)
        {
            for (int i = 0; i < fluxosCargasOriginais.Count; i++)
            {
                if (!Enumerable.SequenceEqual(fluxoCargaAgrupada.Etapas.Select(obj => obj.EtapaFluxoGestaoPatio).ToList(), fluxosCargasOriginais[i].Etapas.Select(obj => obj.EtapaFluxoGestaoPatio).ToList()))
                    return false;
            }

            return true;
        }

        private void LiberarEtapaPosteriorEtapaPosicao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas)
        {
            EtapaFluxoGestaoPatio etapaAtual = etapas[fluxoGestaoPatio.EtapaAtual].EtapaFluxoGestaoPatio;
            bool possuiEtapaPosterior = (etapas.Count() > (fluxoGestaoPatio.EtapaAtual + 1));

            if (possuiEtapaPosterior && (etapaAtual == EtapaFluxoGestaoPatio.Posicao))
            {
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas fluxoGestaoPatioEtapaSeguinte = etapas[fluxoGestaoPatio.EtapaAtual + 1];

                fluxoGestaoPatioEtapaSeguinte.EtapaLiberada = true;

                repositorioFluxoGestaoPatioEtapas.Atualizar(fluxoGestaoPatioEtapaSeguinte);
            }
        }

        private void LiberarProximaEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaAtual)
        {
            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapaAtual.EtapaFluxoGestaoPatio, _unitOfWork, _auditado, _cliente);
            bool liberouEtapa = fluxoGestaoPatioEtapa?.Liberar(fluxoGestaoPatio) ?? true;

            if (!liberouEtapa)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);

            fluxoGestaoPatio.EtapaAtualLiberada = true;
            etapaAtual.EtapaLiberada = true;

            repositorioFluxoGestaoPatioEtapas.Atualizar(etapaAtual);
            repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);

            if (fluxoGestaoPatio.CargaBase.IsCarga())
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                repositorioCargaPedido.BloquearLiberacaoProximaEtapaFluxoGestaoPatioPorCarga(fluxoGestaoPatio.CargaBase.Codigo);
            }

            ExecutarAcoesPorEtapaLiberada(fluxoGestaoPatio, etapaAtual.EtapaFluxoGestaoPatio);
        }

        private void LiberarProximaEtapaAutomaticamente(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando)
                return;

            IFluxoGestaoPatioEtapaLiberarAutomaticamente fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapaLiberarAutomaticamente(fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual, _unitOfWork, _auditado);

            fluxoGestaoPatioEtapa?.LiberarProximaEtapaAutomaticamente(fluxoGestaoPatio);
        }

        private void NotificarAppMotoristasMudancaFluxo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (_cliente == null)
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (!configuracaoGestaoPatio.IsNotificarMotoristaApp(fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual))
                return;

            List<Dominio.Entidades.Usuario> motoristas = (from o in fluxoGestaoPatio.Carga.ListaMotorista where o.CodigoMobile > 0 select o).ToList();

            if (motoristas.Count == 0)
                return;

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            string descricaoEtapa = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(fluxoGestaoPatio)?.Descricao ?? string.Empty;

            dynamic conteudo = new
            {
                pt = $"Sua carga passou para a etapa {descricaoEtapa} em seu fluxo de pátio",
                es = $"Su carga ha pasado al paso {descricaoEtapa}",
                en = $"Your cargo has passed to the step {descricaoEtapa}",
            };

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
                    conteudo,
                    _cliente.Codigo,
                    motorista?.CodigoMobile ?? 0,
                    Dominio.MSMQ.MSMQQueue.SGTMobile,
                    Dominio.SignalR.Hubs.Mobile,
                    SignalR.Mobile.GetHub(MobileHubs.PushNotificationGenerica)
                );

                MSMQ.MSMQ.SendPrivateMessage(notification);
            }
        }

        private void NotificarSignalR(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (_cliente == null)
                return;

            var conteudo = new
            {
                fluxoGestaoPatio.Codigo
            };

            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
                conteudo,
                _cliente.Codigo,
                Dominio.MSMQ.MSMQQueue.SGTWebAdmin,
                Dominio.SignalR.Hubs.GestaoPatio,
                SignalR.Hubs.GestaoPatio.GetHub(SignalR.Hubs.GestaoPatioHubs.FluxoCargaAtualizado)
            );

            MSMQ.MSMQ.SendPrivateMessage(notification);
        }

        private bool PermitirAdicionar(TipoFluxoGestaoPatio tipo, Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            if ((tipo == TipoFluxoGestaoPatio.Origem) && !(cargaBase.TipoOperacao?.HabilitarGestaoPatio ?? false))
                return false;

            if ((tipo == TipoFluxoGestaoPatio.Destino) && !(cargaBase.TipoOperacao?.HabilitarGestaoPatioDestino ?? false))
                return false;

            if ((cargaBase.Filial == null) || (filial == null))
                return false;

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(tipo, filial.Codigo, cargaBase.TipoOperacao?.Codigo ?? 0);

            if (sequenciaGestaoPatio == null)
                return false;

            if (cargaBase.Filial.Codigo == filial.Codigo)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                List<Dominio.Entidades.Cliente> expedidores = repositorioCargaPedido.BuscarExpedidoresPorCarga(cargaBase.Codigo);
                bool expedidorFilial = expedidores.Any(obj => obj.CPF_CNPJ_SemFormato == cargaBase.Filial.CNPJ);

                if (expedidores.Count > 0 && cargaBase.FilialOrigem == null && !configuracaoEmbarcador.NaoExibirInfosAdicionaisGridPatio && !configuracaoJanelaCarregamento.GerarFluxoPatioCargaComExpedidor && !expedidorFilial)
                    return false;
            }


            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(_unitOfWork);

            if (repositorioCarga.GerarFluxoPatioAposConfirmacaoAgendamento(cargaBase.Codigo))
            {
                SituacaoCargaJanelaDescarregamento[] situacoesCargaJanelaDescarregamento = { SituacaoCargaJanelaDescarregamento.AguardandoGeracaoSenha, SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento };
                if (!repositorioCarga.ExisteCargaJanelaDescarregamentoPorFilialDestino(cargaBase.Codigo, situacoesCargaJanelaDescarregamento))
                    return false;
            }

            return true;
        }

        private void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapaFluxo, _unitOfWork, _auditado, _cliente);

            fluxoGestaoPatioEtapa?.RemoverLiberacao(fluxoGestaoPatio);
        }

        private void RemoverTempoEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapaFluxo, _unitOfWork, _auditado, _cliente);

            fluxoGestaoPatioEtapa?.RemoverTempo(fluxoGestaoPatio);
        }

        private void ReprogramarTempoEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa, TimeSpan tempoReprogramar)
        {
            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapa.EtapaFluxoGestaoPatio, _unitOfWork, _auditado, _cliente);

            fluxoGestaoPatioEtapa?.ReprogramarTempo(etapa.FluxoGestaoPatio, tempoReprogramar);
        }

        /// <summary>
        /// Regra da Carrefour: reprograma as próximas etapas somente a partir da etapa InicioViagem
        /// </summary>
        /// <param name="fluxoGestaoPatio"></param>
        /// <param name="etapaPreSequencia"></param>
        /// <param name="etapas"></param>
        private void ReprogramaTempoProximasEtapas(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaPreSequencia, List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas)
        {
            if ((etapaPreSequencia == EtapaFluxoGestaoPatio.InicioViagem) && fluxoGestaoPatio.DataInicioViagem.HasValue && fluxoGestaoPatio.DataInicioViagemPrevista.HasValue)
            {
                TimeSpan tempoReprogramar = fluxoGestaoPatio.DataInicioViagem.Value - fluxoGestaoPatio.DataInicioViagemPrevista.Value;

                for (int i = fluxoGestaoPatio.EtapaAtual; i < etapas.Count; i++)
                    ReprogramarTempoEtapa(etapas[i], tempoReprogramar);
            }
        }

        private void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioAdicionado = ObterFluxoGestaoPatio(cargaNova, filial, fluxoGestaoPatio.Tipo);

            if (fluxoGestaoPatioAdicionado != null)
                return;

            if (fluxoGestaoPatio.Carga.EtapaFaturamentoLiberado && !cargaNova.EtapaFaturamentoLiberado)
            {
                cargaNova.EtapaFaturamentoLiberado = true;
                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(cargaNova);
            }

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            fluxoGestaoPatio.Carga = cargaNova;
            fluxoGestaoPatio.Filial = filial;

            repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa in etapas)
            {
                IFluxoGestaoPatioEtapaAlterarCarga fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapaAlterarCarga(etapa.EtapaFluxoGestaoPatio, _unitOfWork, _auditado);

                fluxoGestaoPatioEtapa?.TrocarCarga(fluxoGestaoPatio, cargaNova);
            }
        }

        private void ValidarPermissaoIniciar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = fluxoGestaoPatio.Carga;

            if (carga == null)
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (!configuracaoGestaoPatio.IniciarFluxoPatioSomenteComCarregamentoAgendado)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento;

            if (fluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem)
                cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarCarregamentoPorCargaEFilial(carga.Codigo, fluxoGestaoPatio.Filial.Codigo);
            else
                cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarDescarregamentoPorCargaEFilial(carga.Codigo, fluxoGestaoPatio.Filial.Codigo);

            if (cargaJanelaCarregamento == null)
                return;

            bool carregamentoAgendado;

            if (fluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem)
                carregamentoAgendado = (!cargaJanelaCarregamento.Excedente && (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.ProntaParaCarregamento));
            else
                carregamentoAgendado = (!cargaJanelaCarregamento.Excedente && (carga.Empresa != null) && (carga.Veiculo != null) && (carga.Motoristas?.Count > 0));

            if (carregamentoAgendado)
                return;

            throw new ServicoException($"O {cargaJanelaCarregamento.Tipo.ObterDescricao().ToLower()} deve ser agendado antes de iniciar o fluxo de pátio");
        }

        private void ValidarPermissaoLiberarProximaEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (_configuracaoFluxoGestaoPatio.LiberarComMensagemSemComfirmacao)
                return;

            MensagemAlertaFluxoGestaoPatio servicoMensagemAlerta = new MensagemAlertaFluxoGestaoPatio(_unitOfWork);

            if (servicoMensagemAlerta.IsMensagemSemConfirmacao(fluxoGestaoPatio, TipoMensagemAlerta.AlteracaoPedidos))
                throw new ServicoException("As alterações de pedidos devem ser confirmadas.");

            if (
                (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InicioViagem) &&
                servicoMensagemAlerta.IsMensagemSemConfirmacao(fluxoGestaoPatio, new List<TipoMensagemAlerta>() { TipoMensagemAlerta.CargaSemRegraAutorizacaoTolerenciaPesagem, TipoMensagemAlerta.CargaAguardandoAprovacaoPesagem })
            )
                throw new ServicoException("A pesagem da carga deve ser aprovada.");
        }

        private void VoltarEtapaAnterior(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaAtual = etapas[fluxoGestaoPatio.EtapaAtual];
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaRetornar = etapas[fluxoGestaoPatio.EtapaAtual - 1];

            etapaAtual.EtapaLiberada = false;

            repositorioFluxoGestaoPatioEtapas.Atualizar(etapaAtual);
            RemoverTempoEtapa(fluxoGestaoPatio, etapaAtual.EtapaFluxoGestaoPatio);

            fluxoGestaoPatio.EtapaAtual--;
            fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Aguardando;
            fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual = etapaRetornar.EtapaFluxoGestaoPatio;

            RemoverTempoEtapa(fluxoGestaoPatio, etapaRetornar.EtapaFluxoGestaoPatio);
            EtapaRetornarda(fluxoGestaoPatio, etapaRetornar.EtapaFluxoGestaoPatio);
            repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
            AuditarVoltarEtapa(fluxoGestaoPatio, etapaAtual.EtapaFluxoGestaoPatio);
        }

        #endregion Métodos Privados

        #region Métodos Privados de Consulta

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento ObterCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            if (!cargaBase.IsCarga())
                return repositorioCargaJanelaCarregamento.BuscarPorPreCarga(cargaBase.Codigo);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = (Dominio.Entidades.Embarcador.Cargas.Carga)cargaBase;
            bool utilizarCargaJanelaCarregamentoDaCargaAgrupada = (configuracaoEmbarcador.GerarFluxoPatioPorCargaAgrupada && (carga.CargaAgrupamento != null) && (carga.Filial?.Codigo == carga.CargaAgrupamento.Filial?.Codigo));

            if (utilizarCargaJanelaCarregamentoDaCargaAgrupada)
                return repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.CargaAgrupamento.Codigo);

            return repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.Carga> ObterCargasOriginaisPorCargaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = repositorioCarga.BuscarCargasOriginais(carga.Codigo);

            return cargasOriginais;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio ObterConfiguracaoGestaoPatio()
        {
            FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new FluxoGestaoPatioConfiguracao(_unitOfWork);

            return servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();
        }

        private DateTime? ObterDataEtapaAnterior(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            EtapaFluxoGestaoPatio? etapaAnterior = ObterEtapaAnterior(fluxoGestaoPatio);

            if (etapaAnterior.HasValue)
                return ObterDataEtapa(fluxoGestaoPatio, etapaAnterior.Value);

            return null;
        }

        private DateTime? ObterDataInicioViagem(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevisaoInicio)
        {
            if (fluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Destino)
                return null;

            TimeSpan? horaLimiteSaidaCD = fluxoGestaoPatio.CargaBase.Rota?.HoraLimiteSaidaCD;

            if (!horaLimiteSaidaCD.HasValue)
                return null;

            return dataPrevisaoInicio.Date.AddMinutes(horaLimiteSaidaCD.Value.TotalMinutes);
        }

        private DateTime ObterDataPrevisaoInicioPorEtapaReferencia(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaReferencia, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dataPrevisaoInicio, DateTime? dataInicioViagem)
        {
            if (etapaReferencia.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem)
            {
                if (etapaReferencia.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioCarregamento)
                    return cargaJanelaCarregamento?.InicioCarregamento ?? dataPrevisaoInicio;

                if (etapaReferencia.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.FimCarregamento)
                    return cargaJanelaCarregamento?.TerminoCarregamento ?? dataPrevisaoInicio;

                if (etapaReferencia.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioViagem)
                    return dataInicioViagem ?? dataPrevisaoInicio;
            }

            return dataPrevisaoInicio;
        }

        private DateTime? ObterDataPrevisaoInicioPorEtapaReferenciaParaAtualizarTempoEtapas(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaReferencia, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime? dataInicioViagem)
        {
            if (etapaReferencia.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem)
            {
                if (etapaReferencia.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioCarregamento)
                    return cargaJanelaCarregamento.InicioCarregamento;

                if (etapaReferencia.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.FimCarregamento)
                    return cargaJanelaCarregamento.TerminoCarregamento;

                if (etapaReferencia.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioViagem)
                    return dataInicioViagem;

                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = ObterConfiguracaoGestaoPatio();

                if (configuracao.SempreAtualizarDataPrevistaAoAlterarHorarioCarregamento)
                    return cargaJanelaCarregamento.InicioCarregamento;
            }

            return null;
        }

        private Dominio.Entidades.Embarcador.Veiculos.Equipamento ObterEquipamentoVinculado(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            if (cargaBase.IsCarga())
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                return repositorioCarga.BuscarPrimeiroEquipamentoVinculado(cargaBase.Codigo);
            }

            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);

            return repositorioPreCarga.BuscarPrimeiroEquipamentoVinculado(cargaBase.Codigo);
        }

        private Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas ObterEtapaReferenciaDefinicaoDataPrevisaoInicio(List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas)
        {
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas primeiraEtapa = etapas.FirstOrDefault();

            if (primeiraEtapa.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaInicioCarregamento = (from etapa in etapas where etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioCarregamento select etapa).FirstOrDefault();

                if (etapaInicioCarregamento != null)
                    return etapaInicioCarregamento;

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaFimCarregamento = (from etapa in etapas where etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.FimCarregamento select etapa).FirstOrDefault();

                if (etapaFimCarregamento != null)
                    return etapaFimCarregamento;

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaInicioViagem = (from etapa in etapas where etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioViagem select etapa).FirstOrDefault();

                if (etapaInicioViagem != null)
                {
                    if (primeiraEtapa.FluxoGestaoPatio.CargaBase.Rota?.HoraLimiteSaidaCD != null)
                        return etapaInicioViagem;
                }
            }

            return primeiraEtapa;
        }

        private EtapaFluxoGestaoPatio? ObterEtapaAnterior(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            if (etapas.Count == 0)
                return null;

            if (etapas[fluxoGestaoPatio.EtapaAtual].EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.Expedicao && !fluxoGestaoPatio.DataFimCarregamento.HasValue && fluxoGestaoPatio.DataInicioCarregamento.HasValue)
                return EtapaFluxoGestaoPatio.Expedicao;

            if (fluxoGestaoPatio.EtapaAtual > 0)
                return etapas[fluxoGestaoPatio.EtapaAtual - 1].EtapaFluxoGestaoPatio;

            return null;
        }

        private List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> ObterEtapas(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);

            return repositorioFluxoGestaoPatioEtapas.BuscarPorGestao(fluxoGestaoPatio.Codigo);
        }

        private Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio ObterFluxoGestaoPatio(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase, Dominio.Entidades.Embarcador.Filiais.Filial filial, TipoFluxoGestaoPatio tipo)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            if (cargaBase.IsCarga())
                return repositorioFluxoGestaoPatio.BuscarPorCargaFilialETipo(cargaBase.Codigo, filial?.Codigo ?? 0, tipo);

            return repositorioFluxoGestaoPatio.BuscarPorPreCargaFilialETipo(cargaBase.Codigo, filial?.Codigo ?? 0, tipo);
        }

        private Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio ObterFluxoGestaoPatioMaisAvancadoPorCargaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!carga.CargaAgrupada)
                return null;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioCargaAgrupada = ObterFluxoGestaoPatio(carga);

            if (fluxoGestaoPatioCargaAgrupada != null)
                return null;

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = ObterCargasOriginaisPorCargaAgrupada(carga);
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatioCargaOriginal = new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal in cargasOriginais)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioCargaOriginal = ObterFluxoGestaoPatio(cargaOriginal);

                if (fluxoGestaoPatioCargaOriginal?.EtapaAtual > 0)
                    fluxosGestaoPatioCargaOriginal.Add(fluxoGestaoPatioCargaOriginal);
            }

            return fluxosGestaoPatioCargaOriginal.OrderByDescending(o => o.EtapaAtual).FirstOrDefault();
        }

        private Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio ObterFluxoGestaoPatioPorCargaCancelada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (!(configuracaoGestaoPatio?.UtilizarFluxoPatioCargaCanceladaAoReenviarCarga ?? false))
                return null;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaCancelada = repositorioCarga.BuscarPorCargaCancelada(carga.CodigoCargaEmbarcador, carga.Filial?.Codigo ?? 0);

            if (cargaCancelada == null)
                return null;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioCargaCancelada = ObterFluxoGestaoPatio(cargaCancelada, cargaCancelada.Filial, TipoFluxoGestaoPatio.Origem);

            if (fluxoGestaoPatioCargaCancelada?.EtapaAtual > 0)
            {
                if (cargaCancelada.EtapaFaturamentoLiberado)
                    VoltarAteEtapa(fluxoGestaoPatioCargaCancelada, EtapaFluxoGestaoPatio.Faturamento);

                return fluxoGestaoPatioCargaCancelada;
            }

            return null;
        }

        private int ObterTempoEtapa(List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasFilial, EtapaFluxoGestaoPatio etapaFluxo)
        {
            return (from etapa in etapasFilial where etapa.Etapa == etapaFluxo select etapa.Tempo).FirstOrDefault();
        }

        #endregion Métodos Privados de Consulta

        #region Métodos Públicos

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Adicionar(cargaBase, tipoServicoMultisoftware, cargaJanelaCarregamento: null, preSetTempoEtapa: null);
        }

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Adicionar(cargaBase, tipoServicoMultisoftware, cargaJanelaCarregamento, preSetTempoEtapa: null);
        }

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();
            if (cargaBase.IsCarga())
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = (Dominio.Entidades.Embarcador.Cargas.Carga)cargaBase;
                bool fluxoGestaoPatioOrigemAdicionado = false;

                if (carga.CargaDePreCarga && repositorioCarga.ExisteCargaFechadaPorPreCarga(carga.Codigo))
                    return;

                if (carga.CargaAgrupada && configuracaoEmbarcador.GerarFluxoPatioPorCargaAgrupada)
                {
                    carga.OcultarNoPatio = true;
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = ObterCargasOriginaisPorCargaAgrupada(carga);

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal in cargasOriginais)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoOrigem = null;
                        if (cargaOriginal.Filial?.Codigo != carga.Filial?.Codigo)
                            cargaJanelaCarregamentoOrigem = repositorioCargaJanelaCarregamento.BuscarPorCarga(cargaOriginal.Codigo);

                        if (cargaJanelaCarregamentoOrigem == null)
                            cargaJanelaCarregamentoOrigem = cargaJanelaCarregamento;

                        Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = Adicionar(cargaOriginal, cargaOriginal.Filial, TipoFluxoGestaoPatio.Origem, cargaOriginal.DataCarregamentoCarga, cargaJanelaCarregamentoOrigem, preSetTempoEtapa: preSetTempoEtapa);

                        if (fluxoGestaoPatio != null)
                            fluxoGestaoPatioOrigemAdicionado = true;

                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio;

                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioMaisAvancadoPorCargaAgrupada = null;
                    bool adicionarFluxoGestaoPatioCargasOriginais = configuracaoGestaoPatio?.AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas ?? false;

                    if (!adicionarFluxoGestaoPatioCargasOriginais)
                        fluxoGestaoPatioMaisAvancadoPorCargaAgrupada = ObterFluxoGestaoPatioMaisAvancadoPorCargaAgrupada(carga);

                    if (fluxoGestaoPatioMaisAvancadoPorCargaAgrupada != null)
                    {
                        TrocarCarga(fluxoGestaoPatioMaisAvancadoPorCargaAgrupada, carga, carga.Filial);
                        fluxoGestaoPatio = fluxoGestaoPatioMaisAvancadoPorCargaAgrupada;
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioPorCargaCancelada = null;

                        if (!adicionarFluxoGestaoPatioCargasOriginais)
                            fluxoGestaoPatioPorCargaCancelada = ObterFluxoGestaoPatioPorCargaCancelada(carga);

                        if (fluxoGestaoPatioPorCargaCancelada != null)
                        {
                            if (cargaJanelaCarregamento != null)
                            {
                                cargaJanelaCarregamento.ObservacaoFluxoPatio = repositorioCargaJanelaCarregamento.BuscarObservacaoFluxoPatioPorCarga(fluxoGestaoPatioPorCargaCancelada.Carga.Codigo);
                                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                            }

                            TrocarCarga(fluxoGestaoPatioPorCargaCancelada, carga, carga.Filial);
                            fluxoGestaoPatio = fluxoGestaoPatioPorCargaCancelada;
                        }
                        else
                        {
                            fluxoGestaoPatio = Adicionar(carga, carga.Filial, TipoFluxoGestaoPatio.Origem, carga.DataCarregamentoCarga, cargaJanelaCarregamento, preSetTempoEtapa: preSetTempoEtapa);
                        }

                    }

                    if (adicionarFluxoGestaoPatioCargasOriginais)
                        AdicionarPorCargasOriginais(fluxoGestaoPatio);

                    if (fluxoGestaoPatio != null)
                        fluxoGestaoPatioOrigemAdicionado = true;
                }

                if (!fluxoGestaoPatioOrigemAdicionado || configuracaoGestaoPatio.GerarFluxoDestinoAntesFinalizarOrigem)
                    AdicionarFluxoGestaoPatioDestino(carga);
            }
            else
            {
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = (Dominio.Entidades.Embarcador.PreCargas.PreCarga)cargaBase;
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio;

                if (preCarga.ProgramacaoCarga)
                    return;

                Adicionar(preCarga, preCarga.Filial, TipoFluxoGestaoPatio.Origem, preCarga.DataPrevisaoEntrega, cargaJanelaCarregamento, preSetTempoEtapa: preSetTempoEtapa);
            }
        }

        /// <summary>
        /// Atualiza a data prevista das etapas do fluxo de pátio de origem utilizando a data de início de carregamento como base
        /// </summary>
        /// <param name="cargaJanelaCarregamento"></param>
        public void AtualizarDataPrevisaoInicioEtapas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            TipoFluxoGestaoPatio tipoFluxoGestaoPatio = cargaJanelaCarregamento.Tipo == TipoCargaJanelaCarregamento.Carregamento ? TipoFluxoGestaoPatio.Origem : TipoFluxoGestaoPatio.Destino;
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = ObterFluxoGestaoPatio(cargaJanelaCarregamento.CargaBase, cargaJanelaCarregamento.CentroCarregamento.Filial, tipoFluxoGestaoPatio);

            if (fluxoGestaoPatio == null)
                return;

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            if (etapas?.Count == 0)
                return;

            DateTime? dataInicioViagem = ObterDataInicioViagem(fluxoGestaoPatio, cargaJanelaCarregamento.InicioCarregamento);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaReferencia = ObterEtapaReferenciaDefinicaoDataPrevisaoInicio(etapas);
            DateTime? dataPrevisaoInicioPorEtapa = ObterDataPrevisaoInicioPorEtapaReferenciaParaAtualizarTempoEtapas(etapaReferencia, cargaJanelaCarregamento, dataInicioViagem);

            if (!dataPrevisaoInicioPorEtapa.HasValue)
            {
                if (cargaJanelaCarregamento.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                    return;

                dataPrevisaoInicioPorEtapa = cargaJanelaCarregamento.InicioCarregamento;
            }

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasFilial = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterEtapasOrdenadas(fluxoGestaoPatio.Tipo, fluxoGestaoPatio.Filial.Codigo, fluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo ?? 0, retornarEtapasDesabilitadas: true);
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa();

            dataPrevisaoInicioPorEtapa = DefinirDataPrevisaoInicioEtapa(fluxoGestaoPatio, etapaReferencia.EtapaFluxoGestaoPatio, dataPrevisaoInicioPorEtapa.Value, cargaJanelaCarregamento?.TerminoCarregamento, dataInicioViagem, preSetTempoEtapa: preSetTempoEtapa);

            DefinirDataPrevisaoInicioEtapasAnteriores(fluxoGestaoPatio, etapas, etapasFilial, etapaReferencia, dataPrevisaoInicioPorEtapa.Value, cargaJanelaCarregamento?.TerminoCarregamento, dataInicioViagem, preSetTempoEtapa);
            DefinirDataPrevisaoInicioEtapasPosteriores(fluxoGestaoPatio, etapas, etapasFilial, etapaReferencia, dataPrevisaoInicioPorEtapa.Value, cargaJanelaCarregamento?.TerminoCarregamento, dataInicioViagem, preSetTempoEtapa);

            new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork).Atualizar(fluxoGestaoPatio);
        }

        /// <summary>
        /// Atualiza a data prevista das etapas do fluxo de pátio de origem utilizando a data de carregamento como base
        /// </summary>
        /// <param name="carga"></param>
        /// <param name="dataCarregamento"></param>
        public void AtualizarDataPrevisaoInicioEtapas(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataCarregamento)
        {
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = ObterFluxoGestaoPatio(carga);

            if (fluxoGestaoPatio == null)
                return;

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            if (etapas?.Count == 0)
                return;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaReferencia = ObterEtapaReferenciaDefinicaoDataPrevisaoInicio(etapas);
            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasFilial = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterEtapasOrdenadas(fluxoGestaoPatio.Tipo, fluxoGestaoPatio.Filial.Codigo, fluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo ?? 0, retornarEtapasDesabilitadas: true);
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa();

            dataCarregamento = DefinirDataPrevisaoInicioEtapa(fluxoGestaoPatio, etapaReferencia.EtapaFluxoGestaoPatio, dataCarregamento, dataFimCarregamento: null, dataInicioViagem: null, preSetTempoEtapa);

            DefinirDataPrevisaoInicioEtapasAnteriores(fluxoGestaoPatio, etapas, etapasFilial, etapaReferencia, dataCarregamento, dataFimCarregamento: null, dataInicioViagem: null, preSetTempoEtapa);
            DefinirDataPrevisaoInicioEtapasPosteriores(fluxoGestaoPatio, etapas, etapasFilial, etapaReferencia, dataCarregamento, dataFimCarregamento: null, dataInicioViagem: null, preSetTempoEtapa);

            new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork).Atualizar(fluxoGestaoPatio);
        }

        /// <summary>
        /// Atualiza a data prevista das etapas dos fluxos de pátio de destino utilizando a data de prevista de entrega como base
        /// </summary>
        /// <param name="cargaEntregas"></param>
        public void AtualizarDataPrevisaoInicioEtapas(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.GerarFluxoPatioDestino)
                return;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEntregas.FirstOrDefault()?.Carga;

            if (carga == null)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatio = repositorioFluxoGestaoPatio.BuscarFluxosDestinoPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio in fluxosGestaoPatio)
            {
                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

                if (etapas?.Count == 0)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = (from o in cargaEntregas where !o.Coleta && o.Cliente != null && o.Cliente.CPF_CNPJ_SemFormato == fluxoGestaoPatio.Filial.CNPJ orderby o.Ordem select o).FirstOrDefault();
                DateTime? dataPrevisaoInicio = cargaEntrega?.DataPrevista;

                if (!dataPrevisaoInicio.HasValue)
                    continue;

                FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasFilial = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterEtapasOrdenadas(fluxoGestaoPatio.Tipo, fluxoGestaoPatio.Filial.Codigo, fluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo ?? 0, retornarEtapasDesabilitadas: true);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa();
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaReferencia = etapas.FirstOrDefault();

                dataPrevisaoInicio = DefinirDataPrevisaoInicioEtapa(fluxoGestaoPatio, etapaReferencia.EtapaFluxoGestaoPatio, dataPrevisaoInicio.Value, dataFimCarregamento: null, dataInicioViagem: null, preSetTempoEtapa: preSetTempoEtapa);

                DefinirDataPrevisaoInicioEtapasAnteriores(fluxoGestaoPatio, etapas, etapasFilial, etapaReferencia, dataPrevisaoInicio.Value, dataFimCarregamento: null, dataInicioViagem: null, preSetTempoEtapa: preSetTempoEtapa);
                DefinirDataPrevisaoInicioEtapasPosteriores(fluxoGestaoPatio, etapas, etapasFilial, etapaReferencia, dataPrevisaoInicio.Value, dataFimCarregamento: null, dataInicioViagem: null, preSetTempoEtapa: preSetTempoEtapa);

                repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
            }
        }

        /// <summary>
        /// Atualiza a data prevista já informada das etapas quando possuir um tempo no pré set
        /// </summary>
        /// <param name="fluxoGestaoPatio"></param>
        /// <param name="preSetTempoEtapa"></param>
        public void AtualizarDataPrevistaEtapas(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa in etapas)
            {
                FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapa.EtapaFluxoGestaoPatio, _unitOfWork, _auditado, _cliente);

                fluxoGestaoPatioEtapa?.AtualizarDataPrevista(fluxoGestaoPatio, preSetTempoEtapa);
            }

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
        }

        public void AtualizarEquipamento(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            if (!(cargaBase.Filial?.InformarEquipamentoFluxoPatio ?? false))
                return;

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio = ObterFluxoGestaoPatio(cargaBase);

            if (fluxoPatio == null)
                return;

            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork);

            fluxoPatio.Equipamento = repositorioFilaCarregamentoVeiculo.BuscarEquipamentoVinculado(cargaBase.Codigo, cargaBase.IsCarga());

            fluxoPatio.Equipamento ??= ObterEquipamentoVinculado(cargaBase);

            new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork).Atualizar(fluxoPatio);
        }

        public void Auditar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, string descricao)
        {
            if ((fluxoGestaoPatio != null) && (_auditado != null))
                Auditoria.Auditoria.Auditar(_auditado, fluxoGestaoPatio, null, descricao, _unitOfWork);
        }

        public void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            if (fluxoGestaoPatio == null)
                return;

            if (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != etapaFluxo)
                return;

            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapaFluxo, _unitOfWork, _auditado, _cliente);

            fluxoGestaoPatioEtapa.Avancar(fluxoGestaoPatio);
        }

        public void AvancarEtapa(Dominio.Entidades.Usuario motorista, string qrCode)
        {
            if (motorista == null)
                throw new ServicoException("Não foi possível encontrar o registro");

            string[] qrCodePartes = qrCode.Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            if (qrCodePartes.Count() != 3)
                throw new ServicoException("QR Code informado é inválido");

            TipoQRCode? tipoQrCode = qrCodePartes[0].ToNullableEnum<TipoQRCode>();
            EtapaFluxoGestaoPatio? etapaFluxo = qrCodePartes[1].ToNullableEnum<EtapaFluxoGestaoPatio>();
            int codigoFilial = qrCodePartes[2].ToInt();

            if (!tipoQrCode.HasValue || (tipoQrCode.Value != TipoQRCode.FluxoPatio))
                throw new ServicoException("QR Code informado é inválido");

            if (!etapaFluxo.HasValue)
                throw new ServicoException("Não foi possível encontrar a etapa do fluxo de pátio");

            Logistica.Manobra servicoManobra = new Logistica.Manobra(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Manobra manobraAtual = servicoManobra.ObterManobraAtual(motorista);

            if (manobraAtual == null)
                throw new ServicoException("Manobra atual não encontrada");

            Dominio.Entidades.Veiculo veiculo = (manobraAtual.Reboques?.Count > 0) ? manobraAtual.Reboques.FirstOrDefault() : manobraAtual.Tracao;
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorEtapaVeiculoEFilial(etapaFluxo.Value, veiculo.Codigo, codigoFilial);

            if (fluxoGestaoPatio == null)
                throw new ServicoException("Não foi possível encontrar o fluxo de pátio");

            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapaFluxo.Value, _unitOfWork, _auditado, _cliente);

            fluxoGestaoPatioEtapa.Avancar(fluxoGestaoPatio);
        }

        public void Cancelar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, string motivoCancelamento)
        {
            if (fluxoGestaoPatio == null)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            if (etapas.Count > 0)
            {
                EtapaFluxoGestaoPatio etapaFluxo = etapas[fluxoGestaoPatio.EtapaAtual].EtapaFluxoGestaoPatio;
                DefinirTempoEtapa(fluxoGestaoPatio, etapaFluxo, dataFluxo: DateTime.Now);
            }

            fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Cancelado;

            repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
            Auditar(fluxoGestaoPatio, motivoCancelamento);
        }

        public void DefinirCargaPorPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = preCarga.Carga;

            if (carga == null)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCarga(preCarga.Codigo);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio in fluxosGestaoPatio)
            {
                fluxoGestaoPatio.Carga = carga;

                repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);

                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

                for (int i = 0; i < etapas.Count; i++)
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa = etapas[i];
                    bool etapaLiberada = (i == 0);

                    DefinirCargaPorPreCarga(fluxoGestaoPatio, etapa.EtapaFluxoGestaoPatio, etapaLiberada);
                }
            }
        }

        public void EnviarEmailsAlertaSla()
        {
            Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla repositorioGestaoPatioAlertaSla = new Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> configuracoesAlertasSla = repositorioGestaoPatioAlertaSla.BuscarPorConfigurados();

            if (configuracoesAlertasSla.Count == 0)
                return;

            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);
            Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(_unitOfWork);
            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);
            IEnumerable<(int CodigoFilial, List<EtapaFluxoGestaoPatio> Etapas)> configuracoesAlertasSlaAgrupadasPorFilial = configuracoesAlertasSla.GroupBy(o => o.Filial.Codigo).Select(g => ValueTuple.Create(g.Key, g.SelectMany(o => o.Etapas).ToList()));

            foreach ((int CodigoFilial, List<EtapaFluxoGestaoPatio> Etapas) configuracaoAlertasSlaAgrupadasPorFilial in configuracoesAlertasSlaAgrupadasPorFilial)
            {
                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> fluxoGestaoPatioEtapas = repositorioFluxoGestaoPatioEtapas.BuscarParaGerarAlertaSla(configuracaoAlertasSlaAgrupadasPorFilial.CodigoFilial, configuracaoAlertasSlaAgrupadasPorFilial.Etapas);

                if (fluxoGestaoPatioEtapas.Count == 0)
                    continue;

                List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> configuracoesAlertasSlaPorFilial = configuracoesAlertasSla.Where(o => o.Filial.Codigo == configuracaoAlertasSlaAgrupadasPorFilial.CodigoFilial).ToList();

                foreach (Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla gestaoPatioAlertaSla in configuracoesAlertasSlaPorFilial)
                {
                    List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> fluxoGestaoPatioEtapasPorConfiguracaoAlertaSla = fluxoGestaoPatioEtapas.Where(o =>
                        o.FluxoGestaoPatio.Filial.Codigo == gestaoPatioAlertaSla.Filial.Codigo &&
                        gestaoPatioAlertaSla.Etapas.Contains(o.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)
                    ).ToList();

                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas fluxoGestaoPatioEtapa in fluxoGestaoPatioEtapasPorConfiguracaoAlertaSla)
                    {
                        int? tempoExcedido = ObterTempoExcedidoEtapaAtual(fluxoGestaoPatioEtapa.FluxoGestaoPatio);

                        if (!tempoExcedido.HasValue)
                            continue;

                        TipoAlertaSlnEmail? tipoAlertaSla = null;

                        if (tempoExcedido > gestaoPatioAlertaSla.TempoExcedido)
                            tipoAlertaSla = TipoAlertaSlnEmail.TempoExcedido;
                        else if ((gestaoPatioAlertaSla.TempoFaltante > 0) && (tempoExcedido > -gestaoPatioAlertaSla.TempoFaltante))
                            tipoAlertaSla = TipoAlertaSlnEmail.TempoFaltante;

                        if (!tipoAlertaSla.HasValue)
                            continue;

                        if (!gestaoPatioAlertaSla.TiposAlertaEmail.Contains(tipoAlertaSla.Value))
                            continue;

                        string descricaoEtapa = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(fluxoGestaoPatioEtapa.FluxoGestaoPatio, fluxoGestaoPatioEtapa.EtapaFluxoGestaoPatio)?.Descricao ?? string.Empty;
                        string assunto = $"{tipoAlertaSla.Value.ObterDescricao()} - {fluxoGestaoPatioEtapa.FluxoGestaoPatio.CargaBase.DescricaoEntidade.AllFirstLetterToUpper()} {fluxoGestaoPatioEtapa.FluxoGestaoPatio.CargaBase.Numero}";
                        StringBuilder mensagem = new StringBuilder();

                        mensagem.AppendLine("Olá,");
                        mensagem.Append($"O fluxo de pátio de {fluxoGestaoPatioEtapa.FluxoGestaoPatio.Tipo.ObterDescricao().ToLower()} da filial {fluxoGestaoPatioEtapa.FluxoGestaoPatio.Filial?.Descricao} ({fluxoGestaoPatioEtapa.FluxoGestaoPatio.Filial?.CNPJ_Formatado}) ");

                        if ((fluxoGestaoPatioEtapa.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem) && fluxoGestaoPatioEtapa.FluxoGestaoPatio.CargaBase.IsCarga())
                        {
                            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisDestino = null;

                            if (fluxoGestaoPatioEtapa.FluxoGestaoPatio.Carga.CargaAgrupamento != null)
                                filiaisDestino = repositorioCargaPedido.BuscarFiliaisPorDestinatariosDaCargaOrigem(fluxoGestaoPatioEtapa.FluxoGestaoPatio.Carga.Codigo);
                            else
                                filiaisDestino = repositorioCargaPedido.BuscarFiliaisPorDestinatariosDaCarga(fluxoGestaoPatioEtapa.FluxoGestaoPatio.Carga.Codigo);

                            if (filiaisDestino.Count == 1)
                                mensagem.AppendLine($"com destino {filiaisDestino.FirstOrDefault().Descricao} ({filiaisDestino.FirstOrDefault().CNPJ_Formatado}) exige atenção.");
                        }
                        else
                            mensagem.AppendLine(" exige atenção.");

                        mensagem.AppendLine();

                        if (tipoAlertaSla.Value == TipoAlertaSlnEmail.TempoExcedido)
                        {
                            mensagem.Append($"A etapa {descricaoEtapa} excedeu {tempoExcedido.Value} minutos do limite.");
                            fluxoGestaoPatioEtapa.DataAlertaTempoExcedido = DateTime.Now;
                        }
                        else if ((tipoAlertaSla.Value == TipoAlertaSlnEmail.TempoFaltante) && !fluxoGestaoPatioEtapa.DataAlertaTempoFaltante.HasValue)
                        {
                            mensagem.Append($"Restam {tempoExcedido.Value * -1} minutos para a data prevista da etapa {descricaoEtapa}.");
                            fluxoGestaoPatioEtapa.DataAlertaTempoFaltante = DateTime.Now;
                        }
                        else
                            continue;

                        servicoEmail.EnviarEmail(null, null, null, null, null, null, assunto, mensagem.ToString(), null, null, null, false, "", 587, _unitOfWork, 0, true, gestaoPatioAlertaSla.Emails.Split(';').ToList());

                        if (gestaoPatioAlertaSla.AlertarTransportadorPorEmail)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                            {
                                AssuntoEmail = assunto,
                                Empresa = fluxoGestaoPatioEtapa.FluxoGestaoPatio.CargaBase.Empresa,
                                Mensagem = mensagem.ToString(),
                                NotificarSomenteEmailPrincipal = true
                            };

                            servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
                        }

                        repositorioFluxoGestaoPatioEtapas.Atualizar(fluxoGestaoPatioEtapa);
                    }
                }
            }
        }

        public void LiberarProximaEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            LiberarProximaEtapa(fluxoGestaoPatio, etapaFluxo, dataFluxo: DateTime.Now);
        }

        public void LiberarProximaEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo, DateTime dataFluxo)
        {
            if (fluxoGestaoPatio == null)
                return;

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            if (!etapas.Any(obj => obj.EtapaFluxoGestaoPatio == etapaFluxo))
                return;

            AnteciparEtapa(fluxoGestaoPatio, etapas, etapaFluxo);

            if (fluxoGestaoPatio.EtapaAtual == 0)
                ValidarPermissaoIniciar(fluxoGestaoPatio);

            ValidarPermissaoLiberarProximaEtapa(fluxoGestaoPatio);

            EtapaFluxoGestaoPatio etapaPreSequencia = etapas[fluxoGestaoPatio.EtapaAtual].EtapaFluxoGestaoPatio;
            bool avancarParaProximaEtapa = etapas[fluxoGestaoPatio.EtapaAtual].EtapaFluxoGestaoPatio == etapaFluxo;
            int proximaEtapa = fluxoGestaoPatio.EtapaAtual + 1;

            if (avancarParaProximaEtapa)
            {
                if (DefinirTempoEtapa(fluxoGestaoPatio, etapaFluxo, dataFluxo))
                    ExecutarAcoesPorEtapaFinalizada(fluxoGestaoPatio, etapaFluxo, dataFluxo);
            }
            else
            {
                proximaEtapa = DefinirTempoEtapasAnteriores(fluxoGestaoPatio, etapas, etapaFluxo, dataFluxo);
                avancarParaProximaEtapa = proximaEtapa > fluxoGestaoPatio.EtapaAtual;
            }

            if (proximaEtapa > etapas.Count)
                return;

            if (etapas.LastOrDefault().EtapaFluxoGestaoPatio == etapaFluxo)
            {
                fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Aprovado;
                fluxoGestaoPatio.DataFinalizacaoFluxo = DateTime.Now;
                fluxoGestaoPatio.EtapaAtual = etapas.Count() - 1;
                fluxoGestaoPatio.VeiculoAtivo = false;
                fluxoGestaoPatio.PendenteIntegracao = true;

                ExecutarAcoesPorFluxoPatioFinalizado(fluxoGestaoPatio);
            }
            else if (avancarParaProximaEtapa)
            {
                fluxoGestaoPatio.EtapaAtual = proximaEtapa;
                fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual = etapas[fluxoGestaoPatio.EtapaAtual].EtapaFluxoGestaoPatio;
                fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Aguardando;

                EtapaInicioViagemAvancada(fluxoGestaoPatio, etapaFluxo);
                LiberarEtapaPosteriorEtapaPosicao(fluxoGestaoPatio, etapas);
                ReprogramaTempoProximasEtapas(fluxoGestaoPatio, etapaPreSequencia, etapas);
                LiberarProximaEtapa(fluxoGestaoPatio, etapas[fluxoGestaoPatio.EtapaAtual]);
                NotificarAppMotoristasMudancaFluxo(fluxoGestaoPatio);
            }

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);

            AuditarAvancarEtapa(fluxoGestaoPatio, etapaFluxo);
            LiberarProximaEtapaAutomaticamente(fluxoGestaoPatio);
            NotificarSignalR(fluxoGestaoPatio);
        }

        public void LiberarProximaEtapaPorCargaAgrupada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo, DateTime dataFluxo)
        {
            if (fluxoGestaoPatio.Carga?.CargaAgrupamento == null)
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (!(configuracaoGestaoPatio?.AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas ?? false))
                return;

            Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupamento = fluxoGestaoPatio.Carga.CargaAgrupamento;
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioCargaAgrupada = ObterFluxoGestaoPatio(cargaAgrupamento);

            if (fluxoGestaoPatioCargaAgrupada == null)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosPatio = repositorioFluxoGestaoPatio.BuscarPorCargaAgrupadaETipo(cargaAgrupamento.Codigo, TipoFluxoGestaoPatio.Origem);

            if (!FluxosGestaoPatioCompativeisPorCargaAgrupada(fluxosPatio, fluxoGestaoPatioCargaAgrupada))
                throw new ServicoException("É necessário que todos os fluxos sejam compatíveis.");

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoMenosAvancado = fluxosPatio.OrderBy(obj => obj.EtapaAtual).FirstOrDefault();

            int menorOrdem = fluxoMenosAvancado?.EtapaAtual ?? 0;

            if (fluxoGestaoPatioCargaAgrupada.EtapaAtual < menorOrdem || fluxosPatio.All(f => f.DataFinalizacaoFluxo.HasValue))
                LiberarProximaEtapa(fluxoGestaoPatioCargaAgrupada, fluxoGestaoPatioCargaAgrupada.GetEtapas()[fluxoGestaoPatioCargaAgrupada.EtapaAtual].EtapaFluxoGestaoPatio, dataFluxo);
        }

        public void ProcessarMacroRecebida(Dominio.Entidades.Embarcador.Veiculos.MacroVeiculo macroVeiculo, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();
            bool macroInicioViagem = (configuracaoGestaoPatio.MacroInicioViagem != null) && (configuracaoGestaoPatio.MacroInicioViagem.Codigo == macroVeiculo.Macro.Codigo);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = null;

            if (macroVeiculo.Carga != null)
                fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCargaETipo(macroVeiculo.Carga.Codigo, TipoFluxoGestaoPatio.Origem);
            else if (macroVeiculo.Veiculo != null)
                fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorVeiculoECarga(macroVeiculo.Veiculo.Codigo, !macroInicioViagem);

            if (fluxoGestaoPatio == null)
                return;

            if ((configuracaoGestaoPatio.MacroChegadaDestinatario != null) && (configuracaoGestaoPatio.MacroChegadaDestinatario.Codigo == macroVeiculo.Macro.Codigo))
                LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.ChegadaLoja, macroVeiculo.DataMacro ?? DateTime.Now);

            if ((configuracaoGestaoPatio.MacroSaidaDestinatario != null) && (configuracaoGestaoPatio.MacroSaidaDestinatario.Codigo == macroVeiculo.Macro.Codigo))
                LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.SaidaLoja, macroVeiculo.DataMacro ?? DateTime.Now);

            if ((configuracaoGestaoPatio.MacroFimViagem != null) && (configuracaoGestaoPatio.MacroFimViagem.Codigo == macroVeiculo.Macro.Codigo))
            {
                LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.FimViagem, macroVeiculo.DataMacro ?? DateTime.Now);

                string descricaoAuditoria = "Fluxo Finalizado";
                Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                Monitoramento.Monitoramento.FinalizarMonitoramento(fluxoGestaoPatio.Carga, DateTime.Now, configuracaoEmbarcador, null, descricaoAuditoria, _unitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoNoFluxoPatio);
                string retorno = servicoCarga.SolicitarEncerramentoCarga(fluxoGestaoPatio.Carga.Codigo, descricaoAuditoria, "", tipoServicoMultisoftware, _unitOfWork, _auditado);

                if (!string.IsNullOrEmpty(retorno))
                    Log.TratarErro(retorno);
            }
        }

        public void ReabrirFluxo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (fluxoGestaoPatio == null)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaAtual = etapas[fluxoGestaoPatio.EtapaAtual];

            fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Aguardando;

            RemoverTempoEtapa(fluxoGestaoPatio, etapaAtual.EtapaFluxoGestaoPatio);
            EtapaRetornarda(fluxoGestaoPatio, etapaAtual.EtapaFluxoGestaoPatio);

            repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
            repositorioFluxoGestaoPatioEtapas.Atualizar(etapaAtual);

            Auditar(fluxoGestaoPatio, "Reabriu o Fluxo");
        }

        public void Reiniciar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioDadosReiniciar fluxoGestaoPatioDadosReiniciar, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (fluxoGestaoPatioDadosReiniciar?.FluxoGestaoPatio == null)
                throw new ServicoException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

            if (fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando)
                throw new ServicoException(Localization.Resources.GestaoPatio.FluxoPatio.NaoPossivelCancelarFluxoNaSituacaoAtual);

            Repositorio.Embarcador.GestaoPatio.FluxoPatioCancelamento repositorioFluxoPatioCancelamento = new Repositorio.Embarcador.GestaoPatio.FluxoPatioCancelamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioCancelamento fluxoPatioCancelamento = new Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioCancelamento()
            {
                FluxoGestaoPatio = fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio,
                Motivo = fluxoGestaoPatioDadosReiniciar.Motivo,
                RemoverVeiculoFilaCarregamento = fluxoGestaoPatioDadosReiniciar.RemoverVeiculoFilaCarregamento,
                VeiculoBloqueado = fluxoGestaoPatioDadosReiniciar.RemoverVeiculoFilaCarregamento ? fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga?.Veiculo : null,
                MotivoRetiradaFilaCarregamento = (fluxoGestaoPatioDadosReiniciar.CodigoMotivoRetiradaFilaCarregamento > 0) ? new Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento() { Codigo = fluxoGestaoPatioDadosReiniciar.CodigoMotivoRetiradaFilaCarregamento } : null
            };

            repositorioFluxoPatioCancelamento.Inserir(fluxoPatioCancelamento);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento;

            if (fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga != null)
            {
                Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga, tipoServicoMultisoftware))
                    throw new ServicoException(Localization.Resources.GestaoPatio.FluxoPatio.NaoPossivelCancelarFluxoNaSituacaoAtualDaCarga);

                if (fluxoGestaoPatioDadosReiniciar.RemoverDadosTransporte || fluxoGestaoPatioDadosReiniciar.RemoverVeiculoFilaCarregamento)
                {
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                    Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWork, Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(tipoServicoMultisoftware));

                    fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga.Initialize();
                    fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga.Motoristas.Clear();
                    fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga.Veiculo = null;
                    fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga.VeiculosVinculados?.Clear();

                    if (fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga.ExigeNotaFiscalParaCalcularFrete)
                        fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga.SituacaoCarga = SituacaoCarga.Nova;

                    Auditoria.Auditoria.AuditarComAlteracoesRealizadas(_auditado, fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga, fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga.GetChanges(), Localization.Resources.GestaoPatio.FluxoPatio.AlteradoPorCancelamentoDePatio, _unitOfWork);
                    repositorioCarga.Atualizar(fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga);

                    if (fluxoGestaoPatioDadosReiniciar.RemoverVeiculoFilaCarregamento)
                        servicoFilaCarregamentoVeiculo.RemoverPorCarga(fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga, fluxoGestaoPatioDadosReiniciar.CodigoMotivoRetiradaFilaCarregamento, tipoServicoMultisoftware);
                    else
                        servicoFilaCarregamentoVeiculo.AtualizarPorCarga(fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga, tipoServicoMultisoftware);
                }

                cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.Carga.Codigo);
            }
            else
                cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.PreCarga.Codigo);

            Cancelar(fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio, fluxoGestaoPatioDadosReiniciar.Motivo);
            Adicionar(fluxoGestaoPatioDadosReiniciar.FluxoGestaoPatio.CargaBase, tipoServicoMultisoftware, cargaJanelaCarregamento);
        }

        public void RejeitarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            if (fluxoGestaoPatio == null)
                return;

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            if (!etapas.Any(obj => obj.EtapaFluxoGestaoPatio == etapaFluxo))
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            DateTime dataFluxo = DateTime.Now;

            if (etapas[fluxoGestaoPatio.EtapaAtual].EtapaFluxoGestaoPatio == etapaFluxo)
            {
                if (DefinirTempoEtapa(fluxoGestaoPatio, etapaFluxo, dataFluxo))
                    ExecutarAcoesPorEtapaFinalizada(fluxoGestaoPatio, etapaFluxo, dataFluxo);
            }
            else
                DefinirTempoEtapasAnteriores(fluxoGestaoPatio, etapas, etapaFluxo, dataFluxo);

            fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Rejeitado;

            repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
            NotificarSignalR(fluxoGestaoPatio);
        }

        public void SalvarObservacaoPorEtapa(int codigoFluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo, string observacao)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa = repositorioFluxoGestaoPatioEtapas.BuscarPorGestaoEEtapa(codigoFluxoGestaoPatio, etapaFluxo);

            if (etapa == null)
                throw new ServicoException("Não foi possível encontrar a etapa para atualizar a observação");

            etapa.Initialize();
            etapa.Observacao = observacao;

            repositorioFluxoGestaoPatioEtapas.Atualizar(etapa, _auditado);
        }

        public void TrocarFilial(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Filiais.Filial filialAntiga, Dominio.Entidades.Embarcador.Filiais.Filial filialNova)
        {
            if ((filialAntiga == null) || (filialAntiga.Codigo == (filialNova?.Codigo ?? 0)))
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCargaFilialETipo(carga.Codigo, filialAntiga.Codigo, TipoFluxoGestaoPatio.Origem);

            if (fluxoGestaoPatio == null)
                return;

            Cancelar(fluxoGestaoPatio, motivoCancelamento: "Fluxo cancelado automaticamente ao trocar a filial da carga");

            if (filialNova != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = ObterCargaJanelaCarregamento(carga);

                Adicionar(carga, filialNova, TipoFluxoGestaoPatio.Origem, carga.DataCarregamentoCarga, cargaJanelaCarregamento, preSetTempoEtapa: null);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCarga(cargaAtual.Codigo);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio in fluxosGestaoPatio)
                TrocarCarga(fluxoGestaoPatio, cargaNova, fluxoGestaoPatio.Filial);
        }

        public void VoltarAteEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.EtapaAtual == 0))
                return;

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaRetornar = etapas.Where(o => o.EtapaFluxoGestaoPatio == etapaFluxo).FirstOrDefault();

            if (etapaRetornar == null)
                return;

            if (etapaRetornar.Ordem >= fluxoGestaoPatio.EtapaAtual)
                return;

            for (int indiceEtapa = fluxoGestaoPatio.EtapaAtual; indiceEtapa > etapaRetornar.Ordem; indiceEtapa--)
                VoltarEtapaAnterior(fluxoGestaoPatio, etapas);
        }

        public void VoltarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo, Dominio.Entidades.Usuario usuario)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.EtapaAtual == 0) || (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != etapaFluxo))
                return;

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            if (!etapas.Any(o => o.EtapaFluxoGestaoPatio == etapaFluxo))
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = ObterConfiguracaoGestaoPatio();

            if (!configuracao.IsPermiteVoltar(etapaFluxo))
                return;

            VoltarEtapaAnterior(fluxoGestaoPatio, etapas);
            NotificarSignalR(fluxoGestaoPatio);
        }

        public void VoltarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo, Dominio.Entidades.Usuario usuario, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio)
        {
            bool? permissaoVoltarEtapa = null;

            if (permissoesPersonalizadasFluxoPatio.Count > 0)
                permissaoVoltarEtapa = permissoesPersonalizadasFluxoPatio.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FluxoPatio_PermiteVoltarEtapa);

            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.EtapaAtual == 0) || (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != etapaFluxo))
                return;

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = ObterEtapas(fluxoGestaoPatio);

            if (!etapas.Any(o => o.EtapaFluxoGestaoPatio == etapaFluxo))
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = ObterConfiguracaoGestaoPatio();

            if (!configuracao.IsPermiteVoltar(etapaFluxo))
                return;

            if (permissaoVoltarEtapa != null && !permissaoVoltarEtapa.Value)
                return;

            VoltarEtapaAnterior(fluxoGestaoPatio, etapas);
            NotificarSignalR(fluxoGestaoPatio);
        }

        #endregion Métodos Públicos

        #region Métodos Públicos de Consulta

        public DateTime? ObterDataEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(etapaFluxo, _unitOfWork, _auditado, _cliente);

            return fluxoGestaoPatioEtapa?.ObterData(fluxoGestaoPatio);
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio ObterFluxoGestaoPatioDestino(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            if (cargaBase.IsCarga())
                return repositorioFluxoGestaoPatio.BuscarPorCargaETipo(cargaBase.Codigo, TipoFluxoGestaoPatio.Destino);

            return repositorioFluxoGestaoPatio.BuscarPorPreCargaETipo(cargaBase.Codigo, TipoFluxoGestaoPatio.Destino);
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio ObterFluxoGestaoPatio(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            if (cargaBase.IsCarga())
                return repositorioFluxoGestaoPatio.BuscarPorCargaETipo(cargaBase.Codigo, TipoFluxoGestaoPatio.Origem);

            return repositorioFluxoGestaoPatio.BuscarPorPreCargaETipo(cargaBase.Codigo, TipoFluxoGestaoPatio.Origem);
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAvancada ObterFluxoGestaoPatioPorEtapaEPlacaMaisAntigo(EtapaFluxoGestaoPatio etapa, string placa)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new(_unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> lista = repositorioFluxoGestaoPatio.BuscarPorEtapaEPlacaVeiculo(
                etapa,
                placa,
                SituacaoEtapaFluxoGestaoPatio.Aguardando,
                default
            ).GetAwaiter().GetResult();

            return new()
            {
                Entidade = lista.OrderBy(x => x.Codigo).FirstOrDefault(),
                HouveramOutrosResultados = lista.Count > 1
            };
        }

        public string ObterObservacaoPorEtapa(int codigoFluxoGestaoPatio, EtapaFluxoGestaoPatio etapaFluxo)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa = repositorioFluxoGestaoPatioEtapas.BuscarPorGestaoEEtapa(codigoFluxoGestaoPatio, etapaFluxo);

            return etapa?.Observacao ?? string.Empty;
        }

        public void InserirEquipamentoFluxoPatio(int codigoEquipamento, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigoEquipamento);

                fluxoGestaoPatio.Initialize();

                string mensagemAuditoria = $"Adicinou o equipamento {equipamento.Descricao}";

                if (fluxoGestaoPatio.Equipamento != null)
                    mensagemAuditoria = $"Alterou o equipamento {fluxoGestaoPatio.Equipamento.Descricao} para {equipamento.Descricao}";

                fluxoGestaoPatio.Equipamento = equipamento;

                repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, fluxoGestaoPatio, fluxoGestaoPatio.GetChanges(), mensagemAuditoria, _unitOfWork);

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public decimal ObterTempoEtapaAnterior(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataAtual)
        {
            DateTime? dataEtapaAnterior = ObterDataEtapaAnterior(fluxoGestaoPatio);
            decimal tempoEmMinutos = 0;

            if (dataEtapaAnterior != null)
                tempoEmMinutos = (decimal)(dataAtual - dataEtapaAnterior.Value).TotalMinutes;

            return tempoEmMinutos;
        }

        public int? ObterTempoExcedidoEtapaAtual(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            FluxoGestaoPatioEtapa fluxoGestaoPatioEtapa = FluxoGestaoPatioEtapaFactory.CriarEtapa(fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual, _unitOfWork, _auditado, _cliente);
            DateTime? dataEtapaAnterior = ObterDataEtapaAnterior(fluxoGestaoPatio);

            return fluxoGestaoPatioEtapa?.ObterTempoExcedido(fluxoGestaoPatio, dataEtapaAnterior);
        }

        #endregion Métodos Públicos de Consulta
    }
}
