using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class CTe : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio, Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioCTe;

        #endregion

        #region Construtores

        public CTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            try
            {
                return _repositorioCTe.ContarConsultaRelatorioCTes(filtrosPesquisa, propriedadesAgrupamento);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                throw;
            }
        }
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTe> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            try
            {
                return _repositorioCTe.ConsultarRelatorioCTes(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                throw;
            }
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/CTes";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            try
            {

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
                Repositorio.Usuario repFuncionario = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCustoViagem repCentroDeCustoViagem = new Repositorio.Embarcador.Logistica.CentroCustoViagem(_unitOfWork);

                List<string> empresas = filtrosPesquisa.CodigosTransportador?.Count > 0 ? repositorioEmpresa.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTransportador) : null;
                List<string> vendedores = filtrosPesquisa.Vendedor?.Count > 0 ? repFuncionario.BuscarDescricaoPorCodigos(filtrosPesquisa.Vendedor) : null;
                List<string> tipoOcorrenciaCTe = filtrosPesquisa.CodigosTipoOcorrencia.Count > 0 ? repTipoOcorrencia.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTipoOcorrencia) : new List<string>();
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = filtrosPesquisa.codigoContratoFrete > 0 ? repContratoFreteTransportador.BuscarPorCodigo(filtrosPesquisa.codigoContratoFrete) : null;
                List<string> cargas = filtrosPesquisa.codigoCarga?.Count > 0 ? repCarga.BuscarNumerosPorCodigos(filtrosPesquisa.codigoCarga) : null;
                Dominio.Entidades.Localidade origem = filtrosPesquisa.codigoOrigem > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.codigoOrigem) : null;
                Dominio.Entidades.Localidade destino = filtrosPesquisa.codigoDestino > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.codigoDestino) : null;
                Dominio.Entidades.Cliente remetente = filtrosPesquisa.cpfCnpjRemetente > 0D ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.cpfCnpjRemetente) : null;
                List<Dominio.Entidades.Cliente> destinatarios = filtrosPesquisa.cpfCnpjDestinatarios.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.cpfCnpjDestinatarios) : new List<Dominio.Entidades.Cliente>();
                List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumento = filtrosPesquisa.modeloDocumento?.Count > 0 ? repModeloDocumentoFiscal.BuscarPorCodigo(filtrosPesquisa.modeloDocumento.ToArray()) : new List<Dominio.Entidades.ModeloDocumentoFiscal>();
                List<string> tiposOperacao = filtrosPesquisa.codigosTipoOperacao?.Count > 0 ? repTipoOperacao.BuscarDescricoesPorCodigos(filtrosPesquisa.codigosTipoOperacao) : null;
                List<string> filiais = filtrosPesquisa.codigosFilial?.Count > 0 ? repFilial.BuscarDescricoesPorCodigos(filtrosPesquisa.codigosFilial) : null;
                List<string> segmentosVeiculo = filtrosPesquisa.SegmentoVeiculo?.Count > 0 ? repSegmentoVeiculo.BuscarDescricoesPorCodigos(filtrosPesquisa.SegmentoVeiculo) : new List<string>();
                Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = filtrosPesquisa.CodigoPortoOrigem > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoOrigem) : null;
                Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = filtrosPesquisa.CodigoPortoDestino > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoDestino) : null;
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = filtrosPesquisa.CodigoViagem > 0 ? repViagem.BuscarPorCodigo(filtrosPesquisa.CodigoViagem) : null;
                Dominio.Entidades.Embarcador.Pedidos.Container container = filtrosPesquisa.CodigoContainer > 0 ? repContainer.BuscarPorCodigo(filtrosPesquisa.CodigoContainer) : null;
                Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
                List<string> gruposPessoas = filtrosPesquisa.gruposPessoas.Count > 0 ? repGrupoPessoa.BuscarDescricaoPorCodigo(filtrosPesquisa.gruposPessoas) : new List<string>();
                List<string> gruposPessoasDiferente = filtrosPesquisa.gruposPessoasDiferente.Count > 0 ? repGrupoPessoa.BuscarDescricaoPorCodigo(filtrosPesquisa.gruposPessoasDiferente) : new List<string>();
                List<string> gruposPessoasRemetente = filtrosPesquisa.GruposPessoasRemetente.Count > 0 ? repGrupoPessoa.BuscarDescricaoPorCodigo(filtrosPesquisa.GruposPessoasRemetente) : new List<string>();
                List<string> centrosResultados = filtrosPesquisa.CodigosCentroResultado.Count > 0 ? repCentroResultado.BuscarDescricaoPorCodigos(filtrosPesquisa.CodigosCentroResultado) : new List<string>();
                List<string> tiposDeCarga = filtrosPesquisa.codigosTipoCarga?.Count > 0 ? repTipoDeCarga.BuscarDescricoesPorCodigos(filtrosPesquisa.codigosTipoCarga) : null;
                Dominio.Entidades.Estado estadoOrigem = !string.IsNullOrWhiteSpace(filtrosPesquisa.estadoOrigem) && filtrosPesquisa.estadoOrigem != "0" ? repEstado.BuscarPorSigla(filtrosPesquisa.estadoOrigem) : null;
                Dominio.Entidades.Estado estadoDestino = !string.IsNullOrWhiteSpace(filtrosPesquisa.estadoDestino) && filtrosPesquisa.estadoDestino != "0" ? repEstado.BuscarPorSigla(filtrosPesquisa.estadoDestino) : null;
                Dominio.Entidades.CFOP cfop = filtrosPesquisa.codigoCFOP > 0 ? repCFOP.BuscarPorCodigo(filtrosPesquisa.codigoCFOP) : null;
                Dominio.Entidades.Cliente terceiro = filtrosPesquisa.cpfCnpjTerceiro > 0D ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.cpfCnpjTerceiro) : null;
                List<int> ctes = filtrosPesquisa.codigosCTes?.Count > 0 ? _repositorioCTe.BuscarNumerosPorCodigos(filtrosPesquisa.codigosCTes) : new List<int>();
                Dominio.Entidades.ModeloVeiculo modeloVeiculo = filtrosPesquisa.ModeloVeiculo > 0 ? repModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.ModeloVeiculo, false) : null;
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = filtrosPesquisa.CodigoContratoFreteTerceiro > 0 ? repContratoFrete.BuscarPorCodigo(filtrosPesquisa.CodigoContratoFreteTerceiro) : null;
                List<string> funcionarios = filtrosPesquisa.FuncionarioResponsavel?.Count > 0 ? repFuncionario.BuscarDescricaoPorCodigos(filtrosPesquisa.FuncionarioResponsavel) : null;
                List<Dominio.Entidades.Cliente> tomadores = filtrosPesquisa.CpfCnpjTomadores.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjTomadores) : new List<Dominio.Entidades.Cliente>();
                Dominio.Entidades.Cliente provedor = filtrosPesquisa.ProvedorOS > 0D ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.ProvedorOS) : null;
                Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem centroCustoViagem = filtrosPesquisa.CentroDeCustoViagemCodigo > 0 ? repCentroDeCustoViagem.BuscarPorCodigo(filtrosPesquisa.CentroDeCustoViagemCodigo) : null;

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pago", filtrosPesquisa.pago));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialEmissao", filtrosPesquisa.dataInicialEmissao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalEmissao", filtrosPesquisa.dataFinalEmissao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialAutorizacao", filtrosPesquisa.dataInicialAutorizacao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalAutorizacao", filtrosPesquisa.dataFinalAutorizacao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialCancelamento", filtrosPesquisa.dataInicialCancelamento));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalCancelamento", filtrosPesquisa.dataFinalCancelamento));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialAnulacao", filtrosPesquisa.dataInicialAnulacao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalAnulacao", filtrosPesquisa.dataFinalAnulacao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialImportacao", filtrosPesquisa.dataInicialImportacao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalImportacao", filtrosPesquisa.dataFinalImportacao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialFatura", filtrosPesquisa.dataInicialFatura));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalFatura", filtrosPesquisa.dataFinalFatura));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntregaInicial", filtrosPesquisa.dataInicialEntrega));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntregaFinal", filtrosPesquisa.dataFinalEntrega));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiDataEntrega", filtrosPesquisa.possuiDataEntrega));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiNFSManual", filtrosPesquisa.possuiNFSManual));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFatura", filtrosPesquisa.situacaoFatura?.ObterDescricao()));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFaturamentoCTe", filtrosPesquisa.faturado.HasValue ? (filtrosPesquisa.faturado.Value ? "Faturado" : "Não Faturado") : null));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresas));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Vendedor", vendedores));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContratoFrete", contrato != null ? (contrato.NumeroEmbarcador + " - " + contrato.Descricao) : null));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDocumentoCreditoDebito", filtrosPesquisa.tipoDocumentoCreditoDebito != TipoDocumentoCreditoDebito.Todos ? (filtrosPesquisa.tipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Credito ? "Crédito" : "Débito") : null));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tiposOperacao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOcorrencia", tipoOcorrenciaCTe));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filiais));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", cargas));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PreCarga", filtrosPesquisa.preCarga));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", origem?.DescricaoCidadeEstado));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino?.DescricaoCidadeEstado));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", remetente?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", string.Join(",", destinatarios.Select(x => x.Descricao))));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", filtrosPesquisa.pedido));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", Servicos.Embarcador.CTe.CTe.ObterDescricaoSituacao(filtrosPesquisa.statusCTe)));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CST", filtrosPesquisa.CST.Select(o => o.ObterDescricao())));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCTe", filtrosPesquisa.tiposCTe.Select(o => o.ObterDescricao())));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumento", modelosDocumento.Select(obj => $"{obj.Descricao} ({obj.Abreviacao})")));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SegmentoVeiculo", segmentosVeiculo));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoProposta", filtrosPesquisa.TipoProposta.Select(o => o.ObterDescricao())));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBooking", filtrosPesquisa.NumeroBooking));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOS", filtrosPesquisa.NumeroOS));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroControle", filtrosPesquisa.NumeroControle));
                if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 1)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacoesCargaMercante != null ? string.Join(",", filtrosPesquisa.SituacoesCargaMercante.Select(o => o.ObterDescricao())) : null));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacaoCarga.ObterDescricao()));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoOrigem", portoOrigem?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoDestino", portoDestino?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Viagem", viagem?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Container", container?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", string.Join(",", tomadores.Select(x => x.Descricao))));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataColeta", filtrosPesquisa.DataInicialColeta, filtrosPesquisa.DataFinalColeta));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GruposPessoas", gruposPessoas));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoServicoMultimodal", filtrosPesquisa.TipoServicoMultimodal.Select(o => o.ObterDescricao())));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeioPorImportacao", filtrosPesquisa.VeioPorImportacao.ObterDescricao()));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteCTeSubstituido", filtrosPesquisa.SomenteCTeSubstituido ? "Sim" : string.Empty));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ApenasCTeEnviadoMercante", filtrosPesquisa.ApenasCTeEnviadoMercante ? "Sim" : string.Empty));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", centrosResultados));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamentoInicial", filtrosPesquisa.DataPagamentoInicial));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamentoFinal", filtrosPesquisa.DataPagamentoFinal));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoInicial", filtrosPesquisa.DataVencimentoInicial));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoFinal", filtrosPesquisa.DataVencimentoFinal));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoServico", filtrosPesquisa.tiposServicos?.Select(o => ((Dominio.Enumeradores.TipoServico)o).ObterDescricao())));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoTomador", filtrosPesquisa.tiposTomadores?.Select(o => ((Dominio.Enumeradores.TipoTomador)o).ObterDescricao())));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", estadoOrigem?.Nome));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", estadoDestino?.Nome));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirNotasFiscais", filtrosPesquisa.exibirNotasFiscais));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTeNaoExistenteFatura", filtrosPesquisa.ctesNaoExistentesEmFaturas));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTeNaoExistenteDocumento", filtrosPesquisa.ctesNaoExistentesEmMinutas));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTeVinculadoACarga", filtrosPesquisa.cteVinculadoACarga));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CargaEmissaoFinalizada", filtrosPesquisa.cargaEmissaoFinalizada));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PropriedadeVeiculo", !string.IsNullOrWhiteSpace(filtrosPesquisa.tipoPropriedadeVeiculo) ? (filtrosPesquisa.tipoPropriedadeVeiculo == "T" ? "Terceiro" : filtrosPesquisa.tipoPropriedadeVeiculo == "P" ? "Próprio" : "Outros") : null));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Serie", filtrosPesquisa.serie));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CFOP", cfop?.CodigoCFOP));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Terceiro", terceiro?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GruposPessoasDiferente", gruposPessoasDiferente));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NFe", filtrosPesquisa.nfe));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GruposPessoasRemetente", gruposPessoasRemetente));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroDocumentoRecebedor", filtrosPesquisa.NumeroDocumentoRecebedor));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ChaveCTe", filtrosPesquisa.ChaveCTe));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedidoCliente", filtrosPesquisa.NumeroPedidoCliente));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDeCarga", tiposDeCarga));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", ctes));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarroceria", filtrosPesquisa.TipoCarroceria?.ObterDescricao()));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoConfirmacaoDocumentos", filtrosPesquisa.DataConfirmacaoDocumentosInicial, filtrosPesquisa.DataConfirmacaoDocumentosFinal));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoProprietarioVeiculo", filtrosPesquisa.TipoProprietarioVeiculo?.ObterDescricao()));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoModal", filtrosPesquisa.TipoModal.ObterDescricao()));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoContratoFreteTerceiro", contratoFrete?.NumeroContrato));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioResponsavel", funcionarios));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AnuladoGerencialmente", filtrosPesquisa.AnuladoGerencialmente ? "Sim" : string.Empty));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOSConvertido", string.Join(", ", filtrosPesquisa.TipoOSConvertido.Select(tipo => tipo.ObterDescricao()))));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOS", string.Join(", ", filtrosPesquisa.TipoOS.Select(tipo => tipo.ObterDescricao()))));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ProvedorOS", provedor != null ? provedor.Nome : string.Empty));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroDeCustoViagemCodigo", centroCustoViagem?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.CentroDeCustoViagem));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CNPJDivergenteCTeMDFe", filtrosPesquisa.CNPJDivergenteCTeMDFe ? "Sim" : string.Empty));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoEmissao", filtrosPesquisa.TipoEmissao.ObterDescricao()));

                return parametros;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                throw;
            }
        }

        protected override List<KeyValuePair<string, dynamic>> ObterSubReportDataSources(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa)
        {
            try
            {
                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.NotaFiscal> listaNotasFiscaisCTes = new List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.NotaFiscal>();

                if (filtrosPesquisa.exibirNotasFiscais)
                    listaNotasFiscaisCTes = _repositorioCTe.ConsultarNotasFiscaisRelatorioCTes(filtrosPesquisa);

                return new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("CTes_NotasFiscais.rpt", listaNotasFiscaisCTes) };
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                throw;
            }
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "DescricaoStatusTitulo")
                return "StatusTitulo";

            if (propriedadeOrdenarOuAgrupar == "Codigo")
                return "NumeroCTe";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoCTe")
                return "TipoCTe";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoServico")
                return "TipoServico";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoTomador")
                return "TipoTomador";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoPagamento")
                return "TipoPagamento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}