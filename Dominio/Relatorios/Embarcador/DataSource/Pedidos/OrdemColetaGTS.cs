using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class OrdemColetaGTS
    {
        // Cabeçalho
        public string Numero { get; set; }

        public string SenhaCarregamento { get; set; }

        public string RemessaOTM { get; set; }

        public string CodigoCD { get; set; }

        public DateTime Data { get; set; }

        public string Remetente { get; set; }
        
        // Fornecedor
        public string CodigoFornecedor { get; set; }

        public string Destinatario { get; set; }

        public string Endereco { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Contato { get; set; }

        // Informacoes
        public string Transportadora { get; set; }

        public string Cavalo { get; set; }

        public string Carreta { get; set; }

        public string Motorista { get; set; }

        public string Operacao{ get; set; }

        public decimal Peso { get; set; }

        public decimal Valor { get; set; }

        public int QuantidadePallet { get; set; }

        public string NumeroLacre { get; set; }

        // Pedido
        public string Pedido { get; set; }

        public string NumeroPedido { get; set; }

        // Veiculo
        public string TemperaturaVeiculo{ get; set; }

        // Documentos
        public string NotaFiscal { get; set; }

        // Geral
        public string Observacoes { get; set; }

        public string SenhaAgendamento { get; set; }

        public string DataEntrega { get; set; }

        public string DataColeta { get; set; }

        public string Orientacao { get; set; }

        public string ContatosGTS { get; set; }

        public string Buonny { get; set; }

        public string Emitente { get; set; }

        public string OrdemGTS { get; set; }

        public string Normatizacao { get; set; }

        public DateTime Impressao { get; set; }
    }
}
