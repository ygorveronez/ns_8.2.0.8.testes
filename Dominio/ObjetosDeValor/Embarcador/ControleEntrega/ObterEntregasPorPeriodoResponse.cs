using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.ControleEntrega
{
    public class ObterEntregasPorPeriodoResponse
    {
        public string numero_carga { get; set; }
        public DateTime? previsao_chegada_inicio { get; set; }
        public DateTime? previsa_chegada_atualizada { get; set; }
        public DateTime? data_carregamento { get; set; }
        public string tendencia_atraso { get; set; }
        public List<string> placas { get; set; }
        public List<string> pacotes { get; set; }
        public string modelo_veiculo { get; set; }
        public int total_pacotes { get; set; }
        public string nome_origem { get; set; }
        public string codigo_origem { get; set; }
        public string nome_transportadora { get; set; }
        public string nome_motorista { get; set; }
        public string cpf_motorista { get; set; }
    }
}
