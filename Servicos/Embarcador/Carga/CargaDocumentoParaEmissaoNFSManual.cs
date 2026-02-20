using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class CargaDocumentoParaEmissaoNFSManual : ServicoBase
    {
        private readonly string _stringConexao;

        public CargaDocumentoParaEmissaoNFSManual(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }        

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Carga.CargaDocumentoParaEmissaoNFSManual ConverterEntidadeCargaDocumentoParaEmissaoNFSManualParaObjeto(Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade();
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa();

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaDocumentoParaEmissaoNFSManual documentoParaEmissaoNFSManual = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDocumentoParaEmissaoNFSManual();

            documentoParaEmissaoNFSManual.Tomador = serPessoa.ConverterObjetoPessoa(cargaDocumentoParaEmissaoNFSManual.Tomador);
            documentoParaEmissaoNFSManual.Destinatario = serPessoa.ConverterObjetoPessoa(cargaDocumentoParaEmissaoNFSManual.Destinatario);
            documentoParaEmissaoNFSManual.Remetente = serPessoa.ConverterObjetoPessoa(cargaDocumentoParaEmissaoNFSManual.Remetente);
            documentoParaEmissaoNFSManual.LocalidadePrestacao = serLocalidade.ConverterObjetoLocalidade(cargaDocumentoParaEmissaoNFSManual.LocalidadePrestacao);
            documentoParaEmissaoNFSManual.Chave = cargaDocumentoParaEmissaoNFSManual.Chave;
            documentoParaEmissaoNFSManual.Numero = cargaDocumentoParaEmissaoNFSManual.Numero;
            documentoParaEmissaoNFSManual.Serie = cargaDocumentoParaEmissaoNFSManual.Serie;
            documentoParaEmissaoNFSManual.DataEmissao = cargaDocumentoParaEmissaoNFSManual.DataEmissao;
            documentoParaEmissaoNFSManual.ValorFrete = cargaDocumentoParaEmissaoNFSManual.ValorFrete;
            documentoParaEmissaoNFSManual.ValorISS = cargaDocumentoParaEmissaoNFSManual.ValorISS;
            documentoParaEmissaoNFSManual.ValorRetencaoISS = cargaDocumentoParaEmissaoNFSManual.ValorRetencaoISS;
            documentoParaEmissaoNFSManual.ValorPrestacaoServico = cargaDocumentoParaEmissaoNFSManual.ValorPrestacaoServico;
            documentoParaEmissaoNFSManual.BaseCalculoISS = cargaDocumentoParaEmissaoNFSManual.BaseCalculoISS;
            documentoParaEmissaoNFSManual.PercentualAliquotaISS = cargaDocumentoParaEmissaoNFSManual.PercentualAliquotaISS;
            documentoParaEmissaoNFSManual.Peso = cargaDocumentoParaEmissaoNFSManual.Peso;

            return documentoParaEmissaoNFSManual;
        }
        #endregion
    }
}
