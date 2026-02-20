using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTipoSubareaCliente
    {
        Portaria = 1,
        Patio = 2,
        Estacionamento = 3,
        Balanca = 4,
        Carregamento = 5,
        Descarregamento = 6,
    }

    public static class TipoTipoSubareaClienteHelper
    {
        public static List<TipoTipoSubareaCliente> GetAll()
        {
            List<TipoTipoSubareaCliente> tipos = new List<TipoTipoSubareaCliente>();
            tipos.Add(TipoTipoSubareaCliente.Balanca);
            tipos.Add(TipoTipoSubareaCliente.Carregamento);
            tipos.Add(TipoTipoSubareaCliente.Descarregamento);
            tipos.Add(TipoTipoSubareaCliente.Estacionamento);
            tipos.Add(TipoTipoSubareaCliente.Patio);
            tipos.Add(TipoTipoSubareaCliente.Portaria);
            return tipos;
        }
        public static string ObterDescricao(this TipoTipoSubareaCliente tipo)
        {
            switch (tipo)
            {
                case TipoTipoSubareaCliente.Portaria: return "Portaria";
                case TipoTipoSubareaCliente.Patio: return "Pátio";
                case TipoTipoSubareaCliente.Estacionamento: return "Estacionamento";
                case TipoTipoSubareaCliente.Balanca: return "Balança";
                case TipoTipoSubareaCliente.Carregamento: return "Carregamento";
                case TipoTipoSubareaCliente.Descarregamento: return "Descarregamento";
                default: return string.Empty;
            }
        }
    }

}
