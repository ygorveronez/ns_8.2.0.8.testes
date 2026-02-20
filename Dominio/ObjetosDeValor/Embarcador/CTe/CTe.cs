using Dominio.Entidades.Embarcador.Rateio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class CTe
    {
        public CTe()
        {
            this.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;
            this.TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato;
        }

        public int Codigo { get; set; }
        public int CodigoDuplicado { get; set; }
        public string Chave { get; set; }
        public string Protocolo { get; set; }
        public string ProtocoloCancelamentoInutilizacao { get; set; }
        public int Numero { get; set; }
        public string Serie { get; set; }
        public string Versao { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public string MensagemRetornoSefaz { get; set; }

        public Dominio.ObjetosDeValor.Localidade LocalidadeInicioPrestacao { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadeFimPrestacao { get; set; }

        public int CFOP { get; set; }
        public string NaturezaOP { get; set; }

        public bool Retira { get; set; }
        public string DetalhesRetira { get; set; }

        public InformacaoCarga InformacaoCarga { get; set; }

        public List<QuantidadeCarga> QuantidadesCarga { get; set; }
        public List<NFe> NFEs { get; set; }
        public List<OutroDocumento> OutrosDocumentos { get; set; }
        public List<NotaFiscal> NotasFiscais { get; set; }
        public List<EntregaSimplificado> Entregas { get; set; }
        public List<Seguro> Seguros { get; set; }

        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }
        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz SituacaoCTeSefaz { get; set; }
        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }
        public Dominio.Enumeradores.TipoImpressao TipoImpressao { get; set; }
        public Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoModal TipoModal { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public Dominio.Enumeradores.OpcaoSimNao IndicadorGlobalizado { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE? IndicadorIETomador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Emitente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Expedidor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Recebedor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso> ProdutosPerigosos { get; set; }

        public string InformacaoAdicionalFisco { get; set; }
        public string InformacaoAdicionalContribuinte { get; set; }

        public List<Observacao> ObservacoesFisco { get; set; }
        public List<Observacao> ObservacoesContribuinte { get; set; }
        public Observacao ObservacoesGeral { get; set; }
        public List<DocumentoAnterior> DocumentosAnteriores { get; set; }
        public List<DocumentoAnteriorPapel> DocumentosAnterioresDePapel { get; set; }
        public Duplicata Duplicata { get; set; }
        public string ChaveCTeComplementado { get; set; }

        public string ChaveCTEReferenciado { get; set; }

        public ModalRodoviario ModalRodoviario { get; set; }
        public ModalAereo ModalAereo { get; set; }
        public ModalAquaviario ModalAquaviario { get; set; }
        public ModalFerroviario ModalFerroviario { get; set; }
        public ModalDutoviario ModalDutoviario { get; set; }
        public ModalMultimodal ModalMultimodal { get; set; }
        public InformacaoModal InformacaoModal { get; set; }
        public List<ModalAquaviarioContainer> Containers { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ValorFrete { get; set; }

        public string NumeroRomaneio { get; set; }

        public string NumeroPedido { get; set; }
        public string NumeroOS { get; set; }

        public string NumeroViagem { get; set; }

        public string Xml { get; set; }

        public decimal PesoTotal { get; set; }

        public CTeSubstituicao CTeSubstituicao { get; set; }

        public CTeAnulacao CTeAnulacao { get; set; }

        public CTeComplementar CTeComplementar { get; set; }
    }

    public class ObjetoValorPersistente
    {
        public ObjetoValorPersistente()
        {
            lstInsert = new List<Object>();
            lstUpdate = new List<Object>();
            lstDelete = new List<Object>();
        }
        public List<Object> lstInsert { get; set; }
        public List<Object> lstUpdate { get; set; }
        public List<Object> lstDelete { get; set; }
        public void Inserir(object obj, bool existeCodigo = false)
        {
            if (obj is Dominio.Entidades.Cliente)
            {
                var cliente = (Dominio.Entidades.Cliente)obj;
                if (lstInsert.OfType<Dominio.Entidades.Cliente>().Where(x => x.CPF_CNPJ == cliente.CPF_CNPJ).FirstOrDefault() == null)
                    lstInsert.Add(cliente);
                return;
            }

            lstInsert.Add(obj);
        }
        public void Atualizar(object obj)
        {
            lstUpdate.Add(obj);
        }
        public void Deletar(object obj)
        {
            lstDelete.Add(obj);
        }
    }


    public class CacheObjetoValorCTe
    {
        public CacheObjetoValorCTe()
        {
            ConfiguracaoTMS = null;
            LstCtesTerceiro = null;
            CacheAtivo = false;
            Auditado = false;
        }
        public bool Auditado { get; set; }

        public bool CacheAtivo { get; set; }
        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoTMS { get; set; }
        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> LstCtesTerceiro { get; set; }
        public List<Dominio.Entidades.ClienteIndex> lstCacheIndexClientes { get; set; }
        public List<Dominio.Entidades.Cliente> lstClientes { get; set; }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> lstGrupoPessoas { get; set; }
        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> lstClienteOutroEndereco { get; set; }
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configWebService { get; set; }

        public List<double> lstCNPJsCPFs { get; set; }
        public List<RateioFormula> lstRateioFormula { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> lstTiposIntegracao { get; set; }
        public List<Dominio.Entidades.Localidade> lstLocalidades { get; set; }
        public List<Dominio.Entidades.Pais> lstPais { get; set; }
        public List<Dominio.Entidades.Embarcador.Localidades.Regiao> lstRegiao { get; set; }
        public List<Dominio.Entidades.Atividade> lstAtividade { get; set; }
        public List<Dominio.Entidades.Empresa> lstEmpresa { get; set; }
        public List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> lstCategoriaPessoa { get; set; }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> lstModalidadePessoas { get; set; }
        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas> lstModalidadeClientePessoas { get; set; }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas> lstModalidadeFornecedorPessoas { get; set; }
        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas> lstModalidadeTransportadoraPessoas { get; set; }
        public List<double> lstCodigosClienteDescarga { get; set; }

        public List<Dominio.Entidades.CFOP> lstCFOP { get; set; }
        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> lstCTeTerceiroQuantidade { get; set; }
        public List<Dominio.Entidades.Banco> lstBancos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao { get; set; }
        
        
        //public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> PersistenciaDeletarCTeTerceiro { get; set; }
        //public List<Dominio.Entidades.Cliente> PersistenciaInsertCliente { get; set; }
        //public List<Dominio.Entidades.Cliente> PersistenciaUpdateCliente { get; set; }
        //public List<Dominio.Entidades.Embarcador.Localidades.Regiao> PersistenciaInsertRegiao { get; set; }
        //public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> PersistenciaInsertOutroEndereco { get; set; }
        //public List<Dominio.Entidades.Localidade> PersistenciaInsertEnderecos { get; set; }
        //public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> PersistenciaInsertClienteOutroEndereco { get; set; }
        //public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> PersistenciaInsertModalidadePessoas { get; set; }
        //public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas> PersistenciaInsertModalidadeClientePessoas { get; set; }
        //public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas> PersistenciaInsertModalidadeFornecedorPessoas { get; set; }
        //public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas> PersistenciaInsertModalidadeTransportadoraPessoas { get; set; }
        //public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> PersistenciaInsertClienteDescarga { get; set; }
        //public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> PersistenciaUpdateClienteDescarga { get; set; }
        //public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> PersistenciaInsertCTeTerceiro { get; set; }
        //public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> PersistenciaInsertCTeTerceiroQuantidade { get; set; }
        //public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> PersistenciaInsertCTeTerceiroSeguro { get; set; }
        //public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> PersistenciaInsertCTeTerceiroOutrosDocumentos { get; set; }
        //public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal> PersistenciaInsertCTeTerceiroNotaFiscal { get; set; }
        //public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> PersistenciaInsertCTeTerceiroNFe { get; set; }
        //public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> PersistenciaInsertCTeTerceiroComponenteFrete { get; set; }

        //public List<Dominio.Entidades.ParticipanteCTe> PersistenciaInsertParticipanteCTe { get; set; }

        //public void Inserir(Dominio.Entidades.ParticipanteCTe ParticipanteCTe)
        //{
        //    PersistenciaInsertParticipanteCTe.Add(ParticipanteCTe);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete CTeTerceiroComponenteFrete)
        //{
        //    PersistenciaInsertCTeTerceiroComponenteFrete.Add(CTeTerceiroComponenteFrete);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe CTeTerceiroNFe)
        //{
        //    PersistenciaInsertCTeTerceiroNFe.Add(CTeTerceiroNFe);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal CTeTerceiroNotaFiscal)
        //{
        //    PersistenciaInsertCTeTerceiroNotaFiscal.Add(CTeTerceiroNotaFiscal);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos CTeTerceiroOutrosDocumentos)
        //{
        //    PersistenciaInsertCTeTerceiroOutrosDocumentos.Add(CTeTerceiroOutrosDocumentos);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro CTeTerceiroSeguro)
        //{
        //    PersistenciaInsertCTeTerceiroSeguro.Add(CTeTerceiroSeguro);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade CTeTerceiroQuantidade)
        //{
        //    PersistenciaInsertCTeTerceiroQuantidade.Add(CTeTerceiroQuantidade);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        //{
        //    PersistenciaInsertCTeTerceiro.Add(cteTerceiro);
        //}
        //public void Inserir(Dominio.Entidades.Cliente Cliente)
        //{
        //    PersistenciaInsertCliente.Add(Cliente);
        //}
        //public void Atualizar(Dominio.Entidades.Cliente Cliente)
        //{
        //    PersistenciaUpdateCliente.Add(Cliente);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.Localidades.Regiao Regiao)
        //{
        //    PersistenciaInsertRegiao.Add(Regiao);
        //}
        //public void InserirOutroEndereco(Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco OutroEndereco)
        //{
        //    PersistenciaInsertOutroEndereco.Add(OutroEndereco);
        //}
        //public void Inserir(Dominio.Entidades.Localidade Endereco)
        //{
        //    PersistenciaInsertEnderecos.Add(Endereco);
        //}
        //public void InserirClienteOutroEndereco(Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco)
        //{
        //    PersistenciaInsertClienteOutroEndereco.Add(ClienteOutroEndereco);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas ModalidadePessoas)
        //{
        //    PersistenciaInsertModalidadePessoas.Add(ModalidadePessoas);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas ModalidadeClientePessoas)
        //{
        //    PersistenciaInsertModalidadeClientePessoas.Add(ModalidadeClientePessoas);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas ModalidadeFornecedorPessoas)
        //{
        //    PersistenciaInsertModalidadeFornecedorPessoas.Add(ModalidadeFornecedorPessoas);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas ModalidadeTransportadoraPessoas)
        //{
        //    PersistenciaInsertModalidadeTransportadoraPessoas.Add(ModalidadeTransportadoraPessoas);
        //}
        //public void Inserir(Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga ClienteDescarga)
        //{
        //    PersistenciaInsertClienteDescarga.Add(ClienteDescarga);
        //}
        //public void Atualizar(Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga ClienteDescarga)
        //{
        //    PersistenciaUpdateClienteDescarga.Add(ClienteDescarga);
        //}


        //public void Deletar(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro)
        //{
        //    PersistenciaDeletarCTeTerceiro.Add(cteTerceiro);
        //}

    }
}
