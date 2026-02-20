using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Atendimento
{
    public class Chamado : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Atendimento.FiltroPesquisaRelatorioChamado, Dominio.Relatorios.Embarcador.DataSource.Atendimentos.RelatorioChamado>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Atendimento.AtendimentoTarefa _repositorioChamado;

        #endregion

        #region Construtores

        public Chamado(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioChamado = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Atendimentos.RelatorioChamado> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Atendimento.FiltroPesquisaRelatorioChamado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioChamado.RelatorioChamado(filtrosPesquisa.Empresa, filtrosPesquisa.EmpresaFilho, filtrosPesquisa.Tela, filtrosPesquisa.Modulo, filtrosPesquisa.Sistema, filtrosPesquisa.Tipo, filtrosPesquisa.Funcionario, filtrosPesquisa.Titulo, filtrosPesquisa.DataChamadoInicial, filtrosPesquisa.DataChamadoFinal, filtrosPesquisa.DataAtendimentoInicial, filtrosPesquisa.DataAtendimentoFinal, filtrosPesquisa.Status, filtrosPesquisa.Prioridade, filtrosPesquisa.Solicitante, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Atendimento.FiltroPesquisaRelatorioChamado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioChamado.ContarRelatorioChamado(filtrosPesquisa.Empresa, filtrosPesquisa.EmpresaFilho, filtrosPesquisa.Tela, filtrosPesquisa.Modulo, filtrosPesquisa.Sistema, filtrosPesquisa.Tipo, filtrosPesquisa.Funcionario, filtrosPesquisa.Titulo, filtrosPesquisa.DataChamadoInicial, filtrosPesquisa.DataChamadoFinal, filtrosPesquisa.DataAtendimentoInicial, filtrosPesquisa.DataAtendimentoFinal, filtrosPesquisa.Status, filtrosPesquisa.Prioridade, filtrosPesquisa.Solicitante);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Atendimento/Chamado";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Atendimento.FiltroPesquisaRelatorioChamado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(_unitOfWork);
            Repositorio.Embarcador.Atendimento.AtendimentoTela repTela = new Repositorio.Embarcador.Atendimento.AtendimentoTela(_unitOfWork);
            Repositorio.Embarcador.Atendimento.AtendimentoTipo repTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(_unitOfWork);
            Repositorio.Embarcador.Atendimento.AtendimentoSistema repSistema = new Repositorio.Embarcador.Atendimento.AtendimentoSistema(_unitOfWork);
            Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(_unitOfWork);

            if (filtrosPesquisa.Tela > 0)
            {
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTela tela = repTela.BuscarPorCodigo(filtrosPesquisa.Tela);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tela", tela.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tela", false));

            if (filtrosPesquisa.Modulo > 0)
            {
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo modulo = repModulo.BuscarPorCodigo(filtrosPesquisa.Modulo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Modulo", modulo.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Modulo", false));

            if (filtrosPesquisa.Sistema > 0)
            {
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoSistema sistema = repSistema.BuscarPorCodigo(filtrosPesquisa.Sistema);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Sistema", sistema.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Sistema", false));

            if (filtrosPesquisa.Tipo > 0)
            {
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo tipo = repTipo.BuscarPorCodigo(filtrosPesquisa.Tipo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", tipo.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", false));

            if (filtrosPesquisa.Funcionario > 0)
            {
                Dominio.Entidades.Usuario funcionario = repFuncionario.BuscarPorCodigo(filtrosPesquisa.Funcionario);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Funcionario", funcionario.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Funcionario", false));

            if (filtrosPesquisa.DataChamadoInicial != DateTime.MinValue || filtrosPesquisa.DataChamadoFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataChamadoInicial != DateTime.MinValue ? filtrosPesquisa.DataChamadoInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataChamadoFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataChamadoFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataChamado", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataChamado", false));

            if (filtrosPesquisa.DataAtendimentoInicial != DateTime.MinValue || filtrosPesquisa.DataAtendimentoFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataAtendimentoInicial != DateTime.MinValue ? filtrosPesquisa.DataAtendimentoInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataAtendimentoFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataAtendimentoFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAtendimento", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAtendimento", false));

            if ((int)filtrosPesquisa.Status > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", filtrosPesquisa.Status.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", false));

            if ((int)filtrosPesquisa.Prioridade > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Prioridade", filtrosPesquisa.Prioridade.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Prioridade", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Titulo))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", filtrosPesquisa.Titulo, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", false));

            if (filtrosPesquisa.Solicitante > 0)
            {
                Dominio.Entidades.Usuario funcionario = repFuncionario.BuscarPorCodigo(filtrosPesquisa.Solicitante);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Solicitante", funcionario.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Solicitante", false));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}