using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class MonitoramentoTratativaAlerta
    {
         private string OberDataFormatada(DateTime data)
         {
              if (data != DateTime.MinValue)
                  return data.ToString("dd/MM/yyyy HH:mm");

             return "";
         }

         public int Codigo { get; set;}
         public string PlacaVeiculo { get; set; }
         public string Carga { get; set; }
         public string Usuario { get; set; }
         public DateTime DataAlerta { get; set; }

         public string DataAlertaFormatado
         {
             get { return OberDataFormatada(DataAlerta); }
         }
         public DateTime DataTratativa { get; set; }
         public string DataTratativaFormatada
         {
             get { return OberDataFormatada(DataTratativa); }
         }

         public string Tratativa { get; set; }
         public string Observacao { get; set; }

         public string TempoTratativa
         {
            get
            {
                if (DataTratativa != DateTime.MinValue && DataAlerta != DateTime.MinValue) {
                    TimeSpan data = DataTratativa.Subtract(DataAlerta);

                    return string.Format("{0}:{1}:{2}", data.TotalHours.ToString("00"), data.Minutes.ToString("00"), data.Seconds.ToString("00"));
                }

                return "";

            }
         }

    }
}
