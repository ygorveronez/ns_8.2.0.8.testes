using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class IntegracaoMotoristaDados
    {
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        /// <summary>
        /// Sexo (F ou M)
        /// </summary>
        public string Sexo { get; set; }
        public string RG { get; set; }
        public string OrgaoEmissRG { get; set; }
        public DateTime? DataEmissRG { get; set; }
        public int? CodProfissao { get; set; }
        public int? NumFormCNH { get; set; }
        public int? NumRegCNH { get; set; }
        public string NumSegurCNH { get; set; }
        public string NumRenachCNH { get; set; }
        public string UFEmissCNH { get; set; }
        public DateTime? DataEmissCNH { get; set; }
        public DateTime? DataVencCNH { get; set; }
        public string CategoriaCNH { get; set; }
        public DateTime? DtPrimEmissCNH { get; set; }
        /// <summary>
        /// Possuí MOPP (curso transporte produtos perigosos) (S ou N) 
        /// </summary>
        public string PossuiMOPP { get; set; }
        public DateTime? DtVencMOPP { get; set; }

        public DateTime? DataAdmissao { get; set; }
        public int? CodIBGECidadeNatal { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string NomePai { get; set; }
        public string NomeMae { get; set; }
        /// <summary>
        /// Estado civil (1-Amasiado 2-Casado 3-Desquitado 4-Divorciado 5-Separado 6-Solteiro 7-Viuvo)
        /// </summary>
        public string EstadoCivil { get; set; }
        public string NomeConjuge { get; set; }
        /// <summary>
        /// Situação da residência (A-Alugada F-Financiada P-Própria) 
        /// </summary>
        public string SituacaoResid { get; set; }
        public int? TempoResid { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public int? CodIBGECidade { get; set; }
        public string CEP { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Radio { get; set; }
        public string SenhaMotorista { get; set; }
    }
}
