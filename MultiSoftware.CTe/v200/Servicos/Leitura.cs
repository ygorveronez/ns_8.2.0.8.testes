using System.Xml.Serialization;

namespace MultiSoftware.CTe.v200.Servicos
{
    public class Leitura
    {
        public static ConhecimentoDeTransporteProcessado.cteProc LerCTeProcessado(string caminho)
        {
            using (System.IO.Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho))
            {
                ConhecimentoDeTransporteProcessado.cteProc cte = LerCTeProcessado(stream);
                return cte;
            }
        }

        public static ConhecimentoDeTransporteProcessado.cteProc LerCTeProcessado(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConhecimentoDeTransporteProcessado.cteProc));
            ConhecimentoDeTransporteProcessado.cteProc cteProcessado = (ConhecimentoDeTransporteProcessado.cteProc)serializer.Deserialize(stream);
            return cteProcessado;
        }

        public static Eventos.TProcEvento LerEvento(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Eventos.TProcEvento));
            Eventos.TProcEvento evento = (Eventos.TProcEvento)serializer.Deserialize(stream);
            return evento;
        }

        public static Eventos.TProcEvento LerCTeCancelado(string caminho)
        {
            using (System.IO.Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho))
            {
                Eventos.TProcEvento evento = LerEvento(stream);
                return evento;
            }
        }

        public static ConhecimentoDeTransporteInutilizado.TProcInutCTe LerCTeInutilizado(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConhecimentoDeTransporteInutilizado.TProcInutCTe));
            ConhecimentoDeTransporteInutilizado.TProcInutCTe cteInutilizado = (ConhecimentoDeTransporteInutilizado.TProcInutCTe)serializer.Deserialize(stream);
            return cteInutilizado;
        }

        public static ConhecimentoDeTransporteInutilizado.TProcInutCTe LerCTeInutilizado(string caminho)
        {
            using (System.IO.Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho))
            {
                ConhecimentoDeTransporteInutilizado.TProcInutCTe cteInutilizado = LerCTeInutilizado(stream);
                return cteInutilizado;
            }
        }

        public static ConhecimentoDeTransporte.TCTe LerCTe(string caminho)
        {
            using (System.IO.Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho))
            {
                ConhecimentoDeTransporte.TCTe cte = LerCTe(stream);
                return cte;
            }
        }

        public static ConhecimentoDeTransporte.TCTe LerCTe(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConhecimentoDeTransporte.TCTe));
            ConhecimentoDeTransporte.TCTe cte = (ConhecimentoDeTransporte.TCTe)serializer.Deserialize(stream);
            return cte;
        }
    }
}
