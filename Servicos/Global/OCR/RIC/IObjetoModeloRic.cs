using System.Collections.Generic;

namespace Servicos.Global
{
    public interface IObjetoModeloRic
    {
        List<string> DataDeColeta_ModeloOCR { get; }
        List<string> Container_ModeloOCR { get; }
        List<string> TipoContainer_ModeloOCR { get; }
        List<string> TaraContainer_ModeloOCR { get; }
        List<string> ArmadorBooking_ModeloOCR { get; }
        List<string> Transportadora_ModeloOCR { get; }
        List<string> Motorista_ModeloOCR { get; }
        List<string> Placa_ModeloOCR { get; }

        /// <summary>
        /// Strings que identificam o modelo de cupom RIC
        /// </summary>
        List<string> IdentificadorDoModeloOCR { get; }

        string DataDeColeta { get; set; }
        string Container { get; set; }
        string TipoContainer { get; set; }
        string TaraContainer { get; set; }
        string ArmadorBooking { get; set; }
        string Transportadora { get; set; }
        string Motorista { get; set; }
        string Placa { get; set; }

    }
}
