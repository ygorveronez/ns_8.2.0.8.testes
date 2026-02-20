using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Pedido;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;


namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargaDocumentoEmissaoNFSManual : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaDocumentoEmissaoNFSManual>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual _repositorioCargaDocumentoParaEmissaoNFSManual;

        #endregion

        #region Construtores

        public CargaDocumentoEmissaoNFSManual(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaDocumentoEmissaoNFSManual> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaDocumentoParaEmissaoNFSManual.ConsultarRelatorioDocumentoEmissaoNFSManual(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaDocumentoParaEmissaoNFSManual.ContarConsultaRelatorioDocumentoEmissaoNFSManual(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/DocumentoEmissaoNFSManual";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Cliente remetente = filtrosPesquisa.CpfCnpjRemetente > 0d ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjRemetente) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CpfCnpjDestinatario > 0d ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario) : null;
            Dominio.Entidades.Cliente tomador = filtrosPesquisa.CpfCnpjTomador > 0d ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjTomador) : null;
            Dominio.Entidades.Localidade localidadePrestacao = filtrosPesquisa.CodigoLocalidadePrestacao > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadePrestacao) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFiliais.Count > 0 ? repositorioFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFiliais) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? repositorioGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", filtrosPesquisa.DataEmissaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoNFSInicial", filtrosPesquisa.DataEmissaoNFSInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoNFSFinal", filtrosPesquisa.DataEmissaoNFSFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiNFSGerada", filtrosPesquisa.PossuiNFSGerada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", remetente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", tomador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filiais != null ? string.Join(", ", filiais?.Select(o => o.Descricao)) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalidadePrestacao", localidadePrestacao?.DescricaoCidadeEstado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoPrestacao", filtrosPesquisa.EstadoLocalidadePrestacao != "0" ? filtrosPesquisa.EstadoLocalidadePrestacao : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", filtrosPesquisa.NumeroInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", filtrosPesquisa.NumeroFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNFSInicial", filtrosPesquisa.NumeroInicialNFS));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNFSFinal", filtrosPesquisa.NumeroFinalNFS));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedidoCliente", filtrosPesquisa.NumeroPedidoCliente));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "SituacaoEntregaDescricao")
                return "SituacaoEntrega";

            return propriedadeOrdenarOuAgrupar;
        }
    }
}
