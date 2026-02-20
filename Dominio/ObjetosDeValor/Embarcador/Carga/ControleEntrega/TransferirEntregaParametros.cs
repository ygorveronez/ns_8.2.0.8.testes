using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class TransferirEntregaParametros
    {
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> CargaEntregasAntigas { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> CargaEntregasNovas { get; set; }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel> CargaEntregasAssinaturaResponsavel { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList> CargaEntregasCheckList { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> CargaEntregasFoto { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal> CargaEntregasFotoNotaFiscal { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal> CargaEntregasGuiaTransporteAnimal { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao> ControleNotaDevolucao { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> CargaEntregasNFeDevolucao { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> OcorrenciasColetaEntregas { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado> CargaEntregaNotaFiscalChamado { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> CargaEntregaProdutoChamado { get; set; }

        public List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento> CargaEventos { get; set; }
        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> PermanenciasClientes { get; set; }
        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente> MonitoramentoHistoricoStatusViagemPermanenciaCliente { get; set; }
        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> PermanenciasSubareas { get; set; }
        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea> MonitoramentoHistoricoStatusViagemPermanenciaSubArea { get; set; }
        public List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> AlertasMonitor { get; set; }
        public List<Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento> PerdasSinalMonitoramento { get; set; }
        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> CargaOcorrencias { get; set; }
        public List<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp> IntegracaoSuperApp { get; set; }

        //gestao dados coleta
        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta> gestaoDadosColetas { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe> GestaoDadosColetaDadosNFe { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte> GestaoDadosColetaDadosTransporte { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao> GestaoDadosColetaIntegracao { get; set; }

        //Notificacoes App
        public List<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp> MonitoramentoNotificacoesApp { get; set; }

        //Chamados
        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> Chamados { get; set; }
        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> ChamadosAnalise { get; set; }
        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia> ChamadosOcorrencias { get; set; }
        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo> ChamadosAnexos { get; set; }
        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoData> ChamadosDatas { get; set; }
        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento> ChamadosInformacoesFechamento { get; set; }
        public List<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento> ChamadosNiveisAtendimento { get; set; }

        //Notas transferencia devolução pallet
        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet> GestaoDevolucaoNFeTransferenciaPallet { get; set; }

    }
}
