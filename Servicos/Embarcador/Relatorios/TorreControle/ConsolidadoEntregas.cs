using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Servicos.Embarcador.Relatorios.TorreControle
{
    public class ConsolidadoEntregas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas, Dominio.Relatorios.Embarcador.DataSource.TorreControle.ConsolidadoEntregas>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega _repositorioCargaEntrega;

        #endregion

        #region Construtores

        public ConsolidadoEntregas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.ConsolidadoEntregas> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaEntrega.ConsultarRelatorioConsolidadoEntregas(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaEntrega.ContarConsultaRelatorioConsolidadoEntregas(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/TorreControle/ConsolidadoEntregas";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNF = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = filtrosPesquisa.CodigoCarga.Count > 0 ? repCarga.BuscarPorCodigos(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = filtrosPesquisa.CodigoPedido > 0 ? repPedido.BuscarPorCodigo(filtrosPesquisa.CodigoPedido) : null;
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNF = filtrosPesquisa.CodigoNotaFiscal > 0 ? repXMLNF.BuscarPorCodigo(filtrosPesquisa.CodigoNotaFiscal) : null;
            List<Dominio.Entidades.Empresa> transportadores = filtrosPesquisa.CodigoTransportador.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Cliente clienteOrigem = filtrosPesquisa.CpfCnpjClienteOrigem > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjClienteOrigem) : null;
            Dominio.Entidades.Cliente clienteDestino = filtrosPesquisa.CpfCnpjClienteDestino > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjClienteDestino) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", cargas != null ? string.Join(", ", cargas.Select(c => c.CodigoCargaEmbarcador)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicioViagemPrevista", filtrosPesquisa?.DataInicioViagemPrevistaInicial, filtrosPesquisa?.DataInicioViagemPrevistaFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicioViagemRealizada", filtrosPesquisa?.DataInicioViagemRealizadaInicial, filtrosPesquisa?.DataInicioViagemRealizadaFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataConfirmacao", filtrosPesquisa?.DataConfirmacaoInicial, filtrosPesquisa?.DataConfirmacaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", pedido?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NotaFiscal", xmlNF?.Numero));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportadores != null ? string.Join(", ", transportadores.Select(t => t.Descricao)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ClienteOrigem", clienteOrigem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ClienteDestino", clienteDestino?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoInteracaoInicioViagem", filtrosPesquisa?.TipoInteracaoInicioViagem?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoInteracaoChegadaViagem", filtrosPesquisa?.TipoInteracaoChegadaViagem?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoViagem", filtrosPesquisa?.StatusViagem?.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            propriedadeOrdenarOuAgrupar.Replace("Formatado", "");
            propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}