namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModeloEmailAgendamentoPedido
    {
        Modelo1 = 1,
        Modelo2 = 2,
    }

    public static class ModeloEmailAgendamentoPedidoHelper
    {
        public static string ObterDescricao(this ModeloEmailAgendamentoPedido modeloEmail)
        {
            switch(modeloEmail)
            {
                case ModeloEmailAgendamentoPedido.Modelo1: return "Modelo 1";
                case ModeloEmailAgendamentoPedido.Modelo2: return "Modelo 2";
                default: return string.Empty;
            }
        }

        //public static string ObterDescricao(int modeloEmail)
        //{
        //    switch (modeloEmail)
        //    {
        //        case ((int)ModeloEmailAgendamentoPedido.Modelo1): return "Modelo 1";
        //        case ((int)ModeloEmailAgendamentoPedido.Modelo2): return "Modelo 2";
        //        default: return string.Empty;
        //    }
        //}
    }
}
