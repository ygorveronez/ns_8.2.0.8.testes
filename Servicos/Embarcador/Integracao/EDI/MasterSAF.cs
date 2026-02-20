using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class MasterSAF
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public MasterSAF(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.ObjetosDeValor.EDI.MasterSAF.DocumentoMasterSAF ObterDocumentoSAF(int numeroItem, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresaLayout, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Empresa empresa)
        {
            return ObterDocumentoSAF(numeroItem, cte, empresaLayout, tipoOperacao, empresa, cancelamento: false);
        }

        private Dominio.ObjetosDeValor.EDI.MasterSAF.DocumentoMasterSAF ObterDocumentoSAF(int numeroItem, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresaLayout, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Empresa empresa, bool cancelamento)
        {
            Repositorio.NFSeItem repositorioNFSeItem = new Repositorio.NFSeItem(_unitOfWork);
            ConfiguracaoContabil.ConfiguracaoContaContabil servicoConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();
            WebService.Pessoas.Pessoa servicoPessoa = new WebService.Pessoas.Pessoa(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = servicoConfiguracaoContaContabil.ObterConfiguracaoContaContabil(cte.Remetente.Cliente, cte.Destinatario.Cliente, cte.TomadorPagador.Cliente, null, empresaLayout, tipoOperacao, null, cte.ModeloDocumentoFiscal, null, _unitOfWork);
            Dominio.Entidades.NFSeItem nFSeItem = repositorioNFSeItem.BuscarPorCTe(cte.Codigo).FirstOrDefault();
            Dominio.ObjetosDeValor.EDI.MasterSAF.DocumentoMasterSAF documentoMasterSAF = new Dominio.ObjetosDeValor.EDI.MasterSAF.DocumentoMasterSAF();

            if (cancelamento)
            {
                documentoMasterSAF.aliquotaCOFINS = cte.Empresa.EmpresaPai?.Configuracao?.AliquotaCOFINS ?? 0m;
                documentoMasterSAF.aliquotaPIS = cte.Empresa.EmpresaPai?.Configuracao?.AliquotaPIS ?? 0m;
                documentoMasterSAF.CodigoMotivoCancelamentoNF = "";
                documentoMasterSAF.MotivoCancelamentoNF = cte.ObservacaoCancelamento;
            }
            else
            {
                documentoMasterSAF.aliquotaCOFINS = cte.Empresa.Configuracao?.AliquotaCOFINS ?? 0m;
                documentoMasterSAF.aliquotaPIS = cte.Empresa.Configuracao?.AliquotaPIS ?? 0m;
                documentoMasterSAF.CodigoEstabelecimento = cte.Empresa.CodigoEstabelecimento;
                documentoMasterSAF.CodigoFilial = cte.Empresa.CodigoEmpresa;
                documentoMasterSAF.CodigoMunicipioISSSimplificado = cte.Empresa.Localidade.CodigoIBGESemUf.ToString();
                documentoMasterSAF.DataSaidaRecebimento = cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.Now;
                documentoMasterSAF.TipoFreteNumerico = cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? "1" : "2";

                if (documentoMasterSAF.aliquotaPIS <= 0m)
                    documentoMasterSAF.aliquotaPIS = cte.Empresa.EmpresaPai?.Configuracao?.AliquotaPIS ?? 0m;

                if (documentoMasterSAF.aliquotaCOFINS <= 0m)
                    documentoMasterSAF.aliquotaCOFINS = cte.Empresa.EmpresaPai?.Configuracao?.AliquotaCOFINS ?? 0m;
            }

            documentoMasterSAF.ValorPIS = cte.ValorPrestacaoServico > 0 ? cte.ValorPrestacaoServico * (documentoMasterSAF.aliquotaPIS / 100) : 0;
            documentoMasterSAF.ValorCOFINS = cte.ValorPrestacaoServico > 0 ? cte.ValorPrestacaoServico * (documentoMasterSAF.aliquotaCOFINS / 100) : 0;
            documentoMasterSAF.CodigoSituacaoPIS = documentoMasterSAF.ValorPIS > 0m ? "50" : "73";
            documentoMasterSAF.CodigoSituacaoCOFINS = documentoMasterSAF.ValorCOFINS > 0m ? "50" : "73";
            documentoMasterSAF.Tomador = servicoPessoa.ConverterObjetoPessoa(cte.TomadorPagador.Cliente);

            if (cte.ISSRetido)
            {
                documentoMasterSAF.AliquotaISSRetido = cte.AliquotaISS;
                documentoMasterSAF.ValorBaseISSRetido = cte.BaseCalculoISS;
                documentoMasterSAF.ValorISSRetido = cte.ValorISSRetido;
            }

            documentoMasterSAF.IndicadorMunicipioISS = documentoMasterSAF.ValorISSRetido > 0m ? "2" : "1";
            documentoMasterSAF.IndicadorResponsavelRecolhimentoISS = documentoMasterSAF.ValorISSRetido > 0m ? "2" : "1";
            documentoMasterSAF.IndicadorTipoRetensao = documentoMasterSAF.ValorISSRetido > 0m ? "1" : "2";

            if ((configuracaoContaContabil != null) && (configuracaoContaContabil.ConfiguracaoContaContabilEscrituracoes.Count > 0))
                documentoMasterSAF.ContaContabil = (
                    from o in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes
                    where o.TipoContaContabil == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.FreteLiquido
                    select o.PlanoContabilidade
                ).FirstOrDefault();

            documentoMasterSAF.AliquotaISS = cte.AliquotaISS;
            documentoMasterSAF.BaseCOFINS = cte.ValorPrestacaoServico;
            documentoMasterSAF.BaseISSTributada = cte.BaseCalculoISS;
            documentoMasterSAF.BasePIS = cte.ValorPrestacaoServico;
            documentoMasterSAF.CFOP = cte.CFOP.CodigoCFOP.ToString();
            documentoMasterSAF.CFOPEntrada = cte.CFOP.CodigoCFOPCompra.ToString();
            documentoMasterSAF.ChaveNFs = cte.Chave;
            documentoMasterSAF.ClassificacaoFiscal = "2";
            documentoMasterSAF.CodigoDestinatarioEmitenteRemetente = documentoMasterSAF.Tomador.CPFCNPJSemFormato;
            documentoMasterSAF.CodigoEmpresa = cte.TomadorPagador.Cliente.CodigoCompanhia; //#ajuste: parece correto
            documentoMasterSAF.CodigoModeloNF = "NF";
            documentoMasterSAF.CodigoMunicipioISS = cte.Empresa.Localidade.CodigoIBGE.ToString();
            documentoMasterSAF.Contribuinte = cte.Destinatario.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS ? "S" : "N";
            documentoMasterSAF.DataCancelamento = cte.DataCancelamento.HasValue ? cte.DataCancelamento.Value : DateTime.Now;
            documentoMasterSAF.DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.Now;
            documentoMasterSAF.DataEmissaoRPS = cte.RPS?.Data ?? DateTime.Now;
            documentoMasterSAF.Destinatario = servicoPessoa.ConverterObjetoPessoa(cte.Destinatario.Cliente);
            documentoMasterSAF.DiscriminacaoDosServicosNFS = nFSeItem?.Discriminacao ?? "";
            documentoMasterSAF.Expedidor = cte.Expedidor != null ? servicoPessoa.ConverterObjetoPessoa(cte.Expedidor.Cliente) : null;
            documentoMasterSAF.IndicadorFiscaJuridica = "5";
            documentoMasterSAF.InscricaoEstadual = cte.Empresa.InscricaoEstadual;
            documentoMasterSAF.ModeloDocumento = cte.ModeloDocumentoFiscal.Numero;
            documentoMasterSAF.NaturezaOperacao = ""; //cte.NaturezaDaOperacao.CodigoIntegracao;
            documentoMasterSAF.NormalDevolucao = "1";
            documentoMasterSAF.NotaFiscalCreditoDebito = "0";
            documentoMasterSAF.NumeroDocumento = cte.Numero.ToString();
            documentoMasterSAF.NumeroItem = numeroItem;
            documentoMasterSAF.NumeroRPS = cte.RPS?.Numero.ToString() ?? "";
            documentoMasterSAF.QuantidadeServico = 1;
            documentoMasterSAF.Recebedor = cte.Recebedor != null ? servicoPessoa.ConverterObjetoPessoa(cte.Recebedor.Cliente) : null;
            documentoMasterSAF.Remetente = servicoPessoa.ConverterObjetoPessoa(cte.Remetente.Cliente);
            documentoMasterSAF.SerieDocumento = cte.Serie.Numero.ToString();
            documentoMasterSAF.SerieRPS = cte.RPS?.Serie ?? "";
            documentoMasterSAF.SituacaoNota = cte.Status == "A" ? "N" : "S";
            documentoMasterSAF.TipoDocumento = "NFZS";
            documentoMasterSAF.TipoFaturamento = "1";
            documentoMasterSAF.TipoFrete = cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? "C" : "F";
            documentoMasterSAF.TipoRPS = "RPS";
            documentoMasterSAF.UFDestino = cte.LocalidadeTerminoPrestacao.Estado.Sigla;
            documentoMasterSAF.UFOrigem = cte.LocalidadeInicioPrestacao.Estado.Sigla;
            documentoMasterSAF.ValorDoFrete = cte.ValorFrete;
            documentoMasterSAF.ValorISS = cte.ValorISS;
            documentoMasterSAF.ValorProdutosServico = cte.ValorAReceber;
            documentoMasterSAF.ValorServicoTransporte = cte.ValorFrete;
            documentoMasterSAF.ValorTotalDocumentoFiscal = cte.ValorAReceber;
            documentoMasterSAF.ValorTotalServicos = cte.ValorPrestacaoServico;

            return documentoMasterSAF;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF ConverterLoteEscrituracaoParaMasterSAF(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao loteEscrituracaoEDIIntegracao, Dominio.Entidades.Empresa empresaEDI)
        {
            loteEscrituracaoEDIIntegracao.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao = loteEscrituracaoEDIIntegracao.LoteEscrituracao;
            Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = new Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF();

            for (var i = 0; i < loteEscrituracao.DocumentosEscrituracao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao documentoEscrituracao = loteEscrituracao.DocumentosEscrituracao[i];

                masterSAF.Documentos.Add(ObterDocumentoSAF(i + 1, documentoEscrituracao.CTe, empresaEDI, documentoEscrituracao.Carga?.TipoOperacao, documentoEscrituracao.CTe.Empresa));

                loteEscrituracaoEDIIntegracao.CodigosCTes.Add(documentoEscrituracao.CTe.Codigo);
            }

            return masterSAF;
        }

        public Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF ConverterLoteEscrituracaoCancelamentoParaMasterSAF(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao, Dominio.Entidades.Empresa empresaEDI)
        {
            loteEscrituracaoCancelamentoEDIIntegracao.CodigosCTes = new List<int>();
            Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento = loteEscrituracaoCancelamentoEDIIntegracao.LoteEscrituracaoCancelamento;
            Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = new Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF();
            int countDocumentos = loteEscrituracaoCancelamento.DocumentosEscrituracao.Count();

            for (int i = 0; i < countDocumentos; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento documentoEscrituracaoCancelamento = loteEscrituracaoCancelamento.DocumentosEscrituracao[i];
                
                masterSAF.Documentos.Add(ObterDocumentoSAF(i + 1, documentoEscrituracaoCancelamento.CTe, empresaEDI, documentoEscrituracaoCancelamento.Carga?.TipoOperacao, documentoEscrituracaoCancelamento.CTe.Empresa));

                loteEscrituracaoCancelamentoEDIIntegracao.CodigosCTes.Add(documentoEscrituracaoCancelamento.CTe.Codigo);
            }

            return masterSAF;
        }

        public Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF ConverterCargaCancelamentoParaMasterSAF(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = new Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;

            if (cargaCancelamentoIntegracaoEDI.LayoutEDI.ModeloDocumentoFiscais != null && cargaCancelamentoIntegracaoEDI.LayoutEDI.ModeloDocumentoFiscais.Count > 0)
                cargaCTes = repCargaCTe.BuscarPorCargaEModelosDocumentos(cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.Codigo, cargaCancelamentoIntegracaoEDI.LayoutEDI.ModeloDocumentoFiscais.Select(o => o.Codigo).ToList());
            else
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.Codigo);

            for (int i = 0; i < cargaCTes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes[i];

                masterSAF.Documentos.Add(ObterDocumentoSAF(i + 1, cargaCTe.CTe, cargaCancelamentoIntegracaoEDI.Empresa, cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.TipoOperacao, cargaCancelamentoIntegracaoEDI.CargaCancelamento.Carga.Empresa));
            }

            return masterSAF;
        }

        public Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF ConverterCargaParaMasterSAF(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = new Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;

            if (cargaEDIIntegracao.CTe != null)
                cargaCTes = repCargaCTe.BuscarTodosPorCTe(cargaEDIIntegracao.CTe.Codigo);
            else if (cargaEDIIntegracao.LayoutEDI.ModeloDocumentoFiscais != null && cargaEDIIntegracao.LayoutEDI.ModeloDocumentoFiscais.Count > 0)
                cargaCTes = repCargaCTe.BuscarPorCargaEModelosDocumentos(cargaEDIIntegracao.Carga.Codigo, cargaEDIIntegracao.LayoutEDI.ModeloDocumentoFiscais.Select(o => o.Codigo).ToList());
            else
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo);

            for (int i = 0; i < cargaCTes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes[i];

                masterSAF.Documentos.Add(ObterDocumentoSAF(i + 1, cargaCTe.CTe, cargaEDIIntegracao.Empresa, cargaEDIIntegracao.Carga.TipoOperacao, cargaEDIIntegracao.Carga.Empresa));
            }

            return masterSAF;
        }

        public Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF ConverterOcorrenciaParaMasterSAF(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao ocorrenciaEDIIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = ocorrenciaEDIIntegracao.CargaOcorrencia;
            Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = new Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(cargaOcorrencia.Codigo, cargaOcorrencia.EmiteComplementoFilialEmissora);

            for (var i = 0; i < cargaCTesComplementoInfo.Count; i++)
            {
                if (ocorrenciaEDIIntegracao.LayoutEDI.ModeloDocumentoFiscais == null || ocorrenciaEDIIntegracao.LayoutEDI.ModeloDocumentoFiscais.Count <= 0 || ocorrenciaEDIIntegracao.LayoutEDI.ModeloDocumentoFiscais.Contains(cargaCTesComplementoInfo[i].CTe.ModeloDocumentoFiscal))
                    masterSAF.Documentos.Add(ObterDocumentoSAF(i + 1, cargaCTesComplementoInfo[i].CTe, ocorrenciaEDIIntegracao.Empresa, cargaOcorrencia.Carga.TipoOperacao, cargaCTesComplementoInfo[i].CTe.Empresa));
            }

            return masterSAF;
        }

        public Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF ConverterNFSManualParaMasterSAF(Dominio.Entidades.Embarcador.NFS.NFSManualEDIIntegracao nfsManualEDIIntegracao)
        {
            Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = new Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF();

            masterSAF.Documentos.Add(ObterDocumentoSAF(1, nfsManualEDIIntegracao.LancamentoNFSManual.CTe, nfsManualEDIIntegracao.LancamentoNFSManual.CTe.Empresa, null, nfsManualEDIIntegracao.LancamentoNFSManual.CTe.Empresa));

            return masterSAF;
        }

        public Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF ConverterNFSManualCancelamentoParaMasterSAF(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI nfsManualCancelamentoIntegracaoEDI)
        {
            Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF masterSAF = new Dominio.ObjetosDeValor.EDI.MasterSAF.MasterSAF();

            masterSAF.Documentos.Add(ObterDocumentoSAF(1, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.CTe, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.CTe.Empresa, null, nfsManualCancelamentoIntegracaoEDI.NFSManualCancelamento.LancamentoNFSManual.CTe.Empresa, cancelamento: true));

            return masterSAF;
        }

        #endregion Métodos Públicos
    }
}
