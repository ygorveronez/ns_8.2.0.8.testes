using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Documentos
{
    public class GestaoDocumento
    {
        public Int64 Codigo { get; set; }
        public int CodigoCargaCTe { get; set; }
        public int CodigoCTe { get; set; }
        public int CodigoPreCTe { get; set; }
        public SituacaoGestaoDocumento SituacaoGestaoDocumento { get; set; }
        public MotivoInconsistenciaGestaoDocumento MotivoInconsistenciaGestaoDocumento { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string NumeroPedido { get; set; }
        public string Carga { get; set; }
        public string NFeRecebida { get; set; }
        public string Ocorrencia { get; set; }
        public string Chave { get; set; }
        public string TipoOperacao { get; set; }
        private DateTime DataEmissao { get; set; }
        public string Remetente { get; set; }
        public string Tomador { get; set; }
        public string Transportador { get; set; }
        public decimal Valor { get; set; }
        public decimal FreteNfXml { get; set; }
        public string DetalhesInconsistencia { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorEsperado { get; set; }
        public decimal DiferencaValores { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public decimal PesoCarga { get; set; }
        public string PesoCubado { get; set; }
        public string Destinatario { get; set; }
        public string CidadeDestino { get; set; }
        public string UFDestino { get; set; }
        public string CEPDestino { get; set; }
        public decimal ValorNota { get; set; }
        public string ChaveNota { get; set; }
        public string MotivoRejeicao { get; set; }
        public int Prioridade { get; set; }
        public string Regra { get; set; }
        public int QuantidadeImportacoesCTe { get; set; }
        public DateTime DataImportacaoCTe { get; set; }
        public decimal ValorICMSEsperado { get; set; }
        public decimal ValorICMSRecebido { get; set; }
        public decimal AliquotaIBSUF { get; set; }
        public decimal AliquotaIBSMunicipal { get; set; }
        public decimal ValorIBSEstadual { get; set; }
        public decimal ValorIBSMunicipal { get; set; }
        public decimal AliquotaCBS { get; set; }
        public decimal ValorCBS { get; set; }
        public string UltimoAprovador { get; set; }

        #region Colunas Dinâmicas

        public string ValorComponente1 { get; set; }
        public string ValorComponente2 { get; set; }
        public string ValorComponente3 { get; set; }
        public string ValorComponente4 { get; set; }
        public string ValorComponente5 { get; set; }
        public string ValorComponente6 { get; set; }
        public string ValorComponente7 { get; set; }
        public string ValorComponente8 { get; set; }
        public string ValorComponente9 { get; set; }
        public string ValorComponente10 { get; set; }
        public string ValorComponente11 { get; set; }
        public string ValorComponente12 { get; set; }
        public string ValorComponente13 { get; set; }
        public string ValorComponente14 { get; set; }
        public string ValorComponente15 { get; set; }
        public string ValorComponente16 { get; set; }
        public string ValorComponente17 { get; set; }
        public string ValorComponente18 { get; set; }
        public string ValorComponente19 { get; set; }
        public string ValorComponente20 { get; set; }
        public string ValorComponente21 { get; set; }
        public string ValorComponente22 { get; set; }
        public string ValorComponente23 { get; set; }
        public string ValorComponente24 { get; set; }
        public string ValorComponente25 { get; set; }
        public string ValorComponente26 { get; set; }
        public string ValorComponente27 { get; set; }
        public string ValorComponente28 { get; set; }
        public string ValorComponente29 { get; set; }
        public string ValorComponente30 { get; set; }
        public string ValorComponente31 { get; set; }
        public string ValorComponente32 { get; set; }
        public string ValorComponente33 { get; set; }
        public string ValorComponente34 { get; set; }
        public string ValorComponente35 { get; set; }
        public string ValorComponente36 { get; set; }
        public string ValorComponente37 { get; set; }
        public string ValorComponente38 { get; set; }
        public string ValorComponente39 { get; set; }
        public string ValorComponente40 { get; set; }
        public string ValorComponente41 { get; set; }
        public string ValorComponente42 { get; set; }
        public string ValorComponente43 { get; set; }
        public string ValorComponente44 { get; set; }
        public string ValorComponente45 { get; set; }
        public string ValorComponente46 { get; set; }
        public string ValorComponente47 { get; set; }
        public string ValorComponente48 { get; set; }
        public string ValorComponente49 { get; set; }
        public string ValorComponente50 { get; set; }
        public string ValorComponente51 { get; set; }
        public string ValorComponente52 { get; set; }
        public string ValorComponente53 { get; set; }
        public string ValorComponente54 { get; set; }
        public string ValorComponente55 { get; set; }
        public string ValorComponente56 { get; set; }
        public string ValorComponente57 { get; set; }
        public string ValorComponente58 { get; set; }
        public string ValorComponente59 { get; set; }
        public string PercentualComponente1 { get; set; }
        public string PercentualComponente2 { get; set; }
        public string PercentualComponente3 { get; set; }
        public string PercentualComponente4 { get; set; }
        public string PercentualComponente5 { get; set; }
        public string PercentualComponente6 { get; set; }
        public string PercentualComponente7 { get; set; }
        public string PercentualComponente8 { get; set; }
        public string PercentualComponente9 { get; set; }
        public string PercentualComponente10 { get; set; }
        public string PercentualComponente11 { get; set; }
        public string PercentualComponente12 { get; set; }
        public string PercentualComponente13 { get; set; }
        public string PercentualComponente14 { get; set; }
        public string PercentualComponente15 { get; set; }
        public string PercentualComponente16 { get; set; }
        public string PercentualComponente17 { get; set; }
        public string PercentualComponente18 { get; set; }
        public string PercentualComponente19 { get; set; }
        public string PercentualComponente20 { get; set; }
        public string PercentualComponente21 { get; set; }
        public string PercentualComponente22 { get; set; }
        public string PercentualComponente23 { get; set; }
        public string PercentualComponente24 { get; set; }
        public string PercentualComponente25 { get; set; }
        public string PercentualComponente26 { get; set; }
        public string PercentualComponente27 { get; set; }
        public string PercentualComponente28 { get; set; }
        public string PercentualComponente29 { get; set; }
        public string PercentualComponente30 { get; set; }
        public string PercentualComponente31 { get; set; }
        public string PercentualComponente32 { get; set; }
        public string PercentualComponente33 { get; set; }
        public string PercentualComponente34 { get; set; }
        public string PercentualComponente35 { get; set; }
        public string PercentualComponente36 { get; set; }
        public string PercentualComponente37 { get; set; }
        public string PercentualComponente38 { get; set; }
        public string PercentualComponente39 { get; set; }
        public string PercentualComponente40 { get; set; }
        public string PercentualComponente41 { get; set; }
        public string PercentualComponente42 { get; set; }
        public string PercentualComponente43 { get; set; }
        public string PercentualComponente44 { get; set; }
        public string PercentualComponente45 { get; set; }
        public string PercentualComponente46 { get; set; }
        public string PercentualComponente47 { get; set; }
        public string PercentualComponente48 { get; set; }
        public string PercentualComponente49 { get; set; }
        public string PercentualComponente50 { get; set; }
        public string PercentualComponente51 { get; set; }
        public string PercentualComponente52 { get; set; }
        public string PercentualComponente53 { get; set; }
        public string PercentualComponente54 { get; set; }
        public string PercentualComponente55 { get; set; }
        public string PercentualComponente56 { get; set; }
        public string PercentualComponente57 { get; set; }
        public string PercentualComponente58 { get; set; }
        public string PercentualComponente59 { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Situacao
        {
            get { return SituacaoGestaoDocumento.ObterDescricao(); }
        }

        public decimal ValorRecebido
        {
            get { return Valor; }
        }

        public string MotivoPendencia
        {
            get { return !string.IsNullOrWhiteSpace(DetalhesInconsistencia) ? DetalhesInconsistencia : MotivoInconsistenciaGestaoDocumento.ObterDescricao(); }
        }

        public string PercentualDiferencaValor
        {
            get
            {
                if (ValorEsperado > 0)
                    return (((ValorRecebido * 100) / ValorEsperado) - 100).ToString("n2") + "%";
                else
                    return "";
            }
        }

        public string DataImportacaoCTeFormatada
        {
            get { return DataImportacaoCTe != DateTime.MinValue ? DataImportacaoCTe.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}
