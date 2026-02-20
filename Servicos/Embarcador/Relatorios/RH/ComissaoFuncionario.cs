using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.RH
{
    public class ComissaoFuncionario : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario, Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionario>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento _repComissaoFuncionarioMotoristaDocumento;

        #endregion

        #region Construtores

        public ComissaoFuncionario(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionario> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repComissaoFuncionarioMotoristaDocumento.ConsultarRelatorioComissaoFuncionario(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repComissaoFuncionarioMotoristaDocumento.ContarConsultaRelatorioComissaoFuncionario(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/RH/ComissaoFuncionario";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();
            string NomeMotoristas = null;
            if (filtrosPesquisa.Motorista.Count > 0)
            {
                Repositorio.Usuario repUsuarios = new Repositorio.Usuario(_unitOfWork);
                List<Dominio.Entidades.Usuario> motoristas = repUsuarios.BuscarMotoristaPorCodigo(filtrosPesquisa.Motorista);
                NomeMotoristas = String.Join(", ", motoristas.ConvertAll<string>(el => el.Nome));
            }
            parametros.Add(new Parametro("Motoristas", NomeMotoristas));
            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}