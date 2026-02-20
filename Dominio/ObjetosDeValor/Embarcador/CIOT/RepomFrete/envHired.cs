using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class envHired
    {
        public string Country { get; set; }
        public string HiredType { get; set; }
        public string NationalId { get; set; }
        public string Email { get; set; }
        public List<Phones> Phones { get; set; }
        public HiredBrazilianSettings BrazilianSettings { get; set; }
        public Address Address { get; set; }
        public CompanyInformation CompanyInformation { get; set; }
        public HiredPersonalInformation HiredPersonalInformation { get; set; }
        public string FuelVoucherPercentage { get; set; }
    }

    public class Phones
    {
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public bool Preferential { get; set; }
        public string TypeId { get; set; }
    }

    public class HiredBrazilianSettings
    {
        public string RNTRC { get; set; }
        public HiredPessoaFisica HiredPessoaFisica { get; set; }
        public HiredPessoaJuridica HiredPessoaJuridica { get; set; }
    }

    public class HiredPessoaFisica
    {
        public string INSS { get; set; }
        public string RG { get; set; }
    }

    public class HiredPessoaJuridica
    {
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string NomeFantasia { get; set; }
        public bool OptanteSimplesNacional { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Neighborhood { get; set; }
        public string ZipCode { get; set; }
    }

    public class CompanyInformation
    {
        public string CompanyName { get; set; }
    }

    public class HiredPersonalInformation
    {
        public string Name { get; set; }
        public string BirthDate { get; set; }
        public int? LegalDependents { get; set; }
        public string Gender { get; set; }
    }
}