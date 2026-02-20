using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Logistica
{
    public sealed class AutomatizacaoNaoComparecimento
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public AutomatizacaoNaoComparecimento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AdicionarMensagemAlerta(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimento)
        {
            Carga.MensagemAlertaCarga servicoMensagemAlerta = new Carga.MensagemAlertaCarga(_unitOfWork);
            string mensagem = ObterMotivoNaoComparecimento(automatizacaoNaoComparecimento.Gatilho);

            servicoMensagemAlerta.Adicionar(cargaJanelaCarregamento.Carga, automatizacaoNaoComparecimento.Gatilho.ObterTipoMensagemAlerta(), automatizacaoNaoComparecimento.BloquearCarga, mensagem);
        }

        private void DefinirComoNaoComparecido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimento)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            TipoNaoComparecimento tipoNaoComparecido = (automatizacaoNaoComparecimento.Gatilho == GatilhoAutomatizacaoNaoComparecimento.CargaSemVeiculoInformado) ? TipoNaoComparecimento.NaoCompareceuComFalha : TipoNaoComparecimento.NaoCompareceu;

            cargaJanelaCarregamento.Carga.NaoComparecido = tipoNaoComparecido;
            cargaJanelaCarregamento.NaoComparecido = tipoNaoComparecido;

            if (automatizacaoNaoComparecimento.RetornarCargaParaExcedente)
                cargaJanelaCarregamento.Excedente = automatizacaoNaoComparecimento.RetornarCargaParaExcedente;

            repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);

            AdicionarMensagemAlerta(cargaJanelaCarregamento, automatizacaoNaoComparecimento);
            EnviarEmailTransportador(cargaJanelaCarregamento, automatizacaoNaoComparecimento);
        }

        private void EnviarEmailTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimento)
        {
            if (!automatizacaoNaoComparecimento.EnviarEmailTransportador)
                return;

            StringBuilder mensagem = new StringBuilder()
                .AppendLine($"Olá ({cargaJanelaCarregamento.Carga.Empresa.CNPJ_Formatado}) {cargaJanelaCarregamento.Carga.Empresa.Descricao},")
                .AppendLine()
                .AppendLine($"A carga {cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador} foi alterada para no-show.")
                .AppendLine($"Motivo: {ObterMotivoNaoComparecimento(automatizacaoNaoComparecimento.Gatilho)}.");

            Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
            {
                AssuntoEmail = $"Carga alterada para no-show",
                Empresa = cargaJanelaCarregamento.Carga.Empresa,
                Mensagem = mensagem.ToString(),
                NotificarSomenteEmailPrincipal = true
            };

            servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
        }

        private List<int> ObterCodigosJanelasCarregamentoParaDefinirComoNaoComparecido(Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimento)
        {
            DateTime dataTolerancia = DateTime.Now.AddHours(-automatizacaoNaoComparecimento.HorasTolerancia);
            DateTime dataCriacaoCargaInicial = automatizacaoNaoComparecimento.DataCadastro.AddHours(-automatizacaoNaoComparecimento.HorasTolerancia);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            if (automatizacaoNaoComparecimento.Gatilho == GatilhoAutomatizacaoNaoComparecimento.CargaNaoAgendada)
                return repositorioCargaJanelaCarregamento.BuscarCodigosParaMarcarComoNaoComparecimentoPorCargaNaoAgendada(automatizacaoNaoComparecimento.CentroCarregamento.Codigo, dataTolerancia, dataCriacaoCargaInicial);

            if (automatizacaoNaoComparecimento.Gatilho == GatilhoAutomatizacaoNaoComparecimento.CargaSemVeiculoInformado)
                return repositorioCargaJanelaCarregamento.BuscarCodigosParaMarcarComoNaoComparecimentoPorVeiculoNaoInformado(automatizacaoNaoComparecimento.CentroCarregamento.Codigo, dataTolerancia, dataCriacaoCargaInicial);

            if (automatizacaoNaoComparecimento.Gatilho == GatilhoAutomatizacaoNaoComparecimento.VeiculoSemRegistroChegada)
                return repositorioCargaJanelaCarregamento.BuscarCodigosParaMarcarComoNaoComparecimentoPorVeiculoSemRegistroChegada(automatizacaoNaoComparecimento.CentroCarregamento.Codigo, dataTolerancia, dataCriacaoCargaInicial);

            return new List<int>();
        }

        private string ObterMotivoNaoComparecimento(GatilhoAutomatizacaoNaoComparecimento gatilho)
        {
            if (gatilho == GatilhoAutomatizacaoNaoComparecimento.CargaNaoAgendada)
                return "Não foi agendado o horário de carregamento para esta carga";

            if (gatilho == GatilhoAutomatizacaoNaoComparecimento.CargaSemVeiculoInformado)
                return "Não foi informado o veículo que será utilizado para esta carga ";

            return "O veículo informado para a carga está sem registro de chegada";
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void VerificarNaoComparecimentos()
        {
            Repositorio.Embarcador.Logistica.AutomatizacaoNaoComparecimento repositorioAutomatizacaoNaoComparecimento = new Repositorio.Embarcador.Logistica.AutomatizacaoNaoComparecimento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> automatizacoesNaoComparecimento = repositorioAutomatizacaoNaoComparecimento.BuscarTodasConfiguradas();

            if (automatizacoesNaoComparecimento.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = automatizacoesNaoComparecimento.Select(o => o.CentroCarregamento).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento in centrosCarregamento)
            {
                List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> automatizacoesNaoComparecimentoPorCentroCarregamento = automatizacoesNaoComparecimento
                    .Where(o => o.CentroCarregamento.Codigo == centroCarregamento.Codigo)
                    .OrderBy(o => o.HorasTolerancia)
                    .ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimento in automatizacoesNaoComparecimentoPorCentroCarregamento)
                {
                    List<int> codigosJanelasCarregamentoParaDefinirComoNaoComparecido = ObterCodigosJanelasCarregamentoParaDefinirComoNaoComparecido(automatizacaoNaoComparecimento);

                    foreach (int codigoJanelaCarregamento in codigosJanelasCarregamentoParaDefinirComoNaoComparecido)
                    {
                        try
                        {
                            _unitOfWork.Start();

                            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);

                            DefinirComoNaoComparecido(cargaJanelaCarregamento, automatizacaoNaoComparecimento);

                            _unitOfWork.CommitChanges();
                        }
                        catch (Exception excecao)
                        {
                            _unitOfWork.Rollback();
                            Log.TratarErro(excecao);
                        }
                    }
                }
            }
        }

        #endregion Métodos Públicos
    }
}
