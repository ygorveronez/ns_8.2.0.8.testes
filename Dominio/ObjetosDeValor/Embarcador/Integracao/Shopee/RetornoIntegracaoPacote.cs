using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Shopee
{
    public class CteProc
    {
        [XmlElement("CTe")]
        public CTe CTe { get; set; }

        [XmlElement("protCTe")]
        public ProtCTe ProtCTe { get; set; }
    }

    public class CTe
    {
        [XmlElement("infCte")]
        public InfCte InfCte { get; set; }
    }

    public class InfCte
    {
        [XmlElement("ide")]
        public Ide Ide { get; set; }

        [XmlElement("compl")]
        public Compl Compl { get; set; }

        [XmlElement("emit")]
        public Emit Emit { get; set; }

        [XmlElement("rem")]
        public Rem Rem { get; set; }

        [XmlElement("exped")]
        public Exped Exped { get; set; }

        [XmlElement("receb")]
        public Receb Receb { get; set; }

        [XmlElement("dest")]
        public Dest Dest { get; set; }

        [XmlElement("vPrest")]
        public VPrest VPrest { get; set; }

        [XmlElement("imp")]
        public Imp Imp { get; set; }

        [XmlElement("infCTeNorm")]
        public InfCTeNorm InfCTeNorm { get; set; }
    }

    public class Ide
    {
        [XmlElement("cUF")]
        public string CUf { get; set; }

        [XmlElement("cCT")]
        public string CCt { get; set; }

        [XmlElement("CFOP")]
        public string CFOP { get; set; }

        [XmlElement("natOp")]
        public string NatOp { get; set; }

        // Add other properties for the remaining elements
    }

    public class Compl
    {
        [XmlElement("Entrega")]
        public Entrega Entrega { get; set; }

        [XmlElement("xObs")]
        public string XObs { get; set; }

        // Add other properties for the remaining elements
    }

    public class Entrega
    {
        [XmlElement("semData")]
        public SemData SemData { get; set; }

        [XmlElement("semHora")]
        public SemHora SemHora { get; set; }

        // Add other properties for the remaining elements
    }

    public class SemData
    {
        [XmlElement("tpPer")]
        public string TpPer { get; set; }
    }

    public class SemHora
    {
        [XmlElement("tpHor")]
        public string TpHor { get; set; }
    }

    public class Emit
    {
        [XmlElement("CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement("IE")]
        public string IE { get; set; }

        [XmlElement("xNome")]
        public string XNome { get; set; }

        [XmlElement("enderEmit")]
        public EnderEmit EnderEmit { get; set; }
    }

    public class EnderEmit
    {
        [XmlElement("xLgr")]
        public string XLgr { get; set; }

        [XmlElement("nro")]
        public string Nro { get; set; }

        [XmlElement("xBairro")]
        public string XBairro { get; set; }

        [XmlElement("cMun")]
        public string CMun { get; set; }

        [XmlElement("xMun")]
        public string XMun { get; set; }

        [XmlElement("CEP")]
        public string CEP { get; set; }

        [XmlElement("UF")]
        public string UF { get; set; }
    }

    public class Rem
    {
        [XmlElement("CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement("IE")]
        public string IE { get; set; }

        [XmlElement("xNome")]
        public string XNome { get; set; }

        [XmlElement("enderReme")]
        public EnderReme EnderReme { get; set; }
    }

    public class EnderReme
    {
        [XmlElement("xLgr")]
        public string XLgr { get; set; }

        [XmlElement("nro")]
        public string Nro { get; set; }

        [XmlElement("xCpl")]
        public string XCpl { get; set; }

        [XmlElement("xBairro")]
        public string XBairro { get; set; }

        [XmlElement("cMun")]
        public string CMun { get; set; }

        [XmlElement("xMun")]
        public string XMun { get; set; }

        [XmlElement("UF")]
        public string UF { get; set; }
    }


    public class Exped
    {
        [XmlElement("CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement("IE")]
        public string IE { get; set; }

        [XmlElement("xNome")]
        public string XNome { get; set; }

        [XmlElement("enderExped")]
        public EnderExped EnderExped { get; set; }
    }

    public class EnderExped
    {
        [XmlElement("xLgr")]
        public string XLgr { get; set; }

        [XmlElement("nro")]
        public string Nro { get; set; }

        [XmlElement("xBairro")]
        public string XBairro { get; set; }

        [XmlElement("cMun")]
        public string CMun { get; set; }

        [XmlElement("xMun")]
        public string XMun { get; set; }

        [XmlElement("UF")]
        public string UF { get; set; }
    }

    public class Receb
    {
        [XmlElement("CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement("xNome")]
        public string XNome { get; set; }

        [XmlElement("enderReceb")]
        public EnderReceb EnderReceb { get; set; }
    }

    public class EnderReceb
    {
        [XmlElement("xLgr")]
        public string XLgr { get; set; }

        [XmlElement("nro")]
        public string Nro { get; set; }

        [XmlElement("xBairro")]
        public string XBairro { get; set; }

        [XmlElement("cMun")]
        public string CMun { get; set; }

        [XmlElement("xMun")]
        public string XMun { get; set; }

        [XmlElement("UF")]
        public string UF { get; set; }
    }

    public class Dest
    {
        [XmlElement("CPF")]
        public string CPF { get; set; }

        [XmlElement("xNome")]
        public string XNome { get; set; }

        [XmlElement("enderDest")]
        public EnderDest EnderDest { get; set; }
    }

    public class EnderDest
    {
        [XmlElement("xLgr")]
        public string XLgr { get; set; }

        [XmlElement("nro")]
        public string Nro { get; set; }

        [XmlElement("xBairro")]
        public string XBairro { get; set; }

        [XmlElement("cMun")]
        public string CMun { get; set; }

        [XmlElement("xMun")]
        public string XMun { get; set; }

        [XmlElement("UF")]
        public string UF { get; set; }
    }


    public class VPrest
    {
        [XmlElement("vTPrest")]
        public string VTPrest { get; set; }

        [XmlElement("vRec")]
        public string VRec { get; set; }
    }


    public class Imp
    {
        [XmlElement("ICMS")]
        public ICMS ICMS { get; set; }

        [XmlElement("vTotTrib")]
        public string VTotTrib { get; set; }

        [XmlElement("infAdFisco")]
        public string InfAdFisco { get; set; }
    }

    public class ICMS
    {
        [XmlElement("ICMS00")]
        public ICMS00 ICMS00 { get; set; }
    }

    public class ICMS00
    {
        [XmlElement("CST")]
        public string CST { get; set; }

        [XmlElement("vBC")]
        public string VBC { get; set; }

        [XmlElement("pICMS")]
        public string PICMS { get; set; }

        [XmlElement("vICMS")]
        public string VICMS { get; set; }
    }

    public class InfCTeNorm
    {
        [XmlElement("infCarga")]
        public InfCarga InfCarga { get; set; }

        [XmlElement("infDoc")]
        public InfDoc InfDoc { get; set; }

        [XmlElement("infModal")]
        public InfModal InfModal { get; set; }
    }

    public class InfCarga
    {
        [XmlElement("vCarga")]
        public string VCarga { get; set; }

        [XmlElement("proPred")]
        public string ProPred { get; set; }

        [XmlElement("infQ")]
        public InfQ InfQ { get; set; }
    }

    public class InfDoc
    {
        [XmlElement("infNFe")]
        public InfNFe InfNFe { get; set; }
    }

    public class InfNFe
    {
        [XmlElement("chave")]
        public string Chave { get; set; }

        [XmlElement("dPrev")]
        public string DPrev { get; set; }
    }

    public class InfQ
    {
        [XmlElement("cUnid")]
        public string CUnid { get; set; }

        [XmlElement("tpMed")]
        public string TpMed { get; set; }

        [XmlElement("qCarga")]
        public string QCarga { get; set; }
    }

    public class InfModal
    {
        [XmlAttribute("versaoModal")]
        public string VersaoModal { get; set; }

        [XmlElement("rodo")]
        public Rodo Rodo { get; set; }
    }

    public class Rodo
    {
        [XmlElement("RNTRC")]
        public string RNTRC { get; set; }
    }

    public class ProtCTe
    {
        [XmlElement("infProt")]
        public InfProt InfProt { get; set; }
    }

    public class InfProt
    {
        [XmlElement("tpAmb")]
        public string TpAmb { get; set; }

        [XmlElement("verAplic")]
        public string VerAplic { get; set; }

        [XmlElement("chCTe")]
        public string ChCTe { get; set; }

        [XmlElement("dhRecbto")]
        public string DhRecbto { get; set; }

        [XmlElement("nProt")]
        public string NProt { get; set; }

        [XmlElement("digVal")]
        public string DigVal { get; set; }

        [XmlElement("cStat")]
        public string CStat { get; set; }

        [XmlElement("xMotivo")]
        public string XMotivo { get; set; }
    }

    // Add more classes for other elements in a similar manner

    class Program
    {
        static void Main()
        {
            // Your code to deserialize the XML into objects
        }
    }



}
