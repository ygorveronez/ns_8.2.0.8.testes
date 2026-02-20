
namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DTe
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/dte/wsdl/dteRecepcaoLote")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/dte/wsdl/dteRecepcaoLote", IsNullable = false)]
    public partial class dteRecepcaoLoteResult
    {

        private string versaoField;

        private TAmb tpAmbField;

        private string cStatField;

        private string xMotivoField;

        private string nRecField;

        private int protDTeField;

        /// <remarks/>
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
        public string nRec
        {
            get
            {
                return this.nRecField;
            }
            set
            {
                this.nRecField = value;
            }
        }

        /// <remarks/>
        public int protDTe
        {
            get
            {
                return this.protDTeField;
            }
            set
            {
                this.protDTeField = value;
            }
        }
    }
}



