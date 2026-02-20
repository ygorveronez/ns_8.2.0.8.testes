using AdminMultisoftware.Dominio.Entidades.Pessoas;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCargaCancelamento : ServicoBase
    {
        #region Construtores        

        public IntegracaoCargaCancelamento() : base() { }
        public IntegracaoCargaCancelamento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public IntegracaoCargaCancelamento(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, ClienteURLAcesso clienteURLAcesso, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, clienteURLAcesso, cancelationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public void IniciarIntegracoesComDocumentos(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao> cargaCancelamentosIntegracao = repositorioCargaCancelamentoIntegracao.BuscarPorCargaCancelamento(cargaCancelamento.Codigo);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao in cargaCancelamentosIntegracao)
            {
                //cada tipo de integração deve adicionar os documentos necessários nas filas
                switch (cargaCancelamentoIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logiun:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Totvs:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MundialRisk:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPA:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Infolog:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Krona:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAD:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Tecnorisk:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.P44:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vector:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FS:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cassol:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars:
                        AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour:
                        if (new Carrefour.IntegracaoCarrefour(unitOfWork).PossuiIntegracaoCancelamentoCarga(cargaCancelamento.Carga))
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk:
                        AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML:
                        bool possuiIntegracaoCancelamentoBRK = new BrasilRisk.IntegracaoBrasilRisk(unitOfWork).PossuiIntegracaoCancelamentoCarga(cargaCancelamento.Carga);

                        if (cargaCancelamento.Carga.TipoOperacao?.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista == true && cargaCancelamento.Carga.ModeloVeicularCarga?.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoNaCarga == true && !possuiIntegracaoCancelamentoBRK)
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig:
                        if (cargaCancelamento.Carga.CargaDePreCarga)
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST:
                        AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                        AdicionarCargaCancelamentoParaIntegracaoIndividual(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado:
                        AdicionarCargaCancelamentoParaIntegracaoIndividual(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior:
                        if (!cargaCancelamento.DuplicarCarga || (cargaCancelamento.Carga.IntegracoesAvon.Any() && !cargaCancelamento.Carga.CargaTransbordo && cargaCancelamento.Carga.DataFinalizacaoEmissao.HasValue))
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        else if (!cargaCancelamento.Carga.CargaTransbordo && cargaCancelamento.Carga.DataFinalizacaoEmissao.HasValue)
                            AdicionarCargaCancelamentoParaIntegracaoIndividual(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX:
                        if (!cargaCancelamento.Carga.CargaTransbordo && cargaCancelamento.Carga.DataFinalizacaoEmissao.HasValue)
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                        List<TipoCancelamentoCargaDocumento> tiposPermitidos = new List<TipoCancelamentoCargaDocumento>() { TipoCancelamentoCargaDocumento.TodosDocumentos, TipoCancelamentoCargaDocumento.Carga };
                        if (tiposPermitidos.Contains((cargaCancelamento.Carga.TipoOperacao?.ConfiguracaoCarga?.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga)))
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverCancelamento:
                        GerarIntegracaoPorStage(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato:
                        AdicionarCargaCancelamentoParaIntegracaoPorPedidos(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, cargasPedido, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba:
                        if (cargaCancelamento.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos || cargaCancelamento.Carga.SituacaoCarga == SituacaoCarga.EmTransporte)
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace:
                        if ((new Servicos.Embarcador.Integracao.Cebrace.IntegracaoCebrace(unitOfWork)).ExisteIntegracao())
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_API4:
                        AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        if (new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).BuscarSituacaoIntegracaoCargaKMM(cargaCancelamento.Carga) == SituacaoIntegracao.Integrado)
                            AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        AdicionarCargaCancelamentoParaIntegracaoIndividual(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork, true, new List<string> { "CT-e", "NFS-e", "NFS", "ND" }, true);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        AdicionarCargaCancelamentoParaIntegracaoIndividual(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork, false, null);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb:
                        AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS:
                        AdicionarCargaCancelamentoParaIntegracao(cargaCancelamento, cargaCancelamentoIntegracao.TipoIntegracao, unitOfWork);
                        break;

                }
            }
        }

        public void IniciarIntegracoesComEDI(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            IntegracaoEDI.AdicionarEDIParaIntegracao(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
        }

        public static bool AdicionarIntegracoesCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntercab.BuscarIntegracao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga = servicoCarga.ObterTipoIntegracoesPorTipoOperacaoETipoCarga(cargaCancelamento.Carga, unidadeDeTrabalho);

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech))
            {
                AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, unidadeDeTrabalho, true);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb))
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSSmartWeb configuracaoIntegracao = repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistro();

                if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracao && integracoesPorTipoOperacaoETipoCarga.Contains(TipoIntegracao.ATSSmartWeb))
                {
                    Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);
                    Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unidadeDeTrabalho);

                    Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargasIntegracao = repositorioCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb);
                    Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao CargaDadosTransporteIntegracao = repositorioCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb);

                    if ((cargaCargasIntegracao != null && (cargaCargasIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado))
                         || (CargaDadosTransporteIntegracao != null && (CargaDadosTransporteIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)))
                    {
                        AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb, unidadeDeTrabalho);
                        cargaCancelamento.GerouIntegracao = true;
                    }
                }
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig))
            {
                if ((!cargaCancelamento.DuplicarCarga || (!cargaCancelamento.Carga.CargaDePreCargaFechada && !cargaCancelamento.Carga.CargaDePreCargaEmFechamento)) && new Marfrig.IntegracaoOrdemEmbarqueMarfrig(unidadeDeTrabalho).PossuiOrdemEmbarque(cargaCancelamento.Carga))
                {
                    AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig, unidadeDeTrabalho);
                    cargaCancelamento.GerouIntegracao = true;
                }
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy))
            {

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repCargaPedido.PossuiIntegracaoPedido(cargaCancelamento.Carga.Codigo))
                {
                    AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, unidadeDeTrabalho);
                    cargaCancelamento.GerouIntegracao = true;
                }//Gera Integração para alcelormittal e não para os outro 
                else if (!string.IsNullOrWhiteSpace(cargaCancelamento.Carga.IDIdentificacaoTrizzy) && (configuracaoTMS?.UtilizaAppTrizy ?? false))
                {
                    AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, unidadeDeTrabalho);
                    cargaCancelamento.GerouIntegracao = true;
                }
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato))
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracao = repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistro();

                if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.URLEnvioCancelamentoCarga))
                {
                    Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);
                    Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargasIntegracao = repositorioCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato);
                    Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao CargaDadosTransporteIntegracao = repositorioCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato);

                    if ((cargaCargasIntegracao != null && (cargaCargasIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado || !string.IsNullOrEmpty(cargaCargasIntegracao.Protocolo) || !string.IsNullOrEmpty(cargaCargasIntegracao.PreProtocolo)))
                         || (CargaDadosTransporteIntegracao != null && (CargaDadosTransporteIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado || !string.IsNullOrEmpty(CargaDadosTransporteIntegracao.Protocolo) || !string.IsNullOrEmpty(CargaDadosTransporteIntegracao.PreProtocolo))))
                    {
                        AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato, unidadeDeTrabalho);
                        cargaCancelamento.GerouIntegracao = true;
                    }
                }
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Totvs))
            {
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.Buscar();
                if (configuracaoIntegracao != null && configuracaoIntegracao.PossuiIntegracaoDeTotvs && repCargaCTe.PossuiCTeIntegradoComTotvs(cargaCancelamento.Carga.Codigo))
                {
                    AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Totvs, unidadeDeTrabalho);
                    cargaCancelamento.GerouIntegracao = true;
                }
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.P44))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.P44, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk) && repCargaCargaIntegracao.ExisteProtocoloPorCargaETipoIntegracao(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba) && (cargaCancelamento.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos || cargaCancelamento.Carga.SituacaoCarga == SituacaoCarga.EmTransporte))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (cargaCancelamento.Carga.CargaRecebidaDeIntegracao != true && (integracaoIntercab?.AtivarIntegracaoCancelamentoCarga ?? false))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (integracaoEMP?.AtivarIntegracaoCancelamentoCargaEMP ?? false)
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (integracaoEMP?.AtivarIntegracaoNFTPEMP ?? false)
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAD))
            {
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoSAD repositorioSAD = new Repositorio.Embarcador.Configuracoes.IntegracaoSAD(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

                bool gerarIntegracoes = false;

                List<int> codigosCentrosDescarregamento = cargaJanelaDescarregamento?.CentroDescarregamento != null ? new List<int> { cargaJanelaDescarregamento.CentroDescarregamento.Codigo } : new List<int>();
                List<(string URL, int CodigoCentroDescarregamento)> urlsSad = new List<(string URL, int CodigoCentroDescarregamento)>();

                if (codigosCentrosDescarregamento.Count > 0)
                    urlsSad = repositorioSAD.BuscarURLsCancelarAgendaPorCentrosDescarregamento(codigosCentrosDescarregamento);

                string urlSADCentroDescarregamento = cargaJanelaDescarregamento?.CentroDescarregamento != null ? (from obj in urlsSad where obj.CodigoCentroDescarregamento == cargaJanelaDescarregamento.CentroDescarregamento.Codigo select obj.URL).FirstOrDefault() : "";

                if (string.IsNullOrWhiteSpace(urlSADCentroDescarregamento))
                    urlSADCentroDescarregamento = (from obj in urlsSad where obj.CodigoCentroDescarregamento == 0 select obj.URL).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(urlSADCentroDescarregamento))
                    gerarIntegracoes = true;

                if (gerarIntegracoes)
                {
                    AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAD, unidadeDeTrabalho);
                    cargaCancelamento.GerouIntegracao = true;
                }
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cassol))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cassol, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }


            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus))
            {
                AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus, unidadeDeTrabalho);
                cargaCancelamento.GerouIntegracao = true;
            }

            AdicionarIntegracaoCancelamentoPorTipoIntegracaoRestricaoTipoOperacao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverCancelamento, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster, unidadeDeTrabalho, true);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_API4, unidadeDeTrabalho, true);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS, unidadeDeTrabalho, true);
            AdicionarIntegracaoCancelamentoPorIntegracaoComSucessoExistenteCarga(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vector, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoComSucessoExistenteCarga(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FS, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoIntegracaoHUB(cargaCancelamento, unidadeDeTrabalho);

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCargaPermitemAdicionarIntegracao = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                };

                if (!situacoesCargaPermitemAdicionarIntegracao.Contains(cargaCancelamento.SituacaoCargaNoCancelamento))
                    return cargaCancelamento.GerouIntegracao;
            }

            AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorTipoIntegracao(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado, unidadeDeTrabalho);

            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Infolog, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Krona, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Tecnorisk, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCarga(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCarga(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao, unidadeDeTrabalho);
            AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCarga(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPA, unidadeDeTrabalho);
            if (repCargaCargaIntegracao.ExistePorCargaETipo(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST))
                AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCarga(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST, unidadeDeTrabalho);
            if (repCargaCargaIntegracao.ExistePorCargaETipo(cargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9))
                AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCarga(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9, unidadeDeTrabalho);

            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = null;

                if (cargaCancelamento.Carga.TipoOperacao != null && cargaCancelamento.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    if (cargaCancelamento.Carga.TipoOperacao.TipoIntegracao != null && cargaCancelamento.Carga.TipoOperacao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && repositorioCargaCancelamentoIntegracao.ContarPorCancelamentoETipoIntegracao(cargaCancelamento.Codigo, cargaCancelamento.Carga.TipoOperacao.TipoIntegracao.Tipo) <= 0)
                        tipoIntegracao = cargaCancelamento.Carga.TipoOperacao.TipoIntegracao;
                }
                else if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                    {
                        if (tomador.TipoIntegracao != null && tomador.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && repositorioCargaCancelamentoIntegracao.ContarPorCancelamentoETipoIntegracao(cargaCancelamento.Codigo, tomador.TipoIntegracao.Tipo) <= 0)
                            tipoIntegracao = tomador.TipoIntegracao;
                    }
                    else if (tomador.GrupoPessoas.TipoIntegracao != null && tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && repositorioCargaCancelamentoIntegracao.ContarPorCancelamentoETipoIntegracao(cargaCancelamento.Codigo, tomador.GrupoPessoas.TipoIntegracao.Tipo) <= 0)
                        tipoIntegracao = tomador.GrupoPessoas.TipoIntegracao;
                }

                if (tipoIntegracao != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao()
                    {
                        CargaCancelamento = cargaCancelamento,
                        TipoIntegracao = tipoIntegracao
                    };

                    repositorioCargaCancelamentoIntegracao.Inserir(cargaCancelamentoIntegracao);
                }
            }

            cargaCancelamento.GerouIntegracao = true;

            return true;
        }

        public static void AdicionarIntegracaoCancelamentoPorTipoIntegracaoRestricaoTipoOperacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            if (configuracaoGeralCarga == null || configuracaoGeralCarga.TipoIntegracaoCancelamentoPadrao == null)
                return;

            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao()
            {
                CargaCancelamento = cargaCancelamento,
                TipoIntegracao = configuracaoGeralCarga.TipoIntegracaoCancelamentoPadrao
            };
            repositorioCargaCancelamentoIntegracao.Inserir(cargaCancelamentoIntegracao);

        }

        public static void AdicionarIntegracaoCancelamentoPorTipoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCancelamento = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            if (tipoIntegracaoCancelamento == null)
                return;

            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao()
            {
                CargaCancelamento = cargaCancelamento,
                TipoIntegracao = tipoIntegracaoCancelamento
            };
            cargaCancelamento.GerouIntegracao = true;
            repositorioCargaCancelamentoIntegracao.Inserir(cargaCancelamentoIntegracao);
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            IntegracaoEDI servicoIntegracaoEDI = new IntegracaoEDI(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> cancelamentos = await servicoIntegracaoEDI.VerificarIntegracoesPendentesCargaCancelamentoAsync(_unitOfWork);
            cancelamentos.AddRange(VerificarCargaIntegracaoPendentes(_unitOfWork, _tipoServicoMultisoftware, _clienteURLAcesso));

            IntegracaoCTe.VerificarIntegracoesPendentesCargaCancelamento(_unitOfWork, _clienteURLAcesso);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static void AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaFrete(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, TipoIntegracao enumTipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(enumTipoIntegracao);

            if (tipoIntegracao == null)
                return;

            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao = repositorioCargaFreteIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamento.Carga.Codigo, enumTipoIntegracao);

            if (cargaFreteIntegracao == null || cargaFreteIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                return;

            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao()
            {
                CargaCancelamento = cargaCancelamento,
                TipoIntegracao = tipoIntegracao
            };

            cargaCancelamento.GerouIntegracao = true;

            repositorioCargaCancelamentoIntegracao.Inserir(cargaCancelamentoIntegracao);
        }

        private static void AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao enumTipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaCancelamento.Carga.Integracoes == null || cargaCancelamento.Carga.Integracoes.Count <= 0)
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = cargaCancelamento.Carga.Integracoes.Where(o => o.TipoIntegracao.Tipo == enumTipoIntegracao).Select(o => o.TipoIntegracao).FirstOrDefault();

            if (tipoIntegracao == null)
                return;

            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao()
            {
                CargaCancelamento = cargaCancelamento,
                TipoIntegracao = tipoIntegracao
            };

            repositorioCargaCancelamentoIntegracao.Inserir(cargaCancelamentoIntegracao);
        }

        private static void AdicionarIntegracaoCancelamentoIntegracaoHUB(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas repositorioCargaIntegracaoHUB = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas integracao = repositorioCargaIntegracaoHUB.ConsultarIntegracaoCargaEnviadaHUB(cargaCancelamento.Carga.Codigo).GetAwaiter().GetResult();

            if (integracao == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.HUB);

            if (tipoIntegracao == null)
                return;

            cargaCancelamento.GerouIntegracao = true;

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas()
            {
                Carga = cargaCancelamento.Carga,
                TipoIntegracao = tipoIntegracao,
                TipoEnvioHUBOfertas = TipoEnvioHUBOfertas.CancelamentoDemandaOferta,
                ProblemaIntegracao = "",
                DataIntegracao = DateTime.Now,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
            };

            repositorioCargaIntegracaoHUB.Inserir(cargaIntegracao);
        }


        private static void AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao enumTipoIntegracao, Repositorio.UnitOfWork unitOfWork, bool validaIntegracaoDadosDaCarga = false)
        {
            Servicos.Log.GravarInfo("AdicionarIntegracaoCancelamentoPorIntegracaoExistenteCargaComProtocolo");

            if (cargaCancelamento == null || cargaCancelamento.Carga == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = null;

            if (cargaCancelamento.Carga.CargaIntegracoes == null || cargaCancelamento.Carga.CargaIntegracoes.Count() <= 0)
            {
                if (!validaIntegracaoDadosDaCarga)
                    return;
            }
            else
                tipoIntegracao = cargaCancelamento.Carga.CargaIntegracoes.Where(o => o.TipoIntegracao.Tipo == enumTipoIntegracao && !string.IsNullOrWhiteSpace(o.Protocolo)).Select(o => o.TipoIntegracao).FirstOrDefault();


            if (validaIntegracaoDadosDaCarga && tipoIntegracao == null)
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                tipoIntegracao = repCargaDadosTransporteIntegracao.ExisteComProtocoloPorCargaETipoIntegracao(cargaCancelamento.Carga.Codigo, enumTipoIntegracao);
            }

            if (tipoIntegracao == null)
                return;

            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao()
            {
                CargaCancelamento = cargaCancelamento,
                TipoIntegracao = tipoIntegracao
            };
            cargaCancelamento.GerouIntegracao = true;
            repositorioCargaCancelamentoIntegracao.Inserir(cargaCancelamentoIntegracao);
        }

        private void AdicionarCargaCancelamentoParaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unidadeTrabalho);

            if (repCargaCancelamentoCargaIntegracao.ContarPorCargaCancelamentoETipo(cargaCancelamento.Codigo, tipoIntegracao.Codigo) > 0)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao();

            cargaCancelamentoCargaIntegracao.CargaCancelamento = cargaCancelamento;
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas = 0;
            cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "";
            cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            cargaCancelamentoCargaIntegracao.TipoIntegracao = tipoIntegracao;

            repCargaCancelamentoCargaIntegracao.Inserir(cargaCancelamentoCargaIntegracao);
        }

        public void AdicionarCargaCancelamentoParaIntegracaoPorPedidos(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> cargaCancelamentoCargaIntegracoes = repositorioCargaCancelamentoCargaIntegracao.BuscarPedidosPorCargaETipoIntegracao(cargaCancelamento.Carga.Codigo, tipoIntegracao.Codigo);

            if (repositorioCargaCancelamentoCargaIntegracao.ContarPorCargaCancelamentoETipo(cargaCancelamento.Codigo, tipoIntegracao.Codigo) > 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaCancelamentoCargaIntegracoes.Any(o => o.CargaPedido.Codigo == cargaPedido.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao();

                cargaCancelamentoCargaIntegracao.CargaCancelamento = cargaCancelamento;
                cargaCancelamentoCargaIntegracao.CargaPedido = cargaPedido;
                cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaIntegracao.NumeroTentativas = 0;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "";
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                cargaCancelamentoCargaIntegracao.TipoIntegracao = tipoIntegracao;

                repositorioCargaCancelamentoCargaIntegracao.Inserir(cargaCancelamentoCargaIntegracao);
            }
        }

        private void AdicionarCargaCancelamentoParaIntegracaoIndividual(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho, bool considerarDocumentosAnulados = true, List<string> modelosPermitidos = null, bool validarAutorizacaoDocumento = false)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);

            string[] status;
            List<string> statusList = null;

            if (!cargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador && repCargaPedido.ExisteCTeEmitidoNoEmbarcador(cargaCancelamento.Carga.Codigo))
                statusList = new List<string> { "A", "C" };
            else
                statusList = new List<string> { "C" };

            if (considerarDocumentosAnulados)
                statusList.Add("Z");

            status = statusList.ToArray();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            cargaCTes = repCargaCTe.BuscarPorCargaEStatusSemComplementares(cargaCancelamento.Carga.Codigo, status);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                if (repCargaCancelamentoCargaCTeIntegracao.ExistePorCargaCTeETipoIntegracao(cargaCTe.Codigo, tipoIntegracao.Codigo))
                    continue;

                if (modelosPermitidos != null)
                {
                    if (!modelosPermitidos.Any(x => x == (cargaCTe?.CTe?.ModeloDocumentoFiscal?.Abreviacao ?? "")))
                        continue;
                }
                if (validarAutorizacaoDocumento)
                {
                    if (cargaCTe.CTe.Status.Equals("Z") && cargaCTe.CTe.DataAutorizacao == null)
                        continue;
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao()
                {
                    CargaCancelamento = cargaCancelamento,
                    CargaCTe = cargaCTe,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = string.Empty,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = tipoIntegracao
                };

                repCargaCancelamentoCargaCTeIntegracao.Inserir(cargaCancelamentoCargaCTeIntegracao);
            }
        }

        private static void AdicionarIntegracaoCancelamentoPorIntegracaoComSucessoExistenteCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao enumTipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaCancelamento.Carga.Integracoes == null || cargaCancelamento.Carga.Integracoes.Count <= 0)
                return;

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = null;

            if (cargaCancelamento.Carga.CargaIntegracoes != null && cargaCancelamento.Carga.CargaIntegracoes.Count > 0)
                tipoIntegracao = cargaCancelamento.Carga.CargaIntegracoes.Where(o => o.TipoIntegracao.Tipo == enumTipoIntegracao && o.SituacaoIntegracao == SituacaoIntegracao.Integrado).Select(o => o.TipoIntegracao).FirstOrDefault();

            if (tipoIntegracao == null)
                tipoIntegracao = repCargaDadosTransporteIntegracao.ExistePorCargaTipoIntegracaoESituacao(cargaCancelamento.Carga.Codigo, enumTipoIntegracao, SituacaoIntegracao.Integrado);

            if (tipoIntegracao == null)
                return;

            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao cargaCancelamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao()
            {
                CargaCancelamento = cargaCancelamento,
                TipoIntegracao = tipoIntegracao
            };

            cargaCancelamento.GerouIntegracao = true;
            repositorioCargaCancelamentoIntegracao.Inserir(cargaCancelamentoIntegracao);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> VerificarCargaIntegracaoPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso)
        {
            unitOfWork.FlushAndClear();

            //todo: ver a possibilidade de tornar dinamico;
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> cargasCancelamentoCargaIntegracao = repCargaCancelamentoCargaIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> cargasCancelamento = (from obj in cargasCancelamentoCargaIntegracao select obj.CargaCancelamento).Distinct().ToList();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Integracao.Totvs.Movimento svsMovimentoTotvs = new Servicos.Embarcador.Integracao.Totvs.Movimento();
            Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab servicoIntegracaoIntercab = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente in cargasCancelamentoCargaIntegracao)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MundialRisk:
                        Servicos.Embarcador.Integracao.MundialRisk.IntegracaoMundialRisk.CancelarCarga(integracaoPendente, unitOfWork, StringConexao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Logiun:
                        Servicos.Embarcador.Integracao.Logiun.IntegracaoLogiun.CancelarCarga(integracaoPendente, unitOfWork, integracaoPendente.CargaCancelamento.MotivoCancelamento, integracaoPendente.CargaCancelamento.DataCancelamento.Value);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                        if ((configuracaoTMS?.UtilizaAppTrizy ?? false))
                            Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AtualizarViagem(integracaoPendente.CargaCancelamento.Carga, "CANCELED", unitOfWork, integracaoPendente, atualizarCargaAntiga: true);
                        else
                            Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarApiIsRomaneio(integracaoPendente, unitOfWork, integracaoPendente.CargaCancelamento.MotivoCancelamento);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyContrato:
                        new Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Totvs:
                        svsMovimentoTotvs.IntegrarCancelamento(integracaoPendente, unitOfWork, integracaoPendente.CargaCancelamento.MotivoCancelamento);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        servicoIntegracaoIntercab.IntegrarCancelamento(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX:
                        Servicos.Embarcador.Integracao.AX.IntegracaoAX.IntegrarCancelamentoCTe(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:
                        Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.CancelarCarga(integracaoPendente, unitOfWork, StringConexao, tipoServicoMultisoftware);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trafegus:
                        new Servicos.Embarcador.Integracao.Trafegus.IntegracaoCargaTrafegus(unitOfWork, tipoServicoMultisoftware).CancelarCarga(integracaoPendente, StringConexao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior:
                        Servicos.Embarcador.Integracao.Avior.IntegracaoAvior.CancelarCarga(integracaoPendente, unitOfWork, StringConexao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NOX:
                        Servicos.Embarcador.Integracao.NOX.IntegracaoNOX.IntegrarCancelamentoCarga(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva:
                        new Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva(_unitOfWork).CancelarCargaAsync(integracaoPendente).GetAwaiter().GetResult();
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPA:
                        new Servicos.Embarcador.Integracao.DPA.IntegracaoDPA(unitOfWork).CancelarCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe:
                        Servicos.Embarcador.Integracao.GPA.IntegracaoGPA.CancelarCarga(integracaoPendente, unitOfWork, StringConexao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster:
                        Servicos.Embarcador.Integracao.Raster.IntegracaoRaster.CancelarCarga(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech:
                        new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Infolog:
                        new Servicos.Embarcador.Integracao.Infolog.IntegracaoInfolog(unitOfWork, tipoServicoMultisoftware).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny:
                        new Servicos.Embarcador.Integracao.Buonny.SolicitacaoMonitoramento(unitOfWork, tipoServicoMultisoftware).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Krona:
                        new Servicos.Embarcador.Integracao.Krona.IntegracaoKrona(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte:
                        new Servicos.Embarcador.Integracao.TelhaNorte.IntegracaoTelhaNorte(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao:
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.CancelarCargaEscrituracao(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig:
                        new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork).IntegrarCargaCancelamentoOrdemEmbarque(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour:
                        new Servicos.Embarcador.Integracao.Carrefour.IntegracaoCarrefour(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52:
                        new Servicos.Embarcador.Integracao.A52.IntegracaoA52(unitOfWork).CancelarCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAD:
                        new SAD.IntegracaoSAD(unitOfWork, tipoServicoMultisoftware).CancelarAgendamento(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverCancelamento:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarCancelamentoCargaCte(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Tecnorisk:
                        new Servicos.Embarcador.Integracao.Tecnorisk.IntegracaoTecnorisk(unitOfWork).CancelarSolicitacaoMonitoramento(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog:
                        new Servicos.Embarcador.Integracao.ApisulLog.IntegracaoApisulLog(unitOfWork).IntegrarCancelaSMP(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarCargaCancelamento(integracaoPendente, clienteURLAcesso?.URLAcesso ?? "");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                        new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarCargaCancelamento(integracaoPendente, clienteURLAcesso?.URLAcesso ?? "");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCargaCancelamento(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk:
                        Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.IntegrarCargaCancelamento(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML:
                        Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.IntegrarCargaCancelamento(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.P44:
                        new Servicos.Embarcador.Integracao.P44.IntegracaoP44(unitOfWork).IntegrarCargaCancelamento(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba:
                        new Servicos.Embarcador.Integracao.Piracanjuba.IntegracaoPiracanjuba(unitOfWork, tipoServicoMultisoftware).IntegrarCargaCancelamento(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9:
                        new Servicos.Embarcador.Integracao.SAPV9.IntegracaoSAPV9(unitOfWork).IntegrarCancelamentoV9(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST:
                        new Servicos.Embarcador.Integracao.SAPST.IntegracaoSAPST(unitOfWork).IntegrarCancelamentoST(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace:
                        new Servicos.Embarcador.Integracao.Cebrace.IntegracaoCebrace(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_API4:
                        new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarDadosCargaCancelamento(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Vector:
                        new Servicos.Embarcador.Integracao.Vector.IntegracaoVector(unitOfWork).IntegrarCargaCancelamentoRecebimentoViagemStatus(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FS:
                        new Servicos.Embarcador.Integracao.FS.IntegracaoFS(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cassol:
                        new Servicos.Embarcador.Integracao.Cassol.IntegracaoCassol(unitOfWork).IntegrarCancelamentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ATSSmartWeb:
                        new Servicos.Embarcador.Integracao.ATSSmartWeb.IntegracaoATSSmartWeb(unitOfWork).IntegrarCargaCancelamento(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mars:
                        new Servicos.Embarcador.Integracao.Mars.IntegracaoMars(unitOfWork).IntegrarCargaCancelamento(integracaoPendente).GetAwaiter().GetResult();
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS:
                        new Servicos.Embarcador.Integracao.YMS.IntegracaoYMS(unitOfWork).IntegracaoCancelarCarga(integracaoPendente).ConfigureAwait(false).GetAwaiter().GetResult();
                        break;
                    default:
                        break;
                }
            }

            return cargasCancelamento;
        }

        private void GerarIntegracaoPorStage(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repostiorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPeidoXmlNotaFiscal = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);

            List<int> stageAGerarIntegracao = new List<int>();

            if (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos && cargaCancelamento.CTe != null)
                stageAGerarIntegracao.AddRange(repositorioCargaPeidoXmlNotaFiscal.BuscarStagePorCodigoCTe(new List<int>() { cargaCancelamento.CTe.Codigo }, cargaCancelamento.Carga.Codigo));
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repostiorioStage.BuscarStagesPorCargaSemPercuso(cargaCancelamento.Carga.Codigo);
                stageAGerarIntegracao.AddRange(stages.Select(x => x.Codigo).Distinct().ToList());
            }

            foreach (var codigoStage in stageAGerarIntegracao)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao();
                cargaCancelamentoCargaIntegracao.CargaCancelamento = cargaCancelamento;
                cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaIntegracao.NumeroTentativas = 0;
                cargaCancelamentoCargaIntegracao.Stage = new Stage() { Codigo = codigoStage };
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "";
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                cargaCancelamentoCargaIntegracao.TipoIntegracao = tipoIntegracao;
                repCargaCancelamentoCargaIntegracao.Inserir(cargaCancelamentoCargaIntegracao);
            }
        }

        #endregion Métodos Privados
    }
}
