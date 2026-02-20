using System;

namespace Dominio.ObjetosDeValor.OCR
{
    public class ObjetoRicRetorno
    {
        public string DataDeColeta { get; set; }
        public string Container { get; set; }
        public string TipoContainer { get; set; }
        public int TaraContainer { get; set; }
        public string ArmadorBooking { get; set; }
        public string Transportadora { get; set; }
        public string Motorista { get; set; }
        public string Placa { get; set; }

        public string Erro { get; set; }
        //*************

        public int CodigoContainer { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Container.CadastroDeContainer ConverterEmObjetoDeValor()
        {
            var obj = new Dominio.ObjetosDeValor.Embarcador.Container.CadastroDeContainer();
            DateTime dateTime = DateTime.MinValue;

            if (!DateTime.TryParseExact(DataDeColeta, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dateTime))
                if (!DateTime.TryParseExact(DataDeColeta, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateTime))
                    DateTime.TryParseExact(DataDeColeta, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dateTime);

            if (dateTime == DateTime.MinValue)
                dateTime = DateTime.UtcNow;

            obj.DataDeColeta = dateTime;
            obj.Container = Container;
            obj.TipoContainer = TipoContainer;
            obj.TaraContainer = TaraContainer;
            obj.ArmadorBooking = ArmadorBooking;
            obj.Transportadora = Transportadora;
            obj.Motorista = Motorista;
            obj.Placa = Placa;
            return obj;
        }
    }
}
