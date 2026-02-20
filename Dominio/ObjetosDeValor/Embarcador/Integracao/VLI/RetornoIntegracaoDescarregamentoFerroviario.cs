using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VLI
{
    public class RetornoIntegracaoDescarregamentoFerroviario
    {
        public Datum[] Data { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Datum
    {
        public List<NFeDescarregamentoFerroviario> listaNfes { get; set; }
        public Fluxo fluxo { get; set; }
        public LocalDescarga localDescarga { get; set; }
        public DateTime criadoEm { get; set; }
        public DateTime inseridoEm { get; set; }
        public string totalRegistros { get; set; }
        public string chaveUnica { get; set; }
        public int identificador { get; set; }
        public string operacao { get; set; }
        public DateTime dataOperacao { get; set; }
        public DateTime? dataDescarga { get; set; }
        public int identificadorCarregamento { get; set; }
        public string serieCarregamento { get; set; }
        public string numeroCarregamento { get; set; }
        public string codigoVagao { get; set; }
        public string serieVagao { get; set; }
        public string pesoCarregamento { get; set; }
        public string pesoTotal { get; set; }
        public string taraVagao { get; set; }
        public string chaveCte { get; set; }
    }

    public class EmpresaCorrentista
    {
        public string codigo { get; set; }
        public string nome { get; set; }
        public string cnpj { get; set; }
        public string raizCnpj { get; set; }
    }

    public class EmpresaDestinataria
    {
        public string codigo { get; set; }
        public string nome { get; set; }
        public string cnpj { get; set; }
        public string raizCnpj { get; set; }
    }

    public class EmpresaFaturamento
    {
        public string codigo { get; set; }
        public string nome { get; set; }
        public string cnpj { get; set; }
        public string raizCnpj { get; set; }
    }

    public class EmpresaRemetente
    {
        public string codigo { get; set; }
        public string nome { get; set; }
        public string cnpj { get; set; }
        public string raizCnpj { get; set; }
    }

    public class Fluxo
    {
        public int identificador { get; set; }
        public string codigo { get; set; }
        public EmpresaRemetente empresaRemetente { get; set; }
        public EmpresaDestinataria empresaDestinataria { get; set; }
        public EmpresaCorrentista empresaCorrentista { get; set; }
        public EmpresaFaturamento empresaFaturamento { get; set; }
        public Produto produto { get; set; }
        public LocalOrigem localOrigem { get; set; }
        public LocalDestino localDestino { get; set; }
    }

    public class NFeDescarregamentoFerroviario
    {
        public string idDescarga { get; set; }
        public string chave { get; set; }
        public string peso { get; set; }
        public string valor { get; set; }
        public string identificador { get; set; }
        public string tipo { get; set; }
        public string serie { get; set; }
        public string numero { get; set; }
        public string pesoVagao { get; set; }
        public DateTime dataEmissao { get; set; }
        public string chaveUnica { get; set; }
    }

    public class LocalDescarga
    {
        public string identificador { get; set; }
        public string codigo { get; set; }
        public string nome { get; set; }
        public string municipio { get; set; }
        public string estado { get; set; }
        public string codigoTerminal { get; set; }
        public string cnpjTerminal { get; set; }
    }

    public class LocalDestino
    {
        public string identificador { get; set; }
        public string codigo { get; set; }
        public string municipio { get; set; }
        public string estado { get; set; }
        public string codigoTerminal { get; set; }
    }

    public class LocalOrigem
    {
        public string identificador { get; set; }
        public string codigo { get; set; }
        public string municipio { get; set; }
        public string estado { get; set; }
        public string codigoTerminal { get; set; }
    }

    public class Mercadoria
    {
        public string identificador { get; set; }
        public string nome { get; set; }
    }

    public class Produto
    {
        public string identificador { get; set; }
        public string nome { get; set; }
        public Mercadoria mercadoria { get; set; }
    }

}