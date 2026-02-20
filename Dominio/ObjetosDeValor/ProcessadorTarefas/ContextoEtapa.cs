using Dominio.Entidades.ProcessadorTarefas;

namespace Dominio.ObjetosDeValor.ProcessadorTarefas
{
    public class ContextoEtapa
    {
        public string TarefaId { get; set; }

        public RequestDocumento RequestDoc { get; set; }

        public ProcessamentoTarefa Tarefa { get; set; }

        public int IndiceEtapa { get; set; }
    }
}

