
namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class Endereco
    {
        #region Propriedades

        public string Logradouro { get; set; }

        public string Bairro { get; set; }

        public string Numero { get; set; }

        public string Cep { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string LatitudeTransbordo { get; set; }

        public string LongitudeTransbordo { get; set; }

        public Localidade Localidade { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public bool LatitudelongitudeInformada
        {
            get
            {
                return !string.IsNullOrEmpty(Latitude) && !string.IsNullOrEmpty(Longitude);
            }
        }

        #endregion Propriedades com Regras
    }
}
