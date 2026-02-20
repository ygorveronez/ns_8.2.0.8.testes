using System;

namespace Dominio.ObjetosDeValor.EDI.PROV
{
    public class Provisao
    {
        public string Evento { get; set; }

        public string CodigoEvento { get; set; }

        public string CodigoVersao { get; set; }

        public string IdentificadorSistema { get; set; }

        public DateTime DataExtracao { get; set; }

        public string Matriz { get; set; }

        public string Filial { get; set; }

        public DateTime DataContabil { get; set; }

        public string Departamento { get; set; }

        public string CreditoDebito { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal ValorProvisao { get; set; }

        public string CodigoTransacao { get; set; }

        public string DescricaoTransacao { get; set; }
    }
}