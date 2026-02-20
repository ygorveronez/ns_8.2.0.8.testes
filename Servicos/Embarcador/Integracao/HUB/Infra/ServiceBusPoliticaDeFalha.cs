using Azure.Messaging.ServiceBus;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.HUB.Base
{
    public static class ServiceBusPoliticaDeFalha
    {
        public static EnumFalhaMensageria Decidir(Exception ex) =>
            ex switch
            {
                ArgumentNullException => EnumFalhaMensageria.DeadLetter,
                ArgumentException => EnumFalhaMensageria.DeadLetter,
                JsonException => EnumFalhaMensageria.DeadLetter,

                ServiceBusException sbEx when sbEx.IsTransient => EnumFalhaMensageria.Abandon,

                _ => EnumFalhaMensageria.Abandon
            };
    }
}
    