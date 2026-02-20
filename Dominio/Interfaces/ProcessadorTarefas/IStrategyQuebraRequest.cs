using Dominio.Entidades.ProcessadorTarefas;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using System.Collections.Generic;

namespace Dominio.Interfaces.ProcessadorTarefas;

public interface IStrategyQuebraRequest
{
    List<RequestSubtarefa> QuebrarRequestTarefa(ContextoEtapa contexto);
}
