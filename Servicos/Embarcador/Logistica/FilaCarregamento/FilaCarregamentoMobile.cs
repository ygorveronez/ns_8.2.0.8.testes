using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class FilaCarregamentoMobile : FilaCarregamentoBase
    {
        #region Atributos Privados Somente Leitura

        private readonly string _urlBaseOrigemRequisicao;

        #endregion

        #region Construtores

        public FilaCarregamentoMobile(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, OrigemAlteracaoFilaCarregamento.Motorista) { }

        public FilaCarregamentoMobile(Repositorio.UnitOfWorkContainer unitOfWorkContainer) : base(unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista) { }

        public FilaCarregamentoMobile(Repositorio.UnitOfWork unitOfWork, string urlBaseOrigemRequisicao) : base(unitOfWork, OrigemAlteracaoFilaCarregamento.Motorista)
        {
            _urlBaseOrigemRequisicao = urlBaseOrigemRequisicao;
        }

        public FilaCarregamentoMobile(Repositorio.UnitOfWorkContainer unitOfWorkContainer, string urlBaseOrigemRequisicao) : base(unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista)
        {
            _urlBaseOrigemRequisicao = urlBaseOrigemRequisicao;
        }

        #endregion

        #region Métodos Privados

        private Tuple<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista, Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> Adicionar(int codigoMotorista, string latitude, string longitude, bool lojaProximidade, Func<FilaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> ObterTipoRetornoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            FilaCarregamentoMotorista servicoFilaCarregamentoMotorista = new FilaCarregamentoMotorista(_unitOfWorkContainer, _origemAlteracao);
            FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga = ObterTipoRetornoCarga(servicoFilaCarregamentoVeiculo);

            try
            {
                _unitOfWorkContainer.Start();

                if (configuracaoEmbarcador?.FinalizarViagemAnteriorAoEntrarFilaCarregamento ?? false)
                    servicoFilaCarregamentoVeiculo.FinalizarPorMotorista(codigoMotorista);

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = servicoFilaCarregamentoMotorista.Adicionar(codigoMotorista, latitude, longitude, lojaProximidade);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = null;

                if (tipoRetornoCarga != null)
                    filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.VincularFilaCarregamentoMotorista(filaCarregamentoMotorista, tipoRetornoCarga, (configuracaoEmbarcador?.ValidarConjuntoVeiculoPermiteEntrarFilaCarregamentoMobile ?? true), tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();

                if (filaCarregamentoVeiculo != null)
                    CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo);

                return new Tuple<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista, Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>(filaCarregamentoMotorista, filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        private void CriarNotificacaoJanelaCarregamentoAtualizadaOutroAmbiente(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (_unitOfWorkContainer.TransacaoPorContainerAtiva)
                return;

            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoFilaCarregamentoVeiculo.ObterCargaJanelaCarregamento(filaCarregamentoVeiculo);

            if (cargaJanelaCarregamento != null)
                new Hubs.JanelaCarregamento().CriarNotificaoJanelaCarregamentoAtualizadaOutroAmbiente(cargaJanelaCarregamento.Codigo, _urlBaseOrigemRequisicao);
        }

        private void CriarNotificacaoFilaAlteradaOutroAmbiente(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (_unitOfWorkContainer.TransacaoPorContainerAtiva)
                return;

            var hubFilaCarregamento = new Hubs.FilaCarregamento();

            hubFilaCarregamento.CriarNotificaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo.Codigo, _urlBaseOrigemRequisicao);
        }

        private Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo ObterFilaCarregamentoVeiculoRetornar(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo()
            {
                Codigo = filaCarregamentoVeiculo.Codigo,
                ModeloVeicular = filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga?.Descricao ?? "",
                Motorista = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista?.Descricao ?? "",
                Posicao = filaCarregamentoVeiculo.ObterPosicao(),
                Reboques = filaCarregamentoVeiculo.ConjuntoVeiculo.ObterPlacasReboques(),
                Situacao = filaCarregamentoVeiculo.Situacao,
                Tipo = filaCarregamentoVeiculo.Tipo,
                Tracao = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Placa_Formatada ?? ""
            };
        }

        private List<int> ObterListaCodigoCentroCarregamentoPorOperadorLogistica(int codigoUsuario)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repositorioCentroCarregamento.BuscarPorOperadorLogistica(codigoUsuario);

            List<int> listaCodigoCentroCarregamentoPorOperadorLogistica = (
                from centroCarregamento in centrosCarregamento
                select centroCarregamento.Codigo
            ).ToList();

            return listaCodigoCentroCarregamentoPorOperadorLogistica;
        }

        private Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento ObterRetorno(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo == null)
                return null;

            SituacaoFilaCarregamento situacao = SituacaoFilaCarregamento.Disponivel;

            if (SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila().Contains(filaCarregamentoVeiculo.Situacao))
                situacao = SituacaoFilaCarregamento.NaFila;
            else if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmRemocao)
                situacao = SituacaoFilaCarregamento.EmRemocao;
            else if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmTransicao)
                situacao = SituacaoFilaCarregamento.EmTransicao;
            else if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem)
                situacao = SituacaoFilaCarregamento.EmViagem;
            else if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Removida)
                situacao = SituacaoFilaCarregamento.Removido;

            SituacaoMotoristaFilaCarregamento situacaoMotorista = SituacaoMotoristaFilaCarregamento.AguardandoCarga;

            if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
            {
                if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaAlocada)
                    situacaoMotorista = SituacaoMotoristaFilaCarregamento.AguardandoConfirmacao;
                else if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaCancelada)
                    situacaoMotorista = SituacaoMotoristaFilaCarregamento.CargaCancelada;
                else if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaRecusada)
                    situacaoMotorista = SituacaoMotoristaFilaCarregamento.RecusouCarga;
                else if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.SenhaPerdida)
                    situacaoMotorista = SituacaoMotoristaFilaCarregamento.PerdeuSenha;
            }

            string descricaoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Descricao ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(filaCarregamentoVeiculo.Carga?.NumeroDoca))
                descricaoCentroCarregamento = string.Concat(descricaoCentroCarregamento, ", Doca ", filaCarregamentoVeiculo.Carga.NumeroDoca);

            return new Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento()
            {
                Codigo = filaCarregamentoVeiculo.Codigo,
                CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                DescricaoCentroCarregamento = descricaoCentroCarregamento,
                Posicao = filaCarregamentoVeiculo.Posicao,
                Situacao = situacao,
                SituacaoMotorista = situacaoMotorista,
                Tipo = filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Reversa ? TipoFilaCarregamento.Reversa : TipoFilaCarregamento.Vazio
            };
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento AceitarCarga(int codigoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFila(codigoMotorista);

            servicoFilaCarregamentoVeiculo.AceitarCarga(filaCarregamentoVeiculo, tipoServicoMultisoftware);

            CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo);
            CriarNotificacaoJanelaCarregamentoAtualizadaOutroAmbiente(filaCarregamentoVeiculo);

            return ObterRetorno(filaCarregamentoVeiculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento Adicionar(int codigoMotorista, TipoFilaCarregamentoVeiculo tipo, string latitude, string longitude, bool lojaProximidade, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = Adicionar(codigoMotorista, latitude, longitude, lojaProximidade, (servicoFilaCarregamentoVeiculo) =>
            {
                return servicoFilaCarregamentoVeiculo.ObterTipoRetornoCarga(tipo);
            }, tipoServicoMultisoftware).Item2;

            return ObterRetorno(filaCarregamentoVeiculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento Adicionar(int codigoMotorista, int codigoTipoRetornoCarga, string latitude, string longitude, bool lojaProximidade, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = Adicionar(codigoMotorista, latitude, longitude, lojaProximidade, (servicoFilaCarregamentoVeiculo) =>
            {
                return servicoFilaCarregamentoVeiculo.ObterTipoRetornoCarga(codigoTipoRetornoCarga);
            }, tipoServicoMultisoftware).Item2;

            return ObterRetorno(filaCarregamentoVeiculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento Adicionar(Dominio.Entidades.Veiculo veiculo, int codigoTipoRetornoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWorkContainer.UnitOfWork);

            FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = null;

            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

            if (veiculoMotorista != null)
                filaCarregamentoVeiculo = Adicionar(veiculoMotorista.Codigo, latitude: "", longitude: "", lojaProximidade: false, ObterTipoRetornoCarga: (servicoFila) =>
                {
                    return servicoFila.ObterTipoRetornoCargaSemValidacaoNulo(codigoTipoRetornoCarga);
                }, tipoServicoMultisoftware).Item2;
            else if (veiculo.IsTipoVeiculoReboque())
                filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.Adicionar(veiculo, codigoTipoRetornoCarga, tipoServicoMultisoftware);
            else
                throw new ServicoException("A tração informada não possui motorista vinculado");

            if (filaCarregamentoVeiculo != null)
            {
                servicoFilaCarregamentoVeiculo.ConfirmarChegadaVeiculo(filaCarregamentoVeiculo);

                if (veiculoMotorista == null)
                    CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo);
            }

            return ObterRetorno(filaCarregamentoVeiculo);
        }

        public void AdicionarPorVeiculosAtrelados(Dominio.Entidades.Veiculo tracao, Dominio.Entidades.Veiculo reboque, int codigoTipoRetornoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!reboque.IsTipoVeiculoReboque())
                throw new ServicoException("O veículo atrelado deve ser um reboque");

            try
            {
                _unitOfWorkContainer.StartContainer();

                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(tracao.Codigo);

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = Adicionar(veiculoMotorista.Codigo, latitude: "", longitude: "", lojaProximidade: false, ObterTipoRetornoCarga: (servicoFila) =>
                {
                    return null;
                }, tipoServicoMultisoftware).Item1;

                FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.Adicionar(reboque, codigoTipoRetornoCarga, tipoServicoMultisoftware);

                servicoFilaCarregamentoVeiculo.ConfirmarChegadaVeiculo(filaCarregamentoVeiculo);
                servicoFilaCarregamentoVeiculo.AtrelarReboque(filaCarregamentoMotorista, filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChangesContainer();

                CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.RollbackContainer();
                throw;
            }
        }

        public bool ConfirmarChegadaVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculoAlterada = servicoFilaCarregamentoVeiculo.ConfirmarChegadaVeiculo(veiculo);

            if (listaFilaCarregamentoVeiculoAlterada.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculoAlterada in listaFilaCarregamentoVeiculoAlterada)
                    CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculoAlterada);

                return true;
            }

            return false;
        }

        public void ConfirmarChegadaVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);

            servicoFilaCarregamentoVeiculo.ConfirmarChegadaVeiculo(codigoFilaCarregamentoVeiculo);
        }

        public void DesatrelarVeiculo(Dominio.Entidades.Veiculo tracao, Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local)
        {
            FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado filaCarregamentoVeiculoAtrelado = servicoFilaCarregamentoVeiculo.DesatrelarVeiculo(tracao, local);

            CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo);
        }

        public void InformarDoca(int codigoMotorista, string hash)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFila(codigoMotorista);

            servicoFilaCarregamentoVeiculo.InformarDoca(filaCarregamentoVeiculo, hash);
        }

        public void RecusarCarga(int codigoMotorista)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFila(codigoMotorista);

            servicoFilaCarregamentoVeiculo.RecusarCarga(filaCarregamentoVeiculo);

            CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo);
            CriarNotificacaoJanelaCarregamentoAtualizadaOutroAmbiente(filaCarregamentoVeiculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento RemoverReversa(int codigoMotorista, string hash)
        {
            if (hash != "12345")
                throw new ServicoException("O QR Code informado é inválido.");

            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFila(codigoMotorista);

            servicoFilaCarregamentoVeiculo.RemoverReversa(filaCarregamentoVeiculo);

            CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo);

            return ObterRetorno(filaCarregamentoVeiculo);
        }

        public void Sair(int codigoMotorista, int codigoMotivoRetiradaFilaCarregamento)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFila(codigoMotorista);

            servicoFilaCarregamentoVeiculo.SolicitarSaida(filaCarregamentoVeiculo, codigoMotivoRetiradaFilaCarregamento);

            CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo);
        }

        #endregion

        #region Métodos Públicos de Consulta

        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga ObterDadosCarga(int codigoMotorista)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFila(codigoMotorista);

            if (filaCarregamentoVeiculo.Carga == null)
                throw new ServicoException("Nenhuma carga encontrada.");

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoFilaCarregamentoVeiculo.ObterCargaJanelaCarregamento(filaCarregamentoVeiculo);

            return new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga()
            {
                CodigoIntegracao = filaCarregamentoVeiculo.Carga.Codigo,
                DataCarga = filaCarregamentoVeiculo.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss"),
                DataSaida = cargaJanelaCarregamento?.DataSaida != null ? cargaJanelaCarregamento.DataSaida.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                Destino = filaCarregamentoVeiculo.Carga.DadosSumarizados?.Destinos ?? "",
                NumeroCargaEmbarcador = ObterNumeroCarga(filaCarregamentoVeiculo.Carga),
                Origem = filaCarregamentoVeiculo.Carga.Filial?.Descricao ?? "",
                Pedidos = ObterPedidosPorCarga(filaCarregamentoVeiculo.Carga.Codigo),
                Peso = filaCarregamentoVeiculo.Carga.DadosSumarizados?.PesoTotal.ToString("n2"),
                SituacaoCarga = cargaJanelaCarregamento != null && cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgEncosta ? SituacaoCarga.NaLogistica : SituacaoCarga.EmTransporte,
                TipoCarga = filaCarregamentoVeiculo.Carga.TipoDeCarga?.Descricao ?? "",
            };
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> ObterDadosCargasMotorista(int codigoMotorista)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWorkContainer.UnitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasAbertasPorMotorista(codigoMotorista, configuracao.HorasCargaExibidaNoApp ?? 0);

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> cargasRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                cargasRetornar.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga()
                {
                    CodigoIntegracao = carga.Codigo,
                    DataCarga = carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss"),
                    Destino = carga.DadosSumarizados?.Destinos ?? "",
                    NumeroCargaEmbarcador = ObterNumeroCarga(carga),
                    Filial = carga.Filial?.Descricao ?? "",
                    Pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido>(),
                    Peso = carga.DadosSumarizados?.PesoTotal.ToString("n2")
                });
            }

            return cargasRetornar;
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento ObterDadosFilaCarregamento(int codigoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFilaSemValidacaoNulo(codigoMotorista);

            if (filaCarregamentoVeiculo != null)
                return ObterRetorno(filaCarregamentoVeiculo);

            filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoDisponivelVincularFilaCarregamentoMotoristaPorMotorista(codigoMotorista);

            if (filaCarregamentoVeiculo == null)
                return null;

            try
            {
                _unitOfWorkContainer.Start();

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = new FilaCarregamentoMotorista(_unitOfWorkContainer, _origemAlteracao).Adicionar(codigoMotorista);

                servicoFilaCarregamentoVeiculo.VincularFilaCarregamentoMotorista(filaCarregamentoMotorista, filaCarregamentoVeiculo, tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();

                CriarNotificacaoFilaAlteradaOutroAmbiente(filaCarregamentoVeiculo);

                return ObterRetorno(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public string ObterDetalhesCarga(int codigoMotorista)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFila(codigoMotorista);

            return servicoFilaCarregamentoVeiculo.ObterDetalhesCarga(filaCarregamentoVeiculo);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo> ObterListaFilaCarregamentoAguardandoChegadaVeiculo(int codigoUsuario)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Motorista);
            List<int> listaCodigoCentroCarregamentoPorOperadorLogistica = ObterListaCodigoCentroCarregamentoPorOperadorLogistica(codigoUsuario);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterListaFilaCarregamentoVeiculoAguardandoChegadaVeiculo(listaCodigoCentroCarregamentoPorOperadorLogistica);

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo> filasCarregamentoVeiculoRetornar = (
                from filaCarregamentoVeiculo in filasCarregamentoVeiculo
                select ObterFilaCarregamentoVeiculoRetornar(filaCarregamentoVeiculo)
            ).ToList();

            return filasCarregamentoVeiculoRetornar;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga> ObterListaTipoRetornoCarga()
        {
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> listaTipoRetornoCarga = repositorioTipoRetornoCarga.BuscarPorAtivo();

            return (
                from tipo in listaTipoRetornoCarga
                select new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga()
                {
                    Codigo = tipo.Codigo,
                    Descricao = tipo.Descricao
                }
            ).ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao ObterNotificacao(int codigoNotificacao)
        {
            Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario repositorioNotificacaoUsuario = new Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario notificacaoMotorista = repositorioNotificacaoUsuario.BuscarPorCodigo(codigoNotificacao, auditavel: false) ?? throw new ServicoException("Notificação não encontrada");

            if (!notificacaoMotorista.DataLeitura.HasValue)
            {
                notificacaoMotorista.DataLeitura = DateTime.Now;

                repositorioNotificacaoUsuario.Atualizar(notificacaoMotorista);
            }

            return new Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao()
            {
                Assunto = notificacaoMotorista.Notificacao.Assunto,
                CodigoCentroCarregamento = notificacaoMotorista.Notificacao.CentroCarregamento?.Codigo ?? 0,
                Data = notificacaoMotorista.Notificacao.Data.ToString("dd/MM/yyyy HH:mm"),
                DescricaoCentroCarregamento = notificacaoMotorista.Notificacao.CentroCarregamento?.Descricao ?? "",
                Mensagem = notificacaoMotorista.Notificacao.Mensagem
            };
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida> ObterNotificacoes(int codigoMotorista, bool somenteNaoLidas)
        {
            Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario repositorioNotificacaoUsuario = new Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> notificacoesMotorista = repositorioNotificacaoUsuario.ObterNotificacoesPorUsuario(codigoMotorista, somenteNaoLidas);

            return (
                from notificacaoMotorista in notificacoesMotorista
                select new Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida()
                {
                    Codigo = notificacaoMotorista.Codigo,
                    Assunto = notificacaoMotorista.Notificacao.Assunto,
                    Data = notificacaoMotorista.Notificacao.Data.ToString("dd/MM/yyyy HH:mm"),
                    Lida = notificacaoMotorista.DataLeitura.HasValue
                }
            ).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido> ObterPedidosPorCarga(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargasPedidos = repositorioCargaPedido.BuscarPorCarga(codigoCarga);
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargasPedidos)
            {
                Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido pedido = new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido()
                {
                    Peso = cargaPedido.Pedido.PesoTotal.ToString("n2")
                };

                if (cargaPedido.Pedido.Destinatario != null)
                {
                    pedido.CNPJCliente = cargaPedido.Pedido.Destinatario.CPF_CNPJ_SemFormato;
                    pedido.NomeCliente = cargaPedido.Pedido.Destinatario.Descricao;
                    pedido.Destino = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.Destinatario.CodigoIntegracao) ? cargaPedido.Pedido.Destinatario.CodigoIntegracao : cargaPedido.Pedido.Destinatario.CPF_CNPJ_Formatado + " - " + cargaPedido.Pedido.Destinatario.Localidade.Descricao + "/" + cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla;
                    pedido.EnderecoDestino = cargaPedido.Pedido.Destinatario.Endereco + ", " + cargaPedido.Pedido.Destinatario.Numero + ", " + cargaPedido.Pedido.Destinatario.Bairro;
                }

                pedidos.Add(pedido);
            }

            return pedidos;
        }

        #endregion
    }
}
