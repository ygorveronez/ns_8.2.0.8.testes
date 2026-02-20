using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class PlanejamentoVolumeDestinatario
    {
        public int CodigoPlanejamento { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public string NomeDestinatario { get; set; }

        public string CPFCNPJFormatado
        {
            get
            {
                if (CPFCNPJDestinatario != 0)
                {
                    string cpfCnpj = CPFCNPJDestinatario.ToString("F0");

                    if (cpfCnpj.Length > 11)
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(cpfCnpj));
                    else
                        return string.Format(@"{0:000\.000\.000\-00}", long.Parse(cpfCnpj));
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
