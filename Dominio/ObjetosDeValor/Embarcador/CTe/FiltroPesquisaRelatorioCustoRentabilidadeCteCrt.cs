using Dominio.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe;

public sealed class FiltroPesquisaRelatorioCustoRentabilidadeCteCrt
{
    public DateTime? DataInicialEmissao { get; set; }
    public DateTime? DataFinalEmissao { get; set; }

    public int NumeroInicial { get; set; }
    public int NumeroFinal { get; set; }

    public string Pedido { get; set; }
    public string NotaFiscal { get; set; }
    public int Serie { get; set; }
    public List<TipoServico> TipoServico { get; set; }

    public List<string> Situacao { get; set; }
    public bool? CTeVinculadoACarga { get; set; }


    public double CpfCnpjRemetente { get; set; }
    public List<double> CpfCnpjDestinatarios { get; set; }
    public List<double> CpfCnpjTomadores { get; set; }

    public List<int> CodigosVeiculo { get; set; }
    public List<int> CodigosCarga { get; set; }
    public int CodigoFilial { get; set; }

    public List<int> CodigosTransportador { get; set; }
    public List<int> CodigosTipoOperacao { get; set; }
    public List<int> CodigosCTe { get; set; }

    public List<int> CodigosModeloDocumento { get; set; }
}
