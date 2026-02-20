using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class PlanejamentoVolumeRemetente
    {
        public int CodigoPlanejamento { get; set; }
        public double CPFCNPJRemetente { get; set; }
        public string NomeRemetente { get; set; }

        public string CPFCNPJFormatado
        {
            get
            {
                if (CPFCNPJRemetente != 0)
                {
                    string cpfCnpj = CPFCNPJRemetente.ToString("F0");

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
