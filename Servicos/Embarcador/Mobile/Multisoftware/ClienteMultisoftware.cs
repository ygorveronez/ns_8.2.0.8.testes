namespace Servicos.Embarcador.Mobile.Multisoftware
{
    public class ClienteMultisoftware
    {
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Multisoftware.ClienteMultisoftware ConverterClienteMultisoftware(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Dominio.ObjetosDeValor.Embarcador.Mobile.Multisoftware.ClienteMultisoftware clienteMultisoftware = new Dominio.ObjetosDeValor.Embarcador.Mobile.Multisoftware.ClienteMultisoftware();
            clienteMultisoftware.CodigoIntegracao = cliente.Codigo;
            clienteMultisoftware.Nome = cliente.RazaoSocial;
            return clienteMultisoftware;
        }
    }
}
