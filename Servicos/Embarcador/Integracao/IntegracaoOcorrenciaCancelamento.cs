using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public sealed class IntegracaoOcorrenciaCancelamento
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoOcorrenciaCancelamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public bool AdicionarIntegracoesCancelamento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repintegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            IntegracaoCTe serIntegracaoCte = new IntegracaoCTe(_unitOfWork);


            List<TipoIntegracao> tiposIntegracao = ObterTiposIntegracoes(ocorrenciaCancelamento.Ocorrencia);
            bool possuiIntegracao = false;

            foreach (TipoIntegracao tipo in tiposIntegracao)
            {
                if (!TipoIntegracaoHabilitado(tipo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(tipo);
                if (tipoIntegracao == null)
                    continue;

                if (TipoIntegracaoIndividual(tipo))
                {
                    if (tipo == TipoIntegracao.Intercab && ocorrenciaCancelamento.Ocorrencia.OcorrenciaRecebidaDeIntegracao != true)
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repintegracaoIntercab.BuscarIntegracao();

                        if (integracaoIntercab != null && (integracaoIntercab?.AtivarIntegracaoOcorrencias ?? false))
                            AdicionarIntegracaoIndividual(ocorrenciaCancelamento, tipoIntegracao);
                    }
                    if (tipo == TipoIntegracao.EMP)
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

                        if (integracaoEMP != null && (integracaoEMP?.PossuiIntegracaoEMP ?? false) && (integracaoEMP?.AtivarIntegracaoCancelamentoOcorrenciaEMP ?? false))
                            AdicionarIntegracaoIndividual(ocorrenciaCancelamento, tipoIntegracao);
                    }
                    if (tipo == TipoIntegracao.NFTP)
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

                        if (integracaoEMP != null && (integracaoEMP?.AtivarIntegracaoNFTPEMP ?? false))
                            AdicionarIntegracaoIndividual(ocorrenciaCancelamento, tipoIntegracao);
                    }
                    else if (tipo != TipoIntegracao.Intercab)
                        AdicionarIntegracaoIndividual(ocorrenciaCancelamento, tipoIntegracao);
                }
                else if (TipoIntegracaoPorCTe(tipo))
                    AdicionarIntegracaoPorCTe(ocorrenciaCancelamento, tipoIntegracao);
                else if (tipo == TipoIntegracao.KMM)
                {
                    serIntegracaoCte.AdcionarCTesParaEnvioViaIntegracaoIndividual(ocorrenciaCancelamento.Ocorrencia, tipo, _unitOfWork, true, new List<string> { "CT-e", "NFS-e", "NFS", "ND" });
                    AdicionarIntegracaoPorCTe(ocorrenciaCancelamento, tipoIntegracao, new List<string> { "CT-e", "NFS-e", "NFS", "ND" });
                }
                else if (tipo == TipoIntegracao.Globus)
                {
                    Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> cteIntegracoes = repOcorrenciaCTeIntegracao.BuscarPorOcorrencia(ocorrenciaCancelamento.Ocorrencia.Codigo) ??                                                                                new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>();
                    List<string> modelosPermitidos = new List<string>();

                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao cteIntegracao in cteIntegracoes)
                    {
                        if (cteIntegracao.CargaCTe?.CTe?.ModeloDocumentoFiscal != null && !(cteIntegracao.CargaCTe?.CTe?.ModeloDocumentoFiscal?.NaoGerarFaturamento ?? false)) 
                            modelosPermitidos.Add(cteIntegracao.CargaCTe?.CTe?.ModeloDocumentoFiscal.Abreviacao);
                    }

                    if (modelosPermitidos.Count > 0)
                        AdicionarIntegracaoPorCTe(ocorrenciaCancelamento, tipoIntegracao, modelosPermitidos);
                }


                possuiIntegracao = true;
            }

            return possuiIntegracao;
        }

        public void VerificarIntegracoesOcorrenciaPendentes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso)
        {
            VerificarIntegracoesPendentes(tipoServicoMultisoftware, clienteURLAcesso);
            VerificarIntegracoesCTePendentes(tipoServicoMultisoftware, clienteURLAcesso);

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao repOcorrenciaCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);

            Hubs.Ocorrencia servicoNotificacaoOcorrencia = new Hubs.Ocorrencia();
            List<int> codigosCancelamentosOcorrencia = repOcorrenciaCancelamento.BuscarCancelamentosPorSituacao(SituacaoCancelamentoOcorrencia.AguardandoIntegracao, 100);

            for (int i = 0; i < codigosCancelamentosOcorrencia.Count; i++)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorCodigo(codigosCancelamentosOcorrencia[i]);

                if (repOcorrenciaCancelamentoIntegracao.ContarPorOcorrenciaCancelamento(ocorrenciaCancelamento.Codigo, SituacaoIntegracao.AgIntegracao) > 0)
                    continue;

                if (repOcorrenciaCancelamentoIntegracao.ContarPorOcorrenciaCancelamento(ocorrenciaCancelamento.Codigo, SituacaoIntegracao.ProblemaIntegracao) > 0)
                    ocorrenciaCancelamento.Situacao = SituacaoCancelamentoOcorrencia.FalhaIntegracao;
                else if (repOcorrenciaCTeCancelamentoIntegracao.ContarPorOcorrenciaCancelamento(ocorrenciaCancelamento.Codigo, SituacaoIntegracao.ProblemaIntegracao) > 0)
                    ocorrenciaCancelamento.Situacao = SituacaoCancelamentoOcorrencia.FalhaIntegracao;
                else
                    ocorrenciaCancelamento.Situacao = SituacaoCancelamentoOcorrencia.Cancelada;

                repOcorrenciaCancelamento.Atualizar(ocorrenciaCancelamento);
                servicoNotificacaoOcorrencia.InformarCancelamentoAtualizado(ocorrenciaCancelamento.Codigo);

                _unitOfWork.FlushAndClear();
            }
        }

        #endregion

        #region Métodos Privados

        private void VerificarIntegracoesCTePendentes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao> listaIntegracao = repOcorrenciaCTeCancelamentoIntegracao.BuscarPorIntegracaoPendente(numeroTentativasLimite: 2, tempoProximaTentativaEmMinutos: 5, limiteRegistros: 15);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao integracao in listaIntegracao)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Boticario:
                        new Boticario.IntegracaoBoticario(_unitOfWork).IntegrarCancelamentoOcorrencia(integracao);
                        break;
                    case TipoIntegracao.Brado:
                        new Brado.IntegracaoBrado(_unitOfWork).IntegrarCancelamentoOcorrencia(integracao);
                        break;
                    case TipoIntegracao.Unilever:
                        new Unilever.IntegracaoUnilever(_unitOfWork).IntegrarCancelamentoOcorrencia(integracao);
                        break;
                    case TipoIntegracao.EFrete:
                        new EFrete.Recebivel(_unitOfWork, clienteURLAcesso).IntegrarCancelamentoOcorrencia(integracao);
                        break;
                    case TipoIntegracao.KMM:
                        new KMM.IntegracaoKMM(_unitOfWork).IntegrarCancelamentoCargaCTeOcorrencia(integracao);
                        break;
                    case TipoIntegracao.Globus:
                        new Globus.IntegracaoGlobus(_unitOfWork).IntegrarCancelamentoCargaCTeOcorrencia(integracao);
                        break;
                    default:
                        break;
                }
            }
        }

        private void VerificarIntegracoesPendentes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso)
        {
            Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab servicoIntegracaoIntercab = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(_unitOfWork);
            Servicos.Embarcador.Integracao.EMP.IntegracaoEMP svcIntegracaoEMP = new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(_unitOfWork);
            Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP svcIntegracaoNFTP = new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(_unitOfWork);

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao> listaIntegracao = repositorioIntegracao.BuscarPorIntegracaoPendente(numeroTentativasLimite: 2, tempoProximaTentativaEmMinutos: 5, limiteRegistros: 15);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao integracao in listaIntegracao)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Minerva:
                        new Minerva.IntegracaoMinerva(_unitOfWork).IntegrarOcorrenciaCancelamentoIntegracao(integracao);
                        break;

                    case TipoIntegracao.Intercab:
                        servicoIntegracaoIntercab.IntegrarCancelamentoOcorrenciaCTe(integracao);
                        break;
                    case TipoIntegracao.EMP:
                        svcIntegracaoEMP.IntegrarOcorrenciaCancelamento(integracao, clienteURLAcesso?.URLAcesso ?? "");
                        break;
                    case TipoIntegracao.NFTP:
                        svcIntegracaoNFTP.IntegrarOcorrenciaCancelamento(integracao, clienteURLAcesso?.URLAcesso ?? "");
                        break;
                    default:
                        break;
                }
            }
        }

        private List<TipoIntegracao> ObterTiposIntegracoes(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            List<TipoIntegracao> tiposIntegracaoOcorrencia = repOcorrenciaIntegracao.BuscarTiposPorOcorrencia(ocorrencia.Codigo);
            List<TipoIntegracao> tiposIntegracaoCTes = repOcorrenciaCTeIntegracao.ObterTiposIntegracoes(ocorrencia.Codigo);

            tiposIntegracaoOcorrencia.AddRange(tiposIntegracaoCTes);

            return tiposIntegracaoOcorrencia.Distinct().ToList();
        }

        private void AdicionarIntegracaoIndividual(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao repositorioOcorrenciaCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao(_unitOfWork);

            if (repositorioOcorrenciaCancelamentoIntegracao.ContarPorOcorrenciaCancelamento(ocorrenciaCancelamento.Codigo, situacao: null, tipoIntegracao: tipoIntegracao.Tipo) > 0)
                return;

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao integracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao
            {
                OcorrenciaCancelamento = ocorrenciaCancelamento,
                TipoIntegracao = tipoIntegracao,
                DataIntegracao = DateTime.Now,
                NumeroTentativas = 0,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao
            };

            repositorioOcorrenciaCancelamentoIntegracao.Inserir(integracao);
        }

        private void AdicionarIntegracaoPorCTe(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, List<string> modelosPermitidos = null)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> cteIntegracoes = repOcorrenciaCTeIntegracao.BuscarPorOcorrencia(ocorrenciaCancelamento.Ocorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao cteIntegracao in cteIntegracoes)
            {
                if (modelosPermitidos != null)
                {
                    if (!modelosPermitidos.Any(x => x == (cteIntegracao.CargaCTe?.CTe?.ModeloDocumentoFiscal?.Abreviacao ?? "")))
                        continue;
                }

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao integracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao
                {
                    OcorrenciaCTeIntegracao = cteIntegracao,
                    OcorrenciaCancelamento = ocorrenciaCancelamento,
                    TipoIntegracao = tipoIntegracao,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = "",
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao
                };

                repOcorrenciaCTeCancelamentoIntegracao.Inserir(integracao);
            }
        }

        private bool TipoIntegracaoHabilitado(TipoIntegracao tipo)
        {
            List<TipoIntegracao> integracaosHabilitadas = new List<TipoIntegracao>()
            {
                TipoIntegracao.Minerva,
                TipoIntegracao.Boticario,
                TipoIntegracao.Brado,
                TipoIntegracao.EFrete,
                TipoIntegracao.Intercab,
                TipoIntegracao.EMP,
                TipoIntegracao.NFTP,
                TipoIntegracao.KMM,
                TipoIntegracao.Globus
            };

            return integracaosHabilitadas.Contains(tipo);
        }

        private bool TipoIntegracaoIndividual(TipoIntegracao tipo)
        {
            List<TipoIntegracao> integracaosIndividuaisHabilitadas = new List<TipoIntegracao>()
            {
                TipoIntegracao.Minerva,
                TipoIntegracao.Intercab,
                TipoIntegracao.EMP,
                TipoIntegracao.NFTP
            };

            return integracaosIndividuaisHabilitadas.Contains(tipo);
        }

        private bool TipoIntegracaoPorCTe(TipoIntegracao tipo)
        {
            List<TipoIntegracao> integracaosPorCTeHabilitadas = new List<TipoIntegracao>()
            {
                TipoIntegracao.Boticario,
                TipoIntegracao.Brado,
                TipoIntegracao.EFrete,
                TipoIntegracao.KMM
            };

            return integracaosPorCTeHabilitadas.Contains(tipo);
        }

        #endregion
    }
}
