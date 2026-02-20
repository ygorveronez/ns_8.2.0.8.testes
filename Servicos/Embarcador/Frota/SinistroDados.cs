using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Frota;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Frota
{
    public sealed class SinistroDados
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion

        #region Construtores

        public SinistroDados(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        public SinistroDados(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SinistroDados() { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos SalvarEnvolvido(Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro, SinistroEnvolvidoDTO sinistroEnvolvidoDTO)
        {
            Repositorio.Embarcador.Frota.SinistroDocumentacaoEnvolvidos repositorioSinistroEnvolvido = new Repositorio.Embarcador.Frota.SinistroDocumentacaoEnvolvidos(_unitOfWork);

            Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos envolvido = sinistroEnvolvidoDTO.Codigo > 0 ? repositorioSinistroEnvolvido.BuscarPorCodigo(sinistroEnvolvidoDTO.Codigo, true) : new Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos();

            envolvido.SinistroDados = dadosSinistro;
            envolvido.CNH = sinistroEnvolvidoDTO.CNH;
            envolvido.CPF = sinistroEnvolvidoDTO.CPF;
            envolvido.RG = sinistroEnvolvidoDTO.RG;
            envolvido.TipoEnvolvido = sinistroEnvolvidoDTO.Tipo;
            envolvido.Nome = sinistroEnvolvidoDTO.Nome;
            envolvido.Observacao = sinistroEnvolvidoDTO.Observacao;
            envolvido.TelefonePrincipal = sinistroEnvolvidoDTO.TelefonePrincipal;
            envolvido.TelefoneSecundario = sinistroEnvolvidoDTO.TelefoneSecundario;
            envolvido.Veiculo = sinistroEnvolvidoDTO.Veiculo;

            if (envolvido.Codigo > 0)
                repositorioSinistroEnvolvido.Atualizar(envolvido);
            else
                repositorioSinistroEnvolvido.Inserir(envolvido);

            return envolvido;
        }

        public int IniciarFluxo(SinistroDadosDTO dadosSinistroDTO)
        {
            Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(_unitOfWork);

            Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro = new Dominio.Entidades.Embarcador.Frota.SinistroDados()
            {
                DataLancamentoFluxo = DateTime.Now,
                Situacao = SituacaoEtapaFluxoSinistro.Aberto,
                Numero = dadosSinistroDTO.Numero
            };

            PreencherFluxoSinistro(dadosSinistro, dadosSinistroDTO);

            repositorioSinistro.Inserir(dadosSinistro);

            Auditoria.Auditoria.Auditar(_auditado, dadosSinistro, "Iniciou o fluxo de sinistro", _unitOfWork);

            return dadosSinistro.Codigo;
        }

        public void AtualizarFluxo(SinistroDadosDTO dadosSinistroDTO)
        {
            Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(_unitOfWork);

            Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro = repositorioSinistro.BuscarPorCodigo(dadosSinistroDTO.Codigo, true);

            if (dadosSinistro.Etapa != EtapaSinistro.Dados)
                throw new ServicoException("Fluxo não está mais na etapa inicial.");

            PreencherFluxoSinistro(dadosSinistro, dadosSinistroDTO);

            repositorioSinistro.Atualizar(dadosSinistro);

            List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = dadosSinistro.GetChanges();

            Auditoria.Auditoria.Auditar(_auditado, dadosSinistro, alteracoes, "Atualizou o fluxo de sinistro", _unitOfWork);
        }

        public void AvancarEtapa(Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro, EtapaSinistro etapa)
        {
            if ((int)dadosSinistro.Etapa != ((int)etapa) - 1)
                throw new ServicoException("Não é possível avançar a etapa do fluxo de sinistro.");

            if (dadosSinistro.Etapa == etapa)
                throw new ServicoException("O fluxo de sinistro já está na etapa que está tentando avançar!");

            Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(_unitOfWork);

            dadosSinistro.Etapa = etapa;
            repositorioSinistro.Atualizar(dadosSinistro);

            Auditoria.Auditoria.Auditar(_auditado, dadosSinistro, "Avançou o fluxo de sinistro para a etapa " + etapa.ObterDescricao(), _unitOfWork);
        }

        public void VoltarEtapa(Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro, EtapaSinistro etapa)
        {
            if (dadosSinistro.Etapa == EtapaSinistro.Dados)
                throw new ServicoException("Não é possível voltar a etapa do fluxo de sinistro.");

            if (dadosSinistro.Etapa == etapa)
                throw new ServicoException("O fluxo de sinistro já está na etapa que está tentando voltar!");

            Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(_unitOfWork);

            dadosSinistro.Etapa = etapa;
            repositorioSinistro.Atualizar(dadosSinistro);

            Auditoria.Auditoria.Auditar(_auditado, dadosSinistro, "Voltou o fluxo de sinistro para a etapa " + etapa.ObterDescricao(), _unitOfWork);
        }

        public void SalvarNumeroBoletimOcorrencia(string numeroBoletimOcorrencia, Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro)
        {
            if (dadosSinistro == null)
                throw new ServicoException("O registro não existe");

            Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(_unitOfWork);
            dadosSinistro.NumeroBoletimOcorrencia = numeroBoletimOcorrencia;

            repositorioSinistro.Atualizar(dadosSinistro);

            Auditoria.Auditoria.Auditar(_auditado, dadosSinistro, dadosSinistro.GetChanges(), "Atualizou o número do boletim de ocorrência", _unitOfWork);
        }

        public object ObterDetalhesServico(Dominio.Entidades.Embarcador.Frota.SinistroServico servico)
        {
            return new
            {
                servico.Codigo,
                Servico = new
                {
                    servico.Servico.Codigo,
                    servico.Servico.Descricao
                },
                servico.CustoEstimado,
                servico.CustoMedio,
                servico.Observacao,
                servico.TipoManutencao,
                DescricaoTipoManutencao = servico.TipoManutencao.ObterDescricao(),
                DataUltimaManutencao = servico.UltimaManutencao?.OrdemServico.DataProgramada.ToString("dd/MM/yyyy") ?? string.Empty,
                QuilometragemUltimaManutencao = servico.UltimaManutencao?.OrdemServico.QuilometragemVeiculo,
                servico.TempoEstimado
            };
        }

        #endregion

        #region Métodos Privados

        private void PreencherFluxoSinistro(Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro, SinistroDadosDTO dadosSinistroDTO)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Frota.TipoSinistro repositorioSinistro = new Repositorio.Embarcador.Frota.TipoSinistro(_unitOfWork);
            Repositorio.Embarcador.Frota.GravidadeSinistro repositorioGravidadeSinistro = new Repositorio.Embarcador.Frota.GravidadeSinistro(_unitOfWork);

            dadosSinistro.CausadorSinistro = dadosSinistroDTO.CausadorSinistro;
            dadosSinistro.Cidade = repositorioLocalidade.BuscarPorCodigo(dadosSinistroDTO.CodigoCidade);
            dadosSinistro.DataEmissao = dadosSinistroDTO.DataEmissao;
            dadosSinistro.DataSinistro = dadosSinistroDTO.DataSinistro;
            dadosSinistro.Endereco = dadosSinistroDTO.Endereco;
            dadosSinistro.Local = dadosSinistroDTO.Local;
            dadosSinistro.Motorista = repositorioUsuario.BuscarPorCodigo(dadosSinistroDTO.CodigoMotorista);
            dadosSinistro.NumeroBoletimOcorrencia = dadosSinistroDTO.NumeroBoletimOcorrencia;
            dadosSinistro.Observacao = dadosSinistroDTO.Observacao;
            dadosSinistro.Veiculo = repositorioVeiculo.BuscarPorCodigo(dadosSinistroDTO.CodigoVeiculo);
            dadosSinistro.VeiculoReboque = repositorioVeiculo.BuscarPorCodigo(dadosSinistroDTO.CodigoVeiculoReboque);
            dadosSinistro.Etapa = EtapaSinistro.Documentacao;
            dadosSinistro.TipoSinistro = repositorioSinistro.BuscarPorCodigo(dadosSinistroDTO.CodigoTipoSinistro);
            dadosSinistro.GravidadeSinistro = repositorioGravidadeSinistro.BuscarPorCodigo(dadosSinistroDTO.CodigoGravidadeSinistro);
        }

        #endregion
    }
}
