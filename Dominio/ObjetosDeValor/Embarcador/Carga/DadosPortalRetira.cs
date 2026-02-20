using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class DadosPortalRetira
    {
        public string NomeTransportadora { get; set; }

        public Dominio.Entidades.Empresa Transportador { get; set; }

        public string PlacaVeiculo { get; set; }

        public string EmailNotificacao { get; set; }

        public DateTime DataCarregamentoCarga { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        public Dominio.Entidades.Usuario Motorista { get; set; }

        public Dominio.Entidades.Veiculo Veiculo { get; set; }

        public List<DadosPortalRetiraPedido> Pedidos { get; set; }

        public string CPFMotorista { get; set; }

        public string NomeMotorista { get; set; }

        public string ObservacaoTransportador { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                NomeTransportadora = Transportador?.NomeFantasia ?? string.Empty,
                PlacaVeiculo,
                EmailNotificacao,
                ObservacaoTransportador,
                DataCarregamentoCarga = DataCarregamentoCarga.ToDateTimeString(),
                ModeloVeicularCarga = ModeloVeicularCarga?.Codigo ?? 0,
                TipoOperacao = TipoOperacao?.Codigo ?? 0,
                Filial = Filial?.Codigo ?? 0,
                Motorista = Motorista?.Codigo ?? 0,
                Veiculo = Veiculo?.Codigo ?? 0,
                Transportador = Transportador?.Codigo ?? 0,
                Pedidos
            });
        }
    }
}
