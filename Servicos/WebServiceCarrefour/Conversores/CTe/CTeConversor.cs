using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.WebServiceCarrefour.Conversores.CTe
{
    public sealed class CTe
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CTe(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void PreencherDadosTitulo(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe cte)
        {
            Repositorio.Embarcador.Financeiro.Titulo repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = cargaCTe.CTe.Titulo;

            if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                cte.NumeroBoleto = titulo.NossoNumero;
            else if (cargaCTe.CTe.Fatura != null && cargaCTe.CTe.Fatura.Parcelas != null && cargaCTe.CTe.Fatura.Parcelas.Count > 0 && cargaCTe.CTe.Fatura.Parcelas.FirstOrDefault().CodigoTitulo > 0)
            {
                titulo = repositorioTitulo.BuscarPorCodigo(cargaCTe.CTe.Fatura.Parcelas.FirstOrDefault().CodigoTitulo);

                if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    cte.NumeroBoleto = titulo.NossoNumero;
            }

            if (string.IsNullOrWhiteSpace(cte.NumeroBoleto))
            {
                titulo = repositorioTitulo.BuscarTituloDocumentoPorCTe(cargaCTe.CTe.Codigo);
                if (titulo == null)
                    titulo = repositorioTitulo.BuscarPorCTe(cargaCTe.CTe.Codigo);

                if (titulo != null && !string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    cte.NumeroBoleto = titulo.NossoNumero;
            }

            cte.PDFBoleto = this.ObterBoletoPDF(titulo, codificarUTF8: true);
            cte.NumeroTitulo = titulo?.Codigo ?? 0;
            cte.PrazoPgtoSacado = titulo != null && titulo.DataVencimento.HasValue ? titulo.DataVencimento.Value.ToString("dd/MM/yyyy") : "";
            cte.PrazoPgtoCliente = titulo != null && titulo.DataVencimento.HasValue ? titulo.DataVencimento.Value.ToString("dd/MM/yyyy") : "";
        }

        private string ObterBoletoPDF(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, bool codificarUTF8)
        {
            try
            {
                if ((titulo == null) || !Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                    return string.Empty;

                byte[] dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(titulo.CaminhoBoleto);

                if (dacte == null)
                    return string.Empty;

                string stringDacte = codificarUTF8 ? Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, dacte)) : Convert.ToBase64String(dacte);

                if (string.IsNullOrWhiteSpace(stringDacte))
                    return string.Empty;

                return stringDacte;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                return string.Empty;
            }
        }

        private List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnterior> ObterDocumentosAnteriores(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            if ((cargaCTe.CTe.DocumentosTransporteAnterior == null) || (cargaCTe.CTe.DocumentosTransporteAnterior.Count == 0))
                return new List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnterior>();

            List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnterior> documentosAnterior = new List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnterior>();
            Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);

            foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento in cargaCTe.CTe.DocumentosTransporteAnterior)
            {
                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repositorioCTeTerceiro.BuscarPorChave(documento.Chave);

                if (cteTerceiro != null)
                {
                    Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnterior cteAnterior = new Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnterior()
                    {
                        ChaveCTe = documento.Chave,
                        CNPJCPF = cteTerceiro.TransportadorTerceiro?.CPF_CNPJ_SemFormato ?? "",
                        DataHoraEmissao = cteTerceiro.DataEmissao > DateTime.MinValue ? cteTerceiro.DataEmissao.ToString("dd/MM/yyyy HH:mm") : "",
                        IERG = cteTerceiro.TransportadorTerceiro?.IE_RG ?? "",
                        Numero = cteTerceiro.Numero,
                        PesoTotal = cteTerceiro.Peso,
                        Serie = cteTerceiro.Serie,
                        TipoDocumento = "55",
                        ValorMercadoria = cteTerceiro.ValorTotalMercadoria,
                        ValorTotal = cteTerceiro.ValorAReceber,
                        Notas = new List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnteriorNota>()
                    };

                    if (cteTerceiro.CTesTerceiroNFes?.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe nota in cteTerceiro.CTesTerceiroNFes)
                        {
                            if (nota != null)
                            {
                                Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnteriorNota nfe = new Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTeAnteriorNota()
                                {
                                    ChaveNFe = nota.Chave,
                                    Numero = nota.Numero,
                                    ReferenciaEDI = nota.NumeroReferenciaEDI,
                                    Serie = nota.Serie,
                                    PINSuframa = nota.PINSuframa
                                };

                                cteAnterior.Notas.Add(nfe);
                            }
                        }
                    }

                    documentosAnterior.Add(cteAnterior);
                }
            }

            return documentosAnterior;
        }

        private List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.DocumentosCTe> ObterDocumentos(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.DocumentosCTe> documentos = new List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.DocumentosCTe>();
            Repositorio.DocumentosCTE repositorioDocumentosCTe = new Repositorio.DocumentosCTE(_unitOfWork);
            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repositorioDocumentosCTe.BuscarPorCTe(cargaCTe.CTe.Codigo);

            foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
            {
                Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.DocumentosCTe nota = new Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.DocumentosCTe();

                if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    nota.ChaveNFe = documentoCTe.ChaveNFE;
                else
                {
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Numero)))
                        nota.Numero = documentoCTe.Numero;

                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Serie)))
                        nota.Serie = documentoCTe.Serie;
                }

                if (!string.IsNullOrWhiteSpace(nota.ChaveNFe) || !string.IsNullOrWhiteSpace(nota.Numero))
                    documentos.Add(nota);
            }

            return documentos;
        }

        private List<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista> ObterMotoristas(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Carga.MotoristaConverter servicoConverterMotorista = new Carga.MotoristaConverter(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista> motoristas = new List<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista>();

            foreach (Dominio.Entidades.Usuario motorista in cargaCTe.Carga.Motoristas)
                motoristas.Add(servicoConverterMotorista.Converter(motorista));

            return motoristas;
        }

        private List<int> ObterProtocolosPedidos(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoCTe = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTe.Codigo);
            List<int> protocolosPedidos = new List<int>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoCTe)
                protocolosPedidos.Add(cargaPedido.Pedido.Codigo);

            return protocolosPedidos;
        }

        private string ObterRetornoPDF(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.Status.Equals("A") || cte.Status.Equals("F"))
            {
                Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

                return servicoCTe.ObterDACTE(cte.Codigo, cte.Empresa.Codigo, _unitOfWork);
            }

            return string.Empty;
        }

        private Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.FreteValor ObterValorFrete(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.FreteValor valorFrete = new Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.FreteValor();

            valorFrete.ComponentesAdicionais = ObterValorFreteComponentesAdicionais(cargaCTe);
            valorFrete.FreteProprio = cargaCTe.CTe.ValorFrete;
            valorFrete.ValorTotalAReceber = cargaCTe.CTe.ValorAReceber;
            valorFrete.ValorPrestacaoServico = cargaCTe.CTe.ValorPrestacaoServico;
            valorFrete.ICMS = ObterValorFreteICMS(cargaCTe);
            valorFrete.ISS = ObterValorFreteISS(cargaCTe);

            return valorFrete;
        }

        private List<Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.ComponenteAdicional> ObterValorFreteComponentesAdicionais(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            List<Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.ComponenteAdicional> componentesAdicionais = new List<Dominio.ObjetosDeValor.WebServiceCarrefour.Frete.ComponenteAdicional>();
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repositorioCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> cargaCTeComponentesFrete = repositorioCargaCTeComponentesFrete.BuscarPorSemComposicaoFreteLiquidoCargaCTe(cargaCTe.Codigo);
            Frete.ComponenteFreteConversor servicoConverterComponenteFrete = new Frete.ComponenteFreteConversor(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componente in cargaCTeComponentesFrete)
                componentesAdicionais.Add(servicoConverterComponenteFrete.Converter(componente));

            return componentesAdicionais;
        }

        private Dominio.ObjetosDeValor.WebServiceCarrefour.ICMS.ICMS ObterValorFreteICMS(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Dominio.ObjetosDeValor.WebServiceCarrefour.ICMS.ICMS ICMS = new Dominio.ObjetosDeValor.WebServiceCarrefour.ICMS.ICMS();

            ICMS.Aliquota = cargaCTe.CTe.AliquotaICMS;
            ICMS.CST = cargaCTe.CTe.CST;
            ICMS.IncluirICMSBC = cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
            ICMS.ObservacaoCTe = cargaCTe.CTe.ObservacoesGerais;
            ICMS.PercentualInclusaoBC = cargaCTe.CTe.PercentualICMSIncluirNoFrete;
            ICMS.PercentualReducaoBC = cargaCTe.CTe.PercentualReducaoBaseCalculoICMS;
            ICMS.SimplesNacional = cargaCTe.CTe.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim;
            ICMS.ValorBaseCalculoICMS = cargaCTe.CTe.BaseCalculoICMS;
            ICMS.ValorICMS = cargaCTe.CTe.ValorICMS;

            return ICMS;
        }

        private Dominio.ObjetosDeValor.WebServiceCarrefour.ISS.ISS ObterValorFreteISS(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.ISS.ISS ISS = new Dominio.ObjetosDeValor.WebServiceCarrefour.ISS.ISS();

            ISS.Aliquota = cargaCTe.CTe.AliquotaISS;
            ISS.IncluirISSBaseCalculo = cargaCTe.CTe.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
            ISS.PercentualRetencao = cargaCTe.CTe.PercentualISSRetido;
            ISS.ValorBaseCalculoISS = cargaCTe.CTe.BaseCalculoISS;
            ISS.ValorISS = cargaCTe.CTe.ValorISS;
            ISS.ValorRetencaoISS = cargaCTe.CTe.ValorISSRetido;

            return ISS;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe Converter(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, TipoDocumentoRetorno tipoDocumentoRetorno)
        {
            if (cargaCTe == null)
                return null;

            Frota.VeiculoConversor servicoConverterVeiculo = new Frota.VeiculoConversor(_unitOfWork);
            Localidade.LocalidadeConversor servicoConverterLocalidade = new Localidade.LocalidadeConversor(_unitOfWork);
            Pessoa.EmpresaConverter servicoConverterEmpresa = new Pessoa.EmpresaConverter(_unitOfWork);
            Pessoa.ParticipanteCteConversor servicoConverterParticipanteCte = new Pessoa.ParticipanteCteConversor(_unitOfWork);
            Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe cte = new Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe();

            cte.CodigoCargaCTe = cargaCTe.Codigo;
            cte.Chave = cargaCTe.CTe.Chave;
            cte.ProtocoloAutorizacao = cargaCTe.CTe.Protocolo;
            cte.CFOP = cargaCTe.CTe.CFOP.CodigoCFOP;
            cte.DataEmissao = cargaCTe.CTe.DataEmissao.HasValue ? cargaCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.Lotacao = cargaCTe.CTe.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim;
            cte.Modelo = cargaCTe.CTe.ModeloDocumentoFiscal.Numero;
            cte.Numero = cargaCTe.CTe.Numero;
            cte.NumeroControle = cargaCTe.CTe.NumeroControle;
            cte.Protocolo = cargaCTe.CTe.Codigo;
            cte.Serie = cargaCTe.CTe.Serie.Numero;
            cte.SituacaoCTeSefaz = cargaCTe.CTe.SituacaoCTeSefaz;
            cte.TipoCTE = cargaCTe.CTe.TipoCTE;
            cte.TipoServico = cargaCTe.CTe.TipoServico;
            cte.TipoTomador = cargaCTe.CTe.TipoTomador;
            cte.TipoDocumentoFiscal = cargaCTe.CTe.ModeloDocumentoFiscal?.Abreviacao ?? "";
            cte.MunicipioColeta = cargaCTe.CTe.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? "";
            cte.MunicipioRemetente = cargaCTe.CTe.Remetente?.Localidade?.DescricaoCidadeEstado ?? "";
            cte.MunicipioEntrega = cargaCTe.CTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "";
            cte.TipoIcms = cargaCTe.CTe.CST;
            cte.AliquotaCofins = cargaCTe.CTe.AliquotaCOFINS;
            cte.FlagIsentoIcms = cargaCTe.CTe.CST == "40";
            cte.AliquotaPis = cargaCTe.CTe.AliquotaPIS;
            cte.ValorReducao = cargaCTe.CTe.PercentualReducaoBaseCalculoICMS;
            cte.ValorIcmsReduzido = cargaCTe.CTe.PercentualReducaoBaseCalculoICMS;
            cte.ValorBaseIcmsRemetente = 0;
            cte.ValorIcmsRemetente = 0;
            cte.ValorBaseIcmsDestinatario = cargaCTe.CTe.ValorICMSUFDestino > 0 ? cargaCTe.CTe.BaseCalculoICMS : 0;
            cte.ValorIcmsDestinatario = cargaCTe.CTe.ValorICMSUFDestino;
            cte.ValorBaseIcmsPobreza = cargaCTe.CTe.ValorICMSFCPFim > 0 ? cargaCTe.CTe.BaseCalculoICMS : 0;
            cte.FlagDebitoPisCofins = false;
            cte.FlagDebitoIcms = cargaCTe.CTe.CST == "60";
            cte.FlagIsentoPisCofins = cargaCTe.CTe.AliquotaPIS > 0 ? false : true;
            cte.ValorIss = cargaCTe.CTe.ValorINSS;
            cte.InticativoRetencaoIss = cargaCTe.CTe.ISSRetido;
            cte.DataEmbarque = cargaCTe.CTe.DataInicioPrestacaoServico.HasValue ? cargaCTe.CTe.DataInicioPrestacaoServico.Value.ToString("dd/MM/yyyy HH:mm") : "";
            cte.FlagIsendoContribuicoes = cargaCTe.CTe.CST == "41";
            cte.PercentualIcmsRemetente = cargaCTe.CTe.PercentualICMSPartilha;
            cte.AliquotaIcmsRemetente = cargaCTe.CTe.AliquotaICMSInterna;
            cte.PercentualIcmsDestinatario = 0;
            cte.AliquotaIcmsDestinatario = 0;
            cte.AliquotaIcmsPobreza = cargaCTe.CTe.ValorICMSFCPFim > 0 ? Math.Round(((cargaCTe.CTe.ValorICMSFCPFim * 100) / cargaCTe.CTe.BaseCalculoICMS), 2) : 0;
            cte.ValorIcmsPobreza = cargaCTe.CTe.ValorICMSFCPFim;
            cte.SFCSacado = cargaCTe.CTe.TomadorPagador?.CPF_CNPJ_SemFormato ?? "";
            cte.ValorBaseIcms = cargaCTe.CTe.BaseCalculoICMS;
            cte.AliquotaISS = cargaCTe.CTe.AliquotaICMS;
            cte.OcorreuSinistroAvaria = cargaCTe.CTe.OcorreuSinistroAvaria;
            cte.ValorTotalMercadoria = cargaCTe.CTe.ValorTotalMercadoria;
            cte.VersaoCTE = cargaCTe.CTe.Versao;
            cte.MotivoCancelamento = cargaCTe.CTe.ObservacaoCancelamento;
            cte.ProtocolosDePedidos = ObterProtocolosPedidos(cargaCTe);
            cte.PDF = tipoDocumentoRetorno == TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == TipoDocumentoRetorno.Todos ? this.ObterRetornoPDF(cargaCTe.CTe) : "";
            cte.XML = tipoDocumentoRetorno == TipoDocumentoRetorno.XML || tipoDocumentoRetorno == TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(cargaCTe.CTe, cargaCTe.CTe.Status) : "";
            cte.XMLAutorizacao = tipoDocumentoRetorno == TipoDocumentoRetorno.XML || tipoDocumentoRetorno == TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(cargaCTe.CTe, "A") : "";
            cte.XMLCancelamento = tipoDocumentoRetorno == TipoDocumentoRetorno.XML || tipoDocumentoRetorno == TipoDocumentoRetorno.Todos ? this.ObterRetornoXMLPorStatus(cargaCTe.CTe, "C") : "";
            cte.TransportadoraEmitente = servicoConverterEmpresa.Converter(cargaCTe.CTe.Empresa);
            cte.Expedidor = servicoConverterParticipanteCte.Converter(cargaCTe.CTe.Expedidor);
            cte.Destinatario = servicoConverterParticipanteCte.Converter(cargaCTe.CTe.Destinatario);
            cte.Recebedor = servicoConverterParticipanteCte.Converter(cargaCTe.CTe.Recebedor);
            cte.Remetente = servicoConverterParticipanteCte.Converter(cargaCTe.CTe.Remetente);
            cte.Tomador = servicoConverterParticipanteCte.Converter(cargaCTe.CTe.Tomador);
            cte.LocalidadeFimPrestacao = servicoConverterLocalidade.Converter(cargaCTe.CTe.LocalidadeTerminoPrestacao);
            cte.LocalidadeInicioPrestacao = servicoConverterLocalidade.Converter(cargaCTe.CTe.LocalidadeInicioPrestacao);
            cte.DocumentosAnterior = ObterDocumentosAnteriores(cargaCTe);
            cte.ValorFrete = ObterValorFrete(cargaCTe);
            cte.Motoristas = ObterMotoristas(cargaCTe);
            cte.Documentos = ObterDocumentos(cargaCTe);
            cte.Veiculo = servicoConverterVeiculo.Converter(cargaCTe.Carga.Veiculo, cargaCTe.Carga.VeiculosVinculados.ToList());

            PreencherDadosTitulo(cargaCTe, cte);

            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                cte.NumeroNFSePrefeitura = !string.IsNullOrWhiteSpace(cargaCTe.CTe.NumeroPrefeituraNFSe) ? cargaCTe.CTe.NumeroPrefeituraNFSe : cargaCTe.CTe.Numero.ToString();

            return cte;
        }

        public string ObterRetornoXMLPorStatus(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string status)
        {
            Dominio.Enumeradores.TipoXMLCTe tipo = Dominio.Enumeradores.TipoXMLCTe.Autorizacao;
            if (status.Equals("C") || status.Equals("I"))
                tipo = Dominio.Enumeradores.TipoXMLCTe.Cancelamento;

            Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);
            byte[] data = servicoCTe.ObterXMLCancelamentoAutorizacao(cte, tipo, _unitOfWork);

            if (data == null)
                return string.Empty;

            return Encoding.Default.GetString(data);
        }

        #endregion
    }
}
