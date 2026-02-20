using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Fretes
{
    public class ProvisaoVolumetria : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria, Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria>
    {
        #region Atributos
        private readonly Repositorio.Embarcador.Frete.ProvisaoVolumetria _repProvisaoVolumetria;

        #endregion

        #region Construtores

        public ProvisaoVolumetria(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repProvisaoVolumetria = new Repositorio.Embarcador.Frete.ProvisaoVolumetria(_unitOfWork);
        }

        public ProvisaoVolumetria(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repProvisaoVolumetria = new Repositorio.Embarcador.Frete.ProvisaoVolumetria(_unitOfWork, cancellationToken);
        }

        #endregion
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repProvisaoVolumetria.RelatorioProvisaoVolumetriaAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #region

        #endregion


        #region Métodos Protegidos Sobrescritos
        // TODO: ToList meotdo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repProvisaoVolumetria.RelatorioProvisaoVolumetria(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repProvisaoVolumetria.ContarConsultaRelatorioProvisaoVolumetria(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Fretes/ProvisaoVolumetria";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            List<Dominio.Entidades.Empresa> transportadores = filtrosPesquisa.CodigosTransportador.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosTransportador) : new List<Dominio.Entidades.Empresa>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = filtrosPesquisa.CodigosTipoOperacao?.Count > 0 ? repTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOperacao) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFilial?.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            
            parametros.Add(new Parametro("Transportador", transportadores.Select(o => o.RazaoSocial)));
            parametros.Add(new Parametro("TipoOperacao", tiposOperacao.Select(o => o.Descricao)));
            parametros.Add(new Parametro("Filial", filiais.Select(o => o.Descricao)));

            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataEmissaoNFInicio, filtrosPesquisa.DataEmissaoNFFim));
            parametros.Add(new Parametro("DataIntegracaoPagamento", filtrosPesquisa.DataIntegracaoPagamentoInicio, filtrosPesquisa.DataIntegracaoPagamentoFim));
            parametros.Add(new Parametro("DataVencimento", filtrosPesquisa.DataVencimentoInicio, filtrosPesquisa.DataVencimentoFim));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}