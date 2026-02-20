using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido
{
    public class Pedido
    {
        public List<DadosGeraisPedido> dadosGeraisPedido { get; set; }
        public List<FluxosStatusPedido> fluxosStatusPedido { get; set; }
        public List<DadosQuantidadesPendente> dadosQuantidadesPendentes { get; set; }
        public List<RemessasVinculadasPedidoPesquisado> remessasVinculadasPedidoPesquisado { get; set; }
        public List<FluxoRemessa> fluxoRemessas { get; set; }
        public List<LogsErro> logsErros { get; set; }
    }

    public class DadosGeraisPedido
    {
        public string numOv { get; set; }
        public string codCliente { get; set; }
        public string nomeCliente { get; set; }
        public string codCentro { get; set; }
        public string tpCarregamento { get; set; }
        public string cnpjCliente { get; set; }
    }

    public class DadosQuantidadesPendente
    {
        public string numItem { get; set; }
        public string codMaterial { get; set; }
        public string descMaterial { get; set; }
        public int numQuantPend { get; set; }
        public string unidMedida { get; set; }
    }

    public class FluxoRemessa
    {
        public string numRemessa { get; set; }
        public string codSequencia { get; set; }
        public string codEtapa { get; set; }
        public string codStatus { get; set; }
        public string dataRegistro { get; set; }
        public string horaRegistro { get; set; }
        public string descEtapa { get; set; }
        public string codCorStatus { get; set; }
    }

    public class FluxosStatusPedido
    {
        public string codSequencia { get; set; }
        public string codEtapa { get; set; }
        public string codStatus { get; set; }
        public string dataRegistro { get; set; }
        public string horaRegistro { get; set; }
        public string stAlterado { get; set; }
        public string descEtapa { get; set; }
        public string codCorStatus { get; set; }
    }

    public class LogsErro
    {
        public string descMessagem { get; set; }
        public string tipoMsg { get; set; }
        public string classMsgSap { get; set; }
        public string numMsgSap { get; set; }
        public string msgSap1 { get; set; }
        public string msgSap2 { get; set; }
        public string msgSap3 { get; set; }
        public string msgSap4 { get; set; }
    }

    public class RemessasVinculadasPedidoPesquisado
    {
        public string numRemessa { get; set; }
        public string codItemRemessa { get; set; }
        public string codMaterial { get; set; }
        public string descMaterial { get; set; }
        public int numQuant { get; set; }
        public string unidMedida { get; set; }
        public string nmNotaFiscal { get; set; }
        public string dataRemessa { get; set; }
        public string horaCriaRemessa { get; set; }
    }




}
