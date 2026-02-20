using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WSValidacaoCommerce
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IValidacao" in both code and config file together.
    [ServiceContract]
    public interface IValidacao
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ExisteCliente/{cnpjCpf}")]
        bool ExisteCliente(string cnpjCpf);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool InsereDadosCliente(string cnpjCpf, string fantasia, DateTime data);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool AtualizarDataVencimentoBloqueio(string cnpjCpf, DateTime dataVencimento, DateTime dataBloqueio);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        DateTime? RetornaDataVencimento(string cnpjCpf);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        DateTime? RetornaDataBloqueio(string cnpjCpf);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool BloquearCliente(string cnpjCpf);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool DesbloquearCliente(string cnpjCpf);

        [OperationContract]
        DateTime? RetornaDataAtual();

        [OperationContract]
        System.Threading.Tasks.Task<bool> CadastrarClienteWooCommerceAsync(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa clienteIntegracao, string url, string key, string secret);

        [OperationContract]
        System.Threading.Tasks.Task<bool> CadastrarProdutoWooCommerceAsync(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao, string url, string key, string secret);

        [OperationContract]
        System.Threading.Tasks.Task<List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido>> BuscarPedidosWooCommerceAsync(string url, string key, string secret, bool atualizarParaEmProcessamento);

        [OperationContract]
        List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido> BuscarPedidosTrayAsync(string url, string consumer_key, string consumer_secret, string code, bool atualizarParaEmProcessamento, string status, string statusProcessamento);

        [OperationContract]
        System.Threading.Tasks.Task<bool> ConfirmarPedidoWooCommerceAsync(string url, string key, string secret, string codigoPedido);

        [OperationContract]
        System.Threading.Tasks.Task<string> CadastrarProdutoTrayAsync(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao, string url, string consumer_key, string consumer_secret, string code);

        [OperationContract]
        System.Threading.Tasks.Task<bool> ConfirmarPedidoTrayAsync(string url, string consumer_key, string consumer_secret, string code, string codigoPedido, string statusProcessamento);
    }
}
