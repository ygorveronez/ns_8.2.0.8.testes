using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Diagnostics;

namespace Servicos.Embarcador.Configuracoes
{
    public class Layout
    {
        public static void MontarLayout(dynamic ViewBag, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, Repositorio.UnitOfWork unitOfWork)
        {
            ViewBag.LogoClienteLight = clienteURLAcesso.Cliente.LogoLight;
            ViewBag.SiteCliente = clienteURLAcesso.Cliente.Site;
            ViewBag.AmbienteKMM = clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !clienteURLAcesso.Cliente.Cabotagem;

            bool alteraCorPrincipalPortal = false;

            if (clienteURLAcesso.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe &&
                clienteURLAcesso.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                ViewBag.HeightLogoCliente = "height: " + (clienteURLAcesso.Cliente.HeightLogo > 0 ? clienteURLAcesso.Cliente.HeightLogo : 90) + "px !important;";
                ViewBag.LogoCliente = clienteURLAcesso.Cliente.Logo;
            }

            ViewBag.PossuiLogoCliente = !string.IsNullOrWhiteSpace(ViewBag.LogoCliente);
            ViewBag.AmbienteMultiNFe = clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin;
            ViewBag.MensagemBoasVindasPersonalizada = "";
            ViewBag.LayoutBoasVindasPersonalizado = "";

            if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                ViewBag.Titulo = "MultiTMS";
                ViewBag.IconeLogoAzul = "img/logos/user.png";
                ViewBag.LogoLight = "img/logos/multitms_semendosso_postiva.png";
                ViewBag.LogoDark = "img/logos/multitms_semendosso_negativa.png";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(clienteURLAcesso.Favicon) ? "img/logos/favicon-site-nstech.svg" : clienteURLAcesso.Favicon;
                ViewBag.BoasVindas = "Seja Bem Vindo ao MultiTMS";
                ViewBag.IntroducaoProduto = Localization.Resources.Login.Login.SomosEspecialistasTMSEndEndParaEmbarcadoresDesejamosAproveiteExperiencia;
                ViewBag.LayoutLoginPersonalizado = string.IsNullOrWhiteSpace(clienteURLAcesso.LayoutLoginPersonalizado) ? "" : clienteURLAcesso.LayoutLoginPersonalizado;
                ViewBag.MensagemBoasVindasPersonalizada = string.IsNullOrWhiteSpace(clienteURLAcesso.MensagemBoasVindasPersonalizada) ? "" : clienteURLAcesso.MensagemBoasVindasPersonalizada;
                ViewBag.LogoTerceiroLight = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoClienteTerceiroLight) ? "" : clienteURLAcesso.LogoClienteTerceiroLight;
                ViewBag.LogoTerceiroDark = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoClienteTerceiroDark) ? "" : clienteURLAcesso.LogoClienteTerceiroDark;
            }
            else if (clienteURLAcesso.Cliente.Cabotagem)
            {
                ViewBag.Titulo = "MultiTMS";
                ViewBag.IconeLogo = "img/logos/logo-tms-icon.svg";
                ViewBag.IconeLogoAzul = "img/logos/logo-tms-icon.svg";
                ViewBag.LogoLight = "img/logos/logo-tms-text-light.svg";
                ViewBag.LogoDark = "img/logos/logo-tms-text-dark.svg";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(clienteURLAcesso.Favicon) ? "img/favicon/logo-tms-icon.ico?v=5" : clienteURLAcesso.Favicon;
                ViewBag.BoasVindas = Localization.Resources.Login.Login.SejaBemVindoMultiTMS;
                ViewBag.IntroducaoProduto = Localization.Resources.Login.Login.MelhorTMSParaTransportadorasDesejamosAproveiteExperiencia;
            }
            else if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                ViewBag.Titulo = "KMM";
                ViewBag.IconeLogo = "img/logos/kmm_endossada_positiva.svg";
                ViewBag.IconeLogoAzul = "img/logos/kmm_endossada_positiva.svg";
                ViewBag.LogoLight = "img/logos/kmm_endossada_positiva.svg";
                ViewBag.LogoDark = "img/logos/kmm_endossada_negativa.svg";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(clienteURLAcesso.Favicon) ? "img/favicon/kmm-icon.ico?v=5" : clienteURLAcesso.Favicon;
                ViewBag.BoasVindas = Localization.Resources.Login.Login.SejaBemVindoKMM;
                ViewBag.IntroducaoProduto = Localization.Resources.Login.Login.MelhorTMSParaTransportadorasDesejamosAproveiteExperiencia;
            }
            
            else if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros ||
                     clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
            {
                ViewBag.Titulo = "MultiCTe";
                ViewBag.IconeLogo = "img/logos/logo-cte-icon.svg";
                ViewBag.IconeLogoAzul = "img/logos/logo-tms-icon.svg";
                ViewBag.LogoLight = "img/logos/logo-cte-text-light.svg";
                ViewBag.LogoDark = "img/logos/logo-cte-text-dark.svg";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(clienteURLAcesso.Favicon) ? "img/favicon/logo-cte-icon.ico?v=3" : clienteURLAcesso.Favicon;
                ViewBag.BoasVindas = Localization.Resources.Login.Login.BemVindoPortalMultiTransportador;
                ViewBag.IntroducaoProduto = Localization.Resources.Login.Login.SomosEspecialistasSistemasGestaoLogisticaDeseamosAproveiteExperiencia;
                ViewBag.LayoutLoginPersonalizado = string.IsNullOrWhiteSpace(clienteURLAcesso.LayoutLoginPersonalizadoTransportador) ? "" : clienteURLAcesso.LayoutLoginPersonalizadoTransportador;
                ViewBag.MensagemBoasVindasPersonalizada = string.IsNullOrWhiteSpace(clienteURLAcesso.MensagemBoasVindasPersonalizadaTransportador) ? "" : clienteURLAcesso.MensagemBoasVindasPersonalizadaTransportador;
                ViewBag.LogoTerceiroLight = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoClienteTerceiroLight) ? "" : clienteURLAcesso.LogoClienteTerceiroLight;
                ViewBag.LogoTerceiroDark = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoClienteTerceiroDark) ? "" : clienteURLAcesso.LogoClienteTerceiroDark;
            }
            else if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                ViewBag.Titulo = "ComNFe";
                ViewBag.IconeLogo = "img/logos/logo-nfe-icon.svg";
                ViewBag.IconeLogoAzul = "img/logos/logo-tms-icon.svg";
                ViewBag.LogoLight = "img/logos/logo-nfe-text-light.svg";
                ViewBag.LogoDark = "img/logos/logo-nfe-text-dark.svg";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(clienteURLAcesso.Favicon) ? "img/favicon/logo-nfe-icon.ico?v=3" : clienteURLAcesso.Favicon;
                ViewBag.BoasVindas = Localization.Resources.Login.Login.SejaBemVindoComNFe;
                ViewBag.IntroducaoProduto = "";
            }
            else if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                ViewBag.Titulo = "ComNFe - Admin";
                ViewBag.IconeLogo = "img/logos/logo-nfe-icon.svg";
                ViewBag.IconeLogoAzul = "img/logos/logo-tms-icon.svg";
                ViewBag.LogoLight = "img/logos/logo-nfe-text-light.svg";
                ViewBag.LogoDark = "img/logos/logo-nfe-text-dark.svg";
                ViewBag.Favicon = string.IsNullOrWhiteSpace(clienteURLAcesso.Favicon) ? "img/favicon/logo-nfe-icon.ico?v=3" : clienteURLAcesso.Favicon;
                ViewBag.BoasVindas = Localization.Resources.Login.Login.SejaBemVindoAreaAdministrativaComNFe;
                ViewBag.IntroducaoProduto = "";
            }
            else if (clienteURLAcesso.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                ViewBag.IconeLogo = "img/logos/logo-cte-icon.svg";
                ViewBag.IconeLogoAzul = "img/logos/logo-tms-icon.svg";
                ViewBag.LogoLight = "img/logos/logo-cte-text-light.svg";
                ViewBag.LogoDark = "img/logos/logo-cte-text-dark.svg";
                ViewBag.Titulo = "MultiCTe - Fornecedor";
                ViewBag.Logo = string.IsNullOrWhiteSpace(clienteURLAcesso.LogoLogin) ? "img/logo.png" : clienteURLAcesso.LogoLogin;
                ViewBag.Favicon = string.IsNullOrWhiteSpace(clienteURLAcesso.FaviconLogin) ? "img/favicon/favicon.ico?v=3" : clienteURLAcesso.FaviconLogin;
                ViewBag.Layout = string.IsNullOrWhiteSpace(clienteURLAcesso.LayoutLogin) ? "" : clienteURLAcesso.LayoutLogin;
                ViewBag.CorFundoUsuario = string.IsNullOrWhiteSpace(clienteURLAcesso.CorFundoUsuarioLogin) ? "" : clienteURLAcesso.CorFundoUsuarioLogin;
                ViewBag.LogoPersonalizada = !string.IsNullOrWhiteSpace(ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LogoPersonalizadaFornecedor) ? ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LogoPersonalizadaFornecedor : "cologist-logo.png";
                ViewBag.ExibirConteudoColog = !(ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().OcultarConteudoColog);
                ViewBag.BoasVindas = ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaLayoutPersonalizado ? Localization.Resources.Login.Login.BemVindoPortalAgendamento : Localization.Resources.Login.Login.BemVindoPortalMultiCliente;
                ViewBag.IntroducaoProduto = Localization.Resources.Login.Login.SomosEspecialistasSistemasGestaoLogisticaDeseamosAproveiteExperiencia;
                if (ViewBag.LogoPersonalizada == "logvett-logo.png")
                {
                    alteraCorPrincipalPortal = true;
                }
                
            }

            ViewBag.AlteraCorPrincipalPortal = alteraCorPrincipalPortal;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            ViewBag.Versao = $"{fvi.ProductMajorPart}.{fvi.ProductMinorPart}";
            ViewBag.AnoCorrente = DateTime.Now.Year.ToString();
            ViewBag.DadosEmpresaPai = "";

			if (ViewBag.AmbienteKMM)
			{
				ViewBag.LinkSaibaMais = "https://kmm.com.br/";
				ViewBag.LinkFacebook = "https://www.facebook.com/kmm.logistica";
				ViewBag.LinkLinkedIn = "https://www.linkedin.com/company/kmmlogistica/posts/?feedView=all";
				ViewBag.LinkInstagram = "https://www.instagram.com/kmmbynstech/";
				ViewBag.Copyright = "KMM";
                ViewBag.MobileWebAppCapable = "";
            }
            else
			{
				ViewBag.LinkSaibaMais = "https://multisoftware.com.br/";
				ViewBag.LinkFacebook = "https://www.facebook.com/multitms";
				ViewBag.LinkLinkedIn = "https://br.linkedin.com/company/multisoftware";
				ViewBag.LinkInstagram = "https://www.instagram.com/multitmsbynstech/";
				ViewBag.Copyright = "Multisoftware";
                ViewBag.MobileWebAppCapable = "apple-";
            }
		}
    }
}
