using System.Collections.Generic;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class AliquotaICMSCTe : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe, Dominio.Relatorios.Embarcador.DataSource.Financeiros.AliquotaICMSCTe>
    {
        #region Atributos

        private readonly Repositorio.Aliquota _repositorioAliquotaICMSCTe;

        #endregion

        #region Construtores

        public AliquotaICMSCTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAliquotaICMSCTe = new Repositorio.Aliquota(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AliquotaICMSCTe> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAliquotaICMSCTe.ConsultarRelatorio(filtrosPesquisa.EstadoEmpresa, filtrosPesquisa.EstadoOrigem, filtrosPesquisa.EstadoDestino, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAliquotaICMSCTe.ContarConsultaRelatorio(filtrosPesquisa.EstadoEmpresa, filtrosPesquisa.EstadoOrigem, filtrosPesquisa.EstadoDestino);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/AliquotaICMSCTe";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork); 

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoDestino) && filtrosPesquisa.EstadoDestino != "0")
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(filtrosPesquisa.EstadoDestino);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", estado.Sigla + " - " + estado.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoEmpresa) && filtrosPesquisa.EstadoEmpresa != "0")
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(filtrosPesquisa.EstadoEmpresa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoEmpresa", estado.Sigla + " - " + estado.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoEmpresa", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoOrigem) && filtrosPesquisa.EstadoOrigem != "0")
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(filtrosPesquisa.EstadoOrigem);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", estado.Sigla + " - " + estado.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "Aliquota")
                return "Percentual";

            if (propriedadeOrdenarOuAgrupar == "Atividade")
                return "AtividadeTomador.Codigo";

            if (propriedadeOrdenarOuAgrupar == "AtividadeDestinatario")
                return "AtividadeDestinatario.Codigo";

            if (propriedadeOrdenarOuAgrupar == "CFOP")
                return "CFOP.CodigoCFOP";

            if (propriedadeOrdenarOuAgrupar == "CFOPCompra")
                return "CFOP.CodigoCFOP";

            if (propriedadeOrdenarOuAgrupar == "EstadoDestino")
                return "EstadoDestino.Sigla";

            if (propriedadeOrdenarOuAgrupar == "EstadoOrigem")
                return "EstadoOrigem.Sigla";

            if (propriedadeOrdenarOuAgrupar == "EstadoEmpresa")
                return "EstadoEmpresa.Sigla";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}