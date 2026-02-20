using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb
{
    public class envMotorista
    {
        public string Nome { get; set; }
        public string CPF_CNPJ { get; set; }
        public string CodigoExterno { get; set; }
        public bool? Condutor { get; set; }
        public int? Vinculo { get; set; }
        public string Cidade { get; set; }
        public int? UF { get; set; }
        public string RegistroCNH { get; set; }
        public string Categoria { get; set; }
        public string ValidadeCnh { get; set; }
        public string DataAdmissao { get; set; }
        public string Periodico { get; set; }
        public string Demissao { get; set; }
        public string Pan { get; set; }
        public string Pamcary { get; set; }
        public string NivelEscolar { get; set; }
        public string Email { get; set; }
        public string Matricula { get; set; }
        public envComplemento Complemento { get; set; }
        public envFisicaComplemento FisicaComplemento { get; set; }
        public envEndereco Endereco { get; set; }
        public envParticipante Empresa { get; set; }
    }

    public class envComplemento
    {
        public string Telefone { get; set; }
    }

    public class envFisicaComplemento
    {
        public string DataNascimento { get; set; }
        public string Filiacao { get; set; }
        public string RG { get; set; }
        public string OrgaoExpedidor { get; set; }
    }

    public class envEndereco
    {
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
    }

    public class envJuridicaComplemento
    {
        public string InscricaoEstadual { get; set; }
        public string NomeFantasia { get; set; }
    }
}
