using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class MonitoramentoPosicaoDaFrota
    {
        public Int64 Codigo { get; set; }

        public string PlacaVeiculo { get; set; }

        public string TipoVeiculo { get; set; }

        public string TipoBau { get; set; }

        public int Status { get; set; }

        public string StatusDescricao { get {

                if (Enum.IsDefined(typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao), Status))
                {

                    var status = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao)Status;
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicaoHelper.ObterDescricao(status);
                }

                return "";
                

            }
        }

        public string Posicao { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LatLng
        {
            get
            {

                return $"{Latitude} , {Longitude}";
            }
        }

        public string Regiao { get; set; }

        public string TipoDeTransporte { get; set; }

        public string Transportador { get; set; }
   
    }
        
}
