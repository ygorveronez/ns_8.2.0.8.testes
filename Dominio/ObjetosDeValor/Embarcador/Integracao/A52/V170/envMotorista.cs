using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envMotorista
    {
        public string name { get; set; }

        public int? vinculo { get; set; }

        public bool ativo { get; set; }

        public envMotoristaDocumento documentos { get; set; }

        public string telefone { get; set; }

        public string matricula { get; set; }

        public string celular { get; set; }

        public int? idCargo { get; set; }

        public string dataNascimento { get; set; }

        public string dataAdmissao { get; set; }

        public string dataDesligamento { get; set; }

        public bool cursoMopp { get; set; }

        public string validadeMopp { get; set; }

        public string dataValidadeToxicologico { get; set; }

        public List<envMotoristaEndereco> enderecos { get; set; }
    }

    public class envMotoristaDocumento
    {
        public string cpf { get; set; }

        public string cnh { get; set; }

        public string dataRenovacaoCnh { get; set; }

        public string rg { get; set; }

        public string pis { get; set; }

        public string ctps { get; set; }
    }

    public class envMotoristaEndereco
    {
        public string logradouro { get; set; }

        public string cep { get; set; }

        public string numero { get; set; }

        public string bairro { get; set; }

        public string complemento { get; set; }

        public int? idCidade { get; set; }
    }
}