using Dominio.Entidades.Embarcador.Cargas;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.GerarAgrupamento
{

    public class DadosValidados
    {
        public TipoDeCarga TipoCarga { get; set; }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public Dominio.Entidades.Empresa EmpresaIntegradora { get; set; }

        public Dominio.Entidades.Veiculo Veiculo { get; set; }

        public List<Dominio.Entidades.Veiculo> Reboques { get; set; }

        public Dominio.Entidades.Usuario Motorista { get; set; }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }
    }
}
