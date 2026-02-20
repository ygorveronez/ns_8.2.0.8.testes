namespace Google.OrTools.Api.Services.GoogleOrTools
{
    public class Pedido
    {
        public int Codigo { get; set; }

        public int CodigoCanalEntrega { get; set; }

        public string CanalEntreaga { get; set; }

        /// <summary>
        /// Quantidade máxima de entregas do mesmo canal de entrega no mesmo veiculo.
        /// </summary>
        public int LimiteCanalEntrega { get; set; }

        /// <summary>
        /// Utilizado para "Priorizar" o pedido... adicionando a penalidade do mesmo..
        /// </summary>
        public int Prioridade { get; set; }
    }
}