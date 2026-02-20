using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.CRT;

public class IntegracaoCRT
{
    public int ProtocoloIntegracaoCarga { get; set; }
    public string Numero { get; set; }
    public int Serie { get; set; }
    public string Chave { get; set; }
    public string Protocolo { get; set; }
    public DateTime DataEmissao { get; set; }

    public string CnpjRemetente { get; set; }
    public string CnpjDestinatario { get; set; }
    public string CnpjConsignatario { get; set; }
    public string CnpjTransportador { get; set; }
    public int CodigoIBGEInicioPrestacao { get; set; }
    public int CodigoIBGETerminoPrestacao { get; set; }
    public int CodigoIBGEEmissao { get; set; }

    public string CST { get; set; }
    public decimal Peso { get; set; }
    public decimal ValorMercadoria { get; set; }
    public decimal QuantidadeItensCarga { get; set; }
    public string PlacaVeiculo { get; set; }

    public decimal ValorCotacaMoeda { get; set; }
    public decimal ValorTotalMoeda { get; set; }
    public decimal ValorTotal { get; set; }
    public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }
    public decimal ValorICMS { get; set; }
    public decimal BaseCalculoICMS { get; set; }
    public decimal PercentualReducaoBaseCalculoICMS { get; set; }
    public decimal AliquotaICMS { get; set; }

    public List<Dominio.ObjetosDeValor.WebService.CRT.ComponentePrestacaoCRT> ComponentesFrete { get; set; }
    public List<Dominio.ObjetosDeValor.WebService.CRT.Factura> Facturas { get; set; }
}
