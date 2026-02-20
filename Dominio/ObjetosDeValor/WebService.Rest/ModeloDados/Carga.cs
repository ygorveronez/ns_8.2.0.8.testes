using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Carga
    {
        public int Protocolo { get; set; }

        public string Numero { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataInicioViagem { get; set; }

        public DateTime? DataFimViagem { get; set; }

        public decimal Peso { get; set; }

        public decimal Distancia { get; set; }

        public string Situacao { get; set; }

        public string NumeroDocaCarga { get; set; }

        public string NomeOperador { get; set; }

        public DateTime? DataInteresse { get; set; }

        public DateTime? DataCargaContratada { get; set; }

        public Filial Filial { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public TipoOperacao TipoOperacao { get; set; }

        public ModeloVeicularCarga ModeloVeicular { get; set; }

        public Veiculo Veiculo { get; set; }

        public Empresa Tranportador { get; set; }

        public List<Motorista> Motoristas { get; set; }

        public List<CargaPedido> Pedidos { get; set; }

        public List<Integracao> Integracoes { get; set; }

        public List<ConhecimentoTransporteEletronico> Ctes { get; set; }

        public List<ManifestoEletronicoDocumentosFiscais> Mdfes { get; set; }
    }
}
