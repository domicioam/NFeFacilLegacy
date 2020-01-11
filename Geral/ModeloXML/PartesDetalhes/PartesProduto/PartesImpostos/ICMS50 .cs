﻿namespace BaseGeral.ModeloXML.PartesDetalhes.PartesProduto.PartesImpostos
{
    /// <summary>
    /// Grupo Tributação ICMS = 40, 41, 50.
    /// </summary>
    public class ICMS50 : ComumICMS, IRegimeNormal
    {
        public ICMS50()
        {
        }

        public ICMS50(int origem, string cst, string vICMSDeson, string motDesICMS) : base(origem, cst, false)
        {
            this.vICMSDeson = vICMSDeson;
            this.motDesICMS = motDesICMS;
        }
    }
}
