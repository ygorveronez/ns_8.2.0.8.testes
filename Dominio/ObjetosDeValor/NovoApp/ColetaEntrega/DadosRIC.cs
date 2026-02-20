
using System;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class DadosRIC
    {
        public long DataDeColeta { get; set; }
        public string Container { get; set; }
        public string TipoContainer { get; set; }
        public int TaraContainer { get; set; }
        public string ArmadorBooking { get; set; }
        public string Transportadora { get; set; }
        public string Motorista { get; set; }
        public string Placa { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Container.CadastroDeContainer ConverterEmObjetoDeValor()
        {
            var obj = new Dominio.ObjetosDeValor.Embarcador.Container.CadastroDeContainer();
            obj.DataDeColeta = Utilidades.DateTime.FromUnixSeconds(DataDeColeta) ?? DateTime.MinValue;
            obj.Container = Container?.ToUpperInvariant();
            obj.TipoContainer = TipoContainer?.ToUpperInvariant();
            obj.TaraContainer = TaraContainer;
            obj.ArmadorBooking = ArmadorBooking;
            obj.Transportadora = Transportadora;
            obj.Motorista = Motorista;
            obj.Placa = Placa?.ToUpperInvariant();
            return obj;
        }
    }
}
