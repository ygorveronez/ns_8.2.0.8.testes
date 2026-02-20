using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class HistoricoVinculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.HistoricoVinculo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.HistoricoVinculo _repositorioHistoricoVinculo;

        #endregion

        #region Construtores

        public HistoricoVinculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioHistoricoVinculo = new Repositorio.Embarcador.Cargas.HistoricoVinculo(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.HistoricoVinculo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioHistoricoVinculo.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioHistoricoVinculo.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/HistoricoVinculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repFila = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork);


            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.Veiculo > 0d ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.Motorista > 0d ? repUsuario.BuscarPorCodigo(filtrosPesquisa.Motorista) : null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = filtrosPesquisa.Pedido > 0d ? repPedido.BuscarPorCodigo(filtrosPesquisa.Pedido) : null;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.Carga > 0d ? repCarga.BuscarPorCodigo(filtrosPesquisa.Carga) : null;
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo fila = filtrosPesquisa.FilaCarregamento > 0d ? repFila.BuscarPorCodigo(filtrosPesquisa.FilaCarregamento) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa ?? string.Empty, true));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Nome ?? string.Empty, true));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalVinculo", filtrosPesquisa.LocalVinculo?.ObterDescricao() ?? string.Empty ));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", pedido?.NumeroPedidoEmbarcador ?? string.Empty, true ));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", carga?.CodigoCargaEmbarcador ?? string.Empty, true ));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fila", fila?.Codigo.ToString() ?? string.Empty, true ));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataVinculoFormatada")
                propriedadeOrdenarOuAgrupar = "DataVinculo";

            if (propriedadeOrdenarOuAgrupar == "DataDesvinculoFormatada")
                propriedadeOrdenarOuAgrupar = "DataDesvinculo";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}