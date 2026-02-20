using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DataBaseAlerta
    {
        PrevisaoEntrega = 0,
        DataAgendamento = 1,
    }

    public static class DataBaseAlertaHelper
    {
        public static DataBaseAlerta ObterTipoDeAlertaPorDescricao(string Descricao)
        {
            foreach (int i in Enum.GetValues(typeof(DataBaseAlerta)))
            {
                var tipo = (DataBaseAlerta)Enum.ToObject(typeof(DataBaseAlerta), i);
                if (DataBaseAlertaHelper.ObterDescricao(tipo) == Descricao)
                    return tipo;
            }

            return DataBaseAlerta.PrevisaoEntrega;
        }

        public static string ObterDescricao(this DataBaseAlerta tipo)
        {
            switch (tipo)
            {
                case DataBaseAlerta.PrevisaoEntrega:
                    return "Previsão de Entrega";
                case DataBaseAlerta.DataAgendamento:
                    return "Data de Agendamento";
                default:
                    return "Previsão de Entrega";
            }
        }
    }
}
