using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCarga : ServicoBase
    {

        #region Construtores

        public IntegracaoCarga(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public IntegracaoCarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken = default) : base(unitOfWork, clienteMultisoftware, cancellationToken) { }

        public IntegracaoCarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken) { }

        public IntegracaoCarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware, cancelationToken)
        {
        }

        #endregion Construtores

        #region Métodos Públicos

        public void InformarIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipo(carga.Codigo, tipoIntegracao);

            if (cargaIntegracao == null)
            {
                cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracao();
                cargaIntegracao.Carga = carga;
                cargaIntegracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);

                repCargaIntegracao.Inserir(cargaIntegracao);
            }
        }

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            IntegracaoCTe serIntegracaoCte = new IntegracaoCTe(unitOfWork);
            IntegracaoEnvioProgramado serIntegracaoEnvioProgramado = new IntegracaoEnvioProgramado(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            bool integracaoFilialEmissora = carga.EmiteMDFeFilialEmissora && carga.UtilizarCTesAnterioresComoCTeFilialEmissora;
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repositorioIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repositorioIntegracaoEMP.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> cargaIntegracoes = repCargaIntegracao.BuscarPorCarga(carga.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga = servicoCarga.ObterTipoIntegracoesPorTipoOperacaoETipoCarga(carga, unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao in cargaIntegracoes)
            {
                //transbordo não gera integração de ct-e pois só existe MDF-e
                if (carga.CargaTransbordo && !cargaIntegracao.TipoIntegracao.IntegrarCargaTransbordo)
                    continue;

                //cada tipo de integração deve adicionar os documentos necessários nas filas
                switch (cargaIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAEscrituracaoCTe:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ravex:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTePagamentoLoggi:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Yandeh:
                        serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(carga, cargaIntegracao.TipoIntegracao.Tipo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior:
                        if (!cargaIntegracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                            serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(carga, cargaIntegracao.TipoIntegracao.Tipo);
                        else
                            serIntegracaoCte.AdcionarApenasOPrimeirCTesParaEnvioViaIntegracaoIndividual(carga, cargaIntegracao.TipoIntegracao.Tipo, unitOfWork);//quando possui integração com avon, o Avior espera receber apenas o numero da minuta e o valor total.
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                        AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        if (carga?.TipoOperacao?.ConfiguracaoIntegracao?.IntegrarDocumentos ?? false)
                            serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(carga, cargaIntegracao.TipoIntegracao.Tipo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:
                        if (carga.TipoOperacao == null || !carga.TipoOperacao.IntegrarPreSMAngelLira || carga.TipoOperacao.ReintegrarSMCargaAngelLira)
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        if ((integracaoIntercab?.IntegracaoDocumentacaoCarga ?? false) && carga.CargaRecebidaDeIntegracao != true)
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX:
                        if (!(carga.TipoOperacao?.IntegrarPreSMNOX ?? false))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech:
                        if (!carga.ExigeNotaFiscalParaCalcularFrete || !(carga.ExigeNotaFiscalParaCalcularFrete && cargaIntegracao.TipoIntegracao.GerarIntegracaoDadosTransporteCarga && !(carga.TipoOperacao?.ConfiguracaoIntegracao.NaoIntegrarEtapa1Opentech ?? false)))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logvett:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trafegus:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MundialRisk:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logiun:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CadastrosMulti:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MicDta:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Totvs:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PH:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GoldenService:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcadorCIOT:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DiariaMotoristaProprio:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MarfrigOrdemEmbarque:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverFourKites:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Infolog:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DTe:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPA:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Tecnorisk:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FrimesaFrete:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Loggi:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_API4:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LogRisk:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Atlas:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Froggr:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyEventos:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dexco:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ComproveiRota:
                        AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        if ((integracaoEMP?.AtivarIntegracaoCargaEMP ?? false))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:

                        if (integracaoEMP?.AtivarIntegracaoNFTPEMP ?? false)
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxCTE:
                        {
                            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                            if (!repositorioCargaCTe.ExistePorCargaEModeloDocumentoFiscal(carga.Codigo, Dominio.Enumeradores.TipoDocumento.CTe))
                                continue;

                            if (carga.TipoOperacao?.ConfiguracaoIntegracao?.PossuiTempoEnvioIntegracaoDocumentosCarga ?? false)
                                serIntegracaoEnvioProgramado.AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao);
                            else
                                AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxNFE:
                        {
                            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                            if (!repositorioCargaCTe.ExistePorCargaEModeloDocumentoFiscal(carga.Codigo, Dominio.Enumeradores.TipoDocumento.NFSe))
                                continue;

                            if (carga.TipoOperacao?.ConfiguracaoIntegracao?.PossuiTempoEnvioIntegracaoDocumentosCarga ?? false)
                                serIntegracaoEnvioProgramado.AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao);
                            else
                                AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxProvisao:
                        if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxProvisao))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour:
                        if (new Carrefour.IntegracaoCarrefour(unitOfWork).PossuiIntegracaoCarga())
                        {
                            if (!serIntegracaoEnvioProgramado.AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao))
                                AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Krona:
                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork).ExistePorCargaEResponsavelSeguro(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPA:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX:
                        AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, true, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                        if (!(carga.TipoOperacao?.ConfiguracaoTrizy?.NaoFinalizarPreTrip ?? false) && !(carga.Empresa?.NaoGerarIntegracaoSuperAppTrizy ?? false))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, false, true);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo:
                        if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, true, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador:
                        if (new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork).ExistePorCargaConfiguracaoIntegrarXMLCTe(carga.Codigo))
                            serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(carga, cargaIntegracao.TipoIntegracao.Tipo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverRelevanciaCustos:
                        if (new Servicos.Embarcador.Carga.Carga(unitOfWork).CenarioIntegracaoConfigurado(ref carga, unitOfWork, cargaIntegracao.TipoIntegracao.Tipo))
                            ProcessarIntegracaoRelevanciaCusto(carga, cargaIntegracao.TipoIntegracao, unitOfWork, true, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FrimesaValePedagio:
                        //if (repCargaValePedagio.ExistePorCarga(carga.Codigo))
                        AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, true, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                        GerarIntegracaoSomenteDasCargaCteComDocumento(unitOfWork, carga, cargaIntegracao.TipoIntegracao.Tipo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverInfrutifera:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverRecusa:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverOcorrencias:
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ValoresCTeLoggi:
                        if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ValoresCTeLoggi))
                            serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(carga, cargaIntegracao.TipoIntegracao.Tipo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST:
                        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                        Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);
                        if (contratoFrete != null)
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9:
                        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteV9 = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                        Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFreteV9 = repContratoFreteV9.BuscarPorCarga(carga.Codigo);
                        if (contratoFreteV9 != null)
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GrupoSC:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars:
                        if (!integracoesPorTipoOperacaoETipoCarga.Contains(cargaIntegracao.TipoIntegracao.Tipo))
                            break;

                        List<Dominio.Enumeradores.TipoDocumento> modelosDocumentos = new List<Dominio.Enumeradores.TipoDocumento>()
                        {
                            Dominio.Enumeradores.TipoDocumento.CTe,
                            Dominio.Enumeradores.TipoDocumento.NFSe
                        };

                        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentosEnvio = serIntegracaoCte.BuscarDocumentosPorModeloDocumentoFiscalECarga(modelosDocumentos, carga.Codigo, unitOfWork);

                        serIntegracaoEnvioProgramado.AdicionarCTesLoteParaIntegracao(documentosEnvio, cargaIntegracao.TipoIntegracao, carga);

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        GerarIntegracaoSomenteDasCargaCteComDocumento(unitOfWork, carga, cargaIntegracao.TipoIntegracao.Tipo, new List<string> { "CT-e", "NFS-e", "NFS", "ND" });
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vector:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTeAnterioresLoggi:
                        if (integracoesPorTipoOperacaoETipoCarga.Contains(cargaIntegracao.TipoIntegracao.Tipo))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        List<int> codigosCargaCte = repCargaCTe.BuscarCodigosPorCarga(carga.Codigo);
                        List<string> modelosPermitidos = new List<string>();

                        foreach (int codigoCargaCte in codigosCargaCte)
                        {
                            var cargaCte = repCargaCTe.BuscarPorCodigo(codigoCargaCte);
                            if (cargaCte?.CTe?.ModeloDocumentoFiscal != null && !cargaCte.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento)
                                modelosPermitidos.Add(cargaCte.CTe.ModeloDocumentoFiscal.Abreviacao);
                        }
                        if (modelosPermitidos.Count > 0)
                            GerarIntegracaoSomenteDasCargaCteComDocumento(unitOfWork, carga, cargaIntegracao.TipoIntegracao.Tipo, modelosPermitidos);

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TransSat:
                        if (carga?.TipoOperacao?.ConfiguracaoTipoOperacaoIntegracaoTransSat?.PossuiIntegracaoTransSat == true)
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.WeberChile:
                        serIntegracaoEnvioProgramado.AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux:
                        if (integracoesPorTipoOperacaoETipoCarga.Contains(cargaIntegracao.TipoIntegracao.Tipo))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SistemaTransben:
                        if (integracoesPorTipoOperacaoETipoCarga.Contains(cargaIntegracao.TipoIntegracao.Tipo))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb:
                        if (integracoesPorTipoOperacaoETipoCarga.Contains(cargaIntegracao.TipoIntegracao.Tipo))
                            AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, false, false, integracaoFilialEmissora);
                        break;
                    default:
                        break;
                }
            }
        }

        public void ProcessarIntegracaoRelevanciaCusto(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho, bool gerarNovaIntegracaoSeJaIntegrado, bool integracaoColeta)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoStage repositorioPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeTrabalho, tipoIntegracao.IntegracaoTransportador);


            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
                return;

            if (carga?.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false)//ver com victor pq esse if.. 
            {
                if (carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)//carga filho de pre-checkin
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);
                    Dominio.Entidades.Embarcador.Pedidos.Stage stagePorPedido = repositorioPedidoStage.BuscarRelevantesCustoPorPedido(cargaPedido.Pedido.Codigo);



                    if (repCargaIntegracao.ExisteIntegracaoParaEstaStage(stagePorPedido?.Codigo ?? 0))
                        return;

                    Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao();
                    cargaIntegracao.Carga = carga;
                    cargaIntegracao.DataIntegracao = DateTime.Now;
                    cargaIntegracao.NumeroTentativas = 0;
                    cargaIntegracao.ProblemaIntegracao = "";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    cargaIntegracao.TipoIntegracao = tipoIntegracao;
                    cargaIntegracao.IntegracaoColeta = integracaoColeta;
                    cargaIntegracao.RealizarIntegracaoCompleta = false;
                    cargaIntegracao.Stage = stagePorPedido;

                    repCargaIntegracao.Inserir(cargaIntegracao);

                    return;
                }
                else
                    AdicionarCargaParaIntegracao(carga, tipoIntegracao, unidadeTrabalho, gerarNovaIntegracaoSeJaIntegrado, integracaoColeta);

                return;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido item in cargasPedidos)
            {
                if (item.NotasFiscais.Count == 0)
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.Stage stage = repositorioPedidoStage.BuscarStagePorCargaPedido(item.Codigo);
                if (repCargaIntegracao.ExisteIntegracaoParaEstaStage(stage?.Codigo ?? 0))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao();
                cargaIntegracao.Carga = carga;
                cargaIntegracao.DataIntegracao = DateTime.Now;
                cargaIntegracao.NumeroTentativas = 0;
                cargaIntegracao.ProblemaIntegracao = "";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                cargaIntegracao.TipoIntegracao = tipoIntegracao;
                cargaIntegracao.IntegracaoColeta = integracaoColeta;
                cargaIntegracao.RealizarIntegracaoCompleta = false;
                cargaIntegracao.Stage = stage;

                repCargaIntegracao.Inserir(cargaIntegracao);
            }
        }

        public void IniciarIntegracoesComEDI(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int codigoCTe = 0)
        {
            IntegracaoEDI.AdicionarEDIParaIntegracao(carga, tipoServicoMultisoftware, unitOfWork, codigoCTe);
        }

        public async Task VerificarIntegracoesPendentesAsync(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURL)
        {
            Servicos.Embarcador.Integracao.IntegracaoCTe servicoIntegracaoCTe = new IntegracaoCTe(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado servicoIntegracaoEnvioProgramado = new Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoEDI servicoIntegracaoEDI = new Servicos.Embarcador.Integracao.IntegracaoEDI(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<int> codigosCargas = await servicoIntegracaoCTe.VerificarIntegracoesPendentesIndividuaisAsync(auditado, clienteURL);

            codigosCargas.AddRange(servicoIntegracaoCTe.VerificarIntegracoesPendentesLote());
            codigosCargas.AddRange(await servicoIntegracaoEDI.VerificarIntegracoesPendentesAsync());
            codigosCargas.AddRange(await VerificarCargaIntegracaoPendentesAsync(auditado, clienteURL));
            codigosCargas.AddRange(await VerificarIntegracoesCargaDadosTransportePendentesAsync(clienteURL, auditado));
            codigosCargas.AddRange(servicoIntegracaoEnvioProgramado.VerificarIntegracaoesCargaPendentes());

            codigosCargas = codigosCargas.Distinct().ToList();

            foreach (int codigoCarga in codigosCargas)
                await AtualizarSituacaoCargaIntegracaoAsync(codigoCarga);
        }

        public void VerificarIntegracoesFreteIntegracaoPendente(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteUrlAcesso)
        {
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao> cargaFreteIntegracaosPendentes = repositorioCargaFreteIntegracao.RegistrosPendentesDeIntegracao();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao in cargaFreteIntegracaosPendentes)
            {
                switch (cargaFreteIntegracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Unilever:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarCargaFrete(cargaFreteIntegracao, unitOfWork, auditado, tipoServicoMultisoftware, webServiceConsultaCTe);
                        break;
                    case TipoIntegracao.UnileverLinkNotas:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarLinkNotas(cargaFreteIntegracao, unitOfWork, auditado, tipoServicoMultisoftware, webServiceConsultaCTe);
                        break;
                    case TipoIntegracao.RejeicaoCte:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarPrechekinTransferencia(cargaFreteIntegracao);
                        break;
                    case TipoIntegracao.Raster:
                        Servicos.Embarcador.Integracao.Raster.IntegracaoRaster.IntegrarCargaFrete(cargaFreteIntegracao, unitOfWork, tipoServicoMultisoftware);
                        break;
                    case TipoIntegracao.Cebrace:
                        new Servicos.Embarcador.Integracao.Cebrace.IntegracaoCebrace(unitOfWork).IntegrarCargaFrete(cargaFreteIntegracao);
                        break;
                    default:
                        cargaFreteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaFreteIntegracao.ProblemaIntegracao = "Integração não implementada";
                        cargaFreteIntegracao.NumeroTentativas++;
                        cargaFreteIntegracao.DataIntegracao = DateTime.Now;
                        break;
                }

                repositorioCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);
            }
        }

        public void GerarIntegracaoSomenteDasCargaCteComDocumento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoIntegracao tipoIntegracao, List<string> modelosPermitidos = null)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);

            if (!repCargaCTe.EstaCargaPossuiCte(carga.Codigo))
                return;

            if (carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                return;

            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                //só deve enviar se todos os ctes (inclusive das filhos) ja estao confirmados ou recusaReprovada ou sem regra aprovacao
                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentos = repStageAgrupamento.BuscarPorCargaDt(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in agrupamentos)
                    if (agrupamento.CargaGerada == null)//nao tem carga, nao tem CargaCte da filho.
                        return;

                if (repCargaCTe.CargaPossuiCtesInviaveisParaIntegracao(carga.Codigo))
                    return;
            }

            List<int> codigosCargaCte = new List<int>();

            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
                codigosCargaCte = repCargaCTe.BuscarCodigosPorCargaPrechekinAceito(carga.Codigo);
            else
                codigosCargaCte = repCargaCTe.BuscarCodigosPorCarga(carga.Codigo);

            foreach (int codigoCargaCte in codigosCargaCte)
            {
                if (repCargaCTeIntegracao.ExisteIntegracaoParaCargaCte(codigoCargaCte))
                    continue;

                if (modelosPermitidos != null)
                {
                    if (!modelosPermitidos.Any(x => x == (repCargaCTe.BuscarPorCodigo(codigoCargaCte)?.CTe?.ModeloDocumentoFiscal?.Abreviacao ?? "")))
                        continue;
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao();
                cargaCTeIntegracao.CargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe() { Codigo = codigoCargaCte };
                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas = 0;
                cargaCTeIntegracao.ProblemaIntegracao = "";
                cargaCTeIntegracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);
                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repCargaCTeIntegracao.Inserir(cargaCTeIntegracao);
            }
        }

        public void AdicionarIntegracoesCargaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.ExigeNotaFiscalParaCalcularFrete ||
               (carga.SituacaoCarga != SituacaoCarga.CalculoFrete && carga.SituacaoCarga != SituacaoCarga.AgTransportador && carga.SituacaoCarga != SituacaoCarga.AgNFe)
            )
                return;


            new Servicos.Embarcador.Carga.Carga(unitOfWork).CriarIntegracaoesCargaFrete(ref carga, unitOfWork);

        }

        public async Task ReenviarIntegracoesCargaDadosTransportePosFreteAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> cargaIntegracoes = await repCargaIntegracao.BuscarPorCargaAsync(carga.Codigo);
            List<TipoIntegracao> integracoesPermitidas = new List<TipoIntegracao>() { TipoIntegracao.SAP_API4 };

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao in cargaIntegracoes)
            {
                if (!integracoesPermitidas.Contains(cargaIntegracao.TipoIntegracao.Tipo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = await repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracaoAsync(carga.Codigo, cargaIntegracao.TipoIntegracao.Tipo);

                if (cargaDadosTransporteIntegracao == null)
                    continue;

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                await repCargaDadosTransporteIntegracao.AtualizarAsync(cargaDadosTransporteIntegracao);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Públicos Estáticos

        public static Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao AdicionarCargaParaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho, bool gerarNovaIntegracaoSeJaIntegrado, bool integracaoColeta)
        {
            return AdicionarCargaParaIntegracao(carga, tipoIntegracao, unidadeTrabalho, gerarNovaIntegracaoSeJaIntegrado, integracaoColeta, false, false);
        }

        public static Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao AdicionarCargaParaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho, bool gerarNovaIntegracaoSeJaIntegrado, bool integracaoColeta, bool integracaoFilialEmissora, bool finalizarCargaAnterior = false, bool forcarNovaIntegracao = false)
        {
            if (carga.CargaRecebidaDeIntegracao)
                return null;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeTrabalho, tipoIntegracao.IntegracaoTransportador);

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao.Codigo, integracaoColeta);


            if (cargaCargaIntegracao != null)
            {
                if ((cargaCargaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado || !gerarNovaIntegracaoSeJaIntegrado) && !forcarNovaIntegracao)
                    return cargaCargaIntegracao;
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao();

            cargaIntegracao.Carga = carga;
            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas = 0;
            cargaIntegracao.ProblemaIntegracao = "";
            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            cargaIntegracao.TipoIntegracao = tipoIntegracao;
            cargaIntegracao.IntegracaoColeta = integracaoColeta;
            cargaIntegracao.RealizarIntegracaoCompleta = integracaoIntercab?.AtivarIntegracaoCargaAtualParaNovo ?? false;
            cargaIntegracao.Stage = null;
            cargaIntegracao.IntegracaoFilialEmissora = integracaoFilialEmissora;
            cargaIntegracao.FinalizarCargaAnterior = finalizarCargaAnterior;
            repCargaIntegracao.Inserir(cargaIntegracao);

            return cargaIntegracao;
        }

        public static Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao AdicionarCargaDadosTransporteIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho, bool gerarNovaIntegracaoSeJaIntegrado, bool integracaoColeta, bool gerarNovoRegistro = false, string protocoloAuxiliar = "")
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaCargaIntegracao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao.Codigo, integracaoColeta);


            if (cargaCargaIntegracao != null && !gerarNovoRegistro)
            {
                if (cargaCargaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado || !gerarNovaIntegracaoSeJaIntegrado)
                    return cargaCargaIntegracao;
            }

            if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab && carga.CargaRecebidaDeIntegracao == true)
                return null;

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao();

            cargaIntegracao.Carga = carga;
            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas = 0;
            cargaIntegracao.ProblemaIntegracao = "";
            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            cargaIntegracao.TipoIntegracao = tipoIntegracao;
            cargaIntegracao.IntegracaoColeta = integracaoColeta;
            cargaIntegracao.Protocolo = protocoloAuxiliar;

            repCargaIntegracao.Inserir(cargaIntegracao);

            return cargaIntegracao;
        }

        public static void AdicionarIntegracoesCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, bool etapaDocumentos)
        {
            // Flag etapaDocumentos criada para definir a etapa atual
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioConfiguracaoIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem repIntegracaoPortalCabotagem = new Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem(unidadeDeTrabalho);
            Repositorio.Embarcador.Transportadores.GrupoTransportadorIntegracao repGrupoTransportadorIntegracao = new Repositorio.Embarcador.Transportadores.GrupoTransportadorIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao repositorioConfiguracaoFilialIntegracao = new Repositorio.Embarcador.Integracao.ConfiguracaoFilialIntegracao(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga = servicoCarga.ObterTipoIntegracoesPorTipoOperacaoETipoCarga(carga, unidadeDeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoGrupoTransportador = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorFilialTipoOperacao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

            if (carga.Empresa != null)
            {
                integracoesPorTipoOperacaoGrupoTransportador = repGrupoTransportadorIntegracao.BuscarTiposIntegracaoPorTransportadorAsync(carga.Empresa.Codigo).GetAwaiter().GetResult();
            }

            if (carga.TipoOperacao != null)
            {
                integracoesPorFilialTipoOperacao = repositorioConfiguracaoFilialIntegracao.BuscarPorFilialETipoOperacaoAsync(carga.Filial.Codigo, carga.TipoOperacao.Codigo).GetAwaiter().GetResult();
            }

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (carga.CargaRecebidaDeIntegracao != true)
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab = repositorioConfiguracaoIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPortalCabotagem integracaoPortalCabotagem = repIntegracaoPortalCabotagem.BuscarPrimeiroRegistro();
                AdicionarIntegracaoIntercab(configuracaoIntegracaoIntercab, carga, unidadeDeTrabalho);
                AdicionarIntegracaoPortalCabotagem(integracaoPortalCabotagem, carga, unidadeDeTrabalho);
            }

            AdicionarIntegracaoANTT(carga, unidadeDeTrabalho);
            AdicionarIntegracaoAngelLira(carga, unidadeDeTrabalho);
            AdicionarIntegracaoCarrefour(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoGoldenService(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoBrasilRisk(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoBrasilRiskSML(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoMundialRisk(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoLogiun(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoTrizy(carga, configuracaoIntegracao, tipoServicoMultisoftware, unidadeDeTrabalho);
            AdicionarIntegracaoTrizyContrato(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoArcelorMittal(carga, tipoServicoMultisoftware, unidadeDeTrabalho);
            AdicionarIntegracaoDocumentoAnterior(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoMICDTA(carga, unidadeDeTrabalho);
            AdicionarIntegracaoAX(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoTotvs(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoDTe(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoPH(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoNOX(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoGPA(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoMultiEmbarcadorCIOT(carga, unidadeDeTrabalho);
            AdicionarIntegracaoDiariaPagamentoMotorista(carga, unidadeDeTrabalho);
            AdicionarIntegracaoPadrao(carga, unidadeDeTrabalho);
            AdicionarIntegracaoRaster(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoKrona(carga, configuracaoIntegracao, integracoesPorTipoOperacaoETipoCarga, tipoServicoMultisoftware, unidadeDeTrabalho);
            AdicionarIntegracaoInfolog(carga, configuracaoIntegracao, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoBuonny(carga, configuracaoIntegracao, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoSaintGobain(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoMagalogEscrituracao(carga, unidadeDeTrabalho);
            AdicionarIntegracaoA52(carga, unidadeDeTrabalho);
            AdicionarIntegracaoOpentech(carga, unidadeDeTrabalho);
            //AdicionarIntegracaoOpentechV10(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoMultiEmbarcador(carga, cargaPedidos, tipoServicoMultisoftware, unidadeDeTrabalho);
            AdicionarIntegracaoGadle(carga, configuracaoIntegracao, unidadeDeTrabalho);
            AdicionarIntegracaoMarfrigOrdemEmbarque(carga, configuracaoIntegracao, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoDPA(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoUnilever(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoUnileverDadoValePedagio(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoUnileverRelevanciaCusto(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoDigitalCom(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoTecnorisk(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoNeokohm(carga, unidadeDeTrabalho);
            AdicionarIntegracaoApisulLog(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoFrimesaFrete(carga, cargaPedidos, unidadeDeTrabalho);
            AdicionarIntegracaoFrimesaValePedagio(carga, cargaPedidos, unidadeDeTrabalho);
            AdicionarIntegracaoLoggi(carga, unidadeDeTrabalho);
            AdicionarIntegracaoCTePagamentoLoggi(carga, unidadeDeTrabalho);
            AdicionarIntegracaoSAP(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoBoticario(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoEMP(carga, unidadeDeTrabalho);
            AdicionarIntegracaoNFTP(carga, unidadeDeTrabalho);
            AdicionarIntegracaoUnileverDadosTransporte(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoSAPV9(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoSAPST(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoBrado(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoEShip(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoLogRisk(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            RemoverIntegracoesPorConfiguracaoEmissao(carga, unidadeDeTrabalho);
            AdicionarIntegracaoFrotaCarga(carga, unidadeDeTrabalho);
            AdicionarIntegracaoMoniloc(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoYandeh(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoComprovei(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoKMM(carga, unidadeDeTrabalho);
            AdicionarIntegracaoAtlas(carga, unidadeDeTrabalho);
            AdicionarIntegracaoCalisto(carga, unidadeDeTrabalho);
            AdicionarIntegracaoLogvett(carga, unidadeDeTrabalho);
            AdicionarIntegracaoPorGrupoMotivoChamado(carga, unidadeDeTrabalho);
            AdicionarIntegracaoEMPSIL(carga, unidadeDeTrabalho);
            AdicionarIntegracaoElectrolux(carga, unidadeDeTrabalho);
            AdicionarIntegracaoFroggr(carga, unidadeDeTrabalho);
            AdicionarIntegracaoValoresCTeLoggi(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoCTeAnterioresLoggi(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoArcelorMittalParaDadosTransporte(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoATSLog(carga, unidadeDeTrabalho);
            AdicionarIntegracaoTrizyEventos(carga, unidadeDeTrabalho);
            AdicionarIntegracaoCebrace(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoSAP_API4(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoDexco(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoMars(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoGrupoSC(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoFlora(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoObramax(carga, unidadeDeTrabalho);
            AdicionarIntegracaoAssai(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoVector(carga, integracoesPorTipoOperacaoGrupoTransportador, unidadeDeTrabalho);
            AdicionarIntegracaoSenior(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoObramaxCTEs(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoObramaxNFSe(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoObramaxProvisao(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoFS(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoGlobus(carga, unidadeDeTrabalho);
            AdicionarIntegracaoVedacit(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoTransSat(carga, unidadeDeTrabalho);
            AdicionarIntegracaoCassol(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoWeberChile(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoLactalis(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoComproveiRota(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoSistemaTransben(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoATSSmartWeb(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoMinerva(carga, integracoesPorTipoOperacaoETipoCarga, unidadeDeTrabalho);
            AdicionarIntegracaoYMS(carga, integracoesPorFilialTipoOperacao, unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoHUBOfertas = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho).BuscarPorTipo(TipoIntegracao.HUB);
            AdicionarIntegracaoSkyMark(carga, unidadeDeTrabalho);

            if (tipoIntegracaoHUBOfertas != null)
            {
                bool retorno = new HUB.IntegracaoHUBOfertas(unidadeDeTrabalho, tipoServicoMultisoftware).AdicionarIntegracaoHUB(carga, tipoIntegracaoHUBOfertas).GetAwaiter().GetResult();
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();
                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracoes = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

                if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    if (carga.TipoOperacao.TipoIntegracao != null &&
                        carga.TipoOperacao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                        carga.TipoOperacao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                        repCargaIntegracao.ContarPorCargaETipoIntegracao(carga.Codigo, carga.TipoOperacao.TipoIntegracao.Tipo) <= 0)
                        tiposIntegracoes.Add(carga.TipoOperacao.TipoIntegracao);

                    if (carga.TipoOperacao.ConfiguracaoIntegracao?.UtilizarTipoIntegracaoGrupoPessoas ?? false)
                    {
                        if (tomador != null && tomador.GrupoPessoas != null && tomador.GrupoPessoas.TipoIntegracao != null &&
                            tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                            tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                            repCargaIntegracao.ContarPorCargaETipoIntegracao(carga.Codigo, tomador.GrupoPessoas.TipoIntegracao.Tipo) <= 0)
                            tiposIntegracoes.Add(tomador.GrupoPessoas.TipoIntegracao);
                    }
                }
                else if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                    {
                        if (tomador.TipoIntegracao != null &&
                            tomador.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                            tomador.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                            repCargaIntegracao.ContarPorCargaETipoIntegracao(carga.Codigo, tomador.TipoIntegracao.Tipo) <= 0)
                            tiposIntegracoes.Add(tomador.TipoIntegracao);
                    }
                    else
                    {
                        if (tomador.GrupoPessoas.TipoIntegracao != null &&
                            tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                            tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                            repCargaIntegracao.ContarPorCargaETipoIntegracao(carga.Codigo, tomador.GrupoPessoas.TipoIntegracao.Tipo) <= 0)
                            tiposIntegracoes.Add(tomador.GrupoPessoas.TipoIntegracao);
                    }
                }

                if (tiposIntegracoes?.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracoes)
                    {
                        if (tipoIntegracao == null || repCargaIntegracao.ExistePorTipoIntegracao(carga.Codigo, tipoIntegracao.Codigo))
                            continue;

                        Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracao
                        {
                            Carga = carga,
                            TipoIntegracao = tipoIntegracao
                        };
                        repCargaIntegracao.Inserir(cargaIntegracao);

                    }
                }
            }
        }

        public static void RemoverIntegracoesPorConfiguracaoEmissao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesRemover = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercement,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MercadoLivre
            };

            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

            repCargaIntegracao.DeletarPorCargaETipoIntegracao(carga.Codigo, integracoesRemover);
        }

        private static bool ValidarDadosTransporteInformados(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas)
        {
            bool informados = true;
            if (((carga.Veiculo == null || cargaMotoristas == null || cargaMotoristas.Count == 0) && !carga.NaoExigeVeiculoParaEmissao) || carga.Empresa == null)
                informados = false;

            return informados;
        }

        public static void AdicionarIntegracoesCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool atualizouMotorista = false, bool atualizouModeloVeicular = false)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoA52 repConfiguracaoIntegracaoA52 = new Repositorio.Embarcador.Configuracoes.IntegracaoA52(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repositorioConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoUnilever.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repositorioConfiguracaoMotorista.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> cargaIntegracoes = repCargaIntegracao.BuscarPorCarga(carga.Codigo, true);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao in cargaIntegracoes)
            {
                if (carga.CargaTransbordo && !cargaIntegracao.TipoIntegracao.IntegrarCargaTransbordo)
                    continue;

                //cada tipo de integração deve adicionar os documentos necessários nas filas
                switch (cargaIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) && ((carga.TipoOperacao?.IntegrarPreSMAngelLira ?? false || carga.TipoOperacao.ReintegrarSMCargaAngelLira) || repCargaPedido.ExistePedidoComPontoPartidaPorCarga(carga.Codigo)))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) && (carga.TipoOperacao?.IntegrarPreSMNOX ?? false))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ANTT:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Flora:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Obramax:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas))
                            if (configuracaoIntegracao.GerarIntegracaoPreSmEtapaCargaDadosTransporteRaster)
                                AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) && (carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn) && new DigitalCom.IntegracaoDigitalCom(unitOfWork).PermitirGerarIntegracao(configuracaoVeiculo))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Neokohm:
                        Servicos.Embarcador.Integracao.Neokohm.IntegracaoNeokohm serNeokohm = new Servicos.Embarcador.Integracao.Neokohm.IntegracaoNeokohm(unitOfWork);
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) && serNeokohm.ValidarSeDeveGerarIntegracaoNeokohm(carga))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcadorFrotaCarga:
                        if (carga.Veiculo != null)
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) && carga.ExigeNotaFiscalParaCalcularFrete && !(carga.TipoOperacao?.ConfiguracaoIntegracao?.NaoIntegrarEtapa1Opentech ?? false))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, configuracaoIntegracao?.IntegrarColetaOpentech ?? false, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, tipoServicoMultisoftware, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Eship:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas))
                        {
                            if (carga.ExigeNotaFiscalParaCalcularFrete || (carga.TipoOperacao?.IntegrarCargasMultiEmbarcador ?? false))
                                AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                            else
                                AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, true, false);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Gadle:
                        if (carga.Empresa != null)
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas))
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracaoA52 = repConfiguracaoIntegracaoA52.BuscarPrimeiroRegistro();
                            if (configuracaoIntegracaoA52?.IntegrarMacrosDadosTransporteCarga ?? false)
                                AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas))
                        {
                            if (cargaIntegracao.Carga?.TipoOperacao?.ConfiguracaoCarga?.PermitirVisualizarOrdenarAsZonasDeTransporte ?? false && (cargaIntegracao.Carga?.TipoOperacao?.ConfiguracaoIntegracao?.IntegrarDadosTransporte ?? false))
                            {
                                AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                            }
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        if (carga.CargaRecebidaDeIntegracao != true && (integracaoIntercab?.IntegracaoDocumentacaoCarga ?? false))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Moniloc:
                        AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever: //Foi mexido para quando ele apertar Gerar Transportes
                        if ((carga?.TipoOperacao?.ConfiguracaoCalculoFrete?.ExecutarPreCalculoFrete ?? false) && carga.TipoOperacao != null && carga.TipoOperacao.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn)
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosValePedagio:
                        if (configuracaoIntegracaoUnilever.IntegrarDadosValePedagio && ValidarDadosTransporteInformados(carga, cargaMotoristas))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosTransporte:
                        if ((carga?.TipoOperacao?.Integracoes?.Any(t => t.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosTransporte) ?? false))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) &&
                            !(carga.Empresa.NaoGerarIntegracaoSuperAppTrizy ?? false) && carga.TipoOperacao != null &&
                            carga.Filial != null && carga.Filial.HabilitarPreViagemTrizy && carga.Filial.TipoOperacoesTrizy.Any(x => x.Codigo == carga.TipoOperacao.Codigo))
                        {
                            Dominio.Entidades.Usuario motorista = cargaMotoristas.Select(x => x.Motorista).FirstOrDefault();
                            bool podeIntegrar = true;
                            if (motorista != null && configuracaoMotorista.MotoristasIgnorados != null && configuracaoMotorista.MotoristasIgnorados.Count > 0 && configuracaoMotorista.MotoristasIgnorados.Any(obj => obj.ToLower() == motorista.Nome.ToLower()))
                                podeIntegrar = false;

                            Servicos.Log.TratarErro($"Carga: '{carga.Codigo}', Motorista: '{motorista?.Nome}' -> podeIntegrar: '{podeIntegrar}'", "IntegracaoPreTripPodeIntegrar");

                            if (podeIntegrar)
                                AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork, atualizouMotorista);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato:
                        AdicionarCargaDadosTransporteParaIntegracaoPorPedidos(carga, cargaIntegracao.TipoIntegracao, cargasPedido, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Calisto:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas))
                        {
                            if (carga.ExigeNotaFiscalParaCalcularFrete || (carga.TipoOperacao?.IntegrarCargasMultiEmbarcador ?? false))
                                AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                            else
                                AdicionarCargaParaIntegracao(carga, cargaIntegracao.TipoIntegracao, unitOfWork, true, false);
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        Repositorio.Embarcador.Configuracoes.IntegracaoEMP repositorioIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repositorioIntegracaoEMP.Buscar();

                        if ((integracaoEMP?.AtivarIntegracaoParaSILEMP ?? false) && (carga.TipoOperacao?.ConfiguracaoEMP?.AtivarIntegracaoComSIL ?? false) && (carga.CategoriaOS != CategoriaOS.Negocio))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FS:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Onisys:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) && (cargaIntegracao.Carga?.TipoOperacao?.ConfiguracaoIntegracao?.IntegrarDadosTransporte ?? false))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSLog:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_API4:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vedacit:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Lactalis:
                        AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cassol:
                        Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repositorioCargaDadosTransporteIntegracao.ExisteComProtocoloPorCargaETipoIntegracaoRetornoCargaDadosTransporte(carga.Codigo, TipoIntegracao.Cassol);

                        if ((ValidarDadosTransporteInformados(carga, cargaMotoristas) && cargaDadosTransporteIntegracao != null))
                        {
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        }

                        if (cargaDadosTransporteIntegracao is null && carga.TipoCarregamento != null)
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) && (configuracao.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.AoInformarVeiculoNaCarga))
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vector:
                        if (carga.Empresa != null)
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork, false, atualizouModeloVeicular);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Skymark:
                        if (ValidarDadosTransporteInformados(carga, cargaMotoristas) && carga.Veiculo?.Tipo == "T")
                            AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Senior:
                        AdicionarCargaDadosTransporteParaIntegracao(carga, cargaIntegracao.TipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task AtualizarSituacaoCargaIntegracaoAsync(int codigoCarga)
        {
            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.Carga.Carga svcCarga = new Carga.Carga(_unitOfWork, _cancellationToken);

            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repositorioFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao repositorioTipoOperacaoIntegracao = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();

            bool integracaoFilialEmissora = false;

            if (carga.EmpresaFilialEmissora != null && !carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                integracaoFilialEmissora = true;

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao)
            {
                if (repositorioCargaCTeIntegracao.ContarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                    await repositorioCargaEDIIntegracao.ContarPorCargaAsync(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, integracaoFilialEmissora) > 0 ||
                    repositorioCargaCargaIntegracao.ContarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, integracaoFilialEmissora) > 0)
                {
                    carga.PossuiPendencia = true;
                    carga.MotivoPendencia = "Problema ao realizar as integrações.";
                    await repositorioCarga.AtualizarAsync(carga);

                    //vai passar muitas vezes por aqui se for avon, por isso não deixa cair no hub da carga
                    if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        return;
                }
                else if (repositorioCargaCTeIntegracao.ContarPorCarga(carga.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno }) > 0 ||
                         await repositorioCargaEDIIntegracao.ContarPorCargaAsync(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, integracaoFilialEmissora) > 0 ||
                         repositorioCargaCargaIntegracao.ContarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, integracaoFilialEmissora) > 0)
                {
                    return;
                }
                else
                {
                    if (carga.TipoOperacao != null && repositorioTipoOperacaoIntegracao.ExisteTipoOperacaoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MarfrigImpressaoDocumentos, carga.TipoOperacao.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MarfrigImpressaoDocumentos);
                        Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoImpressaoMarfrig = AdicionarCargaParaIntegracao(carga, tipoIntegracao, _unitOfWork, false, false);
                        if (cargaIntegracaoImpressaoMarfrig.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                        {
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao;
                            carga = svcCarga.AtualizarStatusCustoExtra(carga, svcHubCarga, repositorioCarga);
                            await repositorioCarga.AtualizarAsync(carga);
                            return;
                        }

                    }

                    if (!integracaoFilialEmissora)
                    {
                        if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe))
                        {
                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte;
                            carga = svcCarga.AtualizarStatusCustoExtra(carga, svcHubCarga, repositorioCarga);
                            carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                            Servicos.Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema, OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema }, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", _unitOfWork);
                        }
                        else
                        {
                            carga.SituacaoCarga = (configuracaoTMS).SituacaoCargaAposIntegracao;
                            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos && (carga.TipoOperacao?.NaoNecessarioConfirmarImpressaoDocumentos ?? false))
                            {
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte;
                                carga = svcCarga.AtualizarStatusCustoExtra(carga, svcHubCarga, repositorioCarga);
                                carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                                Servicos.Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema, OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema }, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", _unitOfWork);
                            }
                        }

                        await repositorioCarga.AtualizarAsync(carga);

                        if (svcCarga.PermitirFinalizarCargaAutomaticamente(carga, configuracaoTMS, _tipoServicoMultisoftware, repositorioCargaMDFe))
                            svcCarga.ValidarCargasFinalizadas(ref carga, _tipoServicoMultisoftware, null, _unitOfWork);

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                        {
                            new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(_unitOfWork).GerarIntegracaoNotificacao(carga, TipoNotificacaoApp.MotoristaPodeSeguirViagem);
                            if ((configuracaoTMS).QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte)
                                await Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciarAsync(carga, configuracaoTMS, null, "Carga em transporte", _unitOfWork);
                        }
                    }
                    else
                        Servicos.Embarcador.Carga.Documentos.LiberarEmissaoFilialEmissora(carga, _unitOfWork);
                }

                svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);
            }
            else
            {
                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega = repositorioFluxoColetaEntrega.BuscarPorCarga(carga.Codigo);

                    if (fluxoColetaEntrega != null)
                    {
                        if (fluxoColetaEntrega.EtapasOrdenadas[fluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Integracao)
                        {
                            if (repositorioCargaCTeIntegracao.ContarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0 ||
                                await repositorioCargaEDIIntegracao.ContarPorCargaAsync(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, integracaoFilialEmissora) > 0 ||
                                repositorioCargaCargaIntegracao.ContarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, integracaoFilialEmissora) > 0)
                            {
                                Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarRejeitarEtapa(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Integracao, _unitOfWork);
                            }
                            else if (repositorioCargaCTeIntegracao.ContarPorCarga(carga.Codigo, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno }) > 0 ||
                                     await repositorioCargaEDIIntegracao.ContarPorCargaAsync(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, integracaoFilialEmissora) > 0 ||
                                     repositorioCargaCargaIntegracao.ContarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, integracaoFilialEmissora) > 0)
                            {
                                return;
                            }
                            else
                            {
                                fluxoColetaEntrega.DataIntegracao = DateTime.Now;

                                await repositorioFluxoColetaEntrega.AtualizarAsync(fluxoColetaEntrega);

                                Servicos.Embarcador.Hubs.FluxoColetaEntrega hubFluxoColetaEntrega = new Hubs.FluxoColetaEntrega();
                                hubFluxoColetaEntrega.InformarFluxoColetaEntregaAtualizado(fluxoColetaEntrega.Codigo, _unitOfWork.StringConexao);

                                Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Integracao, _unitOfWork);
                            }
                        }
                    }
                }
            }

            if (carga.AguardarIntegracaoDadosTransporte && carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao> situacoes = repositorioCargaDadosTransporteIntegracao.BuscarSituacoesPorCarga(carga.Codigo);

                if (!situacoes.Any() || situacoes.All(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado))
                {
                    carga.ProblemaIntegracaoDadosTransporte = false;
                    carga.PossuiPendencia = false;
                    carga.MotivoPendencia = string.Empty;

                    if (carga.NaoExigeVeiculoParaEmissao || (carga.Veiculo != null && carga.Motoristas.Count > 0 && carga.TipoDeCarga != null && carga.ModeloVeicularCarga != null))
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                    else
                        carga.AguardarIntegracaoDadosTransporte = false;

                    await repositorioCarga.AtualizarAsync(carga);

                    svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);
                }
                else if (!carga.LiberadaComProblemaIntegracaoDadosTransporte && situacoes.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao))
                {
                    carga.ProblemaIntegracaoDadosTransporte = true;
                    carga.PossuiPendencia = true;
                    carga.MotivoPendencia = "Problema ao realizar as integrações.";

                    await repositorioCarga.AtualizarAsync(carga);

                    svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);
                }
            }

            if ((configuracaoTMS).NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada && carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
            {
                if (!carga.ExigeNotaFiscalParaCalcularFrete)
                {
                    if (repositorioCargaCargaIntegracao.ContarEtapaTransportadorPorCargaDiferenteColeta(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                        carga.AguardarIntegracaoEtapaTransportador = false;
                    else if ((repositorioCargaCargaIntegracao.ContarEtapaTransportadorPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0) ||
                       (repositorioCargaCargaIntegracao.ContarEtapaTransportadorPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0) ||
                       (repositorioCargaCargaIntegracao.ContarEtapaTransportadorPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno) > 0))
                        carga.AguardarIntegracaoEtapaTransportador = true;
                    else
                        carga.AguardarIntegracaoEtapaTransportador = false;
                    await repositorioCarga.AtualizarAsync(carga);
                }
                else
                {
                    if (repositorioCargaDadosTransporteIntegracao.ContarEtapaTransportadorPorCargaDiferenteColeta(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                        carga.AguardarIntegracaoEtapaTransportador = false;
                    else if ((repositorioCargaDadosTransporteIntegracao.ContarEtapaTransportadorPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0) ||
                       (repositorioCargaDadosTransporteIntegracao.ContarEtapaTransportadorPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0) ||
                       (repositorioCargaDadosTransporteIntegracao.ContarEtapaTransportadorPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno) > 0))
                        carga.AguardarIntegracaoEtapaTransportador = true;
                    else
                        carga.AguardarIntegracaoEtapaTransportador = false;

                    if (VerificarSeACargaPermiteAvancoAutomaticoDaEtapaNFe(carga, _unitOfWork))
                    {
                        carga.ProcessandoDocumentosFiscais = true;
                        carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                    }

                    await repositorioCarga.AtualizarAsync(carga);
                }
            }
        }

        private static bool VerificarSeACargaPermiteAvancoAutomaticoDaEtapaNFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.AguardarIntegracaoEtapaTransportador)
                return false;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (!repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo))
                return false;

            if ((carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && !(carga.TipoOperacao?.NaoExigeConformacaoDasNotasEmissao ?? false))
                return false;

            return true;
        }

        public static void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec servicoIntegracaoOrtec = new Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork);
            if (servicoIntegracaoOrtec.IsPossuiIntegracaoOrtec())
                servicoIntegracaoOrtec.TrocarCarga(cargaAtual, cargaNova, tipoServicoMultisoftware);

        }

        public async Task<bool> VerificarSePossuiIntegracaoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool integracaoFilialEmissora, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioIntegracaoCTe = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioIntegracaoCarga = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

            if (repositorioIntegracaoCTe.ContarPorCargaESituacaoDiff(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            if (await repositorioIntegracaoEDI.ContarPorCargaESituacaoDiffAsync(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, integracaoFilialEmissora) > 0)
                return true;

            if (repositorioIntegracaoCarga.ContarPorCargaESituacaoDiff(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            return false;
        }

        public static void ReenviarIntegracaoDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Servicos.Log.TratarErro($"Carga: {carga.CodigoCargaEmbarcador} | {carga.Codigo}; TipoIntegracao: {tipoIntegracao.ObterDescricao()}", "TRIZY_AtualizarMotoristas");
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (!repTipoIntegracao.ExistePorTipo(tipoIntegracao))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracao = repIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao);

            Servicos.Log.TratarErro($"Carga: {carga.CodigoCargaEmbarcador} | {carga.Codigo}; Integracao: {integracao?.Codigo ?? 0}", "TRIZY_AtualizarMotoristas");
            if (integracao != null)
            {
                if (tipoIntegracao == TipoIntegracao.Trizy && auditado != null)
                    Auditoria.Auditoria.Auditar(auditado, carga, "Atualização do motorista - integração reenviada - PreTrip", unitOfWork);

                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                repIntegracao.Atualizar(integracao);
                Servicos.Log.TratarErro($"Carga: {carga.CodigoCargaEmbarcador} | {carga.Codigo}; integracao.SituacaoIntegracao: {integracao.SituacaoIntegracao}", "TRIZY_AtualizarMotoristas");
            }
        }

        public static void AdicionarCargaDadosTransporteParaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, bool integracaoColeta, bool naoAvancarRejeicaoIntegracaoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool atualizouMotorista = false, bool atualizouModeloVeicular = false)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao.Codigo, integracaoColeta);

            if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab && carga.CargaRecebidaDeIntegracao)
                return;

            List<TipoIntegracao> situacaoesPermitidasParaReenviarIntegracaoTodaVezQueSalvaDocumentoTransporte = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                TipoIntegracao.SaintGobain,
                TipoIntegracao.DigitalCom,
                TipoIntegracao.UnileverDadosTransporte,
                TipoIntegracao.UnileverDadosValePedagio,
                TipoIntegracao.ATSLog,
                TipoIntegracao.Cebrace,
                TipoIntegracao.Obramax,
                TipoIntegracao.Vedacit,
                TipoIntegracao.Cassol,
                TipoIntegracao.ApisulLog,
                TipoIntegracao.Routeasy,
                TipoIntegracao.SAP_API4,
                TipoIntegracao.YMS,
                TipoIntegracao.Skymark,
                TipoIntegracao.Onisys
            };

            if (atualizouMotorista)
                situacaoesPermitidasParaReenviarIntegracaoTodaVezQueSalvaDocumentoTransporte.Add(TipoIntegracao.Trizy);

            if (atualizouModeloVeicular)
                situacaoesPermitidasParaReenviarIntegracaoTodaVezQueSalvaDocumentoTransporte.Add(TipoIntegracao.Vector);

            if (cargaDadosTransporteIntegracao == null)
            {
                cargaDadosTransporteIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao()
                {
                    Carga = carga,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = "",
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = tipoIntegracao,
                    IntegracaoColeta = integracaoColeta
                };

                repCargaDadosTransporteIntegracao.Inserir(cargaDadosTransporteIntegracao);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || tipoIntegracao.BloquearEtapaTransporteSeRejeitar)
                    carga.AguardarIntegracaoDadosTransporte = true;
            }
            else if (situacaoesPermitidasParaReenviarIntegracaoTodaVezQueSalvaDocumentoTransporte.Contains(tipoIntegracao.Tipo))
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
            }

            carga.AguardarIntegracaoEtapaTransportador = naoAvancarRejeicaoIntegracaoTransportador;

            repCarga.Atualizar(carga);
        }

        #endregion Métodos Públicos Estáticos

        #region Métodos Privados de Integração

        private async Task<List<int>> VerificarCargaIntegracaoPendentesAsync(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso)
        {
            _unitOfWork.FlushAndClear();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork, _cancellationToken);

            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(_unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.VerificarCargaIntegracaoPendentes);

            List<Task<int>> tasks = new List<Task<int>>();
            List<int> codigosIntegracoesPendentes = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCargaCargaIntegracao.BuscarIntegracoesPendentesDiferentesDeTipoAsync(numeroTentativas, minutosACadaTentativa, "DataIntegracao", "asc", limiteRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva).GetAwaiter().GetResult());

            foreach (int codigo in codigosIntegracoesPendentes)
                tasks.Add(Task.Run(() => ProcessaCargaIntegaracaoAguardandoAsync(codigo, auditado, clienteURLAcesso)));

            return (await Task.WhenAll(tasks)).ToList();
        }

        private async Task<int> ProcessaCargaIntegaracaoAguardandoAsync(int codigoCargaCargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente = await repositorioCargaCargaIntegracao.BuscarPorCodigoAsync(codigoCargaCargaIntegracao, false);

            switch (integracaoPendente.TipoIntegracao.Tipo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech:
                    if (integracaoPendente.TipoIntegracao.IntegracaoTransportador)
                        new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unitOfWork).IntegrarCargaEtapaTransporte(integracaoPendente);
                    else
                        new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unitOfWork).IntegrarCarga(integracaoPendente, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Eship:
                    Servicos.Embarcador.Integracao.Eship.IntegracaoEship serIntegracaoEship = new Eship.IntegracaoEship(unitOfWork);
                    serIntegracaoEship.MontarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:
                    Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.IntegrarCarga(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DTe:
                    Servicos.Embarcador.Integracao.DTe.Recepcao.IntegrarRecepcaoLote(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trafegus:
                    new Servicos.Embarcador.Integracao.Trafegus.IntegracaoCargaTrafegus(unitOfWork, _tipoServicoMultisoftware).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PortalCabotagem:
                    new Servicos.Embarcador.Integracao.PortalCabotagem.IntegracaoPortalCabotagem(unitOfWork).Integrar(integracaoPendente.CodigosCTes.FirstOrDefault());
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk:
                    Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.IntegrarCarga(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MundialRisk:
                    Servicos.Embarcador.Integracao.MundialRisk.IntegracaoMundialRisk.IntegrarCarga(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logiun:
                    Servicos.Embarcador.Integracao.Logiun.IntegracaoLogiun.IntegrarCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                    if (await repositorioConfiguracao.UtilizaAppTrizy())
                    {
                        if (integracaoPendente.Carga.Filial?.HabilitarPreViagemTrizy ?? false)
                            if (!integracaoPendente.FinalizarCargaAnterior || string.IsNullOrWhiteSpace(integracaoPendente.Carga.IDIdentificacaoTrizzy))
                                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarCargaAPPTrizy(integracaoPendente, unitOfWork, clienteMultisoftware: _clienteMultisoftware);
                            else
                                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AtualizarViagem(integracaoPendente.Carga, "CANCELED", unitOfWork, null, false, null, false, null, integracaoPendente);
                        else
                            Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarCargaAPPTrizy(integracaoPendente, unitOfWork, clienteMultisoftware: _clienteMultisoftware);
                    }
                    else
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarApiIsRomaneio(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal:
                    new Servicos.Embarcador.Integracao.ArcelorMittal.IntegracaoArcelorMittal(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CadastrosMulti:
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCTeAnterior.IntegrarDocumentoAnterior(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MicDta:
                    Servicos.Embarcador.Integracao.MICDTA.IntegracaoMICDTA.IntegrarMICDTA(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX:
                    Servicos.Embarcador.Integracao.AX.IntegracaoAX.IntegrarContratoFrete(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Totvs:
                    Servicos.Embarcador.Integracao.Totvs.Carga.IntegrarCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PH:
                    Servicos.Embarcador.Integracao.PH.IntegracaoPH.IntegrarCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DiariaMotoristaProprio:
                    Servicos.Embarcador.Integracao.Diaria.DiariaMotorista.IntegrarCarga(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny:
                    new Servicos.Embarcador.Integracao.Buonny.SolicitacaoMonitoramento(unitOfWork, _tipoServicoMultisoftware).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX:
                    Servicos.Embarcador.Integracao.NOX.IntegracaoNOX.IntegrarCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour:
                    new Servicos.Embarcador.Integracao.Carrefour.IntegracaoCarrefour(unitOfWork).IntegrarCarga(integracaoPendente, integracaoPendente.Carga, aguardaRecebimento: true);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GoldenService:
                    Servicos.Embarcador.Integracao.GoldenService.IntegracaoGoldenService.IntegrarCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPA:
                    Servicos.Embarcador.Integracao.GPA.IntegracaoGPA.IntegrarCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe:
                    Servicos.Embarcador.Integracao.GPA.IntegracaoGPA.IntegrarMDFeCIOT(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcadorCIOT:
                    Servicos.Embarcador.Integracao.MultiEmbarcador.CIOT.IntegrarCIOT(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog:
                    Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao:
                    Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarCargaEscrituracao(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogMDFe:
                    Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarMDFeManual(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig:
                    new Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MarfrigOrdemEmbarque:
                    new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster:
                    Servicos.Embarcador.Integracao.Raster.IntegracaoRaster.IntegrarCarga(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.RasterAtualizacao:
                    new Servicos.Embarcador.Integracao.Raster.IntegracaoRaster(unitOfWork).IntegrarAtualizacaoRaster(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverFourKites:
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Krona:
                    new Servicos.Embarcador.Integracao.Krona.IntegracaoKrona(unitOfWork).IntegrarCarga(integracaoPendente, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Infolog:
                    new Servicos.Embarcador.Integracao.Infolog.IntegracaoInfolog(unitOfWork, _tipoServicoMultisoftware).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee:
                    Servicos.Embarcador.Integracao.Digibee.IntegracaoDigibee.IntegrarCargaTransporte(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX:
                    Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarCTeMDFe(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo:
                    Servicos.Embarcador.Integracao.Riachuelo.IntegracaoRiachuelo.IntegrarFinalizacaoCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba:
                    new Servicos.Embarcador.Integracao.Piracanjuba.IntegracaoPiracanjuba(unitOfWork, _tipoServicoMultisoftware).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain:
                    new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork).IntegrarValores(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52:
                    new Servicos.Embarcador.Integracao.A52.IntegracaoA52(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPA:
                    new Servicos.Embarcador.Integracao.DPA.IntegracaoDPA(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPACiot:
                    new Servicos.Embarcador.Integracao.DPA.IntegracaoDPA(unitOfWork).IntegrarCIOT(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverRelevanciaCustos:
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRelevanciCustos(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog:
                    if (integracaoPendente.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                        await new Servicos.Embarcador.Integracao.ApisulLog.IntegracaoApisulLog(unitOfWork).IntegrarSMP(integracaoPendente);
                    else
                        await new Servicos.Embarcador.Integracao.ApisulLog.IntegracaoApisulLog(unitOfWork).BuscarSMPAsync(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FrimesaFrete:
                    Frimesa.IntegracaoFrimesa servicoIntegracaoFrimesa = new Servicos.Embarcador.Integracao.Frimesa.IntegracaoFrimesa(unitOfWork);
                    if (servicoIntegracaoFrimesa.PermiteFinalizarIntegracaoFrete(integracaoPendente))
                        servicoIntegracaoFrimesa.IntegrarFrete(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FrimesaValePedagio:
                    Frimesa.IntegracaoFrimesa servicoIntegracaoFrimesaValePedagio = new Servicos.Embarcador.Integracao.Frimesa.IntegracaoFrimesa(unitOfWork);
                    if (servicoIntegracaoFrimesaValePedagio.PermiteFinalizarIntegracaoValePedagio(integracaoPendente))
                        servicoIntegracaoFrimesaValePedagio.IntegrarValePedagio(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                    {
                        if (integracaoPendente.RealizarIntegracaoCompleta)
                            new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork).IntegrarCargaCompleta(integracaoPendente);
                        else
                            new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork).IntegrarTodosDocumentos(integracaoPendente);
                        break;
                    }
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                    new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                    new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarCarga(integracaoPendente, clienteURLAcesso?.URLAcesso ?? "");
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                    new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarCarga(integracaoPendente, clienteURLAcesso?.URLAcesso ?? "");
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Tecnorisk:
                    new Servicos.Embarcador.Integracao.Tecnorisk.IntegracaoTecnorisk(unitOfWork).IntegrarSolicitacaoMonitoramento(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Loggi:
                    new Servicos.Embarcador.Integracao.Loggi.IntegracaoLoggi(unitOfWork, auditado).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxCTE:
                    new Servicos.Embarcador.Integracao.Obramax.IntegracaoObramax(unitOfWork, string.Empty).IntegrarCTeCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxNFE:
                    new Servicos.Embarcador.Integracao.Obramax.IntegracaoObramax(unitOfWork, string.Empty).IntegrarNFSeCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxProvisao:
                    new Servicos.Embarcador.Integracao.Obramax.IntegracaoObramax(unitOfWork, string.Empty).IntegrarProvisao(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9:
                    new Servicos.Embarcador.Integracao.SAPV9.IntegracaoSAPV9(unitOfWork).IntegrarContratoFreteTerceiro(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST:
                    new Servicos.Embarcador.Integracao.SAPST.IntegracaoSAPST(unitOfWork).IntegrarAdiantamentoContrato(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LogRisk:
                    new Servicos.Embarcador.Integracao.LogRisk.SolicitacaoMonitoramento(unitOfWork, _tipoServicoMultisoftware).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei:
                    Servicos.Embarcador.Integracao.Comprovei.IntegracaoComprovei.IntegrarCarga(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                    new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Atlas:
                    new Servicos.Embarcador.Integracao.Atlas.IntegracaoAtlas(integracaoPendente, unitOfWork, _tipoServicoMultisoftware).IntegrarCarga();
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Calisto:
                    new Servicos.Embarcador.Integracao.Calisto.IntegracaoCalisto(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MarfrigImpressaoDocumentos:
                    new Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig(unitOfWork, auditado).IntegrarImpressaoDocumentos(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logvett:
                    new Servicos.Embarcador.Integracao.LogVett.IntegracaoLogvett(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverRecusa:
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarUnileverRecusa(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverInfrutifera:
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarUnileverInfrutifera(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux:
                    new Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxCONEMB(unitOfWork, integracaoPendente).IntegrarCarga();
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Froggr:
                    new Servicos.Embarcador.Integracao.Froggr.IntegracaoFroggr(unitOfWork).SolicitaSM(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                    new Boticario.IntegracaoBoticario(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTeAnterioresLoggi:
                    (new Servicos.Embarcador.Integracao.Loggi.IntegracaoLoggi(unitOfWork, auditado)).IntegrarCargaCTeAnterioresLoggi(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyEventos:
                    (new Servicos.Embarcador.Integracao.TrizyEventos.IntegracaoTrizyEventos(unitOfWork, auditado)).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace:
                    (new Servicos.Embarcador.Integracao.Cebrace.IntegracaoCebrace(unitOfWork)).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dexco:
                    new Servicos.Embarcador.Integracao.Dexco.IntegracaoDexco(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vector:
                    new Servicos.Embarcador.Integracao.Vector.IntegracaoVector(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TransSat:
                    new Servicos.Embarcador.Integracao.TransSat.IntegracaoTransSat(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ComproveiRota:
                    new Servicos.Embarcador.Integracao.Comprovei.IntegracaoComproveiRota(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SistemaTransben:
                    new Servicos.Embarcador.Integracao.SistemaTransben.IntegracaoSistemaTransben(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb:
                    new Servicos.Embarcador.Integracao.ATSSmartWeb.IntegracaoATSSmartWeb(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                default:
                    integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar";
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendente.NumeroTentativas++;
                    integracaoPendente.DataIntegracao = DateTime.Now;
                    await repositorioCargaCargaIntegracao.AtualizarAsync(integracaoPendente);
                    break;
            }

            if (integracaoPendente.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && integracaoPendente.NumeroTentativas >= 2)
                GerarAtendimentoParaIntegracoesPendentes(integracaoPendente, unitOfWork);

            return integracaoPendente.Carga.Codigo;
        }

        private async Task<List<int>> VerificarIntegracoesCargaDadosTransportePendentesAsync(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURL, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(_unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.VerificarIntegracoesCargaDadosTransportePendentes);

            List<int> codigosIntegracoesPendentes = servicoOrquestradorFila.Ordenar((limiteRegistros) => repCargaDadosTransporteIntegracao.BuscarIntegracoesPendentesAsync(numeroTentativas, minutosACadaTentativa, "DataIntegracao", "asc", limiteRegistros).GetAwaiter().GetResult());

            List<Task<int>> tasks = new List<Task<int>>();
            foreach (int codigo in codigosIntegracoesPendentes)
                tasks.Add(Task.Run(() => ProcessaCargaDadosTransporteAguardandoAsync(codigo, auditado, clienteURL)));

            return (await Task.WhenAll(tasks)).ToList();
        }

        private async Task<int> ProcessaCargaDadosTransporteAguardandoAsync(int codigoCargaDadosTransporteIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURL)
        {
            Servicos.Embarcador.Hubs.Carga servicoNotificacaoCarga = new Hubs.Carga();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork, _cancellationToken);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoPendente = await repositorioCargaDadosTransporteIntegracao.BuscarPorCodigoAsync(codigoCargaDadosTransporteIntegracao, false);


            switch (integracaoPendente.TipoIntegracao.Tipo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:
                    Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.IntegrarCarga(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Neokohm:
                    new Servicos.Embarcador.Integracao.Neokohm.IntegracaoNeokohm(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ANTT:
                    Servicos.Embarcador.CIOT.Pagbem svcPagbem = new CIOT.Pagbem();
                    svcPagbem.IntegrarANTTCarga(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                    if (await repositorioConfiguracao.UtilizaAppTrizy())
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarPreTripAPPTrizy(integracaoPendente, unitOfWork, _clienteMultisoftware);
                    else
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarApiEnvioComprovante(integracaoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComprovanteTrizy.ComprovanteEntrega, unitOfWork, null, null, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato:
                    new Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobainAgendamento:
                    new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork).IntegrarCarga(integracaoPendente, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobainCarga:
                    new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork).IntegrarCargaSnowFlake(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech:
                    new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unitOfWork).IntegrarCargaEtapaTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster:
                    Servicos.Embarcador.Integracao.Raster.IntegracaoRaster.IntegrarCargaDadosTransporte(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig:
                    Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig servicoMarfrig = new Marfrig.IntegracaoMarfrig(unitOfWork);
                    servicoMarfrig.IntegrarCargaPreCalculoFrete(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Gadle:
                    Servicos.Embarcador.Integracao.Gadle.IntegracaoGadle servicoGadle = new Gadle.IntegracaoGadle(unitOfWork);
                    servicoGadle.IntegraDadosCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52:
                    Servicos.Embarcador.Integracao.A52.IntegracaoA52 servicoA52 = new A52.IntegracaoA52(unitOfWork);
                    servicoA52.IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                    Servicos.Embarcador.Integracao.Boticario.IntegracaoBoticario servicoBoticario = new Boticario.IntegracaoBoticario(unitOfWork);
                    servicoBoticario.IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                    new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Moniloc:
                    new Servicos.Embarcador.Integracao.Moniloc.IntegracaoMoniloc(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRetornoCargaPreCalculoFrete(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosValePedagio:
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarDadosValePedagio(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom:
                    new Servicos.Embarcador.Integracao.DigitalCom.IntegracaoDigitalCom(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverLeilaoManual:
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarLeilaoManual(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosTransporte:
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Eship:
                    new Eship.IntegracaoEship(unitOfWork).ControlePortaria(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado:
                    new Servicos.Embarcador.Integracao.Brado.IntegracaoBrado(unitOfWork).IntegrarCargasDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Calisto:
                    new Servicos.Embarcador.Integracao.Calisto.IntegracaoCalisto(unitOfWork).IntegrarCargasDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcadorFrotaCarga:
                    new Servicos.Embarcador.Integracao.MultiEmbarcador.IntegracaoFrotaDaCarga(unitOfWork).IntegrarFrotaDaCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                    new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarDadosCarga(integracaoPendente, clienteURL?.URLAcesso ?? "");
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                    new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarDadosCarga(integracaoPendente, clienteURL?.URLAcesso ?? "");
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal:
                    new Servicos.Embarcador.Integracao.ArcelorMittal.IntegracaoArcelorMittal(unitOfWork).IntegrarDadosCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSLog:
                    new Servicos.Embarcador.Integracao.ATSLog.IntegracaoATSLog(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX:
                    Servicos.Embarcador.Integracao.NOX.IntegracaoNOX.IntegrarCargaDadosTransporte(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace:
                    new Servicos.Embarcador.Integracao.Cebrace.IntegracaoCebrace(unitOfWork).IntegrarDadosCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_API4:
                    new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarDadosCarga(integracaoPendente, auditado);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML:
                    Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.IntegrarSMLVeiculoVazio(integracaoPendente, unitOfWork, _tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Flora:
                    new Servicos.Embarcador.Integracao.Flora.IntegracaoFlora(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Obramax:
                    new Servicos.Embarcador.Integracao.Obramax.IntegracaoObramax(unitOfWork, clienteURL.URLAcesso).IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FS:
                    new Servicos.Embarcador.Integracao.FS.IntegracaoFS(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vedacit:
                    new Servicos.Embarcador.Integracao.Vedacit.IntegracaoVedacit(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cassol:
                    new Servicos.Embarcador.Integracao.Cassol.IntegracaoCassol(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Lactalis:
                    new Servicos.Embarcador.Integracao.Lactalis.IntegracaoLactalis(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee:
                    Servicos.Embarcador.Integracao.Digibee.IntegracaoDigibee.IntegrarCargaDadosTransporte(integracaoPendente, unitOfWork);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog:
                    await new Servicos.Embarcador.Integracao.ApisulLog.IntegracaoApisulLog(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente, _clienteMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb:
                    new Servicos.Embarcador.Integracao.ATSSmartWeb.IntegracaoATSSmartWeb(unitOfWork).IntegrarCarga(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Routeasy:
                    new Servicos.Embarcador.Integracao.RoutEasy.IntegracaoRoutEasy(unitOfWork).IntegrarPedidosAtualizacaoSituacao(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Senior:
                    await new Servicos.Embarcador.Integracao.Senior.IntegracaoSenior(unitOfWork).IntegrarCargaDadosTransporteAsync(integracaoPendente, _cancellationToken);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vector:
                    new Servicos.Embarcador.Integracao.Vector.IntegracaoVector(unitOfWork).IntegrarRecebimentoViagem(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS:
                    new Servicos.Embarcador.Integracao.YMS.IntegracaoYMS(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Skymark:
                    await new Servicos.Embarcador.Integracao.SkyMark.IntegracaoSkyMark(unitOfWork, _cancellationToken).IntegrarCargaDadosTransporteAsync(integracaoPendente);
                    break;
                default:
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendente.ProblemaIntegracao = "Integração não implementada.";
                    integracaoPendente.NumeroTentativas++;
                    integracaoPendente.DataIntegracao = DateTime.Now;

                    await repositorioCargaDadosTransporteIntegracao.AtualizarAsync(integracaoPendente);

                    break;
            }

            servicoNotificacaoCarga.InformarCargaDadosTransporteIntegracaoAtualizado(integracaoPendente.Carga.Codigo);

            return integracaoPendente.Carga.Codigo;
        }

        #endregion Métodos Privados de Integração

        #region Métodos Privados Estáticos

        private static void AdicionarIntegracaoMultiEmbarcadorCIOT(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoOperacao == null || carga.TipoOperacao.HabilitarIntegracaoMultiEmbarcador != true)
                return;

            if (carga.TipoOperacao.IntegrarCIOTMultiEmbarcador == true)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcadorCIOT, unitOfWork);
        }

        private static void AdicionarIntegracaoMultiEmbarcador(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!repositorioCargaCTe.ExistePorCargaConfiguracaoIntegrarXMLCTe(carga.Codigo))
                    return;
            }
            else
            {
                if (!cargaPedidos.Any(obj => obj.ObterTomador()?.GrupoPessoas?.HabilitarIntegracaoXmlCteMultiEmbarcador == true))
                    return;
            }

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador, unitOfWork);
        }

        private static void AdicionarIntegracaoDiariaPagamentoMotorista(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoOperacao == null || carga.TipoOperacao.GerarDiariaMotoristaProprio != true || carga.Motoristas == null || carga.Motoristas.Count() == 0 || carga.Motoristas.FirstOrDefault().TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro)
                return;

            if (carga.TipoOperacao.GerarDiariaMotoristaProprio == true)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DiariaMotoristaProprio, unitOfWork);
        }

        private static void AdicionarIntegracaoBrasilRisk(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoBrasilRisk && (carga.TipoOperacao?.PossuiIntegracaoBrasilRisk ?? false) == true && !(carga?.Empresa?.NaoGerarSMNaBrk ?? false))

                if (configuracaoIntegracao.ValorBaseBrasilRisk > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidos = repCargaPedido.BuscarCodigosCargaPedidoPorCarga(carga.Codigo, false);
                    decimal valorMercadoria = 1;
                    if (cargaPedidos?.Count > 0)
                    {
                        IEnumerable<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoXMLNotaFiscal> notasFiscais = cargaPedidos.SelectMany(cargaPedido => cargaPedido.NotasFiscais).Distinct();
                        valorMercadoria = notasFiscais.Sum(notaFiscal => notaFiscal.Valor);
                    }

                    if (valorMercadoria >= configuracaoIntegracao.ValorBaseBrasilRisk)
                        AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk, unitOfWork, false);
                }
                else
                    AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk, unitOfWork, false);
            else
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk, unitOfWork);
        }

        private static void AdicionarIntegracaoMundialRisk(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoMundialRisk && carga.TipoOperacao?.PossuiIntegracaoMundialRisk == true)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MundialRisk, unitOfWork, false);
            else
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MundialRisk, unitOfWork);
        }

        private static void AdicionarIntegracaoTrizy(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            if (carga == null || !(configuracaoIntegracao?.PossuiIntegracaoTrizy ?? false))
                return;

            if (((carga.Motoristas == null) || !(carga.Motoristas.Any()) || (carga.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false)) && !(carga.Filial?.HabilitarPreViagemTrizy ?? false) && (carga.Empresa?.NaoGerarIntegracaoSuperAppTrizy ?? false))
                return;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy integracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();

            if (integracaoTrizy?.ValidarIntegracaoPorOperacao ?? false)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga = servicoCarga.ObterTipoIntegracoesPorTipoOperacaoETipoCarga(carga, unitOfWork);
                if (!integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy))
                    return;
            }

            Servicos.Log.TratarErro($"AdicionarIntegracaoTrizy -> Carga: '{carga.Codigo}'", "IntegracaoTrizy");
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repCargaPedido.PossuiIntegracaoPedido(carga.Codigo))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, unitOfWork, false);
            else
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, unitOfWork, false);

        }

        private static void AdicionarIntegracaoTrizyContrato(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(configuracaoIntegracao?.PossuiIntegracaoTrizy ?? false))
                return;

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato, unitOfWork);
        }

        private static void AdicionarIntegracaoArcelorMittal(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repConfiguracaoArcelor = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(unitOfWork);
            if (carga == null)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal integracaoArcelorMittal = repConfiguracaoArcelor.Buscar();

            if (!string.IsNullOrWhiteSpace(integracaoArcelorMittal?.URLConfirmarAvancoTransporte))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal, unitOfWork);
        }

        private static void AdicionarIntegracaoDocumentoAnterior(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoCadastroMulti repositorioIntegracaoCadastroMulti = new Repositorio.Embarcador.Configuracoes.IntegracaoCadastroMulti(unitOfWork);

            if (carga == null || configuracaoIntegracao == null)
                return;

            if (configuracaoIntegracao.PossuiIntegracaoDeCadastrosMulti && repCargaPedido.PossuiIntegracaoDocumentoAnterior(carga.Codigo))
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCadastroMulti integracaoCadastroMulti = repositorioIntegracaoCadastroMulti.Buscar();

                if (integracaoCadastroMulti?.EnviarDocumentacaoCTeAverbacaoInstancia ?? false)
                    AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CadastrosMulti, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoMICDTA(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.RealizarIntegracaoComMicDta ?? false))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMicDta repositorioConfiguracaoIntegracaoMicDta = new Repositorio.Embarcador.Configuracoes.IntegracaoMicDta(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta configuracaoIntegracao = repositorioConfiguracaoIntegracaoMicDta.Buscar();

            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracao && !configuracaoIntegracao.GerarIntegracaNaEtapaDoFrete)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MicDta, unitOfWork);
        }

        private static void AdicionarIntegracaoAX(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoAX && carga.Terceiro != null && (carga.TipoOperacao == null || !carga.TipoOperacao.NaoRealizarIntegracaoComAX.HasValue || !carga.TipoOperacao.NaoRealizarIntegracaoComAX.Value))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX, unitOfWork, false);
        }

        private static void AdicionarIntegracaoTotvs(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoDeTotvs)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Totvs, unitOfWork);
        }

        private static void AdicionarIntegracaoDTe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.URLRecepcaoDTe))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DTe, unitOfWork);
        }

        private static void AdicionarIntegracaoLogiun(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoLogiun && carga.TipoOperacao?.PossuiIntegracaoLogiun == true)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logiun, unitOfWork, false);
            else
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logiun, unitOfWork);
        }

        private static void AdicionarIntegracaoPH(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoPH)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PH, unitOfWork);
        }

        private static void AdicionarIntegracaoAngelLira(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.PossuiIntegracaoAngelLira ?? false))
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira, unitOfWork);
            else
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira, unitOfWork, false);
        }

        private static void AdicionarIntegracaoGlobus(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus integracaoGlobus = repIntegracaoGlobus.Buscar();

            if (integracaoGlobus != null && integracaoGlobus.PossuiIntegracao && (integracaoGlobus.IntegrarComEscritaFiscal))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus, unitOfWork, false);
            else
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus, unitOfWork);
        }

        private static void AdicionarIntegracaoTransSat(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTransSat repIntegracaoTransSat = new Repositorio.Embarcador.Configuracoes.IntegracaoTransSat(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTransSat integracaoTransSat = repIntegracaoTransSat.Buscar();

            if (integracaoTransSat != null && integracaoTransSat.PossuiIntegracao)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TransSat, unitOfWork, false);

        }

        private static void AdicionarIntegracaoNeokohm(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Neokohm.IntegracaoNeokohm serNeokohm = new Servicos.Embarcador.Integracao.Neokohm.IntegracaoNeokohm(unitOfWork);

            if (serNeokohm.ValidarSeDeveGerarIntegracaoNeokohm(carga))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Neokohm, unitOfWork);
        }

        private static void AdicionarIntegracaoOpentech(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            decimal valorMinimoMercadoria = carga.TipoOperacao?.ValorMinimoMercadoriaOpenTech ?? 0;

            if ((carga.TipoOperacao?.NaoIntegrarOpentech ?? true) || (carga.Veiculo?.NaoIntegrarOpentech ?? true) ||
                (valorMinimoMercadoria > 0m && valorMinimoMercadoria > repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(carga.Codigo)))
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, unitOfWork);
            else
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, unitOfWork);
        }

        //private static void AdicionarIntegracaoOpentechV10(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        //{
        //    if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTechV10))
        //        AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTechV10, unitOfWork);
        //}

        private static void AdicionarIntegracaoANTT(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.PossuiIntegracaoANTT ?? false) || !carga.FreteDeTerceiro)
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ANTT, unitOfWork);
            else
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ANTT, unitOfWork);
        }

        private static void AdicionarIntegracaoIntercab(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracaoIntercab != null && (integracaoIntercab.AtivarIntegracaoCargas || integracaoIntercab.IntegracaoDocumentacaoCarga))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab, unitOfWork);
        }

        private static void AdicionarIntegracaoPortalCabotagem(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPortalCabotagem integracaoPortalCabotagem, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracaoPortalCabotagem != null && (integracaoPortalCabotagem.AtivarEnvioXMLCTE || integracaoPortalCabotagem.AtivarEnvioPDFCTE))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PortalCabotagem, unitOfWork);
        }

        private static void AdicionarIntegracaoA52(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.PossuiIntegracaoA52 ?? false))
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52, unitOfWork);
            else
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52, unitOfWork, false);
        }

        private static void AdicionarIntegracaoCarrefour(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour, unitOfWork);
        }

        private static void AdicionarIntegracaoGoldenService(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.PossuiIntegracaoGoldenService ?? false))
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GoldenService, unitOfWork);
            else if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoGoldenService)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GoldenService, unitOfWork, false);
        }

        private static void AdicionarIntegracaoKrona(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Krona))
            {
                if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoKrona)
                {

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork).ExistePorCargaEResponsavelSeguro(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador))
                        AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Krona, unitOfWork);
                }
            }
        }

        private static void AdicionarIntegracaoBuonny(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny) && (!carga.TipoOperacao?.NaoGerarMonitoramento ?? true))
            {
                if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenBuonny))
                    AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny, unitOfWork, false);
            }
        }

        private static void AdicionarIntegracaoInfolog(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Infolog))
            {
                if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoInfolog)
                    AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Infolog, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoSaintGobain(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            bool tipoOperacaoPermiteIntegrarSaintGobain = !(carga.TipoOperacao?.SelecionarRetiradaProduto ?? false);
            if (carga?.Empresa?.EmpresaPropria ?? false)
                tipoOperacaoPermiteIntegrarSaintGobain = false;

            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoSaintGobain && tipoOperacaoPermiteIntegrarSaintGobain)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain, unitOfWork);
        }

        private static void AdicionarIntegracaoNOX(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.PossuiIntegracaoNOX ?? false))
            {
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX, unitOfWork);

                return;
            }

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            if (!(carga.TipoOperacao?.IntegrarPreSMNOX ?? false) && carga.TipoOperacao.ValorMinimoMercadoriaNOX > 0m && carga.TipoOperacao.ValorMinimoMercadoriaNOX > repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(carga.Codigo))
                return;

            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoNOX)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX, unitOfWork, false);
        }

        private static void AdicionarIntegracaoRaster(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoRaster && carga.TipoOperacao?.PossuiIntegracaoRaster == true)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster, unitOfWork, false);
        }

        private static void AdicionarIntegracaoGPA(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoGPA)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPA, unitOfWork);
        }

        private static void AdicionarIntegracaoMagalogEscrituracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEscrituracaoMagalog))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao, unitOfWork);
        }

        private static void AdicionarIntegracaoGadle(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoGadle repositorioConfiguracaoIntegracaoGadle = new Repositorio.Embarcador.Configuracoes.IntegracaoGadle(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle configuracaoIntegracaoGadle = repositorioConfiguracaoIntegracaoGadle.Buscar();

            if (configuracaoIntegracaoGadle != null && configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoGadle && carga.Empresa?.TipoIntegracaoCarga != null && carga.Empresa?.TipoIntegracaoCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Gadle)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Gadle, unitOfWork);
            else
                RemoverCargaIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Gadle, unitOfWork);
        }

        private static void AdicionarIntegracaoPadrao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracaoTMS != null && configuracaoTMS.SistemaIntegracaoPadraoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(configuracaoTMS.SistemaIntegracaoPadraoCarga);

                if (tipoIntegracao != null && tipoIntegracao.GerarIntegracaoFechamentoCarga)
                {
                    Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Embarcador.Integracao.IntegracaoCarga(unitOfWork);
                    serIntegracaoCarga.InformarIntegracaoCarga(carga, configuracaoTMS.SistemaIntegracaoPadraoCarga, unitOfWork);
                }
            }
        }

        private static void AdicionarIntegracaoMarfrigOrdemEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MarfrigOrdemEmbarque))
            {
                if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoMarfrig)
                    AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MarfrigOrdemEmbarque, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoDPA(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPA))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPA, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoUnilever(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            //integracao do précalculo de frete
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
            {
                carga.CargaPossuiPreCalculoFrete = true;
                repCarga.Atualizar(carga);

                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoUnileverDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosTransporte))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosTransporte, unitOfWork);
        }

        private static void AdicionarIntegracaoSAPV9(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9))
            {
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);
                if (contratoFrete != null)
                    AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoSAPST(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST))
            {
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);
                if (contratoFrete != null)
                    AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoLogRisk(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LogRisk))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LogRisk, unitOfWork);
        }

        private static void AdicionarIntegracaoArcelorMittalParaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal, unitOfWork);
        }

        private static void AdicionarIntegracaoBrasilRiskSML(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoBrasilRisk)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML, unitOfWork);

        }

        private static void AdicionarIntegracaoFrotaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.GrupoPessoaPrincipal != null
                && !string.IsNullOrEmpty(carga.GrupoPessoaPrincipal.TokenIntegracaoMultiEmbarcador)
                && carga.GrupoPessoaPrincipal.HabilitarIntegracaoVeiculoMultiEmbarcador == true
                && !string.IsNullOrEmpty(carga.GrupoPessoaPrincipal.TokenIntegracaoMultiEmbarcador))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcadorFrotaCarga, unitOfWork, false);
        }

        private static void AdicionarIntegracaoMoniloc(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Moniloc))
                AdicionarIntegracaoComRestricao(carga, TipoIntegracao.Moniloc, unitOfWork, true);
        }

        private static void AdicionarIntegracaoUnileverDadoValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosValePedagio))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosValePedagio, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoApisulLog(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.ApisulLog))
                return;

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog, unitOfWork);
        }

        private static void AdicionarIntegracaoCebrace(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoSAP_API4(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.SAP_API4))
                return;

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_API4, unitOfWork);
        }

        private static void AdicionarIntegracaoMinerva(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.Minerva))
                return;

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva, unitOfWork);
        }

        private static void AdicionarIntegracaoDexco(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dexco))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dexco, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoWeberChile(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.WeberChile))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.WeberChile, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoFS(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FS))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FS, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoLactalis(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Lactalis))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Lactalis, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoGrupoSC(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GrupoSC))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GrupoSC, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoObramaxCTEs(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxCTE))
                return;

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxCTE, unitOfWork);
        }

        private static void AdicionarIntegracaoObramaxNFSe(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxNFE))
                return;

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxNFE, unitOfWork);
        }

        private static void AdicionarIntegracaoObramaxProvisao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxProvisao))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ObramaxProvisao, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoMars(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoFlora(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Flora))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Flora, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoFroggr(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Froggr, unitOfWork);
        }

        private static void AdicionarIntegracaoUnileverRelevanciaCusto(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverRelevanciaCustos))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverRelevanciaCustos, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoDigitalCom(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoTecnorisk(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTecnorisk repIntegracaoTecnorisk = new Repositorio.Embarcador.Configuracoes.IntegracaoTecnorisk(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk configTecnorisk = repIntegracaoTecnorisk.Buscar();

            if (configTecnorisk?.PossuiIntegracaoTecnorisk ?? false)
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Tecnorisk, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoBoticario(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoFrimesaValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repIntegracaoFrimesa = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa integracaoFrimesa = repIntegracaoFrimesa.Buscar();

            if (!(carga.Empresa?.CompraValePedagio ?? false))
                return;

            if (integracaoFrimesa != null)
                AdicionarIntegracaoTipoOperacaoERemetenteGrupoPessoa(carga, cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FrimesaValePedagio, unitOfWork);
        }

        private static void AdicionarIntegracaoFrimesaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repIntegracaoFrimesa = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa integracaoFrimesa = repIntegracaoFrimesa.Buscar();

            if (integracaoFrimesa != null)
                AdicionarIntegracaoTipoOperacaoERemetenteGrupoPessoa(carga, cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FrimesaFrete, unitOfWork);
        }

        private static void AdicionarIntegracaoLoggi(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoLoggi repositorioIntegracaoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggi(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi integracaoLoggi = repositorioIntegracaoLoggi.Buscar();

            if (integracaoLoggi?.PossuiIntegracao ?? false)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Loggi, unitOfWork);
        }

        private static void AdicionarIntegracaoValoresCTeLoggi(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.ValoresCTeLoggi))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi repositorioIntegracaoValoresCTeLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi integracaoValoresCTeLoggi = repositorioIntegracaoValoresCTeLoggi.Buscar();

            if (integracaoValoresCTeLoggi?.PossuiIntegracao ?? false)
                AdicionarIntegracaoComRestricao(carga, TipoIntegracao.ValoresCTeLoggi, unitOfWork);
        }

        private static void AdicionarIntegracaoCTeAnterioresLoggi(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.CTeAnterioresLoggi))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi repositorioIntegracaoCTeAnterioresLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi integracaoCTeAnterioresLoggi = repositorioIntegracaoCTeAnterioresLoggi.BuscarPrimeiroRegistro();

            if (integracaoCTeAnterioresLoggi?.PossuiIntegracao ?? false)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTeAnterioresLoggi, unitOfWork);
        }

        private static void AdicionarIntegracaoTrizyEventos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizyEventos repositorioIntegracaoTrizyEventos = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizyEventos(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizyEventos configuracaoTrizyEventos = repositorioIntegracaoTrizyEventos.BuscarPrimeiroRegistro();

            if (configuracaoTrizyEventos?.PossuiIntegracao ?? false)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyEventos, unitOfWork);
        }

        private static void AdicionarIntegracaoCTePagamentoLoggi(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi repositorioIntegracaoCTePagamentoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi integracaoCTePagamentoLoggi = repositorioIntegracaoCTePagamentoLoggi.Buscar();

            if (integracaoCTePagamentoLoggi?.PossuiIntegracao ?? false)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTePagamentoLoggi, unitOfWork);
        }

        private static void AdicionarIntegracaoSAP(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSAP repositorioIntegracaoSap = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP integracaoSap = repositorioIntegracaoSap.Buscar();

            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP) && (integracaoSap?.PossuiIntegracao ?? false))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP, unitOfWork);
            }
        }


        private static void AdicionarIntegracaoNFTP(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repositorioIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repositorioIntegracaoEMP.Buscar();

            if ((integracaoEMP?.AtivarIntegracaoNFTPEMP ?? false))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP, unitOfWork);

        }

        private static void AdicionarIntegracaoEMP(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repositorioIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repositorioIntegracaoEMP.Buscar();

            if ((integracaoEMP?.PossuiIntegracaoEMP ?? false) && (integracaoEMP?.AtivarIntegracaoCargaEMP ?? false))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP, unitOfWork);

        }

        private static void AdicionarIntegracaoEMPSIL(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repositorioIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repositorioIntegracaoEMP.Buscar();

            if ((integracaoEMP?.AtivarIntegracaoParaSILEMP ?? false) && (carga.TipoOperacao?.ConfiguracaoEMP?.AtivarIntegracaoComSIL ?? false))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP, unitOfWork);

        }

        private static void AdicionarIntegracaoBrado(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoEShip(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Eship))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Eship, unitOfWork);
        }

        private static void AdicionarIntegracaoYandeh(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Yandeh))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Yandeh, unitOfWork);
        }

        private static void AdicionarIntegracaoComprovei(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoComprovei repositorioIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei integracaoComprovei = repositorioIntegracaoComprovei.Buscar();

            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei) && (integracaoComprovei?.PossuiIntegracao ?? false))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoComproveiRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoComproveiRota repositorioIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComproveiRota(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota integracaoComprovei = repositorioIntegracaoComprovei.Buscar();

            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ComproveiRota) && (integracaoComprovei?.PossuiIntegracao ?? false))
            {
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ComproveiRota, unitOfWork);
            }
        }

        private static void AdicionarIntegracaoKMM(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM, unitOfWork, false);
        }

        private static void AdicionarIntegracaoAtlas(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoAtlas repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoAtlas(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAtlas configuracaoIntegracao = repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistro();

            if (configuracaoIntegracao?.Ativa ?? false)
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Atlas, unitOfWork);
        }

        private static void AdicionarIntegracaoCalisto(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Calisto, unitOfWork);
        }

        private static void AdicionarIntegracaoLogvett(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            if (!repCargaCIOT.ExisteCIOTPorCarga(carga.Codigo))
                return;

            AdicionarIntegracaoComRestricao(carga, TipoIntegracao.Logvett, unitOfWork);
        }

        private static void AdicionarIntegracaoElectrolux(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux, unitOfWork);
        }

        private static void AdicionarIntegracaoObramax(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Obramax, unitOfWork);
        }

        public static void AdicionarIntegracaoRouteasy(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Routeasy, unitOfWork);
        }

        private static void AdicionarIntegracaoAssai(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee))
                return;

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee, unitOfWork);
        }

        private static void AdicionarIntegracaoCassol(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.Cassol))
                return;

            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cassol, unitOfWork);
        }

        private static void AdicionarIntegracaoVedacit(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.Vedacit))
                return;

            AdicionarIntegracaoComRestricao(carga, TipoIntegracao.Vedacit, unitOfWork);
        }

        private static void AdicionarIntegracaoSistemaTransben(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSistemaTransben repIntegracaoSistemaTransben = new Repositorio.Embarcador.Configuracoes.IntegracaoSistemaTransben(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSistemaTransben integracaoSistemaTransben = repIntegracaoSistemaTransben.Buscar();

            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SistemaTransben) && (integracaoSistemaTransben?.EnviarDadosCargaParaSistemaTransben ?? false))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SistemaTransben, unitOfWork);

        }
        private static void AdicionarIntegracaoATSSmartWeb(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb repIntegracaoATSSmartWeb = new Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSSmartWeb integracaoATSSmartWeb = repIntegracaoATSSmartWeb.Buscar();

            if (integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb) && (integracaoATSSmartWeb?.PossuiIntegracao ?? false))
                AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb, unitOfWork);

        }

        private static void AdicionarIntegracaoPorGrupoMotivoChamado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao repositorioGrupoTipoChamado = new Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Chamados.Chamado existeChamado = repositorioChamado.BuscarPorCarga(carga.Codigo);

            if (existeChamado == null || existeChamado.MotivoChamado == null || existeChamado.MotivoChamado.GrupoMotivoChamado == null)
                return;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaoConfiguradasoNoGrupo = repositorioGrupoTipoChamado.BuscarTiposIntegracoesPorGrupo(existeChamado.MotivoChamado.GrupoMotivoChamado.Codigo);

            foreach (TipoIntegracao tipoIntegracao in tipoIntegracaoConfiguradasoNoGrupo)
                AdicionarIntegracaoComRestricao(carga, tipoIntegracao, unitOfWork);
        }

        private static void RemoverCargaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipo(carga.Codigo, tipoIntegracao);

            if (cargaIntegracao != null)
                repCargaIntegracao.Deletar(cargaIntegracao);
        }

        private static void AdicionarIntegracaoComRestricao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao enumTipoIntegracao, Repositorio.UnitOfWork unitOfWork, bool AvaliarIntegracoesPorTipo = true)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repConfiguracaoIntegracaoGrupoPessoas = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(enumTipoIntegracao);

            //não pode comentar isso, da problemas em várias integrações, favor reveja o seu problema na raiz
            if (tipoIntegracao == null)
                return;

            bool adicionarIntegracao = false;

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repConfiguracaoIntegracaoGrupoPessoas.BuscarPorTipoIntegracao(enumTipoIntegracao);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupadas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            if (carga.CargaAgrupada)
                cargasAgrupadas = repCarga.BuscarCargasOriginais(carga.Codigo);
            else
                cargasAgrupadas.Add(carga);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal in cargasAgrupadas)
            {
                if (integracaoGrupoPessoas.Any(o => o.Habilitado))
                {
                    if (cargaOriginal.GrupoPessoaPrincipal != null && integracaoGrupoPessoas.Where(o => o.Habilitado).Any(o => o.GrupoPessoas.Codigo == cargaOriginal.GrupoPessoaPrincipal.Codigo))
                        adicionarIntegracao = true;
                }
                else if (integracaoGrupoPessoas.Any(o => !o.Habilitado))
                {
                    if (cargaOriginal.GrupoPessoaPrincipal == null || !integracaoGrupoPessoas.Where(o => !o.Habilitado).Any(o => o.GrupoPessoas.Codigo == cargaOriginal.GrupoPessoaPrincipal.Codigo))
                        adicionarIntegracao = true;
                }
                //Sempre estava ficando True, adicionado validação só da Opentech para não impactar
                else if ((tipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech && tipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour)
                    || (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech && tipoIntegracao.IntegracaoTransportador)
                    || (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM && tipoIntegracao.IntegracaoTransportador))
                {
                    adicionarIntegracao = true;
                }

                if (adicionarIntegracao)
                    break;
            }

            if (AvaliarIntegracoesPorTipo)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga = servicoCarga.ObterTipoIntegracoesPorTipoOperacaoETipoCarga(carga, unitOfWork);
                if (integracoesPorTipoOperacaoETipoCarga.Count > 0)
                {
                    adicionarIntegracao = integracoesPorTipoOperacaoETipoCarga.Contains(enumTipoIntegracao);
                }
            }

            if (adicionarIntegracao && !repCargaIntegracao.ExistePorTipoIntegracao(carga.Codigo, tipoIntegracao.Codigo))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracao()
                {
                    Carga = carga,
                    TipoIntegracao = tipoIntegracao
                };

                repCargaIntegracao.Inserir(cargaIntegracao);
            }
        }

        private static void AdicionarIntegracaoTipoOperacaoERemetenteGrupoPessoa(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repositorioGrupoPessoasIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = null;
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao entidadeTipoIntegracao = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            if (entidadeTipoIntegracao == null)
                return;

            bool possuiIntegracaoTipoOperacao = (carga.TipoOperacao?.Integracoes.Any(o => o.Tipo == tipoIntegracao) ?? false);
            if (!possuiIntegracaoTipoOperacao)
                return;

            List<Dominio.Entidades.Cliente> tomadores = new List<Dominio.Entidades.Cliente>();
            tomadores.AddRange(cargaPedidos.Select(o => o.ObterTomador()));

            if (tomadores.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repositorioGrupoPessoasIntegracao.BuscarPorGruposPessoas(tomadores.Select(o => o.GrupoPessoas?.Codigo ?? 0).ToList());

            if (integracaoGrupoPessoas.Count == 0 || !integracaoGrupoPessoas.Any(o => o.TipoIntegracao.Tipo == tipoIntegracao))
                return;

            cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracao()
            {
                Carga = carga,
                TipoIntegracao = entidadeTipoIntegracao
            };

            repCargaIntegracao.Inserir(cargaIntegracao);
        }

        private static void GerarAtendimentoParaIntegracoesPendentes(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repositorioChamado.BuscarPorIntegracao(integracaoPendente.TipoIntegracao.Codigo);

            if (motivoChamado == null)
                return;

            Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado()
            {
                Observacao = $"Atendimento gerado a partir da rejeição da integração com {integracaoPendente.TipoIntegracao?.Descricao} na carga  {integracaoPendente.Carga?.CodigoCargaEmbarcador ?? ""}",
                MotivoChamado = motivoChamado,
                Carga = integracaoPendente.Carga,
                Empresa = integracaoPendente.Carga.Empresa,
                Cliente = integracaoPendente.Carga.Pedidos?.FirstOrDefault()?.Pedido?.Remetente,
                TipoCliente = configuracaoTMS.ChamadoOcorrenciaUsaRemetente ? Dominio.Enumeradores.TipoTomador.Remetente : Dominio.Enumeradores.TipoTomador.Destinatario
            };

            Dominio.Entidades.Usuario usuario = integracaoPendente.Carga.Motoristas.FirstOrDefault();

            if (usuario == null)
                return;

            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = Servicos.Embarcador.Chamado.Chamado.AbrirChamado(objetoChamado, usuario, 0, null, unitOfWork);
        }

        private static void AdicionarCargaDadosTransporteParaIntegracaoPorPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool integracaoColeta, bool naoAvancarRejeicaoIntegracaoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> cargaDadosTransporteIntegracoes = repCargaDadosTransporteIntegracao.BuscarPedidosPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao.Codigo, integracaoColeta);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaDadosTransporteIntegracoes.Any(o => o.CargaPedido.Codigo == cargaPedido.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao()
                {
                    Carga = carga,
                    CargaPedido = cargaPedido,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = "",
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = tipoIntegracao,
                    IntegracaoColeta = integracaoColeta
                };

                repCargaDadosTransporteIntegracao.Inserir(cargaDadosTransporteIntegracao);
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || tipoIntegracao.BloquearEtapaTransporteSeRejeitar)
                carga.AguardarIntegracaoDadosTransporte = true;

            carga.AguardarIntegracaoEtapaTransportador = naoAvancarRejeicaoIntegracaoTransportador;

            repCarga.Atualizar(carga);
        }

        private static void AdicionarIntegracaoATSLog(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracaoComRestricao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSLog, unitOfWork);
        }

        private static void AdicionarIntegracaoVector(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.Vector))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoVector repositorioIntegracaoVector = new Repositorio.Embarcador.Configuracoes.IntegracaoVector(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVector configuracaoIntegracaoVector = repositorioIntegracaoVector.BuscarPrimeiroRegistro();

            if (!(configuracaoIntegracaoVector?.PossuiIntegracao ?? false))
                return;

            AdicionarIntegracaoComRestricao(carga, TipoIntegracao.Vector, unitOfWork);
        }

        private static void AdicionarIntegracaoSenior(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Senior))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSenior repositorioIntegracaoSenior = new Repositorio.Embarcador.Configuracoes.IntegracaoSenior(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSenior configuracaoIntegracaoSenior = repositorioIntegracaoSenior.BuscarPrimeiroRegistro();

            if (!(configuracaoIntegracaoSenior?.PossuiIntegracao ?? false))
                return;

            AdicionarIntegracaoComRestricao(carga, TipoIntegracao.Senior, unitOfWork);
        }

        private static void AdicionarIntegracaoYMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<TipoIntegracao> integracoesPorTipoOperacaoEFilial, Repositorio.UnitOfWork unitOfWork)
        {
            if (!integracoesPorTipoOperacaoEFilial.Contains(TipoIntegracao.YMS))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoYMS repositorioIntegracaoYMS = new Repositorio.Embarcador.Configuracoes.IntegracaoYMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYMS configuracaoIntegracaoYMS = repositorioIntegracaoYMS.BuscarPrimeiroRegistroAsync().Result;

            if (!(configuracaoIntegracaoYMS?.PossuiIntegracao ?? false))
                return;

            AdicionarIntegracaoComRestricao(carga, TipoIntegracao.YMS, unitOfWork);
        }

        public static void AdicionarIntegracaoSkyMark(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSkyMark repositorioIntegracaoSkyMark = new Repositorio.Embarcador.Configuracoes.IntegracaoSkyMark(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSkyMark configuracaoIntegracaoSkyMark = repositorioIntegracaoSkyMark.BuscarPrimeiroRegistro();

            if (!(configuracaoIntegracaoSkyMark?.HabilitarIntegracao ?? false))
                return;

            AdicionarIntegracaoComRestricao(carga, TipoIntegracao.Skymark, unitOfWork);
        }

        #endregion Métodos Privados Estáticos
    }
}