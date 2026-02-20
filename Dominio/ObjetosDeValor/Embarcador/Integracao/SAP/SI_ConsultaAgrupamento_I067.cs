using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAP
{


    public class Mensagem
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class MensagemItem
    {
        public List<Mensagem> Msgs { get; set; }
    }

    public class Item
    {
        public long Referencia { get; set; }
        public long NumFatura { get; set; }
        public int Status { get; set; }
        public string StMessage { get; set; }
        public int Empresa { get; set; }
        public long Documento { get; set; }
        public int Exercicio { get; set; }
        public MensagemItem Msg { get; set; }
    }

    public class _Itens
    {
        public Item Itens { get; set; }
    }

    public class Agrup
    {
        public string Origem { get; set; }
        public string Cliente { get; set; }
        public string IdControle { get; set; }
        public long Lote { get; set; }
        public int Status { get; set; }
        public string StMessage { get; set; }
        public _Itens Item { get; set; }
    }

    public class Agrups
    {
        public Agrup Agrup { get; set; }
    }

    public class Root
    {
        public Agrups Agrups { get; set; }
    }

    public class ConsultaAgrupamento_I067
    {
        public string GUID { get; set; }
        public string Origem { get; set; }
        public string Cliente { get; set; }
        public string Referencia { get; set; }
        public string IdControle { get; set; }
        public string NumFatura { get; set; }
    }

}
