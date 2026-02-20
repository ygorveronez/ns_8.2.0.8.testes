namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Carga
{
    public sealed class Motorista
    {
        public int Codigo { get; set; }

        public string CPF { get; set; }

        public string CodigoIntegracao { get; set; }

        public string Nome { get; set; }

        public string DataHabilitacao { get; set; }

        public string DataAdmissao { get; set; }

        public string DataNascimento { get; set; }

        public string DataValidadeGR { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista { get; set; }

        public string RG { get; set; }

        public string NumeroHabilitacao { get; set; }

        public Pessoas.DadosBancarios DadosBancarios { get; set; }

        public string Email { get; set; }

        public string DataVencimentoHabilitacao { get; set; }

        public Localidade.Endereco Endereco { get; set; }

        public bool? Ativo { get; set; }

        public string MotivoBloqueio { get; set; }

        public string DataSuspensaoInicio { get; set; }

        public string DataSuspensaoFim { get; set; }

        public Pessoas.Empresa Transportador { get; set; }
    }
}
