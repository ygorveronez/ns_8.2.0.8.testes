using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.CTe;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Hangfire.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class ComissaoVendedorCTe : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioComissaoVendedorCTe, Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repComissaoVendedorCTe;

        #endregion

        #region Construtores


        public ComissaoVendedorCTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base (unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repComissaoVendedorCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        public ComissaoVendedorCTe(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repComissaoVendedorCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, cancellationToken);
        }

        #endregion

        #region
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe>> ConsultarRegistrosAsync(FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return await _repComissaoVendedorCTe.ConsultarRelatorioComissaoVendedorCTeAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.ComissaoVendedorCTe> ConsultarRegistros(FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repComissaoVendedorCTe.ConsultarRelatorioComissaoVendedorCTe(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repComissaoVendedorCTe.ContarConsultaRelatorioComissaoVendedorCTe(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/ComissaoVendedorCTe";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioComissaoVendedorCTe filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Usuario repositorioFuncionario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = filtrosPesquisa.CodigoGrupoPessoa > 0 ? repositorioGrupoPessoa.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoa) : null;
            Dominio.Entidades.Usuario vendedor = filtrosPesquisa.CodigoVendedor > 0 ? repositorioFuncionario.BuscarPorCodigo(filtrosPesquisa.CodigoVendedor) : null;

            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Parametro("GrupoPessoas", grupoPessoa?.Descricao));
            parametros.Add(new Parametro("Vendedor", vendedor?.Descricao));

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
