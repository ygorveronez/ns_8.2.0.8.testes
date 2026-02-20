using System.Xml.Serialization;

namespace MultiSoftware.CTe.v400.Servicos
{
    public  class Leitura
    {
        public static ConhecimentoDeTransporteProcessado.cteProc LerCTeProcessado(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConhecimentoDeTransporteProcessado.cteProc));
            ConhecimentoDeTransporteProcessado.cteProc cteProcessado = (ConhecimentoDeTransporteProcessado.cteProc)serializer.Deserialize(stream);
            return cteProcessado;
        }

        public static ConhecimentoDeTransporte.TCTe LerCTe(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConhecimentoDeTransporte.TCTe));
            ConhecimentoDeTransporte.TCTe cte = (ConhecimentoDeTransporte.TCTe)serializer.Deserialize(stream);
            return cte;
        }

        public static Eventos.TProcEvento LerEvento(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Eventos.TProcEvento));
            Eventos.TProcEvento evento = (Eventos.TProcEvento)serializer.Deserialize(stream);
            return evento;
        }
    }
}
