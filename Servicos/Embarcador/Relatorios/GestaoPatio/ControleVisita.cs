using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.GestaoPatio
{
    public class ControleVisita : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita, Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ControleVisita>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.GestaoPatio.ControleVisita _repositorioControleVisita;

        #endregion

        #region Construtores

        public ControleVisita(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(_unitOfWork);
        }
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ControleVisita> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioControleVisita.ConsultarRelatorioControleVisita(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioControleVisita.ContarConsultaRelatorioControleVisita(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/GestaoPatio/ControleVisita";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioControleVisita filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            string pattern = "dd/MM/yyyy HH:MM";

            Repositorio.Setor repSetor = new Repositorio.Setor(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            if (filtrosPesquisa.DataInicialEntrada != DateTime.MinValue || filtrosPesquisa.DataFinalEntrada != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicialEntrada != DateTime.MinValue ? filtrosPesquisa.DataInicialEntrada.ToString(pattern) + " " : "";
                data += filtrosPesquisa.DataFinalEntrada != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinalEntrada.ToString(pattern) : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntrada", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntrada", false));

            if (filtrosPesquisa.DataInicialSaida != DateTime.MinValue || filtrosPesquisa.DataFinalSaida != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicialSaida != DateTime.MinValue ? filtrosPesquisa.DataInicialSaida.ToString(pattern) + " " : "";
                data += filtrosPesquisa.DataFinalSaida != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinalSaida.ToString(pattern) : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataSaida", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataSaida", false));

            if (filtrosPesquisa.Setor > 0)
            {
                Dominio.Entidades.Setor obj = repSetor.BuscarPorCodigo(filtrosPesquisa.Setor);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Setor", obj.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Setor", false));

            if (filtrosPesquisa.Autorizador > 0)
            {
                Dominio.Entidades.Usuario obj = repUsuario.BuscarPorCodigo(filtrosPesquisa.Autorizador);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Autorizador", obj.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Autorizador", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CPF.ToString()))
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CPF", filtrosPesquisa.CPF.ToString(), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CPF", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }
        #endregion
    }
}
