using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Repositorio;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.TorreControle
{
    public class DevolucaoNotasFiscais : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais, Dominio.Relatorios.Embarcador.DataSource.TorreControle.DevolucaoNotasFiscais>
    {

        #region Atributos

        private readonly Repositorio.Embarcador.Pedidos.XMLNotaFiscal _repositorioNotaFiscal;

        #endregion

        #region Construtores

        public DevolucaoNotasFiscais(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
        }

        #endregion

        protected override List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.DevolucaoNotasFiscais> ConsultarRegistros(FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioNotaFiscal.ConsultaRelatorioDevolucaoNotasFiscais(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioNotaFiscal.ContarConsultaRelatorioDevolucaoNotasFiscais(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/TorreControle/DevolucaoNotasFiscais";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }
    }
}
