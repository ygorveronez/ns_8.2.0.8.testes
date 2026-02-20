using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.NFS;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoCTe : ServicoBase
    {        
        public IntegracaoCTe() : base() { }
        public IntegracaoCTe(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        public IntegracaoCTe(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken) { }

        #region Métodos Públicos

        /// <summary>
        /// adicionar os CT-es para uma fila para serem enviados individualmente para integração com o sistema emissor
        /// </summary>
        /// <param name="carga"></param>
        /// <param name="tipoIntegracao"></param>
        /// <param name="unitOfWork"></param>
        public void AdcionarCTesParaEnvioViaIntegracaoIndividual(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unidadeTrabalho);

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

            int count = 0;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCarga = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            bool apenasCTe = tipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAEscrituracaoCTe;

            List<int> codigosCargaCTes = repCargaCTe.BuscarCodigosPorCargaSemIntegracao(carga.Codigo, tipoIntegracao, apenasCTe);

            foreach (int codigoCargaCTe in codigosCargaCTes)
            {
                if (count == 25)
                {
                    count = 0;

                    unidadeTrabalho.Dispose();
                    unidadeTrabalho = new Repositorio.UnitOfWork(StringConexao);

                    repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeTrabalho);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao();
                cargaCTeIntegracao.CargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe() { Codigo = codigoCargaCTe };
                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas = 0;
                cargaCTeIntegracao.ProblemaIntegracao = "";
                cargaCTeIntegracao.TipoIntegracao = tipoIntegracaoCarga;
                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repCargaCTeIntegracao.Inserir(cargaCTeIntegracao);

                count++;
            }
        }

        public void AdcionarCTesParaEnvioViaIntegracaoIndividual(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual LancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repositorioNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unitOfWork);

            if (repositorioNFSManualCTeIntegracao.ExistePorLancamentoNFSManualETipo(LancamentoNFSManual.Codigo, tipoIntegracao))
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoLancamentoNFSManual = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);
            Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao()
            {
                LancamentoNFSManual = LancamentoNFSManual,
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracaoLancamentoNFSManual,
                ProblemaIntegracao = "",
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
            };

            repositorioNFSManualCTeIntegracao.Inserir(nfsManualCTeIntegracao);
        }

        public void AdcionarCTesParaEnvioViaIntegracaoIndividual(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

            int count = 0;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCarga = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            List<int> codigosLancamentoNFSManual = repNFSManualCancelamentoIntegracaoCTe.BuscarCodigosPorNFSManualCancelamentoSemIntegracao(nfsManualCancelamento.Codigo, tipoIntegracao, 200);

            foreach (int codigoLancamentoNFSManual in codigosLancamentoNFSManual)
            {
                if (count == 25)
                {
                    count = 0;

                    unitOfWork.FlushAndClear();
                }

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfsManualCancelamentoIntegracaoCTe = new Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe
                {
                    NFSManualCancelamento = nfsManualCancelamento,
                    LancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(codigoLancamentoNFSManual),
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = "",
                    TipoIntegracao = tipoIntegracaoCarga,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                };

                repNFSManualCancelamentoIntegracaoCTe.Inserir(nfsManualCancelamentoIntegracaoCTe);

                count++;
            }
        }

        public void AdcionarCTesParaEnvioViaIntegracaoIndividual(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork = null, bool controlarUnity = true, List<string> modelosPermitidos = null)
        {
            Repositorio.UnitOfWork unidadeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCarga = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);
            if (tipoIntegracaoCarga == null)
                return;

            List<int> codigosCargaCTes = new List<int>();

            if (ocorrencia.ValorOcorrencia > 0m || (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)) //possui complemento, então pega os CT-es complementares
                codigosCargaCTes = repCargaCTe.BuscarCodigosPorOcorrenciaSemIntegracao(ocorrencia.Codigo, tipoIntegracao, 200);
            else
                codigosCargaCTes = repOcorrenciaCTeIntegracao.BuscarCodigosCargaCTePorOcorrenciaSemIntegracao(ocorrencia.Codigo, tipoIntegracao, 200);

            List<int> cargaCTesExisteIntegracao = new List<int>();
            if (tipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.InteliPost)
                cargaCTesExisteIntegracao = repOcorrenciaCTeIntegracao.BuscarCargaCTesPorCargaCTes(codigosCargaCTes, ocorrencia.TipoOcorrencia.Codigo);

            int count = 0;
            foreach (int codigoCargaCTe in codigosCargaCTes)
            {
                if (!cargaCTesExisteIntegracao.Contains(codigoCargaCTe))
                {
                    if (controlarUnity && count == 25)
                    {
                        count = 0;

                        unidadeTrabalho.Dispose();
                        unidadeTrabalho = new Repositorio.UnitOfWork(StringConexao);

                        repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeTrabalho);
                        repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                        repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeTrabalho);
                    }

                    Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao
                    {
                        CargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe),
                        CargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(ocorrencia.Codigo),
                        TipoIntegracao = tipoIntegracaoCarga,

                        DataIntegracao = tipoIntegracao == TipoIntegracao.Frimesa ? DateTime.Now.AddHours(2) : DateTime.Now,
                        NumeroTentativas = 0,
                        ProblemaIntegracao = "",
                        SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                    };

                    if (modelosPermitidos != null)
                    {
                        if (!modelosPermitidos.Any(x => x == (ocorrenciaCTeIntegracao.CargaCTe?.CTe?.ModeloDocumentoFiscal?.Abreviacao ?? "")))
                            continue;
                    }

                    repOcorrenciaCTeIntegracao.Inserir(ocorrenciaCTeIntegracao);

                    count++;
                }
            }
        }

        public void AdcionarApenasOPrimeirCTesParaEnvioViaIntegracaoIndividual(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork = null)
        {
            Repositorio.UnitOfWork unidadeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (repOcorrenciaCTeIntegracao.ContarPorOcorrenciaETipoIntegracao(ocorrencia.Codigo, tipoIntegracao) > 0)
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCarga = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            int codigoCargaCTe = repCargaCTe.BuscarPrimeiroCodigoPorOcorrencia(ocorrencia.Codigo);
            if (codigoCargaCTe > 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao();
                ocorrenciaCTeIntegracao.CargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe() { Codigo = codigoCargaCTe };
                ocorrenciaCTeIntegracao.CargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia() { Codigo = ocorrencia.Codigo };
                ocorrenciaCTeIntegracao.DataIntegracao = tipoIntegracao == TipoIntegracao.Frimesa ? DateTime.Now.AddHours(2) : DateTime.Now;
                ocorrenciaCTeIntegracao.NumeroTentativas = 0;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "";
                ocorrenciaCTeIntegracao.TipoIntegracao = tipoIntegracaoCarga;
                ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repOcorrenciaCTeIntegracao.Inserir(ocorrenciaCTeIntegracao);
            }

        }

        public void AdcionarCTesDistintosParaEnvioViaIntegracaoIndividual(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork = null)
        {
            Repositorio.UnitOfWork unidadeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCarga = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);
            if (tipoIntegracaoCarga == null)
                return;

            List<int> codigosCargaCTes = new List<int>();

            if (ocorrencia.ValorOcorrencia > 0m || (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)) //possui complemento, então pega os CT-es complementares
                codigosCargaCTes = repCargaCTe.BuscarCodigosPorOcorrenciaSemIntegracao(ocorrencia.Codigo, tipoIntegracao, 200);
            else
                codigosCargaCTes = repOcorrenciaCTeIntegracao.BuscarCodigosCargaCTePorOcorrenciaSemIntegracao(ocorrencia.Codigo, tipoIntegracao, 200);

            List<int> ctesExisteIntegracao = new List<int>();


            int count = 0;
            foreach (int codigoCargaCTe in codigosCargaCTes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                if (cargaCTe != null && !ctesExisteIntegracao.Contains(cargaCTe.CTe.Codigo))
                {
                    if (count == 25)
                    {
                        count = 0;

                        unidadeTrabalho.Dispose();
                        unidadeTrabalho = new Repositorio.UnitOfWork(StringConexao);

                        repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeTrabalho);
                        repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeTrabalho);
                    }

                    Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao
                    {
                        CargaCTe = cargaCTe,
                        CargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(ocorrencia.Codigo),
                        TipoIntegracao = tipoIntegracaoCarga,

                        DataIntegracao = DateTime.Now,
                        NumeroTentativas = 0,
                        ProblemaIntegracao = "",
                        SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
                    };

                    repOcorrenciaCTeIntegracao.Inserir(ocorrenciaCTeIntegracao);

                    ctesExisteIntegracao.Add(cargaCTe.CTe.Codigo);
                    count++;
                }
            }

        }

        public void AdcionarApenasOPrimeirCTesParaEnvioViaIntegracaoIndividual(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (repCargaCTeIntegracao.ContarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao) > 0)
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCarga = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            int codigoCargaCTe = repCargaCTe.BuscarPrimeiroCodigoPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao();
            cargaCTeIntegracao.CargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe() { Codigo = codigoCargaCTe };
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas = 0;
            cargaCTeIntegracao.ProblemaIntegracao = "";
            cargaCTeIntegracao.TipoIntegracao = tipoIntegracaoCarga;
            cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

            repCargaCTeIntegracao.Inserir(cargaCTeIntegracao);

        }

        public static void VerificarIntegracoesPendentesCargaCancelamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURL)
        {
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 100;

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(unitOfWork);

            List<int> cargaCancelamentoCargaCTeIntegracoes = repCargaCancelamentoCargaCTeIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);

            for (int i = 0; i < cargaCancelamentoCargaCTeIntegracoes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegracao = repCargaCancelamentoCargaCTeIntegracao.BuscarPorCodigo(cargaCancelamentoCargaCTeIntegracoes[i], false);

                switch (cargaCancelamentoCargaCTeIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior:
                        Servicos.Embarcador.Integracao.Avior.IntegracaoAvior.CancelarCTe(ref cargaCancelamentoCargaCTeIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                        new Boticario.IntegracaoBoticario(unitOfWork).IntegrarCancelamentoCargaCTe(ref cargaCancelamentoCargaCTeIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado:
                        new Brado.IntegracaoBrado(unitOfWork).IntegrarCancelamentoCargaCTe(ref cargaCancelamentoCargaCTeIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                        new Servicos.Embarcador.Integracao.Camil.IntegracaoCamil(unitOfWork).IntegrarCancelamentoPagamentoCargaCTeIntegracao(cargaCancelamentoCargaCTeIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                        (new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork)).IntegrarCancelamentoCargaCTe(cargaCancelamentoCargaCTeIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                        new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarCancelamentoCargaCTe(cargaCancelamentoCargaCTeIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete:
                        new Servicos.Embarcador.Integracao.EFrete.Recebivel(unitOfWork, clienteURL).IntegrarCancelamentoCte(cargaCancelamentoCargaCTeIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCancelamentoCargaCTe(cargaCancelamentoCargaCTeIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork).IntegrarCancelamentoCargaCTe(cargaCancelamentoCargaCTeIntegracao);
                        break;
                    default:
                        break;
                }

                unitOfWork.FlushAndClear();
            }
        }

        public async Task<List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>> VerificarIntegracoesPendentesIndividuaisOcorrenciasAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia)
        {
            int numeroTentativas = configuracaoOcorrencia?.NumeroTentativasIntegracao != 0 ? configuracaoOcorrencia.NumeroTentativasIntegracao : 2;
            double minutosACadaTentativa = configuracaoOcorrencia?.IntervaloMinutosEntreIntegracoes != 0 ? configuracaoOcorrencia.IntervaloMinutosEntreIntegracoes : 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ocorrenciaCTeIntegracoesPendente = repositorioOcorrenciaCTeIntegracao.BuscarCTeIntegracaoPendente(numeroTentativas, minutosACadaTentativa, "DataIntegracao", "asc", numeroRegistrosPorVez, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Individual);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = (from obj in ocorrenciaCTeIntegracoesPendente select obj.CargaOcorrencia).Distinct().ToList();

            Servicos.Embarcador.Integracao.Intelipost.IntegracaoOcorrencia servicoIntegracaoOcorrencia = new Intelipost.IntegracaoOcorrencia(unitOfWork);
            Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog servicoIntegracaoMagalog = new Magalog.IntegracaoMagalog(unitOfWork);
            Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab servicoIntegracaoIntercab = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente in ocorrenciaCTeIntegracoesPendente)
            {
                switch (ocorrenciaCTeIntegracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await new Servicos.Embarcador.Integracao.FTP.IntegracaoFTP(unitOfWork, _tipoServicoMultisoftware, _cancellationToken).EnviarCTeAsync(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig:
                        new Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva:
                        new Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva(unitOfWork).IntegrarOcorrenciaIntegracao(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PortalCabotagem:
                        new Servicos.Embarcador.Integracao.PortalCabotagem.IntegracaoPortalCabotagem(unitOfWork).Integrar(ocorrenciaCTeIntegracaoPendente.CodigosCTes.FirstOrDefault());
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.InteliPost:
                        servicoIntegracaoOcorrencia.EnviarOcorrencia(ocorrenciaCTeIntegracaoPendente, clienteMultisoftware);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador:
                        Servicos.Embarcador.Integracao.MultiEmbarcador.Ocorrencia.IntegracarOcorrencia(ocorrenciaCTeIntegracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog:
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao:
                        servicoIntegracaoMagalog.IntegrarDocumentosEscrituracaoOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                        new Boticario.IntegracaoBoticario(unitOfWork).IntegrarCTeComplemento(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo:
                        Servicos.Embarcador.Integracao.Riachuelo.IntegracaoRiachuelo.IntegrarNFesEntregues(ocorrenciaCTeIntegracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX:
                        Servicos.Embarcador.Integracao.AX.IntegracaoAX.IntegrarComplementoCTe(ocorrenciaCTeIntegracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        servicoIntegracaoIntercab.IntegrarOcorrenciaCTe(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente, clienteURLAcesso?.URLAcesso ?? "");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                        new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente, clienteURLAcesso?.URLAcesso ?? "");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                        new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado:
                        new Servicos.Embarcador.Integracao.Brado.IntegracaoBrado(unitOfWork).IntegrarCargaCteOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                        new Servicos.Embarcador.Integracao.Camil.IntegracaoCamil(unitOfWork).IntegrarCargaOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frimesa:
                        if (DateTime.Now >= ocorrenciaCTeIntegracaoPendente.CargaCTe?.CTe?.DataAutorizacao?.AddHours(2))
                            new Servicos.Embarcador.Integracao.Frimesa.IntegracaoFrimesa(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete:
                        new Servicos.Embarcador.Integracao.EFrete.Recebivel(unitOfWork, clienteURLAcesso).IntegrarCargaCteOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei:
                        Servicos.Embarcador.Integracao.Comprovei.IntegracaoComprovei.IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Runtec:
                        new Servicos.Embarcador.Integracao.Runtec.IntegracaoRuntec(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux:
                        new Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxOCOREN(unitOfWork, ocorrenciaCTeIntegracaoPendente).IntegrarOcorrencia();
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ConfirmaFacil:
                        new Servicos.Embarcador.Integracao.ConfirmaFacil.IntegracaoConfirmaFacil(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTePagamentoLoggi:
                        new Servicos.Embarcador.Integracao.Loggi.IntegracaoLoggi(unitOfWork, null).IntegrarOcorrenciaCTe(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCargaCTeOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork).IntegrarCargaCTeOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Olfar:
                        new Servicos.Embarcador.Integracao.Olfar.IntegracaoOlfar(unitOfWork).IntegrarCargaCTeOcorrencia(ocorrenciaCTeIntegracaoPendente);
                        break;
                    default:
                        ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = "Tipo de integração não configurada";
                        break;
                }

                await repositorioOcorrenciaCTeIntegracao.AtualizarAsync(ocorrenciaCTeIntegracaoPendente);
            }

            return ocorrencias;
        }

        public static byte[] GerarGZIPXMLAutorizacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            var xml = (from o in cte.XMLs where o.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao select o).FirstOrDefault();

            if (xml == null)
                return null;

            byte[] bytesXML = Encoding.ASCII.GetBytes(xml.XML);

            return Utilidades.File.GerarGZIP(bytesXML);
        }

        public static byte[] GerarXMLAutorizacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            var xml = (from o in cte.XMLs where o.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao select o).FirstOrDefault();

            if (xml == null)
                return null;

            byte[] bytesXML = Encoding.UTF8.GetBytes(xml.XML);

            return bytesXML;
        }

        public async Task<List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>> VerificarIntegracoesPendentesIndividuaisLancamentoNFSManualAsync()
        {
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = "asc",
                LimiteRegistros = 15,
                PropriedadeOrdenar = "Codigo"
            };

            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repositorioNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain servicoIntegracaoSaintGobain = new SaintGobain.IntegracaoSaintGobain(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> nfsManualCTeIntegracoesPendente = await repositorioNFSManualCTeIntegracao.BuscarCTeIntegracaoPendenteAsync(numeroTentativas, minutosACadaTentativa, parametrosConsulta);
            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentosNFSManual = (from obj in nfsManualCTeIntegracoesPendente select obj.LancamentoNFSManual).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao in nfsManualCTeIntegracoesPendente)
            {
                switch (nfsManualCTeIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken).EnviarCTeAsync(nfsManualCTeIntegracao);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain:
                        servicoIntegracaoSaintGobain.IntegrarLancamentoNFSManual(nfsManualCTeIntegracao);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva:
                        new Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva(base._unitOfWork).IntegrarLancamentoNFSManual(nfsManualCTeIntegracao);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:

                        //Integra com o KMM somente quando o status for "A" (Autorizada) da NFS
                        if (nfsManualCTeIntegracao?.LancamentoNFSManual?.CTe?.Status == "A")
                        {
                            Repositorio.Embarcador.NFS.LancamentoNFSManual repositorioLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);
                            Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = nfsManualCTeIntegracao.LancamentoNFSManual;

                            lancamentoNFSManual = Servicos.Embarcador.Integracao.IntegracaoNFSManual.AtualizarNumeracaoNFSManualIntegracao(lancamentoNFSManual, _unitOfWork);
                            await repositorioLancamentoNFSManual.AtualizarAsync(lancamentoNFSManual);

                            new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(_unitOfWork).IntegrarNFSeManual(nfsManualCTeIntegracao);
                        }
                        break;
                    case TipoIntegracao.Globus:
                        new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork).IntegrarNFSEManual(nfsManualCTeIntegracao);
                        break;

                    case TipoIntegracao.Frimesa:
                        new Servicos.Embarcador.Integracao.Frimesa.IntegracaoFrimesa(_unitOfWork).IntegrarNFSManual(nfsManualCTeIntegracao);
                        break;

                    default:
                        break;
                }

                await repositorioNFSManualCTeIntegracao.AtualizarAsync(nfsManualCTeIntegracao);
            }

            return lancamentosNFSManual;
        }

        public async Task<List<NFSManualCancelamento>> VerificarIntegracoesPendentesIndividuaisNFSManualCancelamentoAsync()
        {
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repositorioNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe> nfsManualCancelamentoIntegracaoCTes = await repositorioNFSManualCancelamentoIntegracaoCTe.BuscarIntegracaoPendenteAsync(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Individual);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> nfsManualCancelamentos = (from obj in nfsManualCancelamentoIntegracaoCTes select obj.NFSManualCancelamento).Distinct().ToList();

            Servicos.Embarcador.Integracao.KMM.IntegracaoKMM servicoIntegracaoKMM = new KMM.IntegracaoKMM(_unitOfWork);
            Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus servicoIntegracaoGlobus = new Globus.IntegracaoGlobus(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfsManualCancelamentoIntegracaoCTe in nfsManualCancelamentoIntegracaoCTes)
            {
                switch (nfsManualCancelamentoIntegracaoCTe.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await new Servicos.Embarcador.Integracao.FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken).EnviarCTeAsync(nfsManualCancelamentoIntegracaoCTe);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        servicoIntegracaoKMM.IntegrarCancelamentoNFSeManual(nfsManualCancelamentoIntegracaoCTe);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        servicoIntegracaoGlobus.IntegrarCancelamentoNFSeManual(nfsManualCancelamentoIntegracaoCTe);
                        break;
                    default:
                        break;
                }

                await repositorioNFSManualCancelamentoIntegracaoCTe.AtualizarAsync(nfsManualCancelamentoIntegracaoCTe);
            }

            return nfsManualCancelamentos;
        }

        public async Task<List<int>> VerificarIntegracoesPendentesIndividuaisAsync(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURL)
        {
            _unitOfWork.FlushAndClear();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 50;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> cargaCTeIntegracoesPendente = await repositorioCargaCTeIntegracao.BuscarCTeIntegracaoPendenteAsync(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Individual, _cancellationToken);

            List<int> cargas = (from obj in cargaCTeIntegracoesPendente select obj.CargaCTe.Carga.Codigo).Distinct().ToList();

            Servicos.Embarcador.Integracao.Avior.IntegracaoAvior serIntegracaoAvior = new Avior.IntegracaoAvior(_unitOfWork);

            for (var i = 0; i < cargaCTeIntegracoesPendente.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracaoPendente = cargaCTeIntegracoesPendente[i];

                switch (cargaCTeIntegracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior:
                        Repositorio.Embarcador.Cargas.CargaIntegracaoAvon repCargaIntegracaoAvon = new Repositorio.Embarcador.Cargas.CargaIntegracaoAvon(_unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon cargaIntegracaoAvon = repCargaIntegracaoAvon.BuscarPorCarga(cargaCTeIntegracaoPendente.CargaCTe.Carga.Codigo);
                        serIntegracaoAvior.EnviarCTesAvior(ref cargaCTeIntegracaoPendente, cargaIntegracaoAvon, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await new Servicos.Embarcador.Integracao.FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken).EnviarCTeAsync(cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAEscrituracaoCTe:
                        Servicos.Embarcador.Integracao.GPA.IntegracaoGPA.IntegrarEscrituracaoCTe(ref cargaCTeIntegracaoPendente, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ravex:
                        Servicos.Embarcador.Integracao.Ravex.Carga.IntegrarCTe(ref cargaCTeIntegracaoPendente, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario:
                        (new Servicos.Embarcador.Integracao.Boticario.IntegracaoBoticario(_unitOfWork)).IntegrarCargaCTe(ref cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador:
                        new MultiEmbarcador.CTe(_unitOfWork).IntegrarCargaCTe(cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                        (new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(_unitOfWork)).IntegrarCargaCTe(cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTePagamentoLoggi:
                        (new Servicos.Embarcador.Integracao.Loggi.IntegracaoLoggi(_unitOfWork, auditado)).IntegrarCargaCTe(ref cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Brado:
                        new Servicos.Embarcador.Integracao.Brado.IntegracaoBrado(_unitOfWork).IntegrarCargaCte(cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Yandeh:
                        (new Servicos.Embarcador.Integracao.Yandeh.IntegracaoYandeh(_unitOfWork)).IntegrarCargaCTe(ref cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete:
                        new Servicos.Embarcador.Integracao.EFrete.Recebivel(_unitOfWork, clienteURL).IntegrarCargaCte(cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ValoresCTeLoggi:
                        (new Servicos.Embarcador.Integracao.Loggi.IntegracaoLoggi(_unitOfWork, auditado)).IntegrarCargaValoresCTeLoggi(cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        (new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(_unitOfWork)).IntegrarCargaCTe(cargaCTeIntegracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        (new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork)).IntegrarCargaCTe(cargaCTeIntegracaoPendente);
                        break;
                    default:
                        break;
                }

                await repositorioCargaCTeIntegracao.AtualizarAsync(cargaCTeIntegracaoPendente);
            }

            return cargas;
        }

        public List<int> VerificarIntegracoesPendentesLote()
        {
            AdicionarIntegracoesALote();

            List<int> cargas = new List<int>();

            cargas.AddRange(ConsultarIntegracoesLote());
            cargas.AddRange(EnviarIntegracoesLote());

            return cargas.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> VerificarIntegracoesNFSManualPendentesLote(Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracoesNFSManualALote(unitOfWork);

            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentosNFSManual = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            lancamentosNFSManual.AddRange(ConsultarIntegracoesLoteNFSManual(unitOfWork));
            lancamentosNFSManual.AddRange(EnviarIntegracoesLoteNFSManual(unitOfWork));

            return lancamentosNFSManual.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> VerificarIntegracoesNFSManualCancelamentoPendentesLote(Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracoesNFSManualALote(unitOfWork);

            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentosNFSManual = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            lancamentosNFSManual.AddRange(ConsultarIntegracoesLoteNFSManual(unitOfWork));
            lancamentosNFSManual.AddRange(EnviarIntegracoesLoteNFSManual(unitOfWork));

            return lancamentosNFSManual.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> VerificarIntegracoesPendentesLoteOcorrencia(Repositorio.UnitOfWork unitOfWork)
        {
            AdicionarIntegracoesALoteOcorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> cargaOcorrencia = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            cargaOcorrencia.AddRange(ConsultarIntegracoesLoteOcorrencia(unitOfWork));
            cargaOcorrencia.AddRange(EnviarIntegracoesLoteOcorrencia(unitOfWork));

            return cargaOcorrencia.Distinct().ToList();
        }

        public void GerarIntegracoesCartaCorrecaoCte(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao, Repositorio.UnitOfWork unitOfWork)
        {
            if (cartaCorrecao.Status != Dominio.Enumeradores.StatusCCe.Autorizado)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repositorioIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);

            List<TipoIntegracao> tiposIntegracaoAutorizados = new List<TipoIntegracao> { TipoIntegracao.Intercab, TipoIntegracao.EMP, TipoIntegracao.NFTP };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(tiposIntegracaoAutorizados);
            if (tiposIntegracao.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                if (tipoIntegracao.Tipo == TipoIntegracao.Intercab)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();
                    if (integracaoIntercab == null)
                        continue;

                    if (integracaoIntercab.PossuiIntegracaoIntercab && integracaoIntercab.AtivarIntegracaoCartaCorrecao)
                        AdicionarCartaCorrecaoIntegracao(cartaCorrecao, tipoIntegracao, unitOfWork);
                }
                else if (tipoIntegracao.Tipo == TipoIntegracao.EMP)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repositorioIntegracaoEMP.Buscar();
                    if (integracaoEMP == null)
                        continue;

                    if (integracaoEMP.PossuiIntegracaoEMP && integracaoEMP.AtivarIntegracaoCartaCorrecaoEMP && !repositorioCargaCTe.CargaSVMDoCTe(cartaCorrecao.CTe.Codigo))
                        AdicionarCartaCorrecaoIntegracao(cartaCorrecao, tipoIntegracao, unitOfWork);
                }
                else if (tipoIntegracao.Tipo == TipoIntegracao.NFTP)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repositorioIntegracaoEMP.Buscar();
                    if (integracaoEMP == null)
                        continue;

                    if (integracaoEMP.AtivarIntegracaoNFTPEMP && !repositorioCargaCTe.CargaSVMDoCTe(cartaCorrecao.CTe.Codigo))
                        AdicionarCartaCorrecaoIntegracao(cartaCorrecao, tipoIntegracao, unitOfWork);
                }
            }
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCtesPorCarga(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<int> codigosCargasCTe = repositorioCargaCTe.BuscarCodigosInteiroCTePorCarga(codigoCarga);

            return repositorioCTe.BuscarCTesPorCodigos(codigosCargasCTe);
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarDocumentosPorModeloDocumentoFiscalECarga(List<Dominio.Enumeradores.TipoDocumento> modelosDocumentos, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<int> codigosCTes = repositorioCargaCTe.BuscarCodigosPorModeloDocumentoFiscalECarga(modelosDocumentos, codigoCarga);
            return repositorioCTe.BuscarCTesPorCodigos(codigosCTes);
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao AdicionarCartaCorrecaoIntegracao(Dominio.Entidades.CartaDeCorrecaoEletronica cartaCorrecao, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCartaCorrecaoIntegracao = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao cartaCorrecaoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao()
            {
                CartaCorrecao = cartaCorrecao,
                TipoIntegracao = tipoIntegracao,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao
            };

            repCartaCorrecaoIntegracao.Inserir(cartaCorrecaoIntegracao);

            return cartaCorrecaoIntegracao;
        }

        private List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> EnviarIntegracoesLoteOcorrencia(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroLotesPorVez = 3;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote repOcorrenciaCTeIntegracaoLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote> lotesPendentes = repOcorrenciaCTeIntegracaoLote.BuscarLotesPendentes(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroLotesPorVez);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote in lotesPendentes)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote = repOcorrenciaCTeIntegracaoLote.BuscarCTesPorLote(lote.Codigo);

                if (ctesDoLote.Count > 0)
                {
                    ocorrencias.AddRange(ctesDoLote.Select(o => o.CargaOcorrencia).Distinct());

                    switch (lote.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                            Servicos.Embarcador.Integracao.Avon.IntegracaoCTeAvon.EnviarRetorno(lote, ctesDoLote, unitOfWork);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura:
                            Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura.EnviarRetornoDT(lote, ctesDoLote, unitOfWork);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    lote.ProblemaIntegracao = "Nenhum CT-e vinculado ao lote, por isso o lote foi setado para integrado.";

                    repOcorrenciaCTeIntegracaoLote.Atualizar(lote);
                }
            }

            return ocorrencias.Distinct().ToList();
        }

        private List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> EnviarIntegracoesLoteNFSManual(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentosNFSManual = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 60;
            int numeroLotesPorVez = 3;

            Repositorio.Embarcador.NFS.NFSManualIntegracaoLote repNFSManualIntegracaoLote = new Repositorio.Embarcador.NFS.NFSManualIntegracaoLote(unitOfWork);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote> lotesPendentes = repNFSManualIntegracaoLote.BuscarLotesPendentes(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon, "Codigo", "asc", 3);

            lotesPendentes.AddRange(repNFSManualIntegracaoLote.BuscarLotesPendentes(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura, "Codigo", "asc", numeroLotesPorVez));

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote in lotesPendentes)
            {
                List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote = repNFSManualIntegracaoLote.BuscarCTesPorLote(lote.Codigo);

                if (ctesDoLote.Count > 0)
                {
                    lancamentosNFSManual.AddRange(ctesDoLote.Select(o => o.LancamentoNFSManual).Distinct());

                    switch (lote.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                            Servicos.Embarcador.Integracao.Avon.IntegracaoCTeAvon.EnviarRetorno(lote, ctesDoLote, unitOfWork);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura:
                            Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura.EnviarRetornoDT(lote, ctesDoLote, unitOfWork);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    lote.ProblemaIntegracao = "Nenhum CT-e vinculado ao lote, por isso o lote foi setado para integrado.";

                    repNFSManualIntegracaoLote.Atualizar(lote);
                }
            }

            return lancamentosNFSManual.Distinct().ToList();
        }

        private List<int> EnviarIntegracoesLote()
        {
            _unitOfWork.FlushAndClear();

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 60;
            int numeroLotesPorVez = 3;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote repCargaCTeIntegracaoLote = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote> lotesPendentes = repCargaCTeIntegracaoLote.BuscarLotesPendentes(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon, "Codigo", "asc", 3);

            lotesPendentes.AddRange(repCargaCTeIntegracaoLote.BuscarLotesPendentes(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura, "Codigo", "asc", numeroLotesPorVez));

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote lote in lotesPendentes)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesDoLote = repCargaCTeIntegracaoLote.BuscarCTesPorLote(lote.Codigo);

                if (ctesDoLote.Count > 0)
                {
                    cargas.AddRange(ctesDoLote.Select(o => o.CargaCTe.Carga).Distinct());

                    switch (lote.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                            Servicos.Embarcador.Integracao.Avon.IntegracaoCTeAvon.EnviarRetorno(lote, ctesDoLote, _unitOfWork);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura:
                            Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura.EnviarRetornoDT(lote, ctesDoLote, _unitOfWork);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    lote.ProblemaIntegracao = "Nenhum CT-e vinculado ao lote, por isso o lote foi setado para integrado.";

                    repCargaCTeIntegracaoLote.Atualizar(lote);
                }
            }

            return cargas.Select(x => x.Codigo).Distinct().ToList();
        }

        private List<int> ConsultarIntegracoesLote()
        {
            _unitOfWork.FlushAndClear();

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            int numeroLotesPorVez = 3;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote repCargaCTeIntegracaoLote = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote> lotesPendentes = repCargaCTeIntegracaoLote.BuscarLotesAguardandoRetorno("DataUltimaConsultaRetorno", "asc", numeroLotesPorVez, 360);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote lote in lotesPendentes)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesDoLote = repCargaCTeIntegracaoLote.BuscarCTesPorLote(lote.Codigo);

                if (ctesDoLote.Count > 0)
                {
                    cargas.AddRange(ctesDoLote.Select(o => o.CargaCTe.Carga).Distinct());

                    switch (lote.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                            Servicos.Embarcador.Integracao.Avon.IntegracaoCTeAvon.ConsultarRetorno(lote, ctesDoLote, _unitOfWork);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    lote.ProblemaIntegracao = "Nenhum CT-e vinculado ao lote, por isso o lote foi setado para integrado.";

                    repCargaCTeIntegracaoLote.Atualizar(lote);
                }
            }

            return cargas.Select(x => x.Codigo).Distinct().ToList();
        }

        private List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> ConsultarIntegracoesLoteNFSManual(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentosNFSManual = new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            int numeroLotesPorVez = 3;

            Repositorio.Embarcador.NFS.NFSManualIntegracaoLote repNFSManualIntegracaoLote = new Repositorio.Embarcador.NFS.NFSManualIntegracaoLote(unitOfWork);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote> lotesPendentes = repNFSManualIntegracaoLote.BuscarLotesAguardandoRetorno("DataUltimaConsultaRetorno", "asc", numeroLotesPorVez, 360);

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote in lotesPendentes)
            {
                List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote = repNFSManualIntegracaoLote.BuscarCTesPorLote(lote.Codigo);

                if (ctesDoLote.Count > 0)
                {
                    lancamentosNFSManual.AddRange(ctesDoLote.Select(o => o.LancamentoNFSManual).Distinct());

                    switch (lote.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                            Servicos.Embarcador.Integracao.Avon.IntegracaoCTeAvon.ConsultarRetorno(lote, ctesDoLote, unitOfWork);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    lote.ProblemaIntegracao = "Nenhum CT-e vinculado ao lote, por isso o lote foi setado para integrado.";

                    repNFSManualIntegracaoLote.Atualizar(lote);
                }
            }

            return lancamentosNFSManual.Distinct().ToList();
        }

        private List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ConsultarIntegracoesLoteOcorrencia(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            int numeroLotesPorVez = 3;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote repOcorrenciaCTeIntegracaoLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote> lotesPendentes = repOcorrenciaCTeIntegracaoLote.BuscarLotesAguardandoRetorno("DataUltimaConsultaRetorno", "asc", numeroLotesPorVez, 360);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote in lotesPendentes)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote = repOcorrenciaCTeIntegracaoLote.BuscarCTesPorLote(lote.Codigo);

                if (ctesDoLote.Count > 0)
                {
                    ocorrencias.AddRange(ctesDoLote.Select(o => o.CargaCTe.CargaCTeComplementoInfo.CargaOcorrencia).Distinct());

                    switch (lote.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon:
                            Servicos.Embarcador.Integracao.Avon.IntegracaoCTeAvon.ConsultarRetorno(lote, ctesDoLote, unitOfWork);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    lote.ProblemaIntegracao = "Nenhum CT-e vinculado ao lote, por isso o lote foi setado para integrado.";

                    repOcorrenciaCTeIntegracaoLote.Atualizar(lote);
                }
            }

            return ocorrencias.Distinct().ToList();
        }

        private void AdicionarIntegracoesALote()
        {
            _unitOfWork.FlushAndClear();

            int numeroRegistrosPorVez = 5000;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote repCargaCTeIntegracaoLote = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoNatura repCargaIntegracaoNatura = new Repositorio.Embarcador.Cargas.CargaIntegracaoNatura(_unitOfWork);
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDTNatura = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> cargaCTeIntegracoesPendente = repCargaCTeIntegracao.BuscarCTeIntegracaoSemLote("Codigo", "asc", numeroRegistrosPorVez);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracoes = cargaCTeIntegracoesPendente.Select(o => o.TipoIntegracao).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracoes)
            {
                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                {
                    var cargas = (from obj in cargaCTeIntegracoesPendente where obj.TipoIntegracao.Codigo == tipoIntegracao.Codigo select obj.CargaCTe.Carga).Distinct().ToList();

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> cargaCTeIntegracoesDaCarga = cargaCTeIntegracoesPendente.Where(o => o.CargaCTe.Carga.Codigo == carga.Codigo).ToList();

                        if (cargaCTeIntegracoesDaCarga.Count() == repCargaCTe.ContarConhecimentoPorCarga(carga.Codigo))
                        {
                            if (!repCargaIntegracaoNatura.ExistePorCarga(carga.Codigo) || repCargaIntegracaoNatura.ExisteDTSemNotaFiscalPorCarga(carga.Codigo))
                            {
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao in cargaCTeIntegracoesDaCarga)
                                {
                                    cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    cargaCTeIntegracao.ProblemaIntegracao = "Nenhum documento de transporte da natura vinculado à carga.";
                                    cargaCTeIntegracao.NumeroTentativas++;

                                    repCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
                                }
                            }
                            else
                            {
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura cargaIntegracaoNatura in carga.IntegracoesNatura)
                                {
                                    List<string> chavesDasNotas = repNotaFiscalDTNatura.BuscarChavePorDocumentoTransporte(cargaIntegracaoNatura.DocumentoTransporte.Codigo);

                                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesParaOLoteNatura = (from obj in cargaCTeIntegracoesDaCarga where obj.CargaCTe.CTe.Documentos.Any(o => chavesDasNotas.Contains(o.ChaveNFE)) select obj).Distinct().ToList();

                                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote lote = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote
                                    {
                                        SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                                        TipoIntegracao = tipoIntegracao,
                                        IntegracaoNatura = cargaIntegracaoNatura,
                                        DataUltimaConsultaRetorno = DateTime.Now
                                    };

                                    repCargaCTeIntegracaoLote.Inserir(lote);

                                    repCargaCTeIntegracao.SetarLotePorCodigos(ctesParaOLoteNatura.Select(o => o.Codigo).ToList(), lote.Codigo);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int countIntegracoes = cargaCTeIntegracoesPendente.Count(o => o.TipoIntegracao.Codigo == tipoIntegracao.Codigo);

                    for (var i = 0; i < countIntegracoes; i += tipoIntegracao.QuantidadeMaximaEnvioLote)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesDoLote = cargaCTeIntegracoesPendente.Where(o => o.TipoIntegracao.Codigo == tipoIntegracao.Codigo).Skip(i).Take(tipoIntegracao.QuantidadeMaximaEnvioLote).ToList();

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote lote = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote();

                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        lote.TipoIntegracao = tipoIntegracao;
                        lote.DataUltimaConsultaRetorno = DateTime.Now;

                        repCargaCTeIntegracaoLote.Inserir(lote);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao in ctesDoLote)
                        {
                            cargaCTeIntegracao.Lote = lote;

                            repCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
                        }
                    }
                }
            }
        }

        private void AdicionarIntegracoesNFSManualALote(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            unidadeDeTrabalho.FlushAndClear();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = "asc",
                LimiteRegistros = 1000,
                PropriedadeOrdenar = "Codigo"
            };

            Repositorio.Embarcador.NFS.NFSManualIntegracaoLote repNFSManualIntegracaoLote = new Repositorio.Embarcador.NFS.NFSManualIntegracaoLote(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> nfsManualCteIntegracoesPendente = repNFSManualCTeIntegracao.BuscarCTeIntegracaoSemLote(parametrosConsulta);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracoes = nfsManualCteIntegracoesPendente.Select(o => o.TipoIntegracao).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracoes)
            {
                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                {
                    var lancamentosNFSManual = (from obj in nfsManualCteIntegracoesPendente where obj.TipoIntegracao.Codigo == tipoIntegracao.Codigo select obj.LancamentoNFSManual).Distinct().ToList();

                    foreach (Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual in lancamentosNFSManual)
                    {
                        List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> nfsManualCTeIntegracoesDaCarga = nfsManualCteIntegracoesPendente.Where(o => o.LancamentoNFSManual.Codigo == lancamentoNFSManual.Codigo).ToList();

                        //if (nfsManualCTeIntegracoesDaCarga.Count() == lancamentoNFSManual.CargaCTes.Where(o => o.CargaCTeComplementoInfo == null).Count())
                        //{
                        unidadeDeTrabalho.Start();

                        if (lancamentoNFSManual.IntegracoesNatura.Count <= 0 || lancamentoNFSManual.IntegracoesNatura.Any(o => o.DocumentoTransporte.NotasFiscais.Count <= 0))
                        {
                            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao in nfsManualCTeIntegracoesDaCarga)
                            {
                                nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                nfsManualCTeIntegracao.ProblemaIntegracao = "Nenhum documento de transporte da natura vinculado à carga.";

                                repNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
                            }
                        }
                        else
                        {
                            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoNatura nfsManualIntegracaoNatura in lancamentoNFSManual.IntegracoesNatura)
                            {
                                List<string> chavesDasNotas = (from obj in nfsManualIntegracaoNatura.DocumentoTransporte.NotasFiscais select obj.Chave).ToList();

                                List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesParaOLoteNatura = (from obj in nfsManualCTeIntegracoesDaCarga where obj.LancamentoNFSManual.CTe.Documentos.Any(o => chavesDasNotas.Contains(o.ChaveNFE)) select obj).Distinct().ToList();

                                Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote();

                                lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                                lote.TipoIntegracao = tipoIntegracao;
                                lote.IntegracaoNatura = nfsManualIntegracaoNatura;
                                lote.DataUltimaConsultaRetorno = DateTime.Now;

                                repNFSManualIntegracaoLote.Inserir(lote);

                                foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao in ctesParaOLoteNatura)
                                {
                                    nfsManualCTeIntegracao.Lote = lote;
                                    repNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
                                }
                            }
                        }

                        unidadeDeTrabalho.CommitChanges();
                        //}
                    }
                }
                else
                {
                    int countIntegracoes = nfsManualCteIntegracoesPendente.Where(o => o.TipoIntegracao.Codigo == tipoIntegracao.Codigo).Count();

                    for (var i = 0; i < countIntegracoes; i += tipoIntegracao.QuantidadeMaximaEnvioLote)
                    {
                        List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote = nfsManualCteIntegracoesPendente.Where(o => o.TipoIntegracao.Codigo == tipoIntegracao.Codigo).Skip(i).Take(tipoIntegracao.QuantidadeMaximaEnvioLote).ToList();

                        unidadeDeTrabalho.Start();

                        Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote();

                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        lote.TipoIntegracao = tipoIntegracao;
                        lote.DataUltimaConsultaRetorno = DateTime.Now;

                        repNFSManualIntegracaoLote.Inserir(lote);

                        foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao cargaCTeIntegracao in ctesDoLote)
                        {
                            cargaCTeIntegracao.Lote = lote;

                            repNFSManualCTeIntegracao.Atualizar(cargaCTeIntegracao);
                        }

                        unidadeDeTrabalho.CommitChanges();
                    }
                }
            }
        }

        private void AdicionarIntegracoesALoteOcorrencia(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            int numeroRegistrosPorVez = 1000;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote repOcorrenciaCTeIntegracaoLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDTNatura = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ocorrenciaCTeIntegracoesPendente = repOcorrenciaCTeIntegracao.BuscarCTeIntegracaoSemLote("Codigo", "asc", numeroRegistrosPorVez);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracoes = ocorrenciaCTeIntegracoesPendente.Select(o => o.TipoIntegracao).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracoes)
            {
                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                {
                    var ocorrencias = (from obj in ocorrenciaCTeIntegracoesPendente where obj.TipoIntegracao.Codigo == tipoIntegracao.Codigo select obj.CargaOcorrencia).Distinct().ToList();

                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
                    {
                        List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ocorrenciaCTeIntegracoesDaOcorrencia = ocorrenciaCTeIntegracoesPendente.Where(o => o.CargaOcorrencia.Codigo == ocorrencia.Codigo).ToList();
                        if (ocorrenciaCTeIntegracoesDaOcorrencia.Count() == repCargaOcorrenciaDocumento.ContarPorCTEsOcorrencia(ocorrencia.Codigo))
                        {
                            if (ocorrencia.DTNatura != null)
                            {
                                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote();

                                lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                                lote.TipoIntegracao = tipoIntegracao;
                                lote.DataUltimaConsultaRetorno = DateTime.Now;

                                repOcorrenciaCTeIntegracaoLote.Inserir(lote);

                                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ocorrenciaCTeIntegracoesDaOcorrencia)
                                {
                                    ocorrenciaCTeIntegracao.Lote = lote;

                                    repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                                }
                            }
                            else if (ocorrencia.Carga != null && ocorrencia.Carga.IntegracoesNatura.Count <= 0)
                            {
                                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ocorrenciaCTeIntegracoesDaOcorrencia)
                                {
                                    ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    ocorrenciaCTeIntegracao.ProblemaIntegracao = "Nenhum documento de transporte da natura vinculado à ocorrencia.";

                                    repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                                }
                            }
                            else if (ocorrencia.Carga != null)
                            {
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura ocorrenciaIntegracaoNatura in ocorrencia.Carga.IntegracoesNatura)
                                {
                                    List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesParaOLoteNatura = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();

                                    List<string> chavesDasNotas = repNotaFiscalDTNatura.BuscarChavePorDocumentoTransporte(ocorrenciaIntegracaoNatura.DocumentoTransporte.Codigo);

                                    if (ocorrencia.ComponenteFrete != null) //ct-e complementar
                                    {
                                        List<string> chavesDosCTesPais = (from obj in ocorrencia.Carga.CargaCTes.Where(o => o.CTe.Documentos.Any(c => chavesDasNotas.Contains(c.ChaveNFE))) select obj.CTe.Chave).ToList();

                                        ctesParaOLoteNatura = (from obj in ocorrenciaCTeIntegracoesDaOcorrencia where chavesDosCTesPais.Contains(obj.CargaCTe.CTe.ChaveCTESubComp) select obj).Distinct().ToList();
                                    }
                                    else
                                    {
                                        ctesParaOLoteNatura = (from obj in ocorrenciaCTeIntegracoesDaOcorrencia where obj.CargaCTe.CTe.Documentos.Any(o => chavesDasNotas.Contains(o.ChaveNFE)) select obj).ToList();
                                    }

                                    if (ctesParaOLoteNatura.Count > 0)
                                    {
                                        Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote();

                                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                                        lote.TipoIntegracao = tipoIntegracao;
                                        lote.IntegracaoNatura = ocorrenciaIntegracaoNatura;
                                        lote.DataUltimaConsultaRetorno = DateTime.Now;

                                        repOcorrenciaCTeIntegracaoLote.Inserir(lote);

                                        foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ctesParaOLoteNatura)
                                        {
                                            ocorrenciaCTeIntegracao.Lote = lote;

                                            repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                                        }
                                    }
                                    else
                                    {
                                        foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ocorrenciaCTeIntegracoesDaOcorrencia)
                                        {
                                            ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                            ocorrenciaCTeIntegracao.ProblemaIntegracao = "Nenhum CT-e encontrado para as notas deste DT da natura.";

                                            repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                                        }
                                    }
                                }
                            }

                            //unidadeDeTrabalho.CommitChanges();
                        }
                    }
                }
                else
                {
                    int countIntegracoes = ocorrenciaCTeIntegracoesPendente.Where(o => o.TipoIntegracao.Codigo == tipoIntegracao.Codigo).Count();

                    for (var i = 0; i < countIntegracoes; i += tipoIntegracao.QuantidadeMaximaEnvioLote)
                    {
                        List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote = ocorrenciaCTeIntegracoesPendente.Where(o => o.TipoIntegracao.Codigo == tipoIntegracao.Codigo).Skip(i).Take(tipoIntegracao.QuantidadeMaximaEnvioLote).ToList();

                        unidadeDeTrabalho.Start();

                        Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote();

                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        lote.TipoIntegracao = tipoIntegracao;
                        lote.DataUltimaConsultaRetorno = DateTime.Now;

                        repOcorrenciaCTeIntegracaoLote.Inserir(lote);

                        foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ctesDoLote)
                        {
                            ocorrenciaCTeIntegracao.Lote = lote;

                            repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                        }

                        unidadeDeTrabalho.CommitChanges();
                    }
                }
            }
        }

        #endregion
    }
}
