using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamento
    {
        #region Atributos

        private Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoCarregamento _configuracaoCarregamento;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamento(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, configuracaoCarregamento: null) { }

        public CargaJanelaCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, configuracaoCarregamento: null) { }

        public CargaJanelaCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoCarregamento configuracaoCarregamento)
        {
            _configuracaoCarregamento = configuracaoCarregamento ?? new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoCarregamento();
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AdicionarPorDestinoDaCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Carga.CargaAgrupamento != null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisDestino = repositorioCargaPedido.BuscarFiliaisPorDestinatariosDaCarga(cargaJanelaCarregamento.Carga.Codigo);

            if (filiaisDestino.Count == 0)
                return;

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiaisDestino)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(cargaJanelaCarregamento.Carga.TipoDeCarga?.Codigo ?? 0, filial.Codigo, true, cargaJanelaCarregamento?.Carga ?? null);

                if (!(centroCarregamento?.GerarJanelaCarregamentoDestino ?? false))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaDescarregamento = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento()
                {
                    Carga = cargaJanelaCarregamento.Carga,
                    TransportadorOriginal = cargaJanelaCarregamento.Carga.Empresa,
                    CentroCarregamento = centroCarregamento,
                    Tipo = TipoCargaJanelaCarregamento.Descarregamento
                };

                DefinirInformacoesPorDestinoDaCarga(cargaJanelaDescarregamento, cargaJanelaCarregamento, filial);
            }
        }

        private void AtualizarDataInicialColetaDosPedidos(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (!cargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            if (!configuracaoJanelaCarregamento.AtualizarDataInicialColetaAoAlterarHorarioCarregamento)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                cargaPedido.Pedido.DataInicialColeta = cargaJanelaCarregamento.InicioCarregamento;
                repositorioPedido.Atualizar(cargaPedido.Pedido);
            }
        }

        public string VerificarEGerarObservacaoFinal(string observacaoJanela, string observacaoCarga)
        {
            if (string.IsNullOrWhiteSpace(observacaoJanela))
                return string.Empty;

            string obsJanela = observacaoJanela.Trim();
            string obsCarga = observacaoCarga?.Trim() ?? string.Empty;

            if (observacaoJanela.Contains("/") && (!string.IsNullOrEmpty(obsCarga)))
            {
                string[] observacao = observacaoJanela.Split('/');
                string primeiraParte = observacao[0].Trim();
                return $"{primeiraParte} / {obsCarga}";
            }

            if (!string.Equals(obsJanela, obsCarga, StringComparison.OrdinalIgnoreCase) && obsJanela.IndexOf(obsCarga, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return $"{obsJanela} / {obsCarga}";
            }

            return obsJanela;
        }

        private void DefinirCentroCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            cargaJanelaCarregamento.CentroCarregamento = ObterCentroCarregamento(cargaJanelaCarregamento.CargaBase);
        }

        private void DefinirInformacoesPorCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            bool adicionarCargaJanelaCarregamento = cargaJanelaCarregamento.Codigo <= 0;
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamentoAnterior = cargaJanelaCarregamento.CentroCarregamento;

            DefinirCentroCarregamento(cargaJanelaCarregamento);
            DefinirRotaDaCarga(cargaJanelaCarregamento);

            if (adicionarCargaJanelaCarregamento || !cargaJanelaCarregamento.IsTempoCarregamentoValido())
            {
                DefinirHorarioCarregamentoInicialPorCarga(cargaJanelaCarregamento);
                AtualizarDataInicialColetaDosPedidos(cargaJanelaCarregamento);
            }

            if (adicionarCargaJanelaCarregamento)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                bool encaixarHorario = false;

                if (cargaJanelaCarregamento.Carga.Carregamento != null)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial repositorioCarregamentoFilial = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoFilial(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial dadosPorFilial = repositorioCarregamentoFilial.BuscarPorCarregamentoEFilial(cargaJanelaCarregamento.Carga.Carregamento.Codigo, cargaJanelaCarregamento.Carga.Filial?.Codigo ?? 0);
                    encaixarHorario = dadosPorFilial?.EncaixarHorario ?? cargaJanelaCarregamento.Carga.Carregamento.EncaixarHorario;
                }

                cargaJanelaCarregamento.DataProximaSituacao = DateTime.Now;
                cargaJanelaCarregamento.HorarioEncaixado = encaixarHorario;
                cargaJanelaCarregamento.ObservacaoTransportador = repositorioCargaPedido.BuscarObservacaoPedidoPorCarga(cargaJanelaCarregamento.Carga.Codigo);
                cargaJanelaCarregamento.Peso = cargaJanelaCarregamento.Carga.DadosSumarizados?.PesoTotal ?? 0;
                cargaJanelaCarregamento.Volume = cargaJanelaCarregamento.Carga.DadosSumarizados?.VolumesTotal ?? 0;
                cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores;
                cargaJanelaCarregamento.TransportadorExclusivo = ObterTransportadorExclusivo(cargaJanelaCarregamento);
                cargaJanelaCarregamento.TransportadorOriginal = cargaJanelaCarregamento.Carga.Empresa;

                repositorioCargaJanelaCarregamento.Inserir(cargaJanelaCarregamento);
            }

            DisponibilizarParaTransportadores(cargaJanelaCarregamento, tipoServicoMultisoftware, adicionarCargaJanelaCarregamento);
            DefinirHorarioCarregamentoPorCarga(cargaJanelaCarregamento, adicionarCargaJanelaCarregamento);
            DefinirPrioridade(cargaJanelaCarregamento, centroCarregamentoAnterior);

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);

            new Integracao.SaintGobain.IntegracaoSaintGobain(_unitOfWork).ReenviarIntegrarCarregamento(cargaJanelaCarregamento);

            if (adicionarCargaJanelaCarregamento)
                new ControleCarregamento(_unitOfWork).Criar(cargaJanelaCarregamento);
        }

        private void DefinirInformacoesPorDestinoDaCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            bool adicionarCargaJanelaDescarregamento = cargaJanelaDescarregamento.Codigo <= 0;
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamentoAnterior = cargaJanelaCarregamento.CentroCarregamento;

            if (adicionarCargaJanelaDescarregamento || !cargaJanelaDescarregamento.IsTempoCarregamentoValido())
                DefinirHorarioCarregamentoInicialPorDestinoDaCarga(cargaJanelaDescarregamento, cargaJanelaCarregamento, filial);

            if (adicionarCargaJanelaDescarregamento)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                cargaJanelaDescarregamento.DataProximaSituacao = DateTime.Now;
                cargaJanelaDescarregamento.HorarioEncaixado = cargaJanelaCarregamento.HorarioEncaixado;
                cargaJanelaDescarregamento.Peso = repositorioCargaPedido.BuscarPesoPorDestinatariosDaCarga(cargaJanelaDescarregamento.Carga.Codigo, double.Parse(filial.CNPJ));
                cargaJanelaDescarregamento.Volume = repositorioCargaPedido.BuscarVolumesPorDestinatariosDaCarga(cargaJanelaDescarregamento.Carga.Codigo, double.Parse(filial.CNPJ));
                cargaJanelaDescarregamento.Situacao = SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores;

                repositorioCargaJanelaCarregamento.Inserir(cargaJanelaDescarregamento);
            }

            DefinirHorarioCarregamentoPorCarga(cargaJanelaDescarregamento, adicionarCargaJanelaDescarregamento);
            DefinirPrioridade(cargaJanelaDescarregamento, centroCarregamentoAnterior);

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaDescarregamento);
        }

        private void DefinirInformacoesPorPreCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool atualizarProgramacaoCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            bool adicionarCargaJanelaCarregamento = cargaJanelaCarregamento.Codigo <= 0;
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamentoAnterior = cargaJanelaCarregamento.CentroCarregamento;

            DefinirCentroCarregamento(cargaJanelaCarregamento);
            DefinirRotaDaPreCarga(cargaJanelaCarregamento);
            DefinirHorarioCarregamentoInicialPorPreCarga(cargaJanelaCarregamento, atualizarProgramacaoCarregamento);

            if (adicionarCargaJanelaCarregamento)
            {
                ControleCarregamento servicoControleCarregamento = new ControleCarregamento(_unitOfWork);

                cargaJanelaCarregamento.DataProximaSituacao = DateTime.Now;
                cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemTransportador;

                repositorioCargaJanelaCarregamento.Inserir(cargaJanelaCarregamento);
                servicoControleCarregamento.Criar(cargaJanelaCarregamento);
            }

            DefinirHorarioCarregamentoPorPreCarga(cargaJanelaCarregamento, adicionarCargaJanelaCarregamento, atualizarProgramacaoCarregamento);
            DefinirPrioridade(cargaJanelaCarregamento, centroCarregamentoAnterior);

            cargaJanelaCarregamento.DataProximaSituacao = new PrazoSituacaoCarga(_unitOfWork, configuracaoEmbarcador).ObterDataProximaSituacao(cargaJanelaCarregamento);

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        private void DefinirHorarioCarregamentoInicialPorCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoEmbarcador);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repositorioCargaPedido.BuscarPrimeiraPorCargaOrdenadoPelaDataMenorDataCarregamento(cargaJanelaCarregamento?.Carga?.Codigo ?? 0);

            if ((cargaJanelaCarregamento.CentroCarregamento?.PegarObrigatoriamenteHorarioDaPrimeiraColetaParaDataDeCarregamento ?? false) && (primeiroCargaPedido != null) && primeiroCargaPedido.Pedido.DataCarregamentoPedido.HasValue)
                cargaJanelaCarregamento.InicioCarregamento = primeiroCargaPedido.Pedido.DataCarregamentoPedido.Value;
            else
            {
                if (Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoAmbiente().CalcularHorarioDoCarregamento.HasValue && Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoAmbiente().CalcularHorarioDoCarregamento.Value == true)
                    cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.Carga.DataCarregamentoCarga ?? DateTime.Now.Date.AddDays(cargaJanelaCarregamento.CentroCarregamento?.DiasAdicionaisAlocacaoCargaJanelaCarregamento ?? 1);
                else
                    cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.Carga.DataCarregamentoCarga ?? DateTime.Now.Date;

                TimeSpan? horaLimite = cargaJanelaCarregamento.Carga.Rota?.HoraInicioCarregamento;

                if (horaLimite.HasValue)
                {
                    cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.InicioCarregamento.Date.AddMinutes(horaLimite.Value.TotalMinutes);

                    if (DateTime.Now > cargaJanelaCarregamento.InicioCarregamento)
                        cargaJanelaCarregamento.InicioCarregamento = DateTime.Now.Date.AddDays(1).AddMinutes(horaLimite.Value.TotalMinutes);
                }
            }

            cargaJanelaCarregamento.TempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterTempoCarregamento(cargaJanelaCarregamento, cargaJanelaCarregamento.InicioCarregamento.TimeOfDay);
            cargaJanelaCarregamento.TerminoCarregamento = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);

            if (!cargaJanelaCarregamento.CarregamentoReservado)
                cargaJanelaCarregamento.DataCarregamentoProgramada = cargaJanelaCarregamento.InicioCarregamento;
        }

        private void DefinirHorarioCarregamentoInicialPorDestinoDaCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoEmbarcador);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorClienteECarga(cargaJanelaDescarregamento.Carga.Codigo, double.Parse(filial.CNPJ));
            DateTime? dataInicioDescarregamento = cargaEntrega?.DataPrevista ?? cargaJanelaDescarregamento.Carga.DataInicioViagemPrevista ?? cargaJanelaCarregamento?.TerminoCarregamento ?? cargaJanelaDescarregamento.Carga.DataCarregamentoCarga;

            cargaJanelaDescarregamento.InicioCarregamento = dataInicioDescarregamento.Value;
            cargaJanelaDescarregamento.TempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterTempoCarregamento(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioCarregamento.TimeOfDay);
            cargaJanelaDescarregamento.TerminoCarregamento = cargaJanelaDescarregamento.InicioCarregamento.AddMinutes(cargaJanelaDescarregamento.TempoCarregamento);
            cargaJanelaDescarregamento.DataCarregamentoProgramada = cargaJanelaDescarregamento.InicioCarregamento;
        }

        private void DefinirHorarioCarregamentoInicialPorPreCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool atualizarProgramacaoCarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoEmbarcador);

            if (cargaJanelaCarregamento.PreCarga.EscalaVeiculoEscalado == null)
            {
                if (cargaJanelaCarregamento.PreCarga.Rota != null)
                    cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.PreCarga.DataPrevisaoEntrega.Value.AddMinutes(-cargaJanelaCarregamento.PreCarga.Rota.TempoDeViagemEmMinutos);
                else
                    cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.PreCarga.DataPrevisaoEntrega.Value;
            }
            else
                cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.PreCarga.EscalaVeiculoEscalado.EscalaOrigemDestino.ExpedicaoEscala.GeracaoEscala.DataEscala.Add(cargaJanelaCarregamento.PreCarga.EscalaVeiculoEscalado.HoraCarregamento);

            cargaJanelaCarregamento.TempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterTempoCarregamento(cargaJanelaCarregamento, cargaJanelaCarregamento.InicioCarregamento.TimeOfDay);
            cargaJanelaCarregamento.TerminoCarregamento = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);

            if (!cargaJanelaCarregamento.CarregamentoReservado || atualizarProgramacaoCarregamento)
                cargaJanelaCarregamento.DataCarregamentoProgramada = cargaJanelaCarregamento.InicioCarregamento;
        }

        private void DefinirHorarioCarregamentoPorCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool adicionarCargaJanelaCarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (cargaJanelaCarregamento.CentroCarregamento == null)
            {
                cargaJanelaCarregamento.Excedente = false;
                return;
            }

            if (!cargaJanelaCarregamento.IsTempoCarregamentoValido())
            {
                if (configuracaoEmbarcador.BloquearGeracaoCargaComJanelaCarregamentoExcedente)
                    throw new ServicoException("Não existe uma configuração de tempo de carregamento criada para as configurações desta carga");

                cargaJanelaCarregamento.Excedente = true;
                return;
            }

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKlios repositorioIntegracaoKlios = new Repositorio.Embarcador.Configuracoes.IntegracaoKlios(_unitOfWork);

            if (
                repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Klios) &&
                (repositorioIntegracaoKlios.Buscar()?.PossuiIntegracao ?? false) &&
                (cargaJanelaCarregamento.Carga.Veiculo != null && cargaJanelaCarregamento.Carga.Motoristas != null && cargaJanelaCarregamento.Carga.Motoristas.Count > 0)
            )
            {
                cargaJanelaCarregamento.Excedente = true;
                return;
            }

            if ((cargaJanelaCarregamento.CentroCarregamento.CargasComoExcedentesNaJanela || cargaJanelaCarregamento.CentroCarregamento.PermiteTransportadorSelecionarHorarioCarregamento) && !cargaJanelaCarregamento.HorarioEncaixado && (adicionarCargaJanelaCarregamento || cargaJanelaCarregamento.Excedente))
            {
                cargaJanelaCarregamento.Excedente = true;
                return;
            }

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidade = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
            {
                BloquearJanelaCarregamentoExcedente = configuracaoEmbarcador.BloquearGeracaoCargaComJanelaCarregamentoExcedente,
                CodigoCarga = cargaJanelaCarregamento.Carga.Codigo,
                CodigoModeloVeicularCarga = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.Codigo ?? 0,
                CodigoTipoOperacao = cargaJanelaCarregamento.Carga.TipoOperacao?.Codigo ?? 0,
                CodigoTransportador = cargaJanelaCarregamento.Carga.Empresa?.Codigo ?? 0,
                PermitirCapacidadeCarregamentoExcedida = _configuracaoCarregamento.PermitirCapacidadeCarregamentoExcedida,
                PermitirHorarioCarregamentoComLimiteAtingido = !adicionarCargaJanelaCarregamento || _configuracaoCarregamento.PermitirHorarioCarregamentoComLimiteAtingido,
                PermitirHorarioCarregamentoInferiorAoAtual = _configuracaoCarregamento.PermitirHorarioCarregamentoInferiorAoAtual,
                DiasLimiteParaDefinicaoHorarioCarregamento = _configuracaoCarregamento.DiasLimiteParaDefinicaoHorarioCarregamento
            };

            if (cargaJanelaCarregamento.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                configuracaoDisponibilidade.CpfCnpjCliente = repositorioCargaPedido.BuscarPrimeiroDestinatarioDePedidoPorCarga(cargaJanelaCarregamento.Carga.Codigo)?.CPF_CNPJ ?? 0d;
            else
                configuracaoDisponibilidade.CpfCnpjCliente = double.Parse(cargaJanelaCarregamento.CentroCarregamento.Filial.CNPJ);

            if ((cargaJanelaCarregamento.InicioCarregamento <= DateTime.Now) && cargaJanelaCarregamento.CentroCarregamento.SeDataInformadaForInferiorDataAtualUtilizarDataAtualComoReferenciaHorarioInicialJanelaCarregamento)
                cargaJanelaCarregamento.InicioCarregamento = DateTime.Now.AddMinutes(2);

            DateTime dataCarregamentoAnterior = cargaJanelaCarregamento.CarregamentoReservado ? cargaJanelaCarregamento.DataCarregamentoProgramada : cargaJanelaCarregamento.InicioCarregamento;
            CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new CargaJanelaCarregamentoDisponibilidade(_unitOfWork, configuracaoEmbarcador, configuracaoDisponibilidade);

            try
            {
                servicoCargaJanelaCarregamentoDisponibilidade.DefinirHorarioCarregamento(cargaJanelaCarregamento);
            }
            catch (ServicoException excecao) when (
                (excecao.ErrorCode == CodigoExcecao.PrevisaoCarregamentoIndisponivel) ||
                (excecao.ErrorCode == CodigoExcecao.HorarioCarregamentoIndisponivel) ||
                (excecao.ErrorCode == CodigoExcecao.HorarioCarregamentoInferiorAtual) ||
                (excecao.ErrorCode == CodigoExcecao.HorarioCarregamentoInferiorAoTolerado) ||
                (excecao.ErrorCode == CodigoExcecao.HorarioLimiteCarregamentoAtingido)
            )
            {
                if (servicoCargaJanelaCarregamentoDisponibilidade.IsCalcularHorarioCarregamento() || (configuracaoDisponibilidade.DiasLimiteParaDefinicaoHorarioCarregamento <= 0))
                    throw;

                cargaJanelaCarregamento.InicioCarregamento = dataCarregamentoAnterior;
                servicoCargaJanelaCarregamentoDisponibilidade.DefinirHorarioCarregamentoAteLimiteTentativas(cargaJanelaCarregamento);
            }

            if (cargaJanelaCarregamento.Excedente)
                return;

            if (cargaJanelaCarregamento.Tipo == TipoCargaJanelaCarregamento.Carregamento)
            {
                if (!cargaJanelaCarregamento.Carga.DataCarregamentoCarga.HasValue)
                    cargaJanelaCarregamento.Carga.DataPrevisaoTerminoCarga = cargaJanelaCarregamento.TerminoCarregamento;

                cargaJanelaCarregamento.Carga.DataCarregamentoCarga = cargaJanelaCarregamento.InicioCarregamento;
            }

            new Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
        }

        private void DefinirHorarioCarregamentoPorPreCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool adicionarCargaJanelaCarregamento, bool atualizarProgramacaoCarregamento)
        {
            if (cargaJanelaCarregamento.CentroCarregamento == null)
            {
                cargaJanelaCarregamento.Excedente = false;
                return;
            }

            if (cargaJanelaCarregamento.TempoCarregamento == 0)
            {
                cargaJanelaCarregamento.Excedente = true;
                return;
            }

            if ((cargaJanelaCarregamento.InicioCarregamento <= DateTime.Now) && cargaJanelaCarregamento.CentroCarregamento.SeDataInformadaForInferiorDataAtualUtilizarDataAtualComoReferenciaHorarioInicialJanelaCarregamento)
                cargaJanelaCarregamento.InicioCarregamento = DateTime.Now.AddMinutes(2);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidade = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
            {
                PermitirHorarioCarregamentoComLimiteAtingido = !(adicionarCargaJanelaCarregamento || atualizarProgramacaoCarregamento)
            };
            CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new CargaJanelaCarregamentoDisponibilidade(_unitOfWork, configuracaoEmbarcador, configuracaoDisponibilidade);

            servicoCargaJanelaCarregamentoDisponibilidade.DefinirHorarioCarregamento(cargaJanelaCarregamento);

            if (cargaJanelaCarregamento.Excedente)
                return;

            new Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
        }

        private void DefinirPrioridade(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamentoAnterior)
        {
            CargaJanelaCarregamentoPrioridade servicoCargaJanelaCarregamentoPrioridade = new CargaJanelaCarregamentoPrioridade(_unitOfWork);

            if (cargaJanelaCarregamento.Excedente || (centroCarregamentoAnterior?.Codigo != cargaJanelaCarregamento.CentroCarregamento?.Codigo))
                servicoCargaJanelaCarregamentoPrioridade.RemoverPrioridade(cargaJanelaCarregamento, centroCarregamentoAnterior);

            if (!cargaJanelaCarregamento.Excedente && ((centroCarregamentoAnterior?.Codigo != cargaJanelaCarregamento.CentroCarregamento?.Codigo) || (cargaJanelaCarregamento.Prioridade <= 0)))
                servicoCargaJanelaCarregamentoPrioridade.AdicionarPrioridade(cargaJanelaCarregamento);
        }

        private void DefinirRotaDaCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Carga.Rota != null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);
            List<Dominio.Entidades.Localidade> destinos = (from o in cargaPedidos where o.Destino != null select o.Destino).Distinct().ToList();

            if (destinos.Count == 0)
                return;

            Dominio.Entidades.Localidade origem = cargaPedidos.First().Origem;
            List<Dominio.Entidades.Estado> estadosDestino = (from o in destinos select o.Estado).Distinct().ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestinoOrdenadas = destinos.Select(o => new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem() { Localidade = o }).ToList();

            Dominio.Entidades.RotaFrete rotaFrete = ObterRota(origem, localidadesDestinoOrdenadas, estadosDestino);

            if (cargaJanelaCarregamento.Carga.TipoOperacao != null && rotaFrete != null)
            {
                if (!(rotaFrete.TipoOperacao == null || rotaFrete.TipoOperacao.Codigo == cargaJanelaCarregamento.Carga.TipoOperacao.Codigo))
                    rotaFrete = null;
            }

            cargaJanelaCarregamento.Carga.Rota = rotaFrete;

            new Servicos.Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(cargaJanelaCarregamento.Carga.Rota);
        }

        private void DefinirRotaDaPreCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            List<Dominio.Entidades.Cliente> destinatarios = cargaJanelaCarregamento.PreCarga.Destinatarios?.ToList() ?? new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Localidade> destinos = (from o in destinatarios where o.Localidade != null select o.Localidade).Distinct().ToList();

            if (destinos.Count == 0)
                return;

            Dominio.Entidades.Localidade origem = cargaJanelaCarregamento.PreCarga.Filial.Localidade;
            List<Dominio.Entidades.Estado> estadosDestino = (from o in destinos select o.Estado).Distinct().ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestinoOrdenadas = destinos.Select(o => new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem() { Localidade = o }).ToList();

            cargaJanelaCarregamento.PreCarga.Rota = ObterRota(origem, localidadesDestinoOrdenadas, estadosDestino);
        }

        private void DisponibilizarParaTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool adicionarCargaJanelaCarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
            {
                CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new CargaJanelaCarregamentoTransportadorTerceiro(_unitOfWork, configuracaoEmbarcador);

                servicoCargaJanelaCarregamentoTransportadorTerceiro.DisponibilizarAutomaticamenteParaTransportadoresTerceiros(cargaJanelaCarregamento, tipoServicoMultisoftware);
            }
            else
            {
                CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new CargaJanelaCarregamentoTransportador(_unitOfWork, configuracaoEmbarcador);

                if (cargaJanelaCarregamento.Carga?.Carregamento?.GrupoTransportador != null)
                {
                    if (adicionarCargaJanelaCarregamento)
                    {
                        CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new CargaJanelaCarregamentoCotacao(_unitOfWork, configuracaoEmbarcador);
                        CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);

                        servicoCargaJanelaCarregamentoCotacao.LiberarParaCotacaoAutomaticamente(cargaJanelaCarregamento, tipoServicoMultisoftware);
                        servicoCargaJanelaCarregamentoTransportador.DisponibilizarParaTransportadoresPorGrupoTransportador(cargaJanelaCarregamento, cargaJanelaCarregamento.Carga.Carregamento.GrupoTransportador);
                        servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaLiberadaParaCotacaoParaTranportadores(cargaJanelaCarregamento);
                    }
                }
                else
                    servicoCargaJanelaCarregamentoTransportador.DisponibilizarAutomaticamenteParaTransportadores(cargaJanelaCarregamento, tipoServicoMultisoftware);
            }
        }

        private bool IsPermitirAtualizarSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgEncosta)
                return false;

            if ((cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgAprovacaoComercial) || (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.ReprovacaoComercial))
                return false;

            if ((cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores) && (cargaJanelaCarregamento.CentroCarregamento?.LiberarCargaManualmenteParaTransportadores ?? false))
                return false;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if ((cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores) && (cargaJanelaCarregamento.Carga.Empresa == null))
                return configuracaoEmbarcador.UtilizarFilaCarregamento;

            if ((cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador) && configuracaoEmbarcador.UtilizarFilaCarregamento)
            {
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unidadeDeTrabalho);
                bool possuiFilaCarregamentoVeiculoAguardandoConfirmacao = (cargaJanelaCarregamento.Carga != null) && repositorioFilaCarregamentoVeiculo.ExistePorCargaESituacao(cargaJanelaCarregamento.Carga.Codigo, SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao);

                return !possuiFilaCarregamentoVeiculoAguardandoConfirmacao;
            }

            return true;
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);

            return repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(cargaBase?.TipoDeCarga?.Codigo ?? 0, cargaBase?.Filial?.Codigo ?? 0, true);
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private Dominio.Entidades.RotaFrete ObterRota(Dominio.Entidades.Localidade origem, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestinoOrdenadas, List<Dominio.Entidades.Estado> estadosDestino)
        {
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            List<Dominio.Entidades.RotaFrete> rotasPorOrigemEDestinos = repositorioRotaFrete.BuscarPorOrigemEDestinos(origem, localidadesDestinoOrdenadas, estadosDestino);

            if (rotasPorOrigemEDestinos.Count > 0)
                return rotasPorOrigemEDestinos.FirstOrDefault();

            Dominio.Entidades.RotaFrete rotaPorOrigemEEstadoDestino = repositorioRotaFrete.BuscarPorOrigemEEstadoDestino(origem?.Codigo ?? 0, estadosDestino?.FirstOrDefault()?.Sigla ?? "", ativo: true);

            return rotaPorOrigemEEstadoDestino;
        }

        private Dominio.Entidades.Empresa ObterTransportadorExclusivo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.CentroCarregamento == null)
                return null;

            List<Dominio.Entidades.Cliente> clientesDestinatarios = cargaJanelaCarregamento.CargaBase?.DadosSumarizados?.ClientesDestinatarios?.ToList();

            if ((clientesDestinatarios == null) || (clientesDestinatarios.Count == 0))
                return null;

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador centroCarregamentoTransportador in cargaJanelaCarregamento.CentroCarregamento.Transportadores)
            {
                foreach (Dominio.Entidades.Cliente clienteDestino in centroCarregamentoTransportador.ClientesDestino)
                {
                    if (clientesDestinatarios.Any(o => o.CPF_CNPJ == clienteDestino.CPF_CNPJ))
                        return centroCarregamentoTransportador.Transportador;
                }
            }

            return null;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento AdicionarPorCarga(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            if (carga == null)
                throw new ServicoException("Não foi possível encontrar a carga");

            if (carga.TipoOperacao?.NaoUtilizaJanelaCarregamento ?? false)
                return null;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);
            bool adicionarCargaJanelaCarregamento = (cargaJanelaCarregamento == null);

            if (adicionarCargaJanelaCarregamento)
                cargaJanelaCarregamento = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento()
                {
                    Carga = carga,
                    Tipo = TipoCargaJanelaCarregamento.Carregamento,
                    TransportadorOriginal = carga.Empresa
                };

            DefinirInformacoesPorCarga(cargaJanelaCarregamento, tipoServicoMultisoftware);

            if (adicionarCargaJanelaCarregamento)
                AdicionarPorDestinoDaCarga(cargaJanelaCarregamento);

            return cargaJanelaCarregamento;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento AdicionarPorPreCarga(int codigoPreCarga)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorCodigo(codigoPreCarga);

            if (preCarga == null)
                throw new ServicoException("Não foi possível encontrar o pré planejamento");

            if (preCarga.ProgramacaoCarga)
                return null;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(preCarga.Codigo);
            bool adicionarCargaJanelaCarregamento = (cargaJanelaCarregamento == null);

            if (adicionarCargaJanelaCarregamento)
                cargaJanelaCarregamento = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento()
                {
                    PreCarga = preCarga,
                    Tipo = TipoCargaJanelaCarregamento.Carregamento
                };

            DefinirInformacoesPorPreCarga(cargaJanelaCarregamento, atualizarProgramacaoCarregamento: false);

            return cargaJanelaCarregamento;
        }

        public void AlterarSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento situacao)
        {
            PrazoSituacaoCarga servicoPrazoSituacaoCarga = new PrazoSituacaoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            cargaJanelaCarregamento.Situacao = situacao;
            cargaJanelaCarregamento.DataSituacaoAtual = DateTime.Now;
            cargaJanelaCarregamento.DataProximaSituacao = servicoPrazoSituacaoCarga.ObterDataProximaSituacao(cargaJanelaCarregamento);

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        public void AtualizarDataPrevisaoChegada(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dataPrevisaoChegada)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            cargaJanelaCarregamento.DataPrevisaoChegada = dataPrevisaoChegada;

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        public void AtualizarPorCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((cargaJanelaCarregamento.CentroCarregamento != null) && (cargaJanelaCarregamento.Carga.Rota != null) && cargaJanelaCarregamento.Carga.SituacaoCarga.IsSituacaoCargaFaturada())
                return;

            DefinirInformacoesPorCarga(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        public void AtualizarPorPreCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool atualizarProgramacaoCarregamento)
        {
            DefinirInformacoesPorPreCarga(cargaJanelaCarregamento, atualizarProgramacaoCarregamento);
        }

        public void AtualizarObservacaoTransportadorPorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorPedido(pedido.Codigo);

            if (cargas.Count == 0)
                return;

            if (ObterConfiguracaoEmbarcador().IncluirCargaCanceladaProcessarDT)
                return;

            List<int> codigosCarga = cargas.Select(o => o.Codigo).ToList();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Logistica.CargaJanelaCarregamentoConsulta(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCargas(codigosCarga);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCarga);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == carga.Codigo select o).FirstOrDefault();

                if ((cargaJanelaCarregamento == null) || cargaJanelaCarregamento.ObservacaoTransportadorInformadaManualmente)
                    continue;

                List<string> observacoes = new List<string>();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosPorCarga = (
                    from o in cargaPedidos
                    where o.Carga.Codigo == carga.Codigo
                    orderby o.Pedido.Observacao
                    select o.Pedido
                ).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoPorCarga in pedidosPorCarga)
                {
                    if (pedidoPorCarga.Codigo == pedido.Codigo)
                    {
                        if (!string.IsNullOrWhiteSpace(pedido.Observacao))
                            observacoes.Add(pedido.Observacao);
                    }
                    else if (!string.IsNullOrWhiteSpace(pedidoPorCarga.Observacao))
                        observacoes.Add(pedidoPorCarga.Observacao);
                }

                cargaJanelaCarregamento.ObservacaoTransportador = string.Join(" | ", observacoes.Distinct()).Left(1000) ?? "";

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            }
        }

        public void AtualizarSituacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !(configuracaoJanelaCarregamento?.GerarJanelaDeCarregamento ?? false))
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            if (cargaJanelaCarregamento == null)
                return;

            AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        public void AtualizarSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaJanelaCarregamento == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            if (cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador == null)
            {
                if (!IsPermitirAtualizarSituacao(cargaJanelaCarregamento, _unitOfWork))
                    return;

                if (cargaJanelaCarregamento.Carga == null || (cargaJanelaCarregamento.Carga.CalculandoFrete && cargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Nova && !configuracaoEmbarcador.ExibirCargaSemValorFreteJanelaCarregamentoTransportador))
                    return;

                CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);
                CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new CargaJanelaCarregamentoTransportador(_unitOfWork, configuracaoEmbarcador);
                CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new CargaJanelaCarregamentoTransportadorTerceiro(_unitOfWork, configuracaoEmbarcador);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repositorioCargaMotorista.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);
                SituacaoCargaJanelaCarregamento situacaoAnteriorCargaJanelaCarregamento = cargaJanelaCarregamento.Situacao;
                bool gerarFluxoGestaoPatio = false;

                if (cargaJanelaCarregamento.Carga.ValorFreteAPagar <= 0 && (cargaJanelaCarregamento.Carga.TipoFreteEscolhido != TipoFreteEscolhido.Cliente) && !cargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete && !configuracaoEmbarcador.ExibirCargaSemValorFreteJanelaCarregamentoTransportador)
                {
                    cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemValorFrete;
                }
                else if (cargaJanelaCarregamento.Carga.Empresa == null && !configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                {
                    cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemTransportador;
                }
                else if (cargaJanelaCarregamento.Carga.Terceiro == null && configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                {
                    cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemTransportador;
                }
                else if (cargaJanelaCarregamento.Carga.Veiculo == null || cargaMotoristas.Count() == 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador;

                    if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                        cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarSemRejeicao(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.Carga.Terceiro.Codigo);
                    else
                        cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarSemRejeicao(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.Carga.Empresa.Codigo);

                    bool situacaoCargaJanelaCarregamentoTransportadadorPermiteEnviarEmail = cargaJanelaCarregamentoTransportador?.Situacao.PermitirEnviarEmailCargaDisponibilizada() ?? true;

                    repositorioCargaJanelaCarregamentoTransportador.SetarCargaJanelaCarregamentoTransportadoInteresseRejeitado(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamentoTransportador?.Codigo ?? 0);

                    if (cargaJanelaCarregamentoTransportador == null)
                    {
                        int tempoAguardarConfirmacaoTransportador = cargaJanelaCarregamento.CentroCarregamento?.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente ?? 0;

                        cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                        {
                            CargaJanelaCarregamento = cargaJanelaCarregamento,
                            HorarioLimiteConfirmarCarga = (tempoAguardarConfirmacaoTransportador > 0) ? (DateTime?)DateTime.Now.AddMinutes(tempoAguardarConfirmacaoTransportador) : null,
                            Tipo = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga
                        };

                        if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                        {
                            cargaJanelaCarregamentoTransportador.Terceiro = cargaJanelaCarregamento.Carga.Terceiro;
                            cargaJanelaCarregamentoTransportador.HorarioLiberacao = DateTime.Now;
                        }
                        else
                        {
                            cargaJanelaCarregamentoTransportador.Transportador = cargaJanelaCarregamento.Carga.Empresa;
                            cargaJanelaCarregamentoTransportador.HorarioLiberacao = servicoCargaJanelaCarregamentoTransportador.ObterHorarioLiberacao(cargaJanelaCarregamento.Carga.Empresa);
                        }

                        cargaJanelaCarregamento.DataDisponibilizacaoTransportadores = DateTime.Now;

                        DefinirDataDisponibilizacaoTransportadores(cargaJanelaCarregamento);
                        repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);
                    }

                    cargaJanelaCarregamentoTransportador.DataCargaContratada = DateTime.Now;

                    if (cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga.HasValue)
                    {
                        cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.AgAceiteTransportador;
                        cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgAceite;
                    }
                    else
                    {
                        cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador;
                        cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao;
                    }

                    if (situacaoCargaJanelaCarregamentoTransportadadorPermiteEnviarEmail)
                    {
                        servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);

                        if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                            servicoCargaJanelaCarregamentoTransportadorTerceiro.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, mensagem: $"Carga disponibilizada para o transportador terceiro {(cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao ? "informar os dados de transporte" : "realizar a confirmaçao")}");
                        else
                            servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, mensagem: $"Carga disponibilizada para o transportador {(cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao ? "informar os dados de transporte" : "realizar a confirmaçao")}");
                    }

                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);

                    gerarFluxoGestaoPatio = (cargaJanelaCarregamento.CentroCarregamento?.GerarGuaritaMesmoSemVeiculoInformado ?? false);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador;
                    bool enviarEmailCargaDisponibilizada;
                    bool salvarHistoricoConfirmacaoCarga = false;

                    if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                        cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarSemRejeicao(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.Carga?.Terceiro?.CPF_CNPJ ?? 0);
                    else
                        cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarSemRejeicao(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.Carga.Empresa.Codigo);

                    repositorioCargaJanelaCarregamentoTransportador.SetarCargaJanelaCarregamentoTransportadoInteresseRejeitado(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamentoTransportador?.Codigo ?? 0);

                    if (cargaJanelaCarregamentoTransportador == null)
                    {
                        cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                        {
                            CargaJanelaCarregamento = cargaJanelaCarregamento,
                            Tipo = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga,
                            Situacao = SituacaoCargaJanelaCarregamentoTransportador.Confirmada,
                            DataCargaContratada = DateTime.Now
                        };

                        if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                        {
                            cargaJanelaCarregamentoTransportador.Terceiro = cargaJanelaCarregamento.Carga.Terceiro;
                            cargaJanelaCarregamentoTransportador.HorarioLiberacao = DateTime.Now;
                        }
                        else
                        {
                            cargaJanelaCarregamentoTransportador.Transportador = cargaJanelaCarregamento.Carga.Empresa;
                            cargaJanelaCarregamentoTransportador.HorarioLiberacao = servicoCargaJanelaCarregamentoTransportador.ObterHorarioLiberacao(cargaJanelaCarregamento.Carga.Empresa);
                        }

                        DefinirDataDisponibilizacaoTransportadores(cargaJanelaCarregamento);

                        enviarEmailCargaDisponibilizada = true;
                        salvarHistoricoConfirmacaoCarga = true;
                    }
                    else
                    {
                        enviarEmailCargaDisponibilizada = cargaJanelaCarregamentoTransportador.Situacao.PermitirEnviarEmailCargaDisponibilizada();

                        if ((cargaJanelaCarregamento.CentroCarregamento?.AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente ?? false) && cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga.HasValue)
                            cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgAceite;
                        else
                        {
                            salvarHistoricoConfirmacaoCarga = cargaJanelaCarregamentoTransportador.Situacao != SituacaoCargaJanelaCarregamentoTransportador.Confirmada;
                            cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga = null;
                            cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Confirmada;
                        }

                        if (!cargaJanelaCarregamentoTransportador.DataCargaContratada.HasValue)
                            cargaJanelaCarregamentoTransportador.DataCargaContratada = DateTime.Now;
                    }

                    if (cargaJanelaCarregamentoTransportador.Codigo > 0)
                        repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    else
                        repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);

                    if (enviarEmailCargaDisponibilizada)
                        servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);

                    if (salvarHistoricoConfirmacaoCarga)
                    {
                        if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                            servicoCargaJanelaCarregamentoTransportadorTerceiro.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, mensagem: "Carga confirmada para o transportador terceiro");
                        else
                            servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, mensagem: "Carga confirmada para o transportador");
                    }

                    if (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite)
                        cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.AgAceiteTransportador;
                    else if (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Confirmada)
                    {
                        cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.ProntaParaCarregamento;
                        gerarFluxoGestaoPatio = true;
                    }
                }

                if (gerarFluxoGestaoPatio)
                    new GestaoPatio.FluxoGestaoPatio(_unitOfWork).Adicionar(cargaJanelaCarregamento.Carga, tipoServicoMultisoftware, cargaJanelaCarregamento);

                if (situacaoAnteriorCargaJanelaCarregamento != cargaJanelaCarregamento.Situacao || !cargaJanelaCarregamento.DataSituacaoAtual.HasValue)
                    cargaJanelaCarregamento.DataSituacaoAtual = DateTime.Now;

                if ((cargaJanelaCarregamento.Carga?.CargaAgrupada ?? false) && configuracaoEmbarcador.GerarFluxoPatioPorCargaAgrupada)
                {
                    Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Hubs.JanelaCarregamento();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargaJanelaCarregamentosAgrupada = repositorioCargaJanelaCarregamento.BuscarPorJanelaAgrupamento(cargaJanelaCarregamento.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamentoAgrupada in cargaJanelaCarregamentosAgrupada)
                    {
                        AtualizarSituacao(janelaCarregamentoAgrupada, tipoServicoMultisoftware);
                        servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(janelaCarregamentoAgrupada);
                    }
                }
            }
            else
            {
                cargaJanelaCarregamento.Situacao = cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador.Situacao;
                cargaJanelaCarregamento.DataSituacaoAtual = cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador.DataSituacaoAtual;
            }

            cargaJanelaCarregamento.DataProximaSituacao = new PrazoSituacaoCarga(_unitOfWork, configuracaoEmbarcador).ObterDataProximaSituacao(cargaJanelaCarregamento);

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        public void DefinirCargaPorPreCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);

            preCarga.Carga = carga;

            repositorioPreCarga.Atualizar(preCarga);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(preCarga.Codigo);

            if (cargaJanelaCarregamento == null)
                return;

            if (cargaJanelaCarregamento.Carga != null)
            {
                if ((cargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Cancelada) && (cargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Anulada))
                    return;

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> cargasJanelaCarregamentoGuarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita in cargasJanelaCarregamentoGuarita)
                    repositorioCargaJanelaCarregamentoGuarita.Deletar(cargaJanelaCarregamentoGuarita);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.Buscar(cargaJanelaCarregamento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadores in cargaJanelaCarregamentoTransportador)
                    repositorioCargaJanelaCarregamentoTransportador.Deletar(cargaJanelaCarregamentoTransportadores);

                cargaJanelaCarregamento.Carga = carga;

                if (carga.DataCarregamentoCarga.HasValue)
                {
                    cargaJanelaCarregamento.DataCarregamentoProgramada = carga.DataCarregamentoCarga.Value;

                    if (cargaJanelaCarregamento.CarregamentoReservado && cargaJanelaCarregamento.Excedente)
                        cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.DataCarregamentoProgramada;
                }

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            }
            else
            {
                cargaJanelaCarregamento.Carga = carga;

                if (carga.DataCarregamentoCarga.HasValue)
                {
                    cargaJanelaCarregamento.DataCarregamentoProgramada = carga.DataCarregamentoCarga.Value;

                    if (cargaJanelaCarregamento.CarregamentoReservado && cargaJanelaCarregamento.Excedente)
                        cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.DataCarregamentoProgramada;
                }

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            }
        }

        public async Task DefinirCargaPorPreCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);

            preCarga.Carga = carga;

            await repositorioPreCarga.AtualizarAsync(preCarga);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(preCarga.Codigo);

            if (cargaJanelaCarregamento == null)
                return;

            if (cargaJanelaCarregamento.Carga != null)
            {
                if ((cargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Cancelada) && (cargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Anulada))
                    return;

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> cargasJanelaCarregamentoGuarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita in cargasJanelaCarregamentoGuarita)
                    repositorioCargaJanelaCarregamentoGuarita.Deletar(cargaJanelaCarregamentoGuarita);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.Buscar(cargaJanelaCarregamento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadores in cargaJanelaCarregamentoTransportador)
                    repositorioCargaJanelaCarregamentoTransportador.Deletar(cargaJanelaCarregamentoTransportadores);

                cargaJanelaCarregamento.Carga = carga;

                if (carga.DataCarregamentoCarga.HasValue)
                {
                    cargaJanelaCarregamento.DataCarregamentoProgramada = carga.DataCarregamentoCarga.Value;

                    if (cargaJanelaCarregamento.CarregamentoReservado && cargaJanelaCarregamento.Excedente)
                        cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.DataCarregamentoProgramada;
                }

                await repositorioCargaJanelaCarregamento.AtualizarAsync(cargaJanelaCarregamento);
            }
            else
            {
                cargaJanelaCarregamento.Carga = carga;

                if (carga.DataCarregamentoCarga.HasValue)
                {
                    cargaJanelaCarregamento.DataCarregamentoProgramada = carga.DataCarregamentoCarga.Value;

                    if (cargaJanelaCarregamento.CarregamentoReservado && cargaJanelaCarregamento.Excedente)
                        cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.DataCarregamentoProgramada;
                }

                await repositorioCargaJanelaCarregamento.AtualizarAsync(cargaJanelaCarregamento);
            }
        }

        public void DefinirDataDisponibilizacaoTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.DataDisponibilizacaoTransportadores.HasValue)
                return;

            cargaJanelaCarregamento.DataDisponibilizacaoTransportadores = DateTime.Now;

            new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork).Atualizar(cargaJanelaCarregamento);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento DesagendarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                throw new ServicoException($"Não foi possível encontrar o registro.");

            Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (Carga.Carga.IsCargaBloqueada(carga, _unitOfWork))
                throw new ServicoException($"A {carga.DescricaoEntidade} está bloqueada e não permite alteração.");

            if (carga.CargaDePreCargaEmFechamento)
                throw new ServicoException($"A carga desta pré carga já está sendo fechada, é necessário aguardar o fechamento para depois realizar o cancelamento da mesma.");

            if (carga.CargaDePreCargaFechada)
            {
                if (!carga.CargaDePreCarga)
                    throw new ServicoException($"Não é possível desagendar uma carga gerada a partir de uma pré carga, é necessário cancelar a mesma.");
            }

            if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                throw new ServicoException($"A situação da carga {carga.CodigoCargaEmbarcador} não permite que ela seja desagendada, é necessário cancelar a mesma.");

            Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(_unitOfWork);

            if (servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueSituacaoNaoPermiteDesagendar(carga))
                throw new ServicoException($"A situação da ordem de embarque não permite que ela seja desagendada.");

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
            {
                Carga = carga,
                LiberarPedidosParaMontagemCarga = true,
                MotivoCancelamento = "Viagem atualizada ao desagendar a carga.",
                TipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador
            };

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoEmbarcador, _unitOfWork);
            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, _unitOfWork, _unitOfWork.StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, SituacaoCarregamento.EmMontagem);

            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                throw new ServicoException($"A carga {carga.CodigoCargaEmbarcador} não pode ser desagendada. Motivo: {cargaCancelamento.MensagemRejeicaoCancelamento}.");

            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgConfirmacao)
            {
                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;

                new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork).Atualizar(cargaCancelamento);
            }

            return cargaCancelamento;
        }

        public bool IsPermitirAdicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            //se precisa da nota antes do calculo do frete ou não precisa de veículo não faz sentido ir para a janela de carregamento;
            bool permitirAdicionarJanelaCarregamento = (
                (
                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ||
                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor) ||
                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) ||
                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (configuracaoJanelaCarregamento?.GerarJanelaDeCarregamento ?? false))
                ) &&
                !carga.NaoExigeVeiculoParaEmissao &&
                (!carga.ExigeNotaFiscalParaCalcularFrete || (carga.TipoOperacao?.HabilitarGestaoPatio ?? false) || configuracaoEmbarcador.ExigirNotaFiscalParaCalcularFreteCarga) &&
                (carga.CargaPreCarga == null)
            );

            if (!permitirAdicionarJanelaCarregamento)
                return false;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Cliente> expedidores = (carga.CargaAgrupamento == null) ? repositorioCargaPedido.BuscarExpedidoresPorCarga(carga.Codigo) : repositorioCargaPedido.BuscarExpedidoresPorCargaOrigem(carga.Codigo);
            bool expedidorFilial = expedidores.Any(o => o.CPF_CNPJ_SemFormato == carga.Filial.CNPJ);

            if (expedidores.Count > 0 && carga.FilialOrigem == null && !configuracaoEmbarcador.NaoExibirInfosAdicionaisGridPatio && !configuracaoJanelaCarregamento.GerarFluxoPatioCargaComExpedidor && !expedidorFilial)
                return false;

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = ObterCentroCarregamento(carga);

            if (centroCarregamento?.LiberarJanelaCarregamentoSomenteComAgendamentoRealizadoClienteAgendadoSemNota ?? false)
            {
                if (!ValidarInsercaoJanelaCarregamentoPorPedidosAgendados(carga))
                    return false;
            }

            if (carga.Redespacho != null)
            {
                if (configuracaoJanelaCarregamento.BloquearGeracaoJanelaParaCargaRedespacho && !(centroCarregamento?.PermitirGeracaoJanelaParaCargaRedespacho ?? false))
                    return false;
            }

            return true;
        }

        public void RemoverPorCargaEmAgrupamento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarParaRemover(carga.Codigo);

            if (listaCargaJanelaCarregamento.Count <= 0)
                return;

            CargaJanelaCarregamentoPrioridade servicoCargaJanelaCarregamentoPrioridade = new CargaJanelaCarregamentoPrioridade(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in listaCargaJanelaCarregamento)
            {
                int prioridadeAnteriorAgrupamento = cargaJanelaCarregamento.Prioridade;

                servicoCargaJanelaCarregamentoPrioridade.RemoverPrioridade(cargaJanelaCarregamento);

                cargaJanelaCarregamento.CentroCarregamento = null;
                cargaJanelaCarregamento.Prioridade = prioridadeAnteriorAgrupamento;

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            cargaNova.DataCarregamentoCarga = cargaAtual.DataCarregamentoCarga;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarTodasPorCarga(cargaAtual.Codigo);

            if (cargasJanelaCarregamento.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamento)
            {
                cargaJanelaCarregamento.Carga = cargaNova;

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
            }
        }

        public bool ValidarInsercaoJanelaCarregamentoPorPedidosAgendados(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo).Select(o => o.Pedido).ToList();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            if (configuracaoGeral?.PermitirAgendamentoPedidosSemCarga ?? false)
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    if ((pedido?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.ExigeAgendamento ?? false) && !(pedido?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.AgendamentoExigeNotaFiscal ?? false))
                        if (!(pedido.SituacaoAgendamentoEntregaPedido == SituacaoAgendamentoEntregaPedido.Agendado && pedido.DataAgendamento.HasValue))
                            return false;

            return true;
        }

        public dynamic ObterChecklist(int codigoJanela, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklist repChecklist = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklist(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist checklist = repChecklist.BuscarPorCargaJanelaCarregamento(codigoJanela);

            if (checklist == null)
                return null;

            if (checklist.UltimaCarga == null || checklist.PenultimaCarga == null || checklist.AntepenultimaCarga == null)
                throw new ServicoException("As cargas da checklist não foram carregadas corretamente.");

            bool exigirConfirmacaoTracao = checklist.CargaJanelaCarregamento?.Carga?.TipoOperacao?.ExigePlacaTracao ?? false;

            var reboques1 = (from obj in checklist.UltimaCarga.Reboques
                             select new
                             {
                                 Codigo = obj?.Codigo ?? 0,
                                 Descricao = exigirConfirmacaoTracao ? obj.Descricao : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(obj),
                             }).ToList();

            var reboques2 = (from obj in checklist.PenultimaCarga.Reboques
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 Descricao = exigirConfirmacaoTracao ? obj.Descricao : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(obj),
                             }).ToList();

            var reboques3 = (from obj in checklist.AntepenultimaCarga.Reboques
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 Descricao = exigirConfirmacaoTracao ? obj.Descricao : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(obj),
                             }).ToList();

            if (reboques1?.Count == 0 || reboques2?.Count == 0 || reboques3?.Count == 0)
                throw new ServicoException("Os reboques da checklist não foram carregados corretamente.");

            return new
            {
                checklist.Codigo,
                UltimaCarga = new
                {
                    checklist.UltimaCarga.Codigo,
                    DataChecklist = checklist.UltimaCarga?.DataChecklist != null ? checklist.UltimaCarga.DataChecklist.ToString("dd/MM/yyyy") : "",
                    Veiculo = new
                    {
                        Codigo = checklist.UltimaCarga.Veiculo?.Codigo ?? 0,
                        Descricao = exigirConfirmacaoTracao ? checklist.UltimaCarga.Veiculo?.Descricao ?? "" : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(checklist.UltimaCarga.Veiculo),
                    },
                    Reboque = reboques1[0],
                    SegundoReboque = reboques1[1],
                    TerceiroReboque = reboques1[2],
                    GrupoProduto = new
                    {
                        Codigo = checklist.UltimaCarga.GrupoProduto?.Codigo ?? 0,
                        Descricao = checklist.UltimaCarga.GrupoProduto?.Descricao ?? "",
                    },
                    checklist.UltimaCarga.RegimeLimpeza,
                    Anexos = checklist.UltimaCarga.Anexos != null ? from obj in checklist.UltimaCarga.Anexos
                                                                    select new
                                                                    {
                                                                        obj.Codigo,
                                                                        obj.Descricao,
                                                                        obj.NomeArquivo,
                                                                    } : null,
                },
                PenultimaCarga = new
                {
                    checklist.PenultimaCarga.Codigo,
                    DataChecklist = checklist.PenultimaCarga?.DataChecklist != null ? checklist.PenultimaCarga.DataChecklist.ToString("dd/MM/yyyy") : "",
                    Veiculo = new
                    {
                        Codigo = checklist.PenultimaCarga.Veiculo?.Codigo ?? 0,
                        Descricao = exigirConfirmacaoTracao ? checklist.PenultimaCarga.Veiculo?.Descricao ?? "" : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(checklist.PenultimaCarga.Veiculo),
                    },
                    Reboque = reboques2[0],
                    SegundoReboque = reboques2[1],
                    TerceiroReboque = reboques2[2],
                    GrupoProduto = new
                    {
                        Codigo = checklist.PenultimaCarga.GrupoProduto?.Codigo ?? 0,
                        Descricao = checklist.PenultimaCarga.GrupoProduto?.Descricao ?? "",
                    },
                    checklist.PenultimaCarga.RegimeLimpeza,
                    Anexos = checklist.PenultimaCarga.Anexos != null ? from obj in checklist.PenultimaCarga.Anexos
                                                                       select new
                                                                       {
                                                                           obj.Codigo,
                                                                           obj.Descricao,
                                                                           obj.NomeArquivo,
                                                                       } : null,
                },
                AntepenultimaCarga = new
                {
                    checklist.AntepenultimaCarga.Codigo,
                    DataChecklist = checklist.AntepenultimaCarga?.DataChecklist != null ? checklist.AntepenultimaCarga.DataChecklist.ToString("dd/MM/yyyy") : "",
                    Veiculo = new
                    {
                        Codigo = checklist.AntepenultimaCarga.Veiculo?.Codigo ?? 0,
                        Descricao = exigirConfirmacaoTracao ? checklist.AntepenultimaCarga.Veiculo?.Descricao ?? "" : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(checklist.AntepenultimaCarga.Veiculo),
                    },
                    Reboque = reboques3[0],
                    SegundoReboque = reboques3[1],
                    TerceiroReboque = reboques3[2],
                    GrupoProduto = new
                    {
                        Codigo = checklist.AntepenultimaCarga.GrupoProduto?.Codigo ?? 0,
                        Descricao = checklist.AntepenultimaCarga.GrupoProduto?.Descricao ?? "",
                    },
                    checklist.AntepenultimaCarga.RegimeLimpeza,
                    Anexos = checklist.AntepenultimaCarga.Anexos != null ? from obj in checklist.AntepenultimaCarga.Anexos
                                                                           select new
                                                                           {
                                                                               obj.Codigo,
                                                                               obj.Descricao,
                                                                               obj.NomeArquivo,
                                                                           } : null,
                },
            };
        }

        #endregion Métodos Públicos
    }
}