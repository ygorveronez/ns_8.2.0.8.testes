using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.WebServiceCarrefour.Conversores.Frota
{
    class VeiculoConversor
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public VeiculoConversor(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void PreencherDadosProprietario(Dominio.Entidades.Veiculo veiculoConverter, Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Veiculo veiculo)
        {
            veiculo.TipoPropriedadeVeiculo = veiculoConverter.Tipo == "P" ? TipoPropriedadeVeiculo.Proprio : TipoPropriedadeVeiculo.Terceiros;

            if (veiculo.TipoPropriedadeVeiculo == TipoPropriedadeVeiculo.Terceiros)
                return;

            veiculo.Proprietario = new Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Proprietario();
            veiculo.Proprietario.TipoTACVeiculo = veiculoConverter.TipoProprietario;

            if (veiculoConverter.Proprietario != null)
            {
                Pessoa.EmpresaConverter servicoConverterEmpresa = new Pessoa.EmpresaConverter(_unitOfWork);

                veiculo.Proprietario.TransportadorTerceiro = servicoConverterEmpresa.Converter(veiculoConverter.Proprietario);

                if (veiculo.Proprietario.TransportadorTerceiro != null)
                {
                    veiculo.Proprietario.TransportadorTerceiro.RNTRC = veiculoConverter.RNTRC > 0 ? veiculoConverter.RNTRC.ToString() : "";
                    veiculo.Proprietario.CIOT = veiculoConverter.CIOT;
                }
            }
            else
                veiculo.TipoPropriedadeVeiculo = TipoPropriedadeVeiculo.Proprio;
        }

        private List<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista> ObterMotoristas(Dominio.Entidades.Veiculo veiculoConverter)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Carga.MotoristaConverter servicoConverterMotorista = new Carga.MotoristaConverter(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista> motoristas = new List<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista>();

            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoConverter.Codigo);
            if (veiculoMotorista != null)
                motoristas.Add(servicoConverterMotorista.Converter(veiculoMotorista));

            return motoristas;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Veiculo Converter(Dominio.Entidades.Veiculo veiculoConverter)
        {
            if (veiculoConverter == null)
                return null;

            Carga.ModeloVeicularCargaConversor servicoConverterModeloVeicularCarga = new Carga.ModeloVeicularCargaConversor();
            Pessoa.EmpresaConverter servicoConverterEmpresa = new Pessoa.EmpresaConverter(_unitOfWork);
            Pessoa.GrupoPessoaConversor servicoConverterGrupoPessoa = new Pessoa.GrupoPessoaConversor();
            ModeloConversor servicoConverterModelo = new ModeloConversor();
            Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Veiculo veiculo = new Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Veiculo();

            veiculo.Reboques = new List<Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Veiculo>();
            veiculo.AnoFabricacao = veiculoConverter.AnoFabricacao;
            veiculo.AnoModelo = veiculoConverter.AnoModelo;
            veiculo.Ativo = veiculoConverter.Ativo;
            veiculo.CapacidadeKG = veiculoConverter.CapacidadeKG;
            veiculo.CapacidadeM3 = veiculoConverter.CapacidadeM3;
            veiculo.Codigo = veiculoConverter.Codigo;
            veiculo.DataAquisicao = veiculoConverter.DataCompra.HasValue ? veiculoConverter.DataCompra.Value.ToString("dd/MM/yyyy") : "";
            veiculo.NumeroChassi = veiculoConverter.Chassi;
            veiculo.NumeroFrota = veiculoConverter.NumeroFrota;
            veiculo.NumeroMotor = veiculoConverter.NumeroMotor;
            veiculo.Placa = veiculoConverter.Placa;
            veiculo.Renavam = veiculoConverter.Renavam;
            veiculo.RNTC = veiculoConverter.RNTRC.ToString();
            veiculo.Tara = veiculoConverter.Tara;
            veiculo.TipoRodado = veiculoConverter.TipoRodado.ToEnum<TipoRodado>();
            veiculo.TipoCarroceria = !string.IsNullOrWhiteSpace(veiculoConverter.TipoCarroceria) ? veiculoConverter.TipoCarroceria.ToEnum<TipoCarroceria>() : TipoCarroceria.NaoAplicavel;
            veiculo.TipoVeiculo = veiculoConverter.TipoVeiculo == "0" ? TipoVeiculo.Tracao : TipoVeiculo.Reboque;
            veiculo.UF = veiculoConverter.Estado.Sigla;
            veiculo.GrupoPessoaSegmento = servicoConverterGrupoPessoa.Converter(veiculoConverter.GrupoPessoas);
            veiculo.Transportador = servicoConverterEmpresa.Converter(veiculoConverter.Empresa);
            veiculo.Modelo = servicoConverterModelo.Converter(veiculoConverter.Modelo);
            veiculo.ModeloVeicular = servicoConverterModeloVeicularCarga.Converter(veiculoConverter.ModeloVeicularCarga);
            veiculo.Motoristas = ObterMotoristas(veiculoConverter);

            PreencherDadosProprietario(veiculoConverter, veiculo);

            return veiculo;
        }

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Veiculo Converter(Dominio.Entidades.Veiculo veiculoConverter, List<Dominio.Entidades.Veiculo> reboquesConverter)
        {
            if (veiculoConverter == null)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Veiculo veiculo = Converter(veiculoConverter);

            foreach (Dominio.Entidades.Veiculo reboqueConverter in reboquesConverter)
            {
                Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Veiculo reboque = Converter(reboqueConverter);

                reboque.Reboques = null;
                veiculo.Reboques.Add(reboque);
            }

            return veiculo;
        }

        #endregion
    }
}
