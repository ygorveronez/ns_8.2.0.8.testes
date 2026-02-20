using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.LogRisk
{
    public class IntegracaoLogRisk
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoLogRisk(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM RetornarObjetoRequisicaoSM(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoLogRisk repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoLogRisk(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogRisk configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.Usuario) || string.IsNullOrWhiteSpace(configuracaoIntegracao.Senha) || string.IsNullOrWhiteSpace(configuracaoIntegracao.Dominio) || string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJCliente))
                throw new ServicoException("Configuração de integração com LogRisk não encontrada ou incompleta");

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);

            Dominio.Entidades.Usuario motorista = new Dominio.Entidades.Usuario();

            List<Dominio.Entidades.Usuario> motoristas = repCargaMotorista.BuscarMotoristasPorCarga(cargaIntegracao.Carga.Codigo);
            if (cargaIntegracao.Carga.Motoristas != null && cargaIntegracao.Carga.Motoristas.Count > 0)
                motorista = cargaIntegracao.Carga.Motoristas.FirstOrDefault();
            else if (motoristas != null && motoristas.Count > 0)
                motorista = motoristas.FirstOrDefault();

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaIntegracao.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido UltimoCargaPedido = repCargaPedido.BuscarUltimaEntregaCarga(cargaIntegracao.Carga.Codigo);

            decimal peso = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(cargaIntegracao.Carga.Codigo);
            decimal valorTotalNotas = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(cargaIntegracao.Carga.Codigo);
            int numeroPrimeiraNota = cargaPedido.NotasFiscais.First()?.XMLNotaFiscal?.Numero ?? 1;            
            string seriePrimeiraNota = cargaPedido.NotasFiscais.First()?.XMLNotaFiscal?.SerieOuSerieDaChave ?? "";
            int numeroSegundaNota = cargaPedido.NotasFiscais.FirstOrDefault(x => x.XMLNotaFiscal?.Numero != numeroPrimeiraNota)?.XMLNotaFiscal?.Numero ?? numeroPrimeiraNota + 1;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM();

            retorno.cliente_documento = configuracaoIntegracao.CNPJCliente ?? "";
            retorno.fornecedor_id = ((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.LogRisk).ToString();
            retorno.usuario = configuracaoIntegracao.Usuario;
            retorno.senha = configuracaoIntegracao.Senha;
            retorno.token = null;

            retorno.tagsLogin = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.TagsLogin();
            retorno.tagsLogin.dominio = configuracaoIntegracao.Dominio;
            retorno.tagsLogin.ipporta_dns = null;
            retorno.sm_numero_viagem = null;
            retorno.sm_tipo_operacao = cargaIntegracao.Carga.TipoOperacao.Codigo.ToString();
            retorno.sm_tipo_manutencao = null;

            retorno.transportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Transportador();
            retorno.transportador.codigo = !string.IsNullOrEmpty(cargaIntegracao.Carga?.Empresa?.CodigoIntegracao) ? cargaIntegracao.Carga.Empresa.CodigoIntegracao : ""; // codigo logrisk
            retorno.transportador.documento_identificador = !string.IsNullOrEmpty(cargaIntegracao.Carga.TipoOperacao?.CNPJTransportadoraBrasilRisk) ? cargaIntegracao.Carga.TipoOperacao?.CNPJTransportadoraBrasilRisk : cargaIntegracao.Carga.Empresa.CNPJ_SemFormato;
            retorno.transportador.pais_origem = cargaIntegracao.Carga.Empresa.Localidade.Pais.Codigo;

            retorno.embarcador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Embarcador();
            retorno.embarcador.codigo = "3751740"; // codigo logrisk
            retorno.embarcador.pais_origem = 484;// cargaIntegracao.Carga.Empresa.Localidade.Pais.Codigo;
            retorno.embarcador.documento_identificador = cargaIntegracao.Carga.Empresa.CNPJ_SemFormato;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Condutor condutor = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Condutor();
            condutor.documento_identificador = motorista?.CPF?.Length == 11 ? motorista?.CPF : "00000000000";
            condutor.nome = motorista?.Nome ?? "";
            condutor.numero_telefone = motorista?.Telefone ?? "";
            condutor.ddd_telefone = motorista?.Telefone ?? "";
            condutor.tipo_contato = "1";
            condutor.uf_rg = motorista?.EstadoRG != null ? motorista.EstadoRG.CodigoIBGE : 42;
            condutor.pais_origem = 484; //motorista?.Localidade?.Pais?.Codigo ?? cargaIntegracao.Carga.Empresa.Localidade.Pais.Codigo;
            retorno.condutor = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Condutor>();
            retorno.condutor.Add(condutor);

            retorno.tracionador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Tracionador();
            retorno.tracionador.identificador = cargaIntegracao.Carga.Veiculo?.Placa ?? "";
            retorno.tracionador.pais_origem = 484;// cargaIntegracao.Carga.Veiculo.LocalidadeAtual.Pais.Codigo;
            retorno.tracionador.tipo_contato = 1;

            int codigoEmpresaTransportadora = 3751739;
            int.TryParse(cargaIntegracao.Carga.Veiculo.NumeroEquipamentoRastreador, out codigoEmpresaTransportadora);

            retorno.tracionador.empresa_device_rastreamento = codigoEmpresaTransportadora;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Reboque reboque = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Reboque();
            reboque.identificador = cargaIntegracao.Carga.VeiculosVinculados.FirstOrDefault()?.Placa;
            retorno.reboque = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Reboque>();
            retorno.reboque.Add(reboque);

            retorno.origem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Origem();
            retorno.origem.uf = cargaPedido.Pedido.Remetente.Localidade.Estado.Sigla;
            retorno.origem.cidade = cargaPedido.Pedido.Remetente.Localidade.Descricao;
            retorno.origem.bairro = cargaPedido.Pedido.Remetente.Bairro;
            retorno.origem.rua = cargaPedido.Pedido.Remetente.Localidade.Descricao;
            retorno.origem.numero = cargaPedido.Pedido.Remetente.Numero;
            retorno.origem.codigo = null;
            retorno.origem.nome = cargaPedido.Pedido.Remetente.Localidade.Descricao;
            retorno.origem.pais = cargaPedido.Pedido.Remetente.Localidade.Pais.Codigo;


            retorno.destino = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Destino();
            retorno.destino.cidade = cargaPedido.Pedido.Destinatario.Localidade.Descricao;
            retorno.destino.uf = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla;
            retorno.destino.nome = cargaPedido.Pedido.Destinatario.Localidade.Descricao;
            retorno.destino.rua = cargaPedido.Pedido.Destinatario.Endereco;
            retorno.destino.numero = cargaPedido.Pedido.Destinatario.Numero;
            retorno.destino.bairro = cargaPedido.Pedido.Destinatario.Bairro;
            retorno.destino.codigo = null;

            retorno.data_previsao_inicio = cargaIntegracao.Carga.DataInicialPrevisaoCarregamento.HasValue && UltimoCargaPedido.PrevisaoEntrega.HasValue ? cargaIntegracao.Carga.DataInicialPrevisaoCarregamento.Value.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss") : cargaIntegracao.DataIntegracao.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
            retorno.data_previsao_fim = UltimoCargaPedido != null && UltimoCargaPedido.PrevisaoEntrega.HasValue ? UltimoCargaPedido.PrevisaoEntrega.Value.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss") : cargaIntegracao.DataIntegracao.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");

            retorno.ponto_coleta = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoColeta>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoColeta pontoColeta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoColeta();
            pontoColeta.sequencia = "1";
            pontoColeta.documento_identificador = cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato;
            pontoColeta.nome = cargaPedido.Pedido.Remetente.Nome;
            pontoColeta.endereco_uf = null;
            pontoColeta.endereco_codigo_cidade_ibge = 22084;

            pontoColeta.endereco_cep = cargaPedido.Pedido.Remetente.CEP;
            pontoColeta.nome_rua = cargaPedido.Pedido.Remetente.Endereco;

            int enderecoNumero = 0;
            int.TryParse(cargaPedido.Pedido.Remetente.Numero, out enderecoNumero);
            pontoColeta.numero = enderecoNumero;

            pontoColeta.previsao_chegada = cargaPedido.PrevisaoEntrega?.ToString("yyyy-MM-dd HH:mm:ss") ?? retorno.data_previsao_fim;
            pontoColeta.previsao_saida = retorno.data_previsao_inicio;
            retorno.ponto_coleta.Add(pontoColeta);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoColeta documentoColeta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoColeta();
            documentoColeta.numero_documento = cargaPedido.Codigo.ToString() + numeroPrimeiraNota.ToString();
            documentoColeta.serie_documento = null;
            documentoColeta.produto_coletado = !String.IsNullOrEmpty(cargaIntegracao.Carga?.TipoDeCarga?.ProdutoPredominante) ? cargaIntegracao.Carga?.TipoDeCarga?.ProdutoPredominante : "18642";
            documentoColeta.valor_coletado = valorTotalNotas;
            documentoColeta.peso_coletado = (double)cargaPedido.Peso;
            documentoColeta.quantidade_coletada = cargaPedido.NotasFiscais?.Count() ?? 1;
            documentoColeta.tipo_documento = 1;
            documentoColeta.volume_produto = cargaPedido.QtVolumes.ToString();
            pontoColeta.documento_coleta = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoColeta>();
            pontoColeta.documento_coleta.Add(documentoColeta);

            retorno.ponto_entrega = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega pontoEntrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega();
            pontoEntrega.sequencia = "1";
            pontoEntrega.documento_identificador = cargaPedido.Pedido.Destinatario.CPF_CNPJ_SemFormato;
            pontoEntrega.nome_cliente = cargaPedido.Pedido.Destinatario.Nome;
            pontoEntrega.remetente_descricao = cargaPedido.Pedido.Destinatario.NomeFantasia;
            pontoEntrega.tipo = 1;
            pontoEntrega.nome = cargaPedido.Pedido.Destinatario.Localidade.Descricao;
            pontoEntrega.endereco_uf = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla;
            pontoEntrega.endereco_codigo_cidade_ibge = 22084;//cargaPedido.Pedido.Destinatario.Localidade.CodigoCidade.ToInt();
            pontoEntrega.endereco_cep = Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Destinatario.CEP);
            pontoEntrega.nome_rua = cargaPedido.Pedido.Destinatario.Endereco;
            pontoEntrega.nome_bairro = cargaPedido.Pedido.Destinatario.Bairro;
            pontoEntrega.nome_complemento = cargaPedido.Pedido.Destinatario.Complemento;
            pontoEntrega.numero = cargaPedido.Pedido.Destinatario.Numero;
            pontoEntrega.pais = 484;//cargaPedido.Pedido.Destinatario.Localidade.Pais.Codigo;
            pontoEntrega.previsao_chegada = cargaPedido.Pedido.DataETA?.ToString("yyyy-MM-dd HH:mm:ss") ?? retorno.data_previsao_fim;
            pontoEntrega.previsao_saida = cargaPedido.Pedido.DataPrevisaoSaida?.ToString("yyyy-MM-dd HH:mm:ss") ?? retorno.data_previsao_inicio;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega documentoEntrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega();
            documentoEntrega.tipo_documento = 1;
            documentoEntrega.numero_documento = cargaPedido.Codigo.ToString() + numeroSegundaNota.ToString();
            documentoEntrega.serie_documento = seriePrimeiraNota;
            documentoEntrega.centro_custo = null;
            documentoEntrega.produto_entregue = documentoColeta.produto_coletado;
            documentoEntrega.valor_entregue = valorTotalNotas;
            documentoEntrega.valor_documento = valorTotalNotas;
            documentoEntrega.peso_entregue = (double)peso;
            documentoEntrega.quantidade_entregue = cargaPedido.NotasFiscais?.Count() ?? 1;
            pontoEntrega.documento_entrega = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega>();
            pontoEntrega.documento_entrega.Add(documentoEntrega);
            retorno.ponto_entrega.Add(pontoEntrega);

            return retorno;
        }

        #endregion

    }
}
