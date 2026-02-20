using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.ConsultaTabelaFrete)]
public class ConsultaTabelaFreteReport:ReportBase
{
    public ConsultaTabelaFreteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = extraData.GetValue<string>("filtrosPesquisa").FromJson<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente >();
        List<PropriedadeAgrupamento> propriedades = extraData.GetValue<string>("propriedades").FromJson<List<PropriedadeAgrupamento>>();
        Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = extraData.GetValue<string>("parametrosConsulta").FromJson<Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta>();
        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        var relatorioTemporario = extraData.GetValue<string>("relatorioTemporario").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();

        string query = string.Empty;
        Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> listaConsultaTabelaFrete = repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta, out query);

        Servicos.Log.TratarErro(query, "ConsultaTabelaFrete");

        CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemporario, listaConsultaTabelaFrete, _unitOfWork);
        List<Parametro> parametros = ObterParametrosConsulta(filtrosPesquisa, _unitOfWork);

        _servicoRelatorioReportService.PreecherParamentrosFiltro(relatorio, relatorioControleGeracao, relatorioTemporario, parametros);
        _servicoRelatorioReportService.GerarRelatorio(relatorio, relatorioControleGeracao, "Fretes/ConsultaTabelaFrete", _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }
    private List<Parametro> ObterParametrosConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Parametro> parametros = new List<Parametro>();
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoTransporteFrete repositorioContratoTransportadorFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? repositorioGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas) : null;
            Dominio.Entidades.Localidade localidadeOrigem = filtrosPesquisa.CodigoLocalidadeOrigem > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadeOrigem) : null;
            List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> contratosTransporteFrete = filtrosPesquisa.CodigosContratoTransportador.Count > 0 ? repositorioContratoTransportadorFrete.BuscarPorCodigos(filtrosPesquisa.CodigosContratoTransportador) : new List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();
            Dominio.Entidades.Localidade localidadeDestino = filtrosPesquisa.CodigoLocalidadeDestino > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadeDestino) : null;
            Dominio.Entidades.Embarcador.Localidades.Regiao regiao = filtrosPesquisa.CodigoRegiaoDestino > 0 ? repositorioRegiao.BuscarPorCodigo(filtrosPesquisa.CodigoRegiaoDestino) : null;
            Dominio.Entidades.Estado ufOrigem = !string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoEstadoOrigem) ? repositorioEstado.BuscarPorSigla(filtrosPesquisa.CodigoEstadoOrigem) : null;
            Dominio.Entidades.Estado ufDestino = !string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoEstadoDestino) ? repositorioEstado.BuscarPorSigla(filtrosPesquisa.CodigoEstadoDestino) : null;
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = filtrosPesquisa.CodigoTipoCarga > 0 ? repositorioTipoCarga.BuscarPorCodigo(filtrosPesquisa.CodigoTipoCarga) : null;
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosReboque = filtrosPesquisa.CodigosModeloReboque?.Count > 0 ? repositorioModeloVeicular.BuscarPorCodigos(filtrosPesquisa.CodigosModeloReboque) : new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloTracao = filtrosPesquisa.CodigoModeloTracao > 0 ? repositorioModeloVeicular.BuscarPorCodigo(filtrosPesquisa.CodigoModeloTracao) : null;
            Dominio.Entidades.Cliente clienteRemetente = filtrosPesquisa.CpfCnpjRemetente > 0d ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjRemetente) : null;
            Dominio.Entidades.Cliente clienteDestinatario = filtrosPesquisa.CpfCnpjDestinatario > 0d ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario) : null;
            Dominio.Entidades.Cliente clienteTomador = filtrosPesquisa.CpfCnpjTomador > 0d ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjTomador) : null;
            Dominio.Entidades.Cliente clienteTransportadorTerceiro = filtrosPesquisa.CpfCnpjTransportadorTerceiro > 0d ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjTransportadorTerceiro) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            List<string> descricoes = filtrosPesquisa.CodigosComplemento?.Count > 0 ? repositorioComponenteFrete.BuscarDescricaoPorCodigo(filtrosPesquisa.CodigosComplemento.ToArray()) : new List<string>();
            string descricaoTabelaFrete = repositorioTabelaFrete.BuscarDescricaoPorCodigo(filtrosPesquisa.CodigoTabelaFrete);

            parametros.Add(new Parametro("TabelaFrete", descricaoTabelaFrete, true));
            parametros.Add(new Parametro("Vigencia", filtrosPesquisa.DataInicialVigencia, filtrosPesquisa.DataFinalVigencia));
            parametros.Add(new Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Parametro("Origem", localidadeOrigem?.Descricao));
            parametros.Add(new Parametro("Destino", localidadeDestino?.Descricao));
            parametros.Add(new Parametro("RegiaoDestino", regiao?.Descricao));
            parametros.Add(new Parametro("EstadoOrigem", ufOrigem?.Nome));
            parametros.Add(new Parametro("EstadoDestino", ufDestino?.Nome));
            parametros.Add(new Parametro("TipoCarga", tipoCarga?.Descricao));
            parametros.Add(new Parametro("ModeloReboque", string.Join(", ", modelosReboque.Select(o => o.Descricao))));
            parametros.Add(new Parametro("ModeloTracao", modeloTracao?.Descricao));
            parametros.Add(new Parametro("Remetente", clienteRemetente?.Descricao));
            parametros.Add(new Parametro("Destinatario", clienteDestinatario?.Descricao));
            parametros.Add(new Parametro("Tomador", clienteTomador?.Descricao));
            parametros.Add(new Parametro("Empresa", empresa?.Descricao));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Parametro("TipoPagamento", filtrosPesquisa.TipoPagamentoEmissao.HasValue ? filtrosPesquisa.TipoPagamentoEmissao.Value == TipoPagamentoEmissao.A_Pagar ? "A pagar" : filtrosPesquisa.TipoPagamentoEmissao.Value == TipoPagamentoEmissao.Pago ? "Pago" : "Outros" : null));
            parametros.Add(new Parametro("ComponenteFrete", string.Join(", ", descricoes)));
            parametros.Add(new Parametro("TabelaComCargaRealizada", filtrosPesquisa.TabelaComCargaRealizada ? "Sim" : null));
            parametros.Add(new Parametro("TransportadorTerceiro", clienteTransportadorTerceiro?.Descricao));
            parametros.Add(new Parametro("SomenteEmVigencia", filtrosPesquisa.SomenteEmVigencia ? "Sim" : null));
            parametros.Add(new Parametro("ContratoFrete", string.Join(", ", contratosTransporteFrete.Select(x => x.NomeContrato))));

            //Atualizar a informação também no AjusteTabelaFreteController

            return parametros;
        }

}