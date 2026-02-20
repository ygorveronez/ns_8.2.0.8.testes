using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pessoas
{
    public class EnderecoSecundario : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroEnderecoSecundario, Dominio.Relatorios.Embarcador.DataSource.Pessoas.EnderecosSecundarios>
    {
        #region Atributos
        
        private readonly Repositorio.Embarcador.Pessoas.ClienteOutroEndereco _repositorioEnderecoSecundario;

        #endregion

        #region Construtores

        public EnderecoSecundario(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioEnderecoSecundario = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.EnderecosSecundarios> ConsultarRegistros(FiltroEnderecoSecundario filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioEnderecoSecundario.ConsultarRelatorioEnderecoSecundario(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroEnderecoSecundario filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioEnderecoSecundario.ContarConsultaRelatorioEnderecoSecundario(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pessoas/EnderecoSecundario";
        }

        protected override List<Parametro> ObterParametros(FiltroEnderecoSecundario filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);

            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CPFCliente > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CPFCliente) : null;
            Dominio.Entidades.Localidade cidade = filtrosPesquisa.Cidade > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.Cidade) : null;

            List<Parametro> parametros = new List<Parametro>();

            parametros.Add(new Parametro("CodigoIntegracao", filtrosPesquisa?.CodigoIntegracao));
            parametros.Add(new Parametro("Cidade", cidade?.Descricao));
            parametros.Add(new Parametro("CPFCliente", cliente?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Descricao") && propriedadeOrdenarOuAgrupar != "Descricao")
                return propriedadeOrdenarOuAgrupar.Replace("Descricao", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }
    }
}
