using MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Sefaz
{
    public class EventoComprovanteEntregaCte
    {

        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
        [System.Xml.Serialization.XmlRoot(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
        public class procEventoCTe : TProcEvento
        {
        }

        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
        [System.Xml.Serialization.XmlRoot("eventoCTe", Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
        public class TEvento
        {
            private TEventoInfEvento infEventoField;

            private XElement signatureField;

            private string versaoField;

            /// <remarks/>
            public TEventoInfEvento infEvento
            {
                get
                {
                    return this.infEventoField;
                }
                set
                {
                    this.infEventoField = value;
                }
            }

            /// <remarks/>
            //[System.Xml.Serialization.XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
            public XElement Signature
            {
                get
                {
                    return this.signatureField;
                }
                set
                {
                    this.signatureField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttribute()]
            public string versao
            {
                get
                {
                    return this.versaoField;
                }
                set
                {
                    this.versaoField = value;
                }
            }
        }
        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
        public class TEventoInfEvento
        {
            private TCOrgaoIBGE cOrgaoField;

            private TAmb tpAmbField;

            private string cNPJField;

            private string chCTeField;

            private string dhEventoField;

            private string tpEventoField;

            private string nSeqEventoField;

            private TEventoTEventoInfEventoTDetEvento detEventoField;

            private string idField;

            /// <remarks/>
            public TCOrgaoIBGE cOrgao
            {
                get
                {
                    return this.cOrgaoField;
                }
                set
                {
                    this.cOrgaoField = value;
                }
            }

            /// <remarks/>
            public TAmb tpAmb
            {
                get
                {
                    return this.tpAmbField;
                }
                set
                {
                    this.tpAmbField = value;
                }
            }

            /// <remarks/>
            public string CNPJ
            {
                get
                {
                    return this.cNPJField;
                }
                set
                {
                    this.cNPJField = value;
                }
            }

            /// <remarks/>
            public string chCTe
            {
                get
                {
                    return this.chCTeField;
                }
                set
                {
                    this.chCTeField = value;
                }
            }

            /// <remarks/>
            public string dhEvento
            {
                get
                {
                    return this.dhEventoField;
                }
                set
                {
                    this.dhEventoField = value;
                }
            }

            /// <remarks/>
            public string tpEvento
            {
                get
                {
                    return this.tpEventoField;
                }
                set
                {
                    this.tpEventoField = value;
                }
            }

            /// <remarks/>
            public string nSeqEvento
            {
                get
                {
                    return this.nSeqEventoField;
                }
                set
                {
                    this.nSeqEventoField = value;
                }
            }

            /// <remarks/>
            public TEventoTEventoInfEventoTDetEvento detEvento
            {
                get
                {
                    return this.detEventoField;
                }
                set
                {
                    this.detEventoField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttribute(DataType = "ID")]
            public string Id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }
        }


        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
        [System.Xml.Serialization.XmlRoot("detEvento", IsNullable = false)]
        public partial class TEventoTEventoInfEventoTDetEvento
        {

            private List<TevCECTe> evCECTeField;

            private string verEventoField;

            [System.Xml.Serialization.XmlElement("evCECTe")]
            public List<TevCECTe> evCECTe
            {
                get
                {
                    return this.evCECTeField;
                }
                set
                {
                    this.evCECTeField = value;
                }
            }

            [System.Xml.Serialization.XmlAttribute()]
            public string versaoEvento
            {
                get
                {
                    return this.verEventoField;
                }
                set
                {
                    this.verEventoField = value;
                }
            }
        }


        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(Namespace = "http://www.portalfiscal.inf.br/cte")]
        [System.Xml.Serialization.XmlRoot("evCECTe", IsNullable = false)]
        public class TevCECTe
        {

            private string descEventoField;

            private string nProtField;

            private string dhEntregaField;

            private string nDocField;

            private string xNomeField;

            private string latitudeField;

            private string longitudeField;

            private string hashEntregaField;

            private string dhHashEntregaField;

            private List<TInfEntrega> infEntregaField;

            /// <remarks/>
            public string descEvento
            {
                get
                {
                    return this.descEventoField;
                }
                set
                {
                    this.descEventoField = value;
                }
            }

            /// <remarks/>
            public string nProt
            {
                get
                {
                    return this.nProtField;
                }
                set
                {
                    this.nProtField = value;
                }
            }

            /// <remarks/>
            public string dhEntrega
            {
                get
                {
                    return this.dhEntregaField;
                }
                set
                {
                    this.dhEntregaField = value;
                }
            }

            /// <remarks/>
            public string nDoc
            {
                get
                {
                    return this.nDocField;
                }
                set
                {
                    this.nDocField = value;
                }
            }

            /// <remarks/>
            public string xNome
            {
                get
                {
                    return this.xNomeField;
                }
                set
                {
                    this.xNomeField = value;
                }
            }

            /// <remarks/>
            public string latitude
            {
                get
                {
                    return this.latitudeField;
                }
                set
                {
                    this.latitudeField = value;
                }
            }

            /// <remarks/>
            public string longitude
            {
                get
                {
                    return this.longitudeField;
                }
                set
                {
                    this.longitudeField = value;
                }
            }

            /// <remarks/>
            public string hashEntrega
            {
                get
                {
                    return this.hashEntregaField;
                }
                set
                {
                    this.hashEntregaField = value;
                }
            }

            /// <remarks/>
            public string dhHashEntrega
            {
                get
                {
                    return this.dhHashEntregaField;
                }
                set
                {
                    this.dhHashEntregaField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElement("infEntrega")]
            public List<TInfEntrega> infEntrega
            {
                get
                {
                    return this.infEntregaField;
                }
                set
                {
                    this.infEntregaField = value;
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(Namespace = "http://www.portalfiscal.inf.br/cte")]
        [System.Xml.Serialization.XmlRoot("infEntrega", IsNullable = false)]
        public class TInfEntrega
        {

            private string chNFeField;

            public string chNFe
            {
                get
                {
                    return this.chNFeField;
                }
                set
                {
                    this.chNFeField = value;
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(Namespace = "http://www.portalfiscal.inf.br/cte")]
        [System.Xml.Serialization.XmlRoot("procEventoCTe", Namespace = "http://www.portalfiscal.inf.br/cte")]
        public class TProcEvento
        {
            private TEvento eventoCTeField;

            private TRetEvento retEventoCTeField;

            private string versaoField;

            /// <remarks/>
            public TEvento eventoCTe
            {
                get
                {
                    return this.eventoCTeField;
                }
                set
                {
                    this.eventoCTeField = value;
                }
            }

            /// <remarks/>
            public TRetEvento retEventoCTe
            {
                get
                {
                    return this.retEventoCTeField;
                }
                set
                {
                    this.retEventoCTeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttribute()]
            public string versao
            {
                get
                {
                    return this.versaoField;
                }
                set
                {
                    this.versaoField = value;
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(Namespace = "http://www.portalfiscal.inf.br/cte")]
        [System.Xml.Serialization.XmlRoot("retEventoCTe", Namespace = "http://www.portalfiscal.inf.br/cte")]
        public class TRetEvento
        {
            private TRetEventoInfEvento infEventoField;

            private XmlElement signatureField;

            private string versaoField;

            /// <remarks/>
            public TRetEventoInfEvento infEvento
            {
                get
                {
                    return this.infEventoField;
                }
                set
                {
                    this.infEventoField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
            public XmlElement Signature
            {
                get
                {
                    return this.signatureField;
                }
                set
                {
                    this.signatureField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttribute()]
            public string versao
            {
                get
                {
                    return this.versaoField;
                }
                set
                {
                    this.versaoField = value;
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("xsd", "4.0.30319.1")]
        [System.Serializable()]
        [System.Diagnostics.DebuggerStepThrough()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
        [System.Xml.Serialization.XmlRoot("infEvento", Namespace = "http://www.portalfiscal.inf.br/cte")]
        public class TRetEventoInfEvento
        {
            private TAmb tpAmbField;

            private string verAplicField;

            private TCOrgaoIBGE cOrgaoField;

            private string cStatField;

            private string xMotivoField;

            private string chCTeField;

            private string tpEventoField;

            private string xEventoField;

            private string nSeqEventoField;

            private string dhRegEventoField;

            private string nProtField;

            private string idField;

            /// <remarks/>
            public TAmb tpAmb
            {
                get
                {
                    return this.tpAmbField;
                }
                set
                {
                    this.tpAmbField = value;
                }
            }

            /// <remarks/>
            public string verAplic
            {
                get
                {
                    return this.verAplicField;
                }
                set
                {
                    this.verAplicField = value;
                }
            }

            /// <remarks/>
            public TCOrgaoIBGE cOrgao
            {
                get
                {
                    return this.cOrgaoField;
                }
                set
                {
                    this.cOrgaoField = value;
                }
            }

            /// <remarks/>
            public string cStat
            {
                get
                {
                    return this.cStatField;
                }
                set
                {
                    this.cStatField = value;
                }
            }

            /// <remarks/>
            public string xMotivo
            {
                get
                {
                    return this.xMotivoField;
                }
                set
                {
                    this.xMotivoField = value;
                }
            }

            /// <remarks/>
            public string chCTe
            {
                get
                {
                    return this.chCTeField;
                }
                set
                {
                    this.chCTeField = value;
                }
            }

            /// <remarks/>
            public string tpEvento
            {
                get
                {
                    return this.tpEventoField;
                }
                set
                {
                    this.tpEventoField = value;
                }
            }

            /// <remarks/>
            public string xEvento
            {
                get
                {
                    return this.xEventoField;
                }
                set
                {
                    this.xEventoField = value;
                }
            }

            /// <remarks/>
            public string nSeqEvento
            {
                get
                {
                    return this.nSeqEventoField;
                }
                set
                {
                    this.nSeqEventoField = value;
                }
            }

            /// <remarks/>
            public string dhRegEvento
            {
                get
                {
                    return this.dhRegEventoField;
                }
                set
                {
                    this.dhRegEventoField = value;
                }
            }

            /// <remarks/>
            public string nProt
            {
                get
                {
                    return this.nProtField;
                }
                set
                {
                    this.nProtField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttribute(DataType = "ID")]
            public string Id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }
        }


    }
}
