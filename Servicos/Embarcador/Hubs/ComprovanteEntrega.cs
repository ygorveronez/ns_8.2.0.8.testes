namespace Servicos.Embarcador.Hubs
{
    public class ComprovanteEntrega : HubBase<ComprovanteEntrega>
    {
        public void InformarComprovantesEntregaStatus(int CodigoLoteComprovante, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var retorno = new
            {
                Codigo = CodigoLoteComprovante,
                Situacao = situacao
            };

            SendToAll("informarComprovantesEntregaStatus", retorno);
        }
    }
}
