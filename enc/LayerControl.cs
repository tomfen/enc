using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks.Layers;

namespace enc
{
    public partial class LayerControl : UserControl
    {
        public LayerControl(bool biasEnabled, int? neurons)
        {
            InitializeComponent();

            checkBox1.Enabled = biasEnabled;
            if(neurons != null)
            {
                numericUpDown1.Value = (decimal)neurons;
                numericUpDown1.Enabled = false;
            }
        }

        private void LayerControl_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(IActivationFunction));

            var functions = from t in assembly.GetTypes()
                   where t.GetInterfaces().Contains(typeof(IActivationFunction))
                   select t.Name;

            comboBox1.Items.AddRange(functions.ToArray());

            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        internal ILayer CreateLayer()
        {
            IActivationFunction activation = 
                (IActivationFunction) Assembly.GetExecutingAssembly().CreateInstance((string)comboBox1.SelectedItem);
            return new BasicLayer(activation, checkBox1.Checked, (int)numericUpDown1.Value);
        }
    }
}
