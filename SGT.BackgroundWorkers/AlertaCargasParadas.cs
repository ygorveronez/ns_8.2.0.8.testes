using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 600000)]

    public class AlertaCargasParadas : LongRunningProcessBase<AlertaCargasParadas>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarAlertaCargasParadas(unitOfWork);
        }

        private void ProcessarAlertaCargasParadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (!configuracaoTMS.HabilitarAlertaCargasParadas || configuracaoTMS.TempoMinutosAlertaCargasParadas <= 0 || string.IsNullOrWhiteSpace(configuracaoTMS.EmailsAlertaCargasParadas))
                return;

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            //Processando documentação
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.NumerosCargas> cargasParadasProcessamendoDocumento = repCarga.BuscarCargasParadasConfirmacaoDocumentos(configuracaoTMS.TempoMinutosAlertaCargasParadas, DateTime.Now);
            if (cargasParadasProcessamendoDocumento != null && cargasParadasProcessamendoDocumento.Count > 0)
            {
                string numeroCargas = string.Join(", ", cargasParadasProcessamendoDocumento.Select(c => c.NumeroCarga).ToList());
                string tituloEmail = "Cargas aguardando Processamento dos Documentos. (" + _clienteMultisoftware.RazaoSocial + ")";
                string descricaoAlerta = "Existem cargas aguardando o processamento dos documentos a mais de " + configuracaoTMS.TempoMinutosAlertaCargasParadas + " minutos.<br/><br/>";
                descricaoAlerta += "Cargas: " + numeroCargas;
                if (descricaoAlerta.Length > 2000)
                    descricaoAlerta = descricaoAlerta.Substring(0, 2000);

                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(configuracaoTMS.EmailsAlertaCargasParadas))
                    emails.AddRange(configuracaoTMS.EmailsAlertaCargasParadas.Split(';').ToList());

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    if (!Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, tituloEmail, descricaoAlerta, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp))
                        Servicos.Log.TratarErro(mensagemErro, "AlertaCargasParadas");
                }

                Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic alertaCarcaParadaProcessamendoDocumento = new Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic();
                alertaCarcaParadaProcessamendoDocumento.ValorAlerta = cargasParadasProcessamendoDocumento.Count;
                alertaCarcaParadaProcessamendoDocumento.Cliente = _clienteMultisoftware.RazaoSocial;
                alertaCarcaParadaProcessamendoDocumento.TipoServico = _clienteMultisoftware.ClienteConfiguracao.TipoServicoMultisoftware;
                alertaCarcaParadaProcessamendoDocumento.DataAtual = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                alertaCarcaParadaProcessamendoDocumento.DescricaoAlerta = "Existem cargas aguardando o processamento dos documentos a mais de " + configuracaoTMS.TempoMinutosAlertaCargasParadas + " minutos.";
                alertaCarcaParadaProcessamendoDocumento.CodigoTipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogElastic.CargasAguardandoProcessamentoDocumentos;

                Servicos.Log.TratarErro(alertaCarcaParadaProcessamendoDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Info);
            }

            //Calculando Frete
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.NumerosCargas> cargasParadasCalculandoFrete = repCarga.BuscarCargasParadasCalculandoFrete(configuracaoTMS.TempoMinutosAlertaCargasParadas, DateTime.Now);
            if (cargasParadasCalculandoFrete != null && cargasParadasCalculandoFrete.Count > 0)
            {
                string numeroCargas = string.Join(", ", cargasParadasCalculandoFrete.Select(c => c.NumeroCarga).ToList());
                string tituloEmail = "Cargas aguardando Calculando Frete. (" + _clienteMultisoftware.RazaoSocial + ")";
                string descricaoAlerta = "Existem cargas aguardando o calculo do frete a mais de " + configuracaoTMS.TempoMinutosAlertaCargasParadas + " minutos.<br/><br/>";
                descricaoAlerta += "Cargas: " + numeroCargas;
                if (descricaoAlerta.Length > 2000)
                    descricaoAlerta = descricaoAlerta.Substring(0, 2000);

                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(configuracaoTMS.EmailsAlertaCargasParadas))
                    emails.AddRange(configuracaoTMS.EmailsAlertaCargasParadas.Split(';').ToList());

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    if (!Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, tituloEmail, descricaoAlerta, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp))
                        Servicos.Log.TratarErro(mensagemErro, "AlertaCargasParadas");
                }

                Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic alertaCarcaParadaCalculandoFrete = new Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic();
                alertaCarcaParadaCalculandoFrete.ValorAlerta = cargasParadasCalculandoFrete.Count;
                alertaCarcaParadaCalculandoFrete.Cliente = _clienteMultisoftware.RazaoSocial;
                alertaCarcaParadaCalculandoFrete.TipoServico = _clienteMultisoftware.ClienteConfiguracao.TipoServicoMultisoftware;
                alertaCarcaParadaCalculandoFrete.DataAtual = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                alertaCarcaParadaCalculandoFrete.DescricaoAlerta = "Existem cargas aguardando o calculo do frete a mais de " + configuracaoTMS.TempoMinutosAlertaCargasParadas + " minutos.";
                alertaCarcaParadaCalculandoFrete.CodigoTipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogElastic.CargasParadasCalculandoFrete;

                Servicos.Log.TratarErro(alertaCarcaParadaCalculandoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Info);
            }

            //Emissao de Documento
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.NumerosCargas> cargasParadasEmitindoDocumento = repCarga.BuscarCargasParadasEmitindoDocumentos(configuracaoTMS.TempoMinutosAlertaCargasParadas, DateTime.Now);
            if (cargasParadasEmitindoDocumento != null && cargasParadasEmitindoDocumento.Count > 0)
            {
                string numeroCargas = string.Join(", ", cargasParadasEmitindoDocumento.Select(c => c.NumeroCarga).ToList());
                string tituloEmail = "Cargas aguardando Emissão do Documento. (" + _clienteMultisoftware.RazaoSocial + ")";
                string descricaoAlerta = "Existem cargas aguardando a emissão do documento a mais de " + configuracaoTMS.TempoMinutosAlertaCargasParadas + " minutos.<br/><br/>";
                descricaoAlerta += "Cargas: " + numeroCargas;
                if (descricaoAlerta.Length > 2000)
                    descricaoAlerta = descricaoAlerta.Substring(0, 2000);

                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(configuracaoTMS.EmailsAlertaCargasParadas))
                    emails.AddRange(configuracaoTMS.EmailsAlertaCargasParadas.Split(';').ToList());

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    if (!Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, tituloEmail, descricaoAlerta, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp))
                        Servicos.Log.TratarErro(mensagemErro, "AlertaCargasParadas");
                }

                Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic alertaCarcaParadasEmitindoDocumentos = new Dominio.ObjetosDeValor.Embarcador.Logs.LogElastic();
                alertaCarcaParadasEmitindoDocumentos.ValorAlerta = cargasParadasEmitindoDocumento.Count;
                alertaCarcaParadasEmitindoDocumentos.Cliente = _clienteMultisoftware.RazaoSocial;
                alertaCarcaParadasEmitindoDocumentos.TipoServico = _clienteMultisoftware.ClienteConfiguracao.TipoServicoMultisoftware;
                alertaCarcaParadasEmitindoDocumentos.DataAtual = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                alertaCarcaParadasEmitindoDocumentos.DescricaoAlerta = "Existem cargas aguardando a emissão do documento a mais de " + configuracaoTMS.TempoMinutosAlertaCargasParadas + " minutos.";
                alertaCarcaParadasEmitindoDocumentos.CodigoTipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogElastic.CargasParadasCalculandoFrete;

                Servicos.Log.TratarErro(alertaCarcaParadasEmitindoDocumentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogSistema.Info);

            }
        }

    

    }
}