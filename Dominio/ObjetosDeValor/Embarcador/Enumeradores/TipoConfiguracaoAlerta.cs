namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoConfiguracaoAlerta
    {
        ApoliceSeguro = 1,
        CertificadoDigital = 2,
        RotaNaoCadastrada = 3,
        PedidoSemTabelaFrete = 4,
        Antt = 5,
        Cnh = 6,
        MDFEPendenteDeEncerramento = 7,
        RegraICMS = 8,
        PendenciaNfsManual = 9
    }

    public static class TipoConfiguracaoAlertaHelper
    {
        public static string ObterDescricao(this TipoConfiguracaoAlerta tipo)
        {
            switch (tipo)
            {
                case TipoConfiguracaoAlerta.Antt: return "ANTT";
                case TipoConfiguracaoAlerta.ApoliceSeguro: return "Apólice de Seguro";
                case TipoConfiguracaoAlerta.CertificadoDigital: return "Certificado Digital";
                case TipoConfiguracaoAlerta.Cnh: return "CNH";
                case TipoConfiguracaoAlerta.RotaNaoCadastrada: return "Rota não Cadastrada";
                case TipoConfiguracaoAlerta.PedidoSemTabelaFrete: return "Pedido sem tabela de frete";
                case TipoConfiguracaoAlerta.MDFEPendenteDeEncerramento: return "MDF-e pendente de Encerramento";
                case TipoConfiguracaoAlerta.RegraICMS: return "Regra ICMS";
                case TipoConfiguracaoAlerta.PendenciaNfsManual: return "Pendência NFS Manual";
                default: return string.Empty;
            }
        }
    }
}

