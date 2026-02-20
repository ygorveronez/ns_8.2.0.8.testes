using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Motorista
    {
        public int Codigo { get; set; }
        public string CPF { get; set; }
        public string CategoriaCNH { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Nome { get; set; }
        public string DataHabilitacao { get; set; }
        public string DataAdmissao { get; set; }
        public string DataNascimento { get; set; }
        public string DataValidadeGR { get; set; }
        public Enumeradores.TipoMotorista tipoMotorista { get; set; }
        public Enumeradores.Escolaridade? Escolaridade { get; set; }
        public Enumeradores.EstadoCivil? EstadoCivil { get; set; }
        public string RG { get; set; }
        public string EstadoRG { get; set; }
        public string NumeroHabilitacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios DadosBancarios { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios> ListaDadosBancarios { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MotoristaContato> ListaContatos { get; set; }
        public string Email { get; set; }
        public string DataVencimentoHabilitacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco Endereco { get; set; }
        public bool? Ativo { get; set; }
        public string MotivoBloqueio { get; set; }
        public string DataSuspensaoInicio { get; set; }
        public string DataSuspensaoFim { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Transportador { get; set; }
        public string NumeroCartao { get; set; }
        public string NumeroPISPASEP { get; set; }

        public string DataEmissaoRG { get; set; }
        public Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG? OrgaoEmissorRG { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador? SituacaoColaborador { get; set; }
        public string DataDemissao { get; set; }
        public bool? UtilizaMultiMobile { get; set; }
    }
}
