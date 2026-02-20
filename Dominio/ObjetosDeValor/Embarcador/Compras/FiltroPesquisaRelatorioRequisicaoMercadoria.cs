using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Compras
{
    public class FiltroPesquisaRelatorioRequisicaoMercadoria
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int Produto { get; set; }
        public int GrupoProduto { get; set; }
        public int Colaborador { get; set; }
        public int FuncionarioRequisitado { get; set; }
        public int Motivo { get; set; }
        public List<SituacaoRequisicaoMercadoria> Situacao { get; set; }
        public ModoRequisicaoMercadoria Tipo { get; set; }
        public int CodigoEmpresa { get; set; }
        public int SetorFuncionario { get; set; }
    }
}
