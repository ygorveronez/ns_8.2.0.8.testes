using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote
{
    public class Protocolo
    {
        public virtual string NumeroProtocolo { get; set; }

        public virtual string Filial { get; set; }

        public virtual string Transportadora { get; set; }

        public virtual DateTime DataEnvio { get; set; }

        public virtual int QuantidadeDocumentos { get; set; }

        public virtual string AosCuidado { get; set; }

        public virtual string NomeEnviando { get; set; }

        public virtual string NomeRecebedor { get; set; }
    }
}
