namespace SGT.WebAdmin
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(this WebApplication app)
        {
            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller}/{action=Index}"
            );

            app.MapControllerRoute(
                name: "Default",
                pattern: "{controller=MasterLayout}/{action=Index}"
            );

            app.MapControllerRoute(
                name: "FeedbackRastreioEntrega",
                pattern: "rastreio-entrega-antigo/feedback",
                defaults: new { controller = "RastreioEntrega", action = "Feedback" }
            );

            app.MapControllerRoute(
                name: "RastreioEntrega",
                pattern: "rastreio-entrega-antigo/{token}",
                defaults: new { controller = "RastreioEntrega", action = "Rastreamento", token = "" }
            );

            app.MapControllerRoute(
                name: "FeedbackNovoRastreioEntrega",
                pattern: "rastreio-entrega/feedback",
                defaults: new { controller = "NovoRastreioEntrega", action = "Feedback" }
            );

            app.MapControllerRoute(
                name: "NovoRastreioEntrega",
                pattern: "rastreio-entrega/{token}",
                defaults: new { controller = "NovoRastreioEntrega", action = "Rastreamento", token = "" }
            );

            app.MapControllerRoute(
                name: "FeedbackRastreamento",
                pattern: "sua-entrega/feedback",
                defaults: new { controller = "SuaEntrega", action = "Feedback" }
            );

            app.MapControllerRoute(
                name: "DownloadAnexoRastreamento",
                pattern: "sua-entrega/DownloadAnexo",
                defaults: new { controller = "SuaEntrega", action = "DownloadAnexo" }
            );

            app.MapControllerRoute(
                name: "SalvarObservacaoAvaliacao",
                pattern: "sua-entrega/SalvarObservacaoAvaliacao",
                defaults: new { controller = "SuaEntrega", action = "SalvarObservacaoAvaliacao" }
            );

            app.MapControllerRoute(
                name: "Rastreamento",
                pattern: "sua-entrega/{token}",
                defaults: new { controller = "SuaEntrega", action = "Rastreamento", token = "" }
            );

            app.MapControllerRoute(
                name: "RastreamentoVisualizacaoMapa",
                pattern: "visualizacao-monitoramento/{token}",
                defaults: new { controller = "RastreamentoMonitoramento", action = "RastreamentoVisualizacaoMapa", token = "" }
            );

            app.MapControllerRoute(
                name: "ConsultarPedido",
                pattern: "consultar-pedido",
                defaults: new { controller = "GetPedido", action = "ValidarToken" }
            );

            app.MapControllerRoute(
                name: "PoliticaPrivacidade",
                pattern: "politica-privacidade/{id}",
                defaults: new { controller = "Ajuda", action = "PoliticaPrivacidade", id = "" }
            );

            app.MapControllerRoute(
                name: "LoginMotorista",
                pattern: "LoginMotorista",
                defaults: new { controller = "LoginMotorista", action = "IndexMotorista" }
            );

            app.MapControllerRoute(
                name: "LoginInterno",
                pattern: "LoginInterno",
                defaults: new { controller = "LoginInterno", action = "Index" }
            );

            app.MapControllerRoute(
                name: "AprovacaoOcorrencia",
                pattern: "aprovacao-ocorrencia/{token}",
                defaults: new { controller = "AutorizacaoOcorrenciaAprovador", action = "Aprovacao", token = "" }
            );

            app.MapControllerRoute(
                name: "AprovacaoCarga",
                pattern: "aprovacao-carga/{token}",
                defaults: new { controller = "AutorizacaoCargaAprovador", action = "Aprovacao", token = "" }
            );

            app.MapControllerRoute(
                name: "AprovacaoTabelaFrete",
                pattern: "aprovacao-tabelafrete/{token}",
                defaults: new { controller = "AutorizacaoTabelaFreteAprovador", action = "Aprovacao", token = "" }
            );

            app.MapControllerRoute(
                name: "AprovacaoCarregamento",
                pattern: "aprovacao-carregamento/{token}",
                defaults: new { controller = "AutorizacaoCarregamentoAprovador", action = "Aprovacao", token = "" }
            );

            app.MapControllerRoute(
                name: "DownloadDocumentos",
                pattern: "download-documentos-carga/{token}",
                defaults: new { controller = "DownloadDocumentos", action = "Validacao", token = "" }
            );

            app.MapControllerRoute(
                name: "DownloadLoteXMLMDFeDownloadDocumentos",
                pattern: "downloads-documentos-carga/DownloadLoteXMLMDFe",
                defaults: new { controller = "DownloadDocumentos", action = "DownloadLoteXMLMDFe" }
            );

            app.MapControllerRoute(
                name: "DownloadLoteDocumentosMDFeDownloadDocumentos",
                pattern: "downloads-documentos-carga/DownloadLoteDocumentosMDFe",
                defaults: new { controller = "DownloadDocumentos", action = "DownloadLoteDocumentosMDFe" }
            );

            app.MapControllerRoute(
                name: "DownloadLoteDAMDFEDownloadDocumentos",
                pattern: "downloads-documentos-carga/DownloadLoteDAMDFE",
                defaults: new { controller = "DownloadDocumentos", action = "DownloadLoteDAMDFE" }
            );

            app.MapControllerRoute(
                name: "DownloadLoteDACTEDownloadDocumentos",
                pattern: "downloads-documentos-carga/DownloadLoteDACTE",
                defaults: new { controller = "DownloadDocumentos", action = "DownloadLoteDACTE" }
            );

            app.MapControllerRoute(
                name: "DownloadLoteDocumentosCTeDownloadDocumentos",
                pattern: "downloads-documentos-carga/DownloadLoteDocumentosCTe",
                defaults: new { controller = "DownloadDocumentos", action = "DownloadLoteDocumentosCTe" }
            );

            app.MapControllerRoute(
                name: "DownloadLoteXMLCTeDownloadDocumentos",
                pattern: "downloads-documentos-carga/DownloadLoteXMLCTe",
                defaults: new { controller = "DownloadDocumentos", action = "DownloadLoteXMLCTe" }
            );

            app.MapControllerRoute(
                name: "AcessoViaTokenMultiClifor",
                pattern: "acesso-via-token-multiclifor",
                defaults: new { controller = "AcessoViaTokenMultiClifor", action = "Acessar" }
                );

            app.MapControllerRoute(
                name: "PortalCliente",
                pattern: "PortalCliente/Titulo/{token}",
                defaults: new { controller = "PortalCliente", action = "Index", token = "" }
            );

            app.MapHub<Servicos.Embarcador.Hubs.AjusteTabela>("/hubs/ajusteTabela");
            app.MapHub<Servicos.Embarcador.Hubs.Avaria>("/hubs/avaria");
            app.MapHub<Servicos.Embarcador.Hubs.BaixaTituloReceber>("/hubs/baixaTituloReceber");
            app.MapHub<Servicos.Embarcador.Hubs.CancelamentoProvisao>("/hubs/cancelamentoProvisao");
            app.MapHub<Servicos.Embarcador.Hubs.Carga>("/hubs/carga");
            app.MapHub<Servicos.Embarcador.Hubs.Chamado>("/hubs/chamado");
            app.MapHub<Servicos.Embarcador.Hubs.Chat>("/hubs/chat");
            app.MapHub<Servicos.Embarcador.Hubs.ComprovanteEntrega>("/hubs/comprovanteEntrega");
            app.MapHub<Servicos.Embarcador.Hubs.ControleSaldo>("/hubs/controleSaldo");
            app.MapHub<Servicos.Embarcador.Hubs.Fatura>("/hubs/fatura");
            app.MapHub<Servicos.Embarcador.Hubs.Fechamento>("/hubs/fechamento");
            app.MapHub<Servicos.Embarcador.Hubs.FilaCarregamento>("/hubs/filaCarregamento");
            app.MapHub<Servicos.Embarcador.Hubs.FilaCarregamentoReversa>("/hubs/filaCarregamentoReversa");
            app.MapHub<Servicos.Embarcador.Hubs.FluxoColetaEntrega>("/hubs/fluxoColetaEntrega");
            app.MapHub<Servicos.Embarcador.Hubs.FluxoPatio>("/hubs/fluxoPatio");
            app.MapHub<Servicos.Embarcador.Hubs.IntegracaoAvon>("/hubs/integracaoAvon");
            app.MapHub<Servicos.Embarcador.Hubs.IntegracaoMercadoLivre>("/hubs/integracaoMercadoLivre");
            app.MapHub<Servicos.Embarcador.Hubs.JanelaCarregamento>("/hubs/janelaCarregamento");
            app.MapHub<Servicos.Embarcador.Hubs.Manobra>("/hubs/manobra");
            app.MapHub<Servicos.Embarcador.Hubs.MDFeAquaviario>("/hubs/mdfeAquaviario");
            app.MapHub<Servicos.Embarcador.Hubs.MDFeManual>("/hubs/mdfeManual");
            app.MapHub<Servicos.Embarcador.Hubs.MontagemCarga>("/hubs/montagemCarga");
            app.MapHub<Servicos.Embarcador.Hubs.MontagemFeeder>("/hubs/montagemFeeder");
            app.MapHub<Servicos.Embarcador.Hubs.NFSManual>("/hubs/nfsManual");
            app.MapHub<Servicos.Embarcador.Hubs.Notificacao>("/hubs/notificacao");
            app.MapHub<Servicos.Embarcador.Hubs.Ocorrencia>("/hubs/ocorrencia");
            app.MapHub<Servicos.Embarcador.Hubs.Pagamento>("/hubs/pagamento");
            app.MapHub<Servicos.Embarcador.Hubs.Provisao>("/hubs/provisao");

            app.MapHub<Servicos.Embarcador.HubsMobile.NotificacaoMobile>("/hubsMobile/notificacaoMobile");

            //app.MapHub<Servicos.SignalR.Mobile>("/hubs/mobile");
            app.MapHub<Servicos.SignalR.Hubs.AcompanhamentoCarga>("/hubs/acompanhamentoCarga");
            //app.MapHub<Servicos.SignalR.Hubs.ChamadoChat>("/hubs/chamadoChat");
            app.MapHub<Servicos.SignalR.Hubs.ControleColetaEntrega>("/hubs/controleColetaEntrega");
            //app.MapHub<Servicos.SignalR.Hubs.EtapasCarga>("/hubs/etapasCarga");
            app.MapHub<Servicos.SignalR.Hubs.GestaoPatio>("/hubs/gestaoPatio");
            app.MapHub<Servicos.SignalR.Hubs.Monitoramento>("/hubs/monitoramento");
            app.MapHub<Servicos.SignalR.Hubs.GestaoDevolucao>("/hubs/gestaoDevolucao");
            app.MapHub<Servicos.SignalR.Hubs.Pedidos>("/hubs/pedidos");
        }
    }
}