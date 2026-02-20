namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.AnaliseCadastral
{
    public class RequisicaoConsultaAnaliseCadastro
    {
        public string solicitante_id { get; set; }
        public string solicitante_token { get; set; }
        public int cliente_id { get; set; }
        public string cliente_cnpj { get; set; }
        public int fornecedor_id { get; set; }
        public string usuario { get; set; }
        public string senha { get; set; }
        public string token { get; set; }
        public TagsLogin tagsLogin { get; set; }
        public int solicitao_pesquisa_numero { get; set; }
        public string profissional_cpf { get; set; }
        public string veiculo_placa { get; set; }
        public string carreteiro { get; set; }
        public int carga_tipo { get; set; }
        public decimal carga_valor { get; set; }
        public int carga_pais_origem { get; set; }
        public int carga_uf_origem { get; set; }
        public int carga_cidade_origem { get; set; }
        public int carga_pais_destino { get; set; }
    }


    public class TagsLogin
    {
        public string dominio { get; set; }
        public string ipporta_dns { get; set; }
        public string grupo_cliente { get; set; }
    }
}
