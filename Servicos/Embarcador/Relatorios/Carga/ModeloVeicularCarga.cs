using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Cargas
{
    public class ModeloVeicularCarga : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaModeloVeicularCarga, Dominio.Relatorios.Embarcador.DataSource.Cargas.ModeloVeicularCarga.ModeloVeicularCarga>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.ModeloVeicularCarga _repositorioModeloVeicularCarga;

        #endregion

        #region Construtores

        public ModeloVeicularCarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ModeloVeicularCarga.ModeloVeicularCarga> ConsultarRegistros(FiltroPesquisaModeloVeicularCarga filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioModeloVeicularCarga.ConsultarRelatorioModeloVeicularCarga(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaModeloVeicularCarga filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioModeloVeicularCarga.ContarConsultaRelatorioModeloVeicularCarga(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/ModeloVeicularCarga";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaModeloVeicularCarga filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modeloVeicularCarga = !filtrosPesquisa.ModeloVeicular.IsNullOrEmpty() ? repositorioModeloVeicularCarga.BuscarPorCodigos(filtrosPesquisa.ModeloVeicular) : null;

            parametros.Add(new Parametro("ModeloVeicular", !modeloVeicularCarga.IsNullOrEmpty()  ? string.Join(",", modeloVeicularCarga.Select(x => x.Descricao)) : ""));
            parametros.Add(new Parametro("Ativo", filtrosPesquisa.Ativo));
            parametros.Add(new Parametro("Tipo", string.Join(", ", filtrosPesquisa.Tipo)));
            
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
    }
}
