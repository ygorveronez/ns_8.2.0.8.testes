using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public abstract class AbstractAlertaCarga
    {

        #region Atributos protegidos

        protected virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga TipoAlertaCarga { get; set; }
        protected virtual Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga ConfiguracaoAlertaCarga { get; set; }

        protected Repositorio.UnitOfWork unitOfWork;

        protected string stringConexao;

        #endregion

        #region Métodos púbicos

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga GetTipoAlerta()
        {
            return this.TipoAlertaCarga;
        }

        public int GetTempoEvento()
        {
            return this.ConfiguracaoAlertaCarga.TempoEvento;
        }

        public void Processar(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            // Execura a regra específica do evento 
            ProcessarEvento(alertas);

            TratarAlertaAutomaticamente(alertas);
        }


        #endregion

        #region Métodos abstratos

        public abstract void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas);

        #endregion

        #region Métodos protegidos
        protected void CriarAlerta(IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas, int codigoCarga, int cargaEntrega, int codigoChamado, string descricao)
        {
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(this.unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repchamado = new Repositorio.Embarcador.Chamados.Chamado(this.unitOfWork);
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null;

            if (codigoChamado > 0)
                chamado = repchamado.BuscarPorCodigo(codigoChamado);

            Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento alertaCarga = new Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento()
            {
                TipoAlerta = this.TipoAlertaCarga,
                Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto,
                DataCadastro = DateTime.Now,
                DataEvento = DateTime.Now,
                Carga = new Dominio.Entidades.Embarcador.Cargas.Carga { Codigo = codigoCarga },
                CargaEntrega = cargaEntrega > 0 ? new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega { Codigo = cargaEntrega } : null,
                AlertaDescricao = descricao.Length > 300 ? descricao.Substring(0, 300) : descricao,
                Chamado = chamado
            };

            repCargaEvento.Inserir(alertaCarga);

            // Chama o callback que roda depois da criação de um alerta
            ExecutarDepoisDeCriarAlerta(alertaCarga);

            // Adiciona na lista de alertas abertos para posteriores vericações de existência do alerta nesta mesma sessão
            alertas.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga
            {
                Codigo = alertaCarga.Codigo,
                CodigoCarga = alertaCarga.Carga?.Codigo ?? null,
                DataCadastro = alertaCarga.DataCadastro,
                Data = alertaCarga.DataEvento,
                DataFim = alertaCarga.DataEvento,
                TipoAlerta = alertaCarga.TipoAlerta,
                Status = alertaCarga.Status
            });

            if (this.ConfiguracaoAlertaCarga?.TempoLimiteTratativaAutomaticaTime.TotalSeconds > 0 && this.ConfiguracaoAlertaCarga?.TempoLimiteTratativaAutomaticaTime.TotalSeconds <= 60)
            {
                //se a tratativa do alerta for menor que 1 minuto, vamos trata-lo agora.
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);
                Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga repAlertaAcompanhamentoCarga = new Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento alertaCargaTratado = repCargaEvento.BuscarPorCodigo(alertaCarga.Codigo);
                if (alertaCargaTratado != null)
                {
                    alertaCargaTratado.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
                    alertaCargaTratado.Observacao = "Tratamento de alerta atomático após atingir tempo de permanência em aberto - Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    repCargaEvento.Atualizar(alertaCargaTratado);

                    Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga alertaAcompanhamentoCarga = repAlertaAcompanhamentoCarga.BuscarAlertaAbertoAlertaEventoCarga(alertaCarga.Codigo);
                    if (alertaAcompanhamentoCarga != null)
                    {
                        alertaAcompanhamentoCarga.AlertaTratado = true;
                        repAlertaAcompanhamentoCarga.Atualizar(alertaAcompanhamentoCarga);
                    }

                    servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, alertaCargaTratado);
                }
            }
        }

        protected void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento)
        {
            CriarAlertaAcompanhamentoCarga(cargaEvento);

            if (cargaEvento.Carga.Empresa == null)
            {
                Servicos.Log.GravarInfo($"CARGA {cargaEvento.Carga.CodigoCargaEmbarcador} não possui empresa vinculada para enviar email");
                return;
            }

            if (this.ConfiguracaoAlertaCarga.EnvioEmailTransportador || this.ConfiguracaoAlertaCarga.EnvioEmailCliente )
            {
                string tituloAlerta = cargaEvento.Descricao;
                string assunto = $@"[Alerta Cargas] {tituloAlerta}";
                string mensagem = $@"<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%"">
                            <tr>
                                <td style=""padding:20px; background-color:#EEE;""> 
                                    <table border=""0"" cellspacing=""0"" cellpadding=""0"" style=""width:500px"" align=""center"">
                                        <tr>
                                            <td>
                                                <div style=""border-bottom: 1px solid #EEE;padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 36px; font-weight: bold;"">Alerta {tituloAlerta}</div>
                                                <div style=""border-bottom: 1px solid #EEE;padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 14px;line-height: 150%; "">
                                                    <p style=""margin:0 0 20px 0"">Foi identificado um comportamento anormal com o veículo monitorado e um alerta foi registrado.</p>
                                                    <p style=""margin:0 0 20px 0; padding:10px; border: 1px solid #DDD; border-radius: 4px;line-height: 150%; "">
                                                        <label style=""display: inline-block; width: 100px; font-style: italic;"">Data:</label> {cargaEvento.DataEvento.ToString("dd/MM/yyyy HH:mm:ss")}<br/>
                                                        <label style=""display: inline-block; width: 100px; font-style: italic;"">Motorista(s):</label> {cargaEvento.Carga.NomeMotoristas}<br/>
                                                        <label style=""display: inline-block; width: 100px; font-style: italic;"">Transportador:</label> {cargaEvento.Carga.Empresa.CNPJ_Formatado} - {cargaEvento.Carga.Empresa.RazaoSocial}<br/>
                                                        <label style=""display: inline-block; width: 100px; font-style: italic;"">Alerta:</label> {cargaEvento.Codigo} - {cargaEvento.Descricao}<br/>
                                                        <label style=""display: inline-block; width: 100px; font-style: italic;"">Valor:</label> {cargaEvento.AlertaDescricao}<br/>
                                                    </p>
                                                    <p style=""margin: 0px; line-height: 150%; "">Será necessário registrar a tratativa no sistema.</p>
                                                </div>
                                                <div style=""padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 14px;line-height: 150%; "">
                                                    <strong>ALERTA CARGAS</strong><br/>
                                                    <span style=""font-style:italic"">Multisoftware</span>
                                                </div>
                                                <div style=""padding:20px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; font-size: 10px; color:#CCC"">E-mail enviado automaticamente.</div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                          </table>";

                if (this.ConfiguracaoAlertaCarga.EnvioEmailCliente)
                {
                    //TODO: 
                }

                if (this.ConfiguracaoAlertaCarga.EnvioEmailTransportador)
                {
                    EnviarEmailNotificacao(cargaEvento.Carga?.Empresa.Email, mensagem, assunto, stringConexao, unitOfWork);
                }
            }
         

        }


        protected AbstractAlertaCarga(Repositorio.UnitOfWork unitOfWork, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga TipoAlertaCarga)
        {
            this.unitOfWork = unitOfWork;
            this.TipoAlertaCarga = TipoAlertaCarga;
            this.stringConexao = stringConexao;
            this.CarregarConfiguracaoAlerta();
        }

        /**
         * Verifica se o evento está configurado e ativo
         */
        protected bool EstaAtivo()
        {
            return this.ConfiguracaoAlertaCarga?.Ativo ?? false;
        }

        protected bool PossuiTratativaAutomatica()
        {
            return this.ConfiguracaoAlertaCarga?.TempoLimiteTratativaAutomaticaTime.TotalSeconds > 0 ? true : false;
        }

        protected Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga ObterUltimoAlertaDaLista(IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas, int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Todos)
        {
            if (alertas != null)
            {
                int total = alertas.Count;
                for (int i = total - 1; i >= 0; i--)
                {
                    if (
                        alertas[i].TipoAlerta == this.TipoAlertaCarga
                        &&
                        (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Todos || alertas[i].Status == status)
                        &&
                        (alertas[i].CodigoCarga != null && alertas[i].CodigoCarga == codigoCarga)

                    )
                    {
                        return alertas[i];
                    }
                }
            }
            return null;
        }

        protected Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga ObterUltimoAlertaAbertoDaListaPorTipo(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto)
        {
            if (alertas != null)
            {
                int total = alertas.Count;
                for (int i = total - 1; i >= 0; i--)
                {
                    if (
                        alertas[i].TipoAlerta == this.TipoAlertaCarga
                        &&
                        (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Todos || alertas[i].Status == status)
                    )
                    {
                        return alertas[i];
                    }
                }
            }
            return null;
        }

        protected bool ExisteAlertaAbertoOuFechadoHaPouco(int codigocarga, IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            if (codigocarga > 0)
            {
                // Busca o último alerta para o veículo ou para a carga
                Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga alerta = ObterUltimoAlertaDaLista(alertas, codigocarga);
                if (alerta != null)
                {

                    // Não deve gerar um novo evento caso o alerta ainda esteja em aberto
                    if (alerta.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto) return true;

                    // Deve respeitar o tempo mínimo configurado entre os alertas
                    int diferencaEmMinutos = (int)(DateTime.Now - alerta.Data).TotalMinutes;
                    if (diferencaEmMinutos <= this.ConfiguracaoAlertaCarga.Tempo) return true;

                }
            }
            return false;
        }

        protected void TratarAlertaAutomaticamente(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            if (this.PossuiTratativaAutomatica())
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga alerta = ObterUltimoAlertaAbertoDaListaPorTipo(alertas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);

                if (alerta != null)
                {
                    TimeSpan diferencaEmMinutos = (DateTime.Now - alerta.Data);
                    if (diferencaEmMinutos >= this.ConfiguracaoAlertaCarga.TempoLimiteTratativaAutomaticaTime)
                    {
                        Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(this.unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarPorCodigo(alerta.Codigo);

                        cargaEvento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
                        cargaEvento.Observacao = "Tratamento de alerta atomático após atingir tempo de permanência em aberto - Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                        repCargaEvento.Atualizar(cargaEvento);
                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                    }
                }
            }
        }

        protected bool ExisteAlertaCarga(int codigocarga, List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            if (codigocarga > 0)
            {
                // Busca o último alerta para o veículo ou para a carga
                Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga alerta = ObterUltimoAlertaDaLista(alertas, codigocarga);
                if (alerta != null)
                    return true;
            }
            return false;
        }

        #endregion

        #region Metodos Privados

        private void CarregarConfiguracaoAlerta()
        {
            Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repconfigAlertaCarga = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(this.unitOfWork);
            this.ConfiguracaoAlertaCarga = repconfigAlertaCarga.BuscarAtivo(this.TipoAlertaCarga);
        }

        private void EnviarEmailNotificacao(string email, string mensagem, string assunto, string stringConexao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            //if (!enviroment.Equals("PRODUCAO")) email = "fernando@multisoftware.com.br";
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                    serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email.Trim(), "", "", assunto, mensagem, string.Empty, null, "", true, string.Empty, 0, unidadeTrabalho, 0, false);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private void CriarAlertaAcompanhamentoCarga(Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento)
        {
            if (this.ConfiguracaoAlertaCarga.GerarAlertaAcompanhamentoCarga)
            {
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);
                Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga repAlertaAcompanhamentoCarga = new Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga alertaAcompanhamentoCarga = new Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga
                {
                    Carga = cargaEvento.Carga,
                    AlertaMonitor = null,
                    CargaEvento = cargaEvento,
                    AlertaTratado = false,
                    DataCadastro = DateTime.Now,
                    DataEvento = cargaEvento.DataEvento,
                    CargaEntrega = cargaEvento.CargaEntrega
                };

                repAlertaAcompanhamentoCarga.Inserir(alertaAcompanhamentoCarga);
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(cargaEvento.Carga);
            }
        }
        #endregion

    }
}
