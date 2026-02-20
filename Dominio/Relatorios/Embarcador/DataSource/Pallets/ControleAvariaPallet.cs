using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class ControleAvariaPallet
    {
        public int Codigo { get; set; }

        public DateTime Data { get; set; }

        public string Filial { get; set; }

        public string FilialCnpj { get; set; }

        public string FilialCodigoIntegracao { get; set; }

        public string MotivoAvaria { get; set; }

        public int Numero { get; set; }

        public string Observacao { get; set; }

        public int Quantidade { get; set; }

        public string Setor { get; set; }

        public string Situacao { get; set; }

        public string Transportador { get; set; }

        public string TransportadorCnpj { get; set; }

        public string TransportadorCodigoIntegracao { get; set; }
    }
}
