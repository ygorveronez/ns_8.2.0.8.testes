using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class AditivoContratoFreteTransportador
    {
        public string OrdinalAditivo { get; set; }

        public string ContratanteRazaoSocial { get; set; }
        public string ContratanteCPFCNPJ { get; set; }
        public string ContratanteNaturezaJuridica { get; set; }
        public string ContratanteEndereco { get; set; }
        public string ContratanteRepresentantesLegais { get; set; }
        public string ContratanteCidade { get; set; }
        public string ContratanteEstado { get; set; }
        public string ContratanteCEP { get; set; }
        public string ContratantePais { get; set; }


        public string ContratadoRazaoSocial { get; set; }
        public string ContratadoCPFCNPJ { get; set; }
        public string ContratadoNaturezaJuridica { get; set; }
        public string ContratadoEndereco { get; set; }
        public string ContratadoRepresentantesLegais { get; set; }
        public string ContratadoCidade { get; set; }
        public string ContratadoEstado { get; set; }
        public string ContratadoCEP { get; set; }
        public string ContratadoPais { get; set; }

        public DateTime DataContrato { get; set; }
        public string PrazoContrato { get; set; }

        public string DataAssinatura { get; set; }
        public string LocalAssinatura { get; set; }
    }
}
