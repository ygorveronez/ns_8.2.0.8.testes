using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Contatos
{
    public class TipoContatoCliente : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Contatos.FiltroPesquisaRelatorioTipoContatoCliente, Dominio.Relatorios.Embarcador.DataSource.Contatos.TipoContatoCliente.TipoContatoCliente>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Contatos.PessoaContato _repositorioTipoContatoCliente;

        #endregion

        #region Construtores

        public TipoContatoCliente(Repositorio.UnitOfWork _unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(_unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTipoContatoCliente = new Repositorio.Embarcador.Contatos.PessoaContato(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Contatos.TipoContatoCliente.TipoContatoCliente> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Contatos.FiltroPesquisaRelatorioTipoContatoCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioTipoContatoCliente.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Contatos.FiltroPesquisaRelatorioTipoContatoCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioTipoContatoCliente.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Contatos/TipoContatoCliente";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Contatos.FiltroPesquisaRelatorioTipoContatoCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Contatos.TipoContato repTipoContato = new Repositorio.Embarcador.Contatos.TipoContato(_unitOfWork);
            Repositorio.Embarcador.Contatos.PessoaContato repPessoaContato = new Repositorio.Embarcador.Contatos.PessoaContato(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas) : null;
            Dominio.Entidades.Cliente pessoa = filtrosPesquisa.CpfCnpjPessoa > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjPessoa) : null;
            List<string> tiposContato = filtrosPesquisa.TiposContato.Count > 0 ? repTipoContato.BuscarDescricaoPorCodigo(filtrosPesquisa.TiposContato) : new List<string>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", tiposContato));

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
