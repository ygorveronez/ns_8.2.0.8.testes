using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class parametersEntregas
    {
        public string identificador { get; set; }
        public int tipo { get; set; }
        public int publica { get; set; }
        public string agencia { get; set; }
        public decimal peso_bruto { get; set; }
        public decimal peso_bruto_min { get; set; }
        public decimal peso_bruto_max { get; set; }
        public string[] tipo_carreta { get; set; }
        public string[] tipo_carroceria { get; set; }
        public decimal valor { get; set; }
        public int valor_por { get; set; }
        public string observacao { get; set; }
        public string data_carregamento { get; set; }
        public int proposta_consolidada_id { get; set; }
        public string cpf_motorista { get; set; }
        public string transportadora { get; set; }
        public int pedagio_pago_embarcador { get; set; }
        public int excluir { get; set; }
        public EnderecoOrigem origem { get; set; }
        public List<EnderecoEntrega> entregas { get; set; }
        public decimal liberar_leilao { get; set; }
        public decimal adiantamento { get; set; }
        public decimal adiantamento_min { get; set; }
        public decimal adiantamento_max { get; set; }
        //public Leilao leilao { get; set; }
        //public Adicionais campos_adicionais { get; set; }
    }
}
