namespace Servicos.Embarcador.Hubs
{
    public class FluxoColetaEntrega : HubBase<FluxoColetaEntrega>
    {
        public void InformarFluxoColetaEntregaAtualizado(int codigoFluxoColetaEntrega, string stringConexao, Dominio.Entidades.Usuario usuarioAtualizouFluxoColetaEntrega = null)
        {
            var retorno = new
            {
                CodigoFluxoColetaEntrega = codigoFluxoColetaEntrega
            };

            if (usuarioAtualizouFluxoColetaEntrega != null)
                SendToClient(usuarioAtualizouFluxoColetaEntrega.Codigo.ToString(), "informarFluxoColetaEntregaAtualizada", retorno);
            else
                SendToAll("informarFluxoColetaEntregaAtualizada", retorno);
        }
    }
}
