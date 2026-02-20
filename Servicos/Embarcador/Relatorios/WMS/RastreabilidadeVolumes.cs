using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.WMS;
using Dominio.Relatorios.Embarcador.DataSource.WMS;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.WMS
{
    public class RastreabilidadeVolumes : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioRastreabilidadeVolumes, Dominio.Relatorios.Embarcador.DataSource.WMS.RastreamentoVolumes>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.WMS.RecebimentoMercadoria _repRastreabilidadeVolumes;

        #endregion

        #region Construtores

        public RastreabilidadeVolumes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repRastreabilidadeVolumes = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        // TODO: ToList metodo override
        protected override List<RastreamentoVolumes> ConsultarRegistros(FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repRastreabilidadeVolumes.ConsultarRelatorioRastreabilidadeVolumes(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repRastreabilidadeVolumes.ContarConsultaRelatorioRastreabilidadeVolumes(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/WMS/RastreabilidadeVolumes";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.Carga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.Carga) : null;
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = filtrosPesquisa.ProdutoEmbarcador > 0 ? repProdutoEmbarcador.BuscarPorCodigo(filtrosPesquisa.ProdutoEmbarcador) : null;

            parametros.Add(new Parametro("Carga", carga?.Descricao ?? ""));
            parametros.Add(new Parametro("ProdutoEmbarcador", produto?.Descricao ?? ""));
            parametros.Add(new Parametro("DataPedidoInicial", filtrosPesquisa.DataPedidoInicial));
            parametros.Add(new Parametro("DataPedidoFinal", filtrosPesquisa.DataPedidoFinal));
            parametros.Add(new Parametro("DataRecebimentoInicial", filtrosPesquisa.DataRecebimentoInicial));
            parametros.Add(new Parametro("DataRecebimentoFinal", filtrosPesquisa.DataRecebimentoFinal));
            parametros.Add(new Parametro("NumeroPedido", filtrosPesquisa.NumeroPedido));
            parametros.Add(new Parametro("Agrupamento", ""));

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
