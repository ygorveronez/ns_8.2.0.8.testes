using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.PagamentosMotoristas
{
    public class PendenciaMotorista : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista, Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PendenciaMotorista>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista _repositorio;

        #endregion

        #region Construtores

        public PendenciaMotorista(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorio = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PendenciaMotorista> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorio.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorio.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/PagamentosMotoristas/PendenciaMotorista";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);


            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("Valor", filtrosPesquisa.ValorInicial, filtrosPesquisa.ValorFinal));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao?.ObterDescricao()));
            parametros.Add(new Parametro("Motorista", motorista?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}