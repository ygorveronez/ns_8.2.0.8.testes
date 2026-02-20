namespace Servicos.Embarcador.Localidades
{
    public class Endereco : ServicoBase
    {        
        public Endereco(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco ConverterObjetoEnderecoPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco)
        {
            if (pedidoEndereco == null)
                return null;

            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidade();
            Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            endereco.Bairro = pedidoEndereco.Bairro;
            endereco.CEP = pedidoEndereco.CEP;
            endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(pedidoEndereco.Localidade);
            
            endereco.Complemento = pedidoEndereco.Complemento;
            endereco.Telefone = pedidoEndereco.Telefone;
            endereco.InscricaoEstadual = pedidoEndereco.IE_RG;
            endereco.Logradouro = pedidoEndereco.Endereco;
            endereco.Numero = pedidoEndereco.Numero;

            return endereco;
        }

    }
}
