using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPessoas" in both code and config file together.
    [ServiceContract]
    public interface IPessoas
    {
        [OperationContract]
        Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarPessoas(int? inicio, int? limite, bool? consultarApenasAtualizados);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoPessoa(string cnpj);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadoresTerceiro(int? inicio, int? limite, bool? consultarApenasAtualizados);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTransportadoresTerceiro(string cnpj);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Container>> BuscarContainer(int? inicio, int? limite, bool? consultarApenasAtualizados);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoContainer(string codigoIntegracao);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade>> BuscarRamosDeAtividade(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>> BuscarGrupoPessoas(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo>> BuscarVeiculos(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista>> BuscarMotoristas(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> SalvarCliente(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa clienteIntegracao);

        [OperationContract]
        Retorno<bool> SalvarClienteComplementar(Dominio.ObjetosDeValor.Embarcador.Pessoas.PessoaComplementar clienteComplementarIntegracao);

        [OperationContract]
        Retorno<bool> SalvarContainer(Dominio.ObjetosDeValor.Embarcador.Carga.Container containerIntegracao);

        [OperationContract]
        Retorno<bool> SalvarProdutoEmbarcador(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao);

        [OperationContract]
        Retorno<bool> SalvarNavio(Dominio.ObjetosDeValor.Embarcador.Carga.Navio navioIntegracao);

        [OperationContract]
        Retorno<bool> SalvarViagem(Dominio.ObjetosDeValor.Embarcador.Carga.Viagem viagemIntegracao);

        [OperationContract]
        Retorno<bool> SalvarLocalidade(Dominio.ObjetosDeValor.Localidade localidade);

        [OperationContract]
        Retorno<bool> SalvarConfiguracaoFatura(Dominio.ObjetosDeValor.Embarcador.Carga.ConfiguracaoFatura configuracaoFatura);

        [OperationContract]
        Retorno<bool> SalvarAbastecimento(Dominio.ObjetosDeValor.Embarcador.Carga.Abastecimento abastecimento);

        [OperationContract]
        Retorno<bool> SalvarGrupoPessoa(Dominio.ObjetosDeValor.Embarcador.Carga.GrupoPessoa grupoPessoaIntegracao);

        [OperationContract]
        Retorno<int> BuscarProtocoloMotorista(string ddd, string telefone);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Protocolo>> BuscarCargaMotorista(int protocoloMotorista);

        [OperationContract]
        Retorno<bool> SalvarUsuario(Dominio.ObjetosDeValor.WebService.Usuario.UsuarioIntegracao usuario);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno>> ObterUsuarios(int? inicio, int? limite);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Usuario.DetalheUsuario> ObterDadosUsuario(string codigoIntegracao);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarClientesPendentesIntegracao(int? quantidade);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoPessoaERP(List<long> listaProtocolos);

    }
}
