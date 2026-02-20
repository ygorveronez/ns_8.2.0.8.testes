using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class Carga : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioCarga>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;

        #endregion

        #region Construtores

        public Carga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        }


        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioCarga> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCarga.ConsultarRelatorioCarga(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCarga.ContarConsultaRelatorioCarga(filtrosPesquisa, propriedadesAgrupamento);
        }

        //public override void ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioCarga> listaRegistros, out int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        //{
        //    AtualizarPropriedadesOrdenacaoEAgrupamento(parametrosConsulta);

        //    totalRegistros = ContarRegistros(filtrosPesquisa, propriedadesAgrupamento);

        //    if (parametrosConsulta.LimiteRegistros != 0 && totalRegistros > parametrosConsulta.LimiteRegistros)
        //        throw new ServicoException("A quantia total de Registros ultrapassa o Limite Configurado, por gentileza ajuste os filtros de Data para uma janela de Data menor.");

        //    listaRegistros = (totalRegistros > 0) ? ConsultarRegistros(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioCarga>();
        //}

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/Carga";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoSeparacao repTipoSeparacao = new Repositorio.Embarcador.Cargas.TipoSeparacao(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCustoViagem repCentroDeCustoViagem = new Repositorio.Embarcador.Logistica.CentroCustoViagem(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(_unitOfWork);

            List<Dominio.Entidades.Usuario> motoristas = filtrosPesquisa.CodigosMotorista?.Count > 0 ? repUsuario.BuscarPorCodigos(filtrosPesquisa.CodigosMotorista) : new List<Dominio.Entidades.Usuario>();
            List<string> veiculos = filtrosPesquisa.CodigosVeiculos?.Count > 0 ? repVeiculo.BuscarPlacas(filtrosPesquisa.CodigosVeiculos) : null;
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculo = filtrosPesquisa.CodigosModeloVeicularCarga.Count > 0 ? repModeloVeiculo.BuscarPorCodigos(filtrosPesquisa.CodigosModeloVeicularCarga) : new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = filtrosPesquisa.CodigoTipoCarga > 0 ? repTipoCarga.BuscarPorCodigo(filtrosPesquisa.CodigoTipoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.RotaFrete rota = filtrosPesquisa.CodigoRota > 0 ? repRotaFrete.BuscarPorCodigo(filtrosPesquisa.CodigoRota) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CpfCnpjDestinatario > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario) : null;
            Dominio.Entidades.Cliente remetente = filtrosPesquisa.CpfCnpjRemetente > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjRemetente) : null;
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = filtrosPesquisa.CodigosCentroCarregamento.Count > 0 ? repCentroCarregamento.BuscarPorCodigos(filtrosPesquisa.CodigosCentroCarregamento) : new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento>();
            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupo = filtrosPesquisa.CodigoGrupoPessoas.Count() > 0 ? repGrupoPessoas.BuscarPorCodigos(filtrosPesquisa.CodigoGrupoPessoas) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            List<Dominio.Entidades.Localidade> origens = filtrosPesquisa.CodigosOrigem?.Count > 0 ? repLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosOrigem) : new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = filtrosPesquisa.CodigosDestino?.Count > 0 ? repLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosDestino) : new List<Dominio.Entidades.Localidade>();
            Dominio.Entidades.Usuario operador = filtrosPesquisa.CodigoOperador > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoOperador) : null;
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento _carregamento = filtrosPesquisa.CodigoCarregamento > 0 ? repCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCarregamento) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = filtrosPesquisa.PortoOrigem > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.PortoOrigem) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = filtrosPesquisa.PortoDestino > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.PortoDestino) : null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = filtrosPesquisa.CodigoPedidoViagemNavio > 0 ? repViagem.BuscarPorCodigo(filtrosPesquisa.CodigoPedidoViagemNavio) : null;
            Dominio.Entidades.Embarcador.Pedidos.Container container = filtrosPesquisa.Container > 0 ? repContainer.BuscarPorCodigo(filtrosPesquisa.Container) : null;
            Dominio.Entidades.Embarcador.Cargas.TipoSeparacao tipoSeparacao = filtrosPesquisa.CodigoTipoSeparacao > 0 ? repTipoSeparacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoSeparacao, false) : null;
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = filtrosPesquisa.CodigosTabelasFrete.Count > 0 ? repTabelaFrete.BuscarTabelasParaPrioridadeCalculo(filtrosPesquisa.CodigosTabelasFrete) : new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centrosResultado = filtrosPesquisa.CodigosCentroResultado.Count > 0 ? repCentroResultado.BuscarPorCodigos(filtrosPesquisa.CodigosCentroResultado) : new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>(); ;
            List<string> expedidores = filtrosPesquisa.CpfCnpjExpedidores?.Count > 0 ? repCliente.BuscarNomesPorCPFCNPJs(filtrosPesquisa.CpfCnpjExpedidores) : null;
            List<string> recebedores = filtrosPesquisa.CpfCnpjRecebedores?.Count > 0 ? repCliente.BuscarNomesPorCPFCNPJs(filtrosPesquisa.CpfCnpjRecebedores) : null;
            Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = filtrosPesquisa.CanalEntrega > 0 ? repCanalEntrega.BuscarPorCodigo(filtrosPesquisa.CanalEntrega) : null;
            Dominio.Entidades.Cliente provedor = filtrosPesquisa.CodigoProvedor > 0D ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoProvedor) : null;
            Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem centroCustoViagem = filtrosPesquisa.CentroDeCustoViagemCodigo > 0 ? repCentroDeCustoViagem.BuscarPorCodigo(filtrosPesquisa.CentroDeCustoViagemCodigo) : null;
            List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProduto = filtrosPesquisa.CodigosGrupoProduto.Count > 0 ? repGrupoProduto.BuscarPorCodigos(filtrosPesquisa.CodigosGrupoProduto) : new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>(); 

            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial, filtrosPesquisa.HabilitarHoraFiltroDataInicialFinalRelatorioCargas, Localization.Resources.Relatorios.Cargas.Carga.DataInicial));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal, filtrosPesquisa.HabilitarHoraFiltroDataInicialFinalRelatorioCargas, Localization.Resources.Relatorios.Cargas.Carga.DataFinal));
            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 2)
                parametros.Add(new Parametro("Situacao", string.Join(", ", filtrosPesquisa.SituacoesCargaMercante.Select(o => o.ObterDescricao())), "", Localization.Resources.Gerais.Geral.Situacao));
            else
                parametros.Add(new Parametro("Situacao", string.Join(", ", filtrosPesquisa.Situacoes.Select(o => o.ObterDescricao())), "", Localization.Resources.Gerais.Geral.Situacao));
            parametros.Add(new Parametro("Transportador", empresa != null ? empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial : null, Localization.Resources.Relatorios.Cargas.Carga.Transportador));
            parametros.Add(new Parametro("SomenteTerceiros", filtrosPesquisa.SomenteTerceiros, Localization.Resources.Relatorios.Cargas.Carga.CargasDeTerceiros));
            parametros.Add(new Parametro("Transbordo", filtrosPesquisa.Transbordo, Localization.Resources.Relatorios.Cargas.Carga.Transbordo));
            parametros.Add(new Parametro("Filial", filial?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.Filial));
            parametros.Add(new Parametro("CentroCarregamento", string.Join(", ", centrosCarregamento.Select(o => o.Descricao)), Localization.Resources.Relatorios.Cargas.Carga.CentroDeCarregamento));
            parametros.Add(new Parametro("Remetente", remetente?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.Remetente));
            parametros.Add(new Parametro("Destinatario", destinatario?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.Destinatario));
            parametros.Add(new Parametro("GrupoPessoas", string.Join(", ", grupo.Select(o => o.Descricao)), Localization.Resources.Relatorios.Cargas.Carga.GrupoDePessoas));
            parametros.Add(new Parametro("Origem", origens.Select(o => o.DescricaoCidadeEstado).ToList(), Localization.Resources.Relatorios.Cargas.Carga.Origem));
            parametros.Add(new Parametro("Destino", destinos.Select(o => o.DescricaoCidadeEstado).ToList(), Localization.Resources.Relatorios.Cargas.Carga.Destino));
            parametros.Add(new Parametro("Rota", rota?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.Rota));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.TipoDeOperacao));
            parametros.Add(new Parametro("TipoCarga", tipoCarga?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.TipoDeCarga));
            parametros.Add(new Parametro("ModeloVeiculo", string.Join(", ", modelosVeiculo.Select(o => o.Descricao)), Localization.Resources.Relatorios.Cargas.Carga.ModeloDoVeiculo));
            parametros.Add(new Parametro("TipoLocalPrestacao", filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.interMunicipal ? Localization.Resources.Relatorios.Cargas.Carga.Intermunicipal : filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.intraMunicipal ? Localization.Resources.Relatorios.Cargas.Carga.Municipal : null, Localization.Resources.Relatorios.Cargas.Carga.TipoDaPrestacao));
            parametros.Add(new Parametro("Veiculo", veiculos, Localization.Resources.Relatorios.Cargas.Carga.Veiculo));
            parametros.Add(new Parametro("Motorista", motoristas.Select(o => o.Descricao).ToList(), Localization.Resources.Relatorios.Cargas.Carga.Motorista));
            parametros.Add(new Parametro("Carregamento", _carregamento?.NumeroCarregamento, Localization.Resources.Relatorios.Cargas.Carga.Carregamento));
            parametros.Add(new Parametro("Operador", operador?.Nome, Localization.Resources.Relatorios.Cargas.Carga.Operador));
            parametros.Add(new Parametro("Pedido", filtrosPesquisa.Pedido, Localization.Resources.Relatorios.Cargas.Carga.Pedido));
            parametros.Add(new Parametro("PreCarga", filtrosPesquisa.PreCarga, Localization.Resources.Relatorios.Cargas.Carga.PreCarga));
            parametros.Add(new Parametro("CodigoCargaEmbarcador", filtrosPesquisa.CodigoCargaEmbarcador, Localization.Resources.Relatorios.Cargas.Carga.NumeroCarga));
            parametros.Add(new Parametro("DataAnulacaoInicial", filtrosPesquisa.DataAnulacaoInicial, false, Localization.Resources.Relatorios.Cargas.Carga.DataAnulacaoInicial));
            parametros.Add(new Parametro("DataAnulacaoFinal", filtrosPesquisa.DataAnulacaoFinal, false, Localization.Resources.Relatorios.Cargas.Carga.DataAnulacaoFinal));
            parametros.Add(new Parametro("DataInicialInicioEmissaoDocumentos", filtrosPesquisa.DataInicialInicioEmissaoDocumentos, false, Localization.Resources.Relatorios.Cargas.Carga.DataInicialInicioEmissaoDocumentos));
            parametros.Add(new Parametro("DataFinalInicioEmissaoDocumentos", filtrosPesquisa.DataFinalInicioEmissaoDocumentos, false, Localization.Resources.Relatorios.Cargas.Carga.DataFinalInicioEmissaoDocumentos));
            parametros.Add(new Parametro("DataInicialFimEmissaoDocumentos", filtrosPesquisa.DataInicialFimEmissaoDocumentos, false, Localization.Resources.Relatorios.Cargas.Carga.DataInicialFimEmissaoDocumentos));
            parametros.Add(new Parametro("DataFinalFimEmissaoDocumentos", filtrosPesquisa.DataFinalFimEmissaoDocumentos, false, Localization.Resources.Relatorios.Cargas.Carga.DataFinalFimEmissaoDocumentos));
            parametros.Add(new Parametro("Mdfe", filtrosPesquisa.NumeroMDFe, Localization.Resources.Relatorios.Cargas.Carga.MDFe));
            parametros.Add(new Parametro("DeliveryTerm", filtrosPesquisa.DeliveryTerm, Localization.Resources.Relatorios.Cargas.Carga.DeliveryTerm));
            parametros.Add(new Parametro("IdAutorizacao", filtrosPesquisa.IdAutorizacao, Localization.Resources.Relatorios.Cargas.Carga.IDAutorizacao));
            parametros.Add(new Parametro("DataInclusaoBookingInicial", filtrosPesquisa.DataInclusaoBookingInicial, false, Localization.Resources.Relatorios.Cargas.Carga.DataInicialInclusaoBooking));
            parametros.Add(new Parametro("DataInclusaoBookingLimite", filtrosPesquisa.DataInclusaoBookingLimite, false, Localization.Resources.Relatorios.Cargas.Carga.DataFinalInclusaoBooking));
            parametros.Add(new Parametro("DataInclusaoPCPInicial", filtrosPesquisa.DataInclusaoPCPInicial, false, Localization.Resources.Relatorios.Cargas.Carga.DataInicialInclusaoPCP));
            parametros.Add(new Parametro("DataInclusaoPCPLimite", filtrosPesquisa.DataInclusaoPCPLimite, false, Localization.Resources.Relatorios.Cargas.Carga.DataFinalInclusaoPCP));
            parametros.Add(new Parametro("NumeroBooking", filtrosPesquisa.NumeroBooking, Localization.Resources.Relatorios.Cargas.Carga.NumeroBooking));
            parametros.Add(new Parametro("NumeroOS", filtrosPesquisa.NumeroOS, Localization.Resources.Relatorios.Cargas.Carga.NumeroOrdemServico));
            parametros.Add(new Parametro("NumeroNota", filtrosPesquisa.NumeroNF, Localization.Resources.Relatorios.Cargas.Carga.NumeroNotaFiscal));
            parametros.Add(new Parametro("NumeroControle", filtrosPesquisa.NumeroControle, Localization.Resources.Relatorios.Cargas.Carga.NumeroControle));
            parametros.Add(new Parametro("SituacaoCTe", Servicos.Embarcador.CTe.CTe.ObterDescricaoSituacao(filtrosPesquisa.SituacaoCTe), Localization.Resources.Relatorios.Cargas.Carga.SituacaoCTe));
            parametros.Add(new Parametro("PortoOrigem", portoOrigem?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.PortoOrigem));
            parametros.Add(new Parametro("PortoDestino", portoDestino?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.PortoDestino));
            parametros.Add(new Parametro("Viagem", viagem?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.Viagem));
            parametros.Add(new Parametro("Container", container?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.Container));
            parametros.Add(new Parametro("TipoSeparacao", tipoSeparacao?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.TipoSeparacao));
            parametros.Add(new Parametro("TabelaFrete", string.Join(", ", tabelasFrete.Select(o => o.Descricao)), Localization.Resources.Relatorios.Cargas.Carga.TabelaFrete));
            parametros.Add(new Parametro("TipoCTe", string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ObterDescricao())), Localization.Resources.Relatorios.Cargas.Carga.TipoDoCTe));
            parametros.Add(new Parametro("TipoPropostaMultimodal", string.Join(", ", filtrosPesquisa.TipoPropostaMultimodal.Select(o => o.ObterDescricao())), Localization.Resources.Relatorios.Cargas.Carga.TipoProposta));
            parametros.Add(new Parametro("TipoServicoMultimodal", string.Join(", ", filtrosPesquisa.TipoServicoMultimodal.Select(o => o.ObterDescricao())), Localization.Resources.Relatorios.Cargas.Carga.TipoServico));
            parametros.Add(new Parametro("VeioPorImportacao", string.Join(", ", filtrosPesquisa.VeioPorImportacao.ObterDescricao()), Localization.Resources.Relatorios.Cargas.Carga.VeioPorImportacao));
            parametros.Add(new Parametro("SomenteCTeSubstituido", string.Join(", ", filtrosPesquisa.SomenteCTeSubstituido ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao), Localization.Resources.Relatorios.Cargas.Carga.SomenteCTeSubstituido));
            parametros.Add(new Parametro("CentroResultado", string.Join(", ", centrosResultado?.Select(o => o.Descricao)), Localization.Resources.Relatorios.Cargas.Carga.CentroResultado));
            parametros.Add(new Parametro("InformacoesCargaPreCarga", filtrosPesquisa.InformacoesRelatorioCargas.ObterDescricao(), Localization.Resources.Relatorios.Cargas.Carga.TipoDeInformacao));
            parametros.Add(new Parametro("Problemas", filtrosPesquisa.Problemas.ObterDescricao(), Localization.Resources.Relatorios.Cargas.Carga.Problemas));
            parametros.Add(new Parametro("PeriodoEncerramento", filtrosPesquisa.DataEncerramentoInicial, filtrosPesquisa.DataEncerramentoFinal, false, Localization.Resources.Relatorios.Cargas.Carga.PeriodoEncerramento));
            parametros.Add(new Parametro("PeriodoConfirmacaoDocumentos", filtrosPesquisa.DataConfirmacaoDocumentosInicial, filtrosPesquisa.DataConfirmacaoDocumentosFinal, false, Localization.Resources.Relatorios.Cargas.Carga.PeriodoConfirmacaoDocumentos));
            parametros.Add(new Parametro("Expedidor", expedidores, Localization.Resources.Relatorios.Cargas.Carga.Expedidor));
            parametros.Add(new Parametro("Recebedor", recebedores, Localization.Resources.Relatorios.Cargas.Carga.Recebedor));
            parametros.Add(new Parametro("CargaTrechos", filtrosPesquisa.CargaTrechos.ObterDescricao(), Localization.Resources.Relatorios.Cargas.Carga.CargaTrechos));
            parametros.Add(new Parametro("CanalEntrega", canalEntrega?.Descricao, "Canal Entrega"));
            parametros.Add(new Parametro("CargaTrechoSumarizada", filtrosPesquisa.CargaTrechoSumarizada?.ObterDescricao(), Localization.Resources.Relatorios.Cargas.Carga.CargaDeTrecho));
            parametros.Add(new Parametro("DataFaturamentoInicial", filtrosPesquisa.DataFaturamentoInicial, false, Localization.Resources.Relatorios.Cargas.Carga.DataFaturamentoInicial));
            parametros.Add(new Parametro("DataFaturamentoFinal", filtrosPesquisa.DataFaturamentoFinal, false, Localization.Resources.Relatorios.Cargas.Carga.DataFaturamentoFinal));
            parametros.Add(new Parametro("TipoOSConvertido", filtrosPesquisa.TipoOSConvertido.Count > 0 ? string.Join(", ", filtrosPesquisa.TipoOSConvertido.Select(o => o.ToString("d"))) : string.Empty));
            parametros.Add(new Parametro("TipoOS", filtrosPesquisa.TipoOS.Count > 0 ? string.Join(", ", filtrosPesquisa.TipoOS.Select(o => o.ToString("d"))) : string.Empty));
            parametros.Add(new Parametro("DirecionamentoCustoExtra", filtrosPesquisa.DirecionamentoCustoExtra.Count > 0 ? string.Join(", ", filtrosPesquisa.DirecionamentoCustoExtra.Select(o => o.ToString("d"))) : string.Empty));
            parametros.Add(new Parametro("StatusCustoExtra", filtrosPesquisa.StatusCustoExtra.Count > 0 ? string.Join(", ", filtrosPesquisa.StatusCustoExtra.Select(o => o.ToString("d"))) : string.Empty));
            parametros.Add(new Parametro("CodigosProvedores", filtrosPesquisa.CodigosProvedores.Count > 0 ? string.Join(", ", filtrosPesquisa.CodigosProvedores.Select(o => o.ToString())) : string.Empty));
            parametros.Add(new Parametro("CentroDeCustoViagemCodigo", centroCustoViagem?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.CentroDeCustoViagem));
            parametros.Add(new Parametro("GrupoProduto", string.Join(", ", gruposProduto?.Select(o => o.Descricao)), Localization.Resources.Relatorios.Cargas.Carga.GrupoProduto));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacao")
                return "SituacaoCarga";

            if (propriedadeOrdenarOuAgrupar == "DescricaoDataCarregamento")
                return "DataCarga";

            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}