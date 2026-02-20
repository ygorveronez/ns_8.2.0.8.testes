using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco
{
    public class ConsultaMotorista
    {
        public string cnpjCliente { get; set; }
        public string cpfUsuario { get; set; }
        public string cnpjTransportadora { get; set; }
        public string cnpjEmbarcador { get; set; }
        public Motorista motorista { get; set; }
        public List<Veiculo> veiculos { get; set; }
    }

    public class Motorista
    {
        public string cpf { get; set; }
        public string senhaMot { get; set; }
    }

    public class Veiculo
    {
        public string tipo { get; set; }
        public string placa { get; set; }
        public Proprietario proprietario { get; set; }
    }

    public class Proprietario
    {
        public string tipoPessoa { get; set; }
        public string cpf { get; set; }
        public string cnpj { get; set; }
        public string codigoRntrc { get; set; }
    }
}
