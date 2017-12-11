﻿using Windows.UI.Xaml.Controls;

// O modelo de item de Caixa de Diálogo de Conteúdo está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe.Impostos
{
    public sealed partial class DefinirTotalImpostos : ContentDialog
    {
        public string ValorTotalTributos { get; private set; }

        public DefinirTotalImpostos()
        {
            this.InitializeComponent();
        }
    }
}
