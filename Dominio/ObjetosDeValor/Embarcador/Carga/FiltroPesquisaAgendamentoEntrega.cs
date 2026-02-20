using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaAgendamentoEntrega
    {
        public double CpfCnpjDestinatario { get; set; }

        public double CpfCnpjRemetente { get; set; }

        public string Placa { get; set; }

        public string Senha { get; set; }

        public int SenhaAgendamento { get; set; }

        public SituacaoAgendamentoEntrega? Situacao { get; set; }
    }
}
