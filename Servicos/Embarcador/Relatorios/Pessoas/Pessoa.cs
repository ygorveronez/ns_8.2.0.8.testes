using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pessoas
{
    public class Pessoa : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa, Dominio.Relatorios.Embarcador.DataSource.Pessoas.Pessoa>
    {
        #region Atributos

        private readonly Repositorio.Cliente _repositorioPessoa;

        #endregion

        #region Construtores

        public Pessoa(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPessoa = new Repositorio.Cliente(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.Pessoa> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioPessoa.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPessoa.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pessoas/Pessoa";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repCategoria = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoa) : null;
            Dominio.Entidades.Atividade atividade = filtrosPesquisa.CodigoAtividade > 0 ? repAtividade.BuscarPorCodigo(filtrosPesquisa.CodigoAtividade) : null;
            Dominio.Entidades.Localidade localidade = filtrosPesquisa.CodigoLocalidade > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidade) : null;
            List<Dominio.Entidades.Estado> estados = filtrosPesquisa.Estado.Count > 0 ? repEstado.BuscarPorSiglas(filtrosPesquisa.Estado) : new List<Dominio.Entidades.Estado>();
            Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoria = filtrosPesquisa.CodigoCategoria > 0 ? repCategoria.BuscarPorCodigo(filtrosPesquisa.CodigoCategoria) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Atividade", atividade?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Localidade", localidade?.DescricaoCidadeEstado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Estado", string.Join(",", estados.Select(x => x.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao.HasValue ? filtrosPesquisa.Situacao.Value ? "Ativo" : "Inativo" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Modalidade", string.Join(",", filtrosPesquisa.ModalidadePessoa.Select(x => x.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", filtrosPesquisa.TipoPessoa.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCadastro", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteSemCodigoIntegracao", filtrosPesquisa.SomenteSemCodigoIntegracao.HasValue ? filtrosPesquisa.SomenteSemCodigoIntegracao.Value ? "Sim" : "Não" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bloqueado", filtrosPesquisa.Bloqueado.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AguardandoConferenciaInformacao", filtrosPesquisa.AguardandoConferenciaInformacao.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Categoria", categoria?.Descricao));

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