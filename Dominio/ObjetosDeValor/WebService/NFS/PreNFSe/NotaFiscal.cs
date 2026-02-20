using System;

namespace Dominio.ObjetosDeValor.WebService.NFS.PreNFSe
{
    public class NotaFiscal
    {
        #region Propriedades

        public string Chave { get; set; }
        
        public DateTime DataEmissao { get; set; }

        public decimal Peso { get; set; }

        public decimal PesoLiquido { get; set; }

        public int Volumes { get; set; }

        public decimal Valor { get; set; }

        #endregion Propriedades
    }
}
