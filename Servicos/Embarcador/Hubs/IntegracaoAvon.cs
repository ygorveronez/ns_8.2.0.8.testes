namespace Servicos.Embarcador.Hubs
{
    public class IntegracaoAvon : HubBase<IntegracaoAvon>
    {
        public void InformarMinutaAtualizada(int codigoCarga, int quantidadeGerada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon situacao)
        {
            var retorno = new
            {
                CodigoCarga = codigoCarga,
                QuantidadeGerada = quantidadeGerada,
                Situacao = (int)situacao
            };

			SendToAll("informarMinutaAtualizada", retorno);
        }
    }
}
