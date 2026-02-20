using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class Pre_NFS_E_XML
    {
        public cabecalho cabecalho;
        public List<Reg_Detalhe> Detalhes_Pre_NFS_e;
        public Resumo Resumo;
    }
    public class cabecalho
    {
        public string Cab_CNPJ { get; set; }
        public string Cab_Bloco_Pre_NFS_e { get; set; }
        public string Cab_Descricao { get; set; }
        public string Cab_Versao { get; set; }
        public string Cab_Data { get; set; }
    }
    public class Detalhes_Pre_NFS_e
    {
        public List<Reg_Detalhe> Reg_Detalhe { get; set; }
    }
    public class Reg_Detalhe
    {
        public string Pre_NFS_E_Det_Seq { get; set; }
        public string Pre_NFS_E_Det_Cod_Validacao { get; set; }
        public string Pre_NFS_E_Det_IP_Cadastro { get; set; }
        public string Pre_NFS_E_Det_IP_Cancelamento { get; set; }
        public string Pre_NFS_E_Det_NFS_E_Numero { get; set; }
        public string Pre_NFS_E_Det_NFS_E_Especie { get; set; }
        public string Pre_NFS_E_Det_NFS_E_Atividade { get; set; }
        public string Pre_NFS_E_Det_NFS_E_Mes_PA { get; set; }
        public string Pre_NFS_E_Det_NFS_E_Ano_PA { get; set; }
    }

    public class Resumo
    {
        public string Resumo_Quantidade_Detalhes { get; set; }
    }
}
