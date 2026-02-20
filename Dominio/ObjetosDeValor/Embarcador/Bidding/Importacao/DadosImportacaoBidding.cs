using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.Importacao
{
    public class DadosImportacaoBidding
    {
        public List<Dominio.Entidades.Localidade> Localidades { get; set; }
        public List<Dominio.Entidades.Cliente> Clientes { get; set; }
        public List<Dominio.Entidades.Estado> Estados { get; set; }
        public List<Dominio.Entidades.Embarcador.Localidades.Regiao> Regioes { get; set; }
        public List<Dominio.Entidades.RotaFrete> Rotas { get; set; }
        public List<(string De, string Ate)> FaixasCEP { get; set; }
        public List<Dominio.Entidades.Pais> Paises { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModelosVeiculares { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> TiposCarga { get; set; }
        public List<Dominio.Entidades.Cliente> Tomadores { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> GruposModeloVeicular { get; set; }
        public List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> ModelosCarroceria { get; set; }
        public List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> TiposBaseline { get; set; }
        public List<Dominio.Entidades.Embarcador.Filiais.Filial> FiliaisParticipantes { get; set; }

    }
}
