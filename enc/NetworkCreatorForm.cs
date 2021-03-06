﻿using Encog.Neural.Networks;
using System;
using System.Windows.Forms;

namespace enc
{
    public partial class NetworkCreatorForm : Form
    {
        public BasicNetwork network { get; set; }

        public NetworkCreatorForm(int inputs, int outputs)
        {
            InitializeComponent();

            addLayerPanel(LayerControl.LayerType.Input, inputs);
            addLayerPanel(LayerControl.LayerType.Output, outputs);
        }

        private void addLayerPanel(LayerControl.LayerType type, int? neurons)
        {
            var panel = new LayerControl(type, neurons)
            {
                Dock = DockStyle.Top,
            };
            flowPanel.Controls.Add(panel);
            flowPanel.Controls.SetChildIndex(panel, 1);
        }

        private void removeLayerPanel()
        {
            if(flowPanel.Controls.Count > 2)
            {
                flowPanel.Controls.RemoveAt(1);
            }
        }

        private void CreateNetwork()
        {
            network = new BasicNetwork();

            foreach(LayerControl layerControl in flowPanel.Controls)
            {
                network.AddLayer(layerControl.CreateLayer());
            }

            network.Structure.FinalizeStructure();
            network.Reset();
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            addLayerPanel(LayerControl.LayerType.Hidden, null);
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            removeLayerPanel();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CreateNetwork();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
