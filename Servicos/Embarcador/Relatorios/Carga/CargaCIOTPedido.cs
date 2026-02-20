using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargaCIOTPedido : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido, Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOTPedido>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaCIOT _repCargaCIOTPedido;

        #endregion

        #region Construtores

        public CargaCIOTPedido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repCargaCIOTPedido = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOTPedido> ConsultarRegistros(FiltroPesquisaRelatorioCargaCIOTPedido filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repCargaCIOTPedido.ConsultarRelatorioCargaCIOTPedido(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioCargaCIOTPedido filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repCargaCIOTPedido.ContarConsultaRelatorioCargaCIOTPedido(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Documentos/CargaCIOTPedido";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioCargaCIOTPedido filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            if (filtrosPesquisa.DataEncerramentoInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoInicial", filtrosPesquisa.DataEncerramentoInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoInicial", false));

            if (filtrosPesquisa.DataEncerramentoFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoFinal", filtrosPesquisa.DataEncerramentoFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEncerramentoFinal", false));

            if (filtrosPesquisa.DataAberturaInicial != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaInicial", filtrosPesquisa.DataAberturaInicial.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaInicial", false));

            if (filtrosPesquisa.DataAberturaFinal != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaFinal", filtrosPesquisa.DataAberturaFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAberturaFinal", false));

            if (filtrosPesquisa.Proprietario > 0)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                Dominio.Entidades.Cliente _proprietario = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Proprietario);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Proprietario", _proprietario.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Proprietario", false));

            if (filtrosPesquisa.Veiculo > 0)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Dominio.Entidades.Veiculo _veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", _veiculo.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            if (filtrosPesquisa.Motorista > 0)
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
                Dominio.Entidades.Usuario _motorista = repUsuario.BuscarPorCodigo(filtrosPesquisa.Motorista);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", _motorista.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Numero))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", filtrosPesquisa.Numero, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", filtrosPesquisa.Carga, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", false));

            if (filtrosPesquisa.Situacao != null)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOTHelper.ObterDescricao(filtrosPesquisa.Situacao), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

            if (filtrosPesquisa.Transportador > 0)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(filtrosPesquisa.Transportador);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador.RazaoSocial, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "MotoristaFormatado")
                return "MotoristaCPFCNPJ";

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
