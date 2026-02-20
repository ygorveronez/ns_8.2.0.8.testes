using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.NFe
{
    public class NotasEmitidas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas, Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidas>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.NotaFiscal.NotaFiscal _repositorioNotasEmitidas;

        #endregion

        #region Construtores

        public NotasEmitidas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioNotasEmitidas = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidas> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioNotasEmitidas.RelatorioNotasEmitidas(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioNotasEmitidas.ContarRelatorioNotasEmitidas(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/NFe/NotasEmitidas";
        }

        protected override List<KeyValuePair<string, dynamic>> ObterSubReportDataSources(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoAgrupar = "",
                DirecaoOrdenar = "",
                InicioRegistros = 0,
                LimiteRegistros = 0,
                PropriedadeAgrupar = "",
                PropriedadeOrdenar = ""
            };
            IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidas> listaNotasEmitidas = _repositorioNotasEmitidas.RelatorioNotasEmitidas(filtrosPesquisa, parametrosConsulta);
            IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasItens> listaItensNF = new List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasItens>();

            if (filtrosPesquisa.ExibirItens)
                listaItensNF = _repositorioNotasEmitidas.ConsultarItensRelatorioNotasEmitidas(string.Join(", ", listaNotasEmitidas.Select(o => o.CodigoNFe).ToList()), string.Join(", ", listaNotasEmitidas.Select(o => o.CodigoNFSe).ToList()));

            return new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("Itens", listaItensNF) };
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Atividade repAtividade = new Repositorio.Atividade(_unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);

            List<Dominio.Entidades.CFOP> cfops = filtrosPesquisa.CodigosCFOP.Count > 0 ? repCFOP.BuscarPorCodigos(filtrosPesquisa.CodigosCFOP) : new List<Dominio.Entidades.CFOP>();

            parametros.Add(new Parametro("NumeroInicial", filtrosPesquisa.NumeroInicial));
            parametros.Add(new Parametro("NumeroFinal", filtrosPesquisa.NumeroFinal));
            parametros.Add(new Parametro("Serie", filtrosPesquisa.Serie));

            if (filtrosPesquisa.CodigoAtividade > 0)
            {
                Dominio.Entidades.Atividade atividade = repAtividade.BuscarPorCodigo(filtrosPesquisa.CodigoAtividade);
                parametros.Add(new Parametro("Atividade", "(" + atividade.Codigo.ToString() + ") " + atividade.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Atividade", false));

            if (filtrosPesquisa.CodigoNaturezaOperacao > 0)
            {
                Dominio.Entidades.NaturezaDaOperacao natureza = repNaturezaDaOperacao.BuscarPorId(filtrosPesquisa.CodigoNaturezaOperacao);
                parametros.Add(new Parametro("Natureza", "(" + natureza.Codigo.ToString() + ") " + natureza.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Natureza", false));

            if (filtrosPesquisa.CnpjPessoa > 0)
            {
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPessoa);
                parametros.Add(new Parametro("Cliente", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
            }
            else
                parametros.Add(new Parametro("Cliente", false));

            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("DataProcessamento", filtrosPesquisa.DataProcessamento));
            parametros.Add(new Parametro("DataSaida", filtrosPesquisa.DataSaida));

            if ((int)filtrosPesquisa.Status > 0)
                parametros.Add(new Parametro("Status", filtrosPesquisa.Status.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("Status", false));

            if (filtrosPesquisa.TipoEmissao.HasValue)
                parametros.Add(new Parametro("TipoEmissao", filtrosPesquisa.TipoEmissao.Value.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("TipoEmissao", false));

            parametros.Add(new Parametro("Chave", filtrosPesquisa.Chave));

            if ((int)filtrosPesquisa.TipoDocumento > 0)
                parametros.Add(new Parametro("TipoDocumento", filtrosPesquisa.TipoDocumento.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("TipoDocumento", false));

            if (filtrosPesquisa.CodigosUsuario != null && filtrosPesquisa.CodigosUsuario.Count > 0)
            {
                List<Dominio.Entidades.Usuario> usuarios = filtrosPesquisa.CodigosUsuario.Count > 0 ? repUsuario.BuscarUsuariosPorCodigos(filtrosPesquisa.CodigosUsuario.ToArray(), null) : new List<Dominio.Entidades.Usuario>();
                parametros.Add(new Parametro("Usuario", string.Join(", ", from obj in usuarios select obj.Nome), true));
            }
            else
                parametros.Add(new Parametro("Usuario", false));

            if (filtrosPesquisa.ExibirItens)
                parametros.Add(new Parametro("ExibirItens", "Sim", true));
            else
                parametros.Add(new Parametro("ExibirItens", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Parametro("Agrupamento", false));

            parametros.Add(new Parametro("CFOP", cfops.Select(o => o.CodigoCFOP.ToString())));


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