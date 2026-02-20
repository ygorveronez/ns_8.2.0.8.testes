//////using System;
//////using System.Collections.Generic;
//////using System.Linq;
//////using System.ServiceModel;
//////using System.Web;
//////using System.Xml.Serialization;

//////namespace Dominio.Ferroviario.AcompanhamentoTrem
//////{
//////    // using System.Xml.Serialization;
//////    // XmlSerializer serializer = new XmlSerializer(typeof(Root));
//////    // using (StringReader reader = new StringReader(xml))
//////    // {
//////    //    var test = (Root)serializer.Deserialize(reader);
//////    // }

//////    [XmlRoot(ElementName = "PrefixoTrem")]
//////    public class PrefixoTrem
//////    {

//////        [XmlElement(ElementName = "codigoPrefixoTrem")]
//////        public string CodigoPrefixoTrem { get; set; }

//////        [XmlElement(ElementName = "dataHoraInicioValidadePrefixo")]
//////        public string DataHoraInicioValidadePrefixo { get; set; }
//////    }

//////    [XmlRoot(ElementName = "FamiliaTrem")]
//////    public class FamiliaTrem
//////    {

//////        [XmlElement(ElementName = "codigoFamilia")]
//////        public string CodigoFamilia { get; set; }

//////        [XmlElement(ElementName = "sentido")]
//////        public string Sentido { get; set; }
//////    }

//////    [XmlRoot(ElementName = "PatioItinerarioAtual")]
//////    public class PatioItinerarioAtual
//////    {

//////        [XmlElement(ElementName = "siglaPatio")]
//////        public string SiglaPatio { get; set; }

//////        [XmlElement(ElementName = "eventoCirculacao")]
//////        public string EventoCirculacao { get; set; }

//////        [XmlElement(ElementName = "dataHoraCirculacao")]
//////        public string DataHoraCirculacao { get; set; }

//////        [XmlAttribute(AttributeName = "sequencia")]
//////        public string Sequencia { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "PatioItinerario")]
//////    public class PatioItinerario
//////    {

//////        [XmlElement(ElementName = "siglaPatio")]
//////        public string SiglaPatio { get; set; }

//////        [XmlAttribute(AttributeName = "sequencia")]
//////        public string Sequencia { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "patiosTranspostos")]
//////    public class PatiosTranspostos
//////    {

//////        [XmlElement(ElementName = "PatioItinerario")]
//////        public PatioItinerario PatioItinerario { get; set; }
//////    }

//////    [XmlRoot(ElementName = "DadosCadastrais")]
//////    public class DadosCadastrais
//////    {

//////        [XmlElement(ElementName = "tipoVagao")]
//////        public string TipoVagao { get; set; }

//////        [XmlElement(ElementName = "identificador")]
//////        public string Identificador { get; set; }

//////        [XmlElement(ElementName = "tara")]
//////        public string Tara { get; set; }

//////        [XmlElement(ElementName = "capacidadeNominal")]
//////        public string CapacidadeNominal { get; set; }

//////        [XmlElement(ElementName = "CNPJProprietario")]
//////        public string CNPJProprietario { get; set; }

//////        [XmlElement(ElementName = "etiquetaRFID")]
//////        public string EtiquetaRFID { get; set; }

//////        [XmlElement(ElementName = "dataUltimaAtualizacao")]
//////        public string DataUltimaAtualizacao { get; set; }

//////        [XmlElement(ElementName = "operadora")]
//////        public string Operadora { get; set; }

//////        [XmlAttribute(AttributeName = "IdVagao")]
//////        public string IdVagao { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }

//////        [XmlElement(ElementName = "pesoBruto")]
//////        public string PesoBruto { get; set; }

//////        [XmlAttribute(AttributeName = "IdLocomotiva")]
//////        public string IdLocomotiva { get; set; }
//////    }

//////    [XmlRoot(ElementName = "refDocFiscal")]
//////    public class RefDocFiscal
//////    {

//////        [XmlAttribute(AttributeName = "idDocFiscal")]
//////        public string IdDocFiscal { get; set; }
//////    }

//////    [XmlRoot(ElementName = "docsFiscaisVagao")]
//////    public class DocsFiscaisVagao
//////    {

//////        [XmlElement(ElementName = "refDocFiscal")]
//////        public RefDocFiscal RefDocFiscal { get; set; }
//////    }

//////    [XmlRoot(ElementName = "refDocTransporte")]
//////    public class RefDocTransporte
//////    {

//////        [XmlAttribute(AttributeName = "idDocTransporte")]
//////        public string IdDocTransporte { get; set; }
//////    }

//////    [XmlRoot(ElementName = "docsTransporte")]
//////    public class DocsTransporte
//////    {

//////        [XmlElement(ElementName = "refDocTransporte")]
//////        public RefDocTransporte RefDocTransporte { get; set; }
//////    }

//////    [XmlRoot(ElementName = "refItemCarga")]
//////    public class RefItemCarga
//////    {

//////        [XmlAttribute(AttributeName = "idItemCarga")]
//////        public string IdItemCarga { get; set; }
//////    }

//////    [XmlRoot(ElementName = "itensCarga")]
//////    public class ItensCarga
//////    {

//////        [XmlElement(ElementName = "refItemCarga")]
//////        public RefItemCarga RefItemCarga { get; set; }

//////        [XmlElement(ElementName = "ItemCarga")]
//////        public ItemCarga ItemCarga { get; set; }
//////    }

//////    [XmlRoot(ElementName = "Vagao")]
//////    public class Vagao
//////    {

//////        [XmlElement(ElementName = "DadosCadastrais")]
//////        public DadosCadastrais DadosCadastrais { get; set; }

//////        [XmlElement(ElementName = "posicao")]
//////        public string Posicao { get; set; }

//////        [XmlElement(ElementName = "lotacao")]
//////        public string Lotacao { get; set; }

//////        [XmlElement(ElementName = "siglaPatioDestino")]
//////        public string SiglaPatioDestino { get; set; }

//////        [XmlElement(ElementName = "siglaPatioOrigem")]
//////        public string SiglaPatioOrigem { get; set; }

//////        [XmlElement(ElementName = "docsFiscaisVagao")]
//////        public DocsFiscaisVagao DocsFiscaisVagao { get; set; }

//////        [XmlElement(ElementName = "docsTransporte")]
//////        public DocsTransporte DocsTransporte { get; set; }

//////        [XmlElement(ElementName = "quantidadeCarga")]
//////        public string QuantidadeCarga { get; set; }

//////        [XmlElement(ElementName = "itensCarga")]
//////        public ItensCarga ItensCarga { get; set; }

//////        [XmlElement(ElementName = "pesoVagaoToneladaBruta")]
//////        public string PesoVagaoToneladaBruta { get; set; }

//////        [XmlElement(ElementName = "pesoCargaTonelada")]
//////        public string PesoCargaTonelada { get; set; }

//////        [XmlElement(ElementName = "indicadorProprietarioLona")]
//////        public string IndicadorProprietarioLona { get; set; }

//////        [XmlElement(ElementName = "numeroLona")]
//////        public string NumeroLona { get; set; }

//////        [XmlElement(ElementName = "codigoVagaoRefCliente")]
//////        public string CodigoVagaoRefCliente { get; set; }

//////        [XmlAttribute(AttributeName = "IdVagao")]
//////        public string IdVagao { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "vagoes")]
//////    public class Vagoes
//////    {

//////        [XmlElement(ElementName = "Vagao")]
//////        public Vagao Vagao { get; set; }
//////    }

//////    [XmlRoot(ElementName = "Locomotiva")]
//////    public class Locomotiva
//////    {

//////        [XmlElement(ElementName = "DadosCadastrais")]
//////        public DadosCadastrais DadosCadastrais { get; set; }

//////        [XmlElement(ElementName = "posicao")]
//////        public string Posicao { get; set; }

//////        [XmlElement(ElementName = "funcao")]
//////        public string Funcao { get; set; }

//////        [XmlElement(ElementName = "sentido")]
//////        public string Sentido { get; set; }

//////        [XmlAttribute(AttributeName = "IdLocomotiva")]
//////        public string IdLocomotiva { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "locomotivas")]
//////    public class Locomotivas
//////    {

//////        [XmlElement(ElementName = "Locomotiva")]
//////        public Locomotiva Locomotiva { get; set; }
//////    }

//////    [XmlRoot(ElementName = "composicao")]
//////    public class Composicao
//////    {

//////        [XmlElement(ElementName = "vagoes")]
//////        public Vagoes Vagoes { get; set; }

//////        [XmlElement(ElementName = "locomotivas")]
//////        public Locomotivas Locomotivas { get; set; }
//////    }

//////    [XmlRoot(ElementName = "endereco")]
//////    public class Endereco
//////    {

//////        [XmlElement(ElementName = "xLgr")]
//////        public string XLgr { get; set; }

//////        [XmlElement(ElementName = "nro")]
//////        public string Nro { get; set; }

//////        [XmlElement(ElementName = "xCpl")]
//////        public string XCpl { get; set; }

//////        [XmlElement(ElementName = "xBairro")]
//////        public string XBairro { get; set; }

//////        [XmlElement(ElementName = "cMun")]
//////        public string CMun { get; set; }

//////        [XmlElement(ElementName = "xMun")]
//////        public string XMun { get; set; }

//////        [XmlElement(ElementName = "CEP")]
//////        public string CEP { get; set; }

//////        [XmlElement(ElementName = "UF")]
//////        public string UF { get; set; }

//////        [XmlElement(ElementName = "cPais")]
//////        public string CPais { get; set; }

//////        [XmlElement(ElementName = "xPais")]
//////        public string XPais { get; set; }
//////    }

//////    [XmlRoot(ElementName = "remetente")]
//////    public class Remetente
//////    {

//////        [XmlElement(ElementName = "CNPJ")]
//////        public string CNPJ { get; set; }

//////        [XmlElement(ElementName = "CPF")]
//////        public string CPF { get; set; }

//////        [XmlElement(ElementName = "IE")]
//////        public string IE { get; set; }

//////        [XmlElement(ElementName = "xNome")]
//////        public string XNome { get; set; }

//////        [XmlElement(ElementName = "xFant")]
//////        public string XFant { get; set; }

//////        [XmlElement(ElementName = "endereco")]
//////        public Endereco Endereco { get; set; }
//////    }

//////    [XmlRoot(ElementName = "destinatario")]
//////    public class Destinatario
//////    {

//////        [XmlElement(ElementName = "CNPJ")]
//////        public string CNPJ { get; set; }

//////        [XmlElement(ElementName = "CPF")]
//////        public string CPF { get; set; }

//////        [XmlElement(ElementName = "IE")]
//////        public string IE { get; set; }

//////        [XmlElement(ElementName = "xNome")]
//////        public string XNome { get; set; }

//////        [XmlElement(ElementName = "xFant")]
//////        public string XFant { get; set; }

//////        [XmlElement(ElementName = "endereco")]
//////        public Endereco Endereco { get; set; }
//////    }

//////    [XmlRoot(ElementName = "infNF")]
//////    public class InfNF
//////    {

//////        [XmlElement(ElementName = "nRoma")]
//////        public string NRoma { get; set; }

//////        [XmlElement(ElementName = "serie")]
//////        public string Serie { get; set; }

//////        [XmlElement(ElementName = "vBC")]
//////        public string VBC { get; set; }

//////        [XmlElement(ElementName = "vICMS")]
//////        public string VICMS { get; set; }

//////        [XmlElement(ElementName = "vBCST")]
//////        public string VBCST { get; set; }

//////        [XmlElement(ElementName = "vST")]
//////        public string VST { get; set; }

//////        [XmlElement(ElementName = "vProd")]
//////        public string VProd { get; set; }
//////    }

//////    [XmlRoot(ElementName = "infNFe")]
//////    public class InfNFe
//////    {

//////        [XmlElement(ElementName = "chaveNFe")]
//////        public string ChaveNFe { get; set; }
//////    }

//////    [XmlRoot(ElementName = "infOutros")]
//////    public class InfOutros
//////    {

//////        [XmlElement(ElementName = "tpDoc")]
//////        public string TpDoc { get; set; }

//////        [XmlElement(ElementName = "descOutros")]
//////        public string DescOutros { get; set; }
//////    }

//////    [XmlRoot(ElementName = "ItemDocumentoFiscal")]
//////    public class ItemDocumentoFiscal
//////    {

//////        [XmlElement(ElementName = "valor")]
//////        public string Valor { get; set; }

//////        [XmlElement(ElementName = "descricaoDetalhada")]
//////        public string DescricaoDetalhada { get; set; }

//////        [XmlElement(ElementName = "codigoDescricaoDetalhada")]
//////        public string CodigoDescricaoDetalhada { get; set; }

//////        [XmlElement(ElementName = "NCM")]
//////        public string NCM { get; set; }

//////        [XmlElement(ElementName = "quantidadeUnidadeMedida")]
//////        public string QuantidadeUnidadeMedida { get; set; }

//////        [XmlElement(ElementName = "descricaoUnidadeMedida")]
//////        public string DescricaoUnidadeMedida { get; set; }
//////    }

//////    [XmlRoot(ElementName = "itensDocumentoFiscal")]
//////    public class ItensDocumentoFiscal
//////    {

//////        [XmlElement(ElementName = "ItemDocumentoFiscal")]
//////        public ItemDocumentoFiscal ItemDocumentoFiscal { get; set; }
//////    }

//////    [XmlRoot(ElementName = "DocumentoFiscal")]
//////    public class DocumentoFiscal
//////    {

//////        [XmlElement(ElementName = "nDoc")]
//////        public string NDoc { get; set; }

//////        [XmlElement(ElementName = "dEmi")]
//////        public string DEmi { get; set; }

//////        [XmlElement(ElementName = "valor")]
//////        public string Valor { get; set; }

//////        [XmlElement(ElementName = "nPeso")]
//////        public string NPeso { get; set; }

//////        [XmlElement(ElementName = "remetente")]
//////        public Remetente Remetente { get; set; }

//////        [XmlElement(ElementName = "destinatario")]
//////        public Destinatario Destinatario { get; set; }

//////        [XmlElement(ElementName = "NCMPredominante")]
//////        public string NCMPredominante { get; set; }

//////        [XmlElement(ElementName = "codigoDocumentoRefCliente")]
//////        public string CodigoDocumentoRefCliente { get; set; }

//////        [XmlElement(ElementName = "nCFOP")]
//////        public string NCFOP { get; set; }

//////        [XmlElement(ElementName = "infNF")]
//////        public InfNF InfNF { get; set; }

//////        [XmlElement(ElementName = "infNFe")]
//////        public InfNFe InfNFe { get; set; }

//////        [XmlElement(ElementName = "infOutros")]
//////        public InfOutros InfOutros { get; set; }

//////        [XmlElement(ElementName = "itensDocumentoFiscal")]
//////        public ItensDocumentoFiscal ItensDocumentoFiscal { get; set; }

//////        [XmlAttribute(AttributeName = "idDocFiscal")]
//////        public string IdDocFiscal { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "documentosFiscais")]
//////    public class DocumentosFiscais
//////    {

//////        [XmlElement(ElementName = "DocumentoFiscal")]
//////        public DocumentoFiscal DocumentoFiscal { get; set; }
//////    }

//////    [XmlRoot(ElementName = "identificadorCTe")]
//////    public class IdentificadorCTe
//////    {

//////        [XmlElement(ElementName = "tipoEmissao")]
//////        public string TipoEmissao { get; set; }

//////        [XmlElement(ElementName = "serie")]
//////        public string Serie { get; set; }

//////        [XmlElement(ElementName = "numeroFiscal")]
//////        public string NumeroFiscal { get; set; }

//////        [XmlElement(ElementName = "chaveCTe")]
//////        public string ChaveCTe { get; set; }

//////        [XmlElement(ElementName = "chaveCTeOriginal")]
//////        public string ChaveCTeOriginal { get; set; }
//////    }

//////    [XmlRoot(ElementName = "fluxoTransporte")]
//////    public class FluxoTransporte
//////    {

//////        [XmlElement(ElementName = "numeroFluxo")]
//////        public string NumeroFluxo { get; set; }

//////        [XmlElement(ElementName = "numeroFluxoOutraFerrovia")]
//////        public string NumeroFluxoOutraFerrovia { get; set; }
//////    }

//////    [XmlRoot(ElementName = "docsFiscais")]
//////    public class DocsFiscais
//////    {

//////        [XmlElement(ElementName = "refDocFiscal")]
//////        public RefDocFiscal RefDocFiscal { get; set; }
//////    }

//////    [XmlRoot(ElementName = "refDocumentoTransporteAnterior")]
//////    public class RefDocumentoTransporteAnterior
//////    {

//////        [XmlAttribute(AttributeName = "idDocumentoTransporteAnterior")]
//////        public string IdDocumentoTransporteAnterior { get; set; }
//////    }

//////    [XmlRoot(ElementName = "documentosTransporteAnterior")]
//////    public class DocumentosTransporteAnterior
//////    {

//////        [XmlElement(ElementName = "refDocumentoTransporteAnterior")]
//////        public RefDocumentoTransporteAnterior RefDocumentoTransporteAnterior { get; set; }

//////        [XmlElement(ElementName = "DocumentoTransporteAnterior")]
//////        public DocumentoTransporteAnterior DocumentoTransporteAnterior { get; set; }
//////    }

//////    [XmlRoot(ElementName = "RotaFerroviaria")]
//////    public class RotaFerroviaria
//////    {

//////        [XmlElement(ElementName = "siglaPatioOrigem")]
//////        public string SiglaPatioOrigem { get; set; }

//////        [XmlElement(ElementName = "siglaTerminalOrigem")]
//////        public string SiglaTerminalOrigem { get; set; }

//////        [XmlElement(ElementName = "siglaPatioDestino")]
//////        public string SiglaPatioDestino { get; set; }

//////        [XmlElement(ElementName = "siglaTerminalDestino")]
//////        public string SiglaTerminalDestino { get; set; }

//////        [XmlElement(ElementName = "siglaPatioIntercambioOrigem")]
//////        public string SiglaPatioIntercambioOrigem { get; set; }

//////        [XmlElement(ElementName = "siglaPatioIntercambioDestino")]
//////        public string SiglaPatioIntercambioDestino { get; set; }
//////    }

//////    [XmlRoot(ElementName = "toma03")]
//////    public class Toma03
//////    {

//////        [XmlElement(ElementName = "toma")]
//////        public string Toma { get; set; }
//////    }

//////    [XmlRoot(ElementName = "enderToma")]
//////    public class EnderToma
//////    {

//////        [XmlElement(ElementName = "xLgr")]
//////        public string XLgr { get; set; }

//////        [XmlElement(ElementName = "nro")]
//////        public string Nro { get; set; }

//////        [XmlElement(ElementName = "xCpl")]
//////        public string XCpl { get; set; }

//////        [XmlElement(ElementName = "xBairro")]
//////        public string XBairro { get; set; }

//////        [XmlElement(ElementName = "cMun")]
//////        public string CMun { get; set; }

//////        [XmlElement(ElementName = "xMun")]
//////        public string XMun { get; set; }

//////        [XmlElement(ElementName = "CEP")]
//////        public string CEP { get; set; }

//////        [XmlElement(ElementName = "UF")]
//////        public string UF { get; set; }

//////        [XmlElement(ElementName = "cPais")]
//////        public string CPais { get; set; }

//////        [XmlElement(ElementName = "xPais")]
//////        public string XPais { get; set; }
//////    }

//////    [XmlRoot(ElementName = "toma4")]
//////    public class Toma4
//////    {

//////        [XmlElement(ElementName = "toma")]
//////        public string Toma { get; set; }

//////        [XmlElement(ElementName = "CNPJ")]
//////        public string CNPJ { get; set; }

//////        [XmlElement(ElementName = "CPF")]
//////        public string CPF { get; set; }

//////        [XmlElement(ElementName = "IE")]
//////        public string IE { get; set; }

//////        [XmlElement(ElementName = "xNome")]
//////        public string XNome { get; set; }

//////        [XmlElement(ElementName = "xFant")]
//////        public string XFant { get; set; }

//////        [XmlElement(ElementName = "fone")]
//////        public string Fone { get; set; }

//////        [XmlElement(ElementName = "enderToma")]
//////        public EnderToma EnderToma { get; set; }
//////    }

//////    [XmlRoot(ElementName = "tomadorServico")]
//////    public class TomadorServico
//////    {

//////        [XmlElement(ElementName = "toma03")]
//////        public Toma03 Toma03 { get; set; }

//////        [XmlElement(ElementName = "toma4")]
//////        public Toma4 Toma4 { get; set; }
//////    }

//////    [XmlRoot(ElementName = "Local")]
//////    public class Local
//////    {

//////        [XmlElement(ElementName = "CNPJ")]
//////        public string CNPJ { get; set; }

//////        [XmlElement(ElementName = "CPF")]
//////        public string CPF { get; set; }

//////        [XmlElement(ElementName = "xNome")]
//////        public string XNome { get; set; }

//////        [XmlElement(ElementName = "xLgr")]
//////        public string XLgr { get; set; }

//////        [XmlElement(ElementName = "nro")]
//////        public string Nro { get; set; }

//////        [XmlElement(ElementName = "xCpl")]
//////        public string XCpl { get; set; }

//////        [XmlElement(ElementName = "xBairro")]
//////        public string XBairro { get; set; }

//////        [XmlElement(ElementName = "cMun")]
//////        public string CMun { get; set; }

//////        [XmlElement(ElementName = "xMun")]
//////        public string XMun { get; set; }

//////        [XmlElement(ElementName = "UF")]
//////        public string UF { get; set; }
//////    }

//////    [XmlRoot(ElementName = "origemPrestacao")]
//////    public class OrigemPrestacao
//////    {

//////        [XmlElement(ElementName = "origem")]
//////        public string Origem { get; set; }

//////        [XmlElement(ElementName = "Local")]
//////        public Local Local { get; set; }
//////    }

//////    [XmlRoot(ElementName = "destinoPrestacao")]
//////    public class DestinoPrestacao
//////    {

//////        [XmlElement(ElementName = "destino")]
//////        public string Destino { get; set; }

//////        [XmlElement(ElementName = "Local")]
//////        public Local Local { get; set; }
//////    }

//////    [XmlRoot(ElementName = "prefixoTremPartida")]
//////    public class PrefixoTremPartida
//////    {

//////        [XmlElement(ElementName = "codigoPrefixoTrem")]
//////        public string CodigoPrefixoTrem { get; set; }

//////        [XmlElement(ElementName = "dataHoraInicioValidadePrefixo")]
//////        public string DataHoraInicioValidadePrefixo { get; set; }
//////    }

//////    [XmlRoot(ElementName = "prefixoTremUltimaAnexacao")]
//////    public class PrefixoTremUltimaAnexacao
//////    {

//////        [XmlElement(ElementName = "codigoPrefixoTrem")]
//////        public string CodigoPrefixoTrem { get; set; }

//////        [XmlElement(ElementName = "dataHoraInicioValidadePrefixo")]
//////        public string DataHoraInicioValidadePrefixo { get; set; }
//////    }

//////    [XmlRoot(ElementName = "DocumentoTransporte")]
//////    public class DocumentoTransporte
//////    {

//////        [XmlElement(ElementName = "identificadorCTe")]
//////        public IdentificadorCTe IdentificadorCTe { get; set; }

//////        [XmlElement(ElementName = "fluxoTransporte")]
//////        public FluxoTransporte FluxoTransporte { get; set; }

//////        [XmlElement(ElementName = "docsFiscais")]
//////        public DocsFiscais DocsFiscais { get; set; }

//////        [XmlElement(ElementName = "documentosTransporteAnterior")]
//////        public DocumentosTransporteAnterior DocumentosTransporteAnterior { get; set; }

//////        [XmlElement(ElementName = "dataGeracao")]
//////        public string DataGeracao { get; set; }

//////        [XmlElement(ElementName = "RotaFerroviaria")]
//////        public RotaFerroviaria RotaFerroviaria { get; set; }

//////        [XmlElement(ElementName = "tomadorServico")]
//////        public TomadorServico TomadorServico { get; set; }

//////        [XmlElement(ElementName = "origemPrestacao")]
//////        public OrigemPrestacao OrigemPrestacao { get; set; }

//////        [XmlElement(ElementName = "destinoPrestacao")]
//////        public DestinoPrestacao DestinoPrestacao { get; set; }

//////        [XmlElement(ElementName = "prefixoTremPartida")]
//////        public PrefixoTremPartida PrefixoTremPartida { get; set; }

//////        [XmlElement(ElementName = "prefixoTremUltimaAnexacao")]
//////        public PrefixoTremUltimaAnexacao PrefixoTremUltimaAnexacao { get; set; }

//////        [XmlElement(ElementName = "codigoComposicaoCliente")]
//////        public string CodigoComposicaoCliente { get; set; }

//////        [XmlElement(ElementName = "PdfDacte")]
//////        public string PdfDacte { get; set; }

//////        [XmlAttribute(AttributeName = "idDocumentoTransporte")]
//////        public string IdDocumentoTransporte { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "documentosTransporte")]
//////    public class DocumentosTransporte
//////    {

//////        [XmlElement(ElementName = "DocumentoTransporte")]
//////        public DocumentoTransporte DocumentoTransporte { get; set; }
//////    }

//////    [XmlRoot(ElementName = "idDocAntEle")]
//////    public class IdDocAntEle
//////    {

//////        [XmlElement(ElementName = "chave")]
//////        public string Chave { get; set; }
//////    }

//////    [XmlRoot(ElementName = "idDocAntPap")]
//////    public class IdDocAntPap
//////    {

//////        [XmlElement(ElementName = "tpDoc")]
//////        public string TpDoc { get; set; }

//////        [XmlElement(ElementName = "serie")]
//////        public string Serie { get; set; }

//////        [XmlElement(ElementName = "subser")]
//////        public string Subser { get; set; }

//////        [XmlElement(ElementName = "nDoc")]
//////        public string NDoc { get; set; }
//////    }

//////    [XmlRoot(ElementName = "idDocAnt")]
//////    public class IdDocAnt
//////    {

//////        [XmlElement(ElementName = "idDocAntEle")]
//////        public IdDocAntEle IdDocAntEle { get; set; }

//////        [XmlElement(ElementName = "idDocAntPap")]
//////        public IdDocAntPap IdDocAntPap { get; set; }
//////    }

//////    [XmlRoot(ElementName = "DocumentoTransporteAnterior")]
//////    public class DocumentoTransporteAnterior
//////    {

//////        [XmlElement(ElementName = "CNPJ")]
//////        public string CNPJ { get; set; }

//////        [XmlElement(ElementName = "CPF")]
//////        public string CPF { get; set; }

//////        [XmlElement(ElementName = "xNome")]
//////        public string XNome { get; set; }

//////        [XmlElement(ElementName = "IE")]
//////        public string IE { get; set; }

//////        [XmlElement(ElementName = "UF")]
//////        public string UF { get; set; }

//////        [XmlElement(ElementName = "dEmi")]
//////        public string DEmi { get; set; }

//////        [XmlElement(ElementName = "idDocAnt")]
//////        public IdDocAnt IdDocAnt { get; set; }

//////        [XmlElement(ElementName = "docsFiscais")]
//////        public DocsFiscais DocsFiscais { get; set; }

//////        [XmlElement(ElementName = "PdfDacte")]
//////        public string PdfDacte { get; set; }

//////        [XmlAttribute(AttributeName = "idDocTransporte")]
//////        public string IdDocTransporte { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "Equipagem")]
//////    public class Equipagem
//////    {

//////        [XmlElement(ElementName = "dataHoraInicioAtividade")]
//////        public string DataHoraInicioAtividade { get; set; }

//////        [XmlElement(ElementName = "dataHoraTerminoAtividade")]
//////        public string DataHoraTerminoAtividade { get; set; }

//////        [XmlElement(ElementName = "matricula")]
//////        public string Matricula { get; set; }

//////        [XmlElement(ElementName = "nome")]
//////        public string Nome { get; set; }

//////        [XmlElement(ElementName = "atividade")]
//////        public string Atividade { get; set; }
//////    }

//////    [XmlRoot(ElementName = "equipagens")]
//////    public class Equipagens
//////    {

//////        [XmlElement(ElementName = "Equipagem")]
//////        public Equipagem Equipagem { get; set; }
//////    }

//////    [XmlRoot(ElementName = "refVagao")]
//////    public class RefVagao
//////    {

//////        [XmlElement(ElementName = "tipoMovimentacao")]
//////        public string TipoMovimentacao { get; set; }

//////        [XmlElement(ElementName = "dataHoraMovimentacao")]
//////        public string DataHoraMovimentacao { get; set; }

//////        [XmlElement(ElementName = "observacao")]
//////        public string Observacao { get; set; }

//////        [XmlAttribute(AttributeName = "idVagao")]
//////        public string IdVagao { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "movimentacaoRealizada")]
//////    public class MovimentacaoRealizada
//////    {

//////        [XmlElement(ElementName = "refVagao")]
//////        public RefVagao RefVagao { get; set; }
//////    }

//////    [XmlRoot(ElementName = "refDocFiscalCliente")]
//////    public class RefDocFiscalCliente
//////    {

//////        [XmlAttribute(AttributeName = "idDocFiscal")]
//////        public string IdDocFiscal { get; set; }
//////    }

//////    [XmlRoot(ElementName = "docsFiscaisCarga")]
//////    public class DocsFiscaisCarga
//////    {

//////        [XmlElement(ElementName = "refDocFiscalCliente")]
//////        public RefDocFiscalCliente RefDocFiscalCliente { get; set; }
//////    }

//////    [XmlRoot(ElementName = "lacres")]
//////    public class Lacres
//////    {

//////        [XmlElement(ElementName = "nLacre")]
//////        public string NLacre { get; set; }
//////    }

//////    [XmlRoot(ElementName = "ItemCarga")]
//////    public class ItemCarga
//////    {

//////        [XmlElement(ElementName = "identificadorCarga")]
//////        public string IdentificadorCarga { get; set; }

//////        [XmlElement(ElementName = "numeroContainer")]
//////        public string NumeroContainer { get; set; }

//////        [XmlElement(ElementName = "tamanhoContainer")]
//////        public string TamanhoContainer { get; set; }

//////        [XmlElement(ElementName = "docsFiscaisCarga")]
//////        public DocsFiscaisCarga DocsFiscaisCarga { get; set; }

//////        [XmlElement(ElementName = "lacres")]
//////        public Lacres Lacres { get; set; }

//////        [XmlAttribute(AttributeName = "idItemCarga")]
//////        public string IdItemCarga { get; set; }

//////        [XmlText]
//////        public string Text { get; set; }
//////    }

//////    [XmlRoot(ElementName = "Trem")]
//////    public class Trem
//////    {

//////        [XmlElement(ElementName = "CNPJFerroviaTrem")]
//////        public string CNPJFerroviaTrem { get; set; }

//////        [XmlElement(ElementName = "siglaPatioOrigem")]
//////        public string SiglaPatioOrigem { get; set; }

//////        [XmlElement(ElementName = "siglaPatioDestino")]
//////        public string SiglaPatioDestino { get; set; }

//////        [XmlElement(ElementName = "PrefixoTrem")]
//////        public PrefixoTrem PrefixoTrem { get; set; }

//////        [XmlElement(ElementName = "FamiliaTrem")]
//////        public FamiliaTrem FamiliaTrem { get; set; }

//////        [XmlElement(ElementName = "estadoTrem")]
//////        public string EstadoTrem { get; set; }

//////        [XmlElement(ElementName = "PatioItinerarioAtual")]
//////        public PatioItinerarioAtual PatioItinerarioAtual { get; set; }

//////        [XmlElement(ElementName = "patiosTranspostos")]
//////        public PatiosTranspostos PatiosTranspostos { get; set; }

//////        [XmlElement(ElementName = "composicao")]
//////        public Composicao Composicao { get; set; }

//////        [XmlElement(ElementName = "documentosFiscais")]
//////        public DocumentosFiscais DocumentosFiscais { get; set; }

//////        [XmlElement(ElementName = "documentosTransporte")]
//////        public DocumentosTransporte DocumentosTransporte { get; set; }

//////        [XmlElement(ElementName = "documentosTransporteAnterior")]
//////        public DocumentosTransporteAnterior DocumentosTransporteAnterior { get; set; }

//////        [XmlElement(ElementName = "equipagens")]
//////        public Equipagens Equipagens { get; set; }

//////        [XmlElement(ElementName = "dataHoraSaidaOrigem")]
//////        public string DataHoraSaidaOrigem { get; set; }

//////        [XmlElement(ElementName = "dataHoraPrevisaoChegadaDestino")]
//////        public string DataHoraPrevisaoChegadaDestino { get; set; }

//////        [XmlElement(ElementName = "movimentacaoRealizada")]
//////        public MovimentacaoRealizada MovimentacaoRealizada { get; set; }

//////        [XmlElement(ElementName = "itensCarga")]
//////        public ItensCarga ItensCarga { get; set; }

//////        [XmlElement(ElementName = "observacao")]
//////        public string Observacao { get; set; }

//////        [XmlElement(ElementName = "tabela")]
//////        public string Tabela { get; set; }

//////        [XmlElement(ElementName = "identificadorTrem")]
//////        public string IdentificadorTrem { get; set; }

//////        [XmlElement(ElementName = "OtherXML")]
//////        public string OtherXML { get; set; }
//////    }

//////    [XmlRoot(ElementName = "AcompanhamentoTrem")]
//////    public class AcompanhamentoTrem
//////    {

//////        [XmlElement(ElementName = "numeroEnvio")]
//////        public string NumeroEnvio { get; set; }

//////        [XmlElement(ElementName = "dataHoraEnvio")]
//////        public string DataHoraEnvio { get; set; }

//////        [XmlElement(ElementName = "CNPJFerrovia")]
//////        public string CNPJFerrovia { get; set; }

//////        [XmlElement(ElementName = "CNPJDestinatario")]
//////        public string CNPJDestinatario { get; set; }

//////        [XmlElement(ElementName = "nomeProcessoEnvio")]
//////        public string NomeProcessoEnvio { get; set; }

//////        [XmlElement(ElementName = "Trem")]
//////        public Trem Trem { get; set; }

//////    }


//////    [System.ServiceModel.MessageContract(IsWrapped = false, WrapperNamespace = "http://xmlns.mrs.com.br/iti/tipos/acompanhamento")]
//////    public partial class MAcompanhamentoTrem
//////    {
//////        [MessageBodyMember(Name = "AcompanhamentoTrem", Namespace = "http://xmlns.mrs.com.br/iti/tipos/acompanhamento")]
//////        public AcompanhamentoTrem data { get; set; }
//////    }

//////}