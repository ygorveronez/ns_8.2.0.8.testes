using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoDados
    {
        #region Propriedades

        public int CodigoCarga { get; set; }
        
        public int CodigoCentroCarregamento { get; set; }

        public string DescricaoCentroCarregamento { get; set; }

        public bool Excedente { get; set; }

        public DateTime InicioCarregamento { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string DataCarregamentoFormatada
        {
            get
            {
                return Excedente ? string.Empty : InicioCarregamento.ToDateTimeString();
            }
        }

        #endregion Propriedades com Regras
    }
}
