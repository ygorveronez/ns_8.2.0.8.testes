using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Contabeis
{
    public sealed class ImpostoValorAgregado
    {

        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Contrutores

        public ImpostoValorAgregado(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public ImpostoValorAgregado(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private int ObterCodigoImpostoValorAgregado(int codigoStage, List<(int CodigoStage, int CodigoImpostoValorAgregado, TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada, int CodigoCargaPedido)> dadosImpostoValorAgregado)
        {
            List<(int CodigoStage, int CodigoImpostoValorAgregado, TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada, int CodigoCargaPedido)> dadosImpostoValorAgregadoPorStage = dadosImpostoValorAgregado.Where(dados => dados.CodigoStage == codigoStage).ToList();
            List<int> codigosImpostoValorAgregado = dadosImpostoValorAgregadoPorStage.Select(dados => dados.CodigoImpostoValorAgregado).Distinct().ToList();

            if (codigosImpostoValorAgregado.Count == 1 && codigosImpostoValorAgregado[0] == 0)
            {
                List<int> codigosCargaPedidos = dadosImpostoValorAgregadoPorStage.Select(dados => dados.CodigoCargaPedido).Distinct().ToList();
                codigosImpostoValorAgregado = ProcessarIVACargaPedido(codigosCargaPedidos);
            }

            if (codigosImpostoValorAgregado.Count == 1)
                return codigosImpostoValorAgregado.FirstOrDefault();

            List<TipoNotaFiscalIntegrada?> tiposNotaFiscalIntegrada = dadosImpostoValorAgregadoPorStage.Select(dados => dados.TipoNotaFiscalIntegrada).Distinct().ToList();

            if ((tiposNotaFiscalIntegrada.Count == 1) || !tiposNotaFiscalIntegrada.Contains(TipoNotaFiscalIntegrada.RemessaPallet))
                return 0;

            List<int> codigosImpostoValorAgregadoSemRemessaPallet = dadosImpostoValorAgregadoPorStage.Where(dados => dados.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet).Select(dados => dados.CodigoImpostoValorAgregado).Distinct().ToList();

            if (codigosImpostoValorAgregadoSemRemessaPallet.Count > 1)
                return 0;

            return codigosImpostoValorAgregadoSemRemessaPallet.FirstOrDefault();
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void DefinirImpostoValorAgregadoPorStage(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            List<(int CodigoStage, int CodigoImpostoValorAgregado, TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada, int CodigoCargaPedido)> dadosImpostoValorAgregado = repositorioDocumentoProvisao.BuscarDadosImpostoValorAgregadoPorProvisaoComStage(provisao.Codigo);

            if (dadosImpostoValorAgregado.Count == 0)
                return;

            List<int> codigosStages = dadosImpostoValorAgregado.Select(dados => dados.CodigoStage).Distinct().ToList();

            foreach (int codigoStage in codigosStages)
            {
                int codigoImpostoValorAgregado = ObterCodigoImpostoValorAgregado(codigoStage, dadosImpostoValorAgregado);

                repositorioDocumentoProvisao.SetarImpostoValorAgregadoPorProvisaoEStage(provisao.Codigo, codigoStage, codigoImpostoValorAgregado);
            }
        }

        public List<int> ProcessarIVACargaPedido(List<int> codigoCargaPedidos)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCodigos(codigoCargaPedidos);
            List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado> impostoValorAgregado = new List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado>();

            if (cargaPedidos != null && cargaPedidos.Count > 0)
                foreach (var cargaPedido in cargaPedidos)
                {
                    DefinirImpostoValorAgregado(cargaPedido, true);
                    if (cargaPedido.ImpostoValorAgregado != null && !repCargaPedido.ExisteNotaRemessaPallet(cargaPedido.Codigo))
                        impostoValorAgregado.Add(cargaPedido.ImpostoValorAgregado);
                }

            impostoValorAgregado = impostoValorAgregado.Distinct().ToList();

            return impostoValorAgregado.DistinctBy(x => x.CodigoIVA).Select(x => x.Codigo).ToList();
        }

        public void DefinirImpostoValorAgregado(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool atualizarDocumentos = false)
        {
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal ?? repositorioModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

            if (modeloDocumentoFiscal == null)
                return;

            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFilial repositorioProdutoEmbarcadorFilial = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFilial(_unitOfWork);
            UsoMaterial? usoMaterial = repositorioProdutoEmbarcadorFilial.BuscarUsoMaterialPorCargaPedido(cargaPedido.Codigo);

            if (!usoMaterial.HasValue)
                return;

            bool impostoMaiorQueZero = (cargaPedido.ValorICMS + cargaPedido.ValorISS) > 0;
            bool destinatarioExterior = cargaPedido.Pedido.Destinatario.Tipo == "E";
            Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(_unitOfWork);
            Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = repositorioImpostoValorAgregado.BuscarPrimeiro(modeloDocumentoFiscal.Codigo, impostoMaiorQueZero, destinatarioExterior, usoMaterial.Value);

            if (impostoValorAgregado == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            cargaPedido.ImpostoValorAgregado = impostoValorAgregado;

            repositorioCargaPedido.Atualizar(cargaPedido);

            if (!atualizarDocumentos || cargaPedido.NotasFiscais == null || cargaPedido.NotasFiscais.Count == 0)
                return;

            List<int> codigosNotas = cargaPedido.NotasFiscais.Select(x => x.XMLNotaFiscal.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentos = repositorioDocumentoProvisao.BuscarPorCargaENotasSemFiltro(cargaPedido.Carga.Codigo, codigosNotas);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentos)
            {
                documento.ImpostoValorAgregado = cargaPedido.ImpostoValorAgregado;
                repositorioDocumentoProvisao.Atualizar(documento);
            }
        }

        #endregion Métodos Públicos
    }
}
