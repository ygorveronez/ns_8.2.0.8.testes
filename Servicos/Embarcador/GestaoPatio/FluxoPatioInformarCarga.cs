using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class FluxoPatioInformarCarga
    {
        #region Atributos Privados Somente Leitura

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Privados Somente Leitura

        #region Construtores

        public FluxoPatioInformarCarga(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public FluxoPatioInformarCarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AdicionarFluxoGestaoPatio(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico historicoCargaAlocada, Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivoSelecaoMotoristaForaOrdem, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            FluxoGestaoPatio servicoFluxoGestaoPatio = new FluxoGestaoPatio(_unitOfWork, cliente);

            servicoFluxoGestaoPatio.Adicionar(carga, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

            if (fluxoGestaoPatio == null)
                return;

            if (fluxoGestaoPatio.Equipamento == null)
                fluxoGestaoPatio.Equipamento = historicoCargaAlocada.FilaCarregamentoVeiculo.Equipamento;

            string descricaoAuditoria = string.Empty;
            string descricaoEquipamento = string.Empty;

            if (carga.Filial.InformarEquipamentoFluxoPatio)
                descricaoEquipamento = $"| Equipamento: {fluxoGestaoPatio.Equipamento?.Descricao ?? "(Sem Equipamento)"}";

            if (motivoSelecaoMotoristaForaOrdem == null)
                descricaoAuditoria = $"Vinculada fila de carregamento - Posição: {historicoCargaAlocada.Posicao} | Motorista: {historicoCargaAlocada.FilaCarregamentoVeiculo.ConjuntoMotorista.Motorista?.Nome ?? "(Sem Motorista)"} {descricaoEquipamento}";
            else
                descricaoAuditoria = $"Vinculada fila de carregamento fora da ordem - Posição: {historicoCargaAlocada.Posicao} | Motorista: {historicoCargaAlocada.FilaCarregamentoVeiculo.ConjuntoMotorista.Motorista?.Nome ?? "(Sem Motorista)"} | Motivo: {motivoSelecaoMotoristaForaOrdem.Descricao} {descricaoEquipamento}";

            Auditoria.Auditoria.Auditar(_auditado, fluxoGestaoPatio, descricaoAuditoria.Substring(0, Math.Min(descricaoAuditoria.Length, 200)), _unitOfWork);

            if (fluxoGestaoPatio.Equipamento != null && fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != EtapaFluxoGestaoPatio.ChegadaVeiculo)
            {
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

                repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);
            }
            else if (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.ChegadaVeiculo)
            {
                InformarChegadaVeiculo(fluxoGestaoPatio);
                servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.ChegadaVeiculo);
            }
        }

        private void AtualizarCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            new Logistica.CargaJanelaCarregamento(_unitOfWork).AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        private void AtualizarDadosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoCaixaInformarCarga fluxoCaixaInformarCarga)
        {
            carga.DataCarregamentoCarga = fluxoCaixaInformarCarga.DataCarregamento;
            carga.ModeloVeicularCarga = ObterModeloVeicularCarga(fluxoCaixaInformarCarga.CodigoModeloVeicular);
            carga.Empresa = ObterTransportador(fluxoCaixaInformarCarga.CodigoFilaCarregamento);

            AtualizarDoca(carga, fluxoCaixaInformarCarga);
            AtualizarLacre(carga, fluxoCaixaInformarCarga);
        }

        private void AtualizarDoca(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoCaixaInformarCarga fluxoCaixaInformarCarga)
        {
            if (string.IsNullOrWhiteSpace(fluxoCaixaInformarCarga.Doca))
                return;

            carga.NumeroDoca = fluxoCaixaInformarCarga.Doca;

            Servicos.Embarcador.Integracao.Eship.IntegracaoEship serEShip = new Servicos.Embarcador.Integracao.Eship.IntegracaoEship(_unitOfWork);
            serEShip.VerificarIntegracaoEShip(carga);
        }

        private void AtualizarLacre(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoCaixaInformarCarga fluxoCaixaInformarCarga)
        {
            if (string.IsNullOrWhiteSpace(fluxoCaixaInformarCarga.Lacre))
                return;

            string[] lacres = (from o in fluxoCaixaInformarCarga.Lacre.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) select o.Trim()).ToArray();
            Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> lacresAdicionados = repositorioCargaLacre.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> lacresRemover = (from o in lacresAdicionados where !lacres.Contains(o.Numero) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLacre lacreRemover in lacresRemover)
                repositorioCargaLacre.Deletar(lacreRemover);

            foreach (string lacre in lacres)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre = (from o in lacresAdicionados where o.Numero == lacre select o).FirstOrDefault();

                if (cargaLacre != null)
                    continue;

                cargaLacre = new Dominio.Entidades.Embarcador.Cargas.CargaLacre()
                {
                    Carga = carga,
                    Numero = lacre
                };

                repositorioCargaLacre.Inserir(cargaLacre);
            }
        }

        private void InformarChegadaVeiculo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (cargaGuarita == null)
                return;

            cargaGuarita.Situacao = SituacaoCargaGuarita.AguardandoLiberacao;
            cargaGuarita.DataChegadaVeiculo = DateTime.Now;

            repositorioCargaGuarita.Atualizar(cargaGuarita);
            Auditoria.Auditoria.Auditar(_auditado, cargaGuarita, null, "Informou a chegada do Veículo pela adição de carga ao fluxo de pátio.", _unitOfWork);
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento ObterCargaJanelaCarregamento(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga) ?? throw new ServicoException("Janela de carregamento não encontrada");

            if ((tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) && (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador))
                throw new ServicoException("A situação da janela de carregamento não permite adicionar a carga no fluxo de pátio");

            return cargaJanelaCarregamento;
        }

        private Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo ObterFilaCarregamentoVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork);

            return repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada");
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularCarga(int codigoModeloVeicular)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);

            return repositorioModeloVeicular.BuscarPorCodigo(codigoModeloVeicular) ?? throw new ServicoException("Modelo veicular não encontrado");
        }

        private Dominio.Entidades.Empresa ObterTransportador(int codigoFilaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo);

            return filaCarregamentoVeiculo?.ConjuntoVeiculo.ObterEmpresa() ?? throw new ServicoException("Transportador não encontrado");
        }

        private Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem ObterMotivoSelecaoMotoristaForaOrdem(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoCaixaInformarCarga fluxoCaixaInformarCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((fluxoCaixaInformarCarga.PrimeiroNaFila) || (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe))
                return null;

            Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem repositorioMotivoSelecaoMotoristaForaOrdem = new Repositorio.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem(_unitOfWork);

            return repositorioMotivoSelecaoMotoristaForaOrdem.BuscarPorCodigo(fluxoCaixaInformarCarga.CodigoMotivoSelecaoMotoristaForaOrdem) ?? throw new ServicoException("Motivo da seleção do motorista fora da ordem não encontrado");
        }

        private Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico VincularFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivoSelecaoMotoristaForaOrdem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWork, _auditado.Usuario, Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(tipoServicoMultisoftware));

            return servicoFilaCarregamentoVeiculo.AlocarCargaManualmente(filaCarregamentoVeiculo.Codigo, carga, motivoSelecaoMotoristaForaOrdem, tipoServicoMultisoftware);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void AdicionarCarga(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoCaixaInformarCarga fluxoCaixaInformarCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(fluxoCaixaInformarCarga.CodigoCarga) ?? throw new ServicoException("Não foi possível encontrar a carga");
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = ObterFilaCarregamentoVeiculo(fluxoCaixaInformarCarga.CodigoFilaCarregamento);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = ObterCargaJanelaCarregamento(fluxoCaixaInformarCarga.CodigoCarga, tipoServicoMultisoftware);
            Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivoSelecaoMotoristaForaOrdem = ObterMotivoSelecaoMotoristaForaOrdem(fluxoCaixaInformarCarga, tipoServicoMultisoftware);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico historicoCargaAlocada = VincularFilaCarregamento(filaCarregamentoVeiculo, carga, motivoSelecaoMotoristaForaOrdem, tipoServicoMultisoftware);

            AtualizarDadosCarga(carga, fluxoCaixaInformarCarga);
            AtualizarCargaJanelaCarregamento(cargaJanelaCarregamento, tipoServicoMultisoftware);
            AdicionarFluxoGestaoPatio(carga, tipoServicoMultisoftware, historicoCargaAlocada, motivoSelecaoMotoristaForaOrdem, cliente);

            repositorioCarga.Atualizar(carga);
        }

        #endregion Métodos Públicos
    }
}
