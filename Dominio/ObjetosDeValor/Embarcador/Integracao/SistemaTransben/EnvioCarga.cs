using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben
{
    public class EnvioCarga
    {
        public string numeroDaCarga { get; set; }
        public string dataDeCriacao { get; set; }
        public string tipoDaOperacao { get; set; }
        public string origem { get; set; }
        public string destino { get; set; }
        public string operacao { get; set; }
        public string tomadorCnpj { get; set; }
        public string situacao { get; set; }
        public string tipoDecarga { get; set; }
        public string tipoDeVeiculoSolicitado { get; set; }
        public decimal valorFrete { get; set; }
        public decimal peso { get; set; }
        public decimal valorNF { get; set; }
        public decimal pesoLiquido { get; set; }
        public string Gestor { get; set; }
        public string dataCarregamento { get; set; }
        public bool cargaPerigosa { get; set; }
        public string empresaFilial { get; set; }
        public string placas { get; set; }
        public string frota { get; set; }
        public string motorista { get; set; }
        public string recebedor { get; set; }
        public string rota { get; set; }
        public decimal valorTotalDosProdutos { get; set; }
        public int QuantidadeNfs { get; set; }
        public int volumesNFCaixas { get; set; }
        public List<Pedido> pedidos { get; set; }
    }

    public class Pedido
    {
        public string numeroDaCarga { get; set; }
        public string numeroDoPedido { get; set; }
        public string localPalletizacao { get; set; }
        public string dataAgendamento { get; set; }
        public List<CTe> CTEs { get; set; }
    }

    public class CTe
    {
        public string numeroDoCte { get; set; }
        public string numeroDaCarga { get; set; }
        public string numeroDoPedido { get; set; }
        public int serie { get; set; }
        public string localidadeEmissao { get; set; }
        public string inicioPrestacao { get; set; }
        public string terminoPrestacao { get; set; }
        public string dataEmissao { get; set; }
        public string tipoDeServico { get; set; }
        public string destino { get; set; }
        public decimal valorReceber { get; set; }
        public string chave { get; set; }
        public InformacoesEmpresa remetente { get; set; }
        public InformacoesEmpresa destinatario { get; set; }
        public List<NotaFiscal> NFs { get; set; }
    }

    public class InformacoesEmpresa
    {
        public string cpfCnpj { get; set; }
        public string IE { get; set; }
        public string razaoSocial { get; set; }
        public string nomeFantasia { get; set; }
    }

    public class NotaFiscal
    {
        public string numeroDaCarga { get; set; }
        public string numeroDoCte { get; set; }
        public string numeroDoPedido { get; set; }
        public string numerodanf { get; set; }
        public string origem { get; set; }
        public string destino { get; set; }
        public decimal peso { get; set; }
        public decimal valorNF { get; set; }
        public decimal pesoLiquido { get; set; }
        public int volumes { get; set; }
        public string remetente { get; set; }
        public string destinatario { get; set; }
        public int quantidadePallets { get; set; }
    }
}
