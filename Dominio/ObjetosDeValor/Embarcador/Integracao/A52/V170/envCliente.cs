using Dominio.ObjetosDeValor.Embarcador.Abastecimento;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envCliente
    {
        public string nomeFantasia { get; set; }

        public string razaoSocial { get; set; }

        public string cnpj { get; set; }

        public string identificador { get; set; }

        public string sigla { get; set; }

        public bool ativo { get; set; }
        
        public List<envClienteEndereco> enderecos { get; set; }
    }

    public class envClienteEndereco
    {
        public int? id { get; set; }

        public string identificador { get; set; }

        public string sigla { get; set; }

        public int? tipo { get; set; }

        public string cep { get; set; }

        public string logradouro { get; set; }

        public string numero { get; set; }

        public string complemento { get; set; }

        public string bairro { get; set; }

        public int? idCidade { get; set; }

        public int? idPais { get; set; }
        public List<object> poligono { get; set; }
        public Raio raio { get; set; }
    }
    public class Raio
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double raio { get; set; }
    }
}