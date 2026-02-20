using AdminMultisoftware.Dominio.Enumeradores;
using ICSharpCode.SharpZipLib.Zip;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoEDI : ServicoBase
    {
        #region Construtores        

        public IntegracaoEDI(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        public IntegracaoEDI(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #endregion

        #region Criacao Layouts

        public static void AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            if (loteContabilizacao.Empresa != null && loteContabilizacao.Empresa.TransportadorLayoutsEDI != null && loteContabilizacao.Empresa.TransportadorLayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR))
                layoutsEDITransportador = loteContabilizacao.Empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR).ToList();

            if (layoutsEDITransportador.Count <= 0)
                return;

            for (var i = 0; i < layoutsEDITransportador.Count; i++)
            {
                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutEDITransportador = layoutsEDITransportador[i];

                if (!repLoteContabilizacaoIntegracaoEDI.VerificarSeExistePorLoteContabilizacao(loteContabilizacao.Codigo, layoutEDITransportador.TipoIntegracao.Codigo, layoutEDITransportador.LayoutEDI.Codigo))
                {
                    Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI loteContabilizacaoIntegracaoEDI = new Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI();

                    loteContabilizacaoIntegracaoEDI.LoteContabilizacao = loteContabilizacao;
                    loteContabilizacaoIntegracaoEDI.DataIntegracao = DateTime.Now;
                    loteContabilizacaoIntegracaoEDI.NumeroTentativas = 0;
                    loteContabilizacaoIntegracaoEDI.ProblemaIntegracao = string.Empty;
                    loteContabilizacaoIntegracaoEDI.Empresa = loteContabilizacao.Empresa;
                    loteContabilizacaoIntegracaoEDI.TipoIntegracao = layoutEDITransportador.TipoIntegracao;

                    if (layoutEDITransportador.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutEDITransportador.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    loteContabilizacaoIntegracaoEDI.LayoutEDI = layoutEDITransportador.LayoutEDI;

                    repLoteContabilizacaoIntegracaoEDI.Inserir(loteContabilizacaoIntegracaoEDI);
                }
            }
        }

        public static void AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

            //Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.Entidades.Cliente tomador = loteEscrituracao.Tomador;

            int modeloDocumento = loteEscrituracao.ModeloDocumentoFiscal?.Codigo ?? 0;

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = repDocumentoEscrituracao.ObterTipoOperacaoPadraoEscrituracao(loteEscrituracao.Codigo);
            tiposOperacao.AddRange(repDocumentoEscrituracao.ObterTipoOperacaoPadraoEscrituracaoOcorrencia(loteEscrituracao.Codigo));

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsEDITipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacao)
            {
                if (tipoOperacao != null && tipoOperacao.LayoutsEDI != null)
                {
                    if (tipoOperacao.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF))
                        layoutsEDITipoOperacao.AddRange(tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF).ToList());
                }
            }

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            if (tomador?.LayoutsEDI != null && tomador.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF))
                layoutsEDIIntegracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF).ToList();
            else if (tomador?.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null && tomador.GrupoPessoas.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF))
                layoutsEDIIntegracaoGrupo = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF).ToList();

            if (loteEscrituracao.Empresa?.TransportadorLayoutsEDI != null && loteEscrituracao.Empresa.TransportadorLayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF))
                layoutsEDITransportador = loteEscrituracao.Empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF).ToList();

            if (layoutsEDIIntegracaoCliente.Count <= 0 && layoutsEDIIntegracaoGrupo.Count <= 0 && layoutsEDITransportador.Count <= 0 && layoutsEDITipoOperacao.Count <= 0)
                return;

            for (var i = 0; i < layoutsEDIIntegracaoCliente.Count; i++)
            {
                if (layoutsEDIIntegracaoCliente[i].LayoutEDI.ModeloDocumentoFiscais.Count() > 0 && !layoutsEDIIntegracaoCliente[i].LayoutEDI.ModeloDocumentoFiscais.Any(obj => obj.Codigo == modeloDocumento))
                    continue;

                if (!repEscrituracaoEDIIntegracao.VerificarSeExistePorLoteEscrituracao(loteEscrituracao.Codigo, layoutsEDIIntegracaoCliente[i].TipoIntegracao.Codigo, layoutsEDIIntegracaoCliente[i].LayoutEDI.Codigo, tomador?.CPF_CNPJ ?? 0D))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao EscrituracaoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao();
                    EscrituracaoEDIIntegracao.LoteEscrituracao = loteEscrituracao;
                    EscrituracaoEDIIntegracao.DataIntegracao = DateTime.Now;
                    EscrituracaoEDIIntegracao.NumeroTentativas = 0;
                    EscrituracaoEDIIntegracao.ProblemaIntegracao = string.Empty;
                    EscrituracaoEDIIntegracao.TipoIntegracao = layoutsEDIIntegracaoCliente[i].TipoIntegracao;

                    if (layoutsEDIIntegracaoCliente[i].TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutsEDIIntegracaoCliente[i].TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        EscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        EscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    EscrituracaoEDIIntegracao.LayoutEDI = layoutsEDIIntegracaoCliente[i].LayoutEDI;

                    repEscrituracaoEDIIntegracao.Inserir(EscrituracaoEDIIntegracao);
                }
            }

            for (var i = 0; i < layoutsEDITipoOperacao.Count; i++)
            {
                if (layoutsEDITipoOperacao[i].LayoutEDI.ModeloDocumentoFiscais.Count() > 0 && !layoutsEDITipoOperacao[i].LayoutEDI.ModeloDocumentoFiscais.Any(obj => obj.Codigo == modeloDocumento))
                    continue;

                if (!repEscrituracaoEDIIntegracao.VerificarSeExistePorLoteEscrituracao(loteEscrituracao.Codigo, layoutsEDITipoOperacao[i].TipoIntegracao.Codigo, layoutsEDITipoOperacao[i].LayoutEDI.Codigo, tomador?.CPF_CNPJ ?? 0D))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao EscrituracaoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao();
                    EscrituracaoEDIIntegracao.LoteEscrituracao = loteEscrituracao;
                    EscrituracaoEDIIntegracao.DataIntegracao = DateTime.Now;
                    EscrituracaoEDIIntegracao.NumeroTentativas = 0;
                    EscrituracaoEDIIntegracao.ProblemaIntegracao = string.Empty;
                    EscrituracaoEDIIntegracao.TipoIntegracao = layoutsEDITipoOperacao[i].TipoIntegracao;

                    if (layoutsEDITipoOperacao[i].TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutsEDITipoOperacao[i].TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        EscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        EscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    EscrituracaoEDIIntegracao.LayoutEDI = layoutsEDITipoOperacao[i].LayoutEDI;

                    repEscrituracaoEDIIntegracao.Inserir(EscrituracaoEDIIntegracao);
                }
            }

            for (var i = 0; i < layoutsEDIIntegracaoGrupo.Count; i++)
            {
                if (layoutsEDIIntegracaoGrupo[i].LayoutEDI.ModeloDocumentoFiscais.Count() > 0 && !layoutsEDIIntegracaoGrupo[i].LayoutEDI.ModeloDocumentoFiscais.Any(obj => obj.Codigo == modeloDocumento))
                    continue;

                if (!repEscrituracaoEDIIntegracao.VerificarSeExistePorLoteEscrituracao(loteEscrituracao.Codigo, layoutsEDIIntegracaoGrupo[i].TipoIntegracao.Codigo, layoutsEDIIntegracaoGrupo[i].LayoutEDI.Codigo, tomador?.CPF_CNPJ ?? 0D))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao EscrituracaoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao();
                    EscrituracaoEDIIntegracao.LoteEscrituracao = loteEscrituracao;
                    EscrituracaoEDIIntegracao.DataIntegracao = DateTime.Now;
                    EscrituracaoEDIIntegracao.NumeroTentativas = 0;
                    EscrituracaoEDIIntegracao.ProblemaIntegracao = string.Empty;
                    EscrituracaoEDIIntegracao.TipoIntegracao = layoutsEDIIntegracaoGrupo[i].TipoIntegracao;

                    if (layoutsEDIIntegracaoGrupo[i].TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutsEDIIntegracaoGrupo[i].TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        EscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        EscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    EscrituracaoEDIIntegracao.LayoutEDI = layoutsEDIIntegracaoGrupo[i].LayoutEDI;

                    repEscrituracaoEDIIntegracao.Inserir(EscrituracaoEDIIntegracao);

                }
            }

            for (var i = 0; i < layoutsEDITransportador.Count; i++)
            {
                if (layoutsEDITransportador[i].LayoutEDI.ModeloDocumentoFiscais.Count() > 0 && !layoutsEDITransportador[i].LayoutEDI.ModeloDocumentoFiscais.Any(obj => obj.Codigo == modeloDocumento))
                    continue;

                if (!repEscrituracaoEDIIntegracao.VerificarSeExistePorLoteEscrituracao(loteEscrituracao.Codigo, layoutsEDITransportador[i].TipoIntegracao.Codigo, layoutsEDITransportador[i].LayoutEDI.Codigo, tomador?.CPF_CNPJ ?? 0D))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao EscrituracaoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao();
                    EscrituracaoEDIIntegracao.LoteEscrituracao = loteEscrituracao;
                    EscrituracaoEDIIntegracao.DataIntegracao = DateTime.Now;
                    EscrituracaoEDIIntegracao.NumeroTentativas = 0;
                    EscrituracaoEDIIntegracao.ProblemaIntegracao = string.Empty;
                    EscrituracaoEDIIntegracao.Empresa = loteEscrituracao.Empresa;
                    EscrituracaoEDIIntegracao.TipoIntegracao = layoutsEDITransportador[i].TipoIntegracao;

                    if (layoutsEDITransportador[i].TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutsEDITransportador[i].TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        EscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        EscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    EscrituracaoEDIIntegracao.LayoutEDI = layoutsEDITransportador[i].LayoutEDI;

                    repEscrituracaoEDIIntegracao.Inserir(EscrituracaoEDIIntegracao);
                }
            }
        }

        public static void AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(unitOfWork);

            Dominio.Entidades.Cliente tomador = loteEscrituracaoCancelamento.Tomador;

            int modeloDocumento = loteEscrituracaoCancelamento.ModeloDocumentoFiscal?.Codigo ?? 0;

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = repDocumentoEscrituracaoCancelamento.ObterTipoOperacaoPadraoEscrituracao(loteEscrituracaoCancelamento.Codigo);
            tiposOperacao.AddRange(repDocumentoEscrituracaoCancelamento.ObterTipoOperacaoPadraoEscrituracaoOcorrencia(loteEscrituracaoCancelamento.Codigo));

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsEDITipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacao)
            {
                if (tipoOperacao?.LayoutsEDI != null && tipoOperacao.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento))
                    layoutsEDITipoOperacao.AddRange(tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento).ToList());
            }

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            if (tomador?.LayoutsEDI != null && tomador.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento))
                layoutsEDIIntegracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento).ToList();
            else if (tomador?.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null && tomador.GrupoPessoas.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento))
                layoutsEDIIntegracaoGrupo = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento).ToList();

            if (loteEscrituracaoCancelamento.Empresa?.TransportadorLayoutsEDI != null && loteEscrituracaoCancelamento.Empresa.TransportadorLayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento))
                layoutsEDITransportador = loteEscrituracaoCancelamento.Empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento).ToList();

            int countLayoutEDICliente = layoutsEDIIntegracaoCliente.Count();
            int countLayoutEDIGrupo = layoutsEDIIntegracaoGrupo.Count();
            int countLayoutEDITipoOperacao = layoutsEDITipoOperacao.Count();
            int countLayoutEDITransportador = layoutsEDITransportador.Count();

            if (countLayoutEDICliente <= 0 && countLayoutEDIGrupo <= 0 && countLayoutEDITransportador <= 0 && countLayoutEDITipoOperacao <= 0)
                return;

            for (var i = 0; i < countLayoutEDICliente; i++)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI layoutEDICliente = layoutsEDIIntegracaoCliente[i];

                if (layoutEDICliente.LayoutEDI.ModeloDocumentoFiscais.Count() > 0 && !layoutEDICliente.LayoutEDI.ModeloDocumentoFiscais.Any(obj => obj.Codigo == modeloDocumento))
                    continue;

                if (!repLoteEscrituracaoCancelamentoEDIIntegracao.VerificarSeExistePorLoteEscrituracao(loteEscrituracaoCancelamento.Codigo, layoutEDICliente.TipoIntegracao.Codigo, layoutEDICliente.LayoutEDI.Codigo))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao
                    {
                        LoteEscrituracaoCancelamento = loteEscrituracaoCancelamento,
                        DataIntegracao = DateTime.Now,
                        NumeroTentativas = 0,
                        ProblemaIntegracao = string.Empty,
                        TipoIntegracao = layoutEDICliente.TipoIntegracao,
                        LayoutEDI = layoutEDICliente.LayoutEDI
                    };

                    if (layoutEDICliente.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutEDICliente.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repLoteEscrituracaoCancelamentoEDIIntegracao.Inserir(loteEscrituracaoCancelamentoEDIIntegracao);
                }
            }

            for (var i = 0; i < countLayoutEDITipoOperacao; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI layoutEDITipoOperacao = layoutsEDITipoOperacao[i];

                if (layoutEDITipoOperacao.LayoutEDI.ModeloDocumentoFiscais.Count() > 0 && !layoutEDITipoOperacao.LayoutEDI.ModeloDocumentoFiscais.Any(obj => obj.Codigo == modeloDocumento))
                    continue;

                if (!repLoteEscrituracaoCancelamentoEDIIntegracao.VerificarSeExistePorLoteEscrituracao(loteEscrituracaoCancelamento.Codigo, layoutEDITipoOperacao.TipoIntegracao.Codigo, layoutEDITipoOperacao.LayoutEDI.Codigo))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao
                    {
                        LoteEscrituracaoCancelamento = loteEscrituracaoCancelamento,
                        DataIntegracao = DateTime.Now,
                        NumeroTentativas = 0,
                        ProblemaIntegracao = string.Empty,
                        TipoIntegracao = layoutEDITipoOperacao.TipoIntegracao,
                        LayoutEDI = layoutEDITipoOperacao.LayoutEDI
                    };

                    if (layoutEDITipoOperacao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutEDITipoOperacao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repLoteEscrituracaoCancelamentoEDIIntegracao.Inserir(loteEscrituracaoCancelamentoEDIIntegracao);
                }
            }

            for (var i = 0; i < countLayoutEDIGrupo; i++)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI grupoPessoasLayoutEDI = layoutsEDIIntegracaoGrupo[i];

                if (grupoPessoasLayoutEDI.LayoutEDI.ModeloDocumentoFiscais.Count() > 0 && !grupoPessoasLayoutEDI.LayoutEDI.ModeloDocumentoFiscais.Any(obj => obj.Codigo == modeloDocumento))
                    continue;

                if (!repLoteEscrituracaoCancelamentoEDIIntegracao.VerificarSeExistePorLoteEscrituracao(loteEscrituracaoCancelamento.Codigo, grupoPessoasLayoutEDI.TipoIntegracao.Codigo, grupoPessoasLayoutEDI.LayoutEDI.Codigo))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao
                    {
                        LoteEscrituracaoCancelamento = loteEscrituracaoCancelamento,
                        DataIntegracao = DateTime.Now,
                        NumeroTentativas = 0,
                        ProblemaIntegracao = string.Empty,
                        TipoIntegracao = grupoPessoasLayoutEDI.TipoIntegracao,
                        LayoutEDI = grupoPessoasLayoutEDI.LayoutEDI
                    };

                    if (grupoPessoasLayoutEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        grupoPessoasLayoutEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repLoteEscrituracaoCancelamentoEDIIntegracao.Inserir(loteEscrituracaoCancelamentoEDIIntegracao);
                }
            }

            for (var i = 0; i < countLayoutEDITransportador; i++)
            {
                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutEDITransportador = layoutsEDITransportador[i];

                if (layoutEDITransportador.LayoutEDI.ModeloDocumentoFiscais.Count() > 0 && !layoutEDITransportador.LayoutEDI.ModeloDocumentoFiscais.Any(obj => obj.Codigo == modeloDocumento))
                    continue;

                if (!repLoteEscrituracaoCancelamentoEDIIntegracao.VerificarSeExistePorLoteEscrituracao(loteEscrituracaoCancelamento.Codigo, layoutEDITransportador.TipoIntegracao.Codigo, layoutEDITransportador.LayoutEDI.Codigo))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao
                    {
                        LoteEscrituracaoCancelamento = loteEscrituracaoCancelamento,
                        DataIntegracao = DateTime.Now,
                        NumeroTentativas = 0,
                        ProblemaIntegracao = string.Empty,
                        Empresa = loteEscrituracaoCancelamento.Empresa,
                        TipoIntegracao = layoutEDITransportador.TipoIntegracao,
                        LayoutEDI = layoutEDITransportador.LayoutEDI
                    };

                    if (layoutEDITransportador.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutEDITransportador.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repLoteEscrituracaoCancelamentoEDIIntegracao.Inserir(loteEscrituracaoCancelamentoEDIIntegracao);
                }
            }
        }

        public static void AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            //Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.Entidades.Cliente tomador = lancamentoNFSManual.Tomador;

            bool transportadorGeraLoteEscrituracao = lancamentoNFSManual.Transportador?.GerarLoteEscrituracao ?? false;

            bool disponibilizarDocumentosParaLoteEscrituracao = tomador.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracao ?? false;

            if ((tomador.DisponibilizarDocumentosParaLoteEscrituracao || disponibilizarDocumentosParaLoteEscrituracao) && (!tomador.DataViradaProvisao.HasValue || tomador.DataViradaProvisao.Value < lancamentoNFSManual.DataCriacao))
                disponibilizarDocumentosParaLoteEscrituracao = true;

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            if (tomador.LayoutsEDI != null &&
                tomador.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB ||
                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB ||
                                        o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP ||
                                        o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP ||
                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE ||
                                       ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC ||
                                         o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) &&
                                         !disponibilizarDocumentosParaLoteEscrituracao &&
                                         !transportadorGeraLoteEscrituracao)))
                layoutsEDIIntegracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentosParaLoteEscrituracao && !transportadorGeraLoteEscrituracao)).ToList();
            else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null && tomador.GrupoPessoas.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentosParaLoteEscrituracao && !transportadorGeraLoteEscrituracao)))
                layoutsEDIIntegracaoGrupo = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentosParaLoteEscrituracao && !transportadorGeraLoteEscrituracao)).ToList();

            if (lancamentoNFSManual.Transportador.TransportadorLayoutsEDI != null && lancamentoNFSManual.Transportador.TransportadorLayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE) || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentosParaLoteEscrituracao && !transportadorGeraLoteEscrituracao)))
                layoutsEDITransportador = lancamentoNFSManual.Transportador.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE) || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentosParaLoteEscrituracao && !transportadorGeraLoteEscrituracao)).ToList();

            if (layoutsEDIIntegracaoCliente.Count <= 0 && layoutsEDIIntegracaoGrupo.Count <= 0 && layoutsEDITransportador.Count <= 0)
                return;

            for (var i = 0; i < layoutsEDITransportador.Count; i++)
                AdicionarLancamentoNFSManualEDIIntegracao(lancamentoNFSManual, tomador, layoutsEDITransportador[i].LayoutEDI, layoutsEDITransportador[i].TipoIntegracao, lancamentoNFSManual.Transportador, unitOfWork);

            for (var i = 0; i < layoutsEDIIntegracaoCliente.Count; i++)
                AdicionarLancamentoNFSManualEDIIntegracao(lancamentoNFSManual, tomador, layoutsEDIIntegracaoCliente[i].LayoutEDI, layoutsEDIIntegracaoCliente[i].TipoIntegracao, null, unitOfWork);

            for (var i = 0; i < layoutsEDIIntegracaoGrupo.Count; i++)
                AdicionarLancamentoNFSManualEDIIntegracao(lancamentoNFSManual, tomador, layoutsEDIIntegracaoGrupo[i].LayoutEDI, layoutsEDIIntegracaoGrupo[i].TipoIntegracao, null, unitOfWork);

        }

        public static void AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente loteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorLayoutEDI repTransportadorLayoutEDI = new Repositorio.Embarcador.Transportadores.TransportadorLayoutEDI(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = repTransportadorLayoutEDI.BuscarPorTipoLayoutEDI(Dominio.Enumeradores.TipoLayoutEDI.Cliente);

            if (layoutsEDITransportador.Count <= 0)
                return;

            for (var i = 0; i < layoutsEDITransportador.Count; i++)
            {
                Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutEDITransportador = layoutsEDITransportador[i];

                if (!repLoteClienteIntegracaoEDI.VerificarSeExistePorLoteCliente(loteCliente.Codigo, layoutEDITransportador.TipoIntegracao.Codigo, layoutEDITransportador.LayoutEDI.Codigo))
                {
                    Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI loteClienteIntegracaoEDI = new Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI();

                    loteClienteIntegracaoEDI.LoteCliente = loteCliente;
                    loteClienteIntegracaoEDI.DataIntegracao = DateTime.Now;
                    loteClienteIntegracaoEDI.NumeroTentativas = 0;
                    loteClienteIntegracaoEDI.ProblemaIntegracao = string.Empty;
                    loteClienteIntegracaoEDI.Empresa = layoutEDITransportador.Empresa;
                    loteClienteIntegracaoEDI.TipoIntegracao = layoutEDITransportador.TipoIntegracao;

                    if (layoutEDITransportador.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        layoutEDITransportador.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        loteClienteIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        loteClienteIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    loteClienteIntegracaoEDI.LayoutEDI = layoutEDITransportador.LayoutEDI;

                    repLoteClienteIntegracaoEDI.Inserir(loteClienteIntegracaoEDI);
                }
            }
        }

        private static void AdicionarLancamentoNFSManualEDIIntegracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Cliente tomador, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(unitOfWork);

            if (layoutEDI.ModeloDocumentoFiscais != null && layoutEDI.ModeloDocumentoFiscais.Count > 0)
            {
                if (!layoutEDI.ModeloDocumentoFiscais.Contains(lancamentoNFSManual.CTe.ModeloDocumentoFiscal))
                    return;
            }

            if (!repNFSManualEDIIntegracao.VerificarSeExistePorLancamentoNFSManual(lancamentoNFSManual.Codigo, tipoIntegracao.Codigo, layoutEDI.Codigo, tomador.CPF_CNPJ))
            {
                Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao nfsManualEDIIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao();
                nfsManualEDIIntegracao.LancamentoNFSManual = lancamentoNFSManual;
                nfsManualEDIIntegracao.DataIntegracao = DateTime.Now;
                nfsManualEDIIntegracao.NumeroTentativas = 0;
                nfsManualEDIIntegracao.ProblemaIntegracao = string.Empty;
                nfsManualEDIIntegracao.Empresa = empresa;
                nfsManualEDIIntegracao.TipoIntegracao = tipoIntegracao;

                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                    tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                    nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                else
                    nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                nfsManualEDIIntegracao.LayoutEDI = layoutEDI;

                repNFSManualEDIIntegracao.Inserir(nfsManualEDIIntegracao);
            }

        }

        public static void AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

            Dominio.Entidades.Cliente tomador = nfsManualCancelamento.LancamentoNFSManual.Tomador;

            if (tomador == null)
                return;

            bool transportadorGeraLoteEscrituracao = nfsManualCancelamento.LancamentoNFSManual.Transportador?.GerarLoteEscrituracao ?? false;
            bool disponibilizarDocumentosParaLoteEscrituracao = tomador.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracao ?? false;

            if ((tomador.DisponibilizarDocumentosParaLoteEscrituracao || disponibilizarDocumentosParaLoteEscrituracao || transportadorGeraLoteEscrituracao) && (!tomador.DataViradaProvisao.HasValue || tomador.DataViradaProvisao.Value < nfsManualCancelamento.LancamentoNFSManual.DataCriacao))
                disponibilizarDocumentosParaLoteEscrituracao = true;

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            if (tomador.LayoutsEDI != null && tomador.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && !disponibilizarDocumentosParaLoteEscrituracao)))
                layoutsEDIIntegracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && !disponibilizarDocumentosParaLoteEscrituracao)).ToList();
            else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null && tomador.GrupoPessoas.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && !disponibilizarDocumentosParaLoteEscrituracao)))
                layoutsEDIIntegracaoGrupo = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && !disponibilizarDocumentosParaLoteEscrituracao)).ToList();

            if (nfsManualCancelamento.LancamentoNFSManual.Transportador.TransportadorLayoutsEDI != null && nfsManualCancelamento.LancamentoNFSManual.Transportador.TransportadorLayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO) && !disponibilizarDocumentosParaLoteEscrituracao)))
                layoutsEDITransportador = nfsManualCancelamento.LancamentoNFSManual.Transportador.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO) && !disponibilizarDocumentosParaLoteEscrituracao)).ToList();

            if (layoutsEDIIntegracaoCliente.Count() <= 0 && layoutsEDIIntegracaoGrupo.Count() <= 0 && layoutsEDITransportador.Count() <= 0)
                return;

            for (int i = 0; i < layoutsEDITransportador.Count; i++)
                AdicionarLancamentoCancelamentoNFSManualEDIIntegracao(nfsManualCancelamento, tomador, layoutsEDITransportador[i].LayoutEDI, layoutsEDITransportador[i].TipoIntegracao, nfsManualCancelamento.LancamentoNFSManual.Transportador, unitOfWork);

            for (int i = 0; i < layoutsEDIIntegracaoCliente.Count; i++)
                AdicionarLancamentoCancelamentoNFSManualEDIIntegracao(nfsManualCancelamento, tomador, layoutsEDIIntegracaoCliente[i].LayoutEDI, layoutsEDIIntegracaoCliente[i].TipoIntegracao, null, unitOfWork);

            for (int i = 0; i < layoutsEDIIntegracaoGrupo.Count; i++)
                AdicionarLancamentoCancelamentoNFSManualEDIIntegracao(nfsManualCancelamento, tomador, layoutsEDIIntegracaoGrupo[i].LayoutEDI, layoutsEDIIntegracaoGrupo[i].TipoIntegracao, null, unitOfWork);
        }

        private static void AdicionarLancamentoCancelamentoNFSManualEDIIntegracao(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Dominio.Entidades.Cliente tomador, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            if (nfsManualCancelamento.LancamentoNFSManual.CTe == null)
                return;

            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unitOfWork);

            if (layoutEDI.ModeloDocumentoFiscais != null && layoutEDI.ModeloDocumentoFiscais.Count > 0)
            {
                if (!layoutEDI.ModeloDocumentoFiscais.Contains(nfsManualCancelamento.LancamentoNFSManual.CTe.ModeloDocumentoFiscal))
                    return;
            }

            if (!repNFSManualCancelamentoIntegracaoEDI.VerificarSeExistePorNFSManualCancelamento(nfsManualCancelamento.Codigo, tipoIntegracao.Codigo, layoutEDI.Codigo, tomador.CPF_CNPJ))
            {
                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI nfsManualCancelamentoIntegracaoEDI = new Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI
                {
                    NFSManualCancelamento = nfsManualCancelamento,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    Empresa = empresa,
                    ProblemaIntegracao = string.Empty,
                    TipoIntegracao = tipoIntegracao,
                    LayoutEDI = layoutEDI
                };

                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                    tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                    nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                else
                    nfsManualCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repNFSManualCancelamentoIntegracaoEDI.Inserir(nfsManualCancelamentoIntegracaoEDI);
            }

        }

        public static bool AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiIntegracao = false;

            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = pedidoOcorrenciaColetaEntrega.Pedido?.TipoOperacao;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsEDIIntegracaoTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();


            if (tipoOperacao != null)
            {
                if (tipoOperacao.LayoutsEDI != null && tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN).Count() > 0)
                    layoutsEDIIntegracaoTipoOperacao = tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN).ToList();
            }

            Dominio.Entidades.Cliente tomador = pedidoOcorrenciaColetaEntrega.Pedido.ObterTomador();

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();

            if (tomador.LayoutsEDI.Count > 0)
                layoutsEDIIntegracaoCliente = (from o in tomador.LayoutsEDI where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN select o).ToList();
            else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null)
                layoutsEDIIntegracaoGrupo = (from o in tomador.GrupoPessoas.LayoutsEDI where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN select o).ToList();

            for (var i = 0; i < layoutsEDIIntegracaoCliente.Count; i++)
            {
                AdicionarPedidoOcorrenciaColetaEntregaEDIIntegracao(pedidoOcorrenciaColetaEntrega, layoutsEDIIntegracaoCliente[i].LayoutEDI, layoutsEDIIntegracaoCliente[i].TipoIntegracao, unitOfWork);
                possuiIntegracao = true;
            }

            for (var i = 0; i < layoutsEDIIntegracaoGrupo.Count; i++)
            {
                AdicionarPedidoOcorrenciaColetaEntregaEDIIntegracao(pedidoOcorrenciaColetaEntrega, layoutsEDIIntegracaoGrupo[i].LayoutEDI, layoutsEDIIntegracaoGrupo[i].TipoIntegracao, unitOfWork);
                possuiIntegracao = true;
            }

            for (var i = 0; i < layoutsEDIIntegracaoTipoOperacao.Count; i++)
            {
                AdicionarPedidoOcorrenciaColetaEntregaEDIIntegracao(pedidoOcorrenciaColetaEntrega, layoutsEDIIntegracaoTipoOperacao[i].LayoutEDI, layoutsEDIIntegracaoTipoOperacao[i].TipoIntegracao, unitOfWork);
                possuiIntegracao = true;
            }

            return possuiIntegracao;
        }

        private static void AdicionarPedidoOcorrenciaColetaEntregaEDIIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao();
            pedidoOcorrenciaColetaEntregaIntegracao.Initialize();
            pedidoOcorrenciaColetaEntregaIntegracao.PedidoOcorrenciaColetaEntrega = pedidoOcorrenciaColetaEntrega;
            if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada || tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao)
                pedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            else
                pedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

            pedidoOcorrenciaColetaEntregaIntegracao.DataIntegracao = DateTime.Now;
            pedidoOcorrenciaColetaEntregaIntegracao.NumeroTentativas = 0;
            pedidoOcorrenciaColetaEntregaIntegracao.ProblemaIntegracao = "";
            pedidoOcorrenciaColetaEntregaIntegracao.TipoIntegracao = tipoIntegracao;
            pedidoOcorrenciaColetaEntregaIntegracao.LayoutEDI = layoutEDI;

            repPedidoOcorrenciaColetaEntregaIntegracao.Inserir(pedidoOcorrenciaColetaEntregaIntegracao);

            // Auditoria
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };
            Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoOcorrenciaColetaEntregaIntegracao, pedidoOcorrenciaColetaEntregaIntegracao.GetChanges(), "Registro de integração.", unitOfWork);

        }

        public static bool AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool filialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiIntegracao = false;

            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = ocorrencia.Carga?.TipoOperacao;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsEDIIntegracaoTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> complementos = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrencia.Codigo);

            bool transportadorGeraLoteEscrituracao = ocorrencia?.Carga?.Empresa?.GerarLoteEscrituracao ?? false;
            bool possuiNotaDebito = complementos.Any(c => c.CTe != null && c.CTe.ModeloDocumentoFiscal != null && c.CTe.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Debito);
            bool disponibilizarDocumentosParaLoteEscrituracao = (ocorrencia?.Carga?.TipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracao ?? false) || transportadorGeraLoteEscrituracao;

            bool gerouDocumentos = complementos.Any(c => c.CTe != null);

            if (tipoOperacao != null && !filialEmissora)
            {
                if (tipoOperacao.LayoutsEDI != null && tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN || (((((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB && o.LayoutEDI.GerarEDIEmOcorrencias) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF && o.LayoutEDI.GerarEDIEmOcorrencias) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP && o.LayoutEDI.GerarEDIEmOcorrencias) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP && o.LayoutEDI.GerarEDIEmOcorrencias) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB && o.LayoutEDI.GerarEDIEmOcorrencias))) || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DESPESACOMPLEMENTAR) && ocorrencia.ComponenteFrete != null)).Count() > 0)
                    layoutsEDIIntegracaoTipoOperacao = tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN || (((((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB && o.LayoutEDI.GerarEDIEmOcorrencias) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF && o.LayoutEDI.GerarEDIEmOcorrencias) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP && o.LayoutEDI.GerarEDIEmOcorrencias) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP && o.LayoutEDI.GerarEDIEmOcorrencias) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB && o.LayoutEDI.GerarEDIEmOcorrencias))) || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DESPESACOMPLEMENTAR) && ocorrencia.ComponenteFrete != null)).ToList();
            }

            List<Dominio.Entidades.Cliente> tomadores = ocorrencia.Carga != null ? (from obj in ocorrencia.Carga.CargaOrigemPedidos select obj.ObterTomador()).Distinct().ToList() : new List<Dominio.Entidades.Cliente>();
            foreach (Dominio.Entidades.Cliente tomador in tomadores)
            {
                if (tomador == null)
                    continue;

                bool disponibilizarDocumentoEscrituracaoTomador = disponibilizarDocumentosParaLoteEscrituracao;
                if (!disponibilizarDocumentoEscrituracaoTomador)
                {
                    bool grupoDisponibilizarDocumentosParaLoteEscrituracao = tomador.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracao ?? false;
                    if ((tomador.DisponibilizarDocumentosParaLoteEscrituracao || grupoDisponibilizarDocumentosParaLoteEscrituracao) && (!tomador.DataViradaProvisao.HasValue || tomador.DataViradaProvisao.Value < ocorrencia.DataOcorrencia))
                        disponibilizarDocumentoEscrituracaoTomador = true;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao> layoutsEDIIntegracaoCarga = new List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDIFilialEmissora = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();

                // Filtros padrões
                List<Dominio.Enumeradores.TipoLayoutEDI> tiposLayoutCONEMB = new List<Dominio.Enumeradores.TipoLayoutEDI>() { Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP, Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP, Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB, Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF };
                if (!disponibilizarDocumentoEscrituracaoTomador) tiposLayoutCONEMB.Add(Dominio.Enumeradores.TipoLayoutEDI.CONEMB);
                Dominio.Entidades.Empresa empresaFilialEmissora = ocorrencia.Carga?.EmpresaFilialEmissora;

                if (filialEmissora)
                {
                    if (gerouDocumentos)
                    {
                        // Tomador
                        if (tomador.LayoutsEDI.Count > 0 && ocorrencia.ComponenteFrete != null)
                        {
                            layoutsEDIIntegracaoCliente = (from o in tomador.LayoutsEDI
                                                           where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF
                                                           select o).ToList();
                        }
                        // Grupo de Pessoa
                        else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null && ocorrencia.ComponenteFrete != null)
                        {
                            layoutsEDIIntegracaoGrupo = (from o in tomador.GrupoPessoas.LayoutsEDI
                                                         where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF
                                                         select o).ToList();
                        }


                        if (empresaFilialEmissora == null)
                        {
                            if (ocorrencia.Carga?.Filial?.EmpresaEmissora != null)
                                empresaFilialEmissora = ocorrencia.Carga.Filial.EmpresaEmissora;
                        }

                        // Filial Emissora
                        if (empresaFilialEmissora?.TransportadorLayoutsEDI != null && ocorrencia.ComponenteFrete != null)
                        {
                            layoutsEDIFilialEmissora = (from o in empresaFilialEmissora.TransportadorLayoutsEDI
                                                        where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF
                                                        select o).ToList();
                        }

                        //// Transportador
                        //if (ocorrencia.Carga?.Empresa?.TransportadorLayoutsEDI != null)
                        //{
                        //    layoutsEDITransportador = (from o in ocorrencia.Carga.Empresa.TransportadorLayoutsEDI
                        //                               where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC
                        //                               select o).ToList();

                        //}
                    }
                }
                else
                {
                    // Tomador
                    if (tomador.LayoutsEDI.Count > 0)
                    {
                        layoutsEDIIntegracaoCliente = (from o in tomador.LayoutsEDI
                                                       where
                                                        o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN
                                                        ||
                                                        o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OcorenOTIF
                                                        ||
                                                        (
                                                            (
                                                                (o.LayoutEDI.GerarEDIEmOcorrencias && tiposLayoutCONEMB.Contains(o.LayoutEDI.Tipo))
                                                                ||
                                                                (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DESPESACOMPLEMENTAR && !disponibilizarDocumentoEscrituracaoTomador)
                                                            )
                                                            && ocorrencia.ComponenteFrete != null
                                                        )
                                                        || (gerouDocumentos && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF)
                                                        && !disponibilizarDocumentosParaLoteEscrituracao)
                                                       select o).ToList();
                    }
                    // Grupo de Pessoa
                    else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null)
                    {
                        layoutsEDIIntegracaoGrupo = (from o in tomador.GrupoPessoas.LayoutsEDI
                                                     where
                                                        o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN
                                                        ||
                                                        o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OcorenOTIF
                                                        ||
                                                        (
                                                            (
                                                                (o.LayoutEDI.GerarEDIEmOcorrencias && tiposLayoutCONEMB.Contains(o.LayoutEDI.Tipo))
                                                                ||
                                                                (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DESPESACOMPLEMENTAR && !disponibilizarDocumentoEscrituracaoTomador)
                                                            )
                                                            && ocorrencia.ComponenteFrete != null
                                                        )
                                                        || (gerouDocumentos && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF)
                                                        && !disponibilizarDocumentosParaLoteEscrituracao)
                                                     select o).ToList();
                    }

                    // Transportador
                    if (ocorrencia.Carga?.Empresa?.TransportadorLayoutsEDI != null)
                    {
                        layoutsEDITransportador = (from o in ocorrencia.Carga.Empresa.TransportadorLayoutsEDI
                                                   where
                                                        o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN
                                                        ||
                                                        o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OcorenOTIF
                                                        ||
                                                        (
                                                            (
                                                                (o.LayoutEDI.GerarEDIEmOcorrencias && tiposLayoutCONEMB.Contains(o.LayoutEDI.Tipo))
                                                                ||
                                                                (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DESPESACOMPLEMENTAR && !disponibilizarDocumentoEscrituracaoTomador)
                                                                ||
                                                                (gerouDocumentos && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC
                                                                ||
                                                                o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentosParaLoteEscrituracao)
                                                            )
                                                            && ocorrencia.ComponenteFrete != null
                                                        )
                                                   select o).ToList();

                    }
                }

                if (layoutsEDIIntegracaoCliente.Count <= 0 && layoutsEDIIntegracaoGrupo.Count <= 0 && layoutsEDIFilialEmissora.Count <= 0 && layoutsEDITransportador.Count <= 0 && layoutsEDIIntegracaoTipoOperacao.Count <= 0) // && layoutsEDIIntegracaoCarga.Count <= 0
                    continue;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = ocorrencia.Carga != null ? (from obj in ocorrencia.Carga.CargaOrigemPedidos where obj.ObterTomador().Equals(tomador) select obj).ToList() : new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                for (var i = 0; i < layoutsEDIIntegracaoCliente.Count; i++)
                {
                    bool retornoCriacao = AdicionarOcorrenciaEDIIntegracao(ocorrencia, pedidos, layoutsEDIIntegracaoCliente[i].LayoutEDI, layoutsEDIIntegracaoCliente[i].TipoIntegracao, null, filialEmissora, unitOfWork);
                    if (retornoCriacao)
                        possuiIntegracao = true;
                }

                for (var i = 0; i < layoutsEDIIntegracaoGrupo.Count; i++)
                {
                    bool retornoCriacao = AdicionarOcorrenciaEDIIntegracao(ocorrencia, pedidos, layoutsEDIIntegracaoGrupo[i].LayoutEDI, layoutsEDIIntegracaoGrupo[i].TipoIntegracao, null, filialEmissora, unitOfWork);
                    if (retornoCriacao)
                        possuiIntegracao = true;
                }

                for (var i = 0; i < layoutsEDITransportador.Count; i++)
                {
                    bool retornoCriacao = AdicionarOcorrenciaEDIIntegracao(ocorrencia, pedidos, layoutsEDITransportador[i].LayoutEDI, layoutsEDITransportador[i].TipoIntegracao, ocorrencia.Carga?.Empresa, filialEmissora, unitOfWork);
                    if (retornoCriacao)
                        possuiIntegracao = true;
                }

                for (var i = 0; i < layoutsEDIFilialEmissora.Count; i++)
                {
                    bool retornoCriacao = AdicionarOcorrenciaEDIIntegracao(ocorrencia, pedidos, layoutsEDIFilialEmissora[i].LayoutEDI, layoutsEDIFilialEmissora[i].TipoIntegracao, empresaFilialEmissora, filialEmissora, unitOfWork);
                    if (retornoCriacao)
                        possuiIntegracao = true;
                }
            }

            for (var i = 0; i < layoutsEDIIntegracaoTipoOperacao.Count; i++)
            {
                bool retornoCriacao = AdicionarOcorrenciaEDIIntegracao(ocorrencia, ocorrencia.Carga.CargaOrigemPedidos.ToList(), layoutsEDIIntegracaoTipoOperacao[i].LayoutEDI, layoutsEDIIntegracaoTipoOperacao[i].TipoIntegracao, null, filialEmissora, unitOfWork);
                if (retornoCriacao)
                    possuiIntegracao = true;
            }

            return possuiIntegracao;
        }

        private static bool AdicionarOcorrenciaEDIIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Empresa empresa, bool filialEmissora, Repositorio.UnitOfWork unitOfWork)
        {

            if (!layoutEDI.GerarEDIPorNotaFiscal)
                return GerarRegistroCargaEDIINtegracao(ocorrencia, pedidos, layoutEDI, tipoIntegracao, empresa, filialEmissora, null, unitOfWork);
            else
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(ocorrencia.Codigo);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = (from obj in cargaOcorrenciaDocumentos where obj.CargaCTe != null select obj.CargaCTe.CTe).ToList();
                bool valido = false;
                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal in cte.XMLNotaFiscais.ToList())
                    {
                        if (ocorrencia.TipoOcorrencia.OcorrenciaExclusivaParaCanhotosDigitalizados)
                        {
                            if (xMLNotaFiscal.Canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado)
                                continue;
                        }
                        if (GerarRegistroCargaEDIINtegracao(ocorrencia, pedidos, layoutEDI, tipoIntegracao, empresa, filialEmissora, xMLNotaFiscal, unitOfWork))
                            valido = true;
                    }
                }
                return valido;
            }

        }

        private static bool GerarRegistroCargaEDIINtegracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Empresa empresa, bool filialEmissora, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unitOfWork);

            if (!repOcorrenciaEDIIntegracao.VerificarSeExistePorOcorrencia(ocorrencia.Codigo, tipoIntegracao.Codigo, layoutEDI.Codigo, xMLNotaFiscal?.Codigo ?? 0, ocorrencia.TipoOcorrencia?.Codigo ?? 0))
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao ocorrenciaEDIIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao
                {
                    CargaOcorrencia = ocorrencia,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = string.Empty,
                    Empresa = empresa,
                    TipoIntegracao = tipoIntegracao,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    Pedidos = pedidos,
                    FilialEmissora = filialEmissora,
                    LayoutEDI = layoutEDI,
                    XMLNotaFiscal = xMLNotaFiscal
                };
                repOcorrenciaEDIIntegracao.Inserir(ocorrenciaEDIIntegracao);

                return true;
            }
            return false;
        }

        private static bool VerificarSeExistemDocumentosEmitidosPorLayoutModeloNaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.LayoutEDI layoutEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCteComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            bool possui = true;
            if (layoutEDI.ModeloDocumentoFiscais != null && layoutEDI.ModeloDocumentoFiscais.Count > 0)
                possui = repCargaCteComplementoInfo.ExisteModeloDeDocumentoEmitidoNaOcorrencia(ocorrencia.Codigo, layoutEDI.ModeloDocumentoFiscais.Select(o => o.Codigo).ToList());

            return possui;
        }

        public static void AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int codigoCTe = 0)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            if (carga == null && codigoCTe > 0)
                carga = repCargaCTe.BuscarCargaPorCTe(codigoCTe);

            if (carga == null)
                return;

            if (!carga.CargaTransbordo && !carga.CargaSVM)
            {
                //todo: regra especifica para EDIs vinculados ao cadastro da filial emissora, além disso é necessário avaliar.
                bool integracaoFilialEmissora = false;
                if (carga.EmpresaFilialEmissora != null && !carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    integracaoFilialEmissora = true;

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsEDIIntegracaoTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = carga.TipoOperacao;

                bool transportadorGeraLoteEscrituracao = carga?.Empresa?.GerarLoteEscrituracao ?? false;
                bool disponibilizarDocumentosParaLoteEscrituracao = (carga?.TipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracao ?? false) || transportadorGeraLoteEscrituracao;

                if (tipoOperacao?.LayoutsEDI != null && (tipoOperacao.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentosParaLoteEscrituracao) || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS)))
                    layoutsEDIIntegracaoTipoOperacao = tipoOperacao.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentosParaLoteEscrituracao) || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS).ToList();

                List<Dominio.Entidades.Cliente> tomadores = null;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                   carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                    tomadores = carga.Pedidos.Where(o => o.Pedido?.SubContratante != null).Select(o => o.Pedido.SubContratante).Distinct().ToList();
                else
                    tomadores = carga.Pedidos.Select(o => o.ObterTomador()).Distinct().ToList();

                foreach (Dominio.Entidades.Cliente tomador in tomadores)
                {
                    if (tomador != null)
                    {
                        bool disponibilizarDocumentoEscrituracaoTomador = disponibilizarDocumentosParaLoteEscrituracao;
                        if (!disponibilizarDocumentoEscrituracaoTomador)
                        {
                            bool grupoDisponibilizarDocumentosParaLoteEscrituracao = tomador.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracao ?? false;
                            if ((tomador.DisponibilizarDocumentosParaLoteEscrituracao || grupoDisponibilizarDocumentosParaLoteEscrituracao) && (!tomador.DataViradaProvisao.HasValue || tomador.DataViradaProvisao.Value < carga.DataCriacaoCarga))
                                disponibilizarDocumentoEscrituracaoTomador = true;
                        }

                        bool filialEmissora = carga.EmpresaFilialEmissora != null ? true : false;

                        List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();
                        List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
                        List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();
                        List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDIFilialEmissora = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

                        if (tomador.LayoutsEDI != null && tomador.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && (filialEmissora || !disponibilizarDocumentoEscrituracaoTomador))))
                            layoutsEDIIntegracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && (filialEmissora || !disponibilizarDocumentoEscrituracaoTomador))).ToList();
                        else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null && tomador.GrupoPessoas.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && (filialEmissora || !disponibilizarDocumentoEscrituracaoTomador))))
                            layoutsEDIIntegracaoGrupo = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && (filialEmissora || !disponibilizarDocumentoEscrituracaoTomador))).ToList();

                        if (carga.Empresa != null && carga.Empresa.TransportadorLayoutsEDI != null &&
                            carga.Empresa.TransportadorLayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO ||
                                                                           o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM ||
                                                                           o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB ||
                                                                           o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB ||
                                                                           o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS ||
                                                                           o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP ||
                                                                           o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP ||
                                                                           o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF ||
                                                                           o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE ||
                                                                           ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentoEscrituracaoTomador)))
                            layoutsEDITransportador = carga.Empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO ||
                                                                                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM ||
                                                                                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB ||
                                                                                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB ||
                                                                                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS ||
                                                                                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP ||
                                                                                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP ||
                                                                                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF ||
                                                                                                       o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE ||
                                                                                                       ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF) && !disponibilizarDocumentoEscrituracaoTomador)).ToList();

                        if (carga.EmpresaFilialEmissora != null && carga.EmpresaFilialEmissora.TransportadorLayoutsEDI != null && carga.EmpresaFilialEmissora.TransportadorLayoutsEDI.Any(o => (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB) || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS || ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF)) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE)))
                            layoutsEDIFilialEmissora = carga.EmpresaFilialEmissora.TransportadorLayoutsEDI.Where(o => (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB) || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC) || (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE)).ToList();

                        if (layoutsEDIIntegracaoCliente.Count <= 0 && layoutsEDIIntegracaoGrupo.Count <= 0 && layoutsEDITransportador.Count <= 0 && layoutsEDIFilialEmissora.Count <= 0 && layoutsEDIIntegracaoTipoOperacao.Count < 0)
                            continue;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = carga.Pedidos.Where(obj => obj.ObterTomador()?.Equals(tomador) == true).ToList();

                        Dominio.Enumeradores.TipoLayoutEDI layoutVGM = Dominio.Enumeradores.TipoLayoutEDI.VGM;
                        bool possuiLayoutEdiVGM = layoutsEDIIntegracaoTipoOperacao.Where(obj => obj.LayoutEDI.Tipo == layoutVGM).Any() ||
                                                    layoutsEDITransportador.Where(obj => obj.LayoutEDI.Tipo == layoutVGM).Any() ||
                                                    layoutsEDIIntegracaoCliente.Where(obj => obj.LayoutEDI.Tipo == layoutVGM).Any() ||
                                                    layoutsEDIIntegracaoGrupo.Where(obj => obj.LayoutEDI.Tipo == layoutVGM).Any();

                        bool inseridoVGM = false;
                        if (pedidos.Count > 0 && possuiLayoutEdiVGM)
                        {
                            dynamic layoutEDIEntidade = ObterEntidadeLayoutEDIListasPorTipo(layoutVGM, layoutsEDIIntegracaoTipoOperacao, layoutsEDITransportador, layoutsEDIIntegracaoCliente, layoutsEDIIntegracaoGrupo);
                            AdicionarCargaIntegracaoEDIPedidos(carga, pedidos, layoutEDIEntidade?.LayoutEDI, layoutEDIEntidade?.TipoIntegracao, null, unitOfWork);
                            inseridoVGM = true;
                        }

                        if (!integracaoFilialEmissora)
                        {
                            for (var i = 0; i < layoutsEDIIntegracaoTipoOperacao.Count; i++)
                            {
                                if (!inseridoVGM || layoutsEDIIntegracaoTipoOperacao[i].LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.VGM)
                                    AdicionarCargaEDIIntegracao(carga, pedidos, null, layoutsEDIIntegracaoTipoOperacao[i].LayoutEDI, layoutsEDIIntegracaoTipoOperacao[i].TipoIntegracao, null, unitOfWork, true, false, codigoCTe);
                            }

                            for (var i = 0; i < layoutsEDIIntegracaoCliente.Count; i++)
                            {
                                if (!filialEmissora || (layoutsEDIIntegracaoCliente[i].LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.INTNC && filialEmissora && disponibilizarDocumentoEscrituracaoTomador))
                                    AdicionarCargaEDIIntegracao(carga, pedidos, tomador, layoutsEDIIntegracaoCliente[i].LayoutEDI, layoutsEDIIntegracaoCliente[i].TipoIntegracao, null, unitOfWork, true, false, codigoCTe);
                            }

                            for (var i = 0; i < layoutsEDIIntegracaoGrupo.Count; i++)
                            {
                                if (!filialEmissora || (layoutsEDIIntegracaoGrupo[i].LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.INTNC && filialEmissora && disponibilizarDocumentoEscrituracaoTomador))
                                    AdicionarCargaEDIIntegracao(carga, pedidos, tomador, layoutsEDIIntegracaoGrupo[i].LayoutEDI, layoutsEDIIntegracaoGrupo[i].TipoIntegracao, null, unitOfWork, true, false, codigoCTe);
                            }

                            for (var i = 0; i < layoutsEDITransportador.Count; i++)
                                AdicionarCargaEDIIntegracao(carga, pedidos, tomador, layoutsEDITransportador[i].LayoutEDI, layoutsEDITransportador[i].TipoIntegracao, carga.Empresa, unitOfWork, false, false, codigoCTe);

                        }
                        else
                        {
                            for (var i = 0; i < layoutsEDIIntegracaoCliente.Count; i++)
                            {
                                if (filialEmissora && (layoutsEDIIntegracaoCliente[i].LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || !disponibilizarDocumentoEscrituracaoTomador))
                                    AdicionarCargaEDIIntegracao(carga, pedidos, tomador, layoutsEDIIntegracaoCliente[i].LayoutEDI, layoutsEDIIntegracaoCliente[i].TipoIntegracao, null, unitOfWork, true, true, codigoCTe);
                            }

                            for (var i = 0; i < layoutsEDIIntegracaoGrupo.Count; i++)
                            {
                                if (filialEmissora && (layoutsEDIIntegracaoGrupo[i].LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC || !disponibilizarDocumentoEscrituracaoTomador))
                                    AdicionarCargaEDIIntegracao(carga, pedidos, tomador, layoutsEDIIntegracaoGrupo[i].LayoutEDI, layoutsEDIIntegracaoGrupo[i].TipoIntegracao, null, unitOfWork, true, true, codigoCTe);
                            }

                            for (var i = 0; i < layoutsEDIFilialEmissora.Count; i++)
                                AdicionarCargaEDIIntegracao(carga, pedidos, tomador, layoutsEDIFilialEmissora[i].LayoutEDI, layoutsEDIFilialEmissora[i].TipoIntegracao, carga.EmpresaFilialEmissora, unitOfWork, false, true, codigoCTe);
                        }
                    }
                }
            }

            AdicionarEDIFiscalMT(carga, codigoCTe, tipoServicoMultisoftware, unitOfWork);
            AdicionarEDIFiscalUVTRN(carga, unitOfWork);
        }

        private static void AdicionarEDIFiscalMT(Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            bool exigeEDIFiscalMT = carga.Empresa?.Configuracao?.ExigeEDIFiscalMT ?? false;

            if (exigeEDIFiscalMT && carga.Pedidos.Any(obj => obj.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada) && !carga.Pedidos.All(o => o.Origem.Estado.Sigla == "MT") && carga.CargaMDFes.Any(o => o.MDFe.EstadoDescarregamento.Sigla == "MT" || o.MDFe.Percursos.Any(perc => perc.Estado.Sigla == "MT")))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoSemIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao);

                Dominio.Entidades.LayoutEDI layoutEDIFiscal = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.FISCAL, true);

                if (layoutEDIFiscal != null && tipoIntegracaoSemIntegracao != null && !repCargaEDIIntegracao.VerificarSeExistePorCarga(carga.Codigo, tipoIntegracaoSemIntegracao.Codigo, layoutEDIFiscal.Codigo, 0, codigoCTe))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao
                    {
                        Carga = carga,
                        DataIntegracao = DateTime.Now,
                        NumeroTentativas = 0,
                        ProblemaIntegracao = string.Empty,
                        TipoIntegracao = tipoIntegracaoSemIntegracao,
                        CTe = codigoCTe > 0 ? repCTe.BuscarPorCodigo(codigoCTe) : null,
                        LayoutEDI = layoutEDIFiscal
                    };

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    else
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    repCargaEDIIntegracao.Inserir(cargaEDIIntegracao);
                }
            }
        }

        private static void AdicionarEDIFiscalUVTRN(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.UVT_RN, true);

            if (layoutEDI == null)
                return;

            if (!carga.CargaMDFes.Any(o => o.MDFe.EstadoDescarregamento.Sigla == "RN"))
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoSemIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao);

            if (tipoIntegracaoSemIntegracao == null)
                return;

            if (repCargaEDIIntegracao.VerificarSeExistePorCarga(carga.Codigo, tipoIntegracaoSemIntegracao.Codigo, layoutEDI.Codigo, 0D, 0))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao
            {
                Carga = carga,
                DataIntegracao = DateTime.Now,
                NumeroTentativas = 0,
                ProblemaIntegracao = string.Empty,
                TipoIntegracao = tipoIntegracaoSemIntegracao,
                LayoutEDI = layoutEDI,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
            };

            repCargaEDIIntegracao.Inserir(cargaEDIIntegracao);
        }

        private static void AdicionarCargaEDIIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos, Dominio.Entidades.Cliente tomador, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, bool validarSeExisteCTe, bool integracaoFilialEmissora, int codigoCTe = 0)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>() { null };

            if (layoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.AGRO && repCargaCTe.ContarCtesPorCarga(carga.Codigo) <= 0)
                return;

            if (!VerificarSeExistemDocumentosEmitidosPorLayoutModeloNaCarga(carga, layoutEDI, unitOfWork))
                return;

            if (carga.GrupoPessoaPrincipal != null && carga.GrupoPessoaPrincipal.NaoGerarArquivoVgm && layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM)
                return;

            if (carga.CargaRecebidaDeIntegracao)
                return;

            if (layoutEDI.AgruparPorRemetente)
            {
                remetentes = repCargaCTe.BuscarRemetentesPorCargaPedido(pedidos.Select(o => o.Codigo));
                foreach (Dominio.Entidades.Cliente remetente in remetentes)
                {
                    if (!repCargaEDIIntegracao.VerificarSeExistePorCarga(carga.Codigo, tipoIntegracao.Codigo, layoutEDI.Codigo, tomador?.CPF_CNPJ ?? 0D, remetente?.CPF_CNPJ ?? 0D, 0) || codigoCTe > 0)
                    {
                        if (!validarSeExisteCTe || repCargaCTe.ContarPorCargaETomador(carga.Codigo, tomador?.CPF_CNPJ ?? 0D) > 0)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao();
                            cargaEDIIntegracao.Carga = carga;
                            cargaEDIIntegracao.DataIntegracao = DateTime.Now;
                            cargaEDIIntegracao.NumeroTentativas = 0;
                            cargaEDIIntegracao.ProblemaIntegracao = string.Empty;
                            cargaEDIIntegracao.Empresa = empresa;
                            cargaEDIIntegracao.IntegracaoFilialEmissora = integracaoFilialEmissora;
                            cargaEDIIntegracao.TipoIntegracao = tipoIntegracao;
                            cargaEDIIntegracao.Remetente = remetente;
                            cargaEDIIntegracao.CTe = codigoCTe > 0 ? repCTe.BuscarPorCodigo(codigoCTe) : null;

                            if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                                tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            else
                                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                            cargaEDIIntegracao.Pedidos = pedidos;
                            cargaEDIIntegracao.LayoutEDI = layoutEDI;

                            repCargaEDIIntegracao.Inserir(cargaEDIIntegracao);
                        }
                    }
                }
            }
            else
            {
                if (!repCargaEDIIntegracao.VerificarSeExistePorCarga(carga.Codigo, tipoIntegracao.Codigo, layoutEDI.Codigo, 0) || codigoCTe > 0)
                {

                    Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao();
                    cargaEDIIntegracao.Carga = carga;
                    cargaEDIIntegracao.DataIntegracao = DateTime.Now;
                    cargaEDIIntegracao.NumeroTentativas = 0;
                    cargaEDIIntegracao.ProblemaIntegracao = string.Empty;
                    cargaEDIIntegracao.Empresa = empresa;
                    cargaEDIIntegracao.IntegracaoFilialEmissora = integracaoFilialEmissora;
                    cargaEDIIntegracao.TipoIntegracao = tipoIntegracao;
                    cargaEDIIntegracao.CTe = codigoCTe > 0 ? repCTe.BuscarPorCodigo(codigoCTe) : null;

                    if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    cargaEDIIntegracao.Pedidos = pedidos;
                    cargaEDIIntegracao.LayoutEDI = layoutEDI;

                    repCargaEDIIntegracao.Inserir(cargaEDIIntegracao);

                }
            }
        }

        private static void AdicionarCargaIntegracaoEDIPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, bool integracaoFilialEmissora = false)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);

            if (carga.GrupoPessoaPrincipal != null && carga.GrupoPessoaPrincipal.NaoGerarArquivoVgm && layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM)
                return;

            if (carga.CargaRecebidaDeIntegracao)
                return;

            if (layoutEDI == null || tipoIntegracao == null)
                return;

            if (!repCargaEDIIntegracao.VerificarSeExistePorCarga(carga.Codigo, tipoIntegracao.Codigo, layoutEDI.Codigo, 0))
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in pedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao();
                    cargaEDIIntegracao.Carga = carga;
                    cargaEDIIntegracao.DataIntegracao = DateTime.Now;
                    cargaEDIIntegracao.NumeroTentativas = 0;
                    cargaEDIIntegracao.ProblemaIntegracao = string.Empty;
                    cargaEDIIntegracao.Empresa = empresa;
                    cargaEDIIntegracao.IntegracaoFilialEmissora = integracaoFilialEmissora;
                    cargaEDIIntegracao.TipoIntegracao = tipoIntegracao;
                    cargaEDIIntegracao.Container = cargaPedido.Pedido.Container ?? null;

                    if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                        tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    else
                        cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    cargaEDIIntegracao.Pedidos = pedidos;
                    cargaEDIIntegracao.LayoutEDI = layoutEDI;

                    repCargaEDIIntegracao.Inserir(cargaEDIIntegracao);
                }
            }
        }

        private static dynamic ObterEntidadeLayoutEDIListasPorTipo(Dominio.Enumeradores.TipoLayoutEDI tipoLayout, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsEDIIntegracaoTipoOperacao, List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador, List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente, List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo)
        {
            if (layoutsEDIIntegracaoTipoOperacao.Where(obj => obj.LayoutEDI.Tipo == tipoLayout).Any())
                return layoutsEDIIntegracaoTipoOperacao.Where(obj => obj.LayoutEDI.Tipo == tipoLayout).FirstOrDefault();

            if (layoutsEDITransportador.Where(obj => obj.LayoutEDI.Tipo == tipoLayout).Any())
                return layoutsEDITransportador.Where(obj => obj.LayoutEDI.Tipo == tipoLayout).FirstOrDefault();

            if (layoutsEDIIntegracaoCliente.Where(obj => obj.LayoutEDI.Tipo == tipoLayout).Any())
                return layoutsEDIIntegracaoCliente.Where(obj => obj.LayoutEDI.Tipo == tipoLayout).FirstOrDefault();

            if (layoutsEDIIntegracaoGrupo.Where(obj => obj.LayoutEDI.Tipo == tipoLayout).Any())
                return layoutsEDIIntegracaoGrupo.Where(obj => obj.LayoutEDI.Tipo == tipoLayout).FirstOrDefault();

            return null;
        }

        private static bool VerificarSeExistemDocumentosEmitidosPorLayoutModeloNaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.LayoutEDI layoutEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            bool possui = true;
            if (layoutEDI.ModeloDocumentoFiscais != null && layoutEDI.ModeloDocumentoFiscais.Count > 0)
                possui = repCargaCTe.ExisteModeloDeDocumentoEmitidoNaCarga(carga.Codigo, layoutEDI.ModeloDocumentoFiscais.Select(o => o.Codigo).ToList());

            return possui;
        }

        public static void AdicionarEDIParaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCancelamento.Carga;

            if (!carga.CargaTransbordo)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsEDIIntegracaoTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDITransportador = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsEDIFilialEmissora = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = carga.TipoOperacao;

                bool transportadorGeraLoteEscrituracao = carga?.Empresa?.GerarLoteEscrituracao ?? false;
                bool disponibilizarDocumentosParaLoteEscrituracao = (tipoOperacao?.DisponibilizarDocumentosParaLoteEscrituracao) ?? false || transportadorGeraLoteEscrituracao;

                if (tipoOperacao != null)
                {
                    if (tipoOperacao.LayoutsEDI != null && tipoOperacao.LayoutsEDI.Any(o => (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO) || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento))))
                        layoutsEDIIntegracaoTipoOperacao = tipoOperacao.LayoutsEDI.Where(o => (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO) || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento))).ToList();
                }

                if (carga.Empresa != null && carga.Empresa.TransportadorLayoutsEDI != null && carga.Empresa.TransportadorLayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && !disponibilizarDocumentosParaLoteEscrituracao))))
                    layoutsEDITransportador = carga.Empresa.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && ((o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && !disponibilizarDocumentosParaLoteEscrituracao))).ToList();

                if (carga.EmpresaFilialEmissora != null && carga.EmpresaFilialEmissora.TransportadorLayoutsEDI != null && carga.EmpresaFilialEmissora.TransportadorLayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento))))
                    layoutsEDIFilialEmissora = carga.EmpresaFilialEmissora.TransportadorLayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento))).ToList();

                List<Dominio.Entidades.Cliente> tomadores = (from obj in carga.Pedidos select obj.ObterTomador())?.Distinct()?.ToList();
                if (tomadores != null && tomadores.Count > 0)
                {
                    foreach (Dominio.Entidades.Cliente tomador in tomadores)
                    {
                        if (tomador == null)
                            continue;

                        bool disponibilizarDocumentoEscrituracaoTomador = disponibilizarDocumentosParaLoteEscrituracao;
                        if (!disponibilizarDocumentoEscrituracaoTomador)
                        {
                            bool grupoDisponibilizarDocumentosParaLoteEscrituracao = tomador.GrupoPessoas?.DisponibilizarDocumentosParaLoteEscrituracao ?? false;

                            if ((tomador.DisponibilizarDocumentosParaLoteEscrituracao || grupoDisponibilizarDocumentosParaLoteEscrituracao) && (!tomador.DataViradaProvisao.HasValue || tomador.DataViradaProvisao.Value < carga.DataCriacaoCarga))
                                disponibilizarDocumentoEscrituracaoTomador = true;
                        }

                        bool filialEmissora = carga.EmpresaFilialEmissora != null ? true : false;

                        List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsEDIIntegracaoCliente = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();
                        List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsEDIIntegracaoGrupo = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();

                        if (tomador.LayoutsEDI != null && tomador.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && (filialEmissora || !disponibilizarDocumentoEscrituracaoTomador))))
                            layoutsEDIIntegracaoCliente = tomador.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && (filialEmissora || !disponibilizarDocumentoEscrituracaoTomador))).ToList();
                        else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.LayoutsEDI != null && tomador.GrupoPessoas.LayoutsEDI.Any(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && (filialEmissora || !disponibilizarDocumentoEscrituracaoTomador))))
                            layoutsEDIIntegracaoGrupo = tomador.GrupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO || o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento) && (filialEmissora || !disponibilizarDocumentoEscrituracaoTomador))).ToList();

                        if (layoutsEDIIntegracaoCliente.Count <= 0 && layoutsEDIIntegracaoGrupo.Count <= 0 && layoutsEDITransportador.Count <= 0 && layoutsEDIFilialEmissora.Count <= 0 && layoutsEDIIntegracaoTipoOperacao.Count <= 0)
                            continue;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = (from obj in carga.Pedidos where obj.ObterTomador().Equals(tomador) select obj).ToList();

                        for (var i = 0; i < layoutsEDITransportador.Count; i++)
                            AdicionarCargaCancelamentoEDIIntegracao(cargaCancelamento, carga, tomador, layoutsEDITransportador[i].LayoutEDI, layoutsEDITransportador[i].TipoIntegracao, carga.Empresa, unitOfWork);

                        for (var i = 0; i < layoutsEDIFilialEmissora.Count; i++)
                            AdicionarCargaCancelamentoEDIIntegracao(cargaCancelamento, carga, tomador, layoutsEDIFilialEmissora[i].LayoutEDI, layoutsEDIFilialEmissora[i].TipoIntegracao, carga.EmpresaFilialEmissora, unitOfWork);

                        for (var i = 0; i < layoutsEDIIntegracaoCliente.Count; i++)
                            AdicionarCargaCancelamentoEDIIntegracao(cargaCancelamento, carga, tomador, layoutsEDIIntegracaoCliente[i].LayoutEDI, layoutsEDIIntegracaoCliente[i].TipoIntegracao, null, unitOfWork);

                        for (var i = 0; i < layoutsEDIIntegracaoGrupo.Count; i++)
                            AdicionarCargaCancelamentoEDIIntegracao(cargaCancelamento, carga, tomador, layoutsEDIIntegracaoGrupo[i].LayoutEDI, layoutsEDIIntegracaoGrupo[i].TipoIntegracao, null, unitOfWork);
                    }
                }
                for (var i = 0; i < layoutsEDIIntegracaoTipoOperacao.Count; i++)
                    AdicionarCargaCancelamentoEDIIntegracao(cargaCancelamento, carga, null, layoutsEDIIntegracaoTipoOperacao[i].LayoutEDI, layoutsEDIIntegracaoTipoOperacao[i].TipoIntegracao, null, unitOfWork);

            }
        }

        public static void AdicionarCargaCancelamentoEDIIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente tomador, Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>() { null };

            int quantidadeCTe = 0;
            if (tomador != null)
                quantidadeCTe = repCargaCTe.ContarPorCargaETomador(carga.Codigo, tomador.CPF_CNPJ);
            else
                quantidadeCTe = repCargaCTe.ContarCtesPorCarga(carga.Codigo);

            if (quantidadeCTe <= 0)
                return;

            if (!VerificarSeExistemDocumentosEmitidosPorLayoutModeloNaCarga(carga, layoutEDI, unitOfWork))
                return;

            if (!repCargaCancelamentoIntegracaoEDI.VerificarSeExistePorCarga(carga.Codigo, tipoIntegracao.Codigo, layoutEDI.Codigo, tomador?.CPF_CNPJ ?? 0))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI()
                {
                    CargaCancelamento = cargaCancelamento,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = string.Empty,
                    Empresa = empresa,
                    TipoIntegracao = tipoIntegracao,
                    LayoutEDI = layoutEDI
                };

                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao ||
                    tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                    cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                else
                    cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repCargaCancelamentoIntegracaoEDI.Inserir(cargaCancelamentoIntegracaoEDI);
            }
        }

        #endregion

        #region Verificacao

        public async Task<List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao>> VerificarIntegracoesPendentesEscrituracaoAsync()
        {
            _unitOfWork.FlushAndClear();

            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repositorioEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repositorioLoteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(_unitOfWork, _cancellationToken);

            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new Servicos.Embarcador.Integracao.FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao> integracoesPendentes = await repositorioEscrituracaoEDIIntegracao.BuscarIntegracoesPendentesAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao> lancamento = (from obj in integracoesPendentes select obj.LoteEscrituracao).Distinct().ToList();

            for (var i = 0; i < integracoesPendentes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao integracaoPendente = integracoesPendentes[i];

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(ref integracaoPendente, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        if (integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.FISCAL && integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioEscrituracaoEDIIntegracao.AtualizarAsync(integracaoPendente);
            }

            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao> loteEscrituracaoIntegracao = await repositorioLoteEscrituracaoIntegracao.BuscarIntegracoesPendentesAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, _cancellationToken);

            lancamento.AddRange((from obj in loteEscrituracaoIntegracao select obj.LoteEscrituracao).Distinct().ToList());

            // Todo: Mover para o lugar correto
            for (int i = 0; i < loteEscrituracaoIntegracao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao integracaoPendente = loteEscrituracaoIntegracao[i];
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee:
                        Servicos.Embarcador.Integracao.Digibee.IntegracaoDigibee.IntegrarDocumentosEscrituracao(integracaoPendente, _unitOfWork);
                        break;
                }
            }


            return lancamento;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento>> VerificarIntegracoesPendentesEscrituracaoCancelamentoAsync()
        {
            _unitOfWork.FlushAndClear();

            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repositorioLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(_unitOfWork, _cancellationToken);

            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new Servicos.Embarcador.Integracao.FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> integracoesPendentes = await repositorioLoteEscrituracaoCancelamentoEDIIntegracao.BuscarIntegracoesPendentesAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> lancamento = integracoesPendentes.Select(o => o.LoteEscrituracaoCancelamento).Distinct().ToList();

            int countIntegracoesPendentes = integracoesPendentes.Count;

            for (int i = 0; i < countIntegracoesPendentes; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao integracaoPendente = integracoesPendentes[i];

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(ref integracaoPendente, _tipoServicoMultisoftware, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        if (integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.FISCAL && integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioLoteEscrituracaoCancelamentoEDIIntegracao.AtualizarAsync(integracaoPendente);
            }

            return lancamento;
        }

        public async Task<List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>> VerificarIntegracoesPendentesNFSManualAsync()
        {
            _unitOfWork.FlushAndClear();

            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repositorioNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao> integracoesPendentes = repositorioNFSManualEDIIntegracao.BuscarIntegracoesPendentes(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentoManual = (from obj in integracoesPendentes select obj.LancamentoNFSManual).Distinct().ToList();

            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            for (var i = 0; i < integracoesPendentes.Count; i++)
            {
                Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao integracaoPendente = integracoesPendentes[i];

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(ref integracaoPendente, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        if (integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.FISCAL && integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioNFSManualEDIIntegracao.AtualizarAsync(integracaoPendente);
            }
            return lancamentoManual;
        }

        public async Task<List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento>> VerificarIntegracoesPendentesNFSManualCancelamentoAsync()
        {
            _unitOfWork.FlushAndClear();

            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repositorioNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(_unitOfWork);

            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> integracoesPendentes = await repositorioNFSManualCancelamentoIntegracaoEDI.BuscarIntegracoesPendentesAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, _cancellationToken);
            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> nfsManualCancelamentos = (from obj in integracoesPendentes select obj.NFSManualCancelamento).Distinct().ToList();

            for (var i = 0; i < integracoesPendentes.Count; i++)
            {
                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI integracaoPendente = integracoesPendentes[i];

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(integracaoPendente, _tipoServicoMultisoftware, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        if (integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.FISCAL && integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioNFSManualCancelamentoIntegracaoEDI.AtualizarAsync(integracaoPendente);
            }

            return nfsManualCancelamentos;
        }

        public async Task<List<int>> VerificarIntegracoesPendentesAsync()
        {
            _unitOfWork.FlushAndClear();

            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin integracaoMichelin = new Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin(_unitOfWork);
            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao> integracoesPendentes = await repositorioCargaEDIIntegracao.BuscarIntegracoesPendentesAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = (from obj in integracoesPendentes select obj.Carga).Distinct().ToList();

            for (var i = 0; i < integracoesPendentes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao integracaoPendente = integracoesPendentes[i];

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(ref integracaoPendente, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Michelin:
                        integracaoMichelin.EnviarCONEMB(ref integracaoPendente, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        if (integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.FISCAL && integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioCargaEDIIntegracao.AtualizarAsync(integracaoPendente);
            }

            return cargas.Select(x => x.Codigo).Distinct().ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>> VerificarIntegracoesPendentesCargaCancelamentoAsync(Repositorio.UnitOfWork unitOfWork)
        {
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repositorioCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);
            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI> ocorrenciaEDIIntegracaoPendentes = await repositorioCargaCancelamentoIntegracaoEDI.BuscarIntegracoesPendentesAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> cancelamentos = (from obj in ocorrenciaEDIIntegracaoPendentes select obj.CargaCancelamento).Distinct().ToList();

            for (int i = 0; i < ocorrenciaEDIIntegracaoPendentes.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI integracaoPendente = ocorrenciaEDIIntegracaoPendentes[0];
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(ref integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioCargaCancelamentoIntegracaoEDI.AtualizarAsync(integracaoPendente);
            }

            return cancelamentos;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>> VerificarIntegracoesPendentesOcorrenciasAsync(Repositorio.UnitOfWork unitOfWork)
        {
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 10;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unitOfWork);
            Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin integracaoMichelin = new Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin(unitOfWork);
            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new Servicos.Embarcador.Integracao.FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao> ocorrenciaEDIIntegracaoPendentes = repOcorrenciaEDIIntegracao.BuscarIntegracoesPendentes(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> cargaOcorrencias = (from obj in ocorrenciaEDIIntegracaoPendentes select obj.CargaOcorrencia).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao integracaoPendente in ocorrenciaEDIIntegracaoPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Michelin:
                        integracaoMichelin.EnviarOCORREN(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Carrefour:
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                repOcorrenciaEDIIntegracao.Atualizar(integracaoPendente);
            }

            return cargaOcorrencias;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Avarias.Lote>> VerificarIntegracoesPendentesLotesAsync()
        {
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Avarias.LoteEDIIntegracao repositorioLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);
            List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> loteEDIIntegracaoPendentes = repositorioLoteEDIIntegracao.BuscarIntegracoesPendentes(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);
            List<Dominio.Entidades.Embarcador.Avarias.Lote> lotes = (from obj in loteEDIIntegracaoPendentes select obj.Lote).Distinct().ToList();

            for (var i = 0; i < loteEDIIntegracaoPendentes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao integracaoPendente = loteEDIIntegracaoPendentes[i];

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioLoteEDIIntegracao.AtualizarAsync(integracaoPendente);
            }

            return lotes;
        }

        public async Task<List<int>> VerificarIntegracoesPendentesContabilizacaoAsync()
        {
            _unitOfWork.FlushAndClear();

            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repositorioLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> integracoesPendentes = repositorioLoteContabilizacaoIntegracaoEDI.BuscarIntegracoesPendentes(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);

            List<int> lotesContabilizacao = integracoesPendentes.Select(o => o.LoteContabilizacao.Codigo).Distinct().ToList();

            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            for (var i = 0; i < integracoesPendentes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI integracaoPendente = integracoesPendentes[i];

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                        Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(integracaoPendente, _tipoServicoMultisoftware, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        if (integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.FISCAL && integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioLoteContabilizacaoIntegracaoEDI.AtualizarAsync(integracaoPendente);
            }

            return lotesContabilizacao;
        }

        public async Task<List<int>> VerificarIntegracoesPendentesLoteClienteAsync()
        {
            _unitOfWork.FlushAndClear();

            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repositorioLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(_unitOfWork, _cancellationToken);

            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> integracoesPendentes = await repositorioLoteClienteIntegracaoEDI.BuscarIntegracoesPendentesAsync(minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez, _cancellationToken);

            List<int> lotes = integracoesPendentes.Select(o => o.LoteCliente.Codigo).Distinct().ToList();

            for (var i = 0; i < integracoesPendentes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI integracaoPendente = integracoesPendentes[i];

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                        await servicoIntegracaoFTP.EnviarEDIAsync(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada:
                        if (integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.FISCAL && integracaoPendente.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
                            integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        break;
                    default:
                        break;
                }

                await repositorioLoteClienteIntegracaoEDI.AtualizarAsync(integracaoPendente);
            }

            return lotes;
        }

        #endregion

        #region Geracao

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Empresa empresa, Dominio.Entidades.LayoutEDI layoutEDI, List<Dominio.Entidades.Produto> listaProduto, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, out string extensao)
        {
            extensao = ".txt";

            Servicos.Embarcador.Integracao.EDI.EBSProduto svcEBSProduto = new EDI.EBSProduto();
            Dominio.ObjetosDeValor.EDI.EBS.ListaProduto produtos = svcEBSProduto.ConverterProdutosEBS(listaProduto, unidadeDeTrabalho);

            Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, layoutEDI, empresa);

            return serGeracaoEDI.GerarArquivoRecursivo(produtos);
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Empresa empresa, Dominio.Entidades.LayoutEDI layoutEDI, List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> listaNotaEntrada, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, out string extensao)
        {
            extensao = ".txt";

            Servicos.Embarcador.Integracao.EDI.EBSNotaEntrada svcEBSNotaEntrada = new EDI.EBSNotaEntrada();
            Dominio.ObjetosDeValor.EDI.EBS.NotaEntrada notaEntradas = svcEBSNotaEntrada.ConverterNotaEntradaEBS(empresa, listaNotaEntrada, unidadeDeTrabalho);

            Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, layoutEDI, empresa);

            return serGeracaoEDI.GerarArquivoRecursivo(notaEntradas);
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, out string extensao)
        {
            if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();
                Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB conemb = svcCONEMB.ConverterCargaParaCONEMB_MartinBrower(cargaEDIIntegracao, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(conemb);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_VOLKS)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();
                Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.EDICONEMB_VW conemb = svcCONEMB.ConverterCargaParaCONEMB_VOLKS(cargaEDIIntegracao, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(conemb);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();
                Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao conemb = svcCONEMB.ConverterCargaParaCONEMB_CaterpillarImportacao(cargaEDIIntegracao, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(conemb);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();
                Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao conemb = svcCONEMB.ConverterCargaParaCONEMB_CaterpillarExportacao(cargaEDIIntegracao, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(conemb);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.MasterSAF svcMasterSAF = new EDI.MasterSAF(unidadeDeTrabalho);
                Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = svcMasterSAF.ConverterCargaParaMasterSAF(cargaEDIIntegracao);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(masterSAF);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.VGM)
            {
                extensao = ".txt";
                Servicos.Embarcador.Integracao.EDI.VGM svcVGM = new EDI.VGM();
                return svcVGM.GerarArquivoVGM(cargaEDIIntegracao, unidadeDeTrabalho);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.AGRO)
            {
                extensao = ".txt";
                Servicos.Embarcador.Integracao.EDI.AGRO serAgro = new EDI.AGRO();

                Dominio.ObjetosDeValor.EDI.AGRO.AGRO agro = serAgro.ConverterCargaEDIIntegracaoParaAGRO(cargaEDIIntegracao.Carga, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(agro);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.INTNC svcINTNC = new EDI.INTNC();
                Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = svcINTNC.ConverterCargaParaINTNC(cargaEDIIntegracao, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(intnc);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.INTDNE svcINTDNE = new EDI.INTDNE();
                Dominio.ObjetosDeValor.EDI.INTDNE.INTDNE intdne = svcINTDNE.ConverterCargaParaINTDNE(cargaEDIIntegracao.Carga, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(intdne);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();

                Dominio.ObjetosDeValor.EDI.CONEMB_NF.Arquivo arquivo = Servicos.Embarcador.Integracao.EDI.CONEMB_NF.ConverterParaCONEMB_NF(cargaEDIIntegracao, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(arquivo);
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL)
            {
                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = cargaEDIIntegracao.Carga.CargaMDFes.Where(o => o.MDFe.EstadoDescarregamento.Sigla == "MT" || o.MDFe.Percursos.Any(perc => perc.Estado.Sigla == "MT")).Select(o => o.MDFe).ToList();

                if (mdfes.Count > 1)
                {
                    extensao = ".zip";

                    System.IO.MemoryStream fZip = new System.IO.MemoryStream();

                    using (ZipOutputStream zipOStream = new ZipOutputStream(fZip))
                    {
                        zipOStream.SetLevel(9);

                        foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                        {
                            using (System.IO.MemoryStream arquivo = GerarArquivoEDIFiscal(mdfe, cargaEDIIntegracao.LayoutEDI, unidadeDeTrabalho))
                            {
                                byte[] bytesArquivo = arquivo.ToArray();

                                ZipEntry entry = new ZipEntry(string.Concat(mdfe.Chave, " - EDI Fiscal.txt"));
                                entry.DateTime = DateTime.Now;

                                zipOStream.PutNextEntry(entry);
                                zipOStream.Write(bytesArquivo, 0, bytesArquivo.Length);
                                zipOStream.CloseEntry();
                            }
                        }

                        zipOStream.IsStreamOwner = false;
                        zipOStream.Close();
                    }

                    fZip.Position = 0;

                    return fZip;
                }
                else
                {
                    extensao = ".txt";

                    return GerarArquivoEDIFiscal(mdfes[0], cargaEDIIntegracao.LayoutEDI, unidadeDeTrabalho);
                }
            }
            else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
            {
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repCargaMDFe.BuscarPorCargaEEstadoDestino(cargaEDIIntegracao.Carga.Codigo, "RN");

                Servicos.Embarcador.Integracao.EDI.UVTRN svcUVTRN = new EDI.UVTRN();

                Dominio.ObjetosDeValor.EDI.UVTRN.UVTRN ediUVTRN = svcUVTRN.ConverterCargaMDFesParaEDIUVTRN(cargaEDIIntegracao, cargaMDFes);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa);

                extensao = ".mlt";

                return serGeracaoEDI.GerarArquivoRecursivo(ediUVTRN);
            }
            else
            {
                extensao = ".txt";

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                if (cargaEDIIntegracao.CTe != null)
                    ctes.Add(cargaEDIIntegracao.CTe);
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaEDIIntegracao.Pedidos)
                    {
                        if (!cargaEDIIntegracao.IntegracaoFilialEmissora)
                            ctes.AddRange(repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedidoSemCTeFilialEmissora(cargaPedido.Codigo, true));
                        else
                            ctes.AddRange(repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedidoCTeFilialEmissora(cargaPedido.Codigo, true));
                    }
                }

                if (ctes.Count > 0)
                {
                    Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unidadeDeTrabalho, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.Carga.Empresa, ctes.Distinct().ToList());

                    bool substituirNotasPorSubcontratacao = false;
                    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        substituirNotasPorSubcontratacao = true;

                    return svcEDI.GerarArquivo(substituirNotasPorSubcontratacao);
                }
                else
                    return new System.IO.MemoryStream();
            }
        }

        public static System.IO.MemoryStream GerarLoteEDIFatura(List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> integracoes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            System.IO.MemoryStream fZip = new System.IO.MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            zipOStream.SetLevel(9);
            foreach (var faturaIntegracao in integracoes)
            {
                dynamic edi = null;

                if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB_CT)
                {
                    EDI.DOCCOB svcDOCCOB = new EDI.DOCCOB();
                    edi = svcDOCCOB.ConverterFaturaParaDOCCOBCaterpillar(faturaIntegracao.Fatura, unidadeDeTrabalho);
                }
                else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB)
                {
                    EDI.DOCCOB svcDOCCOB = new EDI.DOCCOB();
                    edi = svcDOCCOB.ConverterFaturaParaDOCCOB(faturaIntegracao.Fatura, unidadeDeTrabalho);
                }
                else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP)
                {
                    Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repFaturaDocumento.BuscarConhecimentos(faturaIntegracao.Fatura.Codigo);

                    Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new Servicos.Embarcador.Integracao.EDI.CONEMB();
                    edi = svcCONEMB.ConverterCargaCTeParaCONEMB_CaterpillarImportacao(ctes, null, unidadeDeTrabalho);
                }
                else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR)
                {
                    bool gerarPorCarga = false;
                    if (faturaIntegracao.Fatura.TipoOperacao != null && faturaIntegracao.Fatura.TipoOperacao.GerarEDIFaturaPorCarga)
                    {
                        gerarPorCarga = true;
                    }
                    EDI.INTPFAR svcINTPFAR = new EDI.INTPFAR();

                    if (!gerarPorCarga)//todo: mudar aqui quando criar em produção a escrituração.
                    {
                        if (faturaIntegracao.Fatura.NovoModelo)
                        {
                            if (configuracaoEmbarcador.TipoImpressaoFatura != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura.Multimodal && (faturaIntegracao.Fatura.ClienteTomadorFatura?.GrupoPessoas?.GerarFaturaPorCte ?? false))
                                edi = svcINTPFAR.ConverterFaturaParaINTPFARPorCte(faturaIntegracao, faturaIntegracao.Empresa, unidadeDeTrabalho);
                            else
                                edi = svcINTPFAR.ConverterFaturaParaINTPFAR(faturaIntegracao, faturaIntegracao.Empresa, unidadeDeTrabalho);
                        }
                        else
                            edi = svcINTPFAR.ConverterFaturaAntigaParaINTPFAR(faturaIntegracao, faturaIntegracao.Empresa, unidadeDeTrabalho);
                    }
                    else
                        edi = svcINTPFAR.ConverterCargaParaINTPFAR(faturaIntegracao, unidadeDeTrabalho);
                }
                else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB_VAXXINOVA)
                {
                    EDI.DOCCOB svcDOCCOB = new EDI.DOCCOB();
                    edi = svcDOCCOB.ConverterFaturaParaDOCCOBVaxxinova(faturaIntegracao.Fatura, unidadeDeTrabalho);
                }

                GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, faturaIntegracao.LayoutEDI, faturaIntegracao.Fatura.Empresa);
                Servicos.Log.TratarErro(faturaIntegracao.LayoutEDI.Descricao);
                using (System.IO.MemoryStream arquivo = serGeracaoEDI.GerarArquivoRecursivo(edi))
                {
                    byte[] bytesArquivo = arquivo.ToArray();
                    string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(faturaIntegracao, unidadeDeTrabalho, configuracaoEmbarcador.UtilizaEmissaoMultimodal, true);

                    ZipEntry entry = new ZipEntry(string.Concat(nomeArquivo));
                    entry.DateTime = DateTime.Now;

                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(bytesArquivo, 0, bytesArquivo.Length);
                    zipOStream.CloseEntry();
                }
            }

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;
            return fZip;
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao loteEscrituracaoEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, out string extensao)
        {
            if (loteEscrituracaoEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.INTNC svcINTNC = new EDI.INTNC();
                Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = svcINTNC.ConverterLoteEscrituracaoParaINTNC(loteEscrituracaoEDIIntegracao, loteEscrituracaoEDIIntegracao.Empresa, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, loteEscrituracaoEDIIntegracao.LayoutEDI, null);

                return serGeracaoEDI.GerarArquivoRecursivo(intnc);
            }
            else if (loteEscrituracaoEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.MasterSAF svcMasterSAF = new EDI.MasterSAF(unidadeDeTrabalho);
                Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = svcMasterSAF.ConverterLoteEscrituracaoParaMasterSAF(loteEscrituracaoEDIIntegracao, loteEscrituracaoEDIIntegracao.Empresa);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, loteEscrituracaoEDIIntegracao.LayoutEDI, null);

                System.Text.Encoding encoding = Utilidades.Encoding.ObterEncoding(loteEscrituracaoEDIIntegracao.LayoutEDI.Encoding);

                return serGeracaoEDI.GerarArquivoRecursivo(masterSAF, encoding);
            }
            else
            {
                extensao = ".txt";
                return null;
            }
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao, out string extensao)
        {
            if (loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.INTNC svcINTNC = new EDI.INTNC();
                Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = svcINTNC.ConverterLoteEscrituracaoCancelamentoParaINTNC(loteEscrituracaoCancelamentoEDIIntegracao, loteEscrituracaoCancelamentoEDIIntegracao.Empresa, unitOfWork);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unitOfWork, loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI, null);

                return serGeracaoEDI.GerarArquivoRecursivo(intnc);
            }
            else if (loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.MasterSAF svcMasterSAF = new EDI.MasterSAF(unitOfWork);
                Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = svcMasterSAF.ConverterLoteEscrituracaoCancelamentoParaMasterSAF(loteEscrituracaoCancelamentoEDIIntegracao, loteEscrituracaoCancelamentoEDIIntegracao.Empresa);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unitOfWork, loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI, null);

                return serGeracaoEDI.GerarArquivoRecursivo(masterSAF);
            }
            else
            {
                extensao = ".txt";
                return null;
            }
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao nfsManualEDIIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao, out string extensao)
        {
            //if (nfsManualEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB)
            //{
            //    extensao = ".txt";

            //    Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();
            //    Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB conemb = svcCONEMB.ConverterCargaParaCONEMB_MartinBrower(nfsManualEDIIntegracao.Carga, unidadeDeTrabalho);

            //    Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(nfsManualEDIIntegracao.Carga.Empresa, nfsManualEDIIntegracao.LayoutEDI, unidadeDeTrabalho.StringConexao);

            //    return serGeracaoEDI.GerarArquivoRecursivo(conemb);
            //}
            //else 
            if (nfsManualEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.INTNC svcINTNC = new EDI.INTNC();
                Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = svcINTNC.ConverterCargaParaINTNC(nfsManualEDIIntegracao, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, nfsManualEDIIntegracao.LayoutEDI, nfsManualEDIIntegracao.LancamentoNFSManual.Transportador);

                return serGeracaoEDI.GerarArquivoRecursivo(intnc);
            }
            else if (nfsManualEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTDNE)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.INTDNE svcINTDNE = new EDI.INTDNE();
                Dominio.ObjetosDeValor.EDI.INTDNE.INTDNE intdne = svcINTDNE.ConverterCargaParaINTDNE(nfsManualEDIIntegracao.LancamentoNFSManual, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, nfsManualEDIIntegracao.LayoutEDI, nfsManualEDIIntegracao.LancamentoNFSManual.Transportador);

                return serGeracaoEDI.GerarArquivoRecursivo(intdne);
            }
            else if (nfsManualEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.MasterSAF svcMasterSAF = new EDI.MasterSAF(unidadeDeTrabalho);
                Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = svcMasterSAF.ConverterNFSManualParaMasterSAF(nfsManualEDIIntegracao);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, nfsManualEDIIntegracao.LayoutEDI, null);

                return serGeracaoEDI.GerarArquivoRecursivo(masterSAF);
            }
            else
            {
                extensao = ".txt";

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                ctes.Add(nfsManualEDIIntegracao.LancamentoNFSManual.CTe);

                Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unidadeDeTrabalho, nfsManualEDIIntegracao.LayoutEDI, nfsManualEDIIntegracao.LancamentoNFSManual.Transportador, ctes.Distinct().ToList());

                bool substituirNotasPorSubcontratacao = false;
                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    substituirNotasPorSubcontratacao = true;

                return svcEDI.GerarArquivo(substituirNotasPorSubcontratacao);
            }
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI nfsManualCancelamentoIntegracaoEDI, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho, out string extensao)
        {
            if (nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.INTNC svcINTNC = new EDI.INTNC();
                Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = svcINTNC.ConverterCargaParaINTNC(nfsManualCancelamentoIntegracaoEDI, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, nfsManualCancelamentoIntegracaoEDI.LayoutEDI, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.Transportador);

                return serGeracaoEDI.GerarArquivoRecursivo(intnc);
            }
            else if (nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento)
            {
                extensao = ".txt";

                Servicos.Embarcador.Integracao.EDI.MasterSAF svcMasterSAF = new EDI.MasterSAF(unidadeDeTrabalho);
                Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = svcMasterSAF.ConverterNFSManualCancelamentoParaMasterSAF(nfsManualCancelamentoIntegracaoEDI);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, nfsManualCancelamentoIntegracaoEDI.LayoutEDI, null);

                return serGeracaoEDI.GerarArquivoRecursivo(masterSAF);
            }
            else
            {
                extensao = ".txt";

                //Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unidadeDeTrabalho);

                //List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                //ctes.Add(nfsManualEDIIntegracao.LancamentoNFSManual.CTe);

                //Servicos.GeracaoEDI svcEDI = new GeracaoEDI(nfsManualEDIIntegracao.LancamentoNFSManual.Transportador, nfsManualEDIIntegracao.LayoutEDI, ctes.Distinct().ToList(), unidadeDeTrabalho.StringConexao);

                //bool substituirNotasPorSubcontratacao = false;
                //if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                //    substituirNotasPorSubcontratacao = true;

                //return svcEDI.GerarArquivo(substituirNotasPorSubcontratacao);

                return null;
            }
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao cancelamentoProvisaoEDI, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            //if (cancelamentoProvisaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV)
            //{
            //    Servicos.Embarcador.Integracao.EDI.PROV svcEDI = new Servicos.Embarcador.Integracao.EDI.PROV();
            //    Dominio.ObjetosDeValor.EDI.PROV.Provisao obj = svcEDI.GerarPorProvisao(cancelamentoProvisaoEDI.Provisao, unidadeDeTrabalho);

            //    if (obj != null)
            //    {
            //        Servicos.GeracaoEDI svcGeracaoEDI = new GeracaoEDI(cancelamentoProvisaoEDI.Provisao.Empresa, cancelamentoProvisaoEDI.LayoutEDI, unidadeDeTrabalho.StringConexao);

            //        return svcGeracaoEDI.GerarArquivoRecursivo(obj);
            //    }
            //    else
            //        return null;
            //}
            //else 
            if (cancelamentoProvisaoEDI?.LayoutEDI?.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV_INTPFAR)
            {
                EDI.INTPFAR svcINTPFAR = new EDI.INTPFAR();
                dynamic edi = svcINTPFAR.ConverterCancelamentoProvisaoParaINTPFAR(cancelamentoProvisaoEDI.CancelamentoProvisao, unidadeDeTrabalho);
                GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cancelamentoProvisaoEDI.LayoutEDI, cancelamentoProvisaoEDI.CancelamentoProvisao.Empresa);
                return serGeracaoEDI.GerarArquivoRecursivo(edi);
            }
            else
                return null;
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDI, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (pagamentoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR)
            {
                EDI.INTPFAR svcINTPFAR = new EDI.INTPFAR();
                dynamic edi = svcINTPFAR.ConverterPagamentoParaINTPFAR(pagamentoEDI, unidadeDeTrabalho);
                GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, pagamentoEDI.LayoutEDI, pagamentoEDI.Pagamento.Empresa);
                return serGeracaoEDI.GerarArquivoRecursivo(edi);
            }
            else if (pagamentoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.ImportsysCTe)
            {
                EDI.ImportsysCTe svcImportsysCTe = new EDI.ImportsysCTe();
                dynamic edi = svcImportsysCTe.ConverterPagamentoParaImportsysCTe(pagamentoEDI, unidadeDeTrabalho);
                GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, pagamentoEDI.LayoutEDI, pagamentoEDI.Pagamento.Empresa);
                return serGeracaoEDI.GerarArquivoRecursivo(edi);
            }
            else if (pagamentoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.ImportsysVP)
            {
                EDI.ImportsysVP svcImportsysVP = new EDI.ImportsysVP();
                dynamic edi = svcImportsysVP.ConverterPagamentoParaImportsysVP(pagamentoEDI, unidadeDeTrabalho);
                GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, pagamentoEDI.LayoutEDI, pagamentoEDI.Pagamento.Empresa);
                return serGeracaoEDI.GerarArquivoRecursivo(edi);
            }
            else
                return null;
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao cancelamentoEDI, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (cancelamentoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR)
            {
                // AINDA NÃO IMPLEMENTADO PELO SAP DA DANONE

                //EDI.INTPFAR svcINTPFAR = new EDI.INTPFAR();
                //dynamic edi = svcINTPFAR.ConverterPagamentoParaINTPFAR(cancelamentoEDI.CancelamentoPagamento, unidadeDeTrabalho);
                //GeracaoEDI serGeracaoEDI = new GeracaoEDI(cancelamentoEDI.CancelamentoPagamento.Empresa, cancelamentoEDI.LayoutEDI, unidadeDeTrabalho.StringConexao);
                //return serGeracaoEDI.GerarArquivoRecursivo(edi);
                return null;
            }
            else
                return null;
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao provisaoEDI, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (provisaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV)
            {
                Servicos.Embarcador.Integracao.EDI.PROV svcEDI = new Servicos.Embarcador.Integracao.EDI.PROV();
                Dominio.ObjetosDeValor.EDI.PROV.Provisao obj = svcEDI.GerarPorProvisao(provisaoEDI.Provisao, unidadeDeTrabalho);

                if (obj != null)
                {
                    Servicos.GeracaoEDI svcGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, provisaoEDI.LayoutEDI);

                    return svcGeracaoEDI.GerarArquivoRecursivo(obj);
                }
                else
                    return null;
            }
            else if (provisaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PROV_INTPFAR)
            {
                EDI.INTPFAR svcINTPFAR = new EDI.INTPFAR();
                dynamic edi = svcINTPFAR.ConverterProvisaoParaINTPFAR(provisaoEDI.Provisao, unidadeDeTrabalho);
                GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, provisaoEDI.LayoutEDI);
                return serGeracaoEDI.GerarArquivoRecursivo(edi);
            }
            else
                return null;
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao loteEDIIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Embarcador.Integracao.EDI.EAI svcEAI = new Servicos.Embarcador.Integracao.EDI.EAI();
            Dominio.ObjetosDeValor.EDI.EAI.Lote lote = svcEAI.GerarPorLote(loteEDIIntegracao, unidadeDeTrabalho);

            Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unidadeDeTrabalho, loteEDIIntegracao.LayoutEDI, loteEDIIntegracao.Lote.Transportador);

            return svcEDI.GerarArquivoRecursivo(lote);
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeDeTrabalho);

            if (cargaCancelamentoIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CANCELAMENTO)
            {
                var ctes = (from o in cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.CargaCTes select o.CTe).ToList();
                Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unidadeDeTrabalho, cargaCancelamentoIntegracaoEDI.LayoutEDI, cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga?.Empresa, ctes);

                return svcEDI.GerarArquivo();
            }
            else if (cargaCancelamentoIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MasterSAFCancelamento)
            {
                Servicos.Embarcador.Integracao.EDI.MasterSAF svcMasterSAF = new EDI.MasterSAF(unidadeDeTrabalho);
                Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = svcMasterSAF.ConverterCargaCancelamentoParaMasterSAF(cargaCancelamentoIntegracaoEDI);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaCancelamentoIntegracaoEDI.LayoutEDI, cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(masterSAF);
            }
            else if (cargaCancelamentoIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO)
            {
                Servicos.Embarcador.Integracao.EDI.INTNC svcINTNC = new EDI.INTNC();
                Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = svcINTNC.ConverterCargaCancelamentoParaINTNC(cargaCancelamentoIntegracaoEDI, unidadeDeTrabalho);
                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, cargaCancelamentoIntegracaoEDI.LayoutEDI, cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga?.Empresa);
                return serGeracaoEDI.GerarArquivoRecursivo(intnc);
            }
            else
            {
                return null;
            }
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao ocorrenciaEDIIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unidadeDeTrabalho);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo, false);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesOcorrencia = (from obj in cargaCTeComplementoInfo select obj.CTe).ToList();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = ocorrenciaEDIIntegracao.CargaOcorrencia.Carga;

                if (carga == null)
                    carga = ocorrenciaEDIIntegracao.CargaOcorrencia.Cargas.FirstOrDefault();

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();
                Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB conemb = svcCONEMB.ConverterCargaCTeParaCONEMB_MartinBrower(ctesOcorrencia, carga, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ctesOcorrencia.First().Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(conemb);
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo, false);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesOcorrencia = (from obj in cargaCTeComplementoInfo select obj.CTe).ToList();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = ocorrenciaEDIIntegracao.CargaOcorrencia.Carga;

                if (carga == null)
                    carga = ocorrenciaEDIIntegracao.CargaOcorrencia.Cargas.FirstOrDefault();

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();
                Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao conemb = svcCONEMB.ConverterCargaCTeParaCONEMB_CaterpillarImportacao(ctesOcorrencia, carga, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ctesOcorrencia.First().Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(conemb);
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo, false);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesOcorrencia = (from obj in cargaCTeComplementoInfo select obj.CTe).ToList();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = ocorrenciaEDIIntegracao.CargaOcorrencia.Carga;

                if (carga == null)
                    carga = ocorrenciaEDIIntegracao.CargaOcorrencia.Cargas.FirstOrDefault();

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new EDI.CONEMB();
                Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao conemb = svcCONEMB.ConverterCargaCTeParaCONEMB_CaterpillarExportacao(ctesOcorrencia, carga, unidadeDeTrabalho);

                Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ctesOcorrencia.First().Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(conemb);
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo, false);
                ctes.AddRange((from obj in cargaCTeComplementoInfo select obj.CTe).ToList());

                Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.CargaOcorrencia.Carga?.Empresa, ctes);

                return svcEDI.GerarArquivo();
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo);
                ctes.AddRange((from obj in cargaOcorrenciaDocumentos where obj.CargaCTe != null select obj.CargaCTe.CTe).ToList());

                Servicos.Embarcador.Integracao.EDI.OCOREN svcOCOREN = new EDI.OCOREN();

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                if (ocorrenciaEDIIntegracao.XMLNotaFiscal != null)
                    xMLNotasFiscais.Add(ocorrenciaEDIIntegracao.XMLNotaFiscal);

                Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ocoren = svcOCOREN.ConverterParaOCOREN(xMLNotasFiscais, ctes, ocorrenciaEDIIntegracao.CargaOcorrencia.TipoOcorrencia, ocorrenciaEDIIntegracao.CargaOcorrencia.CodigoTipoOcorrenciaParaIntegracao, ocorrenciaEDIIntegracao.CargaOcorrencia.DataOcorrencia, ocorrenciaEDIIntegracao.CargaOcorrencia.DataEvento, ocorrenciaEDIIntegracao.CargaOcorrencia.Observacao, unidadeDeTrabalho, ocorrenciaEDIIntegracao.CargaOcorrencia, ocorrenciaEDIIntegracao.LayoutEDI);

                Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.CargaOcorrencia.Carga?.Empresa);

                return svcEDI.GerarArquivoRecursivo(ocoren);
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC)
            {
                if (ocorrenciaEDIIntegracao.CargaOcorrencia.Carga != null)
                {
                    Servicos.Embarcador.Integracao.EDI.INTNC svcINTNC = new EDI.INTNC();
                    Dominio.ObjetosDeValor.EDI.INTNC.INTNC intnc = svcINTNC.ConverterOcorrenciaParaINTNC(ocorrenciaEDIIntegracao, unidadeDeTrabalho);

                    Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.Empresa != null ? ocorrenciaEDIIntegracao.Empresa : ocorrenciaEDIIntegracao.CargaOcorrencia.Carga?.Empresa);

                    return serGeracaoEDI.GerarArquivoRecursivo(intnc);
                }
                else
                {
                    return null;
                }
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.MarterSAF)
            {
                if (ocorrenciaEDIIntegracao.CargaOcorrencia.Carga != null)
                {
                    Servicos.Embarcador.Integracao.EDI.MasterSAF svcMasterSAF = new EDI.MasterSAF(unidadeDeTrabalho);
                    Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = svcMasterSAF.ConverterOcorrenciaParaMasterSAF(ocorrenciaEDIIntegracao);

                    Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.Empresa != null ? ocorrenciaEDIIntegracao.Empresa : ocorrenciaEDIIntegracao.CargaOcorrencia.Carga?.Empresa);

                    return serGeracaoEDI.GerarArquivoRecursivo(masterSAF);
                }
                else
                {
                    return null;
                }
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DESPESACOMPLEMENTAR)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo);
                ctes.AddRange((from obj in cargaOcorrenciaDocumentos where obj.CargaCTe != null select obj.CargaCTe.CTe).ToList());

                Servicos.Embarcador.Integracao.EDI.DESPESACOMPLEMENTAR svcDESPESACOMPLEMENTAR = new EDI.DESPESACOMPLEMENTAR();

                Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR.DNE Dne = svcDESPESACOMPLEMENTAR.ConverterParaDESPESACOMPLEMENTAR(ctes, ocorrenciaEDIIntegracao.CargaOcorrencia, ocorrenciaEDIIntegracao.CargaOcorrencia.TipoOcorrencia, ocorrenciaEDIIntegracao.CargaOcorrencia.DataOcorrencia, ocorrenciaEDIIntegracao.CargaOcorrencia.Observacao, unidadeDeTrabalho);

                Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.CargaOcorrencia.Carga?.Empresa);

                return svcEDI.GerarArquivoRecursivo(Dne);
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OcorenOTIF)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo);
                ctes.AddRange((from obj in cargaOcorrenciaDocumentos where obj.CargaCTe != null select obj.CargaCTe.CTe).ToList());

                Servicos.Embarcador.Integracao.EDI.OCOREN svcOCOREN = new EDI.OCOREN();
                return svcOCOREN.GerarArquivoOcorenOTIF(ctes, ocorrenciaEDIIntegracao.CargaOcorrencia.TipoOcorrencia, ocorrenciaEDIIntegracao.CargaOcorrencia.DataOcorrencia, ocorrenciaEDIIntegracao.CargaOcorrencia.DataEvento ?? ocorrenciaEDIIntegracao.CargaOcorrencia.DataOcorrencia, unidadeDeTrabalho);
            }
            else if (ocorrenciaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_NF)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo, false);
                ctes.AddRange(cargaCTeComplementoInfo.Select(o => o.CTe));

                Dominio.ObjetosDeValor.EDI.CONEMB_NF.Arquivo arquivo = Servicos.Embarcador.Integracao.EDI.CONEMB_NF.ConverterParaCONEMB_NF(ctes);

                Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unidadeDeTrabalho, ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.CargaOcorrencia.Carga?.Empresa);

                return svcEDI.GerarArquivoRecursivo(arquivo);
            }
            else
            {
                return null;
            }
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if ((integracao.LayoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.GEN) || (integracao.FechamentoFrete.Contrato.Transportador == null))
                return null;

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeDeTrabalho);
            EDI.GEN svcGEN = new EDI.GEN();
            GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, integracao.LayoutEDI, integracao.FechamentoFrete.Contrato.Transportador);

            List<int> codigosCargaOcorrencias = (from o in integracao.FechamentoFrete.Ocorrencias
                                                 where
         o.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal != null
         && o.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe
         && o.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe
         && o.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS
                                                 select o.Ocorrencia.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> complementos = repCargaCTeComplementoInfo.BuscarPorOcorrencias(codigosCargaOcorrencias);
            Dominio.ObjetosDeValor.EDI.GEN.Cabecalho gen = svcGEN.ConverterOcorrenciaParaGEN(complementos, integracao.FechamentoFrete, unidadeDeTrabalho);

            return serGeracaoEDI.GerarArquivoRecursivo(gen);
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic edi = null;

            if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB_CT)
            {
                EDI.DOCCOB svcDOCCOB = new EDI.DOCCOB();
                edi = svcDOCCOB.ConverterFaturaParaDOCCOBCaterpillar(faturaIntegracao.Fatura, unidadeDeTrabalho);
            }
            else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB)
            {
                EDI.DOCCOB svcDOCCOB = new EDI.DOCCOB();
                edi = svcDOCCOB.ConverterFaturaParaDOCCOB(faturaIntegracao.Fatura, unidadeDeTrabalho);
            }
            else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP)
            {
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repFaturaDocumento.BuscarConhecimentos(faturaIntegracao.Fatura.Codigo);

                Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new Servicos.Embarcador.Integracao.EDI.CONEMB();
                edi = svcCONEMB.ConverterCargaCTeParaCONEMB_CaterpillarImportacao(ctes, null, unidadeDeTrabalho);
            }
            else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR)
            {
                bool gerarPorCarga = false;
                if (faturaIntegracao.Fatura.TipoOperacao != null && faturaIntegracao.Fatura.TipoOperacao.GerarEDIFaturaPorCarga)
                {
                    gerarPorCarga = true;
                }
                EDI.INTPFAR svcINTPFAR = new EDI.INTPFAR();

                if (!gerarPorCarga)//todo: mudar aqui quando criar em produção a escrituração.
                {
                    if (faturaIntegracao.Fatura.NovoModelo)
                    {
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                        if (configuracaoEmbarcador.TipoImpressaoFatura != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura.Multimodal && (faturaIntegracao.Fatura.ClienteTomadorFatura?.GrupoPessoas?.GerarFaturaPorCte ?? false))
                            edi = svcINTPFAR.ConverterFaturaParaINTPFARPorCte(faturaIntegracao, faturaIntegracao.Empresa, unidadeDeTrabalho);
                        else
                            edi = svcINTPFAR.ConverterFaturaParaINTPFAR(faturaIntegracao, faturaIntegracao.Empresa, unidadeDeTrabalho);
                    }
                    else
                        edi = svcINTPFAR.ConverterFaturaAntigaParaINTPFAR(faturaIntegracao, faturaIntegracao.Empresa, unidadeDeTrabalho);
                }
                else
                    edi = svcINTPFAR.ConverterCargaParaINTPFAR(faturaIntegracao, unidadeDeTrabalho);
            }
            else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB_VAXXINOVA)
            {
                EDI.DOCCOB svcDOCCOB = new EDI.DOCCOB();
                edi = svcDOCCOB.ConverterFaturaParaDOCCOBVaxxinova(faturaIntegracao.Fatura, unidadeDeTrabalho);
            }
            else if (faturaIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PREFAT)
            {
                EDI.PREFAT prefat = new EDI.PREFAT();
                edi = prefat.ConverterFaturaParaPREFAT(faturaIntegracao.Fatura, unidadeDeTrabalho);
            }

            GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, faturaIntegracao.LayoutEDI, faturaIntegracao.Fatura.Empresa);
            return serGeracaoEDI.GerarArquivoRecursivo(edi);
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.Entidades.LayoutEDI layoutEDI, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.EDI.DOCCOB.EDIDOCCOB edi = null;

            if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB)
            {
                EDI.DOCCOB svcDOCCOB = new EDI.DOCCOB();
                edi = svcDOCCOB.ConverterTituloParaDOCCOB(titulo, unidadeDeTrabalho);
            }
            GeracaoEDI serGeracaoEDI = new GeracaoEDI(unidadeDeTrabalho, layoutEDI, titulo.Empresa);
            return serGeracaoEDI.GerarArquivoRecursivo(edi);
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI loteContabilizacaoIntegracaoEDI, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string extensao)
        {
            extensao = ".txt";

            if (loteContabilizacaoIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR)
            {
                Servicos.Embarcador.Integracao.EDI.INTPFAR svcINTPFAR = new Servicos.Embarcador.Integracao.EDI.INTPFAR();

                Dominio.ObjetosDeValor.EDI.INTPFAR.INTPFAR edi = svcINTPFAR.ConverterLoteContabilizacaoParaINTPFAR(loteContabilizacaoIntegracaoEDI, unitOfWork);

                Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, loteContabilizacaoIntegracaoEDI.LayoutEDI, loteContabilizacaoIntegracaoEDI.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(edi);
            }

            return null;
        }

        public static System.IO.MemoryStream GerarEDI(Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI loteClienteIntegracaoEDI, Repositorio.UnitOfWork unitOfWork, out string extensao)
        {
            extensao = ".txt";

            if (loteClienteIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.Cliente)
            {
                Servicos.Embarcador.Integracao.EDI.LoteCliente svcLoteCliente = new Servicos.Embarcador.Integracao.EDI.LoteCliente(unitOfWork);

                Dominio.ObjetosDeValor.EDI.LoteCliente.LoteCliente edi = svcLoteCliente.ConverterLoteClienteParaEDI(loteClienteIntegracaoEDI);

                Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, loteClienteIntegracaoEDI.LayoutEDI, loteClienteIntegracaoEDI.Empresa);

                return serGeracaoEDI.GerarArquivoRecursivo(edi);
            }

            return null;
        }

        #endregion

        #region NomenclaturaArquivos

        public static string ObterExtencaoPadrao(Dominio.Entidades.LayoutEDI layoutEDI)
        {
            string extensao = "txt";
            if (!string.IsNullOrWhiteSpace(layoutEDI.ExtensaoArquivo))
                extensao = layoutEDI.ExtensaoArquivo.Replace(".", "");
            extensao = "." + extensao;

            return extensao;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.Entidades.LayoutEDI layoutEDI, string extensao)
        {
            extensao = "";//ObterExtencaoPadrao(layoutEDI);
            if (layoutEDI != null && !string.IsNullOrWhiteSpace(layoutEDI.Nomenclatura))
                return ObterNomenclaturaLayoutEDI(carregamento.Codigo, layoutEDI.Nomenclatura, carregamento.Empresa, null, carregamento.NumeroCarregamento, carregamento.DataCriacao) + extensao;
            else
                return "EDI-" + layoutEDI.Tipo.ToString("g") + "-" + carregamento.NumeroCarregamento.ToString() + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.LayoutEDI layoutEDI, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            extensao = ObterExtencaoPadrao(layoutEDI);
            if (layoutEDI != null && !string.IsNullOrWhiteSpace(layoutEDI.Nomenclatura))
            {
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);

                string numeroFatura = layoutEDI.Nomenclatura.Contains("#NumeroFatura#") ? (repFaturaDocumento.BuscarPrimeiroNumeroFaturaPorCTe(cte.Codigo)?.ToString() ?? string.Empty) : string.Empty;
                int protocoloCarga = cte.ProtocoloCarga;

                return ObterNomenclaturaLayoutEDI(cte.Codigo, layoutEDI.Nomenclatura, cte.Empresa, cte.TomadorPagador.Cliente, cte.Numero.ToString(), cte.DataEmissao.Value, string.Empty, numeroFatura, protocoloCarga) + extensao;
            }
            else
                return "EDI-" + layoutEDI.Tipo.ToString("g") + "-" + cte.Numero.ToString() + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao loteEscrituracaoEDIIntegracao, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(loteEscrituracaoEDIIntegracao.NomeArquivo))
            {
                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repLoteEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unitOfWork);

                if (extensao != ".zip")
                    extensao = ObterExtencaoPadrao(loteEscrituracaoEDIIntegracao.LayoutEDI);

                if (loteEscrituracaoEDIIntegracao.LayoutEDI != null && !string.IsNullOrWhiteSpace(loteEscrituracaoEDIIntegracao.LayoutEDI.Nomenclatura))
                    loteEscrituracaoEDIIntegracao.NomeArquivo = ObterNomenclaturaLayoutEDI(loteEscrituracaoEDIIntegracao.Codigo, loteEscrituracaoEDIIntegracao.LayoutEDI.Nomenclatura, loteEscrituracaoEDIIntegracao.LoteEscrituracao.Empresa, loteEscrituracaoEDIIntegracao.LoteEscrituracao.Tomador, loteEscrituracaoEDIIntegracao.LoteEscrituracao.Numero.ToString(), loteEscrituracaoEDIIntegracao.LoteEscrituracao.DataGeracaoLote.HasValue ? loteEscrituracaoEDIIntegracao.LoteEscrituracao.DataGeracaoLote.Value : DateTime.Now) + extensao;
                else
                    loteEscrituracaoEDIIntegracao.NomeArquivo = "EDI-" + loteEscrituracaoEDIIntegracao.LayoutEDI.Tipo.ToString("g") + "-" + loteEscrituracaoEDIIntegracao.LoteEscrituracao.Numero.ToString() + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(loteEscrituracaoEDIIntegracao.CodigosCTes, loteEscrituracaoEDIIntegracao.LayoutEDI, loteEscrituracaoEDIIntegracao.NomeArquivo, unitOfWork);

                repLoteEscrituracaoEDIIntegracao.Atualizar(loteEscrituracaoEDIIntegracao);
            }
            return loteEscrituracaoEDIIntegracao.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(loteEscrituracaoCancelamentoEDIIntegracao.NomeArquivo))
            {
                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

                if (extensao != ".zip")
                    extensao = ObterExtencaoPadrao(loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI);

                if (loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI != null && !string.IsNullOrWhiteSpace(loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Nomenclatura))
                    loteEscrituracaoCancelamentoEDIIntegracao.NomeArquivo = ObterNomenclaturaLayoutEDI(loteEscrituracaoCancelamentoEDIIntegracao.Codigo, loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Nomenclatura, loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Empresa, loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Tomador, loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Numero.ToString(), loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.DataGeracaoLote.HasValue ? loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.DataGeracaoLote.Value : DateTime.Now) + extensao;
                else
                    loteEscrituracaoCancelamentoEDIIntegracao.NomeArquivo = "EDI-" + loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Tipo.ToString("g") + "-" + loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento.Numero.ToString() + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(loteEscrituracaoCancelamentoEDIIntegracao.CodigosCTes, loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI, loteEscrituracaoCancelamentoEDIIntegracao.NomeArquivo, unitOfWork);

                repLoteEscrituracaoCancelamentoEDIIntegracao.Atualizar(loteEscrituracaoCancelamentoEDIIntegracao);
            }

            return loteEscrituracaoCancelamentoEDIIntegracao.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao nfsManualEDIIntegracao, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(nfsManualEDIIntegracao.NomeArquivo))
            {
                Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(unitOfWork);

                if (extensao != ".zip")
                    extensao = ObterExtencaoPadrao(nfsManualEDIIntegracao.LayoutEDI);

                if (nfsManualEDIIntegracao.LayoutEDI != null && !string.IsNullOrWhiteSpace(nfsManualEDIIntegracao.LayoutEDI.Nomenclatura))
                    nfsManualEDIIntegracao.NomeArquivo = ObterNomenclaturaLayoutEDI(nfsManualEDIIntegracao.Codigo, nfsManualEDIIntegracao.LayoutEDI.Nomenclatura, nfsManualEDIIntegracao.LancamentoNFSManual.Transportador, nfsManualEDIIntegracao.LancamentoNFSManual.Tomador, nfsManualEDIIntegracao.LancamentoNFSManual.DadosNFS.Numero.ToString(), nfsManualEDIIntegracao.LancamentoNFSManual.DadosNFS?.DataEmissao ?? DateTime.Now) + extensao;
                else
                    nfsManualEDIIntegracao.NomeArquivo = "EDI-" + nfsManualEDIIntegracao.LayoutEDI.Tipo.ToString("g") + "-" + nfsManualEDIIntegracao.LancamentoNFSManual.DadosNFS.Numero.ToString() + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(nfsManualEDIIntegracao.CodigosCTes, nfsManualEDIIntegracao.LayoutEDI, nfsManualEDIIntegracao.NomeArquivo, unitOfWork);

                repNFSManualEDIIntegracao.Atualizar(nfsManualEDIIntegracao);
            }
            return nfsManualEDIIntegracao.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI nfsManualCancelamentoIntegracaoEDI, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(nfsManualCancelamentoIntegracaoEDI.NomeArquivo))
            {
                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unitOfWork);

                if (extensao != ".zip")
                    extensao = ObterExtencaoPadrao(nfsManualCancelamentoIntegracaoEDI.LayoutEDI);

                if (nfsManualCancelamentoIntegracaoEDI.LayoutEDI != null && !string.IsNullOrWhiteSpace(nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Nomenclatura))
                    nfsManualCancelamentoIntegracaoEDI.NomeArquivo = ObterNomenclaturaLayoutEDI(nfsManualCancelamentoIntegracaoEDI.Codigo, nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Nomenclatura, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.Transportador, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.Tomador, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.DadosNFS.Numero.ToString(), nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.DadosNFS?.DataEmissao ?? DateTime.Now) + extensao;
                else
                    nfsManualCancelamentoIntegracaoEDI.NomeArquivo = "EDI-" + nfsManualCancelamentoIntegracaoEDI.LayoutEDI.Tipo.ToString("g") + "-" + nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.DadosNFS.Numero.ToString() + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(nfsManualCancelamentoIntegracaoEDI.CodigosCTes, nfsManualCancelamentoIntegracaoEDI.LayoutEDI, nfsManualCancelamentoIntegracaoEDI.NomeArquivo, unitOfWork);

                repNFSManualCancelamentoIntegracaoEDI.Atualizar(nfsManualCancelamentoIntegracaoEDI);
            }

            return nfsManualCancelamentoIntegracaoEDI.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(cargaEDIIntegracao.NomeArquivo))
            {
                Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);

                if (extensao != ".zip")
                    extensao = ObterExtencaoPadrao(cargaEDIIntegracao.LayoutEDI);

                if (cargaEDIIntegracao.LayoutEDI != null && !string.IsNullOrWhiteSpace(cargaEDIIntegracao.LayoutEDI.Nomenclatura))
                {
                    Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);

                    string numeroFatura = cargaEDIIntegracao.LayoutEDI.Nomenclatura.Contains("#NumeroFatura#") ? (repFaturaDocumento.BuscarPrimeiroNumeroFaturaPorCargaCTe(cargaEDIIntegracao.Carga.Codigo)?.ToString() ?? string.Empty) : string.Empty;
                    int protocoloCarga = cargaEDIIntegracao.Carga.Protocolo;

                    cargaEDIIntegracao.NomeArquivo = ObterNomenclaturaLayoutEDI(cargaEDIIntegracao.Codigo, cargaEDIIntegracao.LayoutEDI.Nomenclatura, cargaEDIIntegracao.Carga.Empresa, cargaEDIIntegracao.Carga.Pedidos.FirstOrDefault().ObterTomador(), cargaEDIIntegracao.Carga.CodigoCargaEmbarcador, cargaEDIIntegracao.CTe != null ? cargaEDIIntegracao.DataIntegracao : cargaEDIIntegracao.Carga.DataFinalizacaoEmissao.HasValue ? cargaEDIIntegracao.Carga.DataFinalizacaoEmissao.Value : cargaEDIIntegracao.Carga.DataCriacaoCarga, "", numeroFatura, protocoloCarga) + extensao;
                }
                else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL && extensao != ".zip")
                    cargaEDIIntegracao.NomeArquivo = cargaEDIIntegracao.Carga.CargaMDFes.Where(o => o.MDFe.EstadoDescarregamento.Sigla == "MT" || o.MDFe.Percursos.Any(perc => perc.Estado.Sigla == "MT")).Select(o => o.MDFe.Chave).First() + " - EDI Fiscal.txt";
                else if (cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.UVT_RN)
                    cargaEDIIntegracao.NomeArquivo = $"{cargaEDIIntegracao.Carga?.Empresa?.InscricaoEstadual}_{cargaEDIIntegracao.Carga.Veiculo?.Placa}_{cargaEDIIntegracao.Carga.Motoristas?.FirstOrDefault()?.CPF}_{cargaEDIIntegracao.DataIntegracao:ddMMyyyyHHmmss}{extensao}";
                else
                    cargaEDIIntegracao.NomeArquivo = "EDI-" + cargaEDIIntegracao.LayoutEDI.Tipo.ToString("g") + "-" + cargaEDIIntegracao.Carga.CodigoCargaEmbarcador + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(cargaEDIIntegracao.CodigosCTes, cargaEDIIntegracao.LayoutEDI, cargaEDIIntegracao.NomeArquivo, unitOfWork);

                repCargaEDIIntegracao.Atualizar(cargaEDIIntegracao);
            }

            return cargaEDIIntegracao.NomeArquivo;


        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao ocorrenciaEDIIntegracao, bool incrementarSequencia, Repositorio.UnitOfWork unitOfWork, bool canhoto = false)
        {
            string extensao = ObterExtencaoPadrao(ocorrenciaEDIIntegracao.LayoutEDI);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);

            if (string.IsNullOrWhiteSpace(ocorrenciaEDIIntegracao.NomeArquivo))
            {
                Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao repOcorrenciaEDIIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao(unitOfWork);

                if (ocorrenciaEDIIntegracao.CargaOcorrencia.Carga != null && ocorrenciaEDIIntegracao.LayoutEDI != null && !string.IsNullOrWhiteSpace(ocorrenciaEDIIntegracao.LayoutEDI.Nomenclatura))
                {
                    string numeroFatura = ocorrenciaEDIIntegracao.LayoutEDI.Nomenclatura.Contains("#NumeroFatura#") ? (repFaturaDocumento.BuscarPrimeiroNumeroFaturaPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo)?.ToString() ?? string.Empty) : string.Empty;
                    int protocoloCarga = ocorrenciaEDIIntegracao.CargaOcorrencia?.Carga?.Protocolo ?? 0;

                    Dominio.Entidades.Cliente cliente = ocorrenciaEDIIntegracao.CargaOcorrencia.Carga.CargaOrigemPedidos.FirstOrDefault().ObterTomador();
                    string numero = ocorrenciaEDIIntegracao.XMLNotaFiscal != null ? (ocorrenciaEDIIntegracao.XMLNotaFiscal.Numero.ToString() + "_" + ocorrenciaEDIIntegracao.XMLNotaFiscal.Serie.ToString()) : (ocorrenciaEDIIntegracao.CargaOcorrencia.NumeroOcorrencia.ToString());
                    ocorrenciaEDIIntegracao.NomeArquivo = ObterNomenclaturaLayoutEDI(incrementarSequencia ? ocorrenciaEDIIntegracao.ObterSequencia() : ocorrenciaEDIIntegracao.Codigo, ocorrenciaEDIIntegracao.LayoutEDI.Nomenclatura, ocorrenciaEDIIntegracao.CargaOcorrencia.Carga.Empresa, ocorrenciaEDIIntegracao.XMLNotaFiscal?.Emitente ?? cliente, numero, ocorrenciaEDIIntegracao.CargaOcorrencia.DataAlteracao, ocorrenciaEDIIntegracao.CargaOcorrencia.CodigoTipoOcorrenciaParaIntegracao, numeroFatura, protocoloCarga, ocorrenciaEDIIntegracao.CargaOcorrencia.TipoOcorrencia?.Descricao ?? "") + extensao;
                }
                else
                    ocorrenciaEDIIntegracao.NomeArquivo = ObterNomeArquivoEDI(ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.CargaOcorrencia.NumeroOcorrencia.ToString());

                VincularCTesArquivo(ocorrenciaEDIIntegracao.CodigosCTes, ocorrenciaEDIIntegracao.LayoutEDI, ocorrenciaEDIIntegracao.NomeArquivo, unitOfWork);

                repOcorrenciaEDIIntegracao.Atualizar(ocorrenciaEDIIntegracao);
            }

            if (canhoto)//regra fixa para acrescentar os zeros a esquerda quando for canhoto.
            {
                Dominio.Entidades.Cliente cliente = ocorrenciaEDIIntegracao.CargaOcorrencia.Carga.CargaOrigemPedidos.FirstOrDefault().ObterTomador();
                string numeroFatura = ocorrenciaEDIIntegracao.LayoutEDI.Nomenclatura.Contains("#NumeroFatura#") ? (repFaturaDocumento.BuscarPrimeiroNumeroFaturaPorOcorrencia(ocorrenciaEDIIntegracao.CargaOcorrencia.Codigo)?.ToString() ?? string.Empty) : string.Empty;
                string numero = ocorrenciaEDIIntegracao.XMLNotaFiscal != null ? (ocorrenciaEDIIntegracao.XMLNotaFiscal.Numero.ToString().PadLeft(8, '0') + "_" + ocorrenciaEDIIntegracao.XMLNotaFiscal.Serie.ToString().PadLeft(3, '0')) : (ocorrenciaEDIIntegracao.CargaOcorrencia.NumeroOcorrencia.ToString());
                int protocoloCarga = ocorrenciaEDIIntegracao.CargaOcorrencia?.Carga?.Protocolo ?? 0;
                return ObterNomenclaturaLayoutEDI(incrementarSequencia ? ocorrenciaEDIIntegracao.ObterSequencia() : ocorrenciaEDIIntegracao.Codigo, ocorrenciaEDIIntegracao.LayoutEDI.Nomenclatura, ocorrenciaEDIIntegracao.CargaOcorrencia.Carga.Empresa, ocorrenciaEDIIntegracao.XMLNotaFiscal?.Emitente ?? cliente, numero, ocorrenciaEDIIntegracao.CargaOcorrencia.DataAlteracao, ocorrenciaEDIIntegracao.CargaOcorrencia.CodigoTipoOcorrenciaParaIntegracao, numeroFatura, protocoloCarga, ocorrenciaEDIIntegracao.CargaOcorrencia.TipoOcorrencia?.Descricao ?? "") + extensao;
            }


            return ocorrenciaEDIIntegracao.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.LayoutEDI layoutEDI, string numero)
        {
            string nomeArquivo = "EDI-" + layoutEDI.Tipo.ToString("g") + "-" + numero + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ObterExtencaoPadrao(layoutEDI);
            return nomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(cargaCancelamentoIntegracaoEDI.NomeArquivo))
            {
                Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);

                string extensao = ObterExtencaoPadrao(cargaCancelamentoIntegracaoEDI.LayoutEDI);
                if (cargaCancelamentoIntegracaoEDI.LayoutEDI != null && !string.IsNullOrWhiteSpace(cargaCancelamentoIntegracaoEDI.LayoutEDI.Nomenclatura))
                    cargaCancelamentoIntegracaoEDI.NomeArquivo = ObterNomenclaturaLayoutEDI(cargaCancelamentoIntegracaoEDI.Codigo, cargaCancelamentoIntegracaoEDI.LayoutEDI.Nomenclatura, cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.Empresa, null, cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.CodigoCargaEmbarcador, cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.DataCriacaoCarga) + extensao;
                else
                    cargaCancelamentoIntegracaoEDI.NomeArquivo = "EDI-" + cargaCancelamentoIntegracaoEDI.LayoutEDI.Tipo.ToString("g") + "-" + cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.CodigoCargaEmbarcador + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(cargaCancelamentoIntegracaoEDI.CodigosCTes, cargaCancelamentoIntegracaoEDI.LayoutEDI, cargaCancelamentoIntegracaoEDI.NomeArquivo, unitOfWork);

                repCargaCancelamentoIntegracaoEDI.Atualizar(cargaCancelamentoIntegracaoEDI);
            }
            return cargaCancelamentoIntegracaoEDI.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao loteEDIIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(loteEDIIntegracao.NomeArquivo))
            {
                Repositorio.Embarcador.Avarias.LoteEDIIntegracao repLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(unitOfWork);
                string extensao = ObterExtencaoPadrao(loteEDIIntegracao.LayoutEDI);
                if (loteEDIIntegracao.LayoutEDI != null && !string.IsNullOrWhiteSpace(loteEDIIntegracao.LayoutEDI.Nomenclatura))
                    loteEDIIntegracao.NomeArquivo = ObterNomenclaturaLayoutEDI(loteEDIIntegracao.Codigo, loteEDIIntegracao.LayoutEDI.Nomenclatura, loteEDIIntegracao.Lote.Transportador, null, loteEDIIntegracao.Lote.Numero.ToString(), loteEDIIntegracao.Lote.DataGeracao) + extensao;
                else
                    loteEDIIntegracao.NomeArquivo = "EDI-" + loteEDIIntegracao.LayoutEDI.Tipo.ToString("g") + "-" + loteEDIIntegracao.Lote.Numero + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(loteEDIIntegracao.CodigosCTes, loteEDIIntegracao.LayoutEDI, loteEDIIntegracao.NomeArquivo, unitOfWork);

                repLoteEDIIntegracao.Atualizar(loteEDIIntegracao);
            }
            return loteEDIIntegracao.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao cancelamentoProvisaoEDI, bool incrementarSequencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(cancelamentoProvisaoEDI.NomeArquivo))
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao repCancelamentoProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao(unitOfWork);
                string extensao = ObterExtencaoPadrao(cancelamentoProvisaoEDI.LayoutEDI);
                if (cancelamentoProvisaoEDI.LayoutEDI != null && !string.IsNullOrWhiteSpace(cancelamentoProvisaoEDI.LayoutEDI.Nomenclatura))
                    cancelamentoProvisaoEDI.NomeArquivo = ObterNomenclaturaLayoutEDI(incrementarSequencia ? cancelamentoProvisaoEDI.ObterSequencia() : 1, cancelamentoProvisaoEDI.LayoutEDI.Nomenclatura, cancelamentoProvisaoEDI.CancelamentoProvisao.Empresa, null, cancelamentoProvisaoEDI.CancelamentoProvisao.Numero.ToString(), cancelamentoProvisaoEDI.CancelamentoProvisao.DataCriacao) + extensao;
                else
                    cancelamentoProvisaoEDI.NomeArquivo = "EDI-" + cancelamentoProvisaoEDI.LayoutEDI.Tipo.ToString("g") + "-" + cancelamentoProvisaoEDI.CancelamentoProvisao.Numero + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(cancelamentoProvisaoEDI.CodigosCTes, cancelamentoProvisaoEDI.LayoutEDI, cancelamentoProvisaoEDI.NomeArquivo, unitOfWork);

                repCancelamentoProvisaoEDIIntegracao.Atualizar(cancelamentoProvisaoEDI);
            }
            return cancelamentoProvisaoEDI.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao provisaoEDI, bool incrementarSequencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(provisaoEDI.NomeArquivo))
            {
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);
                string extensao = ObterExtencaoPadrao(provisaoEDI.LayoutEDI);

                if (provisaoEDI.LayoutEDI != null && !string.IsNullOrWhiteSpace(provisaoEDI.LayoutEDI.Nomenclatura))
                {
                    int idRegistro = incrementarSequencia ? provisaoEDI.ObterSequencia() : 1;
                    Dominio.Entidades.Cliente cliente = null;
                    Dominio.Entidades.Empresa transportador = provisaoEDI.Provisao.Transportadores?.Count == 1 ? provisaoEDI.Provisao.Transportadores.FirstOrDefault() : null;
                    string nomenclatura = ObterNomenclaturaLayoutEDI(idRegistro, provisaoEDI.LayoutEDI.Nomenclatura, transportador, cliente, provisaoEDI.Provisao.Numero.ToString(), provisaoEDI.Provisao.DataCriacao);

                    provisaoEDI.NomeArquivo = $"{nomenclatura}{extensao}";
                }
                else
                    provisaoEDI.NomeArquivo = $"EDI-{provisaoEDI.LayoutEDI.Tipo.ToString("g")}-{provisaoEDI.Provisao.Numero}-{DateTime.Now.ToString("ddMMyyyyHHmmss")}{extensao}";

                VincularCTesArquivo(provisaoEDI.CodigosCTes, provisaoEDI.LayoutEDI, provisaoEDI.NomeArquivo, unitOfWork);

                repProvisaoEDIIntegracao.Atualizar(provisaoEDI);
            }

            return provisaoEDI.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDI, bool incrementarSequencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(pagamentoEDI.NomeArquivo))
            {
                Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(unitOfWork);
                string extensao = ObterExtencaoPadrao(pagamentoEDI.LayoutEDI);
                if (pagamentoEDI.LayoutEDI != null && !string.IsNullOrWhiteSpace(pagamentoEDI.LayoutEDI.Nomenclatura))
                    pagamentoEDI.NomeArquivo = ObterNomenclaturaLayoutEDI(incrementarSequencia ? pagamentoEDI.ObterSequencia() : pagamentoEDI.Codigo, pagamentoEDI.LayoutEDI.Nomenclatura, pagamentoEDI.Pagamento.Empresa, null, pagamentoEDI.Pagamento.Numero.ToString(), pagamentoEDI.Pagamento.DataCriacao) + extensao;
                else
                    pagamentoEDI.NomeArquivo = "EDI-" + pagamentoEDI.LayoutEDI.Tipo.ToString("g") + "-" + pagamentoEDI.Pagamento.Numero + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(pagamentoEDI.CodigosCTes, pagamentoEDI.LayoutEDI, pagamentoEDI.NomeArquivo, unitOfWork);

                repPagamentoEDIIntegracao.Atualizar(pagamentoEDI);
            }
            return pagamentoEDI.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao cancelamentoEDI, bool incrementarSequencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(cancelamentoEDI.NomeArquivo))
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao repCancelamentoPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao(unitOfWork);
                string extensao = ObterExtencaoPadrao(cancelamentoEDI.LayoutEDI);
                if (cancelamentoEDI.LayoutEDI != null && !string.IsNullOrWhiteSpace(cancelamentoEDI.LayoutEDI.Nomenclatura))
                    cancelamentoEDI.NomeArquivo = ObterNomenclaturaLayoutEDI(incrementarSequencia ? cancelamentoEDI.ObterSequencia() : 1, cancelamentoEDI.LayoutEDI.Nomenclatura, cancelamentoEDI.CancelamentoPagamento.Empresa, null, cancelamentoEDI.CancelamentoPagamento.Numero.ToString(), cancelamentoEDI.CancelamentoPagamento.DataCriacao) + extensao;
                else
                    cancelamentoEDI.NomeArquivo = "EDI-" + cancelamentoEDI.LayoutEDI.Tipo.ToString("g") + "-" + cancelamentoEDI.CancelamentoPagamento.Numero + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(cancelamentoEDI.CodigosCTes, cancelamentoEDI.LayoutEDI, cancelamentoEDI.NomeArquivo, unitOfWork);

                repCancelamentoPagamentoEDIIntegracao.Atualizar(cancelamentoEDI);
            }
            return cancelamentoEDI.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.Entidades.LayoutEDI layoutEDI, Repositorio.UnitOfWork unitOfWork, bool utilizarDataEncerramentoFatura, bool adicionarNomeLayout = false)
        {
            string nomeArquivo = "";
            string extensao = ObterExtencaoPadrao(layoutEDI);
            int protocoloCarga = titulo.Documentos?.Where(c => c.CTe != null).Select(c => c.CTe.ProtocoloCarga)?.FirstOrDefault() ?? 0;

            if (layoutEDI != null && !string.IsNullOrWhiteSpace(layoutEDI.Nomenclatura))
                nomeArquivo = ObterNomenclaturaLayoutEDI(titulo.Codigo, layoutEDI.Nomenclatura, titulo.Empresa, titulo.Pessoa, titulo.Codigo.ToString(), (utilizarDataEncerramentoFatura && titulo.DataVencimento.HasValue ? titulo.DataVencimento.Value : titulo.DataEmissao.Value), string.Empty, titulo.Codigo.ToString(), protocoloCarga) + extensao;
            else
                nomeArquivo = "EDI-" + layoutEDI.Tipo.ToString("g") + "-" + titulo.Codigo + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

            return nomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Repositorio.UnitOfWork unitOfWork, bool utilizarDataEncerramentoFatura, bool adicionarNomeLayout = false)
        {
            if (string.IsNullOrWhiteSpace(faturaIntegracao.NomeArquivo))
            {
                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
                string extensao = ObterExtencaoPadrao(faturaIntegracao.LayoutEDI);
                int protocoloCarga = faturaIntegracao.Fatura.Documentos?.Where(c => c.Documento != null && c.Documento.CTe != null).Select(c => c.Documento.CTe.ProtocoloCarga)?.FirstOrDefault() ?? 0;

                if (faturaIntegracao.LayoutEDI != null && !string.IsNullOrWhiteSpace(faturaIntegracao.LayoutEDI.Nomenclatura))
                    faturaIntegracao.NomeArquivo = ObterNomenclaturaLayoutEDI(faturaIntegracao.Codigo, faturaIntegracao.LayoutEDI.Nomenclatura, faturaIntegracao.Fatura.Empresa, faturaIntegracao.Fatura.ClienteTomadorFatura, faturaIntegracao.Fatura.Numero.ToString(), (utilizarDataEncerramentoFatura && faturaIntegracao.Fatura.DataFechamento.HasValue ? faturaIntegracao.Fatura.DataFechamento.Value : faturaIntegracao.Fatura.DataFatura), string.Empty, faturaIntegracao.Fatura.Numero.ToString(), protocoloCarga) + extensao;
                else
                    faturaIntegracao.NomeArquivo = "EDI-" + faturaIntegracao.LayoutEDI.Tipo.ToString("g") + "-" + faturaIntegracao.Fatura.Numero + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                if (adicionarNomeLayout)
                    faturaIntegracao.NomeArquivo = faturaIntegracao.LayoutEDI.Descricao.Replace("\t", "") + " " + faturaIntegracao.NomeArquivo;

                VincularCTesArquivo(faturaIntegracao.CodigosCTes, faturaIntegracao.LayoutEDI, faturaIntegracao.NomeArquivo, unitOfWork);

                repFaturaIntegracao.Atualizar(faturaIntegracao);
            }
            return faturaIntegracao.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao fechamentoFreteCTeIntegracao, bool incrementarSequencia, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(fechamentoFreteCTeIntegracao.NomeArquivo))
            {
                Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(unitOfWork);
                string extensao = ObterExtencaoPadrao(fechamentoFreteCTeIntegracao.LayoutEDI);
                if (fechamentoFreteCTeIntegracao.LayoutEDI != null && !string.IsNullOrWhiteSpace(fechamentoFreteCTeIntegracao.LayoutEDI.Nomenclatura))
                    fechamentoFreteCTeIntegracao.NomeArquivo = ObterNomenclaturaLayoutEDI(incrementarSequencia ? fechamentoFreteCTeIntegracao.ObterSequencia() : 1, fechamentoFreteCTeIntegracao.LayoutEDI.Nomenclatura, fechamentoFreteCTeIntegracao.FechamentoFrete.Contrato.Transportador, null, fechamentoFreteCTeIntegracao.FechamentoFrete.Numero.ToString(), fechamentoFreteCTeIntegracao.FechamentoFrete.DataFechamento) + extensao;
                else
                    fechamentoFreteCTeIntegracao.NomeArquivo = "EDI-" + fechamentoFreteCTeIntegracao.LayoutEDI.Tipo.ToString("g") + "-" + fechamentoFreteCTeIntegracao.FechamentoFrete.Numero + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(fechamentoFreteCTeIntegracao.CodigosCTes, fechamentoFreteCTeIntegracao.LayoutEDI, fechamentoFreteCTeIntegracao.NomeArquivo, unitOfWork);

                repFechamentoFreteCTeIntegracao.Atualizar(fechamentoFreteCTeIntegracao);
            }
            return fechamentoFreteCTeIntegracao.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI loteContabilizacaoIntegracaoEDI, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(loteContabilizacaoIntegracaoEDI.NomeArquivo))
            {
                Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);

                if (extensao != ".zip")
                    extensao = ObterExtencaoPadrao(loteContabilizacaoIntegracaoEDI.LayoutEDI);

                if (loteContabilizacaoIntegracaoEDI.LayoutEDI != null && !string.IsNullOrWhiteSpace(loteContabilizacaoIntegracaoEDI.LayoutEDI.Nomenclatura))
                    loteContabilizacaoIntegracaoEDI.NomeArquivo = ObterNomenclaturaLayoutEDI(loteContabilizacaoIntegracaoEDI.Codigo, loteContabilizacaoIntegracaoEDI.LayoutEDI.Nomenclatura, loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Empresa, loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Tomador, loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Numero.ToString(), loteContabilizacaoIntegracaoEDI.LoteContabilizacao.DataGeracaoLote ?? DateTime.Now) + extensao;
                else
                    loteContabilizacaoIntegracaoEDI.NomeArquivo = "EDI-" + loteContabilizacaoIntegracaoEDI.LayoutEDI.Tipo.ToString("g") + "-" + loteContabilizacaoIntegracaoEDI.LoteContabilizacao.Numero.ToString() + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(loteContabilizacaoIntegracaoEDI.CodigosCTes, loteContabilizacaoIntegracaoEDI.LayoutEDI, loteContabilizacaoIntegracaoEDI.NomeArquivo, unitOfWork);

                repLoteContabilizacaoIntegracaoEDI.Atualizar(loteContabilizacaoIntegracaoEDI);
            }

            return loteContabilizacaoIntegracaoEDI.NomeArquivo;
        }

        public static string ObterNomeArquivoEDI(Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI loteClienteIntegracaoEDI, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(loteClienteIntegracaoEDI.NomeArquivo))
            {
                Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);

                if (extensao != ".zip")
                    extensao = ObterExtencaoPadrao(loteClienteIntegracaoEDI.LayoutEDI);

                if (loteClienteIntegracaoEDI.LayoutEDI != null && !string.IsNullOrWhiteSpace(loteClienteIntegracaoEDI.LayoutEDI.Nomenclatura))
                    loteClienteIntegracaoEDI.NomeArquivo = ObterNomenclaturaLayoutEDI(loteClienteIntegracaoEDI.Codigo, loteClienteIntegracaoEDI.LayoutEDI.Nomenclatura, loteClienteIntegracaoEDI.Empresa, null, loteClienteIntegracaoEDI.LoteCliente.Numero.ToString(), loteClienteIntegracaoEDI.LoteCliente.DataGeracaoLote) + extensao;
                else
                    loteClienteIntegracaoEDI.NomeArquivo = "EDI-" + loteClienteIntegracaoEDI.LayoutEDI.Tipo.ToString("g") + "-" + loteClienteIntegracaoEDI.LoteCliente.Numero.ToString() + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extensao;

                VincularCTesArquivo(loteClienteIntegracaoEDI.CodigosCTes, loteClienteIntegracaoEDI.LayoutEDI, loteClienteIntegracaoEDI.NomeArquivo, unitOfWork);

                repLoteClienteIntegracaoEDI.Atualizar(loteClienteIntegracaoEDI);
            }

            return loteClienteIntegracaoEDI.NomeArquivo;
        }

        public static System.IO.MemoryStream GerarArquivoEDIFiscal(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.LayoutEDI layout, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork, new Dominio.ObjetosDeValor.MDFe.EDIMDFe(mdfe), layout);

            return svcEDI.GerarArquivoMDFe();
        }

        public static string ObterNomeArquivoCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao, string nomenclatura, DateTime dataHora, string numero, Dominio.Entidades.Empresa transportador, int idRegistro, Dominio.Entidades.Cliente cliente, int protocoloCarga, string codigoOcorrencia, DateTime dataHoraOcorrencia, string serieNFE, string emitente, string numeroNFE)
        {
            if (!string.IsNullOrWhiteSpace(nomenclatura))
            {
                return nomenclatura.Replace("#CNPJTransportadora#", transportador != null ? transportador.CNPJ : string.Empty)
                                   .Replace("#CNPJCliente#", cliente != null ? cliente.CPF_CNPJ_SemFormato : string.Empty)
                                   .Replace("#FinalCNPJCliente#", cliente != null ? (cliente.CPF_CNPJ_SemFormato.Length > 6 ? cliente.CPF_CNPJ_SemFormato.Substring(cliente.CPF_CNPJ_SemFormato.Length - 6) : cliente.CPF_CNPJ_SemFormato) : string.Empty)
                                   .Replace("#Ano#", dataHora.ToString("yyyy"))
                                   .Replace("#AnoAbreviado#", dataHora.ToString("yy"))
                                   .Replace("#Mes#", dataHora.ToString("MM"))
                                   .Replace("#Dia#", dataHora.ToString("dd"))
                                   .Replace("#Hora#", dataHora.ToString("HH"))
                                   .Replace("#Minutos#", dataHora.ToString("mm"))
                                   .Replace("#Segundos#", dataHora.ToString("ss"))
                                   .Replace("#Numero#", numero)
                                   .Replace("#NumeroCarregamento#", numero.PadLeft(7, '0'))
                                   .Replace("#IdRegistro#", idRegistro.ToString().PadLeft(2, '0'))
                                   .Replace("#CodigoEmpresa#", transportador?.CodigoEmpresa ?? string.Empty)
                                   .Replace("#CodigoEstabelecimento#", transportador?.CodigoEstabelecimento ?? string.Empty)
                                   .Replace("#ProtocoloCarga#", Utilidades.String.OnlyNumbers(protocoloCarga.ToString("n0")))
                                   .Replace("#CodigoOcorrencia#", codigoOcorrencia.Length > 2 ? codigoOcorrencia.Substring(codigoOcorrencia.Length - 2) : codigoOcorrencia.PadLeft(2, '0'))
                                   .Replace("#DataOcorrencia#", dataHoraOcorrencia.ToString("ddMMyyyy"))
                                   .Replace("#HoraOcorrencia#", dataHoraOcorrencia.ToString("HH"))
                                   .Replace("#MinutoOcorrencia#", dataHoraOcorrencia.ToString("mm"))
                                   .Replace("#NumeroNFE#", numeroNFE.Length > 8 ? numeroNFE.Substring(numeroNFE.Length - 8) : numeroNFE.PadLeft(8, '0'))
                                   .Replace("#SerieNFE#", serieNFE.Length > 3 ? serieNFE.Substring(serieNFE.Length - 3) : serieNFE.PadLeft(3, '0'))
                                   .Replace("#EmitenteNFE#", emitente.Length > 14 ? emitente.Substring(emitente.Length - 14) : emitente.PadLeft(14, '0'));
            }
            else
                return "";
        }

        private static void VincularCTesArquivo(List<int> listaCodigoCte, Dominio.Entidades.LayoutEDI layoutEDI, string nomeArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ArquivoEDICTe repositorioArquivoEDICTe = new Repositorio.Embarcador.Integracao.ArquivoEDICTe(unitOfWork);

            repositorioArquivoEDICTe.VincularCteArquivoEDI(listaCodigoCte, layoutEDI.Codigo, nomeArquivo);
        }

        public static string ObterNomenclaturaLayoutEDI(int idRegistro, string nomenclatura, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Cliente cliente, string numero, DateTime dataHora, string codigoIntegracao = "", string numeroFatura = "", int protocoloCarga = 0, string tipoOcorrencia = "")
        {
            if (!string.IsNullOrWhiteSpace(nomenclatura))
            {
                return nomenclatura.Replace("#CNPJTransportadora#", transportador != null ? transportador.CNPJ : string.Empty)
                                   .Replace("#CNPJCliente#", cliente != null ? cliente.CPF_CNPJ_SemFormato : string.Empty)
                                   .Replace("#FinalCNPJCliente#", cliente != null ? (cliente.CPF_CNPJ_SemFormato.Length > 6 ? cliente.CPF_CNPJ_SemFormato.Substring(cliente.CPF_CNPJ_SemFormato.Length - 6) : cliente.CPF_CNPJ_SemFormato) : string.Empty)
                                   .Replace("#Numero#", numero)
                                   .Replace("#CodigoIntegracao#", codigoIntegracao)
                                   .Replace("#NumeroFatura#", numeroFatura)
                                   .Replace("#TipoOcorrencia#", tipoOcorrencia)
                                   .Replace("#ProtocoloCarga#", Utilidades.String.OnlyNumbers(protocoloCarga.ToString("n0")))
                                   .Replace("#IdRegistro#", idRegistro.ToString().PadLeft(2, '0'))
                                   .Replace("#IdRegistro5#", idRegistro.ToString().PadLeft(5, '0'))
                                   .Replace("#NumeroCarregamento#", numero.PadLeft(7, '0'))
                                   .Replace("#Ano#", dataHora.ToString("yyyy"))
                                   .Replace("#AnoAbreviado#", dataHora.ToString("yy"))
                                   .Replace("#Mes#", dataHora.ToString("MM"))
                                   .Replace("#Dia#", dataHora.ToString("dd"))
                                   .Replace("#Hora#", dataHora.ToString("HH"))
                                   .Replace("#Minutos#", dataHora.ToString("mm"))
                                   .Replace("#Segundos#", dataHora.ToString("ss"))
                                   .Replace("#CodigoEmpresa#", transportador?.CodigoEmpresa ?? string.Empty)
                                   .Replace("#CodigoEstabelecimento#", transportador?.CodigoEstabelecimento ?? string.Empty);
            }
            else
                return "";
        }

        #endregion
    }
}
