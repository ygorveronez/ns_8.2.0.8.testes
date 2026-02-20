using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class CargaDocumentoParaEmissaoNFSManualCancelada : RepositorioBase<Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada>
    {
        #region Construtores

        public CargaDocumentoParaEmissaoNFSManualCancelada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarSelecaoNFSManual(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa)
        {
            var consultaCargaDocumentoParaEmissaoNFSManualCancelada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada>()
                .Where(o => o.LancamentoNFSManual.Codigo == filtrosPesquisa.CodigoLancamentoNFSManual);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.CargaOrigem.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.CargaOrigem.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (filtrosPesquisa.Codigosfilial?.Count > 0)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => filtrosPesquisa.Codigosfilial.Contains(o.CargaDocumentoParaEmissaoNFSManual.CargaOrigem.Filial.Codigo));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.CargaOrigem.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.CargaOrigem.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CpfCnpjTomador > 0)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.DataEmissao >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.DataEmissao <= filtrosPesquisa.DataFinal.Value);

            if (filtrosPesquisa.NumeroInicial > 0)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.Numero <= filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.Moeda.HasValue)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.Moeda == filtrosPesquisa.Moeda);

            if (filtrosPesquisa.ComplementoOcorrencia == NFSManualTipoComplemento.ApenasDocumentosOriginais)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.CargaOcorrencia == null);
            else if (filtrosPesquisa.ComplementoOcorrencia == NFSManualTipoComplemento.ApenasComplementosOcorrencia)
                consultaCargaDocumentoParaEmissaoNFSManualCancelada = consultaCargaDocumentoParaEmissaoNFSManualCancelada.Where(o => o.CargaDocumentoParaEmissaoNFSManual.CargaOcorrencia != null);

            return consultaCargaDocumentoParaEmissaoNFSManualCancelada.Select(obj => obj.CargaDocumentoParaEmissaoNFSManual);
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada BuscarPorDocumentoParaEmissaoNFSManual(int codigoDocumentoParaEmissaoNFSManual)
        {
            var consultaCargaDocumentoParaEmissaoNFSManualCancelada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada>()
                .Where(o => o.CargaDocumentoParaEmissaoNFSManual.Codigo == codigoDocumentoParaEmissaoNFSManual);

            return consultaCargaDocumentoParaEmissaoNFSManualCancelada.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarSelecaoNFSManual(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = ConsultarSelecaoNFSManual(filtrosPesquisa);

            consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual
                .Fetch(obj => obj.CargaOrigem).ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.CargaOrigem).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.FechamentoFrete)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.LocalidadePrestacao)
                .Fetch(obj => obj.Destinatario)
                .Fetch(obj => obj.PedidoXMLNotaFiscal).ThenFetch(obj => obj.XMLNotaFiscal).ThenFetch(obj => obj.Canhoto);

            return ObterLista(consultaCargaDocumentoParaEmissaoNFSManual, parametrosConsulta);
        }

        public int ContarConsultaSelecaoNFSManual(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = ConsultarSelecaoNFSManual(filtrosPesquisa);

            return consultaCargaDocumentoParaEmissaoNFSManual.Count();
        }

        #endregion
    }
}
